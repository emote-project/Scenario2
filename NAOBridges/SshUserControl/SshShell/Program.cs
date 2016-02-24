using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Threading;
namespace SshShell
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var shell = new MySshStream("127.0.0.1", "Eugenio", "hoPaola5858");
            shell.NewOutput += shell_NewOutput;

            shell.RunCommand("ls");

            Console.ReadLine();
        }

        static void shell_NewOutput(object sender, MySshStream.NewOutputEventArgs e)
        {
            Console.WriteLine(e.Line);
        }

        

    }
}
