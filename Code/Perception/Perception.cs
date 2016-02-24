using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thalamus;
using EmoteCommonMessages;
using System.Windows.Forms;
using EmoteEvents.ComplexData;

namespace Perception
{
    public interface IPerceptionPulisher : IThalamusPublisher, IPerceptionEvents
    { }

    public interface IPerceptionClient : EmoteCommonMessages.IEmoteActions { }
    public class PerceptionClient : ThalamusClient, IPerceptionClient
    {

        public class PerceptionClientPublisher : IPerceptionPulisher
        {
           
            dynamic publisher;
            public PerceptionClientPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            public void GazeTracking(int userID, GazeEnum direction,int ConfidenceVal)
            {
                
                publisher.GazeTracking(userID,direction,ConfidenceVal);
            }

            public void HeadTracking(int userID,double X, double Y, double Z ,bool DetectedSkeleton)
            {
                publisher.HeadTracking(userID,X, Y, Z, DetectedSkeleton);
            }

            public void EyebrowsAU(double au2_user1, double au4_user1, double au2_user2, double au4_user2)
            {
                publisher.EyebrowsAU(au2_user1, au4_user1, au2_user2, au4_user2);
            }


            public void PointingPosition(int userID, Hand hand, double X, double Y, double Z)
            {
                publisher.PointingPosition(userID,hand, X, Y, Z);
            }

            public void UserMutualGaze(bool value)
            {
                publisher.UserMutualGaze(value);
            }

            public void UserMutualPoint(bool value, double avegX, double avegY)
            {
                publisher.UserMutualPoint(value, avegX, avegY);
            }


            public void UserTouchChin(int userID, bool value)
            {
                publisher.UserTouchChin(userID, value);
            }


            public void EyebrowsAU2(double au4left_user1, double au4right_user1, double au4left_user2, double au4right_user2)
            {
                publisher.EyebrowsAU2(au4left_user1, au4right_user1, au4left_user2, au4right_user2);
            }







            public void OKAOMessage(int userID, double smile, double confidence, double anger, double disgust, double fear, double joy, double sadness, double surprise, double neutral, string gazeDirection)
            {
                publisher.OKAOMessage(userID, smile, confidence, anger, disgust, fear, joy, sadness, surprise, neutral, gazeDirection);
            }

            public void QSensorMessage(int userID, double Z, double Y, double X, double Temp, double EDA)
            {
                publisher.QSensorMessage(userID, Z, Y, X, Temp, EDA);
            }

        }

        public PerceptionClientPublisher PerceptionPublisher;

        public PerceptionClient(string character = "") :
            base("Perception", character) 
        {
            SetPublisher<IPerceptionPulisher>();
            PerceptionPublisher = new PerceptionClientPublisher(Publisher);
        }
        void IEmoteActions.Reset()
        {
           
        }

        public void Start(string StartMessageInfo_info)
        {
            var info = JsonSerializable.DeserializeFromJson<StartMessageInfo>(StartMessageInfo_info);
            EmoteEvents.ComplexData.StartMessageInfo smi = EmoteEvents.ComplexData.StartMessageInfo.DeserializeFromJson<EmoteEvents.ComplexData.StartMessageInfo>(StartMessageInfo_info);

            string p1name = info.Students[0].firstName;
            long p1id = info.Students[0].thalamusLearnerId;
            string p2name = "";
            long p2id = -1;
            int SessionID = 0;
            Boolean isEmpathic = true;
            isEmpathic = smi.IsEmpathic;
            SessionID=smi.SessionId;

            if (info.Students[1]!=null)
            {
                p2name = info.Students[1].firstName;
                p2id = info.Students[1].thalamusLearnerId;
            }
            if (Form1.startstop == true) //logging for some participant already
            {
                Form1.savelogs();
                Form1.aTimer.Stop();
                Form1.startstop = false;
                System.Threading.Thread.Sleep(1000); 

            }            
            Form1.SessionID = SessionID;
            Form1.isEmpathic = isEmpathic;

            Form1.startstop = true;

            Form1.Fparticipantname = p1name + "" ;
            Form1.Fparticipantname2 = p2name + "";
            Form1.FparticipanID = p1id;
            Form1.FparticipanID2 = p2id;

            Form1.Kinectdata = "";
            Form1.Kinectdata2 = "";

            Form1.OKAOdata = "";
            Form1.OKAOdata2 = "";

            Form1.Qdata1 = "";
            Form1.Qdata2 = "";

            Form1.allQdata = "";
            Form1.allQdata2 = "";
            Form1.allOKAOdata = "";
            Form1.allOKAOdata2 = "";
            Form1.allkinectdata = "";
            Form1.allkinectdata2 = "";

            Form1.saver.Start();
            Form1.aTimer.Start();
        }

        void IEmoteActions.Stop()
        {
           // Form1.startstop = false;
            Form1.savelogs();
            //Form1.saver.Stop();
            Form1.aTimer.Stop();
        }


        void IEmoteActions.SetLearnerInfo(string LearnerInfo_learnerInfo)
        {
            //throw new NotImplementedException();
        }
    }
}
