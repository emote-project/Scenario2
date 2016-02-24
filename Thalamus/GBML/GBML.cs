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
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace GBML
{
    public class GBML
    {

        public static bml LoadFromFile(string filename)
        {
            if (!filename.EndsWith("test.xml")) return null;

            bml bml = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(bml));

                // A FileStream is needed to read the XML document.
                FileStream fs = new FileStream(filename, FileMode.Open);
                XmlReader reader = new XmlTextReader(fs);


                /*NameTable nt = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                nsmgr.AddNamespace("tha", "http://www.w3.org/2001/XMLSchema-instance");
                XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                XmlReaderSettings xset = new XmlReaderSettings();
                xset.ConformanceLevel = ConformanceLevel.Fragment;
                XmlReader reader = XmlReader.Create(fs, xset, context);*/

                
                // Declare an object variable of the type to be deserialized.
                bml = (bml)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading BML file '" + filename + "': " + e.Message);
            }
            return bml;
        }

        public static bml LoadFromText(string text)
        {
            bml bml = null;
            try
            {
                // Create an instance of the XmlSerializer specifying type.
                XmlSerializer serializer = new XmlSerializer(typeof(bml));

                /*NameTable nt = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                nsmgr.AddNamespace("tha", "http://www.w3.org/2001/XMLSchema-instance");
                XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                XmlReaderSettings xset = new XmlReaderSettings();
                xset.ConformanceLevel = ConformanceLevel.Fragment;
                XmlReader reader = XmlReader.Create(fs, xset, context);*/

                /* Create a TextReader to read the file. Specify an
                   Encoding to use. */
                StringReader reader = new StringReader(text);

                // Use the Deserialize method to restore the object's state.
                bml = (bml)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading BML code: " + e.Message);
            }
            return bml;
        }

        private void WriteToFile(bml bmlObj, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(bml));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            // Add two namespaces with prefixes.
            ns.Add("","http://www.bml-initiative.org/bml/bml-1.0");
            // Create an XmlTextWriter using a FileStream.
            Stream fs = new FileStream(filename, FileMode.Create);
            XmlWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
            // Serialize using the XmlTextWriter.
            serializer.Serialize(writer, bmlObj, ns);
            writer.Close();
        }

        public static string WriteToText(bml bmlObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(bml));
            /* Create a StreamWriter to write with. First create a FileStream
               object, and create the StreamWriter specifying an Encoding to use. */
            StringWriter writer = new StringWriter();
            // Serialize using the XmlTextWriter.
            serializer.Serialize(writer, bmlObj);
            writer.Close();
            return writer.ToString();
        }
    }
}
