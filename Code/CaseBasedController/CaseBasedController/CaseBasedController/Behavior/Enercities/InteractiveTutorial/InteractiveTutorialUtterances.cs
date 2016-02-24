using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Detection;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Behavior.Enercities.InteractiveTutorial
{
    class InteractiveTutorialUtterances : BaseBehavior
    {
        private IFeatureDetector _detector;
        private string _firstUttId;

        public string FirstCategory { get; set; }
        public string FirstSubcategory { get; set; }
        public string SecondCategory { get; set; }
        public string SecondSubcategory { get; set; }

        public InteractiveTutorialUtterances(string firstCategory, string firstSubcategory, string secondCategory, string secondSubcategory)
        {
            FirstCategory = firstCategory;
            FirstSubcategory = firstSubcategory;
            SecondCategory = secondCategory;
            SecondSubcategory = secondSubcategory;
        }

        public override void Dispose()
        {
            perceptionClient.UtteranceFinishedEvent -= PerceptionClientOnUtteranceFinishedEvent;
        } 

        public override void Execute(IFeatureDetector detector)
        {
            lock (this.locker)
            {
                _detector = detector;
                System.Threading.Thread.Sleep(300);                                         // Waits to be sure that GameStatus is updated by all the coming events
                _firstUttId = PerformUtterance(FirstCategory, FirstSubcategory);
            }
        }

        public override void Cancel()
        {
        }

        protected override void AttachPerceptionEvents()
        {
            perceptionClient.UtteranceFinishedEvent += PerceptionClientOnUtteranceFinishedEvent;
        }

        
        private void PerceptionClientOnUtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs ifmlUtteranceEventArgs)
        {
            //console.writeline("Utterance finished: "+_firstUttId);
            if (_firstUttId!=null && _firstUttId.Equals(ifmlUtteranceEventArgs.Id))
            {
                RaiseFinishedEvent(_detector);
                _firstUttId = null;
                PerformUtterance(SecondCategory, SecondSubcategory);
            }
        }


        private string PerformUtterance(string category, string subcategory)
        {
            var tagsAndValues = GameInfo.GameStatus.GetTagNamesAndValues();
            var uttId = "cbc" + new System.Random().Next();
            string[] tags = new List<string>(tagsAndValues.Keys).ToArray();
            string[] values = new List<string>(tagsAndValues.Values).ToArray();
            this.actionPublisher.PerformUtteranceFromLibrary(uttId, category, subcategory, tags, values);
            return uttId;
        }
    }
}
