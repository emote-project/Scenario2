using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Simulation;
using EnercitiesAI.AI.States;
using EnercitiesAI.Domain;

namespace EnercitiesAI.AI.Planning
{
    public class AdversarialPlanner : IDisposable
    {
        private const double DEF_MAX_ACTION_VALUE = double.MinValue;
        private readonly ActionPlanner _actionPlanner;
        private readonly GameSimulator _gameSimulator;
        private readonly Dictionary<EnercitiesRole, Strategy> _playersStrategies;
        private readonly GameValuesElement _startGameValues;
        private readonly Strategy _startPlayerStrategy;

        public AdversarialPlanner(
            ActionPlanner actionPlanner, DomainInfo domainInfo, List<EnercitiesRole> playersOrder,
            Dictionary<EnercitiesRole, Strategy> playersStrategies, State startState,
            GameValuesElement startGameValues, EnercitiesRole startPlayerRole)
        {
            this._actionPlanner = actionPlanner;
            this._playersStrategies = playersStrategies;
            this._startGameValues = startGameValues;
            this._startPlayerStrategy = playersStrategies[startPlayerRole];
            this._gameSimulator = new GameSimulator(domainInfo, actionPlanner.GameStatistics, playersOrder)
                                  {
                                      State = startState,
                                  };
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._gameSimulator.Dispose();
        }

        #endregion

        #region Planning methods

        public double GetMaxNextActionValue(int curDepth = 1)
        {
            //finished searching in this branch?
            if (!this._actionPlanner.IsPlanning() ||
                (curDepth > this._actionPlanner.MaxPlanningDepth))
                return DEF_MAX_ACTION_VALUE;

            //gets max next action, in the perspective of cur planning player
            var maxAction = this.GetCurPlayerMaxNextAction();
            if (maxAction == null)
                return DEF_MAX_ACTION_VALUE;

            //stores previous state
            var stateBeforeAction = this._gameSimulator.ReplaceState(this._gameSimulator.State);

            //cur player executes max next action, get the value according to start player
            var startPlayerActionValue =
                this._gameSimulator.GetActionValue(maxAction, this._startGameValues, this._startPlayerStrategy);

            //adds depth penalty 
            startPlayerActionValue -= 1 - Math.Pow(this._actionPlanner.DepthPenalty, curDepth);

            //continues depth-first greedy search
            var maxNextActionValue = this.GetMaxNextActionValue(curDepth + 1);

            //gets max value
            var maxActionValue = Math.Max(startPlayerActionValue, maxNextActionValue);

            //puts back old state
            this._gameSimulator.UndoState(stateBeforeAction);

            return maxActionValue;
        }

        private IPlayerAction GetCurPlayerMaxNextAction()
        {
            var curPlayerStrategy = this._playersStrategies[this._gameSimulator.State.GameInfoState.CurrentRole];
            var nextActionValues = this._gameSimulator.GetNextActionValues(curPlayerStrategy, this._actionPlanner.IsPlanning);

            //updates no action probability (1 action means SkipTurn only)
            this._actionPlanner.UpdateNoPlayProbability(nextActionValues.Count == 1);

            var retVal = nextActionValues.Count == 0 ? null : nextActionValues.First().Action;
            nextActionValues.Clear();
            return retVal;
        }

        #endregion
    }
}