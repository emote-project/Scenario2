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
    public class PerformTutorial : BaseBehavior
    {

        /// <summary>
        ///     The category of the specified
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     The subcategory of the specified
        /// </summary>
        public string Subcategory { get; set; }

        string utt1ID = "";
        string utt2ID = "";
        IFeatureDetector _detector;
        private bool _shouldComment;

        public override void Execute(IFeatureDetector detector)
        {
            _detector = detector;
            //just perform the utterance
            lock (this.locker)
            {
                System.Threading.Thread.Sleep(300);                                         // Waits to be sure that GameStatus is updated by all the coming events
                utt1ID = PerformUtterance("gamestatus","tutorial");
            }
        }

        public override string ToString()
        {
            return "Tutorial :"+Category+":"+Subcategory;
        }

        public override void Cancel()
        {
            this.actionPublisher.CancelUtterance(utt1ID);
        }

        public override void Dispose()
        {
            perceptionClient.ConfirmConstructionEvent -= PerceptionClientOnConfirmConstructionEvent;
        }

        protected override void UtteranceFinishedEvent(string id)
        {
            if (id.Equals(utt1ID))
            {
                actionPublisher.ConfirmConstruction(StructureType.Suburban, 5, 2);
                _shouldComment = true;
            }
            if (id.Equals(utt2ID))
            {
                this.RaiseFinishedEvent(_detector);
            }
        }

        protected override void AttachPerceptionEvents()
        {
            base.AttachPerceptionEvents();
            perceptionClient.ConfirmConstructionEvent += PerceptionClientOnConfirmConstructionEvent;
        }

        private void PerceptionClientOnConfirmConstructionEvent(object sender, GameActionEventArgs gameActionEventArgs)
        {
            if (_shouldComment)     // Avoiding commenting future actions
            {
                _shouldComment = false; 
                utt2ID = PerformUtterance("confirmconstruction", "self");
            }
        }

    }
}