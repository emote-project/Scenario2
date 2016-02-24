using System;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class MessageTrigger : TypeElement
    {
        [XmlAttribute("timetillfiretrigger")]
        public string TimetillfiretriggerStr
        {
            get { return this.TimeTillFireTrigger.ToString(XmlParseUtil.CultureInfo); }
            set { this.TimeTillFireTrigger = XmlParseUtil.ParseDouble(value); }
        }

        [XmlIgnore]
        public double TimeTillFireTrigger { get; private set; }
    }
}