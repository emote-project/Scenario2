using CaseBasedController.Detection;
using System.Collections.Generic;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;

namespace CaseBasedController.Behavior.Enercities
{
    /// <summary>
    ///     Performs a specified utterance.
    /// </summary>
    public class PerformUtterance : BaseBehavior
    {
        public bool FinishImmediately = false;

        public PerformUtterance(bool finishImmediately = false)
        {
            this.Subcategory = this.Category = string.Empty;
            FinishImmediately = finishImmediately;
        }

        /// <summary>
        ///     The category of the specified
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     The subcategory of the specified
        /// </summary>
        public string Subcategory { get; set; }

        string _uttId = "";
        IFeatureDetector _detector;

        public override void Execute(IFeatureDetector detector)
        {
            _detector = detector;
            //just perform the utterance
            lock (this.locker)
            {
                System.Threading.Thread.Sleep(300);                                         // Waits to be sure that GameStatus is updated by all the coming events
                _uttId = PerformUtterance(Category, Subcategory);

                if (FinishImmediately)
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
            if (_uttId.Equals(id))
                this.RaiseFinishedEvent(_detector);
        }

        public override string ToString()
        {
            return "PerformUtterance:"+Category+":"+Subcategory;
        }




    }
}