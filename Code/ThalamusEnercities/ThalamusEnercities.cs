using EmoteEvents;
using Thalamus;
using System.Reflection;

//using Thalamus.Actions; //for ExampleRunTimeBehavior()

using System.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using CookComputing.XmlRpc;
using EmoteEnercitiesMessages;
using EmoteEvents.ComplexData;

/// <authors>
/// Nuno Marques
/// </authors>



namespace ThalamusEnercities
{

	public interface IEnercities :
        EmoteEnercitiesMessages.IEnercitiesExamineActions,
        EmoteCommonMessages.IMapActions, 
        EmoteEnercitiesMessages.IEnercitiesTaskActions,
        EmoteCommonMessages.IApplicationActions,
        EmoteCommonMessages.IEmoteActions,
        EmoteEnercitiesMessages.IEnercitiesThermometerActions,
        IEnercitiesGameStateActions
	{
    }

	internal interface IEnercitiesPublisher : IThalamusPublisher,        
        EmoteCommonMessages.IMenuEvents,
        EmoteCommonMessages.IMapEvents,
        EmoteCommonMessages.ITargetEvents,
        EmoteEnercitiesMessages.IEnercitiesExamineEvents,
        EmoteEnercitiesMessages.IEnercitiesTaskEvents,
        EmoteEnercitiesMessages.IEnercitiesGameStateEvents
	{}

    public interface IEnercitiesRpc : /*IEnercities, */IXmlRpcProxy 
    {
        [XmlRpcMethod]
        void PlayStrategy(string EnercitiesStrategy_strategy);
        [XmlRpcMethod]
        void ConfirmConstruction(string StructureType_structure, int cellX, int cellY);
        [XmlRpcMethod]
        void ImplementPolicy(string PolicyType_policy);
        [XmlRpcMethod]
        void PerformUpgrade(string UpgradeType_upgrade, int cellX, int cellY);
        [XmlRpcMethod]
        void SkipTurn();

        [XmlRpcMethod]
        void PreviewBuildCell(string StructureType_structure, int cellX, int cellY);
        [XmlRpcMethod]
        void ShowBuildMenuTooltip(string StructureCategory_category);
        [XmlRpcMethod]
        void CloseBuildMenuTooltip();
        [XmlRpcMethod]
        void SelectBuildingMenuTool(string StructureType_structure);
        [XmlRpcMethod]
        void UnselectBuildingMenuTool();
        [XmlRpcMethod]
        void ShowPoliciesMenu();
        [XmlRpcMethod]
        void ClosePoliciesMenu();
        [XmlRpcMethod]
        void ShowPolicyTooltip(string PolicyType_policy);
        [XmlRpcMethod]
        void ClosePolicyTooltip();
        [XmlRpcMethod]
        void ShowUpgradesMenu(int cellX, int cellY);
        [XmlRpcMethod]
        void CloseUpgradesMenu();
        [XmlRpcMethod]
        void ShowUpgradeTooltip(int cellX, int cellYstring, string UpgradeType_upgrade);
        [XmlRpcMethod]
        void CloseUpgradeTooltip();

        [XmlRpcMethod]
        void Click();
        [XmlRpcMethod]
        void Highlight(double x, double y);
        [XmlRpcMethod]
        void HighlightRightAnswer();
        [XmlRpcMethod]
        void Pan();
        [XmlRpcMethod]
        void Zoom();

        [XmlRpcMethod]
        void ThermometerNewLevel();
        [XmlRpcMethod]
        void ThermometerAddRound(int quality);  // Quality ranges from 0 to 100 where 1 is bad and 100 is good

        [XmlRpcMethod]
        void Start(int participantId, int participantId2, string participant1Name, string participant2Name, string language);
        [XmlRpcMethod]
        void Stop();
        [XmlRpcMethod]
        void Reset();
        [XmlRpcMethod]
        void SetLearnerInfo(string LearnerInfo_learnerInfo);

    }

    public class ThalamusEnercities : Thalamus.ThalamusClient, IEnercities
    {
        
        private class EnercitiesPublisher : IEnercitiesPublisher
        {
            dynamic publisher;
            public EnercitiesPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }
            
            public void Click(double x, double y)
            {
                publisher.Click(x, y);
            }

            public void Zoom(double[] finger0, double[] finger1, double[] previousFinger0, double[] previousFinger1)
            {
                publisher.Zoom(finger0, finger1, previousFinger0, previousFinger1);
            }


            public void Pan(double x, double y, double prevX, double prevY)
            {
                publisher.Pan(x, y, prevX, prevY);
            }

            public void EndOfGameHide()
            {
                publisher.EndOfGameHide();
            }

            public void EndOfGameShow()
            {
                publisher.EndOfGameShow();
            }

            public void EndOfLevelHide()
            {
                publisher.EndOfLevelHide();
            }

            public void EndOfLevelShow()
            {
                publisher.EndOfLevelShow();
            }

            public void StartGameHide()
            {
                publisher.StartGameHide();
            }

            public void StartGameShow()
            {
                publisher.StartGameShow();
            }

            public void PlayAnimationQueued(string id, string animation)
            {
                publisher.PlayAnimationQueued(id, animation);
            }
        
            public void TargetScreenInfo(string targetName, int X, int Y)
            {
                publisher.TargetScreenInfo(targetName, X, Y);
            }

            public void TargetScreenInfoCopyFrom(string targetName, string copyFromTarget)
            {
                publisher.TargetScreenInfoCopyFrom(targetName, copyFromTarget);
            }

            public void BuildMenuTooltipClosed(StructureCategory category, string categoryTranslated)
            {
                publisher.BuildMenuTooltipClosed(category, categoryTranslated);
            }

            public void BuildMenuTooltipShowed(StructureCategory category, string categoryTranslated)
            {
                publisher.BuildMenuTooltipShowed(category, categoryTranslated);
            }

            public void BuildingMenuToolSelected(StructureType structure, string structureTranslated)
            {
                publisher.BuildingMenuToolSelected(structure, structureTranslated);
            }

            public void BuildingMenuToolUnselected(StructureType structure, string structureTranslated)
            {
                publisher.BuildingMenuToolUnselected(structure, structureTranslated);
            }

            public void PoliciesMenuClosed()
            {
                publisher.PoliciesMenuClosed();
            }

            public void PoliciesMenuShowed()
            {
                publisher.PoliciesMenuShowed();
            }

            public void PolicyTooltipClosed()
            {
                publisher.PolicyTooltipClosed();
            }

            public void PolicyTooltipShowed(PolicyType policy, string policyTranslated)
            {
                publisher.PolicyTooltipShowed(policy, policyTranslated);
            }

            public void UpgradeTooltipClosed()
            {
                publisher.PolicyTooltipClosed();
            }

            public void UpgradeTooltipShowed(UpgradeType upgrade, string upgradeTranslated)
            {
                publisher.UpgradeTooltipShowed(upgrade, upgradeTranslated);
            }

            public void UpgradesMenuClosed()
            {
                publisher.UpgradesMenuClosed();
            }

            public void UpgradesMenuShowed()
            {
                publisher.UpgradesMenuShowed();
            }
            
            public void SkipTurn()
            {
                publisher.SkipTurn();
            }

            public void ConfirmConstruction(StructureType structure, string structureTranslated, int x, int y)
            {
                publisher.ConfirmConstruction(structure, structureTranslated, x, y);
            }

            public void ImplementPolicy(PolicyType policy, string policyTranslated)
            {
                publisher.ImplementPolicy(policy, policyTranslated);
            }

            public void PerformUpgrade(UpgradeType upgrade, string upgradeTranslated, int x, int y)
            {
                publisher.PerformUpgrade(upgrade, upgradeTranslated, x, y);
            }

            public void GameStarted(string player1name, string player1role, string player2name, string player2role)
            {
                publisher.GameStarted(player1name, player1role, player2name, player2role);
            }

            public void EndGameNoOil(int totalScore)
            {
                publisher.EndGameNoOil(totalScore);
            }

            public void EndGameSuccessfull(int totalScore)
            {
                publisher.EndGameSuccessfull(totalScore);
            }

            public void EndGameTimeOut(int totalScore)
            {
                publisher.EndGameTimeOut(totalScore);
            }

            public void TurnChanged(string serializedGameState)
            {
                publisher.TurnChanged(serializedGameState);
            }

            public void ReachedNewLevel(int level)
            {
                publisher.ReachedNewLevel(level);
            }

            public void TargetLink(string targetName, string linkedTargetName)
            {
                publisher.TargetLink(targetName, linkedTargetName);
            }

            public void ResumeGame(string player1name, string player1role, string player2name, string player2role, string serializedGameState)
            {
                publisher.ResumeGame(player1name, player1role, player2name, player2role, serializedGameState);
            }

            public void StrategyGameMoves(string environmentalistMove, string economistMove, string mayorMove, string globalMove)
            {
                publisher.StrategyGameMoves(environmentalistMove, economistMove, mayorMove, globalMove);
            }


            public void ExamineCell(double screenX, double screenY, int cellX, int cellY, StructureType StructureType_structure, string StructureType_translated)
            {
                publisher.ExamineCell(screenX, screenY, cellX, cellY, StructureType_structure, StructureType_translated);
            }

            public void SpeakStop()
            {
                throw new NotImplementedException();
            }

            public void PlayersGender(Gender player1gender, Gender player2gender)
            {
                publisher.PlayersGender(player1gender, player2gender);
            }

            public void PlayersGenderString(string player1gender, string player2gender)
            {
                publisher.PlayersGenderString(player1gender, player2gender);
            }

        }

        private System.Random rand = new System.Random();
        private HttpListener listener;
        private bool serviceRunning;
        private int localPort = 7000;
        private bool shutdown;
        List<HttpListenerContext> httpRequestsQueue = new List<HttpListenerContext>();
        private Thread dispatcherThread;
        private Thread messageDispatcherThread;

        private string remoteUri = "";
        IEnercitiesRpc rpcProxy;
        private int remotePort = 7001;
        private string remoteAddress = "localhost";

        internal IEnercitiesPublisher ThalamusPublisher;

        public ThalamusEnercities(string character = "")
            : base("ThalamusEnercities", character)
        {
            Thalamus.Environment.Instance.setDebug("messages", false);

            dispatcherThread = new Thread(new ThreadStart(DispatcherThreadEnercities));
            messageDispatcherThread = new Thread(new ThreadStart(MessageDispatcherEnercities));
            dispatcherThread.Start();
            messageDispatcherThread.Start();

            SetPublisher<IEnercitiesPublisher>();
            ThalamusPublisher = new EnercitiesPublisher(Publisher);
            

            remoteUri = String.Format("http://{0}:{1}", remoteAddress, remotePort);
            Debug("Enercities endpoint set to " + remoteUri);
            rpcProxy = XmlRpcProxyGen.Create<IEnercitiesRpc>();
            rpcProxy.Expect100Continue = true;
            /*rpcProxy.KeepAlive = false;*/
            rpcProxy.Timeout = 2000;
            rpcProxy.Url = remoteUri;

            this.NewClientConnected += (NewClientConnectedHandler)((name, clientId) =>
            {
                if (lastGameState!=null)
                {
                    Thread.Sleep(1000);
                    ThalamusPublisher.ResumeGame(Player1Name, Player1Role, Player2Name, Player2Role, lastGameState.SerializeToJson());
                }
            });

        }

        #region rpc stuff

        public override void Dispose()
        {
            shutdown = true;

            try
            {
                if (listener != null) listener.Stop();
            }
            catch { }

            try
            {
                if (dispatcherThread != null) dispatcherThread.Join();
            }
            catch { }

            try
            {
                if (messageDispatcherThread != null) messageDispatcherThread.Join();
            }
            catch { }

            base.Dispose();
        }

        public void DispatcherThreadEnercities()
        {
            while (!serviceRunning)
            {
                try
                {
                    Debug("Attempt to start service on port '" + localPort + "'");
                    listener = new HttpListener();
                    listener.Prefixes.Add(string.Format("http://*:{0}/", localPort));
                    listener.Start();
                    Debug("XMLRPC Listening on " + string.Format("http://*:{0}/", localPort));
                    serviceRunning = true;
                }
                catch
                {
                    localPort++;
                    Debug("Port unavaliable.");
                    serviceRunning = false;
                }
            }

            while (!shutdown)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    lock (httpRequestsQueue)
                    {
                        httpRequestsQueue.Add(context);
                    }
                }
                catch (Exception e)
                {
                    DebugException(e);
                    serviceRunning = false;
                    if (listener != null)
                        listener.Close();
                }
            }
            Debug("Terminated DispatcherThreadEnercities");
            //listener.Close();
        }

        public void MessageDispatcherEnercities()
        {
            while (!shutdown)
            {
                bool performSleep = true;
                try
                {
                    if (httpRequestsQueue.Count > 0)
                    {
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (httpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(httpRequestsQueue);
                            httpRequestsQueue.Clear();
                        }
                        foreach (HttpListenerContext r in httpRequests)
                        {
                            //ProcessRequest(r);
                            (new Thread(new ParameterizedThreadStart(ProcessRequestEnercities))).Start(r);
                            performSleep = false;
                        }
                    }


                }
                catch (Exception e)
                {
                    DebugException(e);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated PerceptionInfoDispatcherEnercities");
        }

        public void ProcessRequestEnercities(object oContext)
        {
            try
            {
                XmlRpcListenerService svc = new ThalamusEnercitiesService(this);
                svc.ProcessRequest((HttpListenerContext)oContext);
            }
            catch (Exception e)
            {
                DebugException(e);
            }

        }

        #endregion


        #region publish stuff

        //public void GameStarted(string player1, string player2)
        //{
        //    skipTurnPointing = true;
        //    return;
        //}

        public void EndGameSuccessfull(int totalScore)
        {
            ThalamusPublisher.EndGameSuccessfull(totalScore);
            //ThalamusPublisher.Speak("", "The game is over. We have successfully ended the game! Congratulations! We all scored " + totalScore + " points");
        }

        public void EndGameNoOil(int totalScore)
        {
            ThalamusPublisher.EndGameNoOil(totalScore);
            //ThalamusPublisher.Speak("", "The game is over. Sadly we ran out of oil. We all scored " + totalScore + " points");
        }

        public void EndGameTimeOut(int totalScore)
        {
            ThalamusPublisher.EndGameTimeOut(totalScore);
            //ThalamusPublisher.Speak("", "The game is over. It seems we have spent so many years playing... We all scored " + totalScore + " points");
        }

        EnercitiesGameInfo lastGameState = null;
        public void NotifyTurnChanged(EnercitiesGameInfo gameState)
        {
            lastGameState = gameState;
            ThalamusPublisher.TurnChanged(gameState.SerializeToJson());
        }


        public void NewUpgrade(string upgradeType_enum, string upgrade_translated, int cellX, int cellY)
        {
            //SpeakAndGaze("I'm going to upgrade " + upgradeName.Replace('_', ' '), x, y, z, 0.6f);
            ThalamusPublisher.PerformUpgrade((UpgradeType)Enum.Parse(typeof(UpgradeType), upgradeType_enum), upgrade_translated,0,0);
        }

        public void ImplementPolicy(string policyType_enum, string policy_translated)
        {
            //ThalamusPublisher.Speak("", "I'm going to implement " + policyName.Replace('_', ' '));
            ThalamusPublisher.ImplementPolicy((PolicyType)Enum.Parse(typeof(PolicyType), policyType_enum), policy_translated);
        }

        public void NewLevel(int level)
        {
            //ThalamusPublisher.Speak("", "We have reached level " + level);
            ThalamusPublisher.ReachedNewLevel(level);
        }

        

        //public void CommentUpgrade(string playerName, string upgrade, double x, double y, double z)
        //{
        //    SpeakAndGaze(playerName + " upgraded " + upgrade.Replace('_', ' '), x, y, z, 0.6f);
        //}

        //public void CommentPolicy(string playerName, string policy)
        //{
        //    ThalamusPublisher.Speak("", playerName + " implemented " + policy.Replace('_', ' '));
        //}

        public void ClickOnScreen(double x, double y)
        {
            Console.WriteLine("Click Event on: " + x + ", " + y);
            ThalamusPublisher.Click(x, y);
        }

        public void ZoomOnScreen(double[] finger0, double[] finger1, double[] finger0Start, double[] finger1Start)
        {
            Console.WriteLine(string.Format("Zoom Event finger0 {0},{1} - finger1 {2},{3} - previous finger0 {4},{5} - previous finger1 {6},{7}", finger0[0], finger0[1], finger1[0], finger1[0], finger0Start[0], finger0Start[1], finger1Start[0], finger1Start[1]));
            ThalamusPublisher.Zoom(finger0, finger1, finger0Start, finger1Start);
        }

        public void PanOnScreen(double x, double y, double startX, double startY)
        {
            Console.WriteLine(string.Format("Pan event from {0},{1} to {2},{3}", x, y, startX, startY));
            ThalamusPublisher.Pan(x, y, startX, startY);
        }

        public void StartGameShow()
        {
            ThalamusPublisher.StartGameShow();
        }

        public void StartGameHide()
        {
            ThalamusPublisher.StartGameHide();
        }

        public void EndOfLevelShow()
        {
            ThalamusPublisher.EndOfLevelShow();
        }

        public void EndOfLevelHide()
        {
            ThalamusPublisher.EndOfLevelHide();
        }

        public void EndOfGameShow()
        {
            ThalamusPublisher.EndOfGameShow();
        }

        public void EndOfGameHide()
        {
            ThalamusPublisher.EndOfGameHide();
        }

        #endregion




        string Player1Name = "";
        string Player1Role = "";
        string Player2Name = "";
        string Player2Role = "";
        internal void GameStarted(string player1name, string player1role, string player2name, string player2role)
        {
            Player1Name = player1name;
            Player1Role = player1role;
            Player2Name = player2name;
            Player2Role = player2role;
            ThalamusPublisher.GameStarted(player1name, player1role, player2name, player2role);
        }

        public void Click()
        {
        }

        public void Highlight(double x, double y)
        {
            rpcProxy.Highlight(x, y);
        }

        public void HighlightRightAnswer()
        {
        }

        public void Pan()
        {
        }

        public void Zoom()
        {
        }

        public void ThermometerNewLevel()
        {
            rpcProxy.ThermometerNewLevel();
        }

        public void ThermometerAddRound(int quality)
        {
            rpcProxy.ThermometerAddRound(quality);
        }

        #region IEnercitiesTaskActions Members

        void IEnercitiesTaskActions.PlayStrategy(EnercitiesStrategy strategy)
        {
            try
            {
                rpcProxy.PlayStrategy(strategy.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesTaskActions.ConfirmConstruction(StructureType structure, int cellX, int cellY)
        {
            try
            {
                rpcProxy.ConfirmConstruction(structure.ToString(), cellX, cellY);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesTaskActions.ImplementPolicy(PolicyType policy)
        {
            try
            {
                rpcProxy.ImplementPolicy(policy.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesTaskActions.PerformUpgrade(UpgradeType upgrade, int cellX, int cellY)
        {
            try
            {
                rpcProxy.PerformUpgrade(upgrade.ToString(), cellX, cellY);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesTaskActions.SkipTurn()
        {
            try
            {
                rpcProxy.SkipTurn();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.PreviewBuildCell(StructureType structureType, int cellX, int cellY)
        {
            try
            {
                rpcProxy.PreviewBuildCell(structureType.ToString(), cellX, cellY);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        #endregion

        void IEnercitiesExamineActions.ShowBuildMenuTooltip(StructureCategory category)
        {
            try
            {
                rpcProxy.ShowBuildMenuTooltip(category.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.CloseBuildMenuTooltip()
        {
            try
            {
                rpcProxy.CloseBuildMenuTooltip();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.SelectBuildingMenuTool(StructureType structure)
        {
            try
            {
                rpcProxy.SelectBuildingMenuTool(structure.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.UnselectBuildingMenuTool()
        {
            try
            {
                rpcProxy.UnselectBuildingMenuTool();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.ShowPoliciesMenu()
        {
            try
            {
                rpcProxy.ShowPoliciesMenu();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.ClosePoliciesMenu()
        {
            try
            {
                rpcProxy.ClosePoliciesMenu();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.ShowPolicyTooltip(PolicyType policy)
        {
            try
            {
                rpcProxy.ShowPolicyTooltip(policy.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.ShowUpgradesMenu(int cellX, int cellY)
        {
            try
            {
                rpcProxy.ShowUpgradesMenu(cellX, cellY);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.CloseUpgradesMenu()
        {
            try
            {
                rpcProxy.CloseUpgradesMenu();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.ShowUpgradeTooltip(int cellX, int cellY, UpgradeType upgrade)
        {
            try
            {
                rpcProxy.ShowUpgradeTooltip(cellX,cellY, upgrade.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IEnercitiesExamineActions.CloseUpgradeTooltip()
        {
            try
            {
                rpcProxy.CloseUpgradeTooltip();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }


        void IEnercitiesExamineActions.ClosePolicyTooltip()
        {
            try
            {
                rpcProxy.ClosePolicyTooltip();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void EmoteCommonMessages.IApplicationActions.RunAction(string actionName, string parameters)
        {
            try
            {
                string[] paramsSplit = new string[0];
                int x = -1;
                int y = -1;
                if (parameters.Trim().Length > 0)
                {
                    paramsSplit = parameters.Trim().Split(',');
                    for (int i = 0; i < paramsSplit.Length; i++) paramsSplit[i] = paramsSplit[i].Trim();
                }
                switch (actionName)
                {
                    case "ShowBuildMenuTooltip":
                        rpcProxy.ShowBuildMenuTooltip(paramsSplit[0]);
                        break;
                    case "CloseBuildMenuTooltip":
                        rpcProxy.CloseBuildMenuTooltip();
                        break;
                    case "SelectBuildingMenuTool":
                        rpcProxy.SelectBuildingMenuTool(paramsSplit[0]);
                        break;
                    case "UnselectBuildingMenuTool":
                        rpcProxy.UnselectBuildingMenuTool();
                        break;
                    case "ShowPoliciesMenu":
                        rpcProxy.ShowPoliciesMenu();
                        break;
                    case "ClosePoliciesMenu":
                        rpcProxy.ClosePoliciesMenu();
                        break;
                    case "ShowPolicyTooltip":
                        rpcProxy.ShowPolicyTooltip(paramsSplit[0]);
                        break;
                    case "ClosePolicyTooltip":
                        rpcProxy.ClosePolicyTooltip();
                        break;
                    case "ShowUpgradesMenu":
                        x = int.Parse(paramsSplit[0]);
                        y = int.Parse(paramsSplit[1]);
                        rpcProxy.ShowUpgradesMenu(x, y);
                        break;
                    case "CloseUpgradesMenu":
                        rpcProxy.CloseUpgradesMenu();
                        break;
                    case "ShowUpgradeTooltip":
                        x = int.Parse(paramsSplit[0]);
                        y = int.Parse(paramsSplit[1]);
                        rpcProxy.ShowUpgradesMenu(x, y);
                        break;
                    case "CloseUpgradeTooltip":
                        rpcProxy.CloseUpgradeTooltip();
                        break;
                    case "PreviewBuildCell":
                        x = int.Parse(paramsSplit[1]);
                        y = int.Parse(paramsSplit[2]);
                        rpcProxy.PreviewBuildCell(paramsSplit[0], x, y);
                        break;
                    case "ConfirmConstruction":
                        x = int.Parse(paramsSplit[1]);
                        y = int.Parse(paramsSplit[2]);
                        rpcProxy.ConfirmConstruction(paramsSplit[0], x, y);
                        break;
                    case "ImplementPolicy":
                        rpcProxy.ImplementPolicy(paramsSplit[0]);
                        break;
                    case "PerformUpgrade":
                        x = int.Parse(paramsSplit[1]);
                        y = int.Parse(paramsSplit[2]);
                        rpcProxy.PerformUpgrade(paramsSplit[0], x, y);
                        break;
                    case "SkipTurn":
                        rpcProxy.SkipTurn();
                        break;
                }
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Stop()
        {
            try
            {
                //rpcProxy.Stop();      // The stop message is being used to stop the perceptions client. For enercities we have the IEnercitiesGameStateActions.EndGameTimeout() event.
                                        // This event is sent to enercities thorugh the rpcProxy message rpcProxy.Stop. 
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Reset()
        {
            try
            {
                rpcProxy.Reset();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            try
            {
                rpcProxy.SetLearnerInfo(LearnerInfo_learnerInfo);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Start(string StartMessageInfo_info)
        {
            try
            {
                var info = JsonSerializable.DeserializeFromJson<StartMessageInfo>(StartMessageInfo_info);
                rpcProxy.Start(info.Students[0].thalamusLearnerId, info.Students[1].thalamusLearnerId, info.Students[0].firstName, info.Students[1].firstName, info.Language.ToString());
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void EndGameTimeout()
        {
            try
            {
                rpcProxy.Stop();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }
    }
}
