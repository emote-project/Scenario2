using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlPanel.Database;
using EmoteEvents;
using ControlPanel.Thalamus;

namespace ControlPanel
{
    class DatabaseWindowController
    {
        private IStudentsDatabase _db;

        public DatabaseWindowController(IStudentsDatabase db)
        {
            _db = db;
        }

        public async Task<bool> AddLearnerToLearnerModel(LearnerInfo learnerInfo)
        {
            if (_db != null)
            {
                _db.AddStudent(learnerInfo);
                await Task.Delay(2000);
                await _db.GetAllStudentsAsync();
                return true;
            }
            return false;
        }

        public bool RemoveLearnerFromLearnerModel(LearnerInfo learnerInfo)
        {
            if (_db != null)
            {
                _db.RemoveStudent(learnerInfo);
                return true;
            }
            return false;
        }

        public List<LearnerInfo> ImportCSV(string csvPath)
        {
            List<LearnerInfo> learnerInfos = new List<LearnerInfo>();
            if (File.Exists(csvPath))
            {
                using (TextReader tr = new StreamReader(csvPath))
                {
                    string line = tr.ReadLine();
                    int lineNumber = 0;
                    while (line!=null)
                    {
                        string[] fields = line.Split(',');
                        if (fields.Length != 5 && !(lineNumber==0 ^ fields[0].ToLower().Equals("First Name")) ) return null;                // CSV MALFORMED
                        if (lineNumber > 0)     // the first line contains headers
                        {
                            LearnerInfo li = new LearnerInfo(fields[0], fields[1], fields[2], 0, fields[3], fields[4], 0);
                            learnerInfos.Add(li);
                            _db.AddStudent(li);
                        }
                        line = tr.ReadLine();
                        lineNumber++;

                    }
                }
            }
            return learnerInfos;
        }

        public async Task<List<LearnerInfo>> GetStudentListFromDatabaseAsync()
        {
            if (_db == null) return null;
            return await _db.GetAllStudentsAsync();
        }

    }
}
