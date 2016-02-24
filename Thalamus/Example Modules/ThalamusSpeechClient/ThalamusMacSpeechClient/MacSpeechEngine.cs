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
using System.Linq;
using System.Text;

namespace ThalamusMacSpeechClient
{
    public class MacSpeechEngine : ThalamusSpeechClient.SpeechEngine
    {

        public MacSpeechEngine()
            : base(ThalamusSpeechClient.SpeechEngineType.Mac)
        {}

        public override void Speak(ThalamusSpeechClient.SpeechClient.Speech speech)
        {
            string text = speech.FullText();
            if (text == "") return;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "say";
            proc.StartInfo.Arguments = text;
            Started(speech.Id);
            try
            {
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                SpeechClient.DebugException(e);
            }
            Ended(speech.Id);
        }
    }
}
