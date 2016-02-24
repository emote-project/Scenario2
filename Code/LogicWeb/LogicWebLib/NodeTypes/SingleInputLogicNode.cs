using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib.NodeTypes
{
    /// <summary>
    /// This node will be active when its input is active
    /// </summary>
    public class SingleInputLogicNode : LogicNode
    {
        public SingleInputLogicNode(string description, Node watchedNode)
            : base(description, new List<Node>() { watchedNode })
        {
        }

        public override bool CheckState()
        {
            var ret = InputNodes[0].Active;
            Active = ret;
            return ret;
        }
    }
}
