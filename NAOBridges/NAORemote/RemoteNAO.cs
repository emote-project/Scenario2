using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.SshNet;
using System.IO;
using System.Threading.Tasks;

namespace NAORemote
{
    public class NAORemote
    {
        public class ErrorArgs : EventArgs
        {
            public string Message;
            public ErrorArgs (string message) {
                Message = message;
            }
        }
        public event EventHandler<ErrorArgs> Error;

        string host;
        string username;
        string password;
        public string Workingdirectory { get; private set; }
        public SshClient SSH { get; private set; }

        public IAsyncResult asynch { get; private set; }

        public NAORemote(string host, string username, string password, string workingdirectory = "/home/nao")
        {
            this.host = host;
            this.username = username;
            this.password = password;
            this.Workingdirectory = workingdirectory;
        }

        private ConnectionInfo GenerateConnectionInfo()
        {
            var auth1 = new PasswordAuthenticationMethod(username, password);
            var auth2 = new KeyboardInteractiveAuthenticationMethod(username);
            auth2.AuthenticationPrompt += auth2_AuthenticationPrompt;
            ConnectionInfo ci = new ConnectionInfo(host, username, auth1, auth2);
            return ci;
        }

        void auth2_AuthenticationPrompt(object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e)
        {
            foreach (var prompt in e.Prompts)
            {
                if (prompt.Request.Equals("Password: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    prompt.Response = password; // "Password" acquired from resource
                }
            }
        }

        public Task<bool> ConnectAsync()
        {
            return Task.Run<bool>(() => Connect());
        }

        public bool Connect()
        {
            if (SSH == null)
            {
                SSH = new SshClient(GenerateConnectionInfo());
                SSH.ErrorOccurred += ssh_ErrorOccurred;
            }
            if (!SSH.IsConnected)
            {
                try
                {
                    SSH.Connect();
                }
                catch (System.Exception e)//System.Net.Sockets.SocketException e)
                {
                    if (Error!=null) Error(this, new ErrorArgs(e.Message));
                    return false;
                }
            }
            return true;
        }

        public void Disconnect()
        {
            SSH.Disconnect();
        }

        public Task<bool> UploadFileAsync(string localFileName, Action<ulong> uploadCallback)
        {
            return Task.Run<bool>(() => UploadFile(localFileName, uploadCallback));
        }

        public bool UploadFile(string localFileName, Action<ulong> uploadCallback, string remotePath = "")
        {
            string remoteFileName = remotePath+System.IO.Path.GetFileName(localFileName);

            using (var sftp = new SftpClient(GenerateConnectionInfo()))
            {
                sftp.Connect();
                //TODO: check if the directory exists!
                sftp.ChangeDirectory(Workingdirectory);
                sftp.ErrorOccurred += ssh_ErrorOccurred;

                using (var file = File.OpenRead(localFileName))
                {
                    try
                    {
                        sftp.UploadFile(file, remoteFileName, uploadCallback);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }

                sftp.Disconnect();
            }
            return true;
        }

        public Task<SshCommand> ExecuteCommandAsync(string command)
        {
            return Task.Run<SshCommand>(() => ExecuteCommand(command));
        }

        public SshCommand ExecuteCommand(string command)
        {
            string workingDirectoryCommand = "cd " + Workingdirectory + "; ";
            SshCommand sshcommand = SSH.CreateCommand(workingDirectoryCommand + command);
            if (Connect())
            {
                sshcommand.Execute();
                if (sshcommand.Error != "")
                {
                    string error = "An error occurred executing command: " + sshcommand.Error; 
                    if (Error!=null) Error(this, new ErrorArgs(error));
                    Console.WriteLine(error);
                }
                //Disconnect();
            }
            return sshcommand;
        }

        void ssh_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            //throw new Exception("SSH Error", e.Exception);
            Error(this, new ErrorArgs(e.Exception.Message));
        }
        
    }
    

    
}
