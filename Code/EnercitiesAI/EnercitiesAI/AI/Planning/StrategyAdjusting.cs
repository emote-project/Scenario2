using System;
using EmoteEnercitiesMessages;
using EmoteEvents;
using EnercitiesAI.Domain;

namespace EnercitiesAI.AI.Planning
{
    public class StrategyAdjustment
    {
        public static Strategy GetAdjustedStrategy(
            EnercitiesRole playerRole, Strategy playerStrategy, GameValuesElement gameValues,
            double strategyAdjustment, double populationValue)
        {
            var strategy = playerStrategy.Clone();

            AdjustResources(gameValues, strategy);
            if (!playerRole.Equals(EnercitiesRole.Environmentalist))
                AdjustEnvironment(gameValues, strategy);
            AdjustScores(playerRole, gameValues, strategyAdjustment, strategy);
            AdjustHomes(populationValue, strategy);

            //finally normalize strategy
            strategy.Normalize();
            return strategy;
        }

        private static void AdjustHomes(double populationValue, Strategy strategy)
        {
            //adjusts homes weight based on no-space and no-play probabilities and homes-to-next-level
            strategy.HomesWeight += HomesAdjustment(strategy.HomesAdjustParam).Invoke(populationValue);
        }

        private static void AdjustEnvironment(GameValuesElement gameValues, Strategy strategy)
        {
            //it is easy to loose environment score: every player/role cares about this 
            var environmentWeight = EnvironmentAdjustment(
                strategy.EnvironmentAdjustParam, strategy.ScoreAdjustParam).Invoke(gameValues.EnvironmentScore);
            strategy.EnvironmentWeight += environmentWeight;
        }

        private static void AdjustScores(
            EnercitiesRole playerRole, GameValuesElement gameValues, double strategyAdjustment, Strategy strategy)
        {
            //adjusts the respective player score weights based on predicted score values
            if (playerRole.Equals(EnercitiesRole.Economist))
                strategy.EconomyWeight +=
                    ScoreAdjustment(strategyAdjustment, strategy.ScoreAdjustParam).Invoke(gameValues.EconomyScore);
            else if (playerRole.Equals(EnercitiesRole.Mayor))
                strategy.WellbeingWeight +=
                    ScoreAdjustment(strategyAdjustment, strategy.ScoreAdjustParam).Invoke(gameValues.WellbeingScore);
            else if (playerRole.Equals(EnercitiesRole.Environmentalist))
                strategy.EnvironmentWeight +=
                    ScoreAdjustment(strategyAdjustment, strategy.ScoreAdjustParam).Invoke(gameValues.EnvironmentScore);
        }

        private static void AdjustResources(GameValuesElement gameValues, Strategy strategy)
        {
            //adjusts the several resources weights based on predicted resource levels
            strategy.PowerWeight += ResourceAdjustment(strategy.PowerAdjustParam).Invoke(gameValues.Power);
            strategy.MoneyWeight += ResourceAdjustment(strategy.MoneyAdjustParam).Invoke(gameValues.Money);
            strategy.OilWeight += ResourceAdjustment(strategy.OilAdjustParam).Invoke(gameValues.Oil);
        }

        public static Func<double, double> HomesAdjustment(double param)
        {
            return x => x > 2 ? 0 : 1 - Math.Pow(param, (2 - x));
        }

        public static Func<double, double> ResourceAdjustment(double param)
        {
            return x => Math.Pow(param, x);
        }

        public static Func<double, double> ScoreAdjustment(double multiplyFactor, double curveParam)
        {
            return x => multiplyFactor*Math.Pow(curveParam, x);
        }

        public static Func<double, double> EnvironmentAdjustment(double multiplyFactor, double curveParam)
        {
            return x => multiplyFactor*Math.Pow(curveParam, x);
        }
    }
}