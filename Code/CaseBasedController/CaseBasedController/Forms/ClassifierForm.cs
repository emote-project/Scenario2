using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaseBasedController.Classifier;

namespace CaseBasedController.Forms
{
    public partial class ClassifierForm : Form
    {
        ClassifierController _controller;


        public ClassifierForm(ClassifierController controller)
        {
            InitializeComponent();
        }

        public void Init(ClassifierController controller)
        {
            _controller = controller;
            if (_controller == null)
            {
                throw new Exception("The controller can't be null");
            }
            txtCasePoolInfo.Text = _controller.CasePoolPath;
            txtClassifierInfo.Text = _controller.ClassifierPath;
            numThreshold.Value = (decimal)_controller.AccuracyThreshold;
            UpdateState();
            _controller.InstanceClassifiedEvent += _controller_InstanceClassifiedEvent;
        }

        void _controller_InstanceClassifiedEvent(object sender, InstanceClassifiedEventArgs e)
        {
            this.Invoke(new Action(() =>
                {
                    txtLog.AppendText("("+e.Classification.Accuracy+") "+e.Classification.Label + Environment.NewLine);
                }));
        }

        public void UpdateState()
        {
            if (MainController.IsClassifierEnabled)
            {
                lblState.Text = "Enabled";
                lblState.ForeColor = Color.Green;
            }
            else
            {
                lblState.Text = "Disabled";
                lblState.ForeColor = Color.Red;
            }
        }

        public ClassifierForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load classifier file";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    await _controller.LoadAsync(ofd.FileName);
                    txtClassifierInfo.Text = ofd.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Log(string text)
        {
            if (this.Visible)
                this.Invoke(new Action(() =>
                {
                    txtLog.AppendText(text + Environment.NewLine);
                }));
        }

        private void numThreshold_ValueChanged(object sender, EventArgs e)
        {
            _controller.AccuracyThreshold = (double)((NumericUpDown)sender).Value;
        }

    }
}
