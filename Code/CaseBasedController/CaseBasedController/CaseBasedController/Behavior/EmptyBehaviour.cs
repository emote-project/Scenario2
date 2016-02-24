using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Behavior
{
    public class EmptyBehaviour : BaseBehavior
    {
        public override void Dispose()
        {
            
        }

        public override void Execute(Detection.IFeatureDetector detector)
        {
            this.RaiseFinishedEvent(detector);
        }

        public override void Cancel()
        {
            
        }

        protected override void AttachPerceptionEvents()
        {
            
        }
    }
}
