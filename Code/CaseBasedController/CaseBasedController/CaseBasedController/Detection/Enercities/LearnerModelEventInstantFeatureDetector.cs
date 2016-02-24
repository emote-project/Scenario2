using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    public class LearnerModelEventInstantFeatureDetector : InstantFeatureDetector
    {
        public LearnerModelEventInstantFeatureDetector()
        {
            TimePeriod = 5000;
        }

        public override void Dispose()
        {
            lock (this.locker)
            {
                perceptionClient.LearnerModelMemoryEvent -= PerceptionClientOnLearnerModelMemoryEvent;
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                perceptionClient.LearnerModelMemoryEvent += PerceptionClientOnLearnerModelMemoryEvent;
            }
        }

        private void PerceptionClientOnLearnerModelMemoryEvent(object sender, LearnerModelMemoryEventArgs learnerModelMemoryEventArgs)
        {
            this.OnFeatureDetected();
        }
    }
}
