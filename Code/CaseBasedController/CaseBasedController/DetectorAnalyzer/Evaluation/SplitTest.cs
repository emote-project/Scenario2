using System;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer.ArffLoaders;

namespace DetectorAnalyzer.Evaluation
{
    public class SplitTest : ClassificationTest
    {
        private static readonly Random Random = new Random();
        private readonly double _trainPercent;

        public SplitTest(double trainPercent, IClassifier classifier) : base(classifier)
        {
            this._trainPercent = trainPercent;
        }

        protected override TreeClassificationPerformance Test(string filePath, ClassificationArffLoader firstLoader)
        {
            var numInstances = firstLoader.NumInstances;
            var numTestInstances = (int) ((1d - this._trainPercent)*numInstances);
            var startInstance = Random.Next(numInstances - numTestInstances);
            var endInstance = startInstance + numTestInstances;

            Console.WriteLine("\n======================================");
            Console.WriteLine("Training with {0} of {1}...", numInstances - numTestInstances, numInstances);
            var trainArffLoader = new TrainClassifierArffLoader(this.classifier);
            this.RunLoader(filePath, startInstance, endInstance, trainArffLoader);


            Console.WriteLine("\n======================================");
            Console.WriteLine("Testing with {0} of {1}...", numTestInstances, numInstances);
            var testArffLoader = new TestClassifierArffLoader(trainArffLoader) {MinSupport = this.MinSupport};
            this.RunLoader(filePath, startInstance, endInstance, testArffLoader);

            Console.WriteLine("\n======================================");
            Console.WriteLine("Finished processing all data!");

            return testArffLoader.Performance;
        }
    }
}