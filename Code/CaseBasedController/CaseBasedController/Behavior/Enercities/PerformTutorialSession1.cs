using System;
using CaseBasedController.Detection;
using System.Collections.Generic;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Behavior.Enercities
{
    /// <summary>
    ///     Performs a specified utterance.
    /// </summary>
    public class PerformTutorialSession1 : BaseBehavior
    {
        string _greetingsUttId;
        string _tutorialConstructionUttId;
        string _confirmConstructionUttId;
        string _tutorialConstructionForOtherUttId;

        IFeatureDetector _detector;
        private bool _shouldComment;

        public override void Execute(IFeatureDetector detector)
        {
            _detector = detector;
            //just perform the utterance
            lock (this.locker)
            {
                System.Threading.Thread.Sleep(300);                                         // Waits to be sure that GameStatus is updated by all the coming events
                //console.writeline("Do Greetings");
                _greetingsUttId = PerformUtterance("greeting","welcome");
            }
        }

        public override void Dispose()
        {
        }

        protected override void UtteranceFinishedEvent(string id)
        {
            if (id.Equals(_greetingsUttId))
            {
                //console.writeline("Do tutorial construction");
                _tutorialConstructionUttId = PerformUtterance("tutorial", "OwnConstruction");
            }
            if (id.Equals(_tutorialConstructionUttId))
            {
                _shouldComment = true;
                //console.writeline("Do confirm contruction");
                actionPublisher.ConfirmConstruction(StructureType.Suburban, 5, 2);
                System.Threading.Thread.Sleep(1000);
                _tutorialConstructionForOtherUttId = PerformUtterance("tutorial", "OtherConstruction");
            }
            if (id.Equals(_tutorialConstructionForOtherUttId))
            {
                RaiseFinishedEvent(_detector);
            }
        }


        
        public override string ToString()
        {
            return "Tutorial Session 1";
        }

        public override void Cancel()
        {
        }
    }
}