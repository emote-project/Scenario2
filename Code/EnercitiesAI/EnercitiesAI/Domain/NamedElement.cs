using System;
using System.Xml.Serialization;
using ProtoBuf;
using PS.Utilities.Serialization;

namespace EnercitiesAI.Domain
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [ProtoContract]
    [ProtoInclude(1, typeof(ScoreElement))]
    public class NamedElement : INamedElement, ICloneable
    {
        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region INamedElement Members

        [XmlAttribute("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        #endregion

        #region Equalities and serialization

        public override int GetHashCode()
        {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            return (obj is NamedElement) && this.Equals(((NamedElement) obj));
        }

        public bool Equals(NamedElement other)
        {
            return string.Equals(this.Name, other.Name);
        }

        public NamedElement Clone()
        {
            return this.CloneProto();
        }

        #endregion
    }
}