using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("surfaces", Namespace = "", IsNullable = false)]
    public class Surfaces : NameTypedElementList<Surface, SurfaceType>
    {
        public Surfaces()
        {
            this.BuildRules = new Dictionary<SurfaceType, HashSet<StructureType>>();
        }

        [XmlElement("surface", Form = XmlSchemaForm.Unqualified)]
        public new Surface[] Items
        {
            get { return base.Items; }
            set
            {
                base.Items = value;
                this.Init();
            }
        }

        [XmlIgnore]
        public Dictionary<SurfaceType, HashSet<StructureType>> BuildRules { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            this.BuildRules.Clear();
        }

        protected void Init()
        {
            foreach (var surface in this.Items)
            {
                this.BuildRules[surface.Type] = new HashSet<StructureType>();
                foreach (var buildRule in surface.BuildRules)
                {
                    var structure = EnumUtil<StructureType>.GetType(buildRule);
                    if (!this.BuildRules[surface.Type].Contains(structure))
                        this.BuildRules[surface.Type].Add(structure);
                }
            }
        }
    }
}