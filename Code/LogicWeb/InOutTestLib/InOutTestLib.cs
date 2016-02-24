using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutTestLib.Behaviours;
using InOutTestLib.INs;
using InOutTestLib.INs.ThalamusINs;
using InOutTestLib.Thalamus;
using LogicWebLib;

namespace InOutTestLib
{
    /// <summary>
    /// This logic frame may contain datas coming from the input which may be accessed by the outputs
    /// </summary>
    public class InOutTestLogic : LogicFrame
    {
        public InOutTestLogic()
        {
            AddInput(new FakeInRandom());
            AddInput(new MessageOneIN());

            AddOutput(new OutNode("TestOutNode"){ BehaviourType = typeof (ThreeSecondsBehaviour)});
        }

        public override void Dispose()
        {
            base.Dispose();
            InOutThalamusClient.GetInstance().Dispose();
        }
    }
}
