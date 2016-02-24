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
using System.Reflection;
using System.Globalization;
using System.Collections;

namespace Thalamus
{
    public enum PMLEventType
    {
        Action,
        Perception
    }

    public enum PMLParameterType
    {
        Int,
        String,
        Bool,
        Double,
        IntArray,
        StringArray,
        BoolArray,
        DoubleArray,
        Enum,
        Null
    }

    public class PMLParameter
    {
        public PMLParameterType Type;
        int iValue;
        string sValue;
        bool bValue;
        double fValue;
        string enumName;
        int[] a_iValue;
        string[] a_sValue;
        bool[] a_bValue;
        double[] a_fValue;

        private static IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;

        private static PMLParameter trueParameter = new PMLParameter(true);
        private static PMLParameter falseParameter = new PMLParameter(false);

        private static PMLParameter nullParameter = new PMLParameter();
        public static PMLParameter Null
        {
            get { return nullParameter; }
        }

        public static PMLParameter True
        {
            get { return trueParameter; }
        }

        public static PMLParameter False
        {
            get { return falseParameter; }
        }


        public static object DefaultValue(PMLParameterType paramType)
        {
            switch (paramType)
            {
                case PMLParameterType.Int: return 0;
                case PMLParameterType.String: return "";
                case PMLParameterType.Bool: return false;
                case PMLParameterType.Double: return 0;
                case PMLParameterType.IntArray: return new int[0];
                case PMLParameterType.StringArray: return new string[0];
                case PMLParameterType.BoolArray: return new bool[0];
                case PMLParameterType.DoubleArray: return new double[0];
                case PMLParameterType.Enum:
                case PMLParameterType.Null:
                default:
                    return null;
            }
        }

        public PMLParameter()
        {
            Type = PMLParameterType.Null;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        public PMLParameter(int value)
        {
            this.PMLParameterInt(value);
        }
        public PMLParameter(String value)
        {
            this.PMLParameterString(value);
        }
        public PMLParameter(bool value)
        {
            this.PMLParameterBool(value);
        }
        public PMLParameter(double value)
        {
            this.PMLParameterDouble(value);
        }
        public PMLParameter(int[] value)
        {
            this.PMLParameterIntArray(value);
        }
        public PMLParameter(string[] value)
        {
            this.PMLParameterStringArray(value);
        }
        public PMLParameter(bool[] value)
        {
            this.PMLParameterBoolArray(value);
        }
        public PMLParameter(double[] value)
        {
            this.PMLParameterDoubleArray(value);
        }

        private void PMLParameterInt(int value)
        {
            Type = PMLParameterType.Int;
            iValue = value;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterString(String value)
        {
            Type = PMLParameterType.String;
            iValue = 0;
            sValue = value;
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterBool(bool value)
        {
            Type = PMLParameterType.Bool;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = value;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterDouble(double value)
        {
            Type = PMLParameterType.Double;
            iValue = 0;
            sValue = "";
            fValue = value;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterIntArray(int[] value)
        {
            Type = PMLParameterType.IntArray;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = value;
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterStringArray(string[] value)
        {
            Type = PMLParameterType.StringArray;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = value;
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }
        private void PMLParameterBoolArray(bool[] value)
        {
            Type = PMLParameterType.BoolArray;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = value;
            a_fValue = new double[0];
        }
        private void PMLParameterDoubleArray(double[] value)
        {
            Type = PMLParameterType.DoubleArray;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            enumName = "";
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = value;
        }

        public static PMLParameter FromStringType(string stringType) 
        {
            if (stringType.StartsWith("Enum."))
            {
                string enumName = stringType.Substring(stringType.IndexOf('.') + 1);
                enumName = enumName.Substring(enumName.IndexOf('.') + 1);

                if (PML.EnumInfo.ContainsKey(enumName))
                {
                    PMLParameter p = new PMLParameter();
                    p.MakeEnum(stringType);
                    return p;
                }
                else return new PMLParameter(typeof(string), null);
            }
            if (Enum.IsDefined(typeof(PMLParameterType), stringType)) return new PMLParameter(DataTypes.PMLTypeToSystemType((PMLParameterType)Enum.Parse(typeof(PMLParameterType), stringType)), null);
            return new PMLParameter();
        }

        private void MakeEnum(string enumName)
        {
            Type = PMLParameterType.Enum;
            this.enumName = enumName;
            iValue = 0;
            sValue = "";
            fValue = 0f;
            bValue = false;
            a_iValue = new int[0];
            a_sValue = new string[0];
            a_bValue = new bool[0];
            a_fValue = new double[0];
        }

        public PMLParameter(Type paramType, string svalue)
            : this(paramType, svalue == null ? null : DataTypes.PMLObjectToSystemObject(DataTypes.SystemTypeToPMLType(paramType).ToString(), svalue))
        {
        }

        public PMLParameter(Type paramType, object value)
        {
            if (paramType.IsEnum)
            {
                Type = PMLParameterType.Enum;
                enumName = Assembly.GetAssembly(paramType).ToString();
                enumName = enumName.Substring(0, enumName.IndexOf(',')) + "." + paramType.ToString();
                iValue = 0;
                sValue = value == null ? "" : value.ToString();
                fValue = 0f;
                bValue = false;
                a_iValue = new int[0];
                a_sValue = new string[0];
                a_bValue = new bool[0];
                a_fValue = new double[0];
            }
            else
            {
                Type = DataTypes.SystemTypeToPMLType(paramType);
                switch (Type)
                {
                    case PMLParameterType.Int:
                        PMLParameterInt(value == null ? 0 :(int)value);
                        break;
                    case PMLParameterType.String:
                        PMLParameterString(value == null ? "" : (string)value);
                        break;
                    case PMLParameterType.Bool:
                        PMLParameterBool(value == null ? false : (bool)value);
                        break;
                    case PMLParameterType.Double:
                        PMLParameterDouble(value == null ? 0 : (double)value);
                        break;
                    case PMLParameterType.IntArray:
                        PMLParameterIntArray(value == null ? new int[0] : (int[])value);
                        break;
                    case PMLParameterType.StringArray:
                        PMLParameterStringArray(value == null ? new string[0] : (string[])value);
                        break;
                    case PMLParameterType.BoolArray:
                        PMLParameterBoolArray(value == null ? new bool[0] : (bool[])value);
                        break;
                    case PMLParameterType.DoubleArray:
                        PMLParameterDoubleArray(value == null ? new double[0] : (double[])value);
                        break;
                    case PMLParameterType.Null:
                    default:
                        iValue = 0;
                        sValue = value.ToString();
                        fValue = 0f;
                        bValue = false;
                        a_iValue = new int[0];
                        a_sValue = new string[0];
                        a_bValue = new bool[0];
                        a_fValue = new double[0];
                        break;
                }
            }
        }

        public string StringType()
        {
            if (Type == PMLParameterType.Enum) return "Enum." + enumName;
            else return Type.ToString();
        }

        public string GetValue()
        {
            switch (Type)
            {
                case PMLParameterType.Bool: return bValue.ToString(ifp);
                case PMLParameterType.Double: return fValue.ToString(ifp);
                case PMLParameterType.Int: return iValue.ToString(ifp);
                case PMLParameterType.Enum: return sValue;
                case PMLParameterType.BoolArray:
                    {
                        string s = "";
                        if (a_bValue.Length == 0) return s;
                        foreach (bool b in a_bValue) s += b.ToString() + DataTypes.ArrayItemSeparator;
                        return s.Substring(0, s.Length - 1);
                    }
                case PMLParameterType.IntArray:
                    {
                        string s = "";
                        if (a_iValue.Length == 0) return s;
                        foreach (int i in a_iValue) s += i.ToString() + DataTypes.ArrayItemSeparator;
                        return s.Substring(0, s.Length - 1);
                    }
                case PMLParameterType.DoubleArray:
                    {
                        string s = "";
                        if (a_fValue.Length == 0) return s;
                        foreach (double d in a_fValue) s += d.ToString(ifp) + DataTypes.ArrayItemSeparator;
                        return s.Substring(0, s.Length - 1);
                    }
                case PMLParameterType.StringArray:
                    {
                        string s = "";
                        if (a_sValue.Length == 0) return s;
                        foreach (string s2 in a_sValue) s += s2 + DataTypes.ArrayItemSeparator;
                        return s.Substring(0, s.Length - 1);
                    }
                default: return sValue;
            }
        }

        public override string ToString()
        {
            return GetValue();
        }

        public int GetInt()
        {
            return iValue;
        }
        public double GetDouble()
        {
            return fValue;
        }
        public float GetFloat()
        {
            return (float)fValue;
        }
        public bool GetBool()
        {
            return bValue;
        }
        public string GetString()
        {
            return sValue;
        }

        public int[] GetIntArray()
        {
            return a_iValue;
        }
        public double[] GetDoubleArray()
        {
            return a_fValue;
        }
        public float[] GetFloatArray()
        {
            float[] a_float = new float[a_fValue.Length];
            for (int i = 0; i < a_fValue.Length; i++) a_float[i] = (float)a_fValue[i];
            return a_float;
        }
        public bool[] GetBoolArray()
        {
            return a_bValue;
        }
        public string[] GetStringArray()
        {
            return a_sValue;
        }

        public bool IsEnum
        {
            get
            {
                return Type == PMLParameterType.Enum;
            }
        }
        public string EnumName
        {
            get { return enumName; }
        }
        public string ShortEnumName
        {
            get {
                string stringType = enumName.Substring(enumName.IndexOf('.') + 1);
                stringType = stringType.Substring(stringType.IndexOf('.') + 1);
                return stringType; 
            }
        }
    }

    public class PML
    {
        public static PML Null = new PML("NullPML");
        public static Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
        private static Dictionary<string, string[]> enumInfo = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> EnumInfo
        {
            get { return PML.enumInfo; }
        }
        public bool DontLogDescription = false;

        public static void LoadEnum(string enumName, string[] enumValues)
        {
            string stringType = enumName.Substring(enumName.IndexOf('.') + 1);
            string assembly = stringType.Substring(0, stringType.IndexOf('.'));
            stringType = stringType.Substring(stringType.IndexOf('.') + 1);
            enumInfo[stringType] = enumValues;
        }

        public static void AddAssembly(Assembly a) 
        {
            Assemblies[a.ToString().Substring(0, a.ToString().IndexOf(','))] = a;
        }
        public static void Setup()
        {
            List<Assembly> ass = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            foreach (Assembly a in ass)
            {
                AddAssembly(a);
            }
        }

        public bool IsNull { get { return this == Null; } }

        public static int Count = 1;
        public string Id = "PML" + Count++;
        
        private string name = "";
        public string Name
        {
            get { return name; }
        }

        
        public PMLEventType EventType = PMLEventType.Perception;

        public Dictionary<string, PMLParameter> Parameters = new Dictionary<string, PMLParameter>();

        private string[] parameterNames;
        public string[] ParameterNames
        {
            get { return parameterNames; }
        }
        private string[] parameterTypes;
        public string[] ParameterTypes
        {
            get { return parameterTypes; }
        }
        private string[] parameterValues;
        public string[] ParameterValues
        {
            get { return parameterValues; }
        }

        public override string ToString()
        {
            /*if (DontLogDescription)
            {
                return "(" + Name + "):[no description]";
            }
            else
            {*/
                string str = "(" + Name + "):[";
                foreach (KeyValuePair<string, PMLParameter> p in Parameters)
                {
                    str += p.Key + "=" + p.Value.GetValue().Replace("{", "{{").Replace("}", "}}") + ";";
                }
                return str + "]";
            //}
        }

        public string ToStringSimple()
        {
            string str = "";
            foreach (KeyValuePair<string, PMLParameter> p in Parameters)
            {
                str += p.Key + "=" + p.Value.GetValue().Replace("{", "{{").Replace("}", "}}") + ";";
            }
            return str;
        }

        private void RebuildArrays()
        {
            parameterNames = new string[this.Parameters.Count];
            parameterTypes = new string[this.Parameters.Count];
            parameterValues = new string[this.Parameters.Count];
            int i = 0;
            foreach (KeyValuePair<string, PMLParameter> parameter in this.Parameters)
            {
                parameterNames[i] = parameter.Key;
                parameterTypes[i] = parameter.Value.StringType();
                parameterValues[i] = parameter.Value.GetValue();
                i++;
            }
        }

        public PML() : this("PML" + Count++) { }
        public PML(string name)
        {
            this.name = name;
            this.Parameters = new Dictionary<string, PMLParameter>();
            RebuildArrays();
        }
        public PML(string name, string parameterName, PMLParameter parameterValue): this(name)
        {
            Parameters.Add(parameterName, parameterValue);
            RebuildArrays();
        }
        public PML(string name, Dictionary<string, PMLParameter> parameters)
        {
            this.name = name;
            Parameters = parameters;
            RebuildArrays();
        }

        public PML(string name, string[] parameterNames, string[] parameterTypes, string[] parameterValues = null)
        {
            this.name = name;
            this.parameterNames = parameterNames;
            this.parameterTypes = parameterTypes;
            
            if (parameterValues!=null) this.parameterValues = parameterValues;
            else this.parameterValues = new string[this.parameterNames.Length];

            for (int i = 0; i < parameterNames.Length; i++)
            {
                PMLParameter p;
                if (parameterValues != null) p = new PMLParameter(DataTypes.PMLTypeToSystemType(parameterTypes[i]), DataTypes.PMLObjectToSystemObject(parameterTypes[i], parameterValues[i]));
                else
                {
                    p = PMLParameter.FromStringType(parameterTypes[i]);
                    this.parameterValues[i] = "";
                }
                this.Parameters[parameterNames[i]] = p;
            }
        }

        public PML(MethodBase method) : this(method.Name, method) { }
        public PML(MethodBase method, object[] values = null) : this(method.Name, method, values) { }
        public PML(string name, MethodBase method, object[] values = null)
        {
            this.name = name;
            ParameterInfo[] paramInfo = method.GetParameters();
            int numParameters = paramInfo.Length;
            if (values!=null && numParameters != values.Length)
            {
                Environment.Instance.DebugIf("error", string.Format("PML: Wrong number of arguments for method '{0}' specified (got {1}, espected {2})", method.Name, values.Length, numParameters));
                return;
            }
            for (int i = 0; i < numParameters; i++)
            {
                object paramValue = null;
                if (values != null) paramValue = values[i];
                PMLParameter p;
                if (paramValue is String) p = new PMLParameter(paramInfo[i].ParameterType, (string)paramValue);
                else p = new PMLParameter(paramInfo[i].ParameterType, paramValue);
                this.Parameters[paramInfo[i].Name] = p;
            }
            RebuildArrays();
        }

        public void SetParameter(string paramName, Type type, object value)
        {
            Parameters[paramName] = new PMLParameter(type, value);
            RebuildArrays();
        }

        public PML Decode(string p)
        {
            string orig = p;
            try
            {
                this.name = p.Substring(p.IndexOf('(') + 1, p.IndexOf(')') - 1);
                p = p.Substring(p.IndexOf(':') + 1).Trim();
                p = p.Substring(1, p.IndexOf(']') - 1).Trim();
                if (p.Length > 0)
                {
                    p = p.Substring(0, p.Length - 1);
                    string[] sparams = p.Split(';');
                    foreach (string param in sparams)
                    {
                        this.SetParameter(param.Substring(0, param.IndexOf('=')), typeof(string), param.IndexOf('=')+1== param.Length?"":param.Substring(param.IndexOf('=') + 1));
                    }
                }
            }
            catch
            {
                Console.WriteLine("merda");
            }
            return this;
        }
    }

    public class DataTypes
    {
        private static char arrayItemSeparator = '\x1F'; // Record Separator (RS) control character (ASCII 30)
        public static char ArrayItemSeparator
        {
            get { return arrayItemSeparator; }
        }
        private static char arrayDimensionSeparator = '\x1E'; // Unit Separator (US) control character (ASCII 31)
        public static char ArrayDimensionSeparator
        {
            get { return arrayDimensionSeparator; }
        }
        private static IFormatProvider ifp = CultureInfo.InvariantCulture.NumberFormat;
        public static IFormatProvider Ifp
        {
            get { return DataTypes.ifp; }
        }


        

        public static List<string> ExtractEnumMembers(string enumName)
        {
            Type t = PMLTypeToSystemType(enumName);
            if (t != typeof(string)) return new List<string>(Enum.GetNames(t));
            return new List<string>();
        }


        public static PMLParameterType SystemTypeToPMLType(Type type)
        {
            if (type.IsEnum) return PMLParameterType.Enum;
            if (type.IsArray)
            {
                if (type == typeof(int[])) return PMLParameterType.IntArray;
                else if (type == typeof(bool[])) return PMLParameterType.BoolArray;
                else if (type == typeof(double[]) || type == typeof(float)) return PMLParameterType.DoubleArray;
                return PMLParameterType.StringArray;
            }
            else
            {
                if (type == typeof(int)) return PMLParameterType.Int;
                else if (type == typeof(bool)) return PMLParameterType.Bool;
                else if (type == typeof(double) || type == typeof(float)) return PMLParameterType.Double;
                return PMLParameterType.String;
            }
        }

        public static PMLParameter SystemObjectToPMLObject(Type paramType, object paramValue)
        {
            if (paramType.IsEnum) return new PMLParameter(paramType, paramValue);
            if (paramType.IsArray)
            {
                if (paramType.ToString().ToLower().StartsWith("int") || paramType.ToString().ToLower().StartsWith("system.integer"))
                {
                    if (paramValue == null) return new PMLParameter(new int[0]);
                    else return new PMLParameter((int[])paramValue);
                }
                else if (paramType.ToString().ToLower().StartsWith("bool") || paramType.ToString().ToLower().StartsWith("system.boolean"))
                {
                    if (paramValue == null) return new PMLParameter(new bool[0]);
                    else return new PMLParameter((bool[])paramValue);
                }
                else if (paramType.ToString().ToLower().StartsWith("float") || paramType.ToString().ToLower().StartsWith("system.float"))
                {
                    if (paramValue == null) return new PMLParameter(new double[0]);
                    else
                    {
                        int n = ((float[])paramValue).Length;
                        double[] d_paramValue = new double[n];
                        for (int i = 0; i < n; i++) d_paramValue[i] = (double)((float[])paramValue)[i];
                        return new PMLParameter(d_paramValue);
                    }
                }
                else if (paramType.ToString().ToLower().StartsWith("double") || paramType.ToString().ToLower().StartsWith("system.double"))
                {
                    if (paramValue == null) return new PMLParameter(new double[0]);
                    else return new PMLParameter((double[])paramValue);
                }
                else
                {
                    if (paramValue == null) return new PMLParameter(new string[0]);
                    else return new PMLParameter((string[])paramValue);
                }
            }
            else
            {
                if (paramType == typeof(int))
                {
                    if (paramValue == null) return new PMLParameter((int)0);
                    else return new PMLParameter((int)paramValue);
                }
                else if (paramType == typeof(bool))
                {
                    if (paramValue == null) return new PMLParameter(false);
                    else return new PMLParameter((bool)paramValue);
                }
                else if (paramType == typeof(double))
                {
                    if (paramValue == null) return new PMLParameter((double)0.0f);
                    else return new PMLParameter((double)paramValue);
                }
                else if (paramType == typeof(float))
                {
                    if (paramValue == null) return new PMLParameter((float)0.0f);
                    else return new PMLParameter((float)paramValue);
                }
                else
                {
                    if (paramValue == null) return new PMLParameter("");
                    else return new PMLParameter((string)paramValue);
                }
            }
        }

        public static Type PMLTypeToSystemType(string stringType)
        {
            if (stringType.StartsWith("Enum."))
            {
                stringType = stringType.Substring(stringType.IndexOf('.') + 1);
                string assembly = stringType.Substring(0, stringType.IndexOf('.'));
                stringType = stringType.Substring(stringType.IndexOf('.') + 1);

                Type t = null;
                if (PML.Assemblies.ContainsKey(assembly))
                {
                    Type[] types = PML.Assemblies[assembly].GetTypes();
                    t = PML.Assemblies[assembly].GetType(stringType);
                }
                if (t != null) return t;
                else return typeof(string);
            }
            if (Enum.IsDefined(typeof(PMLParameterType), stringType)) return PMLTypeToSystemType((PMLParameterType)Enum.Parse(typeof(PMLParameterType), stringType));
            return typeof(string);
        }

        public static Type PMLTypeToSystemType(PMLParameterType type)
        {
            if (type == PMLParameterType.Int) return typeof(int);
            else if (type == PMLParameterType.Bool) return typeof(bool);
            else if (type == PMLParameterType.Double) return typeof(double);
            if (type == PMLParameterType.IntArray) return typeof(int[]);
            else if (type == PMLParameterType.BoolArray) return typeof(bool[]);
            else if (type == PMLParameterType.DoubleArray) return typeof(double[]);
            else if (type == PMLParameterType.StringArray) return typeof(string[]);
            return typeof(string);
        }


        public static object PMLObjectToSystemObject(PMLParameter eventParam)
        {
            if (eventParam.IsEnum) return PMLEnumToSystemEnum("Enum." + eventParam.EnumName, eventParam.GetValue());
            else return PMLObjectToSystemObject(eventParam.Type.ToString(), eventParam.GetValue());
        }
        public static object PMLObjectToSystemObject(string type, string value)
        {
            try
            {
                if (type.StartsWith("Enum."))
                {
                    return PMLEnumToSystemEnum(type, value);
                }
                else
                {
                    if (type.EndsWith("Array"))
                    {
                        return PMLArrayToSystemArray(type.Substring(0, type.Length - "Array".Length), value);
                    }
                    else if (type.EndsWith("Array2"))
                    {
                        return PMLArray2ToSystemArray2(type.Substring(0, type.Length - "Array".Length), value);
                    }
                    else if (type == PMLParameterType.Bool.ToString())
                    {
                        object[] constructorParam = new object[] { value };
                        return typeof(bool).InvokeMember("Parse", BindingFlags.InvokeMethod, Type.DefaultBinder, "", constructorParam);
                    }
                    else if (type == PMLParameterType.Double.ToString())
                    {
                        object[] constructorParam = new object[] { value, ifp };
                        return typeof(double).InvokeMember("Parse", BindingFlags.InvokeMethod, Type.DefaultBinder, "", constructorParam);
                    }
                    else if (type == PMLParameterType.Int.ToString())
                    {
                        object[] constructorParam = new object[] { value };
                        return typeof(int).InvokeMember("Parse", BindingFlags.InvokeMethod, Type.DefaultBinder, "", constructorParam);
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            catch
            {
                Environment.Instance.DebugError("PML: Unable to convert '{0}' to '{1}'!", value, type);
                return null;
            }
        }

        private static object PMLArrayToSystemArray(string type, string value)
        {
            string[] items = value.Split(arrayItemSeparator);
            object result;


            if (type == PMLParameterType.Bool.ToString())
            {
                Boolean[] bArray = new Boolean[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    bArray[i] = (Boolean)PMLObjectToSystemObject(type, items[i]);
                }
                result = bArray;
            }
            else if (type == PMLParameterType.Double.ToString())
            {
                Double[] bArray = new Double[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    bArray[i] = (Double)PMLObjectToSystemObject(type, items[i]);
                }
                result = bArray;
            }
            else if (type == PMLParameterType.Int.ToString())
            {
                int[] bArray = new int[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    bArray[i] = (int)PMLObjectToSystemObject(type, items[i]);
                }
                result = bArray;
            }
            else
            {
                String[] bArray = new String[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    bArray[i] = (String)PMLObjectToSystemObject(type, items[i]);
                }
                result = bArray;
            }

            return result;
        }

        private static object PMLArray2ToSystemArray2(string type, string value)
        {
            string[] values2 = value.Split(ArrayDimensionSeparator);
            object[][] result = new object[values2.Length][];

            for (int j = 0; j < values2.Length; j++)
            {
                string[] items = value.Split(ArrayItemSeparator);
                result[j] = new object[items.Length];

                for (int i = 0; i < items.Length; i++)
                {
                    result[j][i] = PMLObjectToSystemObject(type, items[i]);
                }
            }
            return result;
        }

        /*
         * Used for bidimensional arrays!
         * Currently not supported.
         * 
        public string SystemObjectToPMLObject(object oValue, Type paramType)
        {
            string value = "";

            if (paramType.IsArray)
            {
                if (paramType.GetArrayRank() > 2)
                {
                    Environment.Instance.DebugIf("error", string.Format("SystemObjectToThalamus: Array of rank {0} for parameter type '{1}' not supported (maximum is 2)", paramType.GetArrayRank(), paramType.ToString()));
                    return "";
                }

                IEnumerable objects = oValue as IEnumerable;

                if (objects != null)
                {
                    if (paramType.GetArrayRank() == 1)
                    {
                        foreach (object objectsItem in objects)
                        {
                            value += SimpleSystemObjectToPMLObject(objectsItem, paramType.ToString()) + DataTypes.ArrayItemSeparator;
                        }
                        value += DataTypes.ArrayDimensionSeparator;
                    }
                    else
                    {
                        foreach (object objectsItem in objects)
                        {
                            IEnumerable objectsSubList = objectsItem as IEnumerable;
                            if (objects != null)
                            {
                                foreach (object objectsSubItem in objectsSubList)
                                {
                                    value += SimpleSystemObjectToPMLObject(objectsItem, paramType.ToString()) + DataTypes.ArrayItemSeparator;
                                }
                            }
                            else
                            {
                                Environment.Instance.DebugIf("error", string.Format("SystemObjectToThalamus: Wrong object type '{0}' for array parameter!", paramType.ToString()));
                                return "";
                            }
                            value += DataTypes.ArrayDimensionSeparator;
                        }
                    }
                }
                else
                {
                    Environment.Instance.DebugIf("error", string.Format("SystemObjectToThalamus: Wrong object type '{0}' for array parameter!", paramType.Name));
                    return "";
                }
            }
            else value = DataTypes.SystemObjectToPMLObject(oValue, paramType.ToString());
            return value;
        }*/

        internal static object PMLEnumToSystemEnum(string enumName, string value)
        {
            return DataTypes.PMLEnumToSystemEnum(PMLTypeToSystemType(enumName), value);
        }

        internal static object PMLEnumToSystemEnum(Type enumType, string value)
        {
            if (enumType != null && enumType.IsEnum)
            {
                if (Enum.IsDefined(enumType, value)) return Enum.Parse(enumType, value);
                else Environment.Instance.DebugIf("error", "Unable to find value '" + value + "' in Enum '" + enumType.Name + "'!");
            }
            else
            {
                if (enumType.Name == "String") return value.ToString();
                Environment.Instance.DebugIf("error", "Unable to find Enum '" + enumType.Name + "'!");
            }
            return value;
        }
    }
}
