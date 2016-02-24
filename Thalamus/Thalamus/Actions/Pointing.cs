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
    public class Pointing : BehaviorNodes.Action
    {
        public string Target = "";
        public PointingMode Mode = PointingMode.RightHand;
        public double Speed = 1.0f;

        //target, mode, start, end
        public Pointing(string target, PointingMode mode, SyncPoint startTime, SyncPoint endTime) : this("Pointing" + Counter++, target, mode, startTime, endTime) { }
        public Pointing(double hAngle, double vAngle, PointingMode mode, SyncPoint startTime, SyncPoint endTime) : this("Pointing" + Counter++, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), mode, startTime, endTime) { }

        //target, mode, start
        public Pointing(string target, PointingMode mode, SyncPoint startTime) : this("Pointing" + Counter++, target, mode, startTime = SyncPoint.Null, SyncPoint.Null) { }
        public Pointing(double hAngle, double vAngle, PointingMode mode, SyncPoint startTime) : this("Pointing" + Counter++, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), mode, startTime = SyncPoint.Null, SyncPoint.Null) { }

        //target, start, end
        public Pointing(string target, SyncPoint startTime, SyncPoint endTime) : this("Pointing" + Counter++, target, PointingMode.RightHand, startTime, endTime) { }
        public Pointing(double hAngle, double vAngle, SyncPoint startTime, SyncPoint endTime) : this("Pointing" + Counter++, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), PointingMode.RightHand, startTime, endTime) { }

        //target, start
        public Pointing(string target, SyncPoint startTime) : this("Pointing" + Counter++, target, PointingMode.RightHand, startTime = SyncPoint.Null, SyncPoint.Null) { }
        public Pointing(double hAngle, double vAngle, SyncPoint startTime) : this("Pointing" + Counter++, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), PointingMode.RightHand, startTime, SyncPoint.Null) { }

        //id, target, mode, start
        public Pointing(string id, string target, PointingMode mode, SyncPoint startTime) : this(id, target, mode, startTime = SyncPoint.Null, SyncPoint.Null) { }
        public Pointing(string id, double hAngle, double vAngle, PointingMode mode, SyncPoint startTime) : this(id, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), mode, startTime = SyncPoint.Null, SyncPoint.Null) { }

        //id, target, start, end
        public Pointing(string id, string target, SyncPoint startTime, SyncPoint endTime) : this(id, target, PointingMode.RightHand, startTime, endTime) { }
        public Pointing(string id, double hAngle, double vAngle, SyncPoint startTime, SyncPoint endTime) : this(id, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), PointingMode.RightHand, startTime, endTime) { }

        //id, target, start
        public Pointing(string id, string target, SyncPoint startTime) : this(id, target, PointingMode.RightHand, startTime = SyncPoint.Null, SyncPoint.Null) { }
        public Pointing(string id, double hAngle, double vAngle, SyncPoint startTime) : this(id, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), PointingMode.RightHand, startTime, SyncPoint.Null) { }
        
        //id, target, mode, start, end
        public Pointing(string id, double hAngle, double vAngle, PointingMode mode, SyncPoint startTime, SyncPoint endTime) : this(id, "h:" + hAngle.ToString(ifp) + ";v:" + vAngle.ToString(ifp), mode, startTime, endTime) { }
        public Pointing(string id, string target, PointingMode mode, SyncPoint startTime, SyncPoint endTime) : base(id, startTime, endTime) {
            this.Target = target;
            this.Mode = mode;
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            bec.Character.Clients.Pointing(Id, Target, Speed);
        }

        public override void End(BehaviorExecutionContext bec)
        {
            bec.Character.Clients.Pointing(Id, "", Speed);
        }

        public override string ToBml()
        {
            string bml = "<pointing " + base.ToBml() + String.Format(" Target=\"{0}\" Mode=\"{1}\"", Target, Mode.ToString().ToUpper());
            return bml + "/>";
        }

    }
}
