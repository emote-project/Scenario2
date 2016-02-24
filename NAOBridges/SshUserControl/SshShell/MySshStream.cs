using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Threading;

namespace SshShell
{
    public class MySshStream
    {
        public const int COMMAND_TIMEOUT = 5000;
        public class NewOutputEventArgs : EventArgs
        {
            public string Line { get; set; }
        }
        public event EventHandler<NewOutputEventArgs> NewOutput;

        MemoryStream memoryStream = new MemoryStream(new byte[1000000]);
        TextWriter outputWriter;
        TextReader shellOutpu;
        
        ShellStream stream;
        StreamReader reader;
        StreamWriter writer;

        public TextReader ShellOutpu
        {
            get { return shellOutpu; }
            set { shellOutpu = value; }
        }

        public MySshStream(string host, string username, string password)
        {
            SshClient ssh = new SshClient(GenerateConnectionInfo(host, username, password));
            ssh.Connect();
            if (!ssh.IsConnected)
                throw new Exception("Can't connect to host: " + ssh.ConnectionInfo.Host);

            var x = new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            //x.Add(Renci.SshNet.Common.TerminalModes.VERASE, 0);
            stream = ssh.CreateShellStream("dumb", 80, 24, 800, 600, 1024,x);
            stream.DataReceived += stream_DataReceived;
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            outputWriter = new StreamWriter(memoryStream);
            shellOutpu = new StreamReader(memoryStream);
        }

        private ConnectionInfo GenerateConnectionInfo(string host, string username, string password)
        {
            var auth1 = new PasswordAuthenticationMethod(username, password);
            var auth2 = new KeyboardInteractiveAuthenticationMethod(username);
            auth2.AuthenticationPrompt += delegate(object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e)
            {
                foreach (var prompt in e.Prompts)
                {
                    if (prompt.Request.Equals("Password: ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        prompt.Response = password;
                    }
                }
            };
            ConnectionInfo ci = new ConnectionInfo(host, username, auth1, auth2);
            return ci;
        }

        public Task<bool> RunCommandAsync(string cmd)
        {
            return Task<bool>.Run(() => RunCommand(cmd));
        }
        public bool RunCommand(string cmd)
        {
            writer.WriteLine(cmd);
            int timeout = 0;
            while (stream.Length == 0)
            {
                Thread.Sleep(500);
                timeout += 500;
                if (timeout > COMMAND_TIMEOUT)
                    //   throw new Exception("Command timeout (" + cmd + ")");
                    return false;
            }
            return true;
        }

        void stream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            var line = reader.ReadToEnd();
            while (line != null && !line.Equals(""))
            {
                bool x = line.Contains('\n');
                ToOutput(line);
                line = reader.ReadToEnd();
            }
        }

        void ToOutput(string line)
        {
            //line = line.Replace("[0m", Environment.NewLine);
            outputWriter.WriteLine(line);
            outputWriter.Flush();
            Console.WriteLine(line);
            if (NewOutput != null) NewOutput(this, new NewOutputEventArgs() { Line = line });
        }
    }
}
