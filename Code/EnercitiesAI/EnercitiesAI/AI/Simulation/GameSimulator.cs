using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EnercitiesAI.AI.Game;
using EnercitiesAI.AI.States;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;

namespace EnercitiesAI.AI.Simulation
{
    /// <summary>
    ///     Provides methods to **simulate** the effect of player actions and time on the game's
    ///     state according to its dynamics. The simulator contains a "state" of the game, on
    ///     which it performs all the changes. It also contains methods to provide possible next
    ///     actions given the current state, useful for the generation of successor states.
    ///     It is also able to calculate the values of next actions given the current game
    ///     state and a strategy, which is useful for planning.
    /// </summary>
    public partial class GameSimulator : IDisposable
    {
        private readonly DomainInfo _domainInfo;
        private readonly GameStatistics _gameStatistics;
        private readonly SortedSet<Coordinate> _unitsToUpdate = new SortedSet<Coordinate>();
        private double _currentCosts;
        private List<EnercitiesRole> _playersOrder;

        public GameSimulator(DomainInfo domainInfo, GameStatistics gameStatistics, List<EnercitiesRole> playersOrder)
        {
            this._domainInfo = domainInfo;
            this._gameStatistics = gameStatistics;
            this._playersOrder = playersOrder;
        }

        public State State { get; set; }

        public List<EnercitiesRole> PlayersOrder
        {
            set { this._playersOrder = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._unitsToUpdate.Clear();
        }

        #endregion

        public void Init()
        {
            if (this.State == null) return;

            //marks all units for update
            this._unitsToUpdate.Clear();
            foreach (var coord in this.State.StructuresState.GetOccupiedUnits())
                this._unitsToUpdate.Add(coord);

            //just update to initial state
            this.UpdateState(true);
        }

        public State ReplaceState(State newState)
        {
            //replaces current state, clones previous one
            var previousState = this.State;
            this.State = newState.Clone();
            return previousState;
        }

        public void UndoState(State previousClonedState)
        {
            //disposes cloned state, put back old one
            this.State.Dispose();
            this.State = previousClonedState;
        }
    }
}