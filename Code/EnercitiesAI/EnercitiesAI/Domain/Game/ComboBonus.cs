using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    public class ComboBonus
    {
        [XmlIgnore]
        public string IndexType { get; protected set; }

        [XmlIgnore]
        public string ReferenceType { get; protected set; }

        [XmlElement("bonus")]
        public GameValuesElement Bonus { get; set; }
    }
}