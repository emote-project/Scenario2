using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    class NewLevelMenuShowedBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _showed = false;

        public override bool IsActive
        {
            get { return _showed; }
        }

        public override void Dispose()
        {
            perceptionClient.ReachedNewLevelEvent -= PerceptionClientOnEndOfLevelShowEvent;
            perceptionClient.EndOfLevelHideEvent -= PerceptionClientOnEndOfLevelHideEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.ReachedNewLevelEvent += PerceptionClientOnEndOfLevelShowEvent;
            perceptionClient.EndOfLevelHideEvent += PerceptionClientOnEndOfLevelHideEvent;
        }

        private void PerceptionClientOnEndOfLevelHideEvent(object sender, EventArgs eventArgs)
        {
            lock (this.locker)
            {
                //console.writeline("End of level menu HIDDEN");
                _showed = false;
                this.CheckActivationChanged();
            }
        }

        private void PerceptionClientOnEndOfLevelShowEvent(object sender, EventArgs eventArgs)
        {
            lock (this.locker)
            {
                //console.writeline("End of level menu SHOWED");
                _showed = true;
                this.CheckActivationChanged();
            }
        }
    }
}
