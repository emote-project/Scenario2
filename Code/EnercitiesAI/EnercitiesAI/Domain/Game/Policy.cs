using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using EmoteEnercitiesMessages;

namespace EnercitiesAI.Domain.Game
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Policy : TypeElement, IDisposable
    {
        private Dictionary<StructureType, PolicyStructure> _policyStructures;

        [XmlElement("researchcost", Form = XmlSchemaForm.Unqualified)]
        public string ResearchcostStr
        {
            get { return this.ResearchCost.ToString(XmlParseUtil.CultureInfo); }
            set { this.ResearchCost = XmlParseUtil.ParseDouble(value); }
        }

        [XmlElement("researchtime", Form = XmlSchemaForm.Unqualified)]
        public string ResearchtimeStr
        {
            get { return this.ResearchTime.ToString(XmlParseUtil.CultureInfo); }
            set { this.ResearchTime = XmlParseUtil.ParseDouble(value); }
        }

        [XmlArray("affectedstructures", Form = XmlSchemaForm.Unqualified),
         XmlArrayItem("structure", typeof (PolicyStructure), Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public PolicyStructure[] AffectedStructures
        {
            get { return this._policyStructures.Values.ToArray(); }
            set { this._policyStructures = value.ToDictionary(policyStructure => (policyStructure.StructureType)); }
        }

        [XmlIgnore]
        public double ResearchCost { get; private set; }

        [XmlIgnore]
        public double ResearchTime { get; private set; }

        [XmlIgnore]
        public PolicyStructure this[StructureType structureType]
        {
            get
            {
                return this._policyStructures.ContainsKey(structureType)
                    ? this._policyStructures[structureType]
                    : null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._policyStructures.Clear();
        }

        #endregion
    }
}