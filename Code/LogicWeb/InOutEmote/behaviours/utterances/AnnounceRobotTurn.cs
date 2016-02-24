using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceRobotTurn : PerformUtterance
    {
        public AnnounceRobotTurn() : base(UtterancesMapping.TURNCHANGED_SELF)
        {
        }
    }
}
