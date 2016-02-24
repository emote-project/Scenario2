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

namespace ExampleClients
{
	public interface IExampleMindClient : 
		ExampleClientsInterface.IMapEvents
	{}

	public interface IExampleMindClientPublisher : IThalamusPublisher,
		ExampleClientsInterface.IGazeActions,
		ExampleClientsInterface.ISpeakActions,
		ExampleClientsInterface.IScreenActions,
		ExampleClientsInterface.IMapActions
	{}

	public class ExampleMindClient : ThalamusClient, IExampleMindClient
	{
		private class MindPublisher: IExampleMindClientPublisher 
		{
			// Please refer to the ExampleMapClient for explanation on this class
			dynamic publisher;
			public MindPublisher(dynamic publisher) {
				this.publisher = publisher;
			}

			#region IMapActions implementation

			public void Zoom (double zoomFactor)
			{
				publisher.Zoom (zoomFactor);
			}

			public void CreateWaypoint (string id, int x, int y)
			{
				publisher.CreateWaypoint (id, x, y);
			}

			#endregion

			#region IScreenActions implementation

			public void Highlight (int x, int y)
			{
				publisher.Highlight (x, y);
			}

			#endregion

			#region ISpeakActions implementation

			public void Speak (string id, string text)
			{
				publisher.Speak (id, text);
			}

			#endregion

			#region IGazeActions implementation

			public void GazePoint (string id, int x, int y)
			{
				publisher.GazePoint (id, x, y);
			}

			#endregion
		}

		MindPublisher mindPublisher;

		public ExampleMindClient () : base("ExampleMind", "Tiago")
		{
			SetPublisher<IExampleMindClientPublisher> ();
			mindPublisher = new MindPublisher (Publisher);
		}

		//the map events are called whenever the user interacts with the map
		#region IMapEvents implementation
		void ExampleClientsInterface.IMapEvents.Click (int x, int y)
		{
			Debug("User clicked ({0}, {1})", x, y);
			mindPublisher.GazePoint ("", x, y);
		}
		void ExampleClientsInterface.IMapEvents.Zoom (double zoomFactor)
		{
			Debug("User zoomed {0}x", zoomFactor);
			mindPublisher.Speak ("", "Don't zoom too much!");
		}
		void ExampleClientsInterface.IMapEvents.CreatedWayPoint (string id, int x, int y)
		{
            Debug("User created waypoint '{0}' at ({0}, {1})", id, x, y);
			mindPublisher.GazePoint ("", x, y);
			mindPublisher.Speak ("", "That waypoint " + id + " looks really nice.");
			mindPublisher.Highlight (x, y);
		}
		#endregion

	}
}

