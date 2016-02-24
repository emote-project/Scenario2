using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogicWebLib
{
    public class LogicFrame : IDisposable
    {
        public delegate void NewFrameEventHandler(List<InputNode> inputs);
        public event NewFrameEventHandler NewFrameEvent;

        public delegate void BehaviourExecutionCycleEndedEventHandler();
        public event BehaviourExecutionCycleEndedEventHandler BehaviourExecutionCycleEndedEvent;

        private object _locker = new object();

        private List<InputNode> _inputs = new List<InputNode>();
        private List<OutNode> _outputs = new List<OutNode>();

        private Dictionary<int, List<Behaviour>> _queuedBehavioursByFrameIndex = new Dictionary<int, List<Behaviour>>();

        public Dictionary<int, List<Behaviour>> Queue
        {
            get { return _queuedBehavioursByFrameIndex;}
        } 

        private int _frameIndex;

        public int FrameIndex
        {
            get { return _frameIndex; }
            set { _frameIndex = value; }
        }

        public List<InputNode> Inputs
        {
            get { return _inputs; }
            set { _inputs = value; }
        }

        public List<OutNode> Outputs
        {
            get { return _outputs; }
            set { _outputs = value; }
        }

        public void AddInput(InputNode inputNode)
        {
            _inputs.Add(inputNode);
            inputNode.StateChangedEvent += input_StateChangedEvent;
        }

        public void AddOutput(OutNode outNode)
        {
            _outputs.Add(outNode);
        }

        public OutNode AddOutput(string description)
        {
            var outNode = new OutNode(description);
            _outputs.Add(outNode);
            return outNode;
        }

        void input_StateChangedEvent(bool state)
        {
            Console.WriteLine("\n\n--------------------------- Frame: " + FrameIndex);
            if (NewFrameEvent!=null) NewFrameEvent(_inputs);
            CheckOutputs();
            QueueBehaviours();

            ExecuteBehaviours();

            LogFrame();
            LogQueue();
            FrameIndex++;
            Console.WriteLine("\n\n------------------------------------ ");
        }

        private void QueueBehaviours()
        {
            lock (_locker)
            {
                Console.WriteLine("\n------- Queuing behaviours:");
                foreach (var outNode in Outputs.Where(o => o.ShouldExecuteBehaviour))
                {
                    Behaviour behaviour = outNode.GenerateBehaviour();
                    if (!_queuedBehavioursByFrameIndex.ContainsKey(FrameIndex))
                    {
                        Console.WriteLine("\tAdding a frame to the queue. Index: " + FrameIndex);
                        _queuedBehavioursByFrameIndex.Add(FrameIndex, new List<Behaviour>());
                    }
                    _queuedBehavioursByFrameIndex[FrameIndex].Add(behaviour);
                    Console.WriteLine("\t\tAdding behaviour " + behaviour.GetType().Name + " to frame index " + FrameIndex);
                }
            }
        }


        private void ExecuteBehaviours()
        {
            Console.WriteLine("\n------- Executing behaviours:");
            lock (_locker)
            {
                if (_queuedBehavioursByFrameIndex.Count <= 0)
                {
                    Console.WriteLine("No behaviours in the queue");
                    return;
                }
                var firstFrameIndex = _queuedBehavioursByFrameIndex.Keys.OrderBy(x => x).First();
                var firstFrameBehaviours = _queuedBehavioursByFrameIndex[firstFrameIndex];
                var n = firstFrameBehaviours.RemoveAll(x => x.BehaviourState == Behaviour.BehaviourStateType.Executed);      // ####### REMOVING BEHAVIOURS
                Console.WriteLine("Removing " + n + " behaviours from first frame (n. " + firstFrameIndex + ")");

                if (firstFrameBehaviours.Any())
                    // If there is any behaviour still queued for the first frame queued, than execute it
                {
                    Console.WriteLine("Behaviours in last frame: " + firstFrameBehaviours.Count);
                    foreach (var behaviour in firstFrameBehaviours)
                    {
                        Console.WriteLine("Behaviours: " + behaviour.GetType().Name + " - " + behaviour.BehaviourState);
                        if (behaviour.BehaviourState == Behaviour.BehaviourStateType.Queued)
                        {
                            Console.WriteLine("===> Executing behaviour: " + behaviour.GetType().Name);
                            behaviour.Execute();
                            // Every time a behaviour ends it checks if there are more behaviour to execute and execute them
                            behaviour.BehaviourStateChangedEvent += delegate(Behaviour behaviour1)
                            {
                                ExecuteBehaviours();
                            };
                        }
                    }
                }
                else // Otherwise remove this frame and check the next one in queue (recursively calling this function)
                {
                    Console.WriteLine("No behaviours in last frame (" + firstFrameIndex + "). Removing it");
                    _queuedBehavioursByFrameIndex.Remove(firstFrameIndex);                               // ####### REMOVING FRAME
                    ExecuteBehaviours();
                }
            }
            if (BehaviourExecutionCycleEndedEvent != null) BehaviourExecutionCycleEndedEvent();
        }

        private void CheckOutputs()
        {
            foreach (var outNode in Outputs)
            {
                outNode.CheckState();
            }
        }


        private void LogFrame()
        {
            Console.WriteLine("Inputs:");
            foreach (var logicIn in _inputs)
            {
                Console.WriteLine("\t" + logicIn.Description + ": " + logicIn.Active);
            }
            Console.WriteLine("\nOutputs");
            foreach (var outNode in Outputs)
            {
                Console.WriteLine("\t" + outNode.Description + ": " + outNode.Active);
            }
        }

        private void LogQueue()
        {
            try
            {
                Console.WriteLine("\nQueue:");
                var tempQueue = new Dictionary<int, List<Behaviour>>(_queuedBehavioursByFrameIndex);
                // Avoids to fetch an edited list (this would fire an exception)
                foreach (var queuedBehaviour in tempQueue)
                {
                    Console.WriteLine(queuedBehaviour.Key + ": ");
                    foreach (var behaviour in queuedBehaviour.Value)
                    {
                        Console.WriteLine("\t" + behaviour.GetType().Name + ": " + behaviour.BehaviourState);
                    }
                }
                Console.WriteLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine("EXCEPTION: "+ex.Message);
            }
        }

        public static LogicFrame LoadFromFile(string filePath)
        {
            var DLL = Assembly.LoadFile(filePath);
            foreach (Type type in DLL.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(LogicFrame)))
                {
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { });
                    object instance = ctor.Invoke(new object[] { });
                    return (LogicFrame)instance;
                }
            }
            return null;
        }

        public virtual void Dispose()
        {
            lock (_locker)
            {
                foreach (var inputNode in Inputs)
                {
                    inputNode.StateChangedEvent -= input_StateChangedEvent;
                }
            }
        }
    }
}
