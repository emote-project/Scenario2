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
using CaseCanvas.GraphDefinition;
using CaseBasedController;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;

namespace CaseCanvas
{
    
    public partial class MainWindow : Window
    {
        CasePool _casePool;

        public MainWindow()
        {
            InitializeComponent();

            _casePool = CasePool.DeserializeFromJson(@"..\..\..\Tests\EnercitiesDemo.json");

            var graph = CreatePoolGraph(_casePool);
            var logicCore = CreateLogicCore();
            logicCore.Graph = graph;
            gg_Area.LogicCore = logicCore;
            gg_Area.SetVerticesDrag(true);
            gg_Area.GenerateGraph(true);
        }


        GraphExample CreatePoolGraph(CasePool casePool)
        {
            var graph = new GraphExample();
            

            //List<IFeatureDetector> detectors = casePool.GetPool().Select(c => c.Value.Detector).Distinct().ToList <IFeatureDetector>();
            List<IFeatureDetector> detectors = new List<IFeatureDetector>();
            int i = 0;
            foreach(Case c in casePool)
            {
                detectors.AddRange(FindChildrenDetectors(c.Detector));
                if (i++ > 1) break;
            }
            detectors = detectors.Distinct().ToList();
            Dictionary<IFeatureDetector, DataVertex> detectorsVertexes = new Dictionary<IFeatureDetector, DataVertex>();

            i = 0;
            foreach (IFeatureDetector d in detectors)
            {
                DataVertex vert = new DataVertex() { ID = i++, Text = d.ToString() };
                detectorsVertexes.Add(d, vert);
                graph.AddVertex(vert);
            }

            foreach (IFeatureDetector d in detectors)
            {
                if (d is CompositeFeatureDetector)
                {
                    DataVertex v1 = detectorsVertexes[d];
                    List<IFeatureDetector> subDetectors = ((CompositeFeatureDetector)d).Detectors.ToList();
                    foreach(var sub in subDetectors){
                        DataVertex v2 = detectorsVertexes[sub];
                        graph.AddEdge(new DataEdge(v1, v2) { Text = "ComposedOf" });
                    }
                }
                if (d is WatcherFeatureDetector)
                {
                    DataVertex v1 = detectorsVertexes[d];
                    var sub = ((WatcherFeatureDetector)d).WatchedDetector;
                    DataVertex v2 = detectorsVertexes[sub];
                    graph.AddEdge(new DataEdge(v1, v2) { Text = "WatchingTo" });
                    
                }
            }


            //graph.AddEdge(new DataEdge(v1, v2) { Text = "Test edge v1-v2" });

            return graph;
        }


        List<IFeatureDetector> FindChildrenDetectors(IFeatureDetector det)
        {
            List<IFeatureDetector> detectors = new List<IFeatureDetector>();

            if (det is CompositeFeatureDetector)
            {
                foreach (var subDet in ((CompositeFeatureDetector)det).Detectors)
                {
                    detectors.AddRange(FindChildrenDetectors(subDet));
                }
            }
            if (det is WatcherFeatureDetector)
            {
                var subDet = ((WatcherFeatureDetector)det).WatchedDetector;
                detectors.AddRange(FindChildrenDetectors(subDet));
            }
            detectors.Add(det);
            return detectors.Distinct(new FeatureDetectorEqualityComparer()).ToList<IFeatureDetector>();
        }

        GraphExample CreateGraph()
        {
            var graph = new GraphExample();
            var v1 = new DataVertex() { ID = 1, Text = "First" };
            graph.AddVertex(v1);
            var v2 = new DataVertex() { ID = 2, Text = "Second" };
            graph.AddVertex(v2);
            var v3 = new DataVertex() { ID = 3, Text = "3rd" };
            graph.AddVertex(v3);

            graph.AddEdge(new DataEdge(v1,v2) {Text = "Test edge v1-v2"});

            return graph;
        }

        GXLogicCoreExample CreateLogicCore()
        {
            var logicCore = new GXLogicCoreExample();

            //Layout algorithm
            logicCore.DefaultLayoutAlgorithm = GraphX.LayoutAlgorithmTypeEnum.Tree;

            // Layout parameters
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(GraphX.LayoutAlgorithmTypeEnum.Tree);
            ((GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).VertexGap = 300;
            ((GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).LayerGap = 100;
            ((GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).SpanningTreeGeneration = GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SpanningTreeGeneration.BFS;
            //((GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).Direction = GraphX.GraphSharp.Algorithms.Layout.LayoutDirection.TopToBottom;
            //((GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).SpanningTreeGeneration = GraphX.GraphSharp.Algorithms.Layout.Simple.Tree.SpanningTreeGeneration.DFS;

            //No Overlaps
            logicCore.DefaultOverlapRemovalAlgorithm = GraphX.OverlapRemovalAlgorithmTypeEnum.FSA;
            //and parameters
            //logicCore.DefaultOverlapRemovalAlgorithmParams = logicCore.AlgorithmFactory.CreateOverlapRemovalParameters(GraphX.OverlapRemovalAlgorithmTypeEnum.FSA);
            //((GraphX.GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            //((GraphX.GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            //Routing algorithm
            logicCore.DefaultEdgeRoutingAlgorithm = GraphX.EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;

            return logicCore;
        }
    }
}
