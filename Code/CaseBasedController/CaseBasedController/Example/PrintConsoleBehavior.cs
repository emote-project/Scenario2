using System;
using System.Threading;
using CaseBasedController.Detection;

namespace CaseBasedController.Example
{
    internal class PrintConsoleBehavior : ExampleBehavior
    {
        public string Text { get; set; }

        public override void Execute(IFeatureDetector detector)
        {
            //console.writeline(this.Text);

            //simulate execution..
            Thread.Sleep((int) (this.ExecutionDuration*1000));
            this.SendExecutionFinished();
        }

        public override string ToString()
        {
            return string.Format("{0}: \"{1}\"", base.ToString(), this.Text);
        }
    }
}