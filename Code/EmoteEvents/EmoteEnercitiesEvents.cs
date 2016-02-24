using System;
using Thalamus;

namespace EmoteEnercitiesMessages
{
    public enum EnercitiesRole
    {
        Economist,
        Environmentalist,
        Mayor
    }

    public enum EnercitiesStrategy
    {
        Economist,
        Environmentalist,
        Mayor,
        Global
    }

    public enum SurfaceType
    {
        NotUsed = 0,
        Plains = 1,
        Hills = 2,
        Ocean = 3,
        River = 4,
        Hydro_River = 5,
        Plains2 = 6
    }

    public enum PolicyType
    {
        NotUsed = 0,
        CO2_Taxes,
        Electric_Car_Grid,
        Energy_Education_Program,
        Eco_Tourism_Program,
        Sustainable_Technology_Fund
        //Free_Public_Transport,
    }

    public enum UpgradeType
    {
        NotUsed = 0,
        Solar_Roofs = 1,
        Improved_Insulation = 2,
        Rainwater_Storage = 3,
        Bus_Stop = 4,
        Rooftop_Windmills = 5,
        Thermal_Storage = 6,
        Birdhouse = 7,
        Eco_Roofs = 8,
        Subway_Station = 9,
        Energy_Efficient_Lightbulbs = 10,
        Recycling_Facilities = 11,
        CO2_Reduction_Plan = 12,
        Cradle_2_Cradle = 13,
        Bio_Food = 14,
        Veggie_Food = 15,
        Watch_Tower = 16,
        Forest_Health_Plan = 17,
        Wildlife_Preservation = 18,
        Exhaust_Scrubbers = 19,
        Coal_Washing = 20,
        CO2_Storage = 21,
        Bigger_Rotor_Blades = 22,
        Next_Gen_Solar_Cells = 23,
        Moving_Solar_Pads = 24,
        Improved_Uranium_Storage = 25,
        Uranium_Recycling = 26
    }

    public enum StructureType
    {
        NotUsed,
        Park,
        Forest,
        Wildlife_Reserve,
        Suburban,
        Urban,
        Stadium,
        Light_Industry,
        Heavy_Industry,
        Commercial,
        Coal_Plant,
        Nuclear_Plant,
        Windmills,
        Solar_Plant,
        Hydro_Plant,
        City_Hall,
        Coal_Plant_Small,
        Residential_Tower,
        Super_Solar,
        Super_WindTurbine,
        Nuclear_Fusion,
        Market,
        Public_Services
    }

    public enum StructureCategory
    {
        NotUsed,
        Environment,
        Residential,
        Economy,
        Energy,
        Wellbeing
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum ActionType
    {
        SkipTurn,
        BuildStructure,
        UpgradeStructure,
        UpgradeStructures,
        ImplementPolicy
    }

    public interface IEnercitiesGameStateActions : IAction
    {
        /// <summary>
        /// Communicates to Enercities to instantlt end the game
        /// </summary>
        void EndGameTimeout();
    }

    public interface IEnercitiesClassificationEvent : IPerception
    {
        void ClassifierResult(string label);
    }

    public interface IEnercitiesGameStateEvents : IPerception
    {
        void PlayersGender(Gender player1Gender, Gender player2Gender);
        void PlayersGenderString(string player1Gender, string player2Gender);
        void GameStarted(string player1Name, string player1Role, string player2Name, string player2Role);

        [DontLogDetails]
        void ResumeGame(string player1Name, string player1Role, string player2Name, string player2Role,
            string serializedGameState);

        void EndGameSuccessfull(int totalScore);
        void EndGameNoOil(int totalScore);
        void EndGameTimeOut(int totalScore);

        [DontLogDetails]
        void TurnChanged(string serializedGameState);

        void ReachedNewLevel(int level);
        void StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove);
    }

    // Opening menus
    public interface IEnercitiesExamineEvents : IPerception
    {
        // Build Menus  (those tree buttons each player can use)
        void BuildMenuTooltipShowed(StructureCategory category, string translation);
        void BuildMenuTooltipClosed(StructureCategory category, string translation);
        // Build button clicked
        void BuildingMenuToolSelected(StructureType structure, string translation);
        void BuildingMenuToolUnselected(StructureType structure, string translation);
        // Policies Menu
        void PoliciesMenuShowed();
        void PoliciesMenuClosed();
        // Policies
        void PolicyTooltipShowed(PolicyType policy, string translation);
        void PolicyTooltipClosed(); // This one is valid for any of the tooltips previously showed.
        // Upgrades Menu
        void UpgradesMenuShowed();
        void UpgradesMenuClosed();
        // Upgrades
        void UpgradeTooltipShowed(UpgradeType upgrade, string translation);
        void UpgradeTooltipClosed();
    }

    public interface IEnercitiesExamineActions : IAction
    {
        // Build Menus  (those tree buttons each player can use)
        void ShowBuildMenuTooltip(StructureCategory category);
        void CloseBuildMenuTooltip();
        // Build button clicked
        void SelectBuildingMenuTool(StructureType structure);
        void UnselectBuildingMenuTool();
        // Policies Menu
        void ShowPoliciesMenu();
        void ClosePoliciesMenu();
        // Policies
        void ShowPolicyTooltip(PolicyType policy);
        void ClosePolicyTooltip();
        void PreviewBuildCell(StructureType structure, int cellX, int cellY);
        // Upgrades Menu
        void ShowUpgradesMenu(int cellX, int cellY);
        void CloseUpgradesMenu();
        // Upgrades
        void ShowUpgradeTooltip(int cellX, int cellY, UpgradeType upgrade);
        void CloseUpgradeTooltip();
    }

    public interface IEnercitiesTaskEvents : IPerception
    {
        void ConfirmConstruction(StructureType structure, string translation, int cellX, int cellY);
        void ImplementPolicy(PolicyType policy, string translation);
        void PerformUpgrade(UpgradeType upgrade, string translation, int cellX, int cellY);
        void SkipTurn();

        void ExamineCell(double screenX, double screenY, int cellX, int cellY, StructureType StructureType_structure,
            string StructureType_translated);
    }

    public interface IEnercitiesAIActions : IAction
    {
        /// <summary>
        ///     Raised when player strategies prediction/modeling is updated by the AI module after some turn change.
        /// </summary>
        /// <param name="StrategiesSet_strategies">a table containing the strategies for each player/role</param>
        //[DontLogDetails]
        //void StrategiesUpdated(Dictionary<EnercitiesRole, double[]> strategies);
        void StrategiesUpdated(string StrategiesSet_strategies);

        /// <summary>
        ///     The action that defines the best strategy for the player for which the decision was last calculated.
        /// </summary>
        /// <param name="EnercitiesActionInfo_actionInfos">the best action serialized as a string.</param>
        [DontLogDetails]
        void BestActionPlanned(string[] EnercitiesActionInfo_actionInfos);

        /// <summary>
        ///     Raised after an AI module planning step. The actions define the best strategies for the player for which
        ///     the decision was last calculated. Useful to simulate examination of several cells / actions.
        /// </summary>
        /// <param name="EnercitiesActionInfo_actionInfos">
        ///     actions are ordered in ascending order according to their predicted value for
        ///     the player.
        /// </param>
        [DontLogDetails]
        [Obsolete("Deprecated: use ActionsPlanned instead as more information is provided.")]
        void BestActionsPlanned(EnercitiesRole currentPlayer, string[] EnercitiesActionInfo_actionInfos);

        /// <summary>
        ///     Raised after an AI module planning step. The actions define the best strategies for the player for which
        ///     the decision was last calculated. Useful to simulate examination of several cells / actions.
        /// </summary>
        /// <param name="currentPlayer">the role of the player to whom the actions were planned.</param>
        /// <param name="Strategy_planStrategy">the strategy used by the planner to calculate the best/worst actions.</param>
        /// <param name="EnercitiesActionInfo_bestActionInfos">
        ///     the best actions (in ascending order according to their predicted value) calculated during planning.
        /// </param>
        /// <param name="EnercitiesActionInfo_worstActionInfos">
        ///     the worst actions (in descending order according to their predicted value) calculated during planning.
        /// </param>
        [DontLogDetails]
        void ActionsPlanned(EnercitiesRole currentPlayer, string Strategy_planStrategy,
            string[] EnercitiesActionInfo_bestActionInfos, string[] EnercitiesActionInfo_worstActionInfos);

        /// <summary>
        ///     Raised when several game values are predicted by the AI module after some planning step.
        ///     This event can be used to set up alarms when some game values fall bellow/above certain values
        ///     (e.g., Oil&lt;100, EconomyScore&lt;0).
        /// </summary>
        /// <param name="values">
        ///     The 9 values predicted by the AI module during planning:
        ///     <para>0: EconomyScore</para>
        ///     <para>1: EnvironmentScore</para>
        ///     <para>2: WellbeingScore</para>
        ///     <para>3: Money</para>
        ///     <para>4: PowerConsumption</para>
        ///     <para>5: PowerProduction</para>
        ///     <para>6: Oil</para>
        ///     <para>7: Homes</para>
        ///     <para>8: ScoresUniformity</para>
        ///     <para>
        ///         9: NoActionProbability
        ///         (the probability of reaching a state where players have no other option other than SkipTurn,
        ///         i.e. a dead-end, in the near future)
        ///     </para>
        ///     <para>
        ///         10: NoSpaceProbability
        ///         (the probability of reaching a state where  *no player* has a cell available on which to build a
        ///         new structure, according to the standard number of available cells in each level.)
        ///     </para>
        /// </param>
        [DontLogDetails]
        void PredictedValuesUpdated(double[] values);
    }

    public interface IEnercitiesAIPerceptions : IPerception
    {
        //[DontLogDetails]
        //void UpdateStrategies(Dictionary<EnercitiesRole, double[]> strategies);
        /// <summary>
        ///     An updated set of strategies for the AI
        /// </summary>
        /// <param name="strategies"></param>
        void UpdateStrategies(string StrategiesSet_strategies);
    }

    public interface IEnercitiesTaskActions : IAction
    {
        void PlayStrategy(EnercitiesStrategy strategy);
        void ConfirmConstruction(StructureType structure, int cellX, int cellY);
        void ImplementPolicy(PolicyType policy);
        void PerformUpgrade(UpgradeType upgrade, int cellX, int cellY);
        void SkipTurn();
    }

    public interface IEnercitiesThermometerActions : IAction
    {
        void ThermometerNewLevel();
        void ThermometerAddRound(int quality);
    }
}