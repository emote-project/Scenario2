using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PhysicalSpaceManager;
using EmoteCommonMessages;
using EmoteEvents.ComplexData;
using Skene.Utterances;
using System.Collections.Specialized;
using System.Threading;

namespace Skene
{
    public partial class MainWindow : Form
    {
        MainWindowController _controller;
        UtteranceLibrary selectedUtteranceLibrary = null;
        UtteranceValidationSet selectedUVS = null;

        bool dontRefresh = false;

        public MainWindow(string characterName)
        {
            dontRefresh = true;
            InitializeComponent();
            dontRefresh = false;
            _controller = new MainWindowController(this, characterName);
            ReloadSavedSetups();
            _controller.SetupListChanged += delegate(object sender, EventArgs e)
            {
                ReloadSavedSetups();
                Console.WriteLine("Setup list changed");
            };
            _controller.ActiveSetupChanged += delegate(object sender, EventArgs e)
            {
                lblActiveSetup.Text =  (_controller.GetActiveSetup()!=null?_controller.GetActiveSetup()._name:"none");
            };
            _controller.LoadSetups();
            _controller.Client.GazeManager.GazeTargetChanged += (GazeManager.GazeTargetChangedHandler)((gazeTarget) =>
            {
                this.Invoke((MethodInvoker)(() => {
                    if (dontUpdate) return;
                    dontUpdate = true;
                    cmbGazeTarget.SelectedIndex = cmbGazeTarget.FindStringExact(gazeTarget.ToString());
                    dontUpdate = false;
                }));
            });
            _controller.Client.TargetsChanged += (SkeneClient.TargetsChangedHandler)(() =>
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        RefreshInfoTargets();
                    }));
                });
            _controller.Client.UtteranceState += (SkeneClient.UtteranceStateHandler)((state, utterance) => 
            {
                if (state == SkeneClient.UtteranceStates.Performing)
                {
                    (new Thread(new ThreadStart(() =>
                    {
                        lblUtteranceStatus.Invoke((MethodInvoker)(() =>
                        {
                            if (utterance != null)
                            {
                                if (utterance.IsQuestion) lblCurrentUtteranceLabel.Text = "Current Question:";
                                else lblCurrentUtteranceLabel.Text = "Current Utterance:";
                            }

                            lblUtteranceStatus.BackColor = Color.ForestGreen;
                            lblUtteranceStatus.Refresh();
                            Thread.Sleep(100);
                            if (lblUtteranceStatus.BackColor != Color.DarkSalmon)
                            {
                                lblUtteranceStatus.BackColor = SystemColors.Control;
                                lblUtteranceStatus.Refresh();
                            }
                        }));
                    }))).Start();
                }

                if (state == SkeneClient.UtteranceStates.Canceled)
                {
                    (new Thread(new ThreadStart(() =>
                    {
                        lblUtteranceStatus.Invoke((MethodInvoker)(() =>
                        {
                            lblUtteranceStatus.Text = "[" + state.ToString() + "]";
                            lblUtteranceStatus.BackColor = Color.Red;
                            Thread.Sleep(1000);
                            lblUtteranceStatus.BackColor = SystemColors.Control;
                        }));
                    }))).Start();
                }
                lblUtteranceStatus.Invoke((MethodInvoker)(() =>
                {
                    if (state == SkeneClient.UtteranceStates.NVBWaiting)
                    {
                        lblUtteranceStatus.BackColor = Color.CornflowerBlue;
                    }
                    if (state == SkeneClient.UtteranceStates.QuestionWaiting)
                    {
                        lblUtteranceStatus.BackColor = Color.DarkSalmon;
                    }
                    if (state == SkeneClient.UtteranceStates.StandBy && lblUtteranceStatus.BackColor != Color.DarkSalmon)
                    {
                        lblUtteranceStatus.BackColor = SystemColors.Control;
                    }
                    /*if (lblUtteranceStatus.BackColor != Color.Red)
                    {*/
                    lblUtteranceStatus.Text = "[" + state.ToString() + "]";
                    if (state == SkeneClient.UtteranceStates.StandBy) lblCurrentUtterance.Text = "<none>";
                    else if (utterance != null) lblCurrentUtterance.Text = utterance.Text;
                    //}
                }));
            });
            DisableSetupCreationPanel();
            _controller.SetZTrackingCompensation((double)numZTrackingCompensation.Value);
            _controller.SetYTrackingCompensation((double)numVTrackingCompensation.Value);
            numQuestionsDelay.Value = _controller.Client.QuestionsFinishEventDelaySeconds;
            numUtterancesTimeout.Value = _controller.Client.UtteranceTimeoutMs;
            numUtteranceRelativeSpeed.Value = _controller.Client.RelativeSpeed;
            chkRelativeUtteranceSpeed.Checked = _controller.Client.UseRelativeSpeed;
            chkStripSlashes.Checked = _controller.Client.StripSlashes;
            chkUtterancesSafeDuration.Checked = _controller.Client.UseUtterancesSafeDuration;
            numUtterancesSafeDuration.Value = (decimal)_controller.Client.UtterancesSafeDuration;

            dontRefresh = true;
            StringCollection sc = Properties.Settings.Default.TestTags;
            for (int i = 0; i < Math.Ceiling(sc.Count / 2.0); i++)
            {
                dgvTestTags.Rows.Add();
                dgvTestTags.Rows[i].Cells[0].Value = sc[i*2];
                if (sc.Count >= i * 2 + 2) dgvTestTags.Rows[i].Cells[1].Value = sc[i * 2 + 1];
            }
            dontRefresh = false;
            RefreshInfoUtterances();
        }

        private void RefreshInfo()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: //Physical Space
                    break;
                case 1: //Gaze and Point

                    break;
                case 2: //Targets
                    RefreshInfoTargets();
                    break;
                case 3: //Idle Behaviour
                    RefreshInfoIdle();
                    break;
                case 4: //Utterances
                    RefreshInfoUtterances();
                    break;
            }
        }

        List<String> registeredLibrary = new List<string>();

        private void RefreshInfoUtterances()
        {
            dontUpdate = true;
            lstUtteranceLibraries.Items.Clear();
            lstUtteranceValidationSets.Items.Clear();
            int i = 0;
            int selectedLibraryIndex = -1;
            int selectedValidatorIndex = -1;

            chkRelativeUtteranceSpeed.Checked = _controller.Client.UseRelativeSpeed;
            numUtteranceRelativeSpeed.Value = _controller.Client.RelativeSpeed;
            numUtteranceRelativeSpeed.Enabled = chkRelativeUtteranceSpeed.Checked;

            foreach (KeyValuePair<string, UtteranceLibrary> utteranceLibrary in _controller.Client.UtteranceLibraries)
            {
                lstUtteranceLibraries.Items.Add(utteranceLibrary.Key);
                if (!registeredLibrary.Contains(utteranceLibrary.Key)) utteranceLibrary.Value.LibraryModified += ((UtteranceLibrary.LibraryModifiedHandler)(() =>
                {
                    if (selectedUtteranceLibrary == utteranceLibrary.Value) this.Invoke((MethodInvoker)(() => PopulateCategories()));
                }));
                registeredLibrary.Add(utteranceLibrary.Key);
                if (selectedUtteranceLibrary == utteranceLibrary.Value) selectedLibraryIndex = i;
                i++;
            }

            i = 0;
            foreach (KeyValuePair<string, UtteranceValidationSet> uvs in _controller.Client.UtteranceValidationSets)
            {
                lstUtteranceValidationSets.Items.Add(uvs.Key);
                if (selectedUVS == uvs.Value) selectedValidatorIndex = i;
                i++;
            }
            if (selectedValidatorIndex != -1)
            {
                lstUtteranceValidationSets.SelectedIndex = selectedValidatorIndex;
                if (lstUtteranceLibraries.SelectedItem != null) btnVerifyUtteranceLibrary.Enabled = true;
                else btnVerifyUtteranceLibrary.Enabled = false;
            }

            if (!chkUseCompositeLibrary.Checked && Properties.Settings.Default.SelectedLibrary != "")
            {
                for (i = 0; i < lstUtteranceLibraries.Items.Count; i++)
                {
                    if ((string) lstUtteranceLibraries.Items[i] == Properties.Settings.Default.SelectedLibrary)
                    {
                        selectedUtteranceLibrary = _controller.Client.UtteranceLibraries[Properties.Settings.Default.SelectedLibrary];
                        lstUtteranceLibraries.SelectedIndex = i;
                        selectedLibraryIndex = i;
                        PopulateCategories();
                        break;
                    }
                }
            }

            if (selectedLibraryIndex != -1)
            {
                lstUtteranceLibraries.SelectedIndex = selectedLibraryIndex;
                PopulateCategories();
                lstUtteranceCategories.Enabled = true;
                lstUtteranceSubCategories.Enabled = true;
                lstUtterances.Enabled = true;
                if (lstUtteranceValidationSets.SelectedItem!=null) btnVerifyUtteranceLibrary.Enabled = true;
                else btnVerifyUtteranceLibrary.Enabled = false;
            }
            else
            {
                lstUtteranceSubCategories.Items.Clear();
                lstUtteranceCategories.Items.Clear();
                lstUtterances.Items.Clear();
                lstUtteranceCategories.Enabled = false;
                lstUtteranceSubCategories.Enabled = false;
                lstUtterances.Enabled = false;
                btnVerifyUtteranceLibrary.Enabled = false;
            }

            HighlightSetBackchannelButton();

            dontUpdate = false;
        }

        private void HighlightSetBackchannelButton()
        {
            if (_controller.Client.BackchannelingCategory != "")
            {
                if (chkUseCompositeLibrary.Checked)
                {
                    string[] bc_s = _controller.Client.BackchannelingCategory.Split(':');
                    string bcCategory = bc_s[0];
                    string bcSubcategory = bc_s.Length > 1 ? bc_s[1] : "-";
                    Utterance utterance = UtteranceLibrary.GetCompositeUtterance(_controller.Client.UtteranceLibraries.Values.ToList(), bcCategory, bcSubcategory);
                    if (utterance != null)
                    {
                        btnBackchanneling.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        btnBackchanneling.BackColor = Color.Red;
                    }
                }
                else
                {
                    string[] bc_s = _controller.Client.BackchannelingCategory.Split(':');
                    string bcCategory = bc_s[0];
                    string bcSubcategory = bc_s.Length > 1 ? bc_s[1] : "-";

                    if (selectedUtteranceLibrary == null) return;
                    if (selectedUtteranceLibrary.Categories.ContainsKey(bcCategory) && selectedUtteranceLibrary.Categories[bcCategory].Contains(bcSubcategory))
                    {
                        btnBackchanneling.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        btnBackchanneling.BackColor = Color.Red;
                    }
                }
            }
        }

        private void PopulateCategories()
        {
            string selected = lstUtteranceCategories.SelectedItem != null ? lstUtteranceCategories.SelectedItem.ToString() : "";
            lstUtteranceCategories.Items.Clear();
            if (selectedUtteranceLibrary == null)
            {
                lstUtteranceCategories.Enabled = false;
                return;
            }
            lstUtteranceCategories.Enabled = true;
            foreach (string s in selectedUtteranceLibrary.Categories.Keys) lstUtteranceCategories.Items.Add(s);

            lstUtteranceCategories.Enabled = false;
            for (int i = 0; i < lstUtteranceCategories.Items.Count; i++)
            {
                if (lstUtteranceCategories.Items[i].ToString() == selected)
                {
                    lstUtteranceCategories.SelectedIndex = i;
                    break;
                }
            }
            PopulateSubcategories();
        }

        private void PopulateSubcategories()
        {
            if (selectedUtteranceLibrary == null
                || lstUtteranceCategories.SelectedItem == null
                || !selectedUtteranceLibrary.Categories.ContainsKey(lstUtteranceCategories.SelectedItem.ToString()))
            {
                lstUtteranceSubCategories.Items.Clear();
                lstUtteranceSubCategories.Enabled = false;
                btnBackchanneling.Enabled = false;
                return;
            }
            lstUtteranceSubCategories.Enabled = true;
            btnBackchanneling.Enabled = true;
            string selected = lstUtteranceCategories.SelectedItem != null ? lstUtteranceCategories.SelectedItem.ToString() : "";
            lstUtteranceSubCategories.Items.Clear();

            List<string> filteredSubcategories = selectedUtteranceLibrary.FilterSubcategories(lstUtteranceCategories.SelectedItem.ToString());

            foreach (string s in filteredSubcategories) lstUtteranceSubCategories.Items.Add(s);

            for (int i = 0; i < lstUtteranceSubCategories.Items.Count; i++)
            {
                if (lstUtteranceSubCategories.Items[i].ToString() == selected)
                {
                    lstUtteranceSubCategories.SelectedIndex = i;
                    break;
                }
            }

            if (lstUtteranceSubCategories.Items.Count == 1) lstUtteranceSubCategories.SelectedIndex = 0;
            PopulateUtterances();
        }

        private void PopulateUtterances()
        {
            txtUtterance.Text = "";
            if (selectedUtteranceLibrary == null
                || lstUtteranceSubCategories.SelectedItem == null
                || !selectedUtteranceLibrary.Categories.ContainsKey(lstUtteranceCategories.SelectedItem.ToString())
                || lstUtteranceCategories.SelectedItem == null
                || !selectedUtteranceLibrary.Categories[lstUtteranceCategories.SelectedItem.ToString()].Contains(lstUtteranceSubCategories.SelectedItem.ToString()))
            {
                lstUtterances.Items.Clear();
                lstUtterances.Enabled = false;
                return;
            }
            lstUtterances.Enabled = true;
            string selected = lstUtterances.SelectedItem != null ? lstUtterances.SelectedItem.ToString() : "";
            lstUtterances.Items.Clear();

            List<Utterance> filteredUtterances = selectedUtteranceLibrary.FilterUtterances(lstUtteranceCategories.SelectedItem.ToString(), lstUtteranceSubCategories.SelectedItem.ToString());
            int i = 0;
            foreach (Utterance utt in filteredUtterances)
            {
                lstUtterances.Items.Add((utt.Repetitions==RepetitionType.OnceInASession?"1/Sess":utt.Repetitions== RepetitionType.OnceAndForever?"1/Ever":"--") +
                                            " | "+
                                            (utt.IsQuestion?"(?)":"--") +
                                            " | " + 
                                            utt.Text);
                if (utt.Text.Equals(selected)) lstUtterances.SelectedIndex = i;
                i++;
            }
            if (lstUtterances.Items.Count == 1) lstUtterances.SelectedIndex = 0;
        }

        private void RefreshInfoIdle()
        {
            dontUpdate = true;
            chkIdleBehavior.Checked = _controller.Client.IdleManager.IdleState;
            grpIdleBehavior.Enabled = _controller.Client.IdleManager.IdleState;
            dontUpdate = false;
        }

        TargetInfo selectedTarget = null;
        private void RefreshInfoTargets()
        {
            if (tabControl1.SelectedIndex == 2)
            {
                dontUpdate = true;
                lstTargets.Items.Clear();
                List<TargetInfo> targets;
                lock (_controller.Client.Targets)
                {
                    targets = new List<TargetInfo>(_controller.Client.Targets.Values);
                }
                foreach (TargetInfo target in targets)
                {
                    lstTargets.Items.Add(target.TargetName == null ? target.GazeTarget.ToString() : target.TargetName);
                }
                if (selectedTarget != null)
                {
                    grpTargetBehaviour.Enabled = true;
                    grpTargetInfo.Enabled = true;
                    grpTargetLinking.Enabled = true;
                    for (int i = 0; i < lstTargets.Items.Count; i++)
                        if (lstTargets.Items[i].ToString() == selectedTarget.TargetName)
                        {
                            lstTargets.SelectedIndex = i;
                            RefreshInfoSelectedTarget();
                            break;
                        }
                }
                else
                {
                    grpTargetBehaviour.Enabled = false;
                    grpTargetInfo.Enabled = false;
                    grpTargetLinking.Enabled = false;
                }
                dontUpdate = false;
            }
        }

        private void RefreshInfoSelectedTarget()
        {
            dontUpdate = true;

            cmbTargetLinks.Items.Clear();
            List<TargetInfo> targets;
            lock (_controller.Client.Targets)
            {
                targets = new List<TargetInfo>(_controller.Client.Targets.Values);
            }
            foreach (TargetInfo target in targets)
            {
                if (target != selectedTarget) cmbTargetLinks.Items.Add(target.TargetName == null ? target.GazeTarget.ToString() : target.TargetName);
            }

            TargetInfo realSelectedTarget = selectedTarget;
            if (selectedTarget.Linked && _controller.Client.Targets.ContainsKey(selectedTarget.LinkedTargetName)) realSelectedTarget = _controller.Client.Targets[selectedTarget.LinkedTargetName];

            txtTargetName.Text = selectedTarget.TargetName;

            if (selectedTarget.Linked)
            {
                lblTargetLink.Text = realSelectedTarget.TargetName;
                cmbTargetLinks.SelectedIndex = -1;
                cmbTargetLinks.Enabled = false;
                btnSetTargetLink.Text = "Unlink";
                cmbDynamicTarget.Enabled = false;
                radTargetTypeAngle.Enabled = false;
                radTargetTypeScreen.Enabled = false;
                radTargetTypeDynamic.Enabled = false;
                numTargetCoordX.Enabled = false;
                numTargetCoordY.Enabled = false;
            }
            else
            {
                lblTargetLink.Text = "-";
                cmbTargetLinks.Enabled = true;
                btnSetTargetLink.Text = "Set Link";
                cmbDynamicTarget.Enabled = true;
                radTargetTypeAngle.Enabled = true;
                radTargetTypeScreen.Enabled = true;
                radTargetTypeDynamic.Enabled = true;
                numTargetCoordX.Enabled = true;
                numTargetCoordY.Enabled = true;
            }

            switch (realSelectedTarget.GazeTarget)
            {
                case GazeTarget.Angle:
                    numTargetCoordX.DecimalPlaces = 2;
                    numTargetCoordX.Minimum = -180;
                    numTargetCoordX.Maximum = 180;
                    numTargetCoordY.Minimum = -180;
                    numTargetCoordY.Maximum = 180;
                    numTargetCoordX.Value = (decimal)realSelectedTarget.Coordinates.X;
                    numTargetCoordY.Value = (decimal)realSelectedTarget.Coordinates.Y;
                    cmbDynamicTarget.Enabled = false;
                    numTargetCoordX.Enabled = true;
                    numTargetCoordY.Enabled = true;
                    radTargetTypeAngle.Checked = true;
                    cmbDynamicTarget.SelectedIndex = -1;
                    break;
                case GazeTarget.ScreenPoint:
                    numTargetCoordX.DecimalPlaces = 0;
                    numTargetCoordX.Minimum = 0;
                    numTargetCoordX.Maximum = 1920;
                    numTargetCoordY.Minimum = 0;
                    numTargetCoordY.Maximum = 1920;
                    numTargetCoordX.Value = (decimal)realSelectedTarget.Coordinates.X;
                    numTargetCoordY.Value = (decimal)realSelectedTarget.Coordinates.Y;
                    cmbDynamicTarget.Enabled = false;
                    numTargetCoordX.Enabled = true;
                    numTargetCoordY.Enabled = true;
                    radTargetTypeScreen.Checked = true;
                    cmbDynamicTarget.SelectedIndex = -1;
                    break;
                default:
                    cmbDynamicTarget.Enabled = true;
                    numTargetCoordX.Enabled = false;
                    numTargetCoordY.Enabled = false;
                    numTargetCoordX.DecimalPlaces = 0;
                    numTargetCoordX.Minimum = 0;
                    numTargetCoordX.Maximum = 0;
                    numTargetCoordY.Minimum = 0;
                    numTargetCoordY.Maximum = 0;
                    numTargetCoordX.Value = 0;
                    numTargetCoordY.Value = 0;
                    radTargetTypeDynamic.Checked = true;
                    for (int i = 0; i < cmbDynamicTarget.Items.Count; i++)
                    {
                        if (cmbDynamicTarget.Items[i].ToString() == realSelectedTarget.GazeTarget.ToString())
                        {
                            cmbDynamicTarget.SelectedIndex = i;
                            break;
                        }
                    }
                    break;
            }
            dontUpdate = false;
        }

        bool dontUpdate = false;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            foreach (string t in Enum.GetNames(typeof(GazeTarget)))
            {
                cmbGazeTarget.Items.Add(t);
                cmbDynamicTarget.Items.Add(t);
            }

            cmbGazeTarget.SelectedIndex = cmbGazeTarget.FindStringExact(_controller.Client.GazeManager.GazeState.ToString());
            _controller.ActiveSetupChanged += delegate(object s, EventArgs ea)
            {
                groupSetupSketch.Refresh();
                Properties.Settings.Default.LastUsedSetup = ((PhysicalSpace)cmbSetups.SelectedItem)._name;
                Properties.Settings.Default.Save();
            };

            if (Properties.Settings.Default.TargetsFile != "" && File.Exists(Properties.Settings.Default.TargetsFile))
            {
                TargetsFile.Load(Properties.Settings.Default.TargetsFile, _controller.Client);
                txtLoadedTargets.Text = Properties.Settings.Default.TargetsFile;
                RefreshInfo();
            }

            chkUseCompositeLibrary.Checked = Properties.Settings.Default.UseCompositeLibrary;
            chkGazeBreaking.Checked = Properties.Settings.Default.PerformGazeBreaking;
            chkEstablishingGaze.Checked = Properties.Settings.Default.PerformEstablishingGaze;

            numGazeSpeed.Value =  (decimal)_controller.Client.GazeManager.DefaultGazeSpeed;
            numVTrackingCompensation.Value = (decimal)_controller.Client.YTrackingCompensation;
            numZTrackingCompensation.Value = (decimal)_controller.Client.ZTrackingCompensation;

            if (_controller.Client.BackchannelingCategory != "") btnBackchanneling.Text = _controller.Client.BackchannelingCategory;
        }


        private void panelSetupSketch_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Point origin = groupSetupSketch.Location;
            int panelHeight = groupSetupSketch.Size.Height-30;      // -30 just to be sure the image will be draw inside the panel
            int panelWidth = groupSetupSketch.Size.Width-30;
            int distanceFromBorder = 5;

            Pen pen = new Pen(Color.Black);
            pen.Width = 3;
            PhysicalSpace activeSetup = _controller.GetActiveSetup();

            //variables for drawing the picture
            double virtualScreenWidth = 0;
            double virtualScreenHeight = 0;
            if (activeSetup  != null)
            {
                Vector2D reversedForward = new Vector2D(activeSetup._forward.X * -1, activeSetup._forward.Y * -1);
                // Drawing the setup graphical example
                if ((activeSetup._screenSetup._size.X / activeSetup._screenSetup._size.Y) > (panelWidth/panelHeight) ) // screen is wide
                {
                    virtualScreenWidth = panelWidth - distanceFromBorder * 2;
                    virtualScreenHeight = (activeSetup._screenSetup._size.Y * virtualScreenWidth) / activeSetup._screenSetup._size.X;
                }
                else                 // Screen is tall
                {
                    virtualScreenHeight = panelHeight - distanceFromBorder * 2;
                    virtualScreenWidth = activeSetup._screenSetup._size.X * virtualScreenHeight / activeSetup._screenSetup._size.Y;
                }
                /* Inverting the Y axis since the observer considers the 0,0 coordinate coincident with the bottom left angle of the table and 
                 * the interface is printed considering the 0,0 as the upper left angle
                 */
                int robotPositionX = (int)(distanceFromBorder + (activeSetup._headPosition.X * virtualScreenWidth) / activeSetup._screenSetup._size.X);
                int robotPositionY = (int)(distanceFromBorder + virtualScreenHeight - (activeSetup._headPosition.Y * virtualScreenHeight) / activeSetup._screenSetup._size.Y);      
                g.DrawRectangle(pen, new Rectangle(distanceFromBorder, distanceFromBorder, (int)virtualScreenWidth, (int)virtualScreenHeight));
                int forwardVectorVerticeX = (int)(robotPositionX + 30 * reversedForward.X);
                int forwardVectorVerticeY = (int)(robotPositionY + 30 * reversedForward.Y);
                g.DrawLine(pen, (int)robotPositionX, (int)robotPositionY, forwardVectorVerticeX, forwardVectorVerticeY);
                g.DrawRectangle(pen, robotPositionX - 5, robotPositionY - 5, 10, 10);
            }
            
            base.OnPaint(e);
        }

        private void btnNewSetup_Click(object sender, EventArgs e)
        {
            ClearSetupCreationPanelFields();
            EnableSetupCreationPanel();
        }


        private void btnDeleteSetup_Click(object sender, EventArgs e)
        {
            if (cmbSetups.SelectedItem == null)
            {
                MessageBox.Show("No setup selected");
            }
            else
            {
                _controller.Delete((PhysicalSpace)cmbSetups.SelectedItem);
                if (cmbSetups.Items.Count > 0) 
                    cmbSetups.SelectedIndex = 0;
            }
        }

        private void btnSaveSetting_Click(object sender, EventArgs e)
        {
            if (isSetupCreationPanelWellCompiled())
            {
                Vector2D size = new Vector2D(double.Parse(txtScreenSizeX.Text), double.Parse(txtScreenSizeY.Text));
                Vector2D resolution = new Vector2D(double.Parse(txtScreenResX.Text), double.Parse(txtScreenResY.Text));
                Vector3D headPos = new Vector3D(double.Parse(txtHeadPosX.Text), double.Parse(txtHeadPosY.Text), double.Parse(txtHeadPosZ.Text));
                Vector3D leftShoulder = new Vector3D(double.Parse(txtLShoulderX.Text), double.Parse(txtLShoulderY.Text), double.Parse(txtLShoulderZ.Text));
                Vector3D rightShoulder = new Vector3D(double.Parse(txtRShoulderX.Text), double.Parse(txtRShoulderY.Text), double.Parse(txtRShoulderZ.Text));
                Vector2D forwardVector = new Vector2D(double.Parse(txtForwardX.Text), double.Parse(txtForwardY.Text));
                ScreenSetup ss = new ScreenSetup(size, resolution);
                PhysicalSpace spaceSetup = new PhysicalSpace(ss, headPos, rightShoulder, leftShoulder, forwardVector, txtSetupName.Text);
                _controller.Save(spaceSetup);
                ClearSetupCreationPanelFields();
                DisableSetupCreationPanel();
            }
            else
            {
                MessageBox.Show("All the fields need to be filled and the coordinates fields must contain just numbers");
            }
        }

        public void StatusMessage(string msg, Color color)
        {
            MethodInvoker action = delegate
            {
                lblStatus.Text = msg;
                lblStatus.ForeColor = color;
            };
            try
            {
                lblStatus.BeginInvoke(action);
            }
            catch { }
        }

        private void DisableSetupCreationPanel()
        {
            panelSetupCreation.Enabled = false;
            btnSaveSetting.Enabled = false;
        }

        private void EnableSetupCreationPanel()
        {
            panelSetupCreation.Enabled = true;
            btnSaveSetting.Enabled = true;
        }

        private void ReloadSavedSetups()
        {
            cmbSetups.Items.Clear();
            cmbSetups.Items.AddRange(_controller.Setups.ToArray());
            if (cmbSetups.Items.Count > 0)
                cmbSetups.SelectedIndex = 0;
            else
                cmbSetups.Text = "";

            // Reload last used setup
            string lastUsed = Properties.Settings.Default.LastUsedSetup;
            if (!lastUsed.Equals(-1))
            {
                bool found = false;
                int index = 0;
                foreach(PhysicalSpace ps in cmbSetups.Items) {
                    if (ps._name.Equals(lastUsed)) {
                        found = true;
                        break;
                    }
                    index++;
                }

                if (found)
                {
                    cmbSetups.SelectedIndex = index;
                    PhysicalSpace activeSetup = cmbSetups.SelectedItem as PhysicalSpace;
                    _controller.ActivateSetup(activeSetup);
                    AutoFillFields(activeSetup);
                }
            }
        }

        private void ClearSetupCreationPanelFields()
        {
            if (_controller.GetActiveSetup() == null)
            {
                foreach (Control c in panelSetupCreation.Controls)
                {
                    if (c is TextBox)
                    {
                        ((TextBox)c).Text = "";
                    }
                }
            }
            else
            {
                AutoFillFields(_controller.GetActiveSetup());
            }
        }

        /// <summary>
        /// Checks if the fields in the creation panel are not empty and contain the right type of value
        /// </summary>
        /// <returns>true if the fields in the panel have been well compiled</returns>
        private bool isSetupCreationPanelWellCompiled()
        {
            bool retVal = true;
            foreach (Control c in panelSetupCreation.Controls)
            {
                if (c is TextBox)
                {
                    // Checking every field isn't empty
                    TextBox tb = (TextBox)c;
                    retVal = retVal && tb.Text != null
                                    && tb.Text != "";
                    // Checking every field contains the right type of values
                    double temp; 
                    if (retVal && tb != txtSetupName)
                        retVal = retVal && double.TryParse(tb.Text, out temp);
                }
            }
            return retVal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit();
        }

        private void btnLoadSetup_Click(object sender, EventArgs e)
        {
            if (cmbSetups.Items.Count>0){
                PhysicalSpace activeSetup = cmbSetups.SelectedItem as PhysicalSpace;
                _controller.ActivateSetup(activeSetup);
                AutoFillFields(activeSetup);
            }
        }

        private void btnEditSetup_Click(object sender, EventArgs e)
        {
            if (cmbSetups.Items.Count > 0)
            {
                PhysicalSpace ps = cmbSetups.SelectedItem as PhysicalSpace;
                EnableSetupCreationPanel();
                AutoFillFields(ps);
            }
        }

        private void AutoFillFields(PhysicalSpace physicalSpaceSetup)
        {
            txtSetupName.Text = physicalSpaceSetup._name;
            txtForwardX.Text = physicalSpaceSetup._forward.X + "";
            txtForwardY.Text = physicalSpaceSetup._forward.Y + "";
            txtHeadPosX.Text = physicalSpaceSetup._headPosition.X + "";
            txtHeadPosY.Text = physicalSpaceSetup._headPosition.Y + "";
            txtHeadPosZ.Text = physicalSpaceSetup._headPosition.Z + "";

            txtLShoulderX.Text = physicalSpaceSetup._leftShoulderPosition.X + "";
            txtLShoulderY.Text = physicalSpaceSetup._leftShoulderPosition.Y + "";
            txtLShoulderZ.Text = physicalSpaceSetup._leftShoulderPosition.Z + "";

            txtRShoulderX.Text = physicalSpaceSetup._rightShoulderPosition.X + "";
            txtRShoulderY.Text = physicalSpaceSetup._rightShoulderPosition.Y + "";
            txtRShoulderZ.Text = physicalSpaceSetup._rightShoulderPosition.Z + "";

            txtScreenResX.Text = physicalSpaceSetup._screenSetup._resolution.X + "";
            txtScreenResY.Text = physicalSpaceSetup._screenSetup._resolution.Y + "";
            txtScreenSizeX.Text = physicalSpaceSetup._screenSetup._size.X + "";
            txtScreenSizeY.Text = physicalSpaceSetup._screenSetup._size.Y + "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            dontUpdate = true;
            _controller.SwitchGazeTarget(cmbGazeTarget.SelectedItem.ToString());
            dontUpdate = false;
        }


        internal void SetTrackingClickPoint(Vector2D point)
        {
            lblClickAngle.Text = "x: " + point.X + "; y: " + point.Y;
        }

        internal void SetTrackingPersonAngle(int userId, Vector2D angle)
        {
            string str = String.Format("h: {0:0.0}; v: {1:0.0}", angle.X, angle.Y);
            switch (userId)
            {
                case 0: lblPerson1Angle.Text = str; break;
                case 1: lblPerson2Angle.Text = str; break;
                case 2: lblPerson3Angle.Text = str; break;
                case 3: lblPerson4Angle.Text = str; break;
            }
        }

        internal void SetTrackingScreenAngle(Vector2D point)
        {
            lblScreenAngle.Text = "h: " + point.X + "; v: " + point.Y;
        }

        private void numVTrackingCompensation_ValueChanged(object sender, EventArgs e)
        {
            _controller.SetYTrackingCompensation((double)numVTrackingCompensation.Value);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            _controller.SetZTrackingCompensation((double)numZTrackingCompensation.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ckbxAddGaze.Checked)
            {
                _controller.Client.GazeManager.GazeToAngle((double)numPointToH.Value, (double)numPointToV.Value);
            }
            _controller.Client.SkPublisher.PointingAngle("", (double)numPointToH.Value, (double)numPointToV.Value, 0.2, numPointToH.Value < 0 ? Thalamus.Actions.PointingMode.LeftHand : Thalamus.Actions.PointingMode.RightHand);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ckbxAddGaze.Checked)
            {
                _controller.Client.GazeManager.GazeToScreen((double)numPointToH.Value, (double)numPointToV.Value);
            }
            _controller.Client.PointAtScreen("", (double)numPointToH.Value, (double)numPointToV.Value);

        }

        private void btnWave_Click(object sender, EventArgs e)
        {
            if (ckbxAddGaze.Checked)
            {
                _controller.Client.GazeManager.GazeToScreen((double)numPointToH.Value, (double)numPointToV.Value);
            }
            _controller.Client.WaveAtScreen("", (double)numPointToH.Value, (double)numPointToV.Value, (double)numAmplitude.Value, (double)numFequency.Value, (double)numDuration.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ckbxAddGaze.Checked)
            {
                _controller.Client.GazeManager.GazeToAngle((double)numPointToH.Value, (double)numPointToV.Value);
            }
            _controller.Client.SkPublisher.Waving("", (double)numPointToH.Value, (double)numPointToV.Value, (double)numFequency.Value, (double)numAmplitude.Value, (double)numDuration.Value, numPointToH.Value < 0 ? Thalamus.Actions.PointingMode.LeftHand : Thalamus.Actions.PointingMode.RightHand);
        }

        private void numGazeSpeed_ValueChanged(object sender, EventArgs e)
        {
            _controller.Client.GazeManager.DefaultGazeSpeed = (double)numGazeSpeed.Value;
        }

        private void lstTargets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_controller.Client.Targets.ContainsKey(lstTargets.SelectedItem.ToString().ToLower()))
            {
                grpTargetBehaviour.Enabled = true;
                grpTargetInfo.Enabled = true;
                grpTargetLinking.Enabled = true;
                selectedTarget = _controller.Client.Targets[lstTargets.SelectedItem.ToString().ToLower()];
                RefreshInfoSelectedTarget();
            }
            else
            {
                grpTargetBehaviour.Enabled = false;
                grpTargetInfo.Enabled = false;
                grpTargetLinking.Enabled = false;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void numTargetCoordX_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            if (selectedTarget.GazeTarget == GazeTarget.Angle) _controller.Client.SetAngleTarget(selectedTarget.TargetName, new Vector2D((double)numTargetCoordX.Value, (double)numTargetCoordY.Value));
            else if (selectedTarget.GazeTarget == GazeTarget.ScreenPoint) _controller.Client.SetScreenTarget(selectedTarget.TargetName, new Vector2D((double)numTargetCoordX.Value, (double)numTargetCoordY.Value));
        }

        private void numTargetCoordY_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            if (selectedTarget.GazeTarget == GazeTarget.Angle) _controller.Client.SetAngleTarget(selectedTarget.TargetName, new Vector2D((double)numTargetCoordX.Value, (double)numTargetCoordY.Value));
            else if (selectedTarget.GazeTarget == GazeTarget.ScreenPoint) _controller.Client.SetScreenTarget(selectedTarget.TargetName, new Vector2D((double)numTargetCoordX.Value, (double)numTargetCoordY.Value));
        }

        private void radTargetTypeAngle_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            selectedTarget.GazeTarget = GazeTarget.Angle;
            RefreshInfo();
        }

        private void cmbDynamicTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontUpdate || selectedTarget==null || cmbDynamicTarget.SelectedIndex==-1) return;
            GazeTarget gt = (GazeTarget) Enum.Parse(typeof(GazeTarget), cmbDynamicTarget.SelectedItem.ToString());
            selectedTarget.GazeTarget = gt;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            selectedTarget.GazeTarget = GazeTarget.ScreenPoint;
            RefreshInfo();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            RefreshInfo();
        }

        private void btnLoadLibrary_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Targets File|*.taf|All Files|*.*";
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lstTargets.Items.Clear();
                TargetsFile.Load(openFileDialog.FileName, _controller.Client);
                txtLoadedTargets.Text = openFileDialog.FileName;
                Properties.Settings.Default.TargetsFile = openFileDialog.FileName;
                Properties.Settings.Default.Save();
                RefreshInfoTargets();
            }
        }

        private void txtLoadedTargets_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnSaveTargets_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Targets File|*.taf|All Files|*.*";
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                TargetsFile.Save(saveFileDialog.FileName, _controller.Client);
                txtLoadedTargets.Text = saveFileDialog.FileName;
                Properties.Settings.Default.TargetsFile = saveFileDialog.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void btnCancelUtterance_Click(object sender, EventArgs e)
        {
            _controller.Client.CancelUtterance("");
        }

        private void btnSetTargetLink_Click(object sender, EventArgs e)
        {
            if (selectedTarget != null)
            {
                if (selectedTarget.Linked)
                {
                    _controller.Client.UnlinkTarget(selectedTarget.TargetName);
                    RefreshInfoSelectedTarget();
                }
                else if (cmbTargetLinks.SelectedItem != null)
                {
                    _controller.Client.SetTarget(selectedTarget.TargetName, cmbTargetLinks.SelectedItem.ToString());
                    RefreshInfoSelectedTarget();
                }
            }
        }

        private void txtTargetName_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            while (_controller.Client.Targets.ContainsKey("newTarget_" + i)) i++;
            _controller.Client.SetTarget("newTarget_" + i, GazeTarget.None);
            RefreshInfo();
        }

        private void cmbTargetLinks_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkIdleBehavior_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            _controller.Client.SetIdleBehavior(chkIdleBehavior.Checked);
            RefreshInfoIdle();
        }

        private void btnSaveTargetName_Click(object sender, EventArgs e)
        {
            _controller.Client.SwitchTargetName(selectedTarget.TargetName, txtTargetName.Text);
            selectedTarget = _controller.Client.Targets[txtTargetName.Text.ToLower()];
            RefreshInfo();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _controller.Dispose();
        }

        private void lstUtteranceLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            selectedUtteranceLibrary = null;
            Properties.Settings.Default.SelectedLibrary = "";
            foreach (KeyValuePair<string, UtteranceLibrary> utteranceLibrary in _controller.Client.UtteranceLibraries)
            {
                if (utteranceLibrary.Key == lstUtteranceLibraries.SelectedItem.ToString())
                {
                    selectedUtteranceLibrary = utteranceLibrary.Value;
                    Properties.Settings.Default.SelectedLibrary = utteranceLibrary.Key;
                    _controller.Client.SelectedLibrary = (string)lstUtteranceLibraries.SelectedItem;
                    break;
                }
            }

            Properties.Settings.Default.Save();
            RefreshInfoUtterances();
        }

        private void lstUtteranceSubcategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateSubcategories();
        }

        private void lstUtteranceCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateUtterances();
        }

        private void lstUtterances_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUtterance.Text = lstUtterances.SelectedItem.ToString().Split('|')[2];
        }

        private void btnPerformUtterance_Click(object sender, EventArgs e)
        {
            //string cat = (lstUtteranceCategories.SelectedItem == null ? "-" : lstUtteranceCategories.SelectedItem.ToString()) + "." + (lstUtteranceSubCategories.SelectedItem == null ? "-" : lstUtteranceSubCategories.SelectedItem.ToString());
            string id = Guid.NewGuid().ToString();

            List<string> lTags = new List<string>();
            List<string> lValues = new List<string>();
            foreach (DataGridViewRow row in dgvTestTags.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    lTags.Add(row.Cells[0].Value.ToString());
                    lValues.Add(row.Cells[1].Value.ToString());
                }
            }

            _controller.Client.PerformUtterance(id, new Utterance(txtUtterance.Text), lTags.ToArray(), lValues.ToArray());
        }

        private void btnVerifyUtteranceLibrary_Click(object sender, EventArgs e)
        {
            if (selectedUtteranceLibrary == null) return;
            _controller.Client.UtteranceValidationSet = _controller.Client.UtteranceValidationSets[(string)lstUtteranceValidationSets.SelectedItem];
            selectedUtteranceLibrary.Validate(_controller.Client.UtteranceValidationSet);
        }

        private void btnReloadUtteranceLibraries_Click(object sender, EventArgs e)
        {
            _controller.Client.LoadUtteranceLibraries();
            RefreshInfoUtterances();
        }

        private void btnGaze_Click(object sender, EventArgs e)
        {
            if (selectedTarget != null) _controller.Client.GazeManager.Gaze(selectedTarget);
        }

        private void btnGlance_Click(object sender, EventArgs e)
        {
            if (selectedTarget != null) _controller.Client.GazeManager.Glance(selectedTarget);
        }

        private void chkRelativeUtteranceSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            numUtteranceRelativeSpeed.Enabled = chkRelativeUtteranceSpeed.Checked;
            _controller.Client.UseRelativeSpeed = chkRelativeUtteranceSpeed.Checked;
        }

        private void numUtteranceRelativeSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            _controller.Client.RelativeSpeed = Convert.ToInt32(numUtteranceRelativeSpeed.Value);
        }

        private void btnReloadValidationSets_Click(object sender, EventArgs e)
        {
            _controller.Client.LoadUtteranceValidators();
            RefreshInfoUtterances();
        }


        private void chkUseCompositeLibrary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseCompositeLibrary = chkUseCompositeLibrary.Checked;
            Properties.Settings.Default.Save();

            _controller.Client.UseCompositeLibrary = chkUseCompositeLibrary.Checked;

            if (!chkUseCompositeLibrary.Checked){
                if (Properties.Settings.Default.SelectedLibrary != "")
                {
                    for (int i = 0; i < lstUtteranceLibraries.Items.Count; i++)
                    {
                        if ((string)lstUtteranceLibraries.Items[i] == Properties.Settings.Default.SelectedLibrary)
                        {
                            lstUtteranceLibraries.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (lstUtteranceLibraries.Items.Count>0) lstUtteranceLibraries.SelectedIndex = 0;
                _controller.Client.SelectedLibrary = (string)lstUtteranceLibraries.SelectedItem;
            }
        }

        private void lstUtteranceValidationSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnVerifyUtteranceLibrary.Enabled = lstUtteranceValidationSets.SelectedItem!=null;
        }

        private void btnPerformFromLibraryTest_Click(object sender, EventArgs e)
        {
            string category = "";
            string subcategory = "";
            if (lstUtteranceCategories.SelectedItem != null) category = lstUtteranceCategories.SelectedItem.ToString();
            if (lstUtteranceSubCategories.SelectedItem != null) subcategory = lstUtteranceSubCategories.SelectedItem.ToString();

            List<string> lTags = new List<string>();
            List<string> lValues = new List<string>();
            foreach (DataGridViewRow row in dgvTestTags.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    lTags.Add(row.Cells[0].Value.ToString());
                    lValues.Add(row.Cells[1].Value.ToString());
                }
            }
            Console.WriteLine("click");
            _controller.Client.PerformUtteranceFromLibrary(Guid.NewGuid().ToString(), category, subcategory, lTags.ToArray(), lValues.ToArray());
        }

        private void numQuestionsDelay_ValueChanged(object sender, EventArgs e)
        {
            _controller.Client.QuestionsFinishEventDelaySeconds = (int) ((NumericUpDown) sender).Value;
        }

        private void chkStripSlashes_CheckedChanged(object sender, EventArgs e)
        {
            _controller.Client.StripSlashes = chkStripSlashes.Checked;
        }

        private void dgvTestTags_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dontRefresh) return;
            Properties.Settings.Default.TestTags.Clear();
            foreach (DataGridViewRow row in dgvTestTags.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    Properties.Settings.Default.TestTags.Add(row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString());
                    Properties.Settings.Default.TestTags.Add(row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString());
                }
            }
            Properties.Settings.Default.Save();
        }

        private void btnBackchanneling_Click(object sender, EventArgs e)
        {
            if (lstUtteranceCategories.SelectedIndex != -1)
            {
                _controller.Client.BackchannelingCategory = lstUtteranceCategories.SelectedItem.ToString() + (lstUtteranceSubCategories.SelectedIndex != -1 ? ":" + lstUtteranceSubCategories.SelectedItem.ToString() : "");
                btnBackchanneling.Text = _controller.Client.BackchannelingCategory;
                HighlightSetBackchannelButton();
            }
        }

        private void numUtterancesTimeout_ValueChanged(object sender, EventArgs e)
        {
            _controller.Client.UtteranceTimeoutMs = (int)((NumericUpDown)sender).Value;
        }

        private void chkUtterancesSafeDuration_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            numUtterancesSafeDuration.Enabled = chkUtterancesSafeDuration.Checked;
            _controller.Client.UseUtterancesSafeDuration = chkUtterancesSafeDuration.Checked;
        }

        private void numUtterancesSafeDuration_ValueChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            _controller.Client.UtterancesSafeDuration = Convert.ToInt32(numUtterancesSafeDuration.Value);
        }

        private void chkEstablishingGaze_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            _controller.Client.GazeManager.PerformEstablishingGaze = chkEstablishingGaze.Checked;
        }

        private void chkGazeBreaking_CheckedChanged(object sender, EventArgs e)
        {
            if (dontUpdate) return;
            _controller.Client.GazeManager.PerformGazeBreaking = chkGazeBreaking.Checked;
        }

    }
}
