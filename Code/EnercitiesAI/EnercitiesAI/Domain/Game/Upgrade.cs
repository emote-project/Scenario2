using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Upgrade : GameValuesElement
    {
        [XmlElement("researchcost", Form = XmlSchemaForm.Unqualified)]
        public string ResearchcostStr
        {
            get { return this.ResearchCost.ToString(XmlParseUtil.CultureInfo); }
            set { this.ResearchCost = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("researchtime", Form = XmlSchemaForm.Unqualified)]
        public string ResearchtimeStr
        {
            get { return this.ResearchTime.ToString(XmlParseUtil.CultureInfo); }
            set { this.ResearchTime = XmlParseUtil.ParseDouble(value); }
        }

        [XmlIgnore]
        public double ResearchCost { get; private set; }

        [XmlIgnore]
        public double ResearchTime { get; private set; }

        [XmlIgnore]
        public UpgradeType Type
        {
            get { return EnumUtil<UpgradeType>.GetType(this.Name); }
            set { this.Name = value.ToString(); }
        }
    }
}