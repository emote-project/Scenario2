using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class VictoryPoint : INamedElement
    {
        [XmlElement("triggertype", Form = XmlSchemaForm.Unqualified)]
        public string TriggerType { get; set; }

        [XmlElement("triggervalue", Form = XmlSchemaForm.Unqualified)]
        public string TriggervalueStr
        {
            get { return this.Triggervalue.ToString(XmlParseUtil.CultureInfo); }
            set { this.Triggervalue = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("numberofpoints", Form = XmlSchemaForm.Unqualified)]
        public string NumberofpointsStr
        {
            get { return this.NumberOfPoints.ToString(XmlParseUtil.CultureInfo); }
            set { this.NumberOfPoints = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("yearrequirement", Form = XmlSchemaForm.Unqualified)]
        public int YearRequirement { get; set; }

        [XmlAttribute("reference")]
        public string Reference { get; set; }

        [XmlIgnore]
        public double Triggervalue { get; private set; }

        [XmlIgnore]
        public double NumberOfPoints { get; private set; }

        #region INamedElement Members

        [XmlIgnore]
        public string Name
        {
            get { return this.Reference; }
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }
}