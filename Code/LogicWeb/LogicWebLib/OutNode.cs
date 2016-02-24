using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LogicWebLib.NodeTypes;

namespace LogicWebLib
{
    /// <summary>
    /// A specialized Node which state depends on the state of another node. When the state of the watched node changes, this 
    /// output node fires a OnStateChanged event which implementation is left to the derived class.
    /// </summary>
    public class OutNode : LogicNode
    {
        public InputNode.StateChangedEventHandler StateChangedEvent;
        public event Behaviour.BehaviourStateChangedEventHandler BehaviourStateChangedEvent;

        private Type _behaviourType;
        public Type BehaviourType
        {
            get { return _behaviourType; }
            set { _behaviourType = value; }
        }

        public bool _shouldExecuteBehaviour;
        public bool ShouldExecuteBehaviour
        {
            get
            {
                lock (_locker)
                {
                    return _shouldExecuteBehaviour;
                }
            }
            set { _shouldExecuteBehaviour = value; }
        }


        public Node WatchedNode
        {
            get { return InputNodes.Count>0?InputNodes[0]:null; }
            set
            {
                if (value==null) throw new Exception("Cannot add a NULL node");
                InputNodes.Clear();
                InputNodes.Add(value);
                Depth = GetDepth();
            }
        }

        public Behaviour _behaviour;

        
        public OutNode(string description) : base(description,new List<Node>(){})
        {
        }

        override public bool CheckState()
        {
            lock (_locker)
            {
                ShouldExecuteBehaviour = false;
                if (WatchedNode == null) return false;
                var oldState = Active;
                Active = WatchedNode.Active;
                if (oldState != Active)
                {
                    if (StateChangedEvent != null) StateChangedEvent(Active);
                    if (Active) _shouldExecuteBehaviour = true;
                }
                return Active;
            }

        }

        public Behaviour GenerateBehaviour()
        {
            ConstructorInfo ctor = BehaviourType.GetConstructor(new Type[] { });
            if (_behaviour != null) _behaviour.BehaviourStateChangedEvent += _behaviour_BehaviourStateChangedEvent;
            _behaviour = (Behaviour)ctor.Invoke(new object[] { });
            _behaviour.BehaviourStateChangedEvent += _behaviour_BehaviourStateChangedEvent;

            if (BehaviourStateChangedEvent != null) BehaviourStateChangedEvent(_behaviour);     // Calling the event to update the state to "queued"
            return _behaviour;
        }

        void _behaviour_BehaviourStateChangedEvent(Behaviour behaviour)
        {
            if (BehaviourStateChangedEvent != null) BehaviourStateChangedEvent(behaviour);
        }


    }
}
