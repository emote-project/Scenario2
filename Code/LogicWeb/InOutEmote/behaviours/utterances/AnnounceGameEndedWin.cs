using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceGameEndedWin : PerformUtterance
    {
        public AnnounceGameEndedWin() : base(UtterancesMapping.GAMEENDED_WIN)
        {
        }
    }
}
