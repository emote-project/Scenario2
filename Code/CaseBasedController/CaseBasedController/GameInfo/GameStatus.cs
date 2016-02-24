using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CaseBasedController.Thalamus;
using Classification.Classifier;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EmotionalClimateClassification;

namespace CaseBasedController.GameInfo
{
    public enum EmotionalClimate
    {
        Positive,
        Negative,
        Neutral
    }

    /// <summary>
    ///     Records all the game events and scores during the several turns of the game
    /// </summary>
    public static class GameStatus
    {
        private const double POWER_STRAT_ADJUST_PARAM = 0.7;
        private const double MONEY_STRAT_ADJUST_PARAM = 0.9; //0.96;
        private const double OIL_STRAT_ADJUST_PARAM = 0.999; //0.996;
        private const double HOMES_STRAT_ADJUST_PARAM = 0.3; //0.05; //0.0002;

        private static readonly Dictionary<int, uint> PopulationPerLevel =
            new Dictionary<int, uint> {{1, 15}, {2, 45}, {3, 100}, {4, 200}};

        public static List<TurnStatus> States = new List<TurnStatus>();
        public static int Session = 1;

        public static readonly OkaoPerceptionFilter RightSubjOkaoFilter = new OkaoPerceptionFilter();
        public static readonly OkaoPerceptionFilter LeftSubjOkaoFilter = new OkaoPerceptionFilter();
        public static EmotionalClimate EmotionalClimate { get; set; }
        public static bool OKAOPerceptionOccurred { get; set; }

        public static TurnStatus CurrentState
        {
            get
            {
                if (States.Count > 0)
                    return States[States.Count - 1];
                // Ask for information about the game and try rebuild the state queue
                return null;
            }
        }

        public static void Reset()
        {
            States.Clear();
        }

        public static int TurnNumber { get; set; }

        /// <summary>
        ///     Build a collection of the key and values needed by skene to substitude the tags in the utterances
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetTagNamesAndValues()
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
                return temp;
            }
            //throw new Exception("No game state received yet!");
            return new Dictionary<string, string>();
        }

        public static List<WekaClassifier> ECClassifiers;


        #region Helpers

        public static Player GetNextPlayer()
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

        public static Player GetEco()
        {
            return CurrentState.Player1.Role == EnercitiesRole.Economist ? CurrentState.Player1 : CurrentState.Player2;
        }

        public static Player GetEnv()
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

        public static AiActionStrategy GetMainStrategy()
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
                        .Select((value, index) => new {Value = value, Index = index})
                        .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                        .Index;
            return (AiActionStrategy) indexMax;
        }

        #endregion

        #region Game Events Management

        public static void perceptionClient_GameStartedEvent(object sender, GenericGameEventArgs e)
        {
            States.Clear();
            States.Add(new TurnStatus());
            CurrentState.Player1 = new Player(e.Player1Name,
                (EnercitiesRole) Enum.Parse(typeof (EnercitiesRole), e.Player1Role));
            CurrentState.Player2 = new Player(e.Player2Name,
                (EnercitiesRole) Enum.Parse(typeof (EnercitiesRole), e.Player2Role));
            CurrentState.PlayerAI = new Player("", EnercitiesRole.Mayor);

            string currentPlayerName = "";
            EnercitiesRole role = EnercitiesRole.Mayor;
            if (e.SerializedGameState != null)
            {
                var gameInfo = EnercitiesGameInfo.DeserializeFromJson(e.SerializedGameState);
                if (gameInfo.CurrentRole == CurrentState.Player1.Role) currentPlayerName = CurrentState.Player1.Name;
                if (gameInfo.CurrentRole == CurrentState.Player2.Role) currentPlayerName = CurrentState.Player2.Name;
                role = gameInfo.CurrentRole;
            }
            CurrentState.CurrentPlayer = new Player(currentPlayerName, role);
        }

        public static void client_NewLevelEvent(object sender, ReachedNewLevelEventArgs e)
        {
            CurrentState.GameScores.Level = e.Level;
            CurrentState.CurrentLevel = e.Level;
        }

        public static void client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            TurnNumber++;
            if (States.Count > 0)
            {
                States.Add((TurnStatus) CurrentState.Clone());
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
                NormalizeResourceValues(e.GameState);
            }
            else
            {
                MessageBox.Show(
                    "No game start received! Check you started the game after you started this application or check that the first log row contains a GameStarted event");
            }
        }


        private static void NormalizeResourceValues(EnercitiesGameInfo gameState)
        {
            CurrentState.NormalizedPower = Math.Pow(POWER_STRAT_ADJUST_PARAM,
                gameState.PowerProduction - Math.Abs(gameState.PowerConsumption));
            CurrentState.NormalizedMoney = Math.Pow(MONEY_STRAT_ADJUST_PARAM, gameState.Money);
            CurrentState.NormalizedOil = Math.Pow(OIL_STRAT_ADJUST_PARAM, gameState.Oil);

            var homesNeedRatio = ((double) gameState.Population/PopulationPerLevel[gameState.Level]);
            CurrentState.NormalizedPopulation = Math.Pow(HOMES_STRAT_ADJUST_PARAM, homesNeedRatio);

            CurrentState.NormalizedResourcesAverage =
                (CurrentState.NormalizedPower + CurrentState.NormalizedOil + CurrentState.NormalizedMoney +
                 CurrentState.NormalizedPopulation)/4d;
        }

        public static void perceptionClient_BestActionsPlannedEvent(object sender, IAEventArgs e)
        {
            //The system may receive actions related to past turns
            if (CurrentState.CurrentPlayer.Role.Equals(e.CurrentPlayer))
            {
                var actions = new List<EnercitiesActionInfo>();
                foreach (string s in e.EnercitiesActionInfo_bestActionInfos)
                {
                    actions.Add(EnercitiesActionInfo.DeserializeFromJson(s));
                }
                CurrentState.BestActionsForThisTurn = actions;
            }
        }

        public static void client_PerformUpgradeEvent(object sender, GameActionEventArgs e)
        {
            CurrentState.PlayedUpgrade = new TranslatebleEnum(e.ActionTypeEnum, e.Translation, typeof (UpgradeType));
        }

        public static void client_ConfirmConstructionEvent(object sender, GameActionEventArgs e)
        {
            CurrentState.PlayedStructure = new TranslatebleEnum(e.ActionTypeEnum, e.Translation, typeof (StructureType));
        }

        public static void client_ImplementPolicyEvent(object sender, GameActionEventArgs e)
        {
            CurrentState.PlayedPolicy = new TranslatebleEnum(e.ActionTypeEnum, e.Translation, typeof (PolicyType));
        }

        public static void client_ActionsPlannedEvent(object sender, IAEventArgs iaEventArgs)
        {
            CurrentState.StrategiesUsedForLastBestAction =
                Strategy.DeserializeFromJson(iaEventArgs.Strategy_planStrategy);
        }

        public static void client_learnerModelMemoryEvent(MemoryEvent memoryEventData)
        {
            if (CurrentState!=null)
                CurrentState.MemoryEventData = memoryEventData;
        }

        public static void client_start(StartEventArgs startEventArgs)
        {
            if (CurrentState == null)
            {
                var player1 = startEventArgs.StartMessageInfo.Students[0];
                var player2 = startEventArgs.StartMessageInfo.Students[1];
                States = new List<TurnStatus>() { new TurnStatus()
                {
                    Player1 = new Player(player1.firstName, EnercitiesRole.Environmentalist, player1.sex.ToLower().Equals("m")?Gender.Male : Gender.Female),
                    Player2 = new Player(player2.firstName, EnercitiesRole.Environmentalist, player2.sex.ToLower().Equals("m")?Gender.Male : Gender.Female),
                }};
                Session = startEventArgs.StartMessageInfo.SessionId;
            }
        }

        #endregion
    }
}