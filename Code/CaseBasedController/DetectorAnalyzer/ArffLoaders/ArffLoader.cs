using System;
using System.Collections.Generic;
using System.IO;
using PS.Utilities.IO;

namespace DetectorAnalyzer.ArffLoaders
{
    public abstract class ArffLoader : IDisposable
    {
        public const string DO_NOTHING_CLASS_STR = "﻿doNothingClass";
        protected const string TIME_ATTRIBUTE_STR = "Time";
        protected const string BEHAVIOUR_ATTRIBUTE_STR = "Behaviour";
        private const string DATA_STR = "@data";
        private const string RELATION_STR = "@relation ";

        protected ArffLoader()
        {
            this.FeatureIDs = new List<string>();
            this.LabelCount = new Dictionary<string, uint>();
        }

        public int NumInstances { get; private set; }

        public string RelationName { get; private set; }

        public List<string> FeatureIDs { get; private set; }

        public Dictionary<string, uint> LabelCount { get; private set; }

        #region IDisposable Members

        public virtual void Dispose()
        {
            this.LabelCount.Clear();
        }

        #endregion

        public virtual bool Load(string fileName, bool reset = false)
        {
            //checks file
            if (!File.Exists(fileName))
                return false;

            Console.WriteLine("\n======================================");
            Console.WriteLine("Opening {0}...", fileName);

            if (reset) this.Reset();
            var sr = new StreamReader(fileName);
            var line = sr.ReadLine();

            //checks relation name
            if ((line == null) || !line.ToLower().StartsWith(RELATION_STR)) return false;
            this.RelationName = line.Replace(RELATION_STR, "");
            Console.WriteLine("Reading relation {0}...", this.RelationName);

            //reads detector headers
            if (!this.ProcessDetectors(sr)) return false;
            Console.WriteLine("Found {0} detectors.\nProcessing instances...", this.FeatureIDs.Count);

            //reads all detectors states
            this.NumInstances = this.ProcessAllInstances(sr);
            Console.WriteLine("Finished reading all {0} instances.", NumInstances);

            sr.Close();
            sr.Dispose();
            return true;
        }

        protected virtual void Reset()
        {
            this.LabelCount.Clear();
        }

        protected virtual bool ProcessInstance(IList<string> fields, int instanceNum)
        {
            //checks fields and processes behavior
            if (fields.Count < (this.FeatureIDs.Count + 1)) return false;
            var behavior = fields[fields.Count - 1];
            this.ProcessBehavior(behavior);
            return true;
        }

        private bool ProcessDetectors(TextReader sr)
        {
            string line;
            var i = 0;
            while (((line = sr.ReadLine()) != null) && !line.ToLower().Equals(DATA_STR))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = line.Split(new[] {' '});
                var detector = fields.Length == 1 ? "detector" + this.FeatureIDs.Count : fields[1];
                if (!this.ProcessDetector(detector, i++)) return false;
            }
            return this.FeatureIDs.Count > 0;
        }

        protected virtual bool ProcessDetector(string detector, int idx)
        {
            //ignore time and behavior
            if (!detector.Equals(TIME_ATTRIBUTE_STR) &&
                !detector.Equals(BEHAVIOUR_ATTRIBUTE_STR) &&
                !this.FeatureIDs.Contains(detector))
                this.FeatureIDs.Add(detector);

            return true;
        }

        protected virtual int ProcessAllInstances(TextReader sr)
        {
            string line;
            var numInstances = 0;
            while ((line = sr.ReadLine()) != null)
            {
                var fields = line.Split(new[] {','});
                if (this.ProcessInstance(fields, numInstances))
                    numInstances++;
            }
            return numInstances;
        }

        public virtual void PrintResults(string baseDir)
        {
            Console.WriteLine("Printing results to {0}...", baseDir);
            PathUtil.CreateOrClearDirectory(baseDir);

            this.PrintBehaviorStats(baseDir);
        }

        private void PrintBehaviorStats(string baseDir)
        {
            var filePath = Path.GetFullPath(String.Format("{0}/behaviorCounts.csv", baseDir));
            Console.WriteLine("Printing behavior stats to {0}...", Path.GetFileName(filePath));
            if (File.Exists(filePath))
                File.Delete(filePath);

            var sw = new StreamWriter(filePath);

            foreach (var behaviorCount in this.LabelCount)
                sw.WriteLine(behaviorCount.Key + ";" + behaviorCount.Value);

            sw.Close();
            sw.Dispose();
        }

        protected virtual void ProcessBehavior(string behavior)
        {
            if (!this.LabelCount.ContainsKey(behavior))
                this.LabelCount.Add(behavior, 1);
            else
                this.LabelCount[behavior]++;
        }
    }
}