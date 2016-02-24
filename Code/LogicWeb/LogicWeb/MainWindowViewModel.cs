using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using LogicWeb.Annotations;
using LogicWebLib;

namespace LogicWeb
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<QueuedBehaviourVM> _queuedBehaviour = new ObservableCollection<QueuedBehaviourVM>();
        private ObservableCollection<NodeViewModel> nodesWeb = new ObservableCollection<NodeViewModel>();


        public ObservableCollection<QueuedBehaviourVM> QueuedBehaviour
        {
            get { return _queuedBehaviour; }
            set
            {
                _queuedBehaviour = value;
                OnPropertyChanged("QueuedBehaviour");
            }
        }

        public ObservableCollection<NodeViewModel> NodesWeb
        {
            get { return nodesWeb; }
            set
            {
                nodesWeb = value; 
                OnPropertyChanged("NodesWeb");
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class QueuedBehaviourVM : INotifyPropertyChanged
    {
        private int _frameIndex;
        private string _behaviourDescription = "";
        private Behaviour.BehaviourStateType _stateType;
        private long _behaviourID;

        public int FrameIndex
        {
            get { return _frameIndex; }
            set
            {
                _frameIndex = value;
                OnPropertyChanged("FrameIndex");
            }
        }

        public string BehaviourDescription
        {
            get { return _behaviourDescription; }
            set
            {
                _behaviourDescription = value;
                OnPropertyChanged("BehaviourDescription");
            }
        }

        public Behaviour.BehaviourStateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
                OnPropertyChanged("BehaviourStateString");
            }
        }

        public string BehaviourStateString
        {
            get { return StateType.ToString(); }
        }

        public long BehaviourId
        {
            get { return _behaviourID; }
            set
            {
                _behaviourID = value;
                OnPropertyChanged("BehaviourId");
            }
        }


        public QueuedBehaviourVM(Behaviour behaviour, int frameIndex)
        {
            BehaviourId = behaviour.Id;
            BehaviourDescription = behaviour.GetType().Name;
            StateType = behaviour.BehaviourState;
            FrameIndex = frameIndex;
            behaviour.BehaviourStateChangedEvent += behaviour_BehaviourStateChangedEvent;
        }

        void behaviour_BehaviourStateChangedEvent(Behaviour behaviour)
        {
            StateType = Behaviour.BehaviourStateType.Executed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NodeViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Node _node;
        
        
        private string _description;
        private List<NodeViewModel> _parents;
        private bool _active;


        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public List<NodeViewModel> Parents
        {
            get { return _parents; }
            set
            {
                _parents = value;
                OnPropertyChanged("Parents");
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                OnPropertyChanged("Active");
            }
        }

        public Node LogicNode
        {
            get { return _node; }
        }

        public NodeViewModel(Node node)
        {
            _node = node;
            Description = _node.ToString();
            _node.StateChangedEvent += _node_StateChangedEvent;
            Active = _node.Active;

            var logicNode = node as LogicNode;
            if (logicNode != null)
                PopulateChildren(logicNode);

            var outNode = node as OutNode;
            if (outNode != null)
            {
                outNode.BehaviourStateChangedEvent+= delegate(Behaviour behaviour)
                {
                    Description = _node.ToString() + " - " + behaviour.BehaviourState.ToString();
                };
            }
        }
        public void PopulateChildren(LogicNode node)
        {
            if (node.InputNodes.Any())
            {
                _parents = new List<NodeViewModel>(
                    (from inputNode in node.InputNodes
                        select new NodeViewModel(inputNode))
                        .ToList<NodeViewModel>());
            }
        }

        void _node_StateChangedEvent(bool state)
        {
            Active = state;
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _node.StateChangedEvent += _node_StateChangedEvent;
        }
    }
    
}
