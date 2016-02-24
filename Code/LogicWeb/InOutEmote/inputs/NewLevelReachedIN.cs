using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.inputs
{
    public class NewLevelReachedIN : InputNode
    {
        private InOutThalamusClient _client;

        public NewLevelReachedIN() : base("NewLevelReachedIN")
        {
            _client = InOutThalamusClient.GetInstance();

            _client.ReachedNewLevelEvent += _client_ReachedNewLevelEvent;
        }

        void _client_ReachedNewLevelEvent(object sender, ReachedNewLevelEventArgs e)
        {
            Active = true;
            Active = false;
        }
    }
}
