using System;
using Thalamus;

namespace ControlPanel.Thalamus
{

    public interface IControlPanelThalamusPublisher : IAction,
        EmoteCommonMessages.IEmoteActions,
        EmoteCommonMessages.ILearnerModelIdActions,
        EmoteEnercitiesMessages.IEnercitiesGameStateActions
    { }


    public class ControlPanelThalamusPublisher : IControlPanelThalamusPublisher
    {
        private readonly dynamic _publisher;

        public ControlPanelThalamusPublisher(dynamic publisher)
        {
            this._publisher = publisher;
        }

        public void Stop()
        {
            _publisher.Stop();
        }

        public void Reset()
        {
            _publisher.Reset();
        }

        public void SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            _publisher.SetLearnerInfo(LearnerInfo_learnerInfo);
        }

        public void Start(string StartMessageInfo_info)
        {
            _publisher.Start(StartMessageInfo_info);
        }

        public void getNextThalamusId()
        {
            _publisher.getNextThalamusId();
        }

        public void getAllLearnerInfo()
        {
            _publisher.getAllLearnerInfo();
        }


        public void getAllUtterancesForParticipant(int participantId)
        {
            _publisher.getAllUtterancesForParticipant(participantId);
        }

        public void EndGameTimeout()
        {
            _publisher.EndGameTimeout();
        }
    }
}
