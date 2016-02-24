using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class MessageEvent : INamedElement
    {
        [XmlElement("message", Form = XmlSchemaForm.Unqualified)]
        public Message Message { get; set; }

        [XmlArray("triggers", Form = XmlSchemaForm.Unqualified), XmlArrayItem("trigger", typeof (MessageTrigger),
            Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public MessageTrigger[] Triggers { get; set; }

        #region INamedElement Members

        [XmlIgnore]
        public string Name
        {
            get { return this.Message.Name; }
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }
}