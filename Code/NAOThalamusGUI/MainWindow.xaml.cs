using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAOThalamus;
using System.Threading.Tasks;
using Thalamus;
using NAORemote;
using Renci.SshNet;
using NAOIpDiscoverer;

namespace NAOThalamusGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string COMMAND_RUNPYTHON = "python NAOThalamus/naoqiXmlrpc.py > NAOThalamus/Log_Python.txt";
        const string COMMAND_CHECKPYTHON = "ps -A | grep python2.7";
        const string COMMAND_CHECKNAOQI = "ps -A | grep naoqi";
        const string COMMAND_KILLPYTHON = @"kill -9 $(ps -A | grep python2.7 | awk '{print $1;}')";
        const string COMMAND_RESTART_NAOQI = @"echo nao | sudo /etc/init.d/naoqi restart";

        NAOThalamusClient Client;
        
        bool pythonRunning = false;
        string charName = "";

        Discoverer naoDiscoverer = new Discoverer();
        NAOIpDiscoverer.NAOHost selectedNao;

        public MainWindow()
        {
            InitializeComponent();
            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 2)
            {
                charName = args[1];
                //naoAddress = args[2];
                //txtAddress.Text = naoAddress;
                txtCharName.Text = charName;
            }
        }


        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //CheckNaoqiStatusAsync();
            //// Use this always as last command because, if awaited, it will block the flow of the code
            //// to this point untill the python script will end or will be killed.
            //RunPythonAsync();

            Boot();
            txtLogs.Text = "";
        }

        async private void Boot()
        {
            cmbNaosIP.IsEnabled = false;
            btnConnect.Content = "Searching..";
            btnConnect.IsEnabled = false;
            await naoDiscoverer.DiscoverNAO();

#if DEBUG
            // DEBUG<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<DELETE ME
            naoDiscoverer.NAOs.Add(new NAOHost("127.0.0.1","Fake"));
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
#endif
            cmbNaosIP.ItemsSource = naoDiscoverer.NAOs;
            cmbNaosIP.Items.Refresh();
            if (naoDiscoverer.NAOs.Count > 0)
            {
                cmbNaosIP.SelectedIndex = 0;
                cmbNaosIP.IsEnabled = true;
                btnConnect.Content = "Connect";
                btnConnect.IsEnabled = true;

                ConnectThalamusAsync();
                CheckNaoqiStatusAsync();
                RunPythonAsync();
            }
            else
            {
                btnConnect.Content = "Not found";
                await Task.Delay(1000);
                btnConnect.Content = "Rerying in 3";
                await Task.Delay(1000);
                btnConnect.Content = "Rerying in 2";
                await Task.Delay(1000);
                btnConnect.Content = "Rerying in 1";
                await Task.Delay(1000);
                Boot();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Client!=null)
                Client.Dispose();
            //if (pythonRunning)
            //{
            //    e.Cancel = true;
            //    await StopPythonAsync();
            //    System.Threading.Thread.Sleep(500);     //TODO now i'm cheating here. If i don't wait a few moments the app closes before it actually stops th python script. Something goes wrong with timings..
            //    this.Close();
            //}
            Application.Current.Shutdown();
        }


        private async Task<NAOThalamusClient> ConnectThalamusAsync()
        {
            Client = await Task.Run<NAOThalamusClient>((() => ConnectThalamus()));
            if (Client != null)
            {
                Client.ClientConnected += Client_ClientConnected;
                Client.ClientDisconnectedFromThalamus += Client_ClientDisconnectedFromThalamus;
                // Client.DisconnectedFromMaster override di questo nel client e trasformalo in event
                Client.EventLogged += Client_EventLogged;
            }
            return Client;
        }

        private NAOThalamusClient ConnectThalamus(){
            //LogClientStatus("Connecting...");
            if (CheckNaoIsSelected())
            {
                NAOThalamusClient client = new NAOThalamusClient(charName, selectedNao.IP);
                return client;
            }
            else return null;
        }

        private NAOThalamusClient DisconnectThalamus()
        {
            LogClientStatus("Disconnecting..");
            if (Client != null)
            {
                Client.Dispose();
            }
            LogClientStatus("Not connected");
            return Client;
        }

        private Task<NAOThalamusClient> DisconnectThalamusAsync()
        {
            return Task.Run<NAOThalamusClient>(() => DisconnectThalamus());
        }

        private async void ReconnectThalamusClientAsync(){
            charName = txtCharName.Text;
            if (CheckNaoIsSelected())
            {
                btnReconnectClient.IsEnabled = false;
                await DisconnectThalamusAsync();
                await ConnectThalamusAsync();
                btnReconnectClient.IsEnabled = true;
            }
        }

        private async void RestartPythonAsync()
        {
            btnRestartPython.IsEnabled = false;
            await StopPythonAsync();
            LogPythonStatus("Restarting...");
            btnRestartPython.IsEnabled = true;
            await RunPythonAsync();
        }

        private async void CheckNaoqiStatusAsync()
        {
            LogNaoQiStatus("Checking if it is running...");
            SshCommand remoteCommand = await ExecuteCommandAsync(COMMAND_CHECKNAOQI);
            if (remoteCommand != null)
            {
                if (remoteCommand.Result != "")
                {
                    LogNaoQiStatus("Running");
                }
                else
                {
                    LogNaoQiStatus("Not running");
                }
            }
            else
            {
                LogNaoQiStatus("Can't connect to NAO");
            }
        }

        void Client_EventLogged(LogEntry logEntry)
        {
            Console.WriteLine("Thalamus Event: " + logEntry.ToString());
        }

        void Client_ClientDisconnectedFromThalamus(string name, string oldClientId)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                LogClientStatus("Disconnected");
            }));
        }

        

        void Client_ClientConnected()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                LogClientStatus("Connected");
            }));
            
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Client.Dispose();
        }

        private void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client.IsConnected)
                Client.Dispose();
            Client.Start();
        }

        /// <summary>
        /// WARNING: this method will end only when the pthon script ends as well. This means that if this method is "awaited" 
        /// it will keep the awaiting method awaiting untill the python script ends or gets killed
        /// </summary>
        /// <returns></returns>
        private async Task<SshCommand> RunPythonAsync()
        {
            LogPythonStatus("Checking if Python is already running...");
            SshCommand remoteCommand = await ExecuteCommandAsync(COMMAND_CHECKPYTHON);
            if (remoteCommand != null)
            {
                if (remoteCommand.Result != "")
                {
                    LogPythonStatus("Python was already running. No need to restart.");
                }
                else
                {
                    LogPythonStatus("Python is running");
                    remoteCommand = await ExecuteCommandAsync(COMMAND_RUNPYTHON);
                    if (remoteCommand != null)
                        LogPythonStatus("Python not running");
                    else
                        LogPythonStatus("Can't connect to NAO");
                }
                pythonRunning = true;
            }
            else
            {
                LogPythonStatus("Can't connect to NAO");
            }
            return remoteCommand;
        }

        private async Task<SshCommand> StopPythonAsync()
        {
            LogPythonStatus("Stopping..");
            var cmd = await ExecuteCommandAsync(COMMAND_KILLPYTHON);
            if (cmd != null)
                LogPythonStatus("Stopped");
            else
                LogPythonStatus("Can't connect to NAO");
            pythonRunning = false;
            return cmd;
        }

        private async Task<SshCommand> RestartNaoQiAsync()
        {
            LogNaoQiStatus("Restarting...");
            var cmd = await ExecuteCommandAsync(COMMAND_RESTART_NAOQI);
            if (cmd != null)
                LogNaoQiStatus("Restarted");
            else
                LogNaoQiStatus("Can't connect to NAO");
            return cmd;
        }

        private async Task<SshCommand> ExecuteCommandAsync(string command)
        {
            if (selectedNao == null)
            {
                System.Windows.MessageBox.Show("No address selected for NAO", "No address", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                NAORemote.NAORemote remote = new NAORemote.NAORemote(selectedNao.IP, txtUsername.Text, txtPassword.Text, "/home/nao");
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
            Console.WriteLine("Command execution exception: "+message);
            if (e.Message.Contains("Kill") || e.Message.Contains("kill")) 
                return;
            //MessageBox.Show(message, "Command execution exception", MessageBoxButton.OK, MessageBoxImage.Error);
            Dispatcher.BeginInvoke((Action)(() =>
            {
                txtLogs.Text = txtLogs.Text+ System.Environment.NewLine + "REMOTE COMMAND ERROR: "+message;
            }));
                
        }

        private async void btnKillPython_Click(object sender, RoutedEventArgs e)
        {
            btnKillPython.IsEnabled = false;
            LogPythonStatus("Killing process...");
            await StopPythonAsync();
            btnKillPython.IsEnabled = true;
        }

        private void btnRestartPython_Click(object sender, RoutedEventArgs e)
        {
            RestartPythonAsync();
        }

        
        private void btnReconnectClient_Click(object sender, RoutedEventArgs e)
        {
            ReconnectThalamusClientAsync();
        }

        private async void btnRestartNaoqi_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to restart NaoQi?"+System.Environment.NewLine+"THIS WILL SET OFF MOTORS STIFFNESS."+System.Environment.NewLine+"Be sure to hold the robot to avoid falls", "Restarting NaoQi", MessageBoxButton.YesNo,MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                await RestartNaoQiAsync();
            }
        }

        private void LogClientStatus(string message)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtClientStatus.Text = message;
            }));
        }

        private void LogPythonStatus(string message)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtPythonStatus.Text = message;
            }));
        }

        private void LogNaoQiStatus(string message)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtNaoQiStatus.Text = message;
            }));
        }

        //private void btnConnect_Click(object sender, RoutedEventArgs e)
        //{
        //    naoAddress = selectedNao.IP;

        //    if ( Client.IsConnected)
        //    {
        //        var res = MessageBox.Show("You should reconnect this ThalamusClient to update the new robot address correctly", "Reconnect thalamus client?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        //        if (res == MessageBoxResult.Yes)
        //        {
        //            ReconnectThalamusClientAsync();
        //        }
        //    }
        //    if (pythonRunning)
        //    {
        //        var res = MessageBox.Show("Run python on the new address?", "Run python?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
        //        if (res == MessageBoxResult.Yes)
        //        {
        //            RestartPythonAsync();
        //        }
        //    }
        //}

        private void cmbNaosIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedNao = (NAOHost) cmbNaosIP.SelectedItem;
        }


        private bool CheckNaoIsSelected()
        {
            if (selectedNao == null)
            {
                System.Windows.MessageBox.Show("No address defined for NAO.", "No address", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else
                return true;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
