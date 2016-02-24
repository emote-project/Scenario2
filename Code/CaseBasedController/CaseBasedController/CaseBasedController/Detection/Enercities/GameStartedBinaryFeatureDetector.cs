using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    /// Fires only when receiving the GameStartedMessage
    /// </summary>
    public class GameStartedBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _started;
        public override bool IsActive
        {
            get { return _started; }
        }

        public override void Dispose()
        {
            perceptionClient.GameStartedEvent -= perceptionClient_GameStartedEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.GameStartedEvent += perceptionClient_GameStartedEvent;
        }

        void perceptionClient_GameStartedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            lock (this.locker)
            {
                _started = true;
                this.CheckActivationChanged();
            }
        }
    }
}
