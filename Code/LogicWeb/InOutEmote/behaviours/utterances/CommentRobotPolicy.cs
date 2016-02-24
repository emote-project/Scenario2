using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class CommentRobotPolicy : PerformUtterance
    {
        public CommentRobotPolicy() : base(UtterancesMapping.IMPLEMENTPOLICY_SELF)
        {
        }
    }
}
