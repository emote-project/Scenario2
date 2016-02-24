using EmoteEvents;
using EnercitiesAI.AI.Actions;

namespace EnercitiesAI.AI.Game
{
    public class ActionStatePair
    {
        /// <summary>
        ///     The action that was executed.
        /// </summary>
        public IPlayerAction Action { get; set; }

        /// <summary>
        ///     The reported state after the executed action.
        /// </summary>
        public EnercitiesGameInfo State { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((State != null ? State.GetHashCode() : 0)*397) ^ (Action != null ? Action.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}->{1}", this.Action, this.State);
        }

        public override bool Equals(object obj)
        {
            return obj is ActionStatePair && Equals((ActionStatePair) obj);
        }

        public bool Equals(ActionStatePair sap)
        {
            return this.Action.Equals(sap.Action) && this.State.Equals(sap.State);
        }
    }
}