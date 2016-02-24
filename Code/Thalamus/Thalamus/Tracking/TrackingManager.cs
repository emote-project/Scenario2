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

namespace Thalamus
{
    public class TrackingManager : Manager
    {

        
        #region Singleton
        private static TrackingManager instance;
        public static TrackingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TrackingManager();

                }
                return instance;
            }
        }

        private TrackingManager()
            : base("TrackingManager")
        {
            setDebug(true);
        }
        #endregion


        private Dictionary<string, Tracker> trackers = new Dictionary<string, Tracker>();
        public Dictionary<string, Tracker> Trackers
        {
            get { return trackers; }
        }

        public override bool Setup()
        {
            base.Setup();
            Load();
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            TrackingDatabase db = new TrackingDatabase();
            foreach (Tracker t in trackers.Values) db.Objects.Add(t);
            TrackingDatabase.Save(Properties.Settings.Default.TrackingDatabase, db);
            Debug("Saved tracking database to '" + Properties.Settings.Default.TrackingDatabase + "'.");
        }

        public Tracker CreateTracker(string name)
        {
            Tracker t = new Tracker(name);
            trackers[name] = t;
            return t;
        }

        public Tracker SetTracker(string name, float horizontalAngle, float verticalAngle)
        {
            Tracker t = new Tracker(name, horizontalAngle, verticalAngle);
            trackers[name] = t;
            return t;
        }

        public void UpdateTracker(string name, float horizontalAngle, float verticalAngle)
        {
            if (trackers.ContainsKey(name))
            {
                trackers[name].HorizontalAngle = horizontalAngle;
                trackers[name].VerticalAngle= verticalAngle;
            }
        }

        internal void Load()
        {
            Load(Properties.Settings.Default.TrackingDatabase);
        }
        internal void Load(string trackingDatabase)
        {
            if (File.Exists(trackingDatabase))
            {
                TrackingDatabase db = TrackingDatabase.Load(trackingDatabase);
                if (db != null)
                {
                    foreach (Tracker t in db.Objects)
                    {
                        trackers[t.Name] = t;
                    }
                }
                Debug("Loaded " + trackers.Count + " trackers from '" + trackingDatabase + "'.");
            }
            else
            {
                Debug("Tracking database '" + trackingDatabase + "' not found.");
            }
        }
    }
}
