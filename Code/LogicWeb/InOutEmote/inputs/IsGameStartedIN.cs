using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class IsGameStartedIN : InputNode
    {
        public IsGameStartedIN() : base("IsGameStarted")
        {
            var client = InOutThalamusClient.GetInstance();

            client.GameStartedEvent += client_GameStartedEvent;
        }

        void client_GameStartedEvent(object sender, GenericGameEventArgs e)
        {
            Active = true;
        }
    }
}
