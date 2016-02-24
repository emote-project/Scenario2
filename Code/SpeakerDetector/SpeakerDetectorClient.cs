using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;
using EmoteCommonMessages;
using NAudio;
using NAudio.Wave;

namespace SpeakerDetector
{
    public interface ISpeakerDetectorPulisher : IThalamusPublisher, ISoundLocalizationEvents
    { }

    public interface ISpeakerDetectorClient  { }
    public class SpeakerDetectorClient : ThalamusClient, ISpeakerDetectorClient
    {
        public delegate void SpeakerInformationHandler(EmoteCommonMessages.ActiveUser activeSpeaker, double leftDecibels, double rightDecibels);
        public event SpeakerInformationHandler SpeakerInformation;
        private void NotifySpeakerInformation(EmoteCommonMessages.ActiveUser activeSpeaker, double leftDecibels, double rightDecibels)
        {
            if (SpeakerInformation != null) SpeakerInformation(activeSpeaker, leftDecibels, rightDecibels);
        }

        public class SpeakerDetectorPublisher : ISpeakerDetectorPulisher
        {
            dynamic publisher;
            public SpeakerDetectorPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void ActiveSoundUser(ActiveUser userAct, double LeftValue, double RightValue)
            {
                publisher.ActiveSoundUser(userAct, LeftValue, RightValue);
            }
        }
        public SpeakerDetectorPublisher LocalizationPublisher;

        private double decibelThreshold = -12.0;
        public double DecibelThreshold
        {
            get { return decibelThreshold; }
            set {
                Properties.Settings.Default.DecibelThreshold = value;
                Properties.Settings.Default.Save();
                decibelThreshold = value; 
            }
        }
        private double decibelDifference = 6.0;
        public double DecibelDifference
        {
            get { return decibelDifference; }
            set {
                Properties.Settings.Default.DecibelDifference = value;
                Properties.Settings.Default.Save();
                decibelDifference = value; 
            }
        }

        WaveIn waveIn = null;
        private bool leftSpeaking = false;
        private bool rightSpeaking = false;

        private EmoteCommonMessages.ActiveUser activeSpeaker = ActiveUser.None;

        private double lastLeftLevel = -100;
        private double lastRightLevel = -100;

        public SpeakerDetectorClient(string character = "") :
            base("SoundLocalization", character)
        {
            SetPublisher<ISpeakerDetectorPulisher>();
            LocalizationPublisher = new SpeakerDetectorPublisher(Publisher);

            decibelThreshold = Properties.Settings.Default.DecibelThreshold;
            decibelDifference = Properties.Settings.Default.DecibelDifference;
        }

        public Dictionary<string, int> GetDevices()
        {
            Dictionary<string, int> devices = new Dictionary<string, int>();
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device " + waveInDevice + ": " + deviceInfo.ProductName + "," + deviceInfo.Channels + "channels");
                devices[deviceInfo.ProductName] = waveInDevice;
            }
            return devices;
        }

        public void StartRecording(int deviceNumber = 0)
        {
            if (waveIn != null) waveIn.Dispose();
            waveIn = new WaveIn();
            waveIn.DeviceNumber = deviceNumber;
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_Decibels);
            waveIn.BufferMilliseconds = 300;
            int sampleRate = 8000; // 8 kHz
            int channels = 2; // stereo
            waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            waveIn.StartRecording();
        }

        private double CalculateDecibel(byte[] buffer)
        {
            double sum = 0;
            for (var i = 0; i < buffer.Length; i = i + 2)
            {
                double sample = BitConverter.ToInt16(buffer, i) / 32768.0;
                sum += (sample * sample);
            }
            double rms = Math.Sqrt(sum / (buffer.Length / 2));
            var decibel = 20 * Math.Log10(rms);
            return decibel;
        }

        void waveIn_Decibels(object sender, WaveInEventArgs e)
        {
            int Count = e.BytesRecorded / (2 * 2);
            byte[] leftBuffer = new byte[e.BytesRecorded / 2];
            byte[] rightBuffer = new byte[e.BytesRecorded / 2];

            //split the channels
            for (int i = 0; i < e.BytesRecorded; i += 4)
            {
                int bufferIndex = (i / 4) * 2;
                leftBuffer[bufferIndex] = e.Buffer[i];
                leftBuffer[bufferIndex + 1] = e.Buffer[i + 1];
                rightBuffer[bufferIndex] = e.Buffer[i + 2];
                rightBuffer[bufferIndex + 1] = e.Buffer[i + 3];
            }
            double leftDb = CalculateDecibel(leftBuffer);
            double rightDb = CalculateDecibel(rightBuffer);

            bool leftActive = leftSpeaking || leftDb > decibelThreshold;
            if (leftSpeaking && leftDb < (decibelThreshold - decibelDifference)) leftActive = false;

            bool rightActive = rightSpeaking || rightDb > decibelThreshold;
            if (rightSpeaking && rightDb < (decibelThreshold - decibelDifference)) rightActive = false;

            SelectActiveSpeaker(leftActive, rightActive, leftDb, rightDb);
        }



        private void SelectActiveSpeaker(bool leftActive, bool rightActive, double leftDecibels, double rightDecibels)
        {
            EmoteCommonMessages.ActiveUser selectedSpeaker;
            if (leftActive && (!rightActive || (rightDecibels - leftDecibels) < -decibelDifference))
            {
                selectedSpeaker = ActiveUser.Left;
            }
            else if (rightActive && (!leftActive || (leftDecibels - rightDecibels) < -decibelDifference))
            {
                selectedSpeaker = ActiveUser.Right;
            }
            else if (leftActive && rightActive)
            {
                selectedSpeaker = ActiveUser.Both;
            }
            else
            {
                selectedSpeaker = ActiveUser.None;
            }

            NotifySpeakerInformation(selectedSpeaker, leftDecibels, rightDecibels);
            if (selectedSpeaker != activeSpeaker || (selectedSpeaker != ActiveUser.None && (Math.Abs(lastLeftLevel - leftDecibels) > 1 || Math.Abs(lastRightLevel - rightDecibels) > 1)))
            {
                leftSpeaking = selectedSpeaker == ActiveUser.Left || selectedSpeaker == ActiveUser.Both;
                rightSpeaking = selectedSpeaker == ActiveUser.Right || selectedSpeaker == ActiveUser.Both;
                activeSpeaker = selectedSpeaker;
                lastLeftLevel = leftDecibels;
                lastRightLevel = rightDecibels;
                LocalizationPublisher.ActiveSoundUser(activeSpeaker, leftDecibels, rightDecibels);
            }
        }
    }
}