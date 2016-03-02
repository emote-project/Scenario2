using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thalamus;

namespace IntManInterface
{

    public interface IIntManInterfacePublisher : EmoteCommonMessages.IEmoteActions, EmoteMapReadingEvents.IMapActions,
        EmoteCommonMessages.IFMLSpeech, EmoteEnercitiesMessages.IEnercitiesTaskActions
    { }

    public class IntManInterfacePublisher : IIntManInterfacePublisher
    {
        // The Thalamus publisher
        dynamic publisher;
        public IntManInterfacePublisher(dynamic publisher)
        {
            Console.WriteLine("Constructing IntManInterfacePublisher");
            this.publisher = publisher;
            startThreads();
        }

        public void Reset()
        {
            publisher.Reset();
        }

        public void Start(int participantId, int participantId2, string participant1Name, string participant2Name)
        {
            publisher.Start(participantId, participantId2, participant1Name, participant2Name);
        }

        public void Stop()
        {
            publisher.Stop();
        }

        public void BlockUI()
        {
            publisher.BlockUI();
        }

        public void CompassHide()
        {
            publisher.CompassHide();
        }

        public void CompassHighlight()
        {
            publisher.CompassHighlight();
        }

        public void CompassHighlightDirection(string direction)
        {
            publisher.CompassHighlight();
        }

        public void CompassShow()
        {
            publisher.CompassShow();
        }

        public void DistanceHide()
        {
            publisher.DistanceHide();
        }

        public void DistanceHighlight()
        {
            publisher.DistanceHighlight();
        }

        public void DistanceShow()
        {
            publisher.DistanceShow();
        }

        public void DistanceToolEnd()
        {
            publisher.DistanceToolEnd();
        }

        public void DistanceToolReset()
        {
            publisher.DistanceToolReset();
        }

        public void DistanceToolStart()
        {
            publisher.DistanceToolStart();
        }

        public void GiveQuestionnaire(string name)
        {
            publisher.GiveQuestionnaire(name);
        }

        public void HighlightRightAnswer()
        {
            publisher.HighlightRightAnswer();
        }

        public void MapKeyHide()
        {
            publisher.MapKeyHide();
        }

        public void MapKeyHighlight()
        {
            publisher.MapKeyHighlight();
        }

        public void MapKeyShow()
        {
            publisher.MapKeyShow();
        }

        public void StartNextStep()
        {
            publisher.StartNextStep();
        }

        public void StartStep(int stepId)
        {
            publisher.StartStep(stepId);
        }

        public void ToolAction(string toolName, string toolAction)
        {
            publisher.ToolAction(toolName, toolAction);
        }

        public void UnBlockUI()
        {
            publisher.UnBlockUI();
        }

        public void CancelUtterance()
        {
            publisher.CancelUtterance();
        }

        public void PerformUtterance(string utterance, string category)
        {
            publisher.PerformUtterance(utterance, category);
        }

        public void CancelUtterance(string id)
        {
            publisher.CancelUtterance(id);
        }

        public void PerformUtterance(string id, string utterance, string category)
        {
            publisher.PerformUtterance(id, utterance, category);
        }

        public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
        {
            publisher.PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
        }

        public void ConfirmConstruction(EmoteEnercitiesMessages.StructureType structure, int cellX, int cellY)
        {
            Console.WriteLine("2: ConfirmConstruction(" + structure + "," + cellX + "," + cellY + ")");
            publisher.ConfirmConstruction(structure, cellX, cellY);
        }

        public void ImplementPolicy(EmoteEnercitiesMessages.PolicyType policy)
        {
            publisher.ImplementPolicy(policy);
        }

        public void PerformUpgrade(EmoteEnercitiesMessages.UpgradeType upgrade, int cellX, int cellY)
        {
            publisher.PerformUpgrade(upgrade, cellX, cellY);
        }

        public void PlayStrategy(EmoteEnercitiesMessages.EnercitiesStrategy strategy)
        {
            publisher.PlayStrategy(strategy);
        }

        public void SkipTurn()
        {
            publisher.SkipTurn();
        }

        // Listen for RPC messages from the Java side and re-publish them
        HttpListener JavaRpcListener;
        private Thread javaListenerThread;
        private Thread javaRequestDispatcherThread;
        bool serviceRunning = false;
        bool shuttingDown = false;
        private int JavaXmlRpcPort = 11100;
        // private String javaHost = "localhost";

        List<HttpListenerContext> javaHttpRequestsQueue = new List<HttpListenerContext>();

        public void startThreads()
        {
            javaListenerThread = new Thread(new ThreadStart(JavaHttpListenerThread));
            javaListenerThread.Start();

            javaRequestDispatcherThread = new Thread(new ThreadStart(JavaRequestDispatcher));
            javaRequestDispatcherThread.Start();
        }

        public void shutDown()
        {
            shuttingDown = true;
        }

        internal void JavaHttpListenerThread()
        {
            while (!shuttingDown)
            {
                while (!serviceRunning)
                {
                    try
                    {
                        Console.WriteLine("Starting service on port '" + JavaXmlRpcPort + "'");
                        JavaRpcListener = new HttpListener();
                        JavaRpcListener.Prefixes.Add(string.Format("http://*:{0}/", JavaXmlRpcPort));
                        JavaRpcListener.Start();
                        Console.WriteLine("XMLRPC Listening on port " + JavaXmlRpcPort);
                        serviceRunning = true;
                    }
                    catch
                    {
                        Console.WriteLine("Port unavaliable.");
                        serviceRunning = false;
                        Console.ReadLine();
                    }
                }

                try
                {
                    HttpListenerContext context = JavaRpcListener.GetContext();
                    lock (javaHttpRequestsQueue)
                    {
                        javaHttpRequestsQueue.Add(context);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    serviceRunning = false;
                    if (JavaRpcListener != null) JavaRpcListener.Close();
                }
            }
            if (JavaRpcListener != null) JavaRpcListener.Close();
            Console.WriteLine("Terminated JavaHttpListenerThread");
        }

        internal void JavaRequestDispatcher()
        {
            while (!shuttingDown)
            {
                bool performSleep = true;
                try
                {
                    if (javaHttpRequestsQueue.Count > 0)
                    {
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (javaHttpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(javaHttpRequestsQueue);
                            javaHttpRequestsQueue.Clear();
                        }
                        foreach (HttpListenerContext r in httpRequests)
                        {
                            ProcessJavaRequest(r);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Console.WriteLine("Terminated JavaRequestDispatcherThread");
        }

        XmlRpcListenerService svc = null;
        internal void ProcessJavaRequest(object oContext)
        {
            try
            {
                if (svc == null)
                {
                    svc = new JavaThalamusEventHandler(publisher);
                }
                svc.ProcessRequest((HttpListenerContext)oContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        internal class JavaThalamusEventHandler : XmlRpcListenerService
        {
            private dynamic publisher;

            public JavaThalamusEventHandler(dynamic publisher)
            {
                this.publisher = publisher;
            }

            [XmlRpcMethod()]
            public void Reset()
            {
                publisher.Reset();
            }

            //[XmlRpcMethod()]
            //public void Start(int participantId, int participantId2, string participant1Name, string participant2Name)
            //{
              //  publisher.Start(participantId, participantId2, participant1Name, participant2Name);
            //}

            [XmlRpcMethod()]
            public void Start(string StartMessageInfo_info)
            {
                publisher.Start(StartMessageInfo_info);
            }

            [XmlRpcMethod()]
            public void Stop()
            {
                publisher.Stop();
            }

            [XmlRpcMethod()]
            public void BlockUI()
            {
                publisher.BlockUI();
            }

            [XmlRpcMethod()]
            public void CompassHide()
            {
                publisher.CompassHide();
            }

            [XmlRpcMethod()]
            public void CompassHighlight()
            {
                publisher.CompassHighlight();
            }

            [XmlRpcMethod()]
            public void CompassHighlightDirection(string direction)
            {
                publisher.CompassHighlight();
            }

            [XmlRpcMethod()]
            public void CompassShow()
            {
                publisher.CompassShow();
            }

            [XmlRpcMethod()]
            public void DistanceHide()
            {
                publisher.DistanceHide();
            }

            [XmlRpcMethod()]
            public void DistanceHighlight()
            {
                publisher.DistanceHighlight();
            }

            [XmlRpcMethod()]
            public void DistanceShow()
            {
                publisher.DistanceShow();
            }

            [XmlRpcMethod()]
            public void DistanceToolEnd()
            {
                publisher.DistanceToolEnd();
            }

            [XmlRpcMethod()]
            public void DistanceToolReset()
            {
                publisher.DistanceToolReset();
            }

            [XmlRpcMethod()]
            public void DistanceToolStart()
            {
                publisher.DistanceToolStart();
            }

            [XmlRpcMethod()]
            public void GiveQuestionnaire(string name)
            {
                publisher.GiveQuestionnaire(name);
            }

            [XmlRpcMethod()]
            public void HighlightRightAnswer()
            {
                publisher.HighlightRightAnswer();
            }

            [XmlRpcMethod()]
            public void MapKeyHide()
            {
                publisher.MapKeyHide();
            }

            [XmlRpcMethod()]
            public void MapKeyHighlight()
            {
                publisher.MapKeyHighlight();
            }

            [XmlRpcMethod()]
            public void MapKeyShow()
            {
                publisher.MapKeyShow();
            }

            [XmlRpcMethod()]
            public void StartNextStep()
            {
                publisher.StartNextStep();
            }

            [XmlRpcMethod()]
            public void StartStep(int stepId)
            {
                publisher.StartStep(stepId);
            }

            [XmlRpcMethod()]
            public void ToolAction(string toolName, string toolAction)
            {
                publisher.ToolAction(toolName, toolAction);
            }

            [XmlRpcMethod()]
            public void UnBlockUI()
            {
                publisher.UnBlockUI();
            }

            [XmlRpcMethod()]
            public void CancelUtterance(string id)
            {
                publisher.CancelUtterance(id);
            }

            [XmlRpcMethod()]
            public void PerformUtterance(string id, string utterance, string category)
            {
                Console.WriteLine("PerformUtterance('" + id + "', '" + utterance + "', '" + category + "')");
                publisher.PerformUtterance(id, utterance, category);
            }

            [XmlRpcMethod()]
            public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
            {
                Console.WriteLine("PerformUtterance('" + id + "', '" + category + "', '" + subcategory + "')");
                int j = 0;
                foreach (String t in tagNames)
                {
                    Console.WriteLine("Tag: " + t + "," + tagValues[j]);
                    j++;
                }
                publisher.PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
            }

            [XmlRpcMethod()]
            public void PlayStrategy(String strategy)
            {
                publisher.PlayStrategy(Enum.Parse(typeof(EmoteEnercitiesMessages.EnercitiesStrategy), strategy));
            }

            [XmlRpcMethod()]
            public void ConfirmConstruction(String structure, int cellX, int cellY)
            {
                if (structure.StartsWith("performUpgrade"))
                {
                    String[] st = structure.Split(':');
                    String upgrade = st[1];
                    publisher.PerformUpgrade(Enum.Parse(typeof(EmoteEnercitiesMessages.UpgradeType), upgrade), cellX, cellY);
                }
                else if (structure.StartsWith("implementPolicy"))
                {
                    String[] st = structure.Split(':');
                    String policy = st[1];
                    publisher.ImplementPolicy(Enum.Parse(typeof(EmoteEnercitiesMessages.PolicyType), policy));
                }
                else if (structure.Equals("skipTurn"))
                {
                    //Using this because skipturn is not being called directly from the java IM for some reason.. so i am calling
                    //confirmconstruction with skipturn as parameter as a hack.. :)
                    publisher.SkipTurn();
                }
                else
                {
                    EmoteEnercitiesMessages.StructureType st = (EmoteEnercitiesMessages.StructureType)Enum.Parse(typeof(EmoteEnercitiesMessages.StructureType), structure);
                    Console.WriteLine("1: ConfirmConstruction(" + st + "," + cellX + "," + cellY + ")");
                    publisher.ConfirmConstruction(st, cellX, cellY);
                }
            }

            [XmlRpcMethod()]
            void ImplementPolicy(String policy)
            {
                publisher.ImplementPolicy(Enum.Parse(typeof(EmoteEnercitiesMessages.PolicyType), policy));
            }

            [XmlRpcMethod()]
            void PerformUpgrade(String upgrade, int cellX, int cellY)
            {
                publisher.PerformUpgrade(Enum.Parse(typeof(EmoteEnercitiesMessages.UpgradeType), upgrade), cellX, cellY);
            }

            [XmlRpcMethod()]
            void SkipTurn()
            {
                Console.WriteLine("Skipping turn");
                publisher.SkipTurn();
            }

        }


        void EmoteCommonMessages.IEmoteActions.Reset()
        {
            throw new NotImplementedException();
        }

        void EmoteCommonMessages.IEmoteActions.SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            throw new NotImplementedException();
        }

        void EmoteCommonMessages.IEmoteActions.Stop()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.BlockUI()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.CompassHide()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.CompassHighlight()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.CompassHighlightDirection(string direction)
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.CompassShow()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceHide()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceHighlight()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceShow()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceToolEnd()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceToolReset()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.DistanceToolStart()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.GiveQuestionnaire(string name)
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.HighlightRightAnswer()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.MapKeyHide()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.MapKeyHighlight()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.MapKeyShow()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.StartNextStep()
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.StartStep(int stepId)
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.ToolAction(string toolName, string toolAction)
        {
            throw new NotImplementedException();
        }

        void EmoteMapReadingEvents.IMapActions.UnBlockUI()
        {
            throw new NotImplementedException();
        }

        void EmoteCommonMessages.IFMLSpeech.CancelUtterance(string id)
        {
            throw new NotImplementedException();
        }

        void EmoteCommonMessages.IFMLSpeech.PerformUtterance(string id, string utterance, string category)
        {
            throw new NotImplementedException();
        }

        void EmoteCommonMessages.IFMLSpeech.PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
        {
            throw new NotImplementedException();
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskActions.ConfirmConstruction(EmoteEnercitiesMessages.StructureType structure, int cellX, int cellY)
        {
            throw new NotImplementedException();
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskActions.ImplementPolicy(EmoteEnercitiesMessages.PolicyType policy)
        {
            throw new NotImplementedException();
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskActions.PerformUpgrade(EmoteEnercitiesMessages.UpgradeType upgrade, int cellX, int cellY)
        {
            throw new NotImplementedException();
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskActions.PlayStrategy(EmoteEnercitiesMessages.EnercitiesStrategy strategy)
        {
            throw new NotImplementedException();
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskActions.SkipTurn()
        {
            throw new NotImplementedException();
        }


        void EmoteCommonMessages.IEmoteActions.Start(string StartMessageInfo_info)
        {
            throw new NotImplementedException();
        }
    }
}

