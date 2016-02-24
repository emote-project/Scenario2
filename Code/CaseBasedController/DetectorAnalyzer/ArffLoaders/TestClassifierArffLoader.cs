using System;
using System.Collections.Generic;
using System.IO;
using Classification;
using Classification.Classifier;

namespace DetectorAnalyzer.ArffLoaders
{
    public class TestClassifierArffLoader : ClassificationArffLoader
    {
        #region Fields

        private readonly Dictionary<string, int> _behaviorIdxs = new Dictionary<string, int>();
        private readonly TrainClassifierArffLoader _trainLoader;
        private int _progress;

        #endregion

        #region Constructor

        public TestClassifierArffLoader(TrainClassifierArffLoader trainLoader)
        {
            this._trainLoader = trainLoader;
        }

        #endregion

        #region Properties

        public TreeClassificationPerformance Performance { get; private set; }

        protected new Dictionary<string, uint> LabelCount
        {
            get { return this._trainLoader.LabelCount; }
        }

        public double MinSupport { get; set; }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            this.Performance.Dispose();
            this._behaviorIdxs.Clear();
        }

        protected override void Reset()
        {
            //resets algorithms vars
            var classifier = this._trainLoader.Classifier;
            var numLabels = classifier.Labels.Count;
            var counts = new List<uint>(numLabels);
            for (var i = 0; i < numLabels; i++)
                counts.Add(this.LabelCount[classifier.Labels[i]]);
            this.Performance = new TreeClassificationPerformance((uint) numLabels, (TreeClassifier) classifier, counts);
            for (var idx = 0; idx < numLabels; idx++)
                this._behaviorIdxs[classifier.Labels[idx]] = idx;
            this._progress = -1;
        }

        protected override int ProcessAllInstances(TextReader sr)
        {
            if (this.Performance == null) this.Reset();
            var retVal = base.ProcessAllInstances(sr);
            this.Performance.Finish();
            return retVal;
        }

        protected override void ProcessTransaction(
            IDictionary<string, string> featuresState, string label, int transactionNum)
        {
            //checks for never seen class/behavior
            if (!this._behaviorIdxs.ContainsKey(label)) return;

            var classification = ((TreeClassifier)this._trainLoader.Classifier).Classify(featuresState, label);
            this.UpdateMeasures(label, classification);

            //prints progress on screen
            var numInstances = this._trainLoader.Classifier.NumInstances;
            var curProg = (100*transactionNum)/numInstances;
            Console.Write((this._progress != curProg) && (curProg%10).Equals(0)
                ? string.Format("{0}%", curProg)
                : ".");
            this._progress = (int) curProg;
        }

        private void UpdateMeasures(string behavior, ClassificationResult classification)
        {
            //checks confidence level (if maximal support from all trees is sufficient)
            var predictedBehavior = classification.Label;
            var support = classification.Accuracy;
            var numCorrect = 0u;
            var numIncorrect = 0u;
            var numUnsupported = 0u;

            //increases counters and measures accordingly
            if ((predictedBehavior == null) || (support < this.MinSupport))
                numUnsupported = 1;
            else
            {
                this.Performance.ConfusionMatrix[this._behaviorIdxs[predictedBehavior]][this._behaviorIdxs[behavior]]++;
                if (predictedBehavior.Equals(behavior)) numCorrect = 1;
                else numIncorrect = 1;
            }

            this.Performance.AddPerformance(numCorrect, numIncorrect, numUnsupported, support);
        }

        public override void PrintResults(string baseDir)
        {
            var filePath = Path.GetFullPath(string.Format("{0}/performance.csv", baseDir));
            Console.WriteLine("Printing classification performance to {0}...", Path.GetFileName(filePath));
            this.Performance.PrintResults(filePath);
        }
    }
}