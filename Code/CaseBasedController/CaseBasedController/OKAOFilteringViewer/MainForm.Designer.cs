namespace OKAOFilteringViewer
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.variableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.angerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disgustToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sadnessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.surpriseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neutralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gazeVectorYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gazeVectorXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rangeSelectorControl = new CustomRangeSelectorControl.RangeSelectorControl();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.quantitiesChart = new PS.Utilities.Forms.QuantitiesChart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDr = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nUDq = new System.Windows.Forms.NumericUpDown();
            this.menuStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDq)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.FileName = "Session.csv";
            this.openFileDialog.Filter = "CSV files|*.csv";
            this.openFileDialog.RestoreDirectory = true;
            this.openFileDialog.Title = "Choose the OKAO offline data file";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.variableToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(653, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Enabled = false;
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveImageToolStripMenuItem.Text = "&Save Image...";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImageToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // variableToolStripMenuItem
            // 
            this.variableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smileToolStripMenuItem,
            this.angerToolStripMenuItem,
            this.disgustToolStripMenuItem,
            this.fearToolStripMenuItem,
            this.joyToolStripMenuItem,
            this.sadnessToolStripMenuItem,
            this.surpriseToolStripMenuItem,
            this.neutralToolStripMenuItem,
            this.gazeVectorYToolStripMenuItem,
            this.gazeVectorXToolStripMenuItem});
            this.variableToolStripMenuItem.Enabled = false;
            this.variableToolStripMenuItem.Name = "variableToolStripMenuItem";
            this.variableToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.variableToolStripMenuItem.Text = "&Variable";
            // 
            // smileToolStripMenuItem
            // 
            this.smileToolStripMenuItem.Name = "smileToolStripMenuItem";
            this.smileToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.smileToolStripMenuItem.Text = "&Smile";
            this.smileToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // angerToolStripMenuItem
            // 
            this.angerToolStripMenuItem.Name = "angerToolStripMenuItem";
            this.angerToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.angerToolStripMenuItem.Text = "&Anger";
            this.angerToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // disgustToolStripMenuItem
            // 
            this.disgustToolStripMenuItem.Name = "disgustToolStripMenuItem";
            this.disgustToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.disgustToolStripMenuItem.Text = "&Disgust";
            this.disgustToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // fearToolStripMenuItem
            // 
            this.fearToolStripMenuItem.Name = "fearToolStripMenuItem";
            this.fearToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.fearToolStripMenuItem.Text = "&Fear";
            this.fearToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // joyToolStripMenuItem
            // 
            this.joyToolStripMenuItem.Name = "joyToolStripMenuItem";
            this.joyToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.joyToolStripMenuItem.Text = "&Joy";
            this.joyToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // sadnessToolStripMenuItem
            // 
            this.sadnessToolStripMenuItem.Name = "sadnessToolStripMenuItem";
            this.sadnessToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.sadnessToolStripMenuItem.Text = "&Sadness";
            this.sadnessToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // surpriseToolStripMenuItem
            // 
            this.surpriseToolStripMenuItem.Name = "surpriseToolStripMenuItem";
            this.surpriseToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.surpriseToolStripMenuItem.Text = "S&urprise";
            this.surpriseToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // neutralToolStripMenuItem
            // 
            this.neutralToolStripMenuItem.Name = "neutralToolStripMenuItem";
            this.neutralToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.neutralToolStripMenuItem.Text = "&Neutral";
            this.neutralToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // gazeVectorYToolStripMenuItem
            // 
            this.gazeVectorYToolStripMenuItem.Name = "gazeVectorYToolStripMenuItem";
            this.gazeVectorYToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.gazeVectorYToolStripMenuItem.Text = "Gaze vector &Y";
            this.gazeVectorYToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // gazeVectorXToolStripMenuItem
            // 
            this.gazeVectorXToolStripMenuItem.Name = "gazeVectorXToolStripMenuItem";
            this.gazeVectorXToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.gazeVectorXToolStripMenuItem.Text = "Gaze vector &X";
            this.gazeVectorXToolStripMenuItem.Click += new System.EventHandler(this.VariableToolStripMenuItemClick);
            // 
            // rangeSelectorControl
            // 
            this.rangeSelectorControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rangeSelectorControl.BackColor = System.Drawing.SystemColors.Control;
            this.rangeSelectorControl.DelimiterForRange = ",";
            this.rangeSelectorControl.DisabledBarColor = System.Drawing.Color.Gray;
            this.rangeSelectorControl.DisabledRangeLabelColor = System.Drawing.Color.Gray;
            this.rangeSelectorControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangeSelectorControl.GapFromLeftMargin = ((uint)(10u));
            this.rangeSelectorControl.GapFromRightMargin = ((uint)(10u));
            this.rangeSelectorControl.HeightOfThumb = 20F;
            this.rangeSelectorControl.InFocusBarColor = System.Drawing.SystemColors.Highlight;
            this.rangeSelectorControl.InFocusRangeLabelColor = System.Drawing.SystemColors.ControlText;
            this.rangeSelectorControl.LabelFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rangeSelectorControl.LeftThumbImagePath = null;
            this.rangeSelectorControl.Location = new System.Drawing.Point(6, 19);
            this.rangeSelectorControl.MiddleBarWidth = ((uint)(2u));
            this.rangeSelectorControl.Name = "rangeSelectorControl";
            this.rangeSelectorControl.OutputStringFontColor = System.Drawing.Color.Black;
            this.rangeSelectorControl.Range1 = "0";
            this.rangeSelectorControl.Range2 = "5";
            this.rangeSelectorControl.RangeString = "Time interval % (13909 frames):";
            this.rangeSelectorControl.RangeValues = "0,5,10,15,20,25,30,35,40,45,50,55,60,65,70,75,80,85,90,95,100";
            this.rangeSelectorControl.RightThumbImagePath = null;
            this.rangeSelectorControl.Size = new System.Drawing.Size(504, 78);
            this.rangeSelectorControl.TabIndex = 4;
            this.rangeSelectorControl.ThumbColor = System.Drawing.SystemColors.Highlight;
            this.rangeSelectorControl.WidthOfThumb = 10F;
            this.rangeSelectorControl.XMLFileName = null;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "pdf";
            this.saveFileDialog.FileName = "filtered-value.pdf";
            this.saveFileDialog.Filter = "PDF documents|*.pdf";
            this.saveFileDialog.RestoreDirectory = true;
            this.saveFileDialog.Title = "Choose image file to save filtering chart";
            // 
            // quantitiesChart
            // 
            this.quantitiesChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.quantitiesChart.BackColor = System.Drawing.Color.White;
            this.quantitiesChart.Enabled = false;
            this.quantitiesChart.Location = new System.Drawing.Point(0, 27);
            this.quantitiesChart.Name = "quantitiesChart";
            this.quantitiesChart.Size = new System.Drawing.Size(653, 365);
            this.quantitiesChart.TabIndex = 2;
            this.quantitiesChart.Title = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rangeSelectorControl);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 398);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(516, 103);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time Interval";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.nUDr);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.nUDq);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(534, 398);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(107, 103);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter Parameters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "R:";
            // 
            // nUDr
            // 
            this.nUDr.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDr.Location = new System.Drawing.Point(30, 48);
            this.nUDr.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nUDr.Name = "nUDr";
            this.nUDr.Size = new System.Drawing.Size(71, 20);
            this.nUDr.TabIndex = 11;
            this.nUDr.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDr.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Q:";
            // 
            // nUDq
            // 
            this.nUDq.DecimalPlaces = 2;
            this.nUDq.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nUDq.Location = new System.Drawing.Point(30, 22);
            this.nUDq.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDq.Name = "nUDq";
            this.nUDq.Size = new System.Drawing.Size(71, 20);
            this.nUDq.TabIndex = 9;
            this.nUDq.Value = new decimal(new int[] {
            10,
            0,
            0,
            131072});
            this.nUDq.ValueChanged += new System.EventHandler(this.NUDqValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 513);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.quantitiesChart);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "EMOTE OKAO Filtering Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDq)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem variableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gazeVectorXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gazeVectorYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem angerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disgustToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sadnessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem surpriseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neutralToolStripMenuItem;
        private PS.Utilities.Forms.QuantitiesChart quantitiesChart;
        private CustomRangeSelectorControl.RangeSelectorControl rangeSelectorControl;
        private System.Windows.Forms.ToolStripMenuItem joyToolStripMenuItem;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nUDq;
    }
}

