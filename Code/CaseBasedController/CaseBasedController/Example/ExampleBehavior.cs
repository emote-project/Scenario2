using CaseBasedController.Behavior;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Example
{
    internal abstract class ExampleBehavior : IBehavior
    {
        public double ExecutionDuration { get; set; }

        #region IBehavior Members

        public void Dispose()
        {
        }

        public int Priority { get; set; }

        public event ExecutedEventHandler ExecutionFinished;

        public abstract void Execute(IFeatureDetector detector);

        public virtual void Cancel()
        {
        }

        public void Init(IAllActionPublisher actionPublisher, IAllPerceptionClient perceptionClient)
        {
        }

        #endregion

        protected void SendExecutionFinished(IFeatureDetector detector = null)
        {
            if (this.ExecutionFinished != null) this.ExecutionFinished(this, null);
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}