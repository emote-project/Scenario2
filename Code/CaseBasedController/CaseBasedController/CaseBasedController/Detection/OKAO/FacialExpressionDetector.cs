using CaseBasedController.Thalamus;

namespace CaseBasedController.Detection.OKAO
{
    public enum Expression
    {
        Anger,
        Disgust,
        Fear,
        Joy,
        Sadness,
        Surprise,
        Neutral
    }

    public class FacialExpressionDetector : OKAOFeatureDetector
    {
        /// <summary>
        ///     The facial expression we want to detect.
        /// </summary>
        public Expression Expression { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}", base.ToString(), this.Expression);
        }

        protected override void OnOKAOPerceptionEvent(object sender, OKAOPerceptionArgs okaoPerceptionArgs)
        {
            base.OnOKAOPerceptionEvent(sender, okaoPerceptionArgs);

            lock (this.locker)
            {
                var perception = okaoPerceptionArgs.PerceptionLog;
                double value;
                switch (this.Expression)
                {
                    case Expression.Anger:
                        value = perception.anger;
                        break;
                    case Expression.Disgust:
                        value = perception.disgust;
                        break;
                    case Expression.Fear:
                        value = perception.fear;
                        break;
                    case Expression.Joy:
                        value = perception.joy;
                        break;
                    case Expression.Sadness:
                        value = perception.sadness;
                        break;
                    case Expression.Surprise:
                        value = perception.surprise;
                        break;
                    default:
                        value = perception.neutral;
                        break;
                }
                this.UpdateValue(value);
            }
        }
    }
}