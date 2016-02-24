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
    public class FaceShift : Face
    {
		public FaceShift(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this(id, faceFacs, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public FaceShift(string id, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this(id, null, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public FaceShift(string id, List<FaceFacs> faceFacs, SyncPoint startTime) : this(id, faceFacs, null, startTime, SyncPoint.Null, 1.0f) { }

        public FaceShift(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme) : this(id, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceShift(string id, List<FaceLexeme> faceLexeme) : this(id, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceShift(string id, List<FaceFacs> faceFacs) : this(id, faceFacs, null, SyncPoint.Null, SyncPoint.Null, 1.0f) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme) : this("Face" + Counter++, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceShift(List<FaceLexeme> faceLexeme) : this("Face" + Counter++, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public FaceShift(List<FaceFacs> faceFacs) : this("Face" + Counter++, faceFacs, null, SyncPoint.Null, SyncPoint.Null, 1.0f) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public FaceShift(List<FaceLexeme> faceLexeme, SyncPoint startTime) : this("Face" + Counter++, null, faceLexeme, startTime, SyncPoint.Null, 1.0f) { }
        public FaceShift(List<FaceFacs> faceFacs, SyncPoint startTime) : this("Face" + Counter++, faceFacs, null, startTime, SyncPoint.Null, 1.0f) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, endTime, 1.0f) { }
        public FaceShift(List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, null, faceLexeme, startTime, endTime, 1.0f) { }
        public FaceShift(List<FaceFacs> faceFacs, SyncPoint startTime, SyncPoint endTime) : this("Face" + Counter++, faceFacs, null, startTime, endTime, 1.0f) { }




        public FaceShift(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this(id, faceFacs, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceShift(string id, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this(id, null, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceShift(string id, List<FaceFacs> faceFacs, SyncPoint startTime, float intensity) : this(id, faceFacs, null, startTime, SyncPoint.Null, intensity) { }

        public FaceShift(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, float intensity) : this(id, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceShift(string id, List<FaceLexeme> faceLexeme, float intensity) : this(id, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceShift(string id, List<FaceFacs> faceFacs, float intensity) : this(id, faceFacs, null, SyncPoint.Null, SyncPoint.Null, intensity) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceShift(List<FaceLexeme> faceLexeme, float intensity) : this("Face" + Counter++, null, faceLexeme, SyncPoint.Null, SyncPoint.Null, intensity) { }
        public FaceShift(List<FaceFacs> faceFacs, float intensity) : this("Face" + Counter++, faceFacs, null, SyncPoint.Null, SyncPoint.Null, intensity) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceShift(List<FaceLexeme> faceLexeme, SyncPoint startTime, float intensity) : this("Face" + Counter++, null, faceLexeme, startTime, SyncPoint.Null, intensity) { }
        public FaceShift(List<FaceFacs> faceFacs, SyncPoint startTime, float intensity) : this("Face" + Counter++, faceFacs, null, startTime, SyncPoint.Null, intensity) { }

        public FaceShift(List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, faceFacs, faceLexeme, startTime, endTime, intensity) { }
        public FaceShift(List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, null, faceLexeme, startTime, endTime, intensity) { }
        public FaceShift(List<FaceFacs> faceFacs, SyncPoint startTime, SyncPoint endTime, float intensity) : this("Face" + Counter++, faceFacs, null, startTime, endTime, intensity) { }
		
        public FaceShift(string id, List<FaceFacs> faceFacs, List<FaceLexeme> faceLexeme, SyncPoint startTime, SyncPoint endTime, float intensity) : base(id, faceFacs, faceLexeme, startTime, endTime, intensity) {
		}
    }
}
