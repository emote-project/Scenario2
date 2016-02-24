using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thalamus.BML;
using Thalamus;

namespace NAOThalamus
{
    public interface INAOEvents : Thalamus.IPerception
    {
        void SensorTouched(string sensor, bool state);
        void SoundSourceLocalized(double azimuth, double elevation, double confidence);
        void VisionObjectDetected(string[] objectNames, double ratio);
        void AnimationsList(string[] animations);
        void EyeShapeList(string[] eyeShapes);
    }

    public interface ISpeechDetectionEvents : Thalamus.IPerception
    {
        void WordDetected(string[] words);
    }

    public interface INAOThalamusEvents : IBMLEvents, INAOEvents,
        ISpeakEvents,
        ISpeakDetailEvents,
        Thalamus.BML.IWavingEvents,
        ISpeechDetectionEvents
    {
        void Ping();
        //void CloseTcpStream();
    }

    public interface INAOActions : IAction
    {
        void SetIdleBehavior(bool idleState);
        void EyesIntensity(double value);
        void SpeakerVolume(double value);
        void EyeBlink(int count);
        void SlowEyeBlink(int count);
    }
}
