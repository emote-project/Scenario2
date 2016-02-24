using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Classification.Classifier;
using EmoteCommonMessages;
using EmotionalClimateClassification;

namespace ECModule
{
    public enum EmotionalClimate
    {
        Positive,
        Negative,
        Neutral
    }

    public partial class EmotionalClimateForm : Form
    {
        private const string EC_MODEL_DT_FILE_PATH = "Data\\J48_1_1.model";
        private const string EC_MODEL_NN_FILE_PATH = "Data\\MultilayerPerceptron_1_1.model";
        private const string EC_MODEL_SVM_FILE_PATH = "Data\\SMO_1_1.model";
        private const string EC_DATA_ARFF_FILE_PATH = "Data\\ECdata.arff";

        public readonly OkaoPerceptionFilter RightSubjOkaoFilter = new OkaoPerceptionFilter();
        public readonly OkaoPerceptionFilter LeftSubjOkaoFilter = new OkaoPerceptionFilter();
        public EmotionalClimate EmotionalClimate { get; set; }
        public bool OKAOPerceptionOccurred { get; set; }
        public List<WekaClassifier> ECClassifiers;
        private ECThalamusClient _client;

        public EmotionalClimateForm()
        {
            this.InitializeComponent();

            //loads EC models
            ECClassifiers = new List<WekaClassifier>
                    {
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH),
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH),
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH)
                    };

            ECClassifiers[0].Load(EC_MODEL_DT_FILE_PATH);
            ECClassifiers[1].Load(EC_MODEL_NN_FILE_PATH);
            ECClassifiers[2].Load(EC_MODEL_SVM_FILE_PATH);

            _client = new ECThalamusClient(this);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var oldEc = EmotionalClimate;

            //if okao message not received just zero the filters
            if (!OKAOPerceptionOccurred)
            {
                LeftSubjOkaoFilter.UpdateFilters(new OkaoPerception());
                RightSubjOkaoFilter.UpdateFilters(new OkaoPerception());
            }

            //gets classifications
            var numNegativeVotes = 0;
            if (ECClassifiers != null)
            {
                foreach (var classifier in ECClassifiers)
                {
                    var result = classifier.Classify(GetCurrentInstance());
                    if (result != null)
                    {
                        EmotionalClimate ec;
                        Enum.TryParse(result.Label, out ec);
                        if (ec.Equals(EmotionalClimate.Negative))
                            numNegativeVotes++;
                        var textBox = classifier.Equals(ECClassifiers[0])
                            ? this.txtDTProb
                            : classifier.Equals(ECClassifiers[1])
                                ? this.txtNNProb
                                : this.txtSVMProb;
                        textBox.Text = result.Accuracy.ToString("0.000");
                    }
                }
            

                //only change negative EC if all classifiers agree
                EmotionalClimate = numNegativeVotes >= ECClassifiers.Count*this.nudThresh.Value ? 
                    EmotionalClimate.Negative : EmotionalClimate.Positive;

            }

            //if (result.Accuracy >= (double)this.nudDTThresh.Value)
            //    EmotionalClimate = ec;

            //update EC info
            this.txtECLabel.Text = EmotionalClimate.ToString();
            this.txtECLabel.ForeColor = EmotionalClimate.Equals(EmotionalClimate.Positive)
                ? Color.ForestGreen
                : EmotionalClimate.Equals(EmotionalClimate.Negative)
                    ? Color.DarkRed
                    : Color.DimGray;

            //update perceptions info
            this.leftSubjOkaoControl.UpdatePerception(LeftSubjOkaoFilter.FilteredPerception);
            this.rightSubjOkaoControl.UpdatePerception(RightSubjOkaoFilter.FilteredPerception);

            OKAOPerceptionOccurred = false;

            if (oldEc != EmotionalClimate)
            {
                var ecState = EmotionalClimate == EmotionalClimate.Negative
                    ? EmotionalClimateLevel.Negative
                    : EmotionalClimateLevel.Positive;
                _client.ECPublisher.EmotionalClimateLevel(ecState);
            }
        }

        private void CheckEnabledCheckedChanged(object sender, EventArgs e)
        {
            this.timer.Enabled = this.checkEnabled.Checked;
        }

        private Dictionary<string, string> GetCurrentInstance()
        {
            var leftPercept = LeftSubjOkaoFilter.FilteredPerception;
            var rightPercept = RightSubjOkaoFilter.FilteredPerception;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _client.Dispose();
        }

        public void UpdatePerception(OKAOScenario2Perception perc)
        {
            var filter = perc.subject.ToLower().Equals("left")
                ? LeftSubjOkaoFilter
                : RightSubjOkaoFilter;

            filter.UpdateFilters(new OkaoPerception
            {
                Smile = (uint)perc.smile,
                SmileConfidence = (uint)perc.confidence,
                Anger = (uint)perc.anger,
                Disgust = (uint)perc.disgust,
                Fear = (uint)perc.fear,
                Sadness = (uint)perc.sadness,
                Surprise = (uint)perc.surprise,
                Joy = (uint)perc.joy,
                Neutral = (uint)perc.neutral,
                LookAtX = perc.gazeVectorX,
                LookAtY = perc.gazeVectorY,
                LookAt = perc.gazeDirection
            });

            OKAOPerceptionOccurred = true;
        }

        private void EmotionalClimateForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _client.Dispose();
        }
    }
}