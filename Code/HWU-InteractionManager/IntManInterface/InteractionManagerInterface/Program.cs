using System;

namespace IntManInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            IntManInterfaceClient client = new IntManInterfaceClient();
            Console.WriteLine("\nPress a key to close...\n\n");
            Console.ReadLine();
            client.Dispose();
        }
    }
}