using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaseBasedController.Detection.Enercities
{
    public class PlayerInteractionBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _interacted = false;

        public override bool IsActive
        {
            get { lock (this.locker) return _interacted; }
        }

        public override void Dispose()
        {
            lock (this.locker)
            {
                this.perceptionClient.BuildingMenuToolSelectedEvent -= perceptionClient_MessedWithMenus;
                this.perceptionClient.BuildMenuTooltipShowedEvent -= perceptionClient_MessedWithMenus;
                this.perceptionClient.PoliciesMenuShowedEvent -= perceptionClient_MessedWithMenus2;
                this.perceptionClient.UpgradesMenuShowedEvent -= perceptionClient_MessedWithMenus2;
                this.perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                this.perceptionClient.BuildingMenuToolSelectedEvent += perceptionClient_MessedWithMenus;
                this.perceptionClient.BuildMenuTooltipShowedEvent += perceptionClient_MessedWithMenus;
                this.perceptionClient.PoliciesMenuShowedEvent += perceptionClient_MessedWithMenus2;
                this.perceptionClient.UpgradesMenuShowedEvent += perceptionClient_MessedWithMenus2;
                this.perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
            }
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            lock (this.locker)
            {
                _interacted = false;
                CheckActivationChanged();
            }
        }

        // We just need to know whenever one of these events is fired, so we route everything into only one method
        void perceptionClient_MessedWithMenus2(object sender, EventArgs e)
        {
            perceptionClient_MessedWithMenus(sender, null);
        }
        void perceptionClient_MessedWithMenus(object sender, Thalamus.MenuEventArgs e)
        {
            lock (this.locker)
            {
                _interacted = true;
                CheckActivationChanged();
            }
        }

        
    }
}
