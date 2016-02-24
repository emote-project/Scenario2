using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuttyNAOTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    PrintHelp();
                    return;
                } if (args[0].ToLower() == "xar")
                {
                    bool dParameter = false;
                    string path = Directory.GetCurrentDirectory();
                    if (args.Length > 1) {
                        if (args[1].ToLower() == "-d") {
                            dParameter = true;
                        }
                        if (args.Length > 2)
                        {
                            path = args[2];
                        }
                        else path = args[1];
                    }
                    ChoreographToNuttyConverter.ConvertXarFiles(path, dParameter);
                    Console.ReadLine();
                }
            }
            else
            {
                PrintHelp();
            }
        }

        private static void PrintHelp()
        {
            string str = "Usage: \n\n\t";
            str += "To convert Choreograph (xar) files to Nutty files:\n\t\t> " + Path.GetFileName(Environment.GetCommandLineArgs()[0]) + " XAR [-d] <path>\n";
            str += "\t\t<path> represents the folder where a behaviour.xar file is located.\n";
            str += "\t\t If the -d parameter is specified, the subfolders in <path> will each be used.";
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}
