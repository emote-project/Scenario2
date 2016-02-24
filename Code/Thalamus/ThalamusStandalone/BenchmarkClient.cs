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

namespace Thalamus
{
	public interface IBenchmarkEvents : Thalamus.IPerception
	{
		void MessageReceived(int msgId, int ticks);
		void StartBenchmark();
	}

	public interface IBenchmarkPublisher : Thalamus.IThalamusPublisher, IBenchmarkEvents
	{}

	public class BenchmarkClient : ThalamusClient, IBenchmarkEvents
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

			public void MessageReceived (int msgId, int ticks)
			{
				publisher.MessageReceived (msgId, ticks);
			}

			public void StartBenchmark ()
			{
				publisher.StartBenchmark ();
			}

        }

		static int BenchMarkCount = 0;
		int[] gotMessages;

		public int[] GotMessages {
			get {
				return gotMessages;
			}
		}

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

		public BenchmarkClient (int iterations, int fps, int numRounds, bool publish=true) : base("Benchmark" + BenchMarkCount++.ToString(), false)
		{
			this.iterations = iterations;
			this.fps = fps;
			this.publish = publish;
            this.numRounds = numRounds;

			messageDelays = new List<int>();
			gotMessages = new int[iterations+1];
			finished = false;

			SetPublisher<IBenchmarkPublisher> ();
			benchmarkPublisher = new BenchmarkPublisher (Publisher);
		}

		public override void Dispose ()
		{
			if (tBenchmark!=null) tBenchmark.Abort ();
			base.Dispose ();
		}

		#region IBenchmarkEvents implementation

		void IBenchmarkEvents.StartBenchmark ()
		{
			if (publish) {
				tBenchmark = new Thread (new ThreadStart (RunBenchmark));
				Debug ("Starting benchmark!");
				ResetStatistics ();
				tBenchmark.Start ();
			}
		}

		#endregion

		public void RunBenchmark() {

			messageDelays = new List<int>();
			gotMessages = new int[iterations+1];
			finished = false;

			for(int b=1;b<numRounds+1;b++) {
				int initialTicks = System.Environment.TickCount;
				Debug("Starting burst #{0}.", b);
				for (int i=0; i<iterations; i++) {
					benchmarkPublisher.MessageReceived (i+1, System.Environment.TickCount);
					Thread.Sleep (1000 / fps);
				}
				Debug("Send burst #{0} ended.", b);
				int time = System.Environment.TickCount - initialTicks;
				Debug("Total time: {0}. Expected time: {1}", time, iterations*(1000/fps));
				Thread.Sleep (time);
			}

			if (ExpectedMessageCount == 0) {
				Thread.Sleep (1000);
				PrintStatistics ();
				NotifyFinishedBenchmark ();
			}
		}


		#region IBenchmarkEvents implementation
		void IBenchmarkEvents.MessageReceived (int msgId, int ticks)
		{
			if (!finished) {
				gotMessages [msgId]++;
				messageDelays.Add (System.Environment.TickCount - ticks);
				if (messageDelays.Count >= ExpectedMessageCount*numRounds) {
					Debug ("Receiving ended.");
					finished = true;
					int minDelay = int.MaxValue;
					int maxDelay = int.MinValue;
					int delaySum = 0;
					foreach (int i in messageDelays) {
						delaySum += i;
						if (i > maxDelay)
							maxDelay = i;
						if (i < minDelay)
							minDelay = i;
					}
					Thread.Sleep (1000);
					PrintStatistics ();
					Debug ("Delay min: {0}; max:{1}; avg:{2}", minDelay, maxDelay, (delaySum * 1.0f) / messageDelays.Count);
					for (int i=1; i<=iterations; i++) {
						if (gotMessages [i] != (ExpectedMessageCount / iterations)*numRounds)
							Debug ("Message #{0} received {1} times.", i, gotMessages [i]);
					}
					NotifyFinishedBenchmark ();
				}
			}
		}
		#endregion
	}
}

