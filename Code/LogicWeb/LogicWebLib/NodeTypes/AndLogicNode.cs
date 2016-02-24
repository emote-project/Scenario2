using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib.NodeTypes
{
    /// <summary>
    /// This nodes will be active when all its inputs are active
    /// </summary>
    public class AndLogicNode : LogicNode
    {
        public AndLogicNode(string description, List<Node> inputNodes) : base(description, inputNodes)
        {
        }

        public override bool CheckState()
        {
            lock (_locker)
            {
                var ret = true;
                foreach (var inputNode in InputNodes)
                {
                    ret = ret && inputNode.Active;
                }
                Active = ret;
                return ret;
            }
        }

        public override string ToString()
        {
            return "AND | "+base.ToString();
        }
    }
}
