using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAOIpDiscoverer
{
    class Program
    {
        static bool verbose = false;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("help", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Usage: NAOIpDiscoverer" + " [v]\n" +
                                      "v: verbose output");
                    return;
                }
                if (args[0].Equals("v", StringComparison.CurrentCultureIgnoreCase))
                {
                    verbose = true;
                }
            }
            AsyncMain();
#if DEBUG
            Console.ReadLine();
#endif
        }

        static async private void AsyncMain()
        {
            //await EnumerateAllServicesFromAllHosts();

            Console.WriteLine("Searching...");
            Discoverer d = new Discoverer();
            await d.DiscoverNAO();
            foreach (NAOHost nao in d.NAOs)
            {
                if (verbose)
                    Console.WriteLine(nao.Name + ": " + nao.IP);
                else
                    Console.WriteLine(nao.IP);
            }
        }
    }
}
