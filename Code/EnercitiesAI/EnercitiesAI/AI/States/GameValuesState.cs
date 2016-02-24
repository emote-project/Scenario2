using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;
using ProtoBuf;

namespace EnercitiesAI.AI.States
{
    /// <summary>
    ///     Provides all the knowledge about the game values of the world, i.e., the combined values
    ///     of the several units and the global combined game values. Contains methods to update these
    ///     values by summing the values of each unit and checking whether the unit is active or not,
    ///     according to the current power production level.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class GameValuesState : IState
    {
        [ProtoMember(1)] private readonly SortedSet<Coordinate> _powerDownUnits = new SortedSet<Coordinate>();

        [ProtoMember(2)] private readonly Dictionary<Coordinate, GameValuesElement> _unitGameValues =
            new Dictionary<Coordinate, GameValuesElement>();

        public GameValuesState()
        {
            this.OverallGameValues = new GameValuesElement();
        }

        [ProtoMember(3)]
        public GameValuesElement OverallGameValues { get; private set; }

        #region IState Members

        public void Dispose()
        {
            this._unitGameValues.Clear();
            this._powerDownUnits.Clear();
        }

        #endregion

        #region Game values update methods

        public void UpdateOverallGameValues(SortedSet<Coordinate> unitsToUpdate, State state, DomainInfo domainInfo)
        {
            //checks units
            if ((unitsToUpdate == null) || (unitsToUpdate.Count == 0)) return;

            //updates each unit's values, powers them down if necessary
            this.UpdateUnitsGameValues(unitsToUpdate, state, domainInfo);

            //tries to power up units
            this.PowerUpUnits(state, domainInfo);
        }

        protected void UpdateUnitsGameValues(SortedSet<Coordinate> unitsToUpdate, State state, DomainInfo domainInfo)
        {
            //gets old energy values
            var globalPower = this.OverallGameValues.Power;

            //updates each marked unit's values (from the game engine: sorts units by their coordinates)
            foreach (var coord in unitsToUpdate)
            {
                //checks if unit already contributed for global energy
                if (this._unitGameValues.ContainsKey(coord))
                    globalPower -= this._unitGameValues[coord].Power;

                //updates unit's combined values
                var unitCombinedValues = this.GetUnitGameValues(coord, state, domainInfo);

                //gets and checks new effect of unit for global energy
                var newEnergy = globalPower + unitCombinedValues.Power;
                if ((newEnergy < 0) && !this.IsPoweredDown(coord) && this.IsPowerConsumer(coord, state, domainInfo))
                {
                    //if unit turns global energy negative, it has to be shutdown and its values recalculated
                    this.PowerDown(coord);
                    this.UpdateUnitGameValues(coord, this.GetUnitGameValues(coord, state, domainInfo));
                }
                else
                {
                    //otherwise just update global energy and unit values
                    globalPower = newEnergy;
                    this.UpdateUnitGameValues(coord, unitCombinedValues);
                }
            }
        }

        protected void UpdateUnitGameValues(Coordinate coord, GameValuesElement newValues)
        {
            //tries to get old unit values
            var oldValues = this._unitGameValues.ContainsKey(coord)
                ? this._unitGameValues[coord]
                : new GameValuesElement();

            //attributes new unit values
            this._unitGameValues[coord] = newValues;

            //updates overall (combined) score
            this.OverallGameValues.Subtract(oldValues);
            this.OverallGameValues.Add(newValues);
        }

        #endregion

        #region Units power methods

        protected void PowerUpUnits(State state, DomainInfo domainInfo)
        {
            //gets available energy
            var availablePower = this.OverallGameValues.Power;

            //from the game engine: sorts units by their coordinates
            var downUnits = new List<Coordinate>(this._powerDownUnits);
            for (var i = 0; i < downUnits.Count; i++)
            {
                //checks available energy, no need to continue if no more power
                if (availablePower <= 0) return;

                //check unit down
                var downUnit = downUnits[i];
                if (!this.IsPoweredDown(downUnit)) continue;

                //powers up unit, gets its power consumption 
                this.PowerUp(downUnit);
                var updatedValues = this.GetUnitGameValues(downUnit, state, domainInfo);
                var unitPower = -updatedValues.Power;

                //checks if unit can be powered up
                if (unitPower > availablePower)
                {
                    //if not, power down again
                    this.PowerDown(downUnit);
                    continue;
                }

                // if it can, updates available energy and unit values
                this.UpdateUnitGameValues(downUnit, updatedValues);
                availablePower -= unitPower;

                //from game engine: "Make sure we always power up the most applicatable(?) structures"
                i = -1;
            }
        }

        protected bool IsPowerConsumer(Coordinate coord, State state, DomainInfo domainInfo)
        {
            var structureType = state.StructuresState[coord];
            return domainInfo.Structures[structureType].PowerType.Equals(PowerType.Consumer);
        }

        protected void PowerDown(Coordinate coord)
        {
            if (!this.IsPoweredDown(coord))
                this._powerDownUnits.Add(coord);
        }

        protected void PowerUp(Coordinate coord)
        {
            if (this.IsPoweredDown(coord))
                this._powerDownUnits.Remove(coord);
        }

        public bool IsPoweredDown(Coordinate coord)
        {
            return this._powerDownUnits.Contains(coord);
        }

        #endregion

        #region Units game values update methods

        protected GameValuesElement GetUnitGameValues(Coordinate coord, State state, DomainInfo domainInfo)
        {
            //verifies unit empty, return neutral value
            if (state.StructuresState.IsUnitEmpty(coord))
                return new GameValuesElement();

            var structureType = state.StructuresState[coord];
            var structure = domainInfo.Structures[structureType];

            //adds base structure
            var gameValues = new GameValuesElement(structure);

            //combines with upgrades
            gameValues.Add(this.GetCombinedUpgrades(coord, state, domainInfo));

            //combines with policies
            gameValues.Add(this.GetCombinedPolicies(structureType, state, domainInfo));

            //from the game engine code: "Oil should never be > 0 !!!"
            if (gameValues.Oil > 0)
                gameValues.Oil = 0;

            //from game engine code: checks unit is down and changes some unit values accordingly
            if (this.IsPoweredDown(coord))
            {
                gameValues.Homes = 0;
                gameValues.Money = 0;
                gameValues.Oil = 0;
                gameValues.EconomyScore = 0;
                if (gameValues.Power < 0)
                    gameValues.WellbeingScore = 0;

                gameValues.PowerConsumption = gameValues.PowerProduction = 0;
            }

            //combines with combo bonuses (independent of power down..)
            //ignore City Hall on this one, does not count because it is an initial structure
            if (!structureType.Equals(StructureType.City_Hall))
                gameValues.Add(this.GetCombinedBonuses(coord, structure, state, domainInfo));

            return gameValues;
        }

        protected GameValuesElement GetCombinedUpgrades(Coordinate coord, State state, DomainInfo domainInfo)
        {
            var gameValues = new GameValuesElement();

            //adds upgrades to structure
            var upgrades = state.UpgradesState.GetActiveUpgrades(coord);
            if (upgrades == null) return gameValues;
            foreach (var upgradeType in upgrades)
            {
                var upgrade = domainInfo.Upgrades[upgradeType];
                gameValues.Add(upgrade);
            }
            return gameValues;
        }

        protected GameValuesElement GetCombinedPolicies(StructureType structureType, State state, DomainInfo domainInfo)
        {
            var gameValues = new GameValuesElement();

            //adds policies
            var policies = new HashSet<PolicyType>(domainInfo.Policies[structureType]);
            policies.IntersectWith(state.PoliciesState.GetActivePolicies());
            foreach (var policyType in policies)
            {
                var policy = domainInfo.Policies[policyType];
                gameValues.Add(policy[structureType]);
            }
            return gameValues;
        }

        protected GameValuesElement GetCombinedBonuses(
            Coordinate coord, Structure structure, State state, DomainInfo domainInfo)
        {
            var structureType = structure.Type;
            var gameValues = new GameValuesElement();

            //adds combo bonuses
            var surfaceType = domainInfo.WorldGrid[coord].SurfaceType;
            var structureCategory = structure.Category;
            var penaltiesAndBonuses = domainInfo.PenaltiesAndBonuses;
            var structSurfBonus = penaltiesAndBonuses.StructureSurfaceBonus;
            var structSurfPenalties = penaltiesAndBonuses.StructureSurfacePenalties;
            var structStructBonuses = penaltiesAndBonuses.StructureStructureBonuses;

            //add surface bonuses 
            if (structSurfBonus.ContainsKey(structureType))
            {
                var surfaceBonuses = structSurfBonus[structureType];
                if (surfaceBonuses.ContainsKey(surfaceType))
                    gameValues.Add(surfaceBonuses[surfaceType].Bonus);
            }

            //add neighbour structures and surfaces influence 
            var neighbours = domainInfo.WorldGrid.GetNeighbourUnits(coord);
            foreach (var neighbour in neighbours)
            {
                var neighbourStructType = state.StructuresState[neighbour];
                var neighbourSurfType = domainInfo.WorldGrid[neighbour].SurfaceType;
                var neighbourCategory = domainInfo.Structures[neighbourStructType].Category;

                //checks neighbour surface next to structure's category
                if (structSurfPenalties.ContainsKey(structureCategory))
                {
                    var surfaceBonuses = structSurfPenalties[structureCategory];
                    if (surfaceBonuses.ContainsKey(neighbourSurfType))
                        gameValues.Add(surfaceBonuses[neighbourSurfType].Bonus);
                }

                //checks neighbour category next to structure's type
                if (structStructBonuses.ContainsKey(neighbourCategory))
                {
                    var structureBonuses = structStructBonuses[neighbourCategory];
                    if (structureBonuses.ContainsKey(structureType))
                        gameValues.Add(structureBonuses[structureType].Bonus);
                }
            }

            return gameValues;
        }

        #endregion
    }
}