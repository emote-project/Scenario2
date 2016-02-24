namespace CaseBasedController.Detection
{
    public abstract class BinaryFeatureDetector : BaseFeatureDetector
    {
        public override double ActivationLevel
        {
            get { return this.IsActive ? 1 : 0; }
        }
    }
}