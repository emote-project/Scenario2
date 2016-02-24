using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class ScenarioStartValues : ScoreElement
    {
        [XmlElement("population", Form = XmlSchemaForm.Unqualified)]
        public int Population { get; set; }


        [XmlElement("yearsInGame", Form = XmlSchemaForm.Unqualified)]
        public int YearsInGame { get; set; }


        [XmlElement("cash", Form = XmlSchemaForm.Unqualified)]
        public string CashStr
        {
            get { return this.Money.ToString(XmlParseUtil.CultureInfo); }
            set { this.Money = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("taxPerPerson", Form = XmlSchemaForm.Unqualified)]
        public string TaxPerPersonStr
        {
            get { return this.TaxPerPerson.ToString(XmlParseUtil.CultureInfo); }
            set { this.TaxPerPerson = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("oil", Form = XmlSchemaForm.Unqualified)]
        public string OilStr
        {
            get { return this.Oil.ToString(XmlParseUtil.CultureInfo); }
            set { this.Oil = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("startyear", Form = XmlSchemaForm.Unqualified)]
        public int StartYear { get; set; }

        [XmlIgnore]
        public double Money { get; private set; }

        [XmlIgnore]
        public double Oil { get; private set; }

        [XmlIgnore]
        public double TaxPerPerson { get; private set; }
    }
}