using System.Collections.Generic;

namespace DetectorAnalyzer.ArffLoaders
{
    public class ClassificationArffLoader : ArffLoader
    {
        protected const int INVALID_INSTANCE_NUM = -1;

        public ClassificationArffLoader()
        {
            this.IgnoreLabels = new HashSet<string>();
            this.StartInstance = this.EndInstance = INVALID_INSTANCE_NUM;
        }

        public int StartInstance { get; set; }

        public int EndInstance { get; set; }

        public HashSet<string> IgnoreLabels { get; private set; }

        protected override bool ProcessInstance(IList<string> fields, int instanceNum)
        {
            if (!this.InstanceInRange(instanceNum)) return true;
            if (!base.ProcessInstance(fields, instanceNum)) return false;

            //gets and checks behavior
            var behavior = fields[fields.Count - 1];
            if (this.IgnoreLabels.Contains(behavior)) return false;

            //checks time to ignore first and last attribute
            var rawFields = new List<string>(fields);
            if (rawFields.Count == (this.FeatureIDs.Count + 2))
                rawFields.RemoveAt(0);
            rawFields.RemoveAt(fields.Count - 1);

            //processes the new transaction
            this.ProcessTransaction(this.GetTransaction(rawFields), behavior, instanceNum);
            return true;
        }

        protected virtual bool InstanceInRange(int instanceNum)
        {
            return ((this.StartInstance == INVALID_INSTANCE_NUM) ||
                    (this.EndInstance == INVALID_INSTANCE_NUM) ||
                    ((instanceNum >= this.StartInstance) && (instanceNum <= this.EndInstance)));
        }

        protected override void ProcessBehavior(string behavior)
        {
            if (!this.IgnoreLabels.Contains(behavior))
                base.ProcessBehavior(behavior);
        }

        protected IDictionary<string, string> GetTransaction(IList<string> rawFields)
        {
            //gets active (=1) attributes/features names to build a transaction
            var activeFeatures = new Dictionary<string, string>();

            for (var i = 0; i < rawFields.Count; i++)
                activeFeatures.Add(this.FeatureIDs[i], rawFields[i]);

            return activeFeatures;
        }

        protected virtual void ProcessTransaction(
            IDictionary<string, string> featuresState, string label, int transactionNum)
        {
        }
    }
}