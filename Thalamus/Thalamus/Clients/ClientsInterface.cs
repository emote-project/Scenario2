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
using System.Reflection;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using CookComputing.XmlRpc;
using System.Collections;
using System.Timers;
using System.Diagnostics;
using Thalamus.BML;

namespace Thalamus
{

    #region enum and delegates

    public enum BodyActions
    {
        FaceLexeme,
        FaceFacs,
        Gaze,
        Gesture,
        Head,
        Locomotion,
        Pointing,
        Posture,
        Sound,
        Speech,
        Animation
    }

    public delegate void ListeningHandler();
    public delegate void ClientInfoChangedHandler(string clientId);

    #endregion

    public class ClientsInterface : IBMLActions, IBMLCodeAction
    {
        private class PerceptionInfoResponse
        {
            public string Name;
            public string[] Parameters;
            public string[] Types;
            public PerceptionInfoResponse(string name, string[] parameters, string[] types)
            {
                Name = name;
                Parameters = parameters;
                Types = types;
            }
        }

        #region events

        public event ListeningHandler Listening;
        public void NotifyListening()
        {
            if (Listening != null) Listening();
        }

        public event ClientDisconnectedHandler ClientDisconnected;
        public void NotifyClientDisconnected(string clientId)
        {
            if (ClientDisconnected != null) ClientDisconnected(clientId);
            Environment.Instance.NotifyClientDisconnected(clientId);
        }

        public event ClientConnectedHandler ClientConnected;
        public void NotifyClientConnected(string clientId, int port)
        {
            if (ClientConnected != null) ClientConnected(clientId, port);
            Environment.Instance.NotifyClientConnected(clientId, port);
            Debug("Client ready.");
            lock (NewClientEvents)
            {
                NewClientEvents.Add(remoteClients[clientId]);
            }
            remoteClients[clientId].QueueEvent(Guid.NewGuid().ToString(), "IThalamusClient.Connected", new string[] { "uid", "port" }, new string[] { PMLParameterType.String.ToString(), PMLParameterType.Int.ToString() }, new string[] { clientId, port.ToString() });
        }

        /*public event ClientInfoChangedHandler ClientInfoChanged;
        public void NotifyClientInfoChanged(string clientId)
        {
            if (ClientInfoChanged != null) ClientInfoChanged(clientId);
        }*/
        

        #endregion

        #region fields and properties

        protected IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;


        private Dictionary<String, PML> eventInfo = new Dictionary<string, PML>();
        public Dictionary<String, PML> EventInfo
        {
            get { 
				return eventInfo; 
			}
        }

        private Dictionary<string, ThalamusClientProxy> remoteClients = new Dictionary<string, ThalamusClientProxy>();
        public Dictionary<string, ThalamusClientProxy> RemoteClients
        {
            get { return remoteClients; }
        }

        protected Character character = Character.NullCharacter;
        public Character Character
        {
            get { return character; }
        }

        int fastLocalPort = Properties.Settings.Default.CharacterStartPort;
        public int FastLocalPort
        {
            get { return fastLocalPort; }
            set { fastLocalPort = value; }
        }

        int slowLocalPort = Properties.Settings.Default.CharacterStartPort;
        public int SlowLocalPort
        {
            get { return slowLocalPort; }
            set { slowLocalPort = value; }
        }

        int broadcastPort = 9050;
        public int BroadcastPort
        {
            get { return broadcastPort; }
        }


        private Dictionary<string, List<ThalamusClientProxy>> subscribedToEvent = new Dictionary<string, List<ThalamusClientProxy>>();
        public Dictionary<string, List<ThalamusClientProxy>> SubscribedToEvent
        {
            get { return subscribedToEvent; }
        }
        private Dictionary<string, List<ThalamusClientProxy>> publishesEvent = new Dictionary<string, List<ThalamusClientProxy>>();
        public Dictionary<string, List<ThalamusClientProxy>> PublishesEvent
        {
            get { return publishesEvent; }
        }

        HttpListener fastListener;
        HttpListener slowListener;
        bool shutdown = false;
        private Thread fastHttpDispatcherThread;
        private Thread slowHttpDispatcherThread;
        private Thread broadcasterThread;
        private Thread fastMessageDispatcherThread;
        private Thread slowMessageDispatcherThread;
        bool fastServiceRunning = false;
        bool slowServiceRunning = false;

        SocketPermission acceptSocketPermission = new SocketPermission(NetworkAccess.Accept, TransportType.All, "", SocketPermission.AllPorts);
        SocketPermission connectSocketPermission = new SocketPermission(NetworkAccess.Connect, TransportType.All, "", SocketPermission.AllPorts);

        Stopwatch centralTime = new Stopwatch();
        public long CentralTime
        {
            get { return centralTime.ElapsedMilliseconds; }
        }
        bool readyToBroadcast = false;

        List<HttpListenerContext> fastHttpRequestsQueue = new List<HttpListenerContext>();
        List<HttpListenerContext> slowHttpRequestsQueue = new List<HttpListenerContext>();

		private int inboundEventsTotal = 0;
		internal int InboundEventsTotal {
			get {
				return inboundEventsTotal;
			}
			set {
				inboundEventsTotal = value;
			}
		}

		private int outboundEventsTotal = 0;
		internal int OutboundEventsTotal {
			get {
				return outboundEventsTotal;
			}
			set {
				outboundEventsTotal = value;
			}
		}

		private int inboundEventsCounter = 0;
		internal int InboundEventsCounter {
			get {
				return inboundEventsCounter;
			}
			set {
				inboundEventsCounter = value;
			}
		}

		private int outboundEventsCounter = 0;
		internal int OutboundEventsCounter {
			get {
				return outboundEventsCounter;
			}
			set {
				outboundEventsCounter = value;
			}
		}

		private int inboundEventsPerSecond = 0;
		public int InboundEventsPerSecond {
			get {
				return inboundEventsPerSecond;
			}
		}

		private int outboundEventsPerSecond = 0;
		public int OutboundEventsPerSecond {
			get {
				return outboundEventsPerSecond;
			}
		}

        List<PerceptionInfoResponse> perceptionInfoQueue = new List<PerceptionInfoResponse>();

        IBMLActions bmlActionsPublisher;

        #endregion

        #region constructors and overrides

        public ClientsInterface()
        {
            bmlActionsPublisher = this;
        }

        public virtual bool Setup() {
            this.broadcastPort = Properties.Settings.Default.BroadcastPort;

            acceptSocketPermission.Union(connectSocketPermission).Demand();

            fastHttpDispatcherThread = new Thread(new ThreadStart(FastHttpDispatcherThread));
            slowHttpDispatcherThread = new Thread(new ThreadStart(SlowHttpDispatcherThread));
            broadcasterThread = new Thread(new ThreadStart(BroadcastServer));
            fastMessageDispatcherThread = new Thread(new ThreadStart(FastMessageDispatcher));
            slowMessageDispatcherThread = new Thread(new ThreadStart(SlowMessageDispatcher));
			ThreadPool.SetMaxThreads(5000,5000);
            return true;
        }
        public virtual bool Setup(Character character)
        {
            this.character = character;
            return Setup();
        }
        public virtual bool Start() {
            centralTime.Start();
			/*timerPerformance = new System.Timers.Timer();
			timerPerformance.Interval = 1000;
			timerPerformance.Elapsed += new ElapsedEventHandler(TimerPerformanceCounter);
			timerPerformance.Start();*/
            StartServer();
            return true;
        }

        public void StartServer()
        {
            readyToBroadcast = false;
            fastHttpDispatcherThread.Start();
            slowHttpDispatcherThread.Start();
            broadcasterThread.Start();
            fastMessageDispatcherThread.Start();
            slowMessageDispatcherThread.Start();
        }

        public virtual void Dispose() {
            Debug("Disposing Clients for Character '{0}'.", character.Name);

            shutdown = true;
            Debug("Set shutdown flag.");
            try
            {
                if (fastListener != null) fastListener.Stop();
            }
            catch { }
            Debug("Stopped Fast HTTP Listener.");

            try
            {
                if (slowListener != null) slowListener.Stop();
            }
            catch { }
            Debug("Stopped Slow HTTP Listener.");

            List<ThalamusClientProxy> clients = new List<ThalamusClientProxy>(remoteClients.Values);
			remoteClients.Clear ();

            foreach (ThalamusClientProxy client in clients)
            {
                //client.Shutdown();
                client.Disconnect();
            }
            Debug("Shutdown and Disconnected clients.");
            
            foreach (ThalamusClientProxy client in clients) client.Dispose();
            Debug("Disposed clients.");

            try
            {
                if (fastHttpDispatcherThread != null) fastHttpDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (slowHttpDispatcherThread != null) slowHttpDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (broadcasterThread != null) broadcasterThread.Join();
            }
            catch { }

            try
            {
                if (fastMessageDispatcherThread != null) fastMessageDispatcherThread.Join();
            }
            catch { }

            try
            {
                if (slowMessageDispatcherThread != null) slowMessageDispatcherThread.Join();
            }
            catch { }
        }


		public void Debug(string text, params object[] args)
		{
			Environment.Instance.DebugIf ("all", "[Master] " + text, args);
		}

		public void DebugError (string text, params object[] args)
		{
			Environment.Instance.DebugIf ("error", "[Master] " + text, args);
		}
		public void DebugException (Exception e, int location = -1)
		{
			Environment.Instance.DebugIf ("error", "[Master] Exception in {0}{3}: {1}{2}.", new StackFrame(1).GetMethod().Name, e.Message, (e.InnerException==null?"":": " + e.InnerException), (location==-1?"":"(" + location + ")"));
		}

        #endregion

		#region Performance profiler



		internal void TimerPerformanceCounter()
		{
			inboundEventsTotal+=inboundEventsCounter;
			outboundEventsTotal+=outboundEventsCounter;

			inboundEventsPerSecond = inboundEventsCounter;
			outboundEventsPerSecond = outboundEventsCounter;
            //Console.WriteLine(String.Format("inTotal: {0}, outTotal: {1}, in/s: {2}, out/s: {3}", inboundEventsTotal, outboundEventsTotal, inboundEventsPerSecond, outboundEventsPerSecond));
			inboundEventsCounter = 0;
			outboundEventsCounter = 0;
		}

		#endregion

        #region response dispatcher

        public void FastMessageDispatcher()
        {
            while (!shutdown)
            {
				bool performSleep = true;
                try
                {
					if (fastHttpRequestsQueue.Count>0) {
						performSleep =false;
						List<HttpListenerContext> httpRequests;
						lock (fastHttpRequestsQueue)
						{
							httpRequests = new List<HttpListenerContext>(fastHttpRequestsQueue);
							fastHttpRequestsQueue.Clear();
						}
						foreach (HttpListenerContext r in httpRequests)
						{
							(new Thread(new ParameterizedThreadStart(ProcessRequest))).Start(r);
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
            Debug("Terminated FastMessageDispatcher");
        }

        public void SlowMessageDispatcher()
        {
            while (!shutdown)
            {
                bool performSleep = true;
                try
                {
                    if (slowHttpRequestsQueue.Count > 0)
                    {
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (slowHttpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(slowHttpRequestsQueue);
                            slowHttpRequestsQueue.Clear();
                        }
                        foreach (HttpListenerContext r in httpRequests)
                        {
                            (new Thread(new ParameterizedThreadStart(ProcessRequest))).Start(r);
                            performSleep = false;
                        }
                    }

                    if (RegisteringClients.Count > 0)
                    {
                        performSleep = false;
                        List<ThalamusClientProxy> registeringClients;
                        lock (RegisteringClients)
                        {
                            registeringClients = new List<ThalamusClientProxy>(RegisteringClients);
                            RegisteringClients.Clear();
                        }
                        (new Thread(new ParameterizedThreadStart(RegisterNewClients))).Start(registeringClients);
                    }

                    if (NewClientEvents.Count > 0)
                    {
                        performSleep = false;
                        List<ThalamusClientProxy> newClients = new List<ThalamusClientProxy>();
                        lock (NewClientEvents)
                        {
                            foreach (ThalamusClientProxy c in NewClientEvents)
                            {
                                if (remoteClients.ContainsKey(c.ClientId)) newClients.Add(c);
                            }
                            NewClientEvents.Clear();
                        }
                        (new Thread(new ParameterizedThreadStart(NotifyNewClients))).Start(newClients);
                    }

                    if (perceptionInfoQueue.Count > 0)
                    {
                        performSleep = false;
                        List<PerceptionInfoResponse> perceptionInfo;
                        lock (perceptionInfoQueue)
                        {
                            perceptionInfo = new List<PerceptionInfoResponse>(perceptionInfoQueue);
                            perceptionInfoQueue.Clear();
                        }
                        (new Thread(new ParameterizedThreadStart(BroadcastPerceptionInfo))).Start(perceptionInfo);
                    }

                }
                catch (Exception e)
                {
                    DebugException(e);
                }
                if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated SlowMessageDispatcher");
        }

        #endregion

        #region auto discovery UDP broadcast server

        UdpClient udpBroadcastServer = null;

        public void BroadcastServer()
        {
            while (!readyToBroadcast) Thread.Sleep(50);

            Debug("Auto-discovery broadcaster running on UDP port " + Properties.Settings.Default.BroadcastPort);
            while (!shutdown)
            {
                try
                {
                    Environment.AuthorizeAddress(Properties.Settings.Default.BroadcastPort, "Everyone");
                    
                    IPAddress broadcastIp = IPAddress.Parse(Properties.Settings.Default.MulticastAddress);
                    IPEndPoint iep1 = new IPEndPoint(broadcastIp, Properties.Settings.Default.BroadcastPort);

                    /*udpBroadcastServer = new UdpClient();
                    udpBroadcastServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    udpBroadcastServer.Client.Bind(iep1);
                    udpBroadcastServer.JoinMulticastGroup(broadcastIp);
                    udpBroadcastServer.MulticastLoopback = true;
                    byte[] data = Encoding.ASCII.GetBytes("THALAMUS_MASTER " + fastLocalPort.ToString() + " " + slowLocalPort.ToString() + " " + character.Name);
                    udpBroadcastServer.Send(data, data.Length);*/

                    
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    
                    sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.HostToNetworkOrder(Thalamus.Environment.Instance.SelectedNetworkAdapter));
                    sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(broadcastIp));
                    
                    sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, Properties.Settings.Default.MulticastTTL);
                    
                    byte[] data = Encoding.ASCII.GetBytes("THALAMUS_MASTER " + fastLocalPort.ToString() + " " + slowLocalPort.ToString() + " " + character.Name);
                    sock.Connect(iep1);
                    sock.Send(data, data.Length, SocketFlags.None);
                    sock.Close();
                }
                catch (Exception e)
                {
					DebugException(e);
                }
                Thread.Sleep(Properties.Settings.Default.BeaconInterval);
            }
            if (udpBroadcastServer != null) udpBroadcastServer.Close();

            
        }

		public void BroadcastPerceptionInfo(object oPerceptionInfo) {
			List<PerceptionInfoResponse> perceptionInfo = (List<PerceptionInfoResponse>) oPerceptionInfo;

            string[] piNames = new string[perceptionInfo.Count];
            string[][] piParameters = new string[perceptionInfo.Count][];
            string[][] piTypes = new string[perceptionInfo.Count][];
            int i = 0;
			foreach (PerceptionInfoResponse pi in perceptionInfo)
			{
                piNames[i] = pi.Name;
                piParameters[i] = pi.Parameters;
                piTypes[i] = pi.Types;
                i++;
            }
            List<ThalamusClientProxy> clients = new List<ThalamusClientProxy>(remoteClients.Values);
            foreach (ThalamusClientProxy client in clients)
			{
                try
				{
                    if (!client.SendMessage(() => client.SlowProxy.ReceiveEventInfo(piNames, piParameters, piTypes), "ReceivePerceptionInfo"))
                    {
						DebugError("Failed to broadcast PerceptionInfo to '" + client.Name + "'");
					}
                }
				catch (Exception e)
				{
					DebugException(e);
				}
			}
		}

        #endregion

        #region XML Rpc connection

        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }


        public void FastHttpDispatcherThread()
        {
            while (!fastServiceRunning)
            {
                try
                {
					Environment.Instance.DebugIf("all", "Attempt to start fast service on port '" + fastLocalPort + "'");
                    fastListener = new HttpListener();
                    Environment.AuthorizeAddress(fastLocalPort, "Everyone");
                    fastListener.Prefixes.Add(string.Format("http://*:{0}/", fastLocalPort));
                    fastListener.Start();
					Environment.Instance.DebugIf("all", "Fast XMLRPC Listening on port " + fastLocalPort);
                    fastServiceRunning = true;
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
                        fastLocalPort++;
                        Debug("Fast Port unavaliable." + ex.Message + ", " + ex.InnerException);
                        fastServiceRunning = false;
                    }
                }
            }
            //NotifyClientInfoChanged("");
            
			while (!shutdown) {
				try {
					HttpListenerContext context = fastListener.GetContext ();
					lock (fastHttpRequestsQueue) {
						fastHttpRequestsQueue.Add (context);
					}
				} catch (Exception e) {
					if (!e.Message.Contains("thread exit")) DebugException (e);
					fastServiceRunning = false;
					if (fastListener != null)
						fastListener.Close ();
				}
			}
            Debug("Terminated FastHttpDispatcherThread");
            fastListener.Close();
        }

        public void SlowHttpDispatcherThread()
        {
            while (!slowServiceRunning)
            {
				while (!shutdown && !fastServiceRunning)
					Thread.Sleep (250);
				if (!shutdown) {
					try {
						Environment.Instance.DebugIf ("all", "Attempt to start slow service on port '" + slowLocalPort + "'");
						slowListener = new HttpListener ();
						Environment.AuthorizeAddress (slowLocalPort, "Everyone");
						slowListener.Prefixes.Add (string.Format ("http://*:{0}/", slowLocalPort));
						slowListener.Start ();
						Environment.Instance.DebugIf ("all", "Slow XMLRPC Listening on port " + slowLocalPort);
						slowServiceRunning = true;
					} catch (HttpListenerException ex) {
						if (ex.ErrorCode == 5) {
							DebugError ("Permission denied to open socket.\nPlease run this program in Administrator mode.");
							return;
						} else {
							slowLocalPort++;
							Debug ("Slow Port unavaliable." + ex.Message + ", " + ex.InnerException);
							slowServiceRunning = false;
						}
					}
				}
            }
            readyToBroadcast = true;
            //NotifyClientInfoChanged("");

            while (!shutdown)
            {
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
                    slowServiceRunning = false;
                    if (slowListener != null)
                        slowListener.Close();
                }
            }
            Debug("Terminated SlowHttpDispatcherThread");
            slowListener.Close();
        }

		public void ProcessRequest(object oContext) {
			try
			{
				XmlRpcListenerService svc = new ThalamusServerService(this, ((HttpListenerContext) oContext).Request.RemoteEndPoint.Address.ToString());
				svc.ProcessRequest((HttpListenerContext) oContext);
			}
			catch (Exception e) { 
				DebugException(e);
			}

		}

		public void ListenerCallback(IAsyncResult result)
		{
			try
			{
				HttpListener listener = (HttpListener) result.AsyncState;
				HttpListenerContext context = listener.EndGetContext(result);
				ProcessRequest(context);
			}
			catch (Exception e) { 
				DebugException(e);
			}
		}

        private List<ThalamusClientProxy> RegisteringClients = new List<ThalamusClientProxy>();
        private List<ThalamusClientProxy> NewClientEvents = new List<ThalamusClientProxy>();
        private List<string> LostClientEvents = new List<string>();

		public string ConnectClient(string name, string clientId, string clientAddress, int clientFastPort, int clientSlowPort, int numSubscribedEvents)
        {
            string uid = Guid.NewGuid().ToString();
            if (clientId != "" && clientId!=null) uid = clientId;
            if (remoteClients.ContainsKey(uid)) return uid;

            Environment.Instance.Debug("Received new Client named: '{0}' from '{1}:{2}/{3}' with id {4}.", name, clientAddress, clientFastPort, clientSlowPort, uid);

            if (remoteClients.ContainsKey(uid)) return "";
            ThalamusClientProxy newClient = new ThalamusClientProxy(this, name, clientAddress, clientFastPort, clientSlowPort, uid, numSubscribedEvents);
			lock (remoteClients) {
				Dictionary<string, ThalamusClientProxy> newClients = new Dictionary<string, ThalamusClientProxy>(remoteClients);
				newClients.Add (uid, newClient);
				remoteClients = newClients;
			}
            lock (RegisteringClients)
            {
                RegisteringClients.Add(newClient);
            }
            return uid;
        }

        internal void DisconnectClient(string clientId)
        {
            bool notify = false;
            ThalamusClientProxy proxy = null;
			lock (remoteClients) {
				if (remoteClients.ContainsKey (clientId)) {
					notify = true;
                    proxy = remoteClients[clientId];
                    remoteClients.Remove(clientId);
				}
			}
            if (proxy != null)
            {
                List<string> subscribedEvents = new List<string>(proxy.SubscribedEvents);
                List<string> announcedEvents = new List<string>(proxy.AnnouncedEvents);

                lock (eventInfo)
                {
                    lock (subscribedToEvent)
                    {
                        foreach (string eventName in subscribedEvents)
                        {
                            bool remove = true;
                            foreach (ThalamusClientProxy client in remoteClients.Values)
                            {
                                foreach (string subscribedEvent in client.SubscribedEvents)
                                {
                                    if (eventName == subscribedEvent)
                                    {
                                        remove = false;
                                        break;
                                    }
                                }
                                if (!remove) break;

                                foreach (string announcedEvent in client.AnnouncedEvents)
                                {
                                    if (eventName == announcedEvent)
                                    {
                                        remove = false;
                                        break;
                                    }
                                }
                                if (!remove) break;
                            }
                            if (remove) eventInfo.Remove(eventName);
                            subscribedToEvent[eventName].Remove(proxy);
                            if (subscribedToEvent[eventName].Count == 0) subscribedToEvent.Remove(eventName);
                        }
                    }

                    lock (publishesEvent)
                    {
                        foreach (string eventName in announcedEvents)
                        {
                            bool remove = true;
                            foreach (ThalamusClientProxy client in remoteClients.Values)
                            {
                                foreach (string subscribedEvent in client.SubscribedEvents)
                                {
                                    if (eventName == subscribedEvent)
                                    {
                                        remove = false;
                                        break;
                                    }
                                }
                                if (!remove) break;

                                foreach (string announcedEvent in client.AnnouncedEvents)
                                {
                                    if (eventName == announcedEvent)
                                    {
                                        remove = false;
                                        break;
                                    }
                                }
                                if (!remove) break;
                            }
                            if (remove) eventInfo.Remove(eventName);
                            publishesEvent[eventName].Remove(proxy);
                            if (publishesEvent[eventName].Count == 0) publishesEvent.Remove(eventName);
                        }
                    }
                }

                proxy.Dispose();
            }

            lock (LostClientEvents)
            {
                LostClientEvents.Add(clientId);
            }
            Environment.Instance.NotifyEventInformationChanged(character);
            if (notify) NotifyClientDisconnected(clientId);
        }

        #endregion

        #region Perception management and routing

        public void RegisterNewClients(object oclients)
        {
            List<ThalamusClientProxy> registeringClients = (List<ThalamusClientProxy>)oclients;
            foreach (ThalamusClientProxy client in registeringClients)
            {
                PerformRegistration(client);
            }
        }

        public void NotifyNewClients(object oclients)
        {
            List<ThalamusClientProxy> newClients = (List<ThalamusClientProxy>)oclients;

            List<ThalamusClientProxy> notifyClients;
            lock (remoteClients)
            {
                notifyClients = new List<ThalamusClientProxy>(remoteClients.Values);
            }
            foreach (ThalamusClientProxy c in notifyClients)
            {
                foreach (ThalamusClientProxy nc in newClients)
                {
                    if (!newClients.Contains(c))
                    {
                        if (!c.SendMessage(() => c.SlowProxy.NewClientConnected(nc.Name, nc.ClientId), "NewClientConnected"))
                        {
                            Environment.Instance.DebugIf("error", "Failed to notify new client connected'");
                        }
                    }
                }
            }
        }

        public void NotifyLostClients(object oclients)
        {
            List<ThalamusClientProxy> lostClients = (List<ThalamusClientProxy>)oclients;
            List<ThalamusClientProxy> notifyClients;
            lock (remoteClients)
            {
                notifyClients = new List<ThalamusClientProxy>(remoteClients.Values);
            }
            foreach (ThalamusClientProxy c in notifyClients)
            {
                foreach (ThalamusClientProxy lc in lostClients)
                {
                    if (!lostClients.Contains(c))
                    {
                        if (!c.SendMessage(() => c.SlowProxy.ClientDisconnected(lc.Name, lc.ClientId),"ClientDisconnected"))
                        {
                            Environment.Instance.DebugIf("error", "Failed to notify client disconnected'");
                        }
                    }
                }
            }
        }

		public void PerformRegistration(ThalamusClientProxy client)
		{
            if (!client.SendMessage(() => client.SlowProxy.PerformRegistration(), "PerformRegistration"))
            {
				Environment.Instance.DebugIf("error", "Failed to request registration on client '{0}'!", client.Name);
			}
		}

        public void RegisterEvents(string clientId, string[] announcedEvents, string[] subscribedEvents)
        {
            ThalamusClientProxy client;
            if (remoteClients.ContainsKey(clientId)) client = remoteClients[clientId];
            else return;

            lock (publishesEvent)
            {
                foreach (string ev in announcedEvents)
                {
                    if (!publishesEvent.ContainsKey(ev)) publishesEvent[ev] = new List<ThalamusClientProxy>();
                    if (!publishesEvent[ev].Contains(client)) publishesEvent[ev].Add(client);
                }
            }
            lock (subscribedEvents)
            {
                foreach (string ev in subscribedEvents)
                {
                    if (!subscribedToEvent.ContainsKey(ev)) subscribedToEvent[ev] = new List<ThalamusClientProxy>();
                    if (!subscribedToEvent[ev].Contains(client)) subscribedToEvent[ev].Add(client);
                }
            }
            lock (RemoteClients)
            {
                RemoteClients[clientId].Announce(announcedEvents);
                RemoteClients[clientId].Subscribe(subscribedEvents);
            }
            if (RegisteringClients.Contains(client))
            {
                lock (NewClientEvents)
                {
                    NewClientEvents.Add(client);
                }
            }
        }

        public void AnnounceEventInformation(string clientId, string[] eventNames, bool[] isAction, string[][] allParameters, string[][] allTypes, string[] enumNames, string[][] enums)
        {
            ThalamusClientProxy client;
            if (remoteClients.ContainsKey(clientId)) client = remoteClients[clientId];
            else return;

            for (int i = 0; i < enumNames.Length; i++)
            {
                PML.LoadEnum(enumNames[i], enums[i]);
            }

            for (int i = 0; i < eventNames.Length; i++)
            {
                string eventName = eventNames[i];
                
                string[] parameters = allParameters[i];
                string[] types = allTypes[i];
                PML pml = new PML(eventName, parameters, types);
                pml.EventType = isAction[i] ? PMLEventType.Action : PMLEventType.Perception;
                if (!pml.IsNull)
                {
                    lock (eventInfo)
                    {
                        eventInfo[eventName] = pml;
                    }
                    lock (perceptionInfoQueue)
                    {
                        perceptionInfoQueue.Add(new PerceptionInfoResponse(eventName, parameters, types));
                    }
                }
            }
        }

        public void PublishEvents(string[] messageIds, string[] clientIds, string[] perceptionNames, bool[] dontLogDescriptions, string[][] allParameters, string[][] allTypes, string[][] allValues)
        {
            for (int i = 0; i < messageIds.Length; i++)
            {
                PublishEvent(messageIds[i], clientIds[i], perceptionNames[i], dontLogDescriptions[i], allParameters[i], allTypes[i], allValues[i]);
            }
        }

        public void PublishEvent(string messageId, string clientId, string eventName, bool dontLogDescription, string[] parameters, string[] types, string[] values, string syncEvent = "")
        {
            ThalamusClientProxy sourceClient = null;
            inboundEventsCounter++;
            if (remoteClients.ContainsKey(clientId)) sourceClient = remoteClients[clientId];

            PML pml = new PML(eventName, parameters, types, values);
            pml.DontLogDescription = dontLogDescription;
            if (sourceClient != null)
            {
                Environment.Instance.DebugIf("messages", "[T][" + sourceClient.Name + "]Publishing... " + pml.ToString() + ".");
            }
            else
            {
                Environment.Instance.DebugIf("messages", "[T][Unknown]Publishing... " + pml.ToString() + ".");
            }
            Environment.Instance.LogEvent(false, character, sourceClient, pml);

            if (syncEvent != "")
            {
                Behavior b = new Behavior();
                b.AddNode(new Actions.SyncAction(new Actions.SyncPoint(eventName), sourceClient, pml));
                BehaviorPlan.Instance.Add(b);
            }
            else
            {
                Character.SendPerception(pml);
                BroadcastEvent(messageId, sourceClient, eventName, parameters, types, values, pml);
            }
        }

		public void BroadcastEvent(ThalamusClientProxy sourceClient, PML p)
		{
			string[] parameters = new string[p.Parameters.Count];
			string[] values = new string[p.Parameters.Count];
			string[] types = new string[p.Parameters.Count];
			int i = 0;
			foreach (KeyValuePair<string, PMLParameter> param in p.Parameters)
			{
				parameters[i] = param.Key;
				values[i] = param.Value.ToString();
				types[i] = param.Value.Type.ToString();
				i++;
			}
            BroadcastEvent(Guid.NewGuid().ToString(), sourceClient, p.Name, parameters, types, values, p);
		}

        public void BroadcastEvent(string messageId, ThalamusClientProxy sourceClient, string eventName, string[] parameters, string[] types, string[] values, PML ev)
        {
            List<ThalamusClientProxy> clients;
            lock (remoteClients)
            {
                clients = new List<ThalamusClientProxy>(remoteClients.Values);
            }
            foreach (ThalamusClientProxy client in clients)
            {
                if (sourceClient != client && client.SubscribedEvents.Contains(eventName))
                {
                    Environment.Instance.LogEvent(true, character, client, ev);
                    client.QueueEvent(messageId, eventName, parameters, types, values, ev.DontLogDescription);
                    outboundEventsCounter++;
                }
            }
            if (sourceClient != null) Environment.Instance.DebugIf("messages", "[T][" + sourceClient.Name + "]Published " + ev.ToString() + ".");
            else Environment.Instance.DebugIf("messages", "[T][Unknown]Published " + ev.ToString() + ".");
        }


        #endregion

        #region BML actions routing

        private void RouteToClients(MethodBase methodBase, params object[] args)
        {
            string[] parameterNames;
            string[] parameterTypes;
            string[] parameterValues;


            string[] methodNameSplit = methodBase.Name.Split('.');
            string methodName = (methodNameSplit.Length>=2?methodNameSplit[methodNameSplit.Length-2]:"") + "." + methodNameSplit[methodNameSplit.Length-1];
            PML pml = new PML(methodName, methodBase, args);
            //pml.ToArrays(out parameterNames, out parameterTypes, out parameterValues);

            BroadcastEvent(null, pml);
        }

        public void FaceLexeme(string id, string lexeme)
        {
            bmlActionsPublisher.FaceLexeme(id, lexeme);
        }

        public void FaceFacs(string id, int AU, Actions.Side side, double intensity)
        {
            bmlActionsPublisher.FaceFacs(id, AU, side, intensity);
        }

        public void NeutralFaceExpression()
        {
            bmlActionsPublisher.NeutralFaceExpression();
        }

        public void Speak(string id, string text)
        {
            bmlActionsPublisher.Speak(id, text);
        }

        public void SpeakBookmarks(string id, string[] text, string[] bookmarks)
        {
            bmlActionsPublisher.SpeakBookmarks(id, text, bookmarks);
        }

        public void Gaze(string id, double horizontal, double vertical, double speed, bool trackFaces)
        {
            bmlActionsPublisher.Gaze(id, horizontal, vertical, speed, trackFaces);
        }


        public void WalkTo(string id, double x, double y, double angle)
        {
            bmlActionsPublisher.WalkTo(id, x, y, angle);
        }

        public void WalkToTarget(string id, string target)
        {
            bmlActionsPublisher.WalkToTarget(id, target);
        }

        public void StopWalk()
        {
            bmlActionsPublisher.StopWalk();
        }

        public void PlayAnimation(string id, string animation)
        {
            bmlActionsPublisher.PlayAnimation(id, animation);
        }

        public void PlayAnimationQueued(string id, string animation)
        {
            bmlActionsPublisher.PlayAnimationQueued(id, animation);
        }

        public void StopAnimation(string id)
        {
            bmlActionsPublisher.StopAnimation(id);
        }

        public void SetPosture(string id, string posture, double percent, double decay)
        {
            bmlActionsPublisher.SetPosture(id, posture, percent, decay);
        }

        public void ResetPose()
        {
            bmlActionsPublisher.ResetPose();
        }

        public void PlaySound(string id, string SoundName, double Volume, double Pitch)
        {
            bmlActionsPublisher.PlaySound(id, SoundName, Volume, Pitch);
        }

        public void PlaySoundLoop(string id, string SoundName, double Volume, double Pitch)
        {
            bmlActionsPublisher.PlaySoundLoop(id, SoundName, Volume, Pitch);
        }

        public void StopSound(string id)
        {
            bmlActionsPublisher.StopSound(id);
        }

        public void Pointing(string id, string target, double speed = 1.0f, Actions.PointingMode mode = Actions.PointingMode.RightHand)
        {
            bmlActionsPublisher.Pointing(id, target, speed, mode);
        }

        public void PointingAngle(string id, double horizontal, double vertical, double speed = 1.0f, Actions.PointingMode mode = Actions.PointingMode.RightHand)
        {
            bmlActionsPublisher.PointingAngle(id, horizontal, vertical, speed, mode);
        }

        public void Head(string id, string lexeme, int repetitions, double frequency)
        {
            bmlActionsPublisher.Head(id, lexeme, repetitions, frequency);
        }

        #endregion

        #region BML Interface implementation

        void IFaceActions.FaceShiftLexeme(string id, string lexeme) {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[]{ id, lexeme});
        }

        void IFaceActions.FaceLexeme(string id, string lexeme)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, lexeme });
        }

        void IFaceActions.FaceFacs(string id, int AU, Actions.Side side, double intensity)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[]{ id, AU, side, intensity});
        }

        void IFaceActions.NeutralFaceExpression()
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { });
        }

        void ISpeakActions.Speak(string id, string text)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, text });
        }

        void ISpeakActions.SpeakBookmarks(string id, string[] text, string[] bookmarks)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, text, bookmarks });
        }

        void IGazeActions.Gaze(string id, double horizontal, double vertical, double speed, bool trackFaces)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, horizontal, vertical, speed, trackFaces });
        }


        void ILocomotionActions.WalkTo(string id, double x, double y, double angle)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, x, y, angle });
        }

        void ILocomotionActions.WalkToTarget(string id, string target)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, target });
        }

        void ILocomotionActions.StopWalk()
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { });
        }


        void IAnimationActions.PlayAnimation(string id, string animation)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, animation });
        }

        void IAnimationActions.StopAnimation(string id)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id });
        }


        void IPostureActions.SetPosture(string id, string posture, double percent, double decay)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, posture, percent, decay });
        }

        void IPostureActions.ResetPose()
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { });
        }

        void ISoundActions.PlaySound(string id, string SoundName, double Volume, double Pitch)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, SoundName, Volume, Pitch });
        }

        void ISoundActions.PlaySoundLoop(string id, string SoundName, double Volume, double Pitch)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, SoundName, Volume, Pitch });
        }

        void ISoundActions.StopSound(string id)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id });
        }

        void IPointingActions.Pointing(string id, string target, double speed, Actions.PointingMode mode)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, target, speed, mode });
        }

        void IPointingActions.PointingAngle(string id, double horizontal, double vertical, double speed, Actions.PointingMode mode)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, horizontal, vertical, speed, mode });
        }

        void IHeadActions.Head(string id, string lexeme, int repetitions, double amplitude, double frequency)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, lexeme, repetitions, amplitude, frequency });
        }

        void ISpeakActions.SpeakStop()
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { });
        }

        void IAnimationActions.PlayAnimationQueued(string id, string animation)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { animation });
        }

        void IWavingActions.Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, Actions.PointingMode mode)
        {
            RouteToClients(MethodInfo.GetCurrentMethod(), new object[] { id, horizontal, vertical, frequency, amplitude, duration, mode });
        }

        public void BML(string code)
        {
            BehaviorPlan.Instance.Add(BehaviorManager.Instance.BmlToBehavior(GBML.GBML.LoadFromText(code)), Character);
        }

        void IBMLCodeAction.BML(string code)
        {
            BehaviorPlan.Instance.Add(BehaviorManager.Instance.BmlToBehavior(GBML.GBML.LoadFromText(code)), Character);
        }

        

        #endregion
        
    }
}
