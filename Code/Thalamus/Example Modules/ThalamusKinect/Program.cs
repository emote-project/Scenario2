using System;
using System.Collections.Generic;
using System.Text;

namespace ThalamusKinect
{
    class Program
    {
        static void Main(string[] args)
        {
            KinectClient server = new KinectClient();
            Console.WriteLine("\nPress a key to close...\n\n");
            Console.ReadLine();
            server.Dispose();
        }
    }
}
