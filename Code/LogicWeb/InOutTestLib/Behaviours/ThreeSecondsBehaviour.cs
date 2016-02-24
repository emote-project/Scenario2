using System;
using System.Timers;
using LogicWebLib;

namespace InOutTestLib.Behaviours
{
    class ThreeSecondsBehaviour : Behaviour
    {

        public override void BehaviourTask()
        {
            Console.WriteLine("-------------------------------Started");
            Timer t = new Timer(5000);
            t.Start();
            t.AutoReset = false;
            t.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                // waiting 3 seconds
                Console.WriteLine("---------------------------------Ended");
                ExecutionEnded();
            };
        }

    }
}
