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
    public class Behavior
    {
        public enum CompositionType
        {
            Merge,
            Append,
            Replace
        }

        public static CompositionType CompositionTypeFromString(string s) {
            switch (s.ToLower())
            {
                case "merge": return CompositionType.Merge;
                case "append": return CompositionType.Append;
                case "replace": return CompositionType.Replace;
                default: return CompositionType.Merge;
            }
        }

        protected static int Counter = 0;

        public static Behavior NullBehavior = new Behavior();

        protected List<BehaviorNode> nodes = new List<BehaviorNode>();
        protected List<Constraint.Constraint> constraints = new List<Constraint.Constraint>();
        protected Character character;
        protected string id;

        public CompositionType Composition = CompositionType.Merge;

        public List<BehaviorNode> Nodes
        {
            get { return nodes; }
        }
        public List<Constraint.Constraint> Constraints
        {
            get { return constraints; }
        }
        public Character Character
        {
            get { return character; }
        }
        public string Id
        {
            get { return id; }
        }

        public Behavior():this("UnspecifiedBehavior" + Counter++, new Character(), new List<BehaviorNode>()) {}
        public Behavior(Character character):this("UnspecifiedBehavior" + Counter++,character,new List<BehaviorNode>()) {}
        public Behavior(Character character, List<BehaviorNode> nodes):this("UnspecifiedBehavior" + Counter++,character,nodes) {}
        public Behavior(List<BehaviorNode> nodes):this("UnspecifiedBehavior" + Counter++,new Character(),nodes) {}
        public Behavior(string id):this(id,new Character(), new List<BehaviorNode>()) {}
        public Behavior(string id, Character character):this(id,character, new List<BehaviorNode>()) {}

        public Behavior(List<BehaviorNode> nodes, CompositionType composition) : this("UnspecifiedBehavior" + Counter++, new Character(), nodes, composition) { }
        public Behavior(List<BehaviorNode> nodes, List<Constraint.Constraint> constraints) : this("UnspecifiedBehavior" + Counter++, new Character(), nodes, CompositionType.Merge, constraints) { }
        public Behavior(List<BehaviorNode> nodes, CompositionType composition, List<Constraint.Constraint> constraints) : this("UnspecifiedBehavior" + Counter++, new Character(), nodes, composition, constraints) { }


        public Behavior(string id, List<BehaviorNode> nodes):this(id,new Character(),nodes) {}
        public Behavior(string id, List<BehaviorNode> nodes, List<Constraint.Constraint> constraints) : this(id, new Character(), nodes, CompositionType.Merge, constraints) { }

        public Behavior(string id, List<BehaviorNode> nodes, CompositionType composition) : this(id, new Character(), nodes, composition) { }
        public Behavior(string id, List<BehaviorNode> nodes, CompositionType composition, List<Constraint.Constraint> constraints) : this(id, new Character(), nodes, composition, constraints) { }

        public Behavior(Character character, List<BehaviorNode> nodes, List<Constraint.Constraint> constraints) : this("UnspecifiedBehavior" + Counter++, character, nodes, CompositionType.Merge, constraints) { }
        public Behavior(Character character, List<BehaviorNode> nodes, CompositionType composition) : this("UnspecifiedBehavior" + Counter++, character, nodes, composition, new List<Constraint.Constraint>()) { }
        public Behavior(Character character, List<BehaviorNode> nodes, CompositionType composition, List<Constraint.Constraint> constraints) : this("UnspecifiedBehavior" + Counter++, character, nodes, composition, constraints) { }

        public Behavior(string id, Character character, List<BehaviorNode> nodes) : this(id, character, nodes, CompositionType.Merge) {}
        public Behavior(string id, Character character, List<BehaviorNode> nodes, List<Constraint.Constraint> constraints) : this(id, character, nodes, CompositionType.Merge, constraints) { }
        public Behavior(string id, Character character, List<BehaviorNode> nodes, CompositionType composition) : this(id, character, nodes, composition, new List<Constraint.Constraint>()) { }

        public Behavior(string id, Character character, List<BehaviorNode> nodes, CompositionType composition, List<Constraint.Constraint> constraints)
        {
            this.id = id;
            this.character = character;
            this.nodes = nodes;
            this.Composition = composition;
            this.constraints = constraints;

            foreach (BehaviorNode bn in nodes)
            {
                bn.ParentBehavior = this;
                if (bn is BehaviorNodes.Action)
                {
                    (bn as BehaviorNodes.Action).RequiredAction = false;
                    (bn as BehaviorNodes.Action).ParentNode = bn;
                }
                else if (bn is BehaviorNodes.Required)
                {
                    foreach (BehaviorNodes.Action a in (bn as BehaviorNodes.Required).Actions)
                    {
                        a.ParentNode = bn;
                        a.RequiredAction = true;
                        a.ParentBehavior = this;
                    }
                }
            }
        }
		
		public BehaviorNodes.Action GetActionById(string aid) {
			foreach(BehaviorNode bn in nodes) {
				if (bn is BehaviorNodes.Required) {
					foreach(BehaviorNodes.Action a in (bn as BehaviorNodes.Required).Actions) {
						if(a.Id==aid) return a;
					}
				}else{
					if ((bn as BehaviorNodes.Action).Id==aid) return (bn as BehaviorNodes.Action);
				}
			}
			return null;
		}

        public void AddNode(BehaviorNode node)
        {
            nodes.Add(node);
            node.ParentBehavior = this;
        }

        public string NodesDescription()
        {
            string str = "";
            foreach (BehaviorNode b in nodes)
            {
                str += "\t" + b.ToString() + "\n";
            }
            return str;
        }

        public string ConstraintsDescription()
        {
            string str = "";
            foreach (Constraint.Constraint c in constraints)
            {
                str += "\t" + c.ToString() + "\n";
            }
            return str;
        }

        public override string ToString()
        {
            return Id + "(" + this.Composition + ")->(" + Character + "): \n" + NodesDescription() + (constraints.Count>0?"\nConstraints: \n" + ConstraintsDescription():"");
        }

        public string ToBml()
        {
            string bml = String.Format("<bml id=\"{0}\">", this.Id);
            foreach (BehaviorNode bn in nodes) bml += bn.ToBml();
            return bml + "</bml>";
        }
    }
}
