using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    [XmlRoot("messages", Namespace = "", IsNullable = false)]
    public class MessageEvents : NamedElementList<MessageEvent>
    {
        [XmlElement("event", Form = XmlSchemaForm.Unqualified)]
        public new MessageEvent[] Items
        {
            get { return base.Items; }
            set { base.Items = value; }
        }
    }
}