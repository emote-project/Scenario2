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
using Thalamus;

namespace ThalamusLogTool
{
    public partial class frmLog : Form
    {

        private Dictionary<string, Boolean> eventNameFilters = new Dictionary<string, bool>();
        private String eventNamesFilter = "";
        private String highlightEventName = "";

        private ThalamusClient logClient = null;
        public ThalamusClient LogClient
        {
            get { return logClient; }
            set { 
                logClient = value;
                if (logClient == null) this.Text = "Thalamus Event Log";
                else this.Text = "Thalamus Event Log - " + logClient.Name;
            }
        }

        private List<LogEntry> logQueue = new List<LogEntry>();

        Thread tRefresh = null;
        Stopwatch refreshClock = new Stopwatch();
        float refreshRate = 6f;

        protected DataTable logs = new DataTable();
        public DataTable Logs
        {
            get { return logs; }
        }

        public frmLog()
        {
            InitializeComponent();

            logs.Columns.Add("Time", typeof(Double));
            logs.Columns.Add("CharacterName", typeof(String));
            logs.Columns.Add("SourceClient", typeof(String));
            logs.Columns.Add("TargetClient", typeof(String));
            logs.Columns.Add("EventName", typeof(String));
            logs.Columns.Add("EventDetails", typeof(String));

            dgvLog.CellFormatting +=
            new System.Windows.Forms.DataGridViewCellFormattingEventHandler(
            this.dgvLog_CellFormatting);
            refreshClock.Start();

            lstEventNameFilters.Sorted = true;
        }


        private void RefreshLog()
        {
            while (this.Visible)
            {
                if (!chkHold.Checked && updateLog)
                {
                    updateLog = false;
                    List<LogEntry> newLogs;
                    lock (logQueue)
                    {
                        newLogs = new List<LogEntry>(logQueue);
                        logQueue.Clear();
                    }
                    foreach (LogEntry log in newLogs)
                    {
                        if (logs.Rows.Count > numHistory.Value*100)
                        {
                            logs.Rows.RemoveAt(0);
                        }
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
            DataView view = new DataView(logs);
            view.RowFilter = eventNamesFilter;
            int logCount = view.Count;
            if (logCount > 0)
            {
                int skip = (logCount>(int)numHistory.Value)?(logCount-(int)numHistory.Value):0;
                dgvLog.DataSource = view.ToTable().AsEnumerable().Skip(skip).OrderByDescending(log => log[0]).CopyToDataTable();
            }
            else
            {
                dgvLog.DataSource = view.ToTable();
            }
            //dgvLog.Sort(dgvLog.Columns["Time"], ListSortDirection.Descending);
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
            lock (eventNameFilters)
            {
                Dictionary<string, Boolean> newEventNameFilters = new Dictionary<string, bool>(eventNameFilters);
                foreach (string e in eventNameFilters.Keys)
                {
                    if (!eventNames.Contains(e)) newEventNameFilters.Remove(e);
                }
                eventNameFilters = newEventNameFilters;
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
                eventNamesFilter = "[EventName] = ''";
                ProjectTable();
                return;
            }
            else if (count == lstEventNameFilters.Items.Count)
            {
                eventNamesFilter = "[EventName] <> ''";
                ProjectTable();
                return;
            }

            count = 0;
            eventNamesFilter = "EventName In (";
            foreach (KeyValuePair<string, bool> enf in eventNameFilters)
            {
                if (enf.Value)
                {
                    eventNamesFilter += "'" + enf.Key + "', ";
                    count++;
                }
            }
            if (count > 0) eventNamesFilter = eventNamesFilter.Substring(0, eventNamesFilter.Length - 2);
            eventNamesFilter += ")";

            ProjectTable();
        }

        private void btnCollectEventNames_Click(object sender, EventArgs e)
        {
            CollectEventNames();
        }

        public List<String> CollectEventNames(Dictionary<string, PML> evInfo) 
        {
            List<String> eventNames = new List<string>();
            foreach (string eventName in evInfo.Keys)
            {
                if (!eventNames.Contains(eventName)) eventNames.Add(eventName);
            }
            return eventNames;
        }
        public void CollectEventNames()
        {
            List<String> eventNames = new List<string>();
            if (LogClient == null)
            {
                foreach (Character c in Thalamus.Environment.Instance.Characters.Values)
                {
                    lock (c.Clients.EventInfo)
                    {
                        eventNames.AddRange(CollectEventNames(c.Clients.EventInfo));
                    }
                }
            }
            else
            {
                eventNames = CollectEventNames(LogClient.AllEventsInfo);
            }
            SetEventNameFilters(eventNames);
        }

        private void numHistory_ValueChanged(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)(() => ProjectTable()));
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
                if (eventNameFilters.ContainsKey(lstEventNameFilters.Items[i].ToString()))
                {
                    eventNameFilters[lstEventNameFilters.Items[i].ToString()] = true;
                }
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
                if (eventNameFilters.ContainsKey(lstEventNameFilters.Items[i].ToString()))
                {
                    eventNameFilters[lstEventNameFilters.Items[i].ToString()] = false;
                }
            }
            populatingEventNamesList = false;
            RebuildFilters();
        }

        private void lstEventNameFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            highlightEventName = lstEventNameFilters.SelectedItem.ToString();
            this.Invoke((MethodInvoker)(() => ProjectTable()));
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void chkHold_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkHold.Checked)
            {
                this.Invoke((MethodInvoker)(() => ProjectTable()));
                chkHold.BackColor = SystemColors.Control;
            }
            else
            {
                chkHold.BackColor = System.Drawing.Color.Red;
            }
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
                dgvLog.ClearSelection();
            }
            else
            {
                Visible = false;
            }
        }

        private void dgvLog_MouseClick(object sender, MouseEventArgs e)
        {
            chkHold.Checked = true;
        }

        private void dgvLog_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCellCollection cells = dgvLog.Rows[e.RowIndex].Cells;
            string txt = String.Format("{0}\n\n{1} {2}\n", cells[4].Value, ((string)cells[2].Value)==""?">To":"<From", cells[2].Value + "" + cells[3].Value);
            string[] split = (cells[5].Value as string).Split('[');
            split = split[1].Substring(0, split[1].Length-1).Split(';');
            if (split.Length > 0 && split[0].Length > 0)
            {
                foreach (string s in split)
                {
                    if (s.Trim().Length>0) txt += "\n# " + s.Replace("=", " = ");
                }
            }
            MessageBox.Show(txt, "Analyse Event Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
