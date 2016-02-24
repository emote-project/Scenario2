using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    /// Detects whether we are in the indicated sessionNumber or not
    /// </summary>
    class SessionBinaryFeatureDetector : BinaryFeatureDetector
    {
        public int SessionNumber { get; set; }

        /// <summary>
        /// Detects whether we are in the indicated sessionNumber or not
        /// </summary>
        /// <param name="sessionNumber">The sessionNumber to check we are in</param>
        public SessionBinaryFeatureDetector(int sessionNumber)
        {
            SessionNumber = sessionNumber;
        }

        public override bool IsActive
        {
            get
            {
                return GameInfo.GameStatus.Session == SessionNumber;
            }
        }

        public override void Dispose()
        {
            perceptionClient.StartEvent -= PerceptionClientOnStartEvent;
        }

        protected override void AttachEvents()
        {
            perceptionClient.StartEvent += PerceptionClientOnStartEvent;
        }

        private void PerceptionClientOnStartEvent(object sender, StartEventArgs startEventArgs)
        {
            CheckActivationChanged();
        }

        public override string ToString()
        {
            return "SessionFeatureDetector: " + SessionNumber;
        }
    }
}
