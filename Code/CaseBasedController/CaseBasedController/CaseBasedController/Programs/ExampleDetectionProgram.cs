using System;
using System.Threading;
using CaseBasedController.Behavior;
using CaseBasedController.Detection;
using CaseBasedController.Example;
using PS.Utilities.Serialization;

namespace CaseBasedController.Programs
{
    internal class ExampleDetectionProgram
    {
        private const int NUM_STEPS = 100;

        private static void Main(string[] args)
        {
            var casePool = new CasePool();
            IFeatureDetector detector = new RandomFeatureDetector {ActivationThreshold = 0.7};
            IBehavior behavior = new PrintConsoleBehavior {Text = "Activated!"};
            var sampleCase = new Case(detector, behavior);
            casePool.Add(sampleCase);

            detector = new MinuteSecondDetector {Second = 20};
            behavior = new MessageBoxBehavior {Text = "Hey!"};
            sampleCase = new Case(detector, behavior);
            casePool.Add(sampleCase);

            //TestExecute(sampleCase);

            detector.SerializeJsonFile("detector.json", JsonUtil.TypeSpecifySettings);
            behavior.SerializeJsonFile("behavior.json", JsonUtil.TypeSpecifySettings);
            sampleCase.SerializeJsonFile("case.json", JsonUtil.TypeSpecifySettings);
            casePool.SerializeToJson("casepool.json");

            //console.writeline("Tests finished");
            Console.ReadKey();
        }

        private static void TestExecute(Case sampleCase)
        {
            for (var i = 0; i < NUM_STEPS; i++)
            {
                //console.writeline("Time step {0}", i);
                if (sampleCase.Detector.IsActive)
                    sampleCase.Behavior.Execute(sampleCase.Detector);
                Thread.Sleep(50);
            }
        }
    }
}