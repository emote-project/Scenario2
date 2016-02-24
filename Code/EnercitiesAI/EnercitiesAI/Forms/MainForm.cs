using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.AI.Game;
using EnercitiesAI.AI.Planning;
using PS.Utilities;
using PS.Utilities.Forms;

namespace EnercitiesAI.Forms
{
    public partial class MainForm : Form
    {
        private const int PLAY_INTERVAL_SECS = 1;
        private const int PLAY_TIMEOUT_SECS = 10;
        private const int PLAY_NEW_LEVEL_INTERVAL_SECS = 4;
        private const int CELL_SIZE = 60;
        private const int MSG_BOX_TIMEOUT = 5;
        private readonly EnercitiesAIClient _aiClient;
        private readonly object _locker = new object();
        private readonly AutoResetEvent _newStateEvent = new AutoResetEvent(false);

        private readonly Dictionary<EnercitiesRole, StrategyControl> _stratControls =
            new Dictionary<EnercitiesRole, StrategyControl>();

        private readonly Dictionary<EnercitiesRole, TabPage> _tabPages = new Dictionary<EnercitiesRole, TabPage>();
        private readonly WorldForm _worldGridForm = new WorldForm(CELL_SIZE) {Visible = false};
        private bool _changedLevel;
        private bool _ignorePlanning;

        public MainForm(EnercitiesAIClient aiClient)
        {
            this._aiClient = aiClient;
            this.InitializeComponent();

            this.PopulateComboBoxes();
            this.CreateTabPages();

            //inits/clears UI
            this.ClearPlanStats();

            //tries to recover play history info
            this.CheckPreviousSession();

            //attaches events from game
            var game = this._aiClient.Game;
            game.StateUpdated += this.OnGameStateUpdated;
            game.GameInitiated += this.OnGameInitiated;
            this._worldGridForm.Game = game;

            this.EnablePlanMenus(false);
            this.SetStatus("Client started");

            this.bestActionsList.DoubleClick += this.BestActionsListDoubleClick;
        }

        private Game Game
        {
            get { return this._aiClient.Game; }
        }

        private ActionPlanner ActionPlanner
        {
            get { return this.Game.ActionPlanner; }
        }

        #region Overrides

        protected override void OnClosing(CancelEventArgs e)
        {
            this._worldGridForm.Close();
            this._worldGridForm.Dispose();

            this._aiClient.Shutdown();
            base.OnClosing(e);
        }

        protected override void OnMove(EventArgs e)
        {
            this._worldGridForm.Invalidate();
            base.OnMove(e);
        }

        #endregion

        #region Recover session, action history replay methods

        private void CheckPreviousSession()
        {
            //tries to recover action history from file
            var gameLogTempFile = Path.GetFullPath(EnercitiesAIClient.GAME_LOG_FILE_NAME);
            if (!File.Exists(gameLogTempFile)) return;

            lock (this._locker)
            {
                var result = TimeOutMessageBox.Show(MSG_BOX_TIMEOUT,
                    "Previous session temporary file found!\nDo you want to load data (e.g., when the AI client crashes)?",
                    "EMOTE EnerCities AI Recovery Options", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result.Equals(DialogResult.Yes))
                    this._aiClient.RecoverGameSession();
                else
                    File.Delete(gameLogTempFile);
            }
        }

        private void AutoMakePlays()
        {
            this.ClearPlanStats();
            this.EnablePlanMenus(false);

            //auto plays (plan and execute best action) until game is over
            var numActions = 0;
            while (!this.Game.Simulator.IsTerminalState())
            {
                this._aiClient.ExecuteBestActionDirectly();

                //allow time for planning to occur..
                this.WaitForNewState();
                numActions++;
            }

            this.SetStatus(string.Format("Finished auto-playing {0} actions", numActions));
            this.EnablePlanMenus(true);
        }

        private void ReplayAllHistory(Game game)
        {
            lock (this._locker) this._ignorePlanning = true;

            this.ClearPlanStats();
            this.EnablePlanMenus(false);

            //recovers all actions 1-by-1, asks user what to do
            var numActionsPlayed = 0;
            var playAll = false;
            foreach (var stateActionPair in game.ActionHistory)
            {
                var action = stateActionPair.Action;
                if (action == null) continue;

                if (!playAll)
                {
                    var result = YesNoAllMessageBox.Show(
                        string.Format("{0}\r\nDo you want to play this action?", action),
                        string.Format("Play all actions ({0} of {1})", numActionsPlayed + 1, game.ActionHistory.Count));

                    if (result.Equals(DialogResult.No)) continue;
                    if (result.Equals(DialogResult.Cancel)) break;
                    if (result.Equals(DialogResult.Retry)) playAll = true;
                }
                else
                {
                    this.WaitForNewState();
                }

                this.SetActionSequenceStatus(action, numActionsPlayed + 1, game.ActionHistory.Count);
                this._aiClient.ExecuteActionDirectly(action);
                numActionsPlayed++;
            }

            this.SetStatus(string.Format("Finished replaying {0} actions", numActionsPlayed));

            lock (this._locker) this._ignorePlanning = false;
            this.EnablePlanMenus(true);
        }

        private void WaitForNewState()
        {
            //waits for a new state to proceed to new play or time-out
            this._newStateEvent.WaitOne(1000*PLAY_TIMEOUT_SECS);

            //waits extra interval time
            Thread.Sleep(1000*(this._changedLevel ? PLAY_NEW_LEVEL_INTERVAL_SECS : PLAY_INTERVAL_SECS));
        }

        #endregion

        #region Planning and AI client handlers

        private void OnGameStateUpdated(EnercitiesGameInfo gameState, EnercitiesGameInfo expectedState)
        {
            if (this.tabControl.InvokeRequired)
            {
                //invokes the same method but on the form's thread
                FormsUtil.InvokeWhenPossible(this, new StateEventHandler(this.OnGameStateUpdated),
                    new object[] {gameState, expectedState}, this._locker);
                return;
            }

            lock (this._locker)
            {
                //selects strategy tab for current player
                var curTabPage = this._tabPages[this._aiClient.Game.State.GameInfoState.CurrentRole];
                this.tabControl.SelectTab(curTabPage);
                foreach (var tabPage in this._tabPages)
                    tabPage.Value.Text = string.Format("{0}{1}", tabPage.Key,
                        (tabPage.Value.Equals(curTabPage) ? "*" : ""));

                this._changedLevel = (this.gameInfoControl.GameInfo != null) &&
                                     (this.gameInfoControl.GameInfo.Level - 1 < gameState.Level);
                this.gameInfoControl.GameInfo = gameState;
                this.SetStatus(string.Format("New game state: {0}", gameState));
            }

            //signals the new state event
            this._newStateEvent.Set();
        }

        private void OnGameInitiated(object sender, EventArgs e)
        {
            if (this.timeTakenTxtBox.InvokeRequired)
            {
                //invokes the same method but on the form's thread
                FormsUtil.InvokeWhenPossible(this, new EventHandler(this.OnGameInitiated), new[] {sender, e},
                    this._locker);
                return;
            }

            lock (this._locker)
            {
                //initializes UI
                this.ClearPlanStats();

                //attaches planning events
                this.ActionPlanner.PlanningFinished += this.OnPlanningFinished;
                this.ActionPlanner.PlanningStarted += this.OnPlanningStarted;

                this.loadGameHistoryToolStripMenuItem.Enabled = true;

                //update players
                foreach (var player in this._aiClient.Game.Players)
                {
                    this._stratControls[player.Key].Player = player.Value;
                    if (this._stratControls[player.Key].InvokeRequired)
                    {
                        var player1 = player;
                        FormsUtil.InvokeWhenPossible(this,
                            (MethodInvoker) (() => this._stratControls[player1.Key].UpdateControls()));
                    }
                    else this._stratControls[player.Key].UpdateControls();
                }

                this.SetStatus("Game initiated");
            }
        }

        private void OnPlanningStarted(object sender, EventArgs e)
        {
            lock (this._locker) if (this._ignorePlanning) return;

            if (this.InvokeRequired)
            {
                //invokes the same method but on the form's thread
                FormsUtil.InvokeWhenPossible(
                    this, new EventHandler(this.OnPlanningStarted), new[] {sender, e}, this._locker);
                return;
            }

            lock (this._locker)
            {
                this.ClearPlanStats();
                this.EnablePlanMenus(false);
                this.Cursor = Cursors.WaitCursor;
                this.SetStatus("Planning started");
            }
        }

        private void OnPlanningFinished(object sender, EventArgs e)
        {
            lock (this._locker) if (this._ignorePlanning) return;

            if (this.timeTakenTxtBox.InvokeRequired)
            {
                //invokes the same method but on the form's thread
                FormsUtil.InvokeWhenPossible(this, new EventHandler(this.OnPlanningFinished), new[] {sender, e},
                    this._locker);
                return;
            }

            lock (this._locker)
            {
                this.ClearPlanStats();
                this.UpdatePlanStats();

                foreach (var strategyControl in this._stratControls)
                {
                    var lastPlayer = this.ActionPlanner.LastPlayer;
                    strategyControl.Value.UpdateControls(
                        lastPlayer != null && strategyControl.Key.Equals(lastPlayer.Role)
                            ? this.ActionPlanner.LastPlayerStrategy
                            : null);
                }

                if (this.ActionPlanner.LastBestAction == null) return;

                this.UpdateActionsInfo();
                this.EnablePlanMenus(true);
                this.Cursor = Cursors.Default;
                this.SetStatus("Planning finished");
            }
        }

        #endregion

        #region UI handling

        #region Menu handling

        private void LoadGameHistoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
                if (this.openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

            //creates dummy game, load history from file
            var filePath = this.openFileDialog.FileName;
            var game = Game.DeserializeFromJson(filePath);
            if (game == null) return;

            this.SetStatus(string.Format("Game loaded from: '{0}'", filePath));

            //replays all actions in history
            new Task(() => this.ReplayAllHistory(game)).Start();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PlayBestToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
            {
                this._aiClient.ExecuteBestActionDirectly();
                this.planToolStripMenuItem.Enabled = false;
            }
        }

        private void SkipTurnToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
                this._aiClient.ExecuteActionDirectly(new SkipTurn());
        }

        private void AutoPlayBestToolStripMenuItemClick(object sender, EventArgs e)
        {
            new Task(this.AutoMakePlays).Start();
        }

        private void PlanToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
                this.ActionPlanner.PlanNextAction();
        }

        private void WorldGridToolStripMenuItemClick(object sender, EventArgs e)
        {
            this._worldGridForm.Visible =
                this.worldGridToolStripMenuItem.Checked =
                    !this.worldGridToolStripMenuItem.Checked;
            this.Focus();
        }

        private void EnablePlanMenus(bool enable)
        {
            if (this.menuStrip.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(
                    this, new Action<bool>(this.EnablePlanMenus), new object[] {enable}, this._locker);
                return;
            }

            this.actionToolStripMenuItem.Enabled =
                this.optionsToolStripMenuItem.Enabled = enable;
        }

        #endregion

        #region Planning combo boxes

        private void CBoxPlanTimeSelectedIdxChanged(object sender, EventArgs e)
        {
            this.ActionPlanner.MaxProcessTime = (int) this.cBoxPlanTime.SelectedItem;
        }

        private void CBoxPlanDepthSelectedIdxChanged(object sender, EventArgs e)
        {
            this.ActionPlanner.MaxPlanningDepth = (int) this.cBoxPlanDepth.SelectedItem;
        }

        private void CBoxPlanCPUsSelectedIdxChanged(object sender, EventArgs e)
        {
            this.ActionPlanner.MaxCPUs = (uint) this.cBoxPlanCPUs.SelectedItem;
        }

        private void CBoxImitFactorSelectedIdxChanged(object sender, EventArgs e)
        {
            this.Game.SocialImitationFactor = (double) this.cBoxImitFactor.SelectedItem;
        }

        private void CBoxStratLearnSelectedIdxChanged(object sender, EventArgs e)
        {
            this.Game.StrategyLearningRate = (double) this.cBoxStratLearn.SelectedItem;
        }

        #endregion

        private void BestActionsListDoubleClick(object sender, EventArgs e)
        {
            lock (this._locker)
                this._aiClient.ExecuteActionDirectly((IPlayerAction) this.bestActionsList.SelectedItems[0].Tag);
        }

        private void SetStatus(string text)
        {
            if (this.statusStrip.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(
                    this, new Action<string>(this.SetStatus), new object[] {text}, this._locker);
                return;
            }
            this.statusLabel.Text = string.Format("[{0}] - {1}", DateTime.Now.ToShortTimeString(), text);
        }

        private void SetActionSequenceStatus(IPlayerAction action, int numActionsPlayed, int totalActions)
        {
            this.SetStatus(string.Format("Action {0} of {1} played: {2}",
                numActionsPlayed, totalActions, action.ToString().Replace("\r\n", "|")));
        }

        #endregion

        #region Display planning info methods

        private void CreateTabPages()
        {
            this.tabControl.TabPages.Clear();
            this._tabPages.Clear();
            this._stratControls.Clear();

            //adds one strategy control in a separate tab per player
            foreach (var role in this._aiClient.Game.PlayersOrder)
            {
                var tab = new TabPage {Text = role.ToString()};
                var strategyCtrl = new StrategyControl {Enabled = false, Dock = DockStyle.Fill};
                this._tabPages.Add(role, tab);
                this._stratControls.Add(role, strategyCtrl);
                this.tabControl.TabPages.Add(tab);
                tab.Controls.Add(strategyCtrl);
            }
        }

        private void PopulateComboBoxes()
        {
            //adds CPUs
            for (var i = 1u; i <= ProcessUtil.GetCPUCount(); i++)
                this.cBoxPlanCPUs.Items.Add(i);

            //adds plan depth
            for (var i = 0; i < 10; i++)
                this.cBoxPlanDepth.Items.Add(i);

            //adds plan time
            for (var i = 5; i < 60; i += 5)
                this.cBoxPlanTime.Items.Add(i);

            //adds learn / imitation factor
            for (var i = 0d; i <= 1; i += 0.2)
            {
                this.cBoxStratLearn.Items.Add(i);
                this.cBoxImitFactor.Items.Add(i);
            }
        }

        private void UpdateActionsInfo()
        {
            this.bestActionTxtBox.Text = this.GetBestActionText(this.ActionPlanner.LastBestAction);

            //prints info on other best actions
            var actionIdx = 0;
            foreach (var lastBestAction in this.ActionPlanner.LastBestActions)
            {
                if (actionIdx++ == 0) continue;

                var action = lastBestAction.Key;
                if (action is UpgradeStructures)
                {
                    var upgradeStructures = ((UpgradeStructures) action).Upgrades;
                    for (var i = 0; i < upgradeStructures.Count; i++)
                    {
                        var actionStr = string.Format("{0}{1}{2}",
                            i == 0 ? "<" : "  ", upgradeStructures[i],
                            i == upgradeStructures.Count - 1
                                ? string.Format(">: {0:0.00}", lastBestAction.Value)
                                : ";");
                        var item = this.bestActionsList.Items.Add(actionStr);
                        item.Tag = action;
                    }
                }
                else
                {
                    var item = this.bestActionsList.Items.Add(this.GetBestActionText(action));
                    item.Tag = action;
                }
            }
        }

        private string GetBestActionText(IPlayerAction action)
        {
            return string.Format("{0}: {1:0.00}", action, this.ActionPlanner.LastBestActions[action]);
        }

        private void UpdatePlanStats()
        {
            //fills form information from action planner
            this.cBoxPlanTime.Text = this.ActionPlanner.MaxProcessTime.ToString(CultureInfo.InvariantCulture);
            this.cBoxPlanDepth.Text = this.ActionPlanner.MaxPlanningDepth.ToString(CultureInfo.InvariantCulture);
            this.cBoxPlanCPUs.Text = this.ActionPlanner.MaxCPUs.ToString(CultureInfo.InvariantCulture);
            this.cBoxImitFactor.Text = this.Game.SocialImitationFactor.ToString(CultureInfo.InvariantCulture);
            this.cBoxStratLearn.Text = this.Game.StrategyLearningRate.ToString(CultureInfo.InvariantCulture);

            var lastTimeTaken = this.ActionPlanner.LastTimeTaken;
            this.timeTakenTxtBox.Text = lastTimeTaken.ToString("0.00");
            var lastNumVisitedStates = this.ActionPlanner.LastNumVisitedStates;
            var avgTimeState = lastTimeTaken*1000000/lastNumVisitedStates;
            this.avgTimeStateTxtBox.Text = avgTimeState.ToString("0.00");
            this.statesVisitedTxtBox.Text = lastNumVisitedStates.ToString(CultureInfo.InvariantCulture);
            this.expandedStatesTxtBox.Text =
                this.ActionPlanner.LastNumExpandedStates.ToString(CultureInfo.InvariantCulture);

            var predictedGameValues = this.ActionPlanner.PredictedGameValues;
            this.predictedValuesTxtBox.Text = string.Format(
                "Pop:{0}, Mon:{1:0}, Oil:{2:0}, PwCons:{3:0.0}, PwProd:{4:0.0}, EnvScr:{5:0.0}, EcoScr:{6:0.0}, " +
                "WellScr:{7:0.0}, ScrUni:{8:0.0}, NoActProb:{9:0.00}, NoSpaceProb:{10:0.0}",
                predictedGameValues.Homes,
                predictedGameValues.Money,
                predictedGameValues.Oil,
                predictedGameValues.PowerConsumption,
                predictedGameValues.PowerProduction,
                predictedGameValues.EnvironmentScore,
                predictedGameValues.EconomyScore,
                predictedGameValues.WellbeingScore,
                predictedGameValues.ScoresUniformity,
                this.ActionPlanner.NoPlayProbability,
                this.ActionPlanner.NoSpaceProbability);
        }

        private void ClearPlanStats()
        {
            if (this.timeTakenTxtBox.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this, new Action(this.ClearPlanStats), null, this._locker);
                return;
            }

            this.timeTakenTxtBox.Text =
                this.statesVisitedTxtBox.Text =
                    this.expandedStatesTxtBox.Text =
                        this.bestActionTxtBox.Text =
                            this.predictedValuesTxtBox.Text =
                                this.avgTimeStateTxtBox.Text = string.Empty;
            this.bestActionsList.Items.Clear();
        }

        #endregion
    }
}