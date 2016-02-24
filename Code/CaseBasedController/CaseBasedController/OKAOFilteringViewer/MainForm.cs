using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CustomRangeSelectorControl;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PS.Utilities.Math;

namespace OKAOFilteringViewer
{
    public partial class MainForm : Form
    {
        private const char SEPARATOR = ',';
        private const uint VIDEO_WIDTH = 1920;
        private const uint VIDEO_HEIGHT = 1080;
        private const uint MAX_SMILE = 100;
        private const uint MAX_SMILE_CONF = 1000;
        private const uint MAX_EXPRESSION = 100;
        private readonly NotifyClient _objNotifyClient = new NotifyClient();
        private List<double> _angerList;
        private List<double> _disgustList;
        private List<double> _fearList;
        private List<double> _gazeXList;
        private List<double> _gazeYList;
        private List<double> _joyList;
        private List<double> _neutralList;
        private ToolStripMenuItem _prevMenuItem;
        private double _q;
        private double _r;
        private List<double> _sadnessList;
        private List<double> _smileConfList;
        private List<double> _smileList;
        private List<double> _surpriseList;

        public MainForm()
        {
            this.InitializeComponent();

            ExcelUtil.EnableGraphics = true;
            this.rangeSelectorControl.RegisterForChangeEvent(ref this._objNotifyClient);

            this.UpdateFilterParams();
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!this.openFileDialog.ShowDialog().Equals(DialogResult.OK)) return;
            this.ProcessCSV(this.openFileDialog.FileName);
            this.groupBox1.Enabled =
                this.groupBox2.Enabled =
                    this.quantitiesChart.Enabled =
                        this.saveImageToolStripMenuItem.Enabled =
                            this.variableToolStripMenuItem.Enabled = true;

            this.UpdateChart(this.smileToolStripMenuItem);
            this.rangeSelectorControl.RangeString = string.Format("Time interval % ({0} OKAO frames):", this._smileList.Count);
        }

        private void SaveImageToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this._prevMenuItem == null) return;
            this.saveFileDialog.FileName = string.Format("{0}.pdf", this._prevMenuItem.Text.Replace("&", ""));
            if (!this.saveFileDialog.ShowDialog().Equals(DialogResult.OK)) return;
            this.quantitiesChart.SaveImage(this.saveFileDialog.FileName, 800, 600, ChartImageFormat.Pdf);
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProcessCSV(string fileName)
        {
            if (!File.Exists(fileName)) return;

            this._smileList = new List<double>();
            this._smileConfList = new List<double>();
            this._gazeXList = new List<double>();
            this._gazeYList = new List<double>();
            this._angerList = new List<double>();
            this._disgustList = new List<double>();
            this._fearList = new List<double>();
            this._joyList = new List<double>();
            this._sadnessList = new List<double>();
            this._surpriseList = new List<double>();
            this._neutralList = new List<double>();

            string line;
            using (var sr = new StreamReader(fileName))
                while ((line = sr.ReadLine()) != null)
                {
                    var fields = line.Split(SEPARATOR);

                    this._smileList.Add(double.Parse(fields[9])/MAX_SMILE);
                    this._smileConfList.Add(double.Parse(fields[10])/MAX_SMILE_CONF);
                    this._angerList.Add(double.Parse(fields[11])/MAX_EXPRESSION);
                    this._disgustList.Add(double.Parse(fields[12])/MAX_EXPRESSION);
                    this._fearList.Add(double.Parse(fields[13])/MAX_EXPRESSION);
                    this._joyList.Add(double.Parse(fields[14])/MAX_EXPRESSION);
                    this._sadnessList.Add(double.Parse(fields[15])/MAX_EXPRESSION);
                    this._surpriseList.Add(double.Parse(fields[16])/MAX_EXPRESSION);
                    this._neutralList.Add(double.Parse(fields[17])/MAX_EXPRESSION);
                    this._gazeXList.Add(double.Parse(fields[18])/VIDEO_WIDTH);
                    this._gazeYList.Add(double.Parse(fields[19])/VIDEO_HEIGHT);
                }
        }

        private void VariableToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.UpdateChart((ToolStripMenuItem) sender);
        }

        private void UpdateChart(ToolStripMenuItem menuItem)
        {
            List<double> values;
            if (menuItem.Equals(this.smileToolStripMenuItem))
                values = this._smileList;
            else if (menuItem.Equals(this.angerToolStripMenuItem))
                values = this._angerList;
            else if (menuItem.Equals(this.disgustToolStripMenuItem))
                values = this._disgustList;
            else if (menuItem.Equals(this.fearToolStripMenuItem))
                values = this._fearList;
            else if (menuItem.Equals(this.joyToolStripMenuItem))
                values = this._joyList;
            else if (menuItem.Equals(this.sadnessToolStripMenuItem))
                values = this._sadnessList;
            else if (menuItem.Equals(this.surpriseToolStripMenuItem))
                values = this._surpriseList;
            else if (menuItem.Equals(this.neutralToolStripMenuItem))
                values = this._neutralList;
            else if (menuItem.Equals(this.gazeVectorXToolStripMenuItem))
                values = this._gazeXList;
            else if (menuItem.Equals(this.gazeVectorYToolStripMenuItem))
                values = this._gazeYList;
            else return;

            var numSamples = this._smileList.Count;
            var minIdx = (int) ((int.Parse(this._objNotifyClient.Range1)/100d)*numSamples);
            var maxIdx = (int) ((int.Parse(this._objNotifyClient.Range2)/100d)*numSamples);
            var id = menuItem.Text.Replace("&", string.Empty);

            this.UpdateChart(values, minIdx, maxIdx, id);

            if (this._prevMenuItem != null)
                this._prevMenuItem.Checked = false;
            this._prevMenuItem = menuItem;
            this._prevMenuItem.Checked = true;
        }

        private void UpdateChart(List<double> values, int minIdx, int maxIdx, string id)
        {
            if ((values == null) || (values.Count == 0) || (minIdx >= maxIdx)) return;

            //gets range from list
            var rangeValues = new List<Matrix<double>>(maxIdx - minIdx);
            for (var i = minIdx; i < maxIdx; i++)
                rangeValues.Add(GetMatrix(values[i]));
            var rangeQuantity = GetQuantity(rangeValues);

            //filters range
            var rangeFilteredValues = KalmanFilter<double>.Filter(
                rangeValues, this.GetRValues(values, minIdx, maxIdx), GetMatrix(this._q));
            var rangeFilteredQuantity = GetQuantity(rangeFilteredValues);

            //adds range and filtered range to quantities chart
            this.quantitiesChart.Quantities.Clear();
            this.quantitiesChart.Title = string.Format("Filtering {0}", id);
            this.quantitiesChart.Quantities.Add(id, rangeQuantity);
            this.quantitiesChart.Quantities.Add(string.Format("{0} filtered", id), rangeFilteredQuantity);
            this.quantitiesChart.UpdateData();
            this.quantitiesChart.Invalidate();
        }

        private void UpdateChart()
        {
            if (this._prevMenuItem != null)
                this.UpdateChart(this._prevMenuItem);
        }

        private static DiagonalMatrix GetMatrix(double value)
        {
            return new DiagonalMatrix(1, 1, value);
        }

        private List<Matrix<double>> GetRValues(List<double> values, int minIdx, int maxIdx)
        {
            var numValues = maxIdx - minIdx;
            var list = new List<Matrix<double>>(numValues);

            //if value list is smile, take covariance info from smile confidence
            if (values.Equals(this._smileList))
            {
                for (var i = minIdx; i < maxIdx; i++)
                {
                    var smileR = this._smileList[i] * (1 - this._smileConfList[i]) * MAX_SMILE*this._r;
                    list.Add(GetMatrix(smileR));
                }
            }
            else
            {
                var rValueMat = GetMatrix(this._r);
                for (var i = 0; i < numValues; i++)
                    list.Add(rValueMat);
            }
            return list;
        }

        private static StatisticalQuantity GetQuantity(List<Matrix<double>> values)
        {
            var rangeQuantity = new StatisticalQuantity((uint) values.Count);
            rangeQuantity.AddRange(values.Select(mat => mat[0, 0]));
            return rangeQuantity;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.UpdateChart();
        }

        private void NUDqValueChanged(object sender, EventArgs e)
        {
            this.UpdateFilterParams();
            this.UpdateChart();
        }

        private void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            this.UpdateFilterParams();
            this.UpdateChart();
        }

        private void UpdateFilterParams()
        {
            this._q = (double) this.nUDq.Value;
            this._r = (double) this.nUDr.Value;
        }
    }
}