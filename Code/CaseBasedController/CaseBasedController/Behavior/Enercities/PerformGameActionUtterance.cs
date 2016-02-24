using System;
using System.Collections.Generic;
using System.Linq;
using CaseBasedController.Detection;
using CaseBasedController.GameInfo;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;
using EmoteEvents;
using Newtonsoft.Json;

namespace CaseBasedController.Behavior.Enercities
{
    /// <summary>
    ///     Performs an utterance related with the execution of some action.
    /// </summary>
    public class PerformGameActionUtterance : BaseBehavior
    {
        private const string SELF_SUB_CAT_STR = "self";
        private string _uttID;
        private IFeatureDetector _detector;

        private static readonly Dictionary<ActionType, string> Categories =
            new Dictionary<ActionType, string>
            {
                {ActionType.BuildStructure, "ConfirmConstruction"},
                {ActionType.UpgradeStructure, "PerformUpgrade"},
                {ActionType.ImplementPolicy, "ImplementPolicy"}
            };

        public override void Execute(IFeatureDetector detector)
        {
            //console.writeline("Execute PerformGameActionUtterance: " + detector);
            _detector = detector;
            ActionType actionType = ActionType.SkipTurn;
            if (GameStatus.CurrentState.PlayedStructure != null) actionType = ActionType.BuildStructure;
            if (GameStatus.CurrentState.PlayedPolicy != null) actionType = ActionType.ImplementPolicy;
            if (GameStatus.CurrentState.PlayedUpgrade != null) actionType = ActionType.UpgradeStructure;
                
            //just perform the utterance
            lock (this.locker)
            {
                if (Categories.ContainsKey(actionType))
                {
                    var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
                    _uttID = PerformUtterance(Categories[actionType], SELF_SUB_CAT_STR);
                }
                else
                {
                    this.RaiseFinishedEvent(_detector);
                }
            }
        }

        public override void Cancel()
        {
            
        }

        protected override void UtteranceFinishedEvent(string id)
        {
            if (id.Equals(_uttID))
                this.RaiseFinishedEvent(_detector);
        }


    }
}