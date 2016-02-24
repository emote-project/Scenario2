namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Negates the activation state of the <see cref="WatcherFeatureDetector.WatchedDetector" />.
    /// </summary>
    public class NotFeatureDetector : WatcherFeatureDetector
    {
        public override double ActivationLevel
        {
            get { lock (this.locker) return 1d - this.WatchedDetector.ActivationLevel; }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return !this.WatchedDetector.IsActive; }
        }

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker) this.CheckActivationChanged();
        }

        public override string ToString()
        {
            return Description != "" && Description != null ? base.ToString():"Not: " + WatchedDetector.ToString();
        }
    }
}