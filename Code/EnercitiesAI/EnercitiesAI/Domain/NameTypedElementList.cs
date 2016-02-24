using System.Xml.Serialization;

namespace EnercitiesAI.Domain
{
    public class NameTypedElementList<TElem, TType> : NamedElementList<TElem>
        where TElem : INamedElement where TType : struct
    {
        [XmlIgnore]
        public TElem this[TType type]
        {
            get { return this[type.ToString()]; }
        }

        public bool ContainsType(TType type)
        {
            return this.ContainsName(type.ToString());
        }
    }
}