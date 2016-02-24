using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities.FirstActions
{
    class AfterFirstSkipDoneByMayorBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _skipDone;

        public override bool IsActive
        {
            get { return _skipDone; }
        }

        public override void Dispose()
        {
            perceptionClient.SkipTurnEvent -= PerceptionClientOnSkipTurnEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.SkipTurnEvent += PerceptionClientOnSkipTurnEvent;
        }

        private void PerceptionClientOnSkipTurnEvent(object sender, GameActionEventArgs gameActionEventArgs)
        {
            if (GameInfo.GameStatus.CurrentState!=null && GameInfo.GameStatus.CurrentState.CurrentPlayer.Role != EnercitiesRole.Mayor) return;
            _skipDone = true;
            CheckActivationChanged();
        }
    }
}
