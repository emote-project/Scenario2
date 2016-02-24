using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using Thalamus.BML;
using EmoteEvents;
using NuttyThalamus;

namespace NAOThalamus
{

    internal class NAOThalamusEventListener : XmlRpcListenerService, INAOThalamusEvents
    {
        NAOThalamusClient client;
        public NAOThalamusEventListener(NAOThalamusClient client)
        {
            this.client = client;
        }

        [XmlRpcMethod()]
        public void SpeakFinished(string id)
        {
            client.ThalamusPublisher.SpeakFinished(id);
        }

        [XmlRpcMethod()]
        public void SpeakStarted(string id)
        {
            client.ThalamusPublisher.SpeakStarted(id);
        }

        [XmlRpcMethod()]
        public void GazeFinished(string id)
        {
            client.ThalamusPublisher.GazeFinished(id);
        }

        [XmlRpcMethod()]
        public void GazeStarted(string id)
        {
            client.ThalamusPublisher.GazeStarted(id);
        }

        [XmlRpcMethod()]
        public void WalkFinished(string id)
        {
            client.ThalamusPublisher.WalkFinished(id);
        }

        [XmlRpcMethod()]
        public void WalkStarted(string id)
        {
            client.ThalamusPublisher.WalkStarted(id);
        }

        [XmlRpcMethod()]
        public void AnimationFinished(string id)
        {
            client.ThalamusPublisher.AnimationFinished(id);
        }

        [XmlRpcMethod()]
        public void AnimationStarted(string id)
        {
            client.ThalamusPublisher.AnimationStarted(id);
        }

        [XmlRpcMethod()]
        public void SoundFinished(string id)
        {
            client.ThalamusPublisher.SoundFinished(id);
        }

        [XmlRpcMethod()]
        public void SoundStarted(string id)
        {
            client.ThalamusPublisher.SoundStarted(id);
        }

        [XmlRpcMethod()]
        public void Ping() {
        }

        [XmlRpcMethod()]
        public void SensorTouched(string sensor, bool state)
        {
            client.ThalamusPublisher.SensorTouched(sensor, state);
        }

        [XmlRpcMethod()]
        public void SoundSourceLocalized(double azimuth, double elevation, double confidence)
        {
            client.ThalamusPublisher.SoundSourceLocalized(azimuth, elevation, confidence);
        }

        [XmlRpcMethod()]
        public void VisionObjectDetected(string[] objectNames, double ratio)
        {
            client.ThalamusPublisher.VisionObjectDetected(objectNames, ratio);
        }

        [XmlRpcMethod()]
        public void PointingFinished(string id)
        {
            client.ThalamusPublisher.PointingFinished(id);
        }

        [XmlRpcMethod()]
        public void PointingStarted(string id)
        {
            client.ThalamusPublisher.PointingStarted(id);
        }

        [XmlRpcMethod()]
        public void WavingFinished(string id)
        {
            client.ThalamusPublisher.WavingFinished(id);
        }

        [XmlRpcMethod()]
        public void WavingStarted(string id)
        {
            client.ThalamusPublisher.WavingStarted(id);
        }

        [XmlRpcMethod()]
        public void Viseme(int viseme, int nextViseme, double visemePercent, double nextVisemePercent)
        {
            client.ThalamusPublisher.Viseme(viseme, nextViseme, visemePercent, nextVisemePercent);
        }

        [XmlRpcMethod()]
        public void Bookmark(string id)
        {
            client.ThalamusPublisher.Bookmark(id);
        }

        [XmlRpcMethod()]
        public void HeadFinished(string id)
        {
            client.ThalamusPublisher.HeadFinished(id);
        }

        [XmlRpcMethod()]
        public void HeadStarted(string id)
        {
            client.ThalamusPublisher.HeadStarted(id);
        }

        [XmlRpcMethod()]
        public void AnimationsList(string[] animations)
        {
            client.AnimationsList(animations);
        }

        [XmlRpcMethod()]
        public void EyeShapeList(string[] eyeShapes)
        {
            client.EyeShapeList(eyeShapes);
        }

        [XmlRpcMethod()]
        public void WordDetected(string[] words)
        {
            client.ThalamusPublisher.WordDetected(words);
        }

        [XmlRpcMethod()]
        public void HeartbeatEcho(Double ticks, string[] joints, double[] values) 
        {
            client.NotifyHeartbeatEcho(ticks, joints, values);
            if (client.IsConnected) client.ThalamusPublisher.HeartbeatEcho(ticks, joints, values);
        }

        
        /*public void HeartbeatEcho(string ticks, string[] joints, double[] values)
        {
            long t = long.Parse(ticks);
            try {
                HeartbeatEcho(t, joints, values);
            }catch(Exception e) {
                client.DebugException(e);
            }
        }*/
    }
}
