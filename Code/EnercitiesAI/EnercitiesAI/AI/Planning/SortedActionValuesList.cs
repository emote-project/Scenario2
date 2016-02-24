using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EmoteEvents;
using EnercitiesAI.AI.Game;

namespace EnercitiesAI.AI.Planning
{
    public class SortedActionValuesList : IEnumerable<StateActionChange>
    {
        private readonly GameStatistics _gameStatistics;
        private readonly HashSet<StateActionChange> _hash = new HashSet<StateActionChange>();
        private readonly List<StateActionChange> _sortedList = new List<StateActionChange>();
        private readonly Strategy _strategy;

        public SortedActionValuesList(Strategy strategy, GameStatistics gameStatistics)
        {
            this._strategy = strategy;
            this._gameStatistics = gameStatistics;
        }

        public int Count
        {
            get { return this._sortedList.Count; }
        }

        public StateActionChange this[int idx]
        {
            get { return idx >= this.Count ? null : this._sortedList.ElementAt(idx); }
        }

        #region IEnumerable<StateActionChange> Members

        public IEnumerator<StateActionChange> GetEnumerator()
        {
            return this._sortedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public bool Contains(StateActionChange stateActionChange)
        {
            return this._hash.Contains(stateActionChange);
        }

        public void ReAdd(StateActionChange stateActionChange, double objectiveValue)
        {
            if (!this.Contains(stateActionChange))
                return;
            this.Remove(stateActionChange);
            stateActionChange.ObjectiveValue = objectiveValue;
            this.AddToSets(stateActionChange);
        }

        public void Add(StateActionChange stateActionChange)
        {
            //first get the objective value
            stateActionChange.UpdateObjectiveValue(this._strategy, this._gameStatistics);

            //then add in sorted manner
            this.AddToSets(stateActionChange);
        }

        private void AddToSets(StateActionChange stateActionChange)
        {
            var idx = this.GetSortIndex(stateActionChange, 0, this.Count);
            this._sortedList.Insert(idx, stateActionChange);
            this._hash.Add(stateActionChange);
        }

        private int GetSortIndex(StateActionChange stateActionChange, int min, int max)
        {
            while (!min.Equals(max))
            {
                var mid = min + ((max - min)/2);
                var midValue = this._sortedList[mid].ObjectiveValue;
                if (stateActionChange.ObjectiveValue.Equals(midValue)) return mid + 1;
                if (stateActionChange.ObjectiveValue < midValue)
                    min = min.Equals(mid) ? mid + 1 : mid;
                else
                    max = mid;
            }
            return min;
        }

        public void Remove(StateActionChange stateActionChange)
        {
            if (!this.Contains(stateActionChange)) return;
            this._sortedList.Remove(stateActionChange);
            this._hash.Remove(stateActionChange);
        }

        public void Clear()
        {
            this._sortedList.Clear();
            this._hash.Clear();
        }
    }
}