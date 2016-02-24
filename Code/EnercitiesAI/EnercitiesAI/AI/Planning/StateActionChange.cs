using System;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.AI.States;
using EnercitiesAI.Domain;

namespace EnercitiesAI.AI.Planning
{
    public class StateActionChange
    {
        public StateActionChange()
        {
            this.ObjectiveValue = double.MinValue;
        }

        public State State { get; set; }
        public IPlayerAction Action { get; set; }
        public GameValuesElement Change { get; set; }
        public double ObjectiveValue { get; set; }

        #region IComparable<StateActionChange> Members

        public int CompareTo(StateActionChange other)
        {
            var value = -this.ObjectiveValue.CompareTo(other.ObjectiveValue);
            return value.Equals(0) ? 1 : value;
        }

        #endregion

        public void UpdateObjectiveValue(Strategy strategy, GameStatistics gameStatistics)
        {
            var value = 0.0;
            if (gameStatistics != null)
            {
                //first normalizes the game values according to statistics collected during the simulations
                var gameValuesChangeStats = gameStatistics.GameValuesChangeStats;
                var normGameValues = gameValuesChangeStats.GetNormalizedGameValues(this.Change);
                var normUniformity = gameValuesChangeStats.GetNormalizedScoresUniformity(this.Change);

                value = StrategyExtensions.GetObjectiveValue(strategy, normGameValues, normUniformity);
            }
            this.ObjectiveValue = value;
        }

        public override bool Equals(object obj)
        {
            return obj is StateActionChange && this.Equals((StateActionChange) obj);
        }

        public bool Equals(StateActionChange other)
        {
            return this.Action.Equals(other.Action);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Action == null ? 0 : this.Action.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1} -> {2}; {3:0.00}",
                this.State == null ? " " : ((GameValuesElement) this.State.GameInfoState).ToString(),
                this.Action, this.Change, this.ObjectiveValue);
        }
    }
}