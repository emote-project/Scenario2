using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    class NewLevelReachedBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _newLevelReached;

        public override bool IsActive
        {
            get { return _newLevelReached; }
        }

        public override void Dispose()
        {
            perceptionClient.ReachedNewLevelEvent -= perceptionClient_ReachedNewLevelEvent;
            perceptionClient.EndOfLevelHideEvent -= perceptionClient_EndOfLevelHideEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.ReachedNewLevelEvent += perceptionClient_ReachedNewLevelEvent;
            perceptionClient.EndOfLevelHideEvent += perceptionClient_EndOfLevelHideEvent;
        }

        void perceptionClient_EndOfLevelHideEvent(object sender, EventArgs e)
        {
            _newLevelReached = false;
            this.CheckActivationChanged();
        }

        void perceptionClient_ReachedNewLevelEvent(object sender, Thalamus.ReachedNewLevelEventArgs e)
        {
            _newLevelReached = true;
            this.CheckActivationChanged();
        }
    }
}
