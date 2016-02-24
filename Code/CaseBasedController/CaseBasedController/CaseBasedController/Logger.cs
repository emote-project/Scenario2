using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseBasedController
{
    static class Logger
    {
        public static void Log(string message, object sourceClass)
        {
            string className = sourceClass != null ? sourceClass.GetType().Name : "no-source";
            
            Console.WriteLine(className+" >> "+ message+Environment.NewLine);
        }
    }
}
