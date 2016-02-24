using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("upgrades", Namespace = "", IsNullable = false)]
    public class Upgrades : NameTypedElementList<Upgrade, UpgradeType>
    {
        [XmlElement("upgrade", Form = XmlSchemaForm.Unqualified)]
        public new Upgrade[] Items
        {
            get { return base.Items; }
            set { base.Items = value; }
        }
    }
}