using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CaseBasedController.Detection;

namespace CaseBasedController.UserControls.Detectors
{
    
    public partial class DetectorControl : UserControl
    {
        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set
            {
                SetValueDp(EnabledProperty, value);
            }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set
            {
                SetValueDp(DescriptionProperty, value);
            }
        }
        public string DetectorType
        {
            get { return (string)GetValue(DetectorTypeProperty); }
            set
            {
                SetValueDp(DetectorTypeProperty, value);
            }
        }



        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof(bool), typeof(DetectorControl), null);

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DetectorControl), null);

        public static readonly DependencyProperty DetectorTypeProperty =
            DependencyProperty.Register("DetectorType", typeof(string), typeof(DetectorControl), null);

        


        public DetectorControl()
        {
            InitializeComponent();
            //_data = (DetectorVM) LayoutRoot.DataContext;
            LayoutRoot.DataContext = this;
        }

        public DetectorControl(IFeatureDetector detector)
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;

            string detectorType = detector.GetType().ToString();
                detectorType = detectorType.Substring(detectorType.LastIndexOf('.') + 1);

            Description = detector.Description;
            DetectorType = detectorType;
            detector.ActivationChanged += delegate(IFeatureDetector featureDetector, bool activated)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    Enabled = activated;
                }));
            };
        }



        #region helpers

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value,
            [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }
}
