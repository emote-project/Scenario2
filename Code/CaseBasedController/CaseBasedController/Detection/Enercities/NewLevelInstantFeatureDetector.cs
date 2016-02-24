using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaseBasedController.Detection.Enercities
{
    public class NewLevelInstantFeatureDetector : InstantFeatureDetector
    {
        public override void Dispose()
        {
            lock (this.locker)
            {
                this.perceptionClient.ReachedNewLevelEvent -= perceptionClient_ReachedNewLevelEvent;
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                this.perceptionClient.ReachedNewLevelEvent += perceptionClient_ReachedNewLevelEvent;
            }
        }

        void perceptionClient_ReachedNewLevelEvent(object sender, Thalamus.ReachedNewLevelEventArgs e)
        {
            lock (this.locker)
            {
                this.OnFeatureDetected();
            }
        }

    }
}
