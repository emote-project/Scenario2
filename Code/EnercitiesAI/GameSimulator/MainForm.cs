using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI;
using EnercitiesAI.AI;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.Domain;
using PS.Utilities.Forms;

namespace GameSimulator
{
    public partial class MainForm : Form
    {
        private const int MAX_NUM_PLAYS = 150; //100;
        private const int MAX_AI_PROCESS_TIME = 5; //5; //20;//int.MaxValue;
        private const int MAX_PLAN_DEPTH = 3;
        private const int MAX_NUM_SKIPS = 10;
        private const int HISTORY_PLAY_INTERVAL = 1000;
        private const int CELL_SIZE = 80;

        private readonly Dictionary<EnercitiesRole, Strategy> _initialStrategies =
            new Dictionary<EnercitiesRole, Strategy>();

        private readonly object _locker = new object();
        private Game _game;
        private bool _isPaused;
        private uint _numSkipTurns;

        public MainForm()
        {
            this.InitializeComponent();

            this.CreateGame();
            this.EnableMenus(true);

            //resizes the form
            this.worldPanel.CellSize = CELL_SIZE;
            var size = this.worldPanel.ControlSize + new Size(8, 58);
            this.Size = size;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.PrintResults();
            base.OnClosing(e);
        }

        private void CreateGame()
        {
            this._game = new Game(EnercitiesAIClient.XML_BASE_PATH);
            this._game.Init(EnercitiesRole.Environmentalist, EnercitiesRole.Economist);
            this.worldPanel.Game = this._game;
            foreach (var player in this._game.Players.Values)
                this._initialStrategies[player.Role] = player.Strategy.Clone();

            this.Reset();
        }

        private void Reset()
        {
            this._numSkipTurns = 0;
            this.worldPanel.Visible = true;
            this.worldPanel.Invalidate();
        }

        private void EnableMenus(bool enable)
        {
            if (this.worldPanel.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this, new Action<bool>(this.EnableMenus), new object[] {enable});
                return;
            }

            this.simulationToolStripMenuItem.Enabled = this.worldPanel.GridVisible = !enable;
            this.newToolStripMenuItem.Enabled = this.loadGameToolStripMenuItem.Enabled = enable;
            this.worldPanel.Invalidate();
        }

        private void ShowEndMsgBox()
        {
            var isTerminal = this._game.Simulator.IsTerminalLevelState();
            MessageBox.Show((isTerminal ? "\nSustainable city! :)" : "\nCity unsustainable.. :("),
                "Game status", MessageBoxButtons.OK,
                (isTerminal ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation));
        }

        private void RunAIGamePlays()
        {
            this.EnableMenus(false);

            this._game.ActionPlanner.MaxProcessTime = MAX_AI_PROCESS_TIME;
            this._game.ActionPlanner.MaxPlanningDepth = MAX_PLAN_DEPTH;

            //ignores previous game
            if (File.Exists(EnercitiesAIClient.GAME_LOG_FILE_NAME))
                File.Delete(EnercitiesAIClient.GAME_LOG_FILE_NAME);

            this.MakeAIPlays(MAX_NUM_PLAYS);

            this.ShowEndMsgBox();
            this.PrintResults();

            this.EnableMenus(true);
        }

        private void RunHistoryGamePlays()
        {
            this.EnableMenus(false);

            //recovers all actions 1-by-1
            var numActionsPlayed = 0;
            foreach (var stateActionPair in this._game.ActionHistory.ToList())
            {
                var action = stateActionPair.Action;
                if (action == null) continue;

                Thread.Sleep(HISTORY_PLAY_INTERVAL);
                var isPaused = true;
                while (isPaused)
                {
                    Thread.Sleep(HISTORY_PLAY_INTERVAL);
                    lock (this._locker) isPaused = this._isPaused;
                }

                var player = this._game.Players[this._game.State.GameInfoState.CurrentRole];
                this.ExecuteAction(numActionsPlayed, player, action);
                numActionsPlayed++;
            }

            this.ShowEndMsgBox();
            this.EnableMenus(true);
        }

        private bool LoadGame()
        {
            var filePath = this.openFileDialog.FileName;
            this._game = Game.DeserializeFromJson(filePath);
            if (this._game == null) return false;
            this._game.DomainInfoPath = EnercitiesAIClient.XML_BASE_PATH;
            this._game.Init();
            this._game.Simulator.UpdateState();
            this.worldPanel.Game = this._game;
            return this._game != null;
        }

        private void MakeAIPlays(int numPlays)
        {
            //this._game.ActionPlanner.PlanNextAction();
            for (var i = 0; i < numPlays; i++)
            {
                var isPaused = true;
                while (isPaused)
                {
                    Application.DoEvents();
                    lock (this._locker) isPaused = this._isPaused;
                }

                this._game.ActionPlanner.PlanNextAction();
                var lastBestAction = this._game.ActionPlanner.LastBestAction;
                this.ExecuteAction(i, this._game.ActionPlanner.LastPlayer, lastBestAction);

                if (lastBestAction is SkipTurn)
                    this._numSkipTurns++;
                else
                    this._numSkipTurns = 0;
                if ((this._numSkipTurns >= MAX_NUM_SKIPS) || this._game.Simulator.IsTerminalState())
                    break;
            }
        }

        private void ExecuteAction(int i, Player player, IPlayerAction action)
        {
            this._game.ExecuteAction(player.Role, action);
            var valuesChange = this._game.UpdateState(this._game.State.GameInfoState);

            var gain = StrategyExtensions.GetObjectiveValue(player.Strategy, valuesChange);

            var visitedStates = this._game.ActionPlanner.LastNumVisitedStates;
            var timeTaken = this._game.ActionPlanner.LastTimeTaken;
            var avgTime = visitedStates.Equals(0)
                ? 0
                : timeTaken*1000000/visitedStates;

            this.WriteActionConsole(i, player, action, valuesChange, gain, visitedStates, timeTaken, avgTime);
        }

        private void WriteActionConsole(
            int i, Player player, IPlayerAction action, GameValuesElement valuesChange,
            double gain, ulong visitedStates, double timeTaken, double avgTime)
        {
            var level = Math.Min(this._game.State.GameInfoState.Level, 3);
            Console.WriteLine("=============================================================");
            Console.WriteLine("{0}, Play: {1}, Level: {2} ({3}/{4})",
                player.Role, i, level, this._game.State.GameInfoState.Population,
                this._game.DomainInfo.Scenario.WinConditions[level].Population);
            Console.WriteLine("\tSt. values: {0}", (GameValuesElement) this._game.State.GameInfoState);
            Console.WriteLine("\tStrategy: {0}", this._game.ActionPlanner.LastPlayerStrategy.ToLongString());
            Console.WriteLine("\tAction: {0}", action);
            Console.WriteLine("\tValues inc: {0}", valuesChange);
            Console.WriteLine("\tGain: {0}", gain.ToString("0.00"));
            Console.WriteLine("\tPredicted: {0}", this._game.ActionPlanner.PredictedGameValues);
            Console.WriteLine("\tProb no action: {0:0.00}, no space: {1:0.00}",
                this._game.ActionPlanner.NoPlayProbability, this._game.ActionPlanner.NoSpaceProbability);
            Console.WriteLine("\tStates : {0}, time: {1:0.00}s {2:0.00} μs/state (avg = {3:0.00})",
                visitedStates, timeTaken, avgTime, this._game.GameStatistics.AvgTimeStateQuantity.Avg);
        }

        private void PrintResults()
        {
            if (this._game == null) return;

            var isTerminal = this._game.Simulator.IsTerminalLevelState();
            var resultsPath = Path.GetFullPath(
                string.Format("{0}/{1:yy-MM-dd@HH-mm-ss}-{2}",
                    EnercitiesAIClient.GAME_LOG_DIR_NAME, DateTime.Now, isTerminal ? "sust" : "unsust"));

            this._game.SerializeToJson(string.Format("{0}/{1}", resultsPath, EnercitiesAIClient.GAME_LOG_FILE_NAME));
            this._game.GameStatistics.PrintResults(resultsPath);

            this.SaveWorldGridImage(string.Format("{0}/world.png", resultsPath));
            foreach (var player in this._initialStrategies.Keys)
                this._initialStrategies[player].Serialize(string.Format("{0}/Strategies/{1}.json", resultsPath, player));

            this._game = null;
        }

        private void SaveWorldGridImage(string filePath)
        {
            if (this.worldPanel.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this, new Action(() => this.SaveWorldGridImage(filePath)));
                return;
            }

            this.worldPanel.SaveImage(filePath, this.worldPanel.Height,
                FormsUtil.GetImageFormat(Path.GetExtension(filePath)));
        }

        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.CreateGame();

            new Task(this.RunAIGamePlays).Start();
        }

        private void LoadGameToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (!this.LoadGame()) return;
            new Task(this.RunHistoryGamePlays).Start();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PauseToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
            {
                this._isPaused = !this._isPaused;
                this.pauseToolStripMenuItem.Text = this._isPaused ? "&Resume" : "&Pause";
            }
        }

        private void SaveToImageToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
            {
                if (!this.saveImgFileDialog.ShowDialog().Equals(DialogResult.OK)) return;
                this.SaveWorldGridImage(this.saveImgFileDialog.FileName);
            }
        }
    }
}