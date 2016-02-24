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
    [XmlRoot("structures", Namespace = "", IsNullable = false)]
    public class Structures : NameTypedElementList<Structure, StructureType>
    {
        private readonly Dictionary<StructureCategory, HashSet<StructureType>>[] _levelStructures =
            new Dictionary<StructureCategory, HashSet<StructureType>>[DomainInfo.MAX_LEVELS];

        public Structures()
        {
            this.StructureCategories = new Dictionary<StructureType, StructureCategory>();
        }

        [XmlElement("structure", Form = XmlSchemaForm.Unqualified)]
        public new Structure[] Items
        {
            get { return base.Items; }
            set
            {
                base.Items = value;
                this.Init();
            }
        }

        [XmlIgnore]
        public Dictionary<StructureType, StructureCategory> StructureCategories { get; private set; }

        [XmlIgnore]
        public HashSet<StructureType> this[int upToLevel]
        {
            get
            {
                var structures = new HashSet<StructureType>();
                foreach (var category in EnumUtil<StructureCategory>.GetTypes())
                    structures.UnionWith(this[upToLevel, category]);
                return structures;
            }
        }


        [XmlIgnore]
        public HashSet<StructureType> this[int upToLevel, StructureCategory category]
        {
            get
            {
                var structures = new HashSet<StructureType>();
                for (var l = 0; (l <= upToLevel) && (l < DomainInfo.MAX_LEVELS); l++)
                    if (this._levelStructures[l].ContainsKey(category))
                        structures.UnionWith(this._levelStructures[l][category]);
                return structures;
            }
        }

        public override void Dispose()
        {
            foreach (var set in this._levelStructures)
                set.Clear();
            this.StructureCategories.Clear();
            base.Dispose();
        }

        public void Init()
        {
            foreach (var structure in this.Items)
            {
                var level = structure.UnlockLevel - 1;
                if (this._levelStructures[level] == null)
                    this._levelStructures[level] = new Dictionary<StructureCategory, HashSet<StructureType>>();
                if (!this._levelStructures[level].ContainsKey(structure.Category))
                    this._levelStructures[level][structure.Category] = new HashSet<StructureType>();
                if (!this._levelStructures[level][structure.Category].Contains(structure.Type))
                    this._levelStructures[level][structure.Category].Add(structure.Type);
                this.StructureCategories[structure.Type] = structure.Category;
            }
        }
    }
}