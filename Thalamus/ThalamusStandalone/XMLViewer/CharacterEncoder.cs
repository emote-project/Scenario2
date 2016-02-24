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
using System.Text;

namespace CSRichTextBoxSyntaxHighlighting
{
    public class CharacterEncoder
    {
        public static string Encode(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
            {
                return string.Empty;
            }

            StringBuilder encodedText = new StringBuilder();
            for (int i = 0; i < originalText.Length; i++)
            {
                switch (originalText[i])
                {
                    case '"':
                        encodedText.Append("&quot;");
                        break;
                    case '&':
                        encodedText.Append(@"&amp;");
                        break;
                    case '\'':
                        encodedText.Append(@"&apos;");
                        break;
                    case '<':
                        encodedText.Append(@"&lt;");
                        break;
                    case '>':
                        encodedText.Append(@"&gt;");
                        break;

                    // The character '\' should be converted to @"\\" or "\\\\" 
                    case '\\':
                        encodedText.Append(@"\\");
                        break;

                    // The character '{' should be converted to @"\{" or "\\{" 
                    case '{':
                        encodedText.Append(@"\{");
                        break;

                    // The character '}' should be converted to @"\}" or "\\}" 
                    case '}':
                        encodedText.Append(@"\}");
                        break;
                    default:
                        encodedText.Append(originalText[i]);
                        break;
                }

            }
            return encodedText.ToString();
        }
    }
}
