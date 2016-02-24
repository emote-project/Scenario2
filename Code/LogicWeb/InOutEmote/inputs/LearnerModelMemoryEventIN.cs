using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEvents;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class LearnerModelMemoryEventIN : InputNode
    {
        private MemoryEvent.MemoryEventItem _latestMemoryEvent;

        public LearnerModelMemoryEventIN() : base("LearnerModelMemoryEventIN")
        {
            var client = InOutThalamusClient.GetInstance();

            client.LearnerModelMemoryEvent += client_LearnerModelMemoryEvent;
        }

        void client_LearnerModelMemoryEvent(object sender, LearnerModelMemoryEventArgs e)
        {
            if (_latestMemoryEvent!=null &&
                e.MemoryEvent.memoryEventItems[0].category.Equals(_latestMemoryEvent.category) &&               // AVOIDS DUPLICATED ITEMS
                e.MemoryEvent.memoryEventItems[0].subcategory.Equals(_latestMemoryEvent.subcategory))
                return;

            _latestMemoryEvent = e.MemoryEvent.memoryEventItems[0];
            Active = true;
            Active = false;
        }
    }
}
