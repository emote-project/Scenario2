using System;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;

namespace EmoteEvents
{
    /// <summary>
    ///     Defines the strategy of a certain player.
    ///     Each weight [-1; 1] is associated with a particular aspect/value of the game and asserts
    ///     the player "preference" relating that value.
    ///     Higher values mean the player pays more attention to the respective game value.
    ///     A strategy can be modified over time by approximating other strategies.
    ///     A strategy can be inferred given some game values.
    /// </summary>
    public class Strategy : IDisposable, ICloneable
    {
        #region Fields

        public const int NUM_WEIGHTS = 8;
        private readonly DenseVector _weights = new DenseVector(NUM_WEIGHTS);

        #endregion

        #region Constructors

        public Strategy()
        {
        }


        public Strategy(double[] values)
        {
          //  values.CopyTo(this._weights);
            this._weights = values;
        }

        #endregion

        #region Properties

        public static readonly Strategy Empty = new Strategy();

        public double EconomyWeight
        {
            get { return this._weights[0]; }
            set { this._weights[0] = NormalizeValue(value); }
        }

        public double EnvironmentWeight
        {
            get { return this._weights[1]; }
            set { this._weights[1] = NormalizeValue(value); }
        }

        public double WellbeingWeight
        {
            get { return this._weights[2]; }
            set { this._weights[2] = NormalizeValue(value); }
        }

        public double MoneyWeight
        {
            get { return this._weights[3]; }
            set { this._weights[3] = NormalizeValue(value); }
        }

        public double PowerWeight
        {
            get { return this._weights[4]; }
            set { this._weights[4] = NormalizeValue(value); }
        }

        public double OilWeight
        {
            get { return this._weights[5]; }
            set { this._weights[5] = NormalizeValue(value); }
        }

        public double HomesWeight
        {
            get { return this._weights[6]; }
            set { this._weights[6] = NormalizeValue(value); }
        }

        public double ScoreUniformityWeight
        {
            get { return this._weights[7]; }
            set { this._weights[7] = NormalizeValue(value); }
        }

        [JsonIgnore]
        public double[] Weights
        {
            get { return this._weights.Values; }
        }

        public double PowerAdjustParam { get; set; }

        public double MoneyAdjustParam { get; set; }

        public double OilAdjustParam { get; set; }

        public double ScoreAdjustParam { get; set; }

        public double HomesAdjustParam { get; set; }

        public double EnvironmentAdjustParam { get; set; }

        #endregion

        #region Math operations

        private static double NormalizeValue(double value)
        {
            return value > 1d ? 1d : (value < -1d ? -1d : value);
        }

        public static implicit operator DenseVector(Strategy strategy)
        {
            return strategy._weights;
        }

        public static Strategy operator *(Strategy strategy, double value)
        {
            var newStrat = strategy.Clone();
            (strategy._weights*value).CopyTo(newStrat._weights);
            return newStrat;
        }

        public static double operator *(Strategy strategy1, Strategy strategy2)
        {
            return strategy1._weights*strategy2._weights;
        }

        public void Normalize()
        {
            //normalize weights (abs values sum)
            var sum = this._weights.Sum(weight => Math.Abs(weight));
            if (sum.Equals(0)) return;
            for (var i = 0; i < this._weights.Count; i++)
                this._weights[i] /= sum;
        }

        #endregion

        #region Equality members

        public override bool Equals(object obj)
        {
            return (obj is Strategy) && this.Equals((Strategy) obj);
        }

        public bool Equals(Strategy other)
        {
            return this._weights.Equals(other._weights);
        }

        public override int GetHashCode()
        {
            return (this._weights != null ? this._weights.GetHashCode() : 0);
        }

        #endregion

        #region Public Methods

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this._weights.Clear();
        }

        #endregion

        public override string ToString()
        {
            var str = new StringBuilder("[");
            foreach (var elem in this._weights)
                str.Append(string.Format("{0:0.0};", elem));
            str.Remove(str.Length - 1, 1);
            str.Append("]");
            return str.ToString();
        }

        public Strategy Clone()
        {
            var clone = new Strategy
                        {
                            PowerAdjustParam = this.PowerAdjustParam,
                            MoneyAdjustParam = this.MoneyAdjustParam,
                            OilAdjustParam = this.OilAdjustParam,
                            ScoreAdjustParam = this.ScoreAdjustParam,
                            HomesAdjustParam = this.HomesAdjustParam,
                            EnvironmentAdjustParam = this.EnvironmentAdjustParam,
                        };
            this._weights.CopyTo(clone._weights);
            return clone;
        }

        public string SerializeToJson()
        {
            using (var textWriter = new StringWriter())
            {
                new JsonSerializer().Serialize(textWriter, this);
                return textWriter.ToString();
            }

        }

        public static Strategy DeserializeFromJson(string serialized)
        {
            try
            {
                using (var textReader = new StringReader(serialized))
                    return (Strategy) new JsonSerializer().Deserialize(textReader, typeof (Strategy));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize Strategy from '{0}': {1}", serialized, e.Message);
            }
            return null;
        }

        #endregion
    }
}