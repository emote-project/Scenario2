using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib
{
    public class OutputBehaviourExecutedInputNode : InputNode
    {
        private OutNode _outNode;

        public OutputBehaviourExecutedInputNode(OutNode outNode)
            : base("OutputBehaviourExecutedInputNode")
        {
            _outNode = outNode;
            if (outNode == null) throw new Exception("Can't watch a null behavior");
            _outNode.BehaviourStateChangedEvent += _outNode_BehaviourStateChangedEvent;
        }

        void _outNode_BehaviourStateChangedEvent(Behaviour behaviour)
        {
            Active = behaviour.BehaviourState == Behaviour.BehaviourStateType.Executed;
        }

    }
}
