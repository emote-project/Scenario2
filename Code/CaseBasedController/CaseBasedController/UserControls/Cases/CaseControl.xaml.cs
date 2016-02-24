using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CaseBasedController.Detection;
using EuxUtils;

namespace CaseBasedController.UserControls.Cases
{
    public class CaseControlViewModel : ViewModelBase
    {
        private bool _enabled = false;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value; 
                NotifyPropertyChanged("Enabled");
            }
        }

        private string _name = "";

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _behaviour = "";

        public string Behaviour
        {
            get { return _behaviour; }
            set { _behaviour = value; }
        }

    }

    public partial class CaseControl : UserControl, IDisposable
    {
        private CaseControlViewModel _data;
        private Case _case;

        public CaseControl()
        {
            InitializeComponent();
        }


        public CaseControl(Case c)
        {
            InitializeComponent();
            _data = (CaseControlViewModel) DataContext;
            _data.Behaviour = c.Behavior.ToString();
            _data.Name = c.Description;
            
            c.Detector.ActivationChanged += DetectorOnActivationChanged;
            _case = c;
        }

        private void DetectorOnActivationChanged(IFeatureDetector detector, bool activated)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                _data.Enabled = activated;
            }));
        }

        public void Dispose()
        {
            _case.Detector.ActivationChanged -= DetectorOnActivationChanged;
        }
    }
}
