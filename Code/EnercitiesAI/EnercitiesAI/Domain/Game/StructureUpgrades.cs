using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;
using PS.Utilities;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot("structureupgrades", Namespace = "", IsNullable = false)]
    public class StructureUpgrades : NameTypedElementList<StructureUpgradeReferences, UpgradeType>
    {
        private readonly Dictionary<StructureType, HashSet<UpgradeType>> _structureUpgrades =
            new Dictionary<StructureType, HashSet<UpgradeType>>();

        [XmlElement("upgrade", Form = XmlSchemaForm.Unqualified)]
        public new StructureUpgradeReferences[] Items
        {
            get { return base.Items; }
            set
            {
                base.Items = value;
                this.Init();
            }
        }

        [XmlIgnore]
        public HashSet<UpgradeType> this[StructureType structureType]
        {
            get
            {
                return !this._structureUpgrades.ContainsKey(structureType)
                    ? new HashSet<UpgradeType>()
                    : this._structureUpgrades[structureType];
            }
        }

        public override void Dispose()
        {
            this._structureUpgrades.Clear();
            base.Dispose();
        }

        public void Init()
        {
            foreach (var structureUpgradeReferences in this.Items)
            {
                var upgradeType = EnumUtil<UpgradeType>.GetType(structureUpgradeReferences.Name);
                if (structureUpgradeReferences.Structures == null) continue;
                foreach (var structureReference in structureUpgradeReferences.Structures)
                {
                    var structureType = EnumUtil<StructureType>.GetType(structureReference);
                    if (!this._structureUpgrades.ContainsKey(structureType))
                        this._structureUpgrades[structureType] = new HashSet<UpgradeType>();
                    if (!this._structureUpgrades[structureType].Contains(upgradeType))
                        this._structureUpgrades[structureType].Add(upgradeType);
                }
            }
        }
    }
}