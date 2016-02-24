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
using System.Dynamic;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Thalamus
{

    public interface IThalamusPublisher { }
	public class ThalamusPublisher : DynamicObject, IThalamusPublisher
	{
		private ThalamusClient thalamusClient = null;

		private Type interfaceType = typeof(object);
		public Type Interface {
			get {
				return interfaceType;
			}
		}

		private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo> ();
		private Dictionary<string, bool> isAction = new Dictionary<string, bool>();

		public ThalamusPublisher (Type interfaceType, ThalamusClient client)
		{
			this.interfaceType = interfaceType;
			this.thalamusClient = client;
			thalamusClient.Debug("Creating publisher from type '" + interfaceType.Name + "'");
		}

		internal void AddMethod(MethodInfo method) 
		{
			string eventName = method.DeclaringType.Name + "." + method.Name;
			methods [method.Name] = method;
			isAction[method.Name] = ThalamusClient.ImplementsTypeDirectly(method.DeclaringType, typeof(IAction));
			if (isAction [method.Name]) thalamusClient.Debug("Registered action '" + eventName + "' in publisher.");
			else thalamusClient.Debug("Registered perception '" + eventName + "' in publisher.");
		}

		public override bool TryInvokeMember(
			InvokeMemberBinder binder, object[] args, out object result)
		{
			result = null;
			try
			{
				if (methods.ContainsKey(binder.Name)) {
					MethodInfo method = methods[binder.Name];
					string eventName = method.DeclaringType.Name + "." + binder.Name;
                    if (binder.CallInfo.ArgumentCount != args.Length)
                    {
                        thalamusClient.DebugError("Attempt to publish '" + eventName + "' with incorrect number of parameters (expected " + binder.CallInfo.ArgumentCount + ", got " + args.Length + ")!");
                        return false;
                    }
                    /*else
                    {
                        thalamusClient.Debug("Publish '" + eventName + " (expected " + binder.CallInfo.ArgumentCount + ", got " + args.Length + ")");
                        foreach (object o in args) Console.WriteLine(o.ToString());
                    }*/

					/* this isn't working on Mono!
					List<string> parameterNames = new List<string>();
					foreach(ParameterInfo p in method.GetParameters()) {
						parameterNames.Add(p.Name);
						if (!binder.CallInfo.ArgumentNames.Contains(p.Name)) {
							Console.WriteLine("Attemp to publish '" + eventName + "' without specifying parameter '" + p.Name + "'!");
							return false;
						}
					}

					foreach(string s in binder.CallInfo.ArgumentNames) {
						if (!parameterNames.Contains(s)) {
							Console.WriteLine("Attemp to publish '" + eventName + "' without invalid parameter '" + s + "'!");
							return false;
						}
					}*/



					PML pml = new PML(eventName, method, args);
                    if (isAction[binder.Name]) pml.EventType = PMLEventType.Action;
                    if (method.GetCustomAttributes(typeof(DontLogDetailsAttribute), true).Length > 0) pml.DontLogDescription = true;
                    thalamusClient.QueuePublishedEvent(pml);
					return true;
				}
				else
				{
					thalamusClient.DebugError("Method '" + binder.Name + "' not found in type '" + interfaceType.Name + "'!");
					return false;
				}
			}
			catch
			{
				result = null;
				return false;
			}
		}

        /*#region IThalamusCoreActions Members

        public void BML(string code)
        {
            DataTypes.PML pml = new DataTypes.PML("BML", new string[]{"code"}, new string[]{EventParameterType.String.ToString()}, new string[]{code});
            ThalamusEvent ev = pml.ToThalamusEvent();
            ev.EventType = ThalamusEventType.Action;
            thalamusClient.QueuePublishedEvent(ev);
        }

        #endregion*/
    }
}

