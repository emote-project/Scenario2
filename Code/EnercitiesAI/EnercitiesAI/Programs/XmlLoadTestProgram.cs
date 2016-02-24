using System;
using EnercitiesAI.AI.Game;
using EnercitiesAI.Domain.Game;
using EnercitiesAI.Domain.World;
using PS.Utilities.Serialization;

namespace EnercitiesAI.Programs
{
    internal class XmlLoadTestProgram
    {
        internal const string XML_BASE_PATH = @"..\..\..\..\..\EnercitiesEMOTE\Assets\Resources\Level\";

        //const string XML_BASE_PATH = @"xml-test/";

        private static void Main(string[] args)
        {
            var game = new Game(XML_BASE_PATH);
            game.Init();

            TestAllFileTypes();

            Console.WriteLine("\nAll tests finished!");
            Console.ReadKey();
        }

        private static void TestAllFileTypes()
        {
            XmlUtil<WorldGrid>.TestType(string.Format("{0}grid.xml", XML_BASE_PATH));
            XmlUtil<VictoryPoints>.TestType(string.Format("{0}victorypoints.xml", XML_BASE_PATH));
            XmlUtil<Upgrades>.TestType(string.Format("{0}upgrades.xml", XML_BASE_PATH));
            XmlUtil<PenaltiesAndBonuses>.TestType(string.Format("{0}penaltiesandbonusses.xml", XML_BASE_PATH));
            XmlUtil<Surfaces>.TestType(string.Format("{0}surfaces.xml", XML_BASE_PATH));
            XmlUtil<Structures>.TestType(string.Format("{0}structures.xml", XML_BASE_PATH));
            XmlUtil<Scenario>.TestType(string.Format("{0}scenario.xml", XML_BASE_PATH));
            XmlUtil<Policies>.TestType(string.Format("{0}policies.xml", XML_BASE_PATH));
            XmlUtil<StructureUpgrades>.TestType(string.Format("{0}structureupgrades.xml", XML_BASE_PATH));
            XmlUtil<MessageEvents>.TestType(string.Format("{0}triggermessages.xml", XML_BASE_PATH));
        }
    }
}