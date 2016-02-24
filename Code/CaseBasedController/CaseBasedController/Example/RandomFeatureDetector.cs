using System;
using CaseBasedController.Detection;

namespace CaseBasedController.Example
{
    internal class RandomFeatureDetector : ThresholdFeatureDetector
    {
        private readonly Random _random = new Random();

        public override double ActivationLevel
        {
            get { return this._random.NextDouble(); }
        }

        public override void Dispose()
        {
        }

        protected override void AttachEvents()
        {
        }
    }
}