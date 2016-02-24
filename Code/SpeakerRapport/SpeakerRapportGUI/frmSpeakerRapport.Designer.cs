namespace SpeakerRapportGUI
{
    partial class frmSpeakerRapport
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTestText = new System.Windows.Forms.TextBox();
            this.chkRunTest = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numGazeShiftMinimumInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.radGlance = new System.Windows.Forms.RadioButton();
            this.radGaze = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numBaseSpeakerDecibelThreshold = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numBaseVolumeLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGazeShiftMinimumInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseSpeakerDecibelThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseVolumeLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtTestText);
            this.groupBox1.Controls.Add(this.chkRunTest);
            this.groupBox1.Location = new System.Drawing.Point(12, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 78);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Speaker Volume Testing";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Text:";
            // 
            // txtTestText
            // 
            this.txtTestText.Location = new System.Drawing.Point(44, 17);
            this.txtTestText.Name = "txtTestText";
            this.txtTestText.Size = new System.Drawing.Size(130, 20);
            this.txtTestText.TabIndex = 2;
            this.txtTestText.TextChanged += new System.EventHandler(this.txtTestText_TextChanged);
            // 
            // chkRunTest
            // 
            this.chkRunTest.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRunTest.Location = new System.Drawing.Point(6, 43);
            this.chkRunTest.Name = "chkRunTest";
            this.chkRunTest.Size = new System.Drawing.Size(80, 25);
            this.chkRunTest.TabIndex = 1;
            this.chkRunTest.Text = "Run Test";
            this.chkRunTest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRunTest.UseVisualStyleBackColor = true;
            this.chkRunTest.CheckedChanged += new System.EventHandler(this.chkRunTest_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numGazeShiftMinimumInterval);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.radGlance);
            this.groupBox2.Controls.Add(this.radGaze);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 78);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Gazing Behavior";
            // 
            // numGazeShiftMinimumInterval
            // 
            this.numGazeShiftMinimumInterval.Location = new System.Drawing.Point(124, 48);
            this.numGazeShiftMinimumInterval.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numGazeShiftMinimumInterval.Name = "numGazeShiftMinimumInterval";
            this.numGazeShiftMinimumInterval.Size = new System.Drawing.Size(50, 20);
            this.numGazeShiftMinimumInterval.TabIndex = 3;
            this.numGazeShiftMinimumInterval.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numGazeShiftMinimumInterval.ValueChanged += new System.EventHandler(this.numGazeShiftMinimumInterval_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimum Interval (ms):";
            // 
            // radGlance
            // 
            this.radGlance.Appearance = System.Windows.Forms.Appearance.Button;
            this.radGlance.Location = new System.Drawing.Point(98, 19);
            this.radGlance.Name = "radGlance";
            this.radGlance.Size = new System.Drawing.Size(76, 24);
            this.radGlance.TabIndex = 1;
            this.radGlance.TabStop = true;
            this.radGlance.Text = "Glance";
            this.radGlance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radGlance.UseVisualStyleBackColor = true;
            this.radGlance.CheckedChanged += new System.EventHandler(this.radGlance_CheckedChanged);
            // 
            // radGaze
            // 
            this.radGaze.Appearance = System.Windows.Forms.Appearance.Button;
            this.radGaze.Location = new System.Drawing.Point(10, 19);
            this.radGaze.Name = "radGaze";
            this.radGaze.Size = new System.Drawing.Size(76, 24);
            this.radGaze.TabIndex = 0;
            this.radGaze.TabStop = true;
            this.radGaze.Text = "Gaze";
            this.radGaze.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radGaze.UseVisualStyleBackColor = true;
            this.radGaze.CheckedChanged += new System.EventHandler(this.radGaze_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numBaseSpeakerDecibelThreshold);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.numBaseVolumeLevel);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(12, 96);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 64);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Speaker Volume Adaptation";
            // 
            // numBaseSpeakerDecibelThreshold
            // 
            this.numBaseSpeakerDecibelThreshold.Location = new System.Drawing.Point(124, 35);
            this.numBaseSpeakerDecibelThreshold.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numBaseSpeakerDecibelThreshold.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numBaseSpeakerDecibelThreshold.Name = "numBaseSpeakerDecibelThreshold";
            this.numBaseSpeakerDecibelThreshold.Size = new System.Drawing.Size(50, 20);
            this.numBaseSpeakerDecibelThreshold.TabIndex = 3;
            this.numBaseSpeakerDecibelThreshold.Value = new decimal(new int[] {
            12,
            0,
            0,
            -2147483648});
            this.numBaseSpeakerDecibelThreshold.ValueChanged += new System.EventHandler(this.numBaseSpeakerDecibelThreshold_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Threshold Level (dB):";
            // 
            // numBaseVolumeLevel
            // 
            this.numBaseVolumeLevel.Location = new System.Drawing.Point(124, 18);
            this.numBaseVolumeLevel.Name = "numBaseVolumeLevel";
            this.numBaseVolumeLevel.Size = new System.Drawing.Size(50, 20);
            this.numBaseVolumeLevel.TabIndex = 1;
            this.numBaseVolumeLevel.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.numBaseVolumeLevel.ValueChanged += new System.EventHandler(this.numBaseVolumeLevel_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Default Volume (%):";
            // 
            // frmSpeakerRapport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 252);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximumSize = new System.Drawing.Size(226, 290);
            this.MinimumSize = new System.Drawing.Size(226, 290);
            this.Name = "frmSpeakerRapport";
            this.Text = "Speaker Rapport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGazeShiftMinimumInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseSpeakerDecibelThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBaseVolumeLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTestText;
        private System.Windows.Forms.CheckBox chkRunTest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numGazeShiftMinimumInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radGlance;
        private System.Windows.Forms.RadioButton radGaze;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numBaseSpeakerDecibelThreshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numBaseVolumeLevel;
    }
}

