using CaseBasedController.Simulation;
using DetectorAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DetectorAnalyzer.ArffLoaders;

namespace ThalamusLogFeautresExtractor
{
    public class ARFFUtils
    {
        public delegate void LogDelegate(string statusMessage);

        
        static public void ExportARFF(string outputFileName, bool exportWithTime, FeaturesCollector fc, LogDelegate logHandler = null)
        {
            if (logHandler!=null) logHandler("Exporting ARFF file...");
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                writer.WriteLine("@RELATION emoteExample");
                if (exportWithTime)
                    writer.WriteLine("@ATTRIBUTE Time NUMERIC");
                for (int k = 0; k < fc.FeaturesNames.Count; k++)
                {
                    if (fc.FeaturesNames[k].Equals(FeaturesCollector.CLASS_TO_LEARN_NAME))
                    {
                        // Writing all the Behaviour field possible states
                        List<string> states = fc.FeaturesVectors.Select(x => x[x.Length - 1]).Distinct().ToList();
                        string statesStr = "";
                        foreach (string s in states) statesStr += s + ",";
                        statesStr.TrimEnd(',');
                        writer.WriteLine("@ATTRIBUTE " + fc.FeaturesNames[k] + " {" + statesStr + "}");
                    }
                    else
                        writer.WriteLine("@ATTRIBUTE " + fc.FeaturesNames[k] + " {1,0}");
                }

                writer.WriteLine("@DATA");
                foreach (string[] fv in fc.FeaturesVectors)
                {
                    for (int k = 0; k < fv.Length; k++)
                    {
                        if (k == 0)         // If we are exporting the time as well, than we put it as first field
                            if (exportWithTime)
                                writer.Write(fc.FeaturesVectorsTime[fc.FeaturesVectors.IndexOf(fv)].TotalSeconds + ",");
                        // the last element is the class
                        writer.Write(fv[k]);
                        writer.Write((k == (fv.Length - 1)) ? System.Environment.NewLine : ",");
                    }
                }
            }
            if (logHandler!=null) logHandler("Exporting ARFF file...Completed");
        }

        static public void MergeArffs(string path, LogDelegate logHandler)
        {
            if (logHandler != null) logHandler("Starting merge...");
            List<string> arffs = Directory.GetFiles(path).Where(f => Path.GetExtension(f).Equals(".arff")).ToList<string>();
            List<string> attributes = new List<string>();
            List<string> behaviours = new List<string>();
            List<string> data = new List<string>();
            foreach (string arff in arffs)
            {
                using (StreamReader reader = new StreamReader(arff))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                        if (line.StartsWith("@"))
                        {
                            if (line.StartsWith("@ATTRIBUTE Behaviour"))
                            {
                                string be = line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{') - 2);
                                var x = be.Split(',').ToList<string>();
                                behaviours.AddRange(x);
                                behaviours = behaviours.Distinct().ToList();
                            }
                            else
                            {
                                if (!attributes.Contains(line))
                                    attributes.Add(line);
                            }
                        }
                        else
                        {
                            data.Add(line);
                        }
                }
            }
            using (StreamWriter writer = new StreamWriter(path + "\\merged.arff"))
            {
                foreach (string a in attributes)
                {
                    if (!a.Equals("@DATA"))
                        writer.WriteLine(a);
                }
                writer.Write("@ATTRIBUTE Behaviour {");
                for (int i = 0; i < behaviours.Count; i++)
                {
                    writer.Write(behaviours[i]);
                    if (i != behaviours.Count - 1) writer.Write(",");
                }
                writer.Write("}\n");
                writer.WriteLine("@DATA");
                foreach (string d in data) writer.WriteLine(d);
            }
            if (logHandler != null) logHandler("File saved at: "+ path + ".merged.arff\n");
            if (logHandler != null) logHandler("...DONE\n\n");
        }

        static public void CleanARFF(string arffPath, string behaviourToIgnoreFilePath, LogDelegate logHandler = null)
        {
            if (logHandler != null) logHandler("Loading behaviours to clear:");
            List<string> behavioursToIgnore = new List<string>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(behaviourToIgnoreFilePath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    behavioursToIgnore.Add(line);
                    if (logHandler != null) logHandler(line);
                }
            }
            if (logHandler != null) logHandler("Loaded "+behavioursToIgnore.Count+" behaviours to ignore:\n");

            CleanARFF(arffPath, behavioursToIgnore, logHandler);
        }

        static public void CleanARFF(string arffPath, List<string> behavioursToRemove, LogDelegate logHandler = null)
        {
            if (logHandler != null) logHandler("Cleaning arff at: "+arffPath);
            List<string> linesToSave = new List<string>();
            using (StreamReader reader = new StreamReader(arffPath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    bool add = true;
                    if (line.Contains(@"@ATTRIBUTE Behaviour"))
                    {
                        var temp = line.Substring(line.IndexOf('{') + 1, line.Length - (line.IndexOf('{') + 2));
                        List<string> tempArr = temp.Split(',').ToList();
                        line = @"@ATTRIBUTE Behaviour{";
                        foreach (var t in tempArr)
                        {
                            if (!behavioursToRemove.Contains(t))
                            {
                                line += t+",";
                            }
                        }
                        line = line.TrimEnd(',');
                        line += "}";
                    } else {
                        foreach(var b in behavioursToRemove){
                            if (line.Contains(b))
                            {
                                add = false;
                            }
                        }
                    }
                    if (add) linesToSave.Add(line);
                }
            }
            using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(arffPath,".cleaned.arff")))
            {
                foreach (string line in linesToSave)
                {
                    writer.WriteLine(line);
                }
            }
                        if (logHandler != null) logHandler("New arff created at: " + Path.ChangeExtension(arffPath, ".cleaned.arff")+"\nDONE.\n\n");
        }

        static public void CleanARFF_subcategories(string arffPath, LogDelegate logHandler = null)
        {
            if (logHandler != null) logHandler("Cleaning arff at: " + arffPath);
            using (StreamReader reader = new StreamReader(arffPath))
            {
                using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(arffPath, "noSubcategories.arff")))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        var temp = Regex.Replace(line, @"\.\w+(,|\n|)", "");
                        temp = Regex.Replace(temp, @"\.\w+\}", "}");
                        temp = Regex.Replace(temp, @"(?<!(\{|,))IFMLSpeech", ",IFMLSpeech");

                        if (temp.Contains(@"@ATTRIBUTE Behaviour"))
                        {
                            var t = temp.Substring(temp.IndexOf('{') + 1, temp.Length - (temp.IndexOf('{') + 2));
                            List<string> tempArr = t.Split(',').ToList();
                            tempArr = tempArr.Distinct().ToList();
                            temp = "@ATTRIBUTE Behaviour{";
                            foreach (var be in tempArr)
                            {
                                temp += be + ",";
                            }
                            temp = temp.TrimEnd(',');
                            temp += "}";
                        }

                        writer.WriteLine(temp);
                    }
                }
            }
            if (logHandler != null) logHandler("New arff created at: " + Path.ChangeExtension(arffPath, ".noSubcategories.arff") + "\nDONE.\n\n");
        }

        /// <summary>
        /// Compute statistics over a set of arff files contained in a folder
        /// </summary>
        /// <param name="arffsPath"></param>
        /// <returns></returns>
        static public StatisticsArffLoader ComputeStatistics(string arffsPath, LogDelegate logHandler)
        {
            if (logHandler != null) logHandler("Computing statistics...");
            StatisticsArffLoader statisticsAnalyzer = null;
            if (Directory.Exists(arffsPath))
            {
                var arffs = Directory.GetFiles(arffsPath, "*.arff");
                foreach (string arffPath in arffs)
                {
                    if (statisticsAnalyzer == null)
                        statisticsAnalyzer = new StatisticsArffLoader();

                    statisticsAnalyzer.Load(arffPath);
                }
                if (statisticsAnalyzer != null) statisticsAnalyzer.PrintResults(arffsPath + "\\stats");
            }
            else
            {
                throw new DirectoryNotFoundException("Couldn't find directory " + arffsPath);
            }
            if (logHandler != null) logHandler("New statistics created at: " + arffsPath + "\\stats \nDONE.\n\n");
            return statisticsAnalyzer;
        }

    }
}
