using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Example
{
    internal abstract class ExampleFeatureDetector : IFeatureDetector
    {
        private bool _wasActive;

        #region IFeatureDetector Members

        public void Dispose()
        {
        }

        public abstract double ActivationLevel { get; }
        public abstract bool IsActive { get; }
        public event ActivatedEventHandler ActivationChanged;

        public void Init(IAllPerceptionClient perceptionClient)
        {
        }

        public void CheckActivationChanged(bool force)
        {
            if (this.ActivationChanged == null) return;

            var isActive = this.IsActive;
            if (!(isActive ^ this._wasActive)) return;

            this.ActivationChanged(this, isActive);
            this._wasActive = isActive;
        }

        #endregion

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public string Description
        {
            get;
            set;
        }
    }
}