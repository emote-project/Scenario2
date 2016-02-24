using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Classification.Classifier;
using PS.Utilities;
using PS.Utilities.Collections;
using PS.Utilities.Math;

namespace Classification
{
    public class ClassificationPerformance : IDisposable
    {
        private readonly List<uint> _labelCounts;
        private readonly uint _numLabels;
        private uint _numCorrect, _numIncorrect, _numUnsupported;

        public ClassificationPerformance(uint numLabels, IClassifier classifier, IEnumerable<uint> labelCounts)
        {
            this._numLabels = numLabels;
            this.ConfusionMatrix = ArrayUtil.Create2DArray<double>(numLabels, numLabels);
            this.Support = new StatisticalQuantity();

            if ((classifier == null) || (labelCounts == null)) return;

            //gets stats lists from classifier
            this.Labels = classifier.Labels.ToList();
            this._labelCounts = labelCounts.ToList();
        }

        public double[][] ConfusionMatrix { get; private set; }
        public StatisticalQuantity NumCorrect { get; private set; }
        public StatisticalQuantity NumIncorrect { get; private set; }
        public StatisticalQuantity NumUnsupported { get; private set; }
        public StatisticalQuantity Support { get; private set; }
        public List<StatisticalQuantity> LabelCounts { get; private set; }
        public List<string> Labels { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            this.ConfusionMatrix = null;
            this.NumCorrect.Dispose();
            this.NumIncorrect.Dispose();
            this.NumUnsupported.Dispose();
            this.Labels.Clear();
            this.LabelCounts.Clear();
            this._labelCounts.Clear();
        }

        #endregion

        public void AddPerformance(uint numCorrect, uint numIncorrect, uint numUnsupported, double support)
        {
            this._numCorrect += numCorrect;
            this._numIncorrect += numIncorrect;
            this._numUnsupported += numUnsupported;
            this.Support.Value = support;
        }

        public static ClassificationPerformance GetAverage(List<ClassificationPerformance> performances)
        {
            if (performances.Count == 0) return null;
            var numLabels = performances[0]._numLabels;
            var avgPerformance = new ClassificationPerformance(numLabels, null, null)
                                 {
                                     Labels = performances[0].Labels.ToList()
                                 };

            //averages confusion matrix
            for (var i = 0; i < numLabels; i++)
                for (var j = 0; j < numLabels; j++)
                    avgPerformance.ConfusionMatrix[i][j] = GetAverage(i, j, performances);

            //averages other stats
            avgPerformance.NumCorrect = StatisticalQuantity.GetQuantitiesAverage(
                performances.Select(performance => performance.NumCorrect).ToList());
            avgPerformance.NumIncorrect = StatisticalQuantity.GetQuantitiesAverage(
                performances.Select(performance => performance.NumIncorrect).ToList());
            avgPerformance.NumUnsupported = StatisticalQuantity.GetQuantitiesAverage(
                performances.Select(performance => performance.NumUnsupported).ToList());
            avgPerformance.Support = StatisticalQuantity.GetQuantitiesAverage(
                performances.Select(performance => performance.Support).ToList());

            //averages label stats
            avgPerformance.LabelCounts = new List<StatisticalQuantity>((int) numLabels);
            for (var i = 0; i < numLabels; i++)
            {
                var countQuantity = new StatisticalQuantity();
                foreach (var performance in performances)
                    countQuantity.Value = performance._labelCounts[i];
                avgPerformance.LabelCounts.Add(countQuantity);
            }

            return avgPerformance;
        }

        private static double GetAverage(int i, int j, IEnumerable<ClassificationPerformance> performances)
        {
            var quantity = new StatisticalQuantity();
            foreach (var performance in performances)
                quantity.Value = performance.ConfusionMatrix[i][j];
            return quantity.Avg;
        }

        public void Finish()
        {
            this.NumCorrect = new StatisticalQuantity {Value = this._numCorrect};
            this.NumIncorrect = new StatisticalQuantity {Value = this._numIncorrect};
            this.NumUnsupported = new StatisticalQuantity {Value = this._numUnsupported};
            this.Support = new StatisticalQuantity {Value = this.Support.Avg};

            this.LabelCounts = new List<StatisticalQuantity>((int) this._numLabels);
            for (var i = 0; i < this._numLabels; i++)
                this.LabelCounts.Add(new StatisticalQuantity {Value = this._labelCounts[i]});
        }

        public void PrintResults(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var sw = new StreamWriter(filePath);

            //prints header
            for (var i = 0; i < this._numLabels; i++)
                sw.Write("{0}{1}", i.GetExcelColumnName(), ';');
            sw.WriteLine("<- classified as{0}Correct{0}Rel. Accuracy{0}Total{0}Accuracy{0}", ';');

            //prints confusion matrix and calcs totals and accuracy
            var labelTotal = new double[this._numLabels];
            var correct = new double[this._numLabels];
            for (var y = 0; y < this._numLabels; y++)
            {
                var label = this.Labels[y];
                for (var x = 0; x < this._numLabels; x++)
                {
                    var confusedVal = this.ConfusionMatrix[x][y];
                    sw.Write("{0};", confusedVal);

                    labelTotal[y] += confusedVal;
                    if (y == x) correct[y] = confusedVal;
                }

                var correctCount = correct[y];
                var labelCount = this.LabelCounts[y].Avg;
                sw.WriteLine("{0} = {1}{6}{2}{6}{3}{6}{4}{6}{5}{6}",
                    y.GetExcelColumnName(), label,
                    correctCount, correctCount.Equals(0) ? 0 : correct[y]/labelTotal[y],
                    labelCount, correct[y]/labelCount, ';');
            }

            //print other stats
            var total = this.NumCorrect.Avg + this.NumIncorrect.Avg;
            var relAccuracy = labelTotal.Equals(0) ? 0 : this.NumCorrect.Avg/total;
            var accuracy = labelTotal.Equals(0) ? 0 : this.NumCorrect.Avg/(total + this.NumUnsupported.Avg);

            sw.WriteLine();
            sw.WriteLine("Num correct;{0};", this.NumCorrect.Avg);
            sw.WriteLine("Num incorrect;{0};", this.NumIncorrect.Avg);
            sw.WriteLine("Num unsupported;{0};", this.NumUnsupported.Avg);
            sw.WriteLine("Rel accuracy;{0};", relAccuracy);
            sw.WriteLine("Accuracy;{0};", accuracy);
            sw.WriteLine("Avg support;{0};", this.Support.Avg);

            sw.Close();
            sw.Dispose();
        }
    }
}