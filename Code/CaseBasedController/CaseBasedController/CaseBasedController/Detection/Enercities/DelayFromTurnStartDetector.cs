using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CaseBasedController.Detection.Enercities
{
    /// <summary>
    /// Fires when a delay elapses from the turn start
    /// </summary>
    public class DelayFromTurnStartDetector : BaseFeatureDetector
    {
        CaseBasedController.MyTimer.TimerData _timer;

        /// <summary>
        /// Milliseconds from the turn start that have to pass for this detector to be active
        /// </summary>
        public double Delay { get; set; }

        private bool elapsed = false;
        // if the timer elapses when the turnNumber is different from the current in GameState, than it doesn't considers the detector active
        private int turnNumber = 0;

        public override double ActivationLevel
        {
            get { return this.IsActive ? 1 : 0; }
        }

        public override bool IsActive
        {
            get { return this.elapsed && turnNumber == (GameInfo.GameStatus.CurrentState != null ? GameInfo.GameStatus.CurrentState.TurnNumber : 0); }
        }

        public override void Dispose()
        {
            lock (this.locker)
            {
                this.perceptionClient.TurnChangedEvent -= perceptionClient_TurnChangedEvent;
            }
        }

        protected override void AttachEvents()
        {
            lock (this.locker)
            {
                this.perceptionClient.TurnChangedEvent += perceptionClient_TurnChangedEvent;
            }
        }

        void perceptionClient_TurnChangedEvent(object sender, Thalamus.GenericGameEventArgs e)
        {
            if (_timer!=null)
                MyTimer.RemoveTimer(_timer);
            elapsed = false;
            CheckActivationChanged();
            turnNumber = GameInfo.GameStatus.CurrentState.TurnNumber;
            _timer = MyTimer.RegisterTimer(Delay/1000, sim_timer_Elapsed);
        }

        void sim_timer_Elapsed()
        {
            lock (this.locker)
            {
                this.elapsed = true;
                this.CheckActivationChanged();
            }
        }

    }
}
