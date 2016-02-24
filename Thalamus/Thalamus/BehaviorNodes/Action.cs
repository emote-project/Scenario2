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

using Thalamus.Actions;

namespace Thalamus.BehaviorNodes
{
    public abstract class Action : BehaviorNode
    {
		
		
        protected static int Counter = 0;

        private string id;
        public string Id { 
            get { return (ParentBehavior != null ? ParentBehavior.Id + "." + id : id); } 
        }

        protected Thread tAction;

        public bool RequiredAction = false;
        public BehaviorNode ParentNode=null;

        protected SyncPoint startTime = SyncPoint.Null;
        public SyncPoint StartTime { get { return startTime; } }
        protected SyncPoint endTime = SyncPoint.Null;
        public SyncPoint EndTime { get { return endTime; } }

        public string EndTag { get { return id + ":end"; } }
        public string StartTag { get { return id + ":start"; } }

        public Action(SyncPoint startTime) : this("UnspecifiedAction" + Counter++, startTime, SyncPoint.Null) { }
        public Action(SyncPoint startTime, SyncPoint endTime) : this("UnspecifiedAction" + Counter++, startTime, endTime) { }
        public Action(string id, SyncPoint startTime) :this(id,startTime,SyncPoint.Null) { }
		public Action(string id) :this(id,SyncPoint.Null,SyncPoint.Null) { }
		public Action() :this("UnspecifiedAction" + Counter++,SyncPoint.Null,SyncPoint.Null) { }
        public Action(string id, SyncPoint startTime, SyncPoint endTime)
        {
            this.id = id;
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public bool Launch(BehaviorExecutionContext bec)
        {
            if (!RequiredAction || (RequiredAction && ParentNode != null && (ParentNode as Required).IsValid()))
            {
                if (startTime.Offset > 0)
                {
                    tAction = new Thread(new ParameterizedThreadStart(OffsetStart));
                    BehaviorManager.Instance.DebugIf("all", "Launching action(offset): " + Id);
                    tAction.Start(bec);
                }
                else
                {
                    tAction = new Thread(new ParameterizedThreadStart(Start));
                    BehaviorManager.Instance.DebugIf("all", "Launching action: " + Id);
                    tAction.Start(bec);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OffsetStart(object param)
        {
            BehaviorManager.Instance.Debug("going to sleep");
            Thread.Sleep(Convert.ToInt32(startTime.Offset * 1000.0f));
            BehaviorManager.Instance.Debug("awake");
            Start(param);
        }

        public virtual void Failed(BehaviorExecutionContext bec)
        {
            if (RequiredAction && ParentNode != null) (ParentNode as Required).Failed();
            SendEvent(BehaviorPlan.ActionEventType.endx);
        }

        public virtual void End(BehaviorExecutionContext bec)
        {
            SendEvent(BehaviorPlan.ActionEventType.end);
		}

        public virtual void Event(BehaviorPlan.ActionEventType ev, BehaviorExecutionContext bec)
        {
            switch (ev)
            {
                case BehaviorPlan.ActionEventType.start: Launch(bec);
                    break;
                case BehaviorPlan.ActionEventType.end: End(bec);
                    break;
            }
        }

		public virtual SyncPoint GetEvent(string ev) {
			switch(ev.ToLower()) {
                case "start":
                    if (startTime.Type == SyncPointType.Unspecified) return endTime;
                    return startTime;
			    case "end":
                    if (endTime.Type == SyncPointType.Unspecified) return startTime;
                    return endTime;
			    default:
				    return SyncPoint.Null;
			}
		}
		
        protected void RuntimeError(string message)
        {
            BehaviorManager.Instance.DebugIf("error", "Controlled RuntimeError(" + Id + "): " + message);

            if (RequiredAction)
            {
            }
        }

        public void SendEvent(BehaviorPlan.ActionEventType eventType)
        {
            switch (eventType)
            {
                case BehaviorPlan.ActionEventType.end:
                    BehaviorPlan.Instance.Event(Id + ":end");
                    break;
                case BehaviorPlan.ActionEventType.start:
                    BehaviorPlan.Instance.Event(Id + ":start");
                    break;
            }
        }

        public virtual void Start(object param) {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            SendEvent(BehaviorPlan.ActionEventType.start);
            if (endTime.Type == SyncPointType.Unspecified) End(bec);
		}

        public virtual bool IsValid()
        {
            return true;
        }

        public override string ToString()
        {
            return this.GetType().ToString() + "(" + id + "):[" + startTime.ToString() + ", " + endTime.ToString() + "] " + (RequiredAction?"RequiredAction":"");
        }
        public override string ToBml()
        {
            string bml = string.Format(" id=\"{0}\" start=\"{1}\" ", id, startTime.ToBml());
            if (endTime != SyncPoint.Null && endTime != SyncPoint.Internal) bml += "end=\"" + endTime.ToBml() + "\" ";
            return bml;
        }
    }
}
