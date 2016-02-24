using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaseBasedController.GameInfo;
using CaseBasedController.Simulation;
using CaseBasedController.Thalamus;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer;

namespace CaseBasedController.Classifier
{
    public class InstanceClassifiedEventArgs : EventArgs
    {
        public ClassificationResult Classification { get; set; }
        public string[] FeaturesVector { get; set; }
    }

    public class ClassifierController : IDisposable
    {
        private const string IFML_UTT_TEXT = "IFMLSpeech.PerformUtterance";
        private const string BEHAVIOR_DICTIONARY_FILE = "BehaviorsDictionary.csv";
        private readonly ControllerClient _client;

        private readonly Dictionary<string, List<string>> _newBehaviorsDictionary =
            new Dictionary<string, List<string>>();

        private readonly Random _random = new Random();

        private CasePool _casePool;
        private IClassifier _classifier;
        private FeaturesCollector _fc;

        // After firing an event it waits for a while
        int _cooldownMilliseconds = 1500;
        bool _coolingDown = false;

        public ClassifierController(ControllerClient client, string classifierPath, string casePoolPath)
        {
            if (client == null) throw new Exception("Client can't be null");
            if (casePoolPath == null) throw new Exception("CasePoolPath can't be null");

            this._client = client;
            this.ClassifierPath = classifierPath;
            this.CasePoolPath = casePoolPath;
            this.AccuracyThreshold = 00;

            //if (!this.LoadBehaviorDictionary())
                //console.writeline("Could not load dictionary of behaviors!");
        }

        public string ClassifierPath { get; set; }
        public string CasePoolPath { get; set; }

        /// <summary>
        ///     The minimum accuracy required for a classifier result to be resolved to an action
        /// </summary>
        public double AccuracyThreshold { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            this._newBehaviorsDictionary.Clear();
        }

        #endregion

        public event EventHandler<InstanceClassifiedEventArgs> InstanceClassifiedEvent;

        public async Task<bool> LoadAsync(string path = null)
        {
            return await Task.Run(() => Load(path));
        }

        public bool Load(string path = null)
        {
            if (path != null) ClassifierPath = path;
            if (!File.Exists(ClassifierPath))
                throw new FileNotFoundException("Can't find file: " + ClassifierPath);
            if (!File.Exists(CasePoolPath))
                throw new FileNotFoundException("Can't find file: " + CasePoolPath);

            _casePool = CasePool.DeserializeFromJson(CasePoolPath);
            _fc = MainController.CreateFeaturesCollector(_casePool);
            _fc.NewFeaturesVector += _fc_NewFeaturesVector;

            _casePool.Init(_client, _client.ControllerPublisher);

            this._classifier = new TreeArffClassifier();
            this._classifier.Load(ClassifierPath);
            return true;
        }

        private void _fc_NewFeaturesVector(object sender, NewFeaturesVectorEventArgs e)
        {
            try
            {
                var fv = _fc.GetFeaturesVectorWithNames(_fc.FeaturesVectors.Last().ToList());
                var classification = _classifier.Classify(fv);
                string label = classification.Label;
                if (InstanceClassifiedEvent != null) InstanceClassifiedEvent(this, new InstanceClassifiedEventArgs() { Classification = classification, FeaturesVector = e.FeaturesVector });
                //ResolveClassifierResult(classification);
            }
            catch (Exception ex)
            {
                Logger.Log("Exception classying new features vector: "+ex.Message, this);
            }
        }

        async void Cooldown()
        {
            await Task.Delay(_cooldownMilliseconds);
            _coolingDown = false;
        }

        private bool LoadBehaviorDictionary()
        {
            if (!File.Exists(BEHAVIOR_DICTIONARY_FILE)) return false;

            var sr = new StreamReader(BEHAVIOR_DICTIONARY_FILE);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var fields = line.Split(new[] {';'});
                if (fields.Length < 2) continue;

                var key = fields[0];
                var behavior = fields[1];
                if (!this._newBehaviorsDictionary.ContainsKey(key))
                    this._newBehaviorsDictionary.Add(key, new List<string>());
                this._newBehaviorsDictionary[key].Add(behavior);
            }
            sr.Close();
            sr.Dispose();

            return true;
        }

        private void ResolveClassifierResult(ClassificationResult classification)
        {
            if ((this._client == null) ||
                (GameStatus.CurrentState == null) ||
                !MainController.IsClassifierEnabled ||
                (classification.Accuracy < AccuracyThreshold / 100d))
                return;

            var label = classification.Label;
            string category = null;
            string subcategory = null;
            if (label != null)
            {
                if (!this.GetNewSubCategory(label, ref category, ref subcategory))
                {
                    //console.writeline("Could not process classified behavior: {0}", label);
                    return;
                }

                this._client.ControllerPublisher.PerformUtteranceFromLibrary(
                    string.Empty,
                    category,
                    subcategory,
                    GameStatus.GetTagNamesAndValues().Keys.ToArray(),
                    GameStatus.GetTagNamesAndValues().Values.ToArray()
                    );
                MainController.UseClassifier(false);
            }
        }
        
        private bool GetWoZSubCategory(string label, ref string category, ref string subcategory)
        {
            var parts = label.Split(':');
            var thalamusMessage = parts[0];
            if (!thalamusMessage.Equals(IFML_UTT_TEXT)) return false;

            parts = parts[1].Split('.');
            category = parts[0];
            subcategory = string.Empty;
            if (parts.Length > 1) subcategory = parts[1];
            return false;
        }

        private bool GetNewSubCategory(string label, ref string category, ref string subcategory)
        {
            if (!this._newBehaviorsDictionary.ContainsKey(label) || (this._newBehaviorsDictionary[label].Count == 0))
            {
                //console.writeline("No match in dictionary for WoZ behavior: {0}", label);
                return false;
            }

            //transforms classified utterance into new (sub)category by choosing randomly from collection
            var possibleBehaviors = this._newBehaviorsDictionary[label];
            label = possibleBehaviors[this._random.Next(possibleBehaviors.Count)];

            var parts = label.Split(':');
            category = parts[0].ToLower();
            subcategory = string.Empty;
            if (parts.Length > 1) subcategory = parts[1].ToLower();
            return true;
        }
    }
}