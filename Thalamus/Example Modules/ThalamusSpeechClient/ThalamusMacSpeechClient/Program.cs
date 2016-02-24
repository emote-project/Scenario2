using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThalamusMacSpeechClient
{
	class Program
	{
		static void Main(string[] args)
		{
            MacSpeechClient server = new MacSpeechClient();
			Console.WriteLine("\nPress a key to close...\n\n");
			Console.ReadLine();
			server.Dispose();
		}

	}
}
