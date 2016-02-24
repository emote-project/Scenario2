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

namespace Thalamus
{

    public class TGVertex
    {
        public string ID { get; private set; }
        public TGVertex(string id)
        {
            ID = id;
        }
        public override string ToString()
        {
            return ID;
        }
    }

    public class TGEdge : Edge<TGVertex>, INotifyPropertyChanged
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

        public TGEdge(string id, TGVertex source, TGVertex target)
            : base(source, target)
        {
            ID = id;

        }

        public TGEdge(TGVertex source, TGVertex target)
            : base(source, target)
        {
            ID = "";
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

    public class ThalamusGraphLayout : GraphLayout<TGVertex, Edge<TGVertex>, ThalamusGraph> { }
    public class ThalamusGraph : BidirectionalGraph<TGVertex, Edge<TGVertex>>
    {
        public ThalamusGraph() : base(true) { }
    }

    public class ThalamusGraphViewModel : INotifyPropertyChanged
    {
        #region Data

        private string layoutAlgorithmType;
        private ThalamusGraph graph;
        private GraphSharpControl graphControl;
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
            LayoutAlgorithmType = "EfficientSugiyama";
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

        public ThalamusGraph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
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
