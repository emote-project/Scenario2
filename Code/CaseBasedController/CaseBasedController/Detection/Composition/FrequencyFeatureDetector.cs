using System;
using System.Collections.Generic;
using System.Threading;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Detects when other detectors become frequently active during some time period.
    /// </summary>
    public class FrequencyFeatureDetector : WatcherFeatureDetector
    {
        private const double ACTIVATION_FACTOR = 0.1;
        private const double DEF_TIME_PERIOD = 10;
        private const uint DEF_MIN_FREQ = 5;
        private readonly Dictionary<uint, Timer> _activeTimers = new Dictionary<uint, Timer>();
        private uint _timerCounter;

        public FrequencyFeatureDetector()
        {
            this.TimePeriod = DEF_TIME_PERIOD;
            this.MinActiveFrequency = DEF_MIN_FREQ;
        }

        /// <summary>
        ///     The period of time, in seconds, counting from the current time, that the
        ///     <see cref="WatcherFeatureDetector.WatchedDetector" /> must have been activated for this detector to become active.
        /// </summary>
        public double TimePeriod { get; set; }

        /// <summary>
        ///     The minimum number of times that the <see cref="WatcherFeatureDetector.WatchedDetector" /> must have been activated
        ///     during the specified <see cref="TimePeriod" />.
        /// </summary>
        public uint MinActiveFrequency { get; set; }

        public override double ActivationLevel
        {
            get
            {
                //AL = 1 - (0.1 ^ f), where f is the difference between the frequency and the required number of activations 
                //during the last time period
                lock (this.locker)
                    return this._activeTimers.Count < this.MinActiveFrequency
                        ? 0
                        : 1.0 - Math.Pow(ACTIVATION_FACTOR, this._activeTimers.Count - this.MinActiveFrequency);
            }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return this._activeTimers.Count >= this.MinActiveFrequency; }
        }

        public override void Dispose()
        {
            lock (this.locker) this._activeTimers.Clear();
            base.Dispose();
        }

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this.locker)
                if (activated)
                {
                    //starts the timer to elapse after the specified time period
                    var state = this._timerCounter++;
                    var timer = new Timer(this.OnTimerElapsed, state, 0, (int) (this.TimePeriod*1000));

                    //give the reference so that it can later be destroyed.
                    this._activeTimers.Add(state, timer);

                    //checks whether this detector became active as a consequence of this activation
                    this.CheckActivationChanged();
                }
        }

        private void OnTimerElapsed(object state)
        {
            lock (this.locker)
            {
                //disposes of elapsed timer
                var timerID = (uint) state;
                this._activeTimers[timerID].Dispose();
                this._activeTimers.Remove(timerID);

                //checks whether this detector became inactive as a consequence of this inactivation
                this.CheckActivationChanged();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Period: {1:0.##} s, Frequency: {2}",
                base.ToString(), this.TimePeriod, this.MinActiveFrequency);
        }
    }
}