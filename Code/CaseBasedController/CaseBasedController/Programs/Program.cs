using System;
using System.Windows.Forms;
using CaseBasedController.Forms;
using CaseBasedController.Thalamus;
using PS.Utilities.Math;
using CaseBasedController.GameInfo;

namespace CaseBasedController.Programs
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var character = string.Empty;
            var showGUI = false;

            //checks arguments
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    //console.writeline("Usage: {0} <CharacterName> [<ShowGUI>]", Environment.GetCommandLineArgs()[0]);
                    return;
                }
                character = args[0];
                if (args.Length > 1)
                    bool.TryParse(args[1], out showGUI);
            }

            ////creates AI client and attach close window events
            ////var client = new ControllerClient(character);
            var mainController = new MainController(character);
            
            //Console.ReadLine();
            //mainController.Dispose();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(mainController));
        }
    }
}