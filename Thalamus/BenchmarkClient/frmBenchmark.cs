using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Thalamus;

namespace BenchmarkClient
{
    public partial class frmBenchmark : Form
    {

        bool runningBenchmark = false;
        List<BenchmarkClient> benchmarkClients = new List<BenchmarkClient>();

        Thread tbenchmarkWaiter;

        int benchmarkPublishers = 0;
        string characterName = "";


        List<int> benchmarkInboundPerformance = new List<int>();
        List<int> benchmarkOutboundPerformance = new List<int>();

        int benchmarkNumClients, benchmarkNumMessages, benchmarkRate;
        private List<BenchmarkClient> connectedBenchmarkClients = new List<BenchmarkClient>();
        private List<BenchmarkClient> finishedBenchmarkClients = new List<BenchmarkClient>();

        private int benchmarkInitialInboundEvents;
        private int benchmarkInitialOutboundEvents;

        int expectedMessagesPerPublishingClient;
        int expectedMessagesPerNonPublishingClient;

        public frmBenchmark(string character)
        {
            characterName = character;
            InitializeComponent();
            Thalamus.Environment.Instance.PerformanceTimer += new Thalamus.Environment.PerformanceTimerHandler(PerformanceTimerHandler);
            Thalamus.Environment.Instance.Setup();
            Thalamus.Environment.Instance.Start();
        }

        public void PerformanceTimerHandler(int inboundEventsPerSecond, int outboundEventsPerSecond)
        {
            if (runningBenchmark)
            {
                benchmarkInboundPerformance.Add(inboundEventsPerSecond);
                benchmarkOutboundPerformance.Add(outboundEventsPerSecond);
            }
        }

        public void BenchmarkWaiterThread()
        {

            while (finishedBenchmarkClients.Count < benchmarkNumClients)
                Thread.Sleep(30);
            runningBenchmark = false;

            Thalamus.Environment.Instance.Debug("Benchmark Ended.");

            int inboundTotal = Thalamus.Environment.Instance.InboundEventsTotal;
            int outboundTotal = Thalamus.Environment.Instance.OutboundEventsTotal;

            Thalamus.Environment.Instance.Debug("Inbound total: {0}; Outbound totall: {1}", inboundTotal - benchmarkInitialInboundEvents, outboundTotal - benchmarkInitialOutboundEvents);

            btnStartBenchmark.Invoke((MethodInvoker)(() => btnStartBenchmark.Text = "Start"));
            int minDelay = int.MaxValue;
            int maxDelay = int.MinValue;
            int delaySum = 0;
            foreach (int i in benchmarkInboundPerformance)
            {
                delaySum += i;
                if (i > maxDelay)
                    maxDelay = i;
                if (i < minDelay)
                    minDelay = i;
            }
            Thalamus.Environment.Instance.Debug("Inbound message rate (per second) min: {0}; max:{1}; avg:{2}", minDelay, maxDelay, (delaySum * 1.0f) / benchmarkInboundPerformance.Count);
            minDelay = int.MaxValue;
            maxDelay = int.MinValue;
            delaySum = 0;
            foreach (int i in benchmarkOutboundPerformance)
            {
                delaySum += i;
                if (i > maxDelay)
                    maxDelay = i;
                if (i < minDelay)
                    minDelay = i;
            }
            Thalamus.Environment.Instance.Debug("Outbound message rate (per second) min: {0}; max:{0}; avg:{0}", minDelay, maxDelay, (delaySum * 1.0f) / benchmarkOutboundPerformance.Count);


            foreach (BenchmarkClient c in benchmarkClients)
            {
                c.PrintBenchmarkStatistics();
                (new Thread(new ThreadStart((MethodInvoker)(() => c.Dispose())))).Start();
            }
            benchmarkClients.Clear();
        }

        private void ClientConnected(BenchmarkClient cli)
        {
            lock (connectedBenchmarkClients)
            {
                connectedBenchmarkClients.Add(cli);
                Thalamus.Environment.Instance.Debug(connectedBenchmarkClients.Count + "/" + benchmarkNumClients + " clients connected.");
            }
            if (connectedBenchmarkClients.Count == benchmarkNumClients)
            {
                benchmarkInitialInboundEvents = Thalamus.Environment.Instance.InboundEventsTotal;
                benchmarkInitialOutboundEvents = Thalamus.Environment.Instance.OutboundEventsTotal;
                Thalamus.Environment.Instance.Debug("Starting benchmark...");
                foreach (BenchmarkClient c in benchmarkClients)
                {
                    c.StartBenchmark();
                }
                runningBenchmark = true;
            }
        }

        Dictionary<string, Dictionary<int, int>> messages = new Dictionary<string, Dictionary<int, int>>();

        public void RegisterMessage(string cliName, int i, int ticks)
        {
            messages[cliName][i] = ticks;
        }

        private void btnStartBenchmark_Click(object sender, EventArgs e)
        {
            if (runningBenchmark)
            {
                runningBenchmark = false;
                tbenchmarkWaiter.Abort();
                foreach (BenchmarkClient c in benchmarkClients)
                {
                    c.Dispose();
                }

                benchmarkClients.Clear();

                btnStartBenchmark.Text = "Start";
            }
            else
            {
                btnStartBenchmark.Text = "Stop";
                benchmarkNumClients = (int)numBenchmarkClients.Value;
                benchmarkRate = (int)numBenchmarkRate.Value;
                benchmarkNumMessages = (int)numBenchmarkMessages.Value;

                messages = new Dictionary<string, Dictionary<int, int>>();

                benchmarkOutboundPerformance.Clear();
                benchmarkInboundPerformance.Clear();

                connectedBenchmarkClients.Clear();
                finishedBenchmarkClients.Clear();


                for (int i = 0; i < numBenchmarkClients.Value; i++)
                {
                    BenchmarkClient cli = new BenchmarkClient(this, characterName, benchmarkNumMessages, benchmarkRate, (int)numBenchmarkRounds.Value, i < numBenchmarkPublishers.Value);
                    messages[cli.Name] = new Dictionary<int, int>();
                    benchmarkClients.Add(cli);
                    if (cli.Publish)
                        benchmarkPublishers++;
                    cli.ClientConnected += (ThalamusClient.ClientConnectedHandler)(() =>
                    {
                        ClientConnected(cli);
                    });
                    cli.FinishedBenchmark += (BenchmarkClient.FinishedBenchmarkHandler)(() =>
                    {
                        lock (finishedBenchmarkClients)
                        {
                            finishedBenchmarkClients.Add(cli);
                            Thalamus.Environment.Instance.Debug(finishedBenchmarkClients.Count + "/" + benchmarkNumClients + " clients finished.");
                            cli.PrintBenchmarkStatistics();
                        }
                    });
                }

                expectedMessagesPerPublishingClient = ((benchmarkPublishers - 1) * benchmarkNumMessages);
                expectedMessagesPerNonPublishingClient = (benchmarkPublishers * benchmarkNumMessages);

                foreach (BenchmarkClient c in benchmarkClients)
                {
                    if (c.Publish)
                    {
                        c.ExpectedMessageCount = expectedMessagesPerPublishingClient;
                    }
                    else
                    {
                        c.ExpectedMessageCount = expectedMessagesPerNonPublishingClient;
                    }
                    c.Start();
                }

                Thalamus.Environment.Instance.Debug("Launched all clients");

                tbenchmarkWaiter = new Thread(new ThreadStart(BenchmarkWaiterThread));
                tbenchmarkWaiter.Start();
            }
        }

        private void frmBenchmark_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (ThalamusClient c in benchmarkClients) c.Dispose();

        }
    }
}
