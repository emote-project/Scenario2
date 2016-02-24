using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib.NodeTypes
{
    /// <summary>
    /// This node will be active when its input is NOT active
    /// </summary>
    public class NotLogicNode : LogicNode
    {
        public NotLogicNode(string description, Node inputNode) : base(description, new List<Node>(){inputNode})
        {
        }

        public override bool CheckState()
        {
            var ret = !InputNodes[0].Active;
            Active = ret;
            return ret;
        }

        public override string ToString()
        {
            return "NOT | " + base.ToString();
        }
    }
}
