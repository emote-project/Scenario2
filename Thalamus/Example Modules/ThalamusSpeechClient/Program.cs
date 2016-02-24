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
            string character = "";
            if (args.Length > 0)
            {
                character = args[0];
            }
            SpeechClient server = new SpeechClient(character);
            Console.WriteLine("\nPress a key to close...\n\n");
            Console.ReadLine();
            server.Dispose();
        }

    }
}
