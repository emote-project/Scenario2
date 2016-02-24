using System;
using System.Collections.Generic;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer.ArffLoaders;

namespace DetectorAnalyzer.Evaluation
{
    public class CrossValidatonTest : ClassificationTest
    {
        private readonly uint _numFolds;

        public CrossValidatonTest(uint numFolds, IClassifier classifier) : base(classifier)
        {
            this._numFolds = numFolds;
        }

        protected override TreeClassificationPerformance Test(string filePath, ClassificationArffLoader firstLoader)
        {
            //collects the performance in all folds
            var performances = new List<TreeClassificationPerformance>();
            var numInstances = firstLoader.NumInstances;
            var foldSize = numInstances/(int) this._numFolds;
            for (var i = 0; i < this._numFolds; i++)
            {
                //checks intervals for testing
                var startInstance = this._numFolds == 1 ? -1 : foldSize*i;
                var endInstance = this._numFolds == 1
                    ? -1
                    : (i == this._numFolds - 1 ? numInstances : startInstance + foldSize - 1);

                Console.WriteLine("\n======================================");
                Console.WriteLine("Running fold {0} of {1}...", i + 1, this._numFolds);

                var performance = this.RunFold(filePath, startInstance, endInstance);
                performances.Add(performance);
            }

            Console.WriteLine("\n======================================");
            Console.WriteLine("Finished processing all {0} folds!", this._numFolds);

            //gets average of performances
            return TreeClassificationPerformance.GetAverage(performances);
        }

        private TreeClassificationPerformance RunFold(string filePath, int startInstance, int endInstance)
        {
            this.classifier.Dispose();
            var trainArffLoader = new TrainClassifierArffLoader(this.classifier);
            this.RunLoader(filePath, startInstance, endInstance, trainArffLoader);

            var testArffLoader = new TestClassifierArffLoader(trainArffLoader) {MinSupport = this.MinSupport};
            this.RunLoader(filePath, startInstance, endInstance, testArffLoader);

            var performance = testArffLoader.Performance;
            return performance;
        }
    }
}