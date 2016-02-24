using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SpeakerDetector
{
    public partial class frmSpeakerDetector : Form
    {
        SpeakerDetectorClient thalamusClient;
        bool dontUpdate = false;

        public frmSpeakerDetector(string characterName)
        {
            InitializeComponent();
            thalamusClient = new SpeakerDetectorClient(characterName);
            thalamusClient.SpeakerInformation += SetText;
            dontUpdate = true;
            numDecibelsDifference.Value = Convert.ToDecimal(thalamusClient.DecibelDifference);
            numDecibelsThreshold.Value = Convert.ToDecimal(thalamusClient.DecibelThreshold);
            dontUpdate = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thalamusClient.Dispose();
        }

        public void SetText(EmoteCommonMessages.ActiveUser activeSpeaker, double leftDecibels, double rightDecibels)
        {
            string leftUserText = leftDecibels < -meterLeft.Maximum ? "-oo" : (Math.Truncate(leftDecibels).ToString() + "dB");
            string rightUserText = rightDecibels < -meterRight.Maximum ? "-oo" : (Math.Truncate(rightDecibels).ToString() + "dB");

            leftDecibels = Math.Max(0, Math.Min(-leftDecibels, meterLeft.Maximum));
            rightDecibels = Math.Max(0, Math.Min(-rightDecibels, meterRight.Maximum));

            meterLeft.Value = (int)Math.Round(meterLeft.Maximum-leftDecibels);
            meterRight.Value = (int)Math.Round(meterRight.Maximum-rightDecibels);

            try
            {
                this.Invoke((MethodInvoker)(() =>
                {

                    if (lastuser.Text != activeSpeaker.ToString()) lastuser.Text = activeSpeaker.ToString();
                    if (leftuser.Text != leftUserText)
                    {
                        leftuser.Text = leftUserText;
                        Color bg = activeSpeaker == EmoteCommonMessages.ActiveUser.Left || activeSpeaker == EmoteCommonMessages.ActiveUser.Both ? Color.YellowGreen : SystemColors.Control;
                        leftuser.BackColor = bg;
                        pnlLeftUser.BackColor = bg;
                    }
                    if (rightuser.Text != rightUserText)
                    {
                        rightuser.Text = rightUserText;
                        Color bg = activeSpeaker == EmoteCommonMessages.ActiveUser.Right || activeSpeaker == EmoteCommonMessages.ActiveUser.Both ? Color.YellowGreen : SystemColors.Control;
                        rightuser.BackColor = bg;
                        pnlRightUser.BackColor = bg;
                    }
                }));
            }
            catch { }
        }

        private void numDecibelsThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            thalamusClient.DecibelThreshold = Convert.ToDouble(numDecibelsThreshold.Value);
        }

        private void numDecibelsDifference_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            thalamusClient.DecibelDifference = Convert.ToDouble(numDecibelsDifference.Value);
        }

        private void frmSpeakerDetector_Load(object sender, EventArgs e)
        {
            refreshDevicesList();
            if (cmbAudioDevices.Items.Count > 0) thalamusClient.StartRecording(cmbAudioDevices.SelectedIndex != -1 ? cmbAudioDevices.SelectedIndex : 0);
            else MessageBox.Show("No Audio device detected!", "Active Speaker Detector", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void refreshDevicesList()
        {
            Dictionary<string, int> devices = thalamusClient.GetDevices();
            cmbAudioDevices.Items.Clear();
            foreach (string deviceName in devices.Keys)
            {
                cmbAudioDevices.Items.Add(deviceName);
                dontUpdate = true;
                if (deviceName == Properties.Settings.Default.DeviceName) cmbAudioDevices.SelectedIndex = devices[deviceName];
                dontUpdate = false;
            }
            if (cmbAudioDevices.SelectedIndex == -1 && cmbAudioDevices.Items.Count>0)
            {
                dontUpdate = true;
                cmbAudioDevices.SelectedIndex = 0;
                dontUpdate = false;
            }
        }

        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            refreshDevicesList();
        }

        private void cmbAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            if (cmbAudioDevices.SelectedIndex != -1)
            {
                thalamusClient.StartRecording(cmbAudioDevices.SelectedIndex);
                Properties.Settings.Default.DeviceName = cmbAudioDevices.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            }
        }


        private void cmbAudioDevices_MouseClick(object sender, MouseEventArgs e)
        {
            refreshDevicesList();
        }


        //old method

        /*private double AudioThresh = 0.5;
        private double AudioThresh2 = 0.3;

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            int channelactive = -1; //-1 for left +1 for right
            float leftc = 0;
            float rightc = 0;

            bool trLeft = false;
            bool trRight = false;

            int Count = e.BytesRecorded / (2 * 2);
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                if (channelactive == -1) //left channel
                {
                    leftc += sample32 * sample32;
                    channelactive = 1;
                    if (sample32 > AudioThresh)
                        trLeft = true;
                }
                else
                {
                    rightc += sample32 * sample32;
                    channelactive = -1;
                    if (sample32 > AudioThresh)
                        trRight = true;
                }
            }
            leftc /= Count;
            rightc /= Count;
            leftuser.Text = leftc.ToString();
            rightuser.Text = rightc.ToString();

            if (trLeft || leftc > AudioThresh2)
            {
                trLeft = true;
            }
            else
            {
                trLeft = false;
            }

            if (trRight || rightc > AudioThresh2)
            {
                trRight = true;
            }
            else
            {
                trRight = false;
            }
        }*/

    }
}
