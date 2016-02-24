using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using CaseBasedController.Detection;
using EuxUtils;

namespace CasePoolViewer.UserControls.Detectors
{
    //public class DetectorVM : ViewModelBase
    //{
    //    private string _detectorType;
    //    private string _description;

    //    public string Description
    //    {
    //        get { return _description; }
    //        set
    //        {
    //            _description = value;
    //            NotifyPropertyChanged("Description");
    //        }
    //    }
    //    public string DetectorType
    //    {
    //        get { return _detectorType; }
    //        set
    //        {
    //            _detectorType = value;
    //            NotifyPropertyChanged("DetectorType");
    //        }
    //    }
    //}

    public partial class DetectorControl : UserControl
    {
        //private DetectorVM _data;

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set
            {
                SetValueDp(DescriptionProperty, value);
                //_data.Description = value;
            }
        }
        public string DetectorType
        {
            get { return (string)GetValue(DetectorTypeProperty); }
            set
            {
                SetValueDp(DetectorTypeProperty, value);
                //_data.DetectorType = value;
            }
        }

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
