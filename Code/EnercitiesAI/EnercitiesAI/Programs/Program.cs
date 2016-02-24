using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnercitiesAI.Forms;
using PS.Utilities.Math;

namespace EnercitiesAI.Programs
{
    public enum SigType
    {
        CtrlCEvent = 0,
        CtrlBreakEvent = 1,
        CloseEvent = 2,
        LogoffEvent = 5,
        ShutdownEvent = 6
    }

    internal class Program
    {
        private static EventHandler _handler;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        [STAThread]
        private static void Main(string[] args)
        {
            var character = string.Empty;

            //checks arguments
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    Console.WriteLine("Usage: {0} <CharacterName>", Environment.GetCommandLineArgs()[0]);
                    return;
                }
                character = args[0];
            }

            //creates AI client and attach close window events
            var client = new EnercitiesAIClient(character);
            //_handler += client.ConsoleClosed;
            //SetConsoleCtrlHandler(_handler, true);

            //createForm = true;
            ////creates form
            //if (createForm)
            //{
            //    ExcelUtil.EnableGraphics = true;
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new MainForm(client));
            //}
            //else
            //{
            //    client.RecoverGameSession();
            //}
            //Console.ReadLine();

            ExcelUtil.EnableGraphics = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(client));

            client.ConsoleClosed(SigType.CloseEvent);
            client.Dispose();
        }

        #region Nested type: EventHandler

        private delegate bool EventHandler(SigType sig);

        #endregion
    }
}