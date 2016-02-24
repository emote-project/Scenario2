using System;
using System.Text;
using EmoteEvents;
using EnercitiesAI.Domain;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Utilities.Serialization;

namespace EnercitiesAI.AI
{
    public static class StrategyExtensions
    {
        /// <summary>
        ///     When combined with some <see cref="GameValuesElement" /> it returns a value of how good
        ///     or bad the element is having into account the player's "preferences", i.e., its strategy.
        /// </summary>
        public static double GetObjectiveValue(
            Strategy strategy, GameValuesElement values, double scoreUniformity = double.NaN)
        {
            return strategy*FromGameValues(values, scoreUniformity, false);
        }

        public static Strategy FromGameValues(
            GameValuesElement values, double scoreUniformity = double.NaN, bool normalize = true)
        {
            var strategy = new Strategy
                           {
                               EconomyWeight = values.EconomyScore,
                               EnvironmentWeight = values.EnvironmentScore,
                               WellbeingWeight = values.WellbeingScore,
                               MoneyWeight = values.Money,
                               PowerWeight = values.Power,
                               OilWeight = values.Oil,
                               HomesWeight = values.Homes,
                               ScoreUniformityWeight =
                                   double.IsNaN(scoreUniformity) ? values.ScoresUniformity : scoreUniformity
                           };

            //normalizes strategy
            if (normalize) strategy.Normalize();

            return strategy;
        }

        public static string ToLongString(this Strategy strategy)
        {
            var str = new StringBuilder("[");
            str.Append(string.Format("ec{0:0.0};", strategy.EconomyWeight));
            str.Append(string.Format("ev{0:0.0};", strategy.EnvironmentWeight));
            str.Append(string.Format("wb{0:0.0};", strategy.WellbeingWeight));
            str.Append(string.Format("mn{0:0.0};", strategy.EconomyWeight));
            str.Append(string.Format("pw{0:0.0};", strategy.PowerWeight));
            str.Append(string.Format("oi{0:0.0};", strategy.OilWeight));
            str.Append(string.Format("hm{0:0.0};", strategy.HomesWeight));
            str.Append(string.Format("su{0:0.0};", strategy.ScoreUniformityWeight));
            str.Remove(str.Length - 1, 1);
            str.Append("]");
            return str.ToString();
        }

        #region Math transforms

        public static void Approximate(this Strategy strategy, Strategy otherStrategy, double amount)
        {
            //"approximates" the imitator strategy towards the other strategy, S = S + amount (OS - S)
            var diffPoint = (DenseVector) otherStrategy - strategy;
            var newPoint = strategy + (diffPoint*amount);
            Array.Copy(newPoint.Values, strategy.Weights, newPoint.Count);
            strategy.Normalize();
        }

        public static void Average(this Strategy strategy, Strategy otherStrategy, double amount)
        {
            var avg = ((DenseVector) otherStrategy*amount) + (strategy*(1 - amount));
            Array.Copy(avg.Values, strategy.Weights, avg.Count);
        }

        #endregion

        #region Serialization

        public static void Serialize(this Strategy strategy, string filePath)
        {
            strategy.SerializeJsonFile(filePath);
        }

        public static Strategy Deserialize(string filePath)
        {
            return JsonUtil.DeserializeJsonFile<Strategy>(filePath);
        }

        #endregion
    }
}