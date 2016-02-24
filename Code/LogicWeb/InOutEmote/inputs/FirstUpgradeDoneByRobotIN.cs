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
    public class FirstUpgradeDoneByRobotIN :InputNode
    {
        public FirstUpgradeDoneByRobotIN() : base("FirstUpgradeDoneByRobotIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.PerformUpgradeEvent += client_PerformUpgradeEvent;
        }

        void client_PerformUpgradeEvent(object sender, GameActionEventArgs e)
        {
            if (Active) return;
            Active = GameState.GetInstance().CurrentState.CurrentPlayer.Role == EnercitiesRole.Mayor;
        }
    }
}
