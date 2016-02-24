using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceGameEndedTimeUp : PerformUtterance
    {
        public AnnounceGameEndedTimeUp() : base(UtterancesMapping.GAMEENDED_TIMEUP)
        {
        }
    }
}
