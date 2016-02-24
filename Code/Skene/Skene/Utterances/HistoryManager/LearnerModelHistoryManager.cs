using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EmoteEvents;
using EmoteEvents.ComplexData;

namespace Skene.Utterances.HistoryManager
{
    class LearnerModelHistoryManager : IUtterancesHistoryManager, IDisposable
    {
        private readonly SkeneClient _client;

        private readonly List<Utterance> _recentHistory = new List<Utterance>();
        private readonly List<Utterance> _totalHistory = new List<Utterance>();

        public LearnerModelHistoryManager()
        {
            _client = SkeneClient.GetInstance();
            _client.UtteranceHistoryReceivedEvent += ClientOnUtteranceHistoryReceivedEvent;
            _client.StartEvent += ClientOnStartEvent;
        }

        public void AddToHistory(string utteranceThalamusId,Utterance u)
        {
            _client.SkPublisher.UtteranceUsed(u.Id, u.SerializeToJson());
            if (!_recentHistory.Contains(u)) _recentHistory.Add(u);
            if (!_totalHistory.Contains(u)) _totalHistory.Add(u);
        }

        public bool WasRecentlyUsed(Utterance u)
        {
            if (_recentHistory == null) return false;
            return _recentHistory.Any(x => x.Id.Equals(u.Id) && x.Library.Equals(u.Library));
        }

        public bool WasEverUsed(Utterance u)
        {
            if (_totalHistory == null) return false;
            return _totalHistory.Any(x => x.Id.Equals(u.Id) && x.Library.Equals(u.Library));
        }

        public void Dispose()
        {
            _client.UtteranceHistoryReceivedEvent -= ClientOnUtteranceHistoryReceivedEvent;
            _client.StartEvent -= ClientOnStartEvent;
        }


        private void ClientOnStartEvent(object sender, SkeneClient.StartEventArgs startEventArgs)
        {
            _recentHistory.Clear();
            _totalHistory.Clear();
        }

        private void ClientOnUtteranceHistoryReceivedEvent(object sender, SkeneClient.UtteranceHistoryReceivedEventArgs utteranceHistoryReceivedEventArgs)
        {
            foreach (var uhi in utteranceHistoryReceivedEventArgs.UtterancesHistory)
            {
                if (uhi.LibraryId!=null && 
                    !_totalHistory.Any(x => x.Id.Equals(uhi.LibraryId) && x.Library.Equals(uhi.Library ?? "")))
                    _totalHistory.Add(new Utterance(){Id = uhi.LibraryId, Library = uhi.Library ?? ""});
            }
        }

        #region helpers

        

        private Utterance UtteranceFromHistoryItem(UtteranceHistoryItem uhi)                          
        {
            RepetitionType repetition = default(RepetitionType);
            var result = Enum.TryParse<RepetitionType>(uhi.repetitions,true,out repetition);
            if (!result)
            {
                Console.WriteLine("WARNING! Couldn't parse correctly UtteranceHistoryItem repetition field! Value =  "+uhi.repetitions);
            }
            var u = new Utterance(uhi.utteranceId,uhi.library,uhi.utterance,uhi.category,uhi.subcategory,uhi.question,repetition);
            return u;
        }

        #endregion
    }
}
