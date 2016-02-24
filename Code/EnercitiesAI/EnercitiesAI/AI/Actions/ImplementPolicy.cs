using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public class ImplementPolicy : IPlayerAction
    {
        public ImplementPolicy(PolicyType policyType)
        {
            this.PolicyType = policyType;
        }

        public PolicyType PolicyType { get; set; }

        #region IPlayerAction Members

        public virtual EnercitiesActionInfo ToEnercitiesActionInfo()
        {
            return new EnercitiesActionInfo(ActionType.ImplementPolicy)
                   {
                       SubType = (int) this.PolicyType,
                       CellX = 4,
                       CellY = 2,
                   };
        }

        public ActionType Type
        {
            get { return ActionType.ImplementPolicy; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Implement {0}", this.PolicyType);
        }

        public override bool Equals(object obj)
        {
            return (obj is ImplementPolicy) && this.Equals((ImplementPolicy) obj);
        }

        public bool Equals(ImplementPolicy other)
        {
            return this.PolicyType == other.PolicyType;
        }

        public override int GetHashCode()
        {
            return (int) this.PolicyType;
        }
    }
}