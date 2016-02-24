using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAOIpDiscoverer;
using Zeroconf;
using System.Threading.Tasks;
using System.Threading;

namespace NAOIPDiscovererTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Discoverer discoverer = new Discoverer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Updates();
        }

        private async Task FindNAOs()
        {
            lblFeedback.Content = "Looking for NAOs...";
            await discoverer.DiscoverNAO();
            lblFeedback.Content = "Found " + discoverer.NAOs.Count + " NAOs";
            lstServices.ItemsSource = discoverer.NAOs;
            lstServices.Items.Refresh();
        }

        private async void Updates()
        {
            await FindNAOs();
            await Task.Delay(10000);
            Updates();
        }

    }
}
