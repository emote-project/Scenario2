using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class ScenarioWinCondition
    {
        [XmlElement("population", Form = XmlSchemaForm.Unqualified)]
        public int Population { get; set; }

        [XmlElement("sustainablecity", Form = XmlSchemaForm.Unqualified)]
        public bool SustainableCity { get; set; }

        [XmlElement("partime", Form = XmlSchemaForm.Unqualified)]
        public int ParTime { get; set; }

        [XmlAttribute("level")]
        public int Level { get; set; }
    }
}