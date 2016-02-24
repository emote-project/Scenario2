using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ControlPanel.Database;
using ControlPanel.viewModels;
using EmoteEvents;
using Microsoft.WindowsAPICodePack.Dialogs;
using Thalamus;

namespace ControlPanel.Forms
{
    /// <summary>
    /// Interaction logic for DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : Window
    {
        private StudentsDBViewModel _data;
        private DatabaseWindowController _controller;

        public DatabaseWindow(ThalamusClient client, IStudentsDatabase db)
        {
            InitializeComponent();

            _controller = new DatabaseWindowController(db);

            // this snippet helps customizing the DatePicker format. (See -> http://www.codeproject.com/Questions/346685/How-to-Display-DatePicker-in-different-format)
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name); 
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            
            IsEnabled = false;

            if (db.IsConnected())
            {
                LoadDatabaseData();
                DatabaseStatus.Text = "Connected";
                DatabaseStatus.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                DatabaseStatus.Text = "Waiting for connection...";
            }

            db.ConnectedEvent += delegate(object sender, EventArgs args)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DatabaseStatus.Text = "Connected";
                    DatabaseStatus.Foreground = new SolidColorBrush(Colors.Green);
                }));
                LoadDatabaseData();
            };
            db.ConnectedEvent += delegate(object sender, EventArgs args)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DatabaseStatus.Text = "Timeout";
                    DatabaseStatus.Foreground = new SolidColorBrush(Colors.Red);
                }));
            };
            db.ConnectingEvent += delegate(object sender, EventArgs args)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DatabaseStatus.Text = "Connecting...";
                    DatabaseStatus.Foreground = new SolidColorBrush(Colors.Peru);
                }));
            };
        }

        private async void LoadDatabaseData()
        {
            var studentList = await _controller.GetStudentListFromDatabaseAsync();
            if (studentList != null)
                DataContext = new StudentsDBViewModel() {Learners = new ObservableCollection<LearnerInfo>(studentList)};
            else
                DataContext = new StudentsDBViewModel() {Learners = new ObservableCollection<LearnerInfo>()};
            _data = (StudentsDBViewModel)this.DataContext;
            IsEnabled = true;
        }

        #region EVENT HANDLING

        private async void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = false;
            if (txtFirstName.Text != "" && txtLastName.Text != "" && DatePickerBirth.Text != "" &&
                cmbSex.SelectedItem != null && cmbSex.SelectedItem.ToString() != "")
            {
                string sex = cmbSex.SelectedIndex==0?"M":"F";
                if (DatePickerBirth.SelectedDate != null)
                {
                    var learnerInfo = new LearnerInfo(txtFirstName.Text, txtMiddleName.Text, txtLastName.Text, 0, sex,
                        DatePickerBirth.SelectedDate.Value.ToShortDateString(), 0);
                    if (await _controller.AddLearnerToLearnerModel(learnerInfo))
                    {
                        _data.Learners.Add(learnerInfo);
                    }
                    else
                    {
                        MessageBox.Show("Can't add the new Learner to the database", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a birth date", "Missing birth date", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("All fields required", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            AddButton.IsEnabled = true;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this field?","Removing field", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                _data.Learners.RemoveAt(MainGrid.SelectedIndex);
        }

        private void ImportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Load data from CSV";
            dlg.Filters.Add(new CommonFileDialogFilter("CSV Files", "*.csv"));
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var csvPath = dlg.FileName;
                try
                {
                    var learnerList = _controller.ImportCSV(csvPath);
                    if (learnerList != null)
                    {
                        PopulateStudentsList(learnerList);
                    }
                    else
                    {
                        MessageBox.Show("Couldn't import the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                catch (System.IO.IOException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            var studentsList = await _controller.GetStudentListFromDatabaseAsync();
            PopulateStudentsList(studentsList);
        }

        #endregion


        #region helpers

        private void PopulateStudentsList(IList<LearnerInfo> students)
        {
            _data.Learners.Clear();
            foreach (var learnerInfo in students)
            {
                _data.Learners.Add(learnerInfo);
            }
        }

        #endregion

    }
}
