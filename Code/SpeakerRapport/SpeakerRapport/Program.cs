using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeakerRapport
{
    class Program
    {
        static void Main(string[] args)
        {
            string character = "";
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    Console.WriteLine("Useage: " + Environment.GetCommandLineArgs()[0] + " <CharacterName>");
                    return;
                }
                character = args[0];
            }
            SpeakerRapportClient client = new SpeakerRapportClient(character);
            Console.ReadLine();
            client.Dispose();
        }
    }
}
