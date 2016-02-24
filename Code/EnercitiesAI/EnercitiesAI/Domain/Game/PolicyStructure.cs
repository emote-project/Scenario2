using System;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class PolicyStructure : GameValuesElement, INamedElement
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlIgnore]
        public StructureType StructureType
        {
            get { return EnumUtil<StructureType>.GetType(this.Type); }
            set { this.Type = value.ToString(); }
        }

        #region INamedElement Members

        [XmlIgnore]
        string INamedElement.Name
        {
            get { return this.Type; }
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }
}