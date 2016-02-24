using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Composition
{
    public class WasActiveDetector : WatcherFeatureDetector
    {
        bool _active = false;
        CaseBasedController.MyTimer.TimerData _timer;

        double _delay = 5000;
        public double Delay
        {
            get { return _delay; }
            set { _delay = value; }
        } 
        

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (locker)
            {
                if (activated) 
                {
                    if (_timer != null) MyTimer.RemoveTimer(_timer);
                    _active = true;
                    this.CheckActivationChanged();
                    _timer = MyTimer.RegisterTimer(Delay / 1000, Deactivate);
                    this.CheckActivationChanged();
                }
            }
        }

        public override double ActivationLevel
        {
            get { return IsActive ? 1 : 0; }
        }

        public override bool IsActive
        {
            get { return _active; }
        }

        private void Deactivate()
        {
            lock (locker)
            {
                _active = false;
                this.CheckActivationChanged();
            }
        }

        public override void Dispose()
        {
            lock (locker)
            {
                MyTimer.RemoveTimer(_timer);
                base.Dispose();
            }
        }
    }
}
