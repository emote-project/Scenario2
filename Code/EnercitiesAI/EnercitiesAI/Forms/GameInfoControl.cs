using System;
using System.Globalization;
using System.Windows.Forms;
using EmoteEvents;

namespace EnercitiesAI.Forms
{
    public partial class GameInfoControl : UserControl
    {
        private bool _allowChanges;
        private EnercitiesGameInfo _gameInfo;

        public GameInfoControl()
        {
            this._gameInfo = new EnercitiesGameInfo();
            this.InitializeComponent();
            this.AttachTrackBarTxtBoxes();
            this.Update();
        }

        public EnercitiesGameInfo GameInfo
        {
            get { return this._gameInfo; }
            set
            {
                if (value == null) return;
                this._gameInfo = value;
                this.Update();
            }
        }

        public bool AllowChanges
        {
            get { return this._allowChanges; }
            set
            {
                this._allowChanges =
                    this.econTB.Enabled =
                        this.envTB.Enabled =
                            this.wellTB.Enabled =
                                this.monTB.Enabled =
                                    this.oilTB.Enabled =
                                        this.powTB.Enabled =
                                            this.popTB.Enabled =
                                                this.levelTB.Enabled = value;
            }
        }

        public event EventHandler<EnercitiesGameInfo> GameInfoChanged;

        public new void Update()
        {
            this.UpdateTrackBars();
            this.UpdateTextBoxes();
        }

        private void TrackBarScroll(object sender, EventArgs e)
        {
            this.UpdateGameInfo();
        }

        private void AttachTrackBarTxtBoxes()
        {
            this.econTxtBox.Tag = this.econTB;
            this.envTxtBox.Tag = this.envTB;
            this.wellTxtBox.Tag = this.wellTB;
            this.monTxtBox.Tag = this.monTB;
            this.oilTxtBox.Tag = this.oilTB;
            this.powTxtBox.Tag = this.powTB;
            this.popTxtBox.Tag = this.popTB;
            this.levelTxtBox.Tag = this.levelTB;
        }

        private void UpdateTrackBars()
        {
            SetTrackBarValue(this.econTB, this.GameInfo.EconomyScore);
            SetTrackBarValue(this.envTB, this.GameInfo.EnvironmentScore);
            SetTrackBarValue(this.wellTB, this.GameInfo.WellbeingScore);
            SetTrackBarValue(this.monTB, this.GameInfo.Money);
            SetTrackBarValue(this.oilTB, this.GameInfo.Oil);
            SetTrackBarValue(this.powTB, this.GameInfo.PowerProduction - this.GameInfo.PowerConsumption);
            SetTrackBarValue(this.popTB, this.GameInfo.Population);
            SetTrackBarValue(this.levelTB, this.GameInfo.Level + 1);
        }

        private static void SetTrackBarValue(TrackBar trackBar, double value)
        {
            //sets trackbar value and possibly adjusts max and min
            var intValue = (int) value;
            if (intValue < trackBar.Minimum)
                trackBar.Minimum = intValue;
            else if (intValue > trackBar.Maximum)
                trackBar.Maximum = intValue;
            trackBar.Value = intValue;
        }

        private void UpdateTextBoxes()
        {
            this.econTxtBox.Text = this.GameInfo.EconomyScore.ToString(CultureInfo.InvariantCulture);
            this.envTxtBox.Text = this.GameInfo.EnvironmentScore.ToString(CultureInfo.InvariantCulture);
            this.wellTxtBox.Text = this.GameInfo.WellbeingScore.ToString(CultureInfo.InvariantCulture);
            this.monTxtBox.Text = this.GameInfo.Money.ToString(CultureInfo.InvariantCulture);
            this.oilTxtBox.Text = ((int) this.GameInfo.Oil).ToString(CultureInfo.InvariantCulture);
            this.powTxtBox.Text =
                (this.GameInfo.PowerProduction - this.GameInfo.PowerConsumption).ToString(CultureInfo.InvariantCulture);
            this.popTxtBox.Text = this.GameInfo.Population.ToString(CultureInfo.InvariantCulture);
            this.levelTxtBox.Text = (this.GameInfo.Level + 1).ToString(CultureInfo.InvariantCulture);
        }

        private void UpdateGameInfo()
        {
            this.GameInfo.EconomyScore = this.econTB.Value;
            this.GameInfo.EnvironmentScore = this.envTB.Value;
            this.GameInfo.WellbeingScore = this.wellTB.Value;
            this.GameInfo.Money = this.monTB.Value;
            this.GameInfo.Oil = this.oilTB.Value;
            this.GameInfo.PowerProduction = this.powTB.Value;
            this.GameInfo.PowerConsumption = 0;
            this.GameInfo.Population = this.popTB.Value;
            this.GameInfo.Level = this.levelTB.Value - 1;

            this.Update();

            if (this.GameInfoChanged != null)
                this.GameInfoChanged(this, this.GameInfo);
        }

        private void TxtBoxDoubleClicked(object sender, EventArgs e)
        {
            var tb = (TrackBar) ((TextBox) sender).Tag;
            tb.Value = tb.Maximum;

            this.Update();
        }
    }
}