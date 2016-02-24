using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours.utterances
{
    class HelpIdlingUsers : PerformUtterance
    {
        public HelpIdlingUsers() : base(UtterancesMapping.GAMERULES_GENERAL)
        {
        }
    }
}
