using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicWebLib
{

    /// <summary>
    /// A generic node consisting of a state (active/not active) and a textual description
    /// </summary>
    public abstract class Node 
    {
        public delegate void StateChangedEventHandler(bool state);
        public event StateChangedEventHandler StateChangedEvent;

        protected bool _active;
        private string _description;

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                if (StateChangedEvent != null) StateChangedEvent(value);
            }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        protected Node(string description = null)
        {
            _description = description;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Description)?this.GetType().Name.ToString():Description;
        }
    }
}
