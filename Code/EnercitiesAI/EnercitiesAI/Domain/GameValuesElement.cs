using System;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEvents;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using ProtoBuf;

namespace EnercitiesAI.Domain
{
    public enum PowerType
    {
        Null,
        Producer,
        Consumer
    }

    [Serializable]
    [ProtoContract]
    public class GameValuesElement : ScoreElement
    {
        private const double MAX_ABS_ERROR = 0.05;
        private double _powerConsumption;
        private double _powerProduction;

        public GameValuesElement()
        {
            this.PowerType = PowerType.Null;
        }

        public GameValuesElement(GameValuesElement baseElem)
        {
            this.Add(baseElem);
            this.PowerType = baseElem.PowerType;
        }

        [XmlElement("annualcost", Form = XmlSchemaForm.Unqualified)]
        public string AnnualcostStr
        {
            get { return this.Money.ToString(XmlParseUtil.CultureInfo); }
            set { this.Money = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("annualenergy", Form = XmlSchemaForm.Unqualified)]
        public string AnnualenergyStr
        {
            get { return this.Power.ToString(XmlParseUtil.CultureInfo); }
            set
            {
                var energy = XmlParseUtil.ParseDouble(value);

                //sets type of power and consumption/production accordingly
                if (energy > 0)
                {
                    this.PowerType = PowerType.Producer;
                    this.PowerProduction = energy;
                }
                else if (energy < 0)
                {
                    this.PowerType = PowerType.Consumer;
                    this.PowerConsumption = energy;
                }
                else this.PowerType = PowerType.Null;
            }
        }

        [XmlElement("annualoil", Form = XmlSchemaForm.Unqualified)]
        public string AnnualoilStr
        {
            get { return this.Oil.ToString(XmlParseUtil.CultureInfo); }
            set { this.Oil = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("homes", Form = XmlSchemaForm.Unqualified)]
        [ProtoMember(7)]
        public double Homes { get; set; }

        [XmlIgnore]
        [ProtoMember(8)]
        public double Money { get; set; }

        [XmlIgnore]
        [ProtoMember(9)]
        public double Oil { get; set; }

        [XmlIgnore]
        [ProtoMember(10)]
        public double PowerProduction
        {
            get { return this._powerProduction; }
            set
            {
                if (this.PowerType.Equals(PowerType.Consumer))
                    return;

                if (this.PowerType.Equals(PowerType.Producer) && (value < 0))
                {
                    this.PowerType = PowerType.Consumer;
                    this.PowerConsumption = value;
                }
                else
                {
                    this._powerProduction = value;
                }
            }
        }

        [XmlIgnore]
        [ProtoMember(11)]
        public double PowerConsumption
        {
            get { return this._powerConsumption; }
            set
            {
                if (this.PowerType.Equals(PowerType.Producer))
                    return;

                if (this.PowerType.Equals(PowerType.Consumer) && (value > 0))
                {
                    this.PowerType = PowerType.Producer;
                    this.PowerProduction = value;
                }
                else
                {
                    this._powerConsumption = value;
                }
            }
        }

        [XmlIgnore]
        public double Power
        {
            get
            {
                return this.PowerType.Equals(PowerType.Producer)
                    ? this.PowerProduction
                    : this.PowerType.Equals(PowerType.Consumer)
                        ? this.PowerConsumption
                        : this.PowerProduction - Math.Abs(this.PowerConsumption);
            }
        }

        [XmlIgnore]
        [ProtoMember(12)]
        public PowerType PowerType { get; set; }

        #region Equality members

        public bool Equals(GameValuesElement other)
        {
            return base.Equals(other) &&
                   this.Homes.Equals(other.Homes) &&
                   this.Money.AlmostEqual(other.Money, MAX_ABS_ERROR) &&
                   this.Oil.AlmostEqual(other.Oil, MAX_ABS_ERROR) &&
                   this.PowerProduction.AlmostEqual(other.PowerProduction, MAX_ABS_ERROR) &&
                   this.PowerConsumption.AlmostEqual(other.PowerConsumption, MAX_ABS_ERROR);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ this.Homes.GetHashCode();
                hashCode = (hashCode*397) ^ this.Money.GetHashCode();
                hashCode = (hashCode*397) ^ this.Oil.GetHashCode();
                hashCode = (hashCode*397) ^ this.PowerProduction.GetHashCode();
                hashCode = (hashCode*397) ^ this.PowerConsumption.GetHashCode();
                hashCode = (hashCode*397) ^ (int) this.PowerType;
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is GameValuesElement) && this.Equals(((GameValuesElement) obj));
        }

        public bool IsZero()
        {
            return this.Equals(new GameValuesElement());
        }

        #endregion

        #region Conversions

        public static implicit operator DenseVector(GameValuesElement elem)
        {
            return new DenseVector(
                new[]
                {
                    elem.EconomyScore, elem.EnvironmentScore, elem.WellbeingScore,
                    elem.Money, elem.PowerConsumption, elem.PowerProduction,
                    elem.Oil, elem.Homes
                });
        }

        public static implicit operator GameValuesElement(DenseVector vec)
        {
            return new GameValuesElement
                   {
                       EconomyScore = vec[0],
                       EnvironmentScore = vec[1],
                       WellbeingScore = vec[2],
                       Money = vec[3],
                       PowerProduction = vec[4],
                       PowerConsumption = vec[5],
                       Oil = vec[6],
                       Homes = (int) vec[7],
                   };
        }

        public static implicit operator GameValuesElement(EnercitiesGameInfo gameInfo)
        {
            return new GameValuesElement
                   {
                       EconomyScore = gameInfo.EconomyScore,
                       EnvironmentScore = gameInfo.EnvironmentScore,
                       WellbeingScore = gameInfo.WellbeingScore,
                       Money = gameInfo.Money,
                       PowerProduction = gameInfo.PowerProduction,
                       PowerConsumption = gameInfo.PowerConsumption,
                       Oil = gameInfo.Oil,
                       Homes = gameInfo.Population,
                   };
        }

        #endregion

        #region Math operations

        public void Add(GameValuesElement elem)
        {
            this.EconomyScore += elem.EconomyScore;
            this.EnvironmentScore += elem.EnvironmentScore;
            this.WellbeingScore += elem.WellbeingScore;
            this.Money += elem.Money;
            this.Oil += elem.Oil;
            this.Homes += elem.Homes;

            //if element is marked as producer/consumer, add power total to production/consumption
            if (this.PowerType.Equals(PowerType.Producer))
                this.PowerProduction += elem.Power;
            else if (this.PowerType.Equals(PowerType.Consumer))
                this.PowerConsumption += elem.Power;
            else
            {
                this.PowerProduction += elem.PowerProduction;
                this.PowerConsumption += elem.PowerConsumption;
            }
        }

        public void Subtract(GameValuesElement elem)
        {
            this.EconomyScore -= elem.EconomyScore;
            this.EnvironmentScore -= elem.EnvironmentScore;
            this.WellbeingScore -= elem.WellbeingScore;
            this.Money -= elem.Money;
            this.Oil -= elem.Oil;
            this.Homes -= elem.Homes;

            //if element is marked as producer/consumer, subtract power total to production/consumption
            if (this.PowerType.Equals(PowerType.Producer))
                this.PowerProduction -= elem.Power;
            else if (this.PowerType.Equals(PowerType.Consumer))
                this.PowerConsumption -= elem.Power;
            else
            {
                this.PowerProduction -= elem.PowerProduction;
                this.PowerConsumption -= elem.PowerConsumption;
            }
        }

        #endregion

        public override string ToString()
        {
            var str = new StringBuilder("[");
            foreach (var elem in ((DenseVector) this))
                str.Append(string.Format("{0:0.0};", elem));
            str.Remove(str.Length - 1, 1);
            str.Append("]");
            return str.ToString();
        }
    }
}