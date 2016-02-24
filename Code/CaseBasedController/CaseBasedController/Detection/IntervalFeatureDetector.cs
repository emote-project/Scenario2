using Newtonsoft.Json;

namespace CaseBasedController.Detection
{
    /// <summary>
    ///     Detects variables which values are between a certain specified range.
    /// </summary>
    public class IntervalFeatureDetector : BaseFeatureDetector
    {
        private const double MIN_ACTIVATION_VALUE = 0.5;
        private double _halfLength;
        private double _median;

        public override double ActivationLevel
        {
            get
            {
                lock (this.locker)
                {
                    return !this.IsActive
                        ? 0
                        : (this.Value.Equals(this._median)
                            ? 1
                            : (MIN_ACTIVATION_VALUE + ((1 - MIN_ACTIVATION_VALUE)*
                                                       (this.Value < this._median
                                                           ? (this.Value - this.Min)/this._halfLength
                                                           : (this.Max - this.Value)/this._halfLength))));
                }
            }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return (this.Value <= this.Min) || (this.Value >= this.Max); }
        }

        /// <summary>
        ///     The minimum value for which this detector becomes activated.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        ///     The maximum value for which this detector becomes activated.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        ///     The current value of the variable being detected.
        /// </summary>
        [JsonIgnore]
        public double Value { get; set; }

        public override void Dispose()
        {
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                this._halfLength = ((this.Max - this.Min)/2.0);
                this._median = this.Min + this._halfLength;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Min: {1:0.##}, Max: {2:0.##}", base.ToString(), this.Min, this.Max);
        }
    }
}