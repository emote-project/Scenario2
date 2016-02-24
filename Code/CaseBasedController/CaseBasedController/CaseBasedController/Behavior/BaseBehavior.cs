using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Behavior
{
    public abstract class BaseBehavior : IBehavior
    {
        private const int BOOKMARKS_TIMEOUT_MILLISECONDS = 7000;
        private System.Timers.Timer _bookmarksTimeoutTimer;

        protected readonly object locker = new object();
        protected IAllActionPublisher actionPublisher;
        protected IAllPerceptionClient perceptionClient;

        private string _uttId = "";
        private object _locker = new object();

        #region IBehavior Members

        public int Priority { get; set; }

        public event ExecutedEventHandler ExecutionFinished;

        
        public abstract void Execute(IFeatureDetector detector);
        public abstract void Cancel();

        public virtual void Init(IAllActionPublisher publisher, IAllPerceptionClient client)
        {
            this.perceptionClient = client;
            this.actionPublisher = publisher;
            AttachPerceptionEvents();
        }

        #endregion

        protected virtual void AttachPerceptionEvents()
        {
            //Log("Attaching");
            perceptionClient.UtteranceFinishedEvent += perceptionClient_UtteranceFinishedEvent;
            perceptionClient.SpeakBookmarksEvent += perceptionClient_SpeakBookmarksEvent;
        }

       
        public virtual void Dispose()
        {
            //Log("Disposing");
            perceptionClient.UtteranceFinishedEvent -= perceptionClient_UtteranceFinishedEvent;
        }

        void perceptionClient_UtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs e)
        {
            //Log("####### BASE BEHAVIOUR UTTERANCE: <finished> id: " + e.Id);
            if (_uttId == e.Id)
            {
                _uttId = "";
                UtteranceFinishedEvent(e.Id);
            }
        }

        void perceptionClient_SpeakBookmarksEvent(object sender, SpeechEventArgs e)
        {
            RestartBookmarkTimeout(e.ID);
        }

        private void RestartBookmarkTimeout(string id)
        {
            lock (_locker)
            {
                if (_bookmarksTimeoutTimer == null)
                {
                    _bookmarksTimeoutTimer = new Timer(BOOKMARKS_TIMEOUT_MILLISECONDS);
                    _bookmarksTimeoutTimer.Elapsed += delegate(object o, ElapsedEventArgs args)
                    {
                        if (_uttId == id)
                        {
                            _uttId = "";
                            UtteranceFinishedEvent(id);
                        }
                    };
                    _bookmarksTimeoutTimer.AutoReset = false;
                }
                _bookmarksTimeoutTimer.Stop();
                _bookmarksTimeoutTimer.Start();
            }
        }


        protected void RaiseFinishedEvent(IFeatureDetector detector = null)
        {
            if (this.ExecutionFinished != null)
                this.ExecutionFinished(this, detector);
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public string PerformUtterance(string category, string subcategory,  string[] tags = null, string[] values = null)
        {

            var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
            _uttId = "cbc" + new System.Random().Next();
            if (tags == null)
            {
                tags = new List<string>(tagsAndValues.Keys).ToArray();
                values = new List<string>(tagsAndValues.Values).ToArray();
            }

            string suffix = GameInfo.GameStatus.EmotionalClimate.ToString();

            this.actionPublisher.PerformUtteranceFromLibrary(_uttId, category, subcategory + ":" + suffix, tags, values);
            Logger.Log("BASE BEHAVIOUR UTTERANCE: <performing> id: " + _uttId + ", " + category + ", " + subcategory,this);

            RestartBookmarkTimeout(_uttId);

            return _uttId;
        }


        protected virtual void UtteranceFinishedEvent(string id)
        {
            Logger.Log("BASE BEHAVIOUR UTTERANCE: <finished> id: " + id, this);
        }

        

    }
}