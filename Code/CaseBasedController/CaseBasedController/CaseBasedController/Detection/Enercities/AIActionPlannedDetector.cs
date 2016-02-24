using System;
using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using EmoteEvents;

namespace CaseBasedController.Detection.Enercities
{
    public class AIActionPlannedDetector : BaseFeatureDetector
    {
        bool _actionReceived = false;

        public override void Dispose()
        {
            this.perceptionClient.ActionsPlannedEvent -= this.ActionsPlannedEventHandler;
            this.perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            this.perceptionClient.ActionsPlannedEvent += this.ActionsPlannedEventHandler;
            this.perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
        }

        void perceptionClient_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            _actionReceived = false;
            this.CheckActivationChanged();
        }

        private void ActionsPlannedEventHandler(object sender, IAEventArgs e)
        {
            if (!e.CurrentPlayer.Equals(GameStatus.CurrentState.CurrentPlayer.Role)) return;

            //console.writeline(e.CurrentPlayer + " --- " +
                              //EnercitiesActionInfo.DeserializeFromJson(e.EnercitiesActionInfo_bestActionInfos[0])
                              //    .ActionType);
            _actionReceived = true;
            this.CheckActivationChanged();
        }

        public override double ActivationLevel
        {
            get { return IsActive ? 1 : 0;  }
        }

        public override bool IsActive
        {
            get { return _actionReceived; }
        }
    }
}