using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaseBasedController.Behavior;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Simulation;
using CaseBasedController.Thalamus;
using Newtonsoft.Json;
using PS.Utilities.Serialization;

namespace CaseBasedController
{

    #region Delegates

    public delegate void CaseActivationEventHandler(Case item, bool activated);

    public delegate void CaseEventHandler(Case item);

    #endregion

    /// <summary>
    ///     Represents a set of manageable <see cref="Case" />.
    /// </summary>
    public class CasePool : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            this.Clear();
            foreach (var timer in this._activationQueue)
                timer.Dispose();
            this._activationQueue.Clear();
            if (this.FeaturesCollector != null)
                this.FeaturesCollector.Dispose();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when some <see cref="Case" /> becomes (in)active.
        /// </summary>
        public event CaseActivationEventHandler CaseActivationChanged;

        /// <summary>
        ///     Occurs when some <see cref="Case" /> starts being executed.
        /// </summary>
        public event CaseEventHandler CaseExecutionStarted;

        /// <summary>
        ///     Occurs when some <see cref="Case" /> execution has ended.
        /// </summary>
        public event CaseEventHandler CaseExecutionEnded;

        #endregion

        #region Fields

        private readonly SortedSet<Case> _activationQueue = new SortedSet<Case>();
        private readonly object _locker = new object();
        private readonly Dictionary<IFeatureDetector, Case> _pool = new Dictionary<IFeatureDetector, Case>();
        private Case _caseInExecution;

        #endregion

        #region Properties

        [JsonProperty]
        private List<Case> Pool { get; set; }

        [JsonIgnore]
        public FeaturesCollector FeaturesCollector { get; set; }

        #endregion

        #region List methods

        [JsonIgnore]
        public int Count
        {
            get { lock (this._locker) return _pool.Count; }
        }

        public IEnumerator<Case> GetEnumerator()
        {
            lock (this._locker) return this._pool.Values.GetEnumerator();
        }

        public void AddRange(IEnumerable<Case> items)
        {
            foreach (var item in items)
                this.Add(item);
        }

        public bool Add(Case item)
        {
            lock (this._locker)
            {
                if (this.Contains(item)) return false;

                //attaches case activation events
                item.Detector.ActivationChanged += this.OnCaseActivationChanged;

                this._pool.Add(item.Detector, item);
                return true;
            }
        }

        public bool Remove(Case item)
        {
            lock (this._locker)
            {
                if (!this.Contains(item)) return false;

                //detaches case activation events
                item.Detector.ActivationChanged -= this.OnCaseActivationChanged;
                return this._pool.Remove(item.Detector);
            }
        }

        public void Clear()
        {
            lock (this._locker)
                foreach (var caseElem in this._pool.Values.ToList())
                    this.Remove(caseElem);
        }

        public bool Contains(Case item)
        {
            lock (this._locker) return this._pool.ContainsKey(item.Detector);
        }

        #endregion

        #region Serialization

        public void SerializeToJson(string filePath)
        {
            this.Pool = this._pool.Values.ToList();
            this.SerializeJsonFile(filePath, JsonUtil.PreserveReferencesSettings);
        }

        public static CasePool DeserializeFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Cannot find CasePool file", filePath);
            CasePool casePool = null;
            try
            {
                casePool = JsonUtil.DeserializeJsonFile<CasePool>(filePath, JsonUtil.PreserveReferencesSettings);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception(
                    "Can't load CasePool file. Probably the file is not compatible with the current version of the system or it's damaged");
            }

            casePool.AddRange(casePool.Pool);
            casePool.Pool = null;
            return casePool;
        }

        #endregion

        #region Case execution queue

        private void AddCaseToQueue(Case actCase)
        {
            lock (this._locker)
            {
                if (this._activationQueue.Contains(actCase)) return;
                //console.writeline("Adding case to queue: " + actCase);
                //adds the activated case to the queue and processes queue
                this._activationQueue.Add(actCase);

                //console.writeline("Queue: ");
                foreach (var c in _activationQueue)
                {
                    Console.Write(c + ",");
                }
                //console.writeline("-----------");
                this.ProcessQueue();
            }
        }

        private void RemoveCaseFromQueue(Case actCase)
        {
            lock (this._locker)
            {
                if (!this._activationQueue.Contains(actCase)) return;
                //console.writeline("Removing case to queue: " + actCase);
                this._activationQueue.Remove(actCase);
                //console.writeline("Queue: ");
                foreach (var c in _activationQueue)
                {
                    Console.Write(c + ",");
                }
                //console.writeline("-----------");
            }
        }

        private void ProcessQueue()
        {
            lock (this._locker)
            {
                //console.writeline("Processing queue.. " + this._activationQueue.Count);
                //verifies queue, nothing to do
                if (this._activationQueue.Count == 0) return;

                //gets highest prioriy case, based on behavior
                if (_activationQueue.Count(c => c.Enabled)==0) return;
                var topCase = this._activationQueue.First(c => c.Enabled);
                //console.writeline("Top case: " + topCase);

                //verifies ongoing behavior execution
                if (this._caseInExecution == null)
                {
                    //console.writeline("No cases in execution. Executing topcase: " + topCase);
                    //no behavior being executed, execute top case
                    this.ExecuteCase(topCase);
                }
                else if (topCase.CompareTo(this._caseInExecution) > 0 && _caseInExecution.IsCancellable)
                {
                    //console.writeline("Case in execution has lower priority than topcase. Cancelling executing case: " +
                                      //_caseInExecution);
                    //if top case has higher priority, cancel ongoing and re-process queue
                    this._caseInExecution.Behavior.Cancel();
                    this.TerminateCaseExecution();
                }
            }
        }

        private void ExecuteCase(Case topCase)
        {
            lock (this._locker)
            {
                //removes "top case" from queue, attach to termination event and execute it in a new thread
                this._caseInExecution = topCase;
                this.RemoveCaseFromQueue(topCase);
                topCase.Behavior.ExecutionFinished += this.OnCaseExecuted;

                // build the features vector
                //if (Properties.Settings.Default.IsSimulationModeOn)
                //{
                //    string[] features = new string[_pool.Values.Count+1];
                //    int i = 0;
                //    foreach (Case c in _pool.Values)
                //    {
                //        features[i++] = c.Detector.IsActive?"1":"0";
                //    }
                //    features[i] = topCase.Behavior.ToString();
                //}

                ////console.writeline(">>>>>>>>> EXECUTING: " + topCase.Description);

                Task.Factory.StartNew(topCase.Execute);

                //sends case execution started event
                if (this.CaseExecutionStarted != null)
                    this.CaseExecutionStarted(this._caseInExecution);
            }
        }

        private void TerminateCaseExecution()
        {
            lock (this._locker)
            {
                //console.writeline("Terminating case execution: " + _caseInExecution);
                //sends case execution ended event
                if (this.CaseExecutionEnded != null)
                    this.CaseExecutionEnded(this._caseInExecution);

                //stops receiving terminating event for the case, free queue for other
                this._caseInExecution.Behavior.ExecutionFinished -= this.OnCaseExecuted;
                this._caseInExecution = null;
                this.ProcessQueue();
            }
        }

        #endregion

        #region Event handling 

        private void OnCaseActivationChanged(IFeatureDetector detector, bool activated)
        {
            lock (this._locker)
            {
                //console.writeline("Case Activation Changed: Detector = " + detector);
                if (!this._pool.ContainsKey(detector)) return;
                //sends case activation event
                var actCase = this._pool[detector];
                if (this.CaseActivationChanged != null)
                    this.CaseActivationChanged(actCase, activated);

                //console.writeline("Relative case found: " + actCase);

                //if active, try to add case to queue, otherwise tries to remove it
                if (activated)
                {
                    // Add the fired detector to the collector
                    if (FeaturesCollector != null)
                        FeaturesCollector.SetFeature(detector.Description, true);
                    //console.writeline("Case active! Adding it to queue: " + actCase);
                    this.AddCaseToQueue(actCase);

                    //console.writeline("Current activation queue: ");
                    foreach (var c in _activationQueue)
                    {
                        Console.Write(c);
                    }
                    //console.writeline("-----------");
                }
                else
                {
                    // Add the fired detector to the collector
                    if (FeaturesCollector != null)
                        FeaturesCollector.SetFeature(detector.Description, false);
                    if (actCase.ExecutionStarted)
                    {
                        //console.writeline(actCase +
                                          //": case inactive and behaviour already started! Removing it from queue: ");
                        this.RemoveCaseFromQueue(actCase);
                    }
                    else
                    {
                        //console.writeline(actCase + ": case inactive but behaviour not started. Leaving it in the queue: ");
                    }
                    //console.writeline("Current activation queue: " + _activationQueue.Count);
                    foreach (var c in _activationQueue)
                    {
                        Console.Write(c);
                    }
                    //console.writeline("-----------");
                }
            }
        }

        private void OnCaseExecuted(IBehavior behavior, IFeatureDetector detector)
        {
            lock (this._locker)
            {
                //verifies behavior
                if (!behavior.Equals(this._caseInExecution.Behavior)) return;

                //detaches termination event, process queue
                this.TerminateCaseExecution();
                if (detector!=null)
                    this.RemoveCaseFromQueue(this._pool[detector]);
            }
        }

        #endregion

        #region Public Methods

        public Dictionary<IFeatureDetector, Case> GetPool()
        {
            return _pool;
        }

        /// <summary>
        ///     Manually check all <see cref="Case" /> items in this pool for activation. Useful to check for initial activation in
        ///     cases whose detectors are "waiting" for events to become active.
        /// </summary>
        public void ForceCheckCasesActivation()
        {
            foreach (var caseItem in this)
                caseItem.Detector.CheckActivationChanged(true);
        }

        /// <summary>
        ///     Initiates all <see cref="Case" /> items within this <see cref="CasePool" />.
        /// </summary>
        /// <param name="perceptionClient">
        ///     the perception client used by the several <see cref="Case" /> items to detect certain
        ///     states.
        /// </param>
        /// <param name="publisher">the publisher used by the several <see cref="Case" /> items to execute behaviors.</param>
        public void Init(IAllPerceptionClient perceptionClient, IAllActionPublisher publisher)
        {
            //inits each case according to the perception client and action publisher
            foreach (var poolCase in this._pool.Values)
                poolCase.Init(perceptionClient, publisher);
        }

        public IEnumerable<IFeatureDetector> GetAllDetectors()
        {
            List<IFeatureDetector> detectors = new List<IFeatureDetector>();
            foreach (Case c in GetPool().Values)
            {
                detectors.AddRange(FindChildrenDetectors(c.Detector));
            }
            return detectors.Distinct();
        }


        public static List<IFeatureDetector> FindChildrenDetectors(IFeatureDetector det)
        {
            List<IFeatureDetector> detectors = new List<IFeatureDetector>();

            if (det is CompositeFeatureDetector)
            {
                foreach (var subDet in ((CompositeFeatureDetector) det).Detectors)
                {
                    detectors.AddRange(FindChildrenDetectors(subDet));
                }
            }
            if (det is WatcherFeatureDetector)
            {
                var subDet = ((WatcherFeatureDetector) det).WatchedDetector;
                detectors.AddRange(FindChildrenDetectors(subDet));
            }
            detectors.Add(det);
            return detectors.Distinct().ToList();
        }

        #endregion
    }
}