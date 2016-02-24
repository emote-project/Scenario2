namespace CaseBasedController.Forms
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "1"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "A description"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "0"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "True", System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "0.872"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "False")}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "1"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "Detector type"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "True", System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "0.872")}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "1"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "A behavior"),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "True", System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))))}, -1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadClassifierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSortItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.casesTabPage = new System.Windows.Forms.TabPage();
            this.caseListView = new PS.Utilities.Forms.SortableListView();
            this.idColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.priorityColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.activeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.actLvlcolumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.runColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.detectorsTabPage = new System.Windows.Forms.TabPage();
            this.detectorsListView = new PS.Utilities.Forms.SortableListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.behaviorsTabPage = new System.Windows.Forms.TabPage();
            this.behaviorsListView = new PS.Utilities.Forms.SortableListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.casesTabPage.SuspendLayout();
            this.detectorsTabPage.SuspendLayout();
            this.behaviorsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(608, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.loadClassifierToolStripMenuItem,
            this.toolStripSeparator1,
            this.showViewerToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.loadToolStripMenuItem.Text = "&Load Case Pool...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
            // 
            // loadClassifierToolStripMenuItem
            // 
            this.loadClassifierToolStripMenuItem.Name = "loadClassifierToolStripMenuItem";
            this.loadClassifierToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.loadClassifierToolStripMenuItem.Text = "Load Classifier";
            this.loadClassifierToolStripMenuItem.Click += new System.EventHandler(this.loadClassifierToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // showViewerToolStripMenuItem
            // 
            this.showViewerToolStripMenuItem.Name = "showViewerToolStripMenuItem";
            this.showViewerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.showViewerToolStripMenuItem.Text = "Show Viewer";
            this.showViewerToolStripMenuItem.Click += new System.EventHandler(this.showViewerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSortItemsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // autoSortItemsToolStripMenuItem
            // 
            this.autoSortItemsToolStripMenuItem.Name = "autoSortItemsToolStripMenuItem";
            this.autoSortItemsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.autoSortItemsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.autoSortItemsToolStripMenuItem.Text = "Auto &Sort Items";
            this.autoSortItemsToolStripMenuItem.Click += new System.EventHandler(this.AutoSortItemsMenuClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "casepool.json";
            this.openFileDialog.Filter = "CasePool Json Files (.json)|*.json";
            this.openFileDialog.Title = "Choose the CasePool Json file to load";
            // 
            // MainPanel
            // 
            this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainPanel.Controls.Add(this.tabControl);
            this.MainPanel.Location = new System.Drawing.Point(12, 27);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(584, 572);
            this.MainPanel.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.casesTabPage);
            this.tabControl.Controls.Add(this.detectorsTabPage);
            this.tabControl.Controls.Add(this.behaviorsTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(584, 572);
            this.tabControl.TabIndex = 3;
            // 
            // casesTabPage
            // 
            this.casesTabPage.Controls.Add(this.caseListView);
            this.casesTabPage.Location = new System.Drawing.Point(4, 22);
            this.casesTabPage.Name = "casesTabPage";
            this.casesTabPage.Size = new System.Drawing.Size(576, 546);
            this.casesTabPage.TabIndex = 0;
            this.casesTabPage.Text = "Cases";
            this.casesTabPage.UseVisualStyleBackColor = true;
            // 
            // caseListView
            // 
            this.caseListView.AutoSort = false;
            this.caseListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idColumnHeader,
            this.descColumnHeader,
            this.priorityColumnHeader,
            this.activeColumnHeader,
            this.actLvlcolumnHeader,
            this.runColumnHeader});
            this.caseListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.caseListView.FullRowSelect = true;
            this.caseListView.GridLines = true;
            listViewItem1.StateImageIndex = 0;
            this.caseListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.caseListView.Location = new System.Drawing.Point(0, 0);
            this.caseListView.MultiSelect = false;
            this.caseListView.Name = "caseListView";
            this.caseListView.Size = new System.Drawing.Size(576, 546);
            this.caseListView.SortColumn = 0;
            this.caseListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.caseListView.SortInterval = 1000;
            this.caseListView.TabIndex = 0;
            this.caseListView.UseCompatibleStateImageBehavior = false;
            this.caseListView.View = System.Windows.Forms.View.Details;
            // 
            // idColumnHeader
            // 
            this.idColumnHeader.Text = "ID";
            this.idColumnHeader.Width = 47;
            // 
            // descColumnHeader
            // 
            this.descColumnHeader.Text = "Description";
            this.descColumnHeader.Width = 181;
            // 
            // priorityColumnHeader
            // 
            this.priorityColumnHeader.Text = "Priority";
            this.priorityColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.priorityColumnHeader.Width = 43;
            // 
            // activeColumnHeader
            // 
            this.activeColumnHeader.Text = "Active";
            this.activeColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.activeColumnHeader.Width = 57;
            // 
            // actLvlcolumnHeader
            // 
            this.actLvlcolumnHeader.Text = "Activation Level";
            this.actLvlcolumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.actLvlcolumnHeader.Width = 67;
            // 
            // runColumnHeader
            // 
            this.runColumnHeader.Text = "Executing";
            this.runColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // detectorsTabPage
            // 
            this.detectorsTabPage.Controls.Add(this.detectorsListView);
            this.detectorsTabPage.Location = new System.Drawing.Point(4, 22);
            this.detectorsTabPage.Name = "detectorsTabPage";
            this.detectorsTabPage.Size = new System.Drawing.Size(576, 546);
            this.detectorsTabPage.TabIndex = 1;
            this.detectorsTabPage.Text = "Detectors";
            this.detectorsTabPage.UseVisualStyleBackColor = true;
            // 
            // detectorsListView
            // 
            this.detectorsListView.AutoSort = false;
            this.detectorsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5});
            this.detectorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectorsListView.FullRowSelect = true;
            this.detectorsListView.GridLines = true;
            this.detectorsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
            this.detectorsListView.Location = new System.Drawing.Point(0, 0);
            this.detectorsListView.MultiSelect = false;
            this.detectorsListView.Name = "detectorsListView";
            this.detectorsListView.Size = new System.Drawing.Size(576, 546);
            this.detectorsListView.SortColumn = 0;
            this.detectorsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.detectorsListView.SortInterval = 1000;
            this.detectorsListView.TabIndex = 1;
            this.detectorsListView.UseCompatibleStateImageBehavior = false;
            this.detectorsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Case ID";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 260;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Active";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 57;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Activation Level";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 67;
            // 
            // behaviorsTabPage
            // 
            this.behaviorsTabPage.Controls.Add(this.behaviorsListView);
            this.behaviorsTabPage.Location = new System.Drawing.Point(4, 22);
            this.behaviorsTabPage.Name = "behaviorsTabPage";
            this.behaviorsTabPage.Size = new System.Drawing.Size(576, 546);
            this.behaviorsTabPage.TabIndex = 2;
            this.behaviorsTabPage.Text = "Behaviors";
            this.behaviorsTabPage.UseVisualStyleBackColor = true;
            // 
            // behaviorsListView
            // 
            this.behaviorsListView.AutoSort = false;
            this.behaviorsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader6,
            this.columnHeader7});
            this.behaviorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.behaviorsListView.FullRowSelect = true;
            this.behaviorsListView.GridLines = true;
            this.behaviorsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3});
            this.behaviorsListView.Location = new System.Drawing.Point(0, 0);
            this.behaviorsListView.MultiSelect = false;
            this.behaviorsListView.Name = "behaviorsListView";
            this.behaviorsListView.Size = new System.Drawing.Size(576, 546);
            this.behaviorsListView.SortColumn = 0;
            this.behaviorsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.behaviorsListView.SortInterval = 1000;
            this.behaviorsListView.TabIndex = 2;
            this.behaviorsListView.UseCompatibleStateImageBehavior = false;
            this.behaviorsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Case ID";
            this.columnHeader3.Width = 50;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Description";
            this.columnHeader6.Width = 328;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Executing";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 63;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 611);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Case Pool Monitoring";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.casesTabPage.ResumeLayout(false);
            this.detectorsTabPage.ResumeLayout(false);
            this.behaviorsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSortItemsToolStripMenuItem;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage casesTabPage;
        private PS.Utilities.Forms.SortableListView caseListView;
        private System.Windows.Forms.ColumnHeader idColumnHeader;
        private System.Windows.Forms.ColumnHeader descColumnHeader;
        private System.Windows.Forms.ColumnHeader priorityColumnHeader;
        private System.Windows.Forms.ColumnHeader activeColumnHeader;
        private System.Windows.Forms.ColumnHeader actLvlcolumnHeader;
        private System.Windows.Forms.ColumnHeader runColumnHeader;
        private System.Windows.Forms.TabPage detectorsTabPage;
        private PS.Utilities.Forms.SortableListView detectorsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TabPage behaviorsTabPage;
        private PS.Utilities.Forms.SortableListView behaviorsListView;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStripMenuItem loadClassifierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}