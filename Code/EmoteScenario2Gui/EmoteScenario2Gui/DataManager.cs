using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoteScenario2Gui
{
    class DataManager
    {

        const string DEFAULT_DATA_FILE_PATH = @".\Data\modules.txt";

        ObservableCollection<ThalamusModule> _modules;
        public static string DataFilePath
        {
            get
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    string path = args[1];
                    if (path != null && !path.Equals("")) return path;
                }
                return DEFAULT_DATA_FILE_PATH;
            }
        }

        public ObservableCollection<ThalamusModule> Modules
        {
            get { return _modules; }
            set { _modules = value; }
        }



        public static void Save(ObservableCollection<ThalamusModule> modules)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DataFilePath));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(DataFilePath))
            {
                foreach (var module in modules)
                {
                    writer.WriteLine(   module.CommandPath + "," + 
                                        module.Args+","+
                                        module.WindowX+","+
                                        module.WindowY+","+
                                        module.WindowHeigh+","+
                                        module.WindowWidth);
                }
            }
        }

        public static List<ThalamusModule> Load()
        {
            List<ThalamusModule> modules = new List<ThalamusModule>() ;
            if (File.Exists(DataFilePath))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(DataFilePath))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        var parts = line.Split(',');
                        if (parts[0].Equals("")) throw new System.IO.FileFormatException("Invalid file");
                        ThalamusModule module = new ThalamusModule();
                        module.CommandPath = parts[0];
                        module.Args = parts[1];
                        if (parts.Count() > 2)
                        {
                            try
                            {
                                module.WindowX = int.Parse(parts[2]);
                                module.WindowY = int.Parse(parts[3]);
                                module.WindowHeigh = int.Parse(parts[4]);
                                module.WindowWidth = int.Parse(parts[5]);
                            }
                            catch (Exception ex)
                            {
                                throw new System.IO.FileFormatException("Invalid file format");
                            }
                        }
                        modules.Add(module);
                        line = reader.ReadLine();
                    }
                }
            }
            return modules;
        }

        static public bool CheckIfEdited(List<ThalamusModule> modules)
        {            
            var old = Load();
            if (old == null) return true;
            if (old.Count != modules.Count) return true;
            bool edited = false;
            foreach (var m in modules)
            {
                edited = !old.Contains(m);
                if (edited) break;
            }
            return edited;
        }

    }
}
