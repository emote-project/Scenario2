using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thalamus;

namespace ThalamusLogTool
{
    public class LoadingInterfacesErrorEventArgs : EventArgs
    {
        public List<string> NotLoadedTypes { get; set; }

        public LoadingInterfacesErrorEventArgs(List<string> notLoadedTypes)
        {
            NotLoadedTypes = notLoadedTypes;
        }
    }
    public class LoadingAssemblyErrorEventArgs : EventArgs
    {
        public string NotLoadedAssembly { get; set; }

        public LoadingAssemblyErrorEventArgs(string notLoadedAssembly)
        {
            NotLoadedAssembly = notLoadedAssembly;
        }
    }
    public class LoadingAssemblyTypesEventArgs : EventArgs
    {
        public Assembly AssemblyFile { get; set; }
        public System.Reflection.ReflectionTypeLoadException Exception;
        public LoadingAssemblyTypesEventArgs(Assembly assemblyFile, System.Reflection.ReflectionTypeLoadException exception)
        {
            AssemblyFile = assemblyFile;
            Exception = exception;
        }
    }
    public class LogTool
    {
        /// <summary>
        /// Fired when the log contains messages generated with interfaces that were not loaded in the assembly
        /// </summary>
        static public event EventHandler<LoadingInterfacesErrorEventArgs> LoadingInterfacesErrorEvent;
        /// <summary>
        /// Fired when it's impossible to load an assembly (the assembly may reference other missing files or target different systems ...)
        /// </summary>
        static public event EventHandler<LoadingAssemblyErrorEventArgs> LoadingAssemblyErrorEvent;
        /// <summary>
        /// Fired when it's impossible to load the types contained into an assembly. (Check the exceptions property to debug the problem)
        /// </summary>
        static public event EventHandler<LoadingAssemblyTypesEventArgs> LoadingAssemblyTypesErrorEvent;

        static string thalamusLogRegexValidator = @"\(\d\d:\d\d:\d\d\.\d+\) \w*\[log\]: \d+[,\.]?\d+:(\w| )*:(\w| )*:[\w\.]*:\([\w\.]*\):.*";
        public static string ThalamusLogRegexValidator
        {
            get { return thalamusLogRegexValidator; }
        }

        static Dictionary<string, LogEntry> lastLogs = new Dictionary<string, LogEntry>();
        static Dictionary<string, Type> loadedMessageTypes = new Dictionary<string, Type>();
        public static Dictionary<string, Type> LoadedMessageTypes
        {
            get { return LogTool.loadedMessageTypes; }
        }
        static IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;
        static CultureInfo ci = CultureInfo.InvariantCulture;


        public static int IndexOfNth(string str, char c, int n)
        {
            int s = -1;
            for (int i = 0; i < n; i++)
            {
                s = str.IndexOf(c, s + 1);
                if (s == -1) break;
            }
            return s;
        }

        public static Dictionary<string, List<string>> LoadLogSimpleThalamus(string logFile, bool filterReceived = true, bool filterSent = true)
        {
            Dictionary<string, List<string>> logDict = new Dictionary<string, List<string>>();
            string[] logs = logFile.Split('\n');
            foreach (string s in logs) 
            {
                if (s.Trim().Length > 0 && Regex.IsMatch(s,thalamusLogRegexValidator))
                {
                    string log = s.Replace('\r', ' ').Substring(s.IndexOf(':', s.IndexOf(':', s.IndexOf(':') + 1) + 1) + 1);
                    string[] pieces = log.Split(':');
                    
                    string direction = "Sent->";
                    if (pieces[1].Length == 0)
                    {
                        direction = "Received<-";
                        if (!filterReceived && filterSent) continue;
                    }
                    else
                    {
                        if (!filterSent && filterReceived) continue;
                    }
                    string key = pieces[3];

                    if (filterReceived || filterSent) key = direction + key;

                    if (!logDict.ContainsKey(key)) logDict[key] = new List<string>();
                    logDict[key].Add(pieces[0] + ":" + pieces[5]);
                }
            }
            return logDict;
        }


        public static void ConvertThalamusToCsv(string filename, string[] stringlogs)
        {
            if (!Directory.Exists(Manager.CorrectPath(filename))) Directory.CreateDirectory(Manager.CorrectPath(filename));
            List<LogEntry> logs = new List<LogEntry>();
            foreach (string s in stringlogs)
            {
                try
                {
                    if (s.Trim().Length > 0 && Regex.IsMatch(s, thalamusLogRegexValidator))
                    {
                        string log = s.Replace('\r', ' ').Trim();
                        string[] split = log.Split(':');
                        logs.Add(new LogEntry(split[5].Length == 0, double.Parse(split[3].Trim().Replace(',', '.'), ifp), (new PML(split[6])).Decode(log.Substring(IndexOfNth(log, ':', 7) + 1))));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            Dictionary<string, TextWriter> csvFiles = new Dictionary<string, TextWriter>();
            foreach (LogEntry log in logs)
            {
                if (!csvFiles.ContainsKey(log.EventName))
                {
                    csvFiles[log.EventName] = new StreamWriter(Manager.CorrectPath(filename + "\\" + log.EventName + ".csv"), true);
                    if (!lastLogs.ContainsKey(log.EventName))
                    {
                        lastLogs[log.EventName] = null;
                        csvFiles[log.EventName].WriteLine(log.ToCSVHeader());
                    }
                }
            }

            LogEntry lastLog = null;
            foreach (LogEntry log in logs)
            {
                if (lastLogs[log.EventName] != null)
                {
                    lastLog = lastLogs[log.EventName];
                    csvFiles[log.EventName].WriteLine(lastLog.ToCSV(lastLog.Time, log.Time));
                }
                lastLogs[log.EventName] = log;
            }

            foreach (TextWriter tw in csvFiles.Values)
            {
                tw.Close();
            }
        }

        public static Dictionary<string, MethodInfo> LoadMessagesFromAssembly(string assemblyPath = "")
        {
            Dictionary<string, MethodInfo> msgs = new Dictionary<string, MethodInfo>();
            if (File.Exists(assemblyPath))
            {
                Assembly a = null;
                try
                {
                    a = Assembly.LoadFile(assemblyPath);
                }
                catch (System.NotSupportedException e)
                {
                    Console.WriteLine("Couldn't load DLL <" + assemblyPath);
                    if (LoadingAssemblyErrorEvent != null) LoadingAssemblyErrorEvent(null, new LoadingAssemblyErrorEventArgs(assemblyPath));
                    a = null;
                }

                if (a != null)
                {
                    try
                    {
                        foreach (Type t in a.GetTypes())
                        {
                            if (ThalamusClient.ImplementsTypeDirectly(t, typeof(IAction)) || ThalamusClient.ImplementsTypeDirectly(t, typeof(IPerception)))
                            {
                                loadedMessageTypes[t.Name] = t;
                                var methods = t.GetMethods();
                                foreach (var m in methods)
                                {
                                    msgs[t.Name + "." + m.Name] = m;
                                }
                            }
                        }
                        PML.AddAssembly(a);
                    }
                    catch (System.Reflection.ReflectionTypeLoadException re)
                    {
                        Console.WriteLine("Can't load types in assembly: " + a.FullName +
                            System.Environment.NewLine + "\t" + re.Message +
                            System.Environment.NewLine + "\t" + re.LoaderExceptions);
                        if (LoadingAssemblyTypesErrorEvent != null) LoadingAssemblyTypesErrorEvent(null, new LoadingAssemblyTypesEventArgs(a, re));
                    }
                }
            }
            return msgs;
        }

        public static Dictionary<string, Type> LoadClassesFromAssembly(string assemblyPath = "")
        {
            Dictionary<string, Type> msgs = new Dictionary<string, Type>();
            if (File.Exists(assemblyPath))
            {
                Assembly a = null;
                try
                {
                    a = Assembly.LoadFile(assemblyPath);
                }
                catch (System.NotSupportedException e)
                {
                    Console.WriteLine("Couldn't load DLL <" + assemblyPath);
                    if (LoadingAssemblyErrorEvent != null) LoadingAssemblyErrorEvent(null, new LoadingAssemblyErrorEventArgs(assemblyPath));
                    a = null;
                }

                if (a != null)
                {
                    try
                    {
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.IsInterface) continue;
                            if (ThalamusClient.ImplementsTypeDirectly(t, typeof(IAction)))
                            {
                                loadedMessageTypes[t.Name] = t;
                                msgs[t.Name] = t;
                            }
                            else if (ThalamusClient.ImplementsTypeDirectly(t, typeof(IPerception)))
                            {
                                loadedMessageTypes[t.Name] = t;
                                msgs[t.Name] = t;
                            }
                        }
                        PML.AddAssembly(a);
                    }
                    catch (System.Reflection.ReflectionTypeLoadException re)
                    {
                        Console.WriteLine("Can't load types in assembly: " + a.FullName +
                            System.Environment.NewLine + "\t" + re.Message +
                            System.Environment.NewLine + "\t" + re.LoaderExceptions);
                        if (LoadingAssemblyTypesErrorEvent != null) LoadingAssemblyTypesErrorEvent(null, new LoadingAssemblyTypesEventArgs(a, re));
                    }
                }
            }
            return msgs;
        }

        public static Dictionary<string, Type> LoadInterfaces(string assemblyPath = ".") 
        {
            string assembliesPath = assemblyPath;
            List<Assembly> allAssemblies = new List<Assembly>();
            foreach (string ass in Directory.GetFiles(assembliesPath, "*.dll"))
            {
                try
                {
                    allAssemblies.Add(Assembly.ReflectionOnlyLoad(ass));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't load DLL <" + ass);
                    if (LoadingAssemblyErrorEvent != null) LoadingAssemblyErrorEvent(null, new LoadingAssemblyErrorEventArgs(ass));
                }
            }
            foreach (string ass in Directory.GetFiles(assembliesPath, "*.exe"))
            {
                try
                {
                    allAssemblies.Add(Assembly.ReflectionOnlyLoad(ass));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't load EXE library <" + ass);
                    if (LoadingAssemblyErrorEvent != null) LoadingAssemblyErrorEvent(null, new LoadingAssemblyErrorEventArgs(ass));
                }
            }

            //////////// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< INSERT TRY CATCH TO VERIFY IF AND WHEN WE HAVE PROBLEMS LOADING DLLs

            
            foreach (Assembly a in allAssemblies)
            {
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (ThalamusClient.ImplementsTypeDirectly(t, typeof(IAction)))
                        {
                            //string interfaceName = t.Name.Substring(t.Name.LastIndexOf('.') + 1);
                            //Console.WriteLine("Found Action interface: '" + t.Name + "'");
                            loadedMessageTypes[t.Name] = t;
                        }
                        else if (ThalamusClient.ImplementsTypeDirectly(t, typeof(IPerception)))
                        {
                            //Console.WriteLine("Found Perception interface: '" + t.Name + "'");
                            loadedMessageTypes[t.Name] = t;
                        }
                    }
                    PML.AddAssembly(a);
                }
                catch (System.Reflection.ReflectionTypeLoadException re)
                {
                    Console.WriteLine("Can't load types in assembly: " + a.FullName +
                        System.Environment.NewLine + "\t" + re.Message +
                        System.Environment.NewLine + "\t" + re.LoaderExceptions);
                    if (LoadingAssemblyTypesErrorEvent != null) LoadingAssemblyTypesErrorEvent(null, new LoadingAssemblyTypesEventArgs(a, re));
                }
            }
            return loadedMessageTypes;
        }

        public static List<LogEntry> LoadThalamusLogEntries(string filename, string assemblyPath = ".")
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("File '" + filename + "' does not exist!");
            }
            else
            {
                string logFile = "";

                List<LogEntry> logEntries = new List<LogEntry>();

                List<string> failedInterfaces = new List<string>();
                using (StreamReader file = File.OpenText(filename))
                {
                    logFile = file.ReadToEnd();
                    string[] stringlogs = logFile.Split('\n');

                    foreach (string s in stringlogs)
                    {
                        try
                        {
                            if (s.Trim().Length > 0 && Regex.IsMatch(s, thalamusLogRegexValidator))
                            {
                                string log = s.Replace('\r', ' ').Trim();
                                string[] split = log.Split(':');

                                //find split[6] in assemblies
                                string interfaceName = split[6].Substring(0, split[6].IndexOf('.'));
                                if (loadedMessageTypes.ContainsKey(interfaceName))
                                {
                                    MethodInfo[] events = loadedMessageTypes[interfaceName].GetMethods();
                                    string eventName = split[6].Substring(split[6].LastIndexOf('.') + 1);
                                    foreach (MethodInfo ev in events)
                                    {
                                        if (ev.Name == eventName)
                                        {
                                            string value = log.Substring(LogTool.IndexOfNth(log, ':', 8) + 1);
                                            object[] ovalues = new object[] { };
                                            if (value.Length > 2)
                                            {
                                                value = value.Substring(1, value.Length - 3);
                                                string[] splitValues = value.Split(';');
                                                ovalues = new object[splitValues.Length];
                                                for (int i = 0; i < splitValues.Length; i++)
                                                {
                                                    ovalues[i] = splitValues[i].Split('=')[1];
                                                }
                                            }
                                            else value = "";

                                            PML pml = new PML(split[6], ev, ovalues);
                                            LogEntry l = new LogEntry(split[5].Length == 0, double.Parse(split[3].Trim().Replace(',', '.'), ifp), "", split[5].Length == 0 ? split[6] : split[5], pml);
                                            logEntries.Add(l);
                                            //Console.WriteLine("Loaded LogEntry: " + l.ToString());
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!failedInterfaces.Contains(interfaceName)) failedInterfaces.Add(interfaceName);
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                if (failedInterfaces.Count > 0)
                {
                    if (LoadingInterfacesErrorEvent != null) LoadingInterfacesErrorEvent(null, new LoadingInterfacesErrorEventArgs(failedInterfaces));
                    string txt = "Failed to find the following types:\n";
                    foreach (string s in failedInterfaces)
                    {
                        txt += "\t" + s + "\n";
                    }
                    Console.WriteLine(txt);
                    //MessageBox.Show(txt, "Load Thalamus LogEntries", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
                return logEntries;
            }
        }

        public static void ConvertThalamusToSrt(string path, string[] stringlogs, int maxLength = 150)
        {
            Dictionary<double, List<LogEntry>> logs = new Dictionary<double, List<LogEntry>>();
            Dictionary<string, string> EventValues = new Dictionary<string, string>();
            foreach (string s in stringlogs)
            {
                try
                {
                    if (s.Trim().Length > 0 && Regex.IsMatch(s, thalamusLogRegexValidator))
                    {
                        string log = s.Replace('\r', ' ').Trim();
                        string[] split = log.Split(':');
                        Double t = Math.Round(double.Parse(split[3].Trim().Replace(',', '.'), ifp), 1);
                        if (!logs.ContainsKey(t)) logs[t] = new List<LogEntry>();
                        logs[t].Add(new LogEntry(split[5].Length == 0, t, (new PML(split[6])).Decode(log.Substring(IndexOfNth(log, ':', 7) + 1))));
                        if (!EventValues.ContainsKey(split[6].Trim())) EventValues[split[6].Trim()] = "n/a";
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            List<double> times = logs.Keys.ToList();
            int i = 1;
            DateTime last_dt = new DateTime(1, 1, 1, 0, 0, 0, 0);
            using (StreamWriter file = File.CreateText(path))
            {
                foreach (double t in times)
                {
                    int i_t = (int)Math.Truncate(t);
                    int ct_h = i_t / 3600;
                    int ct_m = (i_t - 3600 * ct_h) / 60;
                    int ct_s = i_t - 3600 * ct_h - 60 * ct_m;
                    int ct_ms = (int)((t - Math.Truncate(t)) * 1000);
                    DateTime current_dt = new DateTime(1, 1, 1, ct_h, ct_m, ct_s, ct_ms);

                    //aux_CTh2Srt_Update(ref EventValues, logs[t]);
                    List<string[]> temp = new List<string[]>();
                    foreach (LogEntry log in logs[t])
                    {
                        if (EventValues.ContainsKey(log.EventName)) temp.Add(new string[]{log.EventName,log.Event.ToStringSimple()});
                    }
                    file.Write(aux_CTh2Srt_MakeLine(current_dt.AddMilliseconds(-1), last_dt, i++, aux_CTh2Srt_MakeText(temp, maxLength)));
                    last_dt = current_dt;
                }
            }
        }

        private static string aux_CTh2Srt_MakeLine(DateTime current_t, DateTime last_t, int i, string text)
        {
            string txt = String.Format("{0}{4}{1} --> {2}{4}{3}{4}{4}", i, last_t.ToString("HH:mm:ss,fff", ci), current_t.ToString("HH:mm:ss,fff", ci), text, System.Environment.NewLine);
            return txt;
        }

        private static string aux_CTh2Srt_MakeText(List<string[]> EventValues, int maxLength)
        {
            string txt = "";
            string line = "";
            foreach (string[] ev in EventValues)
            {
                string thisLine = String.Format("{0}:{1}", ev[0], ev[1]);
                if (thisLine.Length > maxLength) thisLine = thisLine.Substring(0, (int)maxLength);
                /*if ((line + thisLine).Length > numConvertThalamusToSrtMaxLineLength.Value)
                {
                    txt += line + "\n";
                    line = "";
                }
                else line+="     ";
                line += thisLine;*/
                txt += thisLine + System.Environment.NewLine;
            }
            char US = (char)31;
            txt = txt.Replace(US, ' ').Trim();
            return txt;
        }

        private static void aux_CTh2Srt_Update(ref Dictionary<string, string> EventValues, List<LogEntry> logs)
        {
            foreach (LogEntry log in logs)
            {
                if (EventValues.ContainsKey(log.EventName)) EventValues[log.EventName] = log.Event.ToStringSimple();
            }
        }

        public static List<LogEntry> ConvertCsvToThalamus(string messageName, string filename, MethodInfo convertToMessageType)
        {
            /*ConstructorInfo[] ctors = convertToObjectType.GetConstructors();
            ConstructorInfo largestCtor = null;
            foreach(var ctor in ctors) if (largestCtor==null || largestCtor.GetParameters().Length<ctor.GetParameters().Length) largestCtor = ctor;
            if (largestCtor != null)
            {
                ParameterInfo[] parameters = largestCtor.GetParameters();*/
                ParameterInfo[] parameters = convertToMessageType.GetParameters();
                List<LogEntry> convertedObjects = new List<LogEntry>();
                /*foreach (string csvFile in csvFiles)
                {*/
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);
                    string line;
                    int lineNumber = 1;
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] paramValues = new string[parameters.Length];
                        string[] paramTypes = new string[parameters.Length];
                        string[] s_params = line.Split(',');
                        if (s_params.Length != parameters.Length)
                        {
                            MessageBox.Show("Specified CSV file does not match parameters list of message '" + convertToMessageType.Name + "'!", "Convert CSV to Thalamus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return new List<LogEntry>();
                        }
                        int i;
                        for (i = 0; i < s_params.Length; i++)
                        {
                            paramValues[i] = s_params[i].Trim();
                        }

                        string[] paramNames = new string[parameters.Length];
                        i = 0;
                        foreach (var p in parameters)
                        {
                            paramNames[i] = p.Name;
                            paramTypes[i++] = DataTypes.SystemTypeToPMLType(p.ParameterType).ToString();
                        }



                        var pml = new PML(messageName, paramNames, paramTypes, paramValues);
                        if (pml != null)
                        {
                            string readTime = pml.Parameters.ContainsKey("time") && pml.Parameters["time"].Type == PMLParameterType.Double ? "time" : "";
                            if (readTime == "") readTime = pml.Parameters.ContainsKey("Time") && pml.Parameters["Time"].Type == PMLParameterType.Double ? "Time" : "";
                            //object obj = ConvertCsvEntryToObject(line, largestCtor, parameters);
                            convertedObjects.Add(new LogEntry(false, readTime == "" ? lineNumber : pml.Parameters[readTime].GetDouble(), pml));
                        }
                        else
                        {
                            Console.WriteLine("Failed to convert line " + lineNumber);
                            break;
                        }
                        lineNumber++;
                    }
                    file.Close();
                //}
                return convertedObjects;
            /*}
            else
            {
                MessageBox.Show("The specified type does not contain any constructor!", "Convert CSV to Thalamus", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<object>();
            }*/
        }

        private static object ConvertCsvEntryToObject(string csv, ConstructorInfo ctor, ParameterInfo[] parameters, char sep =',')
        {
            string[] s_params = csv.Split(sep);
            int i;
            for (i = 0; i < s_params.Length; i++) s_params[i] = s_params[i].Trim();
            i = 0;
            List<object> convertedParams = new List<object>();
            int c_i;
            double c_d;
            bool c_b;
            float c_f;

            try
            {
                foreach (ParameterInfo p in parameters)
                {
                    if (p.ParameterType == typeof(int) && int.TryParse(s_params[i], out c_i)) convertedParams.Add(c_i);
                    else if (p.ParameterType == typeof(double) && double.TryParse(s_params[i], NumberStyles.Float, ifp, out c_d)) convertedParams.Add(c_d);
                    else if (p.ParameterType == typeof(float) && float.TryParse(s_params[i], NumberStyles.Float, ifp, out c_f)) convertedParams.Add(c_f);
                    else if (p.ParameterType == typeof(bool) && bool.TryParse(s_params[i], out c_b)) convertedParams.Add(c_b);
                    else convertedParams.Add(s_params[i]);
                    i++;
                }
                var obj = ctor.Invoke(convertedParams.ToArray());
                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error converting CSV line: " + e.ToString());
            }
            return null;
        }
    }


}
