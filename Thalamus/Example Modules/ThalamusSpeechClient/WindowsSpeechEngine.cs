using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.Speech.Synthesis.TtsEngine;

namespace ThalamusSpeechClient
{
    public class WindowsSpeechEngine : SpeechEngine
    {
        public WindowsSpeechEngine() : base(SpeechEngineType.Windows) { }

        private SpeechSynthesizer tts = null;

        private Dictionary<Prompt, string> ids = new Dictionary<Prompt, string>();
        private Dictionary<int, float> visemes = new Dictionary<int, float>();

        SpeechClient.Speech currentSpeech = new SpeechClient.Speech(); 

        public override void Setup()
        {
            SetupVisemes();
        }

        public override void Start(SpeechClient server)
        {
            base.Start(server);

            tts = new SpeechSynthesizer();
            bool voiceExists = false;
            foreach (InstalledVoice v in tts.GetInstalledVoices())
            {
                Console.WriteLine("WindowsTTS: Found '" + v.VoiceInfo.Name + "' voice.");
                if (v.VoiceInfo.Name == voice) voiceExists = true;
            }
            if (voiceExists) tts.SelectVoice(Properties.Settings.Default.Voice);
            tts.VisemeReached += new EventHandler<VisemeReachedEventArgs>(ProcessViseme);
            tts.PhonemeReached += new EventHandler<PhonemeReachedEventArgs>(ProcessPhonem);
            tts.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(Ended);
            tts.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(Started);
            tts.BookmarkReached += new EventHandler<BookmarkReachedEventArgs>(Bookmark);
            tts.Rate = Properties.Settings.Default.SpeechRate;
            Console.WriteLine("Started WindowsTTS speech engine.");
        }

        private void SetupVisemes()
        {
            visemes = new Dictionary<int, float>();

            // closed
            visemes.Add(0, 0); //silence
            visemes.Add(21, 0); //p, b, m

            // almost closed
            visemes.Add(18, 0.1f); //f, v
            visemes.Add(7, 0.1f); //w, uw

            // slight open
            visemes.Add(14, 0.3f);  //l
            visemes.Add(15, 0.3f);  //s, z
            visemes.Add(16, 0.3f);  //sh, ch, jh, zh
            visemes.Add(17, 0.3f);  //th, dh
            visemes.Add(19, 0.3f);  //d, t, n

            // mid open
            visemes.Add(8, 0.5f); //ow
            visemes.Add(13, 0.5f); //r
            visemes.Add(20, 0.5f); //k, g, ng

            // almost wide open
            visemes.Add(1, 0.7f); //ae, ax, ah
            visemes.Add(2, 0.7f); //aa
            visemes.Add(3, 0.7f); //ao
            visemes.Add(9, 0.7f); //aw
            visemes.Add(12, 0.7f); //h
            visemes.Add(10, 0.7f); //oy
            visemes.Add(11, 0.7f); //ay

            // wide open
            visemes.Add(4, 1.0f); //ey, eh, uh
            visemes.Add(5, 1.0f); //er
            visemes.Add(6, 1.0f); //y, iy, ih, ix
        }

        private float VisemeToPercent(int viseme)
        {
            if (visemes.ContainsKey(viseme)) return visemes[viseme];
            else return 0;
        }

        #region private windows TTS events

        private void ProcessPhonem(object sender, PhonemeReachedEventArgs args)
        {
            //Console.WriteLine(args.Phoneme);
        }

        private void ProcessViseme(object sender, VisemeReachedEventArgs args)
        {
            SpeechClient.SpeechPublisher.Viseme(args.Viseme, args.NextViseme, VisemeToPercent(args.Viseme), VisemeToPercent(args.NextViseme));
        }

        private void Started(object sender, SpeakStartedEventArgs args)
        {
            if (ids.ContainsKey(args.Prompt))
            {
                Started(ids[args.Prompt]);
            }
            else
            {
                Started();
            }
        }

        private void Ended(object sender, SpeakCompletedEventArgs args)
        {
            if (ids.ContainsKey(args.Prompt))
            {
                Ended(ids[args.Prompt]);
                ids.Remove(args.Prompt);
            }
            else
            {
                Ended();
            }
        }

        private void Bookmark(object sender, BookmarkReachedEventArgs args)
        {
            Bookmark(args.Bookmark);
        }

        #endregion

        public override void CancelCurrentSpeech()
        {
            tts.SpeakAsyncCancelAll();
        }

        internal override void Stop()
        {
            tts.SpeakAsyncCancelAll();
        }

        public override void Ended(string id = "")
        {
            if (!currentSpeech.IsNull) id = currentSpeech.Id;
            base.Ended(id);
        }

        public override void Speak(SpeechClient.Speech speech)
        {
            try
            {
                PromptBuilder p = new PromptBuilder();
                p.Culture = tts.Voice.Culture;
                p.StartVoice(p.Culture);
                p.StartSentence();

                p.StartStyle(new PromptStyle(PromptEmphasis.None));
                for (int i = 0; i < speech.Text.Length; i++)
                {
                    if (speech.Bookmarks == null || speech.Bookmarks.Length < i + 1 || speech.Bookmarks[i]=="")
                    {
                        string s = "";
                        for (; i < speech.Text.Length; i++) s += speech.Text[i] + " ";
                        p.AppendSsmlMarkup(s);
                        break;
                    }
                    else
                    {
                        p.AppendSsmlMarkup(speech.Text[i]);
                        p.AppendBookmark(speech.Bookmarks[i]);
                    }
                }
                p.EndStyle();
                p.EndSentence();
                p.EndVoice();
                currentSpeech = speech;
                if (speech.Id != "") ids.Add(tts.SpeakAsync(p), speech.Id);
                else tts.SpeakAsync(p);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("WindowsTTS Failed: " + e.Message);
            }
        }
    }
}
