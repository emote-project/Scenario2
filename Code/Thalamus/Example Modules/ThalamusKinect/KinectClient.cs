using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thalamus;
using Microsoft.Kinect;
using System.IO;
using System.Threading;

namespace ThalamusKinect
{
    public interface IKinectEvents : Thalamus.IPerception
    {
        void SoundDirectionLocated(double angle, double confidence);
    }

    public class KinectClient : ThalamusClient
    {
        #region Publishing

        public interface IKinectClientPublisher : IThalamusPublisher,
            ThalamusKinect.IKinectEvents
        { }

        private class KinectPublisher : IKinectClientPublisher
        {
            /*
             * 	This class serves as a wrapper to the dynamic publisher, 
                because the dynamic publisher will not know the interface definitions and therefore does not provide auto-complete.
                By wrapping the publisher in this class we can use this class instead of using the publisher directly.
                */
            #region Generic code (repeat in all publisher wrappers)
            // Save the dynamic publisher
            dynamic publisher;
            public KinectPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }
            #endregion

            #region IKinectEvents Members

            public void SoundDirectionLocated(double angle, double confidence)
            {
                publisher.SoundDirectionLocated(angle, confidence);
            }

            #endregion
        }

        #endregion

        private DateTime lastSpeechDirectionTime = DateTime.Now;
        private double lastSpeechDirection = double.MaxValue;

        public KinectSensor sensor;
        public KinectAudioSource source;
        private Mutex SoundDirectionMutex = new Mutex();
        private double repeatedDirectionInterval = 20;
        private double repeatedDirectionAngle = 5;
        private double confidenceThreshold = 0.5;
        private bool started = false;

        KinectPublisher kinectPublisher;

        public KinectClient()
            : base("KinectClient")
        {
            SetPublisher<IKinectClientPublisher>();
            kinectPublisher = new KinectPublisher(Publisher);
        }

        public override void ConnectedToMaster()
        {
            Console.WriteLine("Starting...");
            sensor = (from sensorToCheck in KinectSensor.KinectSensors where sensorToCheck.Status == KinectStatus.Connected select sensorToCheck).FirstOrDefault();
            if (sensor == null)
            {
                Console.WriteLine("\n Kinect sensor not detected !\n");
                return;
            }

            source = sensor.AudioSource;
            source.AutomaticGainControlEnabled = true;

            try { sensor.Start(); }
            catch (Exception)
            {
                Console.WriteLine("Sensor cannot Start! \n");
                return;
            }

            source.Start();
            started = true;
            source.SoundSourceAngleChanged += delegate
            {
                if (!IsConnected) return;
                bool send = false;
                
                if (source.SoundSourceAngleConfidence > confidenceThreshold)
                {
                    if (SoundDirectionMutex.WaitOne(0))
                    {
                        if ((Math.Abs(lastSpeechDirection - source.SoundSourceAngle) < repeatedDirectionAngle))
                        {
                            if ((System.DateTime.Now - lastSpeechDirectionTime).TotalSeconds > repeatedDirectionInterval) send = true;
                        }
                        else
                        {
                            send = true;
                        }

                        if (send)
                        {
                            Console.WriteLine("SoundLocated{ angle: " + source.SoundSourceAngle + "; confidence: " + source.SoundSourceAngleConfidence);
                            kinectPublisher.SoundDirectionLocated(source.SoundSourceAngle, source.SoundSourceAngleConfidence);
                            lastSpeechDirectionTime = DateTime.Now;
                            lastSpeechDirection = source.SoundSourceAngle;
                        }
                        SoundDirectionMutex.ReleaseMutex();
                    }
                }
            };
            return;
        }

        public override void Dispose()
        {
            if (started)
            {
                Console.WriteLine("Disposing Kinect...");
                sensor.Stop();  
            }
            base.Dispose();
        }
    }
}
