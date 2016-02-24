using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class PolicyImplementedIN : InputNode
    {
        public PolicyImplementedIN() : base("PolicyImplementedIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.ImplementPolicyEvent += client_ImplementPolicyEvent;
        }

        void client_ImplementPolicyEvent(object sender, GameActionEventArgs e)
        {
            Active = true;
            Active = false;
        }
    }
}
