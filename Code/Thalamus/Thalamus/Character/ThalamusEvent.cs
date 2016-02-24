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
using System.Globalization;
using System.Reflection;

namespace Thalamus
{
    


    /*
    public class PMLsdf
    {
        public static int Count = 1;
        public string Id = "ThalamusEvent" + Count++;
        public string Name = "ThalamusEvent" + Count++;

        public bool DontLogDescription = false;

		public ThalamusEventType EventType = ThalamusEventType.Perception;

        public static PML Null = new PML("NullThalamusEvent");

        public Dictionary<string, EventParameter> Parameters = new Dictionary<string, EventParameter>();


        public PML() : this("ThalamusEvent" + Count++, new Dictionary<string, EventParameter>()) { }
        public PML(string name) : this(name, new Dictionary<string, EventParameter>()) { }
        public PML(string name, string parameterName, EventParameter parameterValue)
        {
            Name = name;
            Parameters.Add(parameterName, parameterValue);
        }
        public PML(string name, Dictionary<string, EventParameter> parameters) {
            Name = name;
            Parameters = parameters;
        }

        public PML(SyncEvents name) : this(name, new Dictionary<string, EventParameter>()) { }
        public PML(SyncEvents name, string parameterName, EventParameter parameterValue)
        {
            Name = name.ToString();
            Parameters.Add(parameterName, parameterValue);
        }
        public PML(SyncEvents name, Dictionary<string, EventParameter> parameters)
        {
            Name = name.ToString();
            Parameters = parameters;
        }

        public void AddParameter(String name, EventParameter parameter)
        {
            if (parameter.Type != EventParameterType.Null) Parameters.Add(name, parameter);
        }

        public override string ToString()
        {
            if (DontLogDescription)
            {
                return "(" + Name + "):[no description]";
            }
            else
            {
                string str = "(" + Name + "):[";
                foreach (KeyValuePair<string, EventParameter> p in Parameters)
                {
                    str += p.Key + "=" + p.Value.GetValue() + ";";
                }
                return str + "]";
            }
        }
    }*/
}
