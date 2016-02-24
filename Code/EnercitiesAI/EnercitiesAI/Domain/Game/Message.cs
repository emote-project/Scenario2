using System;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class Message : INamedElement
    {
        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlAttribute("repeattime")]
        public string RepeatTimeStr
        {
            get { return this.RepeatTime.ToString(XmlParseUtil.CultureInfo); }
            set { this.RepeatTime = XmlParseUtil.ParseDouble(value); }
        }

        [XmlAttribute("reference")]
        public string Reference { get; set; }

        [XmlIgnore]
        public double RepeatTime { get; private set; }

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