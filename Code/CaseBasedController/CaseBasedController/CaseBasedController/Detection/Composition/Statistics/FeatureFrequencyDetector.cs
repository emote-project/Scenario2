using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition.Statistics
{
    /// <summary>
    /// Detects when a detector is being activated with a certain frequency.
    /// A different detector is used to define how the frequency is computed. When this detector activates, the frequency is reset to 0 and the computation starts again.
    /// </summary>
    public class FeatureFrequencyDetector : FeatureStatisticalPropertyDetector
    {
        /// <summary>
        /// when we started calculating the frequency
        /// </summary>
        double _startTimeInSeconds;
        /// <summary>
        /// How many times the watched detector have been activated since we started calculating the frequency
        /// </summary>
        int _timesActivated;

        
        
        /// <summary>
        /// Current frequency detected (it changes as time passes)
        /// </summary>
        public double Frequency
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
        
        public FeatureFrequencyDetector(IFeatureDetector watchedDetector, double average, double standardDeviation, ActivationMode mode) 
            : base (watchedDetector, mode, average, standardDeviation)
        {
            Reset();
        }

        public override void Init(CaseBasedController.Thalamus.IAllPerceptionClient client)
        {
            base.Init(client);
            Reset();
        }

        override protected void Reset()
        {
            _timesActivated = 0;
            _startTimeInSeconds = GetNow();
        }
                
        override protected void TimeElapsedHandler()
        {
            CalculateFrequency();
            this.CheckActivationChanged();
        }

        override protected void _watchedDetector_ActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
            {
                if (activated)
                {
                    _timesActivated++;
                }
                this.CheckActivationChanged();
            }
        }

        private void CalculateFrequency()
        {
            Frequency = _timesActivated; ///(GetNow() - _startTimeInSeconds);
        }

    }
}
