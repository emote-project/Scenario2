/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Threading;
using System.Collections.Generic;
using Thalamus;

namespace BenchmarkClient
{
    public interface IBenchmarkActions : IAction
	{
		void MessageReceived(string clientName, int msgId, int ticks);
	}

	public interface IBenchmarkPublisher : Thalamus.IThalamusPublisher, IBenchmarkActions
	{}

	public class BenchmarkClient : ThalamusClient, IBenchmarkActions
	{
		public delegate void FinishedBenchmarkHandler();
		public event FinishedBenchmarkHandler FinishedBenchmark;

		private void NotifyFinishedBenchmark ()
		{
			if (FinishedBenchmark != null)
				FinishedBenchmark ();
		}

		private class BenchmarkPublisher: IBenchmarkPublisher
		{
			dynamic publisher;

			public BenchmarkPublisher (dynamic publisher)
			{
				this.publisher = publisher;
			}

			public void MessageReceived (string clientName, int msgId, int ticks)
			{
                publisher.MessageReceived(clientName, msgId, ticks);
			}

			public void StartBenchmark ()
			{
				publisher.StartBenchmark ();
			}

        }

		static int BenchMarkCount = 0;
		//Dictionary<string, int[]> gotMessages;
        Dictionary<string, int> gotMessages;


		int iterations = 50;
		int fps = 10;
		Thread tBenchmark;
		bool publish = true;
		int numRounds = 3;

		public bool Publish {
			get {
				return publish;
			}
		}

		bool finished = false;
		public int ExpectedMessageCount = 0;

		List<int> messageDelays = new List<int>();
		BenchmarkPublisher benchmarkPublisher;

		public int ReceivedMessagesCount {
			get { return messageDelays.Count; }
		}

        frmBenchmark form;

        public BenchmarkClient(frmBenchmark form, string characterName, int iterations, int fps, int numRounds, bool publish = true)
            : base("Benchmark" + BenchMarkCount++.ToString(), characterName, false)
		{
            this.form = form;
			this.iterations = iterations;
			this.fps = fps;
			this.publish = publish;
            this.numRounds = numRounds;

			messageDelays = new List<int>();
			finished = false;

			SetPublisher<IBenchmarkPublisher> ();
			benchmarkPublisher = new BenchmarkPublisher (Publisher);
		}

		public override void Dispose ()
		{
			if (tBenchmark!=null) tBenchmark.Abort ();
			base.Dispose ();
		}


		public void RunBenchmark() {

			messageDelays = new List<int>();
			finished = false;

			for(int b=1;b<numRounds+1;b++) {
				int initialTicks = System.Environment.TickCount;
				Debug("Starting burst #{0}.", b);
				for (int i=0; i<iterations; i++) {
                    int t = System.Environment.TickCount;
                    form.RegisterMessage(Name, i, t);
                    benchmarkPublisher.MessageReceived(Name, i, t);
					Thread.Sleep (1000 / fps);
				}
				Debug("Send burst #{0} ended.", b);
				int time = System.Environment.TickCount - initialTicks;
				Debug("Total send time: {0}. Expected send time: {1}", time, iterations*(1000/fps));
                Thread.Sleep(time - iterations * (1000 / fps));
			}

			if (ExpectedMessageCount == 0) {
				Thread.Sleep (1000);
				PrintStatistics ();
				NotifyFinishedBenchmark ();
			}
		}


		#region IBenchmarkEvents implementation
		void IBenchmarkActions.MessageReceived (string clientName, int msgId, int ticks)
		{
            lock (gotMessages)
            {
                string msg = clientName + "#" + msgId.ToString();
                if (!gotMessages.ContainsKey(msg))
                {
                    //gotMessages[clientName] = new int[iterations + 1];
                    gotMessages[msg] = 0;
                }
                bool ok = false;
                int d = 0;
                int i = 0;
                d = System.Environment.TickCount - ticks - i * 5;
                /*while (!ok && i < 5)
                {
                    try
                    {
                        d = System.Environment.TickCount - ticks - i * 5;
                        ok = true;
                    }
                    catch
                    {
                        i++;
                        Thread.Sleep(5);
                    }
                }*/
                //gotMessages[clientName][msgId]++;
                //if (gotMessages[clientName][msgId] > numRounds) Debug("Message {2}#{0} received {1} times.", msgId, gotMessages[clientName][msgId], clientName);

                gotMessages[msg]++;
                if (gotMessages[msg] > numRounds) Debug("Message {0} received {1} times.", msg, gotMessages[msg], clientName);
                messageDelays.Add(d);
            }
            if (!publish && messageDelays.Count%20==0) Debug("Got {0} expecting {1}", messageDelays.Count, ExpectedMessageCount * numRounds);
            if (!finished && messageDelays.Count >= ExpectedMessageCount * numRounds) ReceiveEnded();
		}

        private void ReceiveEnded()
        {
			Debug ("Receiving ended.");
			finished = true;
            Thread.Sleep(1000);
			NotifyFinishedBenchmark ();
        }

        public void PrintBenchmarkStatistics()
        {
            int minDelay = int.MaxValue;
            int maxDelay = int.MinValue;
            int delaySum = 0;
            foreach (int i in messageDelays)
            {
                delaySum += i;
                if (i > maxDelay)
                    maxDelay = i;
                if (i < minDelay)
                    minDelay = i;
            }
            PrintStatistics();
            Debug("Delay min: {0}; max:{1}; avg:{2}", minDelay, maxDelay, (delaySum * 1.0f) / messageDelays.Count);
        }
		#endregion

        #region IBenchmarkActions Members


        public void StartBenchmark()
        {
            gotMessages = new Dictionary<string, int>();
            ResetStatistics();
            if (publish)
            {
                tBenchmark = new Thread(new ThreadStart(RunBenchmark));
                Debug("Starting benchmark!");
                tBenchmark.Start();
            }
        }

        #endregion
    }
}

