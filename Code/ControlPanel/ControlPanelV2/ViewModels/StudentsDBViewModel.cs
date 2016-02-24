using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EuxUtils;
using System.Collections.ObjectModel;
using EmoteEvents;

namespace ControlPanel.viewModels
{
    public class StudentsDBViewModel : ViewModelBase
    {
        private ObservableCollection<LearnerInfo> _learners;

        public ObservableCollection<LearnerInfo> Learners
        {
            get { return _learners; }
            set
            {
                _learners = value;
                NotifyPropertyChanged("Learners");
            }
        }

    }
}
