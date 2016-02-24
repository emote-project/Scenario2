using System;
using System.IO;
using System.Windows.Forms;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI;
using EnercitiesAI.AI;
using EnercitiesAI.AI.Planning;
using EnercitiesAI.Domain;
using OxyPlot;
using PS.Utilities;

namespace StrategyEditor
{
    public partial class MainForm : Form
    {
        private readonly DomainInfo _domainInfo = new DomainInfo();
        private readonly Player _player = new Player(EnercitiesRole.Economist, new Strategy());

        public MainForm()
        {
            this.InitializeComponent();
            this._domainInfo.Load(EnercitiesAIClient.XML_BASE_PATH);
            
            this.EnableElements(false);
            
            this.gameInfoControl.GameInfo = new EnercitiesGameInfo
                                            {
                                                EconomyScore = 2,
                                                EnvironmentScore = 2,
                                                WellbeingScore = 2,
                                                Oil = 1800,
                                                Population = 1,
                                                Money = 100,
                                                PowerProduction = 4,
                                                Level = 0
                                            };
            this.gameInfoControl.GameInfoChanged += this.OnGameInfoChanged;
            this.CreateStrategyAdjustControls();
            this.strategyControl.StrategyChanged += this.OnStrategyChanged;
            this.strategyControl.Player = this._player;
        }

        private void CreateStrategyAdjustControls()
        {
            var palette = OxyPalettes.Rainbow(EnumUtil<ParamType>.GetTypes().Length);
            this.tabCtrlStratParams.TabPages.Clear();
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Money, palette.Colors[0], 0.9, 0.09, 0, 100));
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Oil, palette.Colors[1], 0.99, 0.009, 0, 1800));
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Power, palette.Colors[2], 0.5, 0.49, 0, 50));
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Score, palette.Colors[3], 0.5, 0.4, 0, 20));
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Homes, palette.Colors[4], 0.001, 0.299, 0, 2));
            this.tabCtrlStratParams.TabPages.Add(
                this.CreateAdjustTabPage(ParamType.Environment, palette.Colors[5], 0.15, 0.84, 0, 20));
        }

        private TabPage CreateAdjustTabPage(
            ParamType paramType, OxyColor color, double paramMin, double paramRange, int xMin, int xMax)
        {
            var tabPage = new TabPage(paramType.ToString());
            var stratAdjustControl =
                new StrategyAdjustmentControl(
                    this._domainInfo, this.gameInfoControl.GameInfo, this._player,
                    color, paramType, paramMin, paramRange, xMin, xMax)
                {
                    Dock = DockStyle.Fill
                };
            stratAdjustControl.ParamValueChanged += this.OnParamValueChanged;
            tabPage.Controls.Add(stratAdjustControl);
            return tabPage;
        }

        private void EnableElements(bool enable)
        {
            this.roleToolStripMenuItem.Enabled =
                this.saveToolStripMenuItem.Enabled =
                    this.saveasToolStripMenuItem.Enabled =
                        this.groupBox1.Enabled =
                            this.groupBox2.Enabled =
                                this.groupBox3.Enabled =
                                    this.groupBox4.Enabled = enable;
        }

        private void LoadStrategy(Strategy strategy, string fileName)
        {
            this._player.Strategy = strategy;
            this.strategyControl.UpdateControls();
            this.UpdateParamControls();
            this.UpdateRole(fileName.Contains("eco")
                ? EnercitiesRole.Economist
                : fileName.Contains("env")
                    ? EnercitiesRole.Environmentalist
                    : EnercitiesRole.Mayor);
        }

        private void UpdateParamControls()
        {
            foreach (var tabPage in this.tabCtrlStratParams.TabPages)
                ((StrategyAdjustmentControl) ((TabPage) tabPage).Controls[0]).Update();
        }

        private void SaveStrategy()
        {
            if (this._player.Strategy == null) return;
            this._player.Strategy.Serialize(this.saveFileDialog.FileName);
        }

        private void UpdateRole(EnercitiesRole role)
        {
            this._player.Role = role;
            this.UpdateSimulatedStrategy();
            this.UpdateText();
            this.economistToolStripMenuItem.Checked = role.Equals(EnercitiesRole.Economist);
            this.environmentalistToolStripMenuItem.Checked = role.Equals(EnercitiesRole.Environmentalist);
            this.mayorToolStripMenuItem.Checked = role.Equals(EnercitiesRole.Mayor);
        }

        private void UpdateText(bool noData = false)
        {
            this.Text = string.Format("EMOTE EnerCities Strategy Editor{0}",
                noData
                    ? ""
                    : string.Format(" - '{0}' - {1}", Path.GetFileName(this.saveFileDialog.FileName), this._player.Role));
        }

        private void UpdateSimulatedStrategy()
        {
            var level = this.gameInfoControl.GameInfo.Level;
            var homesNeedRatio =
                1d - ((double) this.gameInfoControl.GameInfo.Population/
                      this._domainInfo.Scenario.WinConditions[level].Population);

            var strategy = StrategyAdjustment.GetAdjustedStrategy(
                this._player.Role, this._player.Strategy, this.gameInfoControl.GameInfo,
                ActionPlanner.DEF_STRAT_ADJUST, 2*(1 - homesNeedRatio));

            this.UpdateTextBoxes(strategy);
        }

        private void UpdateTextBoxes(Strategy strategy)
        {
            this.econTxtBox.Text = string.Format("{0:0.00}", strategy.EconomyWeight);
            this.envTxtBox.Text = string.Format("{0:0.00}", strategy.EnvironmentWeight);
            this.wellTxtBox.Text = string.Format("{0:0.00}", strategy.WellbeingWeight);
            this.monTxtBox.Text = string.Format("{0:0.00}", strategy.MoneyWeight);
            this.oilTxtBox.Text = string.Format("{0:0.00}", strategy.OilWeight);
            this.powTxtBox.Text = string.Format("{0:0.00}", strategy.PowerWeight);
            this.homeTxtBox.Text = string.Format("{0:0.00}", strategy.HomesWeight);
            this.uniformTxtBox.Text = string.Format("{0:0.00}", strategy.ScoreUniformityWeight);
        }

        #region Event handling

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.OK) return;
            var fileName = this.openFileDialog.FileName;
            var strategy = StrategyExtensions.Deserialize(fileName);
            if (strategy == null)
            {
                this.EnableElements(false);
                this.UpdateText(true);
                return;
            }

            this.saveFileDialog.FileName = fileName;
            this.EnableElements(true);
            this.LoadStrategy(strategy, fileName);
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.SaveStrategy();
        }

        private void SaveasToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() != DialogResult.OK) return;
            this.SaveStrategy();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnStrategyChanged(object sender, Strategy e)
        {
            this.UpdateSimulatedStrategy();
        }

        private void OnGameInfoChanged(object sender, EnercitiesGameInfo e)
        {
            this.UpdateSimulatedStrategy();
            this.UpdateParamControls();
        }

        private void OnParamValueChanged(object sender, double e)
        {
            this.UpdateSimulatedStrategy();
        }

        private void EconomistToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.UpdateRole(EnercitiesRole.Economist);
        }

        private void EnvironmentalistToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.UpdateRole(EnercitiesRole.Environmentalist);
        }

        private void MayorToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.UpdateRole(EnercitiesRole.Mayor);
        }

        #endregion
    }
}