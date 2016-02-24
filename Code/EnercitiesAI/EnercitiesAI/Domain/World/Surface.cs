using System;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Surface : ScoreElement
    {
        [XmlIgnore]
        public SurfaceType Type
        {
            get { return EnumUtil<SurfaceType>.GetType(this.Name); }
            set { this.Name = value.ToString(); }
        }

        [XmlArray("build-rules", Form = XmlSchemaForm.Unqualified),
         XmlArrayItem("structure", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string[] BuildRules { get; set; }
    }
}