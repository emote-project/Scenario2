using System;
using System.Collections.Generic;
using System.Text;
using NuttyTracks;

namespace NuttyNAO
{
    public class NAOLegsBodySet : BodySet
    {
        public NAOLegsBodySet()
            : base("NAOLegsBodySet")
        {
            AddChannel("LHipYawPitch");
            AddChannel("LHipRoll");
            AddChannel("LHipPitch");
            AddChannel("LKneePitch");
            AddChannel("LAnklePitch");
            AddChannel("LAnkleRoll");
            AddChannel("RHipYawPitch");
            AddChannel("RHipRoll");
            AddChannel("RHipPitch");
            AddChannel("RKneePitch");
            AddChannel("RAnklePitch");
            AddChannel("RAnkleRoll");
        }
    }
}
