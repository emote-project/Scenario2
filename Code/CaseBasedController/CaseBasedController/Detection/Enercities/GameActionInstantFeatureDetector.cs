using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public class GameActionInstantFeatureDetector : InstantFeatureDetector
    {
        public override void Dispose()
        {
            //detach from all game events
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

        private void OnGameActionEvent(object sender, GameActionEventArgs e)
        {
            this.OnFeatureDetected();
        }
    }
}