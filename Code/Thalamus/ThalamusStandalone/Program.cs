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
using System.Windows.Forms;

namespace Thalamus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        //[MTAThread]
        static void Main(string[] args)
        {
            bool csv = false;
            bool loadScenario = false;
            string initialCharacter = "";
            bool ok = true;

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "help":
                        ok = CmdLineUsage();
                        break;
                    case "-s":
                    case "-scenario":
                        loadScenario = true;
                        break;
                    case "-csv":
                        csv = true;
                        break;
                    default:
                        if (initialCharacter != "" || arg.StartsWith("-")) ok = CmdLineUsage();
                        else initialCharacter = arg;
                        break;
                }
            }

            if (ok)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmThalamus(initialCharacter, csv, loadScenario));
            }
        }

        private static bool CmdLineUsage()
        {
            MessageBox.Show("Usage: ThalamusStandalone.exe [CHARACTER_NAME|SCENARIO_NAME] [-s] [-csv]", "ThalamusStandalone: Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
    }
}
