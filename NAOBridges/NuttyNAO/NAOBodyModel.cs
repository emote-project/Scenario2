using System;
using System.Collections.Generic;
using System.Text;
using NuttyTracks;
using Newtonsoft.Json;

namespace NuttyNAO
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class NAOBodyModel : BodyModel
    {
        public NAOBodyModel() : 
            base("NAOBodyModel") 
        {
        }

        public override void LoadBodySets()
        {
            base.LoadBodySets();
            AddBodySet(new NAOTorsoBodySet());
            //AddBodySet(new NAOLegsBodySet());
            AddBodySet(new NuttyTracks.CoreBodySet());
            EnslaveChannel("CoreBodySet.GazeHeadHorizontal", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadYaw", 1.0 } });
            EnslaveChannel("CoreBodySet.GazeHeadVertical", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadPitch", 1.0 } });
            EnslaveChannel("CoreBodySet.GazeEyesHorizontal", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadYaw", 1.0 } });
            EnslaveChannel("CoreBodySet.GazeEyesVertical", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadPitch", 1.0 } });
            EnslaveChannel("CoreBodySet.GazeBodyHorizontal", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadYaw", 1.0 } });
            EnslaveChannel("CoreBodySet.GazeBodyVertical", new Dictionary<String, double> { { "NAOTorsoBodySet.HeadPitch", 1.0 } });
        }
        public override void BuildSkeleton()
        {
            base.BuildSkeleton();
            SkeletonRoot = AddSkeletonNode(GetChannel("NAOTorsoBodySet.Torso"), SkeletonNode.Null, this, Vector3.Zero, 0, 0, new JointConfig(0, 0, 0, 0)); // doesn't rotate

            if (BodySets.ContainsKey("NAOTorsoBodySet"))
            {
                //head
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.HeadYaw"), SkeletonNodes["NAOTorsoBodySet.Torso"], this, new Vector3(0, 0, 126.5), 0, SkeletonNode.DoF.RotZ, new JointConfig(-119.5, 119.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.HeadPitch"), SkeletonNodes["NAOTorsoBodySet.HeadYaw"], this, new Vector3(0, 0, 0), 0, SkeletonNode.DoF.RotX, new JointConfig(-38.5, 29.5, 50, 0));

                //leftArm
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LShoulderPitch"), SkeletonNodes["NAOTorsoBodySet.Torso"], this, new Vector3(0, 98, 100), 90, SkeletonNode.DoF.RotX, new JointConfig(-119.5, 119.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LShoulderRoll"), SkeletonNodes["NAOTorsoBodySet.LShoulderPitch"], this, new Vector3(0, 0, 0), 0, SkeletonNode.DoF.RotY, new JointConfig(-18, 76, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LElbowYaw"), SkeletonNodes["NAOTorsoBodySet.LShoulderRoll"], this, new Vector3(105, 15, 0), -90, SkeletonNode.DoF.RotX, new JointConfig(-119.5, 119.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LElbowRoll"), SkeletonNodes["NAOTorsoBodySet.LElbowYaw"], this, new Vector3(0, 0, 0), -90, SkeletonNode.DoF.RotZ, new JointConfig(-88.5, -2, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LWristYaw"), SkeletonNodes["NAOTorsoBodySet.LElbowRoll"], this, new Vector3(55.95, 0, 0), 0, SkeletonNode.DoF.RotX, new JointConfig(-104.5, 104.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.LHand"), SkeletonNodes["NAOTorsoBodySet.LWristYaw"], this, new Vector3(69.07, 0, -3.04), 0, SkeletonNode.DoF.RotY, new JointConfig(0, 100, 50, 50));

                //rightArm
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RShoulderPitch"), SkeletonNodes["NAOTorsoBodySet.Torso"], this, new Vector3(0, -98, 100), 90, SkeletonNode.DoF.RotX, new JointConfig(-119.5, 119.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RShoulderRoll"), SkeletonNodes["NAOTorsoBodySet.RShoulderPitch"], this, new Vector3(0, 0, 0), 0, SkeletonNode.DoF.RotY, new JointConfig(-76, 18, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RElbowYaw"), SkeletonNodes["NAOTorsoBodySet.RShoulderRoll"], this, new Vector3(105, -15, 0), 90, SkeletonNode.DoF.RotX, new JointConfig(-119.5, 119.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RElbowRoll"), SkeletonNodes["NAOTorsoBodySet.RElbowYaw"], this, new Vector3(0, 0, 0), 90, SkeletonNode.DoF.RotZ, new JointConfig(2, 88.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RWristYaw"), SkeletonNodes["NAOTorsoBodySet.RElbowRoll"], this, new Vector3(55.95, 0, 0), 0, SkeletonNode.DoF.RotX, new JointConfig(-104.5, 104.5, 50, 0));
                AddSkeletonNode(GetChannel("NAOTorsoBodySet.RHand"), SkeletonNodes["NAOTorsoBodySet.RWristYaw"], this, new Vector3(69.07, 0, -3.04), 0, SkeletonNode.DoF.RotY, new JointConfig(0, 100, 50, 50));
            }

            if (BodySets.ContainsKey("NAOLegsBodySet"))
            {
                //leftLeg
                /*AddSkeletonNode(GetChannel("NAOLegsBodySet.LHipYawPitch"), SkeletonNodes["NAOTorsoBodySet.Torso"], this, new Vector3(0, 50, -85), 0, SkeletonNode.DoF.RotY, new JointConfig(-65.62, 42.44, 50, 0), 3);
                AddSkeletonNode(GetChannel("NAOLegsBodySet.LHipRoll"), SkeletonNodes["NAOLegsBodySet.LHipYawPitch"], this, new Vector3(0, 0, 0), 0, 1, new JointConfig(-21.74, 45.29, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.LHipPitch"), SkeletonNodes["NAOLegsBodySet.LHipRoll"], this, new Vector3(0, 0, 0), 0, 2, new JointConfig(-88, 27.73, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.LKneePitch"), SkeletonNodes["NAOLegsBodySet.LHipPitch"], this, new Vector3(0, 0, -100), 0, 2, new JointConfig(-5.29, 121.04, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.LAnklePitch"), SkeletonNodes["NAOLegsBodySet.LKneePitch"], this, new Vector3(0, 0, -102.9), 0, 2, new JointConfig(-68.15, 52.86, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.LAnkleRoll"), SkeletonNodes["NAOLegsBodySet.LAnklePitch"], this, new Vector3(0, 0, 0), 0, 1, new JointConfig(-22.79, 44.06, 50, 0));

                //rightLeg
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RHipYawPitch"), SkeletonNodes["NAOTorsoBodySet.Torso"], this, new Vector3(0, -50, -85), 0, SkeletonNode.DoF.RotY, new JointConfig(-65.62, 42.44, 50, 0), 3);
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RHipRoll"), SkeletonNodes["NAOLegsBodySet.RHipYawPitch"], this, new Vector3(0, 0, 0), 0, 1, new JointConfig(-45.29, 21.74, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RHipPitch"), SkeletonNodes["NAOLegsBodySet.RHipRoll"], this, new Vector3(0, 0, 0), 0, 2, new JointConfig(-88, 27.73, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RKneePitch"), SkeletonNodes["NAOLegsBodySet.RHipPitch"], this, new Vector3(0, 0, -100), 0, 2, new JointConfig(-5.9, 121.47, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RAnklePitch"), SkeletonNodes["NAOLegsBodySet.RKneePitch"], this, new Vector3(0, 0, -102.9), 0, 2, new JointConfig(-67.97, 53.4, 50, 0));
                AddSkeletonNode(GetChannel("NAOLegsBodySet.RAnkleRoll"), SkeletonNodes["NAOLegsBodySet.RAnklePitch"], this, new Vector3(0, 0, 0), 0, 1, new JointConfig(-44.06, 22.8, 50, 0));*/
            }
        }

    }
}
