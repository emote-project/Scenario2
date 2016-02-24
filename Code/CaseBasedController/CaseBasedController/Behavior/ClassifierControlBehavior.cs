using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Behavior
{
    public class ClassifierControlBehavior : BaseBehavior
    {
        public bool UseClassifier { get; set; }

        public override void Dispose()
        {

        }

        public override void Execute(Detection.IFeatureDetector detector)
        {
            lock (this.locker)
            {
                MainController.UseClassifier(UseClassifier);
                this.RaiseFinishedEvent(detector);
                //console.writeline("Test>>>> " + UseClassifier);
            }
        }

        public override void Cancel()
        {
            
        }

        protected override void AttachPerceptionEvents()
        {
        }
    }
}
