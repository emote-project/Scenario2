using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.Domain;
using EnercitiesAI.Programs;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Utilities;
using PS.Utilities.IO;
using Thalamus;

namespace EnercitiesAI
{
    public interface IEnercitiesAIClient : IEnercitiesGameStateEvents, IEnercitiesTaskEvents, IEnercitiesAIPerceptions
    {
    }

    public class EnercitiesAIClient : ThalamusClient, IEnercitiesAIClient
    {
        public const string XML_BASE_PATH = @".\EnercitiesData\Level\";
        public const string CLIENT_NAME = "EnercitiesAI";
        public const string DEFAULT_CHARACTER_NAME = "";
        public const string GAME_LOG_FILE_NAME = "game.json";
        public const string GAME_LOG_DIR_NAME = "GameLogs";
        private readonly List<UpgradeStructure> _pendingUpgrades = new List<UpgradeStructure>();
        private readonly IEnercitiesAIPublisher _thalamusPublisher;
        private EnercitiesRole _currentRole;
        private bool _gameInSession;
        private IPlayerAction _lastReceivedAction;
        private EnercitiesGameInfo _lastReceivedGameInfo;

        public EnercitiesAIClient(string character = DEFAULT_CHARACTER_NAME)
            : base(CLIENT_NAME, character)
        {
            //just create game with defaults
            this.Game = new Game(XML_BASE_PATH);

            this.SetPublisher<IEnercitiesAIPublisher>();
            this._thalamusPublisher = new EnercitiesAIPublisher(this.Publisher);

            this.InitGameSession(EnercitiesRole.Economist, EnercitiesRole.Environmentalist);
        }

        public Game Game { get; private set; }

        public void Reset()
        {
            this._lastReceivedAction = null;
            this._lastReceivedGameInfo = null;
            this._gameInSession = false;
            this._pendingUpgrades.Clear();
            this.Game.Dispose();
        }

        public void InitGameSession(EnercitiesRole player1Role, EnercitiesRole player2Role)
        {
            lock (this.Game)
            {
                //check previous session, save action history file and dispose
                if (this._gameInSession) this.ClientClosed();
                this.Reset();

                //create game and sets players' order
                this.Game.Init(player1Role, player2Role);
            }
        }

        public bool RecoverGameSession()
        {
            //inits game
            this.InitGameSession(EnercitiesRole.Economist, EnercitiesRole.Environmentalist);

            // tries to recover game from file (for when ai client crashes but enercities does not)
            return this.Game.RecoverFromFile(GAME_LOG_FILE_NAME);
        }

        public void ReplayActionHistory(List<ActionStatePair> playHistory)
        {
            //replays all actions (in Enercities) stored in action history
            foreach (var actionStatePair in playHistory)
                this.ExecuteActionDirectly(actionStatePair.Action);
        }

        #region AI perception event handling

        void IEnercitiesGameStateEvents.GameStarted(
            string player1Name, string player1Role, string player2Name, string player2Role)
        {
            lock (this.Game)
            {
                this.InitGameSession(EnumUtil<EnercitiesRole>.GetType(player1Role),
                    EnumUtil<EnercitiesRole>.GetType(player2Role));
                this._gameInSession = true;
            }
        }

        void IEnercitiesGameStateEvents.ResumeGame(
            string player1Name, string player1Role, string player2Name, string player2Role, string serializedGameState)
        {
        }

        void IEnercitiesGameStateEvents.TurnChanged(string serializedGameState)
        {
            lock (this.Game)
            {
                //gets game info state
                var gameInfo = EnercitiesGameInfo.DeserializeFromJson(serializedGameState);
                gameInfo.Level--;

                //checks last state received to ignore duplicates
                if ((this._lastReceivedGameInfo != null) && this._lastReceivedGameInfo.Equals(gameInfo))
                    return;

                //checks pending upgrades, perform those first
                if (this._pendingUpgrades.Count != 0)
                {
                    var upgrades = new UpgradeStructures(new List<UpgradeStructure>(this._pendingUpgrades));
                    this.Game.ExecuteAction(this._currentRole, upgrades);
                    this._pendingUpgrades.Clear();
                }

                //updates game state
                this.Game.UpdateState(gameInfo);
                this._currentRole = gameInfo.CurrentRole;
                this._lastReceivedGameInfo = gameInfo;

                //publish perceived strategy events 
                this.PublishStrategyEvents();

                //updates AI by planning next action / calculating predictions
                this.Game.ActionPlanner.PlanNextAction();

                //checks results
                if (!this.Game.ActionPlanner.HasValidResults) return;

                //publish action and predicted values events
                this.PublishActionEvents();
                this.PublishPredictedValuesEvents();

                //saves game temp log file
                this.Game.SerializeToJson(GAME_LOG_FILE_NAME);
            }
        }

        void IEnercitiesTaskEvents.ConfirmConstruction(StructureType structure, string translation, int cellX, int cellY)
        {
            lock (this.Game)
                this.ActionExecuted(new BuildStructure(structure) {X = cellX, Y = cellY});
        }

        void IEnercitiesTaskEvents.ImplementPolicy(PolicyType policy, string translation)
        {
            lock (this.Game)
                this.ActionExecuted(new ImplementPolicy(policy));
        }

        void IEnercitiesTaskEvents.PerformUpgrade(UpgradeType upgrade, string translation, int cellX, int cellY)
        {
            lock (this.Game)
                this.ActionExecuted(new UpgradeStructure(upgrade) {X = cellX, Y = cellY});
        }

        void IEnercitiesTaskEvents.SkipTurn()
        {
            lock (this.Game)
                this.ActionExecuted(new SkipTurn());
        }

        void IEnercitiesGameStateEvents.ReachedNewLevel(int level)
        {
            //just correct game state
            lock (this.Game)
                this.Game.State.GameInfoState.Level = level - 1;
        }

        void IEnercitiesAIPerceptions.UpdateStrategies(string StrategiesSet_strategies)
        {
            var strategiesSet = StrategiesSet.DeserializeFromJson(StrategiesSet_strategies);
            if (strategiesSet == null)
                throw new Exception("Can't update strategies. Strategies set is null");

            var strategies = strategiesSet.Strategies;
            lock (this.Game)
            {
                //changes player's strategies
                foreach (var strategy in strategies)
                    strategy.Value.CopyTo(this.Game.Players[strategy.Key].Strategy.Weights, strategy.Value.Length);

                //updates AI by planning next action / calculating predictions
                this.Game.ActionPlanner.PlanNextAction();
            }

            //checks results
            if (!this.Game.ActionPlanner.HasValidResults) return;

            //publish action and predicted values events
            this.PublishActionEvents();
            this.PublishPredictedValuesEvents();
        }

        private void ActionExecuted(IPlayerAction action)
        {
            lock (this.Game)
            {
                //checks action for duplicates, ignore
                if ((this._lastReceivedAction != null) && this._lastReceivedAction.Equals(action))
                    return;

                if (action is UpgradeStructure)
                {
                    //checks for upgrade, add to pending list
                    this._pendingUpgrades.Add((UpgradeStructure) action);
                }
                else
                {
                    //reports action to game manager directly
                    this.Game.ExecuteAction(this._currentRole, action);
                }

                this._lastReceivedAction = action;

                //saves game temp log file
                this.Game.SerializeToJson(GAME_LOG_FILE_NAME);
            }
        }

        #endregion

        #region AI action publishing

        private void PublishActionEvents()
        {
            //raises next actions planned for current player
            lock (this.Game)
            {
                var actionPlanner = this.Game.ActionPlanner;
                this._thalamusPublisher.BestActionPlanned(GetActionInfos(actionPlanner.LastBestAction));
                this._thalamusPublisher.ActionsPlanned(
                    actionPlanner.LastPlayer.Role, actionPlanner.LastPlayerStrategy.SerializeToJson(),
                    GetActionInfos(actionPlanner.LastBestActions.Select(actionValue => actionValue.Key)),
                    GetActionInfos(actionPlanner.LastWorstActions.Select(actionValue => actionValue.Key)));
            }
        }

        private static string[] GetActionInfos(IPlayerAction action)
        {
            var actionInfos = new string[3];
            if (action is UpgradeStructures)
            {
                //only if action is Upgrade we use all 3 actions
                var i = 0;
                foreach (var upgradeStructure in ((UpgradeStructures) action).Upgrades)
                    actionInfos[i++] = upgradeStructure.ToEnercitiesActionInfo().SerializeToJson();
            }
            else
            {
                //otherwise just convert to Json
                actionInfos[0] = action.ToEnercitiesActionInfo().SerializeToJson();
            }
            return actionInfos;
        }

        private static string[] GetActionInfos(IEnumerable<IPlayerAction> actions)
        {
            //transforms all actions infos into a single array 
            var actionInfoList = new List<string>();
            foreach (var action in actions)
                actionInfoList.AddRange(GetActionInfos(action));
            return actionInfoList.ToArray();
        }

        private void PublishPredictedValuesEvents()
        {
            //raises predicted values event, to use for alerts raising
            var predictedGameValues = this.Game.ActionPlanner.PredictedGameValues;
            var allPredictedValues = new double[Strategy.NUM_WEIGHTS + 3];
            Array.Copy(((DenseVector) predictedGameValues).Values, allPredictedValues, Strategy.NUM_WEIGHTS);
            allPredictedValues[8] = predictedGameValues.ScoresUniformity;
            allPredictedValues[9] = this.Game.ActionPlanner.NoPlayProbability;
            allPredictedValues[10] = this.Game.ActionPlanner.NoSpaceProbability;
            this._thalamusPublisher.PredictedValuesUpdated(allPredictedValues);
        }

        private void PublishStrategyEvents()
        {
            //raises strategy update event
            var strategies = this.Game.Players.ToDictionary(
                player => player.Key, player => player.Value.Strategy.Weights);
            this._thalamusPublisher.StrategiesUpdated(new StrategiesSet(strategies).SerializeToJson());
        }

        /// <summary>
        ///     Used internally to test AI generated actions, ie does not go through GamePlayManager or WoZ
        /// </summary>
        internal void AutoPlayBestAction()
        {
            //updates AI by planning next action / calculating predictions
            this.Game.ActionPlanner.PlanNextAction();
            this.ExecuteBestActionDirectly();
        }

        /// <summary>
        ///     Used internally to test AI generated actions, ie does not go through GamePlayManager or WoZ
        /// </summary>
        internal void ExecuteBestActionDirectly()
        {
            IPlayerAction bestAction;
            lock (this.Game)
                bestAction = this.Game.ActionPlanner.LastBestAction;

            this.ExecuteActionDirectly(bestAction);
        }

        /// <summary>
        ///     Used internally to test AI generated actions, ie does not go through GamePlayManager or WoZ
        /// </summary>
        internal void ExecuteActionDirectly(IPlayerAction action)
        {
            //checks action
            if (action == null) return;

            //checks action type and publishes accordingly
            if (action is SkipTurn)
            {
                this._thalamusPublisher.SkipTurn();
            }
            else if (action is ImplementPolicy)
            {
                this._thalamusPublisher.ImplementPolicy(((ImplementPolicy) action).PolicyType);
            }
            else if (action is BuildStructure)
            {
                var buildAction = (BuildStructure) action;
                this._thalamusPublisher.ConfirmConstruction(buildAction.StructureType, buildAction.X, buildAction.Y);
            }
            else if (action is UpgradeStructures)
            {
                var upgradesAction = (UpgradeStructures) action;
                foreach (var upgrade in upgradesAction.Upgrades)
                    this._thalamusPublisher.PerformUpgrade(upgrade.UpgradeType, upgrade.X, upgrade.Y);

                //*** checks the need to send extra SkipTurn actions ***
                if (upgradesAction.Upgrades.Count < DomainInfo.MAX_UPGRADES_ACTION)
                    for (var i = 0; i < DomainInfo.MAX_UPGRADES_ACTION - upgradesAction.Upgrades.Count; i++)
                        this._thalamusPublisher.SkipTurn();
            }
        }

        #endregion

        #region Console shutdown handling

        public bool ConsoleClosed(SigType sig)
        {
            switch (sig)
            {
                case SigType.CtrlCEvent:
                case SigType.LogoffEvent:
                case SigType.ShutdownEvent:
                case SigType.CloseEvent:
                    this.ClientClosed();
                    return true;
                default:
                    return false;
            }
        }

        private void ClientClosed()
        {
            //assuming normal shutdown, print game session results and moves history log file
            lock (this.Game)
            {
                var resultsPath = Path.GetFullPath(
                    string.Format("{0}/{1}", GAME_LOG_DIR_NAME, DateTime.Now.ToString("yy-MM-dd@HH-mm-ss")));

                if (!File.Exists(GAME_LOG_FILE_NAME)) return;

                PathUtil.CreateOrClearDirectory(resultsPath);
                File.Move(GAME_LOG_FILE_NAME, string.Format("{0}/{1}", resultsPath, GAME_LOG_FILE_NAME));

                this.Game.GameStatistics.PrintResults(resultsPath);
            }
        }

        #endregion

        #region Unused Methods

        void IEnercitiesGameStateEvents.PlayersGender(Gender player1Gender, Gender player2Gender)
        {
        }

        void IEnercitiesGameStateEvents.StrategyGameMoves(
            string environmentalistMove, string economistMove, string mayorMove, string globalMove)
        {
        }

        void IEnercitiesGameStateEvents.PlayersGenderString(string player1Gender, string player2Gender)
        {
        }

        void IEnercitiesGameStateEvents.EndGameSuccessfull(int totalScore)
        {
        }

        void IEnercitiesGameStateEvents.EndGameNoOil(int totalScore)
        {
        }

        void IEnercitiesGameStateEvents.EndGameTimeOut(int totalScore)
        {
        }

        public void ExamineCell(double screenX, double screenY, int cellX, int cellY,
            StructureType StructureType_structure, string StructureType_translated)
        {
        }

        #endregion
    }
}