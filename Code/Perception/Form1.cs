using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Globalization;
using System.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;


namespace Perception
{
    public partial class Form1 : Form
    {
        public static System.Timers.Timer aTimer;
        public static System.Timers.Timer saver;
        public static MicroLibrary.MicroTimer videoTimer;
        string RxString;
        string RxString2;
        private PipeServer.Server pipeServer;
        private PipeServer.Server engagementpipe;

        public PerceptionClient thalamusClient;
        private System.IO.Ports.SerialPort serialPort1;
        private System.IO.Ports.SerialPort serialPort2;
        public static int SessionID = 0;
        public static Boolean isEmpathic;
        //confidence values
        private double confX_1V = 0;
        private double confY_1V = 0;
        private int GazeConfidence1 = 0;

        //for second user
        private double confX_2V = 0;
        private double confY_2V = 0;
        private int GazeConfidence2 = 0;


        //okao data
        int smile1 = 0;
        int smile2 = 0;
        int fupdown1 = 0;
        int fleftright1 = 0;
        int fupdown2 = 0;
        int fleftright2 = 0;

        
        int confidence1 = 0;
        int confidence2 = 0;
        int o1Expression1 = 0;
        int o1Expression2 = 0;
        int o1Expression3 = 0;
        int o1Expression4 = 0;
        int o1Expression5 = 0;
        int o1Expression6 = 0;
        int o1Expression7 = 0;
        int o2Expression1 = 0;
        int o2Expression2 = 0;
        int o2Expression3 = 0;
        int o2Expression4 = 0;
        int o2Expression5 = 0;
        int o2Expression6 = 0;
        int o2Expression7 = 0;

        string engagement = "";
        double Xp;
        double Yp;
        double Zp;
        double Xpold;
        double Ypold;
        double Zpold;
        double Xp2old;
        double Yp2old;
        double Zp2old;
      
        #region kinect2 variables
        static double rotY_1 = 0;
        static double rotX_1 = 0;
        static double rotZ_1 = 0;

        static double rotX_2 = 0;
        
        static double rotY_2 = 0;
        static double rotZ_2 = 0;
        static double locX_1 = 0;
        static double locY_1 = 0;
        static double locZ_1 = 0;
        static double locX_2 = 0;
        static double locY_2 = 0;
        static double locZ_2 = 0;
        static double lefthandX_1 = 0;
        static double lefthandY_1 = 0;
        static double lefthandZ_1 = 0;
        static double righthandX_1 = 0;
        static double righthandY_1 = 0;
        static double righthandZ_1 = 0;

        static double righthandX_2 = 0;
        static double righthandY_2 = 0;
        static double righthandZ_2 = 0;
        static double lefthandX_2 = 0;
        static double lefthandY_2 = 0;
        static double lefthandZ_2 = 0;

        static double chinPosX_1 = 0;
        static double chinPosY_1 = 0;
        static double chinPosZ_1 = 0;

        static double chinPosX_2 = 0;
        static double chinPosY_2 = 0;
        static double chinPosZ_2 = 0;
        static bool chin1touched = false;
        static bool chin2touched = false;

        public bool touchscreen1 = false;
        public bool touchscreen2 = false;

        static double eyebrowleft_1 = 0;
        static double eyebrowleft_2 = 0;
        static double eyebrowright_1 = 0;
        static double eyebrowright_2 = 0;

        static string happy1 = "";
        static string happy2 = "";

        static string mouth1 = "";
        static string mouth2 = "";

        //image definitions
        public Image<Bgra, byte> img1;
        public Image<Bgr, byte> img3;
        public Image<Bgr, byte> old;


        public class KinectData
        {
            public AU mAU;
            public string leanX="0";
            public string leanY="0";

            public KinectData()
            {
                mAU = new AU();
            }
        }
        public class AU
        {
            public string JawOpen="0";
            public string JawSlideRight="0";
            public string LeftcheekPuff="0";
            public string LefteyebrowLowerer="0";
            public string LefteyeClosed="0";
            public string LipCornerDepressorLeft="0";
            public string LipCornerDepressorRight="0";
            public string LipCornerPullerLeft="0";
            public string LipCornerPullerRight="0";
            public string LipPucker="0";
            public string LipStretcherLeft="0";
            public string LipStretcherRight="0";
            public string LowerlipDepressorLeft="0";
            public string LowerlipDepressorRight="0";
            public string RightcheekPuff="0";
            public string RighteyebrowLowerer="0";
            public string RighteyeClosed="0";
        }
        public KinectData mKinectData;
        public KinectData mKinectData2;
        

        #endregion

        static bool gazed = false;
        double RotH;
        double RotV;
        double Xp2;
        double Yp2;
        bool sentonce;
        double Zp2;
        double RotH2;
        double RotV2;
        
        
                
        double AU4BrowLower;
        double AU2BrowRaiser;

        public static bool closing;
        double depth;
        public static int scenarioselected;
        Boolean DetectedPerson;
        Boolean DetectedPerson2;
        public SoundPlayer simpleSound;
        public static Boolean startstop;
        public static string Fparticipantname;
        public static string Fparticipantname2;

        public static long FparticipanID;
        public static long FparticipanID2;

        public static String Kinectdata;
        public static String Kinectdata2; //kinect data for user2

        public static String Qdata1;
        public static String Qdata2;

        public static String OKAOdata;
        public static String OKAOdata2; //okao data for user2

        public static String allkinectdata;  //user1 data
        public static String allkinectdata2; //user2 data

        public static String allQdata;  //user1 data
        public static String allQdata2; //user2 data

        public static String allOKAOdata;  //user1 data
        public static String allOKAOdata2; //user2 data

        //Qsensor variables
        public double qArousal1=0;
        public double qtemp1 = 0;
        public double qAccelX1 = 0;
        public double qAccelY1 = 0;
        public double qAccelZ1 = 0;
        public double qArousal2 = 0;
        public double qtemp2 = 0;
        public double qAccelX2 = 0;
        public double qAccelY2 = 0;
        public double qAccelZ2 = 0;
        //gaze enum
        public enum GazeDef
        {
            screenL,
            screenR,
            Robot,
            elsewhere,
            none
        }
        public GazeDef GazeOut1;
        public GazeDef GazeOut2;

        public GazeDef LastGazeOut1; //last value of user gaze to avoid repeating messages
        public GazeDef LastGazeOut2;

        EyebrowsController eyebrowControllerUsr1;
        EyebrowsController eyebrowControllerUsr2;

        #region Kinect2 section
        IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;
        //kinect2 bits
        private KinectSensor sensor = null;
        private BodyFrameSource bodySource = null;
        private HighDefinitionFaceFrameSource highDefinitionFaceFrameSource = null;
        private HighDefinitionFaceFrameReader highDefinitionFaceFrameReader = null;
        private HighDefinitionFaceFrameSource highDefinitionFaceFrameSource2 = null;
        private HighDefinitionFaceFrameReader highDefinitionFaceFrameReader2 = null;

        private byte[] pixels = null;
        private FaceAlignment currentFaceAlignment = null;
        private FaceFrameReader faceReader = null;
        private FaceFrameSource faceSource = null;
        private FaceFrameReader faceReader2 = null;
        private FaceFrameSource faceSource2 = null;
        private string kin1_1 = null;
        private string kin2_1 = null;
        private string kin3_1 = null;
        private string kin1_2 = null;
        private string kin2_2 = null;
        private string kin3_2 = null;

        private VideoWriter videoOut = null;
        

        private ulong currentTrackingId = 0;
        private MultiSourceFrameReader reader;
        private Body[] bodies;
        private string currentBuilderStatus = string.Empty;
        private const FaceFrameFeatures DefaultFaceFrameFeatures = FaceFrameFeatures.PointsInColorSpace
                                | FaceFrameFeatures.Happy
                                | FaceFrameFeatures.FaceEngagement
                                | FaceFrameFeatures.Glasses
                                | FaceFrameFeatures.LeftEyeClosed
                                | FaceFrameFeatures.RightEyeClosed
                                | FaceFrameFeatures.MouthOpen
                                | FaceFrameFeatures.MouthMoved
                                | FaceFrameFeatures.LookingAway
                                | FaceFrameFeatures.RotationOrientation;

        private ulong CurrentTrackingId
        {
            get
            {
                return this.currentTrackingId;
            }

            set
            {
                this.currentTrackingId = value;
            }
        }
        private static double VectorLength(CameraSpacePoint point)
        {
            var result = Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2);

            result = Math.Sqrt(result);

            return result;
        }
        public void InitializeHDFace()
        {
          
            this.sensor = KinectSensor.GetDefault();
            this.bodySource = this.sensor.BodyFrameSource;
            FrameDescription colorFrameDescription = this.sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            
            this.highDefinitionFaceFrameSource = new HighDefinitionFaceFrameSource(sensor);
            this.highDefinitionFaceFrameSource.TrackingIdLost += this.HdFaceSource_TrackingIdLost;

            this.highDefinitionFaceFrameReader = this.highDefinitionFaceFrameSource.OpenReader();
            this.highDefinitionFaceFrameReader.FrameArrived += this.HdFaceReader_FrameArrived; //event gor high def face

            if (scenarioselected == 2)
            {
                this.highDefinitionFaceFrameSource2 = new HighDefinitionFaceFrameSource(sensor);
                this.highDefinitionFaceFrameSource2.TrackingIdLost += this.HdFaceSource_TrackingIdLost2;

                this.highDefinitionFaceFrameReader2 = this.highDefinitionFaceFrameSource2.OpenReader();
                this.highDefinitionFaceFrameReader2.FrameArrived += this.HdFaceReader_FrameArrived2; //event gor high def face
                faceSource2 = new FaceFrameSource(sensor, 0, DefaultFaceFrameFeatures);
                faceReader2 = faceSource2.OpenReader();
                faceReader2.FrameArrived += OnFaceFrameArrived2; //event for face data
                faceSource2.TrackingIdLost += OnTrackingIdLost2;

            }

            this.reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
            
            this.reader.MultiSourceFrameArrived += OnMultiSourceFrameArrived; //event for multiple source (Position)
            this.currentFaceAlignment = new FaceAlignment();

            faceSource = new FaceFrameSource(sensor, 0, DefaultFaceFrameFeatures);
            faceReader = faceSource.OpenReader();
            faceReader.FrameArrived += OnFaceFrameArrived; //event for face data
            
            faceSource.TrackingIdLost += OnTrackingIdLost;
            this.pixels = new byte[colorFrameDescription.Width * colorFrameDescription.Height * colorFrameDescription.BytesPerPixel];
            this.sensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;
            this.sensor.Open();
            
        }
        
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            //if (sensor.IsAvailable == false)
            //    MessageBox.Show("Kinect disconnected!!");
            kinectstatus.Invoke((MethodInvoker)delegate() { kinectstatus.Text = this.sensor.IsAvailable.ToString(); });
        }

        public void saveframe(object sender,
                                  MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            if (videoOut.Ptr != IntPtr.Zero && videoOut != null)
            {
                try
                {
                    if (img3 == null)
                        videoOut.WriteFrame(old); //write old frame in case img3 is not pulled yet
                    else
                    {
                        videoOut.WriteFrame(img3);
                        old = img3;
                        img3 = null;
                    }


                }
                catch { }             //   img3.Dispose();
            }
        }

        public void OnMultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (kinectenable.Checked == false) return;
            var frame = e.FrameReference.AcquireFrame();
            if (frame == null) return;

            if (videocheck.Checked == true && startstop == true)
            {
                using (var colorFrame = frame.ColorFrameReference.AcquireFrame())
                {
                    // Open color frame for video saving
                    if (colorFrame != null)
                    {

                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                            colorFrame.CopyRawFrameDataToArray(pixels);
                        else
                            colorFrame.CopyConvertedFrameDataToArray(this.pixels, ColorImageFormat.Bgra);
                        img1 = new Image<Bgra, byte>(1920, 1080);
                        img1.Bytes = this.pixels;
                        img3 = img1.Convert<Bgr, byte>(); //convert to compatible format
                        img1.Dispose();

                    }
                }
            } 
            // BodyFrame
            using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null)
                        bodies = new Body[bodyFrame.BodyCount];


                    bodyFrame.GetAndRefreshBodyData(bodies);

                    foreach (var body in bodies)
                    {
                        if (body.IsTracked)
                        {
                            var target = body;

                            
                                if (target.Joints[JointType.Head].Position.X < 0 || scenarioselected==1) //left user or one user only
                                {
                                    
                                    
                                    locX_1 = target.Joints[JointType.Head].Position.X;
                                    locY_1 = target.Joints[JointType.Head].Position.Y;
                                    locZ_1 = target.Joints[JointType.Head].Position.Z;
                                    
                                    lefthandX_1 = target.Joints[JointType.HandTipLeft].Position.X;
                                    lefthandY_1 = target.Joints[JointType.HandTipLeft].Position.Y;
                                    lefthandZ_1 = target.Joints[JointType.HandTipLeft].Position.Z;
                                    righthandX_1 = target.Joints[JointType.HandTipRight].Position.X;
                                    righthandY_1 = target.Joints[JointType.HandTipRight].Position.Y;
                                    righthandZ_1 = target.Joints[JointType.HandTipRight].Position.Z;
                                    chinPosX_1 = locX_1;
                                    chinPosY_1 = locY_1;
                                    chinPosZ_1 = locZ_1;
                                    mKinectData.leanX = target.Lean.X.ToString();
                                    mKinectData.leanY = target.Lean.Y.ToString();

                                    this.faceSource.TrackingId = target.TrackingId;
                                    this.highDefinitionFaceFrameSource.TrackingId = target.TrackingId;
                                    
                                    
                                    DetectedPerson = true;
                                    
                                    kin2_1 = "Kin2_1," + target.Joints[JointType.Head].Position.X.ToString() + "," + target.Joints[JointType.Head].Position.Y.ToString() + "," + target.Joints[JointType.Head].Position.Z.ToString() + "," + target.Lean.X.ToString() + "," + target.Lean.Y.ToString() + "," + target.LeanTrackingState.ToString();
                                }
                                else //right user
                                {
                                    
                                    locX_2 = target.Joints[JointType.Head].Position.X;
                                    locY_2 = target.Joints[JointType.Head].Position.Y;
                                    locZ_2 = target.Joints[JointType.Head].Position.Z;
                                    
                                    DetectedPerson2 = true;
                                    lefthandX_2 = target.Joints[JointType.HandTipLeft].Position.X;
                                    lefthandY_2 = target.Joints[JointType.HandTipLeft].Position.Y;
                                    lefthandZ_2 = target.Joints[JointType.HandTipLeft].Position.Z;
                                    righthandX_2 = target.Joints[JointType.HandTipRight].Position.X;
                                    righthandY_2 = target.Joints[JointType.HandTipRight].Position.Y;
                                    righthandZ_2 = target.Joints[JointType.HandTipRight].Position.Z;
                                    chinPosX_2 = locX_2;
                                    chinPosY_2 = locY_2;
                                    chinPosZ_2 = locZ_2;
                                    mKinectData2.leanX = target.Lean.X.ToString();
                                    mKinectData2.leanY = target.Lean.Y.ToString();
                                    
                                    kin2_2 = "Kin2_2," + target.Joints[JointType.Head].Position.X.ToString() + "," + target.Joints[JointType.Head].Position.Y.ToString() + "," + target.Joints[JointType.Head].Position.Z.ToString() + "," + target.Lean.X.ToString() + "," + target.Lean.Y.ToString() + "," + target.LeanTrackingState.ToString();
                                    this.faceSource2.TrackingId = target.TrackingId;
                                    this.highDefinitionFaceFrameSource2.TrackingId = target.TrackingId;
                                }
                                
                                
                            
                        }
                    }
                    if (scenarioselected == 2)
                    {
                        if (this.faceSource2.TrackingId == this.faceSource.TrackingId)//if id are the same then refresh them
                        {
                            this.faceSource.TrackingId = 0;
                            this.highDefinitionFaceFrameSource.TrackingId = 0;
                            this.faceSource2.TrackingId = 0;
                            this.highDefinitionFaceFrameSource2.TrackingId = 0;

                        }
                    }
                }
            }
            
        }

        private static void ExtractFaceRotationInDegrees(Vector4 rotQuaternion, out int pitch, out int yaw, out int roll)
        {
            double x = rotQuaternion.X;
            double y = rotQuaternion.Y;
            double z = rotQuaternion.Z;
            double w = rotQuaternion.W;

            double yawD, pitchD, rollD;
            pitchD = Math.Atan2(2 * ((y * z) + (w * x)), (w * w) - (x * x) - (y * y) + (z * z)) / Math.PI * 180.0;
            yawD = Math.Asin(2 * ((w * y) - (x * z))) / Math.PI * 180.0;
            rollD = Math.Atan2(2 * ((x * y) + (w * z)), (w * w) + (x * x) - (y * y) - (z * z)) / Math.PI * 180.0;

            double increment = 1;
            pitch = (int)((pitchD + ((increment / 2.0) * (pitchD > 0 ? 1.0 : -1.0))) / increment) * (int)increment;
            yaw = (int)((yawD + ((increment / 2.0) * (yawD > 0 ? 1.0 : -1.0))) / increment) * (int)increment;
            roll = (int)((rollD + ((increment / 2.0) * (rollD > 0 ? 1.0 : -1.0))) / increment) * (int)increment;
        }
        private void HdFaceSource_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            var lostTrackingID = e.TrackingId;

            if (this.CurrentTrackingId == lostTrackingID)
            {
                this.CurrentTrackingId = 0;


                this.highDefinitionFaceFrameSource.TrackingId = 0;
                DetectedPerson = false;


            }
        }
        private void HdFaceSource_TrackingIdLost2(object sender, TrackingIdLostEventArgs e)
        {
            var lostTrackingID = e.TrackingId;

            if (this.CurrentTrackingId == lostTrackingID)
            {
                this.CurrentTrackingId = 0;


                this.highDefinitionFaceFrameSource2.TrackingId = 0;

                DetectedPerson2 = false;

            }
        }
        public void HdFaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            if (kinectenable.Checked == false) return;
            using (var frame = e.FrameReference.AcquireFrame())
            {
                // We might miss the chance to acquire the frame; it will be null if it's missed.
                // Also ignore this frame if face tracking failed.
                if (frame == null || !frame.IsFaceTracked)
                {
                    return;
                }

                frame.GetAndRefreshFaceAlignmentResult(this.currentFaceAlignment);
                mKinectData.mAU.JawOpen = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.JawOpen].ToString("0.000");
                mKinectData.mAU.JawSlideRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.JawSlideRight].ToString("0.000");
                mKinectData.mAU.LeftcheekPuff = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LeftcheekPuff].ToString("0.000");
                mKinectData.mAU.LefteyebrowLowerer = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LefteyebrowLowerer].ToString("0.000");
                mKinectData.mAU.LefteyeClosed = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LefteyeClosed].ToString("0.000");
                mKinectData.mAU.LipCornerDepressorLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerDepressorLeft].ToString("0.000");
                mKinectData.mAU.LipCornerDepressorRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerDepressorRight].ToString("0.000");
                mKinectData.mAU.LipCornerPullerLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerPullerLeft].ToString("0.000");
                mKinectData.mAU.LipCornerPullerRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerPullerRight].ToString("0.000");
                mKinectData.mAU.LipPucker = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipPucker].ToString("0.000");
                mKinectData.mAU.LipStretcherLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipStretcherLeft].ToString("0.000");
                mKinectData.mAU.LipStretcherRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipStretcherRight].ToString("0.000");
                mKinectData.mAU.LowerlipDepressorLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LowerlipDepressorLeft].ToString("0.000");
                mKinectData.mAU.LowerlipDepressorRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LowerlipDepressorRight].ToString("0.000");
                mKinectData.mAU.RightcheekPuff = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RightcheekPuff].ToString("0.000");
                mKinectData.mAU.RighteyebrowLowerer = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RighteyebrowLowerer].ToString("0.000");
                mKinectData.mAU.RighteyeClosed = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RighteyeClosed].ToString("0.000");
                
                eyebrowleft_1 = Convert.ToDouble(mKinectData.mAU.LefteyebrowLowerer);
                eyebrowright_1 = Convert.ToDouble(mKinectData.mAU.RighteyebrowLowerer);


                kin3_1 = "kin3_1," + mKinectData.mAU.JawOpen.ToString() + "," + mKinectData.mAU.JawSlideRight.ToString() + "," + mKinectData.mAU.LeftcheekPuff.ToString() + "," + mKinectData.mAU.LefteyebrowLowerer.ToString() + "," + mKinectData.mAU.LefteyeClosed.ToString() + "," + mKinectData.mAU.LipCornerDepressorLeft.ToString() + "," + mKinectData.mAU.LipCornerDepressorRight.ToString() + "," + mKinectData.mAU.LipCornerPullerLeft.ToString() + "," + mKinectData.mAU.LipCornerPullerRight.ToString() + "," + mKinectData.mAU.LipPucker.ToString() + "," + mKinectData.mAU.LipStretcherLeft.ToString() + "," + mKinectData.mAU.LipStretcherRight.ToString() + "," + mKinectData.mAU.LowerlipDepressorLeft.ToString() + "," + mKinectData.mAU.LowerlipDepressorRight.ToString() + "," + mKinectData.mAU.RightcheekPuff.ToString() + "," + mKinectData.mAU.RighteyebrowLowerer.ToString() + "," + mKinectData.mAU.RighteyeClosed.ToString();
            }
        }
        public void HdFaceReader_FrameArrived2(object sender, HighDefinitionFaceFrameArrivedEventArgs e) //right user
        {
            if (kinectenable.Checked == false) return;
            using (var frame = e.FrameReference.AcquireFrame())
            { 
                // We might miss the chance to acquire the frame; it will be null if it's missed.
                // Also ignore this frame if face tracking failed.
                if (frame == null || !frame.IsFaceTracked)
                {
                    return;
                }
                
                frame.GetAndRefreshFaceAlignmentResult(this.currentFaceAlignment);

                mKinectData2.mAU.JawOpen = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.JawOpen].ToString("0.000");
                mKinectData2.mAU.JawSlideRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.JawSlideRight].ToString("0.000");
                mKinectData2.mAU.LeftcheekPuff = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LeftcheekPuff].ToString("0.000");
                mKinectData2.mAU.LefteyebrowLowerer = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LefteyebrowLowerer].ToString("0.000");
                mKinectData2.mAU.LefteyeClosed = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LefteyeClosed].ToString("0.000");
                mKinectData2.mAU.LipCornerDepressorLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerDepressorLeft].ToString("0.000");
                mKinectData2.mAU.LipCornerDepressorRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerDepressorRight].ToString("0.000");
                mKinectData2.mAU.LipCornerPullerLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerPullerLeft].ToString("0.000");
                mKinectData2.mAU.LipCornerPullerRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipCornerPullerRight].ToString("0.000");
                mKinectData2.mAU.LipPucker = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipPucker].ToString("0.000");
                mKinectData2.mAU.LipStretcherLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipStretcherLeft].ToString("0.000");
                mKinectData2.mAU.LipStretcherRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LipStretcherRight].ToString("0.000");
                mKinectData2.mAU.LowerlipDepressorLeft = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LowerlipDepressorLeft].ToString("0.000");
                mKinectData2.mAU.LowerlipDepressorRight = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.LowerlipDepressorRight].ToString("0.000");
                mKinectData2.mAU.RightcheekPuff = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RightcheekPuff].ToString("0.000");
                mKinectData2.mAU.RighteyebrowLowerer = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RighteyebrowLowerer].ToString("0.000");
                mKinectData2.mAU.RighteyeClosed = currentFaceAlignment.AnimationUnits[FaceShapeAnimations.RighteyeClosed].ToString("0.000");
                
                kin3_2 = "kin3_2," + mKinectData2.mAU.JawOpen.ToString() + "," + mKinectData2.mAU.JawSlideRight.ToString() + "," + mKinectData2.mAU.LeftcheekPuff.ToString() + "," + mKinectData2.mAU.LefteyebrowLowerer.ToString() + "," + mKinectData2.mAU.LefteyeClosed.ToString() + "," + mKinectData2.mAU.LipCornerDepressorLeft.ToString() + "," + mKinectData2.mAU.LipCornerDepressorRight.ToString() + "," + mKinectData2.mAU.LipCornerPullerLeft.ToString() + "," + mKinectData2.mAU.LipCornerPullerRight.ToString() + "," + mKinectData2.mAU.LipPucker.ToString() + "," + mKinectData2.mAU.LipStretcherLeft.ToString() + "," + mKinectData2.mAU.LipStretcherRight.ToString() + "," + mKinectData2.mAU.LowerlipDepressorLeft.ToString() + "," + mKinectData2.mAU.LowerlipDepressorRight.ToString() + "," + mKinectData2.mAU.RightcheekPuff.ToString() + "," + mKinectData2.mAU.RighteyebrowLowerer.ToString() + "," + mKinectData2.mAU.RighteyeClosed.ToString();
                eyebrowleft_2 = Convert.ToDouble(mKinectData2.mAU.LefteyebrowLowerer);
                eyebrowright_2 = Convert.ToDouble(mKinectData2.mAU.RighteyebrowLowerer);
                           
            }
        }
        private void OnTrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            this.faceSource.TrackingId = 0;
            DetectedPerson = false;
        }
        private void OnTrackingIdLost2(object sender, TrackingIdLostEventArgs e)
        {
            this.faceSource2.TrackingId = 0;
            DetectedPerson2 = false;
        }

        public void OnFaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            if (kinectenable.Checked == false) return;
            using (var faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame == null) return;
                            var result = faceFrame.FaceFrameResult;
                            if (result == null)
                                return;

                var rotation = result.FaceRotationQuaternion;
                            int x, y, z;
                            ExtractFaceRotationInDegrees(rotation, out x, out y, out z);

                
                            rotX_1 = x;
                            rotY_1 = y;
                            rotZ_1 = z;
                            happy1 = result.FaceProperties[FaceProperty.Happy].ToString();
                            mouth1 = result.FaceProperties[FaceProperty.MouthMoved].ToString();
                            kin1_1 = "kin1_1," + x.ToString() + "," + y.ToString() + "," + z.ToString() + "," + result.FaceProperties[FaceProperty.Happy].ToString() + "," + result.FaceProperties[FaceProperty.MouthMoved].ToString();
                            
            }
        }
        public void OnFaceFrameArrived2(object sender, FaceFrameArrivedEventArgs e)
        {
            if (kinectenable.Checked == false) return;
            using (var faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame == null) return;
                var result = faceFrame.FaceFrameResult;

                if (result == null)
                    return;

                var rotation = result.FaceRotationQuaternion;
                int x, y, z;
                ExtractFaceRotationInDegrees(rotation, out x, out y, out z);
                
                rotX_2 = x;
                rotY_2 = y;
                rotZ_2 = z;
                happy2 = result.FaceProperties[FaceProperty.Happy].ToString();
                mouth2 = result.FaceProperties[FaceProperty.MouthMoved].ToString();
                kin1_2 = "kin1_2," + x.ToString() + "," + y.ToString() + "," + z.ToString() + "," + result.FaceProperties[FaceProperty.Happy].ToString() + "," + result.FaceProperties[FaceProperty.MouthMoved].ToString();

            }
        }
        #endregion

        public Form1(string characterName)
        {
            InitializeComponent();
            mKinectData = new KinectData();
            mKinectData2 = new KinectData(); //second user data
            GazeOut1=new GazeDef();
            GazeOut2 = new GazeDef();
            GazeOut1 = GazeDef.none;
            GazeOut2 = GazeDef.none;
            LastGazeOut1 = new GazeDef();
            LastGazeOut2 = new GazeDef();
            LastGazeOut1 = GazeDef.none;
            LastGazeOut2 = GazeDef.none;

            simpleSound = new SoundPlayer(@"countA.wav");
            eyebrowControllerUsr1 = new EyebrowsController();
            eyebrowControllerUsr2 = new EyebrowsController();
            serialPort1 = new System.IO.Ports.SerialPort();
            serialPort2 = new System.IO.Ports.SerialPort();
            sentonce = false;
            serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
            serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort2_DataReceived);

            thalamusClient = new PerceptionClient(characterName);
            
            pipeServer = new PipeServer.Server();
            engagementpipe = new PipeServer.Server();

            Xp = 0;
            Yp = 0;
            Zp = 0;
            Xp2 = -1;
            Yp2 = -1;
            Zp2 = -1;

            Xpold = 0;
            Ypold = 0;
            Zpold = 0;
            Xp2old = -1;
            Yp2old = -1;
            Zp2old = -1;

            RotH = 0;
            RotV = 0;
            RotH2 = 0;
            RotV2 = 0;
            
            
            depth = 0;

            allOKAOdata = "";
            allOKAOdata2 = "";
            allQdata = "";
            allQdata2 = "";
            allkinectdata = "";
            allkinectdata2 = "";

            startstop = false;
            FparticipanID = 0;
            FparticipanID2 = 1;

            Fparticipantname = "None";
            Fparticipantname2 = "None2";

            isEmpathic = true; //default value

            OKAOdata = "";
            OKAOdata2 = "";
            Kinectdata = "";
            Kinectdata2 = "";

            Qdata1 = "";
            Qdata2 = "";
            
            saver = new System.Timers.Timer();
            saver.Elapsed += new ElapsedEventHandler(OnsaverEvent);
            saver.Enabled = false;
            saver.Interval = 250; //Herz
            DetectedPerson = false;
            aTimer = new System.Timers.Timer();
            aTimer.Stop();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent); //publisher
            aTimer.Interval = 350;//increased the timer to reduce thalamus messages

            videoTimer = new MicroLibrary.MicroTimer();
            videoTimer.Stop();
            videoTimer.MicroTimerElapsed += new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(saveframe);
            videoTimer.Interval = 33333; //30fps

            //thalamusClient.ClientConnected += connected;
            this.pipeServer.MessageReceived +=
                new PipeServer.Server.MessageReceivedHandler(pipeServer_MessageReceived);
            this.engagementpipe.MessageReceived +=
                new PipeServer.Server.MessageReceivedHandler(engagementpipe_MessageReceived);
            button1_Click(this,null); //auto start the pipe servers

            if (scenarioselected == 0)
                scenarioselected = 1;

                 
        }

        void OnTimedEvent(object source, ElapsedEventArgs e) //thalamus publisher
        {
            #region Kinect2
            if (kinectenable.Checked==true)
            {

                //pointing and mutual pointing publisher

                    if (scenarioselected==2)
                    {
                        if (lefthandX_1 >= -0.58 && lefthandX_1 <= 0.58 && lefthandY_1 <= -0.32 && lefthandZ_1 <= 0.65) //left hand of user1 above the screen area
                        {
                            thalamusClient.PerceptionPublisher.PointingPosition(1, EmoteCommonMessages.Hand.lefthand, lefthandX_1, lefthandY_1, lefthandZ_1);
                            //now check for mutual point with user2 first with user2 lefthand
                            if ((lefthandX_1 >= lefthandX_2 - 0.15) && (lefthandX_1 <= lefthandX_2 + 0.15) && (lefthandZ_1 >= lefthandZ_2 - 0.15) && (lefthandZ_1 <= lefthandZ_2 + 0.15))
                                thalamusClient.PerceptionPublisher.UserMutualPoint(true, ((lefthandX_1 + lefthandX_2) / 2), ((lefthandZ_1 + lefthandZ_2) / 2));
                            //then with user2 righthand
                            if ((lefthandX_1 >= righthandX_2 - 0.15) && (lefthandX_1 <= righthandX_2 + 0.15) && (lefthandZ_1 >= righthandZ_2 - 0.15) && (lefthandZ_1 <= righthandZ_2 + 0.15))
                                thalamusClient.PerceptionPublisher.UserMutualPoint(true, ((lefthandX_1 + righthandX_2) / 2), ((lefthandZ_1 + righthandZ_2) / 2));

                        }
                        if (righthandX_1 >= -0.58 && righthandX_1 <= 0.58 && righthandY_1 <= -0.32 && righthandZ_1 <= 0.65) //right hand of user1 above the screen area
                        {

                            thalamusClient.PerceptionPublisher.PointingPosition(1, EmoteCommonMessages.Hand.righthand, righthandX_1, righthandY_1, righthandZ_1);
                            //now check for mutual point with user2 first with user2 lefthand
                            if ((righthandX_1 >= lefthandX_2 - 0.15) && (righthandX_1 <= lefthandX_2 + 0.15) && (righthandZ_1 >= lefthandZ_2 - 0.15) && (righthandZ_1 <= lefthandZ_2 + 0.15))
                                thalamusClient.PerceptionPublisher.UserMutualPoint(true, ((righthandX_1 + lefthandX_2) / 2), ((righthandZ_1 + lefthandZ_2) / 2));
                            //then with user2 righthand
                            if ((righthandX_1 >= righthandX_2 - 0.15) && (righthandX_1 <= righthandX_2 + 0.15) && (righthandZ_1 >= righthandZ_2 - 0.15) && (righthandZ_1 <= righthandZ_2 + 0.15))
                                thalamusClient.PerceptionPublisher.UserMutualPoint(true, ((righthandX_1 + righthandX_2) / 2), ((righthandZ_1 + righthandZ_2) / 2));
                        }
                        if (lefthandX_2 >= -0.58 && lefthandX_2 <= 0.58 && lefthandY_2 <= -0.32 && lefthandZ_2 <= 0.65) //left hand of user2 above the screen area
                        {
                            thalamusClient.PerceptionPublisher.PointingPosition(2, EmoteCommonMessages.Hand.lefthand, lefthandX_2, lefthandY_2, lefthandZ_2);
                        }
                        if (righthandX_2 >= -0.58 && righthandX_2 <= 0.58 && righthandY_2 <= -0.32 && righthandZ_2 <= 0.65) //right hand of user2 above the screen area
                        {
                            thalamusClient.PerceptionPublisher.PointingPosition(2, EmoteCommonMessages.Hand.righthand, righthandX_2, righthandY_2, righthandZ_2);
                        }
                    
                    //eye brow publisher
                    //it utilises the same controller. It uses au2 for left eye and au4 for right eye
                    eyebrowControllerUsr1.SetBrowsValues(eyebrowleft_1, eyebrowright_1);
                    if (scenarioselected == 2)
                    {
                        eyebrowControllerUsr2.SetBrowsValues(eyebrowleft_2, eyebrowright_2);
                    
                    }

                    thalamusClient.PerceptionPublisher.EyebrowsAU(eyebrowControllerUsr1.AU2BrowRaiser, eyebrowControllerUsr1.AU4BrowLower, eyebrowControllerUsr2.AU2BrowRaiser, eyebrowControllerUsr2.AU4BrowLower); //old method
                }
                //detecting mutual gazing and publisher
                
                    if (rotY_1 < -40 && rotY_2 > 40)
                        if(gazed==false)
                        {
                        thalamusClient.PerceptionPublisher.UserMutualGaze(true);
                        gazed=true;
                        }
                    else
                        if(gazed==true)
                        {
                            thalamusClient.PerceptionPublisher.UserMutualGaze(false);
                            gazed = false;
                        }

                
                //detecting touch chin and publisher
                   //first user both hands
                    if ((lefthandX_1 >= chinPosX_1 - 0.1 && lefthandX_1 <= chinPosX_1 + 0.1 && lefthandY_1 >= chinPosY_1 - 0.2 && lefthandY_1 <= chinPosY_1-0.05 && lefthandZ_1 >= chinPosZ_1 - 0.2 ) || (righthandX_1 >= chinPosX_1 - 0.1 && righthandX_1 <= chinPosX_1 + 0.1 && righthandY_1 >= chinPosY_1 - 0.2 && righthandY_1 <= chinPosY_1-0.05 && righthandZ_1 >= chinPosZ_1 - 0.2 ))//user1 touched chin
                    if(chin1touched==false)
                    {
                        thalamusClient.PerceptionPublisher.UserTouchChin(1, true);
                        chin1touched = true;
                    }
                    else
                        if (chin1touched == true)
                        {
                            thalamusClient.PerceptionPublisher.UserTouchChin(1, false);
                            chin1touched = false;
                        }
                    //second user both hands
                    if (scenarioselected==2)
                    {
                        if ((lefthandX_2 >= chinPosX_2 - 0.1 && lefthandX_2 <= chinPosX_2 + 0.1 && lefthandY_2 >= chinPosY_2 - 0.2 && lefthandY_2 <= chinPosY_2 - 0.05 && lefthandZ_2 >= chinPosZ_2 - 0.2) || (righthandX_2 >= chinPosX_2 - 0.1 && righthandX_2 <= chinPosX_2 + 0.1 && righthandY_2 >= chinPosY_2 - 0.2 && righthandY_2 <= chinPosY_2 - 0.05 && righthandZ_2 >= chinPosZ_2 - 0.2))//user1 touched chin
                            if (chin2touched == false)
                            {
                                thalamusClient.PerceptionPublisher.UserTouchChin(2, true);
                                chin2touched = true;
                            }
                            else
                                if (chin2touched == true)
                                {
                                    thalamusClient.PerceptionPublisher.UserTouchChin(2, false);
                                    chin1touched = false;
                                }
                    }
                
                //keep same names for both kinects
                Xp = locX_1;
                Yp = locY_1;
                Zp = locZ_1;
                Xp2 = locX_2;
                Yp2 = locY_2;
                Zp2 = locZ_2;
            }
            #endregion


            if (isEmpathic == true) //publish head position only if its empathic session
            {
                if (Xp != Xpold && Yp != Ypold && Zp != Zpold)
                {
                    thalamusClient.PerceptionPublisher.HeadTracking(1, Xp, Yp, Zp, DetectedPerson);
                    Xpold = Xp;
                    Ypold = Yp;
                    Zpold = Zp;
                }
                if (Xp2 != Xp2old && Yp2 != Yp2old && Zp2 != Zp2old)
                {
                    thalamusClient.PerceptionPublisher.HeadTracking(2, Xp2, Yp2, Zp2, DetectedPerson2);
                    Xp2old = Xp2;
                    Yp2old = Yp2;
                    Zp2old = Zp2;
                }

                //user 1 detected on screen form kinect
                if (DetectedPerson == true)
                {
                    if (GazeOut1 == GazeDef.screenL && GazeOut1 != LastGazeOut1) //screen left
                        thalamusClient.PerceptionPublisher.GazeTracking(1, EmoteCommonMessages.GazeEnum.ScreenLeft, GazeConfidence1);
                    if (GazeOut1 == GazeDef.screenR && GazeOut1 != LastGazeOut1) //screen right
                        thalamusClient.PerceptionPublisher.GazeTracking(1, EmoteCommonMessages.GazeEnum.ScreenRight, GazeConfidence1);
                    if (GazeOut1 == GazeDef.Robot && GazeOut1 != LastGazeOut1) //at robot
                        thalamusClient.PerceptionPublisher.GazeTracking(1, EmoteCommonMessages.GazeEnum.Robot, GazeConfidence1);
                    if (GazeOut1 == GazeDef.elsewhere && GazeOut1 != LastGazeOut1)  //else
                        thalamusClient.PerceptionPublisher.GazeTracking(1, EmoteCommonMessages.GazeEnum.Other, GazeConfidence1);
                    if (GazeOut1 == GazeDef.none && GazeOut1 != LastGazeOut1)  //none
                        thalamusClient.PerceptionPublisher.GazeTracking(1, EmoteCommonMessages.GazeEnum.None, GazeConfidence1);
                    LastGazeOut1 = GazeOut1;
                    //okao message publisher (latest known values)
                    thalamusClient.PerceptionPublisher.OKAOMessage(1, smile1, confidence1, o1Expression1, o1Expression2, o1Expression3, o1Expression4, o1Expression5, o1Expression6, o1Expression7, GazeOut1.ToString());
                    //q sensor publisher
                    if (Qdata1 != "")
                    {
                        string[] q_temp1 = Qdata1.Split(',');
                        thalamusClient.PerceptionPublisher.QSensorMessage(1, Double.Parse(q_temp1[0]), Double.Parse(q_temp1[1]), Double.Parse(q_temp1[2]), Double.Parse(q_temp1[3]), Double.Parse(q_temp1[4]));
                    }

                }

                //second user publishing only if in front of camera/kinect
                if (DetectedPerson2 == true)
                {
                    if (GazeOut2 == GazeDef.screenL && GazeOut2 != LastGazeOut2) //screen left
                        thalamusClient.PerceptionPublisher.GazeTracking(2, EmoteCommonMessages.GazeEnum.ScreenLeft, GazeConfidence2);
                    if (GazeOut2 == GazeDef.screenR && GazeOut2 != LastGazeOut2) //screen right
                        thalamusClient.PerceptionPublisher.GazeTracking(2, EmoteCommonMessages.GazeEnum.ScreenRight, GazeConfidence2);
                    if (GazeOut2 == GazeDef.Robot && GazeOut2 != LastGazeOut2) //at robot
                        thalamusClient.PerceptionPublisher.GazeTracking(2, EmoteCommonMessages.GazeEnum.Robot, GazeConfidence2);
                    if (GazeOut2 == GazeDef.elsewhere && GazeOut2 != LastGazeOut2)  //else
                        thalamusClient.PerceptionPublisher.GazeTracking(2, EmoteCommonMessages.GazeEnum.Other, GazeConfidence2);
                    if (GazeOut1 == GazeDef.none && GazeOut2 != LastGazeOut2)  //none
                        thalamusClient.PerceptionPublisher.GazeTracking(2, EmoteCommonMessages.GazeEnum.None, GazeConfidence2);
                    LastGazeOut2 = GazeOut2;
                    //okao message publisher (latest known values)
                    thalamusClient.PerceptionPublisher.OKAOMessage(2, smile2, confidence2, o2Expression1, o2Expression2, o2Expression3, o2Expression4, o2Expression5, o2Expression6, o2Expression7, GazeOut2.ToString());
                    //q sensor publisher
                    if (Qdata2 != "")
                    {
                        string[] q_temp2 = Qdata2.Split(',');
                        thalamusClient.PerceptionPublisher.QSensorMessage(2, Double.Parse(q_temp2[0]), Double.Parse(q_temp2[1]), Double.Parse(q_temp2[2]), Double.Parse(q_temp2[3]), Double.Parse(q_temp2[4]));
                    }

                }

                DetectedPerson = false;
                DetectedPerson2 = false;
            }
            
            if (checkedListBox1.GetItemChecked(0)==true)//kinect v1 enabled
            {
                //publish mutual gaze similarly to v2
                //detecting mutual gazing and publisher
                if (scenarioselected == 2)
                {
                   
                        if (RotV < -40 && RotV2 > 40)
                            if (gazed == false)
                            {
                                thalamusClient.PerceptionPublisher.UserMutualGaze(true);
                                gazed = true;
                            }
                            else
                                if (gazed == true)
                                {
                                    thalamusClient.PerceptionPublisher.UserMutualGaze(false);
                                    gazed = false;
                                } 
                }

                thalamusClient.PerceptionPublisher.EyebrowsAU(eyebrowControllerUsr1.AU2BrowRaiser, eyebrowControllerUsr1.AU4BrowLower, eyebrowControllerUsr2.AU2BrowRaiser, eyebrowControllerUsr2.AU4BrowLower);
            }
        }

        void OnsaverEvent(object source, ElapsedEventArgs e) //logger timer
        {
           if(startstop==true)
            {
                if ((pipeServer.Running == true) && (sentonce==false))
                {
                    pipeServer.SendMessage("start");
                    sentonce=true;
                    

                }
                //forward data to engagement module
                //check if pipe is open
                if (checkedListBox1.GetItemChecked(8) == true)
                {
                    if (scenarioselected == 1)
                    {
                        engagement = smile1 + ";" + confidence1 + ";" + o1Expression1 + ";" + o1Expression2 + ";" + o1Expression3 +
                            ";" + o1Expression4 + ";" + o1Expression5 + ";" + o1Expression6 + ";" + o1Expression7 + ";" + locX_1 + ";" + locY_1 +
                            ";" + locZ_1 + ";" + mKinectData.leanX + ";" + mKinectData.leanY + ";" + mKinectData.mAU.JawOpen +
                            ";" + mKinectData.mAU.JawSlideRight + ";" + mKinectData.mAU.LeftcheekPuff + ";" + mKinectData.mAU.LefteyebrowLowerer +
                            ";" + mKinectData.mAU.LefteyeClosed + ";" + mKinectData.mAU.LipCornerDepressorLeft + ";" + mKinectData.mAU.LipCornerDepressorRight +
                            ";" + mKinectData.mAU.LipCornerPullerLeft + ";" + mKinectData.mAU.LipCornerPullerRight + ";" + mKinectData.mAU.LipPucker +
                            ";" + mKinectData.mAU.LipStretcherLeft + ";" + mKinectData.mAU.LipStretcherRight + ";" + mKinectData.mAU.LowerlipDepressorLeft +
                            ";" + mKinectData.mAU.LowerlipDepressorRight + ";" + mKinectData.mAU.RightcheekPuff + ";" + mKinectData.mAU.RighteyebrowLowerer +
                            ";" + mKinectData.mAU.RighteyeClosed + ";" + qArousal1 + ";" + qtemp1 + ";" + qAccelX1 + ";" + qAccelY1 + ";" + qAccelZ1 + ";" + (int)GazeOut1 + ";" + chin1touched.ToString() + ";" + touchscreen1.ToString();

                    }
                    else
                    {
                        engagement = smile1 + ";" + confidence1 + ";" + o1Expression1 + ";" + o1Expression2 + ";" + o1Expression3 +
                            ";" + o1Expression4 + ";" + o1Expression5 + ";" + o1Expression6 + ";" + o1Expression7 + ";" + locX_1 + ";" + locY_1 +
                            ";" + locZ_1 + ";" + mKinectData.leanX + ";" + mKinectData.leanY + ";" + mKinectData.mAU.JawOpen +
                            ";" + mKinectData.mAU.JawSlideRight + ";" + mKinectData.mAU.LeftcheekPuff + ";" + mKinectData.mAU.LefteyebrowLowerer +
                            ";" + mKinectData.mAU.LefteyeClosed + ";" + mKinectData.mAU.LipCornerDepressorLeft + ";" + mKinectData.mAU.LipCornerDepressorRight +
                            ";" + mKinectData.mAU.LipCornerPullerLeft + ";" + mKinectData.mAU.LipCornerPullerRight + ";" + mKinectData.mAU.LipPucker +
                            ";" + mKinectData.mAU.LipStretcherLeft + ";" + mKinectData.mAU.LipStretcherRight + ";" + mKinectData.mAU.LowerlipDepressorLeft +
                            ";" + mKinectData.mAU.LowerlipDepressorRight + ";" + mKinectData.mAU.RightcheekPuff + ";" + mKinectData.mAU.RighteyebrowLowerer +
                            ";" + mKinectData.mAU.RighteyeClosed + ";" + qArousal1 + ";" + qtemp1 + ";" + qAccelX1 + ";" + qAccelY1 + ";" + qAccelZ1 + ";" + (int)GazeOut1 + ";" + chin1touched.ToString() + ";" + touchscreen1.ToString() + ";" +
                            
                            smile2 + ";" + confidence2 + ";" + o2Expression1 + ";" + o2Expression2 + ";" + o2Expression3 +
                            ";" + o2Expression4 + ";" + o2Expression5 + ";" + o2Expression6 + ";" + o2Expression7 + ";" + locX_2 + ";" + locY_2 +
                            ";" + locZ_2 + ";" + mKinectData2.leanX + ";" + mKinectData2.leanY + ";" + mKinectData2.mAU.JawOpen +
                            ";" + mKinectData2.mAU.JawSlideRight + ";" + mKinectData2.mAU.LeftcheekPuff + ";" + mKinectData2.mAU.LefteyebrowLowerer +
                            ";" + mKinectData2.mAU.LefteyeClosed + ";" + mKinectData2.mAU.LipCornerDepressorLeft + ";" + mKinectData2.mAU.LipCornerDepressorRight +
                            ";" + mKinectData2.mAU.LipCornerPullerLeft + ";" + mKinectData2.mAU.LipCornerPullerRight + ";" + mKinectData2.mAU.LipPucker +
                            ";" + mKinectData2.mAU.LipStretcherLeft + ";" + mKinectData2.mAU.LipStretcherRight + ";" + mKinectData2.mAU.LowerlipDepressorLeft +
                            ";" + mKinectData2.mAU.LowerlipDepressorRight + ";" + mKinectData2.mAU.RightcheekPuff + ";" + mKinectData2.mAU.RighteyebrowLowerer +
                            ";" + mKinectData2.mAU.RighteyeClosed + ";" + qArousal2 + ";" + qtemp2 + ";" + qAccelX2 + ";" + qAccelY2 + ";" + qAccelZ2 + ";" + (int)GazeOut2 + ";" + chin2touched.ToString() + ";" + touchscreen2.ToString();
                    }

                    if (engagementpipe.Running == true)
                        engagementpipe.SendMessage(engagement);

                }
                if (loggingchk.Checked == true)
                {
                    if(scenarioselected==1)
                    {
                        allOKAOdata+= DateTime.Now.TimeOfDay + "," + OKAOdata + System.Environment.NewLine;
                        allkinectdata+= DateTime.Now.TimeOfDay + "," + Kinectdata + "," + kin1_1 + "," + kin2_1 + "," + kin3_1 + System.Environment.NewLine;
                        allQdata+= DateTime.Now.TimeOfDay + Qdata1+ System.Environment.NewLine;
                    }
                        //alldata += DateTime.Now.TimeOfDay + "," + OKAOdata + "," + Qdata1 + "," + Kinectdata + "," + kin1_1 + "," + kin2_1 + "," + kin3_1 + "," + GazeOut1 + System.Environment.NewLine;
                    else
                    {
                        allOKAOdata += DateTime.Now.TimeOfDay + "," + OKAOdata + System.Environment.NewLine;
                        allkinectdata += DateTime.Now.TimeOfDay + "," + Kinectdata + "," + kin1_1 + "," + kin2_1 + "," + kin3_1 + System.Environment.NewLine;
                        allQdata += DateTime.Now.TimeOfDay + Qdata1 + System.Environment.NewLine;
                        
                        allOKAOdata2 += DateTime.Now.TimeOfDay + "," + OKAOdata2 + System.Environment.NewLine;
                        allkinectdata2 += DateTime.Now.TimeOfDay + "," + Kinectdata2 + "," + kin1_2 + "," + kin2_2 + "," + kin3_2 + System.Environment.NewLine;
                        allQdata2 += DateTime.Now.TimeOfDay + Qdata2 + System.Environment.NewLine;

                        //alldata += DateTime.Now.TimeOfDay + "," + OKAOdata + "," + Qdata1 + "," + Kinectdata + "," + kin1_1 + "," + kin2_1 + "," + kin3_1 + "," + GazeOut1 + System.Environment.NewLine;
                        //alldata2 += DateTime.Now.TimeOfDay + "," + OKAOdata2 + "," + Qdata2 + "," + Kinectdata2 + "," + kin1_2 + "," + kin2_2 + "," + kin3_2 + "," + GazeOut2 + System.Environment.NewLine;

                    }
                        
                    if (loggingtxt.Text.StartsWith("Logging"))
                    { }
                    else
                    {
                        if(scenarioselected==1)
                            this.Invoke(new Action(() => loggingtxt.Text = "Logging data for participantID " + FparticipanID + " Session " + SessionID));
                        else
                            this.Invoke(new Action(() => loggingtxt.Text = "Logging data for participantID " + FparticipanID + " and ID " + FparticipanID2 + " Session " + SessionID));
                        simpleSound.Play();

                        //open videowriter
                        if (videocheck.Checked == true)
                        {
                            
                            this.Invoke(new Action(() => loggingtxt.Text = loggingtxt.Text + System.Environment.NewLine + "Writing video file: " + "video_" + Fparticipantname + ".avi"));
                           
                            System.Threading.Thread.Sleep(1000); //wait for 0.5 second to create the file
                            
                            videoOut = new VideoWriter("video_" + FparticipanID + ".avi", CvInvoke.CV_FOURCC('D', 'I', 'V', 'X'), 30, 1920, 1080, true);
                            
                            
                            videoTimer.Enabled = true;
                        } 
                            
                    }

                }
                else
                { }
        }
            else
                {
                    videoTimer.Enabled = false;     
                    this.Invoke(new Action(() => loggingtxt.Clear()));
                    this.Invoke(new Action(() => loggingtxt.Text = "Log saved"));
                    
                    

                    if (pipeServer.Running == true)
                        pipeServer.SendMessage("stop" + FparticipanID);
                    saver.Stop();
                    sentonce = false;
                    System.Threading.Thread.Sleep(1000);
                    if (videocheck.Checked == true) videoOut.Dispose();
                    simpleSound.Play();
                }
        }
        void pipeServer_MessageReceived(PipeServer.Server.Client client, string message)
        {
            //if (startstop == true)
            //{
                //Console.WriteLine(message);
                this.Invoke(new PipeServer.Server.MessageReceivedHandler(DisplayMessageReceived),
                    new object[] { client, message });
                message = "";
                
            //}
        }
        void engagementpipe_MessageReceived(PipeServer.Server.Client client, string message)
        {
            this.Invoke(new PipeServer.Server.MessageReceivedHandler(DisplayMessageReceived2),
                    new object[] { client, message });
            message = "";
        }
        void DisplayMessageReceived2(PipeServer.Server.Client client, string message)
        {
            if (message.StartsWith("AP_OK")) //engagement module
                checkedListBox1.SetItemChecked(8, true);
        }
        void DisplayMessageReceived(PipeServer.Server.Client client, string message)
        {
            
            
            if (message.StartsWith("Temp")) //q sensor data
            {
                //no more active
            }
            if (message.Substring(1,3)=="Dep") //kinect1 data
            {
                Console.WriteLine("Kinect: " + message);
                aTimer.Enabled = true;
                if (message.StartsWith("1"))
                {
                    string[] parseMsg = message.Split(';');
                    Kinectdata = message;
                    Kinectdata2 = "";
                    try
                    {
                        depth = Convert.ToDouble(parseMsg[0].Substring(parseMsg[0].IndexOf(':') + 1), ifp);
                        Xp = Convert.ToDouble(parseMsg[1].Substring(parseMsg[1].IndexOf(':') + 1), ifp);
                        Yp = Convert.ToDouble(parseMsg[2].Substring(parseMsg[2].IndexOf(':') + 1), ifp);
                        Zp = Convert.ToDouble(parseMsg[3].Substring(parseMsg[3].IndexOf(':') + 1), ifp);
                        RotH = Convert.ToDouble(parseMsg[4].Substring(parseMsg[4].IndexOf(':') + 1), ifp);
                        RotV = Convert.ToDouble(parseMsg[5].Substring(parseMsg[5].IndexOf(':') + 1), ifp);
                    }
                    catch 
                    {
                        depth = 0;
                        Xp = 0;
                        Yp = 0;
                        Zp = 0;
                        RotH = 0;
                        RotV = 0;
                    }

                    DetectedPerson = true;
                   
                    try
                    {

                        AU4BrowLower = Convert.ToDouble(parseMsg[10].Substring(parseMsg[10].IndexOf(':') + 1), ifp);
                        AU2BrowRaiser = Convert.ToDouble(parseMsg[11].Substring(parseMsg[11].IndexOf(':') + 1), ifp);
                    }
                    catch
                    {
                        AU4BrowLower = 0;
                        AU2BrowRaiser = 0;
                    }

                    eyebrowControllerUsr1.SetBrowsValues(AU2BrowRaiser, AU4BrowLower);
                }
                else
                {
                    Kinectdata2 = message;
                    Kinectdata = "";
                    string[] parseMsg = message.Split(';');
                    try
                    {
                                        
                        depth = Convert.ToDouble(parseMsg[0].Substring(parseMsg[0].IndexOf(':') + 1), ifp);
                        Xp2 = Convert.ToDouble(parseMsg[1].Substring(parseMsg[1].IndexOf(':') + 1), ifp);
                        Yp2 = Convert.ToDouble(parseMsg[2].Substring(parseMsg[2].IndexOf(':') + 1), ifp);
                        Zp2 = Convert.ToDouble(parseMsg[3].Substring(parseMsg[3].IndexOf(':') + 1), ifp);
                        RotH2 = Convert.ToDouble(parseMsg[4].Substring(parseMsg[4].IndexOf(':') + 1), ifp);
                        RotV2 = Convert.ToDouble(parseMsg[5].Substring(parseMsg[5].IndexOf(':') + 1), ifp);
                    }
                    catch 
                    {
                        depth = 0;
                        Xp2 = 0;
                        Yp2 = 0;
                        Zp2 = 0;
                        RotH2 = 0;
                        RotV2 = 0;
                    }

                    DetectedPerson2 = true;

                    try
                    {
                        AU4BrowLower = Convert.ToDouble(parseMsg[10].Substring(parseMsg[10].IndexOf(':') + 1), ifp);
                        AU2BrowRaiser = Convert.ToDouble(parseMsg[11].Substring(parseMsg[11].IndexOf(':') + 1), ifp);
                    }
                    catch
                    {
                        AU4BrowLower = 0;
                        AU2BrowRaiser = 0;
                    }
                    eyebrowControllerUsr2.SetBrowsValues(AU2BrowRaiser, AU4BrowLower);
                }

                
                
                checkedListBox1.SetItemChecked(0, true);
           
            }

            if (message.StartsWith("Cam")) //camera data
            {
                if (message.Substring(3, 1) == "1")
                    checkedListBox1.SetItemChecked(2, true);
                if (message.Substring(3, 1) == "2")
                    checkedListBox1.SetItemChecked(3, true);
                if (message.Substring(3, 1) == "3")
                    checkedListBox1.SetItemChecked(4, true);
                if (message.Substring(3, 1) == "4")
                    checkedListBox1.SetItemChecked(5, true);
                if (message.Substring(3, 1) == "5")
                    checkedListBox1.SetItemChecked(6, true);
                if (message.Substring(3, 1) == "6")
                    checkedListBox1.SetItemChecked(7, true);

            }
            if (message.StartsWith("User")) //okao data
            {
                checkedListBox1.SetItemChecked(1, true);
                OKAOdata = message;
                string[] parseMsg1 = OKAOdata.Split(',');

                if (Convert.ToDouble(parseMsg1[1]) == 1)                   
                {//user 1
                    OKAOdata2 = "";
                    
                    try
                    {
                        smile1 = Convert.ToInt16(parseMsg1[3]);
                        confidence1 = Convert.ToInt16(parseMsg1[2]);
                        fupdown1 = Convert.ToInt16(parseMsg1[11]);
                        fleftright1 = Convert.ToInt16(parseMsg1[12]);
                        o1Expression1= Convert.ToInt16(parseMsg1[4]); 
                        o1Expression2= Convert.ToInt16(parseMsg1[5]);
                        o1Expression3= Convert.ToInt16(parseMsg1[6]);
                        o1Expression4= Convert.ToInt16(parseMsg1[7]);
                        o1Expression5= Convert.ToInt16(parseMsg1[8]);
                        o1Expression6 = Convert.ToInt16(parseMsg1[9]);
                        o1Expression7 = Convert.ToInt16(parseMsg1[10]);
                        if(parseMsg1[13]=="screenL")
                            GazeOut1=GazeDef.screenL;
                        if(parseMsg1[13]=="screenR")
                            GazeOut1=GazeDef.screenR;
                        if(parseMsg1[13]=="else")
                            GazeOut1=GazeDef.elsewhere;
                        if(parseMsg1[13]=="robot")
                            GazeOut1=GazeDef.Robot;
                        if (parseMsg1[13] == "none")
                            GazeOut1 = GazeDef.none;
                        this.Invoke(new Action(() =>{
                            lblgaze.Text = GazeOut1.ToString();
                            lblHeadX.Text = parseMsg1[11];
                            lblHeadY.Text = parseMsg1[12];
                        }
                        ));
                        
                    }
                    catch
                    {
                        smile1 = 0;
                        confidence1 = 0;
                        fupdown1 = 0;
                        fleftright1 = 0;
                        fupdown1 = 0;
                        fleftright1 = 0;
                        
                        GazeOut1=GazeDef.none;

                    }
                    
                }
                else
                {//user 2
                    OKAOdata2 = message;
                    OKAOdata = "";
                  
                    try
                    {
                        smile2 = Convert.ToInt16(parseMsg1[3]);
                        confidence2 = Convert.ToInt16(parseMsg1[2]);
                        fupdown2 = Convert.ToInt16(parseMsg1[11]);
                        fleftright2 = Convert.ToInt16(parseMsg1[12]);
                        
                        o2Expression1 = Convert.ToInt16(parseMsg1[4]);
                        o2Expression2 = Convert.ToInt16(parseMsg1[5]);
                        o2Expression3 = Convert.ToInt16(parseMsg1[6]);
                        o2Expression4 = Convert.ToInt16(parseMsg1[7]);
                        o2Expression5 = Convert.ToInt16(parseMsg1[8]);
                        o2Expression6 = Convert.ToInt16(parseMsg1[9]);
                        o2Expression7 = Convert.ToInt16(parseMsg1[10]);
                        if (parseMsg1[13] == "screenL")
                            GazeOut2 = GazeDef.screenL;
                        if (parseMsg1[13] == "screenR")
                            GazeOut2 = GazeDef.screenR;
                        if (parseMsg1[13] == "else")
                            GazeOut2 = GazeDef.elsewhere;
                        if (parseMsg1[13] == "robot")
                            GazeOut2 = GazeDef.Robot;
                        if (parseMsg1[13] == "none")
                            GazeOut2 = GazeDef.none;
                        this.Invoke(new Action(() =>
                        {
                            lblgaze2.Text = GazeOut2.ToString();
                            lblHeadX2.Text = parseMsg1[11];
                            lblHeadY2.Text = parseMsg1[12];
                        }
));
                    }
                    catch
                    {
                        smile2 = 0;
                        confidence2 = 0;
                        fupdown2 = 0;
                        fleftright2 = 0;
                        GazeOut2 = GazeDef.none;
                    }
                        
                }


            }
            message = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            //start the pipe server if it's not already running
            if (!this.pipeServer.Running)
            {
                this.pipeServer.PipeName = "\\\\.\\pipe\\serverpipe";

                this.pipeServer.Start();
                loggingtxt.Text =DateTime.Now.ToString() + ">" + "Server started\r\n" + loggingtxt.Text;
                
                this.button1.Text = "Close Pipe Server";
            }
            else
            {
                this.pipeServer.Stop();
                this.button1.Text = "Open Pipe Server";
                loggingtxt.Text = DateTime.Now.ToString() + ">" + "Server stoped\r\n" + loggingtxt.Text;
            }
            if (!this.engagementpipe.Running)
            {
                this.engagementpipe.PipeName = "\\\\.\\pipe\\affectperception";
                this.engagementpipe.Start();
            }
            else
            {
                this.engagementpipe.Stop();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             
            saver.Dispose();
            aTimer.Dispose();
            pipeServer.Stop();
            if (serialPort1.IsOpen) serialPort1.Close();
            if (serialPort2.IsOpen) serialPort2.Close();
            if(thalamusClient.IsConnected==true)
                thalamusClient.Dispose();
            Environment.Exit(0);
        }


        public static void savelogs()
        {
            //save the log files

            try {File.WriteAllText(NextAvailableFilename("logOKAO_S_" + SessionID + "_ID_" + FparticipanID + ".csv"), allOKAOdata);}catch{};
            try {File.WriteAllText(NextAvailableFilename("logKinect_S_" + SessionID + "_ID_" + FparticipanID + ".csv"), allkinectdata);}catch{};
            try {File.WriteAllText(NextAvailableFilename("logQ_S_" + SessionID + "_ID_" + FparticipanID + ".csv"), allQdata);}catch{};

            if(scenarioselected==2)
            {
                try {File.WriteAllText(NextAvailableFilename("logOKAO_S_" + SessionID + "_ID_" + FparticipanID2 + ".csv"), allOKAOdata2);}catch{};
                try {File.WriteAllText(NextAvailableFilename("logKinect_S_" + SessionID + "_ID_" + FparticipanID2 + ".csv"), allkinectdata2);}catch{};
                try { File.WriteAllText(NextAvailableFilename("logQ_S_" + SessionID + "_ID_" + FparticipanID2 + ".csv"), allQdata2);}catch { };
            }
            videoTimer.Enabled = false;
            allQdata = "";
            allQdata2 = "";
            allOKAOdata = "";
            allOKAOdata2 = "";
            allkinectdata = "";
            allkinectdata2 = "";

            startstop = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           if(isNumeric(textBox1.Text,System.Globalization.NumberStyles.Integer))
            saver.Interval =1000/ Convert.ToDouble(textBox1.Text);

        }
        public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result);
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #region Qsensors
        private void opencom_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true) serialPort1.Close();

            serialPort1.PortName = comport1.Text;
            serialPort1.BaudRate = 115200;
            serialPort1.ReadTimeout = 10000;
            serialPort1.WriteTimeout = 10000;
            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            { MessageBox.Show("Cannot open comm port. Try again 3 seconds after you pressed the Q Sensor button", ex.Message); }

            if (serialPort1.IsOpen)
            {
                opencom1.Enabled = false;
                qlogging.ReadOnly = true;
                closecom1.Enabled = true;
            }
        }

        private void closecom_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                opencom1.Enabled = true;
                closecom1.Enabled = false;
                qlogging.ReadOnly = true;
                qlogging.Clear();

            }
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
                RxString = serialPort1.ReadLine();
                this.Invoke(new EventHandler(DisplayText));
        }
        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RxString2 = serialPort2.ReadLine();
            this.Invoke(new EventHandler(DisplayText2));
        }
        private void DisplayText(object sender, EventArgs e)
        {
            qlogging.AppendText(DateTime.Now + ":" + DateTime.Now.Millisecond.ToString("000") + "," + RxString + System.Environment.NewLine);
            string[] qtemp1 = RxString.Split(',');
            if (qenable.Checked ==true)
            {    //packet,z,x,y,volts,temp,eda
                Qdata1 = (qtemp1.Length > 1 ? qtemp1[1] : "null") + "," + (qtemp1.Length > 2 ? qtemp1[2] : "null") + "," + (qtemp1.Length > 3 ? qtemp1[3] : "null") + "," + (qtemp1.Length > 5 ? qtemp1[5] : "null") + "," + (qtemp1.Length > 6 ? qtemp1[6] : "null");
            }
            serialPort1.DiscardInBuffer();

        }
        private void DisplayText2(object sender, EventArgs e)
        {
            qlogging2.AppendText(DateTime.Now + ":" + DateTime.Now.Millisecond.ToString("000") + "," + RxString2 + System.Environment.NewLine);
            string[] qtemp2 = RxString2.Split(',');
            if  (qenable.Checked ==true)
                Qdata2 = (qtemp2.Length > 1 ? qtemp2[1] : "null")  + "," + (qtemp2.Length > 2 ? qtemp2[2] : "null") + "," + (qtemp2.Length > 3 ? qtemp2[3] : "null") + "," + (qtemp2.Length > 5 ? qtemp2[5] : "null") + "," + (qtemp2.Length > 6 ? qtemp2[6] : "null");
            serialPort2.DiscardInBuffer();

        }
        private void opencom2_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen == true) serialPort2.Close();

            serialPort2.PortName = comport2.Text;
            serialPort2.BaudRate = 115200;
            serialPort2.ReadTimeout = 10000;
            serialPort2.WriteTimeout = 10000;
            try
            {
                serialPort2.Open();
            }
            catch (Exception ex)
            { MessageBox.Show("Cannot open comm port. Try again 3 seconds after you pressed the Q Sensor button", ex.Message); }

            if (serialPort2.IsOpen)
            {
                opencom2.Enabled = false;
                qlogging2.ReadOnly = true;
                closecom2.Enabled = true;
            }
        }

        private void closecom3_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                serialPort2.Close();
                opencom2.Enabled = true;
                closecom2.Enabled = false;
                qlogging2.ReadOnly = true;
                qlogging2.Clear();

            }
        }

        private void qenable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void kinectenable_CheckedChanged(object sender, EventArgs e)
        {
            if (kinectenable.Checked == true)
            {
                if (highDefinitionFaceFrameSource == null || highDefinitionFaceFrameSource2==null)
                    InitializeHDFace();
                else
                    sensor.Open();

                this.Invoke(new Action(() => kinectstatus.Text = "Opening"));
            }
            else
            {
               
                sensor.Close();
                
                this.Invoke(new Action(() => kinectstatus.Text = "Closed"));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (aTimer.Enabled == false)
            {
                
                Fparticipantname = "test";
                Fparticipantname2 = "test2";

                FparticipanID = 0;
                FparticipanID2 = 1;

                Kinectdata = "";
                OKAOdata = "";
                Qdata1 = "";
                Qdata2 = "";

                allOKAOdata = "";
                allkinectdata = "";
                allQdata = "";
                startstop = true;
                saver.Start();
                aTimer.Start();

            }
            else
            {
                savelogs();
                aTimer.Stop();
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            users.SelectedIndex = scenarioselected - 1;
            closing = false;
            kinectenable_CheckedChanged(null,null);

        }

        private void users_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if(users.SelectedIndex==0)//1 user
            {
                qlogging2.Enabled = false;
                opencom2.Enabled = false;
                closecom2.Enabled = false;
                comport2.Enabled = false;
                scenarioselected = 1;
                
            }
            else
            {
                qlogging2.Enabled = true;
                opencom2.Enabled = true;
                closecom2.Enabled = true;
                comport2.Enabled = true;
                scenarioselected = 2;
            }
        }

        #endregion

        private void videocheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void interfacetimer_Tick(object sender, EventArgs e)
        {
            if(locX_1!=0)
            this.Invoke(new Action(() =>
            {
                label23.Text = "X:" + locX_1.ToString();
                label21.Text = "Y:" + locY_1.ToString();
                label22.Text = "Z:" + locZ_1.ToString();
                label28.Text = "Lean X:" + mKinectData.leanX.ToString();
                label24.Text = "Lean Y:" + mKinectData.leanY.ToString();                
            }));
            if (locX_2 != 0)
            this.Invoke(new Action(() =>
            {
                label36.Text = "X:" + locX_2.ToString();
                label35.Text = "Y:" + locY_2.ToString();
                label34.Text = "Z:" + locZ_2.ToString();
                label33.Text = "Lean X:" + mKinectData2.leanX.ToString();
                label32.Text = "Lean Y:" + mKinectData2.leanY.ToString();
            }));
            if(mKinectData.mAU.LipPucker.ToString()!="0")
            this.Invoke(new Action(() =>
            {
                label17.Text = "JawOpen:" + mKinectData.mAU.JawOpen.ToString();
                label1.Text = "JawSlideRight:" + mKinectData.mAU.JawSlideRight.ToString();
                label4.Text = "LeftcheekPuff:" + mKinectData.mAU.LeftcheekPuff.ToString();
                label2.Text = "LefteyebrowLowerer:" + mKinectData.mAU.LefteyebrowLowerer.ToString();
                label5.Text = "LefteyeClosed:" + mKinectData.mAU.LefteyeClosed.ToString();
                label6.Text = "LipCornerDepLeft:" + mKinectData.mAU.LipCornerDepressorLeft.ToString();
                label7.Text = "LipCornerDepRight:" + mKinectData.mAU.LipCornerDepressorRight.ToString();
                label8.Text = "LipCornerPulLeft:" + mKinectData.mAU.LipCornerPullerLeft.ToString();
                label9.Text = "LipCornerPulRight:" + mKinectData.mAU.LipCornerPullerRight.ToString();
                label10.Text = "LipPucker:" + mKinectData.mAU.LipPucker.ToString();
                label11.Text = "LipStretcherLeft:" + mKinectData.mAU.LipStretcherLeft.ToString();
                label12.Text = "LipStretcherRight:" + mKinectData.mAU.LipStretcherRight.ToString();
                label13.Text = "LowerlipDepLeft:" + mKinectData.mAU.LowerlipDepressorLeft.ToString();
                label14.Text = "LowerlipDepRight:" + mKinectData.mAU.LowerlipDepressorRight.ToString();
                label15.Text = "RightcheekPuff:" + mKinectData.mAU.RightcheekPuff.ToString();
                label16.Text = "RighteyebrowLowerer:" + mKinectData.mAU.RighteyebrowLowerer.ToString();
                label56.Text = "RighteyeClosed:" + mKinectData.mAU.RighteyeClosed.ToString();
            }));
            if (mKinectData2.mAU.LipPucker.ToString() != "0")
            this.Invoke(new Action(() =>
            {
                label55.Text = "JawOpen:" + mKinectData2.mAU.JawOpen.ToString();
                label53.Text = "JawSlideRight:" + mKinectData2.mAU.JawSlideRight.ToString();
                label52.Text = "LeftcheekPuff:" + mKinectData2.mAU.LeftcheekPuff.ToString();
                label51.Text = "LefteyebrowLowerer:" + mKinectData2.mAU.LefteyebrowLowerer.ToString();
                label50.Text = "LefteyeClosed:" + mKinectData2.mAU.LefteyeClosed.ToString();
                label49.Text = "LipCornerDepressorLeft:" + mKinectData2.mAU.LipCornerDepressorLeft.ToString();
                label48.Text = "LipCornerDepressorRight:" + mKinectData2.mAU.LipCornerDepressorRight.ToString();
                label47.Text = "LipCornerPullerLeft:" + mKinectData2.mAU.LipCornerPullerLeft.ToString();
                label54.Text = "LipCornerPullerRight:" + mKinectData2.mAU.LipCornerPullerRight.ToString();
                label46.Text = "LipPucker:" + mKinectData2.mAU.LipPucker.ToString();
                label45.Text = "LipStretcherLeft:" + mKinectData2.mAU.LipStretcherLeft.ToString();
                label44.Text = "LipStretcherRight:" + mKinectData2.mAU.LipStretcherRight.ToString();
                label43.Text = "LowerlipDepressorLeft:" + mKinectData2.mAU.LowerlipDepressorLeft.ToString();
                label42.Text = "LowerlipDepressorRight:" + mKinectData2.mAU.LowerlipDepressorRight.ToString();
                label41.Text = "RightcheekPuff:" + mKinectData2.mAU.RightcheekPuff.ToString();
                label40.Text = "RighteyebrowLowerer:" + mKinectData2.mAU.RighteyebrowLowerer.ToString();
                label57.Text = "RighteyeClosed:" + mKinectData2.mAU.RighteyeClosed.ToString();
            }));

            if(rotX_1!=0)
            this.Invoke(new Action(() =>
            {
                label20.Text = "X:" + rotX_1.ToString();
                label18.Text = "Y:" + rotY_1.ToString();
                label19.Text = "Z:" + rotZ_1.ToString();
                label25.Text = "Happy:" + happy1;
                label26.Text = "MouthMoved:" + mouth1;
            }));
            if (rotX_2 != 0)
            this.Invoke(new Action(() =>
            {
                label39.Text = "X:" + rotX_2.ToString();
                label38.Text = "Y:" + rotY_2.ToString();
                label37.Text = "Z:" + rotZ_2.ToString();
                label31.Text = "Happy:" + happy2;
                label30.Text = "MouthMoved:" + mouth2;
            }));
            if(lefthandX_1!=0)
            this.Invoke(new Action(() =>
                {
                    pLx.Text = lefthandX_1.ToString("0.00");
                    pLy.Text = lefthandY_1.ToString("0.00");
                    pLz.Text = lefthandZ_1.ToString("0.00");
                    if (scenarioselected == 2)
                    {
                        pRx.Text = righthandX_2.ToString("0.00");
                        pRy.Text = righthandY_2.ToString("0.00");
                        pRz.Text = righthandZ_2.ToString("0.00");
                    }

                }));
            //confidence calculation
            if((kinectenable.Checked==true || checkedListBox1.GetItemChecked(0)==true) && checkedListBox1.GetItemChecked(1)==true)
            {
                confX_1V = 100-Math.Abs(rotY_1 - fleftright1);
                confY_1V = 100 - Math.Abs(rotX_1 - fupdown1);
                confX_1.Text = confX_1V.ToString();
                confY_1.Text = confY_1V.ToString();
                GazeConfidence1 = (int)((confX_1V + confY_1V) / 2);
                if(scenarioselected==2)
                {
                    confX_2V = 100 - Math.Abs(rotY_2 - fleftright2);
                    confY_2V = 100 - Math.Abs(rotX_2 - fupdown2);
                    confX_2.Text = confX_2V.ToString();
                    confY_2.Text = confY_2V.ToString();
                    GazeConfidence2 = (int)((confX_2V + confY_2V) / 2);
                }
            }
        }
            private static string numberPattern = " ({0})";

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (System.IO.Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(System.IO.Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

            


    }
}
