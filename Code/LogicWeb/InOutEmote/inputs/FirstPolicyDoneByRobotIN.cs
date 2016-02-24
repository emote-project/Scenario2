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
    public class FirstPolicyDoneByRobotIN : InputNode
    {
        public FirstPolicyDoneByRobotIN() : base("FirstPolicyDoneByRobot")
        {
            var client = InOutThalamusClient.GetInstance();

            client.ImplementPolicyEvent += client_ImplementPolicyEvent;
        }

        void client_ImplementPolicyEvent(object sender, GameActionEventArgs e)
        {
            if (Active) return; // never goes back to false
            Active = GameState.GetInstance().CurrentState.CurrentPlayer.Role == EnercitiesRole.Mayor;
        }
    }
}
