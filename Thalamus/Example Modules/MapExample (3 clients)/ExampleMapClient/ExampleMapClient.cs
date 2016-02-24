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
using Thalamus;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace ExampleClients
{
	public interface IExampleMapClient : 
		ExampleClientsInterface.IMapActions, 
		ExampleClientsInterface.IScreenActions,
		ExampleClientsInterface.ISpeakEvents, 
		ExampleClientsInterface.ISpeakActions
	{}

	public interface IExampleMapClientPublisher : IThalamusPublisher,
		ExampleClientsInterface.IMapEvents
	{}


	public class ExampleMapClient : ThalamusClient, IExampleMapClient
	{

		private class MapClientPublisher: IExampleMapClientPublisher 
		{
			/*
			 * 	This class serves as a wrapper to the dynamic publisher, 
				because the dynamic publisher will not know the interface definitions and therefore does not provide auto-complete.
				By wrapping the publisher in this class we can use this class instead of using the publisher directly.
				*/
			#region Generic code (repeat in all publisher wrappers)
			// Save the dynamic publisher
			dynamic publisher;
			public MapClientPublisher(dynamic publisher) {
				this.publisher = publisher;
			}
			#endregion

			#region IMapEvents implementation
			//This makes it easier, cleaner and safer to implement the interfaces without running into errors
			public void Click (int x, int y)
			{
				publisher.Click(x, y);
			}
			public void Zoom (double zoomFactor)
			{
				publisher.Zoom (zoomFactor);
			}
			public void CreatedWayPoint (string id, int x, int y)
			{
				publisher.CreatedWayPoint (id, x, y);
			}
			#endregion
		}

		Dictionary<String, String> subtitles = new Dictionary<string, string>();
		MapClientPublisher mapPublisher;
		Random r = new Random ();

		Thread emulateUserThread;

		public ExampleMapClient () : base("ExampleMap", "Tiago")
		{
			//never forget to first set the publisher, even if we are going to use a publisher wrapper
			SetPublisher<IExampleMapClientPublisher> ();

			//create an instance of the publisher wrapper, and pass the dynamic publisher to the constructor
			mapPublisher = new MapClientPublisher (Publisher);

			//launch a thread that will randomly generate user events
			emulateUserThread=new Thread (new ThreadStart (EmulateUser));
		}

		public override void ConnectedToMaster()
		{
            if (emulateUserThread.ThreadState != ThreadState.Running)
            {
                emulateUserThread = new Thread(new ThreadStart(EmulateUser));
                emulateUserThread.Start();
            }
		}

		public override void Dispose ()
		{
			emulateUserThread.Abort ();
		}

		public void EmulateUser() {
		
			int wayPoints = 1;
			//this method acts as a user, by randomply triggering Click, Zoom and CreateWaypoint events.
			while (Running) {
				if (r.NextDouble () < 0.2) {
					mapPublisher.Zoom (r.NextDouble ());
				} else if (r.NextDouble () < 0.8) {
					mapPublisher.Click (r.Next (0, 1920), r.Next (0, 1080));
				} else {
					mapPublisher.CreatedWayPoint ("Waypoint" + wayPoints++.ToString(), r.Next (0, 1920), r.Next (0, 1080));
				}
				//mapPublisher.CreatedWayPoint ("Waypoint" + wayPoints++.ToString(), r.Next (0, 1920), r.Next (0, 1080));
				Thread.Sleep (r.Next (1000, 2000));
			}
		}


		//The map client also listens to speak events so that it may print out subtitles whenever speak actions are executed
		#region ISpeakActions and ISpeakEvents implementation

		void ExampleClientsInterface.ISpeakActions.Speak (string id, string text) // this is an action
		{
			lock (subtitles) {
				subtitles [id] = text;
			}
		}

		void ExampleClientsInterface.ISpeakEvents.SpeakStarted (string id) // this is a perception
		{
			lock (subtitles) {
				if (subtitles.ContainsKey (id)) {
					Debug("Subtitles ON: '" + subtitles [id] + "'");
				}
			}
		}

		void ExampleClientsInterface.ISpeakEvents.SpeakFinished (string id) // this is a perception
		{
			lock (subtitles) {
				if (subtitles.ContainsKey (id)) 
				{
					Debug("Subtitles OFF: '" + subtitles [id] + "'");
					subtitles.Remove (id);
				}
			}
		}

		#endregion


		//the map actions are called by the mind and perform something that the user can see on the map
		#region IMapActions implementation
		void ExampleClientsInterface.IMapActions.Zoom (double zoomFactor)
		{
			Debug("Call map action 'Zoom({0})'", zoomFactor);
			Thread.Sleep (2000);
			mapPublisher.Zoom (zoomFactor);
		}
		void ExampleClientsInterface.IMapActions.CreateWaypoint (string id, int x, int y)
		{
			Debug("Call map action 'CreateWaypoint({0}, {1}, {2})'", id, x, y);
			Thread.Sleep (1000);
			mapPublisher.CreatedWayPoint (id, x, y);
		}
		#endregion


		//the screen actions can be performed by any client that controls a screen (independently of it having a map or not)
		#region IScreenActions implementation
		void ExampleClientsInterface.IScreenActions.Highlight (int x, int y)
		{
			Debug("Call map action 'Highlight({0}, {1})'", x, y);
		}
		#endregion

	}
}

