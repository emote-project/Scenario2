using System;
using System.Collections.Generic;
using System.IO;
using Classification;
using Classification.Classifier;

namespace EmotionalClimateClassification
{
    internal class WekaTestProgram
    {
        private const string WEKA_TESTS_PATH = @"C:\Users\PEDRO\Tresor\EmotionalClimate";
        private const string TEST_FILE = @"EC-Data - 10 Sessions - 4 SampleSteps.csv";
        private const string OUTPUT_DIR = "output"; //"output-negative-resample-10-5"; //"output";
        private const char SEPARATOR = ';';
        private const double MIN_SUPPORT = 0;//0.5;

        private static void Main(string[] args)
        {
            var arffFile = string.Format("{0}\\{1}\\preprocess\\test-set.arff", WEKA_TESTS_PATH, OUTPUT_DIR);
            var testFile = string.Format("{0}\\datasets\\{1}", WEKA_TESTS_PATH, TEST_FILE);

            var models = Directory.GetFiles(
                string.Format("{0}\\full-models", WEKA_TESTS_PATH), "*.model");
                //string.Format("{0}\\output\\model", WEKA_TESTS_PATH), "*.model");

            foreach (var modelPath in models)
            {
                var performance = TestModel(modelPath, arffFile, testFile);
                var file = string.Format("{0}.csv", Path.GetFileNameWithoutExtension(modelPath));
                performance.PrintResults(file);
            }

            Console.WriteLine("Finished processing!");
            Console.ReadKey();
        }

        private static ClassificationPerformance TestModel(string modelPath, string arffFile, string testFile)
        {
            Console.WriteLine("Testing performance of model: {0}", Path.GetFileName(modelPath));

            ClassificationPerformance performance;
            using (var classifier = new WekaClassifier(arffFile))
            {
                var numLabels = (uint) classifier.Labels.Count;
                var labelCounts = new List<uint>();
                for (var i = 0; i < numLabels; i++)
                    labelCounts.Add(0);

                performance = new ClassificationPerformance(numLabels, classifier, labelCounts);
                classifier.Load(modelPath);

                using (var sr = new StreamReader(testFile))
                {
                    var line = sr.ReadLine();
                    while ((line = sr.ReadLine()) != null)
                    {
                        var fields = line.Split(SEPARATOR);
                        var instance = GetInstance(fields);
                        var trueLabel = fields[23];
                        var trueLabelIdx = classifier.Labels.IndexOf(trueLabel);
                        labelCounts[trueLabelIdx]++;
                        var classification = classifier.Classify(instance);
                        UpdateMeasures(trueLabel, classification, performance, classifier);
                    }
                }

                performance.Finish();

                //gets true label counts
                for (var i = 0; i < numLabels; i++)
                    performance.LabelCounts[i].Value = labelCounts[i];
            }
            return performance;
        }

        private static void UpdateMeasures(string label,
            ClassificationResult classification, ClassificationPerformance performance, WekaClassifier classifier)
        {
            //checks confidence level (if maximal support from all trees is sufficient)
            var predictedLabel = classification.Label;
            var support = classification.Accuracy;
            var numCorrect = 0u;
            var numIncorrect = 0u;
            var numUnsupported = 0u;

            //increases counters and measures accordingly
            if ((predictedLabel == null) || (support < MIN_SUPPORT))
                numUnsupported = 1;
            else
            {
                performance.ConfusionMatrix[classifier.Labels.IndexOf(predictedLabel)][
                    classifier.Labels.IndexOf(label)]++;
                if (predictedLabel.Equals(label)) numCorrect = 1;
                else numIncorrect = 1;
            }

            performance.AddPerformance(numCorrect, numIncorrect, numUnsupported, support);
        }

        private static Dictionary<string, string> GetInstance(string[] fields)
        {
            var features = new Dictionary<string, string>
                           {
                               {"L-Anger", fields[1]},
                               {"L-Disgust", fields[2]},
                               {"L-Fear", fields[3]},
                               {"L-Joy", fields[4]},
                               {"L-Sadness", fields[5]},
                               {"L-Surprise", fields[6]},
                               {"L-Neutral", fields[7]},
                               {"L-LookAt", fields[8]},
                               {"L-LookAtX", fields[9]},
                               {"L-LookAtY", fields[10]},
                               {"L-Smile", fields[11]},
                               {"R-Anger", fields[12]},
                               {"R-Disgust", fields[13]},
                               {"R-Fear", fields[14]},
                               {"R-Joy", fields[15]},
                               {"R-Sadness", fields[16]},
                               {"R-Surprise", fields[17]},
                               {"R-Neutral", fields[18]},
                               {"R-LookAt", fields[19]},
                               {"R-LookAtX", fields[20]},
                               {"R-LookAtY", fields[21]},
                               {"R-Smile", fields[22]}
                           };
            return features;
        }
    }
}