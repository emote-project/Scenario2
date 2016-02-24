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

namespace Thalamus.Actions
{
    public enum SyncPointType
    {
        Absolute,
        Reference,
        Unspecified,
        Internal
    }
    public struct SyncPoint
    {

        private static SyncPoint spNull = new SyncPoint(null);
        private static SyncPoint spInternal = new SyncPoint(SyncPointType.Internal);

        public static SyncPoint Null { get { return spNull; } }
        public static SyncPoint Internal { get { return spInternal; } }

        public SyncPointType Type;
        public string ReferenceValue;
        public float AbsoluteValue;
        public float Offset;
        public SyncPoint(SyncPointType type)
        {
            Type = type;
            AbsoluteValue = 0;
            Offset = 0;
            ReferenceValue = "";
        }
        public SyncPoint(float value)
        {
            Type = SyncPointType.Absolute;
            AbsoluteValue = value;
            Offset = 0;
            ReferenceValue="";
        }
        public SyncPoint(string value)
        {
			if (value==null) {
				Type=SyncPointType.Unspecified;
				AbsoluteValue=0;
				Offset=0;
				ReferenceValue="";
			}else{
	            Type = SyncPointType.Reference;
	            AbsoluteValue = 0;
	            Offset = 0;
	            ReferenceValue = value;
	            string[] s = ReferenceValue.Split('+');
				float f;
	            if (s.Length > 1)
	            {
	                if (float.TryParse(s[1], out f))
	                {
	                    Offset = f;
	                }
	                ReferenceValue = s[0];
	            }else{
					if (float.TryParse(s[0], out f))
	                {
	                    AbsoluteValue = f;
						Type = SyncPointType.Absolute;
	                }
				}
			}
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SyncPoint t1, SyncPoint t2)
        {
            if (!(t1.Type == t2.Type) || !(t1.Offset==t2.Offset)) return false;
            switch (t1.Type)
            {
                case SyncPointType.Absolute:
                    return t1.AbsoluteValue==t2.AbsoluteValue;
                case SyncPointType.Reference:
                    return t1.ReferenceValue==t2.ReferenceValue;
                default:
                    return true;
            }
        }

        public static bool operator !=(SyncPoint t1, SyncPoint t2)
        {
            if (!(t1.Type == t2.Type) || !(t1.Offset == t2.Offset)) return true;
            switch (t1.Type)
            {
                case SyncPointType.Absolute:
                    return t1.AbsoluteValue != t2.AbsoluteValue;
                case SyncPointType.Reference:
                    return t1.ReferenceValue != t2.ReferenceValue;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            if (Type==SyncPointType.Absolute) return AbsoluteValue + "+" + Offset + "<" + Type + ">";
            else if (Type == SyncPointType.Reference) return ReferenceValue + "(+" + Offset + ")" + "<" + Type + ">";
            return "UnspecifiedSyncPoint+" + Offset;
        }

        public string ToBml()
        {
            if (Type == SyncPointType.Reference) return ReferenceValue + "+" + Offset.ToString();
            else return AbsoluteValue.ToString();
        }
    }
}
