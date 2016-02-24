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
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;

namespace Thalamus
{
    public interface IThalamusServer
    {
        [XmlRpcMethod()]
        string Connect(string name, string clientId, int fastPort, int slowPort, int numSubscribedEvents);

        [XmlRpcMethod()]
        void Disconnect(string clientId);

        [XmlRpcMethod()]
        void PongSync(string clientId, long roundTripTime);

        [XmlRpcMethod()]
        void AnnounceEventInformation(string clientId, string[] eventName, bool[] isAction, string[][] parameters, string[][] types, string[] enumNames, string[][] enums);

        [XmlRpcMethod()]
		void PublishEvent(string messageId, string clientId, string perceptionName, bool dontLogDescription, string[] parameters, string[] types, string[] values);

        [XmlRpcMethod()]
        void PublishEvents(string[] messageIds, string[] clientIds, string[] perceptionNames, bool[] dontLogDescriptions, string[][] allParameters, string[][] allTypes, string[][] allValues);

        [XmlRpcMethod()]
        void PublishSyncEvent(string messageId, string clientId, string perceptionName, string syncEvent, bool dontLogDescription, string[] parameters, string[] types, string[] values);

        [XmlRpcMethod()]
        void RegisterEvents(string clientId, string[] announcedEvents, string[] subscribedEvents);

        [XmlRpcMethod()]
        void ClearSubscriptions(string clientId);
    }
}
