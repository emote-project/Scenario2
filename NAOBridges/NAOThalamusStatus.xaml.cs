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
using System.Windows.Shapes;

namespace NAOThalamusGUI
{
    /// <summary>
    /// Interaction logic for NAOThalamusStatus.xaml
    /// </summary>
    public partial class NAOThalamusStatus : Window
    {
        NAOThalamusGUIController controller;
        public NAOThalamusStatus()
        {
            InitializeComponent();
            controller = new NAOThalamusGUIController(shellControl, this.DataContext as NAOStatus);
            controller.Error += controller_Error;
            controller.Start();
        }

        void controller_Error(object sender, NAOThalamusGUIController.NAOThalamusGUIErrorEventArgs e)
        {
            MessageBox.Show(e.Description);
        }

        private void cmdRestartNaoQi_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to restart NaoQi?" + System.Environment.NewLine + "THIS WILL SET OFF MOTORS STIFFNESS." + System.Environment.NewLine + "Be sure to hold the robot to avoid falls", "Restarting NaoQi", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                controller.RestartNaoQiAsync();
            }
        }

        private void cmdStartPython_Click(object sender, RoutedEventArgs e)
        {
            controller.RunPythonAsync();
        }

        private void cmdStopPython_Click(object sender, RoutedEventArgs e)
        {
            controller.StopPythonAsync();
        }

        private void cmdKillPython_Click(object sender, RoutedEventArgs e)
        {
            controller.KillPythonAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controller.StopPythonAsync();
            controller.DisconnectThalamus();
            Application.Current.Shutdown();
        }

        private async void btnInstallPython_Click(object sender, RoutedEventArgs e)
        {
            var res = await controller.InstallPythonAsync();
            System.Windows.Forms.MessageBox.Show("Python "+(!res?"not":"")+" installed correctly");
        }

        private async void btnInstallBehaviours_Click(object sender, RoutedEventArgs e)
        {
            await controller.InstallBehavioursAsync();
        }
    }
}
