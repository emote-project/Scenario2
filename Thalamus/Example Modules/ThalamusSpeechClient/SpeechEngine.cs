using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThalamusSpeechClient
{
    public enum SpeechEngineType
    {
        Windows,
        Text,
        Mac
    }

    public struct VisemeInfo
    {
        public int Viseme;
        public int NextViseme;
        public double VisemePercent;
        public double NextVisemePercent;
        public VisemeInfo(int viseme, double visemePercent, int nextViseme, double nextVisemePercent)
        {
            NextViseme = nextViseme;
            Viseme = viseme;
            VisemePercent = visemePercent;
            NextVisemePercent = nextVisemePercent;
        }
        public override string ToString()
        {
            return string.Format("[Viseme:'{0}'={1}, NextViseme:'{2}'={3}]", Viseme, VisemePercent, NextViseme, NextVisemePercent);
        }
    }
    public class SpeechEngine
    {

        public static SpeechEngine TextTTS = new SpeechEngine(SpeechEngineType.Text);

        protected SpeechEngineType engineType = SpeechEngineType.Text;
        public SpeechEngineType EngineType
        {
            get { return engineType; }
        }
        protected string voice = "DefaultVoice";
        public string Voice
        {
            get { return voice; }
        }

        private List<String> voicesList = new List<String>();
        public List<String> VoicesList
        {
            get { return voicesList; }
        }

        public SpeechEngine(SpeechEngineType type)
        {
            engineType = type;
        }

        public virtual void Setup() { }
        public virtual void Setup(string voice)
        {
            this.voice = voice;
            Setup();
        }
        protected ThalamusSpeechClient.SpeechClient SpeechClient;

        public virtual void Start(SpeechClient client)
        {
            SpeechClient = client;
        }
        public virtual void Dispose() { }

        public virtual void Speak(SpeechClient.Speech speech)
        {
            string fullText = "";

            foreach (string s in speech.Text) fullText += s + " ";
            Console.WriteLine("Speak: " + fullText);
        }

        public virtual void CancelCurrentSpeech() { }

        public void Started(string id = "")
        {
            SpeechClient.SpeechPublisher.SpeakStarted(id);
            Console.WriteLine("Started");
        }

        public virtual void Ended(string id = "")
        {
            SpeechClient.SpeechPublisher.SpeakFinished(id);
            Console.WriteLine("Ended");
        }

        public void Bookmark(string id)
        {
            SpeechClient.SpeechPublisher.Bookmark(id);
        }

        internal virtual void Stop()
        {
        }
    }
}
