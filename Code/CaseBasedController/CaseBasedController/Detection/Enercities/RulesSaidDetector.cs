using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    public class RulesSaidDetector : BaseFeatureDetector
    {
        bool _rulesSaid = false;


        public override double ActivationLevel
        {
            get {
                return IsActive ? 1 : 0;
            }
        }

        public override bool IsActive
        {
            get { return _rulesSaid; }
        }

        public override void Dispose()
        {
            perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
            perceptionClient.PerformUtteranceFromLibraryEvent -= perceptionClient_PerformUtteranceFromLibraryEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.PerformUtteranceFromLibraryEvent += perceptionClient_PerformUtteranceFromLibraryEvent;
            perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            _rulesSaid = false;
            this.CheckActivationChanged();
        }

        void perceptionClient_PerformUtteranceFromLibraryEvent(object sender, Thalamus.PerformUtteranceFromLibraryEventArgs e)
        {
            if (e.Category.ToLower().Equals("rules"))
            {
               _rulesSaid = true;
               this.CheckActivationChanged();
            }
        }
    }
}
