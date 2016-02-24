using System;
using ProtoBuf;

namespace EnercitiesAI.Domain.World
{
    [Serializable]
    [ProtoContract]
    public class Coordinate : IComparable<Coordinate>
    {
        [ProtoMember(1)] public readonly int x;

        [ProtoMember(2)] public readonly int y;

        public Coordinate() { }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        #region IComparable<Coordinate> Members

        public int CompareTo(Coordinate other)
        {
            return this.x == other.x ? this.y.CompareTo(other.y) : this.x.CompareTo(other.x);
        }

        #endregion

        public bool Equals(Coordinate other)
        {
            return this.ToString().Equals(other.ToString());
        }

        public override int GetHashCode()
        {
            return (this.x*397) ^ this.y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", this.x, this.y);
        }

        public override bool Equals(object other)
        {
            return this.ToString().Equals(other.ToString());
        }
    }
}