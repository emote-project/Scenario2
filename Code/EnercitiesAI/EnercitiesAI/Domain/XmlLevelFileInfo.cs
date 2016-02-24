namespace EnercitiesAI.Domain
{
    public class XmlLevelFileInfo
    {
        private const string GRID_FILE_NAME = "grid.xml";
        private const string VICTORY_POINTS_FILE_NAME = "victorypoints.xml";
        private const string UPGRADES_FILE_NAME = "upgrades.xml";
        private const string PENALTIES_AND_BONUSES_FILE_NAME = "penaltiesandbonusses.xml";
        private const string SURFACES_FILE_NAME = "surfaces.xml";
        private const string STRUCTURES_FILE_NAME = "structures.xml";
        private const string SCENARIO_FILE_NAME = "scenario.xml";
        private const string POLICIES_FILE_NAME = "policies.xml";
        private const string STRUCTURE_UPGRADES_FILE_NAME = "structureupgrades.xml";
        private const string MESSAGE_EVENTS_FILE_NAME = "triggermessages.xml";

        public XmlLevelFileInfo()
        {
            //attributes default values
            this.GridFileName = GRID_FILE_NAME;
            this.VictoryPointsFileName = VICTORY_POINTS_FILE_NAME;
            this.UpgradesFileName = UPGRADES_FILE_NAME;
            this.PenaltiesAndBonusesFileName = PENALTIES_AND_BONUSES_FILE_NAME;
            this.SurfacesFileName = SURFACES_FILE_NAME;
            this.StructuresFileName = STRUCTURES_FILE_NAME;
            this.ScenarioFileName = SCENARIO_FILE_NAME;
            this.PoliciesFileName = POLICIES_FILE_NAME;
            this.StructureUpgradesFileName = STRUCTURE_UPGRADES_FILE_NAME;
            this.MessageEventsFileName = MESSAGE_EVENTS_FILE_NAME;
        }

        public string GridFileName { get; set; }
        public string VictoryPointsFileName { get; set; }
        public string UpgradesFileName { get; set; }
        public string PenaltiesAndBonusesFileName { get; set; }
        public string SurfacesFileName { get; set; }
        public string StructuresFileName { get; set; }
        public string ScenarioFileName { get; set; }
        public string PoliciesFileName { get; set; }
        public string StructureUpgradesFileName { get; set; }
        public string MessageEventsFileName { get; set; }
    }
}