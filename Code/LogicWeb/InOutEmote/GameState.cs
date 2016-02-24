using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;
using EmoteEvents;
using InOutEmote.Thalamus;
using InOutEmote.Utils;

namespace InOutEmote
{
    public class GameState
    {
        private object _locker = new object();
        private static GameState _instance = null;

        public int Session = 1;
        public bool OKAOPerceptionOccurred { get; set; }

        private TurnStatus _currentState;
        private WordsVariants _wordsVariants;

        public List<EnercitiesActionInfo> BestActionsForMayor { get; set; }

        public TurnStatus CurrentState
        {
            get
            {
                lock (_locker)
                {
                    return _currentState;
                }
            }
            set
            {
                lock (_locker)
                {
                    _currentState = value;
                }
            }
        }


        public static GameState GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GameState();
            }
            return _instance;
        }
        

        public void Initialize()
        {
            var client = InOutThalamusClient.GetInstance();

            client.GameStartedEvent += perceptionClient_GameStartedEvent;
            client.ReachedNewLevelEvent += client_NewLevelEvent;
            client.TurnChangedEvent += client_TurnChangedEvent;
            client.BestActionPlannedEvent += perceptionClient_BestActionsPlannedEvent;
            client.PerformUpgradeEvent += client_PerformUpgradeEvent;
            client.ConfirmConstructionEvent += client_ConfirmConstructionEvent;
            client.ImplementPolicyEvent += client_ImplementPolicyEvent;
            client.ActionsPlannedEvent += client_ActionsPlannedEvent;
            client.LearnerModelMemoryEvent += client_learnerModelMemoryEvent;
            client.StartEvent += client_start;
            client.ClassifierResultEvent += client_ClassifierResultEvent;

            _wordsVariants = new Utils.WordsVariants();

            CurrentState = new TurnStatus();
        }



        /// <summary>
        /// Build a collection of the key and values needed by skene to substitude the tags in the utterances
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTagNamesAndValues()
        {
            lock (_locker)
            {
                if (CurrentState != null)
                {
                    var temp = new Dictionary<string, string>();
                    if (CurrentState != null)
                    {
                        temp.Add("/currentPlayerSide/", CurrentState.GetCurrentPlayerSide());
                        if (CurrentState.CurrentPlayer != null)
                        {
                            temp.Add("/currentPlayerName/", CurrentState.CurrentPlayer.Name);
                            temp.Add("/artigoDefinidoNextPlayer/", GetNextPlayer().Gender == Gender.Male ? "a" : "o");
                            temp.Add("/currentPlayerRole/", CurrentState.CurrentPlayer.Role.ToString());
                            temp.Add("/artigoDefinidoCurrentPlayer/",
                                CurrentState.CurrentPlayer.Gender == Gender.Male ? "a" : "o");
                        }
                        if (CurrentState.PlayedStructure != null)
                        {
                            temp.Add("/structure/", CurrentState.PlayedStructure.Translation);
                        }
                        if (CurrentState.PlayedPolicy != null)
                        {
                            temp.Add("/policy/", CurrentState.PlayedPolicy.Translation);
                        }
                        if (CurrentState.GameScores != null)
                        {
                            temp.Add("/currentLevel/", CurrentState.CurrentLevel + "");
                        }
                        if (CurrentState.PlayedUpgrade != null)
                        {
                            temp.Add("/upgrade/", CurrentState.PlayedUpgrade.Translation);
                        }
                        var eco = GetEco();
                        if (eco != null)
                        {
                            temp.Add("/artigoDefinidoEco/", eco.Gender == Gender.Male ? "a" : "o");
                            temp.Add("/econame/", eco.Name);
                        }
                        var env = GetEnv();
                        if (env != null)
                        {
                            temp.Add("/artigoDefinidoEnv/", env.Gender == Gender.Male ? "a" : "o");
                            temp.Add("/envname/", env.Name);
                        }
                    }
                    //_wordsVariants.AddWordsVariants(ref temp);
                    return temp;
                }
                throw new Exception("No game state received yet!");
            }
        }

      
        
        #region Helpers

        public Player GetNextPlayer()
        {
            switch (CurrentState.CurrentPlayer.Role)
            {
                case EnercitiesRole.Environmentalist:
                    return GetEco();
                case EnercitiesRole.Economist:
                    return CurrentState.PlayerAI;
                case EnercitiesRole.Mayor:
                    return GetEnv();
                default:
                    return CurrentState.PlayerAI;
            }
        }

        public Player GetEco()
        {
            return CurrentState.Player1.Role == EnercitiesRole.Economist ? CurrentState.Player1 : CurrentState.Player2;
        }

        public Player GetEnv()
        {
            return CurrentState.Player1.Role == EnercitiesRole.Environmentalist
                ? CurrentState.Player1
                : CurrentState.Player2;
        }


        public enum AiActionStrategy
        {
            Economy,
            Environment,
            Population,
            Money,
            Oil,
            Power,
            Wellbeing,
            None
        }

        public AiActionStrategy GetMainStrategy()
        {
            var strategy = CurrentState.StrategiesUsedForLastBestAction;
            if (strategy == null) return AiActionStrategy.None;

            var list = new List<double>
                       {
                           strategy.EconomyWeight,
                           strategy.EnvironmentWeight,
                           strategy.HomesWeight,
                           strategy.MoneyWeight,
                           strategy.OilWeight,
                           strategy.PowerWeight,
                           strategy.WellbeingWeight
                       };
            int indexMax
                = !list.Any()
                    ? -1
                    : list
                        .Select((value, index) => new { Value = value, Index = index })
                        .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                        .Index;
            return (AiActionStrategy)indexMax;
        }

        #endregion

        #region Game Events Management

        void client_ClassifierResultEvent(object sender, ClassificationEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.ClassifierResult = e.Label;
            }
        }

        public void Reset()
        {
            lock (_locker)
            {
                CurrentState = new TurnStatus();
            }
        }

        public void client_start(object sender, StartEventArgs e)
        {
            lock (_locker)
            {
                var player1 = e.StartMessageInfo.Students[0];
                var player2 = e.StartMessageInfo.Students[1];
                var p1 = new Player(player1.firstName, EnercitiesRole.Environmentalist,
                    player1.sex.ToLower().Equals("m") ? Gender.Male : Gender.Female);
                var p2 = new Player(player2.firstName, EnercitiesRole.Environmentalist,
                    player2.sex.ToLower().Equals("m") ? Gender.Male : Gender.Female);
                if (CurrentState == null)
                {
                    CurrentState = new TurnStatus()
                    {
                        Player1 = p1,
                        Player2 = p2
                    };
                }
                else
                {
                    CurrentState.Player1 = p1;
                    CurrentState.Player2 = p2;
                }

                Session = e.StartMessageInfo.SessionId;
            }
        }

        public void perceptionClient_GameStartedEvent(object sender, GenericGameEventArgs e)
        {
            lock (_locker)
            {
                CurrentState = new TurnStatus();
                CurrentState.Player1 = new Player(e.Player1Name,
                    (EnercitiesRole)Enum.Parse(typeof(EnercitiesRole), e.Player1Role));
                CurrentState.Player2 = new Player(e.Player2Name,
                    (EnercitiesRole)Enum.Parse(typeof(EnercitiesRole), e.Player2Role));
                CurrentState.PlayerAI = new Player("", EnercitiesRole.Mayor);

                string currentPlayerName = "";
                EnercitiesRole role = EnercitiesRole.Mayor;
                if (e.SerializedGameState != null)
                {
                    var gameInfo = EnercitiesGameInfo.DeserializeFromJson(e.SerializedGameState);
                    if (gameInfo.CurrentRole == CurrentState.Player1.Role)
                        currentPlayerName = CurrentState.Player1.Name;
                    if (gameInfo.CurrentRole == CurrentState.Player2.Role)
                        currentPlayerName = CurrentState.Player2.Name;
                    role = gameInfo.CurrentRole;
                }
                CurrentState.CurrentPlayer = new Player(currentPlayerName, role);
            }
        }

        public void client_NewLevelEvent(object sender, ReachedNewLevelEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.GameScores.Level = e.Level;
                CurrentState.CurrentLevel = e.Level;
            }
        }

        public void client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.PlayedPolicy = null;
                CurrentState.PlayedStructure = null;
                CurrentState.PlayedUpgrade = null;
                CurrentState.GameScores = e.GameState;
                CurrentState.TurnNumber++;
                if (e.GameState != null)
                {
                    switch (e.GameState.CurrentRole)
                    {
                        case EnercitiesRole.Mayor:
                            CurrentState.CurrentPlayer = CurrentState.PlayerAI;
                            break;
                        case EnercitiesRole.Environmentalist:
                            CurrentState.CurrentPlayer = GetEnv();
                            break;
                        case EnercitiesRole.Economist:
                            CurrentState.CurrentPlayer = GetEco();
                            break;
                    }
                }
            }
        }

        public void perceptionClient_BestActionsPlannedEvent(object sender, IAEventArgs e)
        {
            lock (_locker)
            {
                //The system may receive actions related to past turns
                if (CurrentState.CurrentPlayer.Role == EnercitiesRole.Mayor)
                {
                    var actions = new List<EnercitiesActionInfo>();
                    foreach (string s in e.EnercitiesActionInfo_bestActionInfos)
                    {
                        actions.Add(EnercitiesActionInfo.DeserializeFromJson(s));
                    }
                    BestActionsForMayor = actions;
                }
            }
        }

        public void client_PerformUpgradeEvent(object sender, GameActionEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.PlayedUpgrade = new TranslatebleEnum(e.ActionTypeEnum, e.Translation, typeof(UpgradeType));
            }
        }

        public void client_ConfirmConstructionEvent(object sender, GameActionEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.PlayedStructure = new TranslatebleEnum(e.ActionTypeEnum, e.Translation,
                    typeof(StructureType));
            }
        }

        public void client_ImplementPolicyEvent(object sender, GameActionEventArgs e)
        {
            lock (_locker)
            {
                CurrentState.PlayedPolicy = new TranslatebleEnum(e.ActionTypeEnum, e.Translation, typeof(PolicyType));
            }
        }

        public void client_ActionsPlannedEvent(object sender, IAEventArgs iaEventArgs)
        {
            lock (_locker)
            {
                try
                {
                    CurrentState.StrategiesUsedForLastBestAction =
                        Strategy.DeserializeFromJson(iaEventArgs.Strategy_planStrategy);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception deserializing actions planned message: " + ex.Message);
                }
            }
        }

        public void client_learnerModelMemoryEvent(object sender, LearnerModelMemoryEventArgs e)
        {
            lock (_locker)
            {
                var memoryEventData = e.MemoryEvent;
                if (CurrentState != null)
                    CurrentState.MemoryEventData = memoryEventData;
            }
        }



        #endregion
    }

    public class TurnStatus : ICloneable
    {
        public TurnStatus()
        {
            TurnNumber = 0;
            PositiveEmotionalClimate = true;
            Player1 = new Player("", EnercitiesRole.Environmentalist);
            Player2 = new Player("", EnercitiesRole.Economist);
            PlayerAI = new Player("", EnercitiesRole.Mayor);
            CurrentPlayer = PlayerAI;
        }

        public double NormalizedPopulation { get; set; }

        public double NormalizedPower { get; set; }

        public double NormalizedOil { get; set; }

        public double NormalizedMoney { get; set; }

        public double NormalizedResourcesAverage { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public Player PlayerAI { get; set; }

        public Player CurrentPlayer { get; set; }

        public TranslatebleEnum PlayedPolicy { get; set; }

        public TranslatebleEnum PlayedStructure { get; set; }

        public TranslatebleEnum PlayedUpgrade { get; set; }

        public EnercitiesGameInfo GameScores { get; set; }

        public int TurnNumber { get; set; }

        public Strategy StrategiesUsedForLastBestAction { get; set; }

        public MemoryEvent MemoryEventData { get; set; }

        public int CurrentLevel { get; set; }

        public bool PositiveEmotionalClimate { get; set; }

        public string ClassifierResult { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        public string GetCurrentPlayerSide()
        {
            if (CurrentPlayer != null && !CurrentPlayer.IsAI())
            {
                if (CurrentPlayer.Role == EnercitiesRole.Environmentalist) return "Left";
                return "Right";
            }
            return null;
        }
    }

    /// <summary>
    ///     Stores an enumerator as integer along with its translation
    ///     Eg.: Value=StructureType.Urban, Translation="Casas", Type=StructureType
    /// </summary>
    public class TranslatebleEnum
    {
        public TranslatebleEnum(int enumValue, string translation, Type type)
        {
            this.Value = enumValue;
            this.Translation = translation;
            this.Type = type;
        }

        public int Value { get; set; }

        public string Translation { get; set; }

        public Type Type { get; set; }
    }

    public class Player
    {
        public Gender Gender;
        public string Name;
        public EnercitiesRole Role;

        public Player(string name, EnercitiesRole role, Gender gender = Gender.Male)
        {
            this.Name = name;
            this.Role = role;
            this.Gender = gender;
        }

        public bool IsAI()
        {
            return Role == EnercitiesRole.Mayor;
        }

        public override string ToString()
        {
            return string.Format("'{0}':{1}", Name, Role);
        }
    }
}
