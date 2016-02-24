using System;
using System.Collections.Generic;
using EmoteEnercitiesMessages;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;

namespace EnercitiesAI.AI.Simulation
{
    partial class GameSimulator
    {
        private int TargetPopulation
        {
            get { return this._domainInfo.Scenario.WinConditions[this.State.GameInfoState.Level].Population; }
        }

        public void UpdateState(bool init = false)
        {
            //checks changes
            if (this._unitsToUpdate.Count != 0)
            {
                //update all marked units to update combined values
                this.State.GameValuesState.UpdateOverallGameValues(this._unitsToUpdate, this.State, this._domainInfo);
                this._unitsToUpdate.Clear();
            }

            var gameValues = this.State.GameValuesState.OverallGameValues;

            //updates resources values
            var gameInfo = this.State.GameInfoState;
            gameInfo.Population = (int) gameValues.Homes;
            gameInfo.PowerConsumption = -gameValues.PowerConsumption;
            gameInfo.PowerProduction = gameValues.PowerProduction;

            if (!init)
            {
                gameInfo.MoneyEarning = gameValues.Money*DomainInfo.YEARS_PER_MOVE;
                gameInfo.Money -= gameInfo.MoneyEarning + this._currentCosts;
                gameInfo.Oil += gameValues.Oil*DomainInfo.YEARS_PER_MOVE;
            }
            this._currentCosts = 0;

            //checks level
            gameInfo.Level = Math.Min(gameInfo.Level, DomainInfo.MAX_LEVELS - 1);

            //updates scores
            gameInfo.EconomyScore = gameValues.EconomyScore;
            gameInfo.EnvironmentScore = gameValues.EnvironmentScore;
            gameInfo.WellbeingScore = gameValues.WellbeingScore;
            gameInfo.GlobalScore = gameInfo.EconomyScore + gameInfo.EnvironmentScore + gameInfo.WellbeingScore;

            //changes player role and target population
            gameInfo.CurrentRole = init ? this._playersOrder[2] : this.GetNextPlayer();
            gameInfo.TargetPopulation = this.TargetPopulation;

            if (!init)
            {
                //increases global and level year
                this.State.Year += DomainInfo.YEARS_PER_MOVE;
                this.State.YearsInLevel[gameInfo.Level] += DomainInfo.YEARS_PER_MOVE;
            }

            //checks new level
            if (this.IsTerminalLevelState())
                gameInfo.Level++;
        }

        private IEnumerable<Coordinate> GetAffectedUnits(Coordinate coord, StructureType structureType)
        {
            var coords = new List<Coordinate>();
            var penaltiesAndBonuses = this._domainInfo.PenaltiesAndBonuses;
            var structSurfPenalties = penaltiesAndBonuses.StructureSurfacePenalties;
            var structStructBonuses = penaltiesAndBonuses.StructureStructureBonuses;
            var neighbours = this._domainInfo.WorldGrid.GetNeighbourUnits(coord);
            var surface = this._domainInfo.WorldGrid[coord].SurfaceType;
            var category = this._domainInfo.Structures[structureType].Category;

            //checks the neighbour units that are affected by the given structure
            foreach (var neighbour in neighbours)
            {
                var neighbourType = this.State.StructuresState[neighbour];
                var neighbourCat = this._domainInfo.Structures[neighbourType].Category;

                //checks surface influence in neighbour's category and
                //       category influence in neighbour's type
                if ((structSurfPenalties.ContainsKey(neighbourCat) &&
                     structSurfPenalties[neighbourCat].ContainsKey(surface)) ||
                    (structStructBonuses.ContainsKey(category) &&
                     structStructBonuses[category].ContainsKey(neighbourType)))
                    coords.Add(neighbour);
            }

            return coords;
        }

        private IEnumerable<Coordinate> GetAffectedUnits(PolicyType policyType)
        {
            var coords = new List<Coordinate>();

            //checks policy
            if (!this._domainInfo.Policies.ContainsType(policyType))
                return coords;

            //checks affected structures
            var policy = this._domainInfo.Policies[policyType];
            if (policy.AffectedStructures == null)
                return coords;

            //adds all units that have a structure affected by the policy
            foreach (var structure in policy.AffectedStructures)
                coords.AddRange(this.State.StructuresState.GetStructureUnits(structure.StructureType));

            return coords;
        }

        private EnercitiesRole GetNextPlayer()
        {
            //checks next player (role) according to order list
            var curRoleIdx = this._playersOrder.IndexOf(this.State.GameInfoState.CurrentRole);
            var nextRoleIdx = curRoleIdx == this._playersOrder.Count - 1 ? 0 : curRoleIdx + 1;
            return this._playersOrder[nextRoleIdx];
        }
    }
}