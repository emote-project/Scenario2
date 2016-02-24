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
    public class ThalamusServerService : XmlRpcListenerService, IThalamusServer
    {
        private ClientsInterface clients;
        private string clientAddress;
        public ThalamusServerService(ClientsInterface bodyInterface, string clientAddress)
        {
            this.clients = bodyInterface;
            this.clientAddress = clientAddress;
        }

        public string Connect(string name, string clientId, int fastPort, int slowPort, int numSubscribedEvents)
        {
            return clients.ConnectClient(name, clientId, clientAddress, fastPort, slowPort, numSubscribedEvents);
        }

        public void Disconnect(string clientId)
        {
            if (clients.RemoteClients.ContainsKey(clientId))
            {
                clients.RemoteClients[clientId].Disconnect();
            }
        }

        public void ClearSubscriptions(string clientId)
        {
            if (clients.RemoteClients.ContainsKey(clientId)) clients.RemoteClients[clientId].ClearSubscriptions();
        }


        public void PongSync(string clientId, long roundTripTime)
        {
            if (clients.RemoteClients.ContainsKey(clientId))
            {
                clients.RemoteClients[clientId].Pong(roundTripTime);
            }
        }


        public void AnnounceEventInformation(string clientId, string[] eventNames, bool[] isAction, string[][] parameters, string[][] types, string[] enumNames, string[][] enums)
        {
            clients.AnnounceEventInformation(clientId, eventNames, isAction, parameters, types, enumNames, enums);
        }

        public void PublishEvent(string messageId, string clientId, string perceptionName, bool dontLogDescription, string[] parameters, string[] types, string[] values)
        {
			clients.PublishEvent(messageId, clientId, perceptionName, dontLogDescription, parameters, types, values);
        }

        public void PublishEvents(string[] messageIds, string[] clientIds, string[] perceptionNames, bool[] dontLogDescriptions, string[][] allParameters, string[][] allTypes, string[][] allValues)
        {
            clients.PublishEvents(messageIds, clientIds, perceptionNames, dontLogDescriptions, allParameters, allTypes, allValues);
        }

        public void PublishSyncEvent(string messageId, string clientId, string perceptionName, string syncEvent, bool dontLogDescription, string[] parameters, string[] types, string[] values)
        {
            clients.PublishEvent(messageId, clientId, perceptionName, dontLogDescription, parameters, types, values, syncEvent);
        }

        public void RegisterEvents(string clientId, string[] announcedEvents, string[] subscribedEvents)
        {
            clients.RegisterEvents(clientId, announcedEvents, subscribedEvents);
        }
    }
}
