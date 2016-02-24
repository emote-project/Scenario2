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

namespace Thalamus.Actions
{
    public class Locomotion : BehaviorNodes.Action
    {
        public bool IsXY = false;
        public float X = 0;
        public float Y = 0;
        public float Angle = 0;
        public string Target = "";

        public Locomotion(string target) : this("Locomotion" + Counter++, target) { }
        public Locomotion(string id, string target) : this(id, SyncPoint.Null, target) { }
        public Locomotion(string id, SyncPoint startTime, string target) : this(id, startTime, SyncPoint.Null, target) { }
        public Locomotion(SyncPoint startTime, string target) : this("Locomotion" + Counter++, startTime, SyncPoint.Null, target) { }
        public Locomotion(SyncPoint startTime, SyncPoint endTime, string target) : this("Locomotion" + Counter++, startTime, endTime, target) { }

        public Locomotion(float x) : this("Locomotion" + Counter++, x) { }
        public Locomotion(string id, float x) : this(id, SyncPoint.Null, x) { }
        public Locomotion(string id, SyncPoint startTime, float x) : this(id, startTime, SyncPoint.Null, x) { }
        public Locomotion(SyncPoint startTime, float x) : this("Locomotion" + Counter++, startTime, SyncPoint.Null, x) { }
        public Locomotion(SyncPoint startTime, SyncPoint endTime, float x) : this("Locomotion" + Counter++, startTime, endTime, x) { }

        public Locomotion(float x, float y) : this("Locomotion" + Counter++, x, y) { }
        public Locomotion(string id, float x, float y) : this(id, SyncPoint.Null, x, y) { }
        public Locomotion(string id, SyncPoint startTime, float x, float y) : this(id, startTime, SyncPoint.Null, x, y) { }
        public Locomotion(SyncPoint startTime, float x, float y) : this("Locomotion" + Counter++, startTime, SyncPoint.Null, x, y) { }
        public Locomotion(SyncPoint startTime, SyncPoint endTime, float x, float y) : this("Locomotion" + Counter++, startTime, endTime, x, y) { }

        public Locomotion(float x, float y, float angle) : this("Locomotion" + Counter++, x, y, angle) { }
        public Locomotion(string id, float x, float y, float angle) : this(id, SyncPoint.Null, x, y, angle) { }
        public Locomotion(string id, SyncPoint startTime, float x, float y, float angle) : this(id, startTime, SyncPoint.Null, x, y, angle) { }
        public Locomotion(SyncPoint startTime, float x, float y, float angle) : this("Locomotion" + Counter++, startTime, SyncPoint.Null, x, y, angle) { }
        public Locomotion(SyncPoint startTime, SyncPoint endTime, float x, float y, float angle) : this("Locomotion" + Counter++, startTime, endTime, x, y, angle) { }

        public Locomotion(string id, SyncPoint startTime, SyncPoint endTime, String target) : base(id, startTime, endTime) {
            IsXY = false;
            Target = target;
            string[] splits = target.Split(' ');
            if (splits.Length == 3 && splits[0] == "xy")
            {
                try
                {
                    X = float.Parse(splits[1], ifp);
                    Y = float.Parse(splits[2], ifp);
                }
                catch { }
            }
            else if (splits.Length == 4 && splits[0] == "xyt")
            {
                try
                {
                    X = float.Parse(splits[1], ifp);
                    Y = float.Parse(splits[2], ifp);
                    Angle = float.Parse(splits[3], ifp);
                }
                catch { }
            }
        }

        public Locomotion(string id, SyncPoint startTime, SyncPoint endTime, float x) : this(id, startTime, endTime, x, 0, 0) { }
        public Locomotion(string id, SyncPoint startTime, SyncPoint endTime, float x, float y) : this(id, startTime, endTime, x, y, 0) { }
        public Locomotion(string id, SyncPoint startTime, SyncPoint endTime, float x, float y, float angle) : base(id, startTime, endTime) {
            IsXY = true;
            X = x;
            Y = y;
            Angle = angle;
        }

        public override string ToString()
        {
            return base.ToString() + "> " + (IsXY?("x:" + X + ", y:" + Y + ", angle: " + Angle):"target:'" + Target + "'");
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            endTime = SyncPoint.Internal;
            if (IsXY) bec.Character.Clients.WalkTo(Id, X, Y, Angle);
            else bec.Character.Clients.WalkToTarget(Id, Target);
        }

        public override void End(BehaviorExecutionContext bec)
        {
            ClientsInterface body = bec.Character.Clients;
            body.StopWalk();
            base.End(bec);
        }
        public override string ToBml()
        {
            return "<locomotion " + base.ToBml() + String.Format(" target=\"{0}\"/>", Target);
        }
    }
}
