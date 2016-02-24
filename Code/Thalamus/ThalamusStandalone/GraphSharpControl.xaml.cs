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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GraphSharp.Sample;
using QuickGraph;
using GraphSharp;
using GraphSharp.Controls;
using QuickGraph.Serialization;

namespace Thalamus
{

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GraphSharpControl : UserControl
    {
        //Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
        Dictionary<string, TGVertex> mainNodes = new Dictionary<string, TGVertex>();
        Dictionary<string, List<string>> mainNodesLinks = new Dictionary<string, List<string>>();
        Dictionary<string, KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>>> connectionPoints = new Dictionary<string, KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>>>();
        private bool graphCommited = false;
        public void ClearNodes()
        {
            //nodes = new Dictionary<string, List<string>>();
            graphCommited = false;
            mainNodes = new Dictionary<string, TGVertex>();
            connectionPoints = new Dictionary<string, KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>>>();
        }

        public void CommitGraph()
        {
            graphCommited = true;
        }

        public void AddMainNode(string id, List<string> connectedNodes)
        {
            mainNodes[id] = new TGVertex(id);
            mainNodesLinks[id] = connectedNodes;
        }

        public void AddComposedNode(string id, List<string> inPoints, List<string> outPoints)
        {
            //nodes[id] = connectionsPoints;
            if (!mainNodes.ContainsKey(id)) mainNodes[id] = new TGVertex(id);
            /*if (!connectionPoints.ContainsKey(id)) connectionPoints[id] = new KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>>(new Dictionary<string, TGVertex>(), new Dictionary<string, TGVertex>());

            foreach (string point in inPoints)
            {
                connectionPoints[id].Key[point] = new TGVertex(point);
            }

            foreach (string point in outPoints)
            {
                connectionPoints[id].Value[point] = new TGVertex(point);
            }*/
        }

        public void RelayoutGraph()
        {
            if (!graphCommited || graphLayout.Graph == null) return;
            graphLayout.Relayout();
        }

        public void RenderGraph()
        {
            if (!graphCommited) return;
            var g = new ThalamusGraph();
            foreach (string nodeId in mainNodes.Keys)
            {
                g.AddVertex(mainNodes[nodeId]);
                /*if (connectionPoints.ContainsKey(nodeId))
                {
                    KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>> points = connectionPoints[nodeId];
                    foreach (KeyValuePair<string, TGVertex> point in points.Key)
                    {
                        //g.AddChildVertex(mainNodes[nodeId], point.Value);
                    }
                    foreach (KeyValuePair<string, TGVertex> point in points.Value)
                    {
                        //g.AddChildVertex(mainNodes[nodeId], point.Value);
                    }
                }*/
            }


            foreach (string nodeId in mainNodes.Keys)
            {
                foreach (string connectedNode in mainNodesLinks[nodeId])
                {
                    if (mainNodes.ContainsKey(connectedNode)) g.AddEdge(new TGEdge(mainNodes[connectedNode], mainNodes[nodeId]));
                }

                /*
                if (connectionPoints.ContainsKey(nodeId))
                {
                    KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>> points = connectionPoints[nodeId];
                    foreach (KeyValuePair<string, TGVertex> point in points.Key)
                    {
                        foreach (string otherNodeId in mainNodes.Keys)
                        {
                            if (otherNodeId != nodeId)
                            {
                                KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>> otherPoints = connectionPoints[otherNodeId];
                                foreach (KeyValuePair<string, TGVertex> otherPoint in otherPoints.Value)
                                {
                                    if (point.Key == otherPoint.Key) g.AddEdge(new TGEdge(otherPoint.Value, point.Value));
                                }
                            }
                        }
                    }

                    foreach (KeyValuePair<string, TGVertex> point in points.Value)
                    {
                        foreach (string otherNodeId in mainNodes.Keys)
                        {
                            if (otherNodeId != nodeId)
                            {
                                KeyValuePair<Dictionary<string, TGVertex>, Dictionary<string, TGVertex>> otherPoints = connectionPoints[otherNodeId];
                                foreach (KeyValuePair<string, TGVertex> otherPoint in otherPoints.Key)
                                {
                                    if (point.Key == otherPoint.Key) g.AddEdge(new TGEdge(point.Value, otherPoint.Value));
                                }
                            }
                        }
                    }
                }*/
            }

            graphLayout.LayoutMode = LayoutMode.Automatic;
            graphLayout.LayoutAlgorithmType = "EfficientSugiyama";
            graphLayout.OverlapRemovalConstraint = AlgorithmConstraints.Automatic;
            graphLayout.OverlapRemovalAlgorithmType = "FSA";
            graphLayout.HighlightAlgorithmType = "Simple";
            graphLayout.Graph = g;
            graphLayout.UpdateLayout();
        }
        private ThalamusGraphViewModel vm;

        public GraphSharpControl()
        {
            InitializeComponent();
            vm = new ThalamusGraphViewModel(this);
            this.DataContext = vm;

            /*var g = new CompoundGraph<object, IEdge<object>>();

            */
        }
 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("button clicked");
            //vm.NotifyPropertyChanged("Graph");
        }
    }
}
