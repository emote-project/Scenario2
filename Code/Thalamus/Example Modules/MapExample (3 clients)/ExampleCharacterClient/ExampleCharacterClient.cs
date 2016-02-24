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
using System.Threading;

namespace ExampleClients
{

	//this interface defines what the client subscribes to (both actions and perceptions)
	public interface IExampleCharacterClient :
		ExampleClientsInterface.IGazeActions,
		ExampleClientsInterface.ISpeakActions
	{}

	public interface IExampleCharacterClientPublisher : IThalamusPublisher,
		ExampleClientsInterface.IGazeEvents,
		ExampleClientsInterface.ISpeakEvents
	{}

	public class ExampleCharacterClient : ThalamusClient, IExampleCharacterClient
	{
        private class CharacterPublisher : IExampleCharacterClientPublisher
        {
            // Please refer to the ExampleMapClient for explanation on this class
            dynamic publisher;
            public CharacterPublisher(dynamic publisher)
            {
                this.publisher = publisher;
            }

            #region IGazeEvents Members

            public void GazeStarted(string id)
            {
                publisher.GazeStarted(id);
            }

            public void GazeFinished(string id)
            {
                publisher.GazeFinished(id);
            }

            #endregion

            #region ISpeakEvents Members

            public void SpeakStarted(string id)
            {
                publisher.SpeakStarted(id);
            }

            public void SpeakFinished(string id)
            {
                publisher.SpeakFinished(id);
            }

            #endregion
        }

        CharacterPublisher characterPublisher;


		public ExampleCharacterClient () : base("ExampleCharacter", "Tiago")
		{
			SetPublisher<IExampleCharacterClientPublisher>();
            characterPublisher = new CharacterPublisher(Publisher);
		}

		#region ISpeakActions implementation

		void ExampleClientsInterface.ISpeakActions.Speak (string id, string text)
		{
            characterPublisher.SpeakStarted(id);
			Debug("Speak: " + text);
			Thread.Sleep (2000);
            characterPublisher.SpeakFinished(id);
		}

		#endregion

		#region IGazeActions implementation

		void ExampleClientsInterface.IGazeActions.GazePoint (string id, int x, int y)
		{
            characterPublisher.GazeStarted(id);
            Debug("Gaze to ({0}, {1})", x, y);
			Thread.Sleep (2000);
            characterPublisher.GazeFinished(id);
		}

		#endregion

	}
}

