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
using Newtonsoft.Json;
using System.IO;

namespace Thalamus
{
    public class Scenario
    {
        public String Name { get; set; }
        public String Filename { get; set; }

        private List<CharacterDefinition> characters = new List<CharacterDefinition>();
        public List<CharacterDefinition> Characters
        {
            get { return characters; }
            set { characters = value; }
        }

        public static Scenario Null = new Scenario();

        public static Scenario LoadScenario(string filename)
        {
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                Scenario s = (Scenario)serializer.Deserialize(file, typeof(Scenario));
                s.Filename = filename;
                return s;
            }
        }

        public static void Save(string filename, Scenario scenario)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, scenario);
            }
        }

        public bool IsNull { get { return this == Scenario.Null; } }
    }

    public class CharacterDefinition
    {
        public class Client
        {
            public string CommandLine { get; set; }
            public string Arguments { get; set; }
            public override string ToString()
            {
                return CommandLine + " " + Arguments;
            }
        }

        public CharacterDefinition()
        {
            Clients = new List<Client>();
        }
        public string Name { get; set; }
        public List<Client> Clients { get; set; }
    }

}
