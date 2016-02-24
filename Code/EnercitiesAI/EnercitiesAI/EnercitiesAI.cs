using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmoteEnercitiesMessages;
namespace EnercitiesAI
{
    public class EnercitiesAI
    {
        //External Knowledge
        private int currentLevel;
        private int currentPopulation;
        private int targetPopulation;
        private float currentMoney;
        private float currentOil;
        private float currentPowerConsumption;
        private float currentPowerProduction;

        private float globalScore;
        private float environmentScore;
        private float economyScore;
        private float wellbeingScore;

        System.Random randomGenerator;
        private delegate int ActionGenerator();
        private KeyValuePair<int, ActionGenerator>[] actionGeneratorFunctionsWithProbabilities;
        private List<EnercitiesRole> goals;
        private Dictionary<SurfaceType, StructureType[]> rules;
        private List<Structure> possibleHomes;
        private List<Structure> possibleEnergyStructures;
        private List<Action> possibleActions;
        private EnercitiesRole mainRole;
        private StructureCategory mainRoleStruct;

        public EnercitiesAI()
        {
            actionGeneratorFunctionsWithProbabilities = new KeyValuePair<int, ActionGenerator>[] {
			    new KeyValuePair<int, ActionGenerator>(10, new ActionGenerator(GeneratePolicyActions)),
			    new KeyValuePair<int, ActionGenerator>(80, new ActionGenerator(GenerateUpdateActions)),
			    new KeyValuePair<int, ActionGenerator>(10, new ActionGenerator(GenerateBuildActions))
		    };

            goals = new List<EnercitiesRole>();
            goals.Add(mainRole);

            rules = CommunicationModule.GetBuildRules();

            randomGenerator = new System.Random();

            // Initialize possible homes to build
            possibleHomes = new List<Structure>();

            foreach (Structure structure in Structures.GetStructures())
            {
                if (StructureType.City_Hall != structure.Type && structure.Category == StructureCategory.Residential)
                {
                    possibleHomes.Add(structure);
                }
            }
        }

        internal void Play(ThalamusEnercitiesInterfaces.EnercitiesGameState enercitiesGameState)
        {
            this.currentLevel = enercitiesGameState.Level;
            this.currentPopulation = enercitiesGameState.Population;
            this.targetPopulation = enercitiesGameState.TargetPopulation;
            this.currentMoney = enercitiesGameState.Money;
            this.currentOil = enercitiesGameState.Oil;
            this.currentPowerConsumption = enercitiesGameState.PowerConsumption;
            this.currentPowerProduction = enercitiesGameState.PowerProduction;
            this.globalScore = enercitiesGameState.GlobalScore;
            this.environmentScore = enercitiesGameState.EnvironmentScore;
            this.economyScore = enercitiesGameState.EconomyScore;
            this.wellbeingScore = enercitiesGameState.WellbeingScore;

            int number = randomGenerator.Next(1, 101);
            int prev = 0;
            int i = 0;

            //probabilities
            while (!(number > prev && number <= prev + actionGeneratorFunctionsWithProbabilities[i].Key))
            {
                prev += actionGeneratorFunctionsWithProbabilities[i].Key;
                i++;
            }
            int init = i;
            int possibleActionsCount;
            while ((possibleActionsCount = (actionGeneratorFunctionsWithProbabilities[i].Value)()) == 0 &&
                    (i = (i + 1) % actionGeneratorFunctionsWithProbabilities.Length) != init) ;

            //In case of need, we build homes to avoid losing the game
            HomeCheck();

            if (possibleActionsCount > 0)
            {

                ActionSelector.SortActions(possibleActions);

                int energyNeeded;
                int totalCost;
                int firstAffordable;

                for (firstAffordable = 0; firstAffordable < possibleActions.Count; firstAffordable++)
                {
                    energyNeeded = 0;
                    totalCost = 0;
                    possibleActions[firstAffordable].CalculateCost(firstAffordable, 1, possibleActions, ref energyNeeded, ref totalCost);

                    if (!IsEnoughEnergy((int)-energyNeeded))
                    {
                        GenerateBuildEnergyActions((int)-energyNeeded);
                        firstAffordable = -1;
                        continue;
                    }
                    else if (currentMoney >= totalCost)
                    {
                        ScheduleExecution(firstAffordable, 1);
                        break;
                    }
                }
                if (firstAffordable == possibleActions.Count)
                    CommunicationModule.SkipPlay();
            }
        }
        private bool IsAllowedToBuild(Structure structure)
        {
            return structure.UnlockLevel <= currentLevel
                    && structure.Type != StructureType.City_Hall
                    && (structure.Category == StructureCategory.Residential
                        || structure.Category == mainRole.GoalStruct);
        }

        private int GenerateBuildActions()
        {
            possibleActions = new List<Action>();
            foreach (Structure structure in Structures.GetStructures())
            {
                if (IsAllowedToBuild(structure) && structure.Category != StructureCategory.Residential)
                    possibleActions.Add(new BuildAction((int)structure.BuildingCost, (int)structure.AnnualEnergy, structure, goals));
            }
            int count = 0;
            foreach (EnercitiesGridUnit pos in CommunicationModule.GetEmptySlots())
            {
                if (pos.Level <= currentLevel && pos.Surface.Type != SurfaceType.Ocean)
                    count++;
            }

            return count;
        }

        private int GenerateUpdateActions()
        {
            possibleActions = new List<Action>();
            foreach (EnercitiesGridUnit gunit in BuildManager.StructuresConstructions)
            {
                if (gunit.Structure != null &&
                    gunit.Structure.Category != mainRole.GoalStruct &&
                    gunit.Structure.Category != StructureCategory.Residential &&
                    gunit.Structure.Category != StructureCategory.Energy)
                    continue;
                foreach (UpgradeType upgradeType in CommunicationModule.GetAvailableUpgrades(gunit.Structure))
                {
                    possibleActions.Add(new UpgradeAction(CommunicationModule.GetUpgrade(upgradeType), gunit, goals));
                }
            }
            return possibleActions.Count;
        }

        private int GeneratePolicyActions()
        {
            possibleActions = new List<Action>();
            List<PolicyType> availablePolicies = PolicyManager.GetAvailablePolicies();
            List<PolicyType> smartChoices = PoliciesSmartChoice();

            foreach (PolicyType pt in smartChoices)
            {
                if (availablePolicies.Contains(pt))
                    possibleActions.Add(new PolicyAction(CalculatePolicyEnergy(pt), Policies.GetPolicy(pt), goals));
            }
            return possibleActions.Count;
        }

        //this function is needed for the policy Electric Car Grid 
        private int CalculatePolicyEnergy(PolicyType type)
        {
            int total = 0;
            Policy policy = Policies.GetPolicy(type);

            foreach (KeyValuePair<StructureType, GridValues> entry in policy.AffectedStructures)
            {
                total += ((int)entry.Value.AnnualEnergy) * CommunicationModule.NumberOfConstructions(entry.Key);
            }
            return (total < 0) ? -total : 0;
        }

        public bool IsEnoughEnergy(int energyRequired)
        {
            return energyRequired <= currentPowerProduction - currentPowerConsumption;
        }

        public void GenerateBuildWindmillAction()
        {
            List<EnercitiesGridUnit> emptySlots = CommunicationModule.GetEmptySlots();
            EnercitiesGridUnit gunitPlayed = null;
            Structure windmillStruct = Structures.GetStructure(StructureType.Windmills);
            foreach (EnercitiesGridUnit gunit in emptySlots)
            {
                if (gunit.Level <= currentLevel &&
                    gunit.Surface.Type == SurfaceType.Ocean)
                {
                    if (currentMoney >= windmillStruct.BuildingCost)
                    {
                        gunitPlayed = gunit;
                        BuildAction a = new BuildAction((int)windmillStruct.BuildingCost, (int)windmillStruct.AnnualEnergy, windmillStruct, goals);
                        a.PositionToBuild = gunit;
                        possibleActions.Add(a);
                    }
                    break;
                }
            }
            if (gunitPlayed == null &&
                currentLevel == 1 &&
                (gunitPlayed = FindBestPosition(windmillStruct)) != null &&
                currentMoney >= windmillStruct.BuildingCost)
            {
                /* No ocean to build windmills in level 1 */
                BuildAction a = new BuildAction((int)windmillStruct.BuildingCost, (int)windmillStruct.AnnualEnergy, windmillStruct, goals);
                a.PositionToBuild = gunitPlayed;
                possibleActions.Add(a);
            }
        }

        //Empirically speaking, these are the best energy structures available
        /*
         * Windmills
         * Solar Plant
         * Nuclear Plant
         * Hydro Plant
         * Super WindTurbine
         * Super Solar
         * Nuclear Fusion
         */
        public bool IsGreenEnergy(Structure energyStructure)
        {
            return energyStructure.Type != StructureType.Coal_Plant
                    && energyStructure.Type != StructureType.Coal_Plant_Small;
        }

        private bool IsAllowedToBuildEnergy(Structure energyStruct)
        {
            return (energyStruct.UnlockLevel <= currentLevel
                    && energyStruct.Category == StructureCategory.Energy
                    && energyStruct.Type != StructureType.Windmills
                    && IsGreenEnergy(energyStruct));
        }

        public void GenerateBuildEnergyActions(int energyRequired)
        {
            Structure chosenEnergyStruct = null;
            possibleEnergyStructures = new List<Structure>();
            EnercitiesGridUnit bestPosition = null;
            possibleActions = new List<Action>();

            foreach (Structure structure in Structures.GetStructures())
            {
                if (IsAllowedToBuildEnergy(structure))
                    possibleEnergyStructures.Add(structure);
            }
            possibleEnergyStructures.Sort(new EnergyFacilityComparator());

            for (int i = 0; i < possibleEnergyStructures.Count; i++)
            {
                if (possibleEnergyStructures[i].BuildingCost <= currentMoney && (bestPosition = FindBestPosition(possibleEnergyStructures[i])) != null)
                {
                    chosenEnergyStruct = possibleEnergyStructures[i];
                    break;
                }
            }

            if (chosenEnergyStruct == null)
            {
                GenerateBuildWindmillAction();
            }

            else
            {
                BuildAction action = new BuildAction((int)chosenEnergyStruct.BuildingCost, (int)chosenEnergyStruct.AnnualEnergy, chosenEnergyStruct, goals);
                action.PositionToBuild = bestPosition;
                possibleActions.Add(action);
            }
        }

        private void HomeCheck()
        {
            Structure bestHome = null;
            int emptySlots = 0;
            int highestLevel = 0;

            int remainingPeople = targetPopulation - currentPopulation;

            foreach (EnercitiesGridUnit pos in CommunicationModule.GetEmptySlots())
                if (pos.Level <= currentLevel &&
                    (pos.Surface.Type == SurfaceType.Plains2 ||
                    pos.Surface.Type == SurfaceType.Plains ||
                    pos.Surface.Type == SurfaceType.Hills))
                    emptySlots++;

            if (emptySlots == 0)
                return;

            foreach (Structure structure in possibleHomes)
            {
                if (structure.UnlockLevel <= currentLevel
                    && structure.UnlockLevel > highestLevel)
                {
                    highestLevel = structure.UnlockLevel;
                    bestHome = structure;
                }
            }

            //Debug.Log ("empty slots: " + emptySlots);

            int peoplePerSlot = 0;
            bool peoplePerSlotInit = false;

            if (emptySlots - Constants.HOME_THRESHOLD > 0)
            {
                peoplePerSlotInit = true;
                peoplePerSlot = (remainingPeople - 1) / (emptySlots - Constants.HOME_THRESHOLD) + 1;
                //Debug.Log("remaining people: " + remainingPeople);
                //Debug.Log("peoplePerSlot: " + peoplePerSlot);
                if (currentPowerProduction - currentPowerConsumption < ((peoplePerSlot - 1) / bestHome.Homes + 1) * (-(int)bestHome.AnnualEnergy))
                {
                    if (emptySlots - Constants.HOME_THRESHOLD - Constants.THRESHOLD_ENERGY_DISCOUNT > 0)
                        peoplePerSlot = (remainingPeople - 1) / (emptySlots - Constants.HOME_THRESHOLD - Constants.THRESHOLD_ENERGY_DISCOUNT) + 1;
                    else
                        peoplePerSlotInit = false;
                }
            }

            if (!peoplePerSlotInit || peoplePerSlot - bestHome.Homes >= 0)
            {
                possibleActions = new List<Action>();
                possibleActions.Add(new BuildAction((int)bestHome.BuildingCost, (int)bestHome.AnnualEnergy, bestHome, goals));
            }
        }
    }
}