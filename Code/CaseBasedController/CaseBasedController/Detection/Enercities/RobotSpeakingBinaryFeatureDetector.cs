using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public class RobotSpeakingBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _robotIsSpeaking;

        public override bool IsActive
        {
            get { lock (this.locker) return this._robotIsSpeaking; }
        }

        public override void Dispose()
        {
            //detach from game event
            lock (this.locker)
                if (this.perceptionClient != null)
                {
                    this.perceptionClient.SpeakStartedEvent -= this.OnSpeakStartedEvent;
                    this.perceptionClient.SpeakFinishedEvent -= this.OnSpeakStoppedEvent;
                }
        }

        protected override void AttachEvents()
        {
            //attach to game events
            lock (this.locker)
                if (this.perceptionClient != null)
                {
                    this.perceptionClient.SpeakStartedEvent += this.OnSpeakStartedEvent;
                    this.perceptionClient.SpeakFinishedEvent += this.OnSpeakStoppedEvent;
                }
        }

        private void OnSpeakStartedEvent(object sender, SpeechEventArgs speechEventArgs)
        {
            lock (this.locker)
            {
                this._robotIsSpeaking = true;
                this.CheckActivationChanged();
            }
        }

        private void OnSpeakStoppedEvent(object sender, SpeechEventArgs speechEventArgs)
        {
            lock (this.locker)
            {
                this._robotIsSpeaking = false;
                this.CheckActivationChanged();
            }
        }
    }
}