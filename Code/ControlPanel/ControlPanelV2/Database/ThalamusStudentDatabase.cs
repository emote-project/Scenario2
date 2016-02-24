using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlPanel.Thalamus;
using EmoteEvents;

namespace ControlPanel.Database
{
    class ThalamusStudentDatabase : IStudentsDatabase,IDisposable
    {
        public event EventHandler<StudentListEventArgs> StudentListUpdatedEvent;
        public event EventHandler ConnectedEvent;
        public event EventHandler ConnectingEvent;
        public event EventHandler TimeoutEvent;

        private const int REQUEST_TIMEOUT_MILLISECONDS = 10000;

        private ControlPanelThalamusClient _client;
        private List<LearnerInfo> _studentsList;

        private bool _resultsReady;

        private int _nextThalamusId;

        private bool _connected;

        public ThalamusStudentDatabase()
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Constructor");
            _client = ControlPanelThalamusClient.GetInstance();
            _client.AllLearnerInfoEvent += _client_AllLearnerInfoEvent;
            _client.NextThalamusIdEvent += ClientOnNextThalamusIdEvent;

            // Having a control like this makes it easy to check the state of the db from the interface
            CheckConnection();
            _connected = false;
        }

        #region IStudentsDatabase implementation
        
        public async Task<List<LearnerInfo>> GetAllStudentsAsync()
        {
            return await Task.Run(() => GetAllStudents());
        }


        public List<LearnerInfo> GetAllStudents()
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": GetAllStudents");
            _resultsReady = false;
            _client.LDBPublisher.getAllLearnerInfo();
            bool result = WaitForResults();
            if (!result)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": GetAllStudents -> Returning null list");
                return null;
            }
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": GetAllStudents -> Returning student list ("+(_studentsList!=null?_studentsList.Count.ToString():"null")+"");
            if (StudentListUpdatedEvent!=null) StudentListUpdatedEvent(this, new StudentListEventArgs(){ StudentList = _studentsList});
            return _studentsList;
        }

        public void AddStudent(LearnerInfo learnerInfo)
        {
            learnerInfo.thalamusLearnerId = learnerInfo.mapApplicationId = _nextThalamusId++;
            _client.LDBPublisher.SetLearnerInfo(learnerInfo.SerializeToJson());
        }

        public void RemoveStudent(LearnerInfo learnerInfo)
        {
            // NOT YET IMPLEMENTED
        }

        public void Connect()
        {
            CheckConnection();
        }

        bool IStudentsDatabase.IsConnected()
        {
            return _connected;
        }

        #endregion

        #region Thalamus client events handlers

        void _client_AllLearnerInfoEvent(object sender, ControlPanelThalamusClient.AllLearnerInfoEventArgs e)
        {
            _studentsList = e.Learners;
            _resultsReady = true;
        }

        private void ClientOnNextThalamusIdEvent(object sender, ControlPanelThalamusClient.NextThalamusIdEventArgs nextThalamusIdEventArgs)
        {
            _nextThalamusId = nextThalamusIdEventArgs.Id;
            _resultsReady = true;
            Console.WriteLine("Next thalamus id: "+nextThalamusIdEventArgs.Id);
        }

        #endregion

        #region Helpers 

        private int GetNextThalamusId()
        {
            _resultsReady = false;
            _client.LDBPublisher.getNextThalamusId();
            bool result = WaitForResults();
            if (!result)
            {
                return -1;
            }
            return _nextThalamusId;
        }

        private async Task<int> GetNextThalamusIdAsync()
        {
            return await Task.Run(() => GetNextThalamusId());
        }

        private async void CheckConnection()
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": CheckConnection");
            if (ConnectingEvent != null) ConnectingEvent(this, null);
            _nextThalamusId = await GetNextThalamusIdAsync();
            if (_nextThalamusId != -1)
            {
                _connected = true;
                if (ConnectedEvent != null) ConnectedEvent(this, null);
            }
            else
            {
                _connected = false;
                await Task.Delay(3000);
                CheckConnection();
            }
        }

        private bool WaitForResults()
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": WaitForResults");
            var start = DateTime.Now;
            while (!_resultsReady)
            {
                System.Threading.Thread.Sleep(100);
                if (DateTime.Now.Subtract(start).TotalMilliseconds >= REQUEST_TIMEOUT_MILLISECONDS)
                {
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": WaitForResults -> Timeout");
                    if (TimeoutEvent != null) TimeoutEvent(this, null);
                    return false; // Avoids to let the process waitinf forever
                }
            }
            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": WaitForResults -> Returning true");
            return true;
        }

        #endregion

        public void Dispose()
        {
            _client.AllLearnerInfoEvent -= _client_AllLearnerInfoEvent;
        }

    }
}
