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
using Thalamus;

namespace ThalamusClientControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ThalamusClientControl : UserControl
    {
        private ThalamusClient _client;
        public ThalamusClient Client { 
            get {
                return _client;
            } 
            set { 
                _client = value; 
                RegisterEvents(); 
            } 
        }

        public ThalamusClientControl()
        {
            InitializeComponent();
        }

        private void RegisterEvents(){
            if (Client!=null){
                Client.ClientConnected += Client_ClientConnected;
                Client.ClientDisconnectedFromThalamus += Client_ClientDisconnectedFromThalamus;
                Client.EventLogged += Client_EventLogged;
            }
        }

        void Client_EventLogged(LogEntry logEntry)
        {
            Console.WriteLine("Thalamus Event: " + logEntry.ToString());
        }

        void Client_ClientDisconnectedFromThalamus(string name, string oldClientId)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtStatus.Text = "Disconnected";
            }));
        }

        void Client_ClientConnected()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                txtStatus.Text = "Connected";
            }));
            
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Client.Dispose();
        }

        private void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Client.IsConnected)
                Client.Dispose();
            Client.Start();
        }

    }
}
