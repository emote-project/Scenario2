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

namespace Thalamus.BehaviorNodes
{
    public class Required : BehaviorNode
    {
        public List<BehaviorNodes.Action> Actions;
        private bool StillValid = true;

        public void Failed()
        {
            StillValid = false;
        }

        public bool IsValid()
        {
            if (!StillValid) return false;
            bool valid = true;
            foreach (BehaviorNodes.Action a in Actions) if (!a.IsValid()) valid = false;
            return valid;
        }

        public override string  ToString()
        {
            string str = "Required[";
            foreach (BehaviorNodes.Action a in Actions)
            {
                str += "\t" + a.ToString() + "\n";
            }
            return str+"]\n";
        }
        public override string ToBml()
        {
            string bml = "<required>";
            foreach (Action a in Actions) bml += a.ToBml();
            return bml+"</required>";
        }
    }
}
