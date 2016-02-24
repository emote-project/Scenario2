using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities.FirstActions
{
    class AfterFirstUpgradeDoneByMayorBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _firstUpgradeDone;

        public override bool IsActive
        {
            get { return _firstUpgradeDone; }
        }

        public override void Dispose()
        {
            perceptionClient.PerformUpgradeEvent -= perceptionClient_PerformUpgradeEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.PerformUpgradeEvent += perceptionClient_PerformUpgradeEvent;
        }

        void perceptionClient_PerformUpgradeEvent(object sender, Thalamus.GameActionEventArgs e)
        {
            if (GameInfo.GameStatus.CurrentState.CurrentPlayer.Role != EnercitiesRole.Mayor) return;
            _firstUpgradeDone = true;
            CheckActivationChanged();
        }
    }
}
