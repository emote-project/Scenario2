using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class StructureUpgradeReferences : NamedElement
    {
        [XmlElement("supportedstructure", Form = XmlSchemaForm.Unqualified)]
        public string[] Structures { get; set; }
    }
}