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
using System.Drawing;
using System.Text;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public class XMLViewerSettings
    {
        public const int ElementID = 1;
        public const int ValueID = 2;
        public const int AttributeKeyID = 3;
        public const int AttributeValueID = 4;
        public const int TagID = 5;

        /// <summary>
        /// The color of an Xml element name.
        /// </summary>
        public Color Element { get; set; }

        /// <summary>
        /// The color of an Xml element value.
        /// </summary>
        public Color Value { get; set; }

        /// <summary>
        /// The color of an Attribute Key in Xml element.
        /// </summary>
        public Color AttributeKey { get; set; }

        /// <summary>
        /// The color of an Attribute Value in Xml element.
        /// </summary>
        public Color AttributeValue { get; set; }

        /// <summary>
        /// The color of the tags and operators like "<,/> and =".
        /// </summary>
        public Color Tag { get; set; }

        /// <summary>
        /// Convert the settings to Rtf color definitions.
        /// </summary>
        public string ToRtfFormatString()
        {
            // The Rtf color definition format.
            string format = @"\red{0}\green{1}\blue{2};";

            StringBuilder rtfFormatString = new StringBuilder();

            rtfFormatString.AppendFormat(format, Element.R, Element.G, Element.B);
            rtfFormatString.AppendFormat(format, Value.R, Value.G, Value.B);
            rtfFormatString.AppendFormat(format,
                AttributeKey.R, 
                AttributeKey.G, 
                AttributeKey.B);
            rtfFormatString.AppendFormat(format, AttributeValue.R, 
                AttributeValue.G, AttributeValue.B);
            rtfFormatString.AppendFormat(format, Tag.R, Tag.G, Tag.B);

            return rtfFormatString.ToString();

        }
    }
}
