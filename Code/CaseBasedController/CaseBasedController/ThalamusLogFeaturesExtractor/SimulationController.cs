using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CaseBasedController;
using CaseBasedController.Behavior;
using CaseBasedController.Detection.Composition.Statistics;
using CaseBasedController.GameInfo;
using CaseBasedController.Simulation;
using CaseBasedController.Thalamus;
using DetectorAnalyzer.ArffLoaders;
using Thalamus;
using ThalamusLogFeautresExtractor;
using ThalamusLogTool;
using Environment = System.Environment;

namespace ThalamusLogFeaturesExtractor
{
    public class SimulationController : IDisposable
    {
        #region Delegates

        public delegate void CurrentProgressDelegate(double progress);

        public delegate void LogDelegate(string message);

        #endregion

        #region State enum

        public enum State
        {
            INITIALIZED,
            RUNNING,
            CANCELED,
            FINISHED,
            FILE_LOADED
        }

        #endregion

        private const string OUTPUT_PATH = "../Output/";
        private const string OUTPUT_FILE_NAME = "features";
        private readonly CurrentProgressDelegate _currentProgressHandler;
        private readonly LogDelegate _logHandler;
        private readonly string _thalamusMessagesDLLsPath = "../ThalamusMessagesDLLs/";
        private CasePool _casePool;
        private ControllerClient _client;
        private FeaturesCollector _fc;
        private string _loadedCasePoolPath;
        private List<string> _logFilesPath = new List<string>();
        private double _progress;
        private State _state = State.INITIALIZED;

        public SimulationController(string casePoolPath, string logFilesPath, string thalamusDLLsPath,
            LogDelegate logHandler = null, CurrentProgressDelegate progressHandler = null)
        {
            if (File.Exists(logFilesPath))
            {
                //new ControllerClient();
                _logFilesPath.Add(logFilesPath);
            }
            else if (Directory.Exists(logFilesPath))
            {
                foreach (string logFile in Directory.GetFiles(logFilesPath, "*.log"))
                {
                    _logFilesPath.Add(logFile);
                }
            }
            else
            {
                throw new FileNotFoundException("Can't find file at: " + logFilesPath);
            }

            _casePool = CasePool.DeserializeFromJson(casePoolPath);
            _fc = CreateFeaturesCollector(_casePool);
            _logHandler = logHandler;
            _client = new ControllerClient();
            _client.setDebug(false);
            _client.setDebugIfs(false);
            _thalamusMessagesDLLsPath = thalamusDLLsPath;
            _currentProgressHandler = progressHandler;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Shutdown();
                _client.Dispose();
                _client = null;
            }
            _fc.Dispose();
        }

        #endregion

        public FeaturesCollector CreateFeaturesCollector(CasePool cp)
        {
            List<string> featuresNames = new List<string>();
            foreach (Case c in cp)
            {
                featuresNames.Add(c.Description);
            }
            featuresNames.Add(FeaturesCollector.CLASS_TO_LEARN_NAME);
            return new FeaturesCollector(featuresNames);
        }

        public void Simulate(CancellationToken token)
        {
            string filesFolderPath = PathWithoutFilename(_logFilesPath[0]);
            foreach (string path in Directory.GetFiles(filesFolderPath, "*.arff")) File.Delete(path);
            ResetSimulationEnvironment();
            Log("=================================== RUNNING SIMULATION");
            PrepareAndRunSimulation(token, true);//false);
            Log("=================================== COMPLETED");
        }

        public void SimulateAndAugment(CancellationToken token)
        {
            string filesFolderPath = PathWithoutFilename(_logFilesPath[0]);
            foreach (string path in Directory.GetFiles(filesFolderPath, "*.arff")) File.Delete(path);
            ResetSimulationEnvironment();
            Log("=================================== RUNNING SIMULATION");
            PrepareAndRunSimulation(token, true);
            Log("=================================== COMPUTING STATISTICS");
            var statistics = ComputeStatistics(filesFolderPath);
            Log("=================================== ADDING NEW DETECTORS BASED ON STATISTICS");
            AugmentDetectorsInCasePool(_casePool, statistics);

            _casePool.SerializeToJson(filesFolderPath + "AugmentedCasePool.json");
            _casePool = LoadCasePool(filesFolderPath + "AugmentedCasePool.json");


            ResetSimulationEnvironment();
            Log("=================================== RUNNING NEW SIMULATION");
            PrepareAndRunSimulation(token, false);
            Log("=================================== MERGING ARFFs");
            ARFFUtils.MergeArffs(filesFolderPath, Log);
            Log("=================================== COMPLETED");
        }

        private void ResetSimulationEnvironment()
        {
            MyTimer.Reset();
            //_casePool.Dispose();
            //_casePool = ReLoadCasePool();
            _casePool.Init(_client, _client.ControllerPublisher as ControllerPublisher);
            if (_fc != null) _fc.Dispose();
            _fc = CreateFeaturesCollector(_casePool);
            _casePool.FeaturesCollector = _fc;
            GameStatus.Reset();
        }

        private void PrepareAndRunSimulation(CancellationToken token, bool exportArffWithTime)
        {
            DateTime startTime = DateTime.Now;
            // =============== DEFINE THE NAMES OF THE FEATURES in the fired case collector
            string featuresNames = "";
            foreach (string feature in _fc.FeaturesNames)
            {
                featuresNames += feature + ",";
            }
            Log(featuresNames);

            int fileIndex = 0;
            if (_logFilesPath.Count > 1)
            {
                Log("Processing a set of " + _logFilesPath.Count + " log files\n\n");
            }

            // =============== SELECT EACH LOG FILE TO BE ANALIZED 
            foreach (string logFilePath in _logFilesPath)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    return;
                }
                Log("Processing log file " + (fileIndex + 1) + "/" + _logFilesPath.Count + "\n");

                if (!File.Exists(logFilePath))
                {
                    Log("Log file not found! Cannot find file at: " + logFilePath);
                    return;
                }
                // ============== LOAD THE LOG FILE
                List<LogEntry> thalamusLog = LoadLogFile(logFilePath);
                // ============== RUN THE SIMULATION
                RunSimulation(thalamusLog, token, _logFilesPath.Count, fileIndex);

                // ============== EXPORT THE ARFF
                string outputFileName = Path.ChangeExtension(logFilePath, "arff");
                    //OUTPUT_PATH+OUTPUT_FILE_NAME + fileIndex + ".arff";
                ARFFUtils.ExportARFF(outputFileName, exportArffWithTime, _fc, Log);

                fileIndex++;
            }
            Log("\nTotal simulation duratio: " + DateTime.Now.Subtract(startTime).ToString("hh':'mm':'ss"));
        }

        private void RunSimulation(List<LogEntry> thalamusLog, CancellationToken token, int totalRuns, int runNumber)
        {
            MyTimer.UseWithSimulationTime();
            MyTimer.SetCurrentTime(0);

            _fc.Dispose();
            _fc = CreateFeaturesCollector(_casePool);
            _casePool.FeaturesCollector = _fc;
            _fc.NewFeaturesVector -= fcc_NewFeaturesVector; // avoid duplicated event handler
            _fc.NewFeaturesVector += fcc_NewFeaturesVector;

            // ================================== COLLECT ALL THE ACTIVE FEATURES AT TIME 0
            // the already active features won't fire, since already active, but we need to know they are. 
            var pool = _casePool.GetPool();
            foreach (var c in pool)
            {
                _fc.SetFeature(c.Key.Description, c.Key.IsActive);
            }
            _fc.FeaturesVectors.RemoveRange(0, _fc.FeaturesVectors.Count - 1);
            _fc.FeaturesVectorsTime.RemoveRange(0, _fc.FeaturesVectorsTime.Count - 1);


            var lastSecond = thalamusLog.Last().Time;
            var firstSecond = thalamusLog.First().Time;

            var currentTimeInSeconds = firstSecond;
            MyTimer.SetCurrentTime(currentTimeInSeconds);
            List<LogEntry> queued = thalamusLog; // list of entry to fire
            while (currentTimeInSeconds <= lastSecond)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    return;
                }

                var toFire = new HashSet<LogEntry>(queued.FindAll(e => e.Time <= currentTimeInSeconds));
                queued.RemoveAll(e => toFire.Contains(e));
                foreach (var ev in toFire)
                {
                    // ============= FIRE THE EVENT IN THE THALAMUS CLIENT
                    _client.ReceiveEvent(ev.Event);

                    // ============= COLLECTS FEATURES
                    if (ev.Event.Name.Equals("IFMLSpeech.PerformUtterance"))
                    {
                        _fc.SetClassForThisVector(ev.Event.Name + ":" + ev.Event.Parameters["category"]);
                    }
                }


                SetProgress(currentTimeInSeconds/lastSecond*(100/totalRuns) + (100/totalRuns)*runNumber);

                currentTimeInSeconds += 0.1;
                MyTimer.SetCurrentTime(currentTimeInSeconds);
            }


            //MainController.SetIsSimulating(false);
            // ============== COOLDOWN (waits for all the detectors events to finish firing)
            CoolDown(10);

            MyTimer.UseWithRealTime();
        }

        private void CoolDown(int secs)
        {
            Log("Cooling down...");
            while (secs > 0)
            {
                Log("" + secs--);
                Thread.Sleep(1000);
            }
        }

        public void AugmentDetectorsInCasePool(CasePool casePool, StatisticsArffLoader statisticsAnalyzer)
        {
            foreach (var detectorStatistics in statisticsAnalyzer.ActivationFreqStats)
            {
                var result =
                    casePool.GetPool()
                        .Values.Where(c => c.Description.Equals(detectorStatistics.Key))
                        .Select(c => c.Detector);
                if (result.Count() <= 0) continue;
                var detectorToWatch = result.First();

                var frequencyDetector = new FeatureFrequencyDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.AVERAGE);
                frequencyDetector.Description = detectorToWatch.Description + "_AverageFrequency";
                casePool.Add(new Case(frequencyDetector, new EmptyBehaviour())
                             {
                                 Description =
                                     frequencyDetector.Description
                             });

                var frequencyDetectorLTA = new FeatureFrequencyDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.LESS_THAN_AVERAGE);
                frequencyDetector.Description = detectorToWatch.Description + "_LessThanAverageFrequency";
                casePool.Add(new Case(frequencyDetectorLTA, new EmptyBehaviour())
                             {
                                 Description =
                                     frequencyDetector.Description
                             });

                var frequencyDetectorMTA = new FeatureFrequencyDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.MORE_THAN_AVERAGE);
                frequencyDetector.Description = detectorToWatch.Description + "_MoreThanAverageFrequency";
                casePool.Add(new Case(frequencyDetectorMTA, new EmptyBehaviour())
                             {
                                 Description =
                                     frequencyDetector.Description
                             });
            }
            foreach (var detectorStatistics in statisticsAnalyzer.TimeActiveStats)
            {
                var result =
                    casePool.GetPool()
                        .Values.Where(c => c.Description.Equals(detectorStatistics.Key))
                        .Select(c => c.Detector);
                if (result.Count() <= 0) continue;
                var detectorToWatch = result.First();

                var activeTimeDetector = new FeatureActivationTimeDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.AVERAGE);
                activeTimeDetector.Description = detectorToWatch.Description + "_AverageActiveTime";
                casePool.Add(new Case(activeTimeDetector, new EmptyBehaviour())
                             {
                                 Description =
                                     activeTimeDetector.Description
                             });

                var activeTimeDetectorLTA = new FeatureActivationTimeDetector(detectorToWatch,
                    detectorStatistics.Value.Avg, detectorStatistics.Value.StdDev,
                    FeatureStatisticalPropertyDetector.ActivationMode.LESS_THAN_AVERAGE);
                activeTimeDetector.Description = detectorToWatch.Description + "_LessThanAverageActiveTime";
                casePool.Add(new Case(activeTimeDetectorLTA, new EmptyBehaviour())
                             {
                                 Description =
                                     activeTimeDetector
                                     .Description
                             });

                var activeTimeDetectorMTA = new FeatureActivationTimeDetector(detectorToWatch,
                    detectorStatistics.Value.Avg, detectorStatistics.Value.StdDev,
                    FeatureStatisticalPropertyDetector.ActivationMode.MORE_THAN_AVERAGE);
                activeTimeDetector.Description = detectorToWatch.Description + "_MoreThanAverageActiveTime";
                casePool.Add(new Case(activeTimeDetectorMTA, new EmptyBehaviour())
                             {
                                 Description =
                                     activeTimeDetector
                                     .Description
                             });
            }

            foreach (var detectorStatistics in statisticsAnalyzer.TimeSinceLastActivationStats)
            {
                var result =
                    casePool.GetPool()
                        .Values.Where(c => c.Description.Equals(detectorStatistics.Key))
                        .Select(c => c.Detector);
                if (result.Count() <= 0) continue;
                var detectorToWatch = result.First();

                var activeDelay = new FeatureActivationDelayDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.AVERAGE);
                activeDelay.Description = detectorToWatch.Description + "_AverageActiveDelay";
                casePool.Add(new Case(activeDelay, new EmptyBehaviour()) {Description = activeDelay.Description});

                var activeDelayLTA = new FeatureActivationDelayDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.AVERAGE);
                activeDelay.Description = detectorToWatch.Description + "_LessThanAverageActiveDelay";
                casePool.Add(new Case(activeDelayLTA, new EmptyBehaviour()) {Description = activeDelay.Description});

                var activeDelayMTA = new FeatureActivationDelayDetector(detectorToWatch, detectorStatistics.Value.Avg,
                    detectorStatistics.Value.StdDev, FeatureStatisticalPropertyDetector.ActivationMode.AVERAGE);
                activeDelay.Description = detectorToWatch.Description + "_MoreThanAverageActiveDelay";
                casePool.Add(new Case(activeDelayMTA, new EmptyBehaviour()) {Description = activeDelay.Description});
            }

            _casePool.Init(_client, _client.ControllerPublisher);
        }

        private List<LogEntry> LoadLogFile(string logFilePath)
        {
            LogTool.LoadingInterfacesErrorEvent -= LogTool_LoadingTypesErrorEvent; // Avoid duplicated hanlders
            LogTool.LoadingInterfacesErrorEvent += LogTool_LoadingTypesErrorEvent;
            LogTool.LoadingAssemblyErrorEvent -= LogTool_LoadingAssemblyErrorEvent; // Avoid duplicated hanlders
            LogTool.LoadingAssemblyErrorEvent += LogTool_LoadingAssemblyErrorEvent;
            LogTool.LoadingAssemblyTypesErrorEvent -= LogTool_LoadingAssemblyTypesErrorEvent;
                // Avoid duplicated hanlders
            LogTool.LoadingAssemblyTypesErrorEvent += LogTool_LoadingAssemblyTypesErrorEvent;
            List<LogEntry> thalamusLog = LogTool.LoadThalamusLogEntries(logFilePath,
                Path.GetFullPath(_thalamusMessagesDLLsPath));
                //System.Reflection.Assembly.GetExecutingAssembly().Location));// Application.StartupPath+ "\\..\\ThalamusMessagesDLLs\\");          // TODO: Dynamically load this

            return thalamusLog;
        }

        public void SetLogFile(string filePath)
        {
            _logFilesPath = new List<string> {filePath};
            SetState(State.FILE_LOADED);
        }

        public void SetLogFiles(string folderPath)
        {
            _logFilesPath = new List<string>();
            foreach (string file in Directory.GetFiles(folderPath))
            {
                _logFilesPath.Add(file);
            }
            // Selecting only log files
            _logFilesPath = _logFilesPath.Where(l => Path.GetExtension(l).Equals(".log")).ToList();
            SetState(State.FILE_LOADED);
        }

        public string GetLoadedLogFile()
        {
            return (_logFilesPath.Count > 1
                ? _logFilesPath.Count + " files in " + Path.GetDirectoryName(_logFilesPath.First())
                : _logFilesPath.First());
        }

        /// <summary>
        ///     Compute statistics over a set of arff files contained in a folder
        /// </summary>
        /// <param name="arffsPath"></param>
        /// <returns></returns>
        public StatisticsArffLoader ComputeStatistics(string arffsPath)
        {
            StatisticsArffLoader statisticsAnalyzer = null;
            if (Directory.Exists(arffsPath))
            {
                var arffs = Directory.GetFiles(arffsPath, "*.arff");
                foreach (string arffPath in arffs)
                {
                    if (statisticsAnalyzer == null)
                        statisticsAnalyzer = new StatisticsArffLoader();

                    statisticsAnalyzer.Load(arffPath);
                }
                if (statisticsAnalyzer != null) statisticsAnalyzer.PrintResults(arffsPath + "\\stats");
            }
            else
            {
                throw new DirectoryNotFoundException("Couldn't find directory " + arffsPath);
            }
            return statisticsAnalyzer;
        }

        private void fcc_NewFeaturesVector(object sender, NewFeaturesVectorEventArgs e)
        {
            string txt = MyTimer.GetCurrentTime() + " >> ";
            foreach (string f in e.FeaturesVector)
            {
                txt += f + ",";
            }
            txt.TrimEnd(',');
            Log(txt);
        }

        #region LOGS

        private void Log(string message)
        {
            if (_logHandler != null)
            {
                _logHandler(message);
            }
            //Console.WriteLine("SIMULATOR>> " + message);
        }

        #endregion

        #region ARFF

        /*
        public void MergeArffs(string path)
        {
            List<string> arffs = Directory.GetFiles(path).Where(f => Path.GetExtension(f).Equals(".arff")).ToList<string>();
            List<string> attributes = new List<string>();
            List<string> behaviours = new List<string>();
            List<string> data = new List<string>();
            foreach (string arff in arffs)
            {
                using (StreamReader reader = new StreamReader(arff))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    if (line.StartsWith("@"))
                    {
                        if (line.StartsWith("@ATTRIBUTE Behaviour"))
                        {
                            string be = line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{')-2);
                            var x = be.Split(',').ToList<string>();
                            behaviours.AddRange(x);
                            behaviours = behaviours.Distinct().ToList();
                        }
                        else
                        {
                            if (!attributes.Contains(line))
                                attributes.Add(line);
                        }
                    }
                    else
                    {
                        data.Add(line);
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(path + "\\merged.arff"))
            {
                foreach (string a in attributes)
                {
                    if (!a.Equals("@DATA"))
                        writer.WriteLine(a);
                }
                writer.Write("@ATTRIBUTE Behaviour {");
                for (int i = 0; i < behaviours.Count; i++)
                {
                    writer.Write(behaviours[i]);
                    if (i != behaviours.Count - 1) writer.Write(",");
                }
                writer.Write("}\n");
                writer.WriteLine("@DATA");
                foreach (string d in data) writer.WriteLine(d);
            }
        }

        private void ExportARFF(string outputFileName, bool exportWithTime)
        {
            Log("Exporting ARFF file...");
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                writer.WriteLine("@RELATION emoteExample");
                if (exportWithTime)
                    writer.WriteLine("@ATTRIBUTE Time NUMERIC");
                for (int k = 0; k < _fc.FeaturesNames.Count; k++)
                {
                    if (_fc.FeaturesNames[k].Equals(FeaturesCollector.CLASS_TO_LEARN_NAME))
                    {
                        // Writing all the Behaviour field possible states
                        List<string> states = _fc.FeaturesVectors.Select(x => x[x.Length - 1]).Distinct().ToList();
                        string statesStr = "";
                        foreach (string s in states) statesStr += s + ",";
                        statesStr.TrimEnd(',');
                        writer.WriteLine("@ATTRIBUTE " + _fc.FeaturesNames[k] + " {" + statesStr + "}");
                    }
                    else
                        writer.WriteLine("@ATTRIBUTE " + _fc.FeaturesNames[k] + " {1,0}");
                }
                
                writer.WriteLine("@DATA");
                foreach (string[] fv in _fc.FeaturesVectors)
                {
                    for (int k = 0; k < fv.Length; k++)
                    {
                        if (k == 0)         // If we are exporting the time as well, than we put it as first field
                            if (exportWithTime)
                                writer.Write(_fc.FeaturesVectorsTime[_fc.FeaturesVectors.IndexOf(fv)].TotalSeconds+",");
                        // the last element is the class
                        writer.Write(fv[k]);
                        writer.Write((k == (fv.Length - 1)) ? System.Environment.NewLine : ",");
                    }
                }
            }
            Log("Exporting ARFF file...Completed");
        }
        */

        #endregion

        #region LogTool Loading Error Handling

        private void LogTool_LoadingTypesErrorEvent(object sender, LoadingInterfacesErrorEventArgs e)
        {
            string message = "Can't load following types:";
            foreach (string t in e.NotLoadedTypes)
            {
                message += Environment.NewLine + '\t' + t;
            }
            Log(message);
        }

        private void LogTool_LoadingAssemblyErrorEvent(object sender, LoadingAssemblyErrorEventArgs e)
        {
            string message = "Can't load Assembly: " + e.NotLoadedAssembly;
            Log(message);
        }

        private void LogTool_LoadingAssemblyTypesErrorEvent(object sender, LoadingAssemblyTypesEventArgs e)
        {
            string message = "Can't load types into Assembly named: " + e.AssemblyFile;
            Log(message);
        }

        #endregion

        #region HELPERS

        private string PathWithoutFilename(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);
            return fullPath.Remove(fullPath.Length - fileName.Length);
        }

        public CasePool LoadCasePool(string path)
        {
            CasePool casePool;
            try
            {
                casePool = CasePool.DeserializeFromJson(path);
                casePool.Init(_client, _client.ControllerPublisher);
                _loadedCasePoolPath = path;
                return casePool;
            }
            catch (FileNotFoundException)
            {
                Log("Error loading CasePool file! Can't load file " + Path.GetFullPath(path));
                return null;
            }
        }

        private void SetState(State state)
        {
            _state = state;
        }

        private void SetProgress(double val)
        {
            _progress = val;
            if (_currentProgressHandler != null)
            {
                _currentProgressHandler(val);
            }
        }

        #endregion
    }
}