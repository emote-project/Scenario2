using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LogicWebLib
{
    /// <summary>
    /// The web of logic nodes that read the inputs from a frame, elaborate it and set the output of the frame
    /// </summary>
    public class LogicWeb
    {
        private  object _locker = new object();

        private LogicFrame _logicFrame;
        private List<LogicNode> _web;

        public LogicWeb(LogicFrame logicFrame, List<LogicNode> web)
        {
            _logicFrame = logicFrame;
            _web = web;

            _logicFrame.NewFrameEvent += _logicFrame_NewFrameEvent;
        }

        public List<LogicNode> Web
        {
            get { return _web; }
        }

        public LogicFrame Frame
        {
            get { return _logicFrame; }
        }

        void _logicFrame_NewFrameEvent(List<InputNode> inputs)
        {
            lock (_locker)
            {
                UpdateWeb();
            }
        }

        /// <summary>
        /// Analyze the web of nodes by depth. 
        /// It starts with the nodes having depth = 1, which are those linked directly to an InputNode. 
        /// After that continues increasing depth. In this way nodes that depends on other nodes are checked later, after the 
        /// nodes on which they depends on are updated.
        /// </summary>
        public void UpdateWeb()
        {
            int depth = 1;
            IEnumerable<LogicNode> nodes;
            Console.WriteLine("\n####### Updating Logic Web");
            do
            {
                nodes = _web.Where(x => x.Depth == depth);
                Console.WriteLine("Depth: " + depth);
                foreach (var logicNode in nodes)
                {
                    Console.Write(logicNode.Description + ": " + logicNode.Active + "\t");
                    logicNode.CheckState();
                }
                Console.WriteLine();
                depth++;
            } while (nodes.Any());
            Console.WriteLine("\n###########################");
            Console.WriteLine();
            Console.WriteLine();
        }


        #region Serialization

        public void Save(string path)
        {
            var textWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            File.WriteAllText(path,textWriter.ToString());
        }

        #endregion

    }
}
