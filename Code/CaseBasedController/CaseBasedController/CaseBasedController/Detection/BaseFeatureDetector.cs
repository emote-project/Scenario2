using System.Threading.Tasks;
using CaseBasedController.Thalamus;
using System;

namespace CaseBasedController.Detection
{
    /// <summary>
    ///     The base class for a detector of a specific feature of the system. Includes all the logic
    ///     for raising activation events and activation state.
    /// </summary>
    public abstract class BaseFeatureDetector : IFeatureDetector
    {
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum delay between successive activations (not used if set to 0)
        /// </summary>
        public int ActivationsMinDelayMilliseconds
        {
            get; set;
        }

        private DateTime _lastActivationTime;
        private MyTimer.TimerData _timer;  

        protected readonly object locker = new object();
        private bool _wasActive;
        protected IAllPerceptionClient perceptionClient;

        #region IFeatureDetector Members

        public abstract double ActivationLevel { get; }

        public abstract bool IsActive { get; }

        public event ActivatedEventHandler ActivationChanged;

        public virtual void Init(IAllPerceptionClient client)
        {
            this.perceptionClient = client;
            this.AttachEvents();
        }

        public abstract void Dispose();

        public virtual void CheckActivationChanged(bool force = false)
        {
            lock (this.locker)
            {
                if (this.ActivationChanged == null) return;

                var isActive = this.IsActive;
                if (!(isActive ^ this._wasActive)) return;
                if (!_wasActive && isActive && DateTime.Now.Subtract(_lastActivationTime).TotalMilliseconds < ActivationsMinDelayMilliseconds) return; // Not activating if not enough time passed after last activation
                
                if (isActive) _lastActivationTime = DateTime.Now;
                if (ActivationsMinDelayMilliseconds > 0)
                {
                    _timer = MyTimer.RegisterTimer(ActivationsMinDelayMilliseconds/1000,
                        delegate
                        {
                            CheckActivationChanged();
                        });
                }
                
                Task.Factory.StartNew(() => this.ActivationChanged(this, isActive));
                
                this._wasActive = isActive;
            }
        }

        #endregion

        /// <summary>
        ///     Attaches all relevant events from the <see cref="perceptionClient" />.
        /// </summary>
        protected abstract void AttachEvents();

        public override string ToString()
        {
            return string.Format("{0}({1})", this.Description, this.GetType().Name);
        }

        #region Equality check

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

        //public override bool Equals(object obj)
        //{
        //    //console.writeline("asdjaskdljhaskdhj!");
        //    return this.GetType() == obj.GetType() && this.Description == ((IFeatureDetector)obj).Description;
        //}

        //public override int GetHashCode()
        //{
        //    unchecked // Overflow is fine, just wrap
        //    {
        //        int hash = (int)2166136261;
        //        hash = hash * 16777619 ^ (Description == null ? 0 : Description.GetHashCode());
        //        hash = hash * 16777619 ^ ActivationLevel.GetHashCode();
        //        hash = hash * 16777619 ^ IsActive.GetHashCode();
        //        hash = hash * 16777619 ^ GetType().ToString().GetHashCode();
        //        return hash;
        //    }
        //}

        //public static bool operator ==(BaseFeatureDetector a, BaseFeatureDetector b)
        //{
        //    // If both are null, or both are same instance, return true.
        //    if (System.Object.ReferenceEquals(a, b))
        //    {
        //        return true;
        //    }

        //    // If one is null, but not both, return false.
        //    if (((object)a == null) || ((object)b == null))
        //    {
        //        return false;
        //    }

        //    // Return true if the fields match:
        //    return a.Description == b.Description && a.IsActive == b.IsActive && a.ActivationLevel == b.ActivationLevel;
        //}

        //public static bool operator !=(BaseFeatureDetector a, BaseFeatureDetector b)
        //{
        //    return !(a == b);
        //}

        #endregion
    }
}