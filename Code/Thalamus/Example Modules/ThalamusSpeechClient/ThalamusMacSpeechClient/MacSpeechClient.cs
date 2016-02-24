using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Net.Sockets;
using System.Net;

namespace ThalamusMacSpeechClient
{
    public class MacSpeechClient : ThalamusSpeechClient.SpeechClient
    {
        public MacSpeechClient() :base("MacSpeechClient")
        {
            speechEngine = new MacSpeechEngine();
            speechEngine.Setup();
            speechEngine.Start(this);
        }
    }
}
