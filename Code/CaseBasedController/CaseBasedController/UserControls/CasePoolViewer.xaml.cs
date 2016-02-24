using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
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
using CaseBasedController.UserControls.Cases;
using CaseBasedController.UserControls.Detectors;

namespace CasePoolViewer.UserControls
{

    public partial class CasePoolViewer : UserControl
    {
        private CasePool _casePool;

        private Dictionary<IFeatureDetector,DetectorControl> _detectorControls = new Dictionary<IFeatureDetector, DetectorControl>();
        private Dictionary<Case, CaseControl> _casesControls = new Dictionary<Case, CaseControl>(); 

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
            List<IFeatureDetector> detectors = _casePool.GetAllDetectors().ToList();
            var baseDetectors = detectors.Where(d => !(d is CompositeFeatureDetector));
            var compositeDetectors = detectors.Where(d => (d is CompositeFeatureDetector));

            int baseX = 10;
            int baseY = 10;
            int horizzontalSeparation = 50;
            int verticalSeparation = 320;

            int maxDepth = 0;
            int i = 1;
            foreach (var d in detectors)
            {
                var dc = new DetectorControl(d);
                _detectorControls.Add(d,dc);
                LayoutRoot.Children.Add(dc);

                int threeDepth = ThreeDepth(d);
                maxDepth = Math.Max(maxDepth, threeDepth);
                Canvas.SetTop(dc, baseY + horizzontalSeparation * i++);
                Canvas.SetLeft(dc, baseX + verticalSeparation*(threeDepth));

                List<DetectorControl> linked = new List<DetectorControl>();
                if (d is CompositeFeatureDetector)
                {
                    linked =
                        _detectorControls.Where(
                            detCont => ((CompositeFeatureDetector) d).Detectors.Contains(detCont.Key))
                            .Select(x => x.Value)
                            .ToList();
                }
                if (d is WatcherFeatureDetector)
                {
                    linked =
                        _detectorControls.Where(
                            detCont => ((WatcherFeatureDetector) d).WatchedDetector == detCont.Key)
                            .Select(x => x.Value)
                            .ToList();
                }
                foreach (var detectorControl in linked)
                {
                    DrawLine(detectorControl, dc);
                }
            }

            i = 1;
            foreach (var poolElement in _casePool.GetPool())
            {
                Case c = poolElement.Value;
                IFeatureDetector mainDetector = poolElement.Key;

                var caseControl = new CaseControl(c);
                _casesControls.Add(c, caseControl);
                LayoutRoot.Children.Add(caseControl);
                Canvas.SetTop(caseControl, baseY+horizzontalSeparation*i++);
                Canvas.SetLeft(caseControl, baseY + verticalSeparation * (maxDepth + 1));

                DetectorControl dc = _detectorControls[mainDetector];
                DrawLine(dc,caseControl);
            }
        }

        private void DrawLine(UserControl uc1, UserControl uc2)
        {
            var line = new Line
            {
                Stroke = System.Windows.Media.Brushes.LightSteelBlue,
                X1 = Canvas.GetLeft(uc1) + uc1.Width,
                Y1 = Canvas.GetTop(uc1),
                X2 = Canvas.GetLeft(uc2),
                Y2 = Canvas.GetTop(uc2),
                StrokeThickness = 2
            };
            LayoutRoot.Children.Add(line);
        }

        private int ThreeDepth(IFeatureDetector detector)
        {
            int depth = 0;
            return ThreeDeptRec(detector, ref depth);
        }

        private int ThreeDeptRec(IFeatureDetector detector, ref int depth)
        {
            if (!(detector is CompositeFeatureDetector || detector is WatcherFeatureDetector))
            {
                return depth;
            }
            depth = depth + 1;

            if (detector is CompositeFeatureDetector)
            {
                var subDet = ((CompositeFeatureDetector) detector).Detectors;
                if (subDet == null) return depth;

                foreach (var sub in subDet)
                {
                    var temp = ThreeDeptRec(sub, ref depth);
                    depth = Math.Max(depth, temp);          // I want to consider the max depth among all the branches 
                }
            }
            if (detector is WatcherFeatureDetector)
            {
                var subDet = ((WatcherFeatureDetector)detector).WatchedDetector;
                if (subDet == null) return depth;

                depth = ThreeDeptRec(subDet, ref depth);
            }
            return depth;

        }
    
    }
}
