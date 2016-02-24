using System;
using System.Xml.Serialization;

namespace EnercitiesAI.Domain
{
    [Serializable()]
    [XmlType(AnonymousType = true)]
    public class TypeElement : INamedElement
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        #region INamedElement Members

        [XmlIgnore]
        public string Name
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