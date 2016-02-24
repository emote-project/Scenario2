using System.Collections.Generic;
using System.IO;
using Classification.Classifier;

namespace DetectorAnalyzer.ArffLoaders
{
    public class TrainClassifierArffLoader : ClassificationArffLoader
    {
        public TrainClassifierArffLoader(IClassifier classifier)
        {
            this.Classifier = classifier;
        }

        #region Properties

        public IClassifier Classifier { get; private set; }

        #endregion

        protected override bool InstanceInRange(int instanceNum)
        {
            //for training we **ignore** the given range
            return ((this.StartInstance == INVALID_INSTANCE_NUM) ||
                    (this.EndInstance == INVALID_INSTANCE_NUM) ||
                    (instanceNum < this.StartInstance) ||
                    (instanceNum > this.EndInstance));
        }

        protected override void ProcessTransaction(
            IDictionary<string, string> featuresState, string label, int transactionNum)
        {
            this.Classifier.Train(featuresState, label);
        }

        protected override int ProcessAllInstances(TextReader sr)
        {
            var retVal = base.ProcessAllInstances(sr);
            this.Classifier.PostProcess();
            return retVal;
        }

        public override void PrintResults(string baseDir)
        {
            base.PrintResults(baseDir);

            //saves classifier to json file
            var filePath = string.Format("{0}/classifier.json", baseDir);
            this.Classifier.Save(filePath);
            this.Classifier.Load(filePath);
        }
    }
}