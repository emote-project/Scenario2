namespace Thalamus
{
    partial class frmConflictResolution
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstConflicts = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radActionSubscribersIgnore = new System.Windows.Forms.RadioButton();
            this.radActionSubscribersNone = new System.Windows.Forms.RadioButton();
            this.radActionSubscribersAll = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radActionPublishersIgnore = new System.Windows.Forms.RadioButton();
            this.radActionPublishersNone = new System.Windows.Forms.RadioButton();
            this.radActionPublishersAll = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtConflictRulesFile = new System.Windows.Forms.TextBox();
            this.btnLoadConflictRules = new System.Windows.Forms.Button();
            this.lblConflictRules = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radPerceptionPublishersIgnore = new System.Windows.Forms.RadioButton();
            this.radPerceptionPublishersNone = new System.Windows.Forms.RadioButton();
            this.radPerceptionPublishersAll = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radPerceptionSubscribersIgnore = new System.Windows.Forms.RadioButton();
            this.radPerceptionSubscribersNone = new System.Windows.Forms.RadioButton();
            this.radPerceptionSubscribersAll = new System.Windows.Forms.RadioButton();
            this.btnForceRedetect = new System.Windows.Forms.Button();
            this.tabConflictRule = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnConflictRulesNone = new System.Windows.Forms.Button();
            this.btnConflictRulesAll = new System.Windows.Forms.Button();
            this.clbConflictRules = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnRemoveFunnel = new System.Windows.Forms.Button();
            this.btnSetFunnel = new System.Windows.Forms.Button();
            this.btnFunnelDown = new System.Windows.Forms.Button();
            this.btnFunnelUp = new System.Windows.Forms.Button();
            this.lstFunnel = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabConflictRule.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstConflicts
            // 
            this.lstConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstConflicts.FormattingEnabled = true;
            this.lstConflicts.Location = new System.Drawing.Point(226, 28);
            this.lstConflicts.Name = "lstConflicts";
            this.lstConflicts.Size = new System.Drawing.Size(223, 69);
            this.lstConflicts.TabIndex = 0;
            this.lstConflicts.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radActionSubscribersIgnore);
            this.groupBox1.Controls.Add(this.radActionSubscribersNone);
            this.groupBox1.Controls.Add(this.radActionSubscribersAll);
            this.groupBox1.Location = new System.Drawing.Point(12, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 47);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Action Multi-Subscribers";
            // 
            // radActionSubscribersIgnore
            // 
            this.radActionSubscribersIgnore.AutoSize = true;
            this.radActionSubscribersIgnore.Location = new System.Drawing.Point(18, 19);
            this.radActionSubscribersIgnore.Name = "radActionSubscribersIgnore";
            this.radActionSubscribersIgnore.Size = new System.Drawing.Size(55, 17);
            this.radActionSubscribersIgnore.TabIndex = 6;
            this.radActionSubscribersIgnore.TabStop = true;
            this.radActionSubscribersIgnore.Text = "Ignore";
            this.radActionSubscribersIgnore.UseVisualStyleBackColor = true;
            this.radActionSubscribersIgnore.CheckedChanged += new System.EventHandler(this.radActionSubscribersIgnore_CheckedChanged);
            // 
            // radActionSubscribersNone
            // 
            this.radActionSubscribersNone.AutoSize = true;
            this.radActionSubscribersNone.Location = new System.Drawing.Point(79, 19);
            this.radActionSubscribersNone.Name = "radActionSubscribersNone";
            this.radActionSubscribersNone.Size = new System.Drawing.Size(51, 17);
            this.radActionSubscribersNone.TabIndex = 4;
            this.radActionSubscribersNone.TabStop = true;
            this.radActionSubscribersNone.Text = "None";
            this.radActionSubscribersNone.UseVisualStyleBackColor = true;
            this.radActionSubscribersNone.CheckedChanged += new System.EventHandler(this.radActionSubscribersNone_CheckedChanged);
            // 
            // radActionSubscribersAll
            // 
            this.radActionSubscribersAll.AutoSize = true;
            this.radActionSubscribersAll.Location = new System.Drawing.Point(145, 19);
            this.radActionSubscribersAll.Name = "radActionSubscribersAll";
            this.radActionSubscribersAll.Size = new System.Drawing.Size(36, 17);
            this.radActionSubscribersAll.TabIndex = 3;
            this.radActionSubscribersAll.TabStop = true;
            this.radActionSubscribersAll.Text = "All";
            this.radActionSubscribersAll.UseVisualStyleBackColor = true;
            this.radActionSubscribersAll.CheckedChanged += new System.EventHandler(this.radActionSubscribersAll_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radActionPublishersIgnore);
            this.groupBox2.Controls.Add(this.radActionPublishersNone);
            this.groupBox2.Controls.Add(this.radActionPublishersAll);
            this.groupBox2.Location = new System.Drawing.Point(12, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 47);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default Action Multi-Publishers";
            // 
            // radActionPublishersIgnore
            // 
            this.radActionPublishersIgnore.AutoSize = true;
            this.radActionPublishersIgnore.Location = new System.Drawing.Point(18, 19);
            this.radActionPublishersIgnore.Name = "radActionPublishersIgnore";
            this.radActionPublishersIgnore.Size = new System.Drawing.Size(55, 17);
            this.radActionPublishersIgnore.TabIndex = 5;
            this.radActionPublishersIgnore.TabStop = true;
            this.radActionPublishersIgnore.Text = "Ignore";
            this.radActionPublishersIgnore.UseVisualStyleBackColor = true;
            this.radActionPublishersIgnore.CheckedChanged += new System.EventHandler(this.radActionPublishersIgnore_CheckedChanged);
            // 
            // radActionPublishersNone
            // 
            this.radActionPublishersNone.AutoSize = true;
            this.radActionPublishersNone.Location = new System.Drawing.Point(79, 19);
            this.radActionPublishersNone.Name = "radActionPublishersNone";
            this.radActionPublishersNone.Size = new System.Drawing.Size(51, 17);
            this.radActionPublishersNone.TabIndex = 4;
            this.radActionPublishersNone.TabStop = true;
            this.radActionPublishersNone.Text = "None";
            this.radActionPublishersNone.UseVisualStyleBackColor = true;
            this.radActionPublishersNone.CheckedChanged += new System.EventHandler(this.radActionPublishersNone_CheckedChanged);
            // 
            // radActionPublishersAll
            // 
            this.radActionPublishersAll.AutoSize = true;
            this.radActionPublishersAll.Location = new System.Drawing.Point(145, 19);
            this.radActionPublishersAll.Name = "radActionPublishersAll";
            this.radActionPublishersAll.Size = new System.Drawing.Size(36, 17);
            this.radActionPublishersAll.TabIndex = 3;
            this.radActionPublishersAll.TabStop = true;
            this.radActionPublishersAll.Text = "All";
            this.radActionPublishersAll.UseVisualStyleBackColor = true;
            this.radActionPublishersAll.CheckedChanged += new System.EventHandler(this.radActionPublishersAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(229, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Current Conflicts:";
            // 
            // txtConflictRulesFile
            // 
            this.txtConflictRulesFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConflictRulesFile.Location = new System.Drawing.Point(13, 251);
            this.txtConflictRulesFile.Name = "txtConflictRulesFile";
            this.txtConflictRulesFile.ReadOnly = true;
            this.txtConflictRulesFile.Size = new System.Drawing.Size(273, 20);
            this.txtConflictRulesFile.TabIndex = 6;
            // 
            // btnLoadConflictRules
            // 
            this.btnLoadConflictRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadConflictRules.Location = new System.Drawing.Point(292, 249);
            this.btnLoadConflictRules.Name = "btnLoadConflictRules";
            this.btnLoadConflictRules.Size = new System.Drawing.Size(75, 23);
            this.btnLoadConflictRules.TabIndex = 7;
            this.btnLoadConflictRules.Text = "Load Rules";
            this.btnLoadConflictRules.UseVisualStyleBackColor = true;
            // 
            // lblConflictRules
            // 
            this.lblConflictRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConflictRules.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblConflictRules.Location = new System.Drawing.Point(373, 249);
            this.lblConflictRules.Name = "lblConflictRules";
            this.lblConflictRules.Size = new System.Drawing.Size(76, 23);
            this.lblConflictRules.TabIndex = 8;
            this.lblConflictRules.Text = "- Saved -";
            this.lblConflictRules.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radPerceptionPublishersIgnore);
            this.groupBox4.Controls.Add(this.radPerceptionPublishersNone);
            this.groupBox4.Controls.Add(this.radPerceptionPublishersAll);
            this.groupBox4.Location = new System.Drawing.Point(12, 187);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(208, 47);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Default Perception Multi-Publishers";
            // 
            // radPerceptionPublishersIgnore
            // 
            this.radPerceptionPublishersIgnore.AutoSize = true;
            this.radPerceptionPublishersIgnore.Location = new System.Drawing.Point(18, 19);
            this.radPerceptionPublishersIgnore.Name = "radPerceptionPublishersIgnore";
            this.radPerceptionPublishersIgnore.Size = new System.Drawing.Size(55, 17);
            this.radPerceptionPublishersIgnore.TabIndex = 5;
            this.radPerceptionPublishersIgnore.TabStop = true;
            this.radPerceptionPublishersIgnore.Text = "Ignore";
            this.radPerceptionPublishersIgnore.UseVisualStyleBackColor = true;
            this.radPerceptionPublishersIgnore.CheckedChanged += new System.EventHandler(this.radPerceptionPublishersIgnore_CheckedChanged);
            // 
            // radPerceptionPublishersNone
            // 
            this.radPerceptionPublishersNone.AutoSize = true;
            this.radPerceptionPublishersNone.Location = new System.Drawing.Point(79, 19);
            this.radPerceptionPublishersNone.Name = "radPerceptionPublishersNone";
            this.radPerceptionPublishersNone.Size = new System.Drawing.Size(51, 17);
            this.radPerceptionPublishersNone.TabIndex = 4;
            this.radPerceptionPublishersNone.TabStop = true;
            this.radPerceptionPublishersNone.Text = "None";
            this.radPerceptionPublishersNone.UseVisualStyleBackColor = true;
            this.radPerceptionPublishersNone.CheckedChanged += new System.EventHandler(this.radPerceptionPublishersNone_CheckedChanged);
            // 
            // radPerceptionPublishersAll
            // 
            this.radPerceptionPublishersAll.AutoSize = true;
            this.radPerceptionPublishersAll.Location = new System.Drawing.Point(145, 19);
            this.radPerceptionPublishersAll.Name = "radPerceptionPublishersAll";
            this.radPerceptionPublishersAll.Size = new System.Drawing.Size(36, 17);
            this.radPerceptionPublishersAll.TabIndex = 3;
            this.radPerceptionPublishersAll.TabStop = true;
            this.radPerceptionPublishersAll.Text = "All";
            this.radPerceptionPublishersAll.UseVisualStyleBackColor = true;
            this.radPerceptionPublishersAll.CheckedChanged += new System.EventHandler(this.radPerceptionPublishersAll_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radPerceptionSubscribersIgnore);
            this.groupBox5.Controls.Add(this.radPerceptionSubscribersNone);
            this.groupBox5.Controls.Add(this.radPerceptionSubscribersAll);
            this.groupBox5.Location = new System.Drawing.Point(12, 134);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(208, 47);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Default Perception Multi-Subscribers";
            // 
            // radPerceptionSubscribersIgnore
            // 
            this.radPerceptionSubscribersIgnore.AutoSize = true;
            this.radPerceptionSubscribersIgnore.Location = new System.Drawing.Point(18, 19);
            this.radPerceptionSubscribersIgnore.Name = "radPerceptionSubscribersIgnore";
            this.radPerceptionSubscribersIgnore.Size = new System.Drawing.Size(55, 17);
            this.radPerceptionSubscribersIgnore.TabIndex = 6;
            this.radPerceptionSubscribersIgnore.TabStop = true;
            this.radPerceptionSubscribersIgnore.Text = "Ignore";
            this.radPerceptionSubscribersIgnore.UseVisualStyleBackColor = true;
            this.radPerceptionSubscribersIgnore.CheckedChanged += new System.EventHandler(this.radPerceptionSubscribersIgnore_CheckedChanged);
            // 
            // radPerceptionSubscribersNone
            // 
            this.radPerceptionSubscribersNone.AutoSize = true;
            this.radPerceptionSubscribersNone.Location = new System.Drawing.Point(79, 19);
            this.radPerceptionSubscribersNone.Name = "radPerceptionSubscribersNone";
            this.radPerceptionSubscribersNone.Size = new System.Drawing.Size(51, 17);
            this.radPerceptionSubscribersNone.TabIndex = 4;
            this.radPerceptionSubscribersNone.TabStop = true;
            this.radPerceptionSubscribersNone.Text = "None";
            this.radPerceptionSubscribersNone.UseVisualStyleBackColor = true;
            this.radPerceptionSubscribersNone.CheckedChanged += new System.EventHandler(this.radPerceptionSubscribersNone_CheckedChanged);
            // 
            // radPerceptionSubscribersAll
            // 
            this.radPerceptionSubscribersAll.AutoSize = true;
            this.radPerceptionSubscribersAll.Location = new System.Drawing.Point(145, 19);
            this.radPerceptionSubscribersAll.Name = "radPerceptionSubscribersAll";
            this.radPerceptionSubscribersAll.Size = new System.Drawing.Size(36, 17);
            this.radPerceptionSubscribersAll.TabIndex = 3;
            this.radPerceptionSubscribersAll.TabStop = true;
            this.radPerceptionSubscribersAll.Text = "All";
            this.radPerceptionSubscribersAll.UseVisualStyleBackColor = true;
            this.radPerceptionSubscribersAll.CheckedChanged += new System.EventHandler(this.radPerceptionSubscribersAll_CheckedChanged);
            // 
            // btnForceRedetect
            // 
            this.btnForceRedetect.Location = new System.Drawing.Point(350, 5);
            this.btnForceRedetect.Name = "btnForceRedetect";
            this.btnForceRedetect.Size = new System.Drawing.Size(99, 21);
            this.btnForceRedetect.TabIndex = 11;
            this.btnForceRedetect.Text = "Force Redetect";
            this.btnForceRedetect.UseVisualStyleBackColor = true;
            this.btnForceRedetect.Click += new System.EventHandler(this.btnForceRedetect_Click);
            // 
            // tabConflictRule
            // 
            this.tabConflictRule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabConflictRule.Controls.Add(this.tabPage1);
            this.tabConflictRule.Controls.Add(this.tabPage2);
            this.tabConflictRule.Location = new System.Drawing.Point(226, 103);
            this.tabConflictRule.Name = "tabConflictRule";
            this.tabConflictRule.SelectedIndex = 0;
            this.tabConflictRule.Size = new System.Drawing.Size(223, 140);
            this.tabConflictRule.TabIndex = 12;
            this.tabConflictRule.SelectedIndexChanged += new System.EventHandler(this.tabConflictRule_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnConflictRulesNone);
            this.tabPage1.Controls.Add(this.btnConflictRulesAll);
            this.tabPage1.Controls.Add(this.clbConflictRules);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(215, 114);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Allow/Deny";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnConflictRulesNone
            // 
            this.btnConflictRulesNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConflictRulesNone.Location = new System.Drawing.Point(87, 88);
            this.btnConflictRulesNone.Name = "btnConflictRulesNone";
            this.btnConflictRulesNone.Size = new System.Drawing.Size(75, 23);
            this.btnConflictRulesNone.TabIndex = 5;
            this.btnConflictRulesNone.Text = "None";
            this.btnConflictRulesNone.UseVisualStyleBackColor = true;
            // 
            // btnConflictRulesAll
            // 
            this.btnConflictRulesAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConflictRulesAll.Location = new System.Drawing.Point(6, 88);
            this.btnConflictRulesAll.Name = "btnConflictRulesAll";
            this.btnConflictRulesAll.Size = new System.Drawing.Size(75, 23);
            this.btnConflictRulesAll.TabIndex = 4;
            this.btnConflictRulesAll.Text = "All";
            this.btnConflictRulesAll.UseVisualStyleBackColor = true;
            // 
            // clbConflictRules
            // 
            this.clbConflictRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbConflictRules.FormattingEnabled = true;
            this.clbConflictRules.Location = new System.Drawing.Point(6, 6);
            this.clbConflictRules.Name = "clbConflictRules";
            this.clbConflictRules.Size = new System.Drawing.Size(203, 79);
            this.clbConflictRules.TabIndex = 3;
            this.clbConflictRules.SelectedValueChanged += new System.EventHandler(this.clbConflictRules_SelectedValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnRemoveFunnel);
            this.tabPage2.Controls.Add(this.btnSetFunnel);
            this.tabPage2.Controls.Add(this.btnFunnelDown);
            this.tabPage2.Controls.Add(this.btnFunnelUp);
            this.tabPage2.Controls.Add(this.lstFunnel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(215, 114);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Funnel";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRemoveFunnel
            // 
            this.btnRemoveFunnel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveFunnel.Location = new System.Drawing.Point(137, 29);
            this.btnRemoveFunnel.Name = "btnRemoveFunnel";
            this.btnRemoveFunnel.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFunnel.TabIndex = 3;
            this.btnRemoveFunnel.Text = "Remove Funnel";
            this.btnRemoveFunnel.UseVisualStyleBackColor = true;
            this.btnRemoveFunnel.Click += new System.EventHandler(this.btnRemoveFunnel_Click);
            // 
            // btnSetFunnel
            // 
            this.btnSetFunnel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetFunnel.Location = new System.Drawing.Point(137, 7);
            this.btnSetFunnel.Name = "btnSetFunnel";
            this.btnSetFunnel.Size = new System.Drawing.Size(75, 23);
            this.btnSetFunnel.TabIndex = 4;
            this.btnSetFunnel.Text = "Set Funnel";
            this.btnSetFunnel.UseVisualStyleBackColor = true;
            this.btnSetFunnel.Click += new System.EventHandler(this.btnSetFunnel_Click);
            // 
            // btnFunnelDown
            // 
            this.btnFunnelDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFunnelDown.Location = new System.Drawing.Point(137, 85);
            this.btnFunnelDown.Name = "btnFunnelDown";
            this.btnFunnelDown.Size = new System.Drawing.Size(75, 23);
            this.btnFunnelDown.TabIndex = 2;
            this.btnFunnelDown.Text = "Move Down";
            this.btnFunnelDown.UseVisualStyleBackColor = true;
            this.btnFunnelDown.Click += new System.EventHandler(this.btnFunnelDown_Click);
            // 
            // btnFunnelUp
            // 
            this.btnFunnelUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFunnelUp.Location = new System.Drawing.Point(137, 63);
            this.btnFunnelUp.Name = "btnFunnelUp";
            this.btnFunnelUp.Size = new System.Drawing.Size(75, 23);
            this.btnFunnelUp.TabIndex = 1;
            this.btnFunnelUp.Text = "Move Up";
            this.btnFunnelUp.UseVisualStyleBackColor = true;
            this.btnFunnelUp.Click += new System.EventHandler(this.btnFunnelUp_Click);
            // 
            // lstFunnel
            // 
            this.lstFunnel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFunnel.FormattingEnabled = true;
            this.lstFunnel.Location = new System.Drawing.Point(6, 2);
            this.lstFunnel.Name = "lstFunnel";
            this.lstFunnel.Size = new System.Drawing.Size(125, 108);
            this.lstFunnel.TabIndex = 0;
            this.lstFunnel.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstFunnel_DrawItem);
            this.lstFunnel.SelectedIndexChanged += new System.EventHandler(this.lstFunnel_SelectedIndexChanged);
            // 
            // frmConflictResolution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 276);
            this.Controls.Add(this.tabConflictRule);
            this.Controls.Add(this.btnForceRedetect);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.lblConflictRules);
            this.Controls.Add(this.btnLoadConflictRules);
            this.Controls.Add(this.txtConflictRulesFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lstConflicts);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(469, 303);
            this.Name = "frmConflictResolution";
            this.Text = "Thalamus Conflict Resolution";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmConflictResolution_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabConflictRule.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstConflicts;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radActionSubscribersNone;
        private System.Windows.Forms.RadioButton radActionSubscribersAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radActionPublishersNone;
        private System.Windows.Forms.RadioButton radActionPublishersAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConflictRulesFile;
        private System.Windows.Forms.Button btnLoadConflictRules;
        private System.Windows.Forms.Label lblConflictRules;
        private System.Windows.Forms.RadioButton radActionSubscribersIgnore;
        private System.Windows.Forms.RadioButton radActionPublishersIgnore;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radPerceptionPublishersIgnore;
        private System.Windows.Forms.RadioButton radPerceptionPublishersNone;
        private System.Windows.Forms.RadioButton radPerceptionPublishersAll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radPerceptionSubscribersIgnore;
        private System.Windows.Forms.RadioButton radPerceptionSubscribersNone;
        private System.Windows.Forms.RadioButton radPerceptionSubscribersAll;
        private System.Windows.Forms.Button btnForceRedetect;
        private System.Windows.Forms.TabControl tabConflictRule;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnConflictRulesNone;
        private System.Windows.Forms.Button btnConflictRulesAll;
        private System.Windows.Forms.CheckedListBox clbConflictRules;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnRemoveFunnel;
        private System.Windows.Forms.Button btnFunnelDown;
        private System.Windows.Forms.Button btnFunnelUp;
        private System.Windows.Forms.ListBox lstFunnel;
        private System.Windows.Forms.Button btnSetFunnel;
    }
}