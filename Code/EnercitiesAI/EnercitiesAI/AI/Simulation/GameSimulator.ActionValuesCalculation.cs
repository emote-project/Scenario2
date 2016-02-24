using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Planning;
using EnercitiesAI.Domain;

namespace EnercitiesAI.AI.Simulation
{
    public delegate bool KeepGoingDelegate();

    public partial class GameSimulator
    {
        public SortedActionValuesList GetNextActionValues(Strategy strategy, KeepGoingDelegate keepGoing)
        {
            //gets next possible actions for the player making the decision (top player successors)
            var possibleActions = this.GetSuitableActions();
            var actionValues = new SortedActionValuesList(strategy, this._gameStatistics);
            foreach (var action in possibleActions)
            {
                //checks whether to stop
                if (!keepGoing()) break;

                //clones state before changing
                var stateBeforeAction = this.ReplaceState(this.State);

                //executes action, gets action value 
                var actionValue = this.GetStateActionChange(action, stateBeforeAction.GameInfoState);
                actionValues.Add(actionValue);

                //puts previous state back
                this.UndoState(stateBeforeAction);
            }

            return actionValues;
        }

        public StateActionChange GetStateActionChange(IPlayerAction action, GameValuesElement gameValues)
        {
            //player executes action, simulate it and gets change caused by action in its perspective (/strategy)
            var gameValuesChange = this.GetGameValuesChange(action, gameValues);
            return new StateActionChange {State = this.State.Clone(), Action = action, Change = gameValuesChange};
        }

        public double GetActionValue(IPlayerAction action, GameValuesElement gameValues, Strategy strategy)
        {
            var change = this.GetStateActionChange(action, gameValues);
            change.UpdateObjectiveValue(strategy, this._gameStatistics);
            return change.ObjectiveValue;
        }

        private GameValuesElement GetGameValuesChange(IPlayerAction action, GameValuesElement initialGameValues)
        {
            //simulates action execution and state update
            this.ExecuteAction(action);
            this.UpdateState();

            //gets change in game values
            var gameValues = (GameValuesElement) this.State.GameInfoState;
            var gameValuesChange = (GameValuesElement) gameValues.Clone();
            gameValuesChange.Subtract(initialGameValues);

            //updates statistics
            if (this._gameStatistics != null)
                this._gameStatistics.UpdateGameValuesStats(gameValues, gameValuesChange);

            return gameValuesChange;
        }
    }
}