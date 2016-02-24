using System;
using System.Collections.Generic;
using System.IO;
using EmoteEnercitiesMessages;
using EnercitiesAI.Domain.Game;
using EnercitiesAI.Domain.World;
using PS.Utilities.Serialization;

namespace EnercitiesAI.Domain
{
    /// <summary>
    ///     Represents all the "static" information about the game, i.e., world information, e.g., the world's grid
    ///     of cells, existing structures and surfaces, and also the dynamics / rules of the game, including
    ///     the start values, win/llose conditions, possible upgrades and policies and their effects, etc.
    /// </summary>
    public sealed class DomainInfo : IDisposable
    {
        public const int MAX_LEVELS = 4;
        public const int MAX_UPGRADES_ACTION = 3;
        public const int MAX_UPGRADES_STRUCTURE = 5;
        public const int MAX_WIDTH = 10;
        public const int MAX_HEIGHT = 7;
        public const int YEARS_PER_MOVE = 2;

        private readonly Dictionary<EnercitiesRole, HashSet<StructureCategory>> _allowedPlayerStructCats =
            new Dictionary<EnercitiesRole, HashSet<StructureCategory>>();

        private readonly Dictionary<EnercitiesRole, HashSet<StructureType>>[] _allowedPlayerStructTypes =
            new Dictionary<EnercitiesRole, HashSet<StructureType>>[MAX_LEVELS];

        public DomainInfo()
        {
            //sets default file xml level names
            this.XmlLevelFileInfo = new XmlLevelFileInfo();

            //sets allowed structures per player role
            this._allowedPlayerStructCats[EnercitiesRole.Environmentalist] =
                new HashSet<StructureCategory>
                {
                    StructureCategory.Residential,
                    StructureCategory.Energy,
                    StructureCategory.Environment
                };
            this._allowedPlayerStructCats[EnercitiesRole.Economist] =
                new HashSet<StructureCategory>
                {
                    StructureCategory.Residential,
                    StructureCategory.Energy,
                    StructureCategory.Economy
                };
            this._allowedPlayerStructCats[EnercitiesRole.Mayor] =
                new HashSet<StructureCategory>
                {
                    StructureCategory.Residential,
                    StructureCategory.Energy,
                    StructureCategory.Wellbeing
                };
        }

        public WorldGrid WorldGrid { get; set; }
        public Structures Structures { get; set; }
        public Upgrades Upgrades { get; set; }
        public StructureUpgrades StructureUpgrades { get; set; }
        public Surfaces Surfaces { get; set; }
        public Policies Policies { get; set; }
        public PenaltiesAndBonuses PenaltiesAndBonuses { get; set; }
        public Scenario Scenario { get; set; }
        public MessageEvents MessageEvents { get; set; }
        public VictoryPoints VictoryPoints { get; set; }

        public XmlLevelFileInfo XmlLevelFileInfo { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.WorldGrid.Dispose();
            this.PenaltiesAndBonuses.Dispose();
            this.Policies.Dispose();
            this.Scenario.Dispose();
            this.Structures.Dispose();
            this.StructureUpgrades.Dispose();
            this.Surfaces.Dispose();
            this.MessageEvents.Dispose();
            this.Upgrades.Dispose();
            this.VictoryPoints.Dispose();

            foreach (var set in this._allowedPlayerStructCats.Values)
                set.Clear();
            this._allowedPlayerStructCats.Clear();

            foreach (var allowedPlayerStructType in this._allowedPlayerStructTypes)
                foreach (var set in allowedPlayerStructType.Values)
                    set.Clear();
        }

        #endregion

        public void Load(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Level XML path does not exist!", path);

            //loads all available info on the game domain
            this.WorldGrid = XmlUtil<WorldGrid>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.GridFileName));
            this.VictoryPoints = XmlUtil<VictoryPoints>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.VictoryPointsFileName));
            this.Upgrades = XmlUtil<Upgrades>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.UpgradesFileName));
            this.PenaltiesAndBonuses = XmlUtil<PenaltiesAndBonuses>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.PenaltiesAndBonusesFileName));
            this.Surfaces = XmlUtil<Surfaces>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.SurfacesFileName));
            this.Structures = XmlUtil<Structures>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.StructuresFileName));
            this.Scenario = XmlUtil<Scenario>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.ScenarioFileName));
            this.Policies = XmlUtil<Policies>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.PoliciesFileName));
            this.StructureUpgrades = XmlUtil<StructureUpgrades>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.StructureUpgradesFileName));
            this.MessageEvents = XmlUtil<MessageEvents>.DeserializeXML(
                String.Format("{0}{1}", path, this.XmlLevelFileInfo.MessageEventsFileName));

            this.FillAllowedStructures();
        }

        private void FillAllowedStructures()
        {
            //initializes allowed structure type list
            for (var l = 0; l < MAX_LEVELS; l++)
            {
                var levelStructuresAllowed = new Dictionary<EnercitiesRole, HashSet<StructureType>>();
                this._allowedPlayerStructTypes[l] = levelStructuresAllowed;
                foreach (var playerRole in this._allowedPlayerStructCats.Keys)
                {
                    //adds all structures according to role's allowed categories
                    levelStructuresAllowed[playerRole] = new HashSet<StructureType>();
                    foreach (var cat in this._allowedPlayerStructCats[playerRole])
                        levelStructuresAllowed[playerRole].UnionWith(this.Structures[l, cat]);

                    //tries to add previous levels structures
                    if (l > 0)
                        levelStructuresAllowed[playerRole].UnionWith(this._allowedPlayerStructTypes[l - 1][playerRole]);
                }
            }
        }

        public bool IsAllowedStructure(EnercitiesRole role, StructureCategory category)
        {
            return this._allowedPlayerStructCats.ContainsKey(role) &&
                   this._allowedPlayerStructCats[role].Contains(category);
        }

        public bool IsAllowedStructure(EnercitiesRole role, StructureType structure, int level)
        {
            return (level < MAX_LEVELS) && this._allowedPlayerStructTypes[level].ContainsKey(role) &&
                   this._allowedPlayerStructTypes[level][role].Contains(structure);
        }

        public HashSet<StructureType> GetAllowedStructures(EnercitiesRole role, int level = MAX_LEVELS - 1)
        {
            if ((level >= MAX_LEVELS) || !this._allowedPlayerStructTypes[level].ContainsKey(role))
                return new HashSet<StructureType>();

            return this._allowedPlayerStructTypes[level][role];
        }
    }
}