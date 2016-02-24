using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zeroconf;
using System.Threading.Tasks;

namespace NAOIpDiscoverer
{
    public class NAOHost
    {
        public string IP { get; set; }
        public string Name { get; set; }
        public NAOHost(string ip, string name)
        {
            IP = ip;
            Name = name;
        }
        public override string ToString()
        {
            return Name + " (" + IP + ")";
        }
    }

    public class Discoverer
    {
        public List<NAOHost> NAOs { get; set; }

        public Discoverer() {
            NAOs = new List<NAOHost>();
        }

        public async Task<bool> DiscoverNAO()
        {
            NAOs.Clear();
            IReadOnlyList<IZeroconfHost> results1 = await ZeroconfResolver.ResolveAsync("_nao._tcp.local.", new TimeSpan(0, 0, 5));
            IReadOnlyList<IZeroconfHost> results2 = await ZeroconfResolver.ResolveAsync("_naoqi._tcp.local.", new TimeSpan(0, 0, 5));
            var resultsMerged = results1.Intersect(results2, new IZeroconfHostComparer()).ToList<IZeroconfHost>();
            foreach (var res in resultsMerged)
            {
                NAOs.Add(new NAOHost(res.IPAddress, res.DisplayName));
            }
            return true;
        }

        public static async Task EnumerateAllServicesFromAllHosts()
        {
            ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();
            //var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var d in domains)
            {
                Console.WriteLine(d.Key);
                IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync(d.Key, new TimeSpan(0, 0, 4));
                foreach (var res in results)
                {
                    Console.WriteLine("\t"+res.DisplayName+": "+res.IPAddress);
                }
                Console.WriteLine();
            }
        }

        class IZeroconfHostComparer : IEqualityComparer<IZeroconfHost>
        {
            bool IEqualityComparer<IZeroconfHost>.Equals(IZeroconfHost x, IZeroconfHost y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                string xDisplayName = (x.DisplayName == null ? "" : x.DisplayName);
                string yDisplayName = (y.DisplayName == null ? "" : y.DisplayName);
                string xIP = (x.IPAddress == null ? "" : x.IPAddress);
                string yIP = (y.IPAddress == null ? "" : y.IPAddress);
                return xDisplayName.Equals(yDisplayName) && xIP.Equals(yIP);
            }

            int IEqualityComparer<IZeroconfHost>.GetHashCode(IZeroconfHost obj)
            {
                if (Object.ReferenceEquals(obj, null)) return 0;
                return (obj.DisplayName + obj.IPAddress).GetHashCode();
            }
        }

    }
    
}
