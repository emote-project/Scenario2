/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using CookComputing.XmlRpc;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace Thalamus
{
    public class ThalamusClient : Manager, Thalamus.IThalamusClient
    {
        public static bool IsLinux
        {
            get
            {
                int p = (int)System.Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

		public delegate void ClientConnectedHandler ();
		public event ClientConnectedHandler ClientConnected;
		private void NotifyClientConnected ()
		{
			if (ClientConnected != null)
				ClientConnected ();
		}

        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler ClientDisconnected;
        private void NotifyClientDisconnected()
        {
            if (ClientDisconnected != null)
                ClientDisconnected();
        }

        public delegate void NewClientConnectedHandler(string name, string newClientId);
        public event NewClientConnectedHandler NewClientConnected;
        private void NotifyNewClientConnected(string name, string newClientId)
        {
            if (NewClientConnected != null)
                NewClientConnected(name, newClientId);
        }

        public delegate void ClientDisconnectedFromThalamusHandler(string name, string oldClientId);
        public event ClientDisconnectedFromThalamusHandler ClientDisconnectedFromThalamus;
        private void NotifyClientDisconnectedFromThalamus(string name, string oldClientId)
        {
            if (ClientDisconnectedFromThalamus != null)
                ClientDisconnectedFromThalamus(name, oldClientId);
        }

        public delegate void EventInformationChangedHandler();
        public event EventInformationChangedHandler EventInformationChanged;
        public void NotifyEventInformationChanged()
        {
            if (EventInformationChanged != null) EventInformationChanged();
        }

		IThalamusServerRpc fastProxy;
		IThalamusServerRpc slowProxy;
			
		private ThalamusPublisher publisher;

		virtual protected dynamic Publisher {
			get { return publisher; }
		}

		private string name = "UnnamedThalamusClient";

		public string Name {
			get { return name; }
		}

		private int fastLocalPort = Properties.Settings.Default.CharacterStartPort;
		public int LocalPort {
			get { return fastLocalPort; }
		}

        private int slowLocalPort = Properties.Settings.Default.CharacterStartPort;
        public int SlowPort
        {
            get { return slowLocalPort; }
        }

        private Dictionary<string, PML> eventInfo = new Dictionary<string, PML>();
        public Dictionary<string, PML> EventInfo
        {
            get { return eventInfo; }
        }

        private Dictionary<String, PML> allEventsInfo = new Dictionary<string, PML>();
        public Dictionary<String, PML> AllEventsInfo
        {
            get { return allEventsInfo; }
        }

        List<String> subscribedEvents = new List<String> ();
		public List<String> SubscribedEvents {
			get { return subscribedEvents; }
		}

        List<String> announcedEvents = new List<String>();
        public List<String> AnnouncedEvents
        {
            get { return announcedEvents; }
        }

        bool connected = false;
        public bool IsConnected
        {
            get { return connected; }
        }
		bool connecting = false;
        bool shutdown = false;

        public bool Shutingdown
        {
            get { return shutdown; }
        }

		public bool Running {
			get {
				return !shutdown;
			}
		}

		bool serviceRunningSlow = false;
        bool serviceRunningFast = false;

		public bool UpdateEvents = false;
		UdpClient udpClient;
		private Thread autoDiscoveryThread;
		private Thread fastHttpListenerThread;
        private Thread slowHttpListenerThread;
        private Thread receiveFastHttpRequestsDispatcherThread;
        private Thread receiveSlowHttpRequestsDispatcherThread;
		private Thread receiveMessagesDispatcherThread;
        private Thread sendMessagesDispatcherThread;
		HttpListener fastListener;
        HttpListener slowListener;
		System.Timers.Timer disconnectedTimer = new System.Timers.Timer ();
		Dictionary<string, PML> perceptionQueue = new Dictionary<string, PML>();
		List<PML> publishQueue = new List<PML> ();
		string clientId = Guid.NewGuid ().ToString ();
		string characterName = "";
        string masterAddress = "localhost";
        int masterFastPort = Properties.Settings.Default.CharacterStartPort;
        int masterSlowPort = Properties.Settings.Default.CharacterStartPort;
		public bool Silent = false;
		private bool started = false;
		private bool publishing = false;
		private bool processingFastRequests = false;
        private bool processingSlowRequests = false;
        private bool processMultipleSimultaneousRequests = Properties.Settings.Default.ProcessMultipleSimultaneousRequests;
        //int broadcastPort = 9050;
        protected bool showDebugErrors = true;

        List<HttpListenerContext> fastHttpRequestsQueue = new List<HttpListenerContext>();
        List<HttpListenerContext> slowHttpRequestsQueue = new List<HttpListenerContext>();

		private string manualMasterAddress = "";


        private long localTimeShift = 0;
        public long LocalTimeShift
        {
            get { return localTimeShift; }
            set { 
                localTimeShift = value;
                SetCentralTimeShift(localTimeShift);
            }
        }
        

		private int receivedMessages = 0;
		public int StatisticsReceivedMessages {
			get {
				return receivedMessages;
			}
		}

		private int publishedMessages = 0;
		public int StatisticsPublishedMessages {
			get {
				return publishedMessages;
			}
		}


		public ThalamusClient(string name, string characterName, bool autoStart = true) : this(name, characterName, "", autoStart) {}
        public ThalamusClient(string name, string characterName, string manualMaster, bool autoStart = true)
        : base(name)
		{
			manualMasterAddress = manualMaster;
            PML.Setup();
			this.characterName = characterName;
            if (name == "")
                this.name = this.GetType().Name;
            else
                this.name = name;
			ThreadPool.SetMaxThreads (5000, 5000);

            if (autoStart)
            {
                Start();
            }
		}

		public ThalamusClient () : this("")
		{
            PML.Setup();
		}


		public ThalamusClient (string name, bool autoStart = true) :base(name)
		{
            PML.Setup();
			if (name == "")
				this.name = this.GetType().Name;
			else
				this.name = name;

			ThreadPool.SetMaxThreads (5000, 5000);

			if (autoStart) {
				Start ();
			}

		}

		public void ResetStatistics() {
			publishedMessages = 0;
			receivedMessages = 0;
		}

		public void PrintStatistics() {
			Debug ("Published messages: '{0}'. Received messages: '{1}'.", publishedMessages, receivedMessages);
		}

        SocketPermission acceptSocketPermission = new SocketPermission(NetworkAccess.Accept, TransportType.All, "", SocketPermission.AllPorts);
        SocketPermission connectSocketPermission = new SocketPermission(NetworkAccess.Connect, TransportType.All, "", SocketPermission.AllPorts);

		public override bool Start ()
		{
			if (started)
				return true;

            base.Start();

			CollectSubscriptionInfo ();

            acceptSocketPermission.Union(connectSocketPermission).Demand();

			autoDiscoveryThread = new Thread (new ThreadStart (AutoDiscoveryClient));
			autoDiscoveryThread.Start ();

			fastHttpListenerThread = new Thread (new ThreadStart (FastHttpListenerThread));
			fastHttpListenerThread.Start ();

            slowHttpListenerThread = new Thread(new ThreadStart(SlowHttpListenerThread));
            slowHttpListenerThread.Start();

            receiveFastHttpRequestsDispatcherThread = new Thread(new ThreadStart(ReceiveFastHttpRequestsDispatcher));
            receiveFastHttpRequestsDispatcherThread.Start();

            receiveSlowHttpRequestsDispatcherThread = new Thread(new ThreadStart(ReceiveSlowHttpRequestsDispatcher));
            receiveSlowHttpRequestsDispatcherThread.Start();

			receiveMessagesDispatcherThread = new Thread (new ThreadStart (ReceiveMessagesDispatcher));
			receiveMessagesDispatcherThread.Start ();

            sendMessagesDispatcherThread = new Thread(new ThreadStart(SendMessagesDispatcher));
            sendMessagesDispatcherThread.Start();

			disconnectedTimer.Interval = Properties.Settings.Default.PingInterval * 2;
			disconnectedTimer.Elapsed += new ElapsedEventHandler (TimerCheckDisconnected);
			disconnectedTimer.Enabled = false;
			started = true;
            return true;
		}

        public void WaitUntilShutdown()
        {
            while (!shutdownComplete) Thread.Sleep(100);
        }

        

		public void Shutdown ()
		{
			Thread shutdownThread = new Thread (new ThreadStart (Dispose));
			shutdownThread.Start ();
		}

        private bool shutdownComplete = false;
		public override void Dispose ()
		{
            
            if (IsConnected)
            {
                Debug("Sending Master a Disconnect message...");
                SendMessage(() => slowProxy.Disconnect(clientId), "Disconnect");
                Debug("Sent disconnect message.");
            }
            shutdown = true;
            Debug("Set shutdown flag.");
			disconnectedTimer.Stop ();
			if (autoDiscoveryThread != null)
				autoDiscoveryThread.Join ();
			try 
            {
                if (fastListener != null) fastListener.Stop();
			} 
            catch {}
            Debug("Stopped Fast HTTP listener.");

            try
            {
                if (slowListener != null) slowListener.Stop();
            }
            catch { }
            Debug("Stopped Slow HTTP listener.");

            try
            {
                if (fastHttpListenerThread != null) fastHttpListenerThread.Join();
            }
            catch { }

            try
            {
                if (slowHttpListenerThread != null) slowHttpListenerThread.Join();
            }
            catch { }

            try
            {
                if (receiveFastHttpRequestsDispatcherThread != null) receiveFastHttpRequestsDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (receiveSlowHttpRequestsDispatcherThread != null) receiveSlowHttpRequestsDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (sendMessagesDispatcherThread != null) sendMessagesDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (receiveMessagesDispatcherThread != null) receiveMessagesDispatcherThread.Join();
            }
            catch { }

            shutdownComplete = true;
            base.Dispose();
		}

		private void TimerCheckDisconnected (object source, ElapsedEventArgs e)
		{
			DebugError("Ping timeout!");
			Disconnect ();
		}

		internal void Disconnect ()
		{
			connected = false;
            connecting = false;
			DebugError ("Disconnected from Thalamus Master.");
			disconnectedTimer.Stop ();
            NotifyClientDisconnected();
            DisconnectedFromMaster();
		}

        public virtual void DisconnectedFromMaster() { }

 		public void AutoDiscoveryClient ()
		{
			while (!shutdown) {
				try {
					if (!connected && !connecting) {
						disconnectedTimer.Stop ();

                        Environment.AuthorizeAddress(Properties.Settings.Default.BroadcastPort, "Everyone");
						string resp="";
						string tmpMasterAddress = "";
						int tmpMasterFastPort = Properties.Settings.Default.CharacterStartPort;
						int tmpMasterSlowPort = Properties.Settings.Default.CharacterStartPort + 1;
						string[] stringData = new string[]{""};
						if (manualMasterAddress=="") {
	                        IPAddress broadcastIp = IPAddress.Parse(Properties.Settings.Default.MulticastAddress);
	                        IPEndPoint broadcastEndpoint = new IPEndPoint(IPAddress.Any, Properties.Settings.Default.BroadcastPort);

							udpClient = new UdpClient (AddressFamily.InterNetwork);
	                        udpClient.Client.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(broadcastIp, IPAddress.Any));
							udpClient.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
							udpClient.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
							udpClient.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, udpClient.Client.ReceiveTimeout = Properties.Settings.Default.BeaconInterval*2);
							udpClient.Client.Bind (broadcastEndpoint);
							byte[] data = new byte[1024];
	                        if (characterName == "") Debug("Searching for a Thalamus Character...");
	                        else Debug("Searching for Thalamus Character '{0}'...", characterName);
							data = udpClient.Receive (ref broadcastEndpoint);
							udpClient.Close ();
							resp = Encoding.ASCII.GetString (data, 0, data.Length);
							tmpMasterAddress = broadcastEndpoint.Address.ToString();
							stringData = resp.Split (' ');
							tmpMasterFastPort = int.Parse(stringData[1]);
							tmpMasterSlowPort = int.Parse(stringData[2]);
						}else
						{
							try
							{
								string[] manualMasterSplit = manualMasterAddress.Split(':');
								tmpMasterAddress = manualMasterSplit[0];
								tmpMasterFastPort = int.Parse(manualMasterSplit[1]);
								tmpMasterSlowPort = int.Parse(manualMasterSplit[2]);
							}
							catch
							{
								Debug("Invalid Manual Master setting: '{0}'", manualMasterAddress);
								Thread.Sleep(Properties.Settings.Default.BeaconInterval);
							}
						}


						if (manualMasterAddress!="" || (stringData [0] == "THALAMUS_MASTER" && stringData.Length >= 4)) {
							if (characterName == "" || manualMasterAddress!="" || (characterName != "" && characterName == stringData[3]))
                            {
                                connecting = true;
								masterAddress = tmpMasterAddress;
								masterFastPort = tmpMasterFastPort;
								masterSlowPort = tmpMasterSlowPort;
								Debug(String.Format("Found Thalamus Master for character '{0}' at '{1}:{2}/{3}'.", characterName, masterAddress, masterFastPort, masterSlowPort));
                                try
                                {
                                    while (!serviceRunningSlow && !serviceRunningFast)
                                    {
                                        Thread.Sleep(100);
                                    }

                                    if (fastProxy == null) fastProxy = XmlRpcProxyGen.Create<IThalamusServerRpc>();
                                    if (slowProxy == null) slowProxy = XmlRpcProxyGen.Create<IThalamusServerRpc>();
                                    fastProxy.Url = string.Format("http://{0}:{1}", masterAddress, masterFastPort);
                                    fastProxy.KeepAlive = true;
                                    fastProxy.Timeout = Properties.Settings.Default.FastTimeout;
                                    Debug("Set Master fast endpoint to {0}:{1}", masterAddress, masterFastPort);
                                    slowProxy.Url = string.Format("http://{0}:{1}", masterAddress, masterSlowPort);
                                    slowProxy.KeepAlive = true;
                                    slowProxy.Timeout = Properties.Settings.Default.SlowTimeout;
                                    Debug("Set Master slow endpoint to {0}:{1}", masterAddress, masterSlowPort);


                                    Debug("Connecting...");
                                    if (SendMessage(() => slowProxy.Connect(Name, clientId, fastLocalPort, slowLocalPort, subscribedEvents.Count), "Connect"))
                                    {
                                        connected = true;
                                        Debug("Connected with clientId '" + clientId + "'");
                                        disconnectedTimer.Start();
                                    }
                                    else
                                    {
                                        DebugError("Failed to connect to Master@" + slowProxy.Url + ".");
                                        connecting = false;
                                    }

                                }
                                catch (Exception e)
                                {
                                    DebugException(e, 1);
                                    connecting = false;
                                }
                            }
                            else
                            {
                                Debug("Found Thalamus Character '{0}'", stringData[3]);
                                Thread.Sleep(Properties.Settings.Default.BeaconInterval);
                            }

						}

					} else {
						Thread.Sleep (1000);
					}
				} catch (Exception e) {
                    if (characterName == "") Debug("Unable to find a Thalamus Character.");
                    else Debug("Unable to find Thalamus Character '{0}'.", characterName);
					//DebugException(e, 2);
					connecting = false;
					Thread.Sleep (1000);
				}
			}
			Debug("Terminated AutoDiscoveryClient");
		}


        public void FastHttpListenerThread()
        {
            while (!shutdown)
            {
                while (!serviceRunningFast)
                {
                    try
                    {
                        SocketPermission sp = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts);
                        sp.Demand();
						Debug("Attempt to start service on port '" + fastLocalPort + "'");
                        fastListener = new HttpListener();
                        Environment.AuthorizeAddress(fastLocalPort, "Everyone");
                        fastListener.Prefixes.Add(string.Format("http://*:{0}/", fastLocalPort));
                        fastListener.Start();
						Debug("fast XMLRPC Listening on port " + fastLocalPort);
                        serviceRunningFast = true;
                    }
                    catch (HttpListenerException ex)
                    {
                        if (ex.ErrorCode == 5)
                        {
                            DebugError("Permission denied to open socket.\nPlease run thi program in Administrator mode.");
                            return;
                        }
                        else
                        {
                            fastLocalPort++;
                            Debug("Port unavaliable.");
                            serviceRunningFast = false;
                        }
                    }
                }

                try
                {
					HttpListenerContext context = fastListener.GetContext();
					lock(fastHttpRequestsQueue) {
						fastHttpRequestsQueue.Add(context);
					}
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("thread exit")) DebugException(e);
                    serviceRunningFast = false;
                    fastListener = null;
                }
            }
            if (fastListener != null) fastListener.Close();
            Debug("Terminated FastHttpListenerThread");
        }

        public void SlowHttpListenerThread()
        {
            while (!shutdown)
            {
                while (!serviceRunningSlow)
                {
                    try
                    {
                        Debug("Attempt to start Slow service on port '" + slowLocalPort + "'");
                        slowListener = new HttpListener();
                        Environment.AuthorizeAddress(slowLocalPort, "Everyone");
                        slowListener.Prefixes.Add(string.Format("http://*:{0}/", slowLocalPort));
                        slowListener.Start();
                        Debug("Slow XMLRPC Listening on port " + slowLocalPort);
                        serviceRunningSlow = true;
                    }
                    catch (HttpListenerException ex)
                    {
                        if (ex.ErrorCode == 5)
                        {
                            DebugError("Permission denied to open socket.\nPlease run this program in Administrator mode.");
                            return;
                        }
                        else
                        {
                            slowLocalPort++;
                            Debug("Slow Port unavaliable.");
                            serviceRunningSlow = false;
                        }
                    }
                }

                try
                {
                    HttpListenerContext context = slowListener.GetContext();
                    lock (slowHttpRequestsQueue)
                    {
                        slowHttpRequestsQueue.Add(context);
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("thread exit")) DebugException(e);
                    serviceRunningSlow = false;
                    slowListener = null;
                }
            }
            if (slowListener != null) slowListener.Close();
            Debug("Terminated SlowHttpListenerThread");
        }

        public void ProcessFastRequest(object oContext)
        {
            HttpListenerContext httpRequest = (HttpListenerContext)oContext;
            try
            {
                XmlRpcListenerService svc = new ThalamusClientService(this);
                svc.ProcessRequest((HttpListenerContext)httpRequest);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
            processingFastRequests = false;
        }

		public void ProcessFastRequests(object oContext) {
			List<HttpListenerContext> httpRequests = (List<HttpListenerContext>) oContext;
			foreach (HttpListenerContext context in httpRequests) {
				try {
					XmlRpcListenerService svc = new ThalamusClientService (this);
					svc.ProcessRequest ((HttpListenerContext)context);
				} catch (Exception e) { 
					DebugException (e);
				}
			}
			processingFastRequests = false;
		}

        public void ProcessSlowRequests(object oContext)
        {
            List<HttpListenerContext> httpRequests = (List<HttpListenerContext>)oContext;
            foreach (HttpListenerContext context in httpRequests)
            {
                try
                {
                    XmlRpcListenerService svc = new ThalamusClientService(this);
                    svc.ProcessRequest((HttpListenerContext)context);
                }
                catch (Exception e)
                {
                    DebugException(e);
                }
            }
            processingSlowRequests = false;
        }

		private int sendRetries = 5;
		internal bool SendMessage(Action action, string description = "") {
			bool sent = false;
			int tries = 0;
			while(!sent && tries<sendRetries && !shutdown) {
				try
				{
					action();
					sent=true;
				}
				catch (Exception e) {
                    DebugException(e);
					tries++;
					Thread.Sleep(100); 
				}
			}
            if (tries >= sendRetries)
            {
                if (description == "") DebugError("Unable to execute anonymous remote action.");
                else DebugError("Unable to execute remote action: '{0}'", description);
            }
            else
            {
                publishedMessages++;
            }
			return sent;
		}

        List<string> messageIdRecentHistory = new List<string>();

        public static List<string>[] PartitionList(List<string> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<string>[] partitions = new List<string>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<string>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }
            return partitions;
        }

        public void SendMessagesDispatcher()
        {
            while (!shutdown)
            {
                bool performSleep = true;
                try
                {
                    if (publishQueue.Count > 0 && !publishing)
                    {
                        publishing = true;
                        performSleep = false;
                        List<PML> publishEvents;
                        lock (publishQueue)
                        {
                            publishEvents = new List<PML>(publishQueue);
                            publishQueue.Clear();
                        }

                        try
                        {
                            PublishEvents(publishEvents);

                        }
                        catch (Exception e)
                        {
                            DebugException(e, 1);
                        }
                    }

                    if (UpdateEvents)
                    {
                        UpdateEvents = false;
                        PerformRegistration();
                        performSleep = false;
                    }
                }
                catch (Exception e)
                {
                    DebugException(e, 2);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated SendMessagesDispatcher");
        }

        public void ReceiveFastHttpRequestsDispatcher()
        {
            while (!shutdown)
            {
                bool performSleep = true;
                try
                {
                    if (fastHttpRequestsQueue.Count > 0 && (processMultipleSimultaneousRequests || !processingFastRequests))
                    {
                        processingFastRequests = true;
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (fastHttpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(fastHttpRequestsQueue);
                            fastHttpRequestsQueue.Clear();
                        }

                        try
                        {
                            (new Thread(new ParameterizedThreadStart(ProcessFastRequests))).Start(httpRequests);
                        }
                        catch (Exception e)
                        {
                            DebugException(e, 1);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugException(e, 4);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated ReceiveFastHttpRequestsDispatcher");
        }

        public void ReceiveSlowHttpRequestsDispatcher()
        {
            while (!shutdown)
            {
                bool performSleep = true;
                try
                {
                    if (slowHttpRequestsQueue.Count > 0 && !processingSlowRequests)
                    {
                        processingSlowRequests = true;
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (slowHttpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(slowHttpRequestsQueue);
                            slowHttpRequestsQueue.Clear();
                        }

                        try
                        {
                            (new Thread(new ParameterizedThreadStart(ProcessSlowRequests))).Start(httpRequests);
                        }
                        catch (Exception e)
                        {
                            DebugException(e, 1);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugException(e, 4);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated ReceiveSlowHttpRequestsDispatcher");
        }

        public void ReceiveMessagesDispatcher()
        {
            while (!shutdown)
            {
				bool performSleep = true;
                try
                {
                    if (messageIdRecentHistory.Count > 100)
                    {
                        List<string>[] partitions = PartitionList(messageIdRecentHistory, 2);
                        messageIdRecentHistory = partitions[1];
                    }

					if (perceptionQueue.Count>0) {
						performSleep=false;
						List<PML> perceptionEvents;
						lock (perceptionQueue)
						{
							perceptionEvents = new List<PML>(perceptionQueue.Values);
							perceptionQueue.Clear();
						}
						foreach (PML ev in perceptionEvents)
	                    {
                            if (!messageIdRecentHistory.Contains(ev.Id))
                            {
                                try
                                {
                                    messageIdRecentHistory.Add(ev.Id);
                                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveEvent),ev);
                                    (new Thread(new ParameterizedThreadStart(ReceiveEvent))).Start(ev);

                                }
                                catch (Exception e)
                                {
                                    DebugException(e, 2);
                                }
                            }
	                    }
					}
                }
                catch (Exception e)
                {
					DebugException(e, 4);
                }
				if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated ReceiveMessagesDispatcher");
        }



        #region IThalamusClient Members

        public string GetExecutionCommand()
        {
            return System.Environment.GetCommandLineArgs()[0];
        }

        public string GetExecutionParameters()
        {
            string cmdParams = "";
            int i = 0;
            foreach (string s in System.Environment.GetCommandLineArgs()) if (i++ > 0) cmdParams += s + " ";
            return cmdParams;
        }


        public void PingSync(long centralTime, long timeZone)
        {
            LocalTimeShift = (centralTime - StartTime.ElapsedMilliseconds) + timeZone;
            disconnectedTimer.Stop();
            disconnectedTimer.Start();
            if (!SendMessage(() => slowProxy.PongSync(clientId, centralTime), "PongSync"))
            {
                DebugError("Failed to Pong!");
                Disconnect();
            }
        }

		#endregion


		#region New Subscribing based on Interfaces

		public void SetPublisher<IPublisher>() {
			publisher = new ThalamusPublisher(typeof(IPublisher), this);
			Type[] interfaces = publisher.Interface.GetInterfaces();
			foreach (Type t in interfaces) {
				if (ImplementsTypeDirectly(t, typeof(IAction))) {
					Debug("Found Action interface: '" + t.FullName + "'");
					AnnounceInterfaceActions (t, publisher);
				}
				else if (ImplementsTypeDirectly(t, typeof(IPerception))) {
					Debug("Found Perception interface: '" + t.FullName + "'");
					AnnounceInterfacePerceptions (t, publisher);
				}
			}
		}
        private Dictionary<string, object> ExtraInterfacesTargets = new Dictionary<string, object>();

        protected void RegisterExtraInterface(Type extraInterface, object target)
        {
            if (ImplementsTypeDirectly(extraInterface, typeof(IAction)))
            {
                Debug("Registered runtime Action interface: '" + extraInterface.FullName + "'");
                SubscribeInterfaceActions(extraInterface);
                ExtraInterfacesTargets[extraInterface.Name] = target;
                UpdateEvents = true;
            }
            else if (ImplementsTypeDirectly(extraInterface, typeof(IPerception)))
            {
                Debug("Registered runtime Perception interface: '" + extraInterface.FullName + "'");
                SubscribeInterfacePerceptions(extraInterface);
                ExtraInterfacesTargets[extraInterface.Name] = target;
                UpdateEvents = true;
            }
        }

		private void CollectSubscriptionInfo () {
			Type[] interfaces = this.GetType().GetInterfaces();
			subscribedEvents.Add ("IThalamusClient.Connected");
			foreach (Type t in interfaces) {
				if (ImplementsTypeDirectly(t, typeof(IAction))) {
					Debug("Found Action interface: '" + t.FullName + "'");
					SubscribeInterfaceActions (t);
				}
				else if (ImplementsTypeDirectly(t, typeof(IPerception))) {
					Debug("Found Perception interface: '" + t.Name + "'");
					SubscribeInterfacePerceptions (t);
				}
			}
		}

		private void SubscribeInterfaceActions (Type t)
		{
			MethodInfo[] actions = t.GetMethods ();
			foreach (MethodInfo action in actions) {
				string name = t.Name + "." + action.Name;
                RegisterEventInfo(name, action, true, true);
			}
		}

		private void SubscribeInterfacePerceptions (Type t)
		{
			MethodInfo[] perceptions = t.GetMethods ();
			foreach (MethodInfo perception in perceptions) {
				string name =  t.Name + "." + perception.Name;
				RegisterEventInfo (name, perception, true, false);
			}
		}

		private void AnnounceInterfaceActions (Type t, ThalamusPublisher publisher)
		{
			MethodInfo[] actions = t.GetMethods ();
			foreach (MethodInfo action in actions) {
				string name = t.Name + "." + action.Name;
                RegisterEventInfo(name, action, false, true);
				publisher.AddMethod (action);
			}
		}

		private void AnnounceInterfacePerceptions (Type t, ThalamusPublisher publisher)
		{
			MethodInfo[] perceptions = t.GetMethods ();
			foreach (MethodInfo perception in perceptions) {
				string name =  t.Name + "." + perception.Name;
				RegisterEventInfo (name, perception, false, false);
				publisher.AddMethod (perception);
			}
		}

		private void RegisterEventInfo(string name, MethodInfo method, bool subscription, bool isAction) 
		{
			try {
				PML pml = new PML(name, method);
                pml.EventType = isAction ? PMLEventType.Action : PMLEventType.Perception;
				RegisterEventInfo (name, pml, subscription);
			}catch (Exception e) {
				DebugException(e);
			}
		}
        private void RegisterEventInfo(string name, PML pml, bool subscription)
        {
            lock (eventInfo)
            {
                eventInfo[name] = pml;
            }
            if (subscription)
            {
                lock (subscribedEvents)
                {
                    if (!subscribedEvents.Contains(name)) subscribedEvents.Add(name);
                    Debug("Subscribed to event '" + name + "'");
                }
            }
            else
            {
                lock (subscribedEvents)
                {
                    if (!announcedEvents.Contains(name)) announcedEvents.Add(name);
                    Debug("Announced event '" + name + "'");
                }
            }
        }
        /*
		private void RegisterEventInfo(string name, string[] parameters, string[] types, bool subscription) 
		{
			if (parameters.Length != types.Length)
			{
				DebugError("Unable to add event information for '" + name + "'! Parameters and Types do not have the same length.");
				return;
			}
            lock (eventParameters)
            {
                eventParameters[name] = parameters;
            }
			eventParameterTypes[name] = types;

            
		}*/

		static public bool ImplementsTypeDirectly(Type t, Type baseType) {
			if (t.BaseType != null && ImplementsTypeDirectly(t.BaseType, baseType)) { 
				return false; 
			}
			foreach (var intf in t.GetInterfaces()) {
				if (ImplementsTypeDirectly(intf, baseType)) { 
					return false;
				}
			}
			return t.GetInterfaces().Any(i => i == baseType);
		}

		#endregion


		#region Auxiliary

		private int ParameterIndex(ParameterInfo[] parameters, string paramName)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].Name == paramName) return i;
			}
			return -1;
		}

 		#endregion

		#region Publication and Reception of events

		int queuedReceiveEvents = 0;
        public void QueueEvent(string messageId, string eventName, string[] parameters, string[] types, string[] values, bool dontLogDescription = false)
        {
			if (perceptionQueue.ContainsKey (messageId))
				return;
            PML pml = new PML(eventName, parameters, types, values);
            
            pml.DontLogDescription = dontLogDescription;
            lock (perceptionQueue)
            {
                perceptionQueue[messageId] = pml;
				queuedReceiveEvents++;
                DebugIf("messages", "[" + Name + "][#" + queuedReceiveEvents + "]Queued reception of " + pml.ToString() + ".");
            }
        }

        public void QueueEvents(string[] messageIds, string[] eventNames, string[][] allParameters, string[][] allTypes, string[][] allValues, bool[] dontLogDescriptions) 
        {
            for (int i = 0; i < messageIds.Length; i++)
            {
                QueueEvent(messageIds[i], eventNames[i], allParameters[i], allTypes[i], allValues[i], dontLogDescriptions[i]);
            }
        }

        int queuedPublishEvents = 0;
		public void QueuePublishedEvent(PML ev)
		{
            if (!IsConnected)
            {
                DebugLog("UNCONNECTED_PUBLISH:" + ev.ToString());
                return;
            }
			queuedPublishEvents++;
			lock (publishQueue)
			{
				publishQueue.Add(ev);
				DebugIf("messages", "[" + Name + "][#" + queuedPublishEvents + "]Queued publication of " + ev.ToString() + ".");
			}
		}

        private List<String> extendedInterfaces = new List<String>();
        protected void RegisterExtendedInterface(string interfaceName)
        {
            if (!extendedInterfaces.Contains(interfaceName)) extendedInterfaces.Add(interfaceName);
        }

        protected virtual void ReceiveExtendedInterfaceMessage(PML p) {}
        

        public void ReceiveEvent(object o)
        {
			int eventId;

			receivedMessages++;
			eventId = receivedMessages;

			PML ev = (PML) o;

			string name = ev.Name;
			Type targetType = this.GetType();
            object target = this;
			string[] call;
			MethodInfo callingMethod;
			ParameterInfo[] paramInfo;
			object[] oParameters;
            LogEvent(false, ev);
			DebugIf("messages", "[" + Name + "][#" + eventId + "]Receiving...");
			try
			{
                call = name.Split('.');
                if (targetType.GetInterface(call[0]) == null && ExtraInterfacesTargets.ContainsKey(call[0]))
                {
                    //targetType = ExtraInterfacesTargets[call[0]].Key;
                    target = ExtraInterfacesTargets[call[0]];
                    targetType = target.GetType();
                }

				if (targetType.GetInterface(call[0])!=null) {
					callingMethod = targetType.GetInterface(call[0]).GetMethod(call[1]);
					if (callingMethod != null)
					{

						paramInfo = callingMethod.GetParameters();
						oParameters = new object[callingMethod.GetParameters().Length];

						foreach(KeyValuePair<string, PMLParameter> param in ev.Parameters) {
							int j = ParameterIndex(paramInfo, param.Key);
							if (j < 0 || oParameters.Length<(j+1))
							{
                                if (showDebugErrors)
                                    DebugError("Unable to find parameter " + param.Key + " in method " + name + " in class " + targetType.Name + "!");
								return;
							}
                            else oParameters[j] = DataTypes.PMLObjectToSystemObject(param.Value);
						}
						DebugIf("messages", "[" + Name + "][#" + eventId + "]Received " + ev.ToString() + ".");
                        callingMethod.Invoke(target, oParameters);
					}
					else
					{
                        if (showDebugErrors)
                            DebugError("Unable to find method " + name + " in interface " + call[0] + " in class " + targetType.Name + "!");
					}
				} else
				{
                    if (extendedInterfaces.Contains(call[0]))
                    {
                        ReceiveExtendedInterfaceMessage(ev);
                    }
                    else
                    {
                        if (showDebugErrors)
                            DebugError("Unable to find interface " + call[0] + " in class " + targetType.Name + "!");
                    }
				}

			}
			catch (Exception e)
			{
				DebugException(e);
			}
        }



        public void PerformRegistration()
        {
            connecting = false;

            Debug("Performing registration...");

            if (!SendMessage(() => slowProxy.ClearSubscriptions(clientId), "ClearSubscriptions"))
            {
                DebugError("Failed to clear subscriptions.'");
            }
            else
            {
                Debug("Cleared subscriptions.");
            }



            Debug("Announcing event information...");
            string[] perceptionNames;
            bool[] isAction;
            string[][] allParameters;
            string[][] allTypes;
            Dictionary<string, List<string>> enumsDict = new Dictionary<string, List<string>>();
            string[][] enums;
            string[] enumNames;
			lock (eventInfo) {

                perceptionNames = new string[eventInfo.Count];
                isAction = new bool[eventInfo.Count];
                allParameters = new string[eventInfo.Count][];
                allTypes = new string[eventInfo.Count][];

                int i = 0;
                foreach (PML pml in EventInfo.Values)
                {
                    perceptionNames[i] = pml.Name;
                    allParameters[i] = pml.ParameterNames;
                    allTypes[i] = pml.ParameterTypes;
                    isAction[i] = pml.EventType == PMLEventType.Action;
                    for (int j = 0; j < allTypes[i].Length; j++)
                    {
                        if (allTypes[i][j].StartsWith("Enum."))
                        {
                            enumsDict[allTypes[i][j]] = DataTypes.ExtractEnumMembers(allTypes[i][j]);
                        }
                    }
                    i++;
                }

                enums = new string[enumsDict.Count][];
                enumNames = new string[enumsDict.Count];
                int m = 0;
                foreach (string enumName in enumsDict.Keys)
                {
                    enumNames[m] = enumName;
                    enums[m] = new string[enumsDict[enumName].Count];
                    int n = 0;
                    foreach (string enumMember in enumsDict[enumName])
                    {
                        enums[m][n] = enumMember;
                        n++;
                    }
                    m++;
                }
            }

            if (!SendMessage(() => slowProxy.AnnounceEventInformation(clientId, perceptionNames, isAction, allParameters, allTypes, enumNames, enums), "AnnounceEventInformation"))
            {
                DebugError("Failed to announce event information.");
            }
            else
            {
                Debug("Done.");
            }

			Debug("Announcing and Subscribing to events...");
            string[] a_subscribedEvents;
            string[] a_announcedEvents;
            lock (subscribedEvents)
            {
                a_subscribedEvents = subscribedEvents.ToArray();
            }
            lock (announcedEvents)
            {
                a_announcedEvents = announcedEvents.ToArray();
            }
            if (!SendMessage(() => slowProxy.RegisterEvents(clientId, announcedEvents.ToArray(), subscribedEvents.ToArray()), "RegisterEvents"))
            {
                DebugError("Failed to register to events.");
            }
			Debug("Done.");
            UpdateEvents = false;
        }

		internal void PublishEvents(object o)
		{
			List<PML> publishEvents = (List<PML>)o;

            string[] eventIds;
            string[] clientIds;
            string[] eventNames;
            bool[] dontLogDescriptions;
            string[][] allParameters;
            string[][] allTypes;
            string[][] allValues;

            eventIds = new string[publishEvents.Count];
            clientIds = new string[eventIds.Length];
            eventNames = new string[eventIds.Length];
            dontLogDescriptions = new bool[eventIds.Length];
            allParameters = new string[eventNames.Length][];
            allTypes = new string[eventNames.Length][];
            allValues = new string[eventNames.Length][];

            int k = 0;
            foreach (PML e in publishEvents)
            {
                eventIds[k] = Guid.NewGuid().ToString();
                clientIds[k] = clientId;
                try
                {
                    string[] parameters = new string[e.Parameters.Count];
                    string[] values = new string[e.Parameters.Count];
                    string[] types = new string[e.Parameters.Count];
                    int i = 0;
                    foreach (KeyValuePair<string, PMLParameter> param in e.Parameters)
                    {
                        parameters[i] = param.Key;
                        values[i] = param.Value.ToString();
                        if (param.Value.Type == PMLParameterType.Enum) types[i] = "Enum." + param.Value.EnumName;
                        else types[i] = param.Value.Type.ToString();
                        i++;
                    }
                    eventNames[k] = e.Name;
                    allParameters[k] = parameters;
                    allValues[k] = values;
                    allTypes[k]=types;
                    dontLogDescriptions[k]=e.DontLogDescription;
                    k++;
                    LogEvent(true, e);
                }
                catch (Exception ex)
                {
                    DebugException(ex);
                }
            }
            try
            {
                if (!SendMessage(() => fastProxy.PublishEvents(eventIds, clientIds, eventNames, dontLogDescriptions, allParameters, allTypes, allValues), "PublishEvents"))
                {
                    DebugError("Failed to publish events.");
                }
                else
                {
                    DebugIf("messages", "Published {0} events.", eventIds.Length);
                }
            }
            catch (Exception ex)
            {
                DebugException(ex);
            }
			/*foreach (ThalamusEvent e in publishEvents)
			{
				try {
					string[] parameters = new string[e.Parameters.Count];
					string[] values = new string[e.Parameters.Count];
					string[] types = new string[e.Parameters.Count];
					int i = 0;
					foreach (KeyValuePair<string, EventParameter> param in e.Parameters)
					{
						parameters[i] = param.Key;
						values[i] = param.Value.ToString();
                        if (param.Value.Type == EventParameterType.Enum) types[i] = "Enum." + param.Value.EnumName;
						else types[i] = param.Value.Type.ToString();
						i++;
					}
                    LogEvent(true, e);
					if (!SendMessage (() => fastProxy.PublishEvent(Guid.NewGuid().ToString(), clientId, e.Name, e.DontLogDescription, parameters, types, values),"PublishEvent")) {
						DebugError("Failed to publish event '" + e.ToString() + "'.");
					}
				}
				catch (Exception ex)
				{
					DebugException(ex);
				}
			}*/
			publishing = false;
		}

        #endregion

        #region perception info

        public virtual void ReceiveEventInfo(string[] piNames, string[][] piParameters, string[][] piTypes)
        {
            lock (allEventsInfo)
            {
                allEventsInfo.Clear();
                for (int e = 0; e < piNames.Length; e++)
                {
                    string perceptionName = piNames[e];
                    string[] parameters = piParameters[e];
                    string[] types = piTypes[e];
                    if (parameters.Length != types.Length) return;
                    Dictionary<string, PMLParameter> perceptionParameters = new Dictionary<string, PMLParameter>();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (types[i] == PMLParameterType.Bool.ToString()) perceptionParameters[parameters[i]] = new PMLParameter(false);
                        else if (types[i] == PMLParameterType.Double.ToString()) perceptionParameters[parameters[i]] = new PMLParameter(0.0f);
                        else if (types[i] == PMLParameterType.Int.ToString()) perceptionParameters[parameters[i]] = new PMLParameter(0);
                        else if (types[i] == PMLParameterType.String.ToString()) perceptionParameters[parameters[i]] = new PMLParameter("");
                    }
                    SetPerceptionInfo(new PML(perceptionName, perceptionParameters));
                }

                lock (eventInfo)
                {
                    foreach (PML ev in eventInfo.Values)
                    {
                        SetPerceptionInfo(ev);
                    }
                }
            }
            NotifyEventInformationChanged();
        }

        internal void SetPerceptionInfo(PML p)
        {
            lock (allEventsInfo)
            {
                allEventsInfo[p.Name] = p;
            }
        }

        #endregion

		public virtual void ConnectedToMaster() {}
		
        void IThalamusClient.Connected(string uid, int port) {
            fastProxy.Url = string.Format("http://{0}:{1}", masterAddress, port);
            Debug("Set Master fast endpoint to {0}:{1}", masterAddress, port);
			NotifyClientConnected();
			ConnectedToMaster();
		}

        #region IThalamusClient Members


        void IThalamusClient.NewClientConnected(string name, string newClientId)
        {
            NotifyNewClientConnected(name, newClientId);
        }

        void IThalamusClient.ClientDisconnected(string name, string oldClientId)
        {
            ClientDisconnectedFromThalamus(name, oldClientId);
        }

        #endregion
    }
}
