using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;
using CookComputing.XmlRpc;
using System.Net;
using Thalamus.BML;
using NuttyThalamus;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using JsonObjectStream;

namespace NAOThalamus
{
    internal interface INAOThalamusPublisher : IThalamusPublisher,
        INAOThalamusEvents, INuttyOutputEvents
    { }

    public interface INAOThalamus : NuttyThalamus.INuttyOutputActions, IBMLActions, INAOActions, Thalamus.BML.IWavingActions
    {}

    public class AnimationFrame
    {
        public string[] Joints { get; set; }
        public double[] Values { get; set; }
        public double[] Speed { get; set; }
        public double Time { get; set; }
        public string FrameType { get; set; }
        public double Ticks { get; set; }

        public AnimationFrame(double ticks, string[] joints, double[] values, double[] speed, double time, string frameType)
        {
            this.Joints = joints;
            this.Values = values;
            this.Speed = speed;
            this.Time = time;
            this.FrameType = frameType;
            this.Ticks = ticks;
        }

        public string SerializeToJson()
        {
            StringWriter textWriter = new StringWriter();
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        public static AnimationFrame DeserializeFromJson(string serialized)
        {
            StringReader textReader = new StringReader(serialized);
            JsonSerializer serializer = new JsonSerializer();
            return (AnimationFrame)serializer.Deserialize(textReader, typeof(AnimationFrame));
        }
    }

    
    public class NAOThalamusClient : ThalamusClient, INAOThalamus, ExternalTTS.ISpeechSoundActions, SpeakerRapport.ISpeakerRapportActions
    {

        public event NuttyThalamus.NuttyThalamusOutput.HeartbeatEchoThalamusHandler HeartbeatEcho;
        internal void NotifyHeartbeatEcho(Double ticks, string[] joints, double[] values) {
            if (HeartbeatEcho != null) HeartbeatEcho(ticks, joints, values);
        }
        string[] animationsList = new string[0];
        string[] eyeShapeList = new string[0];

        private class NAOThalamusPublisher : INAOThalamusPublisher
        {
            dynamic publisher;
            public NAOThalamusPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void SpeakFinished(string id)
            {
                publisher.SpeakFinished(id);
            }

            public void SpeakStarted(string id)
            {
                publisher.SpeakStarted(id);
            }

            public void GazeFinished(string id)
            {
                publisher.GazeFinished(id);
            }

            public void GazeStarted(string id)
            {
                publisher.GazeStarted(id);
            }

            public void WalkFinished(string id)
            {
                publisher.WalkFinished(id);
            }

            public void WalkStarted(string id)
            {
                publisher.WalkStarted(id);
            }

            public void AnimationFinished(string id)
            {
                publisher.AnimationFinished(id);
            }

            public void AnimationStarted(string id)
            {
                publisher.AnimationStarted(id);
            }

            public void SoundFinished(string id)
            {
                publisher.SoundFinished(id);
            }

            public void SoundStarted(string id)
            {
                publisher.SoundStarted(id);
            }

            public void Ping() {}

            public void SensorTouched(string sensor, bool state)
            {
                publisher.SensorTouched(sensor, state);
            }

            public void SoundSourceLocalized(double azimuth, double elevation, double confidence)
            {
                publisher.SoundSourceLocalized(azimuth, elevation, confidence);
            }

            public void VisionObjectDetected(string[] objectNames, double ratio)
            {
                publisher.VisionObjectDetected(objectNames, ratio);
            }


            public void Bookmark(string id)
            {
                publisher.Bookmark(id);
            }

            public void Viseme(int viseme, int nextViseme, double visemePercent, double nextVisemePercent)
            {
                publisher.Viseme(viseme, nextViseme, visemePercent, nextVisemePercent);
            }

            public void PointingFinished(string id)
            {
                publisher.PointingFinished(id);
            }

            public void PointingStarted(string id)
            {
                publisher.PointingStarted(id);
            }

            public void WavingFinished(string id)
            {
                publisher.WavingFinished(id);
            }

            public void WavingStarted(string id)
            {
                publisher.WavingStarted(id);
            }

            public void HeadFinished(string id)
            {
                publisher.HeadFinished(id);
            }

            public void HeadStarted(string id)
            {
                publisher.HeadStarted(id);
            }

            public void AnimationsList(string[] animations)
            {
                publisher.AnimationsList(animations);
            }

            public void EyeShapeList(string[] eyeShapes)
            {
                publisher.EyeShapeList(eyeShapes);
            }



            #region ISpeechDetectionEvents Members

            public void WordDetected(string[] words)
            {
                publisher.WordDetected(words);
            }

            #endregion
            public void HeartbeatEcho(Double ticks, string[] joints, double[] values)
            {
                publisher.HeartbeatEcho(ticks, joints, values);
            }
        }
        HttpListener NAOqiListener;
        private Thread naoListenerThread;
        private Thread naoRequestDispatcherThread;
        private Thread naoPinger;

        INAOThalamusActionPublisher NAOqiPublisher;
        private int NAOqiXmlRpcPort = 10000;
        private string tcpStreamAddress = "localhost";
        private string[] possibleIpAddresses;
        private int tcpStreamPort = 9900;

        internal INAOThalamusPublisher ThalamusPublisher;

        JsonObjectStreamServer joss;
		JsonObjectStreamClient josc;

        public NAOThalamusClient(string character = "", string pYAddress = "localhost", bool autoStart = true)
            : base("NAO", character, autoStart)
        {
            setDebug("messages", false);
            
            SetPublisher<INAOThalamusPublisher>();
            ThalamusPublisher = new NAOThalamusPublisher(Publisher);
            IPAddress[] hostEntries;
            try
            {
                hostEntries = Dns.GetHostAddresses(pYAddress);
                possibleIpAddresses = new string[hostEntries.Length];
                int i = 0;
                foreach (IPAddress ip in hostEntries)
                {
                    possibleIpAddresses[i++] = ip.ToString();
                    Debug("Found IP address for '{0}': {1}", pYAddress, possibleIpAddresses[i - 1]);
                }

                if (possibleIpAddresses.Length > 0)
                {
                    tcpStreamAddress = possibleIpAddresses[0];
                }
            }
            catch (Exception e)
            {
                DebugException(e);
                tcpStreamAddress = pYAddress;
            }

            
            NAOqiPublisher = XmlRpcProxyGen.Create<INAOThalamusActionPublisher>();
            //NAOqiPublisher.Url = string.Format("http://127.0.0.1:{0}", NAOqiXmlRpcPort);
            NAOqiPublisher.Url = string.Format("http://{0}:{1}", tcpStreamAddress, NAOqiXmlRpcPort);
            NAOqiPublisher.KeepAlive = true;
            NAOqiPublisher.Timeout = 1500;

            naoListenerThread = new Thread(new ThreadStart(NAOHttpListenerThread));
            naoListenerThread.Start();

            naoRequestDispatcherThread = new Thread(new ThreadStart(NAORequestDispatcher));
            naoRequestDispatcherThread.Start();

            naoPinger = new Thread(new ThreadStart(NAOPinger));
            naoPinger.Start();

            joss = new JsonObjectStreamServer();
            joss.StartListening(NAOqiXmlRpcPort + 3, true);
            joss.ObjectReceived += joss_ObjectReceived;

			josc = new JsonObjectStreamClient (tcpStreamAddress, NAOqiXmlRpcPort + 2);
			josc.Start ();
        }

        public class HeartbeatEchoResponse
        {
            public double Ticks { get; set; }
            public string[] Joints { get; set; }
            public double[] Values { get; set; }

            public static HeartbeatEchoResponse DeserializeFromJson(string serialized)
            {
                StringReader textReader = new StringReader(serialized);
                JsonSerializer serializer = new JsonSerializer();
                return (HeartbeatEchoResponse)serializer.Deserialize(textReader, typeof(HeartbeatEchoResponse));
            }

            public override string ToString()
            {
                string str = "Time: " + Ticks.ToString() + "; ";
                for (int i = 0; i < Joints.Length; i++)
                {
                    str += String.Format("({0}:{1}); ", Joints[i], Values[i]);
                }
                return str;
            }
        }

        void joss_ObjectReceived(string objectType, string objectJson)
        {
            switch (objectType)
            {
                case "HeartbeatEcho":
                    HeartbeatEchoResponse hbe = HeartbeatEchoResponse.DeserializeFromJson(objectJson);
                    NotifyHeartbeatEcho(hbe.Ticks, hbe.Joints, hbe.Values);
                    break;
            }
        }

        bool serviceRunning = false;
        List<HttpListenerContext> naoHttpRequestsQueue = new List<HttpListenerContext>();


        public override void Dispose()
        {
            base.Dispose();

            try
            {
                if (joss!=null) joss.Dispose();
            }
            catch { }

            try
            {
                if (josc != null) josc.Dispose();
            }
            catch { }

            try
            {
                NAOqiListener.Abort();
                if (naoListenerThread != null) naoListenerThread.Join();
            }
            catch { }
            try
            {
                if (naoRequestDispatcherThread != null) naoRequestDispatcherThread.Join();
            }
            catch { }
        }
        bool wasCconnectedToNao = false;

        internal void NAOPinger()
        {
            while (!Shutingdown)
            {
                try
                {
                    NAOqiPublisher.Ping();
                    if (!wasCconnectedToNao) NAOqiPublisher.SetAddress(LocalIPAddress());
                    wasCconnectedToNao = true;
                } catch
                {
                    wasCconnectedToNao = false;
                    DebugError("Unable to connect to NAOqiXmlRpc on address '{0}'!", tcpStreamAddress);
                }
                Thread.Sleep(3000);
            }
        }

        internal void NAOHttpListenerThread()
        {
            while (!Shutingdown)
            {
                while (!serviceRunning)
                {
                    try
                    {
                        //Thalamus.Environment.AuthorizeAddress(NAOqiXmlRpcPort + 1, "Everyone");
                        Debug("Attempt to start service on port '" + NAOqiXmlRpcPort+1 + "'");
                        NAOqiListener = new HttpListener();
                        NAOqiListener.Prefixes.Add(string.Format("http://*:{0}/", NAOqiXmlRpcPort + 1));
                        NAOqiListener.Start();
                        Debug("XMLRPC Listening on port " + NAOqiXmlRpcPort + 1);
                        serviceRunning = true;
                    }
                    catch
                    {
                        Debug("Port unavaliable.");
                        serviceRunning = false;
                        Thread.Sleep(1000);
                    }
                }

                try
                {
                    HttpListenerContext context = NAOqiListener.GetContext();
                    lock (naoHttpRequestsQueue)
                    {
                        naoHttpRequestsQueue.Add(context);
                    }
                }
                catch (Exception e)
                {
                    DebugException(e);
                    serviceRunning = false;
                    if (NAOqiListener != null) NAOqiListener.Close();
                }
            }
            if (NAOqiListener != null) NAOqiListener.Close();
            Debug("Terminated NAOHttpListenerThread");
        }

        internal void NAORequestDispatcher()
        {
            while (!Shutingdown)
            {
                bool performSleep = true;
                try
                {
                    if (naoHttpRequestsQueue.Count > 0)
                    {
                        performSleep = false;
                        List<HttpListenerContext> httpRequests;
                        lock (naoHttpRequestsQueue)
                        {
                            httpRequests = new List<HttpListenerContext>(naoHttpRequestsQueue);
                            naoHttpRequestsQueue.Clear();
                        }
                        foreach (HttpListenerContext r in httpRequests)
                        {
                            ProcessNAORequest(r);
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugException(e);
                }
				if (performSleep) Thread.Sleep(10);
            }
            Debug("Terminated NAORequestDispatcherThread");
        }

        internal void ProcessNAORequest(object oContext)
        {
            try
            {
                XmlRpcListenerService svc = new NAOThalamusEventListener(this);
                svc.ProcessRequest((HttpListenerContext)oContext);
            }
            catch (Exception e)
            {
                DebugException(e);
            }

        }

        void ISpeakActions.Speak(string id, string text)
        {
            try
            {
                DebugIf("messages", "Sending NAO Speak {0}...", id);
                NAOqiPublisher.Speak(id, text);
                DebugIf("messages", "OK Speak {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ISpeakActions.SpeakBookmarks(string id, string[] text, string[] bookmarks)
        {
            try 
            {
                DebugIf("messages", "Sending NAO SpeakBookmarks {0}...", id);
                NAOqiPublisher.SpeakBookmarks(id, text, bookmarks);
                DebugIf("messages", "OK SpeakBookmarks {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IGazeActions.Gaze(string id, double horizontal, double vertical, double speed, bool trackFaces)
        {
            try
            {
                DebugIf("messages", "Sending NAO Gaze {0}...", id);
                NAOqiPublisher.Gaze(id, horizontal, vertical, speed, trackFaces);
                DebugIf("messages", "OK Gaze {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ILocomotionActions.WalkTo(string id, double x, double y, double angle)
        {
            try
            {
                NAOqiPublisher.WalkTo(id, x, y, angle);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ILocomotionActions.WalkToTarget(string id, string target)
        {
            try
            {
                NAOqiPublisher.WalkToTarget(id, target);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ILocomotionActions.StopWalk()
        {
            try
            {
                NAOqiPublisher.StopWalk();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IAnimationActions.StopAnimation(string id)
        {
            try
            {
                NAOqiPublisher.StopAnimation(id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IAnimationActions.PlayAnimation(string id, string animation)
        {
            try
            {
                DebugIf("messages", "Sending NAO PlayAnimation {0}...", id);
                NAOqiPublisher.PlayAnimation(id, animation);
                DebugIf("messages", "OK PlayAnimation {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IPostureActions.ResetPose()
        {
            try
            {
                NAOqiPublisher.ResetPose();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IPostureActions.SetPosture(string id, string posture, double percent, double decay)
        {
            try
            {
                NAOqiPublisher.SetPosture(id, posture, percent, decay);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ISoundActions.PlaySound(string id, string SoundName, double Volume, double Pitch)
        {
            try
            {
                NAOqiPublisher.PlaySound(id, SoundName, Volume, Pitch);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ISoundActions.PlaySoundLoop(string id, string SoundName, double Volume, double Pitch)
        {
            try
            {
                NAOqiPublisher.PlaySoundLoop(id, SoundName, Volume, Pitch);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void ISoundActions.StopSound(string id)
        {
            try
            {
                NAOqiPublisher.StopSound(id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IFaceActions.FaceFacs(string id, int AU, Thalamus.Actions.Side Side, double Intensity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IFaceActions.FaceShiftLexeme(string id, string lexeme)
        {
            try
            {
                NAOqiPublisher.FaceShiftLexeme(id, lexeme);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IFaceActions.FaceLexeme(string id, string lexeme)
        {
            try
            {
                NAOqiPublisher.FaceLexeme(id, lexeme);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void IFaceActions.NeutralFaceExpression()
        {
            try
            {
                NAOqiPublisher.FaceLexeme("", "neutral");
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }


        public void Animate(double ticks, string[] joints, double[] values, double[] speed, double time, string frameType)
        {
			AnimationFrame af = new AnimationFrame (ticks, joints, values, speed, time, frameType);
			josc.Send (af.GetType(), af.SerializeToJson());
        }
        void INuttyOutputActions.Animate(double ticks, string[] joints, double[] values, double[] speed, double time, string frameType)
        {
            Animate(ticks, joints, values, speed, time, frameType);
        }


        void INAOActions.SetIdleBehavior(bool idleState)
        {
            try
            {
                NAOqiPublisher.SetIdleBehavior(idleState);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        //TODO May be better if asyncronous 
        public void SpeakWaveFile(string speechData_serialized)
        {
            Console.WriteLine("SpeakWaveFile()");
            SpeechDataPlayer spd = new SpeechDataPlayer(speechData_serialized);
            spd.Started += spd_started;
            spd.Finished += spd_finished;
            spd.CharacteristicReached += spd_CharacteristicReached;
            spd.Play();

        }

        void spd_CharacteristicReached(object sender, SpeechDataPlayer.SpeechCharacteristicReachedEventArgs e)
        {
            Console.WriteLine("Characteristic reached! " + e.characteristic);
            switch (e.characteristic.type)
            {
                case SpeechSoundDataStructures.SpeechData.SpeechCharacteristic.Types.Bookmark:
                    SpeechSoundDataStructures.SpeechData.SpeechBookmark b = (SpeechSoundDataStructures.SpeechData.SpeechBookmark)e.characteristic;
                    ThalamusPublisher.Bookmark(b.name);
                    break;
                case SpeechSoundDataStructures.SpeechData.SpeechCharacteristic.Types.Viseme:
                    SpeechSoundDataStructures.SpeechData.SpeechViseme v = (SpeechSoundDataStructures.SpeechData.SpeechViseme)e.characteristic;
                    ThalamusPublisher.Viseme(v.value, v.nextViseme, v.percent, v.nextPercent);
                    break;
            }

        }

        void spd_finished(object sender, NAOThalamus.SpeechDataPlayer.SpeechDataPlayerEventArgs e)
        {
            ThalamusPublisher.SpeakFinished(e.speechData.id);
        }

        void spd_started(object sender, NAOThalamus.SpeechDataPlayer.SpeechDataPlayerEventArgs e)
        {
            ThalamusPublisher.SpeakStarted(e.speechData.id);
        }

        public void PointingAngle(string id, double horizontal, double vertical, double speed = 1, Thalamus.Actions.PointingMode mode = Thalamus.Actions.PointingMode.RightHand)
        {
            try
            {
                DebugIf("messages", "Sending NAO Pointing {0}...", id);
                NAOqiPublisher.PointingNao(id, horizontal, vertical, speed, mode.ToString());
                DebugIf("messages", "OK Pointing {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Pointing(string id, string target, double speed = 1, Thalamus.Actions.PointingMode mode = Thalamus.Actions.PointingMode.RightHand)
        {
            try
            {
                DebugIf("messages", "Sending NAO Pointing (Zero) {0}...", id);
                NAOqiPublisher.PointingNao(id, 0, 0, speed, mode.ToString());
                DebugIf("messages", "OK Pointing (Zero) {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public void Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, Thalamus.Actions.PointingMode mode = Thalamus.Actions.PointingMode.RightHand)
        {
            try
            {
                DebugIf("messages", "Sending NAO Waving {0}...", id);
                NAOqiPublisher.WavingNao(id, horizontal, vertical, frequency, amplitude, duration, mode.ToString());
                DebugIf("messages", "OK Waving {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }
        
        public void Head(string id, string lexeme, int repetitions, double amplitude, double frequency)
        {
            try
            {
                DebugIf("messages", "Sending NAO Head {0}...", id);
                NAOqiPublisher.Head(id, lexeme.ToUpper(), repetitions, amplitude, frequency);
                DebugIf("messages", "OK Head {0}.", id);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public override void ConnectedToMaster()
        {
            base.ConnectedToMaster();
            ThalamusPublisher.AnimationsList(this.animationsList);
            ThalamusPublisher.EyeShapeList(this.eyeShapeList);
        }

        void IAnimationActions.PlayAnimationQueued(string id, string animation)
        {
            try
            {
                NAOqiPublisher.PlayAnimationQueued(id, animation);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        internal void AnimationsList(string[] animations)
        {
            this.animationsList = animations;
            if (IsConnected) ThalamusPublisher.AnimationsList(this.animationsList);
        }

        public void SpeakStop()
        {
            try
            {
                NAOqiPublisher.SpeakStop();
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }


        void INAOActions.EyesIntensity(double value)
        {
            try
            {
                NAOqiPublisher.EyesIntensity(value);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }


        void ISpeakActions.SpeakStop()
        {
            SpeakStop();
        }


        void IPointingActions.Pointing(string id, string target, double speed, Thalamus.Actions.PointingMode mode)
        {
            Pointing(id, target, speed, mode);
        }

        void IPointingActions.PointingAngle(string id, double horizontal, double vertical, double speed, Thalamus.Actions.PointingMode mode)
        {
            PointingAngle(id, horizontal, vertical, speed, mode);
        }


        void IHeadActions.Head(string id, string lexeme, int repetitions, double amplitude, double frequency)
        {
            Head(id, lexeme, repetitions, amplitude, frequency);
        }

        void INAOActions.SpeakerVolume(double value)
        {
            try
            {
                NAOqiPublisher.SpeakerVolume(value);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        internal void EyeShapeList(string[] eyeShapes)
        {
            this.eyeShapeList = eyeShapes;
            if (IsConnected) ThalamusPublisher.EyeShapeList(this.eyeShapeList);
        }

        /*void INAOActions.EyeShapeExpression(string eyeShape)
        {
            try
            {
                NAOqiPublisher.EyeShapeExpression(eyeShape);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void INAOActions.EyeShapeShift(string eyeShape)
        {
            try
            {
                NAOqiPublisher.EyeShapeShift(eyeShape);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }*/

        void INAOActions.EyeBlink(int count)
        {
            try
            {
                NAOqiPublisher.EyeBlink(count);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }

        void INAOActions.SlowEyeBlink(int count)
        {
            try
            {
                NAOqiPublisher.SlowEyeBlink(count);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }
       
        void IWavingActions.Waving(string id, double horizontal, double vertical, double frequency, double amplitude, double duration, Thalamus.Actions.PointingMode mode)
        {
            Waving(id, horizontal, vertical, frequency, amplitude, duration, mode);
        }

        void SpeakerRapport.ISpeakerRapportActions.SetSpeakingVolume(double percent)
        {
            try
            {
                DebugIf("messages", "Sending SpeakerVolume {0}...", percent);
                NAOqiPublisher.SpeakerVolume(percent);
                DebugIf("messages", "OK SpeakerVolume {0}.", percent);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }


        /*
        public void Heartbeat(Double ticks) 


        
                NAOqiPublisher.Heartbeat(ticks);
        void INuttyOutputActions.Heartbeat(Double ticks)
        {
            Heartbeat(ticks);
        }*/
    }
}
