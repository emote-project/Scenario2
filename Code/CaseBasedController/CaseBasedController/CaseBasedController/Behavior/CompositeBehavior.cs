using System.Collections.Generic;
using System.Threading.Tasks;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Behavior
{
    /// <summary>
    ///     A behavior is a set of actions triggered in response of a detected <see cref="Case" />.
    /// </summary>
    public abstract class CompositeBehavior : List<IBehavior>, IBehavior
    {
        private readonly object _locker = new object();
        private IFeatureDetector _detector;
        private uint _numExecuted;

        /// <summary>
        ///     Determines how the sub-behaviors are executed: if true, executes the set of <see cref="IBehavior" /> in parallel,
        ///     otherwise in sequence.
        /// </summary>
        public bool InParallel { get; set; }

        #region IBehavior Members

        public int Priority { get; set; }


        public void Dispose()
        {
            this.ForEach(behavior => behavior.Dispose());
        }

        public event ExecutedEventHandler ExecutionFinished;

        public void Execute(IFeatureDetector detector)
        {
            this._detector = detector;
            this._numExecuted = 0;

            //executes all sub-behaviors according to order policy
            if (this.InParallel)
                this.ForEach(action => Task.Factory.StartNew(() => action.Execute(detector)));
            else
                this.ForEach(action => action.Execute(detector));
        }

        public void Cancel()
        {
            lock (this._locker)
            {
                //cancels all sub-behaviors
                this._detector = null;
                this.ForEach(behavior => behavior.Cancel());
            }
        }

        public void Init(IAllActionPublisher actionPublisher, IAllPerceptionClient perceptionClient)
        {
            this.ForEach(behavior => behavior.Init(actionPublisher, perceptionClient));
        }

        #endregion

        public new void Add(IBehavior behavior)
        {
            //attaches sub-behavior event 
            behavior.ExecutionFinished += this.OnSubBehaviorExecutionFinished;
            base.Add(behavior);
        }

        public new void Remove(IBehavior behavior)
        {
            //detaches from sub-behavior event 
            behavior.ExecutionFinished -= this.OnSubBehaviorExecutionFinished;
            base.Remove(behavior);
        }

        private void OnSubBehaviorExecutionFinished(IBehavior behavior, IFeatureDetector detector)
        {
            lock (this._locker)
            {
                //verfies if all sub-behaviors have been executed
                if (++this._numExecuted < this.Count) return;

                //raises finished event
                if (this.ExecutionFinished != null) this.ExecutionFinished(this, this._detector);
                this._detector = null;
            }
        }
    }
}