using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     A detector for activation-related events of another (watched) sub-detector.
    /// </summary>
    public abstract class WatcherFeatureDetector : BaseFeatureDetector
    {
        /// <summary>
        ///     The sub-detector that this detector is watching for (de)activation events.
        /// </summary>
        public IFeatureDetector WatchedDetector { get; set; }

        public override void Init(IAllPerceptionClient client)
        {
            this.WatchedDetector.Init(client);
            base.Init(client);
        }

        public override void Dispose()
        {
            lock (this.locker)
                if (this.WatchedDetector != null)
                    this.WatchedDetector.ActivationChanged -= this.OnDetectorActivationChanged;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.WatchedDetector != null)
                    this.WatchedDetector.ActivationChanged += this.OnDetectorActivationChanged;
        }

        public override void CheckActivationChanged(bool force = false)
        {
            if (force) this.WatchedDetector.CheckActivationChanged();
            base.CheckActivationChanged(force);
        }

        protected abstract void OnDetectorActivationChanged(IFeatureDetector detector, bool activated);

        public override string ToString()
        {
            return Description != "" && Description != null ? base.ToString() : "Watching: " + WatchedDetector.ToString();
        }
    }
}