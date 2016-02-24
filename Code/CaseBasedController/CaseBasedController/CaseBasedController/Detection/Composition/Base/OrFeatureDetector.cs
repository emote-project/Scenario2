using System.Linq;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Represents a disjunction of <see cref="BaseFeatureDetector" />: This means that the activation level
    ///     of this <see cref="BaseFeatureDetector" /> is given by the <see cref="BaseFeatureDetector" /> with the highest
    ///     activation level.
    /// </summary>
    public class OrFeatureDetector : CompositeFeatureDetector
    {
        /// <summary>
        ///     Gets the activation level of the most active (highest activation level) sub-detector.
        /// </summary>
        public override double ActivationLevel
        {
            get { return this.featureDetectors.Max(detector => detector.ActivationLevel); }
        }

        /// <summary>
        ///     True if all sub-detectors are active, false otherwise.
        /// </summary>
        public override bool IsActive
        {
            get { return this.featureDetectors.Any(detector => detector.IsActive); }
        }

        public override string ToString()
        {
            string subDetectors = "";
            foreach (var sub in Detectors) subDetectors += sub + " || ";
            subDetectors.TrimEnd(new char[] { '&', ' ' });
            return Description != "" && Description != null ? base.ToString() : "OR: " + subDetectors;
        }

        protected override void AttachEvents()
        {
        }
    }
}