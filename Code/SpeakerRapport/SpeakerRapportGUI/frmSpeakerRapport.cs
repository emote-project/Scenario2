using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpeakerRapport;

namespace SpeakerRapportGUI
{
    public partial class frmSpeakerRapport : Form
    {
        SpeakerRapportClient client;
        bool dontUpdate = false;
        public frmSpeakerRapport(string character)
        {
            InitializeComponent();

            client = new SpeakerRapportClient(character);
            dontUpdate = true;
            numBaseSpeakerDecibelThreshold.Value = Convert.ToDecimal(client.BaseSpeakerDecibelThreshold);
            numBaseVolumeLevel.Value = Convert.ToDecimal(client.BaseVolumeLevel)*100;
            numGazeShiftMinimumInterval.Value = Convert.ToDecimal(client.GazeShiftMinimumInterval);
            txtTestText.Text = client.TestText;
            chkRunTest.Checked = client.RunTest;
            radGaze.Checked = client.GazingBehavior == SpeakerRapportClient.GazingType.Gaze;
            radGlance.Checked = client.GazingBehavior == SpeakerRapportClient.GazingType.Glance;
            dontUpdate = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Dispose();
        }

        private void numGazeShiftMinimumInterval_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.GazeShiftMinimumInterval = Convert.ToInt32(numGazeShiftMinimumInterval.Value);
        }

        private void numBaseVolumeLevel_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.BaseVolumeLevel = Convert.ToDouble(numBaseVolumeLevel.Value) / 100.0f;
        }

        private void numBaseSpeakerDecibelThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.BaseSpeakerDecibelThreshold = Convert.ToDouble(numBaseSpeakerDecibelThreshold.Value);
        }

        private void txtTestText_TextChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.TestText = txtTestText.Text;
        }

        private void chkRunTest_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.RunTest = chkRunTest.Checked;
        }

        private void radGaze_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.GazingBehavior = SpeakerRapportClient.GazingType.Gaze;
        }

        private void radGlance_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            client.GazingBehavior = SpeakerRapportClient.GazingType.Glance;
        }
    }
}
