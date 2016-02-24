using CaseBasedController.Detection.Enercities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition.Statistics
{
    public abstract class FeatureStatisticalPropertyDetector : Detection.BinaryFeatureDetector
    {
        public enum ActivationMode { MORE_THAN_AVERAGE, LESS_THAN_AVERAGE, AVERAGE }

        
        /// <summary>
        /// The detector which behaivour we want to observe
        /// </summary>
        IFeatureDetector _watchedDetector;
        /// <summary>
        /// when this detector turns active, the measure is set to 0 and the calculations starts again
        /// </summary>
        IFeatureDetector _detectorDefiningFrequency;
        /// <summary>
        /// This is the generic statistical measure it's going to be observed
        /// </summary>
        protected double _measure;


        public IFeatureDetector WatchedDetector
        {
            get { return _watchedDetector; }
            set { _watchedDetector = value; }
        }

        /// <summary>
        /// The SD value will be used to considere an intevall around the average value, centered on the average value, in which this detector will be considered active
        /// </summary>
        public double StandardDeviation { get; set; }
        /// <summary>
        ///  The Average value of the measure we are going to observe. This is the valuevalue around 
        ///  which is created an interval (+/- Standar deviation) in which this feature is considered active
        /// </summary>
        public double Average { get; set; }
        /// <summary>
        /// Defines when the detector should get active
        /// </summary>
        public ActivationMode Mode { get; private set; }


        public FeatureStatisticalPropertyDetector(IFeatureDetector watcherDetector, ActivationMode mode, double average, double standardDeviation)
        {
            _watchedDetector = watcherDetector;
            _detectorDefiningFrequency = new PerformUtteranceDetector() { Description = "performUtteranceDetector", TimePeriod = 0.5 };
            Mode = mode;
            Average = average;
            StandardDeviation = standardDeviation;
        }


        #region BinaryFeatureDetector implementation
        override public bool IsActive
        {
            get
            {
                lock (this.locker)
                {
                    switch (Mode)
                    {
                        case ActivationMode.AVERAGE:
                            return _measure > GetMin() && _measure < GetMax();
                        case ActivationMode.LESS_THAN_AVERAGE:
                            return _measure < GetMin();
                        case ActivationMode.MORE_THAN_AVERAGE:
                            return _measure > GetMax();
                        default:
                            throw new Exception("No mode defined");
                    }
                }
            }
        }

        public override void Init(Thalamus.IAllPerceptionClient client)
        {
            _watchedDetector.Init(client);
            _detectorDefiningFrequency.Init(client);
            base.Init(client);
        }

        public override void Dispose()
        {
            lock (this.locker)
                _watchedDetector.ActivationChanged -= _watchedDetector_ActivationChanged;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                _watchedDetector.ActivationChanged += _watchedDetector_ActivationChanged;
                _detectorDefiningFrequency.ActivationChanged += _detectorDefiningFrequency_ActivationChanged;
                MyTimer.RegisterRepeatingTimer(0.25, TimeElapsedHandler);
            }
        }

        void _detectorDefiningFrequency_ActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
            {
                if (activated)
                {
                    Reset();
                }
            }
        }

        #endregion

        abstract protected void _watchedDetector_ActivationChanged(IFeatureDetector detector, bool activated);
        abstract protected void TimeElapsedHandler();
        abstract protected void Reset();

        #region helpers

        protected double GetMin()
        {
            return Average - StandardDeviation;
        }

        protected double GetMax()
        {
            return Average + StandardDeviation;
        }

        protected double GetNow()
        {
            return MyTimer.GetCurrentTime().TotalMilliseconds / 1000;
        }

        #endregion 

    }
}
