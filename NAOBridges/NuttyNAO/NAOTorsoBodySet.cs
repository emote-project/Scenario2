using System;
using System.Collections.Generic;
using System.Text;
using NuttyTracks;

namespace NuttyNAO
{
    public class NAOTorsoBodySet : BodySet
    {
        public NAOTorsoBodySet()
            : base("NAOTorsoBodySet")
        {
            AddChannel("Torso");
            AddChannel("HeadYaw");
            AddChannel("HeadPitch");
            AddChannel("LShoulderPitch");
            AddChannel("LShoulderRoll");
            AddChannel("LElbowYaw");
            AddChannel("LElbowRoll");
            AddChannel("LWristYaw");
            AddChannel("LHand");
            AddChannel("RShoulderPitch");
            AddChannel("RShoulderRoll");
            AddChannel("RElbowYaw");
            AddChannel("RElbowRoll");
            AddChannel("RWristYaw");
            AddChannel("RHand");
        }
    }
}
