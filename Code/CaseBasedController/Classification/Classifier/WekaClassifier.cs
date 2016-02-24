using System;
using System.Collections.Generic;
using System.IO;
using PicNetML;
using PicNetML.Clss;
using weka.core;
using weka.core.converters;

namespace Classification.Classifier
{
    public class WekaClassifier : IClassifier
    {
        private readonly Instances _instances;
        private IUntypedBaseClassifier<weka.classifiers.Classifier> _classifier;
        private Instance _instance;

        public WekaClassifier(string arffFile)
        {
            if (string.IsNullOrWhiteSpace(arffFile) || !File.Exists(arffFile))
                throw new ArgumentException(string.Format("Invalid ARFF file given: {0}", arffFile), arffFile);

            this.Labels = new List<string>();

            //loads instances from dummy arff file
            var loader = new ArffLoader();
            loader.setFile(new java.io.File(arffFile));
            this._instances = loader.getStructure();
            this._instances.setClassIndex(this._instances.numAttributes() - 1);

            //gets first instance
            var instances = loader.getDataSet();
            instances.setClassIndex(this._instances.numAttributes() - 1);
            this._instance = instances.firstInstance();

            //loads labels
            var classAttr = this._instances.classAttribute();
            this.Labels.Clear();
            for (var i = 0; i < classAttr.numValues(); i++)
                this.Labels.Add(classAttr.value(i));
        }

        #region IClassifier Members

        public void Dispose()
        {
            this.Labels.Clear();
        }

        public List<string> Labels { get; private set; }
        public uint NumInstances { get; set; }

        public ClassificationResult Classify(IDictionary<string, string> featuresVector)
        {
            if (this._classifier == null) return null;
            
            foreach (var feature in featuresVector)
            {
                var att = this._instances.attribute(feature.Key);
                if (att == null) continue;
                if (att.isNumeric())
                {
                    double val;
                    Double.TryParse(feature.Value, out val);
                    this._instance.setValue(att, val);
                }
                else
                {
                    this._instance.setValue(att, feature.Value);
                }
            }

            var pmlInstance = new PmlInstance(this._instance);
            //Console.WriteLine(this._instance);

            double labelID = 0;
            double prob = 0;
            try
            {
                labelID = this._classifier.ClassifyInstance(pmlInstance);
                prob = this._classifier.ClassifyInstanceProba(pmlInstance);
            }
            catch (IndexOutOfRangeException)
            {
            }

            var label = this._instance.classAttribute().value((int) labelID);
            return new ClassificationResult(prob, label);
        }

        public void Train(IDictionary<string, string> featuresVector, string label)
        {
        }

        public void Load(string filePath)
        {
            this._classifier = BaseClassifier.Read(filePath);
        }

        public void Save(string filePath)
        {
            if (this._classifier != null)
                BaseClassifier.FlushToFile(this._classifier, filePath, true);
        }

        public void PostProcess()
        {
        }

        #endregion
    }
}