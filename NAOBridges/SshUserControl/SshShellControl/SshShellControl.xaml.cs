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
using SshShell;

namespace SshShellControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SshShellControl : UserControl
    {
        MySshStream sshStream;
        public SshShellControl()
        {
            InitializeComponent();
        }

        public void AddOutput(string output)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                txtOutput.AppendText(output);
                txtOutput.ScrollToEnd();
            }));
        }

        public async Task<bool> Connect(string host, string username, string password)
        {
            return await Task.Run<bool>(() =>
            {
                try
                {
                    sshStream = new MySshStream(host, username, password);
                    sshStream.NewOutput += sshStream_NewOutput;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error",ex.Message);
                    return false;
                }
            });
        }

        void sshStream_NewOutput(object sender, MySshStream.NewOutputEventArgs e)
        {
            AddOutput(e.Line);
        }

        public async Task<bool> SendCommandAsync(string command)
        {
            return await sshStream.RunCommandAsync(command);
        }
        
    }
}
