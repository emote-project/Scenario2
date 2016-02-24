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
using System.Globalization;
namespace Thalamus
{
    public class LogEntry
    {
        public double Time { get; set; }
        public String CharacterName { get; set; }
        public String SourceClient { get; set; }
        public String TargetClient { get; set; }
        public String EventName { get; set; }
        public String EventInfo { get; set; }
        public bool LogToCSV { get; set; }

        private IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;
        public PML Event;

        public LogEntry(bool Outwards, double time, String CharacterName, String ClientName, PML Event)
        {
            if (Outwards)
            {
                TargetClient = ClientName;
                SourceClient = "";
            }
            else
            {
                TargetClient = "";
                SourceClient = ClientName;
            }
            this.Event = Event;
            this.EventInfo = Event.ToString();
            this.EventName = Event.Name;
            this.CharacterName = CharacterName;
            this.Time = time;
            this.LogToCSV = LogToCSV;
        }

        public LogEntry(bool Outwards, double time, PML Event)
        {
            if (Outwards)
            {
                TargetClient = "Sent";
                SourceClient = "";
            }
            else
            {
                TargetClient = "";
                SourceClient = "Received";
            }
            this.Event = Event;
            this.EventInfo = Event.ToString();
            this.EventName = Event.Name;
            this.CharacterName = CharacterName;
            this.Time = time;
        }

        public override string ToString()
        {
            return Time.ToString() + ":" + TargetClient + ":" + SourceClient + ":" + EventName + ":" + EventInfo;
        }

        public string ToCSVHeader()
        {
            string header = "Begin Time - ss.msec\tEnd Time - ss.msec\tDuration - ss.msec";
            int c = Event.Parameters.Count;
            foreach (string eventName in Event.Parameters.Keys)
            {
                header += "\t" + eventName;
            }
            return header;
        }

        public string ToCSV(double beginTime, double endTime)
        {
            string csv = string.Format("{0}\t{1}\t{2}", beginTime.ToString(ifp), endTime.ToString(ifp), (endTime - beginTime).ToString(ifp));
            int c = Event.Parameters.Count;
            foreach (PMLParameter param in Event.Parameters.Values)
            {
                csv += "\t" + param.GetValue();
            }
            return csv;
        }
    }
}
