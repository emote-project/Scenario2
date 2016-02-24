using System;
using EmoteEvents;
using EnercitiesAI.Domain;
using ProtoBuf;
using PS.Utilities.Serialization;

namespace EnercitiesAI.AI.States
{
    /// <summary>
    ///     Contains all the "dynamic" information in the game, i.e. the score and resource levels, the world
    ///     information, current upgrades and policies, etc.
    ///     This is the main information with which the  players deal and that will affect their decisions
    ///     over time.
    ///     This also contains all  the necessary information to perform a simulated time-step in the game.
    ///     The class can be cloned so to be used e.g. in AI search/reasoning procedures and simulations.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class State : ICloneable, IDisposable
    {
        public State()
        {
            this.YearsInLevel = new int[DomainInfo.MAX_LEVELS];
            this.StructuresState = new StructuresState();
            this.UpgradesState = new UpgradesState();
            this.PoliciesState = new PoliciesState();
            this.GameValuesState = new GameValuesState();
            this.Move = -1;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.StructuresState.Dispose();
            this.UpgradesState.Dispose();
            this.PoliciesState.Dispose();
            this.GameValuesState.Dispose();
            this.GameInfoState = null;
            this.YearsInLevel = null;
        }

        #endregion

        #region Properties

        [ProtoMember(1)]
        public int Year { get; set; }

        [ProtoMember(2)]
        public int[] YearsInLevel { get; private set; }

        [ProtoMember(3)]
        public int Move { get; set; }

        [ProtoMember(4)]
        public EnercitiesGameInfo GameInfoState { get; set; }

        [ProtoMember(5)]
        public StructuresState StructuresState { get; private set; }

        [ProtoMember(6)]
        public UpgradesState UpgradesState { get; private set; }

        [ProtoMember(7)]
        public PoliciesState PoliciesState { get; private set; }

        [ProtoMember(8)]
        public GameValuesState GameValuesState { get; private set; }

        #endregion

        #region Cloning methods

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public State Clone()
        {
            return this.CloneProto();
        }

        #endregion

        #region Equality methods

        public override bool Equals(object obj)
        {
            if (!(obj is State)) return false;
            return this.Equals((State) obj);
        }

        public virtual bool Equals(State state)
        {
            return this.GameInfoState.Equals(state.GameInfoState) &&
                   this.StructuresState.Equals(state.StructuresState) &&
                   this.UpgradesState.Equals(state.UpgradesState) &&
                   this.PoliciesState.Equals(state.PoliciesState);
        }

        public static bool Equals(State s1, State s2)
        {
            return s1.Equals(s2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.GameInfoState != null ? this.GameInfoState.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (this.StructuresState != null ? this.StructuresState.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (this.UpgradesState != null ? this.UpgradesState.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (this.PoliciesState != null ? this.PoliciesState.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}