using System.Collections.Generic;

namespace Classification.Classifier
{
    internal class FakeClassifier : IClassifier
    {
        #region IClassifier Members

        public List<string> Labels { get; private set; }

        public uint NumInstances { get; private set; }

        public ClassificationResult Classify(IDictionary<string, string> featuresVector)
        {
            return new ClassificationResult(0, "");
        }

        public void Train(IDictionary<string, string> featuresVector, string label)
        {
        }

        public void Load(string filePath)
        {
        }

        public void Save(string filePath)
        {
        }

        public void PostProcess()
        {
        }

        public void Dispose()
        {
        }

        #endregion
    }
}