using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class IsFirstSessionIN : InputNode
    {
        public IsFirstSessionIN() : base("IsFirstSessionIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.StartEvent += client_StartEvent;
        }

        void client_StartEvent(object sender, StartEventArgs e)
        {
            Active = e.StartMessageInfo.SessionId == 1;
        }
    }
}
