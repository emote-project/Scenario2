using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    [XmlRoot("points", Namespace = "", IsNullable = false)]
    public class VictoryPoints : NamedElementList<VictoryPoint>
    {
        [XmlElement("item", Form = XmlSchemaForm.Unqualified)]
        public new VictoryPoint[] Items
        {
            get { return base.Items; }
            set { base.Items = value; }
        }
    }
}