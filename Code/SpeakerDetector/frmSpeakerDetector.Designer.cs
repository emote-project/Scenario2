namespace SpeakerDetector
{
    partial class frmSpeakerDetector
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lastuser = new System.Windows.Forms.Label();
            this.meterLeft = new System.Windows.Forms.ProgressBar();
            this.meterRight = new System.Windows.Forms.ProgressBar();
            this.leftuser = new System.Windows.Forms.Label();
            this.rightuser = new System.Windows.Forms.Label();
            this.pnlLeftUser = new System.Windows.Forms.Panel();
            this.pnlRightUser = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numDecibelsDifference = new System.Windows.Forms.NumericUpDown();
            this.numDecibelsThreshold = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAudioDevices = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecibelsDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecibelsThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label3.Location = new System.Drawing.Point(219, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Left User:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label4.Location = new System.Drawing.Point(219, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Right User:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label5.Location = new System.Drawing.Point(219, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Active User:";
            // 
            // lastuser
            // 
            this.lastuser.AutoSize = true;
            this.lastuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lastuser.Location = new System.Drawing.Point(299, 74);
            this.lastuser.Name = "lastuser";
            this.lastuser.Size = new System.Drawing.Size(42, 16);
            this.lastuser.TabIndex = 5;
            this.lastuser.Text = "none";
            // 
            // meterLeft
            // 
            this.meterLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meterLeft.Location = new System.Drawing.Point(278, 7);
            this.meterLeft.Maximum = 60;
            this.meterLeft.Name = "meterLeft";
            this.meterLeft.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.meterLeft.Size = new System.Drawing.Size(102, 23);
            this.meterLeft.TabIndex = 6;
            // 
            // meterRight
            // 
            this.meterRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meterRight.Location = new System.Drawing.Point(278, 33);
            this.meterRight.Maximum = 60;
            this.meterRight.Name = "meterRight";
            this.meterRight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.meterRight.Size = new System.Drawing.Size(102, 23);
            this.meterRight.TabIndex = 7;
            // 
            // leftuser
            // 
            this.leftuser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.leftuser.AutoSize = true;
            this.leftuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.leftuser.Location = new System.Drawing.Point(388, 12);
            this.leftuser.Name = "leftuser";
            this.leftuser.Size = new System.Drawing.Size(31, 16);
            this.leftuser.TabIndex = 8;
            this.leftuser.Text = "-oo";
            // 
            // rightuser
            // 
            this.rightuser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rightuser.AutoSize = true;
            this.rightuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.rightuser.Location = new System.Drawing.Point(388, 38);
            this.rightuser.Name = "rightuser";
            this.rightuser.Size = new System.Drawing.Size(31, 16);
            this.rightuser.TabIndex = 9;
            this.rightuser.Text = "-oo";
            // 
            // pnlLeftUser
            // 
            this.pnlLeftUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLeftUser.Location = new System.Drawing.Point(386, 7);
            this.pnlLeftUser.Name = "pnlLeftUser";
            this.pnlLeftUser.Size = new System.Drawing.Size(52, 26);
            this.pnlLeftUser.TabIndex = 10;
            // 
            // pnlRightUser
            // 
            this.pnlRightUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlRightUser.Location = new System.Drawing.Point(386, 33);
            this.pnlRightUser.Name = "pnlRightUser";
            this.pnlRightUser.Size = new System.Drawing.Size(52, 26);
            this.pnlRightUser.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cmbAudioDevices);
            this.groupBox1.Controls.Add(this.numDecibelsDifference);
            this.groupBox1.Controls.Add(this.numDecibelsThreshold);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 94);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // numDecibelsDifference
            // 
            this.numDecibelsDifference.Location = new System.Drawing.Point(139, 45);
            this.numDecibelsDifference.Name = "numDecibelsDifference";
            this.numDecibelsDifference.Size = new System.Drawing.Size(44, 20);
            this.numDecibelsDifference.TabIndex = 3;
            this.numDecibelsDifference.ValueChanged += new System.EventHandler(this.numDecibelsDifference_ValueChanged);
            // 
            // numDecibelsThreshold
            // 
            this.numDecibelsThreshold.Location = new System.Drawing.Point(139, 24);
            this.numDecibelsThreshold.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numDecibelsThreshold.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numDecibelsThreshold.Name = "numDecibelsThreshold";
            this.numDecibelsThreshold.Size = new System.Drawing.Size(44, 20);
            this.numDecibelsThreshold.TabIndex = 2;
            this.numDecibelsThreshold.ValueChanged += new System.EventHandler(this.numDecibelsThreshold_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Deactivation level (dB):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Activation Threshold (dB):";
            // 
            // cmbAudioDevices
            // 
            this.cmbAudioDevices.FormattingEnabled = true;
            this.cmbAudioDevices.Location = new System.Drawing.Point(56, 67);
            this.cmbAudioDevices.Name = "cmbAudioDevices";
            this.cmbAudioDevices.Size = new System.Drawing.Size(127, 21);
            this.cmbAudioDevices.TabIndex = 4;
            this.cmbAudioDevices.SelectedIndexChanged += new System.EventHandler(this.cmbAudioDevices_SelectedIndexChanged);
            this.cmbAudioDevices.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbAudioDevices_MouseClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Device:";
            // 
            // frmSpeakerDetector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 106);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rightuser);
            this.Controls.Add(this.leftuser);
            this.Controls.Add(this.meterRight);
            this.Controls.Add(this.meterLeft);
            this.Controls.Add(this.lastuser);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pnlLeftUser);
            this.Controls.Add(this.pnlRightUser);
            this.MinimumSize = new System.Drawing.Size(459, 133);
            this.Name = "frmSpeakerDetector";
            this.Text = "Speaker Detector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.frmSpeakerDetector_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecibelsDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecibelsThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lastuser;
        private System.Windows.Forms.ProgressBar meterLeft;
        private System.Windows.Forms.ProgressBar meterRight;
        private System.Windows.Forms.Label leftuser;
        private System.Windows.Forms.Label rightuser;
        private System.Windows.Forms.Panel pnlLeftUser;
        private System.Windows.Forms.Panel pnlRightUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numDecibelsDifference;
        private System.Windows.Forms.NumericUpDown numDecibelsThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbAudioDevices;
    }
}

