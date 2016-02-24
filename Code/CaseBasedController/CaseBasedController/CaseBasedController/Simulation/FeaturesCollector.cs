using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Simulation
{
    public class NewFeaturesVectorEventArgs : EventArgs
    {
        public string[] FeaturesVector { get; set; }
    }

    public class FeaturesCollector : IDisposable
    {
        public event EventHandler<NewFeaturesVectorEventArgs> NewFeaturesVector;

        public const string CLASS_TO_LEARN_NAME = "Behaviour";
        public const string CLASS_TO_LEARN_NULL_VALUE = "doNothingClass";

        private object locker = new object();

        #region Properties
        List<string> _featuresNames = new List<string>();
        public List<string> FeaturesNames
        {
            get { return _featuresNames; }
            set { _featuresNames = value; }
        }
        List<string[]> _featuresVectors = new List<string[]>();
        public List<string[]> FeaturesVectors
        {
            get { return _featuresVectors; }
            set { _featuresVectors = value; }
        }
        List<TimeSpan> _featuresVectorsTime = new List<TimeSpan>();
        public List<TimeSpan> FeaturesVectorsTime
        {
            get { return _featuresVectorsTime; }
            set { _featuresVectorsTime = value; }
        }

        #endregion

        public FeaturesCollector(List<string> featuresNames)
        {
            FeaturesVectors.Clear();
            _featuresVectorsTime.Clear();
            FeaturesNames = featuresNames;
            string[] features = new string[FeaturesNames.Count];
            for (int i = 0; i < FeaturesNames.Count(); i++) features[i] = "0";
            //InstanciateNewFeaturesVector(features);
            features[FeaturesNames.IndexOf(CLASS_TO_LEARN_NAME)] = CLASS_TO_LEARN_NULL_VALUE;
            FeaturesVectors.Add(features);
            FeaturesVectorsTime.Add(MyTimer.GetCurrentTime());
        }

        public void SetClassForThisVector(string className)
        {
            lock (this.locker)
            {
                // if there's already a class for this veatures vector, than copy it and set the new class to the copied one
                int indx = FeaturesNames.IndexOf(CLASS_TO_LEARN_NAME);
                if (!FeaturesVectors.Last()[indx].Equals(CLASS_TO_LEARN_NULL_VALUE))
                {
                    FeaturesVectors.Add(FeaturesVectors.Last());
                    FeaturesVectorsTime.Add(MyTimer.GetCurrentTime());
                }
                FeaturesVectors.Last()[indx] = className;
                FeaturesVectorsTime[FeaturesVectorsTime.Count - 1] = MyTimer.GetCurrentTime();
            }
        }


        public void SetFeature(string featureName, bool active)
        {
            lock (this.locker)
            {
                var indx = FeaturesNames.IndexOf(featureName);
                if (indx < 0)
                {
                    //throw new System.Exception("Cann't find feature <" + featureName + ">");
                }
                else
                {
                    string[] features = new string[FeaturesNames.Count()];
                    Array.Copy(FeaturesVectors.Last(), features, FeaturesVectors.Last().Count());
                    features[indx] = active ? "1" : "0";
                    InstanciateNewFeaturesVector(features);
                }
            }
        }

        private void InstanciateNewFeaturesVector(string[] features)
        {
            lock (this.locker)
            {
                bool isEqual = false;
                if (FeaturesVectors.Count > 0)
                    isEqual = Enumerable.SequenceEqual(FeaturesVectors.Last(), features);
                if (!isEqual)
                {
                    if (NewFeaturesVector != null) NewFeaturesVector(this, new NewFeaturesVectorEventArgs() { FeaturesVector = FeaturesVectors.Last() });
                    FeaturesVectors.Add(features);
                    FeaturesVectorsTime.Add(MyTimer.GetCurrentTime());
                    FeaturesVectors.Last()[FeaturesNames.IndexOf(CLASS_TO_LEARN_NAME)] = CLASS_TO_LEARN_NULL_VALUE;
                }
            }
        }


        /// <summary>
        /// Associate at each feature its name
        /// </summary>
        /// <param name="featuresVector">the vector of features to which associate the names</param>
        /// <returns>A dictionary containing a set of features' names & value pairs</returns>
        public Dictionary<string, string> GetFeaturesVectorWithNames(List<string> featuresVector)
        {
            lock (this.locker)
            {
                var vec = new Dictionary<string, string>();
                for (int i = 0; i < _featuresNames.Count; i++)
                {
                    vec.Add(_featuresNames[i], featuresVector[i]);
                }
                return vec;
            }
        }


        public void Dispose()
        {
            NewFeaturesVector = null;
            FeaturesNames.Clear();
            FeaturesVectors.Clear();
            FeaturesVectorsTime.Clear();
        }
    }
}
