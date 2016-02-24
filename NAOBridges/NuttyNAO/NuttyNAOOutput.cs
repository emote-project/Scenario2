using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuttyTracks;

using NAOThalamus;

namespace NuttyNAO
{
    public class NuttyNAOOutput : BaseNuttyOutput
    {
        NAOThalamusClient nao;

        string naoIP = "";
        public override String InitializationParameters { get { return naoIP; } }

        public NuttyNAOOutput() : this("") { }
        public NuttyNAOOutput(string ip)
        {
            naoIP = ip;
        }

        public override void Start(NuttyManager nutty)
        {
            nutty.Choker = 50;
            nutty.HeartbeatFrequency = 25;
            nutty.BodyModel = nutty.BodyModels["NAOBodyModel"];
            base.Start(nutty);
            virtualOutput = false;
            if (naoIP=="") naoIP = Microsoft.VisualBasic.Interaction.InputBox("NAO's IP address:", "NuttyNAO", Properties.Settings.Default.naoAddress);
            Properties.Settings.Default.naoAddress = naoIP;
            Properties.Settings.Default.Save();
            nao = new NAOThalamusClient("", naoIP, false);
            nao.HeartbeatEcho += nao_HeartbeatEcho;
        }

        void nao_HeartbeatEcho(Double ticks, string[] joints, double[] value)
        {
            AnimationBuffer ab = new AnimationBuffer(nutty.BodyModel, ticks - DateTime.Now.Ticks);
            for (int i = 0; i < joints.Length;i++)
            {
                string channelName = joints[i];
                foreach (BodySet b in ab.BodyModel.BodySets.Values)
                    if (b.Contains(channelName))
                    {
                        channelName = b.Channels[joints[i]].FullName;
                        break;
                    }
                if (ab.BodyModel.AllChannels.ContainsKey(channelName)) ab.SetChannelValue(ab.BodyModel.AllChannels[channelName], value[i]);
            }
            
            outputState = ab;
            NotifyHeartbeatEcho(ticks, ab);
        }

        public override void Animate(double ticks, AnimationBuffer frame)
        {
            int i = 0;
            foreach (KeyValuePair<Channel, bool> m in frame.Mask) if (m.Value) i++;
            string[] joints = new string[i];
            double[] values = new double[i];
            double[] speed = new double[i];
            i = 0;
            foreach (KeyValuePair<Channel, double> channel in frame.Values) 
                if (frame.Mask[channel.Key])
                {
                    joints[i] = channel.Key.FullName;
                    speed[i] = frame.Speed[channel.Key];
                    values[i++] = channel.Value*frame.Passthrough[channel.Key] + (1-frame.Passthrough[channel.Key])*outputState.Values[channel.Key];
                }
            nao.Animate(ticks, joints, values, speed, frame.DeltaSeconds, frame.FrameType.ToString());
        }

        /*public override void Heartbeat(long ticks)
        {
            //nao.Heartbeat(ticks);
        }*/

        public override void Dispose()
        {
            base.Dispose();
            nao.Dispose();
        }

        public override string ToString()
        {
            return "NuttyNAOOutput";
        }
    }
}
