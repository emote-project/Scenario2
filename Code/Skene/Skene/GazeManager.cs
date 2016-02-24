using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PhysicalSpaceManager;
using Thalamus;
using Thalamus.Actions;
using EmoteCommonMessages;

namespace Skene
{
    internal class GazeManager
    {

        #region enums, properties, fields, constructor



        public delegate void GazeTargetChangedHandler(GazeTarget gazeTarget);
        public event GazeTargetChangedHandler GazeTargetChanged;
        private void NotifyGazeTargetChanged(GazeTarget gazeTarget)
        {
            if (GazeTargetChanged != null) GazeTargetChanged(gazeTarget);
        }


        private static Vector2D GazeInterval = new Vector2D(4000, 6000);
        private static Vector2D GlanceInterval = new Vector2D(1000, 2500);
        private static Vector2D GazeRandomAmplitude = new Vector2D(40, 40);
        private static Vector2D GlanceRandomAmplitude = new Vector2D(20, -10);
        private static int GlanceThroughMapCount = 3;
        private static int GlanceAcrossRoomCount = 3;


        SkeneClient SkeneClient;
        public GazeTarget GazeState { get; private set; }
        private double defaultGazeSpeed = 1f;
        public double DefaultGazeSpeed
        {
            get { return defaultGazeSpeed; }
            set {
                defaultGazeSpeed = value;
                Properties.Settings.Default.DefaultGazeSpeed = value;
                Properties.Settings.Default.Save();
            }
        }

        bool performEstablishingGaze = Properties.Settings.Default.PerformEstablishingGaze;
        public bool PerformEstablishingGaze
        {
            get { return performEstablishingGaze; }
            set
            {
                performEstablishingGaze = value;
                Properties.Settings.Default.PerformEstablishingGaze = performEstablishingGaze;
                Properties.Settings.Default.Save();
            }
        }

        bool performGazeBreaking = Properties.Settings.Default.PerformGazeBreaking;
        public bool PerformGazeBreaking
        {
            get { return performGazeBreaking; }
            set
            {
                performGazeBreaking = value;
                Properties.Settings.Default.PerformGazeBreaking = performGazeBreaking;
                Properties.Settings.Default.Save();
            }
        }

        private string gazeId = "";
        private int gazeCounts = 0;
        private int glanceThroughMapToDo = 0;
        private int glanceAcrossRoomToDo = 0;
        private Dictionary<int,bool> isPersonVisible = new Dictionary<int,bool>();
        private Vector2D currentGazeInterval = GazeInterval;
        private Vector2D currentRandomAmplitude = GazeRandomAmplitude;
        private double currentGazeTime = GazeInterval.X;
        private Vector2D currentScreenAngle = new Vector2D(0, 0);
        private Vector2D currentScreenPoint = new Vector2D(0, 0);
        private Vector2D currentGazeAnglePoint = new Vector2D(0, 0);

        private GazeTarget lastPerson = GazeTarget.Person2;

        private bool ignoreClick = true;

        System.Timers.Timer gazeStateTimer;
        Random randomTime = new Random();

        private bool resetOnNext = false;
        private bool started = false;

        public GazeManager(SkeneClient client)
        {
            this.SkeneClient = client;
            this.DefaultGazeSpeed = Properties.Settings.Default.DefaultGazeSpeed;
            client.ClickPointChanged+=Click;
            client.PersonVisibleChanged += PersonVisibleChanged;
            client.PersonGazeDirectionChanged+=PersonGazeDirectionChanged;
            client.PersonAngleChanged += HeadTracking;
            GazeState = GazeTarget.Random;

            gazeStateTimer = new System.Timers.Timer();
            gazeStateTimer.AutoReset = false;
            gazeStateTimer.Elapsed += gazeStateTimer_Elapsed;
        }


        public void Start()
        {
            
            gazeStateTimer.Start();
            SetGazeTime(currentGazeInterval);
            started = true;
        }

        void gazeStateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GazeStateCheck();
        }

        public void Dispose()
        {
            gazeStateTimer.Stop();
            gazeStateTimer.Dispose();
            GazeState = GazeTarget.Random;
        }

        #endregion

        #region gaze state machine

        private bool backFromGlance = false;
        private void GazeStateCheck() {
            try
                {
                    SkeneClient.Debug("GazeStateCheck: {0}, backFromGlance={1}", GazeState, backFromGlance);
                    if (resetOnNext) gazeId = "";
                    if (gazeId != "") resetOnNext = true;
                    else
                    {
                        
                        currentGazeInterval = GazeInterval;
                        currentRandomAmplitude = GazeRandomAmplitude;
                            
                        if (backFromGlance)
                        {
                            if (glanceThroughMapToDo > 0)
                            {
                                currentGazeInterval = GlanceInterval;
                                currentRandomAmplitude = GlanceRandomAmplitude;
                                backFromGlance = true;
                                glanceThroughMapToDo--;
                                GenerateGaze(GazeTarget.ThroughMap, GazeCoordinates(GazeTarget.ThroughMap), DefaultGazeSpeed / 2);
                            }
                            else if (glanceAcrossRoomToDo > 0)
                            {
                                currentGazeInterval = GlanceInterval;
                                currentRandomAmplitude = GlanceRandomAmplitude;
                                backFromGlance = true;
                                glanceAcrossRoomToDo--;
                                GenerateGaze(GazeTarget.AcrossRoom, GazeCoordinates(GazeTarget.AcrossRoom), DefaultGazeSpeed / 2);
                            }
                            else
                            {
                                backFromGlance = false;
                                if (GazeState == GazeTarget.Clicks && ignoreClick)
                                {
                                    GenerateGaze(GazeState, GazeCoordinates(lastPerson), DefaultGazeSpeed / 2);
                                }
                                else
                                {
                                    GenerateGaze(GazeState, GazeCoordinates(GazeState), DefaultGazeSpeed / 2);
                                }
                            }
                            NotifyGazeTargetChanged(GazeState);
                        }
                        else
                        {
                            if (performGazeBreaking && (
                                (lastGazeTarget == GazeTarget.Person && GazeState == GazeTarget.Person)  ||
                                (lastGazeTarget == GazeTarget.Person2 && GazeState == GazeTarget.Person2) ||
                                (lastGazeTarget == GazeTarget.Person3 && GazeState == GazeTarget.Person3) ||
                                (lastGazeTarget == GazeTarget.Person4 && GazeState == GazeTarget.Person4)
                                ))
                            { //breaking Gaze
                                
                                Glance(GazeTarget.Random);
                            }
                            else if (performEstablishingGaze && lastGazeTarget == GazeTarget.Clicks && GazeState == GazeTarget.Clicks)
                            { //making Gaze
                                Glance(lastPerson);
                            }
                            else
                            {
                                GenerateGaze(GazeState, GazeCoordinates(GazeState), DefaultGazeSpeed);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    SkeneClient.DebugException(e);
                }
        }

        private void SetGazeTime(Vector2D time, bool dontRestart = false)
        {
            currentGazeTime = randomTime.NextDouble() * (currentGazeInterval.Y - currentGazeInterval.X) + currentGazeInterval.X;
            SkeneClient.Debug("Gaze time: " + currentGazeTime);
            gazeStateTimer.Interval = currentGazeTime;
            if (!dontRestart) gazeStateTimer.Start();
        }

        private GazeTarget lastGazeTarget = GazeTarget.None;
        public GazeTarget LastGazeTarget
        {
            get { return lastGazeTarget; }
        }
        private Vector2D lastGazeCoordinates = Vector2D.Zero;

        private Vector2D GazeCoordinates(GazeTarget target)
        {
            switch (target)
            {
                case GazeTarget.Person:
                    if (SkeneClient.PersonLocation.ContainsKey(0)) return SkeneClient.PersonLocation[0];
                    else return Vector2D.Zero;
                case GazeTarget.Person2:
                    if (SkeneClient.PersonLocation.ContainsKey(1)) return SkeneClient.PersonLocation[1];
                    else return Vector2D.Zero;
                case GazeTarget.Person3:
                    if (SkeneClient.PersonLocation.ContainsKey(2)) return SkeneClient.PersonLocation[2];
                    else return Vector2D.Zero;
                case GazeTarget.Person4:
                    if (SkeneClient.PersonLocation.ContainsKey(3)) return SkeneClient.PersonLocation[3];
                    else return Vector2D.Zero;
                case GazeTarget.Angle: return currentGazeAnglePoint;
                case GazeTarget.Clicks: return SkeneClient.LastTouchOnScreenCoords;
                case GazeTarget.ScreenPoint: return currentScreenPoint;
                case GazeTarget.None:
                case GazeTarget.ThroughMap:
                case GazeTarget.AcrossRoom:
                case GazeTarget.Random:
                default:
                    return Vector2D.Zero;
            }
        }

        private void GenerateGaze()
        {
            GenerateGaze(GazeState, GazeCoordinates(GazeState), DefaultGazeSpeed);
        }

        private void GenerateGaze(GazeTarget target, Vector2D coordinates, double speed)
        {
            Vector2D randomVector = Vector2D.Random(currentRandomAmplitude.X, currentRandomAmplitude.Y);
            if (lastGazeTarget == target)
            {
				if ((target == GazeTarget.Random && coordinates == lastGazeCoordinates) ||
				    (target != GazeTarget.Random && coordinates == randomVector))
					return;
            }

            gazeId = "BehaviorManagerGaze" + gazeCounts++;
            string thisGazeId = gazeId;
            switch (target)
            {
                case GazeTarget.Person:
                case GazeTarget.Person2:
                case GazeTarget.Person3:
                case GazeTarget.Person4:
                    lastPerson = target;
                    SkeneClient.Debug("Gaze {2}: {0} @ {1}", gazeId, coordinates, target);
                    SkeneClient.SkPublisher.Gaze(thisGazeId, coordinates.X, coordinates.Y+10, speed, true);
                    break;
                case GazeTarget.Angle:
                    SkeneClient.Debug("Gaze Angle: {0} @ {1}", gazeId, coordinates);
                    SkeneClient.SkPublisher.Gaze(thisGazeId, coordinates.X, coordinates.Y, speed);
                    break;
                case GazeTarget.Clicks:
                    SkeneClient.Debug("Gaze Clicks: {0} @ {1}", gazeId, coordinates);
                    if (!ignoreClick) GenerateGazeToScreen(thisGazeId, coordinates.X, coordinates.Y, speed);
                    break;
			    case GazeTarget.ScreenPoint:
				    SkeneClient.Debug ("Gaze ScreenPoint: {0} @ {1}", gazeId, coordinates);
				    SkeneClient.QueueTarget (gazeId, (() => SkeneClient.SkPublisher.Highlight (coordinates.X, coordinates.Y)), "");
                    GenerateGazeToScreen(thisGazeId, coordinates.X, coordinates.Y, speed);
				break;
                case GazeTarget.ThroughMap:
                    SkeneClient.Debug("Gaze ThroughMap: {0} @ {1}", gazeId, randomVector);
                    double w = SkeneClient.ActiveSpaceSetup._screenSetup._resolution.X;
                    double h = SkeneClient.ActiveSpaceSetup._screenSetup._resolution.Y;
                    randomVector = new Vector2D(w/2, h/2)-Vector2D.Random(w/4,h/4);
                    GenerateGazeToScreen(thisGazeId, randomVector.X, randomVector.Y, speed);
                    break;
                case GazeTarget.AcrossRoom:
                case GazeTarget.None:
                case GazeTarget.Random:
                default:
                    SkeneClient.Debug("Gaze Random: {0} @ {1}", gazeId, randomVector);
                    SkeneClient.SkPublisher.Gaze(thisGazeId, randomVector.X, Math.Abs(randomVector.Y), speed/2);
                    break;
            }
            lastGazeTarget = target;
            if (GazeState == GazeTarget.Random) lastGazeCoordinates = randomVector;
            else lastGazeCoordinates = coordinates;
            SetGazeTime(currentGazeInterval);
        }

        public void GenerateGazeToScreen(string id, double x, double y, double speed)
        {
            if (SkeneClient.ActiveSpaceSetup != null)
            {
                currentScreenAngle = SkeneClient.ActiveSpaceSetup.GazeToScreenPoint(x, y);
				SkeneClient.SkPublisher.Gaze(id, currentScreenAngle.X, currentScreenAngle.Y, speed);
            }
            else
            {
                SkeneClient.Debug("ERROR: No physical space setup set!!");
            }
        }

        internal void SwitchGazeTarget(GazeTarget target, bool dontPerform = false)
        {
            if (!started) return;
            NotifyGazeTargetChanged(target);
            GazeState = target;
            if (!dontPerform) GenerateGaze();
        }


        #endregion

        #region Events
        internal void Click(Vector2D point)
        {
            ignoreClick = false;
            if (GazeState == GazeTarget.Clicks) GenerateGaze();
        }

        internal void GlanceToScreen(Vector2D ScreenPoint)
        {
            SetGazeTime(GlanceInterval);
            backFromGlance = true;
            GenerateGaze(GazeTarget.ScreenPoint, ScreenPoint, DefaultGazeSpeed / 2);
        }

        internal void GlanceToAngle(Vector2D angleCoordinates)
        {
            backFromGlance = true;
            GenerateGaze(GazeTarget.Angle, angleCoordinates, DefaultGazeSpeed / 2);
        }

        internal void GlanceToTarget(GazeTarget target)
        {
            SetGazeTime(GlanceInterval);
            backFromGlance = true;
            GenerateGaze(target, GazeCoordinates(target), DefaultGazeSpeed / 2);
           
        }

        public void GazeToScreen(double x, double y, bool dontPerform = false)
        {
            GazeToScreen(new Vector2D(x, y), dontPerform);
        }

        public void GazeToScreen(Vector2D p, bool dontPerform = false)
        {
            currentScreenPoint = p;
            SwitchGazeTarget(GazeTarget.ScreenPoint, dontPerform);
        }

        public void GazeToAngle(double x, double y, bool dontPerform = false)
        {
            GazeToAngle(new Vector2D(x, y), dontPerform);
        }

        public void GazeToAngle(Vector2D p, bool dontPerform = false)
        {
            currentGazeAnglePoint = p;
            SwitchGazeTarget(GazeTarget.Angle, dontPerform);
        }

        internal void PersonVisibleChanged(int userId, bool DetectedSkeleton)
        {
            isPersonVisible[userId] = DetectedSkeleton;
        }

        internal void PersonGazeDirectionChanged(int userId, EmoteCommonMessages.GazeEnum direction)
        {
            //if (direction == EmoteCommonMessages.HeadDirection.Robot) SwitchGazeTarget(GazeTarget.Person);
        }

        internal void GazeFinished(string id)
        {
            Console.WriteLine("end gaze: " + id);
            gazeId = "";
        }

        internal void GazeStarted(string id)
        {}

        #endregion

        internal void HeadTracking(int userId, Vector2D PersonLocation)
        {
            if ((userId == 0 && GazeState == GazeTarget.Person)  ||
                (userId == 1 && GazeState == GazeTarget.Person2) ||
                (userId == 2 && GazeState == GazeTarget.Person3) ||
                (userId == 3 && GazeState == GazeTarget.Person4)
                )
            {
                Console.WriteLine("headtracking info: " + userId + ", " + PersonLocation.X);
                currentGazeInterval = GazeInterval;
                currentRandomAmplitude = GazeRandomAmplitude;
                GenerateGaze(GazeState, PersonLocation, DefaultGazeSpeed / 2);
            }
            else Console.WriteLine("headtracking ignored: " + gazeId);
        }

        internal void Gaze(GazeTarget GazeTarget, bool dontPerform = false)
        {
            if (!started) return;
            ignoreClick = true;
            currentGazeInterval = GazeInterval;
            currentRandomAmplitude = GazeRandomAmplitude;
            SwitchGazeTarget(GazeTarget, dontPerform);
        }

        internal void Gaze(TargetInfo target, bool dontPerform = false)
        {
            if (!started) return;
            currentGazeInterval = GazeInterval;
            currentRandomAmplitude = GazeRandomAmplitude;
            if (target.GazeTarget == GazeTarget.ScreenPoint) GazeToScreen(target.Coordinates, dontPerform);
            else if (target.GazeTarget == GazeTarget.Angle) GazeToAngle(target.Coordinates, dontPerform);
            else Gaze(target.GazeTarget, dontPerform);
        }

        internal void Glance(TargetInfo target)
        {
            if (!started || 
                ((GazeState == GazeTarget.Person ||
                  GazeState == GazeTarget.Person2 ||
                  GazeState == GazeTarget.Person3 ||
                  GazeState == GazeTarget.Person4) 
                && target.GazeTarget != GazeTarget.Person &&
                   target.GazeTarget != GazeTarget.Person2 &&
                   target.GazeTarget != GazeTarget.Person3 &&
                   target.GazeTarget != GazeTarget.Person4)) return;
            currentGazeInterval = GlanceInterval;
            currentRandomAmplitude = GlanceRandomAmplitude;
            if (target.GazeTarget == GazeTarget.ScreenPoint) GlanceToScreen(target.Coordinates);
            else if (target.GazeTarget == GazeTarget.Angle) GlanceToAngle(target.Coordinates);
            else Glance(target.GazeTarget);
            NotifyGazeTargetChanged(target.GazeTarget);
        }

        private void Glance(GazeTarget gazeTarget)
        {
            if (!started || !((GazeState == GazeTarget.Person || GazeState == GazeTarget.Person2) && gazeTarget != GazeTarget.Person && gazeTarget != GazeTarget.Person2)) return;
            currentGazeInterval = GlanceInterval;
            currentRandomAmplitude = GlanceRandomAmplitude;
            if (gazeTarget == GazeTarget.ThroughMap) glanceThroughMapToDo = GlanceThroughMapCount - 1;
            if (gazeTarget == GazeTarget.AcrossRoom)
            {
                currentRandomAmplitude = GazeRandomAmplitude;
                glanceAcrossRoomToDo = GlanceAcrossRoomCount - 1;
            }
            GlanceToTarget(gazeTarget);
        }

        internal void AnimationFinished(string id)
        {
            if (!backFromGlance) GenerateGaze();
        }
    }
}
