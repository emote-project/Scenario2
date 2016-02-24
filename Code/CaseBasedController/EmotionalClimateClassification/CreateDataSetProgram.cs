using System;
using System.IO;
using System.Linq;
using PS.Utilities.Collections;

namespace EmotionalClimateClassification
{
    internal class CreateDataSetProgram
    {
        private const uint MAX_FILES = 10;//19;//5; //10; //100;
        private const uint SAMPLE_INTERVAL = 4; //2;
        private const string EC_ANNOT_FILE_PATTERN = "Session*.txt";
        private const string CSV_FILE = "EC-Data";
        private const char SEPARATOR = ';';

        private static void Main(string[] args)
        {
            if (args.Length == 0) throw new ApplicationException("Sessions directory not provided in args");
            var dir = args[0];

            ProcessFiles(dir);
            Console.ReadKey();
        }

        private static void ProcessFiles(string dir)
        {
            var files = Directory.GetFiles(dir, EC_ANNOT_FILE_PATTERN);
            var numFiles = Math.Min(MAX_FILES, files.Length);
            var dataFile = string.Format("{0}\\{1} - {2} Sessions - {3} SampleSteps.csv",
                dir, CSV_FILE, numFiles, SAMPLE_INTERVAL);
            files = files.Reverse().ToArray();

            using (var streamWriter = new StreamWriter(dataFile))
            {
                //writes header
                streamWriter.WriteLine(GetHeader());

                //process all files
                for (var i = 0; i < numFiles; i++)
                    ProcessFile(files[i], streamWriter);
            }
            Console.WriteLine("Saved all data to: {0}", dataFile);
        }

        private static bool ProcessFile(string ecAnnotFile, StreamWriter writer)
        {
            Console.WriteLine("Processing EC file: {0}", ecAnnotFile);

            //creates necessary handlers
            var fileName = Path.GetFileNameWithoutExtension(ecAnnotFile);
            var dirName = Path.GetDirectoryName(ecAnnotFile);
            var rightCsvFileName = string.Format("{0}\\Right\\{1}-R.txt", dirName, fileName);
            var leftCsvFileName = string.Format("{0}\\Left\\{1}-L.txt", dirName, fileName);
            var rightProcessor = new OkaoCsvProcessor(rightCsvFileName);
            var leftProcessor = new OkaoCsvProcessor(leftCsvFileName);
            var ecProcessor = new ECAnnotationProcessor(ecAnnotFile);

            //processes
            OkaoPerception rightPerception;
            OkaoPerception leftPerception;
            uint sampleCount = 0;
            while (((rightPerception = rightProcessor.ProcessLine()) != null) &&
                   ((leftPerception = leftProcessor.ProcessLine()) != null))
            {
                if (!((double) sampleCount++%SAMPLE_INTERVAL).Equals(0)) continue;

                //gets class and writes line with data
                var time = rightPerception.Time;
                var classification = ecProcessor.GetClassification(time);
                writer.WriteLine(GetLine(leftPerception, rightPerception, time, classification));
            }

            //disposes
            rightProcessor.Dispose();
            leftProcessor.Dispose();
            ecProcessor.Dispose();
            return true;
        }

        private static string GetHeader()
        {
            var headers = OkaoPerception.HeaderStrings;
            var line = string.Format("Time;{0}", headers.ToString(SEPARATOR, false, "L-"));

            headers = OkaoPerception.HeaderStrings;
            line += string.Format(";{0};EC", headers.ToString(SEPARATOR, false, "R-"));

            return line;
        }

        private static string GetLine(
            OkaoPerception left, OkaoPerception right, double time, ECClassification classification)
        {
            var fields = left.ToStrings();
            var line = string.Format("{0};{1}", time, fields.ToString(SEPARATOR));

            fields = right.ToStrings();
            line += string.Format(";{0};{1}", fields.ToString(SEPARATOR), classification);

            return line;
        }
    }
}