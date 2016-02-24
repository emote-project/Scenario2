namespace Classification
{
    public class ClassificationResult
    {
        public ClassificationResult(double accuracy, string label)
        {
            this.Accuracy = accuracy;
            this.Label = label;
        }

        public double Accuracy { get; set; }

        public string Label { get; set; }
    }
}