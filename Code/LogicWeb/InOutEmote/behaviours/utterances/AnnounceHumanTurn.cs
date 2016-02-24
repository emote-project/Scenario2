using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceHumanTurn : PerformUtterance
    {
        public AnnounceHumanTurn() : base(UtterancesMapping.TURNCHANGED_OTHER)
        {
        }
    }
}
