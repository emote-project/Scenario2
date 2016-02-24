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

namespace ExampleClientsInterface
{
	public interface ISpeakActions : Thalamus.IAction
	{
		void Speak(string id, string text);
	}

	public interface ISpeakEvents : Thalamus.IPerception
	{
		void SpeakStarted(string id);
		void SpeakFinished(string id);
	}

	public interface IGazeActions : Thalamus.IAction
	{
		void GazePoint(string id, int x, int y);
	}

	public interface IGazeEvents : Thalamus.IPerception
	{
		void GazeStarted(string id);
		void GazeFinished(string id);
	}

	public interface IMapActions : Thalamus.IAction
	{
		void Zoom(double zoomFactor);
		void CreateWaypoint(string id, int x, int y);
	}

	public interface IMapEvents : Thalamus.IPerception
	{
		void Click(int x, int y);
		void Zoom(double zoomFactor);
		void CreatedWayPoint(string id, int x, int y);
	}

	public interface IScreenActions : Thalamus.IAction
	{
		void Highlight(int x, int y);
	}
	
}

