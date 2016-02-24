
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
using EmoteEvents;
using EmoteCommonMessages;
using EmoteEnercitiesMessages;

namespace LearnerModelThalamus
{
    public interface ILearnerModel : EmoteMapReadingEvents.IGameStateEvents, EmoteMapReadingEvents.IMapEvents, EmoteMapReadingEvents.ITaskEvents, EmoteCommonMessages.IMapEvents, EmoteCommonMessages.IEmoteActions, EmoteCommonMessages.IAffectPerceptionEvents, EmoteCommonMessages.ILearnerModelActions, EmoteCommonMessages.ILearnerModelIdActions, EmoteEnercitiesMessages.IEnercitiesAIPerceptions, EmoteEnercitiesMessages.IEnercitiesExamineEvents, EmoteEnercitiesMessages.IEnercitiesGameStateEvents, EmoteEnercitiesMessages.IEnercitiesTaskEvents, EmoteCommonMessages.IFMLSpeech, EmoteCommonMessages.IFMLSpeechEvents, EmoteCommonMessages.IFMLSpeechEventsExtras, EmoteCommonMessages.ILearnerModelUtteranceHistoryAction, EmoteEnercitiesMessages.IEnercitiesGameStateActions, EmoteCommonMessages.IEmotionalClimate
    { } //subscribing

    public interface ILearnerModelPublisher : IThalamusPublisher, EmoteCommonMessages.ILearnerModelToIMEvents, EmoteCommonMessages.ILearnerModelToAffectPerceptionEvents, EmoteCommonMessages.ILearnerModelIdEvents, EmoteCommonMessages.ILearnerModelToS2IMPerceptionEvents, EmoteCommonMessages.ILearnerModelMemoryEvents
    { } //publishing

    public interface IJavaInterface : EmoteCommonMessages.ILearnerModelToIMEvents, EmoteCommonMessages.ILearnerModelToAffectPerceptionEvents, EmoteCommonMessages.ILearnerModelIdEvents, EmoteCommonMessages.ILearnerModelToS2IMPerceptionEvents, EmoteCommonMessages.ILearnerModelMemoryEvents
    { } //publishing

    public class LearnerModelClient: ThalamusClient, ILearnerModel
    {
        internal class LearnerModelPublisher : ILearnerModelPublisher
        {
            dynamic publisher;
            public LearnerModelPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }


            public void learnerModelValueUpdateAfterAffectPerceptionUpdate(string LearnerStateInfo_learnerState, string AffectPerceptionInfo_AffectiveStates)
            {
                
                publisher.learnerModelValueUpdateAfterAffectPerceptionUpdate(LearnerStateInfo_learnerState , AffectPerceptionInfo_AffectiveStates);   
                
            }

            public void learnerModelValueUpdateBeforeAffectPerceptionUpdate(string LearnerStateInfo_learnerState)
            {
                publisher.learnerModelValueUpdateBeforeAffectPerceptionUpdate(LearnerStateInfo_learnerState);
            }



            public void nextThalamusId(int participantId)
            {
                publisher.nextThalamusId(participantId); 
            }

            public void allLearnerInfo(string[] LearnerInfo_learnerInfos)
            {   
                

          

                publisher.allLearnerInfo(LearnerInfo_learnerInfos); 
            }

            public void learnerModelValueUpdate(string LearnerStateInfo_learnerState)
            {
                publisher.learnerModelValueUpdate(LearnerStateInfo_learnerState);
            }


            public void allUtterancesForParticipant(int participantId, string[] Utterance_utterances)
            {
                publisher.allUtterancesForParticipant(participantId, Utterance_utterances);
            }

            public void learnerModelMemoryEvent(string MemoryEvent_memoryEvent)
            {
                publisher.learnerModelMemoryEvent(MemoryEvent_memoryEvent);
            }
        }

        HttpListener JavaRpcListener;
        private Thread javaListenerThread;
        private Thread javaRequestDispatcherThread;
        bool serviceRunning = false;
        private int JavaXmlRpcPort = 15001;
        private String javaHost = "localhost";
        private String LearnerStateInfo_learnerState;
        private Dictionary<String,String> LearnerStateInfo_learnerStates = new Dictionary<string,string>();
        private Dictionary<String, String> LearnerNames = new Dictionary<string, string>();
        private Dictionary<String, String> LearnerInfos = new Dictionary<string, string>();
        private Dictionary<String, String> RoleToPlayer = new Dictionary<string, string>();
        private Dictionary<String, int> PlayerToId = new Dictionary<string, int>() {{"1",0},{"2",0} };
        private int stepId = 0;
        private int activityId = 0;
        private int scenarioId = 0;
        private int currentLearnerId = 0;
        private int sessionId = 1;
        List<HttpListenerContext> javaHttpRequestsQueue = new List<HttpListenerContext>();


        internal LearnerModelPublisher LMPublisher;
        public LearnerModelClient(string characterName = "")
            : base("LearnerModel",characterName) 
        {
            SetPublisher<ILearnerModelPublisher>();
            LMPublisher = new LearnerModelPublisher(Publisher);
            javaListenerThread = new Thread(new ThreadStart(JavaHttpListenerThread));
            javaListenerThread.Start();

            javaRequestDispatcherThread = new Thread(new ThreadStart(JavaRequestDispatcher));
            javaRequestDispatcherThread.Start();

        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {
                if (javaListenerThread != null) javaListenerThread.Join();
            }
            catch { }
            try
            {
                if (javaRequestDispatcherThread != null) javaRequestDispatcherThread.Join();
            }
            catch { }
        }

        internal void JavaHttpListenerThread()
        {
            while (!Shutingdown)
            {
                while (!serviceRunning)
                {
                    try
                    {
                        Debug("Attempt to start service on port '" + JavaXmlRpcPort + "'");
                        JavaRpcListener = new HttpListener();
                        JavaRpcListener.Prefixes.Add(string.Format("http://*:{0}/", JavaXmlRpcPort));
                        JavaRpcListener.Start();
                        Debug("XMLRPC Listening on port " + JavaXmlRpcPort);
                        serviceRunning = true;
                    }
                    catch
                    {
                        Debug("Port unavaliable.");
                        serviceRunning = false;
                        Thread.Sleep(1000);
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
                    DebugException(e);
                    serviceRunning = false;
                    if (JavaRpcListener != null) JavaRpcListener.Close();
                }
            }
            if (JavaRpcListener != null) JavaRpcListener.Close();
            Debug("Terminated JavaHttpListenerThread");
        }

        internal void JavaRequestDispatcher()
        {
            while (!Shutingdown)
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
                    DebugException(e);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated JavaRequestDispatcherThread");
        }

        internal void ProcessJavaRequest(object oContext)
        {
            try
            {
                XmlRpcListenerService svc = new JavaThalamusEventHandler(this);
                svc.ProcessRequest((HttpListenerContext)oContext);
            }
            catch (Exception e)
            {
                DebugException(e);
            }

        }
        void JavaWebRequest(String endBit, string[] jsonStringArray)
        {
            Console.WriteLine("http://" + javaHost + ":8080/emote/" + endBit);
            var httpWebRequest = (System.Net.HttpWebRequest)WebRequest.Create("http://" + javaHost + ":8080/emote/" + endBit);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            //  using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            // {
            string json = "";
            json = json + "{\"listFromThalamus\":[";
            //json = json + "{[";
            int i = 0;
            foreach (string s in jsonStringArray)
            {
                if (s != "")
                {
                    json = json + s + ",";
                    i++;
                }
                //Console.WriteLine("Item: " + s);
            }
            json = json.Remove(json.Length - 1);
            json = json + "]}";

            // "{\"participantId\":\"" + participantId + "\",\"participantName\":\"" + participantName + "\"}";
            Console.WriteLine("json: " + json);
            Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(json);
            httpWebRequest.ContentLength = data.Length;


            Stream stream = httpWebRequest.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            //  streamWriter.Write(json);

            // }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                Console.WriteLine("Response from Java " + responseText);

            }
        }
        void JavaWebRequest(String endBit)
        {
            try
            {
                Console.WriteLine("http://" + javaHost + ":8080/emote/" + endBit);
                var httpWebRequest =
                    (System.Net.HttpWebRequest) WebRequest.Create("http://" + javaHost + ":8080/emote/" + endBit);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";


                Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes("");
                httpWebRequest.ContentLength = data.Length;


                Stream stream = httpWebRequest.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine("Response from Java " + responseText);

                }
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine("EXCEPTION: "+ex.Message);
            }
        }

        void JavaWebRequest(String endBit, String jsonString)
        {
            Console.WriteLine("http://" + javaHost + ":8080/emote/" + endBit);
            var httpWebRequest = (System.Net.HttpWebRequest)WebRequest.Create("http://" + javaHost + ":8080/emote/" + endBit);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            Console.WriteLine("json: " + jsonString);
            Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(jsonString);
            httpWebRequest.ContentLength = data.Length;


            Stream stream = httpWebRequest.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine("Response from Java " + responseText);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in java web request:\n" + ex.Message);
            }
        }

        void JavaWebRequest(String endBit, Dictionary<String, String> parametersForJson)
        {
            Console.WriteLine("http://" + javaHost + ":8080/emote/" + endBit);
            var httpWebRequest = (System.Net.HttpWebRequest)WebRequest.Create("http://" + javaHost + ":8080/emote/" + endBit);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            //  using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            // {
            string json = "";
            json = json + "{";
            foreach (KeyValuePair<String, String> item in parametersForJson)
            {
                json = json + "\"" + item.Key + "\":\"" + item.Value + "\",";
            }
            json = json.Remove(json.Length - 1);
            json = json + "}";

            // "{\"participantId\":\"" + participantId + "\",\"participantName\":\"" + participantName + "\"}";
            Console.WriteLine("json: " + json);
            Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(json);
            httpWebRequest.ContentLength = data.Length;


            Stream stream = httpWebRequest.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            //  streamWriter.Write(json);

            // }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                Console.WriteLine("Response from Java " + responseText);

            }
        }

        void EmoteMapReadingEvents.ITaskEvents.interactionEvaluation(int learnerId, int stepId, int activityId, int scenarioId, int sessionId, string action, string strategy, string evaluation)
        {
         
        }

        void EmoteMapReadingEvents.ITaskEvents.stepAnswerAttempt(int learnerId, int stepId, int activityId, int scenarioId, int sessionId, bool correct, string[] competencyName, bool[] competencyCorrect, string[] competencyActual, string[] competencyExpected, String mapEventId, String[] comptencyValue, String[] competencyConfidence, String[] oldCompetencyValue, String[] competencyDelta)
        {


            LearnerStateInfo lsi = new LearnerStateInfo( learnerId,  stepId,  activityId,  scenarioId,  sessionId,  correct,  mapEventId);
             if (LearnerNames.ContainsKey(learnerId.ToString()))
            {
                lsi.learnerName = LearnerNames[learnerId.ToString()];
                
            }
             else
             {
                   lsi.learnerName = "friend";
                  
             }
             Console.WriteLine("Got LM Update for learner ID:" + learnerId.ToString() + " Name:" + lsi.learnerName + " EventId" + mapEventId);
            for (int i = 0; i < competencyName.Length; i++)
            {
                lsi.competencyItems.Add(new LearnerStateInfo.CompetencyItem(competencyName[i], competencyCorrect[i], competencyActual[i], competencyExpected[i], Convert.ToDouble(comptencyValue[i]), Convert.ToInt16(competencyConfidence[i]), Convert.ToDouble(oldCompetencyValue[i]), Convert.ToDouble(competencyDelta[i]), EmoteCommonMessages.EvidenceType.numerical, EmoteCommonMessages.Impact.Neutral));
            }
            LearnerStateInfo_learnerState = lsi.SerializeToJson();
            LearnerStateInfo_learnerStates[lsi.mapEventId] =LearnerStateInfo_learnerState;

            Console.WriteLine("Sending Learner State To Java:" + mapEventId);
            JavaWebRequest("learnerModelUpdate", LearnerStateInfo_learnerState);

            
            //TODO this should come after the learner model responds...
          //  Console.WriteLine("Sending Learner State:" + mapEventId);
          //  LMPublisher.learnerModelValueUpdateBeforeAffectPerceptionUpdate(LearnerStateInfo_learnerState);      
        
        }

        void EmoteCommonMessages.ILearnerModelActions.learnerModelUpdate(string LearnerStateInfo_learnerState)
        {
          //  JavaWebRequest("learnerModelUpdate", LearnerStateInfo_learnerState);
        }

        void EmoteCommonMessages.IAffectPerceptionEvents.UserState(string AffectPerceptionInfo_AffectiveStates)
        {
            Console.WriteLine("Got User State from Affect Perception");
            AffectPerceptionInfo api = AffectPerceptionInfo.DeserializeFromJson(AffectPerceptionInfo_AffectiveStates);
            String mapEventId = api.mMapEventId;
            if (mapEventId != null && mapEventId != "")
            {
                //We have a map even id to use.
                Console.WriteLine("Map event id:" + mapEventId);
                 if (LearnerStateInfo_learnerStates.ContainsKey(mapEventId))
                    {
                        Console.WriteLine("Sending message to IM mapEventId:" + mapEventId + " Reason:" + EmoteCommonMessages.LearnerModelUpdateReason.StepAnswerAttempt);
                    
                        LMPublisher.learnerModelValueUpdateAfterAffectPerceptionUpdate(LearnerStateInfo_learnerStates[mapEventId], AffectPerceptionInfo_AffectiveStates);
                    }
                 else
                 {
                        LearnerStateInfo lsi;
                        if (LearnerStateInfo_learnerState != null)
                        {
                            lsi = LearnerStateInfo.DeserializeFromJson(LearnerStateInfo_learnerState);
                        }
                        else
                        {
                           lsi = new LearnerStateInfo(0, 0, 0, 0, 0, true, mapEventId); 
                        }
                      lsi.reasonForUpdate = EmoteCommonMessages.LearnerModelUpdateReason.StepAnswerAttempt;
                      LearnerStateInfo_learnerState = LearnerStateInfo_learnerState = lsi.SerializeToJson();
                      Console.WriteLine("Sending message to IM mapEventId:" + mapEventId + " Reason:" + EmoteCommonMessages.LearnerModelUpdateReason.StepAnswerAttempt);
                     LMPublisher.learnerModelValueUpdateAfterAffectPerceptionUpdate(LearnerStateInfo_learnerState, AffectPerceptionInfo_AffectiveStates);
                    
                 }
            }
            else
            {
                //we have no map event id to use.
                // This is due to an affect update so set the reason appropriatly for the IM to take acount of. 
                LearnerStateInfo lsi;
                        if (LearnerStateInfo_learnerState != null)
                        {
                            lsi = LearnerStateInfo.DeserializeFromJson(LearnerStateInfo_learnerState);
                        }
                        else
                        {
                           lsi = new LearnerStateInfo(0, 0, 0, 0, 0, true, ""); 
                        }
                    lsi.reasonForUpdate = EmoteCommonMessages.LearnerModelUpdateReason.AffectUpdate;
                      LearnerStateInfo_learnerState = LearnerStateInfo_learnerState = lsi.SerializeToJson();
                      Console.WriteLine("Sending message to IM  Reason:" + EmoteCommonMessages.LearnerModelUpdateReason.AffectUpdate);
                    
                     LMPublisher.learnerModelValueUpdateAfterAffectPerceptionUpdate(LearnerStateInfo_learnerState, AffectPerceptionInfo_AffectiveStates);
                   
            
            }


           
           
         /*   AffectPerceptionInfo api = AffectPerceptionInfo.DeserializeFromJson(AffectPerceptionInfo_AffectiveStates);




            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["mapEventId"] = api.mMapEventId.ToString();

            foreach (EmoteEvents.AffectPerceptionInfo.AffectType affect in api.mAffectiveStates)
            {


            }

            JavaWebRequest("thalamusMapAction", parametersForJson);*/
        }

        void EmoteCommonMessages.IMapEvents.Click(double x, double y)
        {
            
        }

        void EmoteCommonMessages.IMapEvents.Pan(double x, double y, double startX, double startY)
        {
            
        }

        void EmoteCommonMessages.IMapEvents.Zoom(double[] finger0, double[] finger1, double[] finger0Start, double[] finger1Start)
        {
            
        }

        void EmoteCommonMessages.IEmoteActions.Reset()
        {
           
        }

        void EmoteCommonMessages.IEmoteActions.Stop()
        {
            
        }
   

        void EmoteCommonMessages.IAffectPerceptionEvents.ProbeRequest(EmoteCommonMessages.Probes type, int urgency)
        {
         
        }


        void EmoteMapReadingEvents.IGameStateEvents.GameState(bool running)
        {
         
        }

        void EmoteMapReadingEvents.IMapEvents.CompassHide()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.CompassShow()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceHide()
        {
         
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceShow()
        {
            
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasEnded()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasReset()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.DistanceToolHasStarted()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.DragTo()
        {
            
        }

        void EmoteMapReadingEvents.IMapEvents.MapKeyHide()
        {
           
        }

        void EmoteMapReadingEvents.IMapEvents.MapKeyShow()
        {
            
        }

        void EmoteMapReadingEvents.IMapEvents.Select(double lat, double lon, string symbolName)
        {
          
        }

        void EmoteMapReadingEvents.IMapEvents.TextShownOnScreen()
        {
            
        }

        void EmoteCommonMessages.IEmoteActions.SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
         
            LearnerInfo li = LearnerInfo.DeserializeFromJson(LearnerInfo_learnerInfo);
            LearnerInfos[li.thalamusLearnerId.ToString()] = LearnerInfo_learnerInfo;
            LearnerNames[li.thalamusLearnerId.ToString()] = li.firstName;
            Console.WriteLine("Setting learner info for ID:" + li.thalamusLearnerId.ToString() + " Name:" + li.firstName);
            JavaWebRequest("setLearnerInfo", LearnerInfo_learnerInfo);
     

        }

   /*     void EmoteCommonMessages.IEmoteActions.Start(int participantId, int participantId2, string participant1Name, string participant2Name)
        {
            PlayerToId["1"] = participantId;
            PlayerToId["2"] = participantId2;

            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["participantId"] = "" + participantId;
            parametersForJson["participantId2"] = "" + participantId2;
            parametersForJson["participant1Name"] = "" + participant1Name;
            parametersForJson["participant2Name"] = "" + participant2Name;
            JavaWebRequest("start", parametersForJson);
        }
        */
        void EmoteCommonMessages.ILearnerModelIdActions.getAllLearnerInfo()
        {
            JavaWebRequest("getAllLearnerInfo");
        }

        void EmoteCommonMessages.ILearnerModelIdActions.getNextThalamusId()
        {
            JavaWebRequest("getNextThalamusId");
        }

        void EmoteEnercitiesMessages.IEnercitiesAIPerceptions.UpdateStrategies(string StrategiesSet_strategies)
        {
            //TODO QUESTION what does this do?
            StrategiesSet ss = EmoteEvents.StrategiesSet.DeserializeFromJson(StrategiesSet_strategies);
            Dictionary<EmoteEnercitiesMessages.EnercitiesRole, double[]> strategies = ss.Strategies;
            foreach (KeyValuePair<EmoteEnercitiesMessages.EnercitiesRole, double[]> item in strategies)
            {
                //json = json + "\"" + item.Key + "\":\"" + item.Value + "\",";
                //double[] item.Value
                if(item.Key==EnercitiesRole.Mayor)
                {
                }
                else
                {

                     Strategy s = new Strategy(item.Value);
                    String player = RoleToPlayer[item.Key.ToString()];
                    int learnerId =  PlayerToId[player];
                //Weight -1 dont do, 1 is the pay attention. 
                //Abolute sum of all weights =1 
                //Scores/Strategy:
                double economy = s.EconomyWeight;
                double environment = s.EnvironmentWeight;
                double wellbeing = s.WellbeingWeight;
                double uniformity = s.ScoreUniformityWeight;

                //Resources:
                double homes = s.HomesWeight; //will tend to increase
                double money = s.MoneyWeight; //Will fluctuate with game? 
                double oil = s.OilWeight; //Will tend to decrease
                double power = s.PowerWeight; //Will fluctuate with game? 
              
                //check the item.key for the role, then check role to send the learner id to the system. Need to make a note based on the role sent in the start message.
                //break out each aspect of s and send to java.. 
                //Just send as evidence at the moment. 
                string[] eis = new string[8];
                EvidenceItem ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2,"economy",economy,EvidenceType.strategyWeight,sessionId);
                eis[0] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "environment", environment, EvidenceType.strategyWeight, sessionId);
                eis[1] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "wellbeing", wellbeing, EvidenceType.strategyWeight, sessionId);
                eis[2] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "uniformity", uniformity, EvidenceType.strategyWeight, sessionId);
                eis[3] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "homes", homes, EvidenceType.resourceWeight, sessionId);
                eis[4] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "money", money, EvidenceType.resourceWeight, sessionId);
                eis[5] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "oil", oil, EvidenceType.resourceWeight, sessionId);
                eis[6] = ei.SerializeToJson();
                ei = new EvidenceItem(learnerId, stepId, activityId, scenarioId, 2, "power", power, EvidenceType.resourceWeight, sessionId);
                eis[7] = ei.SerializeToJson();

                JavaWebRequest("addEvidenceItems", eis);

                }

                }
            //TODO QUESTION how should I record this? 
        }

        private void submitEIForBothLearners(EvidenceItem ei)
        {
            string[] eis = new string[2];
            eis[0] = ei.SerializeToJson();
            ei.learnerId = PlayerToId["2"];
            eis[1] = ei.SerializeToJson();
            JavaWebRequest("addEvidenceItems", eis);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.BuildMenuTooltipClosed(EmoteEnercitiesMessages.StructureCategory category, string translation)
        {
            //TODO update tool use evidence?
            //TODO link to applicationEvidenceID... Stick in a dictionary. key is the structure category+buildmenu. 
         //   EmoteEvents.

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = category.ToString();
            ei.action = "tooltipClosed";
            submitEIForBothLearners(ei);
            
        }

       

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.BuildMenuTooltipShowed(EmoteEnercitiesMessages.StructureCategory category, string translation)
        {
            //TODO update tool use evidence?
            //QUESTION Is this always explicitly closed? Mainly interested in the fact that it is opened. 

            //TODO send evidence:
            //evidence
            //TOOL = build menu
            //applicationEvidenceID = generate Stick in a dictionary. key is the structure category+buildmenu. 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = category.ToString();
            ei.action = "tooltipShowed";
            submitEIForBothLearners(ei);

        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.BuildingMenuToolSelected(EmoteEnercitiesMessages.StructureType structure, string translation)
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = structure.ToString();
            ei.action = "buildingMenuSelected";

            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.BuildingMenuToolUnselected(EmoteEnercitiesMessages.StructureType structure, string translation)
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = structure.ToString();
            ei.action = "buildingMenuUnselected";
            submitEIForBothLearners(ei);
            
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.PoliciesMenuClosed()
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "policyMenuClosed";
            ei.action = "policyMenuClosed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.PoliciesMenuShowed()
        {
            //update tool use evidence

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "policyMenuShowed";
            ei.action = "policyMenuShowed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.PolicyTooltipClosed()
        {
            //update tool use evidence

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "policyTooltipClosed";
            ei.action = "policyTooltipClosed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.PolicyTooltipShowed(EmoteEnercitiesMessages.PolicyType policy, string translation)
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = policy.ToString();
            ei.action = "policyTooltipShowed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.UpgradeTooltipClosed()
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "upgradeTooltipClosed";
            ei.action = "upgradeTooltipClosed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.UpgradeTooltipShowed(EmoteEnercitiesMessages.UpgradeType upgrade, string translation)
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = upgrade.ToString();
            ei.action = "upgradeTooltipShowed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.UpgradesMenuClosed()
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "upgradeMenuClosed";
            ei.action = "upgradeMenuClosed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesExamineEvents.UpgradesMenuShowed()
        {
            //TODO update tool use evidence? 

            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = "upgradeMenuShowed";
            ei.action = "upgradeMenuShowed";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameNoOil(int totalScore)
        {


            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "endGame", totalScore, EvidenceType.endGame, sessionId);
            ei.actual = "noOil";
            submitEIForBothLearners(ei);

        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameSuccessfull(int totalScore)
        {
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "endGame", totalScore, EvidenceType.endGame, sessionId);
            ei.actual = "succesfull";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.EndGameTimeOut(int totalScore)
        {
            //TODO  session and final score
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "endGame", totalScore, EvidenceType.endGame, sessionId);
            ei.actual = "timeOut";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.GameStarted(string player1Name, string player1Role, string player2Name, string player2Role)
        {
            //TODO update session and final score
            //QUESTION Change this for thalmausID. Update session information...
            //TODO roles can be swithched to the enum for the roles. 
            //TODo update this to send
            string[] eis = new string[2];

            RoleToPlayer[player1Role] = "1";
            RoleToPlayer[player2Role] = "2";
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "roll", 0.0, EvidenceType.roll, sessionId);
            ei.actual = player1Role;
            
            eis[0] = ei.SerializeToJson();

            ei = new EvidenceItem(PlayerToId["2"], stepId, activityId, scenarioId, 2, "roll", 0.0, EvidenceType.roll, sessionId);
            ei.actual = player2Role;

            eis[1] = ei.SerializeToJson();
            JavaWebRequest("addEvidenceItems", eis);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.PlayersGender(EmoteEnercitiesMessages.Gender player1Gender, EmoteEnercitiesMessages.Gender player2Gender)
        {
            //QUESTION LearnerInfo.
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.PlayersGenderString(string player1Gender, string player2Gender)
        {
            //QUESTION LearnerInfo.
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.ReachedNewLevel(int level)
        {
            //Update level in learner model. 
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "levelReached", level, EvidenceType.levelReached, sessionId);
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.ResumeGame(string player1Name, string player1Role, string player2Name, string player2Role, string serializedGameState)
        {
            //TODO Question do i need this? 
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove)
        {
            //TODO Question do i need this? Could be used for checking strategy? Does AI do this? 
        }

        void EmoteEnercitiesMessages.IEnercitiesGameStateEvents.TurnChanged(string serializedGameState)
        {

            EnercitiesGameInfo egi = EnercitiesGameInfo.DeserializeFromJson(serializedGameState);
           
            
        /*public EnercitiesRole CurrentRole { get; set; }
        public double EconomyScore { get; set; }
        public double EnvironmentScore { get; set; }
        public double GlobalScore { get; set; }
        public int Level { get; set; }
        public double Money { get; set; }
        public double MoneyEarning { get; set; }
        public double Oil { get; set; }
        public int Population { get; set; }
        public double PowerConsumption { get; set; }
        public double PowerProduction { get; set; }
        public int TargetPopulation { get; set; }
        public double WellbeingScore { get; set; }*/

            //TODO increment the turn changed
            //TODO change the learner id. Robot, 1,2. 

            //TODO Update game state? 
            stepId = egi.Level;

           // submitGameState(serializedGameState);

            if (egi.CurrentRole != EnercitiesRole.Mayor)
            {
                String currentRole = RoleToPlayer[egi.CurrentRole.ToString()];
                currentLearnerId = PlayerToId[currentRole];

                EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "turnChanged",
                    egi.GlobalScore, EvidenceType.turnChanged, sessionId);
                submitEIForBothLearners(ei);
            }

           // JavaWebRequest("addEnercitiesGameState", serializedGameState);
           
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            	
            parametersForJson["learner1"] = ""+PlayerToId["1"];
            parametersForJson["learner2"] = ""+PlayerToId["2"];
            parametersForJson["stepId"] = ""+stepId;
            parametersForJson["activityId"] = ""+activityId;
            parametersForJson["scenarioId"] = ""+scenarioId;
            parametersForJson["emoteScenarioId"] = "" + 2;
            parametersForJson["sessionNumber"] = "" + sessionId;
            
            parametersForJson["currentRole"] = egi.CurrentRole.ToString();
            parametersForJson["economyScore"] = "" + egi.EconomyScore;
            parametersForJson["environmentScore"] = "" + egi.EnvironmentScore;
            parametersForJson["globalScore"] = "" + egi.GlobalScore;
            parametersForJson["level"] = "" + egi.Level;
            parametersForJson["money"] = "" + egi.Money;
            parametersForJson["moneyEarning"] = "" + egi.MoneyEarning;
            parametersForJson["population"] = "" + egi.Population;
            parametersForJson["powerConsumption"] = "" + egi.PowerConsumption;
            parametersForJson["powerProduction"] = "" + egi.PowerProduction;
            parametersForJson["targetPopulation"] = "" + egi.TargetPopulation;
            parametersForJson["wellbeingScore"] = "" + egi.WellbeingScore;
       

            JavaWebRequest("addEnercitiesGameState", parametersForJson);

            if (egi.Money < 0.0)
            {
                EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "crisis", egi.Money, EvidenceType.crisis, sessionId);
                ei.actual = "financial";
                submitEIForBothLearners(ei);
            }
            if (egi.Oil < 0.0)
            {
                EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "crisis", egi.Oil, EvidenceType.crisis, sessionId);
                ei.actual = "energy";
                submitEIForBothLearners(ei);
            }
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ConfirmConstruction(EmoteEnercitiesMessages.StructureType structure, string translation, int cellX, int cellY)
        {
           //TODO Update constructions encountered? 
            //TODO Update learner model with strategy?
            //QUESTION when is this used?
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "confirmConstruction", 0, EvidenceType.takeTurn, sessionId);
            ei.actual = structure.ToString();
            ei.action = "confirmConstruction";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ExamineCell(double screenX, double screenY, int cellX, int cellY, EmoteEnercitiesMessages.StructureType StructureType_structure, string StructureType_translated)
        {
            //TODO Update tool use
            //TODO update knowledge encountered? 
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "toolUsed", 0.0, EvidenceType.toolUse, sessionId);
            ei.actual = StructureType_structure.ToString();
            ei.action = "examineCell";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.ImplementPolicy(EmoteEnercitiesMessages.PolicyType policy, string translation)
        {
            //TODO Update policy encountered
            //TODO Update learner model with strategy?
            //QUESTION when is this used?
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "implementPolicy", 0, EvidenceType.takeTurn, sessionId);
            ei.actual = policy.ToString();
            ei.action = "implementPolicy";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.PerformUpgrade(EmoteEnercitiesMessages.UpgradeType upgrade, string translation, int cellX, int cellY)
        {
            //TODO Update upgrade encountered
            //TODO Update learner model with strategy?
            //QUESTION when is this used?
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "performUpgrade", 0, EvidenceType.takeTurn, sessionId);
            ei.actual = upgrade.ToString();
            ei.action = "performUpgrade";
            submitEIForBothLearners(ei);
        }

        void EmoteEnercitiesMessages.IEnercitiesTaskEvents.SkipTurn()
        {
            //TODO Update strategy?
            //TODO Update learner model with strategy?
            //QUESTION when is this used?
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "skipTurn", 0, EvidenceType.takeTurn, sessionId);
            ei.actual = "skipTurn";
            ei.action = "skipTurn";
            submitEIForBothLearners(ei);
        }

        internal void updateAllLearnerInfo(string[] LearnerInfo_learnerInfos)
        {
            foreach (string LearnerInfo_learnerInfo in LearnerInfo_learnerInfos)
            {
                LearnerInfo li = LearnerInfo.DeserializeFromJson(LearnerInfo_learnerInfo);

                LearnerInfos[li.thalamusLearnerId.ToString()] = LearnerInfo_learnerInfo;
                LearnerNames[li.thalamusLearnerId.ToString()] = li.firstName;
            }
          
        }

        private void utteranceUpdate(Dictionary<string, string> parametersForJson)
        {
            parametersForJson["sessionNumber"] = "" + sessionId;
            JavaWebRequest("utteranceUpdate", parametersForJson);
            if (PlayerToId["2"] != 0)
            {
                parametersForJson["participantId"] = "" + PlayerToId["2"];
                JavaWebRequest("utteranceUpdate", parametersForJson);
            }
        }

        void IFMLSpeech.CancelUtterance(string id)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();          
            parametersForJson["utterenceId"] = "" + id;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["method"] = "" + "cancelUtterance";
            utteranceUpdate(parametersForJson);  
        }

     

        void IFMLSpeech.PerformUtterance(string id, string utterance, string category)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["utterenceId"] = "" + id;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["utterance"] = "" + utterance;
            parametersForJson["category"] = "" + category;
            parametersForJson["method"] = "" + "performUtterance";
            utteranceUpdate(parametersForJson);
         
        }

        void IFMLSpeech.PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["utterenceId"] = "" + id;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["category"] = "" + category;
            parametersForJson["subcategory"] = "" + subcategory;
            parametersForJson["method"] = "" + "performUtteranceFromLibrary";
            utteranceUpdate(parametersForJson);
        }

        void IFMLSpeechEvents.UtteranceFinished(string id)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["utterenceId"] = "" + id;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["method"] = "" + "utteranceFinished";
            utteranceUpdate(parametersForJson);
        }

        void IFMLSpeechEvents.UtteranceStarted(string id)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["utterenceId"] = "" + id;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["method"] = "" + "utteranceStarted";
            utteranceUpdate(parametersForJson);
        }

        void IFMLSpeechEventsExtras.UtteranceIsAQuestion(string id)
        {
            //Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
           // parametersForJson["utterenceId"] = "" + id;
           // parametersForJson["participantId"] = "" + PlayerToId["1"];
           // parametersForJson["method"] = "" + "utteranceIsAQuestion";
          //  utteranceUpdate(parametersForJson);
        }

        void ILearnerModelUtteranceHistoryAction.UtteranceUsed(string id, string Utterance_utterance)
        {
            EmoteEvents.ComplexData.Utterance utternace = EmoteEvents.ComplexData.Utterance.DeserializeFromJson<EmoteEvents.ComplexData.Utterance>(Utterance_utterance);
            Console.WriteLine("Got utterance used, id in method call=" + id + ", utternace.Id=" + utternace.Id + ", utternace.ThalamusId" + utternace.ThalamusId);
            
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["utterenceId"] = "" + utternace.ThalamusId;
            parametersForJson["participantId"] = "" + PlayerToId["1"];
            parametersForJson["utterance"] = "" + utternace.Text;
            parametersForJson["library"] = "" + utternace.Library;
            parametersForJson["repetitions"] = "" + utternace.Repetitions;
            parametersForJson["libraryId"] = "" + id;
            parametersForJson["method"] = "" + "utteranceUsed";
            
            utteranceUpdate(parametersForJson);
        }


        void ILearnerModelIdActions.getAllUtterancesForParticipant(int participantId)
        {
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            parametersForJson["participantId"] = "" + participantId;
            JavaWebRequest("getAllUtterancesForParticipant", parametersForJson);
        }


        void IEmoteActions.Start(string StartMessageInfo_info)
        {
            Console.WriteLine("Start message");

            EmoteEvents.ComplexData.StartMessageInfo smi = EmoteEvents.ComplexData.StartMessageInfo.DeserializeFromJson<EmoteEvents.ComplexData.StartMessageInfo>(StartMessageInfo_info);
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            sessionId = smi.SessionId;
            LearnerInfo li = smi.Students.First();
            PlayerToId["1"] = li.thalamusLearnerId;
            PlayerToId["2"] = 0;
            parametersForJson["participantId"] = "" + li.thalamusLearnerId;
            parametersForJson["participantId2"] = "" + 0;
            parametersForJson["participant1Name"] = "" + li.firstName;
            parametersForJson["participant2Name"] = "";
            parametersForJson["sessionNumber"] = ""+smi.SessionId;
            if (smi.Students.Count > 1)
            {
                LearnerInfo li2 = smi.Students.ElementAt(1);
                PlayerToId["2"] = li2.thalamusLearnerId;
                parametersForJson["participantId2"] = "" + li2.thalamusLearnerId;
                parametersForJson["participant2Name"] = "" + li2.firstName;
            }

            JavaWebRequest("start", parametersForJson);
        }

        void IEnercitiesGameStateActions.EndGameTimeout()
        {
            Console.WriteLine("Got enercities end game");
            Dictionary<String, String> parametersForJson = new Dictionary<String, String>();
            
            parametersForJson["participantId"] = "" + PlayerToId["1"] ;
            parametersForJson["participantId2"] = "" + PlayerToId["2"];
            parametersForJson["participant1Name"] = "";
            parametersForJson["participant2Name"] = "";
            parametersForJson["sessionNumber"] = "" + sessionId;

            JavaWebRequest("enercitiesEndGameTimeout", parametersForJson);
        }

        void IEmotionalClimate.EmotionalClimateLevel(EmotionalClimateLevel level)
        {
            double value = 0.0;
            if (level.Equals(EmotionalClimateLevel.Positive))
            {
                value = 1.0;
            }
            EvidenceItem ei = new EvidenceItem(PlayerToId["1"], stepId, activityId, scenarioId, 2, "emotionalClimateLevel", value, EvidenceType.emotionalClimateLevel, sessionId);
            ei.actual = level.ToString();
            submitEIForBothLearners(ei);
        }
    }
}
