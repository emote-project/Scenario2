using System.Collections.Generic;
using System.Linq;
using EmoteEnercitiesMessages;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;
using PS.Utilities;
using PS.Utilities.Collections;

namespace EnercitiesAI.AI.Simulation
{
    public partial class GameSimulator
    {
        public HashSet<Coordinate> GetBuildableUnits(
            StructureCategory structureCat, int upToLevel = DomainInfo.MAX_LEVELS - 1)
        {
            return this.GetBuildableUnits(
                this._domainInfo.Structures[upToLevel, structureCat], upToLevel);
        }

        public HashSet<Coordinate> GetBuildableUnits(
            HashSet<StructureType> structure, int upToLevel = DomainInfo.MAX_LEVELS - 1)
        {
            //check which empty cell units have surfaces on which to build structures of the given type 
            var emptyBuildable = new HashSet<Coordinate>();
            foreach (var coord in this.State.StructuresState.GetEmptyUnits(upToLevel))
            {
                var surfaceType = this._domainInfo.WorldGrid[coord].SurfaceType;
                var buildRules = this._domainInfo.Surfaces.BuildRules[surfaceType];
                if (buildRules.Overlaps(structure))
                    emptyBuildable.Add(coord);
            }
            return emptyBuildable;
        }

        public List<IPlayerAction> GetSuitableActions(bool ignoreSkip = false)
        {
            //adds a skip turn action
            var actions = new List<IPlayerAction>();
            if (!ignoreSkip) actions.Add(new SkipTurn());

            //adds all suitable actions for each type
            this.AddSuitableBuilds(actions);
            this.AddSuitableUpgrades(actions);
            this.AddSuitablePolicyImpl(actions);

            return actions;
        }

        private void AddSuitableBuilds(List<IPlayerAction> actions)
        {
            var gameInfo = this.State.GameInfoState;
            actions.AddRange(this.GetSuitableBuilds(gameInfo.Money, gameInfo.Level, gameInfo.CurrentRole));
        }

        private List<BuildStructure> GetSuitableBuilds(double availableMoney, int level, EnercitiesRole playerRole)
        {
            //adds structure build actions
            var actions = new List<BuildStructure>();
            foreach (var coord in this.State.StructuresState.GetEmptyUnits(level))
            {
                //checks structures that match the surface type and that are allowed for player
                var surface = this._domainInfo.WorldGrid[coord].SurfaceType;
                var structures = new HashSet<StructureType>(this._domainInfo.Surfaces.BuildRules[surface]);
                structures.IntersectWith(this._domainInfo.GetAllowedStructures(playerRole, level));

                foreach (var structure in structures)
                    if (this.IsSuitableBuild(coord, structure, availableMoney, level, playerRole))
                        actions.Add(new BuildStructure(structure) {X = coord.x, Y = coord.y});
            }
            return actions;
        }

        private void AddSuitableUpgrades(List<IPlayerAction> actions)
        {
            var level = this.State.GameInfoState.Level;
            var structState = this.State.StructuresState;

            //gets individual structure upgrade actions
            var possibleUpgrades = new List<UpgradeStructure>();
            foreach (var coord in structState.GetOccupiedUnits(level))
            {
                //gets possible upgrades given structure type and takes the ones already added to the unit
                var notAddedUpgrades = new HashSet<UpgradeType>(this._domainInfo.StructureUpgrades[structState[coord]]);
                notAddedUpgrades.ExceptWith(this.State.UpgradesState.GetActiveUpgrades(coord));

                foreach (var upgrade in notAddedUpgrades)
                    if (this.IsSuitableUpgrade(coord, upgrade))
                        possibleUpgrades.Add(new UpgradeStructure(upgrade) {X = coord.x, Y = coord.y});
            }

            if (possibleUpgrades.Count == 0) return;

            //if only 3 upgrades or less just combine them
            if (possibleUpgrades.Count <= 3)
            {
                actions.Add(new UpgradeStructures(possibleUpgrades));
                return;
            }

            //adds upgrade combinations, ie 1 upgrade, 2 upgrades & 3 upgrades actions
            for (uint numUp = DomainInfo.MAX_UPGRADES_ACTION; numUp > 2; numUp--)
            {
                var allUpgradesComb = possibleUpgrades.ToArray().AllCombinations(numUp, false);

                actions.AddRange(
                    from upgradeStructures in allUpgradesComb
                    where this.IsSuitableUpgrades(upgradeStructures)
                    select new UpgradeStructures(upgradeStructures.ToList()));
            }
        }

        private void AddSuitablePolicyImpl(List<IPlayerAction> actions)
        {
            //adds possible policy implementations
            actions.AddRange(
                from policyType in EnumUtil<PolicyType>.GetTypes()
                where this.IsSuitablePolicyImplementation(policyType)
                select new ImplementPolicy(policyType));
        }
    }
}