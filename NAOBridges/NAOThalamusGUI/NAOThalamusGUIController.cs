using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EuxHelpers;
using NAOIpDiscoverer;
using Renci.SshNet;
using NAOThalamus;
using System.IO;

namespace NAOThalamusGUI
{
    public class NAOStatus : ViewModelBase
    {
        private bool isPythonRunning = false;
        public bool IsPythonRunning
        {
            get { return isPythonRunning; }
            set { 
                isPythonRunning = value;
                NotifyPropertyChanged("IsPythonRunning");
            }
        }

        private bool isNAOQiRunning = false;
        public bool IsNAOQiRunning
        {
            get { return isNAOQiRunning; }
            set { 
                isNAOQiRunning = value;
                NotifyPropertyChanged("IsNAOQiRunning");
            }
        }

        private bool isThalamusConnected = false;
        public bool IsThalamusConnected
        {
            get { return isThalamusConnected; }
            set { 
                isThalamusConnected = value;
                NotifyPropertyChanged("IsThalamusConnected");
            }
        }
        private bool isSearchingForNAO = false;
        public bool IsSearchingForNAO
        {
            get { return isSearchingForNAO; }
            set { 
                isSearchingForNAO = value;
                NotifyPropertyChanged("IsSearchingForNAO");
            }
        }
        private ObservableCollection<NAOHost> discoveredNAOs = new ObservableCollection<NAOHost>();
        public ObservableCollection<NAOHost> DiscoveredNAOs
        {
            get { return discoveredNAOs; }
            set { discoveredNAOs = value; }
        }

        private NAOHost selectedNAO;
        public NAOHost SelectedNAO
        {
            get { return selectedNAO; }
            set { 
                selectedNAO = value;
                NotifyPropertyChanged("SelectedNAO");
            }
        }

        private bool isPythonInstalled = false;
        public bool IsPythonInstalled
        {
            get { return isPythonInstalled; }
            set
            {
                isPythonInstalled = value;
                NotifyPropertyChanged("IsPythonInstalled");
            }
        }

        private bool isPythonInstalling = false;
        public bool IsPythonInstalling
        {
            get { return isPythonInstalling; }
            set
            {
                isPythonInstalling = value;
                NotifyPropertyChanged("IsPythonInstalling");
            }
        }

        private bool isBehaviourInstalling = false;
        public bool IsBehaviourInstalling
        {
            get { return isBehaviourInstalling; }
            set
            {
                isBehaviourInstalling = value;
                NotifyPropertyChanged("IsBehaviourInstalling");
            }
        }

        private bool isBehaviourInstalled = false;
        public bool IsBehaviourInstalled
        {
            get { return isBehaviourInstalled; }
            set
            {
                isBehaviourInstalled = value;
                NotifyPropertyChanged("IsBehaviourInstalled");
            }
        }

        private bool isBehaviourChecking = false;
        public bool IsBehaviourChecking
        {
            get { return isBehaviourChecking; }
            set
            {
                isBehaviourChecking = value;
                NotifyPropertyChanged("IsBehaviourChecking");
            }
        }

        private int uploadStatus = 0;
        public int UploadStatus
        {
            get { return uploadStatus; }
            set
            {
                uploadStatus = value;
                NotifyPropertyChanged("UploadStatus");
            }
        }

        private string behavioursUploadStatusDescription = "";
        public string BehavioursUploadStatusDescription
        {
            get { return behavioursUploadStatusDescription; }
            set
            {
                behavioursUploadStatusDescription = value;
                NotifyPropertyChanged("BehavioursUploadStatusDescription");
            }
        }

        private string pythonUploadStatusDescription = "";
        public string PythonUploadStatusDescription
        {
            get { return pythonUploadStatusDescription; }
            set
            {
                pythonUploadStatusDescription = value;
                NotifyPropertyChanged("PythonUploadStatusDescription");
            }
        }

    }

    public class NAOThalamusGUIController
    {
        public class NAOThalamusGUIErrorEventArgs : EventArgs
        {
            public string Description { get; set; }
        }
        public event EventHandler<NAOThalamusGUIErrorEventArgs> Error;

        const string COMMAND_RUNPYTHON = "python NAOThalamus/naoqiXmlrpc.py";
        const string COMMAND_CHECKPYTHON = "ps -A | grep python2.7";
        const string COMMAND_CHECKNAOQI = "ps -A | grep naoqi";
        const string COMMAND_KILLPYTHON = @"kill -9 $(ps -A | grep python2.7 | awk '{print $1;}')";
        const string COMMAND_RESTART_NAOQI = @"echo nao | sudo /etc/init.d/naoqi restart";

        SshShellControl.SshShellControl shellControl;
        NAORemote.NAORemote naoRemote;
        NAOThalamusClient naoThalamusClient;
        NAOStatus status;
        Discoverer naoDiscoverer = new Discoverer();
        


        public NAOThalamusGUIController(SshShellControl.SshShellControl shellControl, NAOStatus status)
        {
            this.shellControl = shellControl;
            this.status = status;
        }

        public async void Start()
        {
            
            status.IsSearchingForNAO = true;
            while (status.IsSearchingForNAO)
            {
                await naoDiscoverer.DiscoverNAO();
                if (naoDiscoverer.NAOs.Count > 0)
                {
                    naoRemote = new NAORemote.NAORemote(naoDiscoverer.NAOs[0].IP, "nao", "nao");
                    naoRemote.Connect();
                    status.IsSearchingForNAO = false;
                    status.DiscoveredNAOs = new ObservableCollection<NAOHost>(naoDiscoverer.NAOs);
                    status.SelectedNAO = naoDiscoverer.NAOs[0];
                }
            }
            string character = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : "";
            naoThalamusClient = await ConnectThalamusAsync(character, status.SelectedNAO.IP);

            CheckPythonInstalledAsync();
            CheckBehavioursInstalledAsync();

            await shellControl.Connect(status.SelectedNAO.IP, "nao", "nao");
            
            if (!await CheckIsPythonRunning())
            {
                RunPythonAsync();
                PythonChecker();
                NaoQiChecker();
            }
            else
            {
                LogError("Python already running. You have to manually stop it and run again this application");
            }
        }

        #region Python

        public async void RunPythonAsync()
        {
            await shellControl.SendCommandAsync(COMMAND_RUNPYTHON);
            await Task.Delay(5000);
        }

        public async void StopPythonAsync()
        {
            if (status!=null && status.IsPythonRunning)
                await shellControl.SendCommandAsync("");
        }

        public async void KillPythonAsync()
        {
            await ExecuteCommandAsync(COMMAND_KILLPYTHON);
        }

        private async Task<bool> CheckIsPythonRunning()
        {
            SshCommand remoteCommand = await ExecuteCommandAsync(COMMAND_CHECKPYTHON);
            if (remoteCommand !=null && remoteCommand.Result.Equals(""))
                return false;
            else
                return true;
        }
        private async void PythonChecker()
        {
            if (await CheckIsPythonRunning())
                status.IsPythonRunning = true;
            else
                status.IsPythonRunning = false;
            await Task.Delay(10000);
            PythonChecker();
        }

        #endregion

        #region NAOQI

        private async Task<bool> CheckNaoqiStatusAsync()
        {
            SshCommand remoteCommand = await ExecuteCommandAsync(COMMAND_CHECKNAOQI);
            if (remoteCommand != null && remoteCommand.Result != "")
                return true;
            else
                return false;
        }
        private async void NaoQiChecker()
        {
            if (await CheckNaoqiStatusAsync())
                status.IsNAOQiRunning = true;
            else
                status.IsNAOQiRunning = false;
            await Task.Delay(10000);
            NaoQiChecker();
        }


        public async void RestartNaoQiAsync()
        {
            await ExecuteCommandAsync(COMMAND_RESTART_NAOQI);
        }

        #endregion

        #region Thalamus

        private async Task<NAOThalamusClient> ConnectThalamusAsync(string charName, string naoIP)
        {
            naoThalamusClient = await Task.Run<NAOThalamusClient>((() => ConnectThalamus(charName, naoIP)));
            if (naoThalamusClient != null)
            {
                naoThalamusClient.ClientConnected += delegate() { status.IsThalamusConnected = true; };
                naoThalamusClient.ClientDisconnected += delegate() { status.IsThalamusConnected = false; };
            }
            return naoThalamusClient;
        }


        private NAOThalamusClient ConnectThalamus(string charName, string naoIP)
        {
            NAOThalamusClient client = new NAOThalamusClient(charName, naoIP);
            return client;
        }


        public void DisconnectThalamus()
        {
            if (naoThalamusClient != null)
            {
                naoThalamusClient.Dispose();
            }
        }

        #endregion

        #region remote commands

        private async Task<SshCommand> ExecuteCommandAsync(string command)
        {
            if (status.SelectedNAO == null)
            {
                LogError("No address selected for NAO");
            }
            else
            {
                NAORemote.NAORemote remote = new NAORemote.NAORemote(status.SelectedNAO.IP, "nao", "nao", "/home/nao");
                remote.Error += remote_Error;
                bool connected = await remote.ConnectAsync();
                if (connected)
                {
                    var remoteCommand = await remote.ExecuteCommandAsync(command);
                    remote.Disconnect();
                    return remoteCommand;
                }
            }
            return null;
        }

        void remote_Error(object sender, NAORemote.NAORemote.ErrorArgs e)
        {
            string message = e.Message + System.Environment.NewLine;
            Console.WriteLine("Command execution exception: " + message);
            if (e.Message.Contains("Kill") || e.Message.Contains("kill"))
                return;
        }

        private void LogError(string message)
        {
            if (Error != null)
            {
                Error(this, new NAOThalamusGUIErrorEventArgs() { Description = message });
            }
        }

        #endregion 

        #region INSTALLING FILES

        private async void CheckPythonInstalledAsync()
        {
            //naoRemote = new NAORemote.NAORemote(status.SelectedNAO.IP, "nao", "nao");
            //naoRemote.Connect(); pythonVersion0.flag
            var temp = Directory.GetFiles("./python");
            var versionFileName = "";
            foreach (string fileName in temp)
            {
                if (fileName.Contains("pythonVersion"))
                {
                    versionFileName = Path.GetFileName(fileName);
                    continue;
                }
            }
            var res = await naoRemote.ExecuteCommandAsync("[ -f NAOThalamus/"+versionFileName+" ] && echo 1 || echo 0");
            if (res.Result != null && res.Result.TrimEnd(new char[] { '\n' }).Equals("0"))
            {
                status.IsPythonInstalled = false;
            }
            else
            {
                status.IsPythonInstalled = true;
            }
        }

        private async void CheckBehavioursInstalledAsync()
        {
            status.IsBehaviourChecking = true;
            //naoRemote = new NAORemote.NAORemote(status.SelectedNAO.IP, "nao", "nao");
            //naoRemote.Connect();
            var temp = Directory.GetFiles("./BehavioursInstaller");
            var versionFileName = "";
            foreach (string fileName in temp)
            {
                if (fileName.Contains("behaviourVersion"))
                {
                    versionFileName = Path.GetFileName(fileName);
                    continue;
                }
            }
            var res = await naoRemote.ExecuteCommandAsync("[ -f "+versionFileName+" ] && echo 1 || echo 0");
            if (res.Result != null && res.Result.TrimEnd(new char[] { '\n' }).Equals("0"))
            {
                status.IsBehaviourInstalled = false;
            }
            else
            {
                status.IsBehaviourInstalled = true;
            }
            status.IsBehaviourChecking = false;
        }

        public async Task<bool> InstallPythonAsync()
        {
            status.IsPythonInstalling = true;
            try
            {
                await Task.Run(() =>
                {
                    //naoRemote = new NAORemote.NAORemote(status.SelectedNAO.IP, "nao", "nao");
                    //naoRemote.Connect();
                    var files = Directory.GetFiles("./python");
                    status.PythonUploadStatusDescription = "Creating NAOThalamus directory";
                    naoRemote.Connect();
                    naoRemote.ExecuteCommand("mkdir NAOThalamus");
                    int i=0;
                    foreach (var file in files)
                    {
                        status.PythonUploadStatusDescription = "Installing " + Path.GetFileName(file) + "(" + i++ + "/" + files.Count() + ")";
                        naoRemote.UploadFile(file, UploadStatus, "NAOThalamus/");
                    }
                });
                status.IsPythonInstalled = true;
                status.IsPythonInstalling = false;
                status.PythonUploadStatusDescription = "";
                status.UploadStatus = 0;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Can't install python files on the robot: " + e.Message, "Python installation error");
                status.IsPythonInstalling = false;
                status.IsPythonInstalled = false;
            }
            return status.IsPythonInstalled;
        }

        public async Task<bool> InstallBehavioursAsync()
        {
            status.IsBehaviourInstalling = true;
            try
            {
                await Task.Run(() =>
                {
                    //naoRemote = new NAORemote.NAORemote(status.SelectedNAO.IP, "nao", "nao");
                    //naoRemote.Connect();
                    var files = Directory.GetFiles("./BehavioursInstaller");
                    status.BehavioursUploadStatusDescription = "Creating temporary behaviour directory";
                    naoRemote.ExecuteCommand("mkdir BehavioursInstaller");
                    int i = 0;
                    foreach (var file in files)
                    {
                        status.BehavioursUploadStatusDescription = "Installing " + Path.GetFileName(file) + "(" + i++ + "/" + files.Count() + ")";
                        naoRemote.UploadFile(file, UploadStatus, "BehavioursInstaller/");
                    }
                    status.BehavioursUploadStatusDescription = "Executing installer script...";
                    naoRemote.ExecuteCommand("cd BehavioursInstaller; python installBehaviors.py; cp behaviourVersion*.flag ..; cd ..; rm -R BehavioursInstaller;");
                    status.BehavioursUploadStatusDescription = "Execution terminated";
                });
                status.IsBehaviourInstalled = true;
                status.IsBehaviourInstalling = false;
                status.BehavioursUploadStatusDescription = "";
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Can't install Behaviours on the robot: " + e.Message, "Behaviours installation error");
                status.IsBehaviourInstalling = false;
                status.IsBehaviourInstalled = true;
            }
            return status.IsBehaviourInstalling;
        }

        private void UploadStatus(ulong s)
        {
            status.UploadStatus = (int)(s*100);
        }


        #endregion
    }
}
