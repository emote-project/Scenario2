using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EnercitiesAI.Domain.World;
using ProtoBuf;

namespace EnercitiesAI.AI.States
{
    /// <summary>
    ///     Provides all the knowledge of a certain state of the game regarding which upgrades
    ///     have been performed, etc.
    ///     The class provides methods for accessing and updating the state of the game, e.g. add/remove an
    ///     upgrade to a unit;
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class UpgradesState : IState
    {
        [ProtoMember(1)] private readonly Dictionary<Coordinate, HashSet<UpgradeType>> _activeUpgrades =
            new Dictionary<Coordinate, HashSet<UpgradeType>>();

        #region IState Members

        public void Dispose()
        {
            foreach (var activeUpgrades in this._activeUpgrades.Values)
                activeUpgrades.Clear();
            this._activeUpgrades.Clear();
        }

        #endregion

        #region Upgrade methods

        public HashSet<UpgradeType> GetActiveUpgrades(Coordinate coord)
        {
            return this._activeUpgrades.ContainsKey(coord) ? this._activeUpgrades[coord] : new HashSet<UpgradeType>();
        }

        public bool AddUpgrade(Coordinate coord, UpgradeType upgrade)
        {
            if (!this._activeUpgrades.ContainsKey(coord))
                this._activeUpgrades[coord] = new HashSet<UpgradeType>();
            if (this._activeUpgrades[coord].Contains(upgrade)) return false;

            this._activeUpgrades[coord].Add(upgrade);
            return true;
        }

        public bool RemoveUpgrade(Coordinate coord, UpgradeType upgrade)
        {
            if (!this._activeUpgrades.ContainsKey(coord)) return false;

            this._activeUpgrades[coord].Remove(upgrade);
            return true;
        }

        public bool ClearAllUpgrades(Coordinate coord)
        {
            if (!this._activeUpgrades.ContainsKey(coord)) return false;

            this._activeUpgrades[coord].Clear();
            return true;
        }

        #endregion

        #region Equality methods

        public override bool Equals(object obj)
        {
            return (obj is UpgradesState) && this.Equals((UpgradesState) obj);
        }

        public virtual bool Equals(UpgradesState other)
        {
            //checks active policies and upgrades equal
            var set1 = this._activeUpgrades;
            var set2 = other._activeUpgrades;

            //checks key values equal
            if ((set1.Count != set2.Count) ||
                !new HashSet<Coordinate>(set1.Keys).SetEquals(new HashSet<Coordinate>(set2.Keys)))
                return false;

            //checks values equal
            return set1.Keys.All(key => set1[key].SetEquals(set2[key]));
        }

        public override int GetHashCode()
        {
            return this._activeUpgrades.GetHashCode();
        }

        #endregion
    }
}