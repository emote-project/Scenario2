using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using ProtoBuf;
using PS.Utilities;

namespace EnercitiesAI.AI.States
{
    /// <summary>
    ///     Provides all the knowledge of a certain state of the game regarding which policies
    ///     are implemented and which are available for implementation
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class PoliciesState : IState
    {
        [ProtoMember(1)]
        private readonly HashSet<PolicyType> _activePolicies = new HashSet<PolicyType>();

        #region IState Members

        public void Dispose()
        {
            this._activePolicies.Clear();
        }

        #endregion

        #region Equality methods

        public override bool Equals(object obj)
        {
            return (obj is PoliciesState) && this.Equals((PoliciesState) obj);
        }

        public virtual bool Equals(PoliciesState other)
        {
            //just checks active policies
            return this._activePolicies.SetEquals(other._activePolicies);
        }

        public override int GetHashCode()
        {
            return this._activePolicies.GetHashCode();
        }

        #endregion

        #region Policy methods

        public HashSet<PolicyType> GetInactivePolicies()
        {
            var policyTypes = new HashSet<PolicyType>(EnumUtil<PolicyType>.GetTypes());
            policyTypes.ExceptWith(this._activePolicies);
            return policyTypes;
        }

        public HashSet<PolicyType> GetActivePolicies()
        {
            return new HashSet<PolicyType>(this._activePolicies);
        }

        public bool AddActivePolicy(PolicyType policy)
        {
            if (this._activePolicies.Contains(policy)) return false;

            this._activePolicies.Add(policy);
            return true;
        }

        public bool RemoveActivePolicy(PolicyType policy)
        {
            if (!this._activePolicies.Contains(policy)) return false;

            this._activePolicies.Remove(policy);
            return true;
        }

        public bool IsPolicyActive(PolicyType policyType)
        {
            return this._activePolicies.Contains(policyType);
        }

        #endregion
    }
}