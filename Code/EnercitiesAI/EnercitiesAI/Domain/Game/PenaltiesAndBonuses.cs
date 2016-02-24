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
    [XmlRoot("penaltiesandbonusses", Namespace = "", IsNullable = false)]
    public class PenaltiesAndBonuses : IDisposable
    {
        private StructureStructurePenalty[] _structureStructurePenaltiesArray;
        private StructureSurfaceBonus[] _structureSurfaceBonusArray;
        private StructureSurfacePenalty[] _structureSurfacePenaltiesArray;

        public PenaltiesAndBonuses()
        {
            this.StructureStructureBonuses =
                new Dictionary<StructureCategory, Dictionary<StructureType, ComboBonus>>();
            this.StructureSurfaceBonus = new Dictionary<StructureType, Dictionary<SurfaceType, ComboBonus>>();
            this.StructureSurfacePenalties = new Dictionary<StructureCategory, Dictionary<SurfaceType, ComboBonus>>();
        }

        [XmlArray("combobonusses", Form = XmlSchemaForm.Unqualified), XmlArrayItem("combobonus",
            typeof (StructureSurfaceBonus), Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public StructureSurfaceBonus[] StructureSurfaceBonusArray
        {
            get { return this._structureSurfaceBonusArray; }
            set
            {
                this._structureSurfaceBonusArray = value;
                this.PopulateDictionary(value, this.StructureSurfaceBonus);
            }
        }

        [XmlArray("gridnexttosurface", Form = XmlSchemaForm.Unqualified), XmlArrayItem("penaltybonus",
            typeof (StructureSurfacePenalty), Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public StructureSurfacePenalty[] StructureSurfacePenaltiesArray
        {
            get { return this._structureSurfacePenaltiesArray; }
            set
            {
                this._structureSurfacePenaltiesArray = value;
                this.PopulateDictionary(value, this.StructureSurfacePenalties);
            }
        }

        [XmlArray("gridnexttostructure", Form = XmlSchemaForm.Unqualified), XmlArrayItem("penaltybonus",
            typeof (StructureStructurePenalty), Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public StructureStructurePenalty[] StructureStructurePenaltiesArray
        {
            get { return this._structureStructurePenaltiesArray; }
            set { this.PopulateDictionary(value, this.StructureStructureBonuses); }
        }

        [XmlIgnore]
        public Dictionary<StructureType, Dictionary<SurfaceType, ComboBonus>> StructureSurfaceBonus { get; private set;
        }

        [XmlIgnore]
        public Dictionary<StructureCategory, Dictionary<SurfaceType, ComboBonus>> StructureSurfacePenalties { get;
            private set; }

        [XmlIgnore]
        public Dictionary<StructureCategory, Dictionary<StructureType, ComboBonus>> StructureStructureBonuses { get;
            private set; }

        #region IDisposable Members

        public void Dispose()
        {
            this._structureStructurePenaltiesArray = null;
            this._structureSurfaceBonusArray = null;
            this._structureSurfacePenaltiesArray = null;
        }

        #endregion

        protected void PopulateDictionary<TIndexType, TValueType>(
            IEnumerable<ComboBonus> value, Dictionary<TIndexType, Dictionary<TValueType, ComboBonus>> bonusDictionary)
            where TIndexType : struct where TValueType : struct
        {
            bonusDictionary.Clear();
            foreach (var comboBonus in value)
            {
                var indexType = EnumUtil<TIndexType>.GetType(comboBonus.IndexType);
                var valueType = EnumUtil<TValueType>.GetType(comboBonus.ReferenceType);
                if (!bonusDictionary.ContainsKey(indexType))
                    bonusDictionary[indexType] = new Dictionary<TValueType, ComboBonus>();
                bonusDictionary[indexType].Add(valueType, comboBonus);
            }
        }
    }
}