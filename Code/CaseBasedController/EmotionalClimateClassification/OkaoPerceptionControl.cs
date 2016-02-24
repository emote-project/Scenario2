using System.Windows.Forms;

namespace EmotionalClimateClassification
{
    public partial class OkaoPerceptionControl : UserControl
    {
        public OkaoPerceptionControl()
        {
            this.InitializeComponent();
        }

        public void UpdatePerception(OkaoPerception perception)
        {
            this.txtSmile.Text = perception.Smile.ToString("0.00");
            this.txtSmileConf.Text = perception.SmileConfidence.ToString("0.00");
            this.txtAnger.Text = perception.Anger.ToString("0.00");
            this.txtDisgust.Text = perception.Disgust.ToString("0.00");
            this.txtFear.Text = perception.Fear.ToString("0.00");
            this.txtJoy.Text = perception.Joy.ToString("0.00");
            this.txtSad.Text = perception.Sadness.ToString("0.00");
            this.txtSurprise.Text = perception.Surprise.ToString("0.00");
            this.txtNeutral.Text = perception.Neutral.ToString("0.00");
            this.txtLookX.Text = perception.LookAtX.ToString("0.00");
            this.txtLookY.Text = perception.LookAtY.ToString("0.00");
            this.txtLook.Text = perception.LookAt;
        }
    }
}