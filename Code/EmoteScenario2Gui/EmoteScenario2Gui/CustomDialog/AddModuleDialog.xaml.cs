using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Shapes;

namespace EmoteScenario2Gui.CustomDialog
{
    /// <summary>
    /// Interaction logic for AddModuleDialog.xaml
    /// </summary>
    public partial class AddModuleDialog : Window
    {
        ObservableCollection<ThalamusModule> _modules;
        int _indexElementToEdit = -1;


        public AddModuleDialog(ObservableCollection<ThalamusModule> modules, int indexElementToEdit)
        {
            InitializeComponent();
            _modules = modules;
            _indexElementToEdit = indexElementToEdit;
            txtPath.Text = modules[indexElementToEdit].CommandPath;
            txtArgs.Text = modules[indexElementToEdit].Args;
            btnAdd.Content = "Edit";
            this.Title = "Edit entry";
        }

        public AddModuleDialog(ObservableCollection<ThalamusModule> modules)
        {
            InitializeComponent();
            _modules = modules;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            ThalamusModule tm = new ThalamusModule() { CommandPath = txtPath.Text, Args = txtArgs.Text };
            if (_indexElementToEdit != -1)
            {
                _modules[_indexElementToEdit] = tm;
            }
            else
            {
                _modules.Add(tm);
            }
            DialogResult = true;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select the command executable";
            dlg.IsFolderPicker = false;
            dlg.InitialDirectory = currentDirectory;
            dlg.Filters.Add(new CommonFileDialogFilter("ThalamusModules", "*.exe"));

            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureValidNames = true;
            dlg.ShowPlacesList = true;
            this.IsEnabled = false;
            if (dlg.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                Uri exeUri = new Uri(currentDirectory);
                Uri fileUri = new Uri(dlg.FileName);
                Uri diff = exeUri.MakeRelativeUri(fileUri);
                txtPath.Text = "../"+diff.ToString();
            }
            this.IsEnabled = true;
            this.Focus();
        }


    }
}
