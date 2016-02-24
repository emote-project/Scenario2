using System;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Example
{
    internal class MinuteSecondDetector : ExampleFeatureDetector
    {
        public int Second { get; set; }

        public override double ActivationLevel
        {
            get { return this.IsActive ? 1 : 0; }
        }

        public override bool IsActive
        {
            get { return DateTime.Now.Second.Equals(this.Second); }
        }

        public IAllPerceptionClient PerceptionClient { get; set; }
    }
}