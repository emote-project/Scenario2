using System;
using System.Collections.Generic;
using System.IO;
using PS.Utilities.Math;

namespace DetectorAnalyzer.ArffLoaders
{
    public class StatisticsArffLoader : ArffLoader
    {
        #region Fields

        private bool[] _detectorsActiveStatus;
        private int[] _detectorsFreqSinceLastAction;
        private long[] _detectorsLastActivationTime;
        private long[] _detectorsStatusTime;

        #endregion

        #region Constructor

        public StatisticsArffLoader()
        {
            this.ActivationFreqStats = new StatisticsCollection();
            this.TimeActiveStats = new StatisticsCollection();
            this.TimeInactiveStats = new StatisticsCollection();
            this.TimeSinceLastActivationStats = new StatisticsCollection();
        }

        #endregion

        #region Properties

        public StatisticsCollection ActivationFreqStats { get; private set; }
        public StatisticsCollection TimeActiveStats { get; private set; }
        public StatisticsCollection TimeInactiveStats { get; private set; }
        public StatisticsCollection TimeSinceLastActivationStats { get; private set; }

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            this.ActivationFreqStats.Dispose();
            this.TimeActiveStats.Dispose();
            this.TimeInactiveStats.Dispose();
            this.TimeSinceLastActivationStats.Dispose();

            this.FeatureIDs.Clear();
            this._detectorsActiveStatus = null;
            this._detectorsStatusTime = null;
            this._detectorsLastActivationTime = null;
            this._detectorsFreqSinceLastAction = null;
        }

        #region Processing Methods

        protected override void Reset()
        {
            this.ActivationFreqStats.Clear();
            this.TimeActiveStats.Clear();
            this.TimeInactiveStats.Clear();
            this.TimeSinceLastActivationStats.Clear();
            this.FeatureIDs.Clear();
        }

        protected override bool ProcessDetector(string detector, int idx)
        {
            if (!base.ProcessDetector(detector, idx)) return false;
            if ((idx == 0) && !detector.Equals(TIME_ATTRIBUTE_STR)) return false;

            if (this.ActivationFreqStats.ContainsKey(detector)) return true;
            this.ActivationFreqStats.Add(detector, new StatisticalQuantity());
            this.TimeActiveStats.Add(detector, new StatisticalQuantity());
            this.TimeInactiveStats.Add(detector, new StatisticalQuantity());
            this.TimeSinceLastActivationStats.Add(detector, new StatisticalQuantity());

            return true;
        }

        protected override int ProcessAllInstances(TextReader sr)
        {
            //allocates space for the algorithms variables
            this._detectorsActiveStatus = new bool[this.FeatureIDs.Count];
            this._detectorsStatusTime = new long[this.FeatureIDs.Count];
            this._detectorsLastActivationTime = new long[this.FeatureIDs.Count];
            this._detectorsFreqSinceLastAction = new int[this.FeatureIDs.Count];

            return base.ProcessAllInstances(sr);
        }

        protected override bool ProcessInstance(IList<string> fields, int instanceNum)
        {
            base.ProcessInstance(fields, instanceNum);

            //checks fields
            if (fields.Count != (this.FeatureIDs.Count + 2)) return false;

            //gets time and behavior
            int time;
            if (!Int32.TryParse(fields[0], out time)) return false;
            var behavior = fields[fields.Count - 1];

            //processes detectors
            for (var i = 0; i < this.FeatureIDs.Count; i++)
            {
                int activatedVal;
                if (!Int32.TryParse(fields[i + 1], out activatedVal)) return false;
                this.ProcessDetectorStatus(i, instanceNum, time, activatedVal == 1,
                    !behavior.Equals(DO_NOTHING_CLASS_STR));
            }

            return true;
        }

        private void ProcessDetectorStatus(int i, int eventNum, int time, bool activated, bool wozBehavior)
        {
            //checks first event
            if (eventNum == 0)
            {
                this.ResetDetectorCounters(i, time, activated);
                return;
            }

            var wasActive = this._detectorsActiveStatus[i];

            //if a WoZ action was performed, collect information about this detector
            if (wozBehavior)
                this.ProcessStatsSinceLastBehavior(i, time, activated, wasActive);

            //if nothing changed do nothing
            if (activated ^ wasActive)
                this.ProcessActivationChanged(i, time, activated);
        }

        private void ProcessStatsSinceLastBehavior(int i, int time, bool activated, bool wasActive)
        {
            var detector = this.FeatureIDs[i];
            var timePassedStatus = time - this._detectorsStatusTime[i];
            var timePassedLastActive = this._detectorsLastActivationTime[i].Equals(-1)
                ? 0
                : time - this._detectorsLastActivationTime[i];

            //updates time (in)active, frequency and time since last activation stats
            if (wasActive)
                this.TimeActiveStats[detector].Value = timePassedStatus;
            else
                this.TimeInactiveStats[detector].Value = timePassedStatus;

            this.ActivationFreqStats[detector].Value = this._detectorsFreqSinceLastAction[i];
            this.TimeSinceLastActivationStats[detector].Value = timePassedLastActive;

            //resets counters since last action
            this.ResetDetectorCounters(i, time, activated);
        }

        private void ProcessActivationChanged(int i, int time, bool activated)
        {
            //checks activation, updates accordingly
            if (activated)
            {
                this._detectorsFreqSinceLastAction[i]++;
                this._detectorsLastActivationTime[i] = time;
            }

            //sets current status values
            this._detectorsActiveStatus[i] = activated;
            this._detectorsStatusTime[i] = time;
        }

        private void ResetDetectorCounters(int i, int time, bool activated)
        {
            this._detectorsActiveStatus[i] = activated;
            this._detectorsStatusTime[i] = time;
            this._detectorsFreqSinceLastAction[i] = 0;
            this._detectorsLastActivationTime[i] = -1;
        }

        #endregion

        #region Printing Methods

        public override void PrintResults(string baseDir)
        {
            base.PrintResults(baseDir);
            this.PrintDetectorsStats(baseDir);
        }

        private void PrintDetectorsStats(string baseDir)
        {
            var filePath = Path.GetFullPath(String.Format("{0}/detectorStats.csv", baseDir));
            Console.WriteLine("Printing detector stats to {0}...", Path.GetFileName(filePath));
            if (File.Exists(filePath))
                File.Delete(filePath);

            var sw = new StreamWriter(filePath);
            sw.WriteLine(GetHeader());

            //prints stats for all detectors
            foreach (var detector in this.FeatureIDs)
                sw.WriteLine(this.GetDetectorStatsLine(detector));

            sw.Close();
            sw.Dispose();
        }

        private static string GetHeader()
        {
            return String.Format("Detector;{0}{1}{2}{3}",
                GetStatHeader("freq"),
                GetStatHeader("active"),
                GetStatHeader("inactive"),
                GetStatHeader("timeSLA"));
        }

        private static string GetStatHeader(string prefix)
        {
            return String.Format("{0}-total;{0}-max;{0}-min;{0}-avg;{0}-stdDev;", prefix);
        }

        private string GetDetectorStatsLine(string detector)
        {
            return String.Format("{0};{1}{2}{3}{4}",
                detector,
                GetDetectorStatStr(this.ActivationFreqStats, detector),
                GetDetectorStatStr(this.TimeActiveStats, detector),
                GetDetectorStatStr(this.TimeInactiveStats, detector),
                GetDetectorStatStr(this.TimeSinceLastActivationStats, detector));
        }

        private static string GetDetectorStatStr(StatisticsCollection stats, string detector)
        {
            if (!stats.ContainsKey(detector)) return String.Empty;
            var quantity = stats[detector];
            return String.Format("{0};{1};{2};{3};{4};",
                quantity.Sum, quantity.Max, quantity.Min, quantity.Avg, quantity.StdDev);
        }

        #endregion
    }
}