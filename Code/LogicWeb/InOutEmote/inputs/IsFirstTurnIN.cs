using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class IsFirstTurnIN : InputNode
    {
        public IsFirstTurnIN() : base("IsFirstTurn")
        {
            var client = InOutThalamusClient.GetInstance();

            client.TurnChangedEvent += client_TurnChangedEvent;
        }

        void client_TurnChangedEvent(object sender, GenericGameEventArgs e)
        {
            if (GameState.GetInstance().CurrentState.TurnNumber == 1)
            {
                Active = true;
            }
            else
            {
                Active = false;
            }
        }
    }
}
