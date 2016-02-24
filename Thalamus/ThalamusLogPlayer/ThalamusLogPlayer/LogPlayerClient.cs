using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThalamusLogTool;

namespace ThalamusLogPlayer
{
    class LogPlayerClient : Thalamus.ThalamusClient
    {
        public event EventHandler Loaded;
        public delegate void MessageSentEventHandler(Thalamus.LogEntry message, int messageNumber, int totalMessages);
        public event MessageSentEventHandler MessageSent;

        List<Thalamus.LogEntry> log;
        DateTime startTime;
        string logPath;
        CancellationTokenSource cancellationSource;

        public LogPlayerClient(string characterName = "") : base("LogPlayer",characterName)
        {
            //Load();
        }

        //private async void Load(string path)
        //{
        //    //string path = @"C:\Users\Eugenio\Downloads\testBig.log";
        //    log = await Load(path);
        //}

        public async Task<bool> Play()
        {
            startTime = DateTime.Now;
            int messagesSent =0;
            int totalMessages = log.Count;
            while (log.Count > 0)
            {
                double timeFromStart = DateTime.Now.Subtract(startTime).TotalMilliseconds / 1000;
                Console.WriteLine("Tick: " + timeFromStart);
                List<Thalamus.LogEntry> toPlay = new List<Thalamus.LogEntry>();
                int i=0;
                while (log.Count>i && log[i].Time < timeFromStart)
                {
                    toPlay.Add(log[i]);
                    i++;
                }
                foreach (Thalamus.LogEntry entry in toPlay)
                {
                    QueuePublishedEvent(entry.Event);
                    if (MessageSent!=null) MessageSent(entry,messagesSent++,totalMessages);
                }
                log.RemoveRange(0, i);
                await Task.Delay(250);
            }
            return true;
        }

        public async Task<List<Thalamus.LogEntry>> Load(string path)
        {
            logPath = path;
            cancellationSource = new CancellationTokenSource();
            log = await Task.Run<List<Thalamus.LogEntry>>(() =>
            {
                return ThalamusLogTool.LogTool.LoadThalamusLogEntries(path);
            },cancellationSource.Token);
            if (Loaded != null)
            {
                Loaded(this, null);
                Properties.Settings.Default.LastLogLoaded = logPath;
                Properties.Settings.Default.Save();
            }
            return log;
        }

        public override void Dispose()
        {
            if (cancellationSource != null)
                cancellationSource.Cancel();
            base.Dispose();
        }

    }
}
