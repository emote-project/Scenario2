using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using EmoteEvents.ComplexData;
using Newtonsoft.Json.Schema;
using Skene.Utterances.HistoryManager;

namespace Skene.Utterances
{
    public class UtteranceLibrary
    {
        /// <summary>
        /// Delegate called when an oldy type library is detected. The system will work but all the features linked to the History won't work properly
        /// </summary>
        public delegate void OldTypeLibraryWarning(string warningMessage);

        public const string FIELD_TEXT = "TEXT";
        public const string FIELD_CATEGORY = "CATEGORY";
        public const string FIELD_QUESTION = "QUESTION";
        public const string FIELD_REPETITIONS = "REPETITIONS";
        public const string FIELD_ID = "ID";


        public delegate void LibraryModifiedHandler();
        public event LibraryModifiedHandler LibraryModified;
        private void NotifyLibraryModified()
        {
            if (LibraryModified != null) LibraryModified();
        }

        private Dictionary<string, List<string>> categories = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Categories
        {
            get { return categories; }
        }

        private System.Data.DataTable utterances;
        public System.Data.DataTable Utterances
        {
            get { return utterances; }
        }

        FileSystemWatcher watcher;
        string libraryFilename = "";
        static Random rand;
        public bool CaseSensitiveCategories { get; private set; }

        public string[] UtteranceInstructions = new string[] {};
        public string[] UtteranceTargets = new string[] { };
        public string[] UtteranceAnimations = new string[] { };
        public string[] UtteranceFaceExpressions = new string[] { };
        public string[] UtteranceTags = new string[] { };

        List<string> report_tagsAsTargets = new List<string>();
        List<string> report_tagsAsAnimations = new List<string>();
        List<string> report_tags = new List<string>();
        List<string> report_animations = new List<string>();
        List<string> report_gameInstructions = new List<string>();
        List<string> report_faceExpressions = new List<string>();
        List<string> report_targets = new List<string>();
        List<string> report_warnings = new List<string>();
        List<string> report_errors = new List<string>();
        List<string> report_TTS = new List<string>();

        public void LoadLibrary(string fileName, OldTypeLibraryWarning oldTypeLibraryWarningHandler, bool caseSensitiveCategories = false )
        {
            if (fileName == "" || !File.Exists(fileName))
            {
                string message = string.Format("No such file '{0}'!", fileName);
                Console.WriteLine(message);
                throw new Exception(message);
            }
            CaseSensitiveCategories = caseSensitiveCategories;
            utterances = ImportExcelXLS(fileName, true);
            libraryFilename = fileName;
            LoadCategories();
            //Validate(validator);
            
            Console.WriteLine("Loaded utterance library from '{0}'.", fileName);
            //CreateFileWatcher(fileName);
            rand = new Random(Environment.TickCount);
            
            if (!utterances.Columns.Contains(FIELD_ID))
                if (oldTypeLibraryWarningHandler != null) oldTypeLibraryWarningHandler("The loaded library "+libraryFilename+" is in an old format. The system will work but all the features linked to the History won't work properly. Please update the library.");
        }


        private bool LoadCategories()
        {
            bool result = true;
            for (int i = 0; i < utterances.Rows.Count; i++)
            {
                string category = utterances.Rows[i].Field<string>(FIELD_CATEGORY);

                if (category.Trim().Length == 0 || category.StartsWith(":")) continue;
                string[] split = category.Trim().Split(':');
                if (!CaseSensitiveCategories) split[0] = split[0].ToLower();
                if (!categories.ContainsKey(split[0])) categories[split[0]] = new List<string>();
                string subcat = "-";
                if (split.Length == 1)
                {
                    if (!categories[split[0]].Contains(subcat)) categories[split[0]].Add(subcat);
                }
                else
                {
                    subcat = "";
                    for (int j = 1; j < split.Length; j++)
                    {
                        subcat += (j>1?":":"") + split[j];
                        if (!CaseSensitiveCategories) subcat = subcat.ToLower();
                        if (!categories[split[0]].Contains(subcat)) categories[split[0]].Add(subcat);
                    }
                }
            }

            foreach (string c in categories.Keys)
            {
                foreach (string s in categories[c])
                {
                    List<Utterance> utts = FilterUtterances(c, s);
                    NewRandomHistory(c + ":" + s, utts.Count);
                }
            }

            return result;
        }

        private int StringCount(string text, string occurence)
        {
            int x = text.Length - text.Replace(occurence, "").Length;
            return x;
        }

        #region UTTERANCE VERIFICATION

        string regexAllInstructions = @"<\w+\([\w\/,]+\)>";
        string regexTags = @"(?<=\/)\w+";
        string regexAnimationNames = @"(?<=<ANIMATE\()[\w\/]+";
        string regexGameInstructions = @"(?<=<GAME\()[\w\/,]+";
        string regexFaceExpressions = @"(?<=<FACE\()\w+|(?<=<FACESHIFT\()\w+";
        string regexHeadNodAllInstructions = @"<HEADNOD\(\w*\)>|<HEADNODNEGATIVE\(\w*\)>";
        string regexHeadNodValidInstructions = @"<HEADNOD\(\d*\)>|<HEADNODNEGATIVE\(\d*\)>";
        string regexTargetNames = @"(?<=<INSTRUCTION\()[\w\/]+";
        string regexTTS = @"(?<=\\)\w+";
        string regexTTSPauRspd = @"\\(pau|rspd)=\d+\\";
        string regexTTSAll1 = @"(pau|rspd)=";
        string regexTTSAll2 = @"\\(pau|rspd)";
        

        private bool VerifyUtterance(string text, int line, UtteranceValidationSet validator)
        {
            if (text == null || text.Length == 0) return true;
            bool result = true;
            Regex regex = new Regex(regexAllInstructions, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(text);
            if (matches.Count != StringCount(text, "<"))
            {
                report_errors.Add(string.Format("Missing or Extra '<' in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
                result = false;
            }
            if (matches.Count != StringCount(text, ">"))
            {
                report_errors.Add(string.Format("Missing or Extra '>' in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
                result = false;
            }
            if (matches.Count != StringCount(text, "("))
            {
                report_errors.Add(string.Format("Missing or Extra '(' in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
                result = false;
            }
            if (matches.Count != StringCount(text, ")"))
            {
                report_errors.Add(string.Format("Missing or Extra ')' in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
                result = false;
            }
            
            if (matches.Count > 1)
            {
                foreach (Match match in matches)
                {
                    string instruction = match.Value.Substring(1, match.Value.IndexOf('(') - 1);
                    if (!validator.TargetInstructionValidator.IsValid(instruction) && !validator.NonTargetInstructionValidator.IsValid(instruction))
                    {
                        report_errors.Add(string.Format("Invalid instruction <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", instruction, line, HttpUtility.HtmlEncode(text)));
                        result = false;
                    }
                }
            }

            Regex hnall = new Regex(regexHeadNodAllInstructions, RegexOptions.IgnoreCase);
            MatchCollection hnallmatches = hnall.Matches(text);
            Regex hnv = new Regex(regexHeadNodValidInstructions, RegexOptions.IgnoreCase);
            MatchCollection hnvmatches = hnv.Matches(text);
            if (hnallmatches.Count != hnvmatches.Count)
            {
                report_errors.Add(string.Format("Invalid parameter in HEADNOD or HEADNODNEGATIVE in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
            }

            regex = new Regex(validator.TargetInstructionValidator.BuildRegexParameterString(regexTargetNames), RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.TargetsValidator.IsValid(match.Value))
                {
                    if (StringCount(match.Value, "/") == 2)
                    {
                        string tag = match.Value.Substring(match.Value.IndexOf('/')+1, match.Value.LastIndexOf('/') - match.Value.IndexOf('/') - 1);
                        if (!validator.TagsValidator.IsValid(tag))
                        {
                            report_errors.Add(string.Format("Invalid Tag as Target <b>{0} in utterance in <b>line {1}: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                        }
                        if (!report_tagsAsTargets.Contains(match.Value)) report_tagsAsTargets.Add(match.Value);
                    }
                    else
                    {
                        report_errors.Add(string.Format("Invalid Target <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                        result = false;
                    }
                }
                if (!report_targets.Contains(match.Value)) report_targets.Add(match.Value);
            }

            regex = new Regex(regexAnimationNames, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.AnimationValidator.IsValid(match.Value))
                {
                    if (StringCount(match.Value, "/") == 2)
                    {
                        string tag = match.Value.Substring(match.Value.IndexOf('/') + 1, match.Value.LastIndexOf('/') - match.Value.IndexOf('/') - 1);
                        if (validator.TagsValidator.IsValid(tag))
                        {
                            if (!report_tagsAsAnimations.Contains(match.Value)) report_tagsAsAnimations.Add(match.Value);
                        }
                        else
                        {
                            report_errors.Add(string.Format("Invalid Tag as Animation <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                        }
                    }
                    else
                    {
                        report_errors.Add(string.Format("Invalid Animation <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                        result = false;
                    }
                }
                if (!report_animations.Contains(match.Value)) report_animations.Add(match.Value);
            }

            regex = new Regex(regexFaceExpressions, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.FaceExpressionsValidator.IsValid(match.Value))
                {
                    report_errors.Add(string.Format("Invalid Face Expression <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                    result = false;
                }
                if (!report_faceExpressions.Contains(match.Value)) report_faceExpressions.Add(match.Value);
            }

            regex = new Regex(regexGameInstructions, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.GameInstructions.IsValid(match.Value.IndexOf(',') != -1 ? match.Value.Substring(0, match.Value.IndexOf(',')) : match.Value))
                {
                    report_errors.Add(string.Format("Invalid Game Instruction <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                    result = false;
                }
                if (!report_gameInstructions.Contains(match.Value)) report_gameInstructions.Add(match.Value);
            }

            regex = new Regex(regexTags, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.TagsValidator.IsValid(match.Value))
                {
                    report_errors.Add(string.Format("Invalid Tag <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                    result = false;
                }
                if (!report_tags.Contains(match.Value)) report_tags.Add(match.Value);
            }

            regex = new Regex(regexTTS, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (!validator.TTSValidator.IsValid(match.Value))
                {
                    report_errors.Add(string.Format("Invalid TTS instruction <b>{0}</b> in utterance in <b>line {1}</b>: '{2}'", match.Value, line, HttpUtility.HtmlEncode(text)));
                    result = false;
                }
                if (!report_TTS.Contains(match.Value)) report_TTS.Add(match.Value);
            }

            regex = new Regex(regexTTSPauRspd, RegexOptions.IgnoreCase);
            matches = regex.Matches(text);
            Regex ttsall1 = new Regex(regexTTSAll1, RegexOptions.IgnoreCase);
            MatchCollection tts1matches = ttsall1.Matches(text);
            Regex ttsall2 = new Regex(regexTTSAll2, RegexOptions.IgnoreCase);
            MatchCollection tts2matches = ttsall2.Matches(text);
            if (matches.Count != tts1matches.Count || matches.Count != tts2matches.Count)
            {
                report_errors.Add(string.Format("Invalid TTS pause instruction in utterance in <b>line {0}</b>: '{1}'", line, HttpUtility.HtmlEncode(text)));
            }           

            return result;
        }


        public void Validate(UtteranceValidationSet validator)
        {
            report_tagsAsTargets = new List<string>();
            report_tagsAsAnimations = new List<string>();
            report_tags = new List<string>();
            report_animations = new List<string>();
            report_gameInstructions = new List<string>();
            report_faceExpressions = new List<string>();
            report_targets = new List<string>();
            report_errors = new List<string>();
            report_TTS = new List<string>();
            int validUtterances = 0;
            for (int i = 0; i < utterances.Rows.Count; i++)
            {
                string text = utterances.Rows[i].Field<string>(FIELD_TEXT);
                if (VerifyUtterance(text, i + 2, validator))
                {
                    validUtterances++;
                }
            }
            string reportPath = Path.GetDirectoryName(libraryFilename) + "\\" + Path.GetFileNameWithoutExtension(libraryFilename) + "_validationReport.html";
            TextWriter tw = new StreamWriter(reportPath);
            tw.WriteLine("<html><head /><body><b>Skene Utterances Validation Report</b><br><br><b>Library:</b> {0}<br><b>Validation Set:</b> {5}<br><b>Total Utterances:</b> {1}<br><b>Valid Utterances:</b> {2}<br><b>Invalid Utterances:</b> {3}<br><b>Errors:</b> {4}<br><br><br><b>Error list:</b><br><br>", libraryFilename, utterances.Rows.Count, validUtterances, utterances.Rows.Count - validUtterances, report_errors.Count, validator.Filename);

            tw.WriteLine("<table border='1'>");
            foreach (string line in report_errors)
            {
                tw.WriteLine("<tr><td width=\"3%\" align=\"center\">-</td><td>");
                tw.WriteLine(String.Format(line));
                tw.WriteLine("</td></tr>");
            }
            tw.WriteLine("</table><br><br>");

            tw.WriteLine("<br><b>Tags used in Targets:</b><br>");
            foreach (string line in report_tagsAsTargets)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Tags used in Animations:</b><br>");
            foreach (string line in report_tagsAsAnimations)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Tags:</b><br>");
            foreach (string line in report_tags)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Targets:</b><br>");
            foreach (string line in report_targets)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Animations:</b><br>");
            foreach (string line in report_animations)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Face Expressions:</b><br>");
            foreach (string line in report_faceExpressions)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Game Instructions:</b><br>");
            foreach (string line in report_gameInstructions)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><b>Text-to-Speech Instructions:</b><br>");
            foreach (string line in report_TTS)
            {
                tw.WriteLine(HttpUtility.HtmlEncode(line) + "<br>");
            }

            tw.WriteLine("<br><br><b>End of validation report.</b><br>");
            tw.Close();
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = reportPath
            };
            process.Start();
        }

        #endregion 

        public List<Utterance> FilterUtterances(string category, string subcategory = "-")
        {
            if (!CaseSensitiveCategories)
            {
                category = category.ToLower();
                subcategory = subcategory.ToLower();
            }
            var filteredUtterances = new List<Utterance>();

            if (subcategory == "-")
            {
                filteredUtterances = QueryUtterancesEquals(category + ":-");
                if (filteredUtterances.Count == 0) filteredUtterances = QueryUtterancesEquals(category);
            }
            else
            {
                filteredUtterances = QueryUtterancesEquals(category + ":" + subcategory);
                filteredUtterances.AddRange(QueryUtterancesStartsWith(category + ":" + subcategory + ":"));
            }

            return filteredUtterances;
        }

        private List<Utterance> QueryUtterancesEquals(string fullCategory)
        {
            var filteredUtterances = new List<Utterance>();
            System.Data.EnumerableRowCollection temp;

            //temp = Utterances.AsEnumerable().Where(row => row.Field<string>(FIELD_CATEGORY).ToLower().StartsWith(fullCategory));
            temp = Utterances.AsEnumerable().Where(row => row.Field<string>(FIELD_CATEGORY).ToLower().Equals(fullCategory));
            foreach (DataRow x in temp)
            {
                string id = x.Table.Columns.Contains(FIELD_ID) ? x[FIELD_ID].ToString() : "";
                string library = Path.GetFileName(libraryFilename);
                string text = x[FIELD_TEXT].ToString();
                string[] catSplit = x[FIELD_CATEGORY].ToString().Split(new char[] { ':' }, 2);
                string cat = catSplit[0];
                string sub = catSplit.Length > 1 ? catSplit[1] : "-";
                string memory = x.Table.Columns.Contains(FIELD_QUESTION) ? x[FIELD_QUESTION].ToString() : "";
                string repetitions = x.Table.Columns.Contains(FIELD_REPETITIONS) ? x[FIELD_REPETITIONS].ToString() : "";
                var ut = new Utterance(id, library, text, cat, sub, memory, repetitions);
                filteredUtterances.Add(ut);
            }
            return filteredUtterances;
        }

        private List<Utterance> QueryUtterancesStartsWith(string fullCategory)
        {
            var filteredUtterances = new List<Utterance>();
            System.Data.EnumerableRowCollection temp;
            temp = Utterances.AsEnumerable().Where(row => row.Field<string>(FIELD_CATEGORY).ToLower().StartsWith(fullCategory));
            foreach (DataRow x in temp)
            {
                string id = x.Table.Columns.Contains(FIELD_ID) ? x[FIELD_ID].ToString() : "";
                string library = Path.GetFileName(libraryFilename);
                string text = x[FIELD_TEXT].ToString();
                string[] catSplit = x[FIELD_CATEGORY].ToString().Split(new char[] { ':' }, 2);
                string cat = catSplit[0];
                string sub = catSplit.Length > 1 ? catSplit[1] : "-";
                string memory = x.Table.Columns.Contains(FIELD_QUESTION) ? x[FIELD_QUESTION].ToString() : "";
                string repetitions = x.Table.Columns.Contains(FIELD_REPETITIONS) ? x[FIELD_REPETITIONS].ToString() : "";
                var ut = new Utterance(id, library, text, cat, sub, memory, repetitions);
                filteredUtterances.Add(ut);
            }
            return filteredUtterances;
        }

        public List<string> FilterSubcategories(string category)
        {
            if (!CaseSensitiveCategories)
            {
                category = category.ToLower();
            }
            if (categories.ContainsKey(category)) return categories[category];
            else return new List<string>();
        }

        public Utterance GetUtterance(string category, string subcategory = "-")
        {
            if (subcategory == "") subcategory = "-";
            if (!CaseSensitiveCategories)
            {
                category = category.ToLower();
                subcategory = subcategory.ToLower();
            }
            if (categories.ContainsKey(category))
            {
                if (categories[category].Contains(subcategory))
                {
                    List<Utterance> utterances = FilterUtterances(category, subcategory);
                    return ChooseUtterance(utterances, this, category + ":" + subcategory);
                }
                else return null;
            }
            else return null;
        }

        /// <summary>
        /// Select an utterance from a list of utterances using techniques to avoid repetitions. 
        /// At the moment it considers: 
        /// - Sessions' history: doesn't repeat utterances told in this session or in any of the previous sessions if the utterance is marked as not repeatable in the excel file.
        /// TODO: avoid to repeat the same utterance twice in a row.
        /// </summary>
        /// <param name="utterances"></param>
        /// <returns></returns>
        private static Utterance ChooseUtterance(IList<Utterance> utterances, UtteranceLibrary ul = null, string historicRandomCategory = "")
        {
            List<int> invalidUtteranceIndexes = new List<int>();
            for (int i = 0; i < utterances.Count; i++) if (!CheckHistory(utterances[i])) invalidUtteranceIndexes.Add(i);
            // Starting from a random utterance
            int uttIndex;
            if (ul != null && historicRandomCategory != "")
            {
                uttIndex = ul.HistoricRandom(historicRandomCategory, invalidUtteranceIndexes);
            }
            else
            {
                uttIndex = rand.Next(utterances.Count);
            }
            if (uttIndex == -1) return null;
            return utterances[uttIndex];
            /*
            //int startIndex = uttIndex;
            Utterance selectedUtt = utterances[uttIndex];
            int possibleUtterances = utterances.Count+1;
            // selects a different utterance if the chosed one was already used and can't be used anymore.
            while (!CheckHistory(selectedUtt))
            {
                possibleUtterances--;
                if (possibleUtterances == 0) return null;
                if (ul != null && historicRandomCategory != "")
                {
                    uttIndex = ul.HistoricRandom(historicRandomCategory);
                }
                else
                {
                    uttIndex = rand.Next(utterances.Count);
                }
           
                //int startIndex = uttIndex;
                selectedUtt = utterances[uttIndex];
            }
            return selectedUtt;
             * */
        }

        public static Utterance GetCompositeUtterance(List<UtteranceLibrary> libraries, string category, string subcategory = "-")
        {
            List<Utterance> compositeSubcategories = new List<Utterance>();
            foreach (UtteranceLibrary ul in libraries)
            {
                string c = category;
                string s = subcategory;
                if (!ul.CaseSensitiveCategories)
                {
                    c = c.ToLower();
                    s = s.ToLower();
                }
                if (ul.Categories.ContainsKey(c) && ul.Categories[c].Contains(s))   
                {
                    compositeSubcategories = compositeSubcategories.Union<Utterance>(ul.FilterUtterances(c, s)).ToList();
                }
            }

            if (compositeSubcategories.Count>0)
            {
                //return compositeSubcategories[rand.Next(compositeSubcategories.Count)];
                return ChooseUtterance(compositeSubcategories);
            }
            else return null;

        }

        private void CreateFileWatcher(string filename)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(filename);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = Path.GetFileName(filename); ;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Utterances database modified.");
            NotifyLibraryModified();
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (e.FullPath == libraryFilename)
            {
                Console.WriteLine("Utterances database modified.");
                NotifyLibraryModified();
                
            }
        }

        private System.Data.DataTable ImportExcelXLS(string FileName, bool hasHeaders)
        {
            string HDR = hasHeaders ? "Yes" : "No";
            string strConn;
            //if (FileName.Substring(FileName.LastIndexOf('.')).ToLower() == ".xlsx")
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\";Mode=\"Share Deny None\"";
            /*else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\";Mode=\"Share Deny None\"";*/

            System.Data.DataTable output = new System.Data.DataTable();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                categories = new Dictionary<string, List<string>>();

                conn.Open();
                try
                {
                    System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(
                        OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });


                    foreach (DataRow schemaRow in schemaTable.Rows)
                    {
                        string sheet = schemaRow["TABLE_NAME"].ToString();
                        if (sheet.ToLower() == "utterances$")
                        {
                            try
                            {
                                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "] WHERE [CATEGORY] IS NOT NULL", conn);
                                cmd.CommandType = CommandType.Text;
                                output = new System.Data.DataTable(sheet);
                                new OleDbDataAdapter(cmd).Fill(output);
                            }
                            catch
                            {
                                try
                                {
                                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "] WHERE TEXT<>'' AND CATEGORY<>''", conn);
                                    cmd.CommandType = CommandType.Text;
                                    output = new System.Data.DataTable(sheet);
                                    new OleDbDataAdapter(cmd).Fill(output);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, FileName), ex);
                                }
                            }
                        }
                        /*else if (sheet.ToLower() == "categories$")
                        {
                            System.Data.DataTable categoriesTable;
                            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                            cmd.CommandType = CommandType.Text;

                            categoriesTable = new System.Data.DataTable(sheet);
                            new OleDbDataAdapter(cmd).Fill(categoriesTable);
                            List<string> categoriesList = categoriesTable.AsEnumerable().Select(x => x[0].ToString()).ToList();
                            foreach (string c in categoriesList)
                            {
                                if (c.Trim().Length == 0 || c.StartsWith(":")) continue;
                                string[] split = c.Trim().Split(':');
                                if (!CaseSensitiveCategories) split[0] = split[0].ToLower();
                                if (!categories.ContainsKey(split[0])) categories[split[0]] = new List<string>();
                                if (split.Length > 1)
                                {
                                    if (!CaseSensitiveCategories) categories[split[0]].Add(split[1].ToLower());
                                    else categories[split[0]].Add(split[1]);
                                }
                                else if (!categories[split[0]].Contains("-")) categories[split[0]].Add("-");
                            }
                        }*/
                    }
                }
                catch (System.InvalidOperationException e)
                {
                    System.Windows.Forms.MessageBox.Show("Couldn't load Excel utterances database", "Try to install Microsoft.ACE.OLEDB.12.0 package");
                }
                conn.Close();
            }
            return output;
        }

        #region helpers

        /// <summary>
        /// Check if an utterance can be use. If the utterance is marked as OnceAndForever or OnceInASession, it checks if it was or it wasn't used .
        /// </summary>
        /// <param name="utterance"></param>
        /// <returns>
        /// Returns true if the utterance is valid and can be used
        /// </returns>
        private static bool CheckHistory(Utterance utterance)
        {
            IUtterancesHistoryManager utterancesHistoryManager = HistoryManagerFactory.GetHistoryManager();
            return !(utterance.Repetitions == RepetitionType.OnceAndForever && utterancesHistoryManager.WasEverUsed(utterance)) &&
                   !(utterance.Repetitions == RepetitionType.OnceInASession && utterancesHistoryManager.WasRecentlyUsed(utterance));
        }

        private string ExtractCategoryWithoutSubcategory(string categorystring)
        {
            var ar = categorystring.Split(':');
            if (ar.Length > 0)
            {
                return ar[0];
            }
            else
            {
                throw new Exception("Malformed string: "+categorystring);
            }
        }

        private string ExtractSubcategory(string categorystring)
        {
            var ar = categorystring.Split(':');
            if (ar.Length > 1)
            {
                return ar[1];
            }
            else
            {
                throw new Exception("Malformed string: " + categorystring);
            }
        }



        #endregion

        #region HistoricRandom

        Dictionary<string, Dictionary<int, bool>> randomHistory = new Dictionary<string, Dictionary<int, bool>>();
        int lastRandomHistoryIndex = -1;

        public void NewRandomHistory(string name, int count)
        {
            RestartRandomHistory(name.ToLower(), count);
        }

        public int HistoricRandom(string name, List<int> invalidIndexes)
        {
            int selectedIndex = -1;
            if (randomHistory.ContainsKey(name) && randomHistory[name].Count > 0)
            {

                List<int> list = new List<int>();
                foreach (KeyValuePair<int, bool> index in randomHistory[name]) if (!index.Value && !invalidIndexes.Contains(index.Key)) list.Add(index.Key);
                System.Random randomGen = new System.Random();
                if (list.Count == 0) return -1;

                selectedIndex = list[randomGen.Next(list.Count)];
                if (list.Count > 1)
                {
                    while (selectedIndex == lastRandomHistoryIndex) selectedIndex = list[randomGen.Next(list.Count)];
                }
                randomHistory[name][selectedIndex] = true;

                if (list.Count == 1)
                {
                    RestartRandomHistory(name, randomHistory[name].Count);
                }
            }
            lastRandomHistoryIndex = selectedIndex;
            return selectedIndex;
        }

        private void RestartRandomHistory(string name, int count)
        {
            Dictionary<int, bool> l = new Dictionary<int, bool>();
            for (int i = 0; i < count; i++)
            {
                l[i] = false;
            }
            randomHistory[name] = l;
        }

        #endregion
    }
}
