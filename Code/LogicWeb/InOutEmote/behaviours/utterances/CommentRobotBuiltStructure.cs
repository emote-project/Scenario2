using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote.behaviours.utterances
{
    public class CommentRobotBuiltStructure : PerformUtterance
    {
        public CommentRobotBuiltStructure() : base(UtterancesMapping.CONFIRMCONSTRUCTION_SELF)
        {
        }
    }
}
