using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.Domain;
using EnercitiesAI.Programs;
using MathNet.Numerics.Random;
using PS.Utilities;
using PS.Utilities.Collections;
using PS.Utilities.Forms;
using PS.Utilities.Math;

namespace EnercitiesAI.AI.Estimation
{
    public class RandomWalkEstimator : IProgressHandler, IDisposable
    {
        private static readonly Random Random = new WH2006();
        private readonly uint _cpuCount = ProcessUtil.GetCPUCount();
        private readonly object _locker = new object();
        private readonly uint _numGames;
        private readonly ProgressFormUpdater _progressFormUpdater;
        private uint _numGame;

        public RandomWalkEstimator(uint numGames)
        {
            _numGames = numGames;
            this.GameValuesStats = new GameValuesStatsCollection("pre");
            this.ActionStatsCollection = new StatisticsCollection();
            this.NumGamePlays = new StatisticalQuantity();
            this.ActionStats = new StatisticalQuantity();
            foreach (var role in EnumUtil<EnercitiesRole>.GetTypes())
                this.ActionStatsCollection.Add(role.ToString(), new StatisticalQuantity());

            this._progressFormUpdater = new ProgressFormUpdater(this) {Text = "Estimating state-action stats"};
        }

        public ulong NumStates { get; private set; }

        public uint NumGames
        {
            get { return this.NumGamePlays.SampleCount; }
        }

        public StatisticalQuantity NumGamePlays { get; private set; }
        public StatisticsCollection ActionStatsCollection { get; private set; }
        public StatisticalQuantity ActionStats { get; private set; }
        public GameValuesStatsCollection GameValuesStats { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            this._progressFormUpdater.Dispose();
        }

        #endregion

        #region IProgressHandler Members

        public double ProgressValue
        {
            get { return (double) this._numGame/this._numGames; }
        }

        #endregion

        public void Estimate(bool showProgress = false)
        {
            this._progressFormUpdater.Visible = showProgress;
            ProcessUtil.SetMaximumProcessAffinity();
            ProcessUtil.RunThreads(this.ProcessFirstActions, this._cpuCount*2);
        }

        private void ProcessFirstActions()
        {
            while (true)
            {
                //verify num games
                lock (this._locker)
                    if (this._numGame++ >= this._numGames)
                        return;

                //makes all possible plays with this game order
                this.MakeAllPlays();
            }
        }

        private void MakeAllPlays()
        {
            //runs all plays until game ends
            using (var game = CreateGame())
                MakeNextPlay(game, 0);
        }

        private static Game.Game CreateGame()
        {
            var game = new Game.Game(string.Format("..\\{0}", XmlLoadTestProgram.XML_BASE_PATH));

            //initiates game with given order
            game.Init();
            game.Simulator.UpdateState();
            game.ActionPlanner.MaxPlanningDepth = 0;
            SetRandomPlayersOrder(game);

            return game;
        }

        private void MakeNextPlay(Game.Game game, uint numGamePlays)
        {
            //verifies terminal state
            var simulator = game.Simulator;
            if (simulator.IsTerminalState())
            {
                Console.WriteLine("Finished a game");
                lock (this._locker) this.NumGamePlays.Value = numGamePlays;
                return;
            }

            //gets and verifies all possible next actions (ignore skip)
            var actions = simulator.GetSuitableActions(true);
            if (actions.Count == 0)
            {
                Console.WriteLine("Finished a game no more plays");
                lock (this._locker) this.NumGamePlays.Value = numGamePlays;
                return;
            }

            //update state action stats for the current player's role
            lock (this._locker)
            {
                var currentRole = simulator.State.GameInfoState.CurrentRole;
                this.ActionStatsCollection[currentRole.ToString()].Value = actions.Count;
                this.ActionStats.Value = actions.Count;
            }

            //executes one random action

            var action = actions[Random.Next(actions.Count)];
            {
                //stores previous state
                var stateBeforeAction = simulator.ReplaceState(simulator.State);

                //executes actions and proceed depth-first search
                this.ExecuteAction(game, action);
                this.MakeNextPlay(game, numGamePlays + 1);

                //puts back old state
                simulator.UndoState(stateBeforeAction);
            }

            //update state count
            lock (this._locker) this.NumStates++;
        }

        private void ExecuteAction(Game.Game game, IPlayerAction action)
        {
            var beforeValues = game.State.GameInfoState.Clone();

            game.Simulator.ExecuteAction(action);
            game.Simulator.UpdateState();

            var valuesChange = (GameValuesElement) game.State.GameInfoState;
            valuesChange.Subtract(beforeValues);

            //updates game-value stats
            lock (this._locker) this.GameValuesStats.Update(valuesChange);
        }

        private List<EnercitiesRole[]> GetAllPlayOrders()
        {
            return EnumUtil<EnercitiesRole>.GetTypes().AllPermutations(3, false);
        }

        private static void SetRandomPlayersOrder(Game.Game game)
        {
            var possibleRoles = EnumUtil<EnercitiesRole>.GetTypes().ToList();
            var player1Idx = Random.Next(3);
            var player1 = possibleRoles[player1Idx];
            possibleRoles.RemoveAt(player1Idx);
            var player2 = possibleRoles[Random.Next(2)];
            game.SetPlayersOrder(player1, player2);
        }
    }
}