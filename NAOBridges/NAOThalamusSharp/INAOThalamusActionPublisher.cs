using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using Thalamus.Actions;
using Thalamus.BML;

namespace NAOThalamus
{
    public interface INAOThalamusActionPublisher : IXmlRpcProxy, INAOThalamus
    {
        [XmlRpcMethod]
        new void SpeakerVolume(double value);
        [XmlRpcMethod]
        new void EyesIntensity(double value);
        [XmlRpcMethod]
        new void EyeBlink(int count);
        [XmlRpcMethod]
        new void SlowEyeBlink(int count);
        [XmlRpcMethod]
        new void SpeakStop();
        [XmlRpcMethod]
        new void Speak(string id, string text);
        [XmlRpcMethod]
        new void SpeakBookmarks(string id, string[] text, string[] bookmarks);
        [XmlRpcMethod]
        new void Gaze(string id, double horizontal, double vertical, double speed, bool trackFaces);
        [XmlRpcMethod]
        new void Pointing(string id, string target, double speed = 1, PointingMode mode = PointingMode.RightHand);
        [XmlRpcMethod]
        new void PointingAngle(string id, double horizontal, double vertical, double speed = 1, PointingMode mode = PointingMode.RightHand);
        [XmlRpcMethod]
        void PointingNao(string id, double horizontal, double vertical, double speed, string mode);
        [XmlRpcMethod]
        new void Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, PointingMode mode);
        [XmlRpcMethod]
        void WavingNao(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, string mode);
        [XmlRpcMethod]
        new void Head(string id, string lexeme, int repetitions, double amplitude = 20, double frequency = 1);
        [XmlRpcMethod]
        new void WalkTo(string id, double x, double y, double angle);
        [XmlRpcMethod]
        new void WalkToTarget(string id, string target);
        [XmlRpcMethod]
        new void StopWalk();
        [XmlRpcMethod]
        new void StopAnimation(string id);
        [XmlRpcMethod]
        new void PlayAnimation(string id, string animation);
        [XmlRpcMethod]
        new void PlayAnimationQueued(string id, string animation);
        [XmlRpcMethod]
        new void ResetPose();
        [XmlRpcMethod]
        new void SetPosture(string id, string posture, double percent, double decay);
        [XmlRpcMethod]
        new void PlaySound(string id, string SoundName, double Volume, double Pitch);
        [XmlRpcMethod]
        new void PlaySoundLoop(string id, string SoundName, double Volume, double Pitch);
        [XmlRpcMethod]
        new void StopSound(string id);
        [XmlRpcMethod]
        new void FaceFacs(string id, int AU, Thalamus.Actions.Side Side, double Intensity);
        [XmlRpcMethod]
        new void FaceLexeme(string id, string lexeme);
        [XmlRpcMethod]
        new void FaceShiftLexeme(string id, string lexeme);
        [XmlRpcMethod]
        new void NeutralFaceExpression();
        [XmlRpcMethod]
        new void Animate(double ticks, string[] joints, double[] values, double[] speed, double time, string frameType);
        [XmlRpcMethod]
        new void SetIdleBehavior(bool idleState);
        [XmlRpcMethod]
        void Ping();
        [XmlRpcMethod]
        void SetAddress(string ipAddress);
        /*[XmlRpcMethod]
        void Heartbeat(string ticks);
        [XmlRpcMethod]
        void Heartbeat(double ticks);*/
    }
}
