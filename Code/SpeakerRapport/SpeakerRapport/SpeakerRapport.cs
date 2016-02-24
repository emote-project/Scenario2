using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Thalamus;

namespace SpeakerRapport
{
    public enum Speaker
    {
        LeftSpeaker,
        RightSpeaker
    }

    public interface ISpeakerRapportActions : Thalamus.IAction
    {
        void SetSpeakingVolume(double percent);
    }

    internal interface ISpeakerRapportPulisher : ISpeakerRapportActions, EmoteCommonMessages.IGazeStateActions, Thalamus.BML.ISpeakActions
    { }
    internal interface ISpeakerRapport : EmoteCommonMessages.ISoundLocalizationEvents, Thalamus.BML.ISpeakEvents
    {
    }

    public class SpeakerRapportClient : ThalamusClient, ISpeakerRapport
    {

        public enum GazingType
        {
            Gaze,
            Glance
        }

        internal class SpeakerRapportPublisher : ISpeakerRapportPulisher, IThalamusPublisher
        {
            dynamic publisher;
            public SpeakerRapportPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void SetSpeakingVolume(double percent)
            {
                publisher.SetSpeakingVolume(percent);
            }

            public void GazeAtScreen(double x, double y)
            {
            }

            public void GazeAtTarget(string targetName)
            {
                if (Enum.IsDefined(typeof(Speaker), targetName)) publisher.GazeAtTarget(targetName);
            }

            public void GlanceAtScreen(double x, double y)
            {
            }

            public void GlanceAtTarget(string targetName)
            {
                if (Enum.IsDefined(typeof(Speaker), targetName)) publisher.GlanceAtTarget(targetName);
            }

            public void Speak(string id, string text)
            {
                publisher.Speak(id, text);
            }

            public void SpeakBookmarks(string id, string[] text, string[] bookmarks)
            {
                publisher.SpeakBookmarks(id, text, bookmarks);
            }

            public void SpeakStop()
            {
                publisher.SpeakStop();
            }
        }


        private double baseVolumeLevel = 0.5;
        public double BaseVolumeLevel
        {
            get { return baseVolumeLevel; }
            set {
                baseVolumeLevel = value;
                Properties.Settings.Default["BaseVolumeLevel"] = value;
                Properties.Settings.Default.Save();
            }
        }
        private double baseSpeakerDecibelThreshold = -12.0;
        public double BaseSpeakerDecibelThreshold
        {
            get { return baseSpeakerDecibelThreshold; }
            set { 
                baseSpeakerDecibelThreshold = value;
                Properties.Settings.Default["BaseSpeakerDecibelThreshold"] = value;
                Properties.Settings.Default.Save();
            }
        }
        private int gazeShiftMinimumInterval = 2000;
        public int GazeShiftMinimumInterval
        {
            get { return gazeShiftMinimumInterval; }
            set { 
                gazeShiftMinimumInterval = value;
                Properties.Settings.Default["GazeShiftMinimumInterval"] = value;
                Properties.Settings.Default.Save();
            }
        }
        private string testText = "Consigo falar tao alto como tu.";
        public string TestText
        {
            get { return testText; }
            set
            {
                testText = value;
                Properties.Settings.Default["TestText"] = value;
                Properties.Settings.Default.Save();
            }
        }
        private GazingType gazingBehavior = GazingType.Gaze;
        public GazingType GazingBehavior
        {
            get { return gazingBehavior; }
            set {
                Properties.Settings.Default["GazingBehaviorIsGlance"] = value == GazingType.Glance;
                Properties.Settings.Default.Save();
                gazingBehavior = value; 
            }
        }
        internal SpeakerRapportPublisher SpeakerPublisher;
        
        private EmoteCommonMessages.ActiveUser nextGazed = EmoteCommonMessages.ActiveUser.Left;
        private Stopwatch lastGazeTimer;

        private bool runTest = false;
        public bool RunTest
        {
            get { return runTest; }
            set {
                if (!runTest) SpeakerPublisher.Speak(testId, TestText);
                runTest = value; 
            }
        }
        private string testId = "SpeakerRapportTest";
        

        public SpeakerRapportClient(string character) :base("Speaker Rapport", character)
        {
            SetPublisher<ISpeakerRapportPulisher>();
            SpeakerPublisher = new SpeakerRapportPublisher(Publisher);
            baseVolumeLevel = (double) Properties.Settings.Default["BaseVolumeLevel"];
            baseSpeakerDecibelThreshold = Properties.Settings.Default.BaseSpeakerDecibelThreshold;
            gazeShiftMinimumInterval = Properties.Settings.Default.GazeShiftMinimumInterval;
            testText = Properties.Settings.Default.TestText;
            gazingBehavior = Properties.Settings.Default.GazingBehaviorIsGlance?GazingType.Glance:GazingType.Gaze;
            lastGazeTimer = new Stopwatch();
            lastGazeTimer.Start();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void ConnectedToMaster()
        {
            base.ConnectedToMaster();
            SpeakerPublisher.SetSpeakingVolume(BaseVolumeLevel);
            if (runTest)
            {
                SpeakerPublisher.Speak(testId, TestText);
            }
        }

        void EmoteCommonMessages.ISoundLocalizationEvents.ActiveSoundUser(EmoteCommonMessages.ActiveUser activeUser, double LeftValue, double RightValue)
        {
            Debug(activeUser.ToString());
            if (activeUser == EmoteCommonMessages.ActiveUser.None)
            {
                SpeakerPublisher.SetSpeakingVolume(BaseVolumeLevel);
            }
            else
            {
                double mimicLevel = Math.Max(BaseSpeakerDecibelThreshold, Math.Max(LeftValue, RightValue));
                double setVolume = BaseVolumeLevel + (1 - BaseVolumeLevel) * (1 - (mimicLevel / BaseSpeakerDecibelThreshold));
                Debug("Mimic: {0}; Volume: {1}", mimicLevel, setVolume);
                SpeakerPublisher.SetSpeakingVolume(setVolume);
            }
            

            if (lastGazeTimer.ElapsedMilliseconds >= GazeShiftMinimumInterval)
            {
                if (activeUser == EmoteCommonMessages.ActiveUser.Left || activeUser == EmoteCommonMessages.ActiveUser.Right) nextGazed = activeUser;
                else if (activeUser == EmoteCommonMessages.ActiveUser.Both)
                {
                    if (nextGazed == EmoteCommonMessages.ActiveUser.Left) nextGazed = EmoteCommonMessages.ActiveUser.Right;
                    else nextGazed = EmoteCommonMessages.ActiveUser.Left;
                }
                else nextGazed = EmoteCommonMessages.ActiveUser.None;

                string target = "Random";
                if (nextGazed == EmoteCommonMessages.ActiveUser.Left) target = Speaker.LeftSpeaker.ToString();
                else if (nextGazed == EmoteCommonMessages.ActiveUser.Right) target = Speaker.RightSpeaker.ToString();
                if (target != "Random")
                {
                    if (gazingBehavior == GazingType.Gaze)
                    {
                        Console.WriteLine("Gaze at: " + target);
                        SpeakerPublisher.GazeAtTarget(target);
                    }
                    else
                    {
                        SpeakerPublisher.GlanceAtTarget(target);
                    }
                    lastGazeTimer.Restart();
                }
            }
        }


        void Thalamus.BML.ISpeakEvents.SpeakFinished(string id)
        {
            if (runTest && id == testId) SpeakerPublisher.Speak(testId, TestText);
        }

        void Thalamus.BML.ISpeakEvents.SpeakStarted(string id)
        {
        }
    }
}
