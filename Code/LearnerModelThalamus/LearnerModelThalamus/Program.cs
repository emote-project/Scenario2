using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnerModelThalamus
{
    class Program
    {
        static void Main(string[] args)
        {
            string charName = "";
            if (args.Length > 1)
            {
                charName = args[1];
            }
            LearnerModelClient client = new LearnerModelClient(charName);
            Console.WriteLine("\nPress a key to close...\n\n");
            Console.ReadLine();
            client.Dispose();
        }
    }
}
