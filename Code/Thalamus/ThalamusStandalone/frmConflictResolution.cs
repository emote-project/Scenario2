using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Thalamus.Conflicts;

namespace Thalamus
{
    public partial class frmConflictResolution : Form
    {
        frmThalamus MainForm;
        Environment Thalamus;
        bool dontRefresh = false;
        ConflictRule selectedConflict = null;
        string selectedFunnel = "";
        System.Windows.Forms.Timer timerPaintLable;

        List<int> unselectableFunnelLines = new List<int>();

        public frmConflictResolution(frmThalamus mainForm)
        {
            InitializeComponent();
            lstFunnel.DrawMode = DrawMode.OwnerDrawFixed;
            this.MainForm = mainForm;
            Thalamus = Environment.Instance;
            timerPaintLable = new System.Windows.Forms.Timer();
            timerPaintLable.Interval = 1000;
            timerPaintLable.Tick += timerPaintLable_Tick;
            ConflictManager.Instance.SettingsSaved += ConflictManager_SettingsSaved;
        }

        void timerPaintLable_Tick(object sender, EventArgs e)
        {
            lblConflictRules.ForeColor = System.Drawing.SystemColors.Control;
        }

        void ConflictManager_SettingsSaved()
        {
            lblConflictRules.ForeColor = Color.Red;
            timerPaintLable.Start();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontRefresh) return;
            string conflict = lstConflicts.SelectedItem.ToString();
            string[] conflictSplit = conflict.Split(':');
            selectedFunnel = "";
            lstFunnel.SelectedIndex = -1;
            if (MainForm.SelectedCharacter != null && ConflictManager.Instance.ConflictRules.ContainsKey(MainForm.SelectedCharacter.Name)
                                                    && ConflictManager.Instance.ConflictRules[MainForm.SelectedCharacter.Name].ContainsKey(conflictSplit[1]))
            {
                try
                {
                    selectedConflict = ConflictManager.Instance.ConflictRules[MainForm.SelectedCharacter.Name][conflictSplit[1]][(ConflictRule.ConflictTypes)Enum.Parse(typeof(ConflictRule.ConflictTypes), conflictSplit[0])];
                    tabConflictRule.Enabled = true;
                    RefreshSelectedConflict();
                }
                catch (Exception ex)
                {
                    ConflictManager.Instance.DebugException(ex);
                    selectedConflict = null;
                    tabConflictRule.Enabled = false;
                }
                
            }
        }

        private void lstFunnel_DrawItem(object sender, DrawItemEventArgs e)
        {
            dontRefresh = true;
            e.DrawBackground();
            FontStyle fs = FontStyle.Regular;
            Brush b = Brushes.Black;
            if (unselectableFunnelLines.Contains(e.Index))
            {
                b = Brushes.Gray;
                fs = FontStyle.Italic;
                if (e.State.HasFlag(DrawItemState.Selected))
                {
                    //b = Brushes.White;
                    lstFunnel.SelectedIndex = -1;
                    lstFunnel.Invalidate();
                }
                e.Graphics.DrawString(lstFunnel.Items[e.Index].ToString(), new Font("Microsoft Sans Serif", 8, fs), b, e.Bounds);
            }
            else
            {
                if (e.State.HasFlag(DrawItemState.Selected)) b = Brushes.White;
                e.Graphics.DrawString(lstFunnel.Items[e.Index].ToString(), new Font("Microsoft Sans Serif", 8, fs), b, e.Bounds);
                e.DrawFocusRectangle();
            }
            dontRefresh = false;
        }

        private void RefreshInfo()
        {
            dontRefresh = true;
            if (MainForm.SelectedCharacter !=null && ConflictManager.Instance.ConflictRules.ContainsKey(MainForm.SelectedCharacter.Name)) {
                lstConflicts.Enabled = true;
                lstConflicts.Items.Clear();
                foreach (string conflictEvent in ConflictManager.Instance.ConflictRules[MainForm.SelectedCharacter.Name].Keys)
                {
                    foreach (KeyValuePair<ConflictRule.ConflictTypes, ConflictRule> conflictType in ConflictManager.Instance.ConflictRules[MainForm.SelectedCharacter.Name][conflictEvent])
                    {
                        lstConflicts.Items.Add(conflictType.Key.ToString() + ":" + conflictType.Value.FullEventName);
                    }
                }

                if (selectedConflict != null)
                {
                    tabConflictRule.Enabled = true;
                    bool found = false;
                    for (int i = 0; i < lstConflicts.Items.Count; i++)
                    {
                        if (lstConflicts.Items[i].ToString() == selectedConflict.ConflictType + ":" + selectedConflict.FullEventName)
                        {
                            lstConflicts.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }
                    if (!found) selectedConflict = null;
                    else
                    {
                        RefreshSelectedConflict();
                    }
                }
                else
                {
                    tabConflictRule.Enabled = false;
                }

                switch (ConflictManager.Instance.DefaultRuleOnActionPublishingConflict)
                {
                    case ConflictSettings.DefaultRuleType.All:
                        radActionPublishersAll.Checked = true;
                        break;
                    case ConflictSettings.DefaultRuleType.Ignore:
                        radActionPublishersIgnore.Checked = true;
                        break;
                    default:
                        radActionPublishersNone.Checked = true;
                        break;
                }
                switch (ConflictManager.Instance.DefaultRuleOnActionSubscriptionConflict)
                {
                    case ConflictSettings.DefaultRuleType.All:
                        radActionSubscribersAll.Checked = true;
                        break;
                    case ConflictSettings.DefaultRuleType.Ignore:
                        radActionSubscribersIgnore.Checked = true;
                        break;
                    default:
                        radActionSubscribersNone.Checked = true;
                        break;
                }
                switch (ConflictManager.Instance.DefaultRuleOnPerceptionPublishingConflict)
                {
                    case ConflictSettings.DefaultRuleType.All:
                        radPerceptionPublishersAll.Checked = true;
                        break;
                    case ConflictSettings.DefaultRuleType.Ignore:
                        radPerceptionPublishersIgnore.Checked = true;
                        break;
                    default:
                        radPerceptionPublishersNone.Checked = true;
                        break;
                }
                switch (ConflictManager.Instance.DefaultRuleOnPerceptionSubscriptionConflict)
                {
                    case ConflictSettings.DefaultRuleType.All:
                        radPerceptionSubscribersAll.Checked = true;
                        break;
                    case ConflictSettings.DefaultRuleType.Ignore:
                        radPerceptionSubscribersIgnore.Checked = true;
                        break;
                    default:
                        radPerceptionSubscribersNone.Checked = true;
                        break;
                }
            }
            else{
                lstConflicts.Enabled = false;
                lstConflicts.Items.Clear();
                lstConflicts.Items.Add("No Conflicts.");
            }
            
            dontRefresh = false;
        }

        private void RefreshSelectedConflict() 
        {

            if (tabConflictRule.SelectedIndex == 0)
            {
                clbConflictRules.Items.Clear();
                foreach (KeyValuePair<string, bool> client in selectedConflict.ClientsRules)
                {
                    clbConflictRules.Items.Add(client.Key, client.Value);
                }

                foreach (string client in selectedConflict.UnsolvedClients)
                {
                    clbConflictRules.Items.Add("UNSOLVED:" + client, false);
                }
            }
            else if (tabConflictRule.SelectedIndex == 1)
            {
                lstFunnel.Items.Clear();
                unselectableFunnelLines = new List<int>();
                int i = 0;
                if (selectedConflict.Funnel.Count == 0)
                {
                    lstFunnel.Items.Add("No funnel.");
                    unselectableFunnelLines.Add(i++);
                }
                else
                {
                    unselectableFunnelLines.Add(i++);
                    lstFunnel.Items.Add("- Funnel In -");
                    foreach (string f in selectedConflict.OrderedFunnelClients)
                    {
                        lstFunnel.Items.Add(f);
                        i++;
                    }
                    lstFunnel.Items.Add("- Funnel Out -");
                    unselectableFunnelLines.Add(i);   
                }
                foreach (string c in selectedConflict.ClientsRules.Keys)
                {
                    if (!selectedConflict.Funnel.ContainsKey(c)) lstFunnel.Items.Add(c);
                }
                

                if (selectedFunnel != "")
                {
                    for (i = 0; i < lstFunnel.Items.Count; i++)
                    {
                        if (lstFunnel.Items[i].ToString() == selectedFunnel)
                        {
                            lstFunnel.SelectedIndex = i;
                            break;
                        }
                    }
                }
                RefreshSelectedFunnel();
            }
        }

        private void RefreshSelectedFunnel()
        {
            if (selectedFunnel=="")
            {
                btnFunnelDown.Enabled = false;
                btnFunnelUp.Enabled = false;
                btnSetFunnel.Enabled = false;
                btnRemoveFunnel.Enabled = false;
            }
            else if (selectedConflict.Funnel.ContainsKey(selectedFunnel))
            {
                btnFunnelUp.Enabled = true;
                btnFunnelDown.Enabled = true;
                btnSetFunnel.Enabled = false;
                btnRemoveFunnel.Enabled = true;
            }
            else
            {
                btnFunnelDown.Enabled = false;
                btnFunnelUp.Enabled = false;
                btnSetFunnel.Enabled = true;
                btnRemoveFunnel.Enabled = false;
            }
        }

        public void SetVisible(bool state)
        {
            if (state)
            {
                Visible = true;
            }
            else
            {
                Visible = false;
            }
        }

        internal void ConflictsChanged(Character character)
        {
            RefreshInfo();
        }

        private void frmConflictResolution_Load(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        private void btnForceRedetect_Click(object sender, EventArgs e)
        {
            ConflictManager.Instance.DetectConflicts(MainForm.SelectedCharacter);
        }

        private void clbConflictRules_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void radActionSubscribersIgnore_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionSubscriptionConflict = ConflictSettings.DefaultRuleType.Ignore;
        }

        private void radActionSubscribersNone_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionSubscriptionConflict = ConflictSettings.DefaultRuleType.None;
        }

        private void radActionSubscribersAll_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionSubscriptionConflict = ConflictSettings.DefaultRuleType.All;
        }

        private void radActionPublishersIgnore_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionPublishingConflict = ConflictSettings.DefaultRuleType.Ignore;
        }

        private void radActionPublishersNone_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionPublishingConflict = ConflictSettings.DefaultRuleType.None;
        }

        private void radActionPublishersAll_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnActionPublishingConflict = ConflictSettings.DefaultRuleType.All;
        }

        private void radPerceptionSubscribersIgnore_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionSubscriptionConflict = ConflictSettings.DefaultRuleType.Ignore;
        }

        private void radPerceptionSubscribersNone_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionSubscriptionConflict = ConflictSettings.DefaultRuleType.None;
        }

        private void radPerceptionSubscribersAll_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionSubscriptionConflict = ConflictSettings.DefaultRuleType.All;
        }

        private void radPerceptionPublishersIgnore_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionPublishingConflict = ConflictSettings.DefaultRuleType.Ignore;
        }

        private void radPerceptionPublishersNone_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionPublishingConflict = ConflictSettings.DefaultRuleType.None;
        }

        private void radPerceptionPublishersAll_CheckedChanged(object sender, EventArgs e)
        {
            ConflictManager.Instance.DefaultRuleOnPerceptionPublishingConflict = ConflictSettings.DefaultRuleType.All;
        }

        private void clbConflictRules_SelectedValueChanged(object sender, EventArgs e)
        {
            if (dontRefresh) return;
            if (clbConflictRules.SelectedIndex == -1 || selectedConflict==null) return;
            string clientName = clbConflictRules.SelectedItem.ToString();
            if (clientName.StartsWith("UNSOLVED:")) clientName.Substring("UNSOLVED:".Length);
            ConflictManager.Instance.SetRule(MainForm.SelectedCharacter, selectedConflict.FullEventName, selectedConflict.ConflictType, clientName, clbConflictRules.GetItemChecked(clbConflictRules.SelectedIndex));
        }

        private void tabConflictRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontRefresh) return;
            RefreshSelectedConflict();
        }

        private void btnSetFunnel_Click(object sender, EventArgs e)
        {
            if (MainForm.SelectedCharacter != null && selectedConflict != null && selectedFunnel != "")
            {
                ConflictManager.Instance.SetFunnel(MainForm.SelectedCharacter, selectedConflict, selectedFunnel);
                RefreshSelectedConflict();
            }
        }

        private void lstFunnel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dontRefresh) return;
            if (unselectableFunnelLines.Contains(lstFunnel.SelectedIndex))
            {
                selectedFunnel = "";
            }
            else
            {
                selectedFunnel = lstFunnel.SelectedItem.ToString();
            }
            RefreshSelectedFunnel();
        }

        private void btnRemoveFunnel_Click(object sender, EventArgs e)
        {
            if (selectedConflict != null && selectedFunnel != "")
            {
                selectedConflict.RemoveFunnel(selectedFunnel);
                RefreshSelectedConflict();
            }
        }

        private void btnFunnelUp_Click(object sender, EventArgs e)
        {
            if (selectedConflict != null && selectedFunnel != "")
            {
                selectedConflict.FunnelUp(selectedFunnel);
                RefreshSelectedConflict();
            }
        }

        private void btnFunnelDown_Click(object sender, EventArgs e)
        {
            if (selectedConflict != null && selectedFunnel != "")
            {
                selectedConflict.FunnelDown(selectedFunnel);
                RefreshSelectedConflict();
            }
        }
    }
}
