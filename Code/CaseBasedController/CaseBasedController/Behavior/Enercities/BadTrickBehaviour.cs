using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Behavior.Enercities
{
    class BadTrickBehaviour : BaseBehavior
    {
        private bool _aiTurn = false;
        private bool _bestActionReceived = false;
        private bool _newLevelWindowShowed = false;
        private ExecuteAIRecomGameAction _playGameActionBehaviour;


        public BadTrickBehaviour()
        {
            _playGameActionBehaviour = new ExecuteAIRecomGameAction();
        }

        public override void Execute(IFeatureDetector detector)
        {
            RaiseFinishedEvent();
        }

        protected override void AttachPerceptionEvents()
        {
            base.AttachPerceptionEvents();
            perceptionClient.BestActionsPlannedEvent += perceptionClient_BestActionsPlannedEvent;
            perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
            perceptionClient.ReachedNewLevelEvent += perceptionClient_ReachedNewLevelEvent;
            perceptionClient.EndOfLevelHideEvent += perceptionClient_EndOfLevelHideEvent;
        }

        void perceptionClient_BestActionsPlannedEvent(object sender, IAEventArgs e)
        {
            _bestActionReceived = true;
            CheckActivationChanged();
        }
        
        void perceptionClient_EndOfLevelHideEvent(object sender, EventArgs e)
        {
            _newLevelWindowShowed = false;
            CheckActivationChanged();
        }

        void perceptionClient_ReachedNewLevelEvent(object sender, Thalamus.ReachedNewLevelEventArgs e)
        {
            _newLevelWindowShowed = true;
            CheckActivationChanged();
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            _aiTurn = e.GameState.CurrentRole == EnercitiesRole.Mayor;
            _bestActionReceived = false;
            CheckActivationChanged();
        }

        private void CheckActivationChanged()
        {
            if (_aiTurn && _bestActionReceived && !_newLevelWindowShowed)
            {
                _playGameActionBehaviour.Init(actionPublisher, perceptionClient);
                _playGameActionBehaviour.Execute(null);
            }
        }

        public override void Cancel()
        {
            
        }
    }
}
