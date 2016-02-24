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
using CookComputing.XmlRpc;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace Thalamus
{
    public class ThalamusClientProxy
    {
		public struct QueuedEvent {
			public string EventId;
			public string EventName;
			public string[] Parameters;
			public string[] Types;
			public string[] Values;
            public bool DontLogDescription;

			public QueuedEvent(string EventId, string EventName, string[] Parameters, string[] Types, string[] Values, bool dontLogDescription) {
				this.EventId = EventId;
				this.EventName = EventName;
				this.Parameters = Parameters;
				this.Types=Types;
				this.Values=Values;
                this.DontLogDescription = dontLogDescription;
			}

			public override string ToString()
			{
				string str = "(" + EventName + "):[";
                if (DontLogDescription) str += "no description";
                else
                {
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        str += Parameters[i] + "=" + Values[i] + ";";
                    }
                }
				return str + "]";
			}
		}

        #region properties and fields

        IThalamusClientRpc slowProxy;
		public IThalamusClientRpc SlowProxy {
			get {
				return slowProxy;
			}
		}

        IThalamusClientRpc fastProxy;
        public IThalamusClientRpc FastProxy
		{
            get { return fastProxy; }
        }

        string name = "";
        public string Name
        {
            get { return name; }
        }

        int fastPort = 0;
        public int FastPort
        {
            get { return fastPort; }
        }

        int slowPort = 0;
        public int SlowPort
        {
            get { return slowPort; }
        }

        string address = "";
        public string Address
        {
          get { return address; }
        }

        ClientsInterface characterClients = null;

        public Character Character
        {
            get { return characterClients.Character; }
        }

        string clientId = "";
        public string ClientId
        {
            get { return clientId; }
        }

		private Dictionary<string, QueuedEvent> EventQueue = new Dictionary<string, QueuedEvent>();

        System.Timers.Timer pingTimer = new System.Timers.Timer();
		bool shutdown = false;
		Thread sentMessageDispatcherThread;
        Thread receivedMessageDispatcherThread;
        Thread httpRequestsDispatcherThread;

        long timeZone = 0;
        public long TimeZone
        {
            get { return timeZone; }
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

        List<String> subscribedEvents = new List<string>();
        public List<String> SubscribedEvents
        {
            get { return subscribedEvents; }
        }

        List<String> announcedEvents = new List<string>();
        public List<String> AnnouncedEvents
        {
            get { return announcedEvents; }
        }

		private int numSubscribedEvents = 0;
        private bool connected = false;
        public bool Connected
        {
            get { return connected; }
        }
        bool serviceRunning = false;
        HttpListener listener;

        SocketPermission acceptSocketPermission = new SocketPermission(NetworkAccess.Accept, TransportType.All, "", SocketPermission.AllPorts);
        SocketPermission connectSocketPermission = new SocketPermission(NetworkAccess.Connect, TransportType.All, "", SocketPermission.AllPorts);

        #endregion

        public ThalamusClientProxy(ClientsInterface body, string name, string address, int fastPort, int slowPort, string clientId, int numSubscribedEvents)
        {
            this.name = name;
            this.fastPort = fastPort;
            this.slowPort = slowPort;
            this.address = address;
            this.characterClients = body;
            this.clientId = clientId;
			this.numSubscribedEvents = numSubscribedEvents;

            acceptSocketPermission.Union(connectSocketPermission).Demand();


            fastProxy = XmlRpcProxyGen.Create<IThalamusClientRpc>();
            fastProxy.Url = string.Format("http://{0}:{1}", address, fastPort);
            fastProxy.KeepAlive = true;
            fastProxy.Timeout = Properties.Settings.Default.FastTimeout;

			slowProxy = XmlRpcProxyGen.Create<IThalamusClientRpc>();
            slowProxy.Url = string.Format("http://{0}:{1}", address, slowPort);
			slowProxy.KeepAlive = true;
            slowProxy.Timeout = Properties.Settings.Default.SlowTimeout;
            //slowProxy = fastProxy;

            pingTimer.Interval = Properties.Settings.Default.PingInterval;
            pingTimer.Elapsed+=new ElapsedEventHandler(TimerPing);
            pingTimer.Start();

			sentMessageDispatcherThread = new Thread (new ThreadStart (SentMessageDispatcher));
			sentMessageDispatcherThread.Start ();
            receivedMessageDispatcherThread = new Thread(new ThreadStart(ReceivedMessageDispatcher));
            receivedMessageDispatcherThread.Start();
        }



		public void Debug (string text, params object[] args)
		{
			Environment.Instance.DebugIf ("all", String.Format ("Proxy[{0}] ", Name) + text, args);
		}

		public void DebugError (string text, params object[] args)
		{
			Environment.Instance.DebugIf ("error", String.Format ("Proxy[{0}] ", Name) + text, args);
		}
		public void DebugException (Exception e, int location = -1)
		{
			Environment.Instance.DebugIf ("error", "Client[{3}] Exception in {0}{3}: {1}{2}.", new StackFrame(1).GetMethod().Name, e.Message, (e.InnerException==null?"":": " + e.InnerException), Name, (location==-1?"":"(" + location + ")"));
		}

		private int sendRetries = 5;
        internal bool SendMessage(Action action, string description = "")
        {
			bool sent = false;
			int tries = 0;
			while(!sent && tries<sendRetries) {
				try
				{
					action();
					sent=true;
					publishedMessages++;
				}
				catch {
					tries++;
					Thread.Sleep(100); 
				}
			}
            if (tries >= sendRetries)
            {
                if (description == "") DebugError("Unable to execute anonymous remote action.");
                else DebugError("Unable to execute remote action: '{0}'", description);
            }
			return sent;
		}

        internal long SendTimedMessage(Action action, string description = "")
        {
            long time = -1;
            int tries = 0;
            while (time==-1 && tries < sendRetries)
            {
                try
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    action();
                    timer.Stop();
                    time = timer.ElapsedMilliseconds;
                    publishedMessages++;
                }
                catch
                {
                    tries++;
                    Thread.Sleep(20);
                }
            }
            if (tries >= sendRetries)
            {
                if (description == "") DebugError("Unable to execute anonymous remote action.");
                else DebugError("Unable to execute remote action: '{0}'", description);
            }
            return time;
        }

		private void PerformPing() {
			mustPing=false;

            try
            {
                slowProxy.PingSync(Convert.ToInt32(characterClients.CentralTime), Convert.ToInt32(timeZone));
                //slowProxy.Ping();
                pingTimer.Start();
            }
            catch (Exception e)
            {
                DebugException(e);
                Disconnect();
                pingTimer.Stop();
            }

		}

        int localPort = Properties.Settings.Default.CharacterStartPort;
        List<HttpListenerContext> httpRequestsQueue = new List<HttpListenerContext>();


        public void HttpDispatcherThread()
        {
            while (!serviceRunning)
            {
                try
                {
                    Debug("Attempt to start service for client {0} on port '{1}'", name, localPort);
                    listener = new HttpListener();
                    Environment.AuthorizeAddress(localPort, "Everyone");
                    listener.Prefixes.Add(string.Format("http://*:{0}/", localPort));
                    listener.Start();
                    Debug("XMLRPC for client {0} listening on port {1}", name, localPort);
                    serviceRunning = true;
                    characterClients.NotifyClientConnected(this.clientId, localPort);
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
                    if (!e.Message.Contains("thread exit")) DebugException(e);
                    serviceRunning = false;
                    if (listener != null)
                        listener.Close();
                }
            }
            Debug("Terminated HttpDispatcherThread for client " + name);
            listener.Close();
        }

        public void ReceivedMessageDispatcher()
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
                            (new Thread(new ParameterizedThreadStart(characterClients.ProcessRequest))).Start(r);
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
            Debug("Terminated ReceivedMessageDispatcherThread for client " + name);
        }

		public void SentMessageDispatcher()
		{
            string[] eventIds;
            string[] eventNames;
            string[][] allParameters;
            string[][] allTypes;
            string[][] allValues;
            bool[] dontLogDescriptions;
            List<QueuedEvent> queue;
            bool hasMessages = false;
			while (!shutdown)
			{
				try
				{
					if (mustPing) PerformPing();

                    lock (EventQueue)
                    {
                        queue = new List<QueuedEvent>(EventQueue.Values);
                        EventQueue.Clear();
                    }

                    eventIds = new string[queue.Count];
                    eventNames = new string[queue.Count];
                    allParameters = new string[eventNames.Length][];
                    allTypes = new string[eventNames.Length][];
                    allValues = new string[eventNames.Length][];
                    dontLogDescriptions = new bool[eventNames.Length];

                    int i=0;
					foreach (QueuedEvent ev in queue)
					{
                        int myId = characterClients.OutboundEventsTotal;
                        eventIds[i] = ev.EventId;
                        eventNames[i] = ev.EventName;
                        allParameters[i] = ev.Parameters;
                        allTypes[i] = ev.Types;
                        allValues[i] = ev.Values;
                        dontLogDescriptions[i] = ev.DontLogDescription;
                        i++;
                        hasMessages = true;
					}

                    if (hasMessages)
                    {
                        if (shutdown) return;
                        if (!SendMessage(() => fastProxy.QueueEvents(eventIds, eventNames, allParameters, allTypes, allValues, dontLogDescriptions), "QueueEvents"))
                        {
                            DebugError("Failed to queue events.");
                        }
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
				}
				catch (Exception e)
				{
					DebugException(e);
				}
			}
            Debug("Terminated SentMessageDispatcher for client " + name);
		}

        public override string ToString()
        {
            string str = "";
            List<string> events = new List<string>(subscribedEvents);
            foreach (string p in events) str += p + "; ";
            return string.Format("'{0}'@{1}:{2}/{3} {4}",name,address, fastPort, slowPort, (str==""?"":("{"+str+"}")));
        }

		private bool mustPing = false;
        private void TimerPing(object source, ElapsedEventArgs e)
        {
			pingTimer.Stop();
			mustPing = true;
        }

        public void Dispose()
        {
			shutdown = true;
            Debug("Set shutdown flag.");
            try
            {
                if (listener != null) listener.Stop();
            }
            catch { }
            Debug("Stopped HTTP listener.");

            try
            {
                if (sentMessageDispatcherThread != null) sentMessageDispatcherThread.Abort();
            }
            catch { }

            try
            {
                if (receivedMessageDispatcherThread != null) receivedMessageDispatcherThread.Abort();
            }
            catch { }

            try
            {
                if (httpRequestsDispatcherThread != null) httpRequestsDispatcherThread.Abort();
            }
            catch { }
        }

        public void Shutdown()
        {
            if (!SendMessage(() => slowProxy.Shutdown(), "Shutdown"))
            {
                DebugError("Failed to send shutdown signal.");
            }
        }

        public void Disconnect()
        {
            DebugError("Disconnecting...");
            pingTimer.Stop();
            shutdown = true;
            characterClients.DisconnectClient(clientId);
            //characterClients.NotifyClientInfoChanged(clientId);
			DebugError("Client disconnected.");
		}

        private void ConnectionEstablished()
        {
            connected = true;
            httpRequestsDispatcherThread = new Thread(new ThreadStart(HttpDispatcherThread));
            httpRequestsDispatcherThread.Start();
        }

        public void Subscribe(string[] eventNames)
        {
            try
            {
                lock (subscribedEvents)
                {
                    foreach (string eventName in eventNames)
                    {
                        Debug("Subscribed to event '" + eventName + "'");
                        if (!subscribedEvents.Contains(eventName)) subscribedEvents.Add(eventName);
                    }
                    if (numSubscribedEvents >= 0)
                    {
                        ConnectionEstablished();
                        //characterClients.NotifyClientInfoChanged(clientId);
                        Environment.Instance.NotifyEventInformationChanged(characterClients.Character);
                    }
                }
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Announce(string[] eventNames)
        {
            try
            {
                lock (announcedEvents)
                {
                    foreach (string eventName in eventNames)
                    {
                        Debug("Announced event '" + eventName + "'");
                        if (!announcedEvents.Contains(eventName)) announcedEvents.Add(eventName);
                    }
                    if (numSubscribedEvents == 0)
                    {
                        ConnectionEstablished();
                        //characterClients.NotifyClientInfoChanged(clientId);
                        Environment.Instance.NotifyEventInformationChanged(characterClients.Character);
                    }
                }
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void ClearSubscriptions()
        {
            lock (subscribedEvents)
            {
				Debug("Cleared subscriptions.");
                subscribedEvents.Clear();
                //if (connected) characterClients.NotifyClientInfoChanged(clientId);
            }
        }

		public void QueueEvent (string messageId, string eventName, string[] parameters, string[] types, string[] values, bool dontLogDescription = false)
		{
            if (shutdown) return;
			if (EventQueue.ContainsKey (messageId))
				return;
			receivedMessages++;
			lock (EventQueue) {
				QueuedEvent e = new QueuedEvent (messageId, eventName, parameters, types, values, dontLogDescription);
				EventQueue[messageId]=e;
				Environment.Instance.DebugIf ("messages", "[TP][" + Name + "]Queued reception of " + e.ToString () + ".");
			}
		}


        internal void Pong(long roundTripTime)
        {
            timeZone = characterClients.CentralTime - roundTripTime;
        }


        public string ExecutionCommand()
        {
			return slowProxy.GetExecutionCommand();
        }

        public string ExecutionParameters()
        {
			return slowProxy.GetExecutionParameters();
        }

        
    }
}
