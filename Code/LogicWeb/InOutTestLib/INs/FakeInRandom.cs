using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LogicWebLib;
using Timer = System.Timers.Timer;

namespace InOutTestLib.INs
{
    public class FakeInRandom : InputNode
    {
        private Timer _timer;
        public FakeInRandom()
            : base("Fake-random descriptor")
        {
            Random ran = new Random((int)DateTime.Now.Ticks);
            _timer = new Timer(ran.Next(1, 2) * 1000);
            _timer.AutoReset = true;
            _timer.Elapsed += delegate(object sender, ElapsedEventArgs args)
            {
                Active = !Active;
            };
            _timer.Start();
        }
    }

    public class FakeInStatic : InputNode
    {
        public FakeInStatic()
            : base("Fake-static descriptor")
        {
            Random ran = new Random((int)DateTime.Now.Ticks);
            Active = true;
        }
    }
}
