using System;
using System.Globalization;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class GridUnit
    {
        [XmlElement("surface", Form = XmlSchemaForm.Unqualified)]
        public TypeElement Surface { get; set; }

        [XmlElement("structure", Form = XmlSchemaForm.Unqualified)]
        public NamedElement Structure { get; set; }

        [XmlAttribute("x")]
        public int X { get; set; }

        [XmlAttribute("y")]
        public int Y { get; set; }

        [XmlIgnore]
        public Coordinate Coordinate
        {
            get { return new Coordinate(this.X, this.Y); }
        }

        [XmlAttribute("level")]
        public string LevelStr
        {
            get { return (this.Level + 1).ToString(CultureInfo.InvariantCulture); }
            set { this.Level = int.Parse(value, CultureInfo.InvariantCulture) - 1; }
        }

        [XmlIgnore]
        public int Level { get; set; }

        [XmlIgnore]
        public StructureType StructureType
        {
            get { return EnumUtil<StructureType>.GetType(this.Structure.Name); }
            set { this.Structure.Name = value.ToString(); }
        }

        [XmlIgnore]
        public SurfaceType SurfaceType
        {
            get { return EnumUtil<SurfaceType>.GetType(this.Surface.Type); }
            set { this.Surface.Type = value.ToString(); }
        }

        public override string ToString()
        {
            return string.Format("x:{0},y:{1},l:{2},su:{3},st:{4}",
                this.X, this.Y, this.Level, this.Surface, this.Structure);
        }
    }
}