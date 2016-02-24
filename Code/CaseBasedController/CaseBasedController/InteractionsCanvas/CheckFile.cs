using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractionsCanvas
{
    class CheckFile : IDisposable
    {
        string _path;
        FileSystemWatcher _watcher;

        public CheckFile(string path, MainWindow _mainWindow)
        {
            _path = path;
            //if (!File.Exists(path)) throw new System.IO.FileNotFoundException(path+" not found");
            _watcher = new FileSystemWatcher();
            _watcher.Path = path;
            _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            // Only watch text files.
            //watcher.Filter = "*.txt";

            // Add event handlers.
            _watcher.Changed += delegate(object sender, FileSystemEventArgs e)
            {
                _mainWindow.ReloadCasePool();
            };
            // Begin watching.
            _watcher.EnableRaisingEvents = true;
        }



        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
