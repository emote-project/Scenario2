using System.Windows.Forms;
using CaseBasedController.Detection;

namespace CaseBasedController.Example
{
    internal class MessageBoxBehavior : ExampleBehavior
    {
        public string Text { get; set; }

        public override void Execute(IFeatureDetector detector)
        {
            MessageBox.Show(this.Text);
        }
    }
}