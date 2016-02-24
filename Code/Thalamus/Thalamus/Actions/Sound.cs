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
    public class Sound : BehaviorNodes.Action
    {
        public enum PlaybackMode {
            Regular,
            Loop
        }

        public string SoundName = "";
        public PlaybackMode Mode = PlaybackMode.Regular;
        public float Volume = 1.0f;
        public float Pitch = 1.0f;


        public Sound(string soundName) : this("Sound" + Counter++, soundName, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Sound(string soundName, float volume) : this("Sound" + Counter++, soundName, SyncPoint.Null, SyncPoint.Null, PlaybackMode.Regular, volume) { }
        public Sound(string soundName, PlaybackMode playbackMode) : this("Sound" + Counter++, soundName, SyncPoint.Null, SyncPoint.Null, playbackMode, 1.0f) { }
        public Sound(string soundName, PlaybackMode playbackMode, float volume) : this("Sound" + Counter++, soundName, SyncPoint.Null, SyncPoint.Null, playbackMode, volume, 1.0f) { }

        public Sound(string soundName, SyncPoint startTime) : this("Sound" + Counter++, soundName, startTime, SyncPoint.Null, 1.0f) { }
        public Sound(string soundName, SyncPoint startTime, float volume) : this("Sound" + Counter++, soundName, startTime, SyncPoint.Null, PlaybackMode.Regular, volume) { }
        public Sound(string soundName, SyncPoint startTime, PlaybackMode playbackMode) : this("Sound" + Counter++, soundName, startTime, SyncPoint.Null, playbackMode, 1.0f) { }
        public Sound(string soundName, SyncPoint startTime, PlaybackMode playbackMode, float volume) : this("Sound" + Counter++, soundName, startTime, SyncPoint.Null, playbackMode, volume, 1.0f) { }

        public Sound(string soundName, SyncPoint startTime, SyncPoint endTime) : this("Sound" + Counter++, soundName, startTime, endTime, 1.0f) { }
        public Sound(string soundName, SyncPoint startTime, SyncPoint endTime, float volume) : this("Sound" + Counter++, soundName, startTime, endTime, PlaybackMode.Regular, volume) { }
        public Sound(string soundName, SyncPoint startTime, SyncPoint endTime, PlaybackMode playbackMode) : this("Sound" + Counter++, soundName, startTime, endTime, playbackMode, 1.0f) { }
        public Sound(string soundName, SyncPoint startTime, SyncPoint endTime, PlaybackMode playbackMode, float volume) : this("Sound" + Counter++, soundName, startTime, endTime, playbackMode, volume, 1.0f) { }

        public Sound(string id, string soundName) : this(id, soundName, SyncPoint.Null, SyncPoint.Null, 1.0f) { }
        public Sound(string id, string soundName, float volume) : this(id, soundName, SyncPoint.Null, SyncPoint.Null, PlaybackMode.Regular, volume) { }
        public Sound(string id, string soundName, PlaybackMode playbackMode) : this(id, soundName, SyncPoint.Null, SyncPoint.Null, playbackMode, 1.0f) { }
        public Sound(string id, string soundName, PlaybackMode playbackMode, float volume) : this(id, soundName, SyncPoint.Null, SyncPoint.Null, playbackMode, volume, 1.0f) { }

        public Sound(string id, string soundName, SyncPoint startTime) : this(id, soundName, startTime, SyncPoint.Null, 1.0f) { }
        public Sound(string id, string soundName, SyncPoint startTime, float volume) : this(id, soundName, startTime, SyncPoint.Null, PlaybackMode.Regular, volume) { }
        public Sound(string id, string soundName, SyncPoint startTime, PlaybackMode playbackMode) : this(id, soundName, startTime, SyncPoint.Null, playbackMode, 1.0f) { }
        public Sound(string id, string soundName, SyncPoint startTime, PlaybackMode playbackMode, float volume) : this(id, soundName, startTime, SyncPoint.Null, playbackMode, volume, 1.0f) { }

        public Sound(string id, string soundName, SyncPoint startTime, SyncPoint endTime) : this(id, soundName, startTime, endTime, 1.0f) { }
        public Sound(string id, string soundName, SyncPoint startTime, SyncPoint endTime, float volume) : this(id, soundName, startTime, endTime, PlaybackMode.Regular, volume) { }
        public Sound(string id, string soundName, SyncPoint startTime, SyncPoint endTime, PlaybackMode playbackMode) : this(id, soundName, startTime, endTime, playbackMode, 1.0f) { }
        public Sound(string id, string soundName, SyncPoint startTime, SyncPoint endTime, PlaybackMode playbackMode, float volume) : this(id, soundName, startTime, endTime, playbackMode, volume, 1.0f) {}

        public Sound(string id, string soundName, SyncPoint startTime, SyncPoint endTime, PlaybackMode playbackMode, float volume, float pitch)
            : base(id, startTime, endTime)
        {
            this.Mode = playbackMode;
            this.Volume = volume;
            this.Pitch = pitch;
            this.SoundName = soundName;
        }

        public override void Start(object param)
        {
            BehaviorExecutionContext bec = (BehaviorExecutionContext)param;
            endTime = SyncPoint.Internal;
            switch (Mode)
            {
                case PlaybackMode.Loop:
                    bec.Character.Clients.PlaySoundLoop(Id, SoundName, Volume, Pitch);
                    break;
                default:
                    bec.Character.Clients.PlaySound(Id, SoundName, Volume, Pitch);
                    break;
            }
        }

        public override void End(BehaviorExecutionContext bec)
        {
            bec.Character.Clients.StopSound(Id);
        }
        public override string ToBml()
        {
            string bml = "<sound " + base.ToBml() + String.Format(" SoundName=\"{0}\" Mode=\"{1}\" Volume=\"{2}\" Pitch=\"{3}\"", SoundName, Mode, Volume, Pitch);
            return bml + "/>";
        }
    }
}
