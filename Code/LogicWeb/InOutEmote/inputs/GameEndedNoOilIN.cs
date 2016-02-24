using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class GameEndedNoOilIN : InputNode
    {
        private InOutThalamusClient _client;

        public GameEndedNoOilIN() : base("GameEndedNoOil")
        {
            _client = InOutThalamusClient.GetInstance();

            _client.EndGameNoOilEvent += _client_EndGameNoOilEvent;
        }

        void _client_EndGameNoOilEvent(object sender, EndGameEventArgs e)
        {
            Active = true;
        }
    }
}
