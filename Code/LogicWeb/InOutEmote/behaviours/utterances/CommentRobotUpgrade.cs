using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InOutEmote.behaviours.utterances
{
    public class CommentRobotUpgrade : PerformUtterance
    {
        public CommentRobotUpgrade() : base(UtterancesMapping.PERFORMUPGRADE_SELF)
        {
        }
    }
}
