using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class PerformOwnConstructionTutorial : PerformUtterance
    {
        public PerformOwnConstructionTutorial() : base(UtterancesMapping.TUTORIAL_OWNCONSTRUCTION)
        {
        }
    }
}
