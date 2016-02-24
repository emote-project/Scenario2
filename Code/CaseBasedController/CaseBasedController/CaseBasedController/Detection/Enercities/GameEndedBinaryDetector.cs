using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    class GameEndedBinaryDetector : BinaryFeatureDetector
    {
        public enum EndingType
        {
            NoOil,
            Win,
            TimeUp,
            Any
        }

        public EndingType GameEndType { get; set; }

        private bool _gameEnded = false;
 
        public override bool IsActive
        {
            get { return _gameEnded; }
        }

        public override void Dispose()
        {
            perceptionClient.EndGameNoOilEvent -= PerceptionClientOnEndGameNoOilEvent;
            perceptionClient.EndGameSuccessfullEvent -= PerceptionClientOnEndGameSuccessfullEvent;
            perceptionClient.EndGameTimeOutEvent -= PerceptionClientOnEndGameTimeOutEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.EndGameNoOilEvent += PerceptionClientOnEndGameNoOilEvent;
            perceptionClient.EndGameSuccessfullEvent += PerceptionClientOnEndGameSuccessfullEvent;
            perceptionClient.EndGameTimeOutEvent += PerceptionClientOnEndGameTimeOutEvent;
        }

        private void PerceptionClientOnEndGameTimeOutEvent(object sender, EndGameEventArgs endGameEventArgs)
        {
            if (GameEndType == EndingType.Any || GameEndType == EndingType.TimeUp)
            {
                _gameEnded = true;
                CheckActivationChanged();
            }
        }

        private void PerceptionClientOnEndGameSuccessfullEvent(object sender, EndGameEventArgs endGameEventArgs)
        {
            if (GameEndType == EndingType.Any || GameEndType == EndingType.Win)
            {
                _gameEnded = true;
                CheckActivationChanged();
            }
        }

        private void PerceptionClientOnEndGameNoOilEvent(object sender, EndGameEventArgs endGameEventArgs)
        {
            if (GameEndType == EndingType.Any || GameEndType == EndingType.NoOil)
            {
                _gameEnded = true;
                CheckActivationChanged();
            }
        }
    }
}
