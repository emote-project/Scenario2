using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class GameEndedTimeUpIN : InputNode
    {
        private InOutThalamusClient _client;

        public GameEndedTimeUpIN() : base("GameEmdedTimeUp")
        {
            _client = InOutThalamusClient.GetInstance();

            _client.EndGameTimeOutEvent += _client_EndGameTimeOutEvent;
        }

        void _client_EndGameTimeOutEvent(object sender, EndGameEventArgs e)
        {
            Active = true;
        }
    }
}
