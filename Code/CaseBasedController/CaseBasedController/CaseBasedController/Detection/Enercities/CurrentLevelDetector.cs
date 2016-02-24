using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public class CurrentLevelDetector : BinaryFeatureDetector
    {
        private uint _curLevel;

        /// <summary>
        ///     The level we want to detect.
        /// </summary>
        public uint Level { get; set; }

        public override bool IsActive
        {
            get { lock (this.locker) return this._curLevel.Equals(this.Level); }
        }

        public override void Dispose()
        {
            //detach from game event
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.ReachedNewLevelEvent -= this.OnReachedNewLevelEvent;
        }

        protected override void AttachEvents()
        {
            //attach to game events
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.ReachedNewLevelEvent += this.OnReachedNewLevelEvent;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.ToString(), this.Level);
        }

        private void OnReachedNewLevelEvent(object sender, ReachedNewLevelEventArgs reachedNewLevelEventArgs)
        {
            lock (this.locker)
            {
                this._curLevel = (uint) reachedNewLevelEventArgs.Level;
                this.CheckActivationChanged();
            }
        }
    }
}