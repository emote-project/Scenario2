using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Behavior.Enercities
{
    class PerformWrapupAndGoodbye : BaseBehavior
    {
        private string _firstUttId;
        private string _secondUttId;

        public override void Dispose()
        {
        }

        public override void Execute(IFeatureDetector detector)
        {
            _firstUttId = PerformUtterance("wrapup", "0");

        }

        public override void Cancel()
        {
            

        }

        protected override void UtteranceFinishedEvent(string id)
        {
            if (_firstUttId != null && id.Equals(_firstUttId))
            {
                _secondUttId = PerformUtterance("greeting", "goodbye");
            }
            if (_secondUttId != null && id.Equals(_secondUttId))
            {
                RaiseFinishedEvent();
            }
        }

    }
}
