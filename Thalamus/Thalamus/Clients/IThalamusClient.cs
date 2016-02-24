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
    public interface IThalamusClient
    {
		[XmlRpcMethod]
		void Connected(string uid, int port);

        [XmlRpcMethod]
        void NewClientConnected(string name, string newClientUid);

        [XmlRpcMethod]
        void ClientDisconnected(string name, string uid);

        [XmlRpcMethod]
        void PerformRegistration();

        [XmlRpcMethod]
        void QueueEvent(string messageId, string perceptionName, string[] parameters, string[] types, string[] values, bool dontLogDescription);
        [XmlRpcMethod]
        void QueueEvents(string[] messageIds, string[] perceptionNames, string[][] allParameters, string[][] allTypes, string[][] allValues, bool[] dontLogDescriptions);

        [XmlRpcMethod]
        void ReceiveEventInfo(string[] perceptionName, string[][] parameters, string[][] types);
        
        [XmlRpcMethod]
        void PingSync(long centralTime, long timeZone);

        [XmlRpcMethod]
        void Shutdown();

        [XmlRpcMethod]
        string GetExecutionCommand();

        [XmlRpcMethod]
        string GetExecutionParameters();
    }
}
