using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;
using InOutEmote.Thalamus;
using LogicWebLib;
using Thalamus;

namespace InOutEmote.inputs
{
    public class EcoTurnIN : InputNode
    {
        private InOutThalamusClient _client;

        public EcoTurnIN() : base("EcoTurnIN")
        {
            _client = InOutThalamusClient.GetInstance();
            _client.TurnChangedEvent += _client_TurnChangedEvent;
        }

        void _client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            if (e.GameState.CurrentRole == EnercitiesRole.Economist)
                Active = true;
            else
                Active = false;
        }
    }
}
