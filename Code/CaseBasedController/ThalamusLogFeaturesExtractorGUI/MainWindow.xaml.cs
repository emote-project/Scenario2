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
using ThalamusLogFeautresExtractor;
using Microsoft.WindowsAPICodePack.Dialogs;
using EuxUtils;
using CaseBasedController;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Enercities;
using ThalamusLogFeaturesExtractor;

namespace ThalamusLogFeaturesExtractorGUI
{
    public class MainWindowViewModel : ViewModelBase
    {
        string _casePoolPath;
        public string CasePoolPath {
            get
            {
                return _casePoolPath;
            }
            set
            {
                _casePoolPath = value;
                NotifyPropertyChanged("CasePoolPath");
            }
        }
        string _logFolderPath;
        public string LogFolderPath
        {
            get
            {
                return _logFolderPath;
            }
            set
            {
                _logFolderPath = value;
                NotifyPropertyChanged("LogFolderPath");
            }
        }
        string _thalamusDLLs;
        public string ThalamusDLLs
        {
            get
            {
                return _thalamusDLLs;
            }
            set
            {
                _thalamusDLLs = value;
                NotifyPropertyChanged("ThalamusDLLs");
            }
        }
        bool _isSimulating;
        public bool IsSimulating
        {
            get
            {
                return _isSimulating;
            }
            set
            {
                _isSimulating = value;
                NotifyPropertyChanged("IsSimulating");
                NotifyPropertyChanged("IsNotSimulating");
            }
        }
        public bool IsNotSimulating
        {
            get
            {
                return !_isSimulating;
            }
            set
            {
                IsSimulating = !value;
            }
        }
        double _simulationProgress;
        public double SimulationProgress
        {
            get { return _simulationProgress; }
            set { 
                _simulationProgress = value;
                NotifyPropertyChanged("SimulationProgress");
            }
        }
    }

    public partial class MainWindow : Window
    {
        MainWindowViewModel _data;
        SimulationController _sc;

        System.Threading.CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
            _data = this.DataContext as MainWindowViewModel;
            _data.CasePoolPath = "../../../Tests/MLPool.json";
            _data.LogFolderPath = "../../../Tests/LogFolder";
            _data.ThalamusDLLs = "../../../Tests/DLLs for reading the logs";

            _cts = new System.Threading.CancellationTokenSource();
        }


        #region Controls Events Handling

        private void LoadCasePoolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();          
 
            dlg.DefaultExt = ".json";
            dlg.Filter = "Case Pool File (.json)|*.json";
 
            Nullable<bool> result = dlg.ShowDialog();
 
            if (result == true)
            {
                _data.CasePoolPath = dlg.FileName;
            }
        }

        private void LoadLogFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Load Log Folder";
            dlg.IsFolderPicker = true;

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
                _data.LogFolderPath = dlg.FileName;
            }
        }
        private void LoadThalamusDLLsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Load Thalamus DLLs Folder";
            dlg.IsFolderPicker = true;

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
                _data.ThalamusDLLs = dlg.FileName;
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private async void RunSimulationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(false);
        }


        private async void RunAndAugmentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await RunSimulationAsync(true);
        }


        private void CancelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CancelSimulation();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CancelSimulation();
            if (_sc!=null)
                _sc.Dispose();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            //Environment.Exit(0);
        }

        private void MergeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "This tool will merge all the ARFF files in a folder.\n" +
                            "If there is already a merged file, it will be merged as well.\n" +
                            "You may want to delete or move it before continuing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select the folder containing the ARFFs to merge";
            dlg.IsFolderPicker = true;

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
                try
                {
                    ARFFUtils.MergeArffs(dlg.FileName, Log);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void CleanUnwantedBehavioursMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string behavioursToIgnoreFilePath = "";

            var dlg = new CommonOpenFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("Behaviour list file", "*.txt"));
            dlg.Title = "Behaviours to clear list";
            dlg.IsFolderPicker = false;

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
                behavioursToIgnoreFilePath = dlg.FileName;
            }
            else
            {
                return;
            }
            dlg.Dispose();
            dlg = new CommonOpenFileDialog();
            dlg.Title = "ARFF to clean";
            dlg.Filters.Add(new CommonFileDialogFilter("ARFF file","*.arff") );
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    ARFFUtils.CleanARFF(dlg.FileName, behavioursToIgnoreFilePath, Log);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void CleanSubcategoriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "ARFF to clean";
            dlg.Filters.Add(new CommonFileDialogFilter("ARFF file", "*.arff"));
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    ARFFUtils.CleanARFF_subcategories(dlg.FileName, Log);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void ComputeStatisticsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.Title = "ARFF to clean";
            dlg.IsFolderPicker = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    ARFFUtils.ComputeStatistics(dlg.FileName,Log);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        #endregion

        #region Complex Operations

        private void CancelSimulation()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            
        }

        private async Task<bool> RunSimulationAsync(bool augment)
        {
            return await Task.Run<bool>(() =>
            {
                return RunSimulation(augment);
            });
        }
        private bool RunSimulation(bool augment)
        {
            try
            {
                Log("Simulation started\n\n");
                _data.IsSimulating = true;
                _sc = new SimulationController(_data.CasePoolPath, _data.LogFolderPath, _data.ThalamusDLLs, Log, delegate(double val) { _data.SimulationProgress = val; });
                if (augment)
                    _sc.SimulateAndAugment(_cts.Token);
                else
                    _sc.Simulate(_cts.Token);
                _data.IsSimulating = false;
                return true;
            }
            catch (OperationCanceledException)
            {
                Log("\n\nCanceled");
                _cts.Dispose();
                _data.IsSimulating = false;
                _sc.Dispose();
                _cts = new System.Threading.CancellationTokenSource();
                return false;
            }
        }

        private void Log(string message)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    txtLog.AppendText(message + Environment.NewLine);
                    txtLog.ScrollToEnd();
                }));
            }
            catch (TaskCanceledException) { }
        }



        #endregion





        

        
    }
}
