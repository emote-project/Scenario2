using GraphSharp.Controls;
using QuickGraph;
using System;
using System.Collections.Generic;
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
using InteractionsCanvas.ViewModels;
using CaseBasedController.Detection;
using CaseBasedController;
using CaseBasedController.Detection.Composition;
using Microsoft.WindowsAPICodePack.Dialogs;



namespace InteractionsCanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _casePoolPath = @"..\..\..\Tests\Test.json";
        List<IFeatureDetector> _detectors;

        GraphViewModel _graphModel;
        QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>> _graph;
        CheckFile _fileChecker = null;


        public MainWindow()
        {
            InitializeComponent();
            string llf = Properties.Settings.Default.LastLoadedFile;
            if (llf != null && llf != "" && System.IO.File.Exists(llf))
            {
                _casePoolPath = llf;
                ReloadCasePool();
            }
           
        }

        public void ReloadCasePool()
        {
            //CasePool cp = CaseBasedController.Programs.CasePoolCodingProgram.EnercitiesDemo();
            //var graph = CreatePoolGraph(cp);
            try
            {
                CasePool cp = CasePool.DeserializeFromJson(_casePoolPath);
                _detectors = cp.GetAllDetectors().ToList();

                DrawGraph(_detectors);

                if (_fileChecker != null) _fileChecker.Dispose();
                _fileChecker = new CheckFile(System.IO.Path.GetDirectoryName(_casePoolPath), this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Case pool file not well formed. Couldn't load it." + Environment.NewLine + ex.Message, "Error loading case pool file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawGraph(List<IFeatureDetector> detectors)
        {
            var graph = CreateDetectorsGraph(detectors);
            _graphModel = new GraphViewModel() { Graph = graph };

            this.Dispatcher.Invoke(new Action(() =>
            {
                DataContext = _graphModel;
                cmbDetectors.DataContext = _detectors;
                layout.Relayout();
            }));
        }


        QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>> CreatePoolGraph(CasePool casePool)
        {
            _graph = new QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>>();


            //List<IFeatureDetector> detectors = casePool.GetPool().Select(c => c.Value.Detector).Distinct().ToList <IFeatureDetector>();
            List<IFeatureDetector> detectors = casePool.GetAllDetectors().ToList();
            Dictionary<IFeatureDetector, MyVertex> detectorsVertexes = new Dictionary<IFeatureDetector, MyVertex>();

            foreach (IFeatureDetector d in detectors)
            {
                MyVertex vert = new MyVertex() { Name = d.ToString(), Detector = d };
                detectorsVertexes.Add(d, vert);
                _graph.AddVertex(vert);
            }

            foreach (IFeatureDetector d in detectors)
            {
                if (d is CompositeFeatureDetector)
                {
                    MyVertex v1 = detectorsVertexes[d];
                    List<IFeatureDetector> subDetectors = ((CompositeFeatureDetector)d).Detectors.ToList();
                    foreach (var sub in subDetectors)
                    {
                        MyVertex v2 = detectorsVertexes[sub];
                        _graph.AddEdge(new Edge<MyVertex>(v1, v2));
                    }
                }
                if (d is WatcherFeatureDetector)
                {
                    MyVertex v1 = detectorsVertexes[d];
                    var sub = ((WatcherFeatureDetector)d).WatchedDetector;
                    MyVertex v2 = detectorsVertexes[sub];
                    _graph.AddEdge(new Edge<MyVertex>(v1, v2));

                }
            }



            //graph.AddEdge(new DataEdge(v1, v2) { Text = "Test edge v1-v2" });

            return _graph;
        }

        QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>> CreateDetectorsGraph(List<IFeatureDetector> detectors)
        {
            _graph = new QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>>();

            Dictionary<IFeatureDetector, MyVertex> detectorsVertexes = new Dictionary<IFeatureDetector, MyVertex>();

            // ADDING VERTEXES
            foreach (IFeatureDetector d in detectors)
            {
                MyVertex vert = new MyVertex() { Name = d.ToString(), Detector = d };
                detectorsVertexes.Add(d, vert);
                _graph.AddVertex(vert);
            }


            // ADDING EDGES
            foreach (IFeatureDetector d in detectors)
            {
                if (d is CompositeFeatureDetector)
                {
                    MyVertex v1 = detectorsVertexes[d];
                    List<IFeatureDetector> subDetectors = ((CompositeFeatureDetector)d).Detectors.ToList();
                    foreach (var sub in subDetectors)
                    {
                        MyVertex v2 = detectorsVertexes[sub];
                        _graph.AddEdge(new Edge<MyVertex>(v1, v2));
                    }
                }
                if (d is WatcherFeatureDetector)
                {
                    MyVertex v1 = detectorsVertexes[d];
                    var sub = ((WatcherFeatureDetector)d).WatchedDetector;
                    MyVertex v2 = detectorsVertexes[sub];
                    _graph.AddEdge(new Edge<MyVertex>(v1, v2));

                }
            }

            return _graph;
        }

        #region GUI Event Handling

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            _graph.AddVertex(new MyVertex() { Name = "Test", Detector = null });
        }

        private void Load_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select Case Pool file";

            dlg.Filters.Add(new CommonFileDialogFilter("CasePool json file", "*.json"));

            dlg.AddToMostRecentlyUsedList = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    _casePoolPath = dlg.FileName;
                    ReloadCasePool();
                    Properties.Settings.Default.LastLoadedFile = dlg.FileName;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void cmbDetectors_Selected(object sender, SelectionChangedEventArgs e)
        {
            IFeatureDetector selectedDetector = cmbDetectors.SelectedItem as IFeatureDetector;
            var subDets = CasePool.FindChildrenDetectors(selectedDetector);
            DrawGraph(subDets);
        }

        private void ResetFilter_Button_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph(_detectors);
        }



        #endregion

        
        

    }

}
