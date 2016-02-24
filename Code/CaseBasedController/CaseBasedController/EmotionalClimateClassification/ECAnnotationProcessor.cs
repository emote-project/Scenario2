using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmotionalClimateClassification
{
    public enum ECClassification
    {
        Positive,
        Negative
        //NegativeL,
        //NegativeR,
        //BothNegative
    }

    public class ECAnnotationProcessor : IDisposable
    {
        private const char SEPARATOR = '\t';
        private const string BOTH_NEGATIVE_STR = "Both";
        private const string LEFT_NEGATIVE_STR = "S1";
        private const string RIGHT_NEGATIVE_STR = "S2";
        private readonly Dictionary<double, ECClassification> _annotations = new Dictionary<double, ECClassification>();

        public ECAnnotationProcessor(string ecAnnotFile)
        {
            if (string.IsNullOrWhiteSpace(ecAnnotFile) || !File.Exists(ecAnnotFile))
                throw new ApplicationException(string.Format("Invalid ELAN file provided: {0}", ecAnnotFile));

            this.ProcessELANFile(ecAnnotFile);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._annotations.Clear();
        }

        #endregion

        public ECClassification GetClassification(double seconds)
        {
            var times = new List<double>(this._annotations.Keys);
            for (var i = 1; i < times.Count; i++)
                if (seconds < times[i])
                    return this._annotations[times[i - 1]];

            return this._annotations.Last().Value;
        }

        private void ProcessELANFile(string ecAnnotFile)
        {
            using (var sr = new StreamReader(ecAnnotFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var elems = line.Split(new[] {SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);
                    var time = TimeSpan.Parse(elems[1]);
                    var annotation = elems[4];
                    var classification = annotation.StartsWith(BOTH_NEGATIVE_STR)
                        ? ECClassification.Negative
                        : annotation.StartsWith(LEFT_NEGATIVE_STR)
                            ? ECClassification.Negative
                            : annotation.StartsWith(RIGHT_NEGATIVE_STR)
                                ? ECClassification.Negative
                                : ECClassification.Positive;

                    this._annotations.Add(time.TotalSeconds, classification);
                }
            }
        }
    }
}