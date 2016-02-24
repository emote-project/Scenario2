using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.AI.Simulation;
using EnercitiesAI.Domain;
using PS.Utilities;

namespace EnercitiesAI.AI.Planning
{
    public enum PlanningState
    {
        Running,
        Stopped,
        Interrupting
    }

    public class ActionPlanner : IDisposable
    {
        #region Constructor

        public ActionPlanner(Game.Game game)
        {
            this._game = game;

            //default planning depth: 1 round for each player
            this.MaxPlanningDepth = game.Players.Count;
            this.MaxCPUs = ProcessUtil.GetCPUCount();
            this.AutoGarbageCollect = true;

            //set other param defaults
            this.MaxProcessTime = DEF_MAX_TIME;
            this.NumBestActions = DEF_NUM_ACTIONS;
            this.DepthPenalty = DEF_DEPTH_PEN;
            this.SkipTurnPen = DEF_SKIP_TURN_PEN;
            this.HistoryPenalty = DEF_HISTORY_PEN;
            this.HistoryPenaltySmoothness = DEF_HISTORY_PEN_SMOOTH;
            this.StrategyAdjustmentParam = DEF_STRAT_ADJUST;
        }

        #endregion

        #region Constants

        private const int DEF_NUM_ACTIONS = 3;
        private const int DEF_MAX_TIME = 5;
        private const double DEF_SKIP_TURN_PEN = 0.3; //0.1;
        private const double DEF_DEPTH_PEN = 0.9;
        private const double DEF_HISTORY_PEN_SMOOTH = 0.6;
        private const double DEF_HISTORY_PEN = 0.2;
        public const double DEF_STRAT_ADJUST = 0.2;

        #endregion

        #region Fields

        private readonly Game.Game _game;
        private readonly int[] _initialBuildableCoords = new int[DomainInfo.MAX_LEVELS];
        private readonly object _locker = new object();
        private readonly List<Thread> _planningThreads = new List<Thread>();

        private Dictionary<EnercitiesRole, Strategy> _playersStrategies;
        private GameValuesElement _startGameValues;
        private DateTime _startPlanTime;
        private int _startPlayerActionIdx;
        private SortedActionValuesList _startPlayerActionValues;

        #endregion

        #region Events

        public event EventHandler PlanningFinished;
        public event EventHandler PlanningStarted;

        #endregion

        #region Public properties

        /// <summary>
        ///     The maximum depth of planning, where depth refers to the number of simulated playing turns.
        /// </summary>
        public int MaxPlanningDepth { get; set; }

        /// <summary>
        ///     The maximum time, in seconds, that the planner has available to find the best action.
        /// </summary>
        public int MaxProcessTime { get; set; }

        /// <summary>
        ///     The maximum number of (virtual) CPUs to use for planning.
        /// </summary>
        public uint MaxCPUs { get; set; }

        /// <summary>
        ///     Whether to call the garbage collector after planning.
        /// </summary>
        public bool AutoGarbageCollect { get; set; }

        /// <summary>
        ///     The maximum number of actions to return in LastBestActions.
        /// </summary>
        public int NumBestActions { get; set; }

        /// <summary>
        ///     The time, in seconds, it took the last planning to get the best actions.
        /// </summary>
        public double LastTimeTaken { get; private set; }

        /// <summary>
        ///     The number of states visited during the last action planning.
        /// </summary>
        public ulong LastNumVisitedStates { get; private set; }

        /// <summary>
        ///     The number of states expanded during the last action planning.
        /// </summary>
        public int LastNumExpandedStates { get; private set; }

        /// <summary>
        ///     The specific state of planning.
        /// </summary>
        public PlanningState PlanningState { get; private set; }

        /// <summary>
        ///     Indicates whether the planner has set the properties resulting from the last action planning
        ///     (e.g. LastBestAction, LastNumVisitedStates, etc.).
        /// </summary>
        public bool HasValidResults
        {
            get
            {
                return (this.LastBestAction != null) && (this.LastBestActions.Count > 0) &&
                       (this.LastNumVisitedStates > 0);
            }
        }

        /// <summary>
        ///     The action that defines the best strategy for the player for which the decision was last calculated.
        /// </summary>
        public IPlayerAction LastBestAction
        {
            get
            {
                return (this.LastBestActions == null) || (this.LastBestActions.Count == 0)
                    ? null
                    : this.LastBestActions.First().Key;
            }
        }

        /// <summary>
        ///     The actions that define the best decisions for the player for which the planning was last calculated.
        ///     Actions (keys) are in ascending order according to their predicted value for the player (values).
        /// </summary>
        public Dictionary<IPlayerAction, double> LastBestActions { get; private set; }

        /// <summary>
        ///     The actions that define the worst decisions for the player for which the planning was last calculated.
        ///     Actions (keys) are in descending order according to their predicted value for the player (values).
        /// </summary>
        public Dictionary<IPlayerAction, double> LastWorstActions { get; private set; }

        /// <summary>
        ///     The player for which the decision was last calculated.
        /// </summary>
        public Player LastPlayer { get; private set; }

        /// <summary>
        ///     The <see cref="Strategy" /> used by the player for which the decision was last calculated. This strategy might have
        ///     been adjusted by the planning algorithm, so it may be different than <see cref="LastPlayer" />.Strategy.
        /// </summary>
        public Strategy LastPlayerStrategy
        {
            get
            {
                return this._playersStrategies == null
                    ? this.LastPlayer == null ? new Strategy() : this.LastPlayer.Strategy
                    : this._playersStrategies[this.LastPlayer.Role];
            }
        }

        /// <summary>
        ///     The predicted game values correspond to the average game values calculated through simulation during
        ///     the last step of planning.
        /// </summary>
        public GameValuesElement PredictedGameValues { get; private set; }

        /// <summary>
        ///     The probability of reaching a state where players have no other option other than SkipTurn, possibly
        ///     resulting in a dead-end if all players reach this situation.
        ///     The value was calculated through simulation during the last step of planning.
        /// </summary>
        public double NoPlayProbability { get; private set; }

        /// <summary>
        ///     The probability of reaching a state where *no player* has a cell available on which to build a
        ///     new structure, according to the standard number of available cells in each level.
        ///     The value was calculated through simulation during the last step of planning.
        /// </summary>
        public double NoSpaceProbability { get; private set; }

        /// <summary>
        ///     The penalty attributed to action values that are calculated at a certain depth during planning.
        ///     (ActionValue -= 1 - <see cref="DepthPenalty" />^depth). This is to ensure that immediate effects have a greater
        ///     impact on decisions than possible future values.
        /// </summary>
        public double DepthPenalty { get; set; }

        /// <summary>
        ///     The penalty automatically attributed to <see cref="SkipTurn" /> actions.
        /// </summary>
        public double SkipTurnPen { get; set; }

        /// <summary>
        ///     The penalty attributed to actions that have been executed recently.
        ///     (ActionValue -= <see cref="HistoryPenalty" /> (<see cref="HistoryPenaltySmoothness" />^TimeSinceLastExecution)).
        ///     This is for the planner to make actions that were performed recently less valuable than "new" actions or actions
        ///     performed a long time ago.
        /// </summary>
        public double HistoryPenalty { get; set; }

        /// <summary>
        ///     The smoothness of the function associated with the <see cref="HistoryPenalty" />, according to:
        ///     ActionValue -= <see cref="HistoryPenalty" /> (<see cref="DEF_HISTORY_PEN_SMOOTH" />^TimeSinceLastExecution).
        /// </summary>
        public double HistoryPenaltySmoothness { get; set; }

        /// <summary>
        ///     The factor associated with the adjustment of several weights of the planning player's strategy. This is to make
        ///     sure that some alerts from the predicted game values are reflected in the agent's preferences for plays during
        ///     planning. The higher the factor the higher the influence of alerts in the player's strategy.
        /// </summary>
        public double StrategyAdjustmentParam { get; set; }

        #endregion

        #region Private utility properties

        private GameSimulator GameSimulator
        {
            get { return this._game.Simulator; }
        }

        private Dictionary<EnercitiesRole, Player> Players
        {
            get { return this._game.Players; }
        }

        private double CurrentBuildSpaceRatio
        {
            get
            {
                var level = Math.Min(this._game.State.GameInfoState.Level, DomainInfo.MAX_LEVELS - 1);
                var currentResidentialBuilds =
                    this.GameSimulator.GetBuildableUnits(StructureCategory.Residential, level);
                return (double) currentResidentialBuilds.Count/this._initialBuildableCoords[level];
            }
        }

        internal GameStatistics GameStatistics
        {
            get { return this._game.GameStatistics; }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            this._planningThreads.Clear();
            this._playersStrategies.Clear();
            this._startPlayerActionValues.Clear();
        }

        public void Init()
        {
            //gets initial number of build-able cells
            for (var i = 0; i < DomainInfo.MAX_LEVELS; i++)
                this._initialBuildableCoords[i] =
                    this.GameSimulator.GetBuildableUnits(StructureCategory.Residential, i).Count;
        }

        /// <summary>
        ///     Whether an action planning is in progress.
        /// </summary>
        public bool IsPlanning()
        {
            lock (this._locker)
                return this.PlanningState.Equals(PlanningState.Running) && this.MorePlanTime();
        }

        public void PlanNextAction(Player player = null)
        {
            //executes planning with players own strategies
            this.PlanNextAction(
                this.Players.ToDictionary(stratPlayer => stratPlayer.Key, stratPlayer => stratPlayer.Value.Strategy),
                player);
        }

        public void PlanNextAction(Strategy commonStrategy, Player player = null)
        {
            //executes planning with common single strategy
            this.PlanNextAction(
                this.Players.ToDictionary(stratPlayer => stratPlayer.Key, stratPlayer => commonStrategy),
                player);
        }

        public void PlanNextAction(Dictionary<EnercitiesRole, Strategy> strategies, Player player = null)
        {
            //verify planning in progress, interrupt
            if (this.IsPlanning())
                this.Interrupt();

            lock (this._locker)
            {
                this.PlanningState = PlanningState.Running;

                //resets planning algorithm variables
                this.Reset(strategies, player);

                //calculates the start player action values (breadth-first search)
                this._startPlayerActionValues = this.GameSimulator.GetNextActionValues(
                    this.LastPlayerStrategy, this.IsPlanning);
                this.UpdateNoPlayProbability(this._startPlayerActionValues.Count == 1);
            }

            //performs planning from the top-down, 1 planner per CPU
            ProcessUtil.RunThreads(this.ProcessStartPlayerMoves, this.MaxCPUs*2);

            //finish planning
            lock (this._locker)
            {
                this.Finish();
                this.PlanningState = PlanningState.Stopped;
            }
        }

        internal void UpdateNoPlayProbability(bool noAction)
        {
            lock (this._locker)
                this.NoPlayProbability = ((this.NoPlayProbability*this.LastNumExpandedStates) + (noAction ? 1 : 0))/
                                         (++this.LastNumExpandedStates);
        }

        #endregion

        #region Private planning methods

        /// <summary>
        ///     Adjusts a <see cref="Strategy" /> according to predefined "preferences" in the form of rules. This is used
        ///     to provide special policies when needed, e.g., when some resource is scarce.
        /// </summary>
        /// <param name="playerRole">the role of the player whose strategy is to be adjusted</param>
        private Strategy AdjustPlayerStrategy(EnercitiesRole playerRole)
        {
            var gameValues = this.HasValidResults
                ? this.PredictedGameValues
                : this._game.State.GameInfoState;

            const int maxLevel = DomainInfo.MAX_LEVELS - 1;
            var level = Math.Min(this._game.State.GameInfoState.Level, maxLevel);
            var winConditions = this._game.DomainInfo.Scenario.WinConditions;
            var homesNeedRatio = 1d - (gameValues.Homes/winConditions[level].Population);

            var maxPopulation = winConditions[maxLevel].Population;
            var populationValue = 2d - (this._game.State.GameInfoState.Population > maxPopulation
                ? 0
                : this.NoSpaceProbability + homesNeedRatio);

            return StrategyAdjustment.GetAdjustedStrategy(
                playerRole, this._playersStrategies[playerRole], gameValues,
                this.StrategyAdjustmentParam, populationValue);
        }

        private void ProcessStartPlayerMoves()
        {
            lock (this._locker)
                this._planningThreads.Add(Thread.CurrentThread);

            //while planning and still top actions left
            while (true)
            {
                //verify still planning and time available
                lock (this._locker)
                    if (!this.IsPlanning()) break;

                //tries to get next start player possible action (blocking code)
                StateActionChange stateActionChange;
                AdversarialPlanner adversarialPlanner;
                lock (this._locker)
                {
                    if ((this._startPlayerActionValues.Count == 0) ||
                        (this._startPlayerActionIdx >= this._startPlayerActionValues.Count))
                        break;

                    stateActionChange = this._startPlayerActionValues[this._startPlayerActionIdx++];
                    if (stateActionChange.Action == null) break;

                    adversarialPlanner = new AdversarialPlanner(
                        this, this._game.DomainInfo, this._game.PlayersOrder, this._playersStrategies,
                        stateActionChange.State, this._startGameValues, this.LastPlayer.Role);
                }

                //plans forward according to this action
                var plannedMaxActionValue = adversarialPlanner.GetMaxNextActionValue();

                //get max action value, including future and own top player action
                plannedMaxActionValue = Math.Max(plannedMaxActionValue, stateActionChange.ObjectiveValue);

                //adds possible penalties to action
                plannedMaxActionValue -= this.AddActionPenalties(stateActionChange.Action);

                //replaces start player action value according to other player's actions
                lock (this._locker)
                    this._startPlayerActionValues.ReAdd(stateActionChange, plannedMaxActionValue);

                adversarialPlanner.Dispose();
            }

            lock (this._locker)
                this._planningThreads.Remove(Thread.CurrentThread);
        }

        private double AddActionPenalties(IPlayerAction action)
        {
            var actPenalty = 0.0;

            //verifies Skip action penalty
            if (action is SkipTurn)
                actPenalty += this.SkipTurnPen;

            //verifies action history penalty for this player
            var timeLastAction = this._game.ActionHistory.GetTimeSinceLastActionType(this.LastPlayer.Role, action);
            actPenalty += this.HistoryPenalty*Math.Pow(this.HistoryPenaltySmoothness, timeLastAction);

            return actPenalty;
        }

        private bool MorePlanTime()
        {
            lock (this._locker)
            {
                var morePlanTime = (DateTime.Now.Subtract(this._startPlanTime).TotalSeconds <= this.MaxProcessTime);
                if (!morePlanTime) this.PlanningState = PlanningState.Stopped; // make sure no more planning
                return morePlanTime;
            }
        }

        private void Interrupt()
        {
            this.PlanningState = PlanningState.Interrupting;

            //kills all threads
            foreach (var thread in this._planningThreads.ToList())
                thread.Abort();

            //waits for them to exit
            foreach (var thread in this._planningThreads.ToList())
                thread.Join();

            this._planningThreads.Clear();

            while (!this.PlanningState.Equals(PlanningState.Stopped))
                Thread.Sleep(10);
        }

        private void Reset(Dictionary<EnercitiesRole, Strategy> strategies, Player player)
        {
            //checks player
            if (player == null)
                player = this.Players[this.GameSimulator.State.GameInfoState.CurrentRole];

            this.LastPlayer = player;
            this._playersStrategies = strategies;
            this._startGameValues = this.GameSimulator.State.GameInfoState;

            //adjust start (top) player strategy
            this._playersStrategies[this.LastPlayer.Role] = this.AdjustPlayerStrategy(this.LastPlayer.Role);

            this.GameStatistics.ResetTempStats();
            this.NoPlayProbability = 0;

            this.LastNumExpandedStates = 0;
            this.LastNumVisitedStates = 0;
            if (this.LastBestActions != null) this.LastBestActions.Clear();
            this._startPlayerActionIdx = 0;

            ProcessUtil.SetProcessAffinity(this.MaxCPUs);
            this._startPlanTime = DateTime.Now;

            //raises event
            if (this.PlanningStarted != null)
                this.PlanningStarted(this, EventArgs.Empty);
        }

        private void Finish()
        {
            if (this.PlanningState.Equals(PlanningState.Interrupting)) return;

            //calculates best/worst player actions from planning, 1st sort all actions by action value then select only top "X"
            var maxActions = Math.Min(this.NumBestActions, this._startPlayerActionValues.Count);
            this.LastBestActions = new Dictionary<IPlayerAction, double>(maxActions);
            this.LastWorstActions = new Dictionary<IPlayerAction, double>(maxActions);
            for (var i = 0; i < maxActions; i++)
            {
                var bestActionValue = this._startPlayerActionValues[i];
                this.LastBestActions.Add(bestActionValue.Action, bestActionValue.ObjectiveValue);

                var worstActionValue = this._startPlayerActionValues[this._startPlayerActionValues.Count - i - 1];
                this.LastWorstActions.Add(worstActionValue.Action, worstActionValue.ObjectiveValue);
            }

            //updates predicted game values based on temp values from game stats
            this.PredictedGameValues = this.GameStatistics.TempGameValuesStats.GetAverageGameValues();
            this.LastNumVisitedStates = this.GameStatistics.TempGameValuesStats.ValueCount;

            //calculates no more space probability (does not depend on planning but may be costly)
            this.NoSpaceProbability = 1d - this.CurrentBuildSpaceRatio;

            //measures time taken 
            this.LastTimeTaken = DateTime.Now.Subtract(this._startPlanTime).TotalSeconds;

            //raises event
            if (this.PlanningFinished != null)
                this.PlanningFinished(this, EventArgs.Empty);

            //garbage collect
            if (this.AutoGarbageCollect)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion
    }
}