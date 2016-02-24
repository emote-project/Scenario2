using System.Collections.Generic;
using System.Linq;
using CaseBasedController.Thalamus;
using Newtonsoft.Json;

namespace CaseBasedController.Detection.Composition
{
    /// <summary>
    ///     Represents a <see cref="BaseFeatureDetector" /> that is a composition of several other
    ///     <see cref="IFeatureDetector" /> objects.
    ///     The class is thread-safe to handle with detection events.
    /// </summary>
    public abstract class CompositeFeatureDetector : BaseFeatureDetector //, IList<IFeatureDetector>
    {
        protected List<IFeatureDetector> featureDetectors = new List<IFeatureDetector>();

        [JsonIgnore]
        public int Count
        {
            get { lock (this.locker) return this.featureDetectors.Count; }
        }

        //public IFeatureDetector this[int index]
        //{
        //    get { lock (this.locker) return this.featureDetectors[index]; }
        //    set
        //    {
        //        this.featureDetectors[index] = value;
        //    }
        //}

        /// <summary>
        ///     Gets the list of all <see cref="IFeatureDetector" /> associated with this <see cref="CompositeFeatureDetector" />.
        ///     The internal pool of <see cref="IFeatureDetector" /> is not affected by changes to this list.
        /// </summary>
        public List<IFeatureDetector> Detectors
        {
            get { return this.featureDetectors; }
            set { this.featureDetectors = value; }
        }

        public override void Init(IAllPerceptionClient client)
        {
            this.featureDetectors.ForEach(detector =>
            {
                detector.Init(client);
                detector.ActivationChanged += this.SubDetectorActivationChanged;
            });
            base.Init(client);
        }

        public override void Dispose()
        {
            this.featureDetectors.ForEach(detector => detector.Dispose());
            this.Clear();
        }

        public override void CheckActivationChanged(bool force = false)
        {
            if (force) this.featureDetectors.ForEach(detector => detector.CheckActivationChanged());
            base.CheckActivationChanged(force);
        }

        protected virtual void SubDetectorActivationChanged(IFeatureDetector subDetector, bool activated)
        {
            //check if this detector became active as a result of the change of the sub-detector.
            this.CheckActivationChanged(false);
        }

        #region List methods

        public void Add(IFeatureDetector detector)
        {
            lock (this.locker)
            {
                //attaches activation events
                detector.ActivationChanged += this.SubDetectorActivationChanged;
                this.featureDetectors.Add(detector);
            }
        }

        public bool Remove(IFeatureDetector detector)
        {
            lock (this.locker)
            {
                if (!this.featureDetectors.Contains(detector)) return false;

                //detaches activation events
                detector.ActivationChanged -= this.SubDetectorActivationChanged;
                return this.featureDetectors.Remove(detector);
            }
        }

        public void AddRange(IEnumerable<IFeatureDetector> detectors)
        {
            lock (this.locker)
                foreach (var detector in detectors)
                {
                    this.Add(detector);
                    detector.ActivationChanged += this.SubDetectorActivationChanged;
                }
        }

        public void Clear()
        {
            lock (this.locker)
            {
                foreach (var d in featureDetectors) d.ActivationChanged -= this.SubDetectorActivationChanged;
                this.featureDetectors.Clear();
            }
        }

        #endregion

       
    }
}