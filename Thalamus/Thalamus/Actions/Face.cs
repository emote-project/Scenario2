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
    public class Face : FaceBase
    {
        public Face(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this(id, faceFacs, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public Face(string id, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this(id, null, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public Face(string id, List<FaceFacs> faceFacs, SyncPoint startTime) : this(id, faceFacs, null, startTime, SyncPoint.Null, 1.0f) { }

        public Face(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme) : this(id, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Face(string id, List<FaceLexeme> faceLexeme) : this(id, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Face(string id, List<FaceFacs> faceFacs) : this(id, faceFacs, null, SyncPoint.Null, SyncPoint.Null, 1.0f) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme) : this("Face" + Counter++, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Face(List<FaceLexeme> faceLexeme) : this("Face" + Counter++, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Face(List<FaceFacs> faceFacs) : this("Face" + Counter++, faceFacs, null, SyncPoint.Null, SyncPoint.Null, 1.0f) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public Face(List<FaceLexeme> faceLexeme, SyncPoint startTime) : this("Face" + Counter++, null, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public Face(List<FaceFacs> faceFacs, SyncPoint startTime) : this("Face" + Counter++, faceFacs, null, startTime, SyncPoint.Null, 1.0f) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, endTime, 1.0f) { }
        public Face(List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, null, faceLexeme, startTime, endTime, 1.0f) { }
        public Face(List<FaceFacs> faceFacs, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, faceFacs, null, startTime, endTime, 1.0f) { }




        public Face(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this(id, faceFacs, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public Face(string id, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this(id, null, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public Face(string id, List<FaceFacs> faceFacs, SyncPoint startTime, float intensity) : this(id, faceFacs, null, startTime, SyncPoint.Null, intensity) { }

        public Face(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, float intensity) : this(id, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public Face(string id, List<FaceLexeme> faceLexeme, float intensity) : this(id, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public Face(string id, List<FaceFacs> faceFacs, float intensity) : this(id, faceFacs, null, SyncPoint.Null, SyncPoint.Null, intensity) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public Face(List<FaceLexeme> faceLexeme, float intensity) : this("Face" + Counter++, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public Face(List<FaceFacs> faceFacs, float intensity) : this("Face" + Counter++, faceFacs, null, SyncPoint.Null, SyncPoint.Null, intensity) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public Face(List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this("Face" + Counter++, null, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public Face(List<FaceFacs> faceFacs, SyncPoint startTime, float intensity) : this("Face" + Counter++, faceFacs, null, startTime, SyncPoint.Null, intensity) { }

        public Face(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, endTime, intensity) { }
        public Face(List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, null, faceLexeme, startTime, endTime, intensity) { }
        public Face(List<FaceFacs> faceFacs, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, faceFacs, null, startTime, endTime, intensity) { }
		
        public Face(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : base(id, startTime, endTime, intensity) {
			if (faceFacs!=null) Facs = faceFacs;
			if (faceLexeme!=null) Lexemes = faceLexeme;
		}

        public List<FaceFacs> Facs = new List<FaceFacs>();
        public List<FaceLexeme> Lexemes = new List<FaceLexeme>();
        private List<string> AwaitingIds;
        private BehaviorExecutionContext bec;

        public void SubFaceEnded(string id)
        {
            if (AwaitingIds.Contains(id))
            {
                AwaitingIds.Remove(id);
                if (AwaitingIds.Count == 0)
                {
                    End(bec);
                }
            }
        }

        public override void Start(object param)
        {
            SendEvent(BehaviorPlan.ActionEventType.start);
            bec = (BehaviorExecutionContext)param;
            AwaitingIds = new List<string>();
            foreach (FaceFacs ff in Facs)
            {
                AwaitingIds.Add(ff.Id);
                ff.AnimationEnd += new FaceFacs.AnimationEndHandler(SubFaceEnded);
                ff.Apply(bec);
            }
            foreach (FaceLexeme fl in Lexemes)
            {
                AwaitingIds.Add(fl.Id);
                fl.AnimationEnd += new FaceLexeme.AnimationEndHandler(SubFaceEnded);
                fl.Apply(bec);
            }

            if (endTime.Type == SyncPointType.Absolute)
            {
                Thread.Sleep((int)Math.Round(Math.Max(0, endTime.AbsoluteValue - BehaviorPlan.Instance.SolveSyncPoint(startTime)) * 1000));
                End(bec);
            }
        }

        public override string ToBml()
        {
            string bml = "<face " + base.ToBml() + ">";
            foreach(FaceFacs f in Facs)
            {
                bml += f.ToBml();
            }
            foreach (FaceLexeme l in Lexemes)
            {
                bml += l.ToBml();
            }
            return bml + "</face>";
        }
    }
}
