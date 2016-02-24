using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlPanel.Annotations;

namespace ControlPanel.Forms.UserControls
{
    /// <summary>
    /// Interaction logic for CountDownUserControl.xaml
    /// </summary>
    public partial class CountDownUserControl : UserControl, INotifyPropertyChanged
    {
        public event EventHandler CountDownEnded;

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        private TimeSpan _timeLeft;
        public TimeSpan TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value; 
                OnPropertyChanged("TimeLeft");
                OnPropertyChanged("TimeLeftString");
            }
        }

        public string TimeLeftString
        {
            get
            {
                return TimeLeft.ToString(@"mm\:ss");
            }
        }

        public bool Started
        {
            get
            {
                return _started;
            }
            set
            {
                _started = value;
                OnPropertyChanged("Started");
            }
        }

        private bool _reset = false;
        private bool _started = false;



        public CountDownUserControl()
        {
            InitializeComponent();
        }

        public async void Start()
        {
            if (Started) return;

            Started = true;
            TimeLeft = Duration;
            while (TimeLeft.TotalSeconds > 0  && !_reset)
            {
                TimeLeft=TimeLeft.Subtract(new TimeSpan(0, 0, 0, 1));
                await Task.Delay(1000);
            }
            if (CountDownEnded != null && !_reset) CountDownEnded(this, null);
            _reset = false;
            Started = false;
            TimeLeft = Duration;
        }

        public void Reset()
        {
            _reset = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
