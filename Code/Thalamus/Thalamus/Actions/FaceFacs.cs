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

namespace Thalamus.Actions
{
    public class FaceFacs : FaceBase
    {
        public delegate void AnimationEndHandler(string id);
        public event AnimationEndHandler AnimationEnd;
        protected void NotifyAnimationEnd(string id)
        {
            if (AnimationEnd != null) AnimationEnd(id);
        }

		public Side Side = Side.Both;
		public int AU = -1;
		
        public FaceFacs(string id, int au, Side side, SyncPoint startTime) : this(id, au, side, startTime, SyncPoint.Null) { }
		public FaceFacs(string id, int au, SyncPoint startTime) : this(id, au, Side.Both, startTime, SyncPoint.Null) { }
		
		public FaceFacs(string id, int au, Side side) : this(id, au, side, SyncPoint.Null, SyncPoint.Null) { }
		public FaceFacs(string id, int au) : this(id, au, Side.Both, SyncPoint.Null, SyncPoint.Null) { }
		
		public FaceFacs(int au, Side side) : this("FaceFacs" + Counter++, au, side, SyncPoint.Null, SyncPoint.Null) { }
		public FaceFacs(int au) : this("FaceFacs" + Counter++, au, Side.Both, SyncPoint.Null, SyncPoint.Null) { }
		
        public FaceFacs(int au, Side side, SyncPoint startTime) : this("FaceFacs" + Counter++, au, side, startTime, SyncPoint.Null) { }
		public FaceFacs(int au, SyncPoint startTime) : this("FaceFacs" + Counter++, au, Side.Both, startTime, SyncPoint.Null) { }
		
        public FaceFacs(int au, Side side, SyncPoint startTime, SyncPoint endTime) : this("FaceFacs" + Counter++, au, side, startTime, endTime) { }
		public FaceFacs(int au, SyncPoint startTime, SyncPoint endTime) : this("FaceFacs" + Counter++, au, Side.Both, startTime, endTime) { }
		
        public FaceFacs(string id, int au, Side side, SyncPoint startTime, SyncPoint endTime) : base(id, startTime, endTime) { 
			AU = au;
			Side = side;
		}

        private bool IsSubFace = true;
        BehaviorExecutionContext bec;

        public override void Start(object param)
        {
            IsSubFace = false;
            SendEvent(BehaviorPlan.ActionEventType.start);
            bec = (BehaviorExecutionContext)param;
            Apply(bec);
            if (endTime.Type == SyncPointType.Absolute)
            {
                Thread.Sleep((int)Math.Round(Math.Max(0, endTime.AbsoluteValue - BehaviorPlan.Instance.SolveSyncPoint(startTime)) * 1000));
                End(bec);
            }
        }

        public void Apply(BehaviorExecutionContext bec)
        {
            ClientsInterface body = bec.Character.Clients;
            body.FaceFacs(Id, AU, Side, Amount);
            bec.Character.AnimationEnd += new Character.AnimationEndHandler(FacsEnd);
        }

        public void FacsEnd(string id)
        {
            if (id == Id)
            {
                if (IsSubFace) NotifyAnimationEnd(id);
                else End(bec);
            }
        }

        public void Remove(BehaviorExecutionContext bec)
        {
        }

        public override string ToBml()
        {
            return "<faceFacs " + String.Format("au=\"{0}\" side=\"{1}\" amount=\"{2}\"/>", AU, Side, Amount);
        }
    }
}
