using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thalamus;

namespace ThalamusLogTool
{
    public partial class frmLogTool : Form
    {

        protected IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;

        public frmLogTool()
        {
            InitializeComponent();
            if (Directory.Exists(Properties.Settings.Default.CurrentDirectory)) CurrentDirectory = Properties.Settings.Default.CurrentDirectory;
            else CurrentDirectory = Directory.GetCurrentDirectory();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool DontRefresh = false;
        string loadedSyncLogFile = "";
        string loadedSyncLogFilename = "";
        string currentDirectory = "";
        public string CurrentDirectory
        {
            get { return currentDirectory; }
            set { 
                currentDirectory = value;
                Properties.Settings.Default.CurrentDirectory = currentDirectory;
                Properties.Settings.Default.Save();
            }
        }


        private void RefreshInfo()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: RefreshInfoSync();
                    break;
                case 1: RefreshInfoClean();
                    break;
                case 2: RefreshInfoConvert();
                    break;
                case 3: RefreshInfoMerge();
                    break;
            }
        }

        private void RefreshInfoSync()
        {
            if (DontRefresh) return;
            DontRefresh = true;
            if (loadedSyncLogFile == "")
            {
                lstSyncEvents.Enabled = false;
                lstSyncTimes.Enabled = false;
                grpSyncExport.Enabled = false;
                grpCut.Enabled = false;
                grpSynchronize.Enabled = false;
            }
            else
            {
                lstSyncEvents.Enabled = true;
                if (lstSyncEvents.SelectedIndex == -1 || !loadedSyncLogSplit.ContainsKey((string)lstSyncEvents.SelectedItem))
                {
                    lstSyncTimes.Enabled = false;
                }
                else
                {
                    lstSyncTimes.Enabled = true;
                    if (lstSyncTimes.SelectedIndex == -1)
                    {
                        grpCut.Enabled = false;
                        grpSyncExport.Enabled = false;
                        grpSynchronize.Enabled = false;
                    }
                    else
                    {
                        grpSyncExport.Enabled = true;
                        grpCut.Enabled = chkTrimEnabled.Checked;
                        grpSynchronize.Enabled = chkSyncEnabled.Checked;
                        btnSyncExport.Enabled = chkSyncEnabled.Checked || chkTrimEnabled.Checked;
                    }
                }
            }
            DontRefresh = false;
        }

        private void RefreshInfoClean()
        {
            if (DontRefresh) return;
            DontRefresh = true;

            btnCleanRemoveSelectedFile.Enabled = lstCleanFiles.SelectedIndex != -1;
            btnCleanLoadLogs.Enabled = lstCleanFiles.Items.Count != 0;
            btnCleanSelectAllMsgs.Enabled = clbCleanMessages.Enabled;
            btnCleanSelectNoneMsgs.Enabled = clbCleanMessages.Enabled;
            grpCleanExport.Enabled = clbCleanMessages.Items.Count != 0;
            chkCleanFilterSent.Enabled = clbCleanMessages.Enabled;
            chkCleanFilterReceived.Enabled = clbCleanMessages.Enabled;

            lstFilterSelectionSets.Items.Clear();
            if (filterSelectionSets != null)
            {
                foreach (string ss in filterSelectionSets.Sets.Keys)
                {
                    lstFilterSelectionSets.Items.Add(ss);
                }
            }

            DontRefresh = false;
        }

        private void RefreshInfoConvert()
        {
            if (DontRefresh) return;
            DontRefresh = true;

            DontRefresh = false;
        }

        private void RefreshInfoMerge()
        {
            if (DontRefresh) return;
            DontRefresh = true;

            DontRefresh = false;
        }

        private void btnSyncOpenLog_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Log Files|*.log|Comma-Separated Values Files|*.csv|All Files|*.*";
            openFileDialog.InitialDirectory = CurrentDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtSyncLogFile.Text = openFileDialog.FileName;
                CurrentDirectory = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

        FilterSelectionSets filterSelectionSets = null;
        private void frmLogTool_Load(object sender, EventArgs e)
        {
            filterSelectionSets = new FilterSelectionSets();
            filterSelectionSets.Reload();
            RefreshInfo();
            //LogTool.LoadThalamusEventLogs("logTest.log");
        }

        Dictionary<string, MethodInfo> loadedAssemblyMessages = new Dictionary<string, MethodInfo>();
        private void LoadAssemblyClassTypes(string assembly)
        {
            loadedAssemblyMessages = LogTool.LoadMessagesFromAssembly(assembly);
            lstConvertToThalamusMessage.Items.Clear();
            foreach (KeyValuePair<string, MethodInfo> msgType in loadedAssemblyMessages) 
            {
                lstConvertToThalamusMessage.Items.Add(msgType.Key);
                /*MethodInfo[] events = msgType.Value.GetMethods();
                foreach (MethodInfo m in events)
                {
                    lstConvertToThalamusMessage.Items.Add(msgType.Key + "." + m.Name);
                }*/
            }
        }

        private void LoadSyncFile()
        {
            if (File.Exists(txtSyncLogFile.Text))
            {
                lstSyncEvents.Items.Clear();
                lstSyncTimes.Items.Clear();
                lblLoading.Visible = true;
                lblLoading.Refresh();
                loadedSyncLogFilename = txtSyncLogFile.Text;
                using (StreamReader file = File.OpenText(txtSyncLogFile.Text))
                {
                    loadedSyncLogFile = file.ReadToEnd();
                }
                loadedSyncLogSplit = LogTool.LoadLogSimpleThalamus(loadedSyncLogFile, chkSyncFilterReceived.Checked, chkSyncFilterSent.Checked);
                /*
                if (radSyncLogTypeThalamus.Checked)
                {
                    loadedSyncLogSplit = LoadLogThalamus(loadedSyncLogFile, chkSyncFilterReceived.Checked, chkSyncFilterSent.Checked);
                }
                else
                {
                    loadedSyncLogSplit = LoadLogCSV(loadedSyncLogFile);
                }
                 * */
                foreach (string s in loadedSyncLogSplit.Keys)
                {
                    lstSyncEvents.Items.Add(s);
                }
                lblLoading.Visible = false;
                RefreshInfoSync();
            }
            else
            {
                MessageBox.Show("File '" + txtSyncLogFile.Text + "' does not exist!", "Load Log File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSyncLoadLog_Click(object sender, EventArgs e)
        {
            LoadSyncFile();
        }

        private Dictionary<string, List<string>> LoadLogCSV(string logFile)
        {
            Dictionary<string, List<string>> logSplit = new Dictionary<string, List<string>>();

            return logSplit;
        }

        Dictionary<string, List<string>> loadedSyncLogSplit = new Dictionary<string, List<string>>();
        

        

        int lstSyncEventsSelected = -1;
        private void lstSyncEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSyncTimes.Items.Clear();
            if (loadedSyncLogSplit.ContainsKey((string)lstSyncEvents.SelectedItem))
            {
                lstSyncEventsSelected = lstSyncTimes.SelectedIndex;
                foreach (string s in loadedSyncLogSplit[(string)lstSyncEvents.SelectedItem])
                {
                    lstSyncTimes.Items.Add(s);
                }
            }
            else
            {
                lstSyncEventsSelected = -1;
            }
            RefreshInfoSync();
        }

        int lstSyncTimesSelected = -1;
        private void lstSyncTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string st = ((string)lstSyncTimes.SelectedItem).Split(':')[0].Replace(',','.');
                double t = double.Parse(st, ifp);
                var subseconds = ((decimal) t % 1)*100;
                numSyncSubseconds.Value = subseconds;// subseconds > 99 ? 99 : subseconds;
                t = Math.Truncate(t);
                int hours = (int) Math.Truncate(t / (60 * 60));
                int mins = (int)Math.Truncate((t - (60 * 60 * hours)) / 60);
                int seconds = (int)t - (60 * 60 * hours) - 60 * mins;
                numSyncTime.Value = new DateTime(2014, 1, 1, hours, mins, seconds, DateTimeKind.Unspecified);
                lstSyncTimesSelected = lstSyncTimes.SelectedIndex;
            }
            catch
            {
                MessageBox.Show("Unable to parse the log timing information! Log is in the incorrect format.", "Synchronize Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lstSyncTimes.Items.Clear();
                lstSyncTimesSelected = -1;
            }
            RefreshInfoSync();
        }

        private void chkResync_CheckedChanged(object sender, EventArgs e)
        {
            grpSynchronize.Enabled = chkSyncEnabled.Checked;
            btnSyncExport.Enabled = chkSyncEnabled.Checked || chkTrimEnabled.Checked;
        }

        private void chkTrimEnabled_CheckedChanged(object sender, EventArgs e)
        {
            grpCut.Enabled = chkTrimEnabled.Checked;
            btnSyncExport.Enabled = chkSyncEnabled.Checked || chkTrimEnabled.Checked;
        }

        private void btnSyncExport_Click(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string[] logs = loadedSyncLogFile.Split('\n');
            string filenameExtras = "";
            if (chkTrimEnabled.Checked && chkSyncEnabled.Checked) filenameExtras = "_SyncAndTrimmed";
            else if (chkTrimEnabled.Checked) filenameExtras = "_Trimmed";
            else if (chkSyncEnabled.Checked) filenameExtras = "_Synced";
            if (chkTrimEnabled.Checked)
            {
                if (radSyncFromStart.Checked) filenameExtras += "_FromStart";
                else if (radSyncToEnd.Checked) filenameExtras += "_ToEnd";
                else if (radSyncToNext.Checked) filenameExtras += "_ToNext";
            }

            string filename = Path.GetDirectoryName(loadedSyncLogFilename) + "\\" + Path.GetFileNameWithoutExtension(loadedSyncLogFilename) + filenameExtras + Path.GetExtension(loadedSyncLogFilename);
            int i = 0;
            while (File.Exists(filename))
            {
                filename = Path.GetDirectoryName(loadedSyncLogFilename) + "\\" + Path.GetFileNameWithoutExtension(loadedSyncLogFilename) + filenameExtras + "_" + (i++ + 1).ToString() + Path.GetExtension(loadedSyncLogFilename);
            }
            if (!chkSyncAutoName.Checked)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "Log Files|*.log|All Files|*.*";
                /*
                if (radSyncLogTypeThalamus.Checked) saveFileDialog.Filter = "Log Files|*.log|All Files|*.*";
                else saveFileDialog.Filter = "Comma-Separated Values Files|*.csv|All Files|*.*";
                 * */
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(loadedSyncLogFilename);
                saveFileDialog.FileName = filename;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filename = saveFileDialog.FileName;
                    CurrentDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                }
            }

            using (StreamWriter file = File.CreateText(filename))
            {

                foreach (string s in logs)
                {
                    if (s.Trim().Length > 0 && Regex.IsMatch(s, LogTool.ThalamusLogRegexValidator))
                    {
                        string log = s.Replace('\r', ' ');

                        string[] split = log.Split(':');

                        string currentSt = split[3].Trim();
                        double currentT = double.Parse(currentSt.Replace(',', '.'));

                        string syncBaseSt = ((string)lstSyncTimes.SelectedItem).Split(':')[0];
                        double syncBaseT = double.Parse(syncBaseSt.Replace(',', '.'));
                        double syncNextBaseT = 0;
                        if (radSyncToNext.Checked && lstSyncTimes.Items.Count >= lstSyncTimes.SelectedIndex + 2)
                        {
                            string syncNextBaseSt = ((string)lstSyncTimes.Items[lstSyncTimes.SelectedIndex + 1]).Split(':')[0];
                            syncNextBaseT = double.Parse(syncNextBaseSt.Replace(',', '.'));
                        }

                        double syncFinalT = 3600 * numSyncTime.Value.Hour + 60 * numSyncTime.Value.Minute + numSyncTime.Value.Second + ((double)numSyncSubseconds.Value) / 100.0f;
                        double offset = syncFinalT - syncBaseT;

                        double resyncedT = currentT + offset;
                        if (!chkSyncEnabled.Checked) resyncedT = currentT;
                        bool write = false;
                        if (!chkTrimEnabled.Checked)
                        {
                            write = true;
                        }
                        else
                        {
                            if (radSyncFromStart.Checked && currentT <= syncBaseT) write = true;
                            else if (radSyncToEnd.Checked && currentT >= syncBaseT) write = true;
                            else if (radSyncToNext.Checked && ((!chkSyncToNextInclusive.Checked && currentT >= syncBaseT && currentT < syncNextBaseT) || (chkSyncToNextInclusive.Checked && currentT >= syncBaseT && currentT <= syncNextBaseT))) write = true;
                        }

                        DateTime timestamp = new DateTime(1000,1,1,0,0,0);
                        timestamp = timestamp.AddSeconds(resyncedT);
                        if (write) file.WriteLine(String.Format("({0}){1}: {2}:{3}", timestamp.ToString("HH:mm:ss.fffffff"), split[2].Substring(split[2].IndexOf(')') + 1), resyncedT.ToString(ifp), log.Substring(LogTool.IndexOfNth(log, ':', 4) + 1)));
                    }
                }
                MessageBox.Show("Exported log file to '" + filename + "'.", "Export Log File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void radSyncToNext_CheckedChanged(object sender, EventArgs e)
        {
            chkSyncToNextInclusive.Enabled = radSyncToNext.Checked;
        }

        private void txtSyncLogFile_TextChanged(object sender, EventArgs e)
        {
            btnSyncLoadLog.Enabled = File.Exists(txtSyncLogFile.Text);
        }

        private void btnCleanOpenLogs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Log Files|*.log|Comma-Separated Values Files|*.csv|All Files|*.*";
            openFileDialog.InitialDirectory = CurrentDirectory;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                foreach (string f in openFileDialog.FileNames)
                {
                    lstCleanFiles.Items.Add(f);
                }
            }
            RefreshInfoClean();
        }

        private void btnCleanClearFiles_Click(object sender, EventArgs e)
        {
            lstCleanFiles.Items.Clear();
            clbCleanMessages.Items.Clear();
            clbCleanMessages.Enabled = false;
            RefreshInfoClean();
        }

        private void LoadCleanFiles()
        {
            clbCleanMessages.Items.Clear();
            lblLoading.Visible = true;
            lblLoading.Refresh();
            List<string> invalidFilesMsgs = new List<string>();
            List<string> invalidFiles = new List<string>();
            foreach (string f in lstCleanFiles.Items)
            {
                if (File.Exists(f))
                {
                    string logFile = "";
                    using (StreamReader file = File.OpenText(f))
                    {
                        logFile = file.ReadToEnd();
                    }
                    Dictionary<string, List<string>> logSplit = new Dictionary<string, List<string>>();
                    try
                    {
                        logSplit = LogTool.LoadLogSimpleThalamus(logFile, chkCleanFilterReceived.Checked, chkCleanFilterSent.Checked);
                        /*
                        if (radCleanLogTypeThalamus.Checked)
                        {
                            logSplit = LoadLogThalamus(logFile, chkCleanFilterReceived.Checked, chkCleanFilterSent.Checked);
                        }
                        else
                        {
                            logSplit = LoadLogCSV(logFile);
                        }
                         * */
                    }
                    catch
                    {
                        invalidFilesMsgs.Add("Invalid format: '" + f + "'");
                        invalidFiles.Add(f);
                    }

                    foreach (string s in logSplit.Keys)
                    {
                        if (!clbCleanMessages.Items.Contains(s)) clbCleanMessages.Items.Add(s, true);
                    }
                    clbCleanMessages.Enabled = clbCleanMessages.Items.Count > 0;
                    RefreshInfoClean();
                }
                else
                {
                    invalidFilesMsgs.Add("Inexistent: '" + f + "'");
                    invalidFiles.Add(f);
                }
            }
            foreach (string s in invalidFiles)
            {
                lstCleanFiles.Items.Remove(s);
            }

            if (invalidFilesMsgs.Count > 0)
            {
                string str = "The following files were not loaded:\n";
                foreach (string s in invalidFilesMsgs)
                {
                    str += "'" + s + "'\n";
                }
                MessageBox.Show(str, "Load Log Files", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            lblLoading.Visible = false;
            RefreshInfoClean();
        }




        private void btnCleanLoadLogs_Click(object sender, EventArgs e)
        {
            LoadCleanFiles();
        }

        private void btnCleanRemoveSelectedFile_Click(object sender, EventArgs e)
        {
            if (lstCleanFiles.SelectedIndex != -1)
            {
                lstCleanFiles.Items.RemoveAt(lstCleanFiles.SelectedIndex);
            }
        }

        private void lstCleanFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCleanRemoveSelectedFile.Enabled = lstCleanFiles.SelectedIndex != -1;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void btnCleanSelectNoneMsgs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbCleanMessages.Items.Count; i++)
            {
                clbCleanMessages.SetItemChecked(i, false);
            }
        }

        private void btnCleanSelectAllMsgs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbCleanMessages.Items.Count; i++)
            {
                clbCleanMessages.SetItemChecked(i, true);
            }
        }

        private void btnSyncChangeFilter_Click(object sender, EventArgs e)
        {
            LoadSyncFile();
        }

        private void btnCleanChangeFilter_Click(object sender, EventArgs e)
        {
            LoadCleanFiles();
        }

        private void btnCleanExport_Click(object sender, EventArgs e)
        {
            int exported = 0;
            List<string> failedMsgs = new List<string>();
            foreach (string f in lstCleanFiles.Items)
            {
                try
                {
                    string logFile = "";
                    using (StreamReader file = File.OpenText(f))
                    {
                        logFile = file.ReadToEnd();
                    }

                    string[] logs = logFile.Split('\n');
                    string filenameExtras = "_Filtered";

                    string filename = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + filenameExtras + Path.GetExtension(f);
                    int i = 0;
                    while (File.Exists(filename))
                    {
                        filename = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + filenameExtras + "_" + (i++ + 1).ToString() + Path.GetExtension(f);
                    }
                    if (!chkSyncAutoName.Checked)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();

                        saveFileDialog.Filter = "Log Files|*.log|All Files|*.*";
                        /*
                        if (radCleanLogTypeThalamus.Checked) saveFileDialog.Filter = "Log Files|*.log|All Files|*.*";
                        else saveFileDialog.Filter = "Comma-Separated Values Files|*.csv|All Files|*.*";
                         * */
                        saveFileDialog.InitialDirectory = Path.GetDirectoryName(f);
                        saveFileDialog.FileName = filename;
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filename = saveFileDialog.FileName;
                            CurrentDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        }
                    }

                    List<string> messagesIndexer = new List<string>();
                    for (i = 0; i < clbCleanMessages.Items.Count; i++)
                    {
                        if (clbCleanMessages.GetItemChecked(i)) messagesIndexer.Add((string)clbCleanMessages.Items[i]);
                    }

                    Dictionary<string, string> repeatedMessagesFilter = new Dictionary<string, string>();

                    using (StreamWriter file = File.CreateText(filename))
                    {
                        foreach (string s in logs)
                        {
                            if (s.Trim().Length > 0 && Regex.IsMatch(s, LogTool.ThalamusLogRegexValidator))
                            {
                                string log = s.Replace('\r', ' ');
                                string[] split = log.Split(':');
                                bool write = false;
                                string value = log.Substring(LogTool.IndexOfNth(log, ':', 8));
                                if (!chkCleanRemoveDuplicates.Checked || !repeatedMessagesFilter.ContainsKey(split[6]) || (chkCleanRemoveDuplicates.Checked && repeatedMessagesFilter.ContainsKey(split[6]) && repeatedMessagesFilter[split[6]] != value))
                                {
                                    repeatedMessagesFilter[split[6]] = value;
                                    if (chkCleanFilterReceived.Checked && messagesIndexer.Contains("Received<-" + split[6]) && split[4].Length == 0) write = true;
                                    else if (chkCleanFilterSent.Checked && messagesIndexer.Contains("Sent->" + split[6]) && split[5].Length == 0) write = true;
                                    else if (!chkCleanFilterReceived.Checked && !chkCleanFilterSent.Checked && messagesIndexer.Contains(split[6])) write = true;
                                    if (write) file.WriteLine(log);
                                }
                            }
                        }
                    }
                    exported++;
                }
                catch
                {
                    failedMsgs.Add(f);
                }
            }
            if (lstCleanFiles.Items.Count == exported) MessageBox.Show("Sucessfully exported '" + exported + "' files.", "Export Log File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                string str = "Exported '" + exported + "' files.\n\nThe following files failed to export:";
                foreach (string s in failedMsgs)
                {
                    str += s + "\n";
                }
                MessageBox.Show(str, "Export Log File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnConvertAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Log Files|*.log|Comma-Separated Values Files|*.csv|Text Files|*.txt|All Files|*.*";
            openFileDialog.InitialDirectory = CurrentDirectory;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                foreach (string f in openFileDialog.FileNames)
                {
                    lstConvertFiles.Items.Add(f);
                }
            }
            RefreshInfoConvert();
        }

        private void btnConvertRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstConvertFiles.SelectedIndex != -1)
            {
                lstConvertFiles.Items.RemoveAt(lstConvertFiles.SelectedIndex);
            }
        }

        private void btnConvertClearFiles_Click(object sender, EventArgs e)
        {
            lstConvertFiles.Items.Clear();
            /*clbCleanMessages.Items.Clear();
            clbCleanMessages.Enabled = false;*/
            RefreshInfoConvert();
        }

  
        private void chkCleanAutoName_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstConvertFiles.Items.Count == 0) return;
            int exported = 0;
            List<string> failedMsgs = new List<string>();
            if (radConvertThalamusToCsv.Checked || radConvertThalamusToSrt.Checked)
            {
                string format = "CSV";
                foreach (string f in lstConvertFiles.Items)
                {
                    try
                    {
                        string logFile = "";
                        using (StreamReader file = File.OpenText(f))
                        {
                            logFile = file.ReadToEnd();
                        }

                        string[] logs = logFile.Split('\n');
                        int i = 0;
                        string path;
                        if (radConvertThalamusToCsv.Checked)
                        {
                            format = "CSV";
                            path = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + "_CSV\\";
                            while (Directory.Exists(path))
                            {
                                path = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + "_CSV" + "_" + (i++ + 1).ToString() + "\\";
                            }
                            if (!chkSyncAutoName.Checked)
                            {
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                fbd.SelectedPath = Path.GetDirectoryName(f);
                                fbd.Description = "Save CSV files to...";
                                DialogResult dr = fbd.ShowDialog();
                                if (dr == DialogResult.OK)
                                {
                                    CurrentDirectory = fbd.SelectedPath;
                                    path = CurrentDirectory + "\\";
                                }
                                else if (dr == DialogResult.Cancel)
                                {
                                    if (MessageBox.Show("Continue converting the remaining files?", "Convert Log Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) continue;
                                    else return;
                                }
                            }
                            LogTool.ConvertThalamusToCsv(path, logs);
                        }
                        else
                        {
                            format = "SRT";
                            path = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + ".srt";
                            i = 0;
                            while (File.Exists(path))
                            {
                                path = Path.GetDirectoryName(f) + "\\" + Path.GetFileNameWithoutExtension(f) + "_" + (i++ +1).ToString() + ".srt";
                            }
                            if (!chkSyncAutoName.Checked)
                            {
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.Filter = "Srt Files|All Files|*.*";
                                saveFileDialog.InitialDirectory = Path.GetDirectoryName(f);
                                saveFileDialog.FileName = f;
                                DialogResult dr = saveFileDialog.ShowDialog();
                                if (dr == DialogResult.OK)
                                {
                                    path = saveFileDialog.FileName;
                                    CurrentDirectory = Path.GetDirectoryName(path);
                                }
                                else if (dr == DialogResult.Cancel)
                                {
                                    if (MessageBox.Show("Continue converting the remaining files?", "Convert Log Files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) continue;
                                    else return;
                                }
                            }
                            LogTool.ConvertThalamusToSrt(path, logs, (int) numConvertThalamusToSrtMaxLineLength.Value);
                        }
                        exported++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to convert '" + f + "': " + ex.ToString());
                        failedMsgs.Add(f);
                    }
                }
                if (lstConvertFiles.Items.Count == exported) MessageBox.Show("Sucessfully converted '" + exported + "' Thalamus Logs to " + format + " files.", "Convert Log Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    string str = "Converted '" + exported + "' Thalamus Logs to " + format + " files.\n\nThe following Logs failed to convert:";
                    foreach (string s in failedMsgs)
                    {
                        str += s + "\n";
                    }
                    MessageBox.Show(str, "Convert Log Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (radConvertCsvToThalamus.Checked || radConvertCsvToSrt.Checked)
            {
                if (lstConvertToThalamusMessage.SelectedIndex == -1 || !LogTool.LoadedMessageTypes.ContainsKey(((string)lstConvertToThalamusMessage.SelectedItem).Split('.')[0]) || !loadedAssemblyMessages.ContainsKey((string)lstConvertToThalamusMessage.SelectedItem))
                {
                    MessageBox.Show("A valid Thalamus Message type bust be selected in order to convert from CSV!", "Convert log from CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else {
                    int converted = 0;
                    foreach (string filename in lstConvertFiles.Items) {
                        try
                        {
                            string newFilename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileName(Path.GetFileNameWithoutExtension(filename)) + "_MasterLog.log";
                            int i = 0;
                            while (File.Exists(newFilename))
                            {
                                newFilename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileName(Path.GetFileNameWithoutExtension(filename)) + "_MasterLog_" + (i++ + 1).ToString() + ".log";
                            }
                            if (!chkSyncAutoName.Checked)
                            {
                                SaveFileDialog saveFileDialog = new SaveFileDialog();
                                saveFileDialog.Filter = "Log Files|*.log|All Files|*.*";
                                saveFileDialog.InitialDirectory = Path.GetDirectoryName(filename);
                                saveFileDialog.FileName = filename;
                                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                {
                                    newFilename = saveFileDialog.FileName;
                                    CurrentDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                                }
                            }
                            List<string> csvFiles = new List<string>();
                            foreach (string s in lstConvertFiles.Items) csvFiles.Add(s);

                            var logEntries = LogTool.ConvertCsvToThalamus((string)lstConvertToThalamusMessage.SelectedItem, filename, loadedAssemblyMessages[(string)lstConvertToThalamusMessage.SelectedItem]);
                            if (logEntries.Count > 0)
                            {
                                using (StreamWriter file = File.CreateText(newFilename))
                                {
                                    foreach (LogEntry l in logEntries)
                                    {
                                        TimeSpan t = TimeSpan.FromSeconds(l.Time);
                                        string formatedtext = String.Format("(" + t + ") LoadedFromCSV[log]: " + l.ToString());
                                        file.WriteLine(formatedtext);
                                    }
                                }
                                converted++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to convert CSV files to Thalamus Log:\n" + ex.ToString(), "Convert Log Files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    MessageBox.Show("Converted " + converted + " CSV files to Thalamus Log.", "Convert CSV to Thalamus", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }                    
        }

        List<Dictionary<string, string>> selectionSets = new List<Dictionary<string, string>>();


        

        private void lstConvertFiles_DoubleClick(object sender, EventArgs e)
        {
            btnConvertAddFiles_Click(sender, e);
        }

        private void lstCleanFiles_DoubleClick(object sender, EventArgs e)
        {
            btnCleanOpenLogs_Click(sender, e);
        }

        private void btnFilterSSAdd_Click(object sender, EventArgs e)
        {
            if (txtFilterSS.Text != "")
            {
                List<string> selectedMessages = new List<string>();
                for (int i = 0; i < clbCleanMessages.Items.Count; i++)
                {
                    if (clbCleanMessages.GetItemChecked(i)) selectedMessages.Add(clbCleanMessages.Items[i].ToString());
                }
                filterSelectionSets.Sets[txtFilterSS.Text] = selectedMessages;
                filterSelectionSets.Save();
                RefreshInfoClean();
                btnCleanFilterSSDelete.Enabled = true;
            }
        }

        private void lstFilterSelectionSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterSS.Text = lstFilterSelectionSets.SelectedItem==null?"":lstFilterSelectionSets.SelectedItem.ToString();
            btnCleanFilterSSDelete.Enabled = true;
        }

        private void txtFilterSS_TextChanged(object sender, EventArgs e)
        {
            btnCleanFilterSSDelete.Enabled = lstFilterSelectionSets.Items.Contains(txtFilterSS.Text);
        }

        private void btnCleanFilterSSDelete_Click(object sender, EventArgs e)
        {
            if (filterSelectionSets.Sets.ContainsKey(txtFilterSS.Text))
            {
                filterSelectionSets.Sets.Remove(txtFilterSS.Text);
                filterSelectionSets.Save();
                RefreshInfoClean();
            }
        }

        private void lstFilterSelectionSets_DoubleClick(object sender, EventArgs e)
        {
            txtFilterSS.Text = lstFilterSelectionSets.SelectedItem == null ? "" : lstFilterSelectionSets.SelectedItem.ToString();
            if (filterSelectionSets!=null && filterSelectionSets.Sets.ContainsKey((string)lstFilterSelectionSets.SelectedItem)) {
                btnCleanFilterSSDelete.Enabled = true;
                List<string> ss = filterSelectionSets.Sets[txtFilterSS.Text];
                for (int i = 0; i < clbCleanMessages.Items.Count; i++)
                {
                    if (ss.Contains(clbCleanMessages.Items[i])) clbCleanMessages.SetItemChecked(i, true);
                }
            }
        }

        private void radConvertCsvToThalamus_CheckedChanged(object sender, EventArgs e)
        {
            grpConvertToThalamus.Enabled = radConvertCsvToThalamus.Checked;
        }

        private void btnConvertCSVToThalamusOpenAssembly_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dotNet Assemblies Files|*.dll;*.exe|All Files|*.*";
            openFileDialog.InitialDirectory = CurrentDirectory;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtConvertCSVToThalamusOpenAssembly.Text = openFileDialog.FileName;
                CurrentDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                LoadAssemblyClassTypes(txtConvertCSVToThalamusOpenAssembly.Text);
            }
        }

        private void radConvertThalamusToCsv_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
