using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThalamusLogFeautresExtractor;

namespace ThalamusLogFeaturesExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> arguments = new List<string>(args);

            var casePoolFilePathIndx = arguments.IndexOf(@"\cp");
            var logsFolderPathIndx = arguments.IndexOf(@"\lo");
            var thalamusMessagesDLLsFolderPathIndx = arguments.IndexOf(@"\th");
            var arffFolderIndx = arguments.IndexOf(@"\arffFolder");
            var behavioursListFileIndx = arguments.IndexOf(@"\bhList");
            var arffFileIndx = arguments.IndexOf(@"\arffFile");
            var doSimulationIndx = arguments.IndexOf(@"\simulate");
            var doSimulationAndAugmentIndx = arguments.IndexOf(@"\simulateAndAugment");
            var doMergeArffIndx = arguments.IndexOf(@"\merge");
            var doCleanArffIndx = arguments.IndexOf(@"\cleanArff");
            var doCleanArffSubIndx = arguments.IndexOf(@"\cleanArffSub");

            if (doSimulationIndx != -1 || doSimulationAndAugmentIndx != -1)
            {
                if (casePoolFilePathIndx == -1 || logsFolderPathIndx == -1 || thalamusMessagesDLLsFolderPathIndx == -1)
                {
                    Log("Arguments list is incomplete!");
                    if (casePoolFilePathIndx == -1) Log("Missing Case Pool file path");
                    if (logsFolderPathIndx == -1) Log("Missing Logs folder path");
                    if (thalamusMessagesDLLsFolderPathIndx == -1) Log("Missing Thalamus DLLs folder path");
                    PrintHelp();
                }
                else
                {
                    string casePoolFilePath = arguments[casePoolFilePathIndx + 1];
                    string logsFolderPath = arguments[logsFolderPathIndx + 1];
                    string thalamusMessagesDLLsFolderPath = arguments[thalamusMessagesDLLsFolderPathIndx + 1];

                    SimulationController sc = new SimulationController(casePoolFilePath, logsFolderPath, thalamusMessagesDLLsFolderPath, SimulationLog, LogProgress);
                    if (doSimulationIndx != -1)
                    {
                        sc.Simulate(new System.Threading.CancellationToken());
                        return;
                    }
                    if (doSimulationAndAugmentIndx != -1)
                    {
                        sc.SimulateAndAugment(new System.Threading.CancellationToken());
                        return;
                    }

                }
            }

            if (doMergeArffIndx != -1)
            {
                if (arffFolderIndx != -1)
                {
                    ARFFUtils.MergeArffs(arguments[arffFolderIndx + 1],Log);
                    return;
                }
                else
                {
                    Log("Arguments list is incomplete!");
                    if (arffFolderIndx == -1) Log("Arff folder path");
                }
            }

            if (doCleanArffIndx != -1)
            {
                if (arffFileIndx != -1 && behavioursListFileIndx != -1)
                {
                    ARFFUtils.CleanARFF(arguments[arffFileIndx + 1], arguments[behavioursListFileIndx + 1], Log);
                    return;
                }
                else
                {
                    Log("Arguments list is incomplete!");
                    if (arffFileIndx == -1) Log("Arff file path");
                    if (behavioursListFileIndx == -1) Log("Missing Behaviour List file path");
                }
            }

            if (doCleanArffSubIndx != -1)
            {
                if (arffFileIndx != -1 && behavioursListFileIndx != -1)
                {
                    ARFFUtils.CleanARFF_subcategories(arguments[arffFileIndx+1],Log);
                    return;
                }
                else
                {
                    Log("Arguments list is incomplete!");
                    if (arffFileIndx == -1) Log("Arff file path");
                }
            }


            PrintHelp();
            Console.WriteLine("\n\n\nPress any key to close");
            Console.ReadLine();
        }

        static private void Log(string text)
        {
            Console.WriteLine(">>: " + text);
        }

        static int lastProgress = 0;
        static private void LogProgress(double val)
        {
            if ((int)val != lastProgress)
            {
                Console.WriteLine("Progress>> " + (int)val + "%");
                lastProgress = (int)val;
            }
        }

        static private void SimulationLog(string text)
        {
            Console.WriteLine("LOG: " + text);
        }

        static private void PrintHelp()
        {
            Console.WriteLine("\nHELP:");
            Console.WriteLine("Available commands: \n"+
                            "\t [(\\simulate | \\simulateAndAugment) \\cp <CasePoolPath> \\lo <LogsFolderPath> \\th <ThalamusMessagesDLLs> ]\n" +
                            "\t [ \\merge \\arffFolder <ArffFolderPath> ] \n"+
                            "\t [ \\clean \\bhList <BehaviourListPath> \\arffFile <ArffToCleanPath> ]  \n"+
                            "\t [ \\cleanArffSub \\arffFile <ArffToCleanPath> ]");
            Console.WriteLine("\nThe command don't need to have a specific order.");
            Console.WriteLine("Arffs are elaborated after the simulations so it is possible to do operations on arffs not yet created");
        }
    }
}
