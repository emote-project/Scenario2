using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thalamus;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Net.Sockets;
using System.Net;

namespace ThalamusSpeechClient
{
    public interface ISpeechClient : 
        Thalamus.BML.ISpeakActions
    {}

    public interface ISpeechClientPublisher : IThalamusPublisher,
        Thalamus.BML.ISpeakEvents,
        Thalamus.BML.ISpeakDetailEvents
    { }

    public class SpeechClient : Thalamus.ThalamusClient, ISpeechClient
    {
        public class Speech
        {
            public string Id = "";
            public string[] Text;
            public string[] Bookmarks = new string[0];
            public bool IsNull = true;
            public string FullText()
            {
                string str = "";
                foreach (string s in Text) str += s + " ";
                return str;
            }
            public Speech() {}
            public Speech(string[] text, string[] bookmarks) : this("", text, bookmarks) { }
            public Speech(string id, string[] text) : this(id, text, new string[0]) {}
            public Speech(string[] text) : this("", text, new string[0]) {}
            public Speech(string id, string text): this(id, new string[1] { text }, new string[0]){}
            public Speech(string text) : this("", new string[1] { text }, new string[0]) { }
            public Speech(string id, string[] text, string[] bookmarks)
            {
                Id = id;
                Text = text;
                Bookmarks = bookmarks;
                IsNull = false;
            }
            

        }

        internal class SpeechClientPublisher : ISpeechClientPublisher
        {
            dynamic publisher;
            public SpeechClientPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            #region ISpeakEvents Members

            public void SpeakFinished(string id)
            {
                publisher.SpeakFinished(id);
            }

            public void SpeakStarted(string id)
            {
                publisher.SpeakStarted(id);
            }

            #endregion

            #region ISpeakDetailEvents Members

            public void Bookmark(string id)
            {
                publisher.Bookmark(id);
            }

            public void Viseme(int viseme, int nextViseme, double visemePercent, double nextVisemePercent)
            {
                publisher.Viseme(viseme, nextViseme, visemePercent, nextVisemePercent);
            }

            #endregion
        }

        internal SpeechClientPublisher SpeechPublisher;

        public SpeechClient(string characterName = "")
            : base("SpeechServer", characterName)
        {
            SetPublisher<ISpeechClientPublisher>();
            SpeechPublisher = new SpeechClientPublisher(Publisher);

            try
            {
                Debug("Setup Start");
                if (Enum.IsDefined(typeof(SpeechEngineType), Properties.Settings.Default.TTSEngine))
                {
                    switch ((SpeechEngineType)Enum.Parse(typeof(SpeechEngineType), Properties.Settings.Default.TTSEngine))
                    {
                        case SpeechEngineType.Windows:
                            speechEngine = new WindowsSpeechEngine();
                            break;
                        default:
                            speechEngine = SpeechEngine.TextTTS;
                            break;
                    }
                }
                else speechEngine = SpeechEngine.TextTTS;

                if (speechEngine != null)
                {
                    speechEngine.Setup(Properties.Settings.Default.Voice);
                    speechEngine.Start(this);
                }

                Debug("Setup End");
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        protected SpeechEngine speechEngine = SpeechEngine.TextTTS;

        #region ISpeakActions Members

        void Thalamus.BML.ISpeakActions.Speak(string id, string text)
        {
            if (speechEngine != null)
            {
                Debug("Speaking '" + text + "'");
                speechEngine.Speak(new Speech(id, text));
            }
        }

        void Thalamus.BML.ISpeakActions.SpeakBookmarks(string id, string[] text, string[] bookmarks)
        {
            Speech speech = new Speech(id, text, bookmarks);
            if (speechEngine != null)
            {
                Debug("Speaking '" + speech.FullText() + "'");
                speechEngine.Speak(speech);
            }
        }

        void Thalamus.BML.ISpeakActions.SpeakStop()
        {
            speechEngine.Stop();
        }

        #endregion
    }
}
