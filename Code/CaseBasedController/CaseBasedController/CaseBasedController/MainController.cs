using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaseBasedController.Classifier;
using CaseBasedController.Forms;
using CaseBasedController.GameInfo;
using CaseBasedController.Programs;
using CaseBasedController.Properties;
using CaseBasedController.Simulation;
using CaseBasedController.Thalamus;
using Classification.Classifier;
using EmotionalClimateClassification;

namespace CaseBasedController
{
    public class MainController
    {
        private const string EC_MODEL_DT_FILE_PATH = "Data\\J48_1_1.model";
        private const string EC_MODEL_NN_FILE_PATH = "Data\\MultilayerPerceptron_1_1.model";
        static ControllerClient _client;
        private const string EC_MODEL_SVM_FILE_PATH = "Data\\SMO_1_1.model";
        private const string EC_DATA_ARFF_FILE_PATH = "Data\\ECdata.arff";
        private static readonly List<Form> openedForms = new List<Form>();
        private CasePool _casePool;
        private ClassifierController _classifierController;
        private FeaturesCollector _fc;
        private string _loadedCasePoolPath;

        public MainController(string character)
        {
            // ====================================== KEEP UPDATED THE CASE POOL FILES
#if DEBUG
            CasePoolCodingProgram.EnercitiesDemo().SerializeToJson(@"..\..\Data\EnercitiesDemo.json");
            CasePoolCodingProgram.EnercitiesDemoEmpathic().SerializeToJson(@"..\..\Data\EnercitiesDemoEmpathic.json");
            CasePoolCodingProgram.MLPool().SerializeToJson(@"..\..\Data\MLPool.json");
            CasePoolCodingProgram.TestPool().SerializeToJson(@"..\..\Data\Test.json");
#endif

            // ====================================== INITIALIZES THALAMUS CLIENT
            _client = new ControllerClient(character);
            AIGameActionPlayer.GetInstance().Init(_client);

            

            // ====================================== INITIALIZES TIMER
            MyTimer.UseWithRealTime();

            // ====================================== INITIALIZES THE CASEPOOL 
            string lastLoadedCasePoolPath = Settings.Default.LastLoadedCasePoolPath;
            if (lastLoadedCasePoolPath != null && File.Exists(lastLoadedCasePoolPath))
                _casePool = LoadCasePool(lastLoadedCasePoolPath);
            //Task.Run( () => {
            //    Task.Delay(1000);
            //    LoadClassifierAsync(@"..\..\..\Tests\clean-data.arff", @"..\..\..\Tests\MLPool.json");
            //});

            // ====================================== INITIALIZES THE FEATURES COLLECTOR
            _fc = CreateFeaturesCollector(_casePool);


            LoadECModel();

            // ====================================== SHOWS GUI
            //if (showGUI)
            //{
            //ExcelUtil.EnableGraphics = true;
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    MainForm mainForm = new MainForm(this);
            //    openedForms.Add(mainForm);
            //    Application.Run(mainForm);
            //}
        }

        private async void LoadECModel()
        {
            await Task.Run(() => { 
                //loads EC models
                GameStatus.ECClassifiers = new List<WekaClassifier>
                    {
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH),
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH),
                        new WekaClassifier(EC_DATA_ARFF_FILE_PATH)
                    };

                GameStatus.ECClassifiers[0].Load(EC_MODEL_DT_FILE_PATH);
                GameStatus.ECClassifiers[1].Load(EC_MODEL_NN_FILE_PATH);
                GameStatus.ECClassifiers[2].Load(EC_MODEL_SVM_FILE_PATH);

                //attach OKAO event
                _client.OKAOPerceptionEvent += ClientOKAOPerceptionEvent;
            });
        }

        /// <summary>
        ///     Constructor used for batch executions. It executes a simulation and creates an ARFF based on the casePool and the
        ///     logFile
        /// </summary>
        /// <param name="character"></param>
        /// <param name="casePool"></param>
        /// <param name="logFilePath"></param>
        public MainController(string character, string casePoolPath, string logFilePath)
        {
            MyTimer.UseWithSimulationTime();
            if (!File.Exists(casePoolPath))
            {
                Console.WriteLine("File not found! " + casePoolPath);
                return;
            }
            if (!File.Exists(logFilePath))
            {
                Console.WriteLine("File not found! " + logFilePath);
                return;
            }
            CasePool casePool = LoadCasePool(casePoolPath);
            CancellationTokenSource cts = new CancellationTokenSource();
        }

        public static bool IsClassifierEnabled { get; private set; }

        private static void ClientOKAOPerceptionEvent(object sender, OKAOPerceptionArgs e)
        {
            var percept = e.PerceptionLog;
            var filter = percept.subject.ToLower().Equals("left")
                ? GameStatus.LeftSubjOkaoFilter
                : GameStatus.RightSubjOkaoFilter;

            filter.UpdateFilters(new OkaoPerception
                                 {
                                     Smile = (uint) percept.smile,
                                     SmileConfidence = (uint) percept.confidence,
                                     Anger = (uint) percept.anger,
                                     Disgust = (uint) percept.disgust,
                                     Fear = (uint) percept.fear,
                                     Sadness = (uint) percept.sadness,
                                     Surprise = (uint) percept.surprise,
                                     Joy = (uint) percept.joy,
                                     Neutral = (uint) percept.neutral,
                                     LookAtX = percept.gazeVectorX,
                                     LookAtY = percept.gazeVectorY,
                                     LookAt = percept.gazeDirection
                                 });

            GameStatus.OKAOPerceptionOccurred = true;
        }

        public void Dispose()
        {
            _client.Dispose();
            foreach (Form f in openedForms) f.Close();
        }

        public CasePool LoadCasePool(string path)
        {
            CasePool casePool;
            var mainForm = ((MainForm) openedForms.Find(f => f is MainForm));
            try
            {
                casePool = CasePool.DeserializeFromJson(path);
                casePool.Init(_client, _client.ControllerPublisher);
                if (mainForm != null) mainForm.UpdateCasePool();
                _loadedCasePoolPath = path;

                Settings.Default.LastLoadedCasePoolPath = path;
                Settings.Default.Save();

                return casePool;
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("Can't load file " + Path.GetFullPath(path), "Error loading CasePool file",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        public CasePool ReLoadCasePool()
        {
            if (_loadedCasePoolPath != null)
            {
                return LoadCasePool(_loadedCasePoolPath);
            }
            throw new Exception("No case pool previously loaded");
        }

        public async Task<bool> LoadClassifierAsync(string classifierPath, string casePoolPath)
        {
            _classifierController = new ClassifierController(_client, classifierPath, casePoolPath);
            var ret = await _classifierController.LoadAsync();

            ClassifierForm form = openedForms.Find(x => x is ClassifierForm) as ClassifierForm;
            if (form == null)
            {
                form = new ClassifierForm();
                openedForms.Add(form);
            }
            form.Show();
            form.Init(_classifierController);
            return ret;
        }

        public void EnableMainForm(bool enabled)
        {
            MainForm mf = openedForms.Find(f => f is MainForm) as MainForm;
            mf.Invoke(new Action(() => { mf.Enabled = enabled; }));
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }

        #region Getter and Setters

        public void SetCasePool(CasePool casePool)
        {
            if (_casePool != null)
                _casePool.Dispose();
            _casePool = casePool;
            var mainForm = ((MainForm) openedForms.Find(f => f is MainForm));
            if (mainForm != null) mainForm.UpdateCasePool();
        }

        public CasePool GetCasePool()
        {
            return _casePool;
        }

        public void InitCasePool()
        {
            _casePool.Init(_client, _client.ControllerPublisher);
        }

        public ControllerClient GetClient()
        {
            return _client;
        }

        #endregion

        #region Helpers

        public static FeaturesCollector CreateFeaturesCollector(CasePool cp)
        {
            if (cp == null) return null;
            List<string> featuresNames = new List<string>();
            foreach (Case c in cp)
            {
                featuresNames.Add(c.Description);
            }
            featuresNames.Add(FeaturesCollector.CLASS_TO_LEARN_NAME);
            var fc = new FeaturesCollector(featuresNames);
            cp.FeaturesCollector = fc;
            return fc;
        }

        /// <summary>
        ///     when the classifier is enabled, its results will be resolved into actions
        /// </summary>
        /// <param name="val">the "enabled" state of the classifier</param>
        public static void UseClassifier(bool val)
        {
            IsClassifierEnabled = val;
            ClassifierForm cf = openedForms.Find(f => f is ClassifierForm) as ClassifierForm;
            if (cf != null)
            {
                cf.Invoke(new Action(() => { cf.UpdateState(); }));
            }
        }

        #endregion
        public static void SkipTurn()
        {
            _client.ControllerPublisher.SkipTurn();
        }
    }
}