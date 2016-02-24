using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEnercitiesMessages;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class FirstSkipDoneByRobotIN : InputNode
    {
        public FirstSkipDoneByRobotIN() : base("FirstSkipDoneByRobot")
        {
            var client = InOutThalamusClient.GetInstance();

            client.SkipTurnEvent += client_SkipTurnEvent;
        }

        void client_SkipTurnEvent(object sender, GameActionEventArgs e)
        {
            if (Active) return; // never goes back to false
            Active = GameState.GetInstance().CurrentState.CurrentPlayer.Role == EnercitiesRole.Mayor;
        }
    }
}
