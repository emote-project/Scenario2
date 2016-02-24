using System;
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;
using EmoteEnercitiesMessages;
using EmoteEvents;

namespace ThalamusEnercities
{
    public class ThalamusEnercitiesService : XmlRpcListenerService, IBehaviorModule
    {
        private ThalamusEnercities thalamusEnercities;
        public ThalamusEnercitiesService(ThalamusEnercities thalamusEnercities)
        {
            this.thalamusEnercities = thalamusEnercities;
        }

        public void EndGameSuccessfull(int totalScore)
        {
            thalamusEnercities.EndGameSuccessfull(totalScore);
        }

        public void EndGameNoOil(int totalScore)
        {
            thalamusEnercities.EndGameNoOil(totalScore);
        }

        public void EndGameTimeOut(int totalScore)
        {
            thalamusEnercities.EndGameTimeOut(totalScore);
        }

        public void NotifyTurnChanged(int level, int population, int targetPopulation,
            double money, double earning, double oil, double powerConsumption, double powerProduction,
            double environmentScore, double economyScore, double wellbeingScore, double globalScore, string currentRole)
        {
            Console.WriteLine("NotifyTurnChanged");
            EnercitiesGameInfo gs = new EnercitiesGameInfo();
            gs.Level=level;
            gs.Population=population;
            gs.TargetPopulation=targetPopulation;
            gs.Money=money;
            gs.Oil=oil;
            gs.MoneyEarning = earning;
            gs.PowerConsumption=powerConsumption;
            gs.PowerProduction=powerProduction;
            gs.EnvironmentScore=environmentScore;
            gs.EconomyScore=economyScore;
            gs.WellbeingScore=wellbeingScore;
            gs.GlobalScore=globalScore;
            gs.CurrentRole = (EnercitiesRole)Enum.Parse(typeof(EnercitiesRole), currentRole);
            thalamusEnercities.NotifyTurnChanged(gs);
        }



        public void NewLevel(int level)
        {
            thalamusEnercities.NewLevel(level);
        }

        public void Dispose()
        {
        } 


        public void ClickOnScreenEvent(double x, double y)
        {
            thalamusEnercities.ClickOnScreen(x, y);
        }

        void ZoomOnScreenEvent(double[] finger0, double[] finger1, double[] previousFinger0, double[] previousFinger1)
        {
            thalamusEnercities.ZoomOnScreen(finger0, finger1, previousFinger0, previousFinger1);
        }

        public void ZoomOnScreenEvent(double finger0x, double finger0y, double finger1x, double finger1y, double finger0StartX, double finger0StartY, double finger1StartX, double finger1StartY)
        {
            thalamusEnercities.ZoomOnScreen(new double[] { finger0x, finger0y },
                                                 new double[] { finger1x, finger1y },
                                                 new double[] { finger0StartX, finger0StartY },
                                                 new double[] { finger1StartX, finger1StartY });
        }


        public void PanOnScreenEvent(double x, double y, double firstX, double firstY)
        {
            thalamusEnercities.PanOnScreen(x, y, firstX, firstY);
        }


        public void StartGameShow()
        {
            Console.WriteLine("Starg Game Show not implemented");
        }

        public void StartGameHide()
        {
            Console.WriteLine("Starg Game Hide not implemented");
        }

        public void EndOfLevelShow()
        {
            Console.WriteLine("End of level show");
            thalamusEnercities.EndOfLevelShow();
        }

        public void EndOfLevelHide()
        {
            Console.WriteLine("End of level hide");
            thalamusEnercities.EndOfLevelHide();
        }

        public void EndOfGameShow()
        {
            Console.WriteLine("End Of Game Show");
            thalamusEnercities.EndOfGameShow();
        }

        public void EndOfGameHide()
        {
            Console.WriteLine("End Of Game Hide");
            thalamusEnercities.EndOfGameHide();
        }


        public void BuildMenuTooltipShowed(string StructureCategory_category, string structureCategory_translated)
        {
            Console.WriteLine("BuildMenuTooltipShowed");
            thalamusEnercities.ThalamusPublisher.BuildMenuTooltipShowed((StructureCategory)(Enum.Parse(typeof(StructureCategory), StructureCategory_category)), structureCategory_translated);
        }

        public void BuildMenuTooltipClosed(string StructureCategory_category, string structureCategory_translated)
        {
            Console.WriteLine("BuildMenuTooltipClosed");
            thalamusEnercities.ThalamusPublisher.BuildMenuTooltipClosed((StructureCategory)(Enum.Parse(typeof(StructureCategory), StructureCategory_category)), structureCategory_translated);
        }

        public void BuildingMenuToolSelected(string StructureType_structure, string structureType_translated)
        {
            Console.WriteLine("BuildingMenuToolSelected");
            thalamusEnercities.ThalamusPublisher.BuildingMenuToolSelected((StructureType)(Enum.Parse(typeof(StructureType), StructureType_structure)), structureType_translated);
        }

        public void BuildingMenuToolUnselected(string StructureType_structure, string structureType_translated)
        {
            Console.WriteLine("BuildingMenuToolUnselected");
            thalamusEnercities.ThalamusPublisher.BuildingMenuToolUnselected((StructureType)(Enum.Parse(typeof(StructureType), StructureType_structure)), structureType_translated);
        }

        public void PoliciesMenuShowed()
        {
            Console.WriteLine("PoliciesMenuShowed");
            thalamusEnercities.ThalamusPublisher.PoliciesMenuShowed();
        }

        public void PoliciesMenuClosed()
        {
            Console.WriteLine("PoliciesMenuClosed");
            thalamusEnercities.ThalamusPublisher.PoliciesMenuClosed();
        }

        public void PolicyTooltipShowed(string PolicyType_policy, string policyType_translated)
        {
            Console.WriteLine("PolicyTooltipShowed");
            thalamusEnercities.ThalamusPublisher.PolicyTooltipShowed((PolicyType)(Enum.Parse(typeof(PolicyType), PolicyType_policy)), policyType_translated);
        }

        public void PolicyTooltipClosed()
        {
            Console.WriteLine("PolicyTooltipClosed");
            thalamusEnercities.ThalamusPublisher.PolicyTooltipClosed();
        }

        public void UpgradesMenuShowed()
        {
            Console.WriteLine("UpgradesMenuShowed");
            thalamusEnercities.ThalamusPublisher.UpgradesMenuShowed();
        }

        public void UpgradesMenuClosed()
        {
            Console.WriteLine("UpgradesMenuClosed");
            thalamusEnercities.ThalamusPublisher.UpgradesMenuClosed();
        }

        public void UpgradeTooltipShowed(string UpgradeType_upgrade, string upgradeType_translated)
        {
            Console.WriteLine("UpgradeTooltipShowed");
            thalamusEnercities.ThalamusPublisher.UpgradeTooltipShowed((UpgradeType)(Enum.Parse(typeof(UpgradeType), UpgradeType_upgrade)), upgradeType_translated);
        }

        public void UpgradeTooltipClosed()
        {
            Console.WriteLine("UpgradeTooltipClosed");
            thalamusEnercities.ThalamusPublisher.UpgradeTooltipClosed();
        }

        public void ImplementPolicy(string PolicyType_policy, string policyType_translated)
        {
            Console.WriteLine("ImplementPolicy");
            thalamusEnercities.ImplementPolicy(PolicyType_policy, policyType_translated);
        }

        public void PerformUpgrade(string UpgradeType_upgrade, string upgradeType_translated, int x, int y)
        {
            Console.WriteLine("PerformUpgrade");
            thalamusEnercities.ThalamusPublisher.PerformUpgrade((UpgradeType)(Enum.Parse(typeof(UpgradeType), UpgradeType_upgrade)), upgradeType_translated, x, y);
        }

        public void SkipTurn()
        {
            Console.WriteLine("SkipTurn");
            thalamusEnercities.ThalamusPublisher.SkipTurn();
        }


        public void GameStarted(string player1name, string player1role, string player2name, string player2role)
        {
            Console.WriteLine("GameStarted");
            thalamusEnercities.GameStarted(player1name, player1role, player2name, player2role);
        }

        public void ConfirmConstruction(string StructureType_structure, string structureType_translated, int x, int y)
        {
            Console.WriteLine("ConfirmConstruction");
            thalamusEnercities.ThalamusPublisher.ConfirmConstruction((StructureType)(Enum.Parse(typeof(StructureType), StructureType_structure)), structureType_translated, x, y);
        }

        public void StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove)
        {
            Console.WriteLine("StrategyGameMoves");
            thalamusEnercities.ThalamusPublisher.StrategyGameMoves(environmentalistMove, economistMove, mayorMove, globalMove);
        }

        public void TargetInfo(string targetName, int x, int y)
        {
            thalamusEnercities.ThalamusPublisher.TargetScreenInfo(targetName, x, y);
        }



        public void ExamineCell(double screenX, double screenY, int cellX, int cellY, string StructureType_structure, string StructureType_translated)
        {
            Console.WriteLine("Examine Action " + cellX + "," + cellY + "," + StructureType_structure);
            thalamusEnercities.ThalamusPublisher.ExamineCell(screenX, screenY, cellX, cellY, (StructureType)(Enum.Parse(typeof(StructureType), StructureType_structure)), StructureType_translated);
        }
    }
}
