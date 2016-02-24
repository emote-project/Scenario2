using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EmoteEvents;
using Thalamus;
using Environment = System.Environment;

namespace ControlPanel.Thalamus
{
    public class ControlPanelThalamusClient : ThalamusClient, IControlPanelClient
    {
        private static ControlPanelThalamusClient _instance;

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
        public event EventHandler EnercitiesGameEnded;

        private readonly IControlPanelThalamusPublisher _publisher;
        public IControlPanelThalamusPublisher LDBPublisher
        {
            get { return _publisher; }
        }

        public static ControlPanelThalamusClient GetInstance()
        {
            string[] args = Environment.GetCommandLineArgs();
            string charName = "";
            if (args.Length > 1) charName = args[1];

            if (_instance==null) _instance = new ControlPanelThalamusClient(charName);
            return _instance;
        }

        private ControlPanelThalamusClient(string characterName) : base("EmoteControlPanel", characterName)
        {
            SetPublisher<IControlPanelThalamusPublisher>();
            _publisher = new ControlPanelThalamusPublisher(this.Publisher);
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

        public void PlayersGender(Gender player1Gender, Gender player2Gender)
        {
        }

        public void PlayersGenderString(string player1Gender, string player2Gender)
        {
        }

        public void GameStarted(string player1Name, string player1Role, string player2Name, string player2Role)
        {
        }

        public void ResumeGame(string player1Name, string player1Role, string player2Name, string player2Role,
            string serializedGameState)
        {
        }

        public void EndGameSuccessfull(int totalScore)
        {
            if (EnercitiesGameEnded != null) EnercitiesGameEnded(this,null);
        }

        public void EndGameNoOil(int totalScore)
        {
            if (EnercitiesGameEnded != null) EnercitiesGameEnded(this, null);
        }

        public void EndGameTimeOut(int totalScore)
        {
            if (EnercitiesGameEnded != null) EnercitiesGameEnded(this, null);
        }

        public void TurnChanged(string serializedGameState)
        {
        }

        public void ReachedNewLevel(int level)
        {
        }

        public void StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove)
        {
        }

        #endregion

    }

    

    public interface IControlPanelClient : IPerception,
        EmoteCommonMessages.ILearnerModelIdEvents,
        EmoteEnercitiesMessages.IEnercitiesGameStateEvents
    { }



}
