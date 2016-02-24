using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Behavior
{
    class RandomBehavior : BaseBehavior
    {
        private List<BaseBehavior> behaviors = new List<BaseBehavior>();
        int _indx;

        public List<BaseBehavior> Behaviors
        {
            get { return behaviors; }
            set { behaviors = value; }
        }

        public override void Dispose()
        {
            foreach (var b in behaviors) b.Dispose();
        }

        public override void Execute(Detection.IFeatureDetector detector)
        {
            Random rnd = new Random();
            _indx = rnd.Next(1, behaviors.Count);
            behaviors[_indx].Execute(detector);
        }

        public override void Cancel()
        {
            behaviors[_indx].Cancel();
        }

        protected override void AttachPerceptionEvents()
        {
        }
    }
}
