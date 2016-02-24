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

namespace Thalamus
{

    public class Character
    {
        public static Character NullCharacter = new Character();

        protected static int Counter = 0;
        private string name;
        public string Name { get { return name; } }

        protected ClientsInterface clients;
        public ClientsInterface Clients { get { return clients; } }

        public bool LogToCSV
        {
            get;
            set;
        }


        public Character() : this("UnspecifiedCharacter" + Counter++, new ClientsInterface()) { }
        public Character(string name) : this(name, new ClientsInterface()) { }
        public Character(ClientsInterface body) : this("UnspecifiedCharacter" + Counter++, body) { }

        public Character(string name, ClientsInterface body)
        {
            this.name = name;
            this.clients = body;
            this.LogToCSV = false;
        }

        public virtual void Dispose()
        {
            clients.Dispose();
        }

        public virtual bool Setup()
        {
            clients.Setup(this);
            Environment.Instance.DebugIf("all", "Setup Character '" + name + "'.");
            return true;
        }


        public virtual bool Start()
        {
            clients.Start();
            Environment.Instance.DebugIf("all", "Started Character '" + name + "'");
            return true;
        }

		public void SyncPoint(string id)
        {
            BehaviorPlan.Instance.Event(Name+"."+ id);
        }


		
		public void SendPerception(PML perception)
        {
            //Environment.Instance.DebugIf("all", "Perception: " + perception.ToString());
            if (perception.Parameters.ContainsKey("id") && perception.Parameters["id"].Type == PMLParameterType.String) {
                switch (perception.Name)
                {
                    case "IAnimationEvents.AnimationStarted": NotifyAnimationStart(perception.Parameters["id"].GetString()); break;
                    case "IAnimationEvents.AnimationFinished": NotifyAnimationEnd(perception.Parameters["id"].GetString()); break;
                    case "BML.FaceLexemeStart": NotifyAnimationStart(perception.Parameters["id"].GetString()); break;
                    case "BML.FaceLexemeAttackPeak": break;
                    case "BML.FaceLexemeRelax": break;
                    case "BML.FaceLexemeEnd": NotifyAnimationEnd(perception.Parameters["id"].GetString()); break;
                    case "BML.FaceFacsStart": break;
                    case "BML.FaceFacsAttackPeak": break;
                    case "BML.FaceFacsRelax": break;
                    case "BML.FaceFacsEnd": break;
                    case "IGazeEvents.GazeStarted": NotifyGazeStart(perception.Parameters["id"].GetString()); break;
                    case "BML.GazeReady": break;
                    case "BML.GazeRelax": break;
                    case "IGazeEvents.GazeFinished": NotifyGazeEnd(perception.Parameters["id"].GetString()); break;
                    case "BML.GestureStart": break;
                    case "BML.GestureReady": break;
                    case "BML.GestureStrokeStart": break;
                    case "BML.GestureStroke": break;
                    case "BML.GestureStrokeEnd": break;
                    case "BML.GestureRelax": break;
                    case "BML.GestureEnd": break;
                    case "BML.HeadStart": break;
                    case "BML.HeadReady": break;
                    case "BML.HeadStrokeStart": break;
                    case "BML.HeadStroke": break;
                    case "BML.HeadStrokeEnd": break;
                    case "BML.HeadRelax": break;
                    case "BML.HeadEnd": break;
                    case "BML.HeadDirectionStart": break;
                    case "BML.HeadDirectionEnd": break;
                    case "ILocomotionEvents.WalkStarted": NotifyWalkStart(perception.Parameters["id"].GetString()); break;
                    case "ILocomotionEvents.WalkFinished": NotifyWalkEnd(perception.Parameters["id"].GetString()); break;
                    case "BML.PointingStart": NotifyPointingStart(perception.Parameters["id"].GetString()); break;
                    case "BML.PointingReady": break;
                    case "BML.PointingStrokeStart": break;
                    case "BML.PointingStroke": break;
                    case "BML.PointingStrokeEnd": break;
                    case "BML.PointingRelax": break;
                    case "BML.PointingEnd": NotifyPointingEnd(perception.Parameters["id"].GetString()); break;
                    case "BML.PostureStart": break;
                    case "BML.PostureReady": break;
                    case "BML.PostureRelax": break;
                    case "BML.PostureEnd": break;
                    case "ISoundEvents.SoundStarted": NotifySoundStart(perception.Parameters["id"].GetString()); break;
                    case "ISoundEvents.SoundFinished": NotifySoundEnd(perception.Parameters["id"].GetString()); break;
                    case "ISpeakEvents.SpeakStarted": NotifySpeechStart(perception.Parameters["id"].GetString()); break;
                    case "ISpeakDetailEvents.Bookmark": NotifyBookmark(perception.Parameters["id"].GetString()); break;
                    case "ISpeakEvents.SpeakFinished": NotifySpeechEnd(perception.Parameters["id"].GetString()); break;
                    default:
                        BehaviorPlan.Instance.Event(perception.Name);
                        break;
                }
            }else{
                switch(perception.Name) {
                    case "IBMLCodeAction.BML":
                        {
                            Console.WriteLine("BML for character " + name);
                            if (perception.Parameters.ContainsKey("code"))
                            {
                                BehaviorPlan.Instance.Add(BehaviorManager.Instance.BmlToBehavior(GBML.GBML.LoadFromText(perception.Parameters["code"].GetValue())), this);
                            }
                        }
                        break;
			        case "SensorTouched":
				        if (perception.Parameters.ContainsKey("sensor")) {
					        if (perception.Parameters.ContainsKey("state") && perception.Parameters["state"].Type==PMLParameterType.Bool) NotifyTouch(perception.Parameters["sensor"].GetString(), perception.Parameters["state"].GetBool());
					        else NotifyTouch(perception.Parameters["sensor"].GetString());
				        }
				        break;
			        case "VisionObjectDetected":
				        if (perception.Parameters.ContainsKey("object")) NotifyVisionObjectDetected(perception.Parameters["object"].GetString());
				        break;
			        case "SoundLocated":
				        if (perception.Parameters.ContainsKey("angle") && perception.Parameters["angle"].Type==PMLParameterType.Double) {
                            if ((perception.Parameters.ContainsKey("elevation") && perception.Parameters["elevation"].Type == PMLParameterType.Double) &&
                                (perception.Parameters.ContainsKey("confidence") && perception.Parameters["confidence"].Type == PMLParameterType.Double))
                                NotifySoundLocated(perception.Parameters["angle"].GetDouble(), perception.Parameters["elevation"].GetDouble(), perception.Parameters["confidence"].GetDouble());
					        else NotifySoundLocated(perception.Parameters["angle"].GetDouble());
				        }
				        break;
                    default:
                        BehaviorPlan.Instance.Event(perception.Name);
                        break;
			    }
            }
        }
		
		public delegate void FeedbackHandler(Feedback.FeedbackType feedbackType, string method);
        public event FeedbackHandler Feedback;
        public void NotifyFeedback(Feedback.FeedbackType feedbackType, string method)
        {
            if (Feedback != null) Feedback(feedbackType, method);
        }

        public delegate void TouchHandler(string sensor, bool state);
        public event TouchHandler Touch;
        public void NotifyTouch(string sensor, bool state)
        {
            if (Touch != null) Touch(sensor, state);
        }
		public void NotifyTouch(string sensor)
        {
            if (Touch != null) Touch(sensor, true);
        }

        public delegate void AsleepHandler();
        public event AsleepHandler Asleep;
        public void NotifyAsleep()
        {
            if (Asleep != null) Asleep();
        }

        public delegate void AwakeHandler();
        public event AwakeHandler Awake;
        public void NotifyAwake()
        {
            if (Awake != null) Awake();
        }

        public delegate void GazeEndHandler(string id);
        public event GazeEndHandler GazeEnd;
        public void NotifyGazeEnd(string id)
        {
            if (GazeEnd != null) GazeEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void GazeStartHandler(string id);
        public event GazeStartHandler GazeStart;
        public void NotifyGazeStart(string id)
        {
            if (GazeStart != null) GazeStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void PostureEndHandler(string id);
        public event PostureEndHandler PostureEnd;
        public void NotifyPostureEnd(string id)
        {
            if (PostureEnd != null) PostureEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void PostureStartHandler(string id);
        public event PostureStartHandler PostureStart;
        public void NotifyPostureStart(string id)
        {
            if (PostureStart != null) PostureStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void AnimationEndHandler(string id);
        public event AnimationEndHandler AnimationEnd;
        public void NotifyAnimationEnd(string id)
        {
            if (AnimationEnd != null) AnimationEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void AnimationStartHandler(string id);
        public event AnimationStartHandler AnimationStart;
        public void NotifyAnimationStart(string id)
        {
            if (AnimationStart != null) AnimationStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void FaceLexemeEndHandler(string id);
        public event FaceLexemeEndHandler FaceLexemeEnd;
        public void NotifyFaceLexemeEnd(string id)
        {
            if (FaceLexemeEnd != null) FaceLexemeEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void FaceLexemeStartHandler(string id);
        public event FaceLexemeStartHandler FaceLexemeStart;
        public void NotifyFaceLexemeStart(string id)
        {
            if (FaceLexemeStart != null) FaceLexemeStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void FaceFacsEndHandler(string id);
        public event FaceFacsEndHandler FaceFacsEnd;
        public void NotifyFaceFacsEnd(string id)
        {
            if (FaceFacsEnd != null) FaceFacsEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void FaceFacsStartHandler(string id);
        public event FaceFacsStartHandler FaceFacsStart;
        public void NotifyFaceFacsStart(string id)
        {
            if (FaceFacsStart != null) FaceFacsStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void BookmarkHandler(string id);
        public event BookmarkHandler Bookmark;
        public void NotifyBookmark(string id)
        {
            if (Bookmark != null) Bookmark(id);
            BehaviorPlan.Instance.Event(id);
        }
		
        public delegate void SpeechEndHandler(string id);
        public event SpeechEndHandler SpeechEnd;
        public void NotifySpeechEnd(string id)
        {
            if (SpeechEnd != null) SpeechEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        public delegate void SpeechStartHandler(string id);
        public event SpeechStartHandler SpeechStart;
        public void NotifySpeechStart(string id)
        {
            if (SpeechStart != null) SpeechStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }
		
		public delegate void SoundEndHandler(string id);
        public event SoundEndHandler SoundEnd;
        public void NotifySoundEnd(string id)
        {
            if (SoundEnd != null) SoundEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }

        public delegate void PointingStartHandler(string id);
        public event PointingStartHandler PointingStart;
        public void NotifyPointingStart(string id)
        {
            if (PointingStart != null) PointingStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void PointingEndHandler(string id);
        public event PointingEndHandler PointingEnd;
        public void NotifyPointingEnd(string id)
        {
            if (PointingEnd != null) PointingEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }

        public delegate void SoundStartHandler(string id);
        public event SoundStartHandler SoundStart;
        public void NotifySoundStart(string id)
        {
            if (SoundStart != null) SoundStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void SoundLocatedHandler(double angle, double elevation, double confidence);
        public event SoundLocatedHandler SoundLocated;
        public void NotifySoundLocated(double angle, double elevation, double confidence)
        {
            if (SoundLocated != null) SoundLocated(angle, elevation, confidence);
        }
        public void NotifySoundLocated(double angle)
        {
            if (SoundLocated != null) SoundLocated(angle, 0f, 1f);
        }
		
		public delegate void VisionObjectDetectedHandler(string objectName);
        public event VisionObjectDetectedHandler VisionObjectDetected;
        public void NotifyVisionObjectDetected(string objectName)
        {
            if (VisionObjectDetected != null) VisionObjectDetected(objectName);
        }
		
        public delegate void WalkStartHandler(string id);
        public event WalkStartHandler WalkStart;
        public void NotifyWalkStart(string id)
        {
            if (WalkStart != null) WalkStart(id);
            BehaviorPlan.Instance.Event(id + ":start");
        }

        public delegate void WalkEndHandler(string id);
        public event WalkEndHandler WalkEnd;
        public void NotifyWalkEnd(string id)
        {
            if (WalkEnd != null) WalkEnd(id);
            BehaviorPlan.Instance.Event(id + ":end");
        }
        
		
		public override string ToString ()
		{
			return string.Format ("[Character: Name={0}]", name);
		}

        
    }
}
