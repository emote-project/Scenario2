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
    public class Animation : BehaviorNodes.Action
    {
        public enum AnimationInfluence
        {
            RightArm,
            LeftArm,
            BothArms,
            LeftLeg,
            RightLeg,
            BothLegs,
            FullBody,
            Head,
        }

        public string AnimationName = "";
        public AnimationInfluence Influence = AnimationInfluence.FullBody;


        public Animation(string animationName) : this("Animation" + Counter++, animationName, SyncPoint.Null, SyncPoint.Null, AnimationInfluence.FullBody) { }
        public Animation(string animationName, AnimationInfluence influence) : this("Animation" + Counter++, animationName, SyncPoint.Null, SyncPoint.Null, influence) { }

        public Animation(string animationName, SyncPoint startTime) : this("Animation" + Counter++, animationName, startTime, SyncPoint.Null, AnimationInfluence.FullBody) { }
        public Animation(string animationName, SyncPoint startTime, AnimationInfluence influence) : this("Animation" + Counter++, animationName, startTime, SyncPoint.Null, influence) { }

        public Animation(string animationName, SyncPoint startTime, SyncPoint endTime) : this("Animation" + Counter++, animationName, startTime, endTime, AnimationInfluence.FullBody) { }
        public Animation(string animationName, SyncPoint startTime, SyncPoint endTime, AnimationInfluence influence) : this("Animation" + Counter++, animationName, startTime, endTime, influence) { }

        public Animation(string id, string animationName) : this(id, animationName, SyncPoint.Null, SyncPoint.Null, AnimationInfluence.FullBody) { }
        public Animation(string id, string animationName, AnimationInfluence influence) : this(id, animationName, SyncPoint.Null, SyncPoint.Null, influence) { }

        public Animation(string id, string animationName, SyncPoint startTime, AnimationInfluence influence) : this(id, animationName, startTime, SyncPoint.Null, influence) { }
        public Animation(string id, string animationName, SyncPoint startTime) : this(id, animationName, startTime, SyncPoint.Null, AnimationInfluence.FullBody) { }
        public Animation(string id, string animationName, SyncPoint startTime, SyncPoint endTime) : this(id, animationName, startTime, endTime, AnimationInfluence.FullBody) { }

        public Animation(string id, string animationName, SyncPoint startTime, SyncPoint endTime, AnimationInfluence influence)
            : base(id, startTime, endTime)
        {
            this.AnimationName = animationName;
            this.Influence = influence;
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            bec.Character.Clients.PlayAnimation(Id, AnimationName);
        }

        public override void End(BehaviorExecutionContext bec)
        {
            bec.Character.Clients.StopAnimation(Id);
        }

        public override string ToBml()
        {
            string bml = "<animation " + base.ToBml() + String.Format(" Name=\"{0}\"", AnimationName);
            if (Influence != AnimationInfluence.FullBody) bml += String.Format(" Influence=\"{0}\"", Influence.ToString().ToUpper());
            return bml + "/>";
        }
    }
}
