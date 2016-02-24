using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class UsersIdlingIN : InputNode
    {
        private const int IDLE_TIMEOUT = 18000;
        private Timer _idleTimer;

        public UsersIdlingIN() : base("UsersIdlingIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.TurnChangedEvent += client_TurnChangedEvent;
            client.BuildMenuTooltipShowedEvent += client_BuildMenuTooltipShowedEvent;
            client.BuildingMenuToolSelectedEvent += client_BuildingMenuToolSelectedEvent;
            client.PoliciesMenuShowedEvent += client_PoliciesMenuShowedEvent;
            client.UpgradesMenuShowedEvent += client_UpgradesMenuShowedEvent;
            client.UtteranceStartedEvent += client_UtteranceStartedEvent;
            client.UtteranceFinishedEvent += client_UtteranceFinishedEvent;
            client.StartEvent += client_StartEvent;

            _idleTimer = new Timer(IDLE_TIMEOUT);
            _idleTimer.AutoReset = false;
            _idleTimer.Start();
            _idleTimer.Elapsed += _idleTimer_Elapsed;
        }

        void client_UtteranceFinishedEvent(object sender, IFMLUtteranceEventArgs e)
        {
            _idleTimer.Interval = IDLE_TIMEOUT;
        }

        void client_UtteranceStartedEvent(object sender, IFMLUtteranceEventArgs e)
        {
            _idleTimer.Interval = 999999;
        }


        private void NotIdling()
        {
            _idleTimer.Stop();
            _idleTimer.Start();
            Active = false;
            Console.WriteLine("NOT IDLING");
        }

        private void Idling()
        {
            Active = true;
            Console.WriteLine("IDLING");
        }

        void client_SpeakBookmarksEvent(object sender, SpeechEventArgs e)
        {
            NotIdling();
        }
        
        void _idleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Idling();
        }

        void client_UpgradesMenuShowedEvent(object sender, EventArgs e)
        {
            NotIdling();
        }

        void client_PoliciesMenuShowedEvent(object sender, EventArgs e)
        {
            NotIdling();
        }

        void client_BuildingMenuToolSelectedEvent(object sender, MenuEventArgs e)
        {
            NotIdling();
        }

        void client_BuildMenuTooltipShowedEvent(object sender, MenuEventArgs e)
        {
            NotIdling();
        }

        void client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            NotIdling();
        }

        void client_StartEvent(object sender, StartEventArgs e)
        {
            NotIdling();
        }
    }
}
