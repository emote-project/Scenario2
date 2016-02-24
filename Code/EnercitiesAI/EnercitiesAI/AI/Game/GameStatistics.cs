using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Planning;
using EnercitiesAI.Domain;
using PS.Utilities.Math;

namespace EnercitiesAI.AI.Game
{
    /// <summary>
    ///     Contains statistical information about all dynamic variables in the game, including
    ///     the scores, resources levels, player actions stats, etc.
    /// </summary>
    public class GameStatistics : IDisposable
    {
        private const string ACTIONS_DIR = "Actions";
        private const string STRATEGIES_DIR = "Strategies";
        private const string ECONOMY_PREFIX = "Eco";
        private const string ENVIRONMENT_PREFIX = "Env";
        private const string MAYOR_PREFIX = "May";
        private const string AVG_TIME_STATE_STR = "AvgTimeState";
        private const string VISITED_STATES_STR = "VisitedStates";
        private readonly DomainInfo _domainInfo;
        private readonly object _locker = new object();
        private readonly Dictionary<EnercitiesRole, string> _playerPrefixes;
        private readonly StatisticsCollection _statsCollection = new StatisticsCollection();

        public GameStatistics(DomainInfo domainInfo)
        {
            this._domainInfo = domainInfo;
            this.CreateStats();
            this._playerPrefixes = new Dictionary<EnercitiesRole, string>
                                   {
                                       {EnercitiesRole.Economist, ECONOMY_PREFIX},
                                       {EnercitiesRole.Environmentalist, ENVIRONMENT_PREFIX},
                                       {EnercitiesRole.Mayor, MAYOR_PREFIX}
                                   };
        }

        #region Properties

        public GameValuesStatsCollection GameValuesStats { get; private set; }
        public GameValuesStatsCollection GameValuesChangeStats { get; private set; }
        public GameValuesStatsCollection TempGameValuesStats { get; private set; }

        public Dictionary<EnercitiesRole, StatisticsCollection> ActionStatistics { get; private set; }

        public StatisticalQuantity AvgTimeStateQuantity { get; private set; }
        public StatisticalQuantity NumVisitedStatesQuantity { get; private set; }

        public Dictionary<EnercitiesRole, StrategyStatsCollection> StrategyStatistics { get; private set; }

        private uint MaxNumSamples
        {
            get { return (uint) this._domainInfo.Scenario.StartValues.YearsInGame; }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            //disposes of all statistics
            this._statsCollection.Dispose();
            if (this.TempGameValuesStats != null)
                this.TempGameValuesStats.Dispose();
        }

        public void Init()
        {
            //defines number of samples (= number of years in game)
            this._statsCollection.MaxNumSamples = this.MaxNumSamples;
            this._statsCollection.SampleSteps = 1;

            //adds all statistics to be recorded
            this._statsCollection.AddRange(this.GameValuesStats);
            this._statsCollection.AddRange(this.GameValuesChangeStats);
            foreach (var actionStats in this.ActionStatistics.Values)
                this._statsCollection.AddRange(actionStats);
            this._statsCollection.Add(AVG_TIME_STATE_STR, this.AvgTimeStateQuantity);
            this._statsCollection.Add(VISITED_STATES_STR, this.NumVisitedStatesQuantity);
            foreach (var strategyStats in this.StrategyStatistics.Values)
                this._statsCollection.AddRange(strategyStats);

            //inits / sets all statistics
            this._statsCollection.InitParameters();
        }

        public void ResetTempStats()
        {
            lock (this._locker)
            {
                if (this.TempGameValuesStats != null)
                    this.TempGameValuesStats.Dispose();
                this.TempGameValuesStats = new GameValuesStatsCollection("temp") {MaxNumSamples = this.MaxNumSamples};
                this.TempGameValuesStats.InitParameters();
            }
        }

        public void UpdateGameValuesStats(GameValuesElement gameValues, GameValuesElement gameValuesChange)
        {
            //updates game values stats
            lock (this._locker)
            {
                if (this.TempGameValuesStats != null)
                    this.TempGameValuesStats.Update(gameValues);
                this.GameValuesStats.Update(gameValues);
                this.GameValuesChangeStats.Update(gameValuesChange);
            }
        }

        public void UpdateActionStats(EnercitiesRole role, IPlayerAction action)
        {
            //updates statistics based on action execution (1 if action of type was executed, 0 if not)
            lock (this._locker)
                foreach (var actionStats in this.ActionStatistics[role])
                    actionStats.Value.Value = actionStats.Key.Equals(
                        GetActionString(this._playerPrefixes[role], action.Type))
                        ? 1
                        : 0;
        }

        public void UpdatePlanningStats(ActionPlanner planner)
        {
            //updates statistics bases on last planning step
            var visitedStates = planner.LastNumVisitedStates;
            var timeTaken = planner.LastTimeTaken;
            var avgTime = visitedStates.Equals(0)
                ? 0
                : timeTaken*1000000/visitedStates;

            this.NumVisitedStatesQuantity.Value = visitedStates;
            this.AvgTimeStateQuantity.Value = avgTime;
        }

        public void UpdateStrategyStatistics(Dictionary<EnercitiesRole, Player> players)
        {
            foreach (var strategyStatistic in this.StrategyStatistics)
                strategyStatistic.Value.Update(players[strategyStatistic.Key].Strategy);
        }

        public void PrintResults(string basePath)
        {
            //prints resources
            this.GameValuesStats.PrintResults(string.Format("{0}/GameValues", basePath), false);
            this.GameValuesChangeStats.PrintResults(string.Format("{0}/GameValuesChange", basePath), false);

            //prints actions stats
            this.ActionStatistics[EnercitiesRole.Environmentalist].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-actions.csv", basePath, ACTIONS_DIR, ENVIRONMENT_PREFIX));
            this.ActionStatistics[EnercitiesRole.Economist].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-actions.csv", basePath, ACTIONS_DIR, ECONOMY_PREFIX));
            this.ActionStatistics[EnercitiesRole.Mayor].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-actions.csv", basePath, ACTIONS_DIR, MAYOR_PREFIX));

            //prints planning stats
            this.AvgTimeStateQuantity.PrintStatisticsToCSV(string.Format("{0}/{1}.csv", basePath, AVG_TIME_STATE_STR));
            this.NumVisitedStatesQuantity.PrintStatisticsToCSV(
                string.Format("{0}/{1}.csv", basePath, VISITED_STATES_STR));

            //prints strategy stats
            this.StrategyStatistics[EnercitiesRole.Environmentalist].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-strategy.csv", basePath, STRATEGIES_DIR, ENVIRONMENT_PREFIX));
            this.StrategyStatistics[EnercitiesRole.Economist].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-strategy.csv", basePath, STRATEGIES_DIR, ECONOMY_PREFIX));
            this.StrategyStatistics[EnercitiesRole.Mayor].PrintAllQuantitiesToCSV(
                string.Format("{0}/{1}/{2}-strategy.csv", basePath, STRATEGIES_DIR, MAYOR_PREFIX));
        }

        #endregion

        #region Private Methods

        private void CreateStats()
        {
            //creates all quantities and collections
            this.GameValuesStats = new GameValuesStatsCollection("gvs");
            this.GameValuesChangeStats = new GameValuesStatsCollection("gvcs");

            this.ActionStatistics =
                new Dictionary<EnercitiesRole, StatisticsCollection>
                {
                    {EnercitiesRole.Economist, CreateActionCollection(ECONOMY_PREFIX)},
                    {EnercitiesRole.Environmentalist, CreateActionCollection(ENVIRONMENT_PREFIX)},
                    {EnercitiesRole.Mayor, CreateActionCollection(MAYOR_PREFIX)}
                };

            this.AvgTimeStateQuantity = new StatisticalQuantity();
            this.NumVisitedStatesQuantity = new StatisticalQuantity();

            this.StrategyStatistics =
                new Dictionary<EnercitiesRole, StrategyStatsCollection>
                {
                    {EnercitiesRole.Economist, new StrategyStatsCollection(ECONOMY_PREFIX)},
                    {EnercitiesRole.Environmentalist, new StrategyStatsCollection(ENVIRONMENT_PREFIX)},
                    {EnercitiesRole.Mayor, new StrategyStatsCollection(MAYOR_PREFIX)}
                };
        }

        private static StatisticsCollection CreateActionCollection(string prefix)
        {
            return new StatisticsCollection
                   {
                       {GetActionString(prefix, ActionType.BuildStructure), new StatisticalQuantity()},
                       {GetActionString(prefix, ActionType.UpgradeStructures), new StatisticalQuantity()},
                       {GetActionString(prefix, ActionType.ImplementPolicy), new StatisticalQuantity()},
                       {GetActionString(prefix, ActionType.SkipTurn), new StatisticalQuantity()}
                   };
        }

        private static string GetActionString(string prefix, ActionType actionType)
        {
            return string.Format("{0}{1}", prefix, actionType);
        }

        #endregion
    }
}