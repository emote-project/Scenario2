using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PhysicalSpaceManager;
using Thalamus;
using Thalamus.Actions;
using EmoteCommonMessages;
using Bend.Util;
using System.IO;
using System.Windows.Forms;
using EmoteEvents.ComplexData;
using Newtonsoft.Json;
using Skene.Utterances;
using System.Diagnostics;
using EmoteEvents;


namespace Skene
{
    
    // Using this just to help the creation and debug of new behaviors
    public interface INAOActions : IAction
    {
        void SetIdleBehavior(bool idleState);
        void EyesIntensity(double value);
        void SpeakerVolume(double value);
        void EyeBlink(int count);
        void SlowEyeBlink(int count);
    }

    public interface IFMLSpeechAux : IAction
    {
        void ReplaceTagsAndPerform(string utterance, string category);
    }

    public interface ISkene : 
        EmoteCommonMessages.IEmoteActions,
        EmoteCommonMessages.IGazeStateActions,
        EmoteCommonMessages.IPointStateActions,
        EmoteCommonMessages.IWavingStateAction,
        EmoteCommonMessages.IMapEvents,
        EmoteCommonMessages.IPerceptionEvents,
        Thalamus.BML.IGazeEvents,
        EmoteCommonMessages.ITargetEvents,
        Thalamus.BML.ISpeakDetailEvents,
        Thalamus.BML.ISpeakEvents,
        Thalamus.BML.IAnimationEvents,
        EmoteCommonMessages.ISoundLocalizationEvents,
        EmoteCommonMessages.IFMLSpeech,
        ILearnerModelIdEvents,
        Interfaces.ILibraryActions
        
    { } //subscribing

    public interface ISkenePublisher : IThalamusPublisher,
        Thalamus.BML.IAnimationActions, 
        Thalamus.BML.IBMLCodeAction, 
        Thalamus.BML.IGazeActions, 
        Thalamus.BML.IFaceActions,
        Thalamus.BML.IPostureActions, 
        Thalamus.BML.ISoundActions, 
        Thalamus.BML.ISpeakActions, 
        Thalamus.BML.IPointingActions,
        Thalamus.BML.IWavingActions,
        Thalamus.BML.IHeadActions,
        EmoteCommonMessages.IMapActions,
        EmoteCommonMessages.IApplicationActions,
        EmoteCommonMessages.IFMLSpeechEvents,
        INAOActions,
        ILearnerModelUtteranceHistoryAction,
        IFMLSpeechEventsExtras,
        Interfaces.ILibraryEvents
        //IFMLSpeechAux
    { } //publishing

    public class SkeneClient : ThalamusClient, ISkene
    {
        private int _questionsFinishEventDelaySeconds = 7;

        #region PUBLISHER
        internal class SkenePublisher : ISkenePublisher
        {
            dynamic publisher;

            SkeneClient skene;

            public SkenePublisher(dynamic publisher, SkeneClient skene)
            {
                this.publisher = publisher;
                this.skene = skene;
            }

            public void BML(string code)
            {
                publisher.BML(code);
            }

            public void Speak(string id, string text)
            {
                publisher.Speak(id, text);
            }

            public void SpeakBookmarks(string id, string[] text, string[] bookmarks)
            {
                publisher.SpeakBookmarks(id, text, bookmarks);
            }

            double lastVertical = 0;
            double lastHorizontal = 0;
            double angleThreshold = 1.0f;
            public void Gaze(string id, double horizontal, double vertical, double speed = 1, bool trackFaces = false)
            {
                //if ((Math.Abs(lastVertical-vertical)>angleThreshold) || (Math.Abs(lastHorizontal-horizontal)>angleThreshold)) {
                    publisher.Gaze(id, horizontal, vertical, speed, trackFaces);
                //}
            }

            public void Pointing(string id, string target, double speed = 1, PointingMode mode = PointingMode.RightHand)
            {
                publisher.Pointing(id, target, speed, mode);
            }

            public void PointingAngle(string id, double horizontal, double vertical, double speed = 1, PointingMode mode = PointingMode.RightHand)
            {
                publisher.PointingAngle(id, horizontal, vertical, speed, mode);
            }

            public void Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, PointingMode mode = PointingMode.RightHand)
            {
                publisher.Waving(id, horizontal, vertical, frequency, amplitude, duration, mode);
            }

            public void PlayAnimation(string id, string animation)
            {
                if (id != "") skene.RequestedAnimations[id] = animation;
                publisher.PlayAnimation(id, animation);
            }

            public void StopAnimation(string id)
            {
                publisher.StopAnimation(id);
            }

			public void Head(string id, string lexeme, int repetitions, double amplitude = 20, double frequency = 1)
			{
				publisher.Head(id, lexeme, repetitions, amplitude, frequency);
			}

			public void PlayAnimationQueued(string id, string animation)
			{
                if (id!="") skene.RequestedAnimations[id] = animation;
				publisher.PlayAnimationQueued(id, animation);
			}

            public void ResetPose()
            {
                publisher.ResetPose();
            }

            public void SetPosture(string id, string posture, double percent = 1, double decay = 1)
            {
                publisher.SetPosture(id, posture, percent, decay);
            }

            public void PlaySound(string id, string SoundName, double Volume, double Pitch)
            {
                publisher.PlaySound(id, SoundName, Volume, Pitch);
            }

            public void PlaySoundLoop(string id, string SoundName, double Volume, double Pitch)
            {
                publisher.PlaySoundLoop(id, SoundName, Volume, Pitch);
            }

            public void StopSound(string id)
            {
                publisher.StopSound(id);
            }

            public void Highlight(double x, double y)
            {
                publisher.Highlight(x, y);
            }

            public void HighlightRightAnswer()
            {
                publisher.HighlightRightAnswer();
            }

			public void Click()
            {
                publisher.Click();
            }

			public void Pan()
            {
                publisher.Pan();
            }

			public void Zoom()
            {
                publisher.Zoom();
            }

            public void SetIdleBehavior(bool idleState)
            {
                publisher.SetIdleBehavior(idleState);
            }

            public void ToolAction(string toolName, string toolAction)
            {
                publisher.ToolAction(toolName, toolAction);
            }

            #region ISpeakActions Members


            public void SpeakStop()
            {
                publisher.SpeakStop();
            }

            #endregion

            public void FaceLexeme(string id, string lexeme)
            {
                publisher.FaceLexeme(id, lexeme);
            }

            public void FaceShiftLexeme(string id, string lexeme)
            {
                publisher.FaceShiftLexeme(id, lexeme);
            }

            public void FaceFacs(string id, int AU, Side Side, double Intensity)
            {}

            public void NeutralFaceExpression()
            {
                publisher.NeutralFaceExpression();
            }
            public void RunAction(string actionName, string parameters)
            {
 	            publisher.RunAction(actionName, parameters);
            }

            public void UtteranceFinished(string id)
            {
                skene.NotifyUtteranceState(UtteranceStates.StandBy);
                publisher.UtteranceFinished(id);
            }

            public void UtteranceStarted(string id)
            {
                publisher.UtteranceStarted(id);
            }

            public void UtterancesTagInfo(string tagName, string tagValue)
            {
                publisher.UtterancesTagInfo(tagName, tagValue);
            }

            #region INAOActions Members


            public void EyesIntensity(double value)
            {
                publisher.EyesIntensity(value);
            }

            public void SpeakerVolume(double value)
            {
                publisher.SpeakerVolume(value);
            }

            public void EyeBlink(int count)
            {
                publisher.EyeBlink(count);
            }

            public void SlowEyeBlink(int count)
            {
                publisher.SlowEyeBlink(count);
            }

            #endregion


            #region IFMLSpeechAux Members

            public void ReplaceTagsAndPerform(string utterance, string category)
            {
                publisher.ReplaceTagsAndPerform(utterance, category);
            }

            #endregion

            public void UtteranceUsed(string id,string Utterance_utterance)
            {
                publisher.UtteranceUsed(id,Utterance_utterance);
            }

            public void UtteranceIsAQuestion(string id)
            {
                publisher.UtteranceIsAQuestion(id);
            }

            public void LibraryList(string[] libraries)
            {
                publisher.LibraryList(libraries);
            }

            public void LibraryChanged(string serialized_LibraryContents)
            {
                publisher.LibraryChanged(serialized_LibraryContents);
            }


            public void Utterances(string library, string category, string subcategory, string[] utterances)
            {
                publisher.Utterances(library, category, subcategory, utterances);
            }
        }
        #endregion

        #region delegates and events

        public delegate void PersonAngleChangedHandler(int userId, Vector2D angle);
        public event PersonAngleChangedHandler PersonAngleChanged;
        private void NotifyPersonAngleChanged(int userId, Vector2D angle)
        {
            if (PersonAngleChanged != null) PersonAngleChanged(userId, angle);
        }

        public delegate void PersonGazeDirectionChangedHandler(int userId, EmoteCommonMessages.GazeEnum direction);
        public event PersonGazeDirectionChangedHandler PersonGazeDirectionChanged;
        private void NotifyPersonGazeDirectionChanged(int userId, EmoteCommonMessages.GazeEnum direction)
        {
            if (PersonGazeDirectionChanged != null) PersonGazeDirectionChanged(userId, direction);
        }

        public delegate void ClickPointChangedHandler(Vector2D point);
        public event ClickPointChangedHandler ClickPointChanged;
        private void NotifyClickPointChanged(Vector2D point)
        {
            if (ClickPointChanged != null) ClickPointChanged(point);
        }

        public delegate void ScreenPointChangedHandler(Vector2D point);
        public event ScreenPointChangedHandler ScreenPointChanged;
        private void NotifyPointAngleChanged(Vector2D point)
        {
            if (ScreenPointChanged != null) ScreenPointChanged(point);
        }

        public delegate void PersonVisibleChangedHandler(int userId, bool visible);
        public event PersonVisibleChangedHandler PersonVisibleChanged;
        private void NotifyPersonVisibleChanged(int userId, bool visible)
        {
            if (PersonVisibleChanged != null) PersonVisibleChanged(userId, visible);
        }

        public delegate void TargetsChangedHandler();
        public event TargetsChangedHandler TargetsChanged;
        private void NotifyTargetsChanged()
        {
            if (TargetsChanged != null) TargetsChanged();
        }

        public enum UtteranceStates
        {
            StandBy,
            Performing,
            QuestionWaiting,
            NVBWaiting,
            Canceled
        }


        public delegate void UtteranceStateHandler(UtteranceStates state, Utterance utt);
        public event UtteranceStateHandler UtteranceState;
        private void NotifyUtteranceState(UtteranceStates state, Utterance utt = null)
        {
            if (UtteranceState != null) UtteranceState(state, utt);
        }

        #endregion

        #region fields and properties

        internal SkenePublisher SkPublisher;
        internal GazeManager GazeManager { get; private set; }
        internal IdleManager IdleManager { get; private set; }
        public PhysicalSpace ActiveSpaceSetup { get; set; }
        internal bool IsPersonVisible { get; private set; }

        internal Dictionary<int, Vector2D> PersonLocation { get; private set; }
        internal Vector2D LastTouchOnScreenCoords { get; private set; }
        internal Dictionary<int, EmoteCommonMessages.GazeEnum> PersonGazeDirection { get; private set; }

        private double yTrackingCompensation = Properties.Settings.Default.YTrackingCompensation;
        internal double YTrackingCompensation
        {
            get { return yTrackingCompensation; }
            set {
                yTrackingCompensation = value;
                Properties.Settings.Default.YTrackingCompensation = value;
                Properties.Settings.Default.Save();
            }
        }
        private double zTrackingCompensation = Properties.Settings.Default.ZTrackingCompensation;
        internal double ZTrackingCompensation
        {
            get { return zTrackingCompensation; }
            set
            {
                zTrackingCompensation = value;
                Properties.Settings.Default.ZTrackingCompensation = value;
                Properties.Settings.Default.Save();
            }
        }

        internal Dictionary<string, TargetInfo> Targets { get; private set; }
        private Dictionary<string, List<Action>> targetsQueue = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<string>> targetsUtterances = new Dictionary<string, List<string>>();
        private Utterance performingUtterance;  // The utterance being performed at the moment
        int bookmarkWordInterval = 2;

        private Dictionary<string, string> requestedAnimations = new Dictionary<string, string>();
        public Dictionary<string, string> RequestedAnimations
        {
            get { return requestedAnimations; }
        }
        private Dictionary<string, string> runningAnimations = new Dictionary<string, string>();
        public Dictionary<string, string> RunningAnimations
        {
            get { return runningAnimations; }
        }

        private Dictionary<string, UtteranceLibrary> utteranceLibraries = new Dictionary<string, UtteranceLibrary>();
        public Dictionary<string, UtteranceLibrary> UtteranceLibraries
        {
            get { return utteranceLibraries; }
        }

        public bool isAutoIdle = false;
        private Thread httpThread;
        private Thread utterancesQueueThread;
        MyHttpServer httpServer;
        bool backchanneledCurrentQuestion = false;
        bool backchannellingCurrentQuestion = false;
        ActiveUser currentlyActiveSpeaker = ActiveUser.None;
        bool waitingForQuestionAnswer = false;
        Stopwatch utteranceFinishedTimeoutClock = new Stopwatch();

        private string backchannelingCategory = Properties.Settings.Default.BackchannelingCategory;
        public string BackchannelingCategory
        {
            get { return backchannelingCategory; }
            set {
                backchannelingCategory = value;
                Properties.Settings.Default.BackchannelingCategory = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool UseCompositeLibrary = Properties.Settings.Default.UseCompositeLibrary;
        public string SelectedLibrary = Properties.Settings.Default.SelectedLibrary;


        private bool stripSlashes = Properties.Settings.Default.StripSlashes;
        public bool StripSlashes
        {
            get { return stripSlashes; }
            set
            {
                stripSlashes = value;
                Properties.Settings.Default.StripSlashes = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool useRelativeSpeed = Properties.Settings.Default.UseUtteranceRelativeSpeed;
        public bool UseRelativeSpeed
        {
            get { return useRelativeSpeed; }
            set { 
                useRelativeSpeed = value;
                Properties.Settings.Default.UseUtteranceRelativeSpeed = useRelativeSpeed;
                Properties.Settings.Default.Save();
            }
        }

        private float utterancesSafeDuration = Properties.Settings.Default.UtterancesSafeDurationSeconds;
        public float UtterancesSafeDuration
        {
            get { return utterancesSafeDuration; }
            set
            {
                utterancesSafeDuration = Math.Max(0, value);
                Properties.Settings.Default.UtterancesSafeDurationSeconds = utterancesSafeDuration;
                Properties.Settings.Default.Save();
            }
        }

        private bool useUtterancesSafeDuration = Properties.Settings.Default.UseUtterancesSafeDuration;
        public bool UseUtterancesSafeDuration
        {
            get { return useUtterancesSafeDuration; }
            set
            {
                useUtterancesSafeDuration = value;
                Properties.Settings.Default.UseUtterancesSafeDuration = useUtterancesSafeDuration;
                Properties.Settings.Default.Save();
            }
        }

        private int relativeSpeed = Properties.Settings.Default.UtteranceRelativeSpeed;
        public int RelativeSpeed
        {
          get { return relativeSpeed; }
          set { 
              relativeSpeed = Math.Max(0, Math.Min(100, value));
              Properties.Settings.Default.UtteranceRelativeSpeed = relativeSpeed;
              Properties.Settings.Default.Save();
          }
        }

        private int utteranceTimeoutMs = Properties.Settings.Default.UtteranceTimeoutMs;
        public int UtteranceTimeoutMs
        {
            get { return utteranceTimeoutMs; }
            set
            {
                utteranceTimeoutMs = Math.Max(100, value);
                Properties.Settings.Default.UtteranceTimeoutMs = utteranceTimeoutMs;
                Properties.Settings.Default.Save();
            }
        }

        UtteranceValidationSet utteranceValidationSet;
        public UtteranceValidationSet UtteranceValidationSet
        {
            get { return utteranceValidationSet; }
            set
            {
                utteranceValidationSet = value;
                Properties.Settings.Default.UtteranceValidationSet = value.Filename;
                Properties.Settings.Default.Save();
            }
        }

        private Dictionary<string, Skene.UtteranceValidationSet> utteranceValidationSets;
        public Dictionary<string, Skene.UtteranceValidationSet> UtteranceValidationSets
        {
            get { return utteranceValidationSets; }
        }

        public int QuestionsFinishEventDelaySeconds
        {
            get { return _questionsFinishEventDelaySeconds; }
            set { 
                _questionsFinishEventDelaySeconds = value;
                Properties.Settings.Default.QuestionsFinishEventDelaySeconds = value;
                Properties.Settings.Default.Save();
            }
        }

        private UtteranceValidator ttsValidator =  new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "pau",
                                                            "rspd"
        }));

        private UtteranceValidator nonTargetInstructionsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "ANIMATE",
                                                            "HEADNOD",
                                                            "HEADNODNEGATIVE",
                                                            "GAME",
                                                            "FACE",
                                                            "FACESHIFT",
                                                            "EYEBLINK",
                                                            "SLOWEYEBLINK"
        }));
        private UtteranceValidator targetInstructionsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "GAZEANDPOINTANDHIGHLIGHT",
                                                            "GLANCEANDPOINTANDHIGHLIGHT",
                                                            "GLANCEANDWAVEANDHIGHLIGHT",
                                                            "GAZEANDPOINT",
                                                            "GAZEANDWAVE",
                                                            "GAZEANDHIGHLIGHT",
                                                            "GLANCEANDPOINT",
                                                            "GLANCEANDWAVE",
                                                            "GLANCEANDHIGHLIGHT",
                                                            "POINTANDHIGHLIGHT",
                                                            "WAVEANDHIGHLIGHT",
                                                            "GAZE",
                                                            "GLANCE",
                                                            "POINT",
                                                            "WAVE",
                                                            "HIGHLIGHT",
        }));

        private UtteranceValidator animationsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "diecticEast",
                                                            "diecticEastNorthEast",
                                                            "diecticEastSouthEast",
                                                            "diecticNorth",
                                                            "diecticNorthEast",
                                                            "diecticNorthNorthEast",
                                                            "diecticNorthNorthWest",
                                                            "diecticNorthWest",
                                                            "diecticSouth",
                                                            "diecticSouthEast",
                                                            "diecticSouthSouthEast",
                                                            "diecticSouthSouthWest",
                                                            "diecticSouthWest",
                                                            "diecticWest",
                                                            "diecticWestNorthWest",
                                                            "diecticWestSouthWest",
                                                            "enthusiasm",
                                                            "goodBye",
                                                            "greeting",
                                                            "happy",
                                                            "iconicCompassPoints",
                                                            "iconicCross",
                                                            "metaphoricDichotomicLeft",
                                                            "metaphoricDichotomicRight",
                                                            "metaphoricRevelation",
                                                            "metaphoricScaleDown",
                                                            "metaphoricScaleUp",
                                                            "metaphoricWorld",
                                                            "pointToOther",
                                                            "pointToOtherAndSelf",
                                                            "pointToSelf",
                                                            "pointToSelfAndOther",
                                                            "PointToSelf",
                                                            "PointPlayerLeft",
                                                            "PointPlayerRight",
                                                            "sadness",
                                                            "sadnessNodding",
                                                            "thinking",
                                                            "waitingGesture",
                                                            "supermanThinking",
                                                            "ohNo",
                                                            "rodinThinking",
                                                            "libra"
        }), true);
        private UtteranceValidator tagsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "currentPlayerName",
                                                            "ecoName",
                                                            "envName",
                                                            "currentLevel",
                                                            "totalScore",
                                                            "environmentScore",
                                                            "economyScore",
                                                            "mayorScore",
                                                            "currentOil",
                                                            "currentEnergy",
                                                            "currentMoney",
                                                            "upgrade",
                                                            "policy",
                                                            "structure",
                                                            "structureCategory",
                                                            "currentPlayerSide",
                                                            "currentPlayerRole",
                                                            "tutorRole",
                                                            "level",
                                                            "player1Role",
                                                            "player2Role",
                                                            "artigoDefinidoEco",
                                                            "artigoDefinidoEnv",
                                                            "artigoDefinidoCurrentPlayer",
                                                            "artigoDefinidoNextPlayer"
        }));
        private UtteranceValidator targetsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "mayGUI",
                                                            "ecoGUI",
                                                            "envGUI",
                                                            "currentPlayerGUI",
                                                            "targetCell",
                                                            "Eco",
                                                            "Env",
                                                            "clicks",
                                                            "throughMap",
                                                            "globalGUI",
                                                            "Economist",
                                                            "Environmentalist",
                                                            "mapCenter",
                                                            "sea",
                                                            "ownGUI",
                                                            "skipCurrentPlayer"
        }));
        private UtteranceValidator faceExpressionsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "neutral",
                                                            "shut",
                                                            "happiness",
                                                            "anger",
                                                            "sadness",
                                                            "fear",
                                                            "surprise",
                                                            "disgust"
        }));

        private UtteranceValidator gameInstructionsValidator = new UtteranceValidator(new List<string>(new string[] 
        {
                                                            "ShowBuildMenuTooltip",
                                                            "CloseBuildMenuTooltip",
                                                            "SelectBuildingMenuTool",
                                                            "UnselectBuildingMenuTool",
                                                            "ShowPoliciesMenu",
                                                            "ClosePoliciesMenu",
                                                            "ShowPolicyTooltip",
                                                            "ClosePolicyTooltip",
                                                            "ShowUpgradesMenu",
                                                            "CloseUpgradesMenu",
                                                            "ShowUpgradeTooltip",
                                                            "CloseUpgradeTooltip",
                                                            "PreviewBuildCell"
        }));


        #endregion

        #region constructors and starters

        private static SkeneClient _instance;

        public static SkeneClient GetInstance(string characterName = "")
        {
            return _instance ?? (_instance = new SkeneClient(characterName));
        }

        private SkeneClient(string characterName = "")
            : base("Skene", characterName) 
        {
            this.Targets = new Dictionary<string, TargetInfo>();
            this.PersonGazeDirection = new Dictionary<int, GazeEnum>();
            this.PersonLocation = new Dictionary<int, Vector2D>();
            this.PersonLocation[0] = new Vector2D(-30, 0);
            this.PersonLocation[1] = new Vector2D(30, 0);
            GazeManager = new GazeManager(this);
            IdleManager = new IdleManager(this);
            SetPublisher<ISkenePublisher>();
            SkPublisher = new SkenePublisher(Publisher, this);

            _questionsFinishEventDelaySeconds = Properties.Settings.Default.QuestionsFinishEventDelaySeconds;

            this.NewClientConnected += ((name, newClientId) => {if (name.Contains("NAO")) RedetectMutualGaze();});
            this.ClientConnected += (() => RedetectMutualGaze());

            LoadUtteranceValidators();
            if (utteranceValidationSets.Count > 0)
            {
                if (utteranceValidationSets.ContainsKey(Properties.Settings.Default.UtteranceValidationSet)) utteranceValidationSet = utteranceValidationSets[Properties.Settings.Default.UtteranceValidationSet];
                else
                {
                    foreach(UtteranceValidationSet uvs in utteranceValidationSets.Values) 
                    {
                        UtteranceValidationSet = uvs;
                    }
                }
                Debug("Using Utterance Validation Set: '{0}'", Path.GetFileNameWithoutExtension(utteranceValidationSet.Filename));
            }
            else
            {
                utteranceValidationSet = new UtteranceValidationSet(targetInstructionsValidator, nonTargetInstructionsValidator, animationsValidator, gameInstructionsValidator, tagsValidator, targetsValidator, faceExpressionsValidator, ttsValidator);
                Debug("Using Default Utterance Validation Set.");
            }

            SetTarget(GazeTarget.Person);
            SetTarget(GazeTarget.Person2);
            SetTarget(GazeTarget.Person3);
            SetTarget(GazeTarget.Person4);
            SetTarget(GazeTarget.Clicks);
            SetTarget(GazeTarget.Random);
            SetTarget(GazeTarget.ThroughMap);
            SetTarget(GazeTarget.AcrossRoom);

            SetAngleTarget("topLeft", new Vector2D(-30, 30));
            SetAngleTarget("middleLeft", new Vector2D(-30, 0));
            SetAngleTarget("bottomLeft", new Vector2D(-30, -20));
            SetAngleTarget("topFront", new Vector2D(0, 30));
            SetAngleTarget("middleFront", new Vector2D(0, 0));
            SetAngleTarget("bottomFront", new Vector2D(0, -20));
            SetAngleTarget("topRight", new Vector2D(30, 30));
            SetAngleTarget("middleRight", new Vector2D(30, 0));
            SetAngleTarget("bottomRight", new Vector2D(30, -20));

            SetTarget(SpeakerRapport.Speaker.LeftSpeaker.ToString(), GazeTarget.Person2.ToString());
            SetTarget(SpeakerRapport.Speaker.RightSpeaker.ToString(), GazeTarget.Person.ToString());

            LoadUtteranceLibraries();

            httpServer = new MyHttpServer(2729);
            httpServer.PostRequest += ((requestName, requestData) => this.PerformUtterance("", new Utterance(requestData.Replace("/", "")) ));
            httpServer.Targets = Targets.Keys.ToList();
            httpServer.Instructions = new List<string>() { "<GAZEANDPOINTANDHIGHLIGHT(target)>",
                                                            "<GLANCEANDPOINTANDHIGHLIGHT(target)>",
                                                            "<GLANCEANDWAVEANDHIGHLIGHT(target)>",
                                                            "<GAZEANDPOINT(target)>",
                                                            "<GAZEANDWAVE(target)>",
                                                            "<GAZEANDHIGHLIGHT(target)>",
                                                            "<GLANCEANDPOINT(target)>",
                                                            "<GLANCEANDWAVE(target)>",
                                                            "<GLANCEANDHIGHLIGHT(target)>",
                                                            "<POINTANDHIGHLIGHT(target)>",
                                                            "<WAVEANDHIGHLIGHT(target)>",
                                                            "<GAZE(target)>",
                                                            "<GLANCE(target)>",
                                                            "<POINT(target)>",
                                                            "<WAVE(target)>",
                                                            "<HIGHLIGHT(target)>",
                                                            "<ANIMATE(animationName)>",
                                                            "<HEADNOD(count)>",
                                                            "<HEADNODNEGATIVE(count)>",
                                                            "<GAME(gameAction)>",
                                                            "<FACE(expression)>",
                                                            "<FACESHIFT(expression)>",
                                                            "<EYEBLINK(count)>",
                                                            "<SLOWEYEBLINK(count)>"};
            httpThread = new Thread(new ThreadStart(httpServer.listen));
            httpThread.Start();

            utterancesQueueThread = new Thread(new ThreadStart(UtteranceQueueCycle));
            utterancesQueueThread.Start();

        }

        internal void LoadUtteranceLibraries()
        {
            bool found = false;
            string uttLibPath = CorrectPath(GetCorrectDirectory(Properties.Settings.Default.UtteranceLibrariesDirectory));
            if (Directory.Exists(uttLibPath))
            {
                utteranceLibraries = new Dictionary<string, UtteranceLibrary>();
                string[] extensions = new[] { ".xls", ".xlsx" };
                DirectoryInfo di = new DirectoryInfo(uttLibPath);
                FileInfo[] rgFiles = di.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                foreach (FileInfo fi in rgFiles)
                {
                    try
                    {
                        DebugIf("all", "Load Utterance Library '" + fi.FullName + "'");
                        utteranceLibraries[Path.GetFileNameWithoutExtension(fi.FullName)] = new UtteranceLibrary();
                        utteranceLibraries[Path.GetFileNameWithoutExtension(fi.FullName)].LoadLibrary(fi.FullName,
                            delegate(string message)
                            {
                                MessageBox.Show(message,"Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            } );
                        found = true;
                    }
                    catch (Exception e)
                    {
                        DebugException(e);
                    }
                }
            }
            if (!found)
            {
                System.Windows.Forms.MessageBox.Show("No utterances libraries found in " + System.Environment.NewLine + Path.GetFullPath(uttLibPath), "No utterances",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }


        }

        private string GetCorrectDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                string[] dirSplit = path.Split(Path.DirectorySeparatorChar);
                path = ".\\";
                foreach (string s in dirSplit)
                {
                    if (s != "." && s != "..") path += s + "\\";
                }
            }
            return path;
        }

        internal void LoadUtteranceValidators()
        {
            string validatorsDirectory = CorrectPath(GetCorrectDirectory(Properties.Settings.Default.UtteranceValidatorsDirectory));

            utteranceValidationSets = new Dictionary<string, UtteranceValidationSet>();
            if (Directory.Exists(validatorsDirectory))
            {
                string[] extensions = new[] { ".suvs"};
                DirectoryInfo di = new DirectoryInfo(validatorsDirectory);
                FileInfo[] rgFiles = di.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                foreach (FileInfo fi in rgFiles)
                {
                    try
                    {
                        if (File.Exists(fi.FullName))
                        {
                            Debug("all", "Loading Utterance Validation Set '" + fi.FullName + "'");
                            utteranceValidationSets[Path.GetFileNameWithoutExtension(fi.FullName)] = new UtteranceValidationSet(fi.FullName);
                        }
                        else
                        {
                            DebugError("Utterance Validation Set file '{0}' does not exist!", fi.FullName);
                        }
                    }
                    catch (Exception e)
                    {
                        DebugException(e);
                    }
                }
            }
        }

        public override void ConnectedToMaster()
        {
            GazeManager.Start();
        }

        public override void Dispose()
        {
            base.Dispose();
            GazeManager.Dispose();
            IdleManager.Dispose();
            httpServer.Dispose();
            httpThread.Abort();
            utterancesQueueThread.Abort();
        }

        #endregion

        #region Idle management

        public void SetIdleBehavior(bool state)
        {    
            IdleManager.IdleState = state;
            //BehaviorPublisher.SetIdleBehavior(!state);
        }

        #endregion

        #region Start, Stop, Reset

        void IEmoteActions.Start(string StartMessageInfo_info)
        {
            if (StartEvent != null)
                StartEvent(this,
                    new StartEventArgs()
                    {
                        startInfo = StartMessageInfo.DeserializeFromJson<StartMessageInfo>(StartMessageInfo_info)
                    });
        }

        void IEmoteActions.Reset()
        {
        }

        void IEmoteActions.Stop()
        {
        }

        #endregion

        #region ISpeakDetailEvents

        void Thalamus.BML.ISpeakDetailEvents.Bookmark(string id)
        {
            List<Action> actions = new List<Action>();
            Debug("Bookmark: " + id);
            utteranceFinishedTimeoutClock.Restart();
            if (targetsQueue.ContainsKey(id)) 
            {
                actions = targetsQueue[id];
                targetsQueue.Remove(id);
            }
            foreach (Action a in actions)
            {
                a();
            }
            NotifyUtteranceState(UtteranceStates.Performing);
        }

        void Thalamus.BML.ISpeakDetailEvents.Viseme(int viseme, int nextViseme, double visemePercent, double nextVisemePercent)
        {
        }

        #endregion

        #region Gazing

        void EmoteCommonMessages.IGazeStateActions.GazeAtScreen(double x, double y)
        {
            if (ActiveSpaceSetup != null)
            {
                GazeManager.Gaze(new TargetInfo(new Vector2D(x, y)));
            }
        }

        void EmoteCommonMessages.IGazeStateActions.GlanceAtScreen(double x, double y)
        {
            if (ActiveSpaceSetup != null)
            {
                GazeManager.Glance(new TargetInfo(new Vector2D(x, y)));
            }
        }

        void IGazeStateActions.GazeAtTarget(string targetName)
        {
            if (Targets.ContainsKey(targetName.ToLower())) Targets[targetName.ToLower()].Generate(this, TargetType.Gaze);
        }

        void IGazeStateActions.GlanceAtTarget(string targetName)
        {
            if (Targets.ContainsKey(targetName.ToLower())) Targets[targetName.ToLower()].Generate(this, TargetType.Glance);
        }

        #endregion

        #region Pointing

        public void PointAtScreen(string id, double x, double y)
        {
            if (ActiveSpaceSetup != null)
            {
                Vector2D angles = ActiveSpaceSetup.PointToScreenPoint(x, y);
                SkPublisher.PointingAngle(id, angles.X, angles.Y, 0.5, ActiveSpaceSetup.IsAtRobotRight(x, y) ? Thalamus.Actions.PointingMode.RightHand : Thalamus.Actions.PointingMode.LeftHand);
                SkPublisher.Highlight(x, y);
            }
        }

        void IPointStateActions.PointAtScreen(string id, double x, double y)
        {
            PointAtScreen(id, x, y);
        }

        void IPointStateActions.PointAtTarget(string targetName)
        {
            if (Targets.ContainsKey(targetName)) Targets[targetName].Generate(this, TargetType.Pointing);
        }

        #endregion

        #region Waving

        public void WaveAtScreen(string id, double x, double y, double amplitude, double frequency, double duration)
        {
            if (ActiveSpaceSetup != null)
            {
                Vector2D angles = ActiveSpaceSetup.PointToScreenPoint(x, y);
                SkPublisher.Waving(id, angles.X, angles.Y, frequency, amplitude, duration, ActiveSpaceSetup.IsAtRobotRight(x, y) ? Thalamus.Actions.PointingMode.RightHand : Thalamus.Actions.PointingMode.LeftHand);
            }
        }

        void IWavingStateAction.WaveAtTarget(string targetName)
        {
            if (Targets.ContainsKey(targetName)) Targets[targetName].Generate(this, TargetType.Waving);
        }

        void IWavingStateAction.WaveAtScreen(string id, double x, double y, double amplitude, double frequency, double duration)
        {
            WaveAtScreen(id, x, y, amplitude, frequency, duration);
        }

        #endregion

        #region IGazeEvents Members

        void Thalamus.BML.IGazeEvents.GazeFinished(string id)
        {
            GazeManager.GazeFinished(id);
        }

        void Thalamus.BML.IGazeEvents.GazeStarted(string id)
        {
            GazeManager.GazeStarted(id);
            if (GazeManager.LastGazeTarget == GazeTarget.Person ||
                GazeManager.LastGazeTarget == GazeTarget.Person2 ||
                GazeManager.LastGazeTarget == GazeTarget.Person3 ||
                GazeManager.LastGazeTarget == GazeTarget.Person4)
            {
                currentGazePerson = GazeManager.LastGazeTarget;
                RedetectMutualGaze();
            }
            else
            {
                currentGazePerson = GazeTarget.None;
                RedetectMutualGaze();
            }
        }

        #endregion

        #region Target management

        void EmoteCommonMessages.ITargetEvents.TargetScreenInfo(string targetName, int X, int Y)
        {
            SetScreenTarget(targetName, new Vector2D(X, Y));
        }

        void EmoteCommonMessages.ITargetEvents.TargetLink(string targetName, string linkedTargetName)
        {
            SetTarget(targetName, linkedTargetName);
        }

        internal void SetScreenTarget(string targetName, Vector2D coordinates)
        {
            lock (Targets)
            {
                Targets[targetName.ToLower()] = new TargetInfo(coordinates, targetName, GazeTarget.ScreenPoint);
            }
            NotifyTargetsChanged();
        }

        internal void SetAngleTarget(string targetName, Vector2D coordinates)
        {
            lock (Targets)
            {
                Targets[targetName.ToLower()] = new TargetInfo(coordinates, targetName, GazeTarget.Angle);
            }
            NotifyTargetsChanged();
        }
        internal void SetTarget(GazeTarget target)
        {
            lock (Targets)
            {
                Targets[target.ToString().ToLower()] = new TargetInfo(target);
            }
            NotifyTargetsChanged();
        }
		internal void SetTarget(string targetName, GazeTarget target)
        {
            lock (Targets)
            {
                Targets[targetName.ToLower()] = new TargetInfo(targetName, target);
            }
            NotifyTargetsChanged();
        }
        internal void SwitchTargetName(string oldTargetName, string newTargetName)
        {
            if (!Targets.ContainsKey(oldTargetName.ToLower()))
            {
                DebugError("Targets does not contain target '{0}'!", oldTargetName);
            }
            else if (Targets.ContainsKey(newTargetName.ToLower())) 
            {
                DebugError("Targets already contains target '{0}'!", newTargetName);
            }
            else
            {
                lock (Targets)
                {
                    TargetInfo ti = Targets[oldTargetName.ToLower()];
                    ti.TargetName = newTargetName;
                    Targets.Remove(oldTargetName.ToLower());
                    Targets.Add(newTargetName.ToLower(), ti);
                }
            }
        }

        internal void SetTarget(string targetName, string linkedTargetName)
        {
            if (Targets.ContainsKey(linkedTargetName))
            {
                lock (Targets)
                {
                    Targets[targetName.ToLower()] = new TargetInfo(targetName, linkedTargetName, this);
                }
                NotifyTargetsChanged();
            }
            else if (Targets.ContainsKey(linkedTargetName.ToLower()))
            {
                lock (Targets)
                {
                    Targets[targetName.ToLower()] = new TargetInfo(targetName, linkedTargetName.ToLower(), this);
                }
                NotifyTargetsChanged();
            } 
            else
            {
                DebugError("Unable to link target '{0}' to '{1}' because target '{1}' does not exist!", targetName, linkedTargetName);
            }
        }

        internal void UnlinkTarget(string targetName) 
        {
            if (Targets.ContainsKey(targetName) || Targets.ContainsKey(targetName.ToLower()))
            {
                if (!Targets.ContainsKey(targetName)) targetName = targetName.ToLower();
                if (Targets[targetName].Linked)
                {
                    Targets[targetName].Coordinates = Targets[Targets[targetName].LinkedTargetName].Coordinates;
                    Targets[targetName].GazeTarget = Targets[Targets[targetName].LinkedTargetName].GazeTarget;
                    Targets[targetName].Linked = false;
                }
            }
            else
            {
                DebugError("Unable to unlink target '{0}' because target '{0}' does not exist!", targetName);
            }
        }

        internal void QueueTarget(string bookmark, Action targetInfo, string utteranceId)
        {
            if (utteranceId != "")
            {
                if (!targetsUtterances.ContainsKey(utteranceId)) targetsUtterances[utteranceId] = new List<string>();
                targetsUtterances[utteranceId].Add(bookmark);
            }
            if (!targetsQueue.ContainsKey(bookmark))
            {
                lock (targetsQueue)
                {
                    targetsQueue[bookmark] = new List<Action>();
                }
            }
            if (!targetsQueue[bookmark].Contains(targetInfo)) targetsQueue[bookmark].Add(targetInfo);
        }

        #endregion

        #region Utterances

        private void FinishedUtterance(string id, bool immediately = false) 
        {
            lock(targetsUtterances) {
                lock(targetsQueue) {
                    if (id!="") 
                    {
                        if (targetsUtterances.ContainsKey(id))
                        {
                            foreach (string targetId in targetsUtterances[id])
                            {
                                if (targetsQueue.ContainsKey(targetId)) targetsQueue.Remove(targetId);
                            }
                            targetsUtterances.Remove(id);
                        }
                    }
                    else
                    {
                        foreach(string utteranceId in targetsUtterances.Keys) {
                            foreach (string targetId in targetsUtterances[utteranceId])
                            {
                                if (targetsQueue.ContainsKey(targetId)) targetsQueue.Remove(targetId);
                            }
                        }
                        targetsUtterances.Clear();
                    }
                }
            }
            // Delaying the utterance finished message in case the last utterance was a question
            // TODO: wait for one of the players to say something before sending the utterance finished event. In case no one is speaking, than use a timeout to send the event.
            if (!immediately && performingUtterance != null && performingUtterance.IsQuestion)
            {
                waitingForQuestionAnswer = true;
                NotifyUtteranceState(UtteranceStates.QuestionWaiting);
                (new Thread(new ParameterizedThreadStart(DelayedUtteranceFinished))).Start(id);
            }
            else if (performingUtterance != null && (performingUtterance.TextArray.Length == 0 || performingUtterance.TextArray[0].Trim() == ""))
            {
                NotifyUtteranceState(UtteranceStates.NVBWaiting);
                (new Thread(new ParameterizedThreadStart(DelayedNVBUtteranceFinished))).Start(id);
            }
            else
            {
                waitingForQuestionAnswer = false;
                SkPublisher.UtteranceFinished(id);
                performingUtterance = null;
                Debug("Finished Utterance");
            }
            
            
        }

        private void DelayedNVBUtteranceFinished(object oid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Debug("Waiting for NVB...");
            while (timer.ElapsedMilliseconds < utterancesSafeDuration * 1000) Thread.Sleep(200);
            string id = (string)oid;
            SkPublisher.UtteranceFinished(id);
            performingUtterance = null;
            Debug("Finished Utterance");
        }

        
        private void DelayedUtteranceFinished(object oid)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Debug("Waiting for answer...");
            while (waitingForQuestionAnswer && timer.ElapsedMilliseconds < _questionsFinishEventDelaySeconds * 1000) Thread.Sleep(200);
            if (waitingForQuestionAnswer) Debug("Sick of waiting.");
            else Debug("Got answer.");
            string id = (string)oid;
            SkPublisher.UtteranceFinished(id);
            performingUtterance = null;
            waitingForQuestionAnswer = false;
            Debug("Finished Utterance");
        }

        public void CancelUtterance(string id = "")
        {
            if (id == "" && performingUtterance != null) id = performingUtterance.ThalamusId;
            NotifyUtteranceState(UtteranceStates.Canceled);
            if (targetsUtterances.ContainsKey(id))
            {
                FinishedUtterance(id, true);
            }
            else FinishedUtterance("", true);
            //SkPublisher.PlayAnimation("", "idle");
            SkPublisher.SpeakStop();
        }

        public void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
        {
            utteranceLibraries.Keys.ToArray();
            if (tagNames.Length != tagValues.Length) DebugError("tagNames count differs from tagValues count! These arrays should have the same number of elements.");

            Utterance utterance = null;
            if (UseCompositeLibrary)
            {
                utterance = UtteranceLibrary.GetCompositeUtterance(utteranceLibraries.Values.ToList(), category, subcategory);
            }
            else
            {
                if (utteranceLibraries.ContainsKey(SelectedLibrary)) utterance = utteranceLibraries[SelectedLibrary].GetUtterance(category, subcategory);
            }
            if (utterance==null) utterance = new Utterance("");
            utterance.ThalamusId = id;
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (tagNames[i].Length>0)
                    utterance.Text = utterance.Text.ToLower().Replace(tagNames[i].ToLower(), tagValues[i].ToLower());
            }

            PerformUtterance(id, utterance);
        }

        public bool VerifyUtterance(Utterance utterance)
        {
            try
            {
                return PerformUtterance("", utterance, false) != "";
            }
            catch
            {
                return false;
            }
        }

        public string PerformUtterance(string id, Utterance utterance, string[] tagNames, string[] tagValues, bool execute = true)
        {
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (tagNames[i].Length > 0)
                    utterance.Text = utterance.Text.ToLower().Replace(tagNames[i].ToLower(), tagValues[i].ToLower());
            }
            return PerformUtterance(id, utterance);
        }

        public string PerformUtterance(string id, Utterance utterance, bool execute = true)
        {
            if (utterance == null || utterance.Text == null || utterance.Text.Trim() == "")
            {
                lock (utterancesQueue)
                {
                    Utterance u = new Utterance("");
                    u.ThalamusId = id;
                    utterancesQueue.Enqueue(u);
                    Debug("Queued Empty Utterance");
                }
                return "";
            }

            if (stripSlashes)
            {
                utterance.Text = utterance.Text.Replace('/', ' ');
            }

            string fmlSpeech = utterance.Text;
            if (UseRelativeSpeed) fmlSpeech = String.Format("\\rspd={0}\\ {1}", relativeSpeed.ToString(), fmlSpeech);

            if (id == "") id = Guid.NewGuid().ToString();
            utterance.ThalamusId = id;
            targetsUtterances[id] = new List<string>();
            Regex re = new Regex(@"(?<=>)|(?=<)");
            string[] parts = re.Split(fmlSpeech);
            bool textStarted = false;
            List<String> text = new List<string>();
            List<String> bookmarks = new List<string>();
            string currentBookmark = String.Format("{0:X8}BM_{1}", fmlSpeech.GetHashCode(), 0); ;
            int bookmarkCount = 1;
			int animateCount = 0;
            int headCount = 0;
            int faceCount = 0;
            Action toAddGaze = null;
            Action toAddNonGaze = null;
            bool useNonGaze = false;
            int i = 0;
            List<Action> actions = new List<Action>();

            #region Tags replacement and filling
            foreach (string p in parts)
            {
                i++;
                string block = p.Trim();
                if (block.Length == 0) continue;
                if (block.StartsWith("<") && block.EndsWith(">") && block.Contains("(") && block.Contains(")") && !block.StartsWith("<prosody"))
                {
                    int start = block.IndexOf("(") + 1;
                    string correctParam = block.Substring(start, block.LastIndexOf(")") - start);
                    string param = correctParam.ToLower();
                    block = block.ToUpper().Trim();

                    if (block.StartsWith("<GAZEANDPOINTANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Gaze);
                            toAddNonGaze = () => Targets[param].Generate(this, TargetType.Gaze, true);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "GAZE", param);
                            Debug("Queued <{0}({1})>", "POINT", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCEANDPOINTANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                            Debug("Queued <{0}({1})>", "POINT", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCEANDWAVEANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Waving)));
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                            Debug("Queued <{0}({1})>", "WAVE", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<GAZEANDPOINT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Gaze);
                            toAddNonGaze = () => Targets[param].Generate(this, TargetType.Gaze, true);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            Debug("Queued <{0}({1})>", "GAZE", param);
                            Debug("Queued <{0}({1})>", "POINT", param);
                        }
                    }
                    else if (block.StartsWith("<GAZEANDWAVE")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Gaze);
                            toAddNonGaze = () => Targets[param].Generate(this, TargetType.Gaze, true);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Waving)));
                            Debug("Queued <{0}({1})>", "GAZE", param);
                            Debug("Queued <{0}({1})>", "WAVE", param);
                        }
                    }
                    else if (block.StartsWith("<GAZEANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Gaze);
                            toAddNonGaze = () => Targets[param].Generate(this, TargetType.Gaze, true);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "GAZE", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCEANDPOINT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                            Debug("Queued <{0}({1})>", "POINT", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCEANDWAVE")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Waving)));
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                            Debug("Queued <{0}({1})>", "WAVE", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCEANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<POINTANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            actions.Add((() => Targets[param].Generate(this, TargetType.Waving)));
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "WAVE", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<WAVEANDHIGHLIGHT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "POINT", param);
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else if (block.StartsWith("<GAZE")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Gaze);
                            toAddNonGaze = () => Targets[param].Generate(this, TargetType.Gaze, true);
                            Debug("Queued <{0}({1})>", "GAZE", param);
                        }
                    }
                    else if (block.StartsWith("<GLANCE")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            if (toAddGaze != null) useNonGaze = true;
                            toAddGaze = () => Targets[param].Generate(this, TargetType.Glance);
                            Debug("Queued <{0}({1})>", "GLANCE", param);
                        }
                    }
                    else if (block.StartsWith("<POINT")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            actions.Add((() => Targets[param].Generate(this, TargetType.Pointing)));
                            Debug("Queued <{0}({1})>", "POINT", param);
                        }
                    }
                    else if (block.StartsWith("<WAVE")) //param = target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            actions.Add((() => Targets[param].Generate(this, TargetType.Waving)));
                            Debug("Queued <{0}({1})>", "WAVE", param);
                        }
                    }
                    else if (block.StartsWith("<ANIMATE")) //param = animationName
                    {
                        correctParam.Replace(" ", string.Empty);
                        actions.Add((() => SkPublisher.PlayAnimation(String.Format("{0:X8}ANIMATE_{1}", fmlSpeech.GetHashCode(), animateCount++), correctParam)));
                        Debug("Queued <{0}({1})>", "ANIMATE", correctParam);
                    }
                    else if (block.StartsWith("<HEADNODNEGATIVE")) //param = count
                    {
                        int count = 2;
                        if (param.Length > 0) try { count = int.Parse(param, ifp); }
                            catch { }
                        actions.Add((() => SkPublisher.Head(String.Format("{0:X8}HEAD_{1}", fmlSpeech.GetHashCode(), headCount++), "SHAKE", count, 10, 2)));
                        Debug("Queued <{0}({1})>", "HEADNODNEGATIVE", param);
                    }
                    else if (block.StartsWith("<HEADNOD")) //param = count
                    {
                        int count = 2;
                        if (param.Length > 0) try { count = int.Parse(param, ifp); }
                            catch { }
                        actions.Add((() => SkPublisher.Head(String.Format("{0:X8}HEAD_{1}", fmlSpeech.GetHashCode(), headCount++), "NOD", count, 10, 2)));
                        Debug("Queued <{0}({1})>", "HEADNOD", param);
                    }
                    else if (block.StartsWith("<FACE")) //param = lexeme
                    {
                        toAddGaze = () => SkPublisher.FaceLexeme(String.Format("{0:X8}FACE_{1}", fmlSpeech.GetHashCode(), faceCount++), param); ;
                        Debug("Queued <{0}({1})>", "FACE", param);
                    }
                    else if (block.StartsWith("<FACESHIFT")) //param = lexeme
                    {
                        toAddGaze = () => SkPublisher.FaceShiftLexeme(String.Format("{0:X8}FACESHIFT_{1}", fmlSpeech.GetHashCode(), faceCount++), param); ;
                        Debug("Queued <{0}({1})>", "FACESHIFT", param);
                    }
                    else if (block.StartsWith("<EYEBLINK")) //param = count
                    {
                        int count = 1;
                        if (param.Length > 0) try { count = int.Parse(param, ifp); }
                            catch { }
                        toAddGaze = () => SkPublisher.EyeBlink(count); ;
                        Debug("Queued <{0}({1})>", "EYEBLINK", count);
                    }
                    else if (block.StartsWith("<SLOWEYEBLINK")) //param = count
                    {
                        int count = 1;
                        if (param.Length > 0) try { count = int.Parse(param, ifp); }
                            catch { }
                        toAddGaze = () => SkPublisher.SlowEyeBlink(count); ;
                        Debug("Queued <{0}({1})>", "SLOWEYEBLINK", count);
                    }
                    else if (block.StartsWith("<GAME")) //param = game action
                    {
                        Debug("Queued <{0}({1})>", "GAME", correctParam);
                        string actionName = correctParam.Substring(0, correctParam.IndexOf(',') == -1 ? correctParam.Length : correctParam.IndexOf(','));
                        string actionParam = "";
                        if (correctParam.IndexOf(',') != -1) actionParam = correctParam.Substring(correctParam.IndexOf(',') + 1, correctParam.Length - correctParam.IndexOf(',') - 1);
                        actions.Add((() => SkPublisher.RunAction(actionName, actionParam)));
                    }
                    else if (block.StartsWith("<HIGHLIGHT")) //param = screen target
                    {
                        if (Targets.ContainsKey(param))
                        {
                            actions.Add((() => Targets[param].Generate(this, TargetType.Highlight)));
                            Debug("Queued <{0}({1})>", "HIGHLIGHT", param);
                        }
                    }
                    else
                    {
                        DebugError("Invalid instruction '{0}' in utterance '{1}'!", block, text);
                        return "";
                    }

                    if (execute)
                    {
                        //depois de cada tag
                        if (textStarted)
                        {
                            Debug("bookmark: " + currentBookmark + " block: " + block);
                            foreach (Action a in actions)
                            {
                                QueueTarget(currentBookmark, a, id);
                            }
                        }
                        else
                        {
                            foreach (Action a in actions) a();
                        }
                    }
                    actions.Clear();
                }
                else
                {
                    //just a text block
                    if (textStarted)
                    {
                        if (execute)
                        {
                            if (useNonGaze && toAddNonGaze != null) QueueTarget(currentBookmark, toAddNonGaze, id);
                            if (toAddGaze != null) QueueTarget(currentBookmark, toAddGaze, id);
                        }
                        if (i < parts.Length)
                        {
                            bookmarks.Add(currentBookmark);
                            currentBookmark = String.Format("{0:X8}BM_{1}", fmlSpeech.GetHashCode(), bookmarkCount++);
                        }
                        
                    }
                    else
                    {
                        if (execute)
                        {
                            if (useNonGaze && toAddNonGaze != null) toAddNonGaze();
                            if (toAddGaze != null) toAddGaze();
                        }
                    }
                    useNonGaze = false;
                    toAddGaze = null;
                    toAddNonGaze = null;
                    
                    //break the block into sequences of 3 words and keep a random bookmark inbetween them so that we have a constant status on the text being spoken.
                    string[] blockSplit = block.Split(' ');
                    //block = "";
                    for (int j = 0; j < blockSplit.Length; j++)
                    {
                        block = blockSplit[j] + " ";
                        for (int m = 1; m < bookmarkWordInterval && j+m<blockSplit.Length; m++)
                        {
                            block += blockSplit[j+m] + " ";
                        }
                        j += bookmarkWordInterval-1;
                        text.Add(block);
                        
                        currentBookmark = String.Format("{0:X8}BM_{1}", fmlSpeech.GetHashCode(), bookmarkCount++);
                        bookmarks.Add(currentBookmark);
                    }

                    //text.Add(block);
                    textStarted = true;
                }
            }
#endregion

            if (execute)
            {
                if (textStarted)
                {
                    if (useNonGaze && toAddNonGaze != null) QueueTarget(currentBookmark, toAddNonGaze, id);
                    if (toAddGaze != null) QueueTarget(currentBookmark, toAddGaze, id);
                }
                else
                {
                    if (useNonGaze && toAddNonGaze != null) toAddNonGaze();
                    if (toAddGaze != null) toAddGaze();
                }
                bookmarks.Add(currentBookmark);

                //if (a_text.Length>0) a_text[a_text.Length-1]+="  \\rst\\";
                utterance.SetTextBookmarks(text.ToArray(), bookmarks.ToArray());

                lock (utterancesQueue)
                {
                    utterancesQueue.Enqueue(utterance);
                    Utterances.HistoryManager.HistoryManagerFactory.GetHistoryManager().AddToHistory(utterance.ThalamusId, utterance);
                    Debug("Queued Utterance");
                }

                /*if (currentlyActiveSpeaker == ActiveUser.None)
                {
                    Debug("My turn to speak.");
                    
                }
                else
                {
                    Debug("Being polite...");
                    politeUtterance = utterance;
                }*/
            }
            return id;
        }

        private void ExecuteUtterance(Utterance utterance)
        {
            if (utterance == null || utterance.Text == null || utterance.Text.Trim() == "") {
                performingUtterance = new Utterance();
                SkPublisher.UtteranceStarted(utterance.ThalamusId);
                SkPublisher.UtteranceFinished(utterance.ThalamusId);
                performingUtterance = null;
            }else
            {
                backchanneledCurrentQuestion = false;
                backchannellingCurrentQuestion = false;
                utteranceFinishedTimeoutClock.Restart();
                performingUtterance = utterance;
                NotifyUtteranceState(UtteranceStates.Performing, utterance);
                if (utterance.IsQuestion) SkPublisher.UtteranceIsAQuestion(utterance.ThalamusId);
                SkPublisher.SpeakBookmarks(utterance.ThalamusId, utterance.TextArray, utterance.BookmarkArray);
                
            //Utterances.HistoryManager.HistoryManagerFactory.GetHistoryManager().AddToHistory(utterance.ThalamusId, utterance);     // This works here, but it's not good practice to have it here. Move this code in a better place when refactoring the class
            }
        }


        Queue<Utterance> utterancesQueue = new Queue<Utterance>();

        private void UtteranceQueueCycle()
        {
            while (!Shutingdown)
            {
                if ((utterancesQueue.Count > 0 && currentlyActiveSpeaker == ActiveUser.None && performingUtterance == null))
                {
                    Utterance utterance;
                    /*if (backChannelUtterance != null)
                    {
                        Debug("Launching Backchannel Utterance");
                        utterance = backChannelUtterance;
                        backChannelUtterance = null;
                        waitingForQuestionAnswer = false;
                    }
                    else
                    {*/
                    Debug("Launching Queued Utterance");
                    lock (utterancesQueue)
                    {
                        utterance = utterancesQueue.Dequeue();
                    }
                    //}
                    ExecuteUtterance(utterance);
                }
                if (performingUtterance != null && utteranceFinishedTimeoutClock.ElapsedMilliseconds > utteranceTimeoutMs && !waitingForQuestionAnswer)
                {
                    CancelUtterance();
                }
                Thread.Sleep(100);
            }
        }
        #endregion

        #region MapEvents (Click, Pan, Zoom)

        void IMapEvents.Click(double x, double y)
        {
            LastTouchOnScreenCoords = new Vector2D(x, y);
            NotifyClickPointChanged(LastTouchOnScreenCoords);
            GazeManager.Click(LastTouchOnScreenCoords);
        }

        void IMapEvents.Pan(double x, double y, double prevX, double prevY)
        {
            NotifyClickPointChanged(LastTouchOnScreenCoords);
            GazeManager.Click(LastTouchOnScreenCoords);
            LastTouchOnScreenCoords = new Vector2D((x + prevX) / 2, (y + prevY) / 2);
        }

        void IMapEvents.Zoom(double[] finger0, double[] finger1, double[] previousFinger0, double[] previousFinger1) 
        {
        }
        
        #endregion

        #region BML synchronization - Animation, Speak

        void Thalamus.BML.IAnimationEvents.AnimationFinished(string id)
        {
            if (runningAnimations.ContainsKey(id)) runningAnimations.Remove(id);
            GazeManager.AnimationFinished(id);
            IdleManager.AnimationFinished(id);
        }

        void Thalamus.BML.IAnimationEvents.AnimationStarted(string id)
        {
            if (requestedAnimations.ContainsKey(id))
            {
                runningAnimations[id] = requestedAnimations[id];
                requestedAnimations.Remove(id);
            }
            
            IdleManager.AnimationStarted(id);
        }

        void Thalamus.BML.ISpeakEvents.SpeakStarted(string id) 
        {
            if (targetsUtterances.ContainsKey(id))
            {
                SkPublisher.UtteranceStarted(id);
            }
        }
        void Thalamus.BML.ISpeakEvents.SpeakFinished(string id)
        {
            if (targetsUtterances.ContainsKey(id)) FinishedUtterance(id);
        }

        #endregion

        #region mutual gaze

        GazeTarget currentGazePerson = GazeTarget.None;

        bool mutualGaze = false;

        private void RedetectMutualGaze()
        {
            if ((PersonGazeDirection.ContainsKey(1) && currentGazePerson == GazeTarget.Person && PersonGazeDirection[1] == GazeEnum.Robot) ||
                (PersonGazeDirection.ContainsKey(2) && currentGazePerson == GazeTarget.Person2 && PersonGazeDirection[2] == GazeEnum.Robot) ||
                (PersonGazeDirection.ContainsKey(3) && currentGazePerson == GazeTarget.Person3 && PersonGazeDirection[3] == GazeEnum.Robot) ||
                (PersonGazeDirection.ContainsKey(4) && currentGazePerson == GazeTarget.Person4 && PersonGazeDirection[4] == GazeEnum.Robot))
            {
                if (!mutualGaze)
                {
                    //start mutual gaze behaviour
                    SkPublisher.EyesIntensity(1);
                }
                mutualGaze = true;
            }
            else
            {
                if (mutualGaze)
                {
                    //end mutual gaze behaviour
                    SkPublisher.EyesIntensity(0.3);
                }
                mutualGaze = false;
            }
        }

        #endregion

        #region PerceptionEvents


        void IPerceptionEvents.OKAOMessage(int userID, double smile, double confidence, double anger, double disgust, double fear, double joy, double sadness, double surprise, double neutral, string gazeDirection)
        {
        }

        void IPerceptionEvents.QSensorMessage(int userID, double Z, double Y, double X, double Temp, double EDA)
        {
        }

        void ISoundLocalizationEvents.ActiveSoundUser(ActiveUser userAct, double LeftValue, double RightValue)
        {
            currentlyActiveSpeaker = userAct;
            Debug("Floor: " + currentlyActiveSpeaker);
            if (performingUtterance != null && performingUtterance.IsQuestion && !backchanneledCurrentQuestion && backchannelingCategory != "")
            {
                if (userAct != ActiveUser.None) backchannellingCurrentQuestion = true;
                else if (userAct == ActiveUser.None && backchannellingCurrentQuestion)
                {
                    string[] s_backchannelingCategory = backchannelingCategory.Split(':');
                    string category = s_backchannelingCategory[0];
                    string subcategory = s_backchannelingCategory.Length > 1 ? s_backchannelingCategory[1] : "-";
                    Utterance backChannelUtterance = null;
                    if (UseCompositeLibrary)
                    {
                        backChannelUtterance = UtteranceLibrary.GetCompositeUtterance(utteranceLibraries.Values.ToList(), category, subcategory);
                    }
                    else
                    {
                        if (utteranceLibraries.ContainsKey(SelectedLibrary)) backChannelUtterance = utteranceLibraries[SelectedLibrary].GetUtterance(category, subcategory);
                    }
                    if (backChannelUtterance != null)
                    {
                        backchanneledCurrentQuestion = true;

                        lock (utterancesQueue)
                        {
                            Queue<Utterance> q = new Queue<Utterance>(utterancesQueue);
                            utterancesQueue.Clear();
                            //utterancesQueue.Enqueue(backChannelUtterance);
                            PerformUtterance("", backChannelUtterance);
                            foreach (Utterance u in q)
                            {
                                utterancesQueue.Enqueue(u);
                                Utterances.HistoryManager.HistoryManagerFactory.GetHistoryManager().AddToHistory(u.ThalamusId, u);
                            }
                        }
                        waitingForQuestionAnswer = false;
                    }
                }
            }
        }

        void IPerceptionEvents.EyebrowsAU(double au2_user1, double au4_user1, double au2_user2, double au4_user2)
        {
            
        }
        void IPerceptionEvents.PointingPosition(int userID, EmoteCommonMessages.Hand hand, double X, double Y, double Z)
        {
        }

        void IPerceptionEvents.UserMutualGaze(bool value)
        {
        }

        void IPerceptionEvents.UserMutualPoint(bool value, double avegX, double avegY)
        {
        }

        void IPerceptionEvents.EyebrowsAU2(double au4left_user1, double au4right_user1, double au4left_user2, double au4right_user2)
        {
        }

        void IPerceptionEvents.UserTouchChin(int userID, bool value)
        {
        }

        public void Smile(int UserID, int smileVal, int confienceVal)
        {
            
        }

        void IPerceptionEvents.GazeTracking(int userID, EmoteCommonMessages.GazeEnum direction, int ConfidenceVal)
        {
            if (!PersonGazeDirection.ContainsKey(userID) || PersonGazeDirection[userID] != direction)
            {
                PersonGazeDirection[userID] = direction;
                NotifyPersonGazeDirectionChanged(userID, direction);
                RedetectMutualGaze();
            }
        }

        void EmoteCommonMessages.IPerceptionEvents.HeadTracking(int userID, double X, double Y, double Z, bool DetectedSkeleton)
        {
            userID -= 1;
            if (DetectedSkeleton)
            {
                Z += ZTrackingCompensation / 100;
                Y += YTrackingCompensation / 100;
                PersonLocation[userID] = new Vector2D(-Math.Atan2(X, Z) * (180 / Math.PI), Math.Atan2(Y, Z) * (180 / Math.PI));
                if (userID == 0)
                {
                    SetAngleTarget("env", PersonLocation[userID]);
                    SetAngleTarget("environmentalist", PersonLocation[userID]);
                }
                else if (userID == 1)
                {
                    SetAngleTarget("eco", PersonLocation[userID]);
                    SetAngleTarget("economist", PersonLocation[userID]);
                }
            }
            if ((IsPersonVisible && !DetectedSkeleton) || (!IsPersonVisible && DetectedSkeleton)) NotifyPersonVisibleChanged(userID, DetectedSkeleton);
            IsPersonVisible = DetectedSkeleton;
            if (IsPersonVisible) NotifyPersonAngleChanged(userID, PersonLocation[userID]);
        }

        #endregion

        #region IFMLSpeech Members

        void IFMLSpeech.CancelUtterance(string id)
        {
            CancelUtterance(id);
        }

        void IFMLSpeech.PerformUtterance(string id, string utterance, string category)
        {
            PerformUtterance(id, new Utterance(utterance));
        }

        void IFMLSpeech.PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames, string[] tagValues)
        {
            PerformUtteranceFromLibrary(id, category, subcategory, tagNames, tagValues);
        }

        #endregion

        #region Events

        public class UtteranceHistoryReceivedEventArgs : EventArgs
        {
            public string studentID { get; set; }
            public List<LightUtteranceHistoryItem> UtterancesHistory { get; set; }
        }
        public event EventHandler<UtteranceHistoryReceivedEventArgs> UtteranceHistoryReceivedEvent;

        public class StartEventArgs : EventArgs
        {
            public StartMessageInfo startInfo;
        }
        public event EventHandler<StartEventArgs> StartEvent;

        #endregion

        #region Learner Model

        void IEmoteActions.SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            Console.WriteLine("WARNING! MESSAGE NOT MANAGED! SetLearnerInfo(string LearnerInfo_learnerInfo) (LearnerInfo_learnerInfo = " + LearnerInfo_learnerInfo + ")");
        }

        void ILearnerModelIdEvents.nextThalamusId(int participantId)
        {
        }

        void ILearnerModelIdEvents.allLearnerInfo(string[] LearnerInfo_learnerInfos)
        {
        }

        void ILearnerModelIdEvents.allUtterancesForParticipant(int participantId, string[] Utterance_utterances)
        {
            Utterances.HistoryManager.HistoryManagerFactory.GetHistoryManager();    // Initializes it

            if (UtteranceHistoryReceivedEvent == null) return;
            if (Utterance_utterances == null) return;

            List<LightUtteranceHistoryItem> utterances = Utterance_utterances.Select(JsonSerializable.DeserializeFromJson<LightUtteranceHistoryItem>).ToList();
            UtteranceHistoryReceivedEvent(this,
                new UtteranceHistoryReceivedEventArgs()
                {
                    UtterancesHistory = utterances
                });
        }

        #endregion


        #region LibraryActions

        void Interfaces.ILibraryActions.ChangeLibrary(string newLibrary)
        {
            if (UtteranceLibraries.ContainsKey(newLibrary))
            {
                SelectedLibrary = newLibrary;
                SkPublisher.LibraryChanged((new Interfaces.LibraryInfo(newLibrary, UtteranceLibraries[newLibrary].Categories)).SerializeToJson());
            }
            
        }

        void Interfaces.ILibraryActions.GetLibraries()
        {
            SkPublisher.LibraryList(utteranceLibraries.Keys.ToArray<string>());
        }

        void Interfaces.ILibraryActions.GetUtterances(string category, string subcategory)
        {
            if (SelectedLibrary != "" && UtteranceLibraries.ContainsKey(SelectedLibrary)) 
            {
                List<string> utts = new List<string>();
                foreach(Utterance u in utteranceLibraries[SelectedLibrary].FilterUtterances(category, subcategory)) 
                {
                    utts.Add(u.Text);
                }
                SkPublisher.Utterances(SelectedLibrary, category, subcategory, utts.ToArray());
            }
        }

        #endregion
    }
}
