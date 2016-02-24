using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class StructureUpgradedIN : InputNode
    {
        public StructureUpgradedIN() : base("StructureUpgradedIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.PerformUpgradeEvent += client_PerformUpgradeEvent;
        }

        void client_PerformUpgradeEvent(object sender, GameActionEventArgs e)
        {
            Active = true;
            Active = false;
        }
    }
}
