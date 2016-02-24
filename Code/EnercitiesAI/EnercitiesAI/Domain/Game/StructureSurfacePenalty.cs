using System;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class StructureSurfacePenalty : ComboBonus
    {
        [XmlElement("structurecategory")]
        public TypeElement StructureCategory
        {
            get { return new TypeElement {Type = this.IndexType}; }
            set { this.IndexType = value.Type; }
        }

        [XmlElement("surface")]
        public TypeElement SurfaceName
        {
            get { return new TypeElement {Type = this.ReferenceType}; }
            set { this.ReferenceType = value.Type; }
        }
    }
}