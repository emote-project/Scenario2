using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours
{
    public class  PerformUtterance : Behaviour
    {
        private string _id;
        private string _category;
        private string _subcategory;
        private string[] _suppTags;
        private string[] _suppValues;

        private Dictionary<string, string> _tagsAndValues;
        private InOutThalamusClient _client;

        private EmotionalClimateLevel _ecLevel = EmotionalClimateLevel.Positive;
        static private int _indexer = 0;
        static private object _indexerLocker = new object();


        public PerformUtterance(KeyValuePair<string, string> utteranceCategory, string[] suppTags = null, string[] suppValues = null)
        {
            _category = utteranceCategory.Key;
            _subcategory = utteranceCategory.Value;
            _suppTags = suppTags;
            _suppValues = suppValues;

            var gameState = GameState.GetInstance();
            
            _tagsAndValues = gameState.GetTagNamesAndValues();
            if (_suppTags != null && _suppValues != null)
            {
                for (int i = 0; i < _suppTags.Length; i++)
                {
                    string tag = _suppTags[i];
                    string val = _suppValues[i];

                    if (tag.ToLower().Contains("achievedlevel"))
                    {
                        if (val.Contains('.')) val = val.Substring(0, val.IndexOf('.'));
                    }

                    _tagsAndValues.Add("/" + tag + "/", val);
                }
            }

            _client = InOutThalamusClient.GetInstance();
            _client.UtteranceFinishedEvent += client_UtteranceFinishedEvent;
            _client.EmotionalClimateChangedEvent += _client_EmotionalClimateChangedEvent;
        }


        void _client_EmotionalClimateChangedEvent(object sender, EmotionalClimateChangedEventArgs e)
        {
            _ecLevel = e.ECLevel;
        }

        public override void BehaviourTask()
        {
            lock (_indexerLocker)
            {
                _id = "acp" + _indexer++;
            }

            var tags = _tagsAndValues.Keys.ToArray();
            var values = _tagsAndValues.Values.ToArray();

            if (_ecLevel == EmotionalClimateLevel.Negative)
                _subcategory = _subcategory + ":negative";
            else
                _subcategory = _subcategory + ":positive";

            _client.IOPublisher.PerformUtteranceFromLibrary(_id, _category, _subcategory, tags, values);
            Console.WriteLine("Performing utterance: id " + _id);

        }

        private void client_UtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs e)
        {
            if (_id == e.Id)
            {
                ExecutionEnded();
                Console.WriteLine("Ending utterance: id " + _id);
                _client.UtteranceFinishedEvent -= client_UtteranceFinishedEvent;
            }
        }
    }
}
