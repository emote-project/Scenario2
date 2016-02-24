using System;
using System.Collections.Generic;
using System.Text;

namespace ThalamusEnercities
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
                    Console.WriteLine("Usege: " + Environment.GetCommandLineArgs()[0] + " <CharacterName>");
                    return;
                }
                character = args[0];
                if (args.Length > 1) pyAddress = args[1];
            }
            ThalamusEnercities thalamusEnercities = new ThalamusEnercities(character);
            Console.ReadLine();
            thalamusEnercities.Dispose();
        }
    }
}
