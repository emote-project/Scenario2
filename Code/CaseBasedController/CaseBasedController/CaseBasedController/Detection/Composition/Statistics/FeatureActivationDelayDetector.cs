using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition.Statistics
{
    public class FeatureActivationDelayDetector : FeatureStatisticalPropertyDetector
    {
        double _activationTime;


        public double ActiveDelay
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
            }
        }

        public FeatureActivationDelayDetector(IFeatureDetector watchedDetector, double average, double standardDeviation, ActivationMode mode)
            : base(watchedDetector, mode, average, standardDeviation)
        {
            Reset();
        }


        protected override void _watchedDetector_ActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
            {
                if (activated)
                {
                    _activationTime = GetNow();
                }
            }
        }

        protected override void TimeElapsedHandler()
        {
            ActiveDelay = GetNow() - _activationTime;
            CheckActivationChanged();
        }

        protected override void Reset()
        {
            _activationTime = 0;
            ActiveDelay = 0;
        }
    }
}
