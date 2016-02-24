using System.Collections.Generic;
using System.Text;
using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public class UpgradeStructures : IPlayerAction
    {
        public UpgradeStructures(List<UpgradeStructure> upgrades)
        {
            this.Upgrades = upgrades;
        }

        public List<UpgradeStructure> Upgrades { get; private set; }

        #region IPlayerAction Members

        public EnercitiesActionInfo ToEnercitiesActionInfo()
        {
            //upgrade structures are transformed seperately 
            return null;
        }

        public void ReadCSVLine(string line)
        {
            //upgrade structures are transformed seperately 
        }

        public string WriteCSVLine()
        {
            //upgrade structures are transformed seperately
            return null;
        }

        public ActionType Type
        {
            get { return ActionType.UpgradeStructures; }
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder("Upgrades:");
            foreach (var upgrade in this.Upgrades)
                sb.AppendLine(upgrade.ToString());
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return (obj is UpgradeStructures) && this.Equals((UpgradeStructures) obj);
        }

        public bool Equals(UpgradeStructures other)
        {
            return new HashSet<UpgradeStructure>(this.Upgrades).SetEquals(other.Upgrades);
        }

        public override int GetHashCode()
        {
            return (this.Upgrades != null ? this.Upgrades.GetHashCode() : 0);
        }
    }
}