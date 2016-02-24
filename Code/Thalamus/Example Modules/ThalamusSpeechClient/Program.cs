using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThalamusSpeechClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SpeechClient server = new SpeechClient();
            Console.WriteLine("\nPress a key to close...\n\n");
            Console.ReadLine();
            server.Dispose();
        }

    }
}
