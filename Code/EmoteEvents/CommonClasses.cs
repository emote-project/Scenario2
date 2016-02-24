using System;
using System.IO;
using EmoteEnercitiesMessages;
using Newtonsoft.Json;
using ProtoBuf;

namespace EmoteEvents
{
    [Serializable]
    [ProtoContract]
    public class EnercitiesGameInfo : ICloneable
    {
        public EnercitiesGameInfo()
        {
        }

        public EnercitiesGameInfo(EnercitiesGameInfo gameInfo)
        {
            this.Level = gameInfo.Level;
            this.Population = gameInfo.Population;
            this.TargetPopulation = gameInfo.TargetPopulation;
            this.Money = gameInfo.Money;
            this.MoneyEarning = gameInfo.MoneyEarning;
            this.Oil = gameInfo.Oil;
            this.PowerConsumption = gameInfo.PowerConsumption;
            this.PowerProduction = gameInfo.PowerProduction;
            this.EnvironmentScore = gameInfo.EnvironmentScore;
            this.EconomyScore = gameInfo.EconomyScore;
            this.WellbeingScore = gameInfo.WellbeingScore;
            this.GlobalScore = gameInfo.GlobalScore;
            this.CurrentRole = gameInfo.CurrentRole;
        }

        #region Serialized fields

        private static EnercitiesGameInfo _lastDeserializedState;

        [ProtoMember(1)]
        public EnercitiesRole CurrentRole { get; set; }

        [ProtoMember(2)]
        public int Level { get; set; }

        [ProtoMember(3)]
        public int Population { get; set; }

        [ProtoMember(4)]
        public int TargetPopulation { get; set; }

        [ProtoMember(5)]
        public double Money { get; set; }

        [ProtoMember(6)]
        public double MoneyEarning { get; set; }

        [ProtoMember(7)]
        public double Oil { get; set; }

        [ProtoMember(8)]
        public double PowerConsumption { get; set; }

        [ProtoMember(9)]
        public double PowerProduction { get; set; }

        [ProtoMember(10)]
        public double EnvironmentScore { get; set; }

        [ProtoMember(11)]
        public double EconomyScore { get; set; }

        [ProtoMember(12)]
        public double WellbeingScore { get; set; }

        [ProtoMember(13)]
        public double GlobalScore { get; set; }

        #endregion

        #region Serialization methods

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
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

        #endregion

        #region Equality methods

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public override bool Equals(object obj)
        {
            return (obj is EnercitiesGameInfo) && this.Equals((EnercitiesGameInfo) obj);
        }

        public bool Equals(EnercitiesGameInfo other)
        {
            return Level == other.Level &&
                   Population == other.Population &&
                   TargetPopulation == other.TargetPopulation &&
                   Money.Equals(other.Money) &&
                   MoneyEarning.Equals(other.MoneyEarning) &&
                   Oil.Equals(other.Oil) &&
                   PowerConsumption.Equals(other.PowerConsumption) &&
                   PowerProduction.Equals(other.PowerProduction) &&
                   EnvironmentScore.Equals(other.EnvironmentScore) &&
                   EconomyScore.Equals(other.EconomyScore) &&
                   WellbeingScore.Equals(other.WellbeingScore) &&
                   GlobalScore.Equals(other.GlobalScore) &&
                   CurrentRole == other.CurrentRole;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Level;
                hashCode = (hashCode*397) ^ Population;
                hashCode = (hashCode*397) ^ TargetPopulation;
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

        public EnercitiesGameInfo Clone()
        {
            return new EnercitiesGameInfo(this);
        }

        #endregion

        public override string ToString()
        {
            return
                String.Format(
                    "Lv:{0}, Pop:{1}, TrgtPop:{2}, Mon:{3:0.#}, Oil:{4:0.#}, PwCons:{5:0.##}, PwProd:{6:0.##}, EnvScr:{7:0.#}, EcoScr:{8:0.#}, WellScr:{9:0.#}, GlobScr:{10:0.#}, Role:{11}",
                    Level, Population, TargetPopulation, Money, Oil, PowerConsumption, PowerProduction, EnvironmentScore,
                    EconomyScore, WellbeingScore, GlobalScore, CurrentRole);
        }
    }

    public class EmoteUser
    {
        public enum GenderType { Male, Female };
        private static EmoteUser _lastDeserializedState;

        string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        private GenderType gender;

        public GenderType Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public string SerializeToJson()
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static EmoteUser DeserializeFromJson(string serialized)
        {
            try
            {
                var textReader = new StringReader(serialized);
                var serializer = new JsonSerializer();
                _lastDeserializedState =
                    (EmoteUser)serializer.Deserialize(textReader, typeof(EnercitiesGameInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize EnercitiesGameInfo from '" + serialized + "': " + e.Message);
            }
            return _lastDeserializedState;
        }
    }
}