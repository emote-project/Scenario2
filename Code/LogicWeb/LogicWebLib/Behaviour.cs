using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicWebLib
{
    public abstract class Behaviour
    {
        public delegate void BehaviourStateChangedEventHandler(Behaviour behaviour);
        public event BehaviourStateChangedEventHandler BehaviourStateChangedEvent;

        public enum BehaviourStateType
        {
            Queued,
            Executing,
            Executed
        }

        private static long _nextId = 0;

        private long _id;
        public long Id
        {
            get { return _id; }
        }

        private BehaviourStateType _behaviourState;
        public BehaviourStateType BehaviourState
        {
            get { return _behaviourState; }
            set
            {
                _behaviourState = value;
                if (BehaviourStateChangedEvent != null) BehaviourStateChangedEvent(this);                
            }
        }


        protected Behaviour()
        {
            _id = _nextId++;
        }


        public async virtual void Execute()
        {
            BehaviourState = BehaviourStateType.Executing;

            await Task.Run(() =>
            {
                BehaviourTask();
            });
            //ExecutionEnded();
        }

        public virtual void ExecutionEnded()
        {
            BehaviourState = BehaviourStateType.Executed;
        }

        public abstract void BehaviourTask();

    }

}
