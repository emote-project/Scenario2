using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Detection.Enercities
{
    class AiPlannedActionForMayorBinaryFeatureDetector : BinaryFeatureDetector
    {
        private bool _aiTurn = false;
        private bool _bestActionReceived = false;
        private bool _newLevelWindowShowed = false;

        private bool _isTalking;
        private int _turn = 0;


        public override bool IsActive
        {
            get
            {
                return _aiTurn && _bestActionReceived && !_newLevelWindowShowed && !(GameInfo.GameStatus.Session==1 && _turn<=2);
            }
        }

        

        public override void Dispose()
        {
            perceptionClient.ActionsPlannedEvent -= perceptionClient_ActionsPlannedEvent;
            perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.ActionsPlannedEvent += perceptionClient_ActionsPlannedEvent;
            perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
            perceptionClient.ReachedNewLevelEvent += perceptionClient_ReachedNewLevelEvent;
            perceptionClient.EndOfLevelHideEvent += perceptionClient_EndOfLevelHideEvent;
            perceptionClient.UtteranceFinishedEvent += perceptionClient_UtteranceFinishedEvent;
            perceptionClient.UtteranceStartedEvent += perceptionClient_UtteranceStartedEvent;
        }

        void perceptionClient_UtteranceStartedEvent(object sender, Thalamus.IFMLUtteranceEventArgs e)
        {
            _isTalking = true;
        }

        void perceptionClient_UtteranceFinishedEvent(object sender, Thalamus.IFMLUtteranceEventArgs e)
        {
            _isTalking = false;
        }


        void perceptionClient_ActionsPlannedEvent(object sender, Thalamus.IAEventArgs e)
        {
            _bestActionReceived = true;
            this.CheckActivationChanged();
        }

        void perceptionClient_EndOfLevelHideEvent(object sender, EventArgs e)
        {
            _newLevelWindowShowed = false;
            this.CheckActivationChanged();
        }

        void perceptionClient_ReachedNewLevelEvent(object sender, Thalamus.ReachedNewLevelEventArgs e)
        {
            _newLevelWindowShowed = true;
            this.CheckActivationChanged();
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            _turn++;
            _aiTurn = e.GameState.CurrentRole == EnercitiesRole.Mayor;
            _bestActionReceived = false;
            this.CheckActivationChanged();

            _turnChanged = true;
        }

        public override void CheckActivationChanged(bool force = false)
        {
            base.CheckActivationChanged(force);
            if (IsActive)
            {
                AntiBugMethod();
            }
        }


        private bool _turnChanged = false;
        private async void AntiBugMethod()
        {
            while (_isTalking && !_turnChanged)
            {
                MainController.SkipTurn();
                await Task.Delay(3000);
            }
            _turnChanged = false;
        }



    }
}
