using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaseBasedController.Detection.Composition
{
    public class HasFiredFeatureDetector : WatcherFeatureDetector
    {
        private bool hasFired = false;

        protected override void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            hasFired = true;
            CheckActivationChanged();
        }

        public override double ActivationLevel
        {
            get { lock (this.locker) return IsActive ? 1 : 0; }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return hasFired; }
        }
    }
}
