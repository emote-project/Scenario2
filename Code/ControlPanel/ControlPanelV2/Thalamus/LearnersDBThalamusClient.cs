using System;
using System.Collections.Generic;
using EmoteEvents;
using Thalamus;
using Environment = System.Environment;

namespace ControlPanel.Thalamus
{
    public class LearnersDbThalamusClient : ThalamusClient, ILearnerDbClient
    {
        private static LearnersDbThalamusClient _instance;

        public class AllLearnerInfoEventArgs
        {
            public List<LearnerInfo> Learners { get; private set; }

            public AllLearnerInfoEventArgs(List<LearnerInfo> learners)
            {
                Learners = learners;
            }
        }
        public event EventHandler<AllLearnerInfoEventArgs> AllLearnerInfoEvent;
        public class NextThalamusIdEventArgs
        {
            public int Id { get; private set; }

            public NextThalamusIdEventArgs(int id)
            {
                Id = id;
            }
        }
        public event EventHandler<NextThalamusIdEventArgs> NextThalamusIdEvent;


        private readonly ILearnerDbPublisher _publisher;

        public ILearnerDbPublisher LDBPublisher
        {
            get { return _publisher; }
        }

        public static LearnersDbThalamusClient GetInstance()
        {
            string[] args = Environment.GetCommandLineArgs();
            string charName = "";
            if (args.Length > 1) charName = args[1];

            if (_instance==null) _instance = new LearnersDbThalamusClient(charName);
            return _instance;
        }

        private LearnersDbThalamusClient(string characterName) : base("EmoteControlPanel", characterName)
        {
            SetPublisher<ILearnerDbPublisher>();
            _publisher = new LearnerDBThalamusPublisher(this.Publisher);
        }


        #region PERCEPTIONS

        public void nextThalamusId(int participantId)
        {
            if(NextThalamusIdEvent!=null) NextThalamusIdEvent(this, new NextThalamusIdEventArgs(participantId));
        }

        public void allLearnerInfo(string[] LearnerInfo_learnerInfos)
        {
            List<LearnerInfo> learners = new List<LearnerInfo>();
            foreach (var ls in LearnerInfo_learnerInfos)
            {
                LearnerInfo l = LearnerInfo.DeserializeFromJson(ls);
                learners.Add(l);
            }
            if (AllLearnerInfoEvent!=null) AllLearnerInfoEvent(this,new AllLearnerInfoEventArgs(learners));
        }

        public void allUtterancesForParticipant(int participantId, string[] Utterance_utterances)
        {
            
        }

        #endregion
    }

    

    public interface ILearnerDbClient : IPerception,
        EmoteCommonMessages.ILearnerModelIdEvents
    { }



}
