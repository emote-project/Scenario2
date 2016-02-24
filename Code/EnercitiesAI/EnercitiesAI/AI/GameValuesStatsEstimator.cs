using System;
using System.Linq;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.Domain;
using EnercitiesAI.Programs;
using MathNet.Numerics;
using MathNet.Numerics.Random;
using PS.Utilities;
using PS.Utilities.Forms;

namespace EnercitiesAI.AI
{
    public class GameValuesStatsEstimator : IProgressHandler, IDisposable
    {
        private const int NUM_GAMES = 100;
        private const int MAX_NUM_PLAYS = 1000;
        private static readonly Random Random = new WH2006();
        private readonly uint _cpuCount = ProcessUtil.GetCPUCount();
        private readonly object _locker = new object();

        private readonly int _maxNumGames;
        private readonly int _maxNumPlays;
        private readonly ProgressFormUpdater _progressFormUpdater;

        private int _gameNum;

        public GameValuesStatsEstimator(int numGames = NUM_GAMES, int maxPlays = MAX_NUM_PLAYS)
        {
            this.GameValuesStats = new GameValuesStatsCollection("pre");
            this._maxNumGames = numGames;
            this._maxNumPlays = maxPlays;
            this._progressFormUpdater = new ProgressFormUpdater(this) {Text = "Estimating game values stats"};
        }

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
            get { return (double) (this._gameNum - this._cpuCount)/this._maxNumGames; }
        }

        #endregion

        public void EstimateGameValuesStats(bool showProgress = false)
        {
            this._progressFormUpdater.Visible = showProgress;

            ProcessUtil.SetMaximumProcessAffinity();
            ProcessUtil.RunThreads(this.RunGame, this._cpuCount);
        }

        private void RunGame()
        {
            while (true)
            {
                //verify num games
                int gameNum;
                lock (this._locker)
                    if ((gameNum = this._gameNum++) > this._maxNumGames)
                        return;

                //Console.WriteLine("Running game {0} of {1}", gameNum + 1, this._maxNumGames);
                using (var game = new Game.Game(XmlLoadTestProgram.XML_BASE_PATH))
                {
                    //initiates game, ignore strategies, oil  or money restrictions
                    game.Init();
                    game.Simulator.UpdateState();
                    //this._game.PlayHistory.Add(new ActionStatePair {State = this._game.State.GameInfoState.Clone()});
                    game.ActionPlanner.MaxPlanningDepth = 0;
                    game.State.GameInfoState.Money = game.State.GameInfoState.Oil = 1000000;
                    game.State.GameInfoState.Level = 3;
                    this.SetRandomPlayersOrder(game);

                    //runs random plays
                    var playNum = 0;
                    for (; playNum < this._maxNumPlays; playNum++)
                    {
                        //plans with random strategy (all 0s)
                        game.ActionPlanner.PlanNextAction(new Strategy());
                        this.ExecuteAction(game);

                        //verifies game ended
                        if (game.Simulator.IsTerminalState() ||
                            game.ActionPlanner.NoPlayProbability.AlmostEqual(1))
                            break;
                    }

                    Console.WriteLine("Game {0} terminated at {1} plays", gameNum, playNum + 1);
                }
            }
        }

        private void ExecuteAction(Game.Game game)
        {
            var action = game.ActionPlanner.LastBestAction;
            var beforeValues = game.State.GameInfoState.Clone();

            game.Simulator.ExecuteAction(action);
            game.Simulator.UpdateState();
            //this._game.PlayHistory.Add(new ActionStatePair
            //                           {
            //                               Action = action,
            //                               State = this._game.State.GameInfoState.Clone()
            //                           });

            var valuesChange = (GameValuesElement) game.State.GameInfoState;
            valuesChange.Subtract(beforeValues);

            //updates stats
            this.GameValuesStats.Update(valuesChange);
        }

        private void SetRandomPlayersOrder(Game.Game game)
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