using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public class UpgradeStructure : GridUnitAction
    {
        public UpgradeStructure(UpgradeType upgrade)
        {
            this.UpgradeType = upgrade;
        }

        public UpgradeType UpgradeType { get; set; }

        public override string ToString()
        {
            return string.Format("Upgrade {0} in {1}", this.UpgradeType, base.ToString());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && (obj is UpgradeStructure) && this.Equals((UpgradeStructure) obj);
        }

        public bool Equals(UpgradeStructure other)
        {
            return base.Equals(other) && this.UpgradeType == other.UpgradeType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (int) this.UpgradeType;
            }
        }

        public override EnercitiesActionInfo ToEnercitiesActionInfo()
        {
            return new EnercitiesActionInfo(ActionType.UpgradeStructure)
                   {
                       SubType = (int) this.UpgradeType,
                       CellX = this.X,
                       CellY = this.Y
                   };
        }

        public override ActionType Type
        {
            get { return ActionType.UpgradeStructure; }
        }
    }
}