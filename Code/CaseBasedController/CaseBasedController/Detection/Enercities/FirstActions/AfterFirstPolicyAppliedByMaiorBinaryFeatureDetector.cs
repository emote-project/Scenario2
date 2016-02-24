using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities.FirstActions
{
    class AfterFirstPolicyAppliedByMaiorBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _firstPolicyApplied;

        public override bool IsActive
        {
            get { return _firstPolicyApplied; }
        }

        public override void Dispose()
        {
            perceptionClient.ImplementPolicyEvent -= perceptionClient_ImplementPolicyEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.ImplementPolicyEvent += perceptionClient_ImplementPolicyEvent;
        }

        void perceptionClient_ImplementPolicyEvent(object sender, Thalamus.GameActionEventArgs e)
        {
            if (GameInfo.GameStatus.CurrentState.CurrentPlayer.Role != EnercitiesRole.Mayor) return;
            _firstPolicyApplied = true;
            CheckActivationChanged();
        }
    }
}
