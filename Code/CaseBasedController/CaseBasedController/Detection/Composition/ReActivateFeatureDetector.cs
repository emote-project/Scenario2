using System;
using System.Threading;

namespace CaseBasedController.Detection.Composition
{
    public class ReActivateFeatureDetector : WatcherFeatureDetector
    {
        /// <summary>
        /// The amount of time passed between on activation and another in <B>milliseconds</B>
        /// </summary>
        public double Interval { get; set; }

        bool _active = false;
        CaseBasedController.MyTimer.TimerData _timer;

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (locker)
            {
                if (activated)
                {
                    _active = true;
                    _timer = MyTimer.RegisterTimer(Interval, TimerElapsed);
                    this.CheckActivationChanged();
                }
                else
                {
                    MyTimer.RemoveTimer(_timer);
                    _active = false;
                }
                this.CheckActivationChanged();
            }
        }

        private void TimerElapsed()
        {
            lock (locker)
            {
                this._active = false;
                this.CheckActivationChanged();
                MyTimer.RegisterTimer(1000, ReActivate);
            }
        }

        private void ReActivate()
        {
            lock (locker)
            {
                this._active = false;
                this.CheckActivationChanged();
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
    }
}


















































//namespace CaseBasedController.Detection.Composition
//{
//    /// <summary>
//    ///     Detects when other detectors remain active during a specified time period and re-sending the activation events.
//    /// </summary>
//    public class ReActivateFeatureDetector : TimeBasedFeatureDetector
//    {
//        /// <summary>
//        ///     The amount of time, in seconds, that the <see cref="WatcherFeatureDetector.WatchedDetector" /> is again checked for
//        ///     activation for this detector to become active.
//        /// </summary>
//        public override double TimePeriod { get; set; }

//        /// <summary>
//        ///     The amount of time, in seconds, to leave the detector activated, before deactivating it.
//        /// </summary>
//        public double ActiveTimePeriod { get; set; }

//        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
//        {
//            lock (this.locker)
//                if (activated)
//                {
//                    //starts the timer (for activation re-send) and sends activation "signal"
//                    this.EnableTimer();
//                }
//                else
//                {
//                    //just stops the timer
//                    this.DisableTimer();
//                }
//        }

//        private void Activate()
//        {
//            lock (this.locker)
//            {
//                //instant event detected, activate
//                this.isActive = true;
//                this.CheckActivationChanged();
//            }

//            //wait for a while
//            Thread.Sleep((int) (this.ActiveTimePeriod*1000));

//            MyTimer.RegisterTimer(this.ActiveTimePeriod * 1000, Deactivate);

//        }

//        private void Deactivate()
//        {
//            lock (this.locker)
//            {
//                //deactivate
//                this.isActive = false;
//                this.CheckActivationChanged();
//            }
//        }

//        protected override void OnTimerElapsed()
//        {
//            //sub-detector is still active, just re-send activation "signal"
//            this.Activate();
//        }

//        protected override void EnableTimer()
//        {
//            this.activationTime = DateTime.Now;
//            base.EnableTimer();
//        }

//        public override string ToString()
//        {
//            return string.Format("{0}, Active: {1:0.##} s", base.ToString(), this.ActiveTimePeriod);
//        }
//    }
//}