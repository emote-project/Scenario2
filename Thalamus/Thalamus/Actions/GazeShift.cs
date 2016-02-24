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
    public class GazeShift : Gaze
    {
        public new static GazeShift Null = new GazeShift(Direction.Straight);

        public GazeShift(string id, GazeInfluence influence, Direction offsetDirection) : this(id, "", influence, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, string target, GazeInfluence influence, float offsetAngle) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, GazeInfluence influence, float offsetAngle) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(GazeInfluence influence, Direction offsetDirection) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(GazeInfluence influence, float offsetAngle) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, GazeInfluence influence, Direction offsetDirection, SyncPoint startTime) : this(id, "", influence, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, string target, GazeInfluence influence, float offsetAngle, SyncPoint startTime) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, GazeInfluence influence, float offsetAngle, SyncPoint startTime) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, GazeInfluence influence, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this(id, "", influence, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public GazeShift(GazeInfluence influence, Direction offsetDirection, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(GazeInfluence influence, float offsetAngle, SyncPoint startTime) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(GazeInfluence influence, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", influence, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public GazeShift(string id, GazeInfluence influence, float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public GazeShift(string id, string target, GazeInfluence influence, float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, target, influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public GazeShift(GazeInfluence influence, float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", influence, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }



        public GazeShift(string id, Direction offsetDirection) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, string target, float offsetAngle) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, float offsetAngle) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, Direction offsetDirection, float offsetAngle) : this(id, "", GazeInfluence.Eyes, offsetAngle, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }

        public GazeShift(Direction offsetDirection) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(float offsetAngle) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }

        public GazeShift(string id, Direction offsetDirection, SyncPoint startTime) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this(id, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public GazeShift(string id, string target, float offsetAngle, SyncPoint startTime) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, string target, float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, target, GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public GazeShift(string id, float offsetAngle, SyncPoint startTime) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }

        public GazeShift(Direction offsetDirection, SyncPoint startTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(float offsetAngle, SyncPoint startTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }

        public GazeShift(string target) : this("GazeShift" + Counter++, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }

        public GazeShift(Direction offsetDirection, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, 0, offsetDirection, 1.0f, startTime, endTime) { }
        public GazeShift(string id, float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this(id, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }
        public GazeShift(float offsetAngle, SyncPoint startTime, SyncPoint endTime) : this("Gaze" + Counter++, "", GazeInfluence.Eyes, offsetAngle, Direction.Straight, 1.0f, startTime, endTime) { }


        public GazeShift(string id, string target, GazeInfluence influence) : this(id, target, influence, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, string target, GazeInfluence influence, SyncPoint startTime) : this(id, target, influence, 0, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, string target, GazeInfluence influence, SyncPoint startTime, SyncPoint endTime) : this(id, target, influence, 0, Direction.Straight, 1.0f, startTime, endTime) { }


        public GazeShift(string id, string target) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, SyncPoint.Null, SyncPoint.Null) { }
        public GazeShift(string id, string target, SyncPoint startTime) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, startTime, SyncPoint.Null) { }
        public GazeShift(string id, string target, SyncPoint startTime, SyncPoint endTime) : this(id, target, GazeInfluence.Eyes, 0, Direction.Straight, 1.0f, startTime, endTime) { }
		
		
		
        public GazeShift(string id, string target, GazeInfluence influence, float offsetAngle, Direction offsetDirection, float speed, SyncPoint startTime, SyncPoint endTime) : base(id, target, influence, offsetAngle, offsetDirection, speed, startTime, endTime) 
        {
		}
    }
}
