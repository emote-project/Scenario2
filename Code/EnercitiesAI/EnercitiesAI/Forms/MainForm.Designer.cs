using System.Windows.Forms;

namespace EnercitiesAI.Forms
{
    partial class MainForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("action 1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("action 2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("action 3");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("action 4");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("action 5");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("action 6");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("action 7");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("action 8");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("action 9");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.clientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.planToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.playBestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skipTurnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.autoPlayBestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxAITimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBoxPlanTime = new System.Windows.Forms.ToolStripComboBox();
            this.planDepthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBoxPlanDepth = new System.Windows.Forms.ToolStripComboBox();
            this.numCPUsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBoxPlanCPUs = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.imitationFactorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBoxImitFactor = new System.Windows.Forms.ToolStripComboBox();
            this.stratlearningRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cBoxStratLearn = new System.Windows.Forms.ToolStripComboBox();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.worldGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.strategyControl1 = new EnercitiesAI.Forms.StrategyControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.avgTimeStateTxtBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timeTakenTxtBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.statesVisitedTxtBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.expandedStatesTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.predictedValuesTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bestActionsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bestActionTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gameInfoControl = new EnercitiesAI.Forms.GameInfoControl();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "json";
            this.openFileDialog.FileName = "game.json";
            this.openFileDialog.Filter = "Play history Json files|*.json";
            this.openFileDialog.RestoreDirectory = true;
            this.openFileDialog.Title = "Select the file where the history of plays is stored";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clientToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(834, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            // 
            // clientToolStripMenuItem
            // 
            this.clientToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGameHistoryToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.clientToolStripMenuItem.Name = "clientToolStripMenuItem";
            this.clientToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.clientToolStripMenuItem.Text = "&Client";
            // 
            // loadGameHistoryToolStripMenuItem
            // 
            this.loadGameHistoryToolStripMenuItem.Enabled = false;
            this.loadGameHistoryToolStripMenuItem.Name = "loadGameHistoryToolStripMenuItem";
            this.loadGameHistoryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadGameHistoryToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.loadGameHistoryToolStripMenuItem.Text = "&Load game history...";
            this.loadGameHistoryToolStripMenuItem.Click += new System.EventHandler(this.LoadGameHistoryToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.planToolStripMenuItem,
            this.toolStripSeparator2,
            this.playBestToolStripMenuItem,
            this.skipTurnToolStripMenuItem,
            this.toolStripSeparator3,
            this.autoPlayBestToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "&Action";
            // 
            // planToolStripMenuItem
            // 
            this.planToolStripMenuItem.Name = "planToolStripMenuItem";
            this.planToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.planToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.planToolStripMenuItem.Text = "&Plan";
            this.planToolStripMenuItem.Click += new System.EventHandler(this.PlanToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(189, 6);
            // 
            // playBestToolStripMenuItem
            // 
            this.playBestToolStripMenuItem.Name = "playBestToolStripMenuItem";
            this.playBestToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.playBestToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.playBestToolStripMenuItem.Text = "Play &best";
            this.playBestToolStripMenuItem.Click += new System.EventHandler(this.PlayBestToolStripMenuItemClick);
            // 
            // skipTurnToolStripMenuItem
            // 
            this.skipTurnToolStripMenuItem.Name = "skipTurnToolStripMenuItem";
            this.skipTurnToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.skipTurnToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.skipTurnToolStripMenuItem.Text = "Skip &turn";
            this.skipTurnToolStripMenuItem.Click += new System.EventHandler(this.SkipTurnToolStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(189, 6);
            // 
            // autoPlayBestToolStripMenuItem
            // 
            this.autoPlayBestToolStripMenuItem.Name = "autoPlayBestToolStripMenuItem";
            this.autoPlayBestToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.autoPlayBestToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.autoPlayBestToolStripMenuItem.Text = "&Auto play best";
            this.autoPlayBestToolStripMenuItem.Click += new System.EventHandler(this.AutoPlayBestToolStripMenuItemClick);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maxAITimeToolStripMenuItem,
            this.planDepthToolStripMenuItem,
            this.numCPUsToolStripMenuItem,
            this.toolStripSeparator4,
            this.imitationFactorToolStripMenuItem,
            this.stratlearningRateToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // maxAITimeToolStripMenuItem
            // 
            this.maxAITimeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBoxPlanTime});
            this.maxAITimeToolStripMenuItem.Name = "maxAITimeToolStripMenuItem";
            this.maxAITimeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.maxAITimeToolStripMenuItem.Text = "Max. planning &time";
            // 
            // cBoxPlanTime
            // 
            this.cBoxPlanTime.Name = "cBoxPlanTime";
            this.cBoxPlanTime.Size = new System.Drawing.Size(121, 23);
            this.cBoxPlanTime.SelectedIndexChanged += new System.EventHandler(this.CBoxPlanTimeSelectedIdxChanged);
            // 
            // planDepthToolStripMenuItem
            // 
            this.planDepthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBoxPlanDepth});
            this.planDepthToolStripMenuItem.Name = "planDepthToolStripMenuItem";
            this.planDepthToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.planDepthToolStripMenuItem.Text = "Plan &depth";
            // 
            // cBoxPlanDepth
            // 
            this.cBoxPlanDepth.Name = "cBoxPlanDepth";
            this.cBoxPlanDepth.Size = new System.Drawing.Size(121, 23);
            this.cBoxPlanDepth.SelectedIndexChanged += new System.EventHandler(this.CBoxPlanDepthSelectedIdxChanged);
            // 
            // numCPUsToolStripMenuItem
            // 
            this.numCPUsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBoxPlanCPUs});
            this.numCPUsToolStripMenuItem.Name = "numCPUsToolStripMenuItem";
            this.numCPUsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.numCPUsToolStripMenuItem.Text = "Num. &CPUs";
            // 
            // cBoxPlanCPUs
            // 
            this.cBoxPlanCPUs.Name = "cBoxPlanCPUs";
            this.cBoxPlanCPUs.Size = new System.Drawing.Size(121, 23);
            this.cBoxPlanCPUs.SelectedIndexChanged += new System.EventHandler(this.CBoxPlanCPUsSelectedIdxChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(173, 6);
            // 
            // imitationFactorToolStripMenuItem
            // 
            this.imitationFactorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBoxImitFactor});
            this.imitationFactorToolStripMenuItem.Name = "imitationFactorToolStripMenuItem";
            this.imitationFactorToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.imitationFactorToolStripMenuItem.Text = "&Imitation factor";
            // 
            // cBoxImitFactor
            // 
            this.cBoxImitFactor.Name = "cBoxImitFactor";
            this.cBoxImitFactor.Size = new System.Drawing.Size(121, 23);
            this.cBoxImitFactor.SelectedIndexChanged += new System.EventHandler(this.CBoxImitFactorSelectedIdxChanged);
            // 
            // stratlearningRateToolStripMenuItem
            // 
            this.stratlearningRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cBoxStratLearn});
            this.stratlearningRateToolStripMenuItem.Name = "stratlearningRateToolStripMenuItem";
            this.stratlearningRateToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.stratlearningRateToolStripMenuItem.Text = "Strat. &learning rate";
            // 
            // cBoxStratLearn
            // 
            this.cBoxStratLearn.Name = "cBoxStratLearn";
            this.cBoxStratLearn.Size = new System.Drawing.Size(121, 23);
            this.cBoxStratLearn.SelectedIndexChanged += new System.EventHandler(this.CBoxStratLearnSelectedIdxChanged);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.worldGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // worldGridToolStripMenuItem
            // 
            this.worldGridToolStripMenuItem.Name = "worldGridToolStripMenuItem";
            this.worldGridToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.worldGridToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.worldGridToolStripMenuItem.Text = "&World grid";
            this.worldGridToolStripMenuItem.Click += new System.EventHandler(this.WorldGridToolStripMenuItemClick);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 402);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(834, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(819, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Some info here";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tabControl);
            this.groupBox1.Location = new System.Drawing.Point(12, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 369);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Players\' strategies";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(6, 19);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(263, 344);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.strategyControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(255, 315);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // strategyControl1
            // 
            this.strategyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.strategyControl1.Location = new System.Drawing.Point(3, 3);
            this.strategyControl1.Name = "strategyControl1";
            this.strategyControl1.Player = null;
            this.strategyControl1.Size = new System.Drawing.Size(249, 309);
            this.strategyControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(255, 315);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.groupBox7);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(293, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(529, 369);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Planning";
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.avgTimeStateTxtBox);
            this.groupBox7.Controls.Add(this.label13);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.timeTakenTxtBox);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Location = new System.Drawing.Point(6, 287);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(151, 76);
            this.groupBox7.TabIndex = 65;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Time";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(100, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 67;
            this.label12.Text = "μs/state";
            // 
            // avgTimeStateTxtBox
            // 
            this.avgTimeStateTxtBox.Location = new System.Drawing.Point(50, 48);
            this.avgTimeStateTxtBox.Name = "avgTimeStateTxtBox";
            this.avgTimeStateTxtBox.ReadOnly = true;
            this.avgTimeStateTxtBox.Size = new System.Drawing.Size(46, 20);
            this.avgTimeStateTxtBox.TabIndex = 66;
            this.avgTimeStateTxtBox.Text = "9999.99";
            this.avgTimeStateTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 65;
            this.label13.Text = "avg:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 51;
            this.label5.Text = "taken:";
            // 
            // timeTakenTxtBox
            // 
            this.timeTakenTxtBox.Location = new System.Drawing.Point(50, 22);
            this.timeTakenTxtBox.Name = "timeTakenTxtBox";
            this.timeTakenTxtBox.ReadOnly = true;
            this.timeTakenTxtBox.Size = new System.Drawing.Size(46, 20);
            this.timeTakenTxtBox.TabIndex = 54;
            this.timeTakenTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(100, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 59;
            this.label8.Text = "s";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.statesVisitedTxtBox);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.expandedStatesTxtBox);
            this.groupBox6.Location = new System.Drawing.Point(163, 287);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(118, 76);
            this.groupBox6.TabIndex = 67;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "States";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 59;
            this.label1.Text = "visited:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, -11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 55;
            this.label6.Text = "Visited states:";
            // 
            // statesVisitedTxtBox
            // 
            this.statesVisitedTxtBox.Location = new System.Drawing.Point(69, 19);
            this.statesVisitedTxtBox.Name = "statesVisitedTxtBox";
            this.statesVisitedTxtBox.ReadOnly = true;
            this.statesVisitedTxtBox.Size = new System.Drawing.Size(43, 20);
            this.statesVisitedTxtBox.TabIndex = 56;
            this.statesVisitedTxtBox.Text = "99999";
            this.statesVisitedTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 57;
            this.label7.Text = "expanded:";
            // 
            // expandedStatesTxtBox
            // 
            this.expandedStatesTxtBox.Location = new System.Drawing.Point(69, 45);
            this.expandedStatesTxtBox.Name = "expandedStatesTxtBox";
            this.expandedStatesTxtBox.ReadOnly = true;
            this.expandedStatesTxtBox.Size = new System.Drawing.Size(43, 20);
            this.expandedStatesTxtBox.TabIndex = 58;
            this.expandedStatesTxtBox.Text = "99999";
            this.expandedStatesTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.predictedValuesTxtBox);
            this.groupBox5.Location = new System.Drawing.Point(6, 194);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(275, 87);
            this.groupBox5.TabIndex = 54;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Predicted values";
            // 
            // predictedValuesTxtBox
            // 
            this.predictedValuesTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.predictedValuesTxtBox.Location = new System.Drawing.Point(9, 19);
            this.predictedValuesTxtBox.Multiline = true;
            this.predictedValuesTxtBox.Name = "predictedValuesTxtBox";
            this.predictedValuesTxtBox.ReadOnly = true;
            this.predictedValuesTxtBox.Size = new System.Drawing.Size(260, 62);
            this.predictedValuesTxtBox.TabIndex = 53;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bestActionsList);
            this.groupBox4.Controls.Add(this.bestActionTxtBox);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(275, 169);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Best actions";
            // 
            // bestActionsList
            // 
            this.bestActionsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bestActionsList.BackColor = System.Drawing.SystemColors.Control;
            this.bestActionsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.bestActionsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.bestActionsList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.bestActionsList.Location = new System.Drawing.Point(9, 45);
            this.bestActionsList.MultiSelect = false;
            this.bestActionsList.Name = "bestActionsList";
            this.bestActionsList.Size = new System.Drawing.Size(260, 118);
            this.bestActionsList.TabIndex = 48;
            this.bestActionsList.UseCompatibleStateImageBehavior = false;
            this.bestActionsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 220;
            // 
            // bestActionTxtBox
            // 
            this.bestActionTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bestActionTxtBox.Location = new System.Drawing.Point(9, 19);
            this.bestActionTxtBox.Name = "bestActionTxtBox";
            this.bestActionTxtBox.ReadOnly = true;
            this.bestActionTxtBox.Size = new System.Drawing.Size(260, 20);
            this.bestActionTxtBox.TabIndex = 50;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.gameInfoControl);
            this.groupBox3.Location = new System.Drawing.Point(287, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(236, 344);
            this.groupBox3.TabIndex = 66;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current state";
            // 
            // gameInfoControl
            // 
            this.gameInfoControl.AllowChanges = false;
            this.gameInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameInfoControl.GameInfo = ((EmoteEvents.EnercitiesGameInfo)(resources.GetObject("gameInfoControl.GameInfo")));
            this.gameInfoControl.Location = new System.Drawing.Point(3, 16);
            this.gameInfoControl.Name = "gameInfoControl";
            this.gameInfoControl.Size = new System.Drawing.Size(230, 325);
            this.gameInfoControl.TabIndex = 65;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 424);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EMOTE EnerCities AI client";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenFileDialog openFileDialog;
        private MenuStrip menuStrip;
        private ToolStripMenuItem actionToolStripMenuItem;
        private ToolStripMenuItem playBestToolStripMenuItem;
        private ToolStripMenuItem skipTurnToolStripMenuItem;
        private ToolStripMenuItem clientToolStripMenuItem;
        private ToolStripMenuItem loadGameHistoryToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem maxAITimeToolStripMenuItem;
        private ToolStripMenuItem planDepthToolStripMenuItem;
        private ToolStripMenuItem numCPUsToolStripMenuItem;
        private ToolStripComboBox cBoxPlanTime;
        private ToolStripComboBox cBoxPlanDepth;
        private ToolStripComboBox cBoxPlanCPUs;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem planToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem worldGridToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private GroupBox groupBox1;
        private TabControl tabControl;
        private TabPage tabPage1;
        private StrategyControl strategyControl1;
        private TabPage tabPage2;
        private GroupBox groupBox2;
        private TextBox predictedValuesTxtBox;
        private TextBox bestActionTxtBox;
        private ListView bestActionsList;
        private ColumnHeader columnHeader1;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private Label label5;
        private TextBox timeTakenTxtBox;
        private Label label6;
        private TextBox statesVisitedTxtBox;
        private Label label7;
        private TextBox expandedStatesTxtBox;
        private Label label8;
        private GroupBox groupBox3;
        private GameInfoControl gameInfoControl;
        private GroupBox groupBox7;
        private Label label12;
        private TextBox avgTimeStateTxtBox;
        private Label label13;
        private Label label1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem autoPlayBestToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem imitationFactorToolStripMenuItem;
        private ToolStripComboBox cBoxImitFactor;
        private ToolStripMenuItem stratlearningRateToolStripMenuItem;
        private ToolStripComboBox cBoxStratLearn;
    }
}