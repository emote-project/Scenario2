using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EmoteEvents;
using EnercitiesAI.AI;
using PS.Utilities.Forms;

namespace EnercitiesAI.Forms
{
    public partial class StrategyControl : UserControl
    {
        private const int SCALE = 1000;
        private List<TrackBar> _trackBars;

        public StrategyControl()
        {
            this.InitializeComponent();
            this.AttachTrackBarTxtBoxes();
            this.CreateTrackBarList();
        }

        public Player Player { get; set; }

        private Strategy Strategy { get; set; }
        public event EventHandler<Strategy> StrategyChanged;

        private void CreateTrackBarList()
        {
            this._trackBars = new List<TrackBar>
                              {
                                  this.econTB,
                                  this.envTB,
                                  this.wellTB,
                                  this.monTB,
                                  this.powTB,
                                  this.oilTB,
                                  this.homeTB,
                                  this.uniformTB
                              };
        }

        private void TrackBarScroll(object sender, EventArgs e)
        {
            lock (this.Player)
                this.UpdateStrategy();
        }

        private void AttachTrackBarTxtBoxes()
        {
            this.econTxtBox.Tag = this.econTB;
            this.envTxtBox.Tag = this.envTB;
            this.wellTxtBox.Tag = this.wellTB;
            this.monTxtBox.Tag = this.monTB;
            this.oilTxtBox.Tag = this.oilTB;
            this.powTxtBox.Tag = this.powTB;
            this.homeTxtBox.Tag = this.homeTB;
            this.uniformTxtBox.Tag = this.uniformTB;
        }

        private void UpdateTrackBars()
        {
            if (this.econTB.InvokeRequired)
            {
                //invokes the same method but on the form's thread
                FormsUtil.InvokeWhenPossible(this, new Action(this.UpdateTrackBars), new object[] {});
                return;
            }

            this.econTB.Value = (int) (this.Strategy.EconomyWeight*SCALE);
            this.envTB.Value = (int) (this.Strategy.EnvironmentWeight*SCALE);
            this.wellTB.Value = (int) (this.Strategy.WellbeingWeight*SCALE);
            this.monTB.Value = (int) (this.Strategy.MoneyWeight*SCALE);
            this.oilTB.Value = (int) (this.Strategy.OilWeight*SCALE);
            this.powTB.Value = (int) (this.Strategy.PowerWeight*SCALE);
            this.homeTB.Value = (int) (this.Strategy.HomesWeight*SCALE);
            this.uniformTB.Value = (int) (this.Strategy.ScoreUniformityWeight*SCALE);
        }

        private void UpdateTextBoxes()
        {
            this.econTxtBox.Text = string.Format("{0:0.00}", this.Strategy.EconomyWeight);
            this.envTxtBox.Text = string.Format("{0:0.00}", this.Strategy.EnvironmentWeight);
            this.wellTxtBox.Text = string.Format("{0:0.00}", this.Strategy.WellbeingWeight);
            this.monTxtBox.Text = string.Format("{0:0.00}", this.Strategy.MoneyWeight);
            this.oilTxtBox.Text = string.Format("{0:0.00}", this.Strategy.OilWeight);
            this.powTxtBox.Text = string.Format("{0:0.00}", this.Strategy.PowerWeight);
            this.homeTxtBox.Text = string.Format("{0:0.00}", this.Strategy.HomesWeight);
            this.uniformTxtBox.Text = string.Format("{0:0.00}", this.Strategy.ScoreUniformityWeight);
        }

        private void UpdateStrategy()
        {
            this.Strategy.EconomyWeight = (double) this.econTB.Value/SCALE;
            this.Strategy.EnvironmentWeight = (double) this.envTB.Value/SCALE;
            this.Strategy.WellbeingWeight = (double) this.wellTB.Value/SCALE;
            this.Strategy.MoneyWeight = (double) this.monTB.Value/SCALE;
            this.Strategy.OilWeight = (double) this.oilTB.Value/SCALE;
            this.Strategy.PowerWeight = (double) this.powTB.Value/SCALE;
            this.Strategy.HomesWeight = (double) this.homeTB.Value/SCALE;
            this.Strategy.ScoreUniformityWeight = (double) this.uniformTB.Value/SCALE;

            this.Strategy.Normalize();

            this.UpdateTrackBars();
            this.UpdateTextBoxes();

            if (this.StrategyChanged != null)
                this.StrategyChanged(this, this.Strategy);
        }

        public void UpdateControls(Strategy strategy = null)
        {
            this.Enabled = this.Player != null;

            if (this.Player == null) return;

            this.Strategy = strategy ?? this.Player.Strategy;
            this.UpdateTrackBars();
            this.UpdateTextBoxes();
        }

        private void TxtBoxDoubleClicked(object sender, EventArgs e)
        {
            var tb = (TrackBar) ((TextBox) sender).Tag;
            for (var i = 0; i < this._trackBars.Count; i++)
                this.Strategy.Weights[i] = this._trackBars[i].Equals(tb) ? 1 : 0;

            this.Strategy.Normalize();

            this.UpdateTrackBars();
            this.UpdateTextBoxes();
        }
    }
}