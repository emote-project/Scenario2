using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public class BuildStructure : GridUnitAction
    {
        public BuildStructure(StructureType structure)
        {
            this.StructureType = structure;
        }

        public StructureType StructureType { get; set; }

        public override ActionType Type
        {
            get { return ActionType.BuildStructure; }
        }

        public override string ToString()
        {
            return string.Format("Build {0} in {1}", this.StructureType, base.ToString());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && (obj is BuildStructure) && this.Equals((BuildStructure) obj);
        }

        public bool Equals(BuildStructure other)
        {
            return base.Equals(other) && this.StructureType == other.StructureType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (int) this.StructureType;
            }
        }

        public override EnercitiesActionInfo ToEnercitiesActionInfo()
        {
            return new EnercitiesActionInfo(ActionType.BuildStructure)
                   {
                       SubType = (int) this.StructureType,
                       CellX = this.X,
                       CellY = this.Y
                   };
        }
    }
}