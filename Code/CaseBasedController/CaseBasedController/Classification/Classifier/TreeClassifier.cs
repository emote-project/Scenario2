using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FPTree.Comparers;
using FPTree.Items;
using FPTree.Measures;
using FPTree.Supports;
using FPTree.Trees;
using FPTree.Visualization;
using Newtonsoft.Json;
using PS.Utilities.IO;
using PS.Utilities.Serialization;

namespace Classification.Classifier
{
    [Serializable]
    public class TreeClassifier : IClassifier
    {
        #region Constructor

        public TreeClassifier()
        {
            this.Labels = new List<string>();
            this.LabelTrees = new List<IFPTree>();
            this.NotLabelTrees = new List<IFPTree>();
            this.Cohesions = new SerializableDictionary<IFPTree, double>();
            this.MinFrequency = 0.2f;
        }

        #endregion

        #region IClassifier Members

        public void Dispose()
        {
            this._compactTrees.Clear();
            this._labelIdxs.Clear();
            this.Labels.Clear();
            this.Cohesions.Clear();
            ClearTrees(this.LabelTrees);
            ClearTrees(this.NotLabelTrees);
            if (this.GeneralTree != null) this.GeneralTree.Dispose();
            this.NumInstances = 0;
        }

        #endregion

        #region Fields

        private const string BINARY_EXTENSION = "bin";
        private const string GENERAL_TREE_ID = "General";
        private const string IMAGE_TYPE = "pdf";
        private const string ACTIVE_FEATURE_STR = "1";

        private readonly Dictionary<IFPTree, IFPTree> _compactTrees = new Dictionary<IFPTree, IFPTree>();
        private readonly ItemStringComparer _itemComparer = new ItemStringComparer();
        private readonly Item _itemCreator = new Item("creator");

        [JsonProperty("BehaviorIdxs")] private readonly Dictionary<string, int> _labelIdxs =
            new Dictionary<string, int>();

        private readonly ISupport _support = new JaccardSupport();

        #endregion

        #region Properties

        [JsonProperty]
        public List<IFPTree> LabelTrees { get; private set; }

        [JsonProperty]
        public List<IFPTree> NotLabelTrees { get; private set; }

        [JsonProperty]
        public IFPTree GeneralTree { get; private set; }

        [JsonProperty]
        public SerializableDictionary<IFPTree, double> Cohesions { get; private set; }

        [JsonProperty]
        public double MinFrequency { get; set; }

        [JsonIgnore]
        public uint Count
        {
            get { return (uint) this.LabelTrees.Count; }
        }

        [JsonProperty]
        public uint NumInstances { get; private set; }

        [JsonIgnore]
        public List<string> Labels { get; private set; }

        #endregion

        #region Serialization methods

        public void Load(string filePath)
        {
            //start clean
            this.Dispose();

            //loads classifier from json, copy all elements
            var classifier = Deserialize(filePath, true);
            this.LabelTrees = classifier.LabelTrees;
            this.NotLabelTrees = classifier.NotLabelTrees;
            this.GeneralTree = classifier.GeneralTree;
            this.MinFrequency = classifier.MinFrequency;
            this.NumInstances = classifier.NumInstances;
            this.Cohesions = classifier.Cohesions;

            foreach (var label in classifier._labelIdxs.Keys)
                this.Labels.Add(label);
        }

        public void Save(string filePath)
        {
            this.Save(filePath, true);
        }

        public void Save(string filePath, bool binary)
        {
            if (binary) filePath = PathUtil.ReplaceExtension(filePath, BINARY_EXTENSION);
            this.SerializeJsonFile(filePath, JsonUtil.PreserveReferencesSettings, Formatting.Indented, true, binary);
        }

        public static TreeClassifier Deserialize(string filePath, bool binary)
        {
            if (binary) filePath = PathUtil.ReplaceExtension(filePath, BINARY_EXTENSION);
            return JsonUtil.DeserializeJsonFile<TreeClassifier>(
                filePath, JsonUtil.PreserveReferencesSettings, true, binary);
        }

        #endregion

        #region Public methods

        public ClassificationResult Classify(IDictionary<string, string> featuresVector)
        {
            return this.Classify(featuresVector, null);
        }

        public ClassificationResult Classify(IDictionary<string, string> featuresVector, string label)
        {
            var transaction = this.GetTransaction(featuresVector).ToArray();

            var labelSupports = new Dictionary<string, double>();
            var notLabelSupports = new Dictionary<string, double>();
            var supportDiff = new Dictionary<string, double>();
            for (var i = 0; i < this.Labels.Count; i++)
            {
                var labelSupport = GetWeightedSupport(transaction, this.LabelTrees[i]);
                var notLabelSupport = GetWeightedSupport(transaction, this.NotLabelTrees[i]);

                labelSupports.Add(this.Labels[i], labelSupport);
                notLabelSupports.Add(this.Labels[i], notLabelSupport);
                supportDiff.Add(this.Labels[i], labelSupport - notLabelSupport);
            }

            var sortedLabelSupports = labelSupports.OrderBy(kvp => -kvp.Value).ToList();
            var sortedNotLabelSupports = notLabelSupports.OrderBy(kvp => kvp.Value).ToList();
            var sortedSupportsDiff = supportDiff.OrderBy(kvp => -kvp.Value).ToList();

            var bestLabel = sortedLabelSupports[0].Key;
            var bestNotLabel = sortedNotLabelSupports[0].Key;

            //if (bestLabel.Contains("IFMLSpeech:robot"))
            //    //if (bestLabel.Contains(":robot."))
            //{
            //    if (bestLabel.Equals(bestNotLabel))
            //        return new ClassificationResult(labelSupports[bestLabel], bestLabel);

            //    bestLabel = sortedLabelSupports[1].Key;
            //    return new ClassificationResult(labelSupports[bestLabel], bestLabel);
            //}

            if (bestLabel.Equals(bestNotLabel) && bestLabel.Equals(sortedSupportsDiff[0].Key))
                return new ClassificationResult(labelSupports[bestLabel], bestLabel);
            return new ClassificationResult(0, null);

            return new ClassificationResult(labelSupports[bestLabel],
                (bestLabel.Equals(bestNotLabel) ? bestLabel : null));

            var maxSupport = double.MinValue;
            var minSupport = double.MaxValue;
            string maxLabel = null;
            string minLabel = null;
            for (var i = 0; i < sortedLabelSupports.Count; i++)
            {
                var labelSupport = sortedLabelSupports[i].Value;
                if (labelSupport > maxSupport)
                {
                    maxSupport = labelSupport;
                    maxLabel = sortedLabelSupports[i].Key;
                }
                var notLabelSupport = sortedNotLabelSupports[i].Value;
                if (notLabelSupport < minSupport)
                {
                    minSupport = notLabelSupport;
                    minLabel = sortedNotLabelSupports[i].Key;
                }
            }


            if (maxLabel != minLabel)
            {
                var i = 0;
                i++;
            }

            return new ClassificationResult(
                //maxSupport, (maxLabel != null && maxLabel.Equals(minLabel)) ? maxLabel : null);
                maxSupport, minLabel);
        }

        public void Train(IDictionary<string, string> featuresVector, string label)
        {
            this.NumInstances++;

            //just check to create new trees for the label
            if (!this.Labels.Contains(label))
                this.AddLabel(label);

            //inserts transaction into respective label tree
            var transaction = this.GetTransaction(featuresVector).ToArray();
            this.GetLabelTree(label).InsertTransaction(transaction);

            //inserts transaction into other labels' not trees
            for (var i = 0; i < this.NotLabelTrees.Count; i++)
                if (i != this._labelIdxs[label])
                    this.NotLabelTrees[i].InsertTransaction(transaction);

            //inserts also into general tree
            if (this.GeneralTree == null)
                this.GeneralTree = this.CreateTree(GENERAL_TREE_ID);
            this.GeneralTree.InsertTransaction(transaction);
        }

        public void PostProcess()
        {
            //prunes trees
            Console.WriteLine("Pruning {0} trees...", this.Count);
            this.PruneTrees();

            //compact trees
            Console.WriteLine("Compacting {0} trees...", this.Count);
            this.CompactTrees();

            //gets cohesion measures for all trees
            var dispersion = new DispersionMeasure();
            this.GetCohesions(dispersion, this.LabelTrees);
            this.GetCohesions(dispersion, this.NotLabelTrees);
        }

        public void PrintResults(string baseDir)
        {
            //print all trees to pdf
            this.PrintTrees(baseDir, this.LabelTrees);
            this.PrintTrees(baseDir, this.NotLabelTrees, "Not");
            PrintTree(this.GeneralTree, baseDir, GENERAL_TREE_ID);
        }

        #endregion

        #region Get methods

        private IFPTree GetLabelTree(string label)
        {
            return this.GetTree(label, this.LabelTrees);
        }

        private IFPTree GetNotLabelTree(string label)
        {
            return this.GetTree(label, this.NotLabelTrees);
        }

        private IFPTree GetTree(string label, List<IFPTree> trees)
        {
            return trees[this.GetLabelIndex(label)];
        }

        private int GetLabelIndex(string label)
        {
            return this._labelIdxs[label];
        }

        #endregion

        #region Private methods

        private void AddLabel(string label)
        {
            //creates trees for new label
            this._labelIdxs.Add(label, this._labelIdxs.Count);
            this.LabelTrees.Add(this.CreateTree(label));
            this.NotLabelTrees.Add(this.CreateTree(label));
            this.Labels.Add(label);
        }

        private void GetCohesions(ITreeMeasure dispersion, IEnumerable<IFPTree> trees)
        {
            foreach (var tree in trees)
                this.Cohesions.Add(tree, 1d - dispersion.Calculate(this._compactTrees[tree]));
        }

        private IEnumerable<string> GetTransaction(IDictionary<string, string> featuresVector)
        {
            return (from featureState in featuresVector
                where featureState.Value.Equals(ACTIVE_FEATURE_STR)
                select featureState.Key).ToList();
        }

        private double GetWeightedSupport(IEnumerable<string> transaction, IFPTree tree)
        {
            var itemSet = tree.GetItemSet(transaction);
            return this._support.Calculate(itemSet, tree)
                   *this.Cohesions[tree]
                   *((double) tree.TransactionCount/this.NumInstances);
        }

        private void PrintTrees(string baseDir, List<IFPTree> trees, string prefix = "")
        {
            for (var i = 0; i < trees.Count; i++)
                PrintTree(trees[i], baseDir, prefix + this.Labels[i]);
        }

        private static void PrintTree(IFPTree tree, string baseDir, string fileName)
        {
            var filePath =
                Path.GetFullPath(String.Format("{0}/{1}", baseDir, PathUtil.ReplaceInvalidChars(fileName, '-')));
            FPTreePrinter.Print(tree, filePath, IMAGE_TYPE);
        }

        private static void ClearTrees(List<IFPTree> labelTrees)
        {
            foreach (var tree in labelTrees)
                tree.Dispose();
            labelTrees.Clear();
        }

        private void PruneTrees()
        {
            PruneTrees(this.LabelTrees);
            PruneTrees(this.NotLabelTrees);
            this.GeneralTree.Prune();
        }

        private static void PruneTrees(IEnumerable<IFPTree> labelTrees)
        {
            foreach (var tree in labelTrees)
            {
                tree.Prune();
                tree.RootNode.RecalcMaxDepth();
            }
        }

        private void CompactTrees()
        {
            CompactTrees(this.LabelTrees);
            CompactTrees(this.NotLabelTrees);
            this.GeneralTree = CompactTree(this.GeneralTree);
        }

        private void CompactTrees(IEnumerable<IFPTree> labelTrees)
        {
            foreach (var tree in labelTrees)
                this._compactTrees[tree] = this.CompactTree(tree);
        }

        private CompactTree CompactTree(IFPTree tree)
        {
            var compact = new CompactTree(tree, this._itemComparer, this._itemCreator) {Name = tree.Name};
            compact.Compact();
            return compact;
        }

        private IFPTree CreateTree(string label)
        {
            return new FPTree.Trees.FPTree(this._itemComparer, this._itemCreator)
                   {
                       Name = label,
                       MinFrequency = this.MinFrequency
                   };
        }

        #endregion
    }
}