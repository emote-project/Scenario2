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
using System.Collections;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Thalamus
{
    public delegate void EventLoggedHandler(LogEntry logEntry);
    public delegate void ErrorHandler(string message);

    public abstract class Manager
    {

        public event EventLoggedHandler EventLogged;
        public void NotifyEventLogged(LogEntry logEntry)
        {
            if (EventLogged != null) EventLogged(logEntry);
        }

        public event ErrorHandler Error;
        public void NotifyError(string message)
        {
            if (Error != null) Error(message);
        }

        public bool debugState = true;
        private bool debugIfs = true;
        private Hashtable debugs = new Hashtable();
        protected static IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;
        protected string managerName = "EmptyManager";
        protected bool isShuttingdown = false;
        private Thread tSaveLog;
        private bool logCreated = false;
        private string logFilenameTimestamp = "";
        private string csvLogFilenameTimestamp = "";

        private Stopwatch startTime = Stopwatch.StartNew();
        public Stopwatch StartTime
        {
            get { return startTime; }
        }

        private TimeSpan centralTimeShift = TimeSpan.Zero;
        public void SetCentralTimeShift(long miliseconds)
        {
            centralTimeShift = TimeSpan.FromMilliseconds(miliseconds);
        }

        protected BindingList<LogEntry> csvLogs = new BindingList<LogEntry>();

        protected BindingList<LogEntry> logs = new BindingList<LogEntry>();
        public BindingList<LogEntry> Logs
        {
            get { return logs; }
        }

        public string ManagerName
        {
            get { return managerName; }
        }
        public Manager(string myManager)
        {
            managerName = myManager;
            if (Properties.Settings.Default.UniqueLogFile)
            {
                logFile = Manager.CorrectPath(Properties.Settings.Default.LogDirectory + ManagerName + ".log");
                logFilenameTimestamp = "";
            }
            else
            {
                logFilenameTimestamp = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "at" + DateTime.Now.Hour.ToString().PadLeft(2) + "-" + DateTime.Now.Minute.ToString().PadLeft(2) + "-" + DateTime.Now.Second.ToString().PadLeft(2);
                logFile = Properties.Settings.Default.LogDirectory + ManagerName + "." + logFilenameTimestamp + ".log";
            }
            setDebug("log", true);
            setDebug("error", true);
            tSaveLog = new Thread(new ThreadStart(SaveLogThread));
            tSaveLog.Start();
        }

        public virtual bool Setup()
        {
            DebugIf("all", "Setup.");
            return true;
        }

        public virtual bool Start()
        {
            DebugIf("all", "Started");
            return true;
        }

        public virtual void Dispose()
        {
            isShuttingdown = true;
            if (tSaveLog != null)
            {
                tSaveLog.Abort();
                tSaveLog.Join();
            }
            DumpLogFile(true, true);
        }

        private void SaveLogThread()
        {
            while (!isShuttingdown)
            {
                Thread.Sleep(2000);
                DumpLogFile(true, false);
            }
        }
		
		public static bool RunningWindows() {
			int p = (int) System.Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128)) {
            	return false;
            } else {
            	return true;
            }
		}
		
		public static string CorrectPath(string path) {
			if (RunningWindows()) return path;
			else return path.Replace("\\","/");
		}

        public void setDebug()
        {
            setDebug(true);
        }
        public void setDebug(bool debugstate)
        {
            debugState = debugstate;
        }
        public void setDebugIfs()
        {
            setDebugIfs(true);
        }
        public void setDebugIfs(bool debugState)
        {
            debugIfs = debugState;
        }
        public void setDebug(object debugIf)
        {
            setDebug(debugIf,true);
        }
        public void setDebug(object debugObj, bool debugState)
        {
            if (debugs.ContainsKey(debugObj)) debugs[debugObj] = debugState;
            else debugs.Add(debugObj, debugState);
        }

        public void LogEvent(bool outwards, Character character, ThalamusClientProxy client, PML ev)
        {
            LogEntry l = new LogEntry(outwards, CentralTimePassed().TotalSeconds, character.Name, client == null ? "GUI" : client.Name, ev);
			DebugLog(l.ToString());
            NotifyEventLogged(l);
            logs.Add(l);
            if (character.LogToCSV) lock (csvLogs) { csvLogs.Add(l); }
        }

        private void LogCSV(Character character, PML ev)
        {
        }

        public void LogEvent(bool outwards, PML ev)
        {
            LogEntry l = new LogEntry(outwards, CentralTimePassed().TotalSeconds, ev);
            DebugLog(l.ToString());
            NotifyEventLogged(l);
            logs.Add(l);
        }

        public void Debug(string text, params object[] args)
        {
            DebugIf("log", text, args);
        }

        public void DebugError(string text, params object[] args)
        {
            NotifyError(text);
            DebugIf("error", text, args);
        }
        public void DebugException(Exception e, int location = -1)
        {
            string msg = string.Format("Exception in {0}{3}: {1}{2}.", new StackFrame(1).GetMethod().Name, e.Message, (e.InnerException == null ? "" : ": " + e.InnerException), (location == -1 ? "" : "(" + location + ")"));
            NotifyError(msg);
            DebugIf("error", msg);
        }

        public void DebugLog(string text, params object[] args)
        {
            DebugIf("log", false, text, args);
        }

        private void Debug(bool showInConsole, string text, params Object[] args)
        {
            DebugIf("", showInConsole, text, args);
        }

        
        /*public static void GlobalDebugIf(object debugIf, string text, params Object[] args)
        {
            bool debugSource = false;
            if (debugIf == ((object)"") && globalDebugState)
            {
                debugSource = true;
            }
            else if (debugIf != ((object)"") && globalDebugIfs)
            {
                if (globalDebugs.ContainsKey(debugIf))
                {
                    debugSource = (bool)globalDebugs[debugIf];
                }
            }
            //if this source is active for this debugIf, or if this debugIf is globally active for debug, then go on..
            if (debugSource)
            {
                string formatedtext;
                string debugIfText = (debugIf == ((object)"")) ? "" : "[" + (string)debugIf + "]";
                if (args != null) formatedtext = String.Format("(" + Environment.Instance.CentralTimePassed() + ") GlobalManager" + debugIfText + ": " + text, args);
                else formatedtext = String.Format("(" + Environment.Instance.CentralTimePassed() + ") GlobalManager" + debugIfText + ": " + text);
                Console.WriteLine(formatedtext);
                Environment.Instance.Log(formatedtext);
            } 
        }*/

        public void DebugIf(object debugIf, string text, params Object[] args)
        {
            DebugIf(debugIf, true, text, args);
        }

        private void DebugIf(object debugIf, bool showInConsole, string text, params Object[] args)
        {
            bool debugSource=false;
            if (debugIf == ((object)"") && debugState)
            {
                debugSource = true;
            }
            else if (debugIf != ((object)"") && debugIfs)
            {
                if (debugs.ContainsKey(debugIf))
                {
                    debugSource = (bool)debugs[debugIf];
                }
            }
            //if this source is active for this debugIf, or if this debugIf is globally active for debug, then go on..
            if (debugSource)
            {
                string formatedtext;
                string debugIfText = (debugIf == ((object)"")) ? "" : "[" + (string)debugIf + "]";
                if (args != null) formatedtext = String.Format("(" + CentralTimePassed() + ") " + managerName + debugIfText + ": " + text, args);
                else formatedtext = String.Format("(" + CentralTimePassed() + ") " + managerName + debugIfText + ": " + text);
                if (showInConsole) Console.WriteLine(formatedtext);
                Log(formatedtext);
            } 
        }
        string logFile;
        Dictionary<string, LogEntry> lastLogs = new Dictionary<string, LogEntry>();

        public void ResetCSV(Character c)
        {
            if (Properties.Settings.Default.UniqueLogFile)
            {
                csvLogFilenameTimestamp = "";
            }
            else
            {
                csvLogFilenameTimestamp = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "at" + DateTime.Now.Hour.ToString().PadLeft(2) + "-" + DateTime.Now.Minute.ToString().PadLeft(2) + "-" + DateTime.Now.Second.ToString().PadLeft(2);
            }

            string csvPath = Properties.Settings.Default.LogDirectory + "CSV_" + c.Name + (Properties.Settings.Default.UniqueLogFile ? "" : "_" + csvLogFilenameTimestamp);

            if (Directory.Exists(Manager.CorrectPath(csvPath)) && Properties.Settings.Default.UniqueLogFile)
            {
                Directory.Delete(csvPath, true);
            }
            if (!Directory.Exists(Manager.CorrectPath(csvPath))) Directory.CreateDirectory(Manager.CorrectPath(csvPath));
        }

        public void DumpLogFile(bool increment = false, bool showInfo = false)
        {

            if (!Directory.Exists(Manager.CorrectPath(Properties.Settings.Default.LogDirectory))) Directory.CreateDirectory(Manager.CorrectPath(Properties.Settings.Default.LogDirectory));

            if (showInfo) Debug("Dumped log to '" + logFile + "'");
            
            lock (HistoryLog)
            {
                
                TextWriter tw = new StreamWriter(logFile, increment && logCreated);
                foreach (string line in HistoryLog)
                {
                    tw.WriteLine(line);
                }
                HistoryLog.Clear();
                tw.Close();
                logCreated = true;
            }
            if (csvLogs.Count > 0)
            {
                List<LogEntry> csv;
                lock (csvLogs)
                {
                    csv = new List<LogEntry>(csvLogs);
                    csvLogs.Clear();
                }
                Dictionary<string, Dictionary<string, TextWriter>> characterLogsFiles = new Dictionary<string, Dictionary<string, TextWriter>>();

                foreach(LogEntry log in csv) 
                {
                    if (!characterLogsFiles.ContainsKey(log.CharacterName))
                    {
                        characterLogsFiles[log.CharacterName] = new Dictionary<string, TextWriter>();
                    }

                    string csvPath = Properties.Settings.Default.LogDirectory + "CSV_" + log.CharacterName + (Properties.Settings.Default.UniqueLogFile ? "" : "_" + csvLogFilenameTimestamp);

                    if (Directory.Exists(Manager.CorrectPath(csvPath)) && Properties.Settings.Default.UniqueLogFile)
                    {
                        Directory.Delete(csvPath, true);
                    }
                    if (!Directory.Exists(Manager.CorrectPath(csvPath))) Directory.CreateDirectory(Manager.CorrectPath(csvPath));


                    if (!characterLogsFiles[log.CharacterName].ContainsKey(log.EventName))
                    {
                        characterLogsFiles[log.CharacterName][log.EventName] = new StreamWriter(Manager.CorrectPath(csvPath + "\\" + log.EventName + ".csv"), true);
                        if (!lastLogs.ContainsKey(log.CharacterName + "." + log.EventName))
                        {
                            lastLogs[log.CharacterName + "." + log.EventName] = null;
                            characterLogsFiles[log.CharacterName][log.EventName].WriteLine(log.ToCSVHeader());
                        }
                    }
                }
                
                LogEntry lastLog = null;
                foreach (LogEntry log in csv)
                {
                    if (lastLogs[log.CharacterName + "." + log.EventName] != null)
                    {
                        lastLog = lastLogs[log.CharacterName + "." + log.EventName];
                        characterLogsFiles[log.CharacterName][log.EventName].WriteLine(lastLog.ToCSV(lastLog.Time, log.Time));
                    }
                    lastLogs[log.CharacterName + "." + log.EventName] = log;
                }

                if (showInfo)
                {
                    foreach (LogEntry l in lastLogs.Values)
                    {
                        characterLogsFiles[l.CharacterName][l.EventName].WriteLine(l.ToCSV(l.Time, l.Time + 3));
                    }
                }

                foreach (Dictionary<string, TextWriter> files in characterLogsFiles.Values) 
                {
                    foreach (TextWriter tw in files.Values) tw.Close();
                }
            }
            
        }

        public List<string> HistoryLog = new List<string>();

        public void Log(string text)
        {
            lock (HistoryLog)
            {
                HistoryLog.Add(text);
            }
        }

        public void ClearHistoryLog()
        {
            lock (HistoryLog)
            {
                HistoryLog.Clear();
            }
        }

        public TimeSpan TimePassed()
        {
            return startTime.Elapsed;
        }

        public TimeSpan CentralTimePassed()
        {
            return TimePassed() + centralTimeShift;
        }

        public string TimePassedString()
        {
            TimeSpan t = CentralTimePassed();
            return t.TotalHours.ToString().PadLeft(2) + ":" + t.Minutes.ToString().PadLeft(2) + ":" + t.Seconds.ToString().PadLeft(2);
        }

    }
}
