namespace BenchmarkClient
{
    partial class frmBenchmark
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
            this.grpBenchmark = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numBenchmarkRounds = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numBenchmarkPublishers = new System.Windows.Forms.NumericUpDown();
            this.lblBenchmark = new System.Windows.Forms.Label();
            this.numBenchmarkClients = new System.Windows.Forms.NumericUpDown();
            this.lblBenchmarkIterations = new System.Windows.Forms.Label();
            this.numBenchmarkMessages = new System.Windows.Forms.NumericUpDown();
            this.lblBenchmarkFps = new System.Windows.Forms.Label();
            this.numBenchmarkRate = new System.Windows.Forms.NumericUpDown();
            this.btnStartBenchmark = new System.Windows.Forms.Button();
            this.grpBenchmark.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkRounds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkPublishers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkMessages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkRate)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBenchmark
            // 
            this.grpBenchmark.Controls.Add(this.label10);
            this.grpBenchmark.Controls.Add(this.numBenchmarkRounds);
            this.grpBenchmark.Controls.Add(this.label9);
            this.grpBenchmark.Controls.Add(this.numBenchmarkPublishers);
            this.grpBenchmark.Controls.Add(this.lblBenchmark);
            this.grpBenchmark.Controls.Add(this.numBenchmarkClients);
            this.grpBenchmark.Controls.Add(this.lblBenchmarkIterations);
            this.grpBenchmark.Controls.Add(this.numBenchmarkMessages);
            this.grpBenchmark.Controls.Add(this.lblBenchmarkFps);
            this.grpBenchmark.Controls.Add(this.numBenchmarkRate);
            this.grpBenchmark.Controls.Add(this.btnStartBenchmark);
            this.grpBenchmark.Location = new System.Drawing.Point(12, 12);
            this.grpBenchmark.Name = "grpBenchmark";
            this.grpBenchmark.Size = new System.Drawing.Size(383, 82);
            this.grpBenchmark.TabIndex = 10;
            this.grpBenchmark.TabStop = false;
            this.grpBenchmark.Text = "Settings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(269, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Rounds:";
            // 
            // numBenchmarkRounds
            // 
            this.numBenchmarkRounds.Location = new System.Drawing.Point(322, 21);
            this.numBenchmarkRounds.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numBenchmarkRounds.Name = "numBenchmarkRounds";
            this.numBenchmarkRounds.Size = new System.Drawing.Size(55, 20);
            this.numBenchmarkRounds.TabIndex = 13;
            this.numBenchmarkRounds.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Publishers:";
            // 
            // numBenchmarkPublishers
            // 
            this.numBenchmarkPublishers.Location = new System.Drawing.Point(66, 44);
            this.numBenchmarkPublishers.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numBenchmarkPublishers.Name = "numBenchmarkPublishers";
            this.numBenchmarkPublishers.Size = new System.Drawing.Size(55, 20);
            this.numBenchmarkPublishers.TabIndex = 11;
            this.numBenchmarkPublishers.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // lblBenchmark
            // 
            this.lblBenchmark.AutoSize = true;
            this.lblBenchmark.Location = new System.Drawing.Point(19, 23);
            this.lblBenchmark.Name = "lblBenchmark";
            this.lblBenchmark.Size = new System.Drawing.Size(41, 13);
            this.lblBenchmark.TabIndex = 10;
            this.lblBenchmark.Text = "Clients:";
            // 
            // numBenchmarkClients
            // 
            this.numBenchmarkClients.Location = new System.Drawing.Point(66, 20);
            this.numBenchmarkClients.Name = "numBenchmarkClients";
            this.numBenchmarkClients.Size = new System.Drawing.Size(55, 20);
            this.numBenchmarkClients.TabIndex = 6;
            this.numBenchmarkClients.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblBenchmarkIterations
            // 
            this.lblBenchmarkIterations.AutoSize = true;
            this.lblBenchmarkIterations.Location = new System.Drawing.Point(132, 23);
            this.lblBenchmarkIterations.Name = "lblBenchmarkIterations";
            this.lblBenchmarkIterations.Size = new System.Drawing.Size(58, 13);
            this.lblBenchmarkIterations.TabIndex = 10;
            this.lblBenchmarkIterations.Text = "Messages:";
            // 
            // numBenchmarkMessages
            // 
            this.numBenchmarkMessages.Location = new System.Drawing.Point(196, 21);
            this.numBenchmarkMessages.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numBenchmarkMessages.Name = "numBenchmarkMessages";
            this.numBenchmarkMessages.Size = new System.Drawing.Size(55, 20);
            this.numBenchmarkMessages.TabIndex = 6;
            this.numBenchmarkMessages.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblBenchmarkFps
            // 
            this.lblBenchmarkFps.AutoSize = true;
            this.lblBenchmarkFps.Location = new System.Drawing.Point(141, 46);
            this.lblBenchmarkFps.Name = "lblBenchmarkFps";
            this.lblBenchmarkFps.Size = new System.Drawing.Size(52, 13);
            this.lblBenchmarkFps.TabIndex = 10;
            this.lblBenchmarkFps.Text = "Rate (/s):";
            // 
            // numBenchmarkRate
            // 
            this.numBenchmarkRate.Location = new System.Drawing.Point(196, 44);
            this.numBenchmarkRate.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numBenchmarkRate.Name = "numBenchmarkRate";
            this.numBenchmarkRate.Size = new System.Drawing.Size(55, 20);
            this.numBenchmarkRate.TabIndex = 6;
            this.numBenchmarkRate.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // btnStartBenchmark
            // 
            this.btnStartBenchmark.Location = new System.Drawing.Point(337, 46);
            this.btnStartBenchmark.Name = "btnStartBenchmark";
            this.btnStartBenchmark.Size = new System.Drawing.Size(40, 23);
            this.btnStartBenchmark.TabIndex = 6;
            this.btnStartBenchmark.Text = "Run";
            this.btnStartBenchmark.UseVisualStyleBackColor = true;
            this.btnStartBenchmark.Click += new System.EventHandler(this.btnStartBenchmark_Click);
            // 
            // frmBenchmark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 103);
            this.Controls.Add(this.grpBenchmark);
            this.Name = "frmBenchmark";
            this.Text = "Thalamus Benchmarking Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBenchmark_FormClosing);
            this.grpBenchmark.ResumeLayout(false);
            this.grpBenchmark.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkRounds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkPublishers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkMessages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBenchmarkRate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBenchmark;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numBenchmarkRounds;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numBenchmarkPublishers;
        private System.Windows.Forms.Label lblBenchmark;
        private System.Windows.Forms.NumericUpDown numBenchmarkClients;
        private System.Windows.Forms.Label lblBenchmarkIterations;
        private System.Windows.Forms.NumericUpDown numBenchmarkMessages;
        private System.Windows.Forms.Label lblBenchmarkFps;
        private System.Windows.Forms.NumericUpDown numBenchmarkRate;
        private System.Windows.Forms.Button btnStartBenchmark;
    }
}

