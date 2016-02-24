using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib
{
    /// <summary>
    /// A node which state depends on the state of one or more other nodes
    /// </summary>
    public abstract class LogicNode : Node
    {
        protected object _locker = new object();
        private List<Node> _inputNodes = null;
        private int _depth = 0;
        public List<Node> InputNodes
        {
            get { return _inputNodes; }
        }
        public int Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        protected LogicNode(string description, List<Node> inputNodes)
            : base(description)
        {
            if (inputNodes==null) throw  new Exception("Null node received");
            _inputNodes = inputNodes;
            Depth = GetDepth();
        }

        public abstract bool CheckState();

        protected int GetDepth()
        {
            int depth = 0;
            foreach (var watchedNode in InputNodes)
            {
                int watchedNodeDepth = 0;
                if (watchedNode is LogicNode)
                {
                    watchedNodeDepth =((LogicNode) watchedNode).Depth + 1;
                }
                if (watchedNode is InputNode)
                {
                    watchedNodeDepth = 1;
                }
                if (watchedNodeDepth > depth) depth = watchedNodeDepth;
            }
            //if (depth == 0) throw new Exception("A node can't have depth = 0!");
            return depth;
        }
    }
}
