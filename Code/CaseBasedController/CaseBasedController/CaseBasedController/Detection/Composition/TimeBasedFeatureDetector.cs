using System;
using System.Threading;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Base for time-based feature detectors.
    /// </summary>
    public abstract class TimeBasedFeatureDetector : WatcherFeatureDetector
    {
        protected const double ACTIVATION_FACTOR = 0.8;
        protected const double DEF_MIN_INT = 5;
        protected DateTime activationTime = DateTime.MaxValue;
        protected bool isActive;
        protected CaseBasedController.MyTimer.TimerData timer;

        protected TimeBasedFeatureDetector()
        {
            this.TimePeriod = DEF_MIN_INT;
            this.timer = MyTimer.RegisterTimer(99999999999, OnTimerElapsed);
            this.timer.CreationTime = (double)(DateTime.Today.AddYears(1).Ticks / (decimal)TimeSpan.TicksPerSecond);
        }

        /// <summary>
        ///     The amount of time, in seconds, during which the <see cref="WatcherFeatureDetector.WatchedDetector" /> has to have
        ///     been activated (inactive->active) for this detector to become active.
        /// </summary>
        public abstract double TimePeriod { get; set; }

        public override double ActivationLevel
        {
            get
            {
                //AL = 1 - (0.8 ^ t), where t is time passed since last activation
                lock (this.locker)
                    return this.activationTime.Equals(DateTime.MaxValue)
                        ? 0
                        : 1.0 -
                          Math.Pow(ACTIVATION_FACTOR, DateTime.Now.Subtract(this.activationTime).TotalSeconds);
            }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return this.isActive; }
        }

        public override void Dispose()
        {
            base.Dispose();
            lock (this.locker) MyTimer.RemoveTimer(timer);
        }

        protected abstract void OnTimerElapsed();

        protected virtual void DisableTimer()
        {
            lock (this.locker)
                this.timer.CreationTime = (double)(DateTime.Today.AddYears(1).Ticks / (decimal)TimeSpan.TicksPerSecond);
        }

        protected virtual void EnableTimer()
        {
            lock (this.locker)
            {
                this.timer.CreationTime = (double)(DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerSecond);
                this.timer.Delay = this.TimePeriod * 1000;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Period: {1:0.##} s", base.ToString(), this.TimePeriod);
        }
    }
}