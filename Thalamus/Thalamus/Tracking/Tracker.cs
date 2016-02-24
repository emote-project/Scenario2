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
using System.IO;
using Newtonsoft.Json;

namespace Thalamus
{
    public class TrackingDatabase
    {
        private List<Tracker> objects = new List<Tracker>();
        public List<Tracker> Objects 
        { 
            get { return objects; }
            set { objects = value; }
        }
        public static TrackingDatabase Null = new TrackingDatabase();

        public static TrackingDatabase Load(string filename)
        {
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (TrackingDatabase)serializer.Deserialize(file, typeof(TrackingDatabase));
            }
        }



        public static void Save(string filename, TrackingDatabase trackingDb)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, trackingDb);
            }
        }
    }

    public class Tracker
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private float horizontalAngle;
        public float HorizontalAngle
        {
            get { return horizontalAngle; }
            set { horizontalAngle = value; }
        }

        private float verticalAngle;
        public float VerticalAngle
        {
            get { return verticalAngle; }
            set { verticalAngle = value; }
        }

        public Tracker() { }

        public Tracker(string name, float horizontalAngle, float verticalAngle)
        {
            this.name = name;
            this.horizontalAngle = horizontalAngle;
            this.verticalAngle = verticalAngle;
        }
        public Tracker(string name)
        {
            this.name = name;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
        }
    }
}
