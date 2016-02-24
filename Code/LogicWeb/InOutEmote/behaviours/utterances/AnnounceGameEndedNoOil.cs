using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceGameEndedNoOil : PerformUtterance
    {
        public AnnounceGameEndedNoOil() : base(UtterancesMapping.GAMEENDED_NOOIL)
        {
        }
    }
}
