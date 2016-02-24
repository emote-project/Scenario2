using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Detection;

namespace CaseBasedController.Detection.Enercities
{
    class PerformUtteranceDetector : InstantFeatureDetector
    {
        public override void Dispose()
        {
            lock (this.locker)
                this.perceptionClient.PerformUtteranceEvent -= perceptionClient_PerformUtteranceFromLibraryEvent;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.PerformUtteranceEvent += perceptionClient_PerformUtteranceFromLibraryEvent;
        }

        void perceptionClient_PerformUtteranceFromLibraryEvent(object sender, Thalamus.PerformUtteranceEventArgs e)
        {
            lock (this.locker)
            {
                this.OnFeatureDetected();
            }
        }


    }
}
