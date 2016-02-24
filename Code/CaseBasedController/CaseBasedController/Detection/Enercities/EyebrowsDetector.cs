using CaseBasedController.Detection.Composition;
using CaseBasedController.Thalamus;
using EmoteEnercitiesMessages;
using Newtonsoft.Json;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    ///     Detects the "eyes" expression of one of the players.
    /// </summary>
    public class EyebrowsDetector : BaseFeatureDetector
    {
        public EyebrowsDetector()
        {
            this.AuDetectors = new AndFeatureDetector();
        }

        /// <summary>
        ///     The player this detector refers to.
        /// </summary>
        public EnercitiesRole Player { get; set; }

        /// <summary>
        ///     A detector containing  a <see cref="IntervalFeatureDetector" /> for each eyebrow (left and right).
        /// </summary>
        public AndFeatureDetector AuDetectors { get; private set; }

        [JsonIgnore]
        private IntervalFeatureDetector AU2Detector
        {
            get { return (IntervalFeatureDetector) this.AuDetectors.Detectors[0]; }
        }

        [JsonIgnore]
        private IntervalFeatureDetector AU4Detector
        {
            get { return (IntervalFeatureDetector) this.AuDetectors.Detectors[1]; }
        }

        public override double ActivationLevel
        {
            get { lock (this.locker) return this.AuDetectors.ActivationLevel; }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return this.AuDetectors.IsActive; }
        }

        public override void Dispose()
        {
            lock (this.locker)
            {
                this.perceptionClient.EyebrowsAUEvent -= this.OnEyebrowsAUEvent;
                this.AuDetectors.Dispose();
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.EyebrowsAUEvent += this.OnEyebrowsAUEvent;
        }

        private void OnEyebrowsAUEvent(object sender, EyebrowsAUEventArgs e)
        {
            lock (this.locker)
            {
                //checks activation based on detected words, raise event if necessary
                this.AU2Detector.Value = this.Player.Equals(EnercitiesRole.Economist) ? e.Au2User1 : e.Au2User2;
                this.AU4Detector.Value = this.Player.Equals(EnercitiesRole.Economist) ? e.Au4User1 : e.Au4User2;
                this.CheckActivationChanged();
            }
        }
    }
}