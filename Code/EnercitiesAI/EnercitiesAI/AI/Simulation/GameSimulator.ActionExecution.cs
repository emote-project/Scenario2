using System;
using System.Collections.Generic;
using EnercitiesAI.AI.Actions;
using EnercitiesAI.Domain;
using EnercitiesAI.Domain.World;

namespace EnercitiesAI.AI.Simulation
{
    public partial class GameSimulator
    {
        public bool ExecuteAction(IPlayerAction action)
        {
            if (action is UpgradeStructures)
                return this.ExecuteUpgradeStructure((UpgradeStructures) action);
            if (action is BuildStructure)
                return this.ExecuteBuildStructure((BuildStructure) action);
            if (action is ImplementPolicy)
                return this.ExecuteImplementPolicy((ImplementPolicy) action);
            if (action is SkipTurn)
                return this.ExecuteSkipTurn();
            return false;
        }

        public bool ExecuteBuildStructure(BuildStructure action)
        {
            //checks args
            if ((this.State == null) || (action == null))
                throw new ArgumentException("Given state and action can't be null");

            //gets and verifies structure
            var structureType = action.StructureType;
            if (!this._domainInfo.Structures.ContainsType(structureType)) return false;

            //checks grid unit build suitability
            var coord = new Coordinate(action.X, action.Y);
            if (!this.IsSuitableBuild(coord, structureType))
                return false;

            //attributes the structure to the selected unit
            if (!this.State.StructuresState.ChangeStructure(coord, structureType))
                return false;

            //adds units affected by this action: the unit itself and affected neighbours
            if (!this._unitsToUpdate.Contains(coord))
                this._unitsToUpdate.Add(coord);
            this._unitsToUpdate.UnionWith(this.GetAffectedUnits(coord, structureType));

            //adds building costs
            this._currentCosts += this._domainInfo.Structures[structureType].BuildingCost;

            return true;
        }

        public bool ExecuteUpgradeStructure(UpgradeStructures action)
        {
            //checks args
            if ((this.State == null) || (action == null))
                throw new ArgumentException("Given state and action can't be null");

            //verifies upgrade number
            var upgrades = action.Upgrades;
            if ((upgrades.Count == 0) || (upgrades.Count > DomainInfo.MAX_UPGRADES_ACTION))
                return false;

            var successfullUpgrades = new List<Coordinate>();
            foreach (var upgradeAction in upgrades)
            {
                //gets and verifies upgrade
                var upgradeType = upgradeAction.UpgradeType;
                if (!this._domainInfo.Upgrades.ContainsType(upgradeType)) continue;

                //checks grid unit and upgrade suitability
                var coord = new Coordinate(upgradeAction.X, upgradeAction.Y);
                if (!this.IsSuitableUpgrade(coord, upgradeType))
                    continue;

                //adds the upgrade to the selected unit
                this.State.UpgradesState.AddUpgrade(coord, upgradeType);

                //adds upgrade costs
                this._currentCosts += this._domainInfo.Upgrades[upgradeType].ResearchCost;

                if (!successfullUpgrades.Contains(coord))
                    successfullUpgrades.Add(coord);
            }

            //adds units to update after action: the units that were upgraded
            this._unitsToUpdate.UnionWith(successfullUpgrades);

            return successfullUpgrades.Count != 0;
        }

        public bool ExecuteImplementPolicy(ImplementPolicy action)
        {
            //checks args
            if ((this.State == null) || (action == null))
                throw new ArgumentException("Given state and action can't be null");

            //verifies and activates given policy
            var policyType = action.PolicyType;
            if (this.State.PoliciesState.IsPolicyActive(policyType))
                return false;

            this.State.PoliciesState.AddActivePolicy(policyType);

            //adds units to update after action: the units affected by the policy
            this._unitsToUpdate.UnionWith(this.GetAffectedUnits(policyType));

            //adds policy implementation costs
            this._currentCosts += this._domainInfo.Policies[policyType].ResearchCost;

            return true;
        }

        public bool ExecuteSkipTurn()
        {
            //does nothing to state
            return true;
        }
    }
}