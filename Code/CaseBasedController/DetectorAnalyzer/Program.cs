using System;
using System.Collections.Generic;
using System.IO;
using Classification;
using Classification.Classifier;
using DetectorAnalyzer.Evaluation;

namespace DetectorAnalyzer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var file = "category.arff";
            var file = "subcategory.arff";
            if (args.Length > 0)
                file = args[0];

            var ignoreBehaviors = new HashSet<string>
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

            var evaluator = new CrossValidatonTest(1, new TreeClassifier { MinFrequency = 0.00 }) { MinSupport = 0.00 };
            //var evaluator = new SplitTest(0.66, new TreeClassifier { MinFrequency = 0.00 }) { MinSupport = 0.00 };
            evaluator.IgnoreLabels.UnionWith(ignoreBehaviors);
            var performance = evaluator.Test(file);
            var baseDir = Path.GetFullPath(string.Format("./{0}", Path.GetFileNameWithoutExtension(file)));
            var filePath = Path.GetFullPath(string.Format("{0}/performance.csv", baseDir));
            performance.PrintResults(filePath);
            evaluator.Dispose();


            //var trainArffLoader = new TrainClassifierArffLoader(new TreeClassifier {MinFrequency = 0.00});
            //trainArffLoader.IgnoreLabels.UnionWith(ignoreBehaviors);
            //trainArffLoader.Load(file);

            //var baseDir = Path.GetFullPath(string.Format("./{0}", Path.GetFileNameWithoutExtension(file)));
            //trainArffLoader.PrintResults(Path.GetFullPath(baseDir));

            //var testArffLoader = new TestClassifierArffLoader(trainArffLoader) {MinSupport = 0.00};
            //testArffLoader.IgnoreLabels.UnionWith(ignoreBehaviors);
            //testArffLoader.Load(file);
            //testArffLoader.PrintResults(baseDir);
            //testArffLoader.Dispose();

            //trainArffLoader.Dispose();

            Console.WriteLine("\n======================================");
            Console.WriteLine("Finished processing!");
            //Console.ReadKey();
        }
    }
}