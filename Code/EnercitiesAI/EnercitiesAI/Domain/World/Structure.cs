using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Structure : GameValuesElement
    {
        [XmlElement("category", Form = XmlSchemaForm.Unqualified)]
        public string CategoryStr
        {
            get { return this.Category.ToString(); }
            set { this.Category = EnumUtil<StructureCategory>.GetType(value); }
        }

        [XmlElement("unlocklevel", Form = XmlSchemaForm.Unqualified)]
        public int UnlockLevel { get; set; }

        [XmlElement("buildingcost", Form = XmlSchemaForm.Unqualified)]
        public string BuildingcostStr
        {
            get { return this.BuildingCost.ToString(XmlParseUtil.CultureInfo); }
            set { this.BuildingCost = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("buildtime", Form = XmlSchemaForm.Unqualified)]
        public string BuildtimeStr
        {
            get { return this.BuildTime.ToString(XmlParseUtil.CultureInfo); }
            set { this.BuildTime = XmlParseUtil.ParseDouble(value); }
        }

        [XmlIgnore]
        public double BuildingCost { get; private set; }

        [XmlIgnore]
        public double BuildTime { get; private set; }

        [XmlIgnore]
        public StructureType Type
        {
            get { return EnumUtil<StructureType>.GetType(this.Name); }
            set { this.Name = value.ToString(); }
        }

        [XmlIgnore]
        public StructureCategory Category { get; set; }
    }
}