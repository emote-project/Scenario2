using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thalamus;

namespace InOutTestLib.Thalamus
{
    interface IInOutTestMessages : IPerception
    {
        void TestMessageOne();
        void TestMessageTwo();
    }


    public partial class InOutThalamusClient : ThalamusClient, IInOutTestMessages
    {
        private static InOutThalamusClient _instance = null;


        public static InOutThalamusClient GetInstance()
        {
            return _instance ?? (_instance = new InOutThalamusClient());
        }

        private InOutThalamusClient() : base("InOutTestLibraryClient", "")
        {
        }

        public void TestMessageOne()
        {
            if (TestMessageOneEvent != null) TestMessageOneEvent(this, null);
        }

        public void TestMessageTwo()
        {
            if (TestMessageTwoEvent != null) TestMessageTwoEvent(this, null);
        }
    }
}
