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
using System.Threading.Tasks;
using Thalamus;
namespace Thalamus.BML
{

    public interface IBMLCodeAction : IAction
    {
        void BML(string code);
    }

    public interface IBMLActions : ISpeakActions, IGazeActions, ILocomotionActions, IAnimationActions, IPostureActions, ISoundActions, IFaceActions, IPointingActions, IHeadActions, IWavingActions
    {}

    public interface IBMLEvents : ISpeakEvents, IGazeEvents, ILocomotionEvents, IAnimationEvents, IPostureEvents, ISoundEvents, IFaceEvents, IPointingEvents, IHeadEvents, IWavingEvents { }

    public interface ISpeakActions : IAction
    {
        void Speak(string id, string text);
        void SpeakBookmarks(string id, string[] text, string[] bookmarks);
        void SpeakStop();
    }

    public interface ISpeakEvents : IPerception
    {
        void SpeakStarted(string id);
        void SpeakFinished(string id);
    }

    public interface ISpeakDetailEvents : IPerception
    {
        void Bookmark(string id);
        void Viseme(int viseme, int nextViseme, double visemePercent, double nextVisemePercent);
    }


    public interface ISoundActions : IAction
    {
        void PlaySound(string id, string SoundName, double Volume, double Pitch);
        void PlaySoundLoop(string id, string SoundName, double Volume, double Pitch);
        void StopSound(string id);
    }

    public interface ISoundEvents : IPerception
    {
        void SoundStarted(string id);
        void SoundFinished(string id);
    }


    public interface IPostureActions : IAction
    {
        void SetPosture(string id, string posture, double percent = 1.0f, double decay = 1.0f);
        void ResetPose();
    }

    public interface IPostureEvents : IPerception
    {
    }


    public interface ILocomotionActions : IAction
    {
        void WalkTo(string id, double x, double y, double angle);
        void WalkToTarget(string id, string target);
        void StopWalk();
    }

    public interface ILocomotionEvents : IPerception
    {
        void WalkStarted(string id);
        void WalkFinished(string id);
    }


    public interface IGazeActions : IAction
    {
        void Gaze(string id, double horizontal, double vertical, double speed = 1.0f, bool trackFaces = false);
    }

    public interface IGazeEvents : IPerception
    {
        void GazeStarted(string id);
        void GazeFinished(string id);
    }

    public interface IHeadActions : IAction
    {
        void Head(string id, string lexeme, int repetitions, double amplitude = 20.0f, double frequency = 1.0f);
    }

    public interface IHeadEvents : IPerception
    {
        void HeadStarted(string id);
        void HeadFinished(string id);
    }

    public interface IPointingActions : IAction
    {
        void Pointing(string id, string target, double speed = 1.0f, Actions.PointingMode mode = Actions.PointingMode.RightHand);
        void PointingAngle(string id, double horizontal, double vertical, double speed = 1.0f, Actions.PointingMode mode = Actions.PointingMode.RightHand);
    }

    public interface IPointingEvents : IPerception
    {
        void PointingStarted(string id);
        void PointingFinished(string id);
    }

    public interface IWavingActions : IAction
    {
        void Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, Thalamus.Actions.PointingMode mode = Thalamus.Actions.PointingMode.RightHand);
    }

    public interface IWavingEvents : IPerception
    {
        void WavingStarted(string id);
        void WavingFinished(string id);
    }

    public interface IFaceActions : IAction
    {
        void FaceLexeme(string id, string lexeme);
        void FaceShiftLexeme(string id, string lexeme);
        void FaceFacs(string id, int AU, Actions.Side Side, double Intensity);
        void NeutralFaceExpression();
    }

    public interface IFaceEvents : IPerception
    {
    }


    public interface IAnimationActions : IAction
    {
        void PlayAnimation(string id, string animation);
        void PlayAnimationQueued(string id, string animation);
        void StopAnimation(string id);
    }

    public interface IAnimationEvents : IPerception
    {
        void AnimationStarted(string id);
        void AnimationFinished(string id);
    }
}
