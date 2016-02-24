using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using EmoteEnercitiesMessages;
using EmoteEvents;
using InOutEmote.Thalamus;
using LogicWebLib;
using Timer = System.Timers.Timer;

namespace InOutEmote.behaviours.gameActions
{
    public class PerformBestActionForThisTurn : Behaviour
    {
        private EnercitiesActionInfo _action;
        private EnercitiesActionInfo _action2;
        private EnercitiesActionInfo _action3;
        private Timer _checkNotStuckTimer;

        public PerformBestActionForThisTurn()
        {
            var gameState = GameState.GetInstance();
            _action = gameState.BestActionsForMayor != null ? gameState.BestActionsForMayor[0] : null;
            _action2 = gameState.BestActionsForMayor != null ?  (gameState.BestActionsForMayor.Count>1?gameState.BestActionsForMayor[1]:null) : null;
            _action3 = gameState.BestActionsForMayor != null ? (gameState.BestActionsForMayor.Count > 2 ? gameState.BestActionsForMayor[2] : null) : null;

        }

        public override void BehaviourTask()
        {
            var client = InOutThalamusClient.GetInstance();

            if (_action == null)
            {
                client.IOPublisher.SkipTurn();
                AwaitAndEnd();
                return;
            }

            // Making sure that even if the upgrade process gets stuck, it doesn't stop the game
            _checkNotStuckTimer = new System.Timers.Timer(10000);
            _checkNotStuckTimer.AutoReset = false;
            _checkNotStuckTimer.Start();
            _checkNotStuckTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                if (GameState.GetInstance().CurrentState.CurrentPlayer.IsAI())
                    client.IOPublisher.SkipTurn();
            };

            switch (_action.ActionType)
            {
                case ActionType.BuildStructure:
                    var structure = (StructureType)_action.SubType;
                    client.IOPublisher.ConfirmConstruction(structure, _action.CellX, _action.CellY);
                    break;
                case ActionType.ImplementPolicy:
                    var policy = (PolicyType)_action.SubType;
                    client.IOPublisher.ImplementPolicy(policy);
                    break;
                case ActionType.UpgradeStructure:

                    var upgrade = (UpgradeType)_action.SubType;
                    client.IOPublisher.PerformUpgrade(upgrade, _action.CellX, _action.CellY);
                    Thread.Sleep(2000);

                    if (_action2 != null)
                    {
                        upgrade = (UpgradeType) _action2.SubType;
                        client.IOPublisher.PerformUpgrade(upgrade, _action2.CellX, _action2.CellY);
                        Thread.Sleep(2000);
                    }
                    if (_action3!=null){
                        upgrade = (UpgradeType)_action3.SubType;
                        client.IOPublisher.PerformUpgrade(upgrade, _action3.CellX, _action3.CellY);
                    }

                    break;
                case ActionType.SkipTurn:
                    client.IOPublisher.SkipTurn();
                    break;
            }
            ExecutionEnded();
            //AwaitAndEnd();
        }


        private async void AwaitAndEnd()
        {
            await Task.Delay(3000);
            ExecutionEnded();
        }
    }
}
