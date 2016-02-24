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

namespace Thalamus
{
    public class BehaviorPlan : Manager
    {

        #region Singleton
        private static BehaviorPlan instance;
        public static BehaviorPlan Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BehaviorPlan();

                }
                return instance;
            }
        }


        private BehaviorPlan()
            : base("BehaviorPlan")
        {
            setDebug(true);
            setDebug("error", true);
            setDebug("all", true);
            setDebug("ticks", false);
            setDebug("behaviorLoad", true);
            setDebug("behaviorSchedule", true);
        }
        #endregion
		
		public enum ActionEventType {
			start,
			end,
            endx
		}
		
		public struct ActionEvent {
			public ActionEventType Type;
			public BehaviorNodes.Action Action;
            public BehaviorExecutionContext BEC;
			public ActionEvent(BehaviorNodes.Action action, ActionEventType type, BehaviorExecutionContext bec) {
				this.Type = type;
				this.Action = action;
                this.BEC = bec;
			}
			public override string ToString ()
			{
				return string.Format ("[" + Type + ", " + this.Action.ToString() + "]");
			}
		}

        public bool ExecuteActionsOnlyOnce = true;
        public int CurrentFrame
        {
            get { return currentFrame; }
        }


        private Dictionary<string, List<ActionEvent>> Eventline = new Dictionary<string, List<ActionEvent>>();
        private Dictionary<float, List<ActionEvent>> Timeline = new Dictionary<float, List<ActionEvent>>();
        private List<BehaviorExecutionContext> RunningBehaviors = new List<BehaviorExecutionContext>();
        private List<BehaviorNodes.Action> RunningActions = new List<BehaviorNodes.Action>();

               
        private Thread tExecutionCycle;
        private int currentFrame = 0;
		private bool Terminate = false;
		
		private Mutex scheduler = new Mutex();
        private Mutex eventMutex = new Mutex();

        private Character defaultCharacter = null;

        public void SetDefaultCharacter(string name)
        {
            if (Environment.Instance.Characters.ContainsKey(name)) defaultCharacter = Environment.Instance.Characters[name];
        }

        public void SetDefaultCharacter(Character character)
        {
            if (!Environment.Instance.Characters.ContainsValue(character)) Environment.Instance.AddCharacter(character);
            defaultCharacter = character;
        }


        public void Add(string behavior)
        {
            if (BehaviorManager.Instance.Behaviors.ContainsKey(behavior)) {
                if (defaultCharacter != null) Add(BehaviorManager.Instance.Behaviors[behavior],defaultCharacter);
                else Add(BehaviorManager.Instance.Behaviors[behavior]);
            }
        }
        public void Add(Behavior behavior)
        {
            if (!BehaviorManager.Instance.Behaviors.ContainsValue(behavior)) BehaviorManager.Instance.AddBehavior(behavior);

            if (defaultCharacter != null && behavior.Character==null) Add(behavior, defaultCharacter);
            else Add(behavior, behavior.Character);
        }
        public void Add(string behavior, string character) {
            if (Environment.Instance.Characters.ContainsKey(character)) Add(behavior, Environment.Instance.Characters[character]);
        }
        public void Add(string behavior, Character character)
        {
            if (BehaviorManager.Instance.Behaviors.ContainsKey(behavior)) Add(BehaviorManager.Instance.Behaviors[behavior], character);
        }
        public void Add(Behavior behavior, Character character)
        {
            switch (behavior.Composition)
            {
                case Behavior.CompositionType.Append:
                    AppendBehavior(behavior, character);
                    break;
                case Behavior.CompositionType.Merge:
                    MergeBehavior(behavior, character);
                    break;
                case Behavior.CompositionType.Replace:
                    ReplaceBehavior(behavior, character);
                    break;
            }
        }
		
		
        private void AddTimeline(float time, ActionEvent a)
        {
            if (!Timeline.ContainsKey(time)) Timeline.Add(time, new List<ActionEvent>());
            if (!Timeline[time].Contains(a)) Timeline[time].Add(a);
            DebugIf("behaviorSchedule", "AddTimeline " + time + ": " + a.ToString());
        }
        private void AddEventline(string ev, ActionEvent a)
        {
            string[] evSplit = ev.Split('.');
            if (evSplit.Length > 1)
            {
                if (!Eventline.ContainsKey(ev)) Eventline.Add(ev, new List<ActionEvent>());
                if (!Eventline[ev].Contains(a)) Eventline[ev].Add(a);
            }
            else
            {
                ev = a.BEC.Behavior.Id + "." + ev;
                if (!Eventline.ContainsKey(ev)) Eventline.Add(ev, new List<ActionEvent>());
                if (!Eventline[ev].Contains(a)) Eventline[ev].Add(a);   
            }
            DebugIf("behaviorSchedule", "AddEventline " + ev + ": " + a.ToString());
        }

        private void ScheduleAction(BehaviorNodes.Action a, BehaviorExecutionContext bec)
        {
            if (a.StartTime.Type == SyncPointType.Unspecified)
            {
                a.Event(ActionEventType.start, bec);
                RunningActions.Add(a);
            }
            else
            {
                if (a.StartTime.Type == SyncPointType.Absolute) AddTimeline(a.StartTime.AbsoluteValue * Properties.Settings.Default.PlanFPS + currentFrame, new ActionEvent(a, ActionEventType.start, bec));
                else if (a.StartTime.Type == SyncPointType.Reference) AddEventline(a.StartTime.ReferenceValue, new ActionEvent(a, ActionEventType.start, bec));

                if (a.EndTime.Type == SyncPointType.Absolute) AddTimeline(a.EndTime.AbsoluteValue * Properties.Settings.Default.PlanFPS + currentFrame, new ActionEvent(a, ActionEventType.end, bec));
                else if (a.EndTime.Type == SyncPointType.Reference) AddEventline(a.EndTime.ReferenceValue, new ActionEvent(a, ActionEventType.end, bec));
            }
        }

        public void CancelCurrentPlan()
        {
            Eventline = new Dictionary<string, List<ActionEvent>>();
            Timeline = new Dictionary<float, List<ActionEvent>>();
            RunningBehaviors = new List<BehaviorExecutionContext>();
            RunningActions = new List<BehaviorNodes.Action>();
        }
		
		
		public void TerminateBehavior(BehaviorExecutionContext bec) {
			RunningBehaviors.Remove(bec);
			int total = bec.Nodes.Count;
			int remaining = bec.RemainingNodes.Count;
			DebugIf("all", "Terminated behavior '" + bec.Behavior.Id + "': " + (total-remaining) + " of " + total + "nodes executed.");
		}

        private void ScheduleBehavior(BehaviorExecutionContext context)
        {
            DebugIf("behaviorSchedule", "ScheduleBehavior '" + context.ToString() + "'");
			try {
				scheduler.WaitOne();
	            foreach (BehaviorNode node in context.Nodes)
	            {
	                if (node is BehaviorNodes.Required)
	                {
	                    if ((node as BehaviorNodes.Required).IsValid())
	                    {
	                        foreach (BehaviorNodes.Action a in (node as BehaviorNodes.Required).Actions)
	                        {
	                            a.RequiredAction = true;
	                            a.ParentNode = node;
	                            ScheduleAction(a, context);
	                        }
	                    }
	                }
	                else
	                {
	                    (node as BehaviorNodes.Action).RequiredAction = false;
	                    ScheduleAction((node as BehaviorNodes.Action), context);
	                }
	            }
				scheduler.ReleaseMutex();
				RunningBehaviors.Add(context);
			}catch(Exception e){
				DebugIf("error", "Failed to schedule behavior '" + context.Behavior.Id + "' on character '" + context.Character.Name + "': " + e.Message);
			}
        }

        private void AppendBehavior(Behavior behavior, Character character)
        {
			DebugIf("all", "AppendBehavior '" + behavior.Id + "' on character '" + character.ToString() + "'");
            List<BehaviorNode> nodes = SolveConstraints(behavior.Nodes);
            if(character!=null) ScheduleBehavior(new BehaviorExecutionContext(behavior,nodes,character));
            else ScheduleBehavior(new BehaviorExecutionContext(behavior, nodes));
        }
        private void MergeBehavior(Behavior behavior, Character character)
        {
            DebugIf("all", "MergeBehavior '" + behavior.Id + "' on character '" + character.ToString() + "'");
            List<BehaviorNode> nodes = SolveMergeConstraints(behavior.Nodes);
            if (character != null) ScheduleBehavior(new BehaviorExecutionContext(behavior, nodes, character));
            else ScheduleBehavior(new BehaviorExecutionContext(behavior, nodes));
        }
        private void ReplaceBehavior(Behavior behavior, Character character)
        {
			DebugIf("all", "ReplaceBehavior '" + behavior.Id + "' on character '" + character.ToString() + "'");
            CancelCurrentPlan();
            List<BehaviorNode> nodes = SolveConstraints(behavior.Nodes);
            if (character != null) ScheduleBehavior(new BehaviorExecutionContext(behavior, nodes, character));
            else ScheduleBehavior(new BehaviorExecutionContext(behavior, nodes));
        }

        private List<BehaviorNode> SolveConstraints(List<BehaviorNode> nodes)
        {
            return nodes;
        }

        private List<BehaviorNode> SolveMergeConstraints(List<BehaviorNode> nodes)
        {
            return nodes;
        }
		
		public override void Dispose ()
		{
			Terminate=true;
			Thread.Sleep(Convert.ToInt32(1000.0f / Properties.Settings.Default.PlanFPS));
			tExecutionCycle.Abort();
			tExecutionCycle.Join();
            base.Dispose();
		}


        public override bool Start()
        {
            try
            {
                DebugIf("all", "Starting...");
                tExecutionCycle = new Thread(new ParameterizedThreadStart(ExecutionCycle));
				tExecutionCycle.Start();
                DebugIf("all", "Started.");
                return true;
            }
            catch (Exception e)
            {
                DebugIf("error", e.Message);
                return false;
            }
        }
		
		public float SolveSyncPoint(SyncPoint syncPoint) {
			return SolveSyncPoint(syncPoint,null);
		}
		public float SolveSyncPoint(SyncPoint syncPoint, Behavior localBehavior) {
			if (syncPoint.Type==SyncPointType.Absolute) return syncPoint.AbsoluteValue;
			else {
				Behavior b = null;
				string[] references = syncPoint.ReferenceValue.Split(':');
				if (references.Length==3) {
					if (BehaviorManager.Instance.Behaviors.ContainsKey(references[0])) b = BehaviorManager.Instance.Behaviors[references[0]];
                }
                else if (references.Length == 2 && localBehavior != null)
                {
					b = localBehavior;
				}
				if (b!=null) {
					BehaviorNodes.Action a = b.GetActionById(references[1]);
					if(a!=null) {
						SyncPoint sp = a.GetEvent(references[2]);
						if (sp.Type==SyncPointType.Absolute) return sp.AbsoluteValue;
						else {
							//can support here recursive solving
							return -1;
						}
					}else{
						return -1;
					}
				}else{
					return -1;
				}
			}
		}

        public void Event(string ev) {
			DebugIf("all", "Event: " + ev);
            if (Eventline.ContainsKey(ev))
            {
                DebugIf("all", "Hot event: " + ev);
                List<ActionEvent> actionList = Eventline[ev];
                if (actionList != null)
                {
                    eventMutex.WaitOne();
                    foreach (ActionEvent ae in actionList)
                    {
                        ae.Action.Event(ae.Type, ae.BEC);
                    }
                    eventMutex.ReleaseMutex();
                    if (ExecuteActionsOnlyOnce) Eventline.Remove(ev);
                }
                else Eventline.Remove(ev);
            }
        }

        private void ExecutionCycle(object o)
        {
            while (true && !Terminate)
            {
                try
                {
                    while (true && !Terminate)
                    {
						scheduler.WaitOne();
                        DebugIf("ticks", "Tick: " + currentFrame);
                        if (Timeline.ContainsKey(currentFrame))
                        {
                            DebugIf("all", "Hot time: " + currentFrame);
                            List<ActionEvent> actionList = Timeline[currentFrame];
                            if (actionList != null)
                            {
                                foreach (ActionEvent ae in actionList)
                                {
                                    ae.Action.Event(ae.Type, ae.BEC);
                                    RunningActions.Add(ae.Action);
                                }
                            }
                            Timeline.Remove(currentFrame);
                        }
						currentFrame++;
						scheduler.ReleaseMutex();
                        Thread.Sleep(Convert.ToInt32(1000.0f / Properties.Settings.Default.PlanFPS));
                    }
                }
                catch (Exception e)
                {
                    DebugIf("error", "Execution Cycle: " + e.Message);
                }
            }
        }
    }
}
