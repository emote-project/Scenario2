using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaseBasedController;
using Thalamus;

namespace WekaDataPreprocessor
{
    class Program
    {
        static CasePool casePool;
        static List<Thalamus.LogEntry> thalamusLog;

        static void Main(string[] args)
        {
            casePool = CaseBasedController.Programs.CasePoolCodingProgram.Pool1();
            thalamusLog = ThalamusLogTool.LogTool.LoadThalamusLogEntries(@"C:\Users\Eugenio\Desktop\test.log");

            // fetch every entry in the loaded log and check the activations of the detectors in the pool
            foreach (LogEntry e in thalamusLog)
            {
                
            }
        }
    }
}
