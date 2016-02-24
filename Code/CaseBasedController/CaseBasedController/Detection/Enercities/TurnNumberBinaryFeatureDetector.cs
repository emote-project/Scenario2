using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    class TurnNumberBinaryFeatureDetector : BinaryFeatureDetector
    {
        private int _turnNumber;

        public int TurnNumber
        {
            get { return _turnNumber; }
            set { _turnNumber = value; }
        }


        public TurnNumberBinaryFeatureDetector(int turnNumber)
        {
            _turnNumber = turnNumber;
        }

        public override bool IsActive
        {
            get { return GameInfo.GameStatus.CurrentState!=null && GameInfo.GameStatus.CurrentState.TurnNumber == _turnNumber; }
        }

        
        public override void Dispose()
        {
            perceptionClient.GameStartedEvent += PerceptionClientOnGameStartedEvent;
            perceptionClient.TurnChangedEvent -= PerceptionClientOnTurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.GameStartedEvent += PerceptionClientOnGameStartedEvent;
            perceptionClient.TurnChangedEvent += PerceptionClientOnTurnChangedEvent;
        }

        private void PerceptionClientOnGameStartedEvent(object sender, GenericGameEventArgs genericGameEventArgs)
        {
            CheckActivationChanged();
        }

        private void PerceptionClientOnTurnChangedEvent(object sender, GenericGameEventArgs genericGameEventArgs)
        {
            CheckActivationChanged();
        }

        public override string ToString()
        {
            return "TurnNumberDetector: " + _turnNumber;
        }
    }
}
