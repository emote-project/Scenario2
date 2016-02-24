using System.Collections.Generic;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer.ArffLoaders;

namespace DetectorAnalyzer
{
    public class TreeArffClassifier : IClassifier
    {
        private readonly HashSet<string> _ignoreBehaviors = new HashSet<string>
                                                            {
                                                                "doNothingClass",
                                                                "IFMLSpeech.PerformUtterance:-",
                                                                "IFMLSpeech.PerformUtterance:robot.backup",
                                                                "IFMLSpeech.PerformUtterance:robot.jokes",
                                                                "IFMLSpeech.PerformUtterance:technical.-",
                                                                "IFMLSpeech.PerformUtterance:robot.regreat",
                                                                "IFMLSpeech.PerformUtterance:robot.turntaking",
                                                                "IFMLSpeech.PerformUtterance:status.levelup",
                                                                "IFMLSpeech.PerformUtterance:tutorial.goodbye2",
                                                                "IFMLSpeech.PerformUtterance:tutorial.goodbye1",
                                                                "IFMLSpeech.PerformUtterance:tutorial.welcome",
                                                                "IFMLSpeech.PerformUtterance:PerformUpgrade:Self",
                                                                "IFMLSpeech.PerformUtterance:ConfirmConstruction:Self",
                                                                "IFMLSpeech.PerformUtterance:ImplementPolicy:Self",
                                                                "IFMLSpeech.PerformUtterance:tutorial.1",
                                                                "IFMLSpeech.PerformUtterance:tutorial.endgame",
                                                                //"IFMLSpeech:robot",
                                                                //"IFMLSpeech:strategy",
                                                            };

        private TrainClassifierArffLoader _arffLoader;

        private TreeClassifier TreeClassifier
        {
            get { return this._arffLoader.Classifier as TreeClassifier; }
        }

        public double MinFrequency { get; set; }

        #region IClassifier Members

        public void Dispose()
        {
            this._arffLoader.Dispose();
            this.TreeClassifier.Dispose();
        }

        public List<string> Labels
        {
            get { return this.TreeClassifier.Labels; }
        }

        public uint NumInstances
        {
            get { return this.TreeClassifier.NumInstances; }
        }

        public ClassificationResult Classify(IDictionary<string, string> featuresVector)
        {
            return this.TreeClassifier.Classify(featuresVector);
        }

        public void Train(IDictionary<string, string> featuresVector, string label)
        {
            this.TreeClassifier.Train(featuresVector, label);
        }

        public void Load(string filePath)
        {
            this._arffLoader = new TrainClassifierArffLoader(new TreeClassifier {MinFrequency = this.MinFrequency});
            this._arffLoader.IgnoreLabels.UnionWith(this._ignoreBehaviors);
            this._arffLoader.Load(filePath);
        }

        public void Save(string filePath)
        {
            this.TreeClassifier.Save(filePath);
        }

        public void PostProcess()
        {
            this.TreeClassifier.PostProcess();
        }

        #endregion
    }
}