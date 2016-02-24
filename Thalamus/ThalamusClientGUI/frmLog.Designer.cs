namespace Thalamus
{
    partial class ThalamusClientLogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.logEntryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.logEntryBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFilterAll = new System.Windows.Forms.Button();
            this.btnFilterNone = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numHistory = new System.Windows.Forms.NumericUpDown();
            this.lstEventNameFilters = new System.Windows.Forms.CheckedListBox();
            this.btnCollectEventNames = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.chkHold = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.logEntryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logEntryBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dgvLog
            // 
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvLog.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvLog.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLog.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLog.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLog.Location = new System.Drawing.Point(252, 12);
            this.dgvLog.Name = "dgvLog";
            this.dgvLog.RowHeadersWidth = 15;
            this.dgvLog.RowTemplate.Height = 18;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.Size = new System.Drawing.Size(425, 289);
            this.dgvLog.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Events Filter:";
            // 
            // btnFilterAll
            // 
            this.btnFilterAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterAll.Location = new System.Drawing.Point(15, 307);
            this.btnFilterAll.Name = "btnFilterAll";
            this.btnFilterAll.Size = new System.Drawing.Size(72, 23);
            this.btnFilterAll.TabIndex = 5;
            this.btnFilterAll.Text = "Select All";
            this.btnFilterAll.UseVisualStyleBackColor = true;
            this.btnFilterAll.Click += new System.EventHandler(this.btnFilterAll_Click);
            // 
            // btnFilterNone
            // 
            this.btnFilterNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilterNone.Location = new System.Drawing.Point(93, 307);
            this.btnFilterNone.Name = "btnFilterNone";
            this.btnFilterNone.Size = new System.Drawing.Size(75, 23);
            this.btnFilterNone.TabIndex = 6;
            this.btnFilterNone.Text = "Select None";
            this.btnFilterNone.UseVisualStyleBackColor = true;
            this.btnFilterNone.Click += new System.EventHandler(this.btnFilterNone_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(562, 312);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "History:";
            // 
            // numHistory
            // 
            this.numHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numHistory.Location = new System.Drawing.Point(610, 310);
            this.numHistory.Maximum = new decimal(new int[] {
            276447232,
            23283,
            0,
            0});
            this.numHistory.Name = "numHistory";
            this.numHistory.Size = new System.Drawing.Size(67, 20);
            this.numHistory.TabIndex = 8;
            this.numHistory.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numHistory.ValueChanged += new System.EventHandler(this.numHistory_ValueChanged);
            // 
            // lstEventNameFilters
            // 
            this.lstEventNameFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstEventNameFilters.FormattingEnabled = true;
            this.lstEventNameFilters.Location = new System.Drawing.Point(15, 28);
            this.lstEventNameFilters.Name = "lstEventNameFilters";
            this.lstEventNameFilters.Size = new System.Drawing.Size(231, 244);
            this.lstEventNameFilters.TabIndex = 9;
            this.lstEventNameFilters.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstEventNameFilters_ItemCheck);
            this.lstEventNameFilters.SelectedIndexChanged += new System.EventHandler(this.lstEventNameFilters_SelectedIndexChanged);
            // 
            // btnCollectEventNames
            // 
            this.btnCollectEventNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCollectEventNames.Location = new System.Drawing.Point(15, 278);
            this.btnCollectEventNames.Name = "btnCollectEventNames";
            this.btnCollectEventNames.Size = new System.Drawing.Size(153, 23);
            this.btnCollectEventNames.TabIndex = 10;
            this.btnCollectEventNames.Text = "Collect Event Names";
            this.btnCollectEventNames.UseVisualStyleBackColor = true;
            this.btnCollectEventNames.Click += new System.EventHandler(this.btnCollectEventNames_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLog.Location = new System.Drawing.Point(174, 307);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(72, 23);
            this.btnClearLog.TabIndex = 11;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // chkHold
            // 
            this.chkHold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkHold.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkHold.Location = new System.Drawing.Point(500, 307);
            this.chkHold.Name = "chkHold";
            this.chkHold.Size = new System.Drawing.Size(104, 24);
            this.chkHold.TabIndex = 12;
            this.chkHold.Text = "Hold";
            this.chkHold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkHold.UseVisualStyleBackColor = true;
            this.chkHold.CheckedChanged += new System.EventHandler(this.chkHold_CheckedChanged);
            // 
            // frmLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 339);
            this.Controls.Add(this.chkHold);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.dgvLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFilterAll);
            this.Controls.Add(this.btnFilterNone);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numHistory);
            this.Controls.Add(this.lstEventNameFilters);
            this.Controls.Add(this.btnCollectEventNames);
            this.Name = "frmLog";
            this.Text = "Thalamus Event Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.logEntryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logEntryBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource logEntryBindingSource;
        private System.Windows.Forms.BindingSource logEntryBindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFilterAll;
        private System.Windows.Forms.Button btnFilterNone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numHistory;
        private System.Windows.Forms.CheckedListBox lstEventNameFilters;
        private System.Windows.Forms.Button btnCollectEventNames;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkHold;
    }
}