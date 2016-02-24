using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutTestLib.Thalamus;
using LogicWebLib;

namespace InOutTestLib.INs.ThalamusINs
{
    public class MessageOneIN : InputNode
    {
        public MessageOneIN() : base("MessageOneIn")
        {
            var client = InOutThalamusClient.GetInstance();
            client.TestMessageOneEvent += client_TestMessageOneEvent;
        }

        void client_TestMessageOneEvent(object sender, EventArgs e)
        {
            this.Active = true;
            this.Active = false;
        }
    }
}
