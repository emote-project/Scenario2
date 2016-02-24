using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;
using ProtoBuf;

namespace EnercitiesAI.AI.States
{
    /// <summary>
    ///     Provides all the knowledge of a certain state of the world, i.e. which cells in the grid
    ///     are occupied and which structures do they hold.
    ///     The class provides methods for accessing and updating of the state of the world, e.g.
    ///     add/remove a structure, return the grid units holding a certain type of structure/surface etc.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class StructuresState : IState
    {
        [ProtoMember(1)] private readonly HashSet<Coordinate> _dummyUnits = new HashSet<Coordinate>();

        [ProtoMember(2)] private readonly Dictionary<StructureType, HashSet<Coordinate>> _structureUnits =
            new Dictionary<StructureType, HashSet<Coordinate>>();

        [ProtoMember(4)] private readonly Dictionary<Coordinate, int> _unitsLevels = new Dictionary<Coordinate, int>();

        [ProtoMember(3)] private readonly Dictionary<Coordinate, StructureType> _unitStructures =
            new Dictionary<Coordinate, StructureType>();

        public StructureType this[Coordinate coord]
        {
            get
            {
                return this._unitStructures.ContainsKey(coord) ? this._unitStructures[coord] : StructureType.NotUsed;
            }
        }

        #region IState Members

        public void Dispose()
        {
            this._unitsLevels.Clear();
            this._unitStructures.Clear();
            foreach (var set in this._structureUnits.Values)
                if (set != null)
                    set.Clear();
            this._structureUnits.Clear();
            this._dummyUnits.Clear();
        }

        #endregion

        public void Init(WorldGrid grid)
        {
            //creates coordinates and structure indexes
            foreach (var gridUnit in grid.Units)
            {
                if (gridUnit == null) continue;

                var structureType = gridUnit.StructureType;
                var coordinate = new Coordinate(gridUnit.X, gridUnit.Y);

                this._unitsLevels.Add(coordinate, gridUnit.Level);

                //adds structure units indexes
                this.AddStructureUnit(coordinate, structureType);

                //tests for dummy structures (that do not contribute to scores)
                if (!structureType.Equals(StructureType.NotUsed) &&
                    !structureType.Equals(StructureType.City_Hall))
                    this._dummyUnits.Add(coordinate);
            }
        }

        public void ClearUnits()
        {
            foreach (var coord in this._unitStructures.Keys)
                this.ClearUnit(coord);
        }

        public void ClearUnit(Coordinate unit)
        {
            this.ChangeStructure(unit, StructureType.NotUsed);
        }

        public bool ChangeStructure(Coordinate coord, StructureType structure)
        {
            if (!this._unitStructures.ContainsKey(coord))
                return false;

            //check old structure type
            var oldStructure = this._unitStructures[coord];
            if (oldStructure.Equals(structure)) return false;

            //just replace the structure of a unit
            this.RemoveStructureUnit(coord);
            this.AddStructureUnit(coord, structure);

            //tests for dummy unit (no longer dummy..)
            if (this._dummyUnits.Contains(coord))
                this._dummyUnits.Remove(coord);

            return true;
        }

        public HashSet<Coordinate> GetStructureUnits(
            StructureType structure, int upToLevel = DomainInfo.MAX_LEVELS - 1, bool excluding = false)
        {
            var coords = new List<Coordinate>();
            foreach (var key in this._structureUnits.Keys.Where(key => excluding ^ key.Equals(structure)))
                if (this._structureUnits[key] != null)
                    coords.AddRange(this._structureUnits[key].Where(coord => this._unitsLevels[coord] <= upToLevel));
            return new HashSet<Coordinate>(coords);
        }

        public HashSet<Coordinate> GetEmptyUnits(int upToLevel = DomainInfo.MAX_LEVELS - 1)
        {
            var emptyUnits = this.GetDummyUnits(upToLevel);
            emptyUnits.UnionWith(this.GetStructureUnits(StructureType.NotUsed, upToLevel));
            return emptyUnits;
        }

        private HashSet<Coordinate> GetDummyUnits(int upToLevel = DomainInfo.MAX_LEVELS - 1)
        {
            return new HashSet<Coordinate>(this._dummyUnits.Where(coord => this._unitsLevels[coord] <= upToLevel));
        }

        public HashSet<Coordinate> GetOccupiedUnits(int upToLevel = DomainInfo.MAX_LEVELS - 1)
        {
            //gets all structure types except for the empty/not used one
            return this.GetStructureUnits(StructureType.NotUsed, upToLevel, true);
        }

        public bool IsUnitEmpty(Coordinate coord)
        {
            return this.IsUnitDummy(coord) || this.IsUnitOccupied(coord, StructureType.NotUsed);
        }

        public bool IsUnitOccupied(Coordinate coord, StructureType structureType)
        {
            return this.IsUnitValid(coord) && this[coord].Equals(structureType);
        }

        public bool IsUnitValid(Coordinate coord)
        {
            return this._unitStructures.ContainsKey(coord);
        }

        private void AddStructureUnit(Coordinate coordinate, StructureType structureType)
        {
            this._unitStructures[coordinate] = structureType;
            if (!this._structureUnits.ContainsKey(structureType) || (this._structureUnits[structureType] == null))
                this._structureUnits[structureType] = new HashSet<Coordinate>();
            if (!this._structureUnits[structureType].Contains(coordinate))
                this._structureUnits[structureType].Add(coordinate);
        }

        private void RemoveStructureUnit(Coordinate coordinate)
        {
            if (!this._unitStructures.ContainsKey(coordinate)) return;
            var structureType = this._unitStructures[coordinate];
            this._unitStructures[coordinate] = StructureType.NotUsed;
            if (this._structureUnits.ContainsKey(structureType) &&
                this._structureUnits[structureType].Contains(coordinate))
                this._structureUnits[structureType].Remove(coordinate);
        }

        public bool IsUnitDummy(Coordinate coord)
        {
            return this._dummyUnits.Contains(coord);
        }

        #region Equality methods

        public override bool Equals(object obj)
        {
            return (obj is StructuresState) && this.Equals((StructuresState) obj);
        }

        public bool Equals(StructuresState other)
        {
            return (this._unitStructures.Count == other._unitStructures.Count) &&
                   new HashSet<Coordinate>(this._unitStructures.Keys).SetEquals(
                       new HashSet<Coordinate>(other._unitStructures.Keys)) &&
                   this._unitStructures.Keys.All(key => this._unitStructures[key].Equals(other._unitStructures[key]));
        }

        public override int GetHashCode()
        {
            return (this._unitStructures != null ? this._unitStructures.GetHashCode() : 0);
        }

        #endregion
    }
}