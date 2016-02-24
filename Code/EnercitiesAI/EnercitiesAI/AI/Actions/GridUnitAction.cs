using EmoteEnercitiesMessages;
using EmoteEvents;

namespace EnercitiesAI.AI.Actions
{
    public abstract class GridUnitAction : IPlayerAction
    {
        public int X { get; set; }
        public int Y { get; set; }

        #region IPlayerAction Members

        public abstract EnercitiesActionInfo ToEnercitiesActionInfo();

        public abstract ActionType Type { get; }

        #endregion

        public override string ToString()
        {
            return string.Format("({0},{1})", this.X, this.Y);
        }

        public override bool Equals(object obj)
        {
            return (obj is GridUnitAction) && this.Equals((GridUnitAction) obj);
        }

        protected bool Equals(GridUnitAction other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }
    }
}