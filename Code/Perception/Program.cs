using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Perception
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string character = "";
            if (args.Length > 0)
            {
                character = args[0];
            }
            if (args.Length > 1)
                Form1.scenarioselected = Int16.Parse(args[1]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(character));
        }
    }
}
