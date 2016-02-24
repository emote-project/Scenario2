using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EuxUtils;
using System.Reflection;


namespace EmoteScenario2Gui
{
    public class ThalamusModule : ViewModelBase
    {
        public enum ModuleStatus { NotStarted, Starting, Running, Ending, EndingTimeout, Ended, Error };

        public class ThalamusModuleRunningErrorEventArgs : EventArgs
        {
            public string Message { get; set; }
        }
        public event EventHandler<ThalamusModuleRunningErrorEventArgs> ErrorEvent;
        public class ThalamusModuleStatusChangedEventArgs : EventArgs
        {
            public ModuleStatus Status { get; set; }
        }
        public event EventHandler StatusChangedEvent;



        private bool _inShell = false;
        public bool InShell
        {
            get { return _inShell; }
            set { 
                _inShell = value;
                NotifyPropertyChanged("InShell");
            }
        }

        private string _commandPath = "";
        public string CommandPath
        {
            get { return _commandPath; }
            set { 
                _commandPath = value;
                NotifyPropertyChanged("CommandPath");
                NotifyPropertyChanged("PrintableCommand");
            }
        }

        private string _args = "";
        public string Args
        {
            get { return _args; }
            set { 
                _args = value;
                NotifyPropertyChanged("Args");
                NotifyPropertyChanged("PrintableCommand");
            }
        }

        private ModuleStatus _status;
        public ModuleStatus Status
        {
            get { return _status; }
            set { 
                _status = value;
                NotifyPropertyChanged("Status");
                if (StatusChangedEvent != null) StatusChangedEvent(this, new ThalamusModuleStatusChangedEventArgs() { Status = value });
            }
        }

        private string _statusReport;
        public string StatusReport
        {
            get { return _statusReport; }
            set {
                _statusReport = value;
                NotifyPropertyChanged("StatusReport");
            }
        }

        private int _windowX;
        public int WindowX
        {
            get { return _windowX; }
            set
            {
                _windowX = value;
                NotifyPropertyChanged("WindowX");
            }
        }

        private int _windowY;
        public int WindowY
        {
            get { return _windowY; }
            set
            {
                _windowY = value;
                NotifyPropertyChanged("WindowY");
            }
        }

        private int _windowHeigh;
        public int WindowHeigh
        {
            get { return _windowHeigh; }
            set
            {
                _windowHeigh = value;
                NotifyPropertyChanged("WindowHeigh");
            }
        }

        private int _windowWidth;
        public int WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                NotifyPropertyChanged("WindowWidth");
            }
        }

        public string PrintableCommand
        {
            get
            {
                return System.IO.Path.GetFileName(CommandPath) + " " + Args;
            }
        }

        ProcessStartInfo _pi;
        Process _process;

        public async void RunAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    if (ErrorEvent!=null) ErrorEvent(this, new ThalamusModuleRunningErrorEventArgs() { Message = ex.Message});
                }
            });
        }

        public void Run()
        {
            if (Status == ModuleStatus.NotStarted || Status == ModuleStatus.Ended || Status == ModuleStatus.Error)
            {
                if (_pi == null) _pi = new ProcessStartInfo();
                _pi.CreateNoWindow = false;
                _pi.UseShellExecute = InShell;
                _pi.FileName = System.IO.Path.GetFullPath(CommandPath);
                _pi.WindowStyle = ProcessWindowStyle.Normal;
                _pi.Arguments = Args;
                _pi.WorkingDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(CommandPath));
                try
                {
                    Status = ModuleStatus.Starting;
                    StatusReport = "";
                    using (_process = Process.Start(_pi))
                    {
                        if (WindowWidth!=0 && WindowHeigh!=0) WindowsLayoutEr.RepositionWindow(_process, WindowX, WindowY, WindowWidth, WindowHeigh);
                        Status = ModuleStatus.Running;
                        _process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Status = ModuleStatus.Error;
                    StatusReport = ex.Message;
                    if (ErrorEvent != null) ErrorEvent(this, new ThalamusModuleRunningErrorEventArgs() { Message = ex.Message });
                    return;
                }
                Status = ModuleStatus.Ended;
            }
        }

        public async Task<bool> EndAsync()
        {
            return await Task.Run(() => End());
        }

        public bool End()
        {
            try
            {
                var sap = WindowsLayoutEr.GetSizeAndPosition(_process);
                WindowX = sap.X;
                WindowY = sap.Y;
                WindowWidth = sap.Width;
                WindowHeigh = sap.Heigh;
                Status = ModuleStatus.Ending;
                EndingTimeout();
                _process.CloseMainWindow();
                _process.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while closing process: " + ex.Message);
                return false;
            }
            return true;
        }

        public async void Kill()
        {
            StatusReport = "Killing process in 3..";
            await Task.Delay(1000);
            if (Status == ModuleStatus.Ended) return;
            StatusReport = "Killing process in 3..2..";
            await Task.Delay(1000);
            if (Status == ModuleStatus.Ended) return;
            StatusReport = "Killing process in 3..2..1..";
            await Task.Delay(1000);
            if (Status == ModuleStatus.Ended) return;
            StatusReport = "Killing process in 3..2..1.. KILLING";
            try
            {
                StatusReport = "";
                _process.Kill();
                //var strCmdText = "TASKILL /F /IM " + _process.ProcessName;
                //System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            }
            catch (Exception e)
            {
                StatusReport = "Error: " + e.Message;
                if (ErrorEvent != null) ErrorEvent(this, new ThalamusModuleRunningErrorEventArgs() { Message = e.Message });
                Status = ModuleStatus.Error;
                return;
            }
            Status = ModuleStatus.Ended;
        }

        private async void EndingTimeout()
        {
            await Task.Delay(5000);
            if (Status == ModuleStatus.Ending)
            {
                // Kill
                Status = ModuleStatus.EndingTimeout;
                Kill();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThalamusModule)) return false;
            ThalamusModule other = (ThalamusModule)obj;
            return this.CommandPath.Equals(other.CommandPath) &&
                    this.Args.Equals(other.Args) &&
                    this.WindowHeigh.Equals(other.WindowHeigh) &&
                    this.WindowWidth.Equals(other.WindowWidth) &&
                    this.WindowX.Equals(other.WindowX) &&
                    this.WindowY.Equals(other.WindowY);
        }

    }
}
