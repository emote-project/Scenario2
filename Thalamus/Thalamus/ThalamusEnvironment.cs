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
using System.IO;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Thalamus
{
    
    public delegate void ClientDisconnectedHandler(string clientId);
    public delegate void ClientConnectedHandler(string clientId, int port);
    public delegate void EventInformationChangedHandler(Character character);

    public class Environment : Manager
    {

        public event ClientDisconnectedHandler ClientDisconnected;
        public void NotifyClientDisconnected(string clientId)
        {
            if (ClientDisconnected != null) ClientDisconnected(clientId);
        }

        public event ClientConnectedHandler ClientConnected;
        public void NotifyClientConnected(string clientId, int port)
        {
            if (ClientConnected != null) ClientConnected(clientId, port);
        }

        public event EventInformationChangedHandler EventInformationChanged;
        public void NotifyEventInformationChanged(Character character)
        {
            if (EventInformationChanged != null) EventInformationChanged(character);
        }

        #region Singleton
        private static Environment instance;
        public static Environment Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Environment();

                }
                return instance;
            }
        }

        private Environment()
            : base("ThalamusMaster")
        {;
            setDebug(true);
            setDebug("error", true);
            setDebug("all", true);
			setDebug ("messages", false);
            Debug("Created ThalamusEnvironment instance");
        }
        #endregion

        #region Properties

        public String ScenariosDirectory
        {
            get { return Properties.Settings.Default.ScenariosDirectory; }
        }

        

        public SortedDictionary<string, Character> Characters = new SortedDictionary<string, Character>();
        public Dictionary<string, Scenario> Scenarios = new Dictionary<string, Scenario>();

        private bool HasSetup = false;
        private bool HasStarted = false;

		System.Timers.Timer timerPerformance;

        #endregion

		#region Performance profiler

		public int InboundEventsTotal {
			get {
				int total = 0;
				foreach (Character c in Characters.Values)
					total += c.Clients.InboundEventsTotal;
				return total;
			}
		}

		public int OutboundEventsTotal {
			get {
				int total = 0;
				foreach (Character c in Characters.Values)
					total += c.Clients.OutboundEventsTotal;
				return total;
			}
		}

		public delegate void PerformanceTimerHandler(int inboundEventsPerSecond, int outboundEventsPerSecond);
		public event PerformanceTimerHandler PerformanceTimer;
		private void NotifyPerformanceTimer(int inboundEventsPerSecond, int outboundEventsPerSecond) {
			if (PerformanceTimer!=null) PerformanceTimer(inboundEventsPerSecond, outboundEventsPerSecond);
		}

		private void TimerPerformanceCounter(object sender, ElapsedEventArgs e)
		{
			int inboundEventsPerSecond = 0;
			int outboundEventsPerSecond = 0;
			foreach (Character c in Characters.Values) {
                c.Clients.TimerPerformanceCounter();
				inboundEventsPerSecond += c.Clients.InboundEventsPerSecond;
				outboundEventsPerSecond += c.Clients.OutboundEventsPerSecond;
			}
			NotifyPerformanceTimer(inboundEventsPerSecond, outboundEventsPerSecond);
			//Console.WriteLine(String.Format("Per second: Inbound: {0} / Outbound: {1}", inboundEventsPerSecond, outboundEventsPerSecond));
		}

		#endregion

        #region Characters

        public void AddCharacter(Character character)
        {
			lock (Characters) {
				if (Characters.ContainsKey (character.Name))
					Characters [character.Name] = character;
				else
					Characters.Add (character.Name, character);
			}
            if (HasSetup) character.Setup();
            if (HasStarted) character.Start();
            DebugIf("all", "Added Character '" + character.ToString() + "'");
        }

        public void DeleteCharacter(string Name)
        {
			if (Characters.ContainsKey (Name))
				DeleteCharacter (Characters [Name]);	
        }
        public void DeleteCharacter(Character Character)
        {
			lock (Characters) {
				Characters.Remove (Character.Name);
				Character.Dispose ();
			}
        }

        #endregion

        public static void AuthorizeAddress(int port, string user = "")
        {
            if (user == "") user = System.Environment.UserName;
            string args = string.Format(@"http add urlacl http://*:{0}/ user={1} listen=yes", port, user);

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();
        }

        private int selectedIPNetworkOrder = 0;
        public int SelectedIPNetworkOrder
        {
            get { return selectedIPNetworkOrder; }
        }
        private int selectedNetworkAdapter = Properties.Settings.Default.SelectedNetworkDevice;
        public int SelectedNetworkAdapter
        {
            get { return selectedNetworkAdapter; }
        }
        private Dictionary<int, string> availableNetworks = new Dictionary<int, string>();
        private Dictionary<string, string> broadcastAddresses = new Dictionary<string, string>();
        public Dictionary<int, string> AvailableNetworks
        {
            get { return availableNetworks; }
        }

        public void SelectNetwork(int networkIndex)
        {

            selectedNetworkAdapter = networkIndex;
            Debug("Changed selected network to: " + availableNetworks[networkIndex]);
            Properties.Settings.Default.SelectedNetworkDevice = networkIndex;
            Properties.Settings.Default.Save();
            /*
            if (ipAddress == "0.0.0.0")
            {
                selectedNetworkAdapter = IPAddress.Any;
                Debug("Changed selected network to: " + selectedNetworkAdapter + " | " + IPAddress.Any.ToString());
            }
            else
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    foreach (var ip in properties.UnicastAddresses)
                    {
                        if ((adapter.OperationalStatus == OperationalStatus.Up) && (ip.Address.AddressFamily == AddressFamily.InterNetwork))
                        {
                            if (ipAddress == ip.Address.ToString())
                            {
                                
                            }
                        }
                    }
                }
            }*/
        }

        public override bool Setup()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            /*availableNetworks["0.0.0.0"] = "Any";
            broadcastAddresses["0.0.0.0"] = IPAddress.Broadcast.ToString();*/
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();

                if (!adapter.SupportsMulticast)
                    continue; // multicast is meaningless for this type of connection
                if (OperationalStatus.Up != adapter.OperationalStatus)
                    continue; // this adapter is off or not connected
                IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
                if (null == p)
                    continue; // IPv4 is not configured on this adapter

                Debug("Found network adapter: " + p.Index.ToString() + "|" + p.ToString() + " | " + adapter.Description.ToString());

                availableNetworks[p.Index] = adapter.Description.ToString();
                /*foreach (var ip in properties.UnicastAddresses)
                {
                    if ((adapter.OperationalStatus == OperationalStatus.Up) && (ip.Address.AddressFamily == AddressFamily.InterNetwork))
                    {
                        
                        
                    }
                }*/
                
            }

			foreach (Character c in Characters.Values) c.Setup();
            if (!BehaviorManager.Instance.Setup()) {
				Dispose();
				return false;
			}
			if (!BehaviorPlan.Instance.Setup()) {
				Dispose();
				return false;
			}


            TrackingManager.Instance.Setup();

            LoadScenarios();

            HasSetup = base.Setup();
            return HasSetup;
        }

        private void LoadScenarios()
        {
            Scenarios = new Dictionary<string, Scenario>();
            if (Directory.Exists(CorrectPath(Properties.Settings.Default.ScenariosDirectory)))
            {
                DirectoryInfo di = new DirectoryInfo(CorrectPath(Properties.Settings.Default.ScenariosDirectory));
                FileInfo[] rgFiles = di.GetFiles("*.thalscn");
                foreach (FileInfo fi in rgFiles)
                {
                    DebugIf("all", "Load Scenario '" + fi.FullName + "'");
                    Scenario s = Scenario.LoadScenario(fi.FullName);
                    Scenarios[s.Name] = s;
                }
            }
            else
            {
                DebugIf("error", "Unable to find Scenarios directory '" + CorrectPath(Properties.Settings.Default.ScenariosDirectory) + "'!");
            }
        }

        public void DeleteScenario(string scenarioName)
        {
            Scenario s = null;
            foreach (Scenario scenario in Scenarios.Values)
            {
                if (scenario.Name == scenarioName)
                {
                    s = scenario;
                }
            }
            if (s != null)
            {
                File.Delete(s.Filename);
                Scenarios.Remove(s.Name);
            }
        }

        public Scenario GetScenario(string scenarioName)
        {
            if (Scenarios.ContainsKey(scenarioName)) return Scenarios[scenarioName];
            return Thalamus.Scenario.Null;
        }
        public override bool Start()
        {
			timerPerformance = new System.Timers.Timer();
			timerPerformance.Interval = 1000;
			timerPerformance.Elapsed += new ElapsedEventHandler(TimerPerformanceCounter);
			timerPerformance.Start();

			foreach (Character c in Characters.Values) c.Start();
            if (!BehaviorManager.Instance.Start()) {
				Dispose();
				return false;
			}
			if (!BehaviorPlan.Instance.Start()) {
				Dispose();
				return false;
			}
            if (!ConflictManager.Instance.Start())
            {
                Dispose();
                return false;
            }
            HasStarted = base.Start();
            return HasStarted;
        }

        public override void Dispose()
        {
			if (timerPerformance!=null) timerPerformance.Stop ();
			lock (Characters) {
				foreach (Character c in Characters.Values)
					try {
						c.Dispose ();
					} catch (Exception e) {
						DebugException(e, 1);
					}
			}

			try {
			    BehaviorPlan.Instance.Dispose();
			}catch(Exception e) {
                DebugException(e, 2);
			}
			
			try {
			    BehaviorManager.Instance.Dispose();
			}catch(Exception e) {
                DebugException(e, 3);
			}

            try {
                TrackingManager.Instance.Dispose();
            } catch (Exception e) {
                DebugException(e, 4);
            }

            try
            {
                ConflictManager.Instance.Dispose();
            }
            catch (Exception e)
            {
                DebugException(e, 5);
            }
            base.Dispose();
            Debug("Waiting for pending threads to shut down...");
        }
    }
}
