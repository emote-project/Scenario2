using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib.NodeTypes
{
    /// <summary>
    /// This node will be active when at least one of its input is active
    /// </summary>
    public class OrLogicNode : LogicNode
    {
        public OrLogicNode(string description, List<Node> inputNodes) : base(description, inputNodes)
        {
        }

        public override bool CheckState()
        {
            lock (_locker)
            {
                var ret = false;
                foreach (var inputNode in InputNodes)
                {
                    ret = ret || inputNode.Active;
                }
                Active = ret;
                return ret;
            }
        }

        public override string ToString()
        {
            return "OR | " + base.ToString();
        }
    }
}
