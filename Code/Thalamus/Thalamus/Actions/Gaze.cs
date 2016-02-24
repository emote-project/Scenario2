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
using System.Threading;
using MathNet.Numerics.LinearAlgebra;

namespace Thalamus.Actions
{
    public class Gaze : BehaviorNodes.Action
    {
        public static Gaze Null = new Gaze(Direction.Straight);
		public string Target = "";
		public GazeInfluence Influence = GazeInfluence.Eyes;
		public double OffsetAngle = 0;
		public Direction OffsetDirection = Direction.Straight;
        public double Speed = 1.0f;


        public class GazeAngle : Gaze
        {
            public GazeAngle(string id, Direction direction, double angle, double speed = 1.0f) : base(id, "", GazeInfluence.Eyes, angle, direction, speed, SyncPoint.Null, SyncPoint.Null) { }
            public GazeAngle(string id, double horizontal, double vertical, double speed = 1.0f) : base(id, String.Format("h:{0} v:{1}", horizontal, vertical), GazeInfluence.Eyes, 0, Direction.Straight, speed, SyncPoint.Null, SyncPoint.Null) { }
            public void SetStart(SyncPoint start)
            {
                if (start !=SyncPoint.Null) this.startTime = start;
            }
            public void SetEnd(SyncPoint end)
            {
				if (end != SyncPoint.Null) this.endTime = end;
            }
        }
        
		public Gaze(string id, GazeInfluence influence, Direction offsetDirection) : this(id, "", influence, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, string target, GazeInfluence influence, double offsetAngle) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, GazeInfluence influence, double offsetAngle) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(GazeInfluence influence, Direction offsetDirection) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(GazeInfluence influence, double offsetAngle) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, GazeInfluence influence, Direction offsetDirection, SyncPoint startTime) : this(id, "", influence, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, string target, GazeInfluence influence, double offsetAngle, SyncPoint startTime) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, GazeInfluence influence, double offsetAngle, SyncPoint startTime) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, GazeInfluence influence, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this(id, "", influence, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public Gaze(GazeInfluence influence, Direction offsetDirection, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        
        public Gaze(GazeInfluence influence, Direction offsetDirection, double offsetAngle, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, offsetAngle, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(GazeInfluence influence, Direction offsetDirection, double offsetAngle, double speed, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, offsetAngle, offsetDirection, speed, startTime, SyncPoint.Null) { }
        
        public Gaze(GazeInfluence influence, double offsetAngle, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(GazeInfluence influence, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public Gaze(string id, GazeInfluence influence, double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public Gaze(string id, string target, GazeInfluence influence, double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public Gaze(GazeInfluence influence, double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }



        public Gaze(string id, Direction offsetDirection) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, string target, double offsetAngle) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, double offsetAngle) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }

        public Gaze(Direction offsetDirection) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(double offsetAngle) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string target) : this("Gaze" + Counter++, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }

        public Gaze(string id, Direction offsetDirection, double offsetAngle) : this(id, "", GazeInfluence.Eyes, offsetAngle, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, Direction offsetDirection, double offsetAngle, SyncPoint startTime) : this(id, "", GazeInfluence.Eyes, offsetAngle, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, Direction offsetDirection, SyncPoint startTime) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public Gaze(string id, string target, double offsetAngle, SyncPoint startTime) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, string target, double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public Gaze(string id, double offsetAngle, SyncPoint startTime) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }

        public Gaze(Direction offsetDirection, SyncPoint startTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(double offsetAngle, SyncPoint startTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }

        public Gaze(Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public Gaze(string id, double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public Gaze(double offsetAngle, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }


        public Gaze(string id, string target, GazeInfluence influence) : this(id, target, influence, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, string target, GazeInfluence influence, SyncPoint startTime) : this(id, target, influence, 0, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, string target, GazeInfluence influence, SyncPoint startTime, SyncPoint endTime) : this(id, target, influence, 0, Direction.Straight, 1.0f, startTime, endTime) { }


        public Gaze(string id, string target) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public Gaze(string id, string target, SyncPoint startTime) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public Gaze(string id, string target, SyncPoint startTime, SyncPoint endTime) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, startTime, endTime) { }
		
		
		
        public Gaze(string id, string target, GazeInfluence influence, double offsetAngle, Direction offsetDirection, double speed, SyncPoint startTime, SyncPoint endTime) : base(id, startTime, endTime) { 
			if(target!=null) Target = target;
            Influence = influence;
            OffsetAngle = offsetAngle;
            OffsetDirection = offsetDirection;
            Speed = speed;
		}


        protected void PerformGaze(BehaviorExecutionContext bec)
        {
            ClientsInterface body = bec.Character.Clients;
            bool trackfaces = false;
            Vector v = new Vector(2);
            v[0]=0;
            v[1]=0;
            /*bool cameraSurface = false;
            bool screenSurface = false;*/
            string target = Target.ToLower();
            if (target == "trackfaces")
            {
                trackfaces = true;
                /*case "camera": cameraSurface = true; break;
                case "screen": screenSurface = true; break;*/
            }
            else if (target.Contains("h:") || target.Contains("v:"))
            {
                string[] split = target.Split(' ');
                foreach (string param in split)
                {
                    if (param.StartsWith("h:")) {
                        try {
                            v[0] = double.Parse(param.Substring(2).Trim());
                        }catch{}
                    }
                    if (param.StartsWith("v:"))
                    {
                        try
                        {
                            v[1] = double.Parse(param.Substring(2).Trim());
                        }
                        catch { }
                    }
                }
                body.Gaze(Id, (double)v[0], (double)v[1], Speed, trackfaces);
                return;
            }

            if (trackfaces || Target == "" || !TrackingManager.Instance.Trackers.ContainsKey(Target))
            {
                double angle = OffsetAngle;
                //if (angle == 0) angle = 90;
                
                switch (OffsetDirection)
                {
                    case Direction.Down:
                        v[1] = -angle;
                        break;
                    case Direction.Up:
                        v[1] = angle;
                        break;
                    case Direction.Left:
                        v[0] = angle;
                        break;
                    case Direction.Right:
                        v[0] = -angle;
                        break;
                    case Direction.DownLeft:
                        v[0] = angle;
                        v[1] = -angle;
                        break;
                    case Direction.DownRight:
                        v[0] = -angle;
                        v[1] = -angle;
                        break;
                    case Direction.UpLeft:
                        v[0] = angle;
                        v[1] = angle;
                        break;
                    case Direction.UpRight:
                        v[0] = -angle;
                        v[1] = angle;
                        break;
                    default:
                        v[0] = angle;
                        v[1] = angle;
                        break;
                }
                body.Gaze(Id, (double)v[0], (double)v[1], Speed, trackfaces);
            }
            else
            {
                body.Gaze(Id, (double)TrackingManager.Instance.Trackers[Target].HorizontalAngle, (double)TrackingManager.Instance.Trackers[Target].VerticalAngle, Speed, trackfaces);
            }
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            PerformGaze(bec);
        }


        public override void End(BehaviorExecutionContext bec)
        {
            ClientsInterface body = bec.Character.Clients;
            base.End(bec);
        }
        public override string ToString()
        {
            return base.ToString() + (Target != null && Target != "" ? "Target:" + Target + ", " : "") + "Direction:" + OffsetDirection + ", " + (OffsetAngle != 0 ? "Angle:" + OffsetAngle : "") + (Speed != 0 ? "Speed:" + Speed : "");
        }

        public override string  ToBml()
        {
            string bml = "<gaze " + base.ToBml() + String.Format(" influence=\"{0}\"", Influence);
            if (Target != "") bml += String.Format(" target=\"{0}\"", Target);
            if (OffsetAngle != 0) bml += String.Format(" offsetangle=\"{0}\"", OffsetAngle.ToString(ifp));
            if (Speed != 1.0) bml += String.Format(" speed=\"{0}\"", Speed.ToString(ifp));
            if (OffsetDirection != Direction.Straight) bml += String.Format(" offsetdirection=\"{0}\"", OffsetDirection.ToString().ToUpper());
            return bml + "/>";
        }
        
    }
}
