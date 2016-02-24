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
    public class ActionPlannedByAiForMayorIN : InputNode
    {
        public ActionPlannedByAiForMayorIN() : base("ActionPlannedByAiForMayorIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.ActionsPlannedEvent += client_ActionsPlannedEvent;
        }

        void client_ActionsPlannedEvent(object sender, IAEventArgs e)
        {
            if (e.CurrentPlayer == EnercitiesRole.Mayor)
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
