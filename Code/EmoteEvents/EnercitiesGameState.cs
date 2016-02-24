using System;
using System.IO;
using Newtonsoft.Json;

namespace EmoteEnercitiesMessages
{
    public class EnercitiesGameInfo
    {
        private static EnercitiesGameInfo _lastDeserializedState;

        public EnercitiesGameInfo()
        {
        }

        //serialized fields -->
        public int Level { get; set; }
        public int Population { get; set; }
        public int TargetPopulation { get; set; }
        public double Money { get; set; }
        public double MoneyEarning { get; set; }
        public double Oil { get; set; }
        public double PowerConsumption { get; set; }
        public double PowerProduction { get; set; }
        public double EnvironmentScore { get; set; }
        public double EconomyScore { get; set; }
        public double WellbeingScore { get; set; }
        public double GlobalScore { get; set; }
        public EnercitiesRole CurrentRole { get; set; }
        //<--

        public bool Equals(EnercitiesGameInfo other)
        {
            return Level == other.Level && Population == other.Population && Money.Equals(other.Money) &&
                   MoneyEarning.Equals(other.MoneyEarning) && Oil.Equals(other.Oil) &&
                   PowerConsumption.Equals(other.PowerConsumption) && PowerProduction.Equals(other.PowerProduction) &&
                   EnvironmentScore.Equals(other.EnvironmentScore) && EconomyScore.Equals(other.EconomyScore) &&
                   WellbeingScore.Equals(other.WellbeingScore) && GlobalScore.Equals(other.GlobalScore) &&
                   CurrentRole == other.CurrentRole;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Level;
                hashCode = (hashCode*397) ^ Population;
                hashCode = (hashCode*397) ^ Money.GetHashCode();
                hashCode = (hashCode*397) ^ MoneyEarning.GetHashCode();
                hashCode = (hashCode*397) ^ Oil.GetHashCode();
                hashCode = (hashCode*397) ^ PowerConsumption.GetHashCode();
                hashCode = (hashCode*397) ^ PowerProduction.GetHashCode();
                hashCode = (hashCode*397) ^ EnvironmentScore.GetHashCode();
                hashCode = (hashCode*397) ^ EconomyScore.GetHashCode();
                hashCode = (hashCode*397) ^ WellbeingScore.GetHashCode();
                hashCode = (hashCode*397) ^ GlobalScore.GetHashCode();
                hashCode = (hashCode*397) ^ (int) CurrentRole;
                return hashCode;
            }
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is EnercitiesGameInfo) && (this.Equals((EnercitiesGameInfo) obj));
        }

        public override string ToString()
        {
            return
                String.Format(
                    "Level:{0}, Population:{1}, TargetPopulation:{2}, Money:{3}, Oil:{4}, PowerConsumption:{5}, PowerProduction:{6}, EnvironmentScore:{7}, EconomyScore:{8}, WellbeingScore:{9}, GlobalScore:{10}, CurrentRole:{11}",
                    Level, Population, TargetPopulation, Money, Oil, PowerConsumption, PowerProduction, EnvironmentScore,
                    EconomyScore, WellbeingScore, GlobalScore, CurrentRole.ToString());
        }

        public static EnercitiesGameInfo DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                _lastDeserializedState =
                    (EnercitiesGameInfo) serializer.Deserialize(textReader, typeof (EnercitiesGameInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize EnercitiesGameInfo from '" + serialized + "': " + e.Message);
            }
            return _lastDeserializedState;
        }
    }
}