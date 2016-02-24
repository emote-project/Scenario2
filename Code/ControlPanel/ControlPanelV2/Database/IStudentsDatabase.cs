using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmoteEvents;
using Thalamus;

namespace ControlPanel.Database
{
    public class StudentListEventArgs : EventArgs
    {
        public List<LearnerInfo> StudentList { get; set; }
    }

    public interface IStudentsDatabase
    {
        event EventHandler<StudentListEventArgs> StudentListUpdatedEvent;
        event EventHandler ConnectedEvent;
        event EventHandler ConnectingEvent;
        event EventHandler TimeoutEvent;

        Task<List<LearnerInfo>> GetAllStudentsAsync();
        void AddStudent(LearnerInfo learnerInfo);
        void RemoveStudent(LearnerInfo learnerInfo);
        void Connect();
        bool IsConnected();
    }
}
