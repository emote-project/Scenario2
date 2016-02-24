using System;
using CaseBasedController.Behavior;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController
{
    /// <summary>
    ///     A <see cref="Case" /> is a typical situation in which one employs a certain response.
    ///     The situation is a <see cref="IFeatureDetector" /> dictating when the <see cref="Case" /> should be activated.
    ///     The response is a <see cref="IBehavior" /> that should trigger a set of actions when the situation is perceived.
    /// </summary>
    public class Case : IDisposable, IComparable<Case>
    {
        public Case(IFeatureDetector detector, IBehavior behavior, bool isCancellable = true)
        {
            this.Behavior = behavior;
            this.Detector = detector;
            IsCancellable = isCancellable;
            Enabled = true;
            if (detector == null)
            {
                //console.writeline("Can't find detector for finished behaviour: "+behavior);
                return;
            }
        }

        #region Properties

        /// <summary>
        ///     A description for this <see cref="Case" />.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The response that this case triggers when the situation is detcted.
        /// </summary>
        public IBehavior Behavior { get; private set; }

        /// <summary>
        ///     The situation detection mehcanism associated with the case.
        /// </summary>
        public IFeatureDetector Detector { get; private set; }

        /// <summary>
        ///     The associated behaviour has been started
        /// </summary>
        public bool ExecutionStarted { get; private set; }

        /// <summary>
        ///     Define whether the Case execution is cancellable in case a case with highter priority needs to be executed
        /// </summary>
        public bool IsCancellable { get; private set; }

        /// <summary>
        ///     Define if this case should be considered or not (useful to disable cases at runtime for testing purposes)
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set
            {
                _enabled = value;
                //console.writeline(this.ToString()+" set to "+value);
            }
        }
        private bool _enabled = true;

        #endregion

        #region IComparable<Case> Members

        public int CompareTo(Case other)
        {
            //comparison based on behavior's pre-assigned priority, invert to enable descending-order sorting
            return this.Behavior.Priority.CompareTo(other.Behavior.Priority);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Detector.Dispose();
        }

        #endregion
        public void Execute()
        {
            this.Behavior.Execute(this.Detector);
            ExecutionStarted = true;
        }

        public void Init(IAllPerceptionClient perceptionClient, IAllActionPublisher publisher)
        {
            this.Detector.Init(perceptionClient);
            this.Behavior.Init(publisher, perceptionClient);
        }

        public override string ToString()
        {
            return (this.Description!=null)?this.Description:base.ToString();
        }
    }
}