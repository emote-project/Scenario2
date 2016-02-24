using System;
using System.IO;
using EnercitiesAI.AI.Estimation;

namespace EnercitiesAI.Programs
{
    internal class GameValuesStatsTestProgam
    {
        private const int NUM_GAMES = 1000;
        private const int MAX_NUM_PLAYS = 100;
        private const string PATH_RESULTS = "./game-values-estimations.json";

        private static void Main(string[] args)
        {
            using (var estimator = new GameValuesStatsEstimator(NUM_GAMES, MAX_NUM_PLAYS))
            {
                estimator.Estimate(true);
                estimator.GameValuesStats.SaveToJson(Path.GetFullPath(PATH_RESULTS));
            }

            Console.WriteLine("\nEstimations finished!");
            Console.ReadKey();
        }
    }
}