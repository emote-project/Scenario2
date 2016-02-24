using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    /// Activates when the watched detector turns active and stay active during a specific period of time
    /// </summary>
    public class StayActiveDetector : WatcherFeatureDetector
    {
        CaseBasedController.MyTimer.TimerData _timer;
        bool _stayActive = false;

        /// <summary>
        /// The number of milliseconds the detector will stay active after detecting the watched detective turning active.
        /// </summary>
        public double Delay = 3000; 

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
            {
                if (_timer != null) MyTimer.RemoveTimer(_timer);
                if (activated)
                {
                    _stayActive = true;
                }
                else
                {
                    _timer = MyTimer.RegisterTimer(Delay/1000, Deactivate);
                }
                CheckActivationChanged();
            }
        }

        private void Deactivate(){
            _stayActive = false;
            CheckActivationChanged();
        }

        public override double ActivationLevel
        {
            get 
            {
                lock (this.locker)
                {
                    return IsActive ? 1 : 0;
                }
            }
        }

        public override bool IsActive
        {
            get {
                lock (this.locker)
                {
                    return _stayActive;
                }
            }
        }
    }
}
