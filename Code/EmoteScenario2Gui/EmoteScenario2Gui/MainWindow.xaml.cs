using EmoteScenario2Gui.CustomDialog;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace EmoteScenario2Gui
{
    public class ModuleDescription
    {
        public string ProcessPath { get; set; }
        public string ProcessArgs { get; set; }
    }

    public class MainWindowState : ViewModelBase
    {
        private bool _listContainsItem = false;
        public bool ListContainsItem
        {
            get { return _listContainsItem; }
            set { 
                _listContainsItem = value;
                NotifyPropertyChanged("ListContainsItem");
                NotifyPropertyChanged("CanRunAll");
            }
        }

        private bool _modulesRunning = false;
        public bool ModulesRunning
        {
            get { return _modulesRunning; }
            set {
                _modulesRunning = value;
                NotifyPropertyChanged("ModulesRunning");
                NotifyPropertyChanged("NoModulesRunning");
                NotifyPropertyChanged("CanRunAll");
            }
        }
        public bool NoModulesRunning
        {
            get { return !ModulesRunning; }
        }
        public bool CanRunAll
        {
            get { return ListContainsItem; }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<ThalamusModule> _modules;
        MainWindowState _data;

        public MainWindow()
        {
            InitializeComponent();
            _data = this.DataContext as MainWindowState;
            _modules = new ObservableCollection<ThalamusModule>();
            Load();
            modulesList.DataContext = _modules;

        }

        #region GUI Event Handling

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Are you sure you want to remove the following module?" + Environment.NewLine + 
                                        _modules[modulesList.SelectedIndex].CommandPath, 
                                        "Removing Module", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes) {
                _modules.RemoveAt(modulesList.SelectedIndex);
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            AddModuleDialog addDialog = new AddModuleDialog(_modules);
            var ret = addDialog.ShowDialog();
            if ((bool)ret) _data.ListContainsItem = true;
        }


        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            AddModuleDialog addDialog = new AddModuleDialog(_modules,modulesList.SelectedIndex);
            var ret= addDialog.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item == SaveMenuItem)
            {
                Save();
            }
            else if (item == LoadMenuItem)
            {
                Load();
            }

        }

        private void RunAll_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var m in _modules)
            {
                if (m.Status == ThalamusModule.ModuleStatus.Ended || m.Status == ThalamusModule.ModuleStatus.NotStarted)
                    m.RunAsync();
            }
        }

        private void StopAll_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var m in _modules)
            {
                if (m.Status == ThalamusModule.ModuleStatus.Running)
                    m.EndAsync();
            }
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            ThalamusModule module = (sender as Button).DataContext as ThalamusModule;
            module.RunAsync();
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            ThalamusModule module = (sender as Button).DataContext as ThalamusModule;
            module.EndAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_data.ModulesRunning)
            {
                var result = MessageBox.Show("Do you want to close all the running modules?", "Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else if (result == MessageBoxResult.Yes)
                    foreach (var m in _modules)
                    {
                        if (m.Status == ThalamusModule.ModuleStatus.Running)
                            m.EndAsync();
                    }
            }
            if (DataManager.CheckIfEdited(_modules.ToList()))
            {
                var result = MessageBox.Show("The modules list was edited. Save it?", "Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else if (result == MessageBoxResult.Yes)
                    Save();
            }
        }




        #endregion


        #region Saving/Loading modules list

        void Save()
        {
            DataManager.Save(_modules);
        }

        void Load()
        {
            _modules.Clear();
            foreach (var m in DataManager.Load())
            {
                _modules.Add(m);
                m.StatusChangedEvent += module_StatusChangedEvent;
                m.ErrorEvent += module_ErrorEvent;
            }
            _data.ListContainsItem = _modules.Count > 0;
        }

        #endregion

        void module_ErrorEvent(object sender, ThalamusModule.ThalamusModuleRunningErrorEventArgs e)
        {
            //MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            
        }

        void module_StatusChangedEvent(object sender, EventArgs e)
        {
            if (_data != null)
                _data.ModulesRunning = _modules.Where(m => m.Status == ThalamusModule.ModuleStatus.Running).Count() > 0;
        }

    }
}
