using System;
using System.Collections.Generic;
using System.IO;
using EmoteEvents.ComplexData;

namespace Skene.Utterances.HistoryManager
{
    class SimpleFileHistoryManager : IUtterancesHistoryManager
    {
        private readonly string _totalHistoryFilePath;

        private readonly List<Utterance> _recentHistory = new List<Utterance>();
        private readonly List<Utterance> _totalHistory = new List<Utterance>();

        private static SimpleFileHistoryManager _instance;

        private SimpleFileHistoryManager()
        {
            _totalHistoryFilePath = Properties.Settings.Default.UtteranceLibrariesDirectory + @"\history.txt";
            if (!File.Exists(_totalHistoryFilePath)) File.Create(_totalHistoryFilePath).Dispose();
            using (TextReader reader = new StreamReader(_totalHistoryFilePath))
            {
                string line = reader.ReadLine();
                try
                {
                    while (line != null)
                    {
                        string[] splitted = line.Split(',');
                        _totalHistory.Add(new Utterance(
                            splitted[0],
                            splitted[1],
                            splitted[2],
                            splitted[3],
                            splitted[4],
                            splitted[5],
                            splitted[6]
                            ));
                        line = reader.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _instance = this;
        }

        static public SimpleFileHistoryManager GetInstance()
        {
            if (_instance != null) return _instance;
            return new SimpleFileHistoryManager();
        }

        public void AddToHistory(string utteranceThalamusId, Utterance u)
        {
            if (!_recentHistory.Contains(u)) _recentHistory.Add(u);
            if (!_totalHistory.Contains(u))
            {
                using (TextWriter writer = new StreamWriter(_totalHistoryFilePath, true))
                {
                    writer.WriteLine(u.Id + "," + u.Library + "," + u.Text + "," + u.Category + "," + u.Subcategory + ","+u.IsQuestion+","+u.Repetitions);
                }
                _totalHistory.Add(u);
            }
        }

        public bool WasRecentlyUsed(Utterance u)
        {
            return _recentHistory.Contains(u);
        }

        public bool WasEverUsed(Utterance u)
        {
            return _totalHistory.Contains(u);
        }
    }


}
