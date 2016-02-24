using System;
using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;

namespace EnercitiesAI.AI.Simulation
{
    public partial class GameSimulator
    {
        public bool IsCitySustainable()
        {
            //info taken from game code
            var gameInfo = this.State.GameInfoState;
            return gameInfo.EconomyScore >= 0 &&
                   gameInfo.WellbeingScore >= 0 &&
                   gameInfo.EnvironmentScore >= 0;
        }

        public bool IsTerminalLevelState()
        {
            //gets win conditions for the given level
            var level = Math.Min(this.State.GameInfoState.Level, DomainInfo.MAX_LEVELS - 1);
            var winCondition = this._domainInfo.Scenario.WinConditions[level];

            //verifies all conditions
            return (this.State.GameInfoState.Population >= winCondition.Population) &&
                   (!winCondition.SustainableCity || this.IsCitySustainable());
        }

        public bool IsTerminalState()
        {
            //game terminates when final level is completed or oil reaches 0
            return this.IsTerminalLevelState() || (this.State.GameInfoState.Oil <= 0);
        }

        public bool IsSuitableBuild(Coordinate coord, StructureType structureType)
        {
            var gameInfo = this.State.GameInfoState;
            return IsSuitableBuild(coord, structureType, gameInfo.Money, gameInfo.Level, gameInfo.CurrentRole);
        }

        public bool IsSuitableBuild(
            Coordinate coord, StructureType structureType, double availableMoney, int level, EnercitiesRole playerRole)
        {
            var structure = this._domainInfo.Structures[structureType];
            var worldGrid = this._domainInfo.WorldGrid;
            var surfaceType = this._domainInfo.WorldGrid[coord].SurfaceType;

            //checks structure
            if (!this._domainInfo.Structures.ContainsType(structureType) ||
                structureType.Equals(StructureType.City_Hall) ||
                structureType.Equals(StructureType.NotUsed))
                return false;

            //checks structure <-> surface compability
            if (!this.IsBuildMatch(surfaceType, structureType))
                return false;

            //checks nuclear and coal plant next to river
            if (structureType.Equals(StructureType.Coal_Plant) ||
                structureType.Equals(StructureType.Nuclear_Plant) ||
                structureType.Equals(StructureType.Coal_Plant_Small) ||
                structureType.Equals(StructureType.Nuclear_Fusion))
            {
                var neighbours = worldGrid.GetNeighbourUnits(coord);
                if (!neighbours.Any(
                    neighbour => worldGrid[neighbour].SurfaceType.Equals(SurfaceType.River)))
                    return false;
            }

            //checks allowed structure, unit is empty, level and build cost
            return this._domainInfo.IsAllowedStructure(playerRole, structure.Category) &&
                   this._domainInfo.IsAllowedStructure(playerRole, structureType, level) &&
                   this.State.StructuresState.IsUnitEmpty(coord) &&
                   (structure.BuildingCost <= availableMoney - this._currentCosts) &&
                   (worldGrid[coord].Level <= level);
        }

        protected bool IsBuildMatch(SurfaceType surface, StructureType structure)
        {
            var rules = this._domainInfo.Surfaces.BuildRules;
            return rules.ContainsKey(surface) && rules[surface].Contains(structure);
        }

        protected bool IsBuildable(SurfaceType surface)
        {
            var rules = this._domainInfo.Surfaces.BuildRules;
            return rules.ContainsKey(surface) && (rules[surface].Count > 0);
        }

        public bool IsSuitableUpgrade(Coordinate coord, UpgradeType upgradeType)
        {
            var worldState = this.State.StructuresState;
            var gameInfo = this.State.GameInfoState;
            var level = gameInfo.Level;

            //checks if upgrade type is valid, unit is occupied, upgrade was not already applied and
            // whether max upgrades to unit have been reached
            var activeUpgrades = this.State.UpgradesState.GetActiveUpgrades(coord);
            if (!this._domainInfo.Upgrades.ContainsType(upgradeType) ||
                !worldState.IsUnitValid(coord) || worldState.IsUnitEmpty(coord) ||
                activeUpgrades.Contains(upgradeType) ||
                (activeUpgrades.Count >= DomainInfo.MAX_UPGRADES_STRUCTURE))
                return false;

            //checks existing structure type
            var structureType = worldState[coord];
            var structures = this._domainInfo.Structures;
            if (!structures.ContainsType(structureType))
                return false;

            //checks allowed upgrade, cost level and whether upgrade is applicable to the existing structure
            var structureUpgrades = this._domainInfo.StructureUpgrades;
            var structure = structures[structureType];
            return this._domainInfo.IsAllowedStructure(gameInfo.CurrentRole, structure.Category) &&
                   this._domainInfo.IsAllowedStructure(gameInfo.CurrentRole, structureType, level) &&
                   (this._domainInfo.Upgrades[upgradeType].ResearchCost <=
                    gameInfo.Money - this._currentCosts) &&
                   (this._domainInfo.WorldGrid[coord].Level <= level) &&
                   structureUpgrades.ContainsType(upgradeType) &&
                   structureUpgrades[structureType].Contains(upgradeType);
        }

        protected bool IsSuitableUpgrades(IEnumerable<UpgradeStructure> upgrades)
        {
            //just check if there is enough money to perform all upgrades
            return upgrades.Sum(upgrade => this._domainInfo.Upgrades[upgrade.UpgradeType].ResearchCost) <=
                   (this.State.GameInfoState.Money - this._currentCosts);
        }

        public bool IsSuitablePolicyImplementation(PolicyType policyType)
        {
            //just check policy is not already active and there is available money to apply policy
            return (!this.State.PoliciesState.IsPolicyActive(policyType) &&
                    (this._domainInfo.Policies[policyType].ResearchCost <=
                     this.State.GameInfoState.Money - this._currentCosts));
        }
    }
}