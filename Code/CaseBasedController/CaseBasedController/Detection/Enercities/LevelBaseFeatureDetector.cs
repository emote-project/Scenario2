using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController.Detection.Enercities
{
    public class LevelBaseFeatureDetector : BaseFeatureDetector
    {
        int _turnToDetect = 0;
        bool _isThatTurn = false;

        public int TurnToDetect
        {
            get { return _turnToDetect; }
            set { _turnToDetect = value; }
        }

        public LevelBaseFeatureDetector(int turnToDetect)
        {
            _turnToDetect = turnToDetect;
        }

        public override double ActivationLevel
        {
            get { return IsActive ? 1 : 0; }
        }

        public override bool IsActive
        {
            get 
            {
                return _isThatTurn;
            }
        }

        public override void Dispose()
        {
            lock (this.locker)
            {
                perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
            }
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            _isThatTurn = e.GameState.Level == _turnToDetect;
            CheckActivationChanged();
        }

        public override string ToString()
        {
            return "LevelBaseFeatureDetector (" + TurnToDetect + ")";
        }

    }
}
