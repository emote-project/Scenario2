using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition.Statistics
{
    public class FeatureActivationTimeDetector : FeatureStatisticalPropertyDetector
    {
        double _lastTimeWatchedTurnedActiveInSeconds = 0;

        public double ActiveTime {
            get {
                return _measure;
            }
            set {
                _measure = value;
            }
        }

        public FeatureActivationTimeDetector(IFeatureDetector watchedDetector, double average, double standardDeviation, ActivationMode mode)
            : base(watchedDetector, mode, average, standardDeviation)
        {
            Reset();
        }

        public override void Init(CaseBasedController.Thalamus.IAllPerceptionClient client)
        {
            base.Init(client);
            Reset();
        }

        protected override void _watchedDetector_ActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
            {
                if (!activated)
                {
                    ActiveTime += GetNow() - _lastTimeWatchedTurnedActiveInSeconds;
                }
                else
                {
                    _lastTimeWatchedTurnedActiveInSeconds = GetNow();
                }
            }
        }


        protected override void TimeElapsedHandler()
        {
            this.CheckActivationChanged();
        }


        protected override void Reset()
        {
            ActiveTime = 0;
             _lastTimeWatchedTurnedActiveInSeconds = GetNow();
        }
    }
}
