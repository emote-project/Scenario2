using System.Linq;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Represents a conjunction of <see cref="BaseFeatureDetector" />: This means that the activation level
    ///     of this <see cref="BaseFeatureDetector" /> is given by the weighted sum of all composing
    ///     <see cref="BaseFeatureDetector" />.
    /// </summary>
    public class AndFeatureDetector : CompositeFeatureDetector
    {
        /// <summary>
        ///     Gets the average of the activation levels of the several sub-detectors.
        /// </summary>
        public override double ActivationLevel
        {
            get
            {
                return this.Count == 0
                    ? 0
                    : this.featureDetectors.Sum(detector => detector.ActivationLevel) / this.Count;
            }
        }

        /// <summary>
        ///     True if all sub-detectors are active, false otherwise.
        /// </summary>
        public override bool IsActive
        {
            get { 
                return this.featureDetectors.All(detector => detector.IsActive); 
            }
        }

        protected override void AttachEvents()
        {
            foreach (IFeatureDetector d in Detectors)
            {
                d.ActivationChanged += this.SubDetectorActivationChanged;
            }
        }

        public override string ToString()
        {
            string subDetectors = "";
            foreach(var sub in Detectors) subDetectors+= sub+" && "; 
            subDetectors.TrimEnd(new char[] {'&',' '});
            return Description != "" && Description != null ? base.ToString() : "AND: " + subDetectors;
        }
    }
}