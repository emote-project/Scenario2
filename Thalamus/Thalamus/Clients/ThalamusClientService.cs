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
    public class ThalamusClientService : XmlRpcListenerService, Thalamus.IThalamusClient
    {
        protected ThalamusClient client;
        protected string clientAddress="";
        public ThalamusClientService(ThalamusClient client, string clientAddress = "")
        {
            this.client = client;
            this.clientAddress = clientAddress;
        }

        #region IThalamusClient Members

        public void PingSync(long centralTime, long timeZone)
        {
            try
            {
                client.PingSync(centralTime, timeZone);
            }
            catch
            {
				Environment.Instance.DebugIf("error", "Failed to Ping!");
                client.Disconnect();
            }
        }

        public void Shutdown()
        {
            client.Shutdown();
        }

		public void Connected(string uid, int port)
		{
			client.QueueEvent(Guid.NewGuid().ToString(), "IThalamusClient.Connected", new string[]{"uid"}, new string[]{PMLParameterType.String.ToString()}, new string[]{uid});
		}


        public void NewClientConnected(string name, string newClientUid)
        {
            client.QueueEvent(Guid.NewGuid().ToString(), "IThalamusClient.NewClientConnected", new string[] { "name", "newClientUid" }, new string[] { PMLParameterType.String.ToString(), PMLParameterType.String.ToString() }, new string[] { name, newClientUid });
        }

        public void ClientDisconnected(string name, string oldClientUid)
        {
            client.QueueEvent(Guid.NewGuid().ToString(), "IThalamusClient.ClientDisconnected", new string[] { "name", "oldClientUid" }, new string[] { PMLParameterType.String.ToString(), PMLParameterType.String.ToString() }, new string[] { name, oldClientUid });
        }

        public void QueueEvent(string messageId, string perceptionName, string[] parameters, string[] types, string[] values, bool dontLogDescription)
        {
            client.QueueEvent(messageId, perceptionName, parameters, types, values);
        }

        public void QueueEvents(string[] messageIds, string[] eventNames, string[][] allParameters, string[][] allTypes, string[][] allValues, bool[] dontLogDescriptions)
        {
            client.QueueEvents(messageIds, eventNames, allParameters, allTypes, allValues, dontLogDescriptions);
        }

        public void ReceiveEventInfo(string[] perceptionName, string[][] parameters, string[][] types)
        {
            client.ReceiveEventInfo(perceptionName, parameters, types);
        }

        public void PerformRegistration()
        {
            client.UpdateEvents=true;
        }

        public string GetExecutionCommand()
        {
            return client.GetExecutionCommand();
        }

        public string GetExecutionParameters()
        {
            return client.GetExecutionParameters();
        }

        #endregion
    }
}
