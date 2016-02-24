using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities
{
    public class LastActionTypeDetector : BinaryFeatureDetector
    {
        private bool _actionTypeDetected;

        /// <summary>
        ///     The sprecified action type to be detected.
        /// </summary>
        public ActionType ActionType { get; set; }

        public override bool IsActive
        {
            get { lock (this.locker) return this._actionTypeDetected; }
        }

        public override void Dispose()
        {
            //detach from all game event
            lock (this.locker)
                if (this.perceptionClient != null)
                {
                    this.perceptionClient.ConfirmConstructionEvent -= this.OnGameActionEvent;
                    this.perceptionClient.PerformUpgradeEvent -= this.OnGameActionEvent;
                    this.perceptionClient.ImplementPolicyEvent -= this.OnGameActionEvent;
                    this.perceptionClient.SkipTurnEvent -= this.OnGameActionEvent;
                }
        }

        protected override void AttachEvents()
        {
            //attach to all game events
            lock (this.locker)
                if (this.perceptionClient != null)
                {
                    this.perceptionClient.ConfirmConstructionEvent += this.OnGameActionEvent;
                    this.perceptionClient.PerformUpgradeEvent += this.OnGameActionEvent;
                    this.perceptionClient.ImplementPolicyEvent += this.OnGameActionEvent;
                    this.perceptionClient.SkipTurnEvent += this.OnGameActionEvent;
                }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.ToString(), this.ActionType);
        }

        private void OnGameActionEvent(object sender, GameActionEventArgs e)
        {
            lock (this.locker)
            {
                this._actionTypeDetected = ((ActionType) e.ActionTypeEnum).Equals(this.ActionType);
                this.CheckActivationChanged();
            }
        }
    }
}