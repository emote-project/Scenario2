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
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public class XMLViewer : System.Windows.Forms.RichTextBox
    {
        private XMLViewerSettings settings;
        /// <summary>
        /// The format settings.
        /// </summary>
        public XMLViewerSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new XMLViewerSettings
                    {
                        AttributeKey = Color.Red,
                        AttributeValue = Color.Blue,
                        Tag = Color.Blue,
                        Element = Color.DarkRed,
                        Value = Color.Black,
                    };
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        /// <summary>
        /// Convert the Xml to Rtf with some formats specified in the XMLViewerSettings,
        /// and then set the Rtf property to the value.
        /// </summary>
        /// <param name="includeDeclaration">
        /// Specify whether include the declaration.
        /// </param>
        public void Process(bool includeDeclaration)
        {
            try
            {
                // The Rtf contains 2 parts, header and content. The colortbl is a part of
                // the header, and the {1} will be replaced with the content.
                string rtfFormat = @"{{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052
{{\fonttbl{{\f0\fnil Courier New;}}}}
{{\colortbl ;{0}}}
\viewkind4\uc1\pard\lang1033\f0 
{1}}}";

                // Get the XDocument from the Text property.
                var xmlDoc = XDocument.Parse(this.Text, LoadOptions.None);

                StringBuilder xmlRtfContent = new StringBuilder();

                // If includeDeclaration is true and the XDocument has declaration,
                // then add the declaration to the content.
                if (includeDeclaration && xmlDoc.Declaration != null)
                {

                    // The constants in XMLViewerSettings are used to specify the order 
                    // in colortbl of the Rtf.
                    xmlRtfContent.AppendFormat(@"
\cf{0} <?\cf{1} xml \cf{2} version\cf{0} =\cf0 ""\cf{3} {4}\cf0 "" 
\cf{2} encoding\cf{0} =\cf0 ""\cf{3} {5}\cf0 ""\cf{0} ?>\par",
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.ElementID,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.AttributeValueID,
                        xmlDoc.Declaration.Version,
                        xmlDoc.Declaration.Encoding);
                }

                // Get the Rtf of the root element.
                string rootRtfContent = ProcessElement(xmlDoc.Root, 0);

                xmlRtfContent.Append(rootRtfContent);

                // Construct the completed Rtf, and set the Rtf property to this value.
                this.Rtf = string.Format(rtfFormat, Settings.ToRtfFormatString(),
                    xmlRtfContent.ToString());


            }
            catch (System.Xml.XmlException xmlException)
            {
                throw new ApplicationException(
                    "Please check the input Xml. Error:" + xmlException.Message,
                    xmlException);
            }
            catch
            {
                throw;
            }
        }

        // Get the Rtf of the xml element.
        private string ProcessElement(XElement element, int level)
        {

            // This viewer does not support the Xml file that has Namespace.
            if (!string.IsNullOrEmpty(element.Name.Namespace.NamespaceName))
            {
                throw new ApplicationException(
                    "This viewer does not support the Xml file that has Namespace.");
            }

            string elementRtfFormat = string.Empty;
            StringBuilder childElementsRtfContent = new StringBuilder();
            StringBuilder attributesRtfContent = new StringBuilder();

            // Construct the indent.
            string indent = new string(' ', 4 * level);

            // If the element has child elements or value, then add the element to the 
            // Rtf. {{0}} will be replaced with the attributes and {{1}} will be replaced
            // with the child elements or value.
            if (element.HasElements || !string.IsNullOrWhiteSpace(element.Value))
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >\par
{{1}}
{0}\cf{1} </\cf{2} {3}\cf{1} >\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);

                // Construct the Rtf of child elements.
                if (element.HasElements)
                {
                    foreach (var childElement in element.Elements())
                    {
                        string childElementRtfContent =
                            ProcessElement(childElement, level + 1);
                        childElementsRtfContent.Append(childElementRtfContent);
                    }
                }

                // If !string.IsNullOrWhiteSpace(element.Value), then construct the Rtf 
                // of the value.
                else
                {
                    childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}\par",
                        new string(' ', 4 * (level + 1)),
                        XMLViewerSettings.ValueID,
                        CharacterEncoder.Encode(element.Value.Trim()));
                }
            }

            // This element only has attributes. {{0}} will be replaced with the attributes.
            else
            {
                elementRtfFormat =
                    string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} />\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);
            }

            // Construct the Rtf of the attributes.
            if (element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    string attributeRtfContent = string.Format(
                        @" \cf{0} {3}\cf{1} =\cf0 ""\cf{2} {4}\cf0 """,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.AttributeValueID,
                        attribute.Name,
                       CharacterEncoder.Encode(attribute.Value));
                    attributesRtfContent.Append(attributeRtfContent);
                }
                attributesRtfContent.Append(" ");
            }

            return string.Format(elementRtfFormat, attributesRtfContent,
                childElementsRtfContent);
        }

    }
}
