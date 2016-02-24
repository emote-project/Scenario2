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
    [XmlRoot("policies", Namespace = "", IsNullable = false)]
    public class Policies : NameTypedElementList<Policy, PolicyType>
    {
        private readonly Dictionary<StructureType, HashSet<PolicyType>> _structurePolicies =
            new Dictionary<StructureType, HashSet<PolicyType>>();

        [XmlElement("policy", Form = XmlSchemaForm.Unqualified)]
        public new Policy[] Items
        {
            get { return base.Items; }
            set
            {
                base.Items = value;
                this.Init();
            }
        }

        [XmlIgnore]
        public HashSet<PolicyType> this[StructureType structureType]
        {
            get
            {
                return !this._structurePolicies.ContainsKey(structureType)
                    ? new HashSet<PolicyType>()
                    : this._structurePolicies[structureType];
            }
        }

        public override void Dispose()
        {
            foreach (var policy in this.Items)
                policy.Dispose();
            base.Dispose();
            this._structurePolicies.Clear();
        }

        private void Init()
        {
            foreach (var policy in this.Items)
            {
                var policyType = EnumUtil<PolicyType>.GetType(policy.Name);
                foreach (var affectedStructure in policy.AffectedStructures)
                {
                    var structureType = affectedStructure.StructureType;
                    if (!this._structurePolicies.ContainsKey(structureType))
                        this._structurePolicies[structureType] = new HashSet<PolicyType>();
                    if (!this._structurePolicies[structureType].Contains(policyType))
                        this._structurePolicies[structureType].Add(policyType);
                }
            }
        }
    }
}