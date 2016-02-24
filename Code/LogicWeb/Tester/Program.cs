using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InOutTestLib;
using InOutTestLib.INs;
using LogicWebLib;
using LogicWebLib.NodeTypes;

namespace Tester
{
    class Program
    {
        static LogicFrame _logicFrame = null;

        static void Main(string[] args)
        {

            _logicFrame = LogicFrame.LoadFromFile(@"C:\Users\Eugenio\Documents\EuxPersonal\LogicWeb\InOutTestLib\bin\Debug\InOutTestLib.dll");
            //_logicFrame = new InOutTestLogic();
            if (_logicFrame != null)
            {
                Console.WriteLine("Frame created with "+_logicFrame.Inputs.Count+" inputs");
                _logicFrame.NewFrameEvent += LogicFrameLogicFrameEvent;
            }
            else 
                Console.WriteLine("Frame == null");

            var nodes = new List<LogicNode>();
            InputNode fakeInRandomInput = null;
            InputNode fakeInStaticInput = null;
            foreach (var inputNode in _logicFrame.Inputs)
            {
                if (inputNode.GetType().Name.Equals(typeof(FakeInRandom).Name))
                {
                    fakeInRandomInput = inputNode;
                    break;
                }
            }
            foreach (var inputNode in _logicFrame.Inputs)
            {
                if (inputNode.GetType().Name.Equals(typeof(FakeInStatic).Name))
                {
                    fakeInStaticInput = inputNode;
                    break;
                }
            }

            var andNode = new AndLogicNode("AndTest", new List<Node>(_logicFrame.Inputs));
            nodes.Add(andNode);
            nodes.Add(new OrLogicNode("OrTest", new List<Node>(_logicFrame.Inputs)));
            nodes.Add(new NotLogicNode("NotTestForRandom", fakeInRandomInput));
            nodes.Add(new AndLogicNode("CompositeAnd", new List<Node>(){ andNode, fakeInRandomInput}));

            _logicFrame.Outputs[0].WatchedNode = andNode;

            LogicWeb logicWeb = new LogicWeb(_logicFrame,nodes);



            Console.ReadLine();
        }

        static void LogicFrameLogicFrameEvent(List<InputNode> inputs)
        {
            
        }
    }
}
