using System.Linq;
using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    ///     Detects what is the "subject" the players may be talking about.
    /// </summary>
    public class KeywordDetector : BinaryFeatureDetector
    {
        private bool _keywordDetected;

        /// <summary>
        ///     The sprecified "subject" or "keword" to be detected.
        /// </summary>
        public string Keyword { get; set; }

        public override bool IsActive
        {
            get { lock (this.locker) return this._keywordDetected; }
        }

        public override void Dispose()
        {
            //detach event
            lock (this.locker)
                this.perceptionClient.WordDetectedEvent -= this.OnWordDetectedEvent;
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
                if (this.perceptionClient != null)
                    this.perceptionClient.WordDetectedEvent += this.OnWordDetectedEvent;
        }

        private void OnWordDetectedEvent(object sender, WordDetectedEventArgs e)
        {
            lock (this.locker)
            {
                //checks activation based on detected words, raise event if necessary
                this._keywordDetected = e.Words.Contains(this.Keyword);
                this.CheckActivationChanged();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.ToString(), this.Keyword);
        }
    }
}