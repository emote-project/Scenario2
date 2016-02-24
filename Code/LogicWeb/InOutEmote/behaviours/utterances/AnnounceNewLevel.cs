using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class AnnounceNewLevel : PerformUtterance
    {
        public AnnounceNewLevel() : base(UtterancesMapping.GAMERULES_LEVELUP)
        {
        }
    }
}
