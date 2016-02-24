using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ControlPanel.Annotations;
using ControlPanel.Database;
using ControlPanel.Thalamus;
using ControlPanel.viewModels;
using EmoteEvents;
using EmoteEvents.ComplexData;
using System.IO;

namespace ControlPanel.Forms.UserControls
{
    /// <summary>
    /// Interaction logic for ControlPanelS1.xaml
    /// </summary>
    public partial class ControlPanelS1 : UserControl, INotifyPropertyChanged
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

        public ControlPanelS1()
        {
            InitializeComponent();
            var testList = new List<LearnerInfo>()
            {
                new LearnerInfo("", "", "", 0, "", "", 0),
            };
            Student2ComboBox.ItemsSource = testList;
            Student1ComboBox.SelectedIndex = 0;
            Student2ComboBox.SelectedIndex = 0;
            SessionNumberComboBox.Items.Add(1);
            SessionNumberComboBox.Items.Add(2);
            SessionNumberComboBox.Items.Add(3);
            SessionNumberComboBox.Items.Add(4);
            SessionNumberComboBox.Items.Add(5);
            SessionNumberComboBox.Items.Add(6);
            SessionNumberComboBox.SelectedIndex = 0;

            //populating scenario files names from folder path
            String absolutePath = Environment.CurrentDirectory;
            int relativePathStartIndex = absolutePath.IndexOf("ControlPanel");
            String relativePath = absolutePath.Substring(0, relativePathStartIndex);
            String ScenarioFolder = "S1ScenarioXMLFiles";
            String ScenarioPath = string.Concat(relativePath, ScenarioFolder);

            if (!Directory.Exists(ScenarioPath))
            {
                MessageBox.Show("Can't find folder: " + ScenarioPath, "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                string[] ScenarioFiles = System.IO.Directory.GetFiles(ScenarioPath, "*.xml");

                foreach (string file in ScenarioFiles)
                {
                    ScenarioComboBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));

                }

                ScenarioComboBox.SelectedIndex = 0;
            }
            SetupFormControls();
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
                        Student1ComboBox.ItemsSource = args.StudentList;
                    }));
                }
            };
            Database.ConnectedEvent += delegate(object sender, EventArgs args)
            {
                Database.GetAllStudentsAsync();
            };

            
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (Student1ComboBox.SelectedItem != null)
            {
                var learner1Info = ((LearnerInfo)Student1ComboBox.SelectedItem);
                var learner2Info = ((LearnerInfo)Student2ComboBox.SelectedItem);
                int SessID = Convert.ToInt32(SessionNumberComboBox.SelectedValue);
            
                if (learner1Info != null)
                {
                    if (Client != null)
                    {
                        var startInfo = new StartMessageInfo()
                        {
                            Students = new List<LearnerInfo>() { learner1Info, learner2Info },
                            SessionId = SessID,//int.Parse(SessionNumberTextBox.Text), /// NOT SAFE AT ALL!!!
                            ScenarioXmlName = (string) ScenarioComboBox.SelectedItem,
                            Language = (ScenarioLanguages)LanguageComboBox.SelectedItem,
                            IsEmpathic = Convert.ToBoolean(IsEmpathicCheckBox.IsChecked),
                        };
                        Client.LDBPublisher.Start(startInfo.SerializeToJson());
                    }
                    Started = true;
                    CountDownUserControl.Start();
                    ButtonStop.IsEnabled = true;
                    ButtonStart.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Select the student", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Check your input");
            }
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Do you really want to stop the perceptions? This won't stop the map application!", "Warning", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (res == MessageBoxResult.Yes)
            {
                Client.LDBPublisher.Stop();
                CountDownUserControl.Reset();
            }
            ButtonStart.IsEnabled = true;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Client.LDBPublisher.Reset();
        }


        #region Helpers

        private void SetupFormControls()
        {
            
            LanguageComboBox.SelectedItem = Properties.Settings.Default.S2cp_Language;
            IsEmpathicCheckBox.IsChecked = Properties.Settings.Default.S2cp_IsEmpathic;
            Duration.Text = Properties.Settings.Default.S2cp_Duration.ToString();
            CountDownUserControl.Duration = new TimeSpan(0, 0, Properties.Settings.Default.S2cp_Duration, 0);
            IsEndingAutomaticallyCheckBox.IsChecked = Properties.Settings.Default.S2cp_IsAutoEnding;

            ScenarioComboBox.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            SessionNumberComboBox.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            LanguageComboBox.SelectionChanged += delegate(object sender, SelectionChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEmpathicCheckBox.Checked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };
            Duration.TextChanged += delegate(object sender, TextChangedEventArgs args) { CheckAndSaveControlsStatus(); };
            IsEndingAutomaticallyCheckBox.Checked += delegate(object sender, RoutedEventArgs args) { CheckAndSaveControlsStatus(); };

            CountDownUserControl.CountDownEnded += delegate(object sender, EventArgs args)
            {
                if ((bool) IsEndingAutomaticallyCheckBox.IsChecked)
                    StopButton_OnClick(this,null);
            };
        }

        private void CheckAndSaveControlsStatus()
        {
            Properties.Settings.Default.S2cp_Language = (ScenarioLanguages)LanguageComboBox.SelectedItem;
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
                MessageBox.Show("Only numbers are accepted", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Duration.Text = Duration.Text.Substring(0, Duration.Text.Length - 1);
            }
            Properties.Settings.Default.S2cp_IsAutoEnding = (bool) IsEndingAutomaticallyCheckBox.IsChecked;
            Properties.Settings.Default.Save();
        }

        [Obsolete("DELETEME")]
        private async void PopulateControlsAsync()
        {
            _studentList = await Database.GetAllStudentsAsync();
            Student1ComboBox.ItemsSource = _studentList;
        }

        #endregion

        private void Student1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var learner1Info = ((LearnerInfo)Student1ComboBox.SelectedItem);
            if (learner1Info != null)
            {
                int scenarioDifficulty = 0;
                if (learner1Info.scenario1Difficulty != null)
                {
                    scenarioDifficulty = learner1Info.scenario1Difficulty;


                    if (scenarioDifficulty == 1)
                    {
                        ScenarioDifficulty.Text = "The scenario Difficulty for this student should be Easy";
                    }
                    else if (scenarioDifficulty == 2)
                    {
                        ScenarioDifficulty.Text = "The scenario Difficulty for this student should be Medium";
                    }
                    else if (scenarioDifficulty == 3)
                    {
                        ScenarioDifficulty.Text = "The scenario Difficulty for this student should be Difficult";
                    }
                    else
                    {
                        ScenarioDifficulty.Text = "The value in the object in this method is:" + scenarioDifficulty;
                    }
                }
                else
                {
                    ScenarioDifficulty.Text = "No value from LM";
                }
            }
            else
            {
                ScenarioDifficulty.Text = "Null object in this method...";
            }
        }

        
    }
}
