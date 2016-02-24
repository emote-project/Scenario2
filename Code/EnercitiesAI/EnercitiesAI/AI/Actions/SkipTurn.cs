using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public sealed class SkipTurn : IPlayerAction
    {
        #region IPlayerAction Members

        public EnercitiesActionInfo ToEnercitiesActionInfo()
        {
            return new EnercitiesActionInfo(ActionType.SkipTurn);
        }

        public ActionType Type
        {
            get { return ActionType.SkipTurn; }
        }

        #endregion

        public override string ToString()
        {
            return "Skip turn";
        }

        public override bool Equals(object obj)
        {
            return (obj is SkipTurn);
        }

        public bool Equals(SkipTurn other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}