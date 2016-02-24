using System;
using CaseBasedController.Thalamus;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CaseBasedController.Detection
{

    #region Delegates

    public delegate void ActivatedEventHandler(IFeatureDetector detector, bool activated);

    #endregion

    /// <summary>
    ///     Represents a detector of a specific feature of the system.
    /// </summary>
    public interface IFeatureDetector : IDisposable
    {
        /// <summary>
        /// Descriptive string that explains what this detector does
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     The level of activation of the feature being detected by this <see cref="IFeatureDetector" />.
        ///     The value should range from: 0 (not active) to 1 (fully activated).
        /// </summary>
        [JsonIgnore]
        double ActivationLevel { get; }

        /// <summary>
        ///     Whether this <see cref="IFeatureDetector" />'s feature is currently being "detected".
        /// </summary>
        [JsonIgnore]
        bool IsActive { get; }

        /// <summary>
        ///     Occurs when this <see cref="IFeatureDetector" /> becomes active/inactive.
        /// </summary>
        event ActivatedEventHandler ActivationChanged;

        /// <summary>
        ///     Initiates this <see cref="IFeatureDetector" /> according to the provided <see cref="IAllPerceptionClient" />.
        /// </summary>
        /// <param name="perceptionClient"> The client responsible for this <see cref="BaseFeatureDetector" /> perceptions.</param>
        void Init(IAllPerceptionClient perceptionClient);

        /// <summary>
        ///     Checks whether the activation state of this <see cref="BaseFeatureDetector" /> changed.
        ///     Raises (de)activation events accordingly.
        /// </summary>
        /// <param name="force">
        ///     determines whether this is a manual demand to check for activation events. Useful for initial activation check in
        ///     event-based detectors.
        /// </param>
        void CheckActivationChanged(bool force = false);
    }

    public class FeatureDetectorEqualityComparer : IEqualityComparer<IFeatureDetector>
    {

        public bool Equals(IFeatureDetector b1, IFeatureDetector b2)
        {
            if (b1.Description == b2.Description &
                b1.ActivationLevel == b2.ActivationLevel &
                b1.IsActive == b2.IsActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       public int GetHashCode(IFeatureDetector bx)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ (bx.Description == null ? 0 : bx.Description.GetHashCode());
                hash = hash * 16777619 ^ bx.ActivationLevel.GetHashCode();
                hash = hash * 16777619 ^ bx.IsActive.GetHashCode();
                return hash;
            }
        }


    }
}