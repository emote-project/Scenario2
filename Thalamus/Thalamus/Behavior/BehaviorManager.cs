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
using System.Text;
using System.IO;

using GBML;
using Thalamus.Constraint;
using System.Diagnostics;

namespace Thalamus
{
    public class BehaviorManager : Manager
    {
        #region Singleton
        private static BehaviorManager instance;
        public static BehaviorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BehaviorManager();

                }
                return instance;
            }
        }

        private BehaviorManager()
            : base("BehaviorManager")
        {
            setDebug(true);
            setDebug("error", true);
            setDebug("all", false);
        }
        #endregion

        private SortedDictionary<string, Behavior> behaviors = new SortedDictionary<string, Behavior>();
        public SortedDictionary<string, Behavior> Behaviors
        {
            get { return behaviors; }
        }

        private Dictionary<string, string> originalBmlFiles = new Dictionary<string, string>();
        public Dictionary<string, string> OriginalBmlFiles
        {
            get { return originalBmlFiles; }
        }

        public Behavior BmlToBehavior(bml b)
        {
            if (b == null) return Behavior.NullBehavior;

            //List<BehaviorNode> required = new List<BehaviorNode>();
            List<Constraint.Constraint> constraints = new List<Constraint.Constraint>();
            List<BehaviorNode> nodes = new List<BehaviorNode>();
			if (b.required!=null) {
				foreach(BehaviorBlock reqBlock in b.required) {
					List<BehaviorNodes.Action> reqActions = BmlNodeToActionNode(reqBlock);
					BehaviorNodes.Required required = new BehaviorNodes.Required();
		            foreach (BehaviorNodes.Action a in reqActions)
		            {
		                a.RequiredAction = true;
		                required.Actions.Add(a);
		            }
					nodes.Add(required);
				}
			}
			
			List<BehaviorNodes.Action> actions = BmlNodeToActionNode(b);
	            foreach (BehaviorNodes.Action a in actions)
	            {
	                a.RequiredAction = false;
	                nodes.Add(a);
	            }
			
			if (b.constraint!=null) {
	            foreach (GBML.Constraint c in b.constraint)
	            {
	                Constraint.Constraint gmbconstraint = BmlConstraintToGMBConstraint(c);
	                if (gmbconstraint != null) constraints.Add(gmbconstraint);
	            }
			}

            if (nodes.Count > 0)
            {
                if (b.id != null && b.id != "")
                {
                    if (b.composition != null && b.composition != "")
                    {
                        if (Environment.Instance.Characters.ContainsKey(b.character)) return new Behavior(b.id, Environment.Instance.Characters[b.character], nodes, Behavior.CompositionTypeFromString(b.composition), constraints);
                        else return new Behavior(b.id, nodes, Behavior.CompositionTypeFromString(b.composition), constraints);
                    }
                    else
                    {
                        if (Environment.Instance.Characters.ContainsKey(b.character)) return new Behavior(b.id, Environment.Instance.Characters[b.character], nodes, constraints);
                        else return new Behavior(b.id, nodes, constraints);
                    }
                }
                else
                {
                    if (b.composition != null && b.composition != "")
                    {
                        if (Environment.Instance.Characters.ContainsKey(b.character)) return new Behavior(Environment.Instance.Characters[b.character], nodes, Behavior.CompositionTypeFromString(b.composition), constraints);
                        else return new Behavior(nodes, Behavior.CompositionTypeFromString(b.composition), constraints);
                    }
                    else
                    {
                        if (Environment.Instance.Characters.ContainsKey(b.character)) return new Behavior(Environment.Instance.Characters[b.character], nodes, constraints);
                        else return new Behavior(nodes, constraints);
                    }
                }
            }
            else return Behavior.NullBehavior;
        }

        private Constraint.Constraint BmlConstraintToGMBConstraint(GBML.Constraint c)
        {
            Constraint.Constraint gmbc = null;
            if (c.after != null) gmbc = new Constraint.After();
            if (c.before != null) gmbc = new Constraint.Before();
            if (c.synchronize != null) gmbc = new Constraint.Synchronize();
            return gmbc;
        }
		
		private Actions.Side BMLFacsSideToSide(FacsSide side) {
			switch(side) {
				case FacsSide.LEFT: return Actions.Side.Left;
				case FacsSide.RIGHT: return Actions.Side.Right;
			default: return Actions.Side.Both;
			}
		}
        private Actions.Face BMLFaceToAction(string id, FaceFacs[] aFacs, FaceLexeme[] aLexeme, string start, string end, float amount, string attackPeak, string relax, string overshoot)
        {
			List<Actions.FaceFacs> facs = new List<Actions.FaceFacs>();
			List<Actions.FaceLexeme> lexemes = new List<Actions.FaceLexeme>();
			if (aFacs!=null) foreach(FaceFacs f in aFacs) facs.Add(BMLFaceFacsToAction(f.id, f.au, f.side, f.start, f.end));
			if (aLexeme!=null) foreach(FaceLexeme f in aLexeme) lexemes.Add(BMLFaceLexemeToAction(f.id, f.lexeme, f.start, f.end));

			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
                        return new Actions.Face(id, facs, lexemes, new Actions.SyncPoint(start), new Actions.SyncPoint(end), amount);
					}else{
						//all except id*
                        return new Actions.Face(facs, lexemes, new Actions.SyncPoint(start), new Actions.SyncPoint(end), amount);
					}
				}else{
					if (id!=null && id!="") {
						//all except start
                        return new Actions.Face(id, facs, lexemes, Actions.SyncPoint.Null, new Actions.SyncPoint(end), amount);
					}else{
						//only end
                        return new Actions.Face(facs, lexemes, Actions.SyncPoint.Null, new Actions.SyncPoint(end), amount);
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
                        return new Actions.Face(id, facs, lexemes, new Actions.SyncPoint(start), amount);
					}else{
						//only start*
                        return new Actions.Face(facs, lexemes, new Actions.SyncPoint(start), amount);
					}
				}else{
					if (id!=null && id!="") {
						//only id
                        return new Actions.Face(id, facs, lexemes, amount);
					}else{
						//no params
                        return new Actions.Face(facs, lexemes, amount);
					}
				}
			}
		}
		private Actions.FaceFacs BMLFaceFacsToAction(string id, int au, FacsSide bmlSide, string start, string end) {
			Actions.Side side = BMLFacsSideToSide(bmlSide);
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.FaceFacs(id, au, side, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.FaceFacs(au, side, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.FaceFacs(id, au, side, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.FaceFacs(au, side, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.FaceFacs(id, au, side, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.FaceFacs(au, side, new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.FaceFacs(id, au, side);
					}else{
						//no params
						return new Actions.FaceFacs(au, side);
					}
				}
			}
		}
		private Actions.FaceLexeme BMLFaceLexemeToAction(string id, string lexeme, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.FaceLexeme(id, lexeme, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.FaceLexeme(lexeme, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.FaceLexeme(id, lexeme, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.FaceLexeme(lexeme, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.FaceLexeme(id, lexeme, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.FaceLexeme(lexeme, new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.FaceLexeme(id, lexeme);
					}else{
						//no params
						return new Actions.FaceLexeme(lexeme);
					}
				}
			}
		}
		private Actions.Face BMLFaceShiftToAction(string id, FaceFacs[] aFacs, FaceLexeme[] aLexeme, string start) {
			List<Actions.FaceFacs> facs = new List<Actions.FaceFacs>();
			List<Actions.FaceLexeme> lexemes = new List<Actions.FaceLexeme>();
			if (aFacs!=null) foreach(FaceFacs f in aFacs) facs.Add(BMLFaceFacsToAction(f.id, f.au, f.side, f.start, f.end));
			if (aLexeme!=null) foreach(FaceLexeme f in aLexeme) lexemes.Add(BMLFaceLexemeToAction(f.id, f.lexeme, f.start, f.end));
			if (start!=null && start!="") {
				if (id!=null && id!="") {
					//all except end*
					return new Actions.FaceShift(id, facs, lexemes, new Actions.SyncPoint(start));
				}else{
					//only start*
					return new Actions.FaceShift(facs, lexemes, new Actions.SyncPoint(start));
				}
			}else{
				if (id!=null && id!="") {
					//only id
					return new Actions.FaceShift(id, facs, lexemes);
				}else{
					//no params
					return new Actions.FaceShift(facs, lexemes);
				}
			}
		}

        private Actions.PointingMode BMLPointingModeToPointingMode(BodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case BodyPart.HEAD: return Actions.PointingMode.Head;
                case BodyPart.BOTH_HANDS: return Actions.PointingMode.BothHands;
                case BodyPart.LEFT_HAND: return Actions.PointingMode.LeftHand;
                case BodyPart.RIGHT_HAND:
                case BodyPart.LEFT_FOOT:
                case BodyPart.FOOT:
                case BodyPart.HAND:
                case BodyPart.RIGHT_FOOT:
                default: return Actions.PointingMode.RightHand;
            }
        }

		private Actions.Direction BMLDirectionToDirection(Direction direction) {
			switch(direction) {
				case Direction.DOWN: return Actions.Direction.Down;
				case Direction.UP: return Actions.Direction.Up;
				case Direction.LEFT: return Actions.Direction.Left;
				case Direction.RIGHT: return Actions.Direction.Right;
				case Direction.UPLEFT: return Actions.Direction.UpLeft;
				case Direction.UPRIGHT: return Actions.Direction.UpRight;
				case Direction.DOWNLEFT: return Actions.Direction.DownLeft;
				case Direction.DOWNRIGHT: return Actions.Direction.DownRight;
				default: return Actions.Direction.Straight;
			}
		}

        private Actions.Sound.PlaybackMode BMLSoundPlayModeToSoundPlayMode(GBML.SoundPlayMode mode)
        {
            switch (mode)
            {
                case SoundPlayMode.LOOP: return Actions.Sound.PlaybackMode.Loop;
                default: return Actions.Sound.PlaybackMode.Regular;
            }
        }
		
		private Actions.GazeInfluence BMLGazeInfluenceToGazeInfluence(string influence) {
            if (influence == null) influence = "";
			switch(influence.ToLower()) {
				case "head": return Actions.GazeInfluence.Head;
				case "shoulder": return Actions.GazeInfluence.Shoulder;
				case "waist": return Actions.GazeInfluence.Waist;
				case "whole": return Actions.GazeInfluence.Whole;
				default: return Actions.GazeInfluence.Eyes;
			}
		}
		
		private Actions.BodyPart BMLBodyPartToBodyPart(BodyPart bodyPart) {
			switch(bodyPart) {
				case BodyPart.BOTH_HANDS: return Actions.BodyPart.BothHands;
				case BodyPart.FOOT: return Actions.BodyPart.Foot;
				case BodyPart.HAND: return Actions.BodyPart.Hand;
				case BodyPart.HEAD: return Actions.BodyPart.Head;
				case BodyPart.LEFT_FOOT: return Actions.BodyPart.LeftFoot;
				case BodyPart.LEFT_HAND: return Actions.BodyPart.LeftHand;
				case BodyPart.RIGHT_FOOT: return Actions.BodyPart.RightFoot;
				case BodyPart.RIGHT_HAND: return Actions.BodyPart.RightHand;
			default: return Actions.BodyPart.Head;
			}
		}
		
		private Actions.Gaze BMLGazeToAction(string id, Gaze gaze, string start, string end) {

            if (id == null) id = "";
            Actions.Direction direction = Actions.Direction.Straight;
            if (gaze.offsetdirectionSpecified) direction = BMLDirectionToDirection(gaze.offsetdirection);
            float angle = 0;
            if (gaze.offsetangleSpecified) angle = gaze.offsetangle;
            float speed = 1;
            if (gaze.speedSpecified) speed = gaze.speed;
            Actions.GazeInfluence influence = BMLGazeInfluenceToGazeInfluence(gaze.influence);
            Actions.SyncPoint startPoint = Actions.SyncPoint.Null;
            if (start != null && start != "") startPoint = new Actions.SyncPoint(start);
            Actions.SyncPoint endPoint = Actions.SyncPoint.Null;
            if (end != null && end != "") endPoint = new Actions.SyncPoint(end);
            string target = "";
            if (gaze.target != null) target = gaze.target;

            return new Actions.Gaze(id, target, influence, angle, direction, speed, startPoint, endPoint);
        }
        private Actions.GazeShift BMLGazeShiftToAction(string id, GazeShift gaze, string start, string end)
        {
            if (id == null) id = "";
            Actions.Direction direction = Actions.Direction.Straight;
            if (gaze.offsetdirectionSpecified) direction = BMLDirectionToDirection(gaze.offsetdirection);
            float angle = 0;
            if (gaze.offsetangleSpecified) angle = gaze.offsetangle;
            float speed = 1;
            if (gaze.speedSpecified) speed = gaze.speed;
            Actions.GazeInfluence influence = BMLGazeInfluenceToGazeInfluence(gaze.influence);
            Actions.SyncPoint startPoint = Actions.SyncPoint.Null;
            if (start != null && start != "") startPoint = new Actions.SyncPoint(start);
            Actions.SyncPoint endPoint = Actions.SyncPoint.Null;
            if (end != null && end != "") endPoint = new Actions.SyncPoint(end);
            string target = "";
            if (gaze.target != null) target = gaze.target;

            return new Actions.GazeShift(id, target, influence, angle, direction, speed, startPoint, endPoint);
		}
		private Actions.Gesture BMLGestureToAction(string id, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.Gesture(id, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.Gesture(new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.Gesture(id, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.Gesture(Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.Gesture(id, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.Gesture(new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.Gesture(id);
					}else{
						//no params
						return new Actions.Gesture();
					}
				}
			}
		}
        private Actions.Head BMLHeadToAction(Head head)
        {
            string id = "";
            if (head.id != null) id = head.id;

            string lexeme = "";
            if (head.lexemeSpecified) lexeme = head.lexeme.ToString();
            
            Actions.SyncPoint startPoint = Actions.SyncPoint.Null;
            if (head.start != null && head.start != "") startPoint = new Actions.SyncPoint(head.start);
            Actions.SyncPoint endPoint = Actions.SyncPoint.Null;
            if (head.end != null && head.end != "") endPoint = new Actions.SyncPoint(head.end);

            return new Actions.Head(id, lexeme, head.repetition, startPoint, endPoint);
		}

		private Actions.HeadShift BMLHeadShiftToAction(string id, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.HeadShift(id, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.HeadShift(new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.HeadShift(id, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.HeadShift(Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.HeadShift(id, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.HeadShift(new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.HeadShift(id);
					}else{
						//no params
						return new Actions.HeadShift();
					}
				}
			}
		}
		private Actions.Locomotion BMLLocomotionToAction(GBML.Locomotion loco) {
            //try to parse target into x, y and angle
            string[] targetSplit = loco.target.Split(' ');
            bool isXY = false;
            float x = 0;
            float y = 0;
            float angle = 0;
            float f = 0f;
            if (targetSplit.Length > 0)
            {
                try
                {
                    f = float.Parse(targetSplit[0], ifp);
                    isXY = true;
                    x = f;
                }
                catch { }
            }
            f = 0;
            if (isXY && targetSplit.Length > 1)
            {
                try
                {
                    f = float.Parse(targetSplit[1], ifp);
                    y = f;
                }
                catch { }
            }
            f = 0;
            if (isXY && targetSplit.Length > 2)
            {
                try
                {
                    f = float.Parse(targetSplit[2], ifp);
                    angle = f;
                }
                catch { }
            }
            if (loco.end != null && loco.end != "")
            {
                if (loco.start != null && loco.start != "")
                {
                    if (loco.id != null && loco.id != "")
                    {
						//all params*
                        if (isXY) return new Actions.Locomotion(loco.id, new Actions.SyncPoint(loco.start), new Actions.SyncPoint(loco.end), x, y, angle);
                        else return new Actions.Locomotion(loco.id, new Actions.SyncPoint(loco.start), new Actions.SyncPoint(loco.end), loco.target);
					}else{
						//all except id*
                        if (isXY) return new Actions.Locomotion(new Actions.SyncPoint(loco.start), new Actions.SyncPoint(loco.end), x, y, angle);
                        else return new Actions.Locomotion(new Actions.SyncPoint(loco.start), new Actions.SyncPoint(loco.end), loco.target);
					}
				}else{
                    if (loco.id != null && loco.id != "")
                    {
						//all except start
                        if (isXY) return new Actions.Locomotion(loco.id, Actions.SyncPoint.Null, new Actions.SyncPoint(loco.end), x, y, angle);
                        else return new Actions.Locomotion(loco.id, Actions.SyncPoint.Null, new Actions.SyncPoint(loco.end), loco.target);
					}else{
						//only end
                        if (isXY) return new Actions.Locomotion(Actions.SyncPoint.Null, new Actions.SyncPoint(loco.end), x, y, angle);
                        else return new Actions.Locomotion(Actions.SyncPoint.Null, new Actions.SyncPoint(loco.end), loco.target);
					}
				}
			}else{
                if (loco.start != null && loco.start != "")
                {
                    if (loco.id != null && loco.id != "")
                    {
						//all except end*
                        if (isXY) return new Actions.Locomotion(loco.id, new Actions.SyncPoint(loco.start), x, y, angle);
                        else return new Actions.Locomotion(loco.id, new Actions.SyncPoint(loco.start), loco.target);
					}else{
						//only start*
                        if (isXY) return new Actions.Locomotion(new Actions.SyncPoint(loco.start), x, y, angle);
                        else return new Actions.Locomotion(new Actions.SyncPoint(loco.start), loco.target);
					}
				}else{
                    if (loco.id != null && loco.id != "")
                    {
						//only id
                        if (isXY) return new Actions.Locomotion(loco.id, x, y, angle);
                        else return new Actions.Locomotion(loco.id, loco.target);
					}else{
						//no params
                        if (isXY) return new Actions.Locomotion(loco.target, x, y, angle);
						else return new Actions.Locomotion(loco.target);
					}
				}
			}
		}
		private Actions.Pointing BMLPointingToAction(string id, Pointing pointing, string start, string end) {
            if (id == null) id = "";
            Actions.PointingMode mode = Actions.PointingMode.RightHand;
            if (pointing.modeSpecified) mode = BMLPointingModeToPointingMode(pointing.mode);
            string target = pointing.target;
            
            Actions.SyncPoint startPoint = Actions.SyncPoint.Null;
            if (start != null && start != "") startPoint = new Actions.SyncPoint(start);
            Actions.SyncPoint endPoint = Actions.SyncPoint.Null;
            if (end != null && end != "") endPoint = new Actions.SyncPoint(end);

            return new Actions.Pointing(id, target, mode, startPoint, endPoint);
		}
		private Actions.Posture BMLPostureToAction(string id, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.Posture(id, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.Posture(new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.Posture(id, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.Posture(Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.Posture(id, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.Posture(new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.Posture(id);
					}else{
						//no params
						return new Actions.Posture();
					}
				}
			}
		}
		private Actions.PostureShift BMLPostureShiftToAction(string id, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.PostureShift(id, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.PostureShift(new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.PostureShift(id, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.PostureShift(Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.PostureShift(id, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.PostureShift(new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.PostureShift(id);
					}else{
						//no params
						return new Actions.PostureShift();
					}
				}
			}
		}

        private Actions.Animation BMLAnimationToAction(GBML.Animation a)
        {
            Actions.Animation.AnimationInfluence influence = Actions.Animation.AnimationInfluence.FullBody;
            if (a.InfluenceSpecified) influence = BMLAnimationInfluenceToAnimationInfluence(a.Influence);

            if (a.end != null && a.end != "")
            {
                if (a.start != null && a.start != "")
                {
                    if (a.id != null && a.id != "")
                    {
                        //all params*
                        return new Actions.Animation(a.id, a.Name, new Actions.SyncPoint(a.start), new Actions.SyncPoint(a.end), influence);
                    }
                    else
                    {
                        //all except id*
                        return new Actions.Animation(a.Name, new Actions.SyncPoint(a.start), new Actions.SyncPoint(a.end), influence);
                    }
                }
                else
                {
                    if (a.id != null && a.id != "")
                    {
                        //all except start
                        return new Actions.Animation(a.id, a.Name, Actions.SyncPoint.Null, new Actions.SyncPoint(a.end), influence);
                    }
                    else
                    {
                        //only end
                        return new Actions.Animation(a.Name, Actions.SyncPoint.Null, new Actions.SyncPoint(a.end), influence);
                    }
                }
            }
            else
            {
                if (a.start != null && a.start != "")
                {
                    if (a.id != null && a.id != "")
                    {
                        //all except end*
                        return new Actions.Animation(a.id, a.Name, new Actions.SyncPoint(a.start), influence);
                    }
                    else
                    {
                        //only start*
                        return new Actions.Animation(a.Name, new Actions.SyncPoint(a.start), influence);
                    }
                }
                else
                {
                    if (a.id != null && a.id != "")
                    {
                        //only id
                        return new Actions.Animation(a.id, a.Name, influence);
                    }
                    else
                    {
                        //no params
                        return new Actions.Animation(a.Name, influence);
                    }
                }
            }
        }

        private Actions.Animation.AnimationInfluence BMLAnimationInfluenceToAnimationInfluence(AnimationInfluence animationInfluence)
        {
            switch (animationInfluence)
            {
                case AnimationInfluence.BOTHARMS: return Actions.Animation.AnimationInfluence.BothArms;
                case AnimationInfluence.BOTHLEGS: return Actions.Animation.AnimationInfluence.BothLegs;
                case AnimationInfluence.FULLBODY: return Actions.Animation.AnimationInfluence.FullBody;
                case AnimationInfluence.HEAD: return Actions.Animation.AnimationInfluence.Head;
                case AnimationInfluence.LEFTARM: return Actions.Animation.AnimationInfluence.LeftArm;
                case AnimationInfluence.LEFTLEG: return Actions.Animation.AnimationInfluence.LeftLeg;
                case AnimationInfluence.RIGHTARM: return Actions.Animation.AnimationInfluence.RightArm;
                case AnimationInfluence.RIGHTLEG: return Actions.Animation.AnimationInfluence.RightLeg;
                default: return Actions.Animation.AnimationInfluence.FullBody;
            }
        }

        private Actions.Sound BMLSoundToAction(GBML.Sound sound)
        {
            Actions.Sound.PlaybackMode mode = Actions.Sound.PlaybackMode.Regular;
            if (sound.ModeSpecified) mode = BMLSoundPlayModeToSoundPlayMode(sound.Mode);

            if (sound.end != null && sound.end != "")
            {
                if (sound.start != null && sound.start != "")
                {
                    if (sound.id != null && sound.id != "")
                    {
                        //all params*
						return new Actions.Sound(sound.id, sound.SoundName, new Actions.SyncPoint(sound.start), new Actions.SyncPoint(sound.end), mode);
                    }
                    else
                    {
                        //all except id*
						return new Actions.Sound(sound.SoundName, new Actions.SyncPoint(sound.start), new Actions.SyncPoint(sound.end), mode);
                    }
                }
                else
                {
                    if (sound.id != null && sound.id != "")
                    {
                        //all except start
						return new Actions.Sound(sound.id, sound.SoundName, Actions.SyncPoint.Null, new Actions.SyncPoint(sound.end), mode);
                    }
                    else
                    {
                        //only end
						return new Actions.Sound(sound.SoundName, Actions.SyncPoint.Null, new Actions.SyncPoint(sound.end), mode);
                    }
                }
            }
            else
            {
                if (sound.start != null && sound.start != "")
                {
                    if (sound.id != null && sound.id != "")
                    {
                        //all except end*
						return new Actions.Sound(sound.id, sound.SoundName, new Actions.SyncPoint(sound.start), mode);
                    }
                    else
                    {
                        //only start*
						return new Actions.Sound(sound.SoundName, new Actions.SyncPoint(sound.start), mode);
                    }
                }
                else
                {
                    if (sound.id != null && sound.id != "")
                    {
                        //only id
						return new Actions.Sound(sound.id, sound.SoundName, mode);
                    }
                    else
                    {
                        //no params
						return new Actions.Sound(sound.SoundName, mode);
                    }
                }
            }
        }

		private Actions.Speech BMLSpeechToAction(string id, string start, string end, TextBlock text) {
            string[] evs = null;
            if (text.sync != null)
            {
                evs = new string[text.sync.Length];
                for (int i = 0; i < text.sync.Length; i++)
                {
                    evs[i] = text.sync[i].id;
                }
            }
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
                        
                        return new Actions.Speech(id, text.Text, evs, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
                        return new Actions.Speech(text.Text, evs, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
                        return new Actions.Speech(id, text.Text, evs, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
                        return new Actions.Speech(text.Text, evs, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
                        return new Actions.Speech(id, text.Text, evs, new Actions.SyncPoint(start));
					}else{
						//only start*
                        return new Actions.Speech(text.Text, evs, new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.Speech(id, text.Text, evs);
					}else{
						//no params
						return new Actions.Speech(text.Text, evs);
					}
				}
			}
		}
		private Actions.Wait BMLWaitToAction(string id, string start, string end) {
			if (end!=null && end!="") {
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all params*
						return new Actions.Wait(id, new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}else{
						//all except id*
						return new Actions.Wait(new Actions.SyncPoint(start), new Actions.SyncPoint(end));
					}
				}else{
					if (id!=null && id!="") {
						//all except start
						return new Actions.Wait(id, Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}else{
						//only end
						return new Actions.Wait(Actions.SyncPoint.Null, new Actions.SyncPoint(end));
					}
				}
			}else{
				if (start!=null && start!="") {
					if (id!=null && id!="") {
						//all except end*
						return new Actions.Wait(id, new Actions.SyncPoint(start));
					}else{
						//only start*
						return new Actions.Wait(new Actions.SyncPoint(start));
					}
				}else{
					if (id!=null && id!="") {
						//only id
						return new Actions.Wait(id);
					}else{
						//no params
						return new Actions.Wait();
					}
				}
			}
		}

        private List<BehaviorNodes.Action> BmlNodeToActionNode(BehaviorBlock b)
        {
            List<BehaviorNodes.Action> a = new List<BehaviorNodes.Action>();
            if (b.animation != null && b.animation.Length > 0)
            {
                foreach (GBML.Animation animation in b.animation)
                {
                    a.Add(BMLAnimationToAction(animation));
                }
            }

            if (b.faceShift != null && b.faceShift.Length > 0)
            {
                foreach (GBML.FaceShift faceShift in b.faceShift)
                {
                    a.Add(BMLFaceShiftToAction(faceShift.id, faceShift.facs, faceShift.lexeme, faceShift.start));
                }
            }
            if (b.faceFacs != null && b.faceFacs.Length > 0)
            {
                foreach (GBML.FaceFacs faceFacs in b.faceFacs)
                {
                    a.Add(BMLFaceFacsToAction(faceFacs.id, faceFacs.au,faceFacs.side, faceFacs.start, faceFacs.end));
                }
            }
            if (b.faceLexeme != null && b.faceLexeme.Length > 0)
            {
                foreach (GBML.FaceLexeme faceLexeme in b.faceLexeme)
                {
                    a.Add(BMLFaceLexemeToAction(faceLexeme.id, faceLexeme.lexeme, faceLexeme.start, faceLexeme.end));
                }
            }
            if (b.face != null && b.face.Length > 0)
            {
                foreach (GBML.Face face in b.face)
                {
                    a.Add(BMLFaceToAction(face.id, face.facs, face.lexeme, face.start, face.end, face.amount, face.attackPeak, face.relax, face.overshoot));
                }
            }
            if (b.gazeShift != null && b.gazeShift.Length > 0)
            {
                foreach (GBML.GazeShift gazeShift in b.gazeShift)
                {
                    a.Add(BMLGazeShiftToAction(gazeShift.id, gazeShift, gazeShift.start, gazeShift.end));
                }
            }
            if (b.gaze != null && b.gaze.Length > 0)
            {
                foreach (GBML.Gaze gaze in b.gaze)
                {
                    a.Add(BMLGazeToAction(gaze.id, gaze, gaze.start, gaze.end));
                }
            }
            if (b.gesture != null && b.gesture.Length > 0)
            {
                foreach (GBML.Gesture gesture in b.gesture)
                {
                    a.Add(BMLGestureToAction(gesture.id, gesture.start, gesture.end));
                }
            }
            if (b.pointing != null && b.pointing.Length > 0)
            {
                foreach (GBML.Pointing pointing in b.pointing)
                {
                    a.Add(BMLPointingToAction(pointing.id, pointing, pointing.start, pointing.end));
                }
            }
            if (b.headDirectionShift != null && b.headDirectionShift.Length > 0)
            {
                foreach (GBML.HeadDirectionShift headDirectionShift in b.headDirectionShift)
                {
                    a.Add(BMLHeadShiftToAction(headDirectionShift.id, headDirectionShift.start, headDirectionShift.end));
                }
            }
            if (b.head != null && b.head.Length > 0)
            {
                foreach (GBML.Head head in b.head)
                {
                    a.Add(BMLHeadToAction(head));
                }
            }
            if (b.locomotion != null && b.locomotion.Length > 0)
            {
                foreach (GBML.Locomotion locomotion in b.locomotion)
                {
                    a.Add(BMLLocomotionToAction(locomotion));
                }
            }
            if (b.postureShift != null && b.postureShift.Length > 0)
            {
                foreach (GBML.PostureShift postureShift in b.postureShift)
                {
                    a.Add(BMLPostureShiftToAction(postureShift.id, postureShift.start, postureShift.end));
                }
            }
            if (b.posture != null && b.posture.Length > 0)
            {
                foreach (GBML.Posture posture in b.posture)
                {
                    a.Add(BMLPostureToAction(posture.id, posture.start, posture.end));
                }
            }
            if (b.speech != null && b.speech.Length > 0)
            {
                foreach (GBML.Speech speech in b.speech)
                {
                    if (speech.text != null)
                    {
                        a.Add(BMLSpeechToAction(speech.id, speech.start, speech.end, speech.text));
                    }
                }
            }
            if (b.wait != null && b.wait.Length > 0)
            {
                foreach (GBML.Wait wait in b.wait)
                {
                    a.Add(BMLWaitToAction(wait.id, wait.start, wait.end));
                }
            }
            if (b.sound != null && b.sound.Length > 0)
            {
                foreach (GBML.Sound sound in b.sound)
                {
                    a.Add(BMLSoundToAction(sound));
                }
            }
            return a;
        }

        

        public void AddBehavior(Behavior b)
        {
            if (b == Behavior.NullBehavior) return;
            if (behaviors.ContainsKey(b.Id)) behaviors[b.Id] = b;
            else behaviors.Add(b.Id, b);
            DebugIf("behaviorLoad", "Added behavior '" + b.Id + "': " + b.ToString());
        }


        public bool RunBML(string bml, Character character)
        {
            try
            {
                BehaviorPlan.Instance.Add(BmlToBehavior(GBML.GBML.LoadFromText(bml)), character);
                return true;
            }
            catch (Exception e)
            {
                DebugIf("error", e.Message);
                return false;
            }
        }

        public bool RunBML(string bml)
        {
            try
            {
                BehaviorPlan.Instance.Add(BmlToBehavior(GBML.GBML.LoadFromText(bml)));
                return true;
            }
            catch (Exception e)
            {
                DebugIf("error", e.Message);
                return false;
            }
        }
		
        public override bool Setup()
        {
            try
            {
                DebugIf("all", "Setup Start");

                behaviors.Clear();


                //find bmls
                if (Directory.Exists(CorrectPath(Properties.Settings.Default.BmlDirectory)))
                {
                    DirectoryInfo di = new DirectoryInfo(CorrectPath(Properties.Settings.Default.BmlDirectory));
                    FileInfo[] rgFiles = di.GetFiles("*.xml");
                    foreach (FileInfo fi in rgFiles)
                    {
                        try
                        {
                            DebugIf("all", "Load BML '" + fi.FullName + "'");

                            bml bmlObject = GBML.GBML.LoadFromText(File.ReadAllText(fi.FullName));

                            if (bmlObject != null)
                            {
                                AddBehavior(BmlToBehavior(bmlObject));
                                originalBmlFiles[bmlObject.id] = fi.FullName;
                            }
                            else
                            {
                                DebugIf("error", "Failed to load BML file: '" + fi.FullName + "'!");
                            }
                        }
                        catch (Exception e)
                        {
                            DebugIf("error", e.Message);
                            return false;
                        }
                    }
                }
                else
                {
                    DebugIf("error", "Unable to find BML directory '" + CorrectPath(Properties.Settings.Default.BmlDirectory) + "'!");
                }
                DebugIf("all", "Setup End");
                return true;
            }
            catch (Exception e)
            {
                DebugIf("error", e.Message);
                return false;
            }
        }
		
    }
}
