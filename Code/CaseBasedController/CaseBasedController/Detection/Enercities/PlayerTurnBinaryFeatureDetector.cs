using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    ///     This detector is active during the play turn of some player.
    /// </summary>
    public class PlayerTurnBinaryFeatureDetector : BinaryFeatureDetector
    {
        /// <summary>
        ///     The player this detector refers to.
        /// </summary>
        public EnercitiesRole Player { get; set; }

        public override bool IsActive
        {
            get
            {
                lock (this.locker)
                    return GameStatus.CurrentState != null &&
                           GameStatus.CurrentState.CurrentPlayer.Role.Equals(this.Player);
            }
        }

        public override void Dispose()
        {
            //detach event
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.TurnChangedEvent -= this.OnTurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.TurnChangedEvent += this.OnTurnChangedEvent;
        }

        private void OnTurnChangedEvent(object sender, GenericGameEventArgs genericGameEventArgs)
        {
            //checks activation based on current player turn, raise event if necessary
            lock (this.locker)
                this.CheckActivationChanged();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", base.ToString(), this.Player);
        }
    }
}