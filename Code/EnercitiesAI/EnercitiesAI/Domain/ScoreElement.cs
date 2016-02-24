using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using ProtoBuf;
using PS.Utilities.Math;

namespace EnercitiesAI.Domain
{
    [Serializable]
    [ProtoContract]
    [ProtoInclude(3, typeof(GameValuesElement))]
    public class ScoreElement : NamedElement
    {
        [XmlElement("environmentscore", Form = XmlSchemaForm.Unqualified)]
        public string EnvironmentscoreStr
        {
            get { return this.EnvironmentScore.ToString(XmlParseUtil.CultureInfo); }
            set { this.EnvironmentScore = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("economyscore", Form = XmlSchemaForm.Unqualified)]
        public string EconomyscoreStr
        {
            get { return this.EconomyScore.ToString(XmlParseUtil.CultureInfo); }
            set { this.EconomyScore = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("wellbeingscore", Form = XmlSchemaForm.Unqualified)]
        public string WellbeingscoreStr
        {
            get { return this.WellbeingScore.ToString(XmlParseUtil.CultureInfo); }
            set { this.WellbeingScore = XmlParseUtil.ParseDouble(value); }
        }

        [XmlIgnore]
        [ProtoMember(4)]
        public double EnvironmentScore { get; set; }

        [XmlIgnore]
        [ProtoMember(5)]
        public double EconomyScore { get; set; }

        [XmlIgnore]
        [ProtoMember(6)]
        public double WellbeingScore { get; set; }

        [XmlIgnore]
        public double ScoresUniformity
        {
            get
            {
                var scores = new StatisticalQuantity (3);
                scores.Value = this.EnvironmentScore;
                scores.Value = this.EconomyScore;
                scores.Value = this.WellbeingScore;
                return scores.Avg.Equals(0) ? 0 : -scores.StdDev / scores.Avg;
            }
        }

        public bool Equals(ScoreElement other)
        {
            return base.Equals(other) &&
                   this.EnvironmentScore.Equals(other.EnvironmentScore) &&
                   this.EconomyScore.Equals(other.EconomyScore) &&
                   this.WellbeingScore.Equals(other.WellbeingScore);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ this.EnvironmentScore.GetHashCode();
                hashCode = (hashCode*397) ^ this.EconomyScore.GetHashCode();
                hashCode = (hashCode*397) ^ this.WellbeingScore.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is ScoreElement) && this.Equals(((ScoreElement) obj));
        }
    }
}