using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using EuxUtils;
using ThalamusLogTool;
using Thalamus;

namespace Simulator
{


    public class MainWindowViewModel : ViewModelBase
    {
        string logPath;

        public string LogPath
        {
            get { return logPath; }
            set
            {
                logPath = value;
                NotifyPropertyChanged("LogPath");
            }
        }

        string dllsPath;

        public string DllsPath
        {
            get { return dllsPath; }
            set
            {
                dllsPath = value;
                NotifyPropertyChanged("DllsPath");
            }
        }

        ObservableCollection<string> logEvents = new ObservableCollection<string>();

        public ObservableCollection<string> LogEvents
        {
            get { return logEvents; }
            set
            {
                logEvents = value;
                NotifyPropertyChanged("LogEvents");
            }
        }

        bool connectedToThalamus = false;

        public bool ConnectedToThalamus
        {
            get { return connectedToThalamus; }
            set {
                connectedToThalamus = value;
                NotifyPropertyChanged("ConnectedToThalamus");
            }
        }

    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _data;

        List<LogEntry> _thalamusLog;

        THClient _client;

        public MainWindow()
        {
            InitializeComponent();
            _data = this.DataContext as MainWindowViewModel;

            _data.DllsPath = @"..\..\..\Tests\DLLs for reading the logs\";
            _data.LogPath = @"..\..\..\Tests\LogFolder\s01_Filtered.log";

            Load();
            _client = new THClient();
            _client.ClientConnected += _client_ClientConnected;
            _client.ClientDisconnected += _client_ClientDisconnected;
        }

        void _client_ClientDisconnected()
        {
            _data.ConnectedToThalamus = false;
        }

        void _client_ClientConnected()
        {
            _data.ConnectedToThalamus = true;
        }

        private async void Load()
        {
            _thalamusLog = new List<LogEntry>() ;
            await Task.Run(() => _thalamusLog = LogTool.LoadThalamusLogEntries(_data.LogPath, System.IO.Path.GetFullPath(_data.DllsPath)));
            foreach (var entry in _thalamusLog)
                _data.LogEvents.Add(entry.EventName + "\t" + entry.EventInfo);
        }

        public void EventsListItem_DoubleClickEvent(object sender, RoutedEventArgs e){
            _client.QueuePublishedEvent(_thalamusLog[lstEvents.SelectedIndex].Event);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.Dispose();
        }
    }
}
