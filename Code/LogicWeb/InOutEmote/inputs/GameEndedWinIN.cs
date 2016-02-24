using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class GameEndedWinIN : InputNode
    {
        private InOutThalamusClient _client;

        public GameEndedWinIN() : base("EndGameWin")
        {
            _client = InOutThalamusClient.GetInstance();

            _client.EndGameSuccessfullEvent += _client_EndGameSuccessfullEvent;
        }

        void _client_EndGameSuccessfullEvent(object sender, EndGameEventArgs e)
        {
            Active = true;
        }
    }
}
