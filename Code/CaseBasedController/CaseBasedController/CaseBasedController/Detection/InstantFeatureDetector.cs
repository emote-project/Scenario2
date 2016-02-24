using System.Threading;

namespace CaseBasedController.Detection
{
    /// <summary>
    ///     Detects "instant features", i.e., features that become active when some event occurs. This detector will usually be
    ///     attached to some system event.
    /// </summary>
    public abstract class InstantFeatureDetector : BinaryFeatureDetector
    {
        private const double DEF_TIME_PERIOD = 1;
        private bool _isActive;

        protected InstantFeatureDetector()
        {
            this.TimePeriod = DEF_TIME_PERIOD;
        }


        public override bool IsActive
        {
            get { return this._isActive; }
        }

        /// <summary>
        ///     The amount of time, in seconds, to leave the detector activated after the event is detected, before deactivating
        ///     it.
        /// </summary>
        public double TimePeriod { get; set; }

        protected void OnFeatureDetected()
        {
            lock (this.locker)
            {
                //instant event detected, activate
                this._isActive = true;
                this.CheckActivationChanged();
            }

            //wait for a while
            //Thread.Sleep((int) (this.TimePeriod*1000));

            MyTimer.RegisterTimer(1, _OnFeatureDetected);

        }

        private void _OnFeatureDetected()
        {
            lock (this.locker)
            {
                //deactivate
                this._isActive = false;
                this.CheckActivationChanged();
            }
        }
    }
}