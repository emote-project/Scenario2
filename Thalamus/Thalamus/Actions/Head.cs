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
    public class Head : BehaviorNodes.Action
    {
        public string Lexeme = "";
        public int Repetitions = 1;
        public double Frequency = 1.0f;

		public Head(string id) : base(id) { }
		public Head() : base("Head" + Counter++) { }
        public Head(string lexeme, SyncPoint startTime, SyncPoint endTime) : this("Head" + Counter++, lexeme, 1, 1.0f, startTime, endTime) { }
        public Head(string lexeme, double frequency, SyncPoint startTime, SyncPoint endTime) : this("Head" + Counter++, lexeme, 1, frequency, startTime, endTime) { }
        public Head(string id, string lexeme, SyncPoint startTime, SyncPoint endTime) : this(id, lexeme, 1, 1.0f, startTime, endTime) { }
        public Head(string id, string lexeme, double frequency, SyncPoint startTime, SyncPoint endTime) : this(id, lexeme, 1, frequency, startTime, endTime) { }
        public Head(string lexeme, int repetitions, SyncPoint startTime, SyncPoint endTime) : this("Head" + Counter++, lexeme, repetitions, 1.0f, startTime, endTime) { }
        public Head(string lexeme, int repetitions, double frequency, SyncPoint startTime, SyncPoint endTime) : this("Head" + Counter++, lexeme, repetitions, frequency, startTime, endTime) { }
        public Head(string id, string lexeme, int repetitions, SyncPoint startTime, SyncPoint endTime) : this(id, lexeme, repetitions, 1.0f, startTime, endTime) { }
        public Head(string id, string lexeme, int repetitions, double frequency, SyncPoint startTime, SyncPoint endTime)
            : base(id, startTime, endTime) 
        {
            this.Repetitions = repetitions;
            this.Lexeme = lexeme;
            this.Frequency = frequency;
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            bec.Character.Clients.Head(Id, Lexeme, Repetitions, Frequency);
        }

        public override void End(BehaviorExecutionContext bec)
        {
            bec.Character.Clients.Head(Id, "", Repetitions, Frequency);
        }

        public override string ToBml()
        {
            string bml = "<head " + base.ToBml() + String.Format(" Lexeme=\"{0}\"", Lexeme);
            return bml + "/>";
        }
    }
}
