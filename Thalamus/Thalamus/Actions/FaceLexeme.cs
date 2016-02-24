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
    public class FaceLexeme : FaceBase
    {
        public delegate void AnimationEndHandler(string id);
        public event AnimationEndHandler AnimationEnd;
        protected void NotifyAnimationEnd(string id)
        {
            if (AnimationEnd != null) AnimationEnd(id);
        }

		public string Lexeme = "";
        public FaceLexeme(string id, string lexeme, SyncPoint startTime) : this(id, lexeme, startTime, SyncPoint.Null, 1.0f) { }
        public FaceLexeme(string id, string lexeme) : this(id, lexeme,  SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceLexeme(string lexeme) : this("FaceLexeme" + Counter++, lexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceLexeme(string lexeme, SyncPoint startTime) : this("FaceLexeme" + Counter++, lexeme,startTime, SyncPoint.Null, 1.0f) { }
        public FaceLexeme(string lexeme, SyncPoint startTime, SyncPoint endTime) : this("FaceLexeme" + Counter++, lexeme, startTime, endTime, 1.0f) { }

        public FaceLexeme(string id, string lexeme, SyncPoint startTime, float intensity) : this(id, lexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceLexeme(string id, string lexeme, float intensity) : this(id, lexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceLexeme(string lexeme, float intensity) : this("FaceLexeme" + Counter++, lexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceLexeme(string lexeme, SyncPoint startTime, float intensity) : this("FaceLexeme" + Counter++, lexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceLexeme(string lexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : this("FaceLexeme" + Counter++, lexeme, startTime, endTime, intensity) { }

        public FaceLexeme(string id, string lexeme, SyncPoint startTime, SyncPoint endTime) : this(id, lexeme, startTime, endTime, 1.0f) { }

        public FaceLexeme(string id, string lexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : base(id, startTime, endTime, intensity) { 
			Lexeme = lexeme;
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
            body.FaceLexeme(Id,Lexeme);
            bec.Character.AnimationEnd += new Character.AnimationEndHandler(LexemeEnd);
        }

        public void LexemeEnd(string id)
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
            return "<faceLexeme " + String.Format("lexeme=\"{0}\" amount=\"{1}\"/>", Lexeme, Amount);
        }
    }
}
