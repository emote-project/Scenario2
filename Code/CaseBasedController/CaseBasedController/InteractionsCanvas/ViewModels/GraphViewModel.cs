using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EuxUtils;
using QuickGraph;
using GraphSharp.Controls;
using CaseBasedController.Detection;
using System.Windows.Input;


namespace InteractionsCanvas.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        QuickGraph.BidirectionalGraph<MyVertex, IEdge<MyVertex>> _graph;

        public BidirectionalGraph<MyVertex, IEdge<MyVertex>> Graph
        {
            get { return _graph; }
            set { 
                _graph = value;
                NotifyPropertyChanged("Graph");
            }
        }
    }

    public class MyGraphLayout : GraphLayout<MyVertex, IEdge<MyVertex>, IBidirectionalGraph<MyVertex, IEdge<MyVertex>>> { }

    public partial class MyVertex
    {

        public MyVertex()
        {
            ID = DateTime.Now.Ticks ^ (new Random()).Next();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public IFeatureDetector Detector { get; set; }

        public override string ToString()
        {
            return (Detector == null ? Name : Detector.ToString());
        }

        //VEDI QUI 
        // THIS SHOULD GO SOMEWHERE ELSE!!!!!!!!!
        public void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Check for double-click only
            //if (e.ClickCount >= 2)
            //{
                // The DataContext is my custom vertex
                var vm = (MyVertex)((System.Windows.Controls.StackPanel)sender).DataContext;
                System.Windows.MessageBox.Show(vm.Name + "");
                e.Handled = true; // Avoid further graph handling
            //}
        }

    }

    
    
}