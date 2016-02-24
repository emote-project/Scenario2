namespace CaseBasedController.Detection
{
    /// <summary>
    ///     A feature detector that has a certain level of activation according to the perception of some feature.
    /// </summary>
    public abstract class ThresholdFeatureDetector : BaseFeatureDetector
    {
        private const double DEFAULT_ACTIVATION_THRESHOLD = 0.5;

        protected ThresholdFeatureDetector()
        {
            this.ActivationThreshold = DEFAULT_ACTIVATION_THRESHOLD;
        }

        /// <summary>
        ///     The threshold level above which a <see cref="ThresholdFeatureDetector" /> is considered as being "active".
        /// </summary>
        public double ActivationThreshold { get; set; }

        public override bool IsActive
        {
            get { return this.ActivationLevel >= this.ActivationThreshold; }
        }

        public override string ToString()
        {
            return string.Format("{0}, Threshold: {1:0.##}", base.ToString(), this.ActivationThreshold);
        }
    }
}