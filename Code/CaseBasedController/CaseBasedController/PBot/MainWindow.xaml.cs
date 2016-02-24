using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CaseBasedController;
using CaseBasedController.Classifier;
using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using Classification;
using PBot.Annotations;

namespace PBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string classifierPath = "./Data/classifierStructure.arff";
        private const string casePoolPath = "./Data/inputCasePool.json";
        private const string behaviourDictionaryPath = "./Data/BehaviorsDictionary.csv";

        private int minimumDelayFromLastSpeakActionSeconds = 5;

        private ObservableCollection<string> _results = new ObservableCollection<string>();
        private ControllerClient _client;
        private readonly Dictionary<string, List<string>> _newBehaviorsDictionary = new Dictionary<string, List<string>>();
        private DateTime _lastTimeRobotSpeaking;
        private Random _random;
        private bool _didRobotAlreadyTalk = false;
        private bool _didGameEnd = false;

        public ObservableCollection<string> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            _client = new ControllerClient("","PBot");
            ClassifierController classifier = new ClassifierController(_client,classifierPath, casePoolPath);
            classifier.InstanceClassifiedEvent += classifier_InstanceClassifiedEvent;
            classifier.Load();
            LoadBehaviorDictionary();

            _client.SpeakBookmarksEvent += _client_SpeakBookmarksEvent;
            _client.GameStartedEvent += _client_GameStartedEvent;
            _client.EndGameNoOilEvent += _client_EndGame;
            _client.EndGameTimeOutEvent += _client_EndGame;
            _client.EndGameSuccessfullEvent += _client_EndGame;
        }

        void _client_EndGame(object sender, EndGameEventArgs e)
        {
            _didGameEnd = true;
        }

        void _client_GameStartedEvent(object sender, GenericGameEventArgs e)
        {
            _didRobotAlreadyTalk = false;
            _didGameEnd = false;
        }

        void _client_SpeakBookmarksEvent(object sender, SpeechEventArgs e)
        {
            _lastTimeRobotSpeaking = DateTime.Now;
            _didRobotAlreadyTalk = true;
            _didGameEnd = false;
        }

        void classifier_InstanceClassifiedEvent(object sender, InstanceClassifiedEventArgs e)
        {
            var label = e.Classification.Label;
            this.Dispatcher.Invoke(new Action(() =>
            {
                _results.Insert(0,e.Classification.Accuracy + " - " + label);
            }));
            if (label != null)
            {
                _client.ControllerPublisher.ClassifierResult(label);
                PerformClassifierChoseUtterance(e.Classification);
            }
            Console.WriteLine("Label: "+label);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _client.Dispose();
        }




        private bool LoadBehaviorDictionary()
        {
            var sr = new StreamReader(behaviourDictionaryPath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var fields = line.Split(new[] { ';' });
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


        private bool PerformClassifierChoseUtterance(ClassificationResult classification)
        {
            if (!_didRobotAlreadyTalk || _didGameEnd) return false;        // Not publishing anything if the robot didn't say anything yet (avoiding to publish things before the game starts)

            if (DateTime.Now.Subtract(_lastTimeRobotSpeaking).TotalSeconds < minimumDelayFromLastSpeakActionSeconds)
                return false;

            var label = classification.Label;
            string category = null;
            string subcategory = null;
            if (label != null)
            {
                if (!this.GetNewSubCategory(label, ref category, ref subcategory))
                {
                    //console.writeline("Could not process classified behavior: {0}", label);
                    return false;
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
            return true;
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
            _random = new Random();
            label = possibleBehaviors[_random.Next(possibleBehaviors.Count)];

            var parts = label.Split(':');
            category = parts[0].ToLower();
            subcategory = string.Empty;
            if (parts.Length > 1) subcategory = parts[1].ToLower();
            return true;
        }
    }
}
