using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlPanel.Annotations;
using ControlPanel.Database;
using ControlPanel.Thalamus;
using ControlPanel.viewModels;
using EmoteEvents;
using EmoteEvents.ComplexData;

namespace ControlPanel.Forms.UserControls
{
    /// <summary>
    /// Interaction logic for ControlPanelS2.xaml
    /// </summary>
    public partial class ControlPanelS2 : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private ControlPanelThalamusClient Client { get; set; }
        private IStudentsDatabase Database { get; set; }

        private bool _started = false;
        public bool Started
        {
            get { return _started; }
            set
            {
                _started = value;
                OnPropertyChanged("Started");
            }
        }


        private List<LearnerInfo> _studentList;
        private Window _window;

        public ControlPanelS2()
        {
            InitializeComponent();
            var testList = new List<LearnerInfo>()
            {
                new LearnerInfo("Player1", "", "", 0, "M", "26/03/2015", 0),
                new LearnerInfo("Player2", "", "", 1, "F", "26/03/2015", 1)
            };
            Student1ComboBox.ItemsSource = Student2ComboBox.ItemsSource = testList;
            Student1ComboBox.SelectedIndex = 0;
            Student2ComboBox.SelectedIndex = 1;

            SetupFormControls();

            _window = Application.Current.MainWindow;
        }

        public void Init(ControlPanelThalamusClient client, IStudentsDatabase database)
        {
            Client = client;
            Database = database;
            database.StudentListUpdatedEvent += delegate(object sender, StudentListEventArgs args)
            {
                if (args.StudentList != null)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        Student1ComboBox.ItemsSource = Student2ComboBox.ItemsSource = args.StudentList;
                    }));
                }
            };
            Database.ConnectedEvent += delegate(object sender, EventArgs args)
            {
                Database.GetAllStudentsAsync();
            };

            client.EnercitiesGameEnded += client_EnercitiesGameEnded;
        }

        void client_EnercitiesGameEnded(object sender, EventArgs e)
        {
            CountDownUserControl.Reset();
            this.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(_window, "Game ended!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }));
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            int n;
            if (int.TryParse(SessionNumber.Text, out n) && Student1ComboBox.SelectedItem != null &&
                Student2ComboBox.SelectedItem != null)
            {
                var learner1Info = ((LearnerInfo)Student1ComboBox.SelectedItem);
                var learner2Info = ((LearnerInfo)Student2ComboBox.SelectedItem);
                if (learner1Info != null && learner2Info != null)
                {
                    if (Client != null)
                    {
                        var startInfo = new StartMessageInfo()
                        {
                            Students = new List<LearnerInfo>() {learner1Info, learner2Info},
                            SessionId = int.Parse(SessionNumber.Text), /// NOT SAFE AT ALL!!!
                            ScenarioXmlName = "",
                            Language = (ScenarioLanguages)LanguageComboBox.SelectedItem,
                            IsEmpathic = (bool) IsEmpathicCheckBox.IsChecked
                        };
                        Client.LDBPublisher.Start(startInfo.SerializeToJson());
                    }
                    Started = true;
                    CountDownUserControl.Start();
                }
                else
                {
                    MessageBox.Show(_window,"Select both students", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(_window,"Check your input");
            }
        }

        private void EndGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(_window, "Do you really want to end the game?", "Warning", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (res == MessageBoxResult.Yes)
            {
                Client.LDBPublisher.Stop();
                Client.LDBPublisher.EndGameTimeout();
                CountDownUserControl.Reset();
            }
        }

        #region Helpers

        private void SetupFormControls()
        {
            LanguageComboBox.SelectedItem = Properties.Settings.Default.S2cp_Language;
            SessionNumber.SelectedIndex = Properties.Settings.Default.S2cp_Session;
            IsEmpathicCheckBox.IsChecked = Properties.Settings.Default.S2cp_IsEmpathic;
            Duration.Text = Properties.Settings.Default.S2cp_Duration.ToString();
            CountDownUserControl.Duration = new TimeSpan(0, 0, Properties.Settings.Default.S2cp_Duration, 0);
            CountDownUserControl.CountDownEnded += CountDownUserControlOnCountDownEnded;
            IsEndingAutomaticallyCheckBox.IsChecked = Properties.Settings.Default.S2cp_IsAutoEnding;

            LanguageComboBox.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEmpathicCheckBox.Checked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEmpathicCheckBox.Unchecked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };
            Duration.TextChanged += delegate(object sender, TextChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEndingAutomaticallyCheckBox.Checked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEndingAutomaticallyCheckBox.Unchecked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };
            SessionNumber.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args) { CheckAndSaveControlsStatus(); };
        }

        private void CountDownUserControlOnCountDownEnded(object sender, EventArgs eventArgs)
        {
            if (IsEndingAutomaticallyCheckBox.IsChecked != null && (bool)IsEndingAutomaticallyCheckBox.IsChecked)
            {
                Client.LDBPublisher.EndGameTimeout();
                MessageBox.Show(_window, "Time is up! Game ended", "Time up", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void CheckAndSaveControlsStatus()
        {
            Properties.Settings.Default.S2cp_Language = (ScenarioLanguages)LanguageComboBox.SelectedItem;
            Properties.Settings.Default.S2cp_Session = SessionNumber.SelectedIndex;
            Properties.Settings.Default.S2cp_IsEmpathic = (bool)IsEmpathicCheckBox.IsChecked;
            try
            {
                string txt = Duration.Text == "" ? "0" : Duration.Text;
                int minutes = int.Parse(txt);
                Properties.Settings.Default.S2cp_Duration = minutes; // DANGEROUS
                CountDownUserControl.Duration = new TimeSpan(0,0,minutes,0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(_window, "Only numbers are accepted", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Duration.Text = Duration.Text.Substring(0, Duration.Text.Length - 1);
            }
            Properties.Settings.Default.S2cp_IsAutoEnding = (bool) IsEndingAutomaticallyCheckBox.IsChecked;
            Properties.Settings.Default.Save();
        }

        #endregion

    }
}
