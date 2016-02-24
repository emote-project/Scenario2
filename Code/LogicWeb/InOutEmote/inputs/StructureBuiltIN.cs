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
    public class StructureBuiltIN : InputNode
    {
        public StructureBuiltIN() : base("StructureBuilt")
        {
            var client = InOutThalamusClient.GetInstance();

            client.ConfirmConstructionEvent += client_ConfirmConstructionEvent;
        }

        void client_ConfirmConstructionEvent(object sender, GameActionEventArgs e)
        {
            Active = true;
            Active = false;
        }
    }
}
