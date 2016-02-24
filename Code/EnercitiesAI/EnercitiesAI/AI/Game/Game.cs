using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Planning;
using EnercitiesAI.AI.Simulation;
using EnercitiesAI.AI.States;
using EnercitiesAI.Domain;
using Newtonsoft.Json;
using PS.Utilities;
using PS.Utilities.Serialization;

namespace EnercitiesAI.AI.Game
{

    #region Delegates

    public delegate void StateEventHandler(EnercitiesGameInfo state, EnercitiesGameInfo expectedState);

    public delegate void ActionEventHandler(IPlayerAction action);

    #endregion

    /// <summary>
    ///     Represents the game itself, including the players, the "static" domain info, the "dynamic" state info.
    ///     It is the main interface to make plays by updating the player's AIs, current state, etc.
    /// </summary>
    public class Game : IDisposable
    {
        private const double DEFAULT_STRAT_LEARN_RATE = 0.1; //0.1; //0.3; //0.4;
        private const double DEFAULT_SOCIAL_IMIT_FACTOR = 0.01; //0.05; // 0.2;
        public const string ECO_STRATEGY_JSON = "eco-strategy.json";
        public const string ENV_STRATEGY_JSON = "env-strategy.json";
        public const string MAY_STRATEGY_JSON = "may-strategy.json";
        private GameValuesElement _lastGameValues;
        private EnercitiesRole _lastPlayer;

        public Game(string domainInfoPath)
        {
            //defaults
            this.PlayersOrder = new List<EnercitiesRole>
                                {
                                    EnercitiesRole.Environmentalist,
                                    EnercitiesRole.Economist,
                                    EnercitiesRole.Mayor
                                };
            this.Players = new Dictionary<EnercitiesRole, Player>();
            this.DomainInfoPath = domainInfoPath;
            this.DomainInfo = new DomainInfo();
            this.StrategyLearningRate = DEFAULT_STRAT_LEARN_RATE;
            this.SocialImitationFactor = DEFAULT_SOCIAL_IMIT_FACTOR;
            this.ActionHistory = new ActionHistory();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.State != null) this.State.Dispose();
            if (this.GameStatistics != null) this.GameStatistics.Dispose();
            foreach (var player in this.Players.Values)
                player.Dispose();
            this.Players.Clear();
            if (this.PlayersOrder != null) this.PlayersOrder.Clear();
            this.ActionHistory.Clear();
        }

        #endregion

        #region Events 

        public event StateEventHandler StateUpdated;
        public event EventHandler GameInitiated;
        public event ActionEventHandler ActionExecuted;

        #endregion

        #region Properties

        public double SocialImitationFactor { get; set; }

        public double StrategyLearningRate { get; set; }

        [JsonIgnore]
        public GameStatistics GameStatistics { get; private set; }

        [JsonIgnore]
        public DomainInfo DomainInfo { get; private set; }

        [JsonIgnore]
        public string DomainInfoPath { get; set; }

        [JsonIgnore]
        public State State { get; private set; }

        [JsonIgnore]
        public GameSimulator Simulator { get; private set; }

        [JsonIgnore]
        public ActionPlanner ActionPlanner { get; private set; }

        public Dictionary<EnercitiesRole, Player> Players { get; private set; }

        public ActionHistory ActionHistory { get; private set; }

        public List<EnercitiesRole> PlayersOrder { get; private set; }

        #endregion

        #region Game init methods

        public void Init(
            EnercitiesRole player1Role = EnercitiesRole.Mayor,
            EnercitiesRole player2Role = EnercitiesRole.Environmentalist)
        {
            //loads all domain info from the xml files
            this.DomainInfo.Load(this.DomainInfoPath);

            //create players
            this.CreatePlayer(EnercitiesRole.Economist);
            this.CreatePlayer(EnercitiesRole.Environmentalist);
            this.CreatePlayer(EnercitiesRole.Mayor);

            //initialize statistics collection
            this.GameStatistics = new GameStatistics(this.DomainInfo);
            this.GameStatistics.Init();

            //initialize state and simulator
            this.SetPlayersOrder(player1Role, player2Role);
            this.Simulator = new GameSimulator(this.DomainInfo, this.GameStatistics, this.PlayersOrder)
                             {
                                 State = this.State = this.GetInitialState()
                             };
            this.Simulator.Init();

            //create players' action planner
            this.ActionPlanner = new ActionPlanner(this);
            this.ActionPlanner.Init();
            this.ActionPlanner.PlanningFinished += this.PlanningFinished;

            //raises init event
            if (this.GameInitiated != null)
                this.GameInitiated(this, EventArgs.Empty);
        }

        private State GetInitialState()
        {
            var startValues = this.DomainInfo.Scenario.StartValues;

            //create state with starting world and game info
            var state = new State
                        {
                            Year = startValues.StartYear,
                            GameInfoState = new EnercitiesGameInfo
                                            {
                                                Money = startValues.Money,
                                                Oil = startValues.Oil,
                                                Population = startValues.Population,
                                                EnvironmentScore = startValues.EnvironmentScore,
                                                EconomyScore = startValues.EconomyScore,
                                                WellbeingScore = startValues.WellbeingScore,
                                                Level = 0
                                            }
                        };

            //initialize world state info
            state.StructuresState.Init(this.DomainInfo.WorldGrid);

            return state;
        }

        private void CreatePlayer(EnercitiesRole role)
        {
            if (this.Players.ContainsKey(role)) return;

            //sets default strategy
            var strategy = StrategyExtensions.Deserialize(
                string.Format("Strategies/{0}",
                    role.Equals(EnercitiesRole.Economist)
                        ? ECO_STRATEGY_JSON
                        : role.Equals(EnercitiesRole.Environmentalist)
                            ? ENV_STRATEGY_JSON
                            : MAY_STRATEGY_JSON));
            strategy.Normalize();
            this.Players.Add(role, new Player(role, strategy));
        }

        #endregion

        #region "Event handlers" for EnercitiesAIClient

        public void SetPlayersOrder(EnercitiesRole player1, EnercitiesRole player2)
        {
            this.PlayersOrder.Clear();
            this.PlayersOrder.Add(player1);
            this.PlayersOrder.Add(player2);
            this.PlayersOrder.AddRange(this.Players.Keys.Where(role => !this.PlayersOrder.Contains(role)));

            //sets same player order also in simulator
            if (this.Simulator != null) this.Simulator.PlayersOrder = this.PlayersOrder;
        }

        public GameValuesElement UpdateState(EnercitiesGameInfo gameInfo)
        {
            //updates simulated state
            this.Simulator.UpdateState();

            //level has to be set by simulator as level-up in EnerCities is asynchronous
            gameInfo.Level = this.State.GameInfoState.Level;

            //overrides rest of state based on event from game, gets increment
            var increment = (GameValuesElement) gameInfo;
            increment.Subtract(this._lastGameValues ?? gameInfo);
            this._lastGameValues = this.State.GameInfoState = gameInfo;

            //update statistics
            this.GameStatistics.UpdateGameValuesStats(gameInfo, increment);

            //updates players strategies with normalized values
            var gameValuesStats = this.GameStatistics.GameValuesChangeStats;
            var normIncValues = gameValuesStats.GetNormalizedGameValues(increment);
            var normIncScoresUniformity = gameValuesStats.GetNormalizedScoresUniformity(increment);
            var perceivedStrategy = StrategyExtensions.FromGameValues(normIncValues, normIncScoresUniformity);
            this.UpdatePlayersStrategies(perceivedStrategy);

            //increments move
            this.State.Move++;

            //raises event
            if (this.StateUpdated != null)
                this.StateUpdated(this.State.GameInfoState, gameInfo);

            //adds state to history
            if ((this.ActionHistory.Count == 0) || (this.ActionHistory[this.ActionHistory.Count - 1].State != null))
                this.ActionHistory.Add(new ActionStatePair {State = gameInfo.Clone()});
            else
                this.ActionHistory[this.ActionHistory.Count - 1].State = gameInfo.Clone();

            return increment;
        }

        private void UpdatePlayersStrategies(Strategy lastStrategy)
        {
            //checks increment
            if (lastStrategy.Equals(Strategy.Empty)) return;

            //updates/learns the last player's strategy according to increment in the game's values
            this.Players[this._lastPlayer].Strategy.Average(lastStrategy, this.StrategyLearningRate);

            //updates other player's strategies so to imitate the last strategy
            var otherPlayers = new HashSet<EnercitiesRole>(EnumUtil<EnercitiesRole>.GetTypes());
            otherPlayers.Remove(this._lastPlayer);
            foreach (var otherPlayer in otherPlayers)
                this.Players[otherPlayer].Strategy.Approximate(lastStrategy, this.SocialImitationFactor);

            //updates strategy statistics
            this.GameStatistics.UpdateStrategyStatistics(this.Players);
        }

        public void ExecuteAction(EnercitiesRole playerRole, IPlayerAction action)
        {
            //updates player that performed the action
            this._lastPlayer = playerRole;

            //updates the internal game state
            this.Simulator.ExecuteAction(action);

            //update actions statistics
            this.GameStatistics.UpdateActionStats(playerRole, action);

            //adds action to history
            this.ActionHistory.Add(playerRole, new ActionStatePair {Action = action});

            //raises event
            if (this.ActionExecuted != null)
                this.ActionExecuted(action);
        }

        private void PlanningFinished(object sender, EventArgs e)
        {
            //update planning statistics
            this.GameStatistics.UpdatePlanningStats(this.ActionPlanner);
        }

        #endregion

        #region Serialization methods

        public void SerializeToJson(string filePath)
        {
            this.SerializeJsonFile(filePath, JsonUtil.TypeSpecifySettings);
        }

        public static Game DeserializeFromJson(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<Game>(filePath, JsonUtil.TypeSpecifySettings);
        }

        public bool RecoverFromFile(string filePath)
        {
            var game = DeserializeFromJson(filePath);
            if (game == null) return false;

            //updates all states and actions provided from file
            this.SetPlayersOrder(game.PlayersOrder[0], game.PlayersOrder[1]);
            foreach (var sap in game.ActionHistory)
            {
                if (sap.Action != null) this.ExecuteAction(this.State.GameInfoState.CurrentRole, sap.Action);
                if (sap.State != null) this.UpdateState(sap.State);
            }

            this.Players = game.Players;
            this.SocialImitationFactor = game.SocialImitationFactor;
            this.StrategyLearningRate = game.StrategyLearningRate;

            return true;
        }

        #endregion
    }
}