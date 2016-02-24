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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Data;
using ThalamusLogTool;

namespace Thalamus
{
    public partial class frmThalamus : Form
    {

        public GraphSharpControl GraphControl { get; set; }

        private ThalamusClientProxy selectedGraphClient = null;

        public frmThalamus(string startupParameter = "", bool csv = false, bool loadScenario = false)
        {
            InitializeComponent();

            GraphControl = new GraphSharpControl();
            wpfGraph.Child = GraphControl;

            RefreshInfo();

            thalamus.Setup();
            thalamus.Start();


            logToCsvDefault = csv;

            if (startupParameter != "")
            {
                if (loadScenario) {
                    if (thalamus.Scenarios.ContainsKey(startupParameter))
                    {
                        selectedScenario = thalamus.Scenarios[startupParameter];
                        LoadSelectedScenario();
                    }
                    else MessageBox.Show("Unable to load startup scenario '" + startupParameter + "'!", "Load Scenario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else selectedCharacter = NewCharacter(startupParameter);
            }
            

            
            lstSendAction.Sorted = true;
            lstSendPerception.Sorted = true;
        }

        

        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form();
                prompt.Width = 500;
                prompt.Text = caption;
                Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Height = 150;
                prompt.ShowDialog();
                return textBox.Text;
            }
        }

        private Environment thalamus = Environment.Instance;

        private Character selectedCharacter = null;
        public Character SelectedCharacter
        {
            get { return selectedCharacter; }
        }
        private string selectedBehavior = "";
        private Scenario selectedScenario = Scenario.Null;

        int maxInboundEventsPerSecond = int.MinValue;
        int maxOutboundEventsPerSecond = int.MinValue;

        private bool DontRefresh = false;
        private bool logToCsvDefault = false;

        private void RefreshInfo()
        {
            if (DontRefresh) return;
            lstCharacters.Items.Clear();
            foreach (string c in thalamus.Characters.Keys)
            {
                lstCharacters.Invoke((MethodInvoker)(() =>
                {
                    DontRefresh = true;
                    lstCharacters.Items.Add(c);
                    DontRefresh = false;
                }));
                
            }

            for (int i = 0; i < lstCharacters.Items.Count; i++)
            {
                if (selectedCharacter != null && selectedCharacter.Name == lstCharacters.Items[i].ToString())
                {
                    lstCharacters.Invoke((MethodInvoker)(() => {
                        DontRefresh = true;
                        lstCharacters.SelectedIndex = i;
                        DontRefresh = false;
                    }));
                    break;
                }
            }

            lstScenario.Items.Clear();
            foreach (string s in thalamus.Scenarios.Keys)
            {
                lstCharacters.Invoke((MethodInvoker)(() =>
                {
                    DontRefresh = true;
                    lstScenario.Items.Add(s);
                    DontRefresh = false;
                }));
            }

            if (selectedScenario != Scenario.Null)
            {
                btnLoadScenario.Enabled = true;
                btnDeleteScenario.Enabled = true;
                lstCharacters.Invoke((MethodInvoker)(() =>
                {
                    DontRefresh = true;
                    lstScenario.SelectedItem = selectedScenario.Name;
                    DontRefresh = false;
                }));
            }
            else
            {
                btnLoadScenario.Enabled = false;
                btnDeleteScenario.Enabled = false;
            }

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    RefreshTabCreate();
                    break;
                case 1:
                    RefreshTabBehavior();
                    break;
                case 2:
                    RefreshTabEvents();
                    break;
                case 3:
                    RefreshTabGraph();
                    break;
            }
        }

        private void RefreshTabGraph()
        {
            if (selectedGraphClient == null && selectedCharacter !=null)
            {

                Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
                bool emptyGraph = true;
                foreach (ThalamusClientProxy client in selectedCharacter.Clients.RemoteClients.Values)
                {
                    nodes[client.Name] = new List<string>();
                    foreach (string eventName in client.SubscribedEvents)
                    {
                        foreach (ThalamusClientProxy otherClient in selectedCharacter.Clients.RemoteClients.Values)
                        {
                            if (otherClient != client && otherClient.AnnouncedEvents.Contains(eventName))
                            {
                                nodes[client.Name].Add(otherClient.Name);
                                emptyGraph = false;
                            }
                        }
                    }
                }
                GraphControl.ClearNodes();
                if (!emptyGraph)
                {
                    GraphControl.ClearNodes();
                    foreach (KeyValuePair<string, List<string>> node in nodes)
                    {
                        GraphControl.AddMainNode(node.Key, node.Value);
                    }
                    GraphControl.CommitGraph();
                    GraphControl.RenderGraph();
                }
            }
            /*
            GraphControl.AddNode("Um", new List<string>() { "A", "B" }, new List<string>() { "C" });
            GraphControl.AddNode("Dois", new List<string>() { "A", "D" }, new List<string>() { "B", "A" });
            GraphControl.AddNode("Tres", new List<string>() { "C", "E", "F" }, new List<string>() { });
            GraphControl.AddNode("Quatro", new List<string>() { }, new List<string>() { "F" });
            */
        }

        private void RefreshTabEvents()
        {
            if (DontRefresh) return;
            DontRefresh = true;
            try {
                lstSendAction.Items.Clear();
                lstSendPerception.Items.Clear();
                if (selectedCharacter == null)
                {
                    grpSendAction.Enabled = false;
                    grpSendPerception.Enabled = false;
                }
                else
                {
                    grpSendAction.Enabled = true;
                    grpSendPerception.Enabled = true;

                    List<PML> eventInfo;
                    lock (selectedCharacter.Clients.EventInfo)
                    {
                        eventInfo = new List<PML>(selectedCharacter.Clients.EventInfo.Values);
                    }
                    foreach (PML ev in eventInfo)
                    {
                        if (ev.EventType == PMLEventType.Action)
                        {
                            lstSendAction.Items.Add(ev.Name);
                        }
                        else
                        {
                            lstSendPerception.Items.Add(ev.Name);
                        }
                    }

                    lstSendAction.Sorted = true;
                    lstSendPerception.Sorted = true;

                    /*foreach (ThalamusClientProxy client in selectedCharacter.Clients.RemoteClients.Values)
                    {
                        List<string> subscribedEvents = new List<string>(client.SubscribedEvents);

                        foreach (string ev in subscribedEvents)
                        {
                            if (ev.ToLower().Contains("action") && !lstSendAction.Items.Contains(ev)) lstSendAction.Items.Add(ev);
                            else if (!ev.ToLower().Contains("action") && !lstSendPerception.Items.Contains(ev)) lstSendPerception.Items.Add(ev);
                        }
                    }*/
                    if (selectedActionIndex < lstSendAction.Items.Count) lstSendAction.SelectedIndex = selectedActionIndex;
                    else selectedActionIndex = -1;

                    if (selectedPerceptionIndex < lstSendPerception.Items.Count) lstSendPerception.SelectedIndex = selectedPerceptionIndex;
                    else selectedPerceptionIndex = -1;

                    dgvAction.Rows.Clear();
                    dgvPerception.Rows.Clear();


                    dgvAction.Enabled = false;
                    if (selectedActionIndex != -1)
                    {
                        if (selectedCharacter.Clients.EventInfo.ContainsKey(lstSendAction.SelectedItem as string))
                        {
                            PopulateDataView(dgvAction, selectedCharacter.Clients.EventInfo[lstSendAction.SelectedItem as string]);
                            dgvAction.Enabled = true;
                        }
                    }


                    dgvPerception.Enabled = false;
                    if (selectedPerceptionIndex != -1)
                    {
                        if (selectedCharacter.Clients.EventInfo.ContainsKey(lstSendPerception.SelectedItem as string))
                        {
                            PopulateDataView(dgvPerception, selectedCharacter.Clients.EventInfo[lstSendPerception.SelectedItem as string]);
                            dgvPerception.Enabled = true;
                        }
                    }
                }
            }
            catch 
            {
            }
            finally 
            {
                DontRefresh = false;
            }
        }

        private void PopulateDataView(DataGridView dgv, PML thalamusEvent)
        {
            int rowNumber = 0;
            foreach (KeyValuePair<string, PMLParameter> param in thalamusEvent.Parameters)
            {
                string[] row = { param.Key, param.Value.Type.ToString(), param.Value.GetValue() };
                dgv.Rows.Add(row);
                if (param.Value.Type == PMLParameterType.Enum && PML.EnumInfo.ContainsKey(param.Value.ShortEnumName))
                {
                    DataGridViewComboBoxCell dgvCbcell = new DataGridViewComboBoxCell();
                    foreach(string enumMember in PML.EnumInfo[param.Value.ShortEnumName]) {
                        dgvCbcell.Items.Add(enumMember);
                    }
                    dgv.Rows[rowNumber].Cells[2] = dgvCbcell;
                }
                rowNumber++;
            }
        }

        private void SetupEventTables()
        {
            DataGridViewComboBoxColumn dgvCbcActions = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn dgvCbcPerceptions = new DataGridViewComboBoxColumn();
            dgvCbcActions.Items.Add(PMLParameterType.Bool.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.Double.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.Int.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.String.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.Enum.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.BoolArray.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.DoubleArray.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.IntArray.ToString());
            dgvCbcActions.Items.Add(PMLParameterType.StringArray.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.Bool.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.Double.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.Int.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.String.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.Enum.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.BoolArray.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.DoubleArray.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.IntArray.ToString());
            dgvCbcPerceptions.Items.Add(PMLParameterType.StringArray.ToString());

            dgvAction.ColumnCount = 2;
            dgvAction.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAction.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAction.GridColor = Color.Black;
            dgvAction.RowHeadersVisible = false;
            dgvAction.Columns[0].Name = "Parameter";
            dgvAction.Columns[0].ReadOnly = true;
            dgvAction.Columns.Insert(1, dgvCbcActions);
            dgvAction.Columns[1].Name = "Type";
            dgvAction.Columns[1].ReadOnly = true;
            dgvAction.Columns[2].Name = "Value";
            dgvAction.Columns[1].DefaultCellStyle.Font = new Font(dgvAction.DefaultCellStyle.Font, FontStyle.Italic);
            dgvAction.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvAction.MultiSelect = false;

            dgvPerception.ColumnCount = 2;
            dgvPerception.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvPerception.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvPerception.GridColor = Color.Black;
            dgvPerception.RowHeadersVisible = false;
            dgvPerception.Columns[0].Name = "Parameter";
            dgvPerception.Columns[0].ReadOnly = true;
            dgvPerception.Columns.Insert(1, dgvCbcPerceptions);
            dgvPerception.Columns[1].Name = "Type";
            dgvPerception.Columns[1].ReadOnly = true;
            dgvPerception.Columns[2].Name = "Value";
            dgvPerception.Columns[1].DefaultCellStyle.Font = new Font(dgvAction.DefaultCellStyle.Font, FontStyle.Italic);
            dgvPerception.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvPerception.MultiSelect = false;
        }

        

        private void RefreshTabBehavior()
        {
            if (DontRefresh) return;
           
            btnRunBML.Enabled = (selectedCharacter != null);
            btnCancelAllBehaviors.Enabled = (selectedCharacter != null);

            for (int i = 0; i < lstBehaviors.Items.Count; i++)
            {
                if (selectedBehavior != "" && selectedBehavior == lstBehaviors.Items[i].ToString())
                {
                    DontRefresh = true;
                    lstBehaviors.SelectedIndex = i;
                    DontRefresh = false;
                    break;
                }
            }

        }

        private Thalamus.ThalamusClientProxy selectedBodyServer = null;

        private void RefreshTabCreate()
        {
            if (DontRefresh) return;
            DontRefresh = true;

            rlbNetworks.Items.Clear();
            int i = 0;
            foreach (KeyValuePair<int, string> network in thalamus.AvailableNetworks)
            {
                rlbNetworks.Items.Add(network.Key + " |" + network.Value);
                if (thalamus.SelectedNetworkAdapter == network.Key) rlbNetworks.SelectedIndex = i;
                i++;
            }
            if (rlbNetworks.Items.Count>0 && rlbNetworks.SelectedIndex == -1) rlbNetworks.SelectedIndex = 0;

            lstCharacters.Items.Clear();
            foreach (string c in thalamus.Characters.Keys)
            {
                lstCharacters.Items.Add(c);
            }

            for (i = 0; i < lstCharacters.Items.Count; i++)
            {
                if (selectedCharacter != null && selectedCharacter.Name == lstCharacters.Items[i].ToString())
                {
                    lstCharacters.SelectedIndex = i;
                    break;
                }
            }

            lstBodyServer.Items.Clear();

            if (selectedCharacter == null)
            {
                lstBodyServer.Enabled = false;

                txtName.Text = "";
                btnCreateCharacter.Text = "Create Character";
                lblCharacterPort.Text = "n/a";
                chkCSVLog.Enabled = false;
            }
            else
            {
                chkCSVLog.Enabled = true;
                chkCSVLog.Checked = selectedCharacter.LogToCSV;

                btnCreateCharacter.Text = "Terminate Character";
                lstBodyServer.Enabled = true;
                lblCharacterPort.Text = selectedCharacter.Clients.FastLocalPort.ToString();
                txtName.Text = selectedCharacter.Name;
                
				foreach (ThalamusClientProxy bs in selectedCharacter.Clients.RemoteClients.Values) lstBodyServer.Items.Add(bs);
                for (i = 0; i < lstBodyServer.Items.Count; i++)
                {
                    if (selectedBodyServer != null && selectedCharacter.Clients.RemoteClients.ContainsValue(selectedBodyServer))
                    {
                        lstBodyServer.SelectedIndex = i;
                        break;
                    }
                }
            }
            DontRefresh = false;
        }

        

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInfo();
            if (tabControl1.SelectedTab.Text == "BML")
            {
                lstBehaviors.Items.Clear();
                foreach (string b in BehaviorManager.Instance.Behaviors.Keys)
                {
                    lstBehaviors.Items.Add(b);
                }
            }
        }

        private void lstCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (thalamus.Characters.ContainsKey(lstCharacters.SelectedItem.ToString()))
            {
                selectedCharacter = thalamus.Characters[lstCharacters.SelectedItem.ToString()];
                               
                lstBodyServer.Items.Clear();
                selectedCharacter = thalamus.Characters[lstCharacters.SelectedItem.ToString()];
				foreach (ThalamusClientProxy rc in selectedCharacter.Clients.RemoteClients.Values) lstBodyServer.Items.Add(rc);

            }
            else selectedCharacter = null;
            
            RefreshInfo();
        }

        private void lstBehaviors_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBehavior = lstBehaviors.SelectedItem.ToString();
            RefreshInfo();
        }
        

		

		private void btnStartBenchmark_Click(object sender, EventArgs e) {
			
		}

		public void PerformanceTimerHandler(int inboundEventsPerSecond, int outboundEventsPerSecond) {
            if (maxInboundEventsPerSecond < inboundEventsPerSecond) maxInboundEventsPerSecond = inboundEventsPerSecond;
            if (maxOutboundEventsPerSecond < outboundEventsPerSecond) maxOutboundEventsPerSecond = outboundEventsPerSecond;
            if (lblPerformanceMessageRate.IsHandleCreated)
            {
                lblPerformanceMessageRate.Invoke((MethodInvoker)(() =>
                {
                    lblPerformanceMessageRate.Text = String.Format("Message rate (/sec): Inbound {0}; Outbound {1}", inboundEventsPerSecond, outboundEventsPerSecond);
                    lblPerformanceMessageRate.Refresh();
                    lblPerformanceMaxRate.Text = String.Format("Max rate (/sec): Inbound {0}; Outbound {1}", maxInboundEventsPerSecond, maxOutboundEventsPerSecond);
                    lblPerformanceMaxRate.Refresh();
                }));
            }
		}

		




        

        private void btnCancelAllBehaviors_Click(object sender, EventArgs e)
        {
            BehaviorPlan.Instance.CancelCurrentPlan();
        }

        private void btnPlanSendEvent_Click(object sender, EventArgs e)
        {
            if (selectedCharacter != null) selectedCharacter.SyncPoint(txtPlanSendEvent.Text);
            else BehaviorPlan.Instance.Event(txtPlanSendEvent.Text);
        }

        bool isClosing = false;
        private void frmThalamus_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
            CloseClients("");
            Environment.Instance.Dispose();
        }

        private Character NewCharacter(string name)
        {
            selectedCharacter = new Character(name);

            if (logToCsvDefault && selectedCharacter != null)
            {
                selectedCharacter.LogToCSV = true;
                Environment.Instance.ResetCSV(selectedCharacter);
            }

            thalamus.AddCharacter(selectedCharacter);
            /*selectedCharacter.Clients.ClientInfoChanged += new ClientInfoChangedHandler((ClientInfoChangedHandler)(
                (clientId) =>
                {
                    if (!isClosing) tabControl1.Invoke((MethodInvoker)(() => RefreshInfo()));
                }
                ));*/
            //RefreshInfo();
            return selectedCharacter;
        }

        private void btnCreateCharacter_Click(object sender, EventArgs e)
        {
            if (thalamus.Characters.ContainsKey(txtName.Text))
            {
                //MessageBox.Show("A character named '" + txtName.Text + "' already exists!", "Create Character", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                CloseClients(txtName.Text);
                thalamus.DeleteCharacter(txtName.Text);
                selectedCharacter = null;
                RefreshInfo();
            }
            else
            {
                NewCharacter(txtName.Text);
                RefreshInfo();
            }
        }


        private void numLocalPort_ValueChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }


        private ThalamusLogTool.frmLog logWindow;
        private frmConflictResolution conflictWindow;

        private void frmThalamus_Load(object sender, EventArgs e)
        {
            SetupEventTables();
            RefreshInfo();
            CreateLogWindow();
            CreateConflictWindow();
            thalamus.PerformanceTimer += new Environment.PerformanceTimerHandler(PerformanceTimerHandler);
            thalamus.EventInformationChanged += (EventInformationChangedHandler)((character) =>
            {
                if (selectedCharacter == character && !isClosing) tabControl1.Invoke((MethodInvoker)(() => RefreshInfo()));
            });
        }

        private void lstPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedScenario = thalamus.GetScenario((string)lstScenario.SelectedItem);
            RefreshInfo();
        }

        private void btnLoadScenario_Click(object sender, EventArgs e)
        {
            LoadSelectedScenario();
            RefreshInfo();
        }

        private void LoadSelectedScenario()
        {
            if (!selectedScenario.IsNull)
            {
                Character[] chars = new Character[thalamus.Characters.Count];
                thalamus.Characters.Values.CopyTo(chars, 0);
                foreach (Character c in chars) thalamus.DeleteCharacter(c);
                CloseClients("");

                foreach (CharacterDefinition cd in selectedScenario.Characters)
                {
                    Character newCharacter = new Character(cd.Name);
                    if (selectedCharacter == null) selectedCharacter = newCharacter;
                    if (logToCsvDefault && selectedCharacter != null)
                    {
                        selectedCharacter.LogToCSV = true;
                        Environment.Instance.ResetCSV(selectedCharacter);
                    }

                    thalamus.AddCharacter(newCharacter);

                    foreach (CharacterDefinition.Client client in cd.Clients)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(ClientThread));
                        t.Start(new string[] { client.CommandLine, client.Arguments, newCharacter.Name});
                        Thread.Sleep(1000);
                    }
                }
                lstCharacters.SelectedIndex = lstCharacters.Items.Count - 1;
            }
        }

        

        private Dictionary<string, List<Process>> launchedClients = new Dictionary<string, List<Process>>();

        public void ClientThread(object ocmd)
        {
            string[] cmd = (string[])ocmd;
            try
            {
                ProcessStartInfo pinfo = new ProcessStartInfo() { Verb = "runas" };
                pinfo.WorkingDirectory = Path.GetDirectoryName(cmd[0]);
                pinfo.FileName = cmd[0];
                pinfo.Arguments = cmd[1];
                string characterName = cmd[2];
                lock (launchedClients)
                {
                    if (!launchedClients.ContainsKey(characterName)) launchedClients[characterName] = new List<Process>();
                    launchedClients[characterName].Add(Process.Start(pinfo));
                    Console.WriteLine("Launched client '" + pinfo.FileName + "'");
                }
            }
            catch (Exception e)
            {
                Environment.Instance.Debug("Failed to launch client '" + cmd[0] + " " + cmd[1] + ": " + e.Message);
            }
        }

        private void CloseClients(string character)
        {
            if (character == "")
            {
                foreach (List<Process> clients in launchedClients.Values)
                {
                    foreach (Process p in clients)
                    {
                        if (!p.HasExited) p.Kill();
                    }
                }
            }
            else if (launchedClients.ContainsKey(character)) 
            {
                foreach (Process p in launchedClients[character])
                {
                    if (!p.HasExited) p.Kill();
                }
            }
            
        }

        private void lstPresets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LoadSelectedScenario();
            RefreshInfo();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (thalamus.Characters.ContainsKey(txtName.Text)) btnCreateCharacter.Text = "Terminate Character";
            else btnCreateCharacter.Text = "Create Character";
        }

        private void lstBehaviors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            loadBmlFile();
        }

        private void btnSaveScenario_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Thalamus Scenario |*.thalscn";
            saveFileDialog.Title = "Save Scenario";
            saveFileDialog.InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + "\\" + thalamus.ScenariosDirectory);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Scenario newScenario = new Scenario();
                newScenario.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                foreach(Character c in thalamus.Characters.Values) {
                    CharacterDefinition cd = new CharacterDefinition();
                    cd.Name = c.Name;
                    newScenario.Characters.Add(cd);
                    foreach (KeyValuePair<string, ThalamusClientProxy> client in c.Clients.RemoteClients)
                    {
                        CharacterDefinition.Client clientCommand = new CharacterDefinition.Client();
                        clientCommand.CommandLine = client.Value.ExecutionCommand();
                        if (clientCommand.CommandLine.EndsWith("vshost.exe")) clientCommand.CommandLine = clientCommand.CommandLine.Replace(".vshost.exe", ".exe");
                        clientCommand.Arguments = client.Value.ExecutionParameters();
                        Console.WriteLine(String.Format("Execution command for '{0}': '{1}'.", client.Key, clientCommand.ToString()));
                        cd.Clients.Add(clientCommand);
                    }
                }

                if (thalamus.GetScenario(newScenario.Name) != Thalamus.Scenario.Null)
                {
                    thalamus.Scenarios.Remove(newScenario.Name);
                }

                thalamus.Scenarios[newScenario.Name] = newScenario;
                Scenario.Save(saveFileDialog.FileName, newScenario);
                lstScenario.Items.Clear();
                foreach (string s in thalamus.Scenarios.Keys)
                {
                    lstScenario.Items.Add(s);
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            BehaviorManager.Instance.Setup();
            lstBehaviors.Items.Clear();
            foreach (string b in Thalamus.BehaviorManager.Instance.Behaviors.Keys)
            {
                lstBehaviors.Items.Add(b);
            }
        }

        private void loadBmlFile()
        {
            if (lstBehaviors.SelectedItem != null)
            {
                if (BehaviorManager.Instance.OriginalBmlFiles.ContainsKey(lstBehaviors.SelectedItem as string))
                {
                    //Console.WriteLine(File.ReadAllText(BehaviorManager.Instance.OriginalBmlFiles[lstBehaviors.SelectedItem as string]).Replace("\r\n", "\n").Replace("\n", "\r\n"));
                    txtBml.Text = File.ReadAllText(BehaviorManager.Instance.OriginalBmlFiles[lstBehaviors.SelectedItem as string]).Replace("\r\n", "\n").Replace("\n", "\r\n");
                    /*txtBml.Text = File.ReadAllText(BehaviorManager.Instance.OriginalBmlFiles[lstBehaviors.SelectedItem as string]).Replace("\r\n", "\n").Replace("\n", "\r\n");
                    txtBml.Process(true);
                    txtBml.ZoomFactor = 0.8f;*/
                }
                else
                {
                    txtBml.Text = BehaviorManager.Instance.Behaviors[lstBehaviors.SelectedItem as string].ToBml();
                    //MessageBox.Show("The selected behavior was created in runtime and therefore does no raw BML code is available to load!", "Load BML code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadSelected_Click(object sender, EventArgs e)
        {
            loadBmlFile();
        }

        private void btnRunBML_Click(object sender, EventArgs e)
        {
            if (selectedCharacter != null) BehaviorManager.Instance.RunBML(txtBml.Text, selectedCharacter);
            else BehaviorManager.Instance.RunBML(txtBml.Text);
        }

        private void btnNewBodyServer_Click(object sender, EventArgs e)
        {
            
        }

        private void lstBodyServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBodyServer = lstBodyServer.SelectedItem as ThalamusClientProxy;
        }

        private void btnSendPerception_Click(object sender, EventArgs e)
        {
            //if (selectedCharacter != null && lstSendPerception.SelectedItem != null) selectedCharacter.Body.ReceivePerception(lstSendPerception.SelectedItem as string, new string[] { }, new string[] { }, new string[] { });
            if (selectedCharacter != null && lstSendPerception.SelectedItem != null)
            {
                string[] parameters = new string[0];
                string[] types = new string[0];
                string[] values = new string[0];
                if (selectedCharacter.Clients.EventInfo.ContainsKey(lstSendPerception.SelectedItem as string))
                {
                    parameters = new string[dgvPerception.Rows.Count];
                    types = new string[dgvPerception.Rows.Count];
                    values = new string[dgvPerception.Rows.Count];
                    int i = 0;
                    foreach (DataGridViewRow row in dgvPerception.Rows)
                    {
                        parameters[i] = row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString();
                        types[i] = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();
                        if (types[i] == PMLParameterType.Enum.ToString())
                        {
                            PML pml = selectedCharacter.Clients.EventInfo[lstSendPerception.SelectedItem as string];
                            types[i] = pml.Parameters[parameters[i]].EnumName;
                        }
                        values[i] = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();
                        if (types[i].ToLower().EndsWith("array") && values[i] != "")
                        {
                            values[i].Replace(',', DataTypes.ArrayItemSeparator);
                        }
                        i++;
                    }
                }
				selectedCharacter.Clients.PublishEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), lstSendPerception.SelectedItem as string, false, parameters, types, values);
            }
        }

        private void btnActionName_Click(object sender, EventArgs e)
        {
            if (selectedCharacter != null && lstSendAction.SelectedItem != null)
            {
                string[] parameters = new string[0];
                string[] types = new string[0];
                string[] values = new string[0];
                if (selectedCharacter.Clients.EventInfo.ContainsKey(lstSendAction.SelectedItem as string))
                {
                    parameters = new string[dgvAction.Rows.Count];
                    types = new string[dgvAction.Rows.Count];
                    values = new string[dgvAction.Rows.Count];
                    int i = 0;
                    foreach (DataGridViewRow row in dgvAction.Rows)
                    {
                        parameters[i] = row.Cells[0].Value==null ? "":row.Cells[0].Value.ToString();
                        types[i] = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();
                        if (types[i] == PMLParameterType.Enum.ToString())
                        {
                            PML pml = selectedCharacter.Clients.EventInfo[lstSendAction.SelectedItem as string];
                            types[i] = pml.Parameters[parameters[i]].EnumName;
                        }
                        values[i] = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();
                        if (types[i].ToLower().EndsWith("array") && values[i]!="")
                        {
                            values[i].Replace(',', DataTypes.ArrayItemSeparator);
                        }
                        i++;
                    }
                }
				selectedCharacter.Clients.PublishEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), lstSendAction.SelectedItem as string, false, parameters, types, values);
            }
        }

        int selectedActionIndex = -1;
        int selectedPerceptionIndex = -1;

        private void lstSendAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedActionIndex = lstSendAction.SelectedIndex;
            RefreshInfo();
        }

        private void lstSendPerception_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPerceptionIndex = lstSendPerception.SelectedIndex;
            RefreshInfo();
        }

        private void btnActionAddParameter_Click(object sender, EventArgs e)
        {
            dgvAction.Rows.Add();
        }

        private void btnPerceptionAddParameter_Click(object sender, EventArgs e)
        {
            dgvPerception.Rows.Add();
        }

        private void btnPerceptionRemoveParameter_Click(object sender, EventArgs e)
        {

        }

        private void btnActionRemoveParameter_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteScenario_Click(object sender, EventArgs e)
        {
            thalamus.DeleteScenario(selectedScenario.Name);
            RefreshInfo();
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (DontRefresh) return;
            if (logWindow.Disposing || logWindow.IsDisposed)
            {
                CreateLogWindow();
            }
            logWindow.SetVisible(chkEventLog.Checked);
        }

        private void CreateLogWindow()
        {
            logWindow = new frmLog();
            logWindow.FormClosing += new FormClosingEventHandler((FormClosingEventHandler)(
                    (s, ev) =>
                    {
                        DontRefresh = true;
                        chkEventLog.Checked = false;
                        DontRefresh = false;
                    }
                    ));
            thalamus.EventLogged += new EventLoggedHandler((EventLoggedHandler)(
                (logEntry) =>
                {
                    if (!this.isClosing) logWindow.AddLog(logEntry);
                }));
            thalamus.EventInformationChanged += (EventInformationChangedHandler)((character) =>
            {
                if (logWindow.Visible) logWindow.Invoke((MethodInvoker)(() => logWindow.CollectEventNames()));
            });
        }

        private void CreateConflictWindow()
        {
            conflictWindow = new frmConflictResolution(this);
            conflictWindow.FormClosing += new FormClosingEventHandler((FormClosingEventHandler)(
                    (s, ev) =>
                    {
                        DontRefresh = true;
                        chkConflictWindow.Checked = false;
                        DontRefresh = false;
                    }
                    ));

            ConflictManager.Instance.ConflictsChanged += (ConflictManager.ConflictsChangedHandler)((character) =>
            {
                if (conflictWindow.Visible) conflictWindow.Invoke((MethodInvoker)(() => conflictWindow.ConflictsChanged(character)));

            });
        }

        private void lstCharacters_Click(object sender, EventArgs e)
        {
            Thread.Sleep(50);
            /*if (selectedCharacter != null)
            {
                if (!logs.ContainsKey(selectedCharacter)) logs.Add(selectedCharacter, new BindingList<LogEntry>());
                if (logWindow.Visible)
                {
                    logWindow.SetLog(logs[selectedCharacter]);
                }
            }*/
        }



        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkCSVLog_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedCharacter != null)
            {
                selectedCharacter.LogToCSV = chkCSVLog.Checked;
                if (chkCSVLog.Checked) Environment.Instance.ResetCSV(selectedCharacter);
            }
        }

        private void chkConflictWindow_CheckedChanged(object sender, EventArgs e)
        {
            if (DontRefresh) return;
            if (conflictWindow.Disposing || conflictWindow.IsDisposed)
            {
                CreateLogWindow();
            }
            conflictWindow.SetVisible(chkConflictWindow.Checked);
        }

        private void dgvAction_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void rlbNetworks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rlbNetworks.SelectedItem !=null) {
                thalamus.SelectNetwork(int.Parse(rlbNetworks.SelectedItem.ToString().Split(' ')[0]));
            }
        }
    }
}
