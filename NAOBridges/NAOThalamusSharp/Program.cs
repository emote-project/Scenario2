using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NAOThalamus
{
    class Program
    {
        static void Main(string[] args)
        {
            string character = "";
            string pyAddress = "localhost";
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    Console.WriteLine("Useage: " + Environment.GetCommandLineArgs()[0] + " <CharacterName> <naoXmlRpcPyAddress>");
                    return;
                }
                character = args[0];
                if (args.Length > 1) pyAddress = args[1];
            }
            NAOThalamusClient client = new NAOThalamusClient(character, pyAddress);
            Console.ReadLine();
            client.Dispose();
        }
    }
}
