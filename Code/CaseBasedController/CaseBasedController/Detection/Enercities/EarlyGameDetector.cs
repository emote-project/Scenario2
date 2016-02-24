using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaseBasedController.Detection.Enercities
{
    public class EarlyGameDetector : BaseFeatureDetector
    {
        private int _earlyTurns = 3;


        public EarlyGameDetector(int earlyTurns = 3)
        {
            _earlyTurns = earlyTurns;
        }


        public override double ActivationLevel
        {
            get 
            {
                lock (this.locker)
                {
                    if (GameInfo.GameStatus.CurrentState != null)
                    {
                        double val = _earlyTurns - GameInfo.GameStatus.CurrentState.TurnNumber;
                        val = val / _earlyTurns;
                        return val > 0 ? val : 0;
                    }
                    return 0;
                }
            }
        }

        public override bool IsActive
        {
            get { lock (this.locker) return this.ActivationLevel > 0; }
        }

        public override void Dispose()
        {
            this.perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
        }

        protected override void AttachEvents()
        {
            lock(this.locker)
                this.perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            this.CheckActivationChanged();
        }
    }
}
