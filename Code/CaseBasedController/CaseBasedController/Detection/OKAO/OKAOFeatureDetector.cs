using CaseBasedController.Thalamus;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Utilities.Math;

namespace CaseBasedController.Detection.OKAO
{
    public enum Subject
    {
        Left,
        Right
    }

    public abstract class OKAOFeatureDetector : BinaryFeatureDetector
    {
        private const uint MAX_VALUE = 100;
        private const double DEFAULT_THRESHOLD = 0.5;
        private const double DEFAULT_Q = 0.1;
        private const int DEFAULT_R = 10;
        protected readonly KalmanFilter<double> filter = new KalmanFilter<double>(GetMatrix(0), GetMatrix(0));
        protected bool isSubject;

        protected OKAOFeatureDetector()
        {
            this.Q = DEFAULT_Q;
            this.R = DEFAULT_R;
            this.Threshold = DEFAULT_THRESHOLD;
        }

        /// <summary>
        ///     The subject that this detector refers to.
        /// </summary>
        public Subject Subject { get; set; }

        /// <summary>
        ///     The covariance of the process noise associated with the features's Kalman filter.
        ///     <see cref="http://en.wikipedia.org/wiki/Kalman_filter" />
        /// </summary>
        public double Q
        {
            get { return GetValue(this.filter.Q); }
            set { this.filter.Q = GetMatrix(value); }
        }

        /// <summary>
        ///     The covariance of the observation noise associated with the features's Kalman filter.
        ///     <see cref="http://en.wikipedia.org/wiki/Kalman_filter" />
        /// </summary>
        public double R
        {
            get { return GetValue(this.filter.R); }
            set { this.filter.R = GetMatrix(value); }
        }

        /// <summary>
        ///     The minimum value for the expression for this detector to become active. Value must be in [0, 1].
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        ///     This detector becomes active when the filtered OKAO perception value is above the defined threshold.
        /// </summary>
        public override bool IsActive
        {
            get { lock (this.locker) return this.isSubject && GetValue(this.filter.State) > this.Threshold; }
        }

        public override void Dispose()
        {
            //detach from OKAO event
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.OKAOPerceptionEvent -= this.OnOKAOPerceptionEvent;
        }

        protected override void AttachEvents()
        {
            //attach to OKAO events
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.OKAOPerceptionEvent += this.OnOKAOPerceptionEvent;
        }

        protected virtual void OnOKAOPerceptionEvent(object sender, OKAOPerceptionArgs okaoPerceptionArgs)
        {
            lock (this.locker)
                this.isSubject = okaoPerceptionArgs.PerceptionLog.subject.Equals(this.Subject.ToString().ToLower());
        }

        protected static DiagonalMatrix GetMatrix(double value)
        {
            return new DiagonalMatrix(1, 1, value);
        }

        protected static double GetValue(Matrix<double> parameter)
        {
            return parameter[0, 0];
        }

        protected void UpdateValue(double value)
        {
            //updates the filter with normalized value
            this.filter.Update(GetMatrix(value/MAX_VALUE));

            //checks if value has changed
            this.CheckActivationChanged();
        }
    }
}