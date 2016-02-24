using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.OKAO
{
    public class SmileDetector : OKAOFeatureDetector
    {
        private const double DEFAULT_THRESHOLD = 0.6;
        private const uint MAX_CONF_VALUE = 1000;

        public SmileDetector()
        {
            this.Threshold = DEFAULT_THRESHOLD;
        }

        protected override void OnOKAOPerceptionEvent(object sender, OKAOPerceptionArgs okaoPerceptionArgs)
        {
            base.OnOKAOPerceptionEvent(sender, okaoPerceptionArgs);

            lock (this.locker)
            {
                var smileValue = okaoPerceptionArgs.PerceptionLog.smile;

                //for smile, confidence is used for the covariance
                var smileConf = 1d - (okaoPerceptionArgs.PerceptionLog.confidence/MAX_CONF_VALUE);
                var smileR = smileValue*smileConf*smileConf;
                this.filter.R = GetMatrix(smileR);

                this.UpdateValue(smileValue);
            }
        }
    }
}