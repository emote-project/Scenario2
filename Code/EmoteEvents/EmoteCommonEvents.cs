using Thalamus;

namespace EmoteCommonMessages
{
    /*
	 * When the user interacts with the application
	 * and causes things to happen, 
	 * the application publishes messages tagged as Events (IPerception).
	 * 
	 * The robot can then interact with the application 
	 * by publishing messages tagged as Actions (IAction).
	 */

    public enum LearnerModelUpdateReason
    {
        StepAnswerAttempt, //this is if the learner model update is triggered by a step answer attempt. 
        AffectUpdate,
        // this is if the reason for the learner model update is due to a change in the affect perception module and not triggered by the task.
        ToolUse,
        // not a step answer attempt but due to a tool being used, this won't be sent at the moment but will do it soon. 
        Summary,
        Wrapup
    }

    public enum GazeEnum
    {
        Robot,
        ScreenLeft,
        ScreenRight,
        Other,
        None
    }

    public enum ActiveUser
    {
        Left,
        Right,
        Both,
        None
    }

    public enum GazeTarget
    {
        Random,
        Person,
        Clicks,
        ScreenPoint,
        Angle,
        ThroughMap,
        AcrossRoom,
        None,
        Person2,
        Person3,
        Person4
    }

    public enum TargetType
    {
        Gaze,
        Glance,
        Pointing,
        Waving,
        Highlight
    }

    public enum Hand
    {
        lefthand,
        righthand
    }

    public enum CompetencyType
    {
        numerical,
        impact
    }

    public enum EvidenceType
    {
        numerical,
        impact,
        strategyWeight,
        resourceWeight,
        toolUse,
        endGame,
        roll,
        levelReached, //7
        turnChanged, //8
        takeTurn, //9
        distance, //10
        direction, //11
        symbol, //12
        crisis, //13
        emotionalClimateLevel //14
    }

    public enum Delta
    {
        decrease,
        increase,
        same,
        fluctuation,
        nocomparision
        /*decrease,0
            increase,1
            same,2
            fluctuation,3
            nocomparision4 */
    }


    public enum Impact
    {
        Positive,
        Neutral,
        Negative
    }

    public enum Charge
    {
        Positive,
        Neutral,
        Negative
    }

    public enum PointofFocus
    {
        Social,
        Task,
        NonApplicable,
        Other
    }

    public enum AffectPerceptionState
    {
        Valence,
        Arousal,
        Engagement,
        Attention
    }

    public enum Probes
    {
        Social,
        Task2Robot,
        Robot2Task
    }

    public enum EmotionalClimateLevel
    {
        Positive, Negative
    }

    public interface IEmotionalClimate : IPerception
    {
        void EmotionalClimateLevel(EmotionalClimateLevel level);
    }

    public interface ITargetEvents : IAction
    {
        //use this to create or update targets for gazing/pointing/waving towards a specific point on the screen
        void TargetScreenInfo(string targetName, int X, int Y);
        void TargetLink(string targetName, string linkedTargetName);
    }

    public interface IGazeStateActions : IAction
    {
        /// <summary>
        ///     Gaze to a point on the screen
        /// </summary>
        /// <param name="x">the coordinate value relative to the vertical axis of the screen (the one in front of the robot)</param>
        /// <param name="z">
        ///     the coordinate value relative to the horizzontal axis of the screen (the one going at the sides of the
        ///     robot)
        /// </param>
        void GazeAtScreen(double x, double y);

        void GlanceAtScreen(double x, double y);
        void GazeAtTarget(string targetName);
        void GlanceAtTarget(string targetName);
    }

    public interface IPointStateActions : IAction
    {
        void PointAtScreen(string id, double x, double y);
        void PointAtTarget(string targetName);
    }

    public interface IWavingStateAction : IAction
    {
        void WaveAtTarget(string targetName);
        void WaveAtScreen(string id, double x, double y, double amplitude = 5, double frequency = 4, double duration = 2);
    }

    public interface IFMLSpeech : IAction
    {
        //The id is used in case you need to listen for the UtteranceStarted and UtteranceFinished events.
        //If you don't need it, id can be ""
        //The tagNames and TagValues should have the same dimension, 
        //and in case you have tags that should be replaced in the utterance (like 'playerName'), 
        //then the name and value should correspond to the same index in the arrays
        void PerformUtterance(string id, string utterance, string category);

        void PerformUtteranceFromLibrary(string id, string category, string subcategory, string[] tagNames,
            string[] tagValues);

        void CancelUtterance(string id);
    }

    public interface IFMLSpeechEvents : IPerception
    {
        void UtteranceStarted(string id);
        void UtteranceFinished(string id);
    }

    public interface IFMLSpeechEventsExtras : IPerception
    {
        /// <summary>
        ///     Fired when the utterance being spoken is a question.
        /// </summary>
        /// <param name="id">The id of the relative utterance, which is a question</param>
        void UtteranceIsAQuestion(string id);
    }

    public interface IEmoteActions : IAction
    {
        void Start(string StartMessageInfo_info);
        void Stop();
        void Reset();
        void SetLearnerInfo(string LearnerInfo_learnerInfo);
    }

    public interface IMapEvents : IPerception
    {
        void Click(double x, double y);
        // User clicking on a point on the screen. x and y representing the coordinates of the point in pixels
        void Zoom(double[] finger0, double[] finger1, double[] finger0Start, double[] finger1Start);
        // each array contains the x and the y coordinate of each finger. The "start" represents the coordinates of the location where zoom gesture started
        void Pan(double x, double y, double startX, double startY);
    }

    public interface IMapActions : IAction
    {
        void Click();
        void Zoom();
        void Pan();
        void Highlight(double x, double y);
        void HighlightRightAnswer();
    }

    public interface IApplicationActions : IAction
    {
        void RunAction(string actionName, string parameters);
    }

    public interface ITaskEvents : IPerception
    {
        void StartActivity(string[] playerNames);
        void EndActivity();
    }

    public interface IMenuEvents : IPerception
    {
        void StartGameShow();
        void StartGameHide();
        void EndOfLevelShow();
        void EndOfLevelHide();
        void EndOfGameShow();
        void EndOfGameHide();
    }

    public interface IPerceptionEvents : IPerception
    {
        void GazeTracking(int userID, GazeEnum direction, int ConfidenceVal);
        void HeadTracking(int userID, double X, double Y, double Z, bool DetectedSkeleton);
        //removed
        //void GazeOKAO(int userID,bool gazeAtRobot);
        /// <summary>
        ///     Sends information about the Animation Unit detected on the face of the two users
        ///     For more information: http://msdn.microsoft.com/en-us/library/jj130970.aspx
        ///     NOTE: user 1 is the one that, facing towards the camera, stands on the left of it; user 2 the one on the right. (In
        ///     enercities: user 1: economist, user 2: environmentalist)
        ///     If a scenario with just one user is used than only user 1 values are used.
        /// </summary>
        /// <param name="au2User1">
        ///     Outer Brow Raiser: 0=neutral, -1=fully lowered as a very sad face, +1=raised as in an expression
        ///     of deep surprise
        /// </param>
        /// <param name="au4User1">
        ///     Brow Lowerer: 0=neutral, -1=raised almost all the way, +1=fully lowered (to the limit of the
        ///     eyes)
        /// </param>
        /// <param name="au2User2">as for user 1</param>
        /// <param name="au4User2">as for user 1</param>
        void EyebrowsAU(double au2User1, double au4User1, double au2User2, double au4User2);

        void PointingPosition(int userID, Hand hand, double X, double Y, double Z);
        //for publishing the position of the handtip
        void UserMutualGaze(bool value);
        void UserMutualPoint(bool value, double avegX, double avegY);
        void EyebrowsAU2(double au4LeftUser1, double au4RightUser1, double au4LeftUser2, double au4RightUser2);
        void UserTouchChin(int userID, bool value);

        void OKAOMessage(int userID, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, string gazeDirection);

        // All of these values come directly from the OKAO module without filtering apart the last value: gazeDirection which is labelled after processing and indicates the direction of user's gaze
        // userID 1 or 2
        // For the values below: 0 means the least possible expression and 100 the highest.
        // smile: The Smile degree ranges from 0 to 100, where 0 indicates a frowning face and 100 indicates a full smile
        // confidence: 0-999 indicates the confidence of the system for picking up the smile.
        // anger, disgust, fear, joy, sadness, surprise and neutral are the seven expressions picked up from okao and start from 0 to 100
        // gazeDirection: Gaze at robot, Gaze at screenL, Gaze at screenR, Gaze elsewhere
        void QSensorMessage(int userID, double Z, double Y, double X, double Temp, double EDA);
    }

    public class PerceptionLog : IPerception
    {
        public double anger;
        public double closeRatioLeftEye;
        public double closeRatioRightEye;
        public double confidence;
        public double disgust;
        public double eyesLeftRight;
        public double eyesUpdown;
        public double faceLeftRightDegrees;
        public double faceUpDownDegrees;
        public double fear;
        public string gazeDirection;
        public double gazeVectorX;
        public double gazeVectorY;
        public double headPositionX;
        public double headPositionY;
        public double joy;
        public double neutral;
        public double sadness;
        public double smile;
        public double surprise;
        /*
         *  09 March 2015
         */
        public double time;

        public PerceptionLog(double time, double faceUpDownDegrees, double faceLeftRightDegrees, double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY,
            string gazeDirection)
        {
            this.time = time;
            this.faceUpDownDegrees = faceUpDownDegrees;
            this.faceLeftRightDegrees = faceLeftRightDegrees;
            this.eyesUpdown = eyesUpdown;
            this.eyesLeftRight = eyesLeftRight;
            this.headPositionY = headPositionY;
            this.headPositionX = headPositionX;
            this.closeRatioLeftEye = closeRatioLeftEye;
            this.closeRatioRightEye = closeRatioRightEye;
            this.smile = smile;
            this.confidence = confidence;
            this.anger = anger;
            this.disgust = disgust;
            this.fear = fear;
            this.joy = joy;
            this.sadness = sadness;
            this.surprise = surprise;
            this.neutral = neutral;
            this.gazeVectorX = gazeVectorX;
            this.gazeVectorY = gazeVectorY;
            this.gazeDirection = gazeDirection;
        }
    }

    public class OKAOScenario2Perception : PerceptionLog
    {
        /// <summary>
        ///     Left / Right subject identification. Useful for emotional climate in scenario 2.
        /// </summary>
        public string subject;

        public OKAOScenario2Perception(double time, double faceUpDownDegrees, double faceLeftRightDegrees,
            double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY,
            string gazeDirection, string subject)
            : base(
                time, faceUpDownDegrees, faceLeftRightDegrees, eyesUpdown, eyesLeftRight, headPositionY, headPositionX,
                closeRatioLeftEye, closeRatioRightEye, smile, confidence, anger, disgust, fear, joy, sadness, surprise,
                neutral, gazeVectorX, gazeVectorY, gazeDirection)
        {
            this.subject = subject;
        }
    }

    public interface ISoundLocalizationEvents : IPerception
    {
        void ActiveSoundUser(ActiveUser userAct, double LeftValue, double RightValue);
    }

    public interface ISpeechDetectionEvents : IPerception
    {
        void WordDetected(string[] words);
    }

    public interface IAffectPerceptionEvents : IPerception
    {
        // Confidences are provided in the range of 0-1000
        //  
        void UserState(string AffectPerceptionInfo_AffectiveStates);
        void ProbeRequest(Probes type, int urgency);
    }

    public interface ILearnerModelToAffectPerceptionEvents : IPerception
    {
        //This event is fired after the learner model is updated with task information, but before the latest affect perception module is available.  affect perception is the main subscriber. 
        void learnerModelValueUpdateBeforeAffectPerceptionUpdate(string LearnerStateInfo_learnerState);
    }

    public interface ILearnerModelToS2IMPerceptionEvents : IPerception
    {
        //This event is fired after the learner model is updated with task information, but before the latest affect perception module is available.  affect perception is the main subscriber. 
        void learnerModelValueUpdate(string LearnerStateInfo_learnerState);
    }

    public interface ILearnerModelToIMEvents : IPerception
    {
        //This event is fired after the learner model is updated with task information, and after the latest affect perception module is available. This is intended for the IM. 
        void learnerModelValueUpdateAfterAffectPerceptionUpdate(string LearnerStateInfo_learnerState,
            string AffectPerceptionInfo_AffectiveStates);
    }

    public interface ILearnerModelIdActions : IAction
    {
        void getNextThalamusId(); //Request for an empty thalamus id.
        void getAllLearnerInfo(); //Request for all existing student info
        void getAllUtterancesForParticipant(int participantId);
    }

    public interface ILearnerModelUtteranceHistoryAction : IAction
    {
        /// <summary>
        ///     Comunicates when an utterance was used to the Learner Model. The LM will keep track of the used utterances in each
        ///     session and return this history to the system every new one.
        /// </summary>
        /// <param name="id">
        ///     The ID used in the thalamus messages to identify the utterance. It <b>differs</b> from the
        ///     utterance.id which indentifies the single utterance inside its utterance library
        /// </param>
        /// <param name="Utterance_utterance">
        ///     String resulting from the serialization of the class:
        ///     <see cref="EmoteEvents.ComplexData.Utterance" />
        /// </param>
        void UtteranceUsed(string id, string Utterance_utterance);
    }

    public interface ILearnerModelIdEvents : IPerception
    {
        void nextThalamusId(int participantId); //Empty thalamus id.
        void allLearnerInfo(string[] LearnerInfo_learnerInfos); // all existing student info
        void allUtterancesForParticipant(int participantId, string[] LightUtterance_utterances);
    }

    public interface ILearnerModelActions : IAction
    {
        //This action is so that the tasks can update the learner model...
        void learnerModelUpdate(string LearnerStateInfo_learnerState);
    }

    public interface ILearnerModelMemoryEvents : IPerception
    {
        //This is called from the learner model to the IM when there is a summary, wrap up, or other update to skill level. 
        void learnerModelMemoryEvent(string MemoryEvent_memoryEvent);
    }

    public interface Logs : IPerception
    {
        void PerceptionLog(double time, double faceUpDownDegrees, double faceLeftRightDegrees, double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY,
            string gazeDirection);
    }

    public interface IScenario2Perception : IPerception
    {
        void PerceptionLog(double time, double faceUpDownDegrees, double faceLeftRightDegrees, double eyesUpdown,
            double eyesLeftRight, double headPositionY, double headPositionX, double closeRatioLeftEye,
            double closeRatioRightEye, double smile, double confidence, double anger, double disgust, double fear,
            double joy, double sadness, double surprise, double neutral, double gazeVectorX, double gazeVectorY,
            string gazeDirection, string subject);
    }
}