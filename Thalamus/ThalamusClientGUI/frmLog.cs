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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Thalamus
{
    public partial class ThalamusClientLogForm : Form
    {

        public interface IThalamusClientLogFormParent
        {
            void SetShowLogButton(bool state);
            ThalamusClient GetClient();
        }

        public static ThalamusClientLogForm CreateLogWindow(IThalamusClientLogFormParent parentForm)
        {
            ThalamusClientLogForm logWindow = new ThalamusClientLogForm(parentForm.GetClient());
            logWindow.FormClosing += new FormClosingEventHandler((FormClosingEventHandler)(
                    (s, ev) =>
                    {
                        logWindow.DontRefresh = true;
                        parentForm.SetShowLogButton(false);
                        logWindow.DontRefresh = false;
                    }
                    ));
            parentForm.GetClient().EventLogged += new EventLoggedHandler((EventLoggedHandler)(
                (logEntry) =>
                {
                    try
                    {
                        if (!logWindow.IsDisposed) logWindow.AddLog(logEntry);
                    }
                    //so it doesn't fail when the program is closing
                    catch {
                        Console.WriteLine("Failed to log event " + logEntry.EventName + ".");
                    } 
                }));
            parentForm.GetClient().ClientConnected += (ThalamusClient.ClientConnectedHandler)(() =>
            {
                if (logWindow.Visible) logWindow.Invoke((MethodInvoker)(() => logWindow.CollectEventNames()));
            });
            parentForm.GetClient().NewClientConnected += (ThalamusClient.NewClientConnectedHandler)((name, newClientId) =>
            {
                if (logWindow.Visible) logWindow.Invoke((MethodInvoker)(() => logWindow.CollectEventNames()));
            });

            return logWindow;
        }

        private Dictionary<string, Boolean> eventNameFilters = new Dictionary<string, bool>();
        private String eventNamesFilter = "";
        private String highlightEventName = "";

        private List<LogEntry> logQueue = new List<LogEntry>();

        Thread tRefresh = null;

        public bool DontRefresh = false;

        Stopwatch refreshClock = new Stopwatch();
        float refreshRate = 2f;

        protected DataTable logs = new DataTable();
        public DataTable Logs
        {
            get { return logs; }
        }

        ThalamusClient client = null;

        public ThalamusClientLogForm(ThalamusClient client)
        {
            InitializeComponent();

            this.client = client;

            logs.Columns.Add("Time", typeof(Double));
            logs.Columns.Add("CharacterName", typeof(String));
            logs.Columns.Add("SourceClient", typeof(String));
            logs.Columns.Add("TargetClient", typeof(String));
            logs.Columns.Add("EventName", typeof(String));
            logs.Columns.Add("EventDetails", typeof(String));
                       
            dgvLog.CellFormatting +=new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvLog_CellFormatting);
            refreshClock.Start();

            lstEventNameFilters.Sorted = true;

            /*client.ClientConnected += (ThalamusClient.ClientConnectedHandler)(() =>
            {
                this.Invoke((MethodInvoker)(() => RebuildFilters()));
            });

            client.NewClientConnected += (ThalamusClient.NewClientConnectedHandler)((name, newClientId) =>
            {
                this.Invoke((MethodInvoker)(() => RebuildFilters()));
            });

            client.EventLogged += (EventLoggedHandler)((logEntry) =>
            {
                AddLog(logEntry);
            });*/

        }

        

        private void RefreshLog()
        {
            while (this.Visible)
            {
                if (!chkHold.Checked && updateLog)
                {
                    List<LogEntry> newLogs;
                    lock (logQueue)
                    {
                        newLogs = new List<LogEntry>(logQueue);
                        logQueue.Clear();
                    }
                    foreach (LogEntry log in newLogs)
                    {
                        logs.Rows.Add(log.Time, log.CharacterName, log.SourceClient, log.TargetClient, log.EventName, log.EventInfo);
                    }
                    if (refreshClock.ElapsedMilliseconds > (1000 / refreshRate))
                    {
                        refreshClock.Reset();
                        this.Invoke((MethodInvoker)(() => ProjectTable()));
                    }
                    int sleep = Math.Max(100,(int)Math.Round((1000.0 / refreshRate) - (int)refreshClock.ElapsedMilliseconds));
                    Thread.Sleep(sleep);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            tRefresh = null;
        }

        public void Clear()
        {
            logs = new DataTable();
            logs.Columns.Add("Time", typeof(Double));
            logs.Columns.Add("CharacterName", typeof(String));
            logs.Columns.Add("SourceClient", typeof(String));
            logs.Columns.Add("TargetClient", typeof(String));
            logs.Columns.Add("EventName", typeof(String));
            logs.Columns.Add("EventDetails", typeof(String));
            lstEventNameFilters.Sorted = true;
            logs.Rows.Clear();
            ProjectTable();
        }

        public void ProjectTable()
        {
            refreshClock.Restart();
            int logCount = logs.Rows.Count;
            DataView view = new DataView(logs);
            view.RowFilter = eventNamesFilter;
            if (logCount > 0)
            {
                int skip = (logCount>(int)numHistory.Value)?(logCount-(int)numHistory.Value):0;
                dgvLog.DataSource = view.ToTable().AsEnumerable().OrderByDescending(log=>log[0]).Skip(skip).CopyToDataTable();
            }
            else
            {
                dgvLog.DataSource = view.ToTable();
            }
        }

        private void dgvLog_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((dgvLog.Columns[e.ColumnIndex].Name.Equals("Time")))
            {
                e.CellStyle.Format = "n5";
            }
            else if ((dgvLog.Columns[e.ColumnIndex].Name.Equals("SourceClient") || dgvLog.Columns[e.ColumnIndex].Name.Equals("TargetClient")) && (e.Value == null || e.Value.ToString() == ""))
            {
                e.CellStyle.BackColor = Color.DimGray;
                e.CellStyle.SelectionBackColor = Color.Navy;
            }
        }

        public void AddLog(LogEntry logEntry)
        {
            lock (logQueue)
            {
                logQueue.Add(logEntry);
            }
            updateLog = true;
        }

        bool updateLog = false;
        bool populatingEventNamesList = false;
        public void SetEventNameFilters(List<string> eventNames)
        {
            foreach (string e in eventNameFilters.Keys)
            {
                if (!eventNames.Contains(e)) eventNameFilters.Remove(e);
            }

            foreach (string e in eventNames)
            {
                if (!eventNameFilters.ContainsKey(e)) eventNameFilters[e] = true;
            }
            populatingEventNamesList = true;
            lstEventNameFilters.SuspendLayout();
            lstEventNameFilters.Items.Clear();
            foreach(KeyValuePair<string, bool> enf in eventNameFilters) {
                lstEventNameFilters.Items.Add(enf.Key, enf.Value);
            }
            lstEventNameFilters.ResumeLayout();
            populatingEventNamesList = false;
        }

        private void lstEventNameFilters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (populatingEventNamesList) return;
            if (eventNameFilters.ContainsKey(lstEventNameFilters.Items[e.Index].ToString()))
            {
                eventNameFilters[lstEventNameFilters.Items[e.Index].ToString()] = (e.NewValue == CheckState.Checked);
                RebuildFilters();
            }
        }

        private void RebuildFilters()
        {
            int count=0;
            foreach (bool b in eventNameFilters.Values) if (b) count++;

            if (count == 0)
            {
                eventNamesFilter = "[EventName] <> ''";
                ProjectTable();
                return;
            }
            eventNamesFilter = "[EventName] <> '' ";

            string op;
            bool compareValue;
            if (count < eventNameFilters.Count / 2) //count AND on true
            {
                op = "=";
                compareValue = true;
            }
            else //count AND on false
            {
                op = "<>";
                compareValue = false;
                count = eventNameFilters.Count - count;
            }
            int i = 0;
            foreach (KeyValuePair<string, bool> enf in eventNameFilters)
            {
                if (enf.Value == compareValue)
                {
                    i++;
                    eventNamesFilter += string.Format("AND [EventName] {0} '{1}' ", op, enf.Key);
                }
            }
            ProjectTable();
        }

        private void btnCollectEventNames_Click(object sender, EventArgs e)
        {
            CollectEventNames();
        }

        public void CollectEventNames()
        {
            List<String> eventNames = new List<string>();
            foreach (string eventName in client.SubscribedEvents)
            {
                if (!eventNames.Contains(eventName)) eventNames.Add(eventName);
            }
            foreach (string eventName in client.EventParameters.Keys)
            {
                if (!eventNames.Contains(eventName)) eventNames.Add(eventName);
            }
            SetEventNameFilters(eventNames);
        }

        private void numHistory_ValueChanged(object sender, EventArgs e)
        {
            ProjectTable();
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            populatingEventNamesList = true;
            for (int i = 0; i < lstEventNameFilters.Items.Count; i++)
            {
                lstEventNameFilters.SetItemChecked(i, true);
            }
            populatingEventNamesList = false;
            RebuildFilters();
        }

        private void btnFilterNone_Click(object sender, EventArgs e)
        {
            populatingEventNamesList = true;
            for (int i = 0; i < lstEventNameFilters.Items.Count; i++)
            {
                lstEventNameFilters.SetItemChecked(i, false);
            }
            populatingEventNamesList = false;
            RebuildFilters();
        }

        private void lstEventNameFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            highlightEventName = lstEventNameFilters.SelectedItem.ToString();
            ProjectTable();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void chkHold_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkHold.Checked) ProjectTable();
        }

        public void SetVisible(bool state)
        {
            if (state)
            {
                Visible = true;
                if (tRefresh != null) tRefresh.Abort();
                tRefresh = new Thread(new ThreadStart(RefreshLog));
                tRefresh.Start();
                CollectEventNames();
            }
            else
            {
                Visible = false;
            }
        }

    }
}
