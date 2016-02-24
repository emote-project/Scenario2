using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CaseBasedController.GameInfo;
using EmotionalClimateClassification;

namespace CaseBasedController.Forms
{
    public partial class EmotionalClimateForm : Form
    {
        public EmotionalClimateForm()
        {
            this.InitializeComponent();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            //if okao message not received just zero the filters
            if (!GameStatus.OKAOPerceptionOccurred)
            {
                GameStatus.LeftSubjOkaoFilter.UpdateFilters(new OkaoPerception());
                GameStatus.RightSubjOkaoFilter.UpdateFilters(new OkaoPerception());
            }

            //gets classifications
            var numNegativeVotes = 0;
            EmotionalClimate ec;
            if (GameStatus.ECClassifiers != null)
            {
                foreach (var classifier in GameStatus.ECClassifiers)
                {
                    var result = classifier.Classify(GetCurrentInstance());
                    if (result != null)
                    {
                        Enum.TryParse(result.Label, out ec);
                        if (ec.Equals(EmotionalClimate.Negative))
                            numNegativeVotes++;
                        var textBox = classifier.Equals(GameStatus.ECClassifiers[0])
                            ? this.txtDTProb
                            : classifier.Equals(GameStatus.ECClassifiers[1])
                                ? this.txtNNProb
                                : this.txtSVMProb;
                        textBox.Text = result.Accuracy.ToString("0.000");
                    }
                }
            

                //only change negative EC if all classifiers agree
                GameStatus.EmotionalClimate = numNegativeVotes >= GameStatus.ECClassifiers.Count*this.nudThresh.Value ? 
                    EmotionalClimate.Negative : EmotionalClimate.Positive;

            }

            //if (result.Accuracy >= (double)this.nudDTThresh.Value)
            //    GameStatus.EmotionalClimate = ec;

            //update EC info
            ec = GameStatus.EmotionalClimate;
            this.txtECLabel.Text = ec.ToString();
            this.txtECLabel.ForeColor = ec.Equals(EmotionalClimate.Positive)
                ? Color.ForestGreen
                : ec.Equals(EmotionalClimate.Negative)
                    ? Color.DarkRed
                    : Color.DimGray;

            //update perceptions info
            this.leftSubjOkaoControl.UpdatePerception(GameStatus.LeftSubjOkaoFilter.FilteredPerception);
            this.rightSubjOkaoControl.UpdatePerception(GameStatus.RightSubjOkaoFilter.FilteredPerception);

            GameStatus.OKAOPerceptionOccurred = false;
        }

        private void CheckEnabledCheckedChanged(object sender, EventArgs e)
        {
            this.timer.Enabled = this.checkEnabled.Checked;
        }

        private static Dictionary<string, string> GetCurrentInstance()
        {
            var leftPercept = GameStatus.LeftSubjOkaoFilter.FilteredPerception;
            var rightPercept = GameStatus.RightSubjOkaoFilter.FilteredPerception;
            var features = new Dictionary<string, string>
                           {
                               {"L-Anger", leftPercept.Anger.ToString()},
                               {"L-Disgust", leftPercept.Disgust.ToString()},
                               {"L-Fear", leftPercept.Fear.ToString()},
                               {"L-Joy", leftPercept.Joy.ToString()},
                               {"L-Sadness", leftPercept.Sadness.ToString()},
                               {"L-Surprise", leftPercept.Surprise.ToString()},
                               {"L-Neutral", leftPercept.Neutral.ToString()},
                               {"L-LookAt", leftPercept.LookAt},
                               {"L-LookAtX", leftPercept.LookAtX.ToString(CultureInfo.InvariantCulture)},
                               {"L-LookAtY", leftPercept.LookAtY.ToString(CultureInfo.InvariantCulture)},
                               {"L-Smile", leftPercept.Smile.ToString()},
                               {"R-Anger", rightPercept.Anger.ToString()},
                               {"R-Disgust", rightPercept.Disgust.ToString()},
                               {"R-Fear", rightPercept.Fear.ToString()},
                               {"R-Joy", rightPercept.Joy.ToString()},
                               {"R-Sadness", rightPercept.Sadness.ToString()},
                               {"R-Surprise", rightPercept.Surprise.ToString()},
                               {"R-Neutral", rightPercept.Neutral.ToString()},
                               {"R-LookAt", rightPercept.LookAt},
                               {"R-LookAtX", rightPercept.LookAtX.ToString(CultureInfo.InvariantCulture)},
                               {"R-LookAtY", rightPercept.LookAtY.ToString(CultureInfo.InvariantCulture)},
                               {"R-Smile", rightPercept.Smile.ToString()}
                           };
            return features;
        }

        private void NudTimeValueChanged(object sender, EventArgs e)
        {
            this.timer.Interval = (int) this.nudTime.Value*1000;
        }
    }
}