using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CaseBasedController;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Detection.Enercities;
using CasePoolViewer.UserControls.Detectors;

namespace CasePoolViewer.UserControls
{

    public partial class CasePoolViewer : UserControl
    {
        private CasePool _casePool;

        public CasePool Pool
        {
            get { return _casePool; }
            set { _casePool = value; }
        }


        public CasePoolViewer()
        {
            InitializeComponent();
        }

        public void Initialize(CasePool casePool)
        {
            _casePool = casePool;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Draw();
        }

        public void Draw()
        {
            if (_casePool == null) return;
            List<IFeatureDetector> detectors = (List<IFeatureDetector>)_casePool.GetAllDetectors();
            var baseDetectors = detectors.Where(d => !(d is CompositeFeatureDetector));
            int i = 1;
            foreach (var d in baseDetectors)
            {
                var dc = new DetectorControl()
                {
                    Description = d.Description,
                    DetectorType = d.GetType().ToString()
                };

                LayoutRoot.Children.Add(dc);
                Canvas.SetTop(dc, 10 + 50 * i++);
                Canvas.SetLeft(dc, 10);
            }
        }
    }
}
