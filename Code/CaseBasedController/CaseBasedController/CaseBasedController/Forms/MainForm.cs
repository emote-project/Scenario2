using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CaseBasedController.Behavior;
using CaseBasedController.Detection;
using CaseBasedController.Detection.Composition;
using CaseBasedController.Thalamus;
using PS.Utilities.Forms;
using System.Threading.Tasks;
using CaseBasedController.GameInfo;
using MathNet.Numerics.Random;

namespace CaseBasedController.Forms
{
    public partial class MainForm : Form
    {
        #region Fields

        private const int COL_PADDING = 20;
        private static readonly Color ActivatedBackColor = Color.LightGreen;
        private static readonly Color ExecutingBackColor = Color.Yellow;
        private static readonly Color NeutralBackColor = Color.White;

        private class BehaviourListViewItemData { public IBehavior Behaviour; public ListViewItem Item;}
        private readonly List<BehaviourListViewItemData> _behaviorListViewItems =
            new List<BehaviourListViewItemData>();

        private readonly Dictionary<Case, ListViewItem> _caseListViewItems = new Dictionary<Case, ListViewItem>();

        private MainController _mainController;

        private readonly Dictionary<IFeatureDetector, List<ListViewItem>> _detectorListViewItems =
            new Dictionary<IFeatureDetector, List<ListViewItem>>();

        private readonly object _locker = new object();
        private int _formWidth;

        private CasePoolViewerForm _viewerForm;
        private readonly EmotionalClimateForm _ecForm = new EmotionalClimateForm();

        #endregion

        #region Constructor

        public MainForm(MainController mainController)
        {
            this._mainController = mainController;
            InitializeComponent();

            //sets list views to tags in tabpages 
            this.casesTabPage.Tag = this.caseListView;
            this.detectorsTabPage.Tag = this.detectorsListView;
            this.behaviorsTabPage.Tag = this.behaviorsListView;

            this._formWidth = this.Width;
            this.ProcessCasePool();

            this._ecForm.Show();
        }

        #endregion

        #region Properties 
        
        private SortableListView CurrentListView
        {
            get { return (SortableListView) this.tabControl.SelectedTab.Tag; }
        }

        #endregion

        #region Override form methods

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this._caseListViewItems.Clear();
            this._detectorListViewItems.Clear();
            this._behaviorListViewItems.Clear();
            MainController.Exit();
            this._ecForm.Close();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //adjusts the column widths of all list views
            for (var i = 0; i < this.tabControl.TabPages.Count; i++)
                this.AdjustListViewWidth((SortableListView) this.tabControl.TabPages[i].Tag);

            this._formWidth = this.Width;
        }

        #endregion

        #region UI processing / changing

        private void AdjustListViewWidth(ListView listView)
        {
            lock (this._locker)
            {
                if (this._formWidth == 0) return;
                var scale = (double) this.Size.Width/this._formWidth;
                for (var i = 0; i < listView.Columns.Count; i++)
                    listView.Columns[i].Width = (int) (listView.Columns[i].Width*scale);

                var colsSum = listView.Columns.Cast<ColumnHeader>().Sum(column => column.Width);
                var descWidth = this.Width - colsSum - COL_PADDING;
                listView.Columns[1].Width += descWidth;
            }
        }

        private static string GetActivationLevelText(IFeatureDetector detector)
        {
            return detector.ActivationLevel.ToString("0.000");
        }

        private static string GetSubDescText(object obj, int depth, bool last)
        {
            var str = " ";
            if (depth > 0)
            {
                for (var i = 0; i < depth - 1; i++)
                    str += "         ";
                str += string.Format("{0}─ ", (last ? "└" : "├"));
            }
            return str + obj;
        }

        private void ProcessCasePool()
        {
            lock (this._locker)
            {
                if (_mainController.GetCasePool() == null) return;


                //((CasePoolViewer.UserControls.CasePoolViewer)viewer.Child).Pool = _mainController.GetCasePool();
                //((CasePoolViewer.UserControls.CasePoolViewer)viewer.Child).Draw();
                if (_viewerForm!=null) _viewerForm.SetPool(_mainController.GetCasePool());

                //attaches to events
                _mainController.GetCasePool().CaseActivationChanged -= this.OnCaseActivationChanged;
                _mainController.GetCasePool().CaseExecutionStarted -= this.OnCaseExecutionStarted;
                _mainController.GetCasePool().CaseExecutionEnded -= this.OnCaseExecutionEnded;
                _mainController.GetCasePool().CaseActivationChanged += this.OnCaseActivationChanged;
                _mainController.GetCasePool().CaseExecutionStarted += this.OnCaseExecutionStarted;
                _mainController.GetCasePool().CaseExecutionEnded += this.OnCaseExecutionEnded;

                //adds case details to grid
                var i = 0;
                this.caseListView.Items.Clear();
                this.detectorsListView.Items.Clear();
                this.behaviorsListView.Items.Clear();


                this._caseListViewItems.Clear();
                this._detectorListViewItems.Clear();
                this._behaviorListViewItems.Clear();

                foreach (var aCase in _mainController.GetCasePool())
                {
                    this.CreateCaseListViewItem(aCase, i);
                    this.CreateDetectorListViewItem(aCase.Detector, i, 0, true);
                    this.CreateBehaviorListViewItem(aCase.Behavior, i, 0, true);
                    i++;
                }

                //forces activation check
                _mainController.GetCasePool().ForceCheckCasesActivation();
            }
        }

        private void CreateCaseListViewItem(Case aCase, int caseIdx)
        {
            var caseItem = new ListViewItem(new[]
                                            {
                                                caseIdx.ToString("0"),
                                                aCase.Description,
                                                aCase.Behavior.Priority.ToString("0"),
                                                false.ToString(), "0.000", false.ToString()
                                            }) {Tag = aCase, Checked = aCase.Enabled};
            this._caseListViewItems.Add(aCase, caseItem);
            this.caseListView.Items.Add(caseItem);
        }

        private void CreateDetectorListViewItem(IFeatureDetector detector, int caseIdx, int depth, bool last)
        {
            //adds "top" detector 
            var detectorItem = new ListViewItem(new[]
                                                {
                                                    caseIdx.ToString("0"),
                                                    GetSubDescText(detector, depth, last),
                                                    false.ToString(), "0.000"
                                                }) {Tag = detector};

            //adds to dictionary, subscribe to events
            if (this._detectorListViewItems.Keys.Contains(detector))
            {
                this._detectorListViewItems[detector].Add(detectorItem);
            }
            else
            {
                this._detectorListViewItems.Add(detector, new List<ListViewItem>(){detectorItem});
            }
            this.detectorsListView.Items.Add(detectorItem);
            detector.ActivationChanged += this.OnDetectorActivationChanged;

            //checks for detector type
            if (detector is CompositeFeatureDetector)
            {
                //also adds subdetectors
                var subDetectors = ((CompositeFeatureDetector) detector);
                for (var i = 0; i < subDetectors.Count; i++)
                {
                    var subDetector = subDetectors.Detectors[i];
                    this.CreateDetectorListViewItem(subDetector, caseIdx, depth + 1, i == subDetectors.Count - 1);
                }
            }
            else if (detector is WatcherFeatureDetector)
            {
                //also adds subdetector
                var subDetector = ((WatcherFeatureDetector) detector).WatchedDetector;
                this.CreateDetectorListViewItem(subDetector, caseIdx, depth + 1, true);
            }
        }

        private void CreateBehaviorListViewItem(IBehavior behavior, int caseIdx, int depth, bool last)
        {
            //adds "top" behavior 
            var behaviorItem = new ListViewItem(new[]
                                                {
                                                    caseIdx.ToString("0"),
                                                    GetSubDescText(behavior, depth, last),
                                                    false.ToString()
                                                }) {Tag = behavior};

            //adds to dictionary, subscribe to events
            this._behaviorListViewItems.Add(new BehaviourListViewItemData() { Behaviour = behavior, Item = behaviorItem });
            this.behaviorsListView.Items.Add(behaviorItem);

            //checks for behavior type
            if (!(behavior is CompositeBehavior)) return;

            //also adds subbehaviors
            var subBehaviors = ((CompositeBehavior) behavior);
            for (var i = 0; i < subBehaviors.Count; i++)
            {
                var subDetector = subBehaviors[i];
                this.CreateBehaviorListViewItem(subDetector, caseIdx, depth + 1, i == subBehaviors.Count - 1);
            }
        }

        #endregion

        #region Case pool event handling

        private void OnDetectorActivationChanged(IFeatureDetector detector, bool activated)
        {
            if (this.detectorsListView.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this,
                    new ActivatedEventHandler(this.OnDetectorActivationChanged), new[] { detector, (object)activated },
                    this._locker);
                return;
            }
            lock (this._locker)
            {
                if (!this._detectorListViewItems.ContainsKey(detector)) return;

                var listItems = this._detectorListViewItems[detector];
                foreach (var listItem in listItems)
                {
                    listItem.SubItems[2].Text = activated.ToString();
                    listItem.SubItems[3].Text = GetActivationLevelText(detector);
                    listItem.BackColor = activated ? ActivatedBackColor : NeutralBackColor;
                }
            }
        }

        private void OnCaseActivationChanged(Case item, bool activated)
        {
            if (this.caseListView.InvokeRequired || this.behaviorsListView.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this,
                    new CaseActivationEventHandler(this.OnCaseActivationChanged), new[] { item, (object)activated },
                    this._locker);
                return;
            }
            lock (this._locker)
            {
                //fills case view item info
                if (!this._caseListViewItems.ContainsKey(item)) return;

                var listItem = this._caseListViewItems[item];
                listItem.SubItems[3].Text = activated.ToString();
                listItem.SubItems[4].Text = GetActivationLevelText(item.Detector);
                listItem.BackColor = activated ? ActivatedBackColor : NeutralBackColor;

                //fills behavior view item info
                if (this._behaviorListViewItems.Find(li => li.Behaviour.Equals(item.Behavior)) == null) return;

                List<BehaviourListViewItemData> lvis = this._behaviorListViewItems.FindAll(li => li.Behaviour.Equals(item.Behavior));
                foreach (var lvi in lvis)
                {
                    listItem = lvi.Item;
                    listItem.SubItems[2].Text = activated.ToString();
                    listItem.BackColor = activated ? ActivatedBackColor : NeutralBackColor;
                }
            }
        }

        private void OnCaseExecutionStarted(Case item)
        {
            if (this.caseListView.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this,
                    new CaseEventHandler(this.OnCaseExecutionStarted), new object[] {item},
                    this._locker);
                return;
            }

            lock (this._locker) this.OnCaseExecutionChanged(item, true);
        }

        private void OnCaseExecutionEnded(Case item)
        {
            if (this.caseListView.InvokeRequired)
            {
                FormsUtil.InvokeWhenPossible(this,
                    new CaseEventHandler(this.OnCaseExecutionEnded), new object[] {item},
                    this._locker);
                return;
            }

            lock (this._locker) this.OnCaseExecutionChanged(item, false);
        }

        private void OnCaseExecutionChanged(Case item, bool started)
        {
            if (!this._caseListViewItems.ContainsKey(item)) return;

            var listItem = this._caseListViewItems[item];
            listItem.BackColor = started ? ExecutingBackColor : NeutralBackColor;
            listItem.Font = new Font(listItem.Font, started ? FontStyle.Bold : FontStyle.Regular);
            listItem.SubItems[5].Text = started.ToString();
        }

        #endregion

        #region Forms event handling

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadToolStripMenuItemClick(object sender, EventArgs e)
        {
            lock (this._locker)
            {
                this.openFileDialog.FileName = ControllerClient.CASE_POOL_FILE;
                if (this.openFileDialog.ShowDialog() != DialogResult.OK) return;
                _mainController.SetCasePool(_mainController.LoadCasePool(this.openFileDialog.FileName));
                //loads case pool from file, report to client and refreshes grid list
                //var casePool = CasePool.DeserializeFromJson(this.openFileDialog.FileName);
                //if (casePool != null) this._mainController.SetCasePool(casePool);
                //this.ProcessCasePool();
            }
        }

        private async void loadClassifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load the case pool that will feed classifier";
            ofd.Filter = "Case Pool | *.json";
            string casePoolPath = null;
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                casePoolPath = ofd.FileName;
            }
            else 
            { 
                return;
            }

            ofd = new OpenFileDialog();
            ofd.Title = "Load classifier file";
            ofd.Filter = "Weka ARFF file | *.arff";
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                await _mainController.LoadClassifierAsync(ofd.FileName, casePoolPath);
            }
        }

        private void AutoSortItemsMenuClick(object sender, EventArgs e)
        {
            lock (this._locker)
            {
                var autoSort =
                    this.autoSortItemsToolStripMenuItem.Checked = !this.autoSortItemsToolStripMenuItem.Checked;
                this.caseListView.AutoSort =
                    this.detectorsListView.AutoSort = this.behaviorsListView.AutoSort = autoSort;
            }
        }

        private void showViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!showViewerToolStripMenuItem.Checked)
            {
                if (_viewerForm == null) _viewerForm = new CasePoolViewerForm();
                _viewerForm.Show();
                _viewerForm.SetPool(_mainController.GetCasePool());
            }
            else
            {
                _viewerForm.Close();
            }
        }

        #endregion

        #region Interface Information Editing Methods

        public void UpdateCasePool()
        {
            this.Invoke(new Action(() =>
            {
                this.ProcessCasePool();
            }));
        }


        #endregion

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


        


        

    }
}