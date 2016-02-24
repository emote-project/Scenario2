namespace Thalamus
{
    partial class frmThalamus
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
            this.lstCharacters = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCharacter = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rlbNetworks = new System.Windows.Forms.RadioListBox();
            this.chkConflictWindow = new System.Windows.Forms.CheckBox();
            this.chkCSVLog = new System.Windows.Forms.CheckBox();
            this.lblPerformanceMaxRate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstBodyServer = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCharacterPort = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.btnCreateCharacter = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPerformanceMessageRate = new System.Windows.Forms.Label();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.btnLoadSelected = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.lstBehaviors = new System.Windows.Forms.ListBox();
            this.grpBml = new System.Windows.Forms.GroupBox();
            this.txtBml = new System.Windows.Forms.TextBox();
            this.btnCancelAllBehaviors = new System.Windows.Forms.Button();
            this.btnRunBML = new System.Windows.Forms.Button();
            this.tabPlan = new System.Windows.Forms.TabPage();
            this.chkEventLog = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpSendAction = new System.Windows.Forms.GroupBox();
            this.lstSendAction = new System.Windows.Forms.ListBox();
            this.dgvPerception = new System.Windows.Forms.DataGridView();
            this.btnActionName = new System.Windows.Forms.Button();
            this.dgvAction = new System.Windows.Forms.DataGridView();
            this.btnSendPerception = new System.Windows.Forms.Button();
            this.grpSendPerception = new System.Windows.Forms.GroupBox();
            this.lstSendPerception = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnPlanSendEvent = new System.Windows.Forms.Button();
            this.txtPlanSendEvent = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.wpfGraph = new System.Windows.Forms.Integration.ElementHost();
            this.graphSharpControl1 = new Thalamus.GraphSharpControl();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSaveScenario = new System.Windows.Forms.Button();
            this.btnLoadScenario = new System.Windows.Forms.Button();
            this.lstScenario = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDeleteScenario = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabCharacter.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabBehavior.SuspendLayout();
            this.grpBml.SuspendLayout();
            this.tabPlan.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpSendAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerception)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAction)).BeginInit();
            this.grpSendPerception.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstCharacters
            // 
            this.lstCharacters.FormattingEnabled = true;
            this.lstCharacters.Location = new System.Drawing.Point(12, 28);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(151, 95);
            this.lstCharacters.TabIndex = 0;
            this.lstCharacters.Click += new System.EventHandler(this.lstCharacters_Click);
            this.lstCharacters.SelectedIndexChanged += new System.EventHandler(this.lstCharacters_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabCharacter);
            this.tabControl1.Controls.Add(this.tabBehavior);
            this.tabControl1.Controls.Add(this.tabPlan);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(169, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(556, 271);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabCharacter
            // 
            this.tabCharacter.Controls.Add(this.groupBox2);
            this.tabCharacter.Controls.Add(this.chkConflictWindow);
            this.tabCharacter.Controls.Add(this.chkCSVLog);
            this.tabCharacter.Controls.Add(this.lblPerformanceMaxRate);
            this.tabCharacter.Controls.Add(this.label3);
            this.tabCharacter.Controls.Add(this.lstBodyServer);
            this.tabCharacter.Controls.Add(this.groupBox1);
            this.tabCharacter.Controls.Add(this.lblPerformanceMessageRate);
            this.tabCharacter.Location = new System.Drawing.Point(4, 22);
            this.tabCharacter.Name = "tabCharacter";
            this.tabCharacter.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharacter.Size = new System.Drawing.Size(548, 245);
            this.tabCharacter.TabIndex = 1;
            this.tabCharacter.Text = "Character";
            this.tabCharacter.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rlbNetworks);
            this.groupBox2.Location = new System.Drawing.Point(380, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 69);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Network";
            // 
            // rlbNetworks
            // 
            this.rlbNetworks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rlbNetworks.BackColor = System.Drawing.SystemColors.Window;
            this.rlbNetworks.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.rlbNetworks.FormattingEnabled = true;
            this.rlbNetworks.Location = new System.Drawing.Point(6, 19);
            this.rlbNetworks.Name = "rlbNetworks";
            this.rlbNetworks.Size = new System.Drawing.Size(150, 43);
            this.rlbNetworks.TabIndex = 0;
            this.rlbNetworks.SelectedIndexChanged += new System.EventHandler(this.rlbNetworks_SelectedIndexChanged);
            // 
            // chkConflictWindow
            // 
            this.chkConflictWindow.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkConflictWindow.Location = new System.Drawing.Point(379, 82);
            this.chkConflictWindow.Name = "chkConflictWindow";
            this.chkConflictWindow.Size = new System.Drawing.Size(163, 23);
            this.chkConflictWindow.TabIndex = 17;
            this.chkConflictWindow.Text = "Conflict Management";
            this.chkConflictWindow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkConflictWindow.UseVisualStyleBackColor = true;
            this.chkConflictWindow.CheckedChanged += new System.EventHandler(this.chkConflictWindow_CheckedChanged);
            // 
            // chkCSVLog
            // 
            this.chkCSVLog.AutoSize = true;
            this.chkCSVLog.Location = new System.Drawing.Point(159, 72);
            this.chkCSVLog.Name = "chkCSVLog";
            this.chkCSVLog.Size = new System.Drawing.Size(80, 17);
            this.chkCSVLog.TabIndex = 16;
            this.chkCSVLog.Text = "Log to CSV";
            this.chkCSVLog.UseVisualStyleBackColor = true;
            this.chkCSVLog.CheckedChanged += new System.EventHandler(this.chkCSVLog_CheckedChanged);
            // 
            // lblPerformanceMaxRate
            // 
            this.lblPerformanceMaxRate.AutoSize = true;
            this.lblPerformanceMaxRate.Location = new System.Drawing.Point(156, 31);
            this.lblPerformanceMaxRate.Name = "lblPerformanceMaxRate";
            this.lblPerformanceMaxRate.Size = new System.Drawing.Size(195, 13);
            this.lblPerformanceMaxRate.TabIndex = 15;
            this.lblPerformanceMaxRate.Text = "Max rate (/sec): Inbound 0; Outbound 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Connected clients:";
            // 
            // lstBodyServer
            // 
            this.lstBodyServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBodyServer.Enabled = false;
            this.lstBodyServer.FormattingEnabled = true;
            this.lstBodyServer.HorizontalScrollbar = true;
            this.lstBodyServer.Location = new System.Drawing.Point(6, 110);
            this.lstBodyServer.Name = "lstBodyServer";
            this.lstBodyServer.Size = new System.Drawing.Size(536, 121);
            this.lstBodyServer.TabIndex = 10;
            this.lstBodyServer.SelectedIndexChanged += new System.EventHandler(this.lstBodyServer_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCharacterPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.btnCreateCharacter);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Location = new System.Drawing.Point(3, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 82);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Character";
            // 
            // lblCharacterPort
            // 
            this.lblCharacterPort.AutoSize = true;
            this.lblCharacterPort.Location = new System.Drawing.Point(111, 52);
            this.lblCharacterPort.Name = "lblCharacterPort";
            this.lblCharacterPort.Size = new System.Drawing.Size(24, 13);
            this.lblCharacterPort.TabIndex = 10;
            this.lblCharacterPort.Text = "n/a";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 23);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name:";
            // 
            // btnCreateCharacter
            // 
            this.btnCreateCharacter.Location = new System.Drawing.Point(9, 45);
            this.btnCreateCharacter.Name = "btnCreateCharacter";
            this.btnCreateCharacter.Size = new System.Drawing.Size(70, 23);
            this.btnCreateCharacter.TabIndex = 6;
            this.btnCreateCharacter.Text = "Create";
            this.btnCreateCharacter.UseVisualStyleBackColor = true;
            this.btnCreateCharacter.Click += new System.EventHandler(this.btnCreateCharacter_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(45, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(90, 20);
            this.txtName.TabIndex = 8;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblPerformanceMessageRate
            // 
            this.lblPerformanceMessageRate.AutoSize = true;
            this.lblPerformanceMessageRate.Location = new System.Drawing.Point(156, 15);
            this.lblPerformanceMessageRate.Name = "lblPerformanceMessageRate";
            this.lblPerformanceMessageRate.Size = new System.Drawing.Size(218, 13);
            this.lblPerformanceMessageRate.TabIndex = 14;
            this.lblPerformanceMessageRate.Text = "Message rate (/sec): Inbound 0; Outbound 0";
            // 
            // tabBehavior
            // 
            this.tabBehavior.Controls.Add(this.label7);
            this.tabBehavior.Controls.Add(this.btnLoadSelected);
            this.tabBehavior.Controls.Add(this.btnReload);
            this.tabBehavior.Controls.Add(this.lstBehaviors);
            this.tabBehavior.Controls.Add(this.grpBml);
            this.tabBehavior.Location = new System.Drawing.Point(4, 22);
            this.tabBehavior.Name = "tabBehavior";
            this.tabBehavior.Padding = new System.Windows.Forms.Padding(3);
            this.tabBehavior.Size = new System.Drawing.Size(548, 245);
            this.tabBehavior.TabIndex = 2;
            this.tabBehavior.Text = "BML";
            this.tabBehavior.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(389, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Available Behaviors:";
            // 
            // btnLoadSelected
            // 
            this.btnLoadSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadSelected.Location = new System.Drawing.Point(389, 187);
            this.btnLoadSelected.Name = "btnLoadSelected";
            this.btnLoadSelected.Size = new System.Drawing.Size(153, 23);
            this.btnLoadSelected.TabIndex = 9;
            this.btnLoadSelected.Text = "Load Selected";
            this.btnLoadSelected.UseVisualStyleBackColor = true;
            this.btnLoadSelected.Click += new System.EventHandler(this.btnLoadSelected_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Location = new System.Drawing.Point(389, 216);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(153, 23);
            this.btnReload.TabIndex = 8;
            this.btnReload.Text = "Reload BML Files";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // lstBehaviors
            // 
            this.lstBehaviors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBehaviors.FormattingEnabled = true;
            this.lstBehaviors.Location = new System.Drawing.Point(389, 22);
            this.lstBehaviors.Name = "lstBehaviors";
            this.lstBehaviors.Size = new System.Drawing.Size(153, 147);
            this.lstBehaviors.TabIndex = 7;
            this.lstBehaviors.SelectedIndexChanged += new System.EventHandler(this.lstBehaviors_SelectedIndexChanged);
            this.lstBehaviors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstBehaviors_MouseDoubleClick);
            // 
            // grpBml
            // 
            this.grpBml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBml.Controls.Add(this.txtBml);
            this.grpBml.Controls.Add(this.btnCancelAllBehaviors);
            this.grpBml.Controls.Add(this.btnRunBML);
            this.grpBml.Location = new System.Drawing.Point(3, 3);
            this.grpBml.Name = "grpBml";
            this.grpBml.Size = new System.Drawing.Size(380, 236);
            this.grpBml.TabIndex = 5;
            this.grpBml.TabStop = false;
            this.grpBml.Text = "BML Control";
            // 
            // txtBml
            // 
            this.txtBml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBml.Location = new System.Drawing.Point(6, 19);
            this.txtBml.Multiline = true;
            this.txtBml.Name = "txtBml";
            this.txtBml.Size = new System.Drawing.Size(368, 183);
            this.txtBml.TabIndex = 11;
            // 
            // btnCancelAllBehaviors
            // 
            this.btnCancelAllBehaviors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelAllBehaviors.Location = new System.Drawing.Point(87, 207);
            this.btnCancelAllBehaviors.Name = "btnCancelAllBehaviors";
            this.btnCancelAllBehaviors.Size = new System.Drawing.Size(75, 23);
            this.btnCancelAllBehaviors.TabIndex = 10;
            this.btnCancelAllBehaviors.Text = "Cancel All";
            this.btnCancelAllBehaviors.UseVisualStyleBackColor = true;
            this.btnCancelAllBehaviors.Click += new System.EventHandler(this.btnCancelAllBehaviors_Click);
            // 
            // btnRunBML
            // 
            this.btnRunBML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRunBML.Location = new System.Drawing.Point(6, 207);
            this.btnRunBML.Name = "btnRunBML";
            this.btnRunBML.Size = new System.Drawing.Size(75, 23);
            this.btnRunBML.TabIndex = 7;
            this.btnRunBML.Text = "Run BML";
            this.btnRunBML.UseVisualStyleBackColor = true;
            this.btnRunBML.Click += new System.EventHandler(this.btnRunBML_Click);
            // 
            // tabPlan
            // 
            this.tabPlan.Controls.Add(this.chkEventLog);
            this.tabPlan.Controls.Add(this.tableLayoutPanel1);
            this.tabPlan.Controls.Add(this.groupBox3);
            this.tabPlan.Location = new System.Drawing.Point(4, 22);
            this.tabPlan.Name = "tabPlan";
            this.tabPlan.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlan.Size = new System.Drawing.Size(548, 245);
            this.tabPlan.TabIndex = 6;
            this.tabPlan.Text = "Events";
            this.tabPlan.UseVisualStyleBackColor = true;
            // 
            // chkEventLog
            // 
            this.chkEventLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkEventLog.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkEventLog.Location = new System.Drawing.Point(12, 210);
            this.chkEventLog.Name = "chkEventLog";
            this.chkEventLog.Size = new System.Drawing.Size(128, 24);
            this.chkEventLog.TabIndex = 6;
            this.chkEventLog.Text = "Event Log";
            this.chkEventLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkEventLog.UseVisualStyleBackColor = true;
            this.chkEventLog.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.grpSendAction, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvPerception, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnActionName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.dgvAction, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSendPerception, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.grpSendPerception, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(161, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(386, 229);
            this.tableLayoutPanel1.TabIndex = 5;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // grpSendAction
            // 
            this.grpSendAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSendAction.Controls.Add(this.lstSendAction);
            this.grpSendAction.Location = new System.Drawing.Point(196, 4);
            this.grpSendAction.Name = "grpSendAction";
            this.grpSendAction.Size = new System.Drawing.Size(186, 111);
            this.grpSendAction.TabIndex = 4;
            this.grpSendAction.TabStop = false;
            this.grpSendAction.Text = "Actions";
            // 
            // lstSendAction
            // 
            this.lstSendAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSendAction.FormattingEnabled = true;
            this.lstSendAction.Location = new System.Drawing.Point(6, 19);
            this.lstSendAction.Name = "lstSendAction";
            this.lstSendAction.Size = new System.Drawing.Size(174, 82);
            this.lstSendAction.TabIndex = 9;
            this.lstSendAction.SelectedIndexChanged += new System.EventHandler(this.lstSendAction_SelectedIndexChanged);
            // 
            // dgvPerception
            // 
            this.dgvPerception.AllowUserToAddRows = false;
            this.dgvPerception.AllowUserToDeleteRows = false;
            this.dgvPerception.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPerception.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPerception.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerception.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvPerception.Location = new System.Drawing.Point(4, 122);
            this.dgvPerception.MultiSelect = false;
            this.dgvPerception.Name = "dgvPerception";
            this.dgvPerception.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvPerception.Size = new System.Drawing.Size(185, 72);
            this.dgvPerception.TabIndex = 7;
            // 
            // btnActionName
            // 
            this.btnActionName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnActionName.Location = new System.Drawing.Point(196, 202);
            this.btnActionName.Name = "btnActionName";
            this.btnActionName.Size = new System.Drawing.Size(185, 23);
            this.btnActionName.TabIndex = 3;
            this.btnActionName.Text = "Send";
            this.btnActionName.UseVisualStyleBackColor = true;
            this.btnActionName.Click += new System.EventHandler(this.btnActionName_Click);
            // 
            // dgvAction
            // 
            this.dgvAction.AllowUserToAddRows = false;
            this.dgvAction.AllowUserToDeleteRows = false;
            this.dgvAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAction.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAction.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAction.Location = new System.Drawing.Point(196, 122);
            this.dgvAction.Name = "dgvAction";
            this.dgvAction.Size = new System.Drawing.Size(186, 72);
            this.dgvAction.TabIndex = 6;
            this.dgvAction.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAction_CellContentClick);
            // 
            // btnSendPerception
            // 
            this.btnSendPerception.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSendPerception.Location = new System.Drawing.Point(4, 202);
            this.btnSendPerception.Name = "btnSendPerception";
            this.btnSendPerception.Size = new System.Drawing.Size(185, 23);
            this.btnSendPerception.TabIndex = 3;
            this.btnSendPerception.Text = "Send";
            this.btnSendPerception.UseVisualStyleBackColor = true;
            this.btnSendPerception.Click += new System.EventHandler(this.btnSendPerception_Click);
            // 
            // grpSendPerception
            // 
            this.grpSendPerception.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSendPerception.Controls.Add(this.lstSendPerception);
            this.grpSendPerception.Location = new System.Drawing.Point(4, 4);
            this.grpSendPerception.Name = "grpSendPerception";
            this.grpSendPerception.Size = new System.Drawing.Size(185, 111);
            this.grpSendPerception.TabIndex = 3;
            this.grpSendPerception.TabStop = false;
            this.grpSendPerception.Text = "Perceptions";
            // 
            // lstSendPerception
            // 
            this.lstSendPerception.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSendPerception.FormattingEnabled = true;
            this.lstSendPerception.Location = new System.Drawing.Point(6, 19);
            this.lstSendPerception.Name = "lstSendPerception";
            this.lstSendPerception.Size = new System.Drawing.Size(173, 82);
            this.lstSendPerception.TabIndex = 8;
            this.lstSendPerception.SelectedIndexChanged += new System.EventHandler(this.lstSendPerception_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.btnPlanSendEvent);
            this.groupBox3.Controls.Add(this.txtPlanSendEvent);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(149, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "BML";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "BML Event:";
            // 
            // btnPlanSendEvent
            // 
            this.btnPlanSendEvent.Location = new System.Drawing.Point(6, 62);
            this.btnPlanSendEvent.Name = "btnPlanSendEvent";
            this.btnPlanSendEvent.Size = new System.Drawing.Size(128, 23);
            this.btnPlanSendEvent.TabIndex = 0;
            this.btnPlanSendEvent.Text = "Send";
            this.btnPlanSendEvent.UseVisualStyleBackColor = true;
            this.btnPlanSendEvent.Click += new System.EventHandler(this.btnPlanSendEvent_Click);
            // 
            // txtPlanSendEvent
            // 
            this.txtPlanSendEvent.Location = new System.Drawing.Point(6, 36);
            this.txtPlanSendEvent.Name = "txtPlanSendEvent";
            this.txtPlanSendEvent.Size = new System.Drawing.Size(128, 20);
            this.txtPlanSendEvent.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.wpfGraph);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(548, 245);
            this.tabPage1.TabIndex = 7;
            this.tabPage1.Text = "Graph";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // wpfGraph
            // 
            this.wpfGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wpfGraph.Location = new System.Drawing.Point(0, 0);
            this.wpfGraph.Name = "wpfGraph";
            this.wpfGraph.Size = new System.Drawing.Size(548, 245);
            this.wpfGraph.TabIndex = 1;
            this.wpfGraph.Text = "elementHost2";
            this.wpfGraph.Child = this.graphSharpControl1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 126);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Scenarios:";
            // 
            // btnSaveScenario
            // 
            this.btnSaveScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveScenario.Location = new System.Drawing.Point(69, 256);
            this.btnSaveScenario.Name = "btnSaveScenario";
            this.btnSaveScenario.Size = new System.Drawing.Size(44, 23);
            this.btnSaveScenario.TabIndex = 8;
            this.btnSaveScenario.Text = "Save";
            this.btnSaveScenario.UseVisualStyleBackColor = true;
            this.btnSaveScenario.Click += new System.EventHandler(this.btnSaveScenario_Click);
            // 
            // btnLoadScenario
            // 
            this.btnLoadScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadScenario.Location = new System.Drawing.Point(119, 256);
            this.btnLoadScenario.Name = "btnLoadScenario";
            this.btnLoadScenario.Size = new System.Drawing.Size(44, 23);
            this.btnLoadScenario.TabIndex = 7;
            this.btnLoadScenario.Text = "Load";
            this.btnLoadScenario.UseVisualStyleBackColor = true;
            this.btnLoadScenario.Click += new System.EventHandler(this.btnLoadScenario_Click);
            // 
            // lstScenario
            // 
            this.lstScenario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstScenario.FormattingEnabled = true;
            this.lstScenario.Location = new System.Drawing.Point(12, 142);
            this.lstScenario.Name = "lstScenario";
            this.lstScenario.Size = new System.Drawing.Size(151, 95);
            this.lstScenario.TabIndex = 3;
            this.lstScenario.SelectedIndexChanged += new System.EventHandler(this.lstPresets_SelectedIndexChanged);
            this.lstScenario.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstPresets_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Characters:";
            // 
            // btnDeleteScenario
            // 
            this.btnDeleteScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteScenario.Location = new System.Drawing.Point(12, 256);
            this.btnDeleteScenario.Name = "btnDeleteScenario";
            this.btnDeleteScenario.Size = new System.Drawing.Size(51, 23);
            this.btnDeleteScenario.TabIndex = 9;
            this.btnDeleteScenario.Text = "Delete";
            this.btnDeleteScenario.UseVisualStyleBackColor = true;
            this.btnDeleteScenario.Click += new System.EventHandler(this.btnDeleteScenario_Click);
            // 
            // frmThalamus
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(751, 298);
            this.Controls.Add(this.btnDeleteScenario);
            this.Controls.Add(this.btnSaveScenario);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadScenario);
            this.Controls.Add(this.lstCharacters);
            this.Controls.Add(this.lstScenario);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(759, 315);
            this.Name = "frmThalamus";
            this.Text = "Thalamus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmThalamus_FormClosing);
            this.Load += new System.EventHandler(this.frmThalamus_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCharacter.ResumeLayout(false);
            this.tabCharacter.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabBehavior.ResumeLayout(false);
            this.tabBehavior.PerformLayout();
            this.grpBml.ResumeLayout(false);
            this.grpBml.PerformLayout();
            this.tabPlan.ResumeLayout(false);
            this.tabPlan.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grpSendAction.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerception)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAction)).EndInit();
            this.grpSendPerception.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstCharacters;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCharacter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabBehavior;
        private System.Windows.Forms.TabPage tabPlan;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnCreateCharacter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstBodyServer;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox lstScenario;
        private System.Windows.Forms.Button btnLoadScenario;
        private System.Windows.Forms.Button btnSaveScenario;
        private System.Windows.Forms.GroupBox grpBml;
        private System.Windows.Forms.Button btnCancelAllBehaviors;
        private System.Windows.Forms.Button btnRunBML;
        private System.Windows.Forms.Button btnLoadSelected;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.ListBox lstBehaviors;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBml;
        private System.Windows.Forms.Label lblCharacterPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnPlanSendEvent;
        private System.Windows.Forms.TextBox txtPlanSendEvent;
        private System.Windows.Forms.GroupBox grpSendPerception;
        private System.Windows.Forms.GroupBox grpSendAction;
        private System.Windows.Forms.Button btnActionName;
        private System.Windows.Forms.Button btnSendPerception;
        private System.Windows.Forms.DataGridView dgvPerception;
        private System.Windows.Forms.ListBox lstSendAction;
        private System.Windows.Forms.ListBox lstSendPerception;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnDeleteScenario;
        private System.Windows.Forms.CheckBox chkEventLog;
        private System.Windows.Forms.Label lblPerformanceMessageRate;
        private System.Windows.Forms.DataGridView dgvAction;
        private System.Windows.Forms.Label lblPerformanceMaxRate;
        private System.Windows.Forms.CheckBox chkCSVLog;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Integration.ElementHost wpfGraph;
        private GraphSharpControl graphSharpControl1;
        private System.Windows.Forms.CheckBox chkConflictWindow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioListBox rlbNetworks;
    }
}

