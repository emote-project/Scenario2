using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EuxHelpers;

namespace ThalamusLogPlayer
{
    public class MainWindowViewModel : ViewModelBase
    {
        string logPath;
        public string LogPath
        {
            get { return logPath; }
            set { 
                logPath = value;
                NotifyPropertyChanged("LogPath");
            }
        }

        double progress = 0;
        public double Progress
        {
            get { return progress; }
            set { 
                progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        bool canPlay = false;
        public bool CanPlay
        {
            get { return canPlay; }
            set { 
                canPlay = value;
                NotifyPropertyChanged("CanPlay");
            }
        }

        bool isPlaying = false;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set {
                isPlaying = value;
                NotifyPropertyChanged("IsPlaying");
                CalculateIfCanPlay();
            }
        }

        bool isConnectedToThalamus = false;
        public bool IsConnectedToThalamus
        {
            get { return isConnectedToThalamus; }
            set { 
                isConnectedToThalamus = value;
                NotifyPropertyChanged("IsConnectedToThalamus");
                CalculateIfCanPlay();
            }
        }

        bool isLogLoaded = false;
        public bool IsLogLoaded
        {
            get { return isLogLoaded; }
            set { 
                isLogLoaded = value;
                NotifyPropertyChanged("IsLogLoaded");
                CalculateIfCanPlay();
            }
        }

        bool isLogLoading = false;
        public bool IsLogLoading
        {
            get { return isLogLoading; }
            set { 
                isLogLoading = value;
                NotifyPropertyChanged("IsLogLoading");
                CalculateIfCanPlay();
            }
        }
        
        private void CalculateIfCanPlay(){
            CanPlay = !isLogLoading && IsLogLoaded && IsConnectedToThalamus && !IsPlaying;
        }
    }
    
    public partial class MainWindow : Window
    {
        LogPlayerClient logPlayer;
        MainWindowViewModel data;

        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            string charName = "";
            if (args.Length > 1)
                charName = args[1];
            logPlayer = new LogPlayerClient(charName);
            logPlayer.ClientConnected += delegate() { data.IsConnectedToThalamus = true; };
            logPlayer.ClientDisconnected += delegate() { data.IsConnectedToThalamus = false; };
            logPlayer.MessageSent += delegate(Thalamus.LogEntry message, int messageNumber, int totalMessages)
            {
                data.Progress = (double)messageNumber * 100 / (double)totalMessages;
            };
            ThalamusStatusControl.SetClient(logPlayer);
            data = DataContext as MainWindowViewModel;
            data.LogPath = Properties.Settings.Default.LastLogLoaded;
        }

        private async void cmdLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".log"; // Default file extension
            dlg.Filter = "Thalamus Logs (.log)|*.log"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                data.LogPath = dlg.FileName;
                data.IsLogLoading = true;
                data.IsLogLoaded = false;
                await logPlayer.Load(data.LogPath);
                data.IsLogLoading = false;
                data.IsLogLoaded = true;
            }
        }


        private async void cmdPlay_Click(object sender, RoutedEventArgs e)
        {
            data.IsPlaying = true;
            await logPlayer.Play();
            data.IsPlaying = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (logPlayer != null)
            {
                logPlayer.Dispose();
            }
            Application.Current.Shutdown();
        }

    }
}
