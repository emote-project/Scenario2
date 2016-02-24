using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using ControlPanel.Database;
using ControlPanel.Thalamus;
using ControlPanel.Thalamus.UserControl;
using ControlPanel.viewModels;

namespace ControlPanel.Forms
{
    /// <summary>
    /// Interaction logic for EmoteControlPanel.xaml
    /// </summary>
    public partial class EmoteControlPanel : Window
    {
        private ControlPanelThalamusClient _client;
        private IStudentsDatabase _db;

        private DatabaseWindow _databaseWindow;

        public EmoteControlPanel()
        {
            InitializeComponent();
            StudentDatabaseMenuItem.IsEnabled = false;
            _client = ControlPanelThalamusClient.GetInstance();
            _client.ClientConnected += delegate
            {
                _db = new ThalamusStudentDatabase();
                _db.ConnectedEvent += DbOnConnectedEvent;
                _db.ConnectingEvent += DbOnConnectingEvent;
                _db.TimeoutEvent += DbOnTimeoutEvent;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    StudentDatabaseMenuItem.IsEnabled = true;
                    DatabaseStatus.Text = "Connecting...";
                    ControlPanelS2.IsEnabled = true;
                    ControlPanelS2.Init(_client,_db);

                    ControlPanelS1.IsEnabled = true;
                    ControlPanelS1.Init(_client, _db);
                    
                }));
            };
            ThalamusStatus.WatchedClient = _client;
            MainTabPanel.SelectedIndex = Properties.Settings.Default.SelectedTab;

#if !DEBUG
            ControlPanelS2.IsEnabled = false;
#endif
        }

        private void DbOnConnectingEvent(object sender, EventArgs eventArgs)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                DatabaseStatus.Text = "Connecting...";
                DatabaseStatus.Foreground = new SolidColorBrush(Colors.Peru);
            }));
        }

        private void DbOnTimeoutEvent(object sender, EventArgs eventArgs)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                DatabaseStatus.Text = "Timeout";
                DatabaseStatus.Foreground = new SolidColorBrush(Colors.Red);
            }));
        }

        private void DbOnConnectedEvent(object sender, EventArgs eventArgs)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                DatabaseStatus.Text = "Connected";
                DatabaseStatus.Foreground = new SolidColorBrush(Colors.Green);
            }));
        }

        private void EmoteControlPanel_OnClosing(object sender, CancelEventArgs e)
        {
            if (_databaseWindow!=null)
                _databaseWindow.Close();
            _client.Dispose();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_databaseWindow==null || !_databaseWindow.IsLoaded) _databaseWindow = new DatabaseWindow(_client,_db);
            _databaseWindow.Show();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.SelectedTab = MainTabPanel.SelectedIndex;
            Properties.Settings.Default.Save();
        }




    }
}
