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

namespace Thalamus
{
    public class BehaviorExecutionContext
    {
        public Character Character;
        public Behavior Behavior;
        public List<BehaviorNode> Nodes;
		
		public List<BehaviorNode> RemainingNodes;

        public BehaviorExecutionContext(Behavior behavior, List<BehaviorNode> nodes) : this(behavior, nodes, new Character()) { }
        public BehaviorExecutionContext(Behavior behavior, List<BehaviorNode> nodes, Character character)
        {
            Behavior = behavior;
            Nodes = nodes;
            Character = character;
        }
		
		public void FinishedNode(BehaviorNode node) {
			if (Nodes.Contains(node)) {
				if (RemainingNodes.Contains(node)) {
					RemainingNodes.Remove(node);
					if (RemainingNodes.Count==0) {
						BehaviorPlan.Instance.TerminateBehavior(this);
					}
				}else{
					BehaviorPlan.Instance.DebugIf("error","Re-terminating behavior node '"+node.ToString() +"' on behavior '"+ Behavior.Id+"'");
				}
			}else{
				BehaviorPlan.Instance.DebugIf("error","Disconnected behavior node '"+node.ToString() +"' terminating on behavior '"+ Behavior.Id+"'");
			}
		}
		
		public override string ToString ()
		{
			return Behavior.Id;
		}
    }
}
