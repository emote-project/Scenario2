using System.Collections.Generic;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.AI.States;
using EnercitiesAI.Domain;

namespace EnercitiesAI.AI.Planning
{
    public class PlanningUtil
    {
        private readonly ActionPlanner _actionPlanner;
        private readonly GameSimulator _gameSimulator;

        public PlanningUtil(ActionPlanner actionPlanner, GameSimulator gameSimulator)
        {
            this._actionPlanner = actionPlanner;
            this._gameSimulator = gameSimulator;
        }

        public Player CurrentPlayer
        {
            get { return this._actionPlanner.Players[this._gameSimulator.State.GameInfoState.CurrentRole]; }
        }

        public Strategy CurrentPlayerStrategy
        {
            get
            {
                return this._actionPlanner.PlanStrategies == null
                    ? this.CurrentPlayer.Strategy
                    : this._actionPlanner.PlanStrategies[this.CurrentPlayer.Role];
            }
        }

        public List<ActionValue> GetNextActionValues(Strategy strategy)
        {
            //gets next possible actions for the player making the decision (top player successors)
            var possibleActions = this.GetNextSuitableActions();
            var actionValues = new List<ActionValue>();
            foreach (var action in possibleActions)
            {
                //clones state before changing
                var stateBeforeAction = this.ReplaceState(this._gameSimulator.State.Clone());

                //executes action, gets action value 
                var actionValue = this.GetActionValue(action, stateBeforeAction.GameInfoState, strategy);
                actionValues.Add(actionValue);

                //puts previous state back
                this.UndoState(stateBeforeAction);
            }

            //sorts action values
            actionValues.Sort();
            return actionValues;
        }

        public ActionValue GetMaxNextAction(Strategy strategy)
        {
            var nextActionValues = this.GetNextActionValues(strategy);
            return nextActionValues.Count == 0 ? new ActionValue() : nextActionValues[0];
        }

        private GameValuesElement GetGameValuesChange(
            IPlayerAction action, GameValuesElement initialGameValues)
        {
            //simulates action execution and state update
            this._gameSimulator.ExecuteAction(action);
            this._gameSimulator.UpdateState();

            //gets change in game values
            var state = this._gameSimulator.State;
            var stateGameValues = (GameValuesElement) state.GameInfoState;
            var gameValuesChange = (GameValuesElement) stateGameValues.Clone();
            gameValuesChange.Subtract(initialGameValues);

            //updates predictions
            this._actionPlanner.UpdatePredictedGameValues(stateGameValues);

            return gameValuesChange;
        }

        public ActionValue GetActionValue(
            IPlayerAction action, GameValuesElement gameValues, Strategy strategy)
        {
            //player executes action, simulate it and gets action value in its perspective (/strategy)
            var gameValuesChange = this.GetGameValuesChange(action, gameValues);
            return new ActionValue
                   {
                       State = this._gameSimulator.State.Clone(),
                       Action = action,
                       Value = StrategyExtensions.GetValue(strategy, gameValuesChange)
                   };
        }

        private IEnumerable<IPlayerAction> GetNextSuitableActions()
        {
            //gets possible actions from simulator for current player
            var actions = this._gameSimulator.GetSuitableActions();

            //updates no action probability (1 action means SkipTurn only)
            var noAction = actions.Count == 1;

            //common resources, lock code
            this._actionPlanner.UpdateNoPlayProbability(noAction);

            return actions;
        }

        public State ReplaceState(State newState)
        {
            //replaces state
            var previousState = this._gameSimulator.State;
            this._gameSimulator.State = newState;
            return previousState;
        }

        public void UndoState(State previousState)
        {
            //disposes cloned state, put back old one before moving next action
            this._gameSimulator.State.Dispose();
            this._gameSimulator.State = previousState;
        }
    }
}