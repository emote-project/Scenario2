using CaseBasedController.Thalamus;
using EmoteCommonMessages;

namespace CaseBasedController.Detection.Enercities
{
    public class HumanPlayerSpeakingBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _playerIsSpeaking;

        /// <summary>
        ///     The player this detector refers to.
        /// </summary>
        public ActiveUser HumanPlayer { get; set; }

        public override bool IsActive
        {
            get { lock (this.locker) return this._playerIsSpeaking; }
        }

        public override void Dispose()
        {
            //detach from game event
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.ActiveSoundUserEvent -= this.OnActiveSoundUserEvent;
        }

        protected override void AttachEvents()
        {
            //attach to game events
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.ActiveSoundUserEvent += this.OnActiveSoundUserEvent;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.ToString(), this.HumanPlayer);
        }

        private void OnActiveSoundUserEvent(object sender, ActiveSoundUserEventArgs e)
        {
            lock (this.locker)
            {
                this._playerIsSpeaking = e.UserAct.Equals(ActiveUser.Both) || e.UserAct.Equals(this.HumanPlayer);
                this.CheckActivationChanged();
            }
        }
    }
}