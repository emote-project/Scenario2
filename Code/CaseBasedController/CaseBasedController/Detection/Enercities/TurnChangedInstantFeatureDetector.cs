using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public class TurnChangedInstantFeatureDetector : InstantFeatureDetector
    {
        public override void Dispose()
        {
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.TurnChangedEvent += this.TurnChangedEvent;
        }

        private void TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            this.OnFeatureDetected();
        }
    }
}