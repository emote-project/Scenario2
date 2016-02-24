using System;
using System.IO;
using EnercitiesAI.AI.Estimation;
using PS.Utilities.Serialization;

namespace EnercitiesAI.Programs
{
    internal class RandomWalkProgam
    {
        private const string PATH_RESULTS = "./state-action-estimations.json";
        private const int NUM_GAMES = 1000000;

        private static void Main(string[] args)
        {
            using (var estimator = new RandomWalkEstimator(NUM_GAMES))
            {
                estimator.Estimate(true);
                estimator.SerializeJsonFile(Path.GetFullPath(PATH_RESULTS));
            }

            Console.WriteLine("\nEstimations finished!");
            //Console.ReadKey();
        }
    }
}