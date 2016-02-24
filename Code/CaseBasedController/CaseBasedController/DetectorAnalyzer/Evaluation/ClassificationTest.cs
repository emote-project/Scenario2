using System;
using System.Collections.Generic;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer.ArffLoaders;

namespace DetectorAnalyzer.Evaluation
{
    public abstract class ClassificationTest : IDisposable
    {
        protected IClassifier classifier;

        protected ClassificationTest(IClassifier classifier)
        {
            this.classifier = classifier;
            this.IgnoreLabels = new HashSet<string>();
        }

        public HashSet<string> IgnoreLabels { get; private set; }
        public double MinSupport { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.IgnoreLabels.Clear();
        }

        #endregion

        public TreeClassificationPerformance Test(string filePath)
        {
            //first determines num instances and labels
            var firstLoader = new ClassificationArffLoader();
            firstLoader.IgnoreLabels.UnionWith(this.IgnoreLabels);
            if (!firstLoader.Load(filePath))
            {
                Console.WriteLine("Could not load given file: {0}", filePath);
                return null;
            }

            return Test(filePath, firstLoader);
        }

        protected abstract TreeClassificationPerformance Test(string filePath, ClassificationArffLoader firstLoader);

        protected void RunLoader(string filePath, int startInstance, int endInstance, ClassificationArffLoader loader)
        {
            var trainArffLoader = loader;
            trainArffLoader.StartInstance = startInstance;
            trainArffLoader.EndInstance = endInstance;
            trainArffLoader.IgnoreLabels.UnionWith(this.IgnoreLabels);
            trainArffLoader.Load(filePath);
        }
    }
}