using System;
using CaseBasedController.Detection;
using EmoteEnercitiesMessages;
using EmoteEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using Thalamus.Actions;

namespace CaseBasedController.Behavior.Enercities
{
    /// <summary>
    ///     Executes a given action in Enercities.
    /// </summary>
    public class ExecuteAIRecomGameAction : BaseBehavior
    {
        private static readonly Dictionary<ActionType, string> Categories =
            new Dictionary<ActionType, string>
            {
                {ActionType.BuildStructure, "ConfirmConstruction"},
                {ActionType.UpgradeStructure, "PerformUpgrade"},
                {ActionType.ImplementPolicy, "ImplementPolicy"}
            };

        private IFeatureDetector _detector;
        private string _lastUttId;
        // Used as a switch to avoid commenting actions made in other situations. Look in the confirmConstruction/implementPolicy/.. event handler. They are always listening for actions. If they happen in a turn different from AI they will fire anyway. This flag avoids this situation.
        private bool _commentAction = false;

        public override void Execute(IFeatureDetector detector)
        {
            _detector = detector;
            lock (this.locker)
            {
                _commentAction = true;
                var action = GameInfo.GameStatus.CurrentState.BestActionsForThisTurn[0];
                

                // Some shit is going on
                if (action == null)
                {
                    this.actionPublisher.SkipTurn();
                    return;
                }
                

                //executes action accordingly
                switch (action.ActionType)
                {
                    case ActionType.BuildStructure:
                        var structure = (StructureType) action.SubType;
                        this.actionPublisher.ConfirmConstruction(structure, action.CellX, action.CellY);
                        break;
                    case ActionType.ImplementPolicy:
                        var policy = (PolicyType) action.SubType;
                        this.actionPublisher.ImplementPolicy(policy);
                        break;
                    case ActionType.UpgradeStructure:
                        var upgrade = (UpgradeType) action.SubType;
                        this.actionPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);
                        System.Threading.Thread.Sleep(2000);

                        var actions =
                            GameInfo.GameStatus.CurrentState.BestActionsForThisTurn.Where(x => (x != null)).ToArray();

                        action = actions[1];
                        upgrade = (UpgradeType) action.SubType;
                        this.actionPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);
                        System.Threading.Thread.Sleep(2000);

                        action = actions[2];
                        upgrade = (UpgradeType) action.SubType;
                        this.actionPublisher.PerformUpgrade(upgrade, action.CellX, action.CellY);

                        break;
                    case ActionType.SkipTurn:
                        this.actionPublisher.SkipTurn();
                        break;
                }
            }
        }

         

        public override void Cancel()
        {
        }

        protected override void AttachPerceptionEvents()
        {
            base.AttachPerceptionEvents();
            perceptionClient.UtteranceFinishedEvent += PerceptionClientOnUtteranceFinishedEvent;
            perceptionClient.ConfirmConstructionEvent += PerceptionClientOnConfirmConstructionEvent;
            perceptionClient.ImplementPolicyEvent += PerceptionClientOnImplementPolicyEvent;
            perceptionClient.PerformUpgradeEvent += PerceptionClientOnPerformUpgradeEvent;
            perceptionClient.SkipTurnEvent += perceptionClient_SkipTurnEvent;
        }

        void perceptionClient_SkipTurnEvent(object sender, GameActionEventArgs e)
        {
            _commentAction = false;
            RaiseFinishedEvent(_detector);
        }

        private void PerceptionClientOnUtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs ifmlUtteranceEventArgs)
        {
            if (ifmlUtteranceEventArgs.Id.Equals(_lastUttId))
                RaiseFinishedEvent(_detector);
        }

        private void PerceptionClientOnPerformUpgradeEvent(object sender, GameActionEventArgs gameActionEventArgs)
        {
            CommentAction(ActionType.UpgradeStructure, gameActionEventArgs.ActionTypeEnum);
        }

        private void PerceptionClientOnImplementPolicyEvent(object sender, GameActionEventArgs gameActionEventArgs)
        {
            CommentAction(ActionType.ImplementPolicy, gameActionEventArgs.ActionTypeEnum);
        }

        private void PerceptionClientOnConfirmConstructionEvent(object sender, GameActionEventArgs gameActionEventArgs)
        {
            CommentAction(ActionType.BuildStructure, gameActionEventArgs.ActionTypeEnum);
        }



        public override void Dispose()
        {
            perceptionClient.UtteranceFinishedEvent -= PerceptionClientOnUtteranceFinishedEvent;
            perceptionClient.ConfirmConstructionEvent -= PerceptionClientOnConfirmConstructionEvent;
            perceptionClient.ImplementPolicyEvent -= PerceptionClientOnImplementPolicyEvent;
            perceptionClient.PerformUpgradeEvent -= PerceptionClientOnPerformUpgradeEvent;
            perceptionClient.SkipTurnEvent -= perceptionClient_SkipTurnEvent;
        }


        private async void CommentAction(ActionType actionType, int actionTypeEnum)
        {
            await Task.Delay(500);
            lock (this.locker)
            {
                if (!_commentAction) return;
                _commentAction = false;
                _lastUttId = PerformUtterance(Categories[actionType], "self");


                //console.writeline("Explaining: main strategy = "+GameStatus.GetMainStrategy().ToString()+" - Action: "+actionType);

                // If the action and the main strategy used to compute it are clearly linked together, than we perform an utterance that explains this link
                switch (actionType)
                {
                    case ActionType.BuildStructure:
                        StructureType structureType = (StructureType) actionTypeEnum;
                        //console.writeline("Structure: "+structureType.ToString());

                        // ############ EXPLAIN BUILDING FOR POPULATION 
                        if (
                            (structureType == StructureType.Suburban || 
                            structureType == StructureType.Urban || 
                            structureType == StructureType.Residential_Tower) &&
                            (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Population)
                           )
                        {
                            _lastUttId = PerformUtterance("Explanation", "Population");
                        }

                        // ############ EXPLAIN BUILDING FOR ENERGY 
                        if (
                            (structureType == StructureType.Coal_Plant_Small || 
                            structureType == StructureType.Coal_Plant || 
                            structureType == StructureType.Hydro_Plant || 
                            structureType == StructureType.Nuclear_Fusion || 
                            structureType == StructureType.Nuclear_Plant ||
                            structureType == StructureType.Solar_Plant || 
                            structureType == StructureType.Super_Solar || 
                            structureType == StructureType.Super_WindTurbine || 
                            structureType == StructureType.Windmills) &&
                            (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Power)
                           )
                        {
                            _lastUttId = PerformUtterance("Explanation", "Energy");
                        }

                        // ############ EXPLAIN BUILDING FOR WELLBEING 
                        if (
                            (structureType == StructureType.Market ||
                            structureType == StructureType.Public_Services ||
                            structureType == StructureType.Stadium ||
                            structureType == StructureType.Commercial) &&
                            (GameStatus.GetMainStrategy() == GameStatus.AiActionStrategy.Wellbeing)
                           )
                        {
                            _lastUttId = PerformUtterance("Explanation", "Wellbeing");
                        }

                        break;
                }
            }
        }


        private string PerformUtterance(string category, string subcategory)
        {
            var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
            var uttId = "cbc" + new System.Random().Next();
            string[] tags = new List<string>(tagsAndValues.Keys).ToArray();
            string[] values = new List<string>(tagsAndValues.Values).ToArray();
            this.actionPublisher.PerformUtteranceFromLibrary(uttId, category, subcategory, tags, values);
            
            //console.writeline(">>>>>>>>>>>>>> PERFORMING: " + category + ", " + subcategory);

            return uttId;

        }
    }
}