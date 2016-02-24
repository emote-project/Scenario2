using System;
using Thalamus;

namespace ControlPanel.Thalamus
{

    public interface ILearnerDbPublisher : IAction,
        EmoteCommonMessages.IEmoteActions,
        EmoteCommonMessages.ILearnerModelIdActions,
        EmoteEnercitiesMessages.IEnercitiesGameStateActions
    { }


    public class LearnerDBThalamusPublisher : ILearnerDbPublisher
    {
        private readonly dynamic _publisher;

        public LearnerDBThalamusPublisher(dynamic publisher)
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

#warning DELETE THIS METHOD ASAP
        [Obsolete("Deleteme")]
        public void Start(int participantId, int participantId2, string participant1Name, string participant2Name)
        {
            _publisher.Start(participantId, participantId2, participant1Name, participant2Name);
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
