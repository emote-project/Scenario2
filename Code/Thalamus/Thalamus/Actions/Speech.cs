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
    public class Speech : BehaviorNodes.Action
    {
        public string[] Text=new string[0];
        public string[] SyncEvents = new string[0];

        public Speech(string id, string text) : this(id, new string[]{text}, null, SyncPoint.Null, SyncPoint.Null) { }
        public Speech(string text) : this("Speech" + Counter++, new string[] { text }, null, SyncPoint.Null, SyncPoint.Null) { }
        public Speech(string id, string text, SyncPoint startTime) : this(id, new string[] { text }, null, startTime, SyncPoint.Null) { }
        public Speech(string text, SyncPoint startTime) : this("Speech" + Counter++, new string[] { text }, null, startTime, SyncPoint.Null) { }
        public Speech(string text, SyncPoint startTime, SyncPoint endTime) : this("Speech" + Counter++, new string[] { text }, null, startTime, endTime) { }        


		public Speech(string id, string[] text) : this(id,text,null,SyncPoint.Null,SyncPoint.Null) { }
        public Speech(string id, string[] text, string[] events) : this(id, text, events, SyncPoint.Null, SyncPoint.Null) { }

        public Speech(string[] text) : this("Speech" + Counter++, text, null, SyncPoint.Null, SyncPoint.Null) { }
        public Speech(string[] text, string[] events) : this("Speech" + Counter++, text, events, SyncPoint.Null, SyncPoint.Null) { }

        public Speech(string id, string[] text, SyncPoint startTime) : this(id, text, null, startTime, SyncPoint.Null) { }
        public Speech(string id, string[] text, string[] events, SyncPoint startTime) : this(id, text, events, startTime, SyncPoint.Null) { }

        public Speech(string[] text, SyncPoint startTime) : this("Speech" + Counter++, text, null, startTime, SyncPoint.Null) { }
        public Speech(string[] text, string[] events, SyncPoint startTime) : this("Speech" + Counter++, text, events, startTime, SyncPoint.Null) { }

        public Speech(string[] text, SyncPoint startTime, SyncPoint endTime) : this("Speech" + Counter++, text, null, startTime, endTime) { }
        public Speech(string[] text, string[] events, SyncPoint startTime, SyncPoint endTime) : this("Speech" + Counter++, text, events, startTime, endTime) { }
        public Speech(string id, string[] text, string[] events, SyncPoint startTime, SyncPoint endTime)
            : base(id, startTime, endTime)
        {
            this.Text = text;
            if (events != null) this.SyncEvents = events;
        }

        protected override void ChangedParentBehavior()
        {
            base.ChangedParentBehavior();
            if (ParentBehavior != null)
            {
                for (int i = 0; i < SyncEvents.Length; i++) SyncEvents[i] = ParentBehavior.Id + "." + SyncEvents[i];
            }
        }

        public string FullText()
        {
            string str = "";
            foreach (string s in Text) str += s + " ";
            return str;
        }

        public override string ToString()
        {
            return base.ToString() + "> " + FullText();
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            endTime = SyncPoint.Internal;
            if (SyncEvents.Length > 0) bec.Character.Clients.SpeakBookmarks(Id, this.Text, this.SyncEvents);
            else
            {
                string t = "";
                for (int i = 0; i < Text.Length; i++) t += Text[i] + " ";
                bec.Character.Clients.Speak(Id, t);
            }
        }

        public override string ToBml()
        {
            string bml = "<speech " + base.ToBml() + "><text>";
            for (int i = 0; i < Text.Length; i++)
            {
                bml += Text[i];
                if (SyncEvents.Length > i) bml += String.Format(" <sync id=\"{0}\"/>", SyncEvents[i]);
            }
            return bml + "</text></speech>";
        }
    }
}
