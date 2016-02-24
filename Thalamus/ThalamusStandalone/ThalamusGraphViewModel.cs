/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphSharp.Controls;
using QuickGraph;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections;
using System.Windows.Shapes;

namespace Thalamus
{

    public class FastFadeOutTransition : FadeTransition
    {
        public FastFadeOutTransition()
            : base(0.0, 0.0, 1)
        {

        }
    }

    public class FastFadeInTransition : FadeTransition
    {
        public FastFadeInTransition()
            : base(1.0, 1.0, 1)
        {

        }
    }

    public class TGVertex
    {
        public string ID { get; private set; }
        public bool IsReturnButton { get; set; }
        public bool IsSelected { get; set; }
        public TGVertex(string id)
        {
            ID = id;
            IsReturnButton = false;
        }
        public override string ToString()
        {
            return ID;
        }
    }

    public class TGTag
    {
        public string ID { get; set; }
        public TGTag(string id)
        {
            ID = id;
        }
        public override string ToString()
        {
            return ID;
        }
    }

    public class EdgeColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TGEdge : TaggedEdge<TGVertex, TGTag>, INotifyPropertyChanged
    {
        private string id;
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }


        public Color EdgeColor { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public TGEdge(TGVertex source, TGVertex target, string id = "")
            : base(source, target, new TGTag(id))
        {
            ID = id;
            IsSelected = false;
            EdgeColor = Colors.White;
        }

        public override string ToString()
        {
            return string.Format("{0}", ID);
        }


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }

    public class EdgeLabelControl : TextBlock
    {

        TransformGroup localTransform = new TransformGroup();
        public bool Recalculate = true;
        public bool Rearrange = true;
        public bool Repaint = true;

        private Point lastPoint1 = new Point(0, 0);
        private Point lastPoint2 = new Point(0, 0);

        private VertexControl lastSource = null;
        private VertexControl lastTraget = null;

        public TransformGroup LocalTransform
        {
            get { return localTransform; }
        }

        public EdgeLabelControl()
        {
            LayoutUpdated += EdgeLabelControl_LayoutUpdated;
        }

        private EdgeControl GetEdgeControl(DependencyObject parent)
        {
            //if (this.Content != null) Console.WriteLine(this.Content.GetType());
            //else Console.WriteLine("null content");
            while (parent != null)
                if (parent is EdgeControl)
                    return (EdgeControl)parent;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            return null;
        }

        private static double GetAngleBetweenPoints(Point point1, Point point2)
        {
            return Math.Atan2(point1.Y - point2.Y, point2.X - point1.X);
        }

        private static double GetDistanceBetweenPoints(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        private static double GetLabelDistance(double edgeLength)
        {
            return edgeLength/2 + edgeLength / 4;  // set the label halfway the length of the edge
        }

        private static bool IsOutEdge(EdgeControl edge)
        {
            return (edge.Edge as TGEdge).Source.ID == (edge.Source.DataContext as ThalamusGraphViewModel).GraphControl.HooveredNode;
        }

        private void EdgeLabelControl_LayoutUpdated(object sender, EventArgs e)
        {

            if (!IsLoaded)
                return;
            var edgeControl = GetEdgeControl(VisualParent);
            if (edgeControl == null)
                return;
            var source = edgeControl.Source;
            var p1 = new Point(GraphCanvas.GetX(source), GraphCanvas.GetY(source));
            var target = edgeControl.Target;
            var p2 = new Point(GraphCanvas.GetX(target), GraphCanvas.GetY(target));

            if (lastSource == source && lastTraget == target) Repaint = false;
            else Repaint = true;

            if (lastPoint1 == p1 && lastPoint2 == p2) Recalculate = false;
            else {
                Recalculate = true;
                Rearrange = true;
                lastPoint1 = p1;
                lastPoint2 = p2;
            }
            //Console.WriteLine("Layout oupdated {0} -> {1}", (source.Vertex as TGVertex).ID, (target.Vertex as TGVertex).ID);

            double edgeLength;
            var routePoints = edgeControl.RoutePoints;
            if (routePoints == null)
                // the edge is a single segment (p1,p2)
                edgeLength = GetLabelDistance(GetDistanceBetweenPoints(p1, p2));
            else
            {
                // the edge has one or more segments
                // compute the total length of all the segments
                edgeLength = 0;
                for (int i = 0; i <= routePoints.Length; ++i)
                    if (i == 0)
                        edgeLength += GetDistanceBetweenPoints(p1, routePoints[0]);
                    else if (i == routePoints.Length)
                        edgeLength += GetDistanceBetweenPoints(routePoints[routePoints.Length - 1], p2);
                    else
                        edgeLength += GetDistanceBetweenPoints(routePoints[i - 1], routePoints[i]);
                // find the line segment where the half distance is located
                edgeLength = GetLabelDistance(edgeLength);
                Point newp1 = p1;
                Point newp2 = p2;
                for (int i = 0; i <= routePoints.Length; ++i)
                {
                    double lengthOfSegment;
                    if (i == 0)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = p1, newp2 = routePoints[0]);
                    else if (i == routePoints.Length)
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[routePoints.Length - 1], newp2 = p2);
                    else
                        lengthOfSegment = GetDistanceBetweenPoints(newp1 = routePoints[i - 1], newp2 = routePoints[i]);
                    if (lengthOfSegment >= edgeLength)
                        break;
                    edgeLength -= lengthOfSegment;
                }
                // redefine our edge points
                p1 = newp1;
                p2 = newp2;
            }
            // align the point so that it  passes through the center of the label content
            var p = p1;
            var desiredSize = DesiredSize;
            p.Offset(-desiredSize.Width / 2, -desiredSize.Height / 2);

            // move it "edgLength" on the segment
            var angleBetweenPoints = GetAngleBetweenPoints(p1, p2);
            p.Offset(edgeLength * Math.Cos(angleBetweenPoints), -edgeLength * Math.Sin(angleBetweenPoints));

            if (Repaint)
            {
                if (ThalamusGraph.MainGraph != null && ThalamusGraph.MainGraph.Vertices.Contains(edgeControl.Source.Vertex))
                {
                    if ((IsOutEdge(edgeControl) && (edgeControl.Parent as ThalamusGraphLayout).IsSemiHighlightedEdge((TGEdge)edgeControl.Edge)) || (edgeControl.Edge as TGEdge).Source.ID == (edgeControl.Source.DataContext as ThalamusGraphViewModel).GraphControl.SelectedNode)
                    {
                        Visibility = System.Windows.Visibility.Visible;
                        Foreground = new SolidColorBrush(Colors.Yellow);
                        (edgeControl.Edge as TGEdge).IsSelected = true;
                    }
                    else if ((!IsOutEdge(edgeControl) && (edgeControl.Parent as ThalamusGraphLayout).IsSemiHighlightedEdge((TGEdge)edgeControl.Edge)) || (edgeControl.Edge as TGEdge).Target.ID == (edgeControl.Source.DataContext as ThalamusGraphViewModel).GraphControl.SelectedNode)
                    {
                        Visibility = System.Windows.Visibility.Visible;
                        Foreground = new SolidColorBrush(Colors.Turquoise);
                        (edgeControl.Edge as TGEdge).IsSelected = true;
                    }
                    else
                    {
                        Visibility = System.Windows.Visibility.Hidden;
                        Foreground = new SolidColorBrush(Colors.White);
                        (edgeControl.Edge as TGEdge).IsSelected = false;
                    }
                }
            }

            if (Recalculate)
            {
                try
                {
                    localTransform = new TransformGroup();
                    double angle = -GetAngleBetweenPoints(p1, p2) * (180 / Math.PI);
                    if (angle < -90 || angle > 90) angle += 180;

                    TranslateTransform t = new TranslateTransform(0, -DesiredSize.Height/2);
                    localTransform.Children.Add(t);
                    
                    RotateTransform r = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = angle };
                    localTransform.Children.Add(r);

                    ScaleTransform s = new ScaleTransform(0.5, 0.5);
                    localTransform.Children.Add(s);
                    
                    
                }
                catch { }
            }

            Arrange(new Rect(p, desiredSize));
        }
    }

    public class ThalamusGraphLayout : GraphLayout<TGVertex, TGEdge, ThalamusGraph> {
    }
    public class ThalamusGraph : BidirectionalGraph<TGVertex, TGEdge>
    {
        public static ThalamusGraph MainGraph = null;

        public ThalamusGraph() : base(true) { }
    }

    public class TGHighlightParameters : GraphSharp.Algorithms.Highlight.HighlightParameterBase
    {
    }


    public class ThalamusGraphViewModel : INotifyPropertyChanged
    {
        #region Data

        private string layoutAlgorithmType;
        private GraphSharpControl graphControl;

        public GraphSharpControl GraphControl
        {
            get { return graphControl; }
        }
        private List<String> layoutAlgorithmTypes = new List<string>();
        #endregion

        #region Ctor

        public ThalamusGraphViewModel(GraphSharpControl graphControl)
        {
            this.graphControl = graphControl;
            //Add Layout Algorithm Types
            layoutAlgorithmTypes.Add("BoundedFR");
            layoutAlgorithmTypes.Add("Circular");
            layoutAlgorithmTypes.Add("CompoundFDP");
            layoutAlgorithmTypes.Add("EfficientSugiyama");
            layoutAlgorithmTypes.Add("FR");
            layoutAlgorithmTypes.Add("ISOM");
            layoutAlgorithmTypes.Add("KK");
            layoutAlgorithmTypes.Add("LinLog");
            layoutAlgorithmTypes.Add("Tree");

            //Pick a default Layout Algorithm Type
            LayoutAlgorithmType = "KK";
            graphControl.graphLayout.LayoutUpdated += graphControl_LayoutUpdated;
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void RearrangeTags()
        {
            foreach (UIElement u in this.graphControl.graphLayout.Children)
            {
                if (u is EdgeControl)
                {
                    IEnumerable<EdgeLabelControl> labels = FindVisualChildren<EdgeLabelControl>(u);
                    foreach (EdgeLabelControl e in labels)
                    {
                        if (e.Rearrange)
                        {
                            //Console.WriteLine(e.Text);
                            e.RenderTransformOrigin = new Point(0.5, 0.5);
                            e.RenderTransform = e.LocalTransform;
                        }
                    }

                    /*IEnumerable<Path> paths = FindVisualChildren<Path>(u);
                    foreach (Path p in paths)
                    {
                        p.RenderTransform = new TranslateTransform(5, 0);
                    }*/
                }
            }
        }

        void graphControl_LayoutUpdated(object sender, EventArgs e)
        {
            RearrangeTags();
        }
        #endregion
 
        public List<String> LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }
        }
        public string LayoutAlgorithmType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("LayoutAlgorithmType");
                graphControl.RelayoutGraph();
            }
        }

        public List<String> FilterTypes
        {
            get { return layoutAlgorithmTypes; }
        }
        public string SelectedFilterType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("SelectedFilterType");
                graphControl.RelayoutGraph();
            }
        }

        public List<String> EventTypes
        {
            get { return layoutAlgorithmTypes; }
        }
        public string SelectedEventType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("SelectedEventType");
                graphControl.RelayoutGraph();
            }
        }

        public ThalamusGraph Graph
        {
            get { return ThalamusGraph.MainGraph; }
            set
            {
                ThalamusGraph.MainGraph = value;
                NotifyPropertyChanged("Graph");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs pce = new PropertyChangedEventArgs(info);
                PropertyChanged(this, pce);
            }
        }
    }
}
