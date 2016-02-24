namespace EnercitiesAI.Forms
{
    partial class StrategyControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.econTB = new System.Windows.Forms.TrackBar();
            this.label15 = new System.Windows.Forms.Label();
            this.econTxtBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.envTB = new System.Windows.Forms.TrackBar();
            this.envTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.wellTB = new System.Windows.Forms.TrackBar();
            this.wellTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.monTB = new System.Windows.Forms.TrackBar();
            this.monTxtBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.powTB = new System.Windows.Forms.TrackBar();
            this.powTxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.oilTB = new System.Windows.Forms.TrackBar();
            this.oilTxtBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.homeTB = new System.Windows.Forms.TrackBar();
            this.homeTxtBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.uniformTB = new System.Windows.Forms.TrackBar();
            this.uniformTxtBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.econTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.envTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wellTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.powTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oilTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.homeTB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uniformTB)).BeginInit();
            this.SuspendLayout();
            // 
            // econTB
            // 
            this.econTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.econTB.LargeChange = 250;
            this.econTB.Location = new System.Drawing.Point(62, 6);
            this.econTB.Maximum = 1000;
            this.econTB.Minimum = -1000;
            this.econTB.Name = "econTB";
            this.econTB.Size = new System.Drawing.Size(161, 42);
            this.econTB.SmallChange = 100;
            this.econTB.TabIndex = 0;
            this.econTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.econTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 13);
            this.label15.TabIndex = 20;
            this.label15.Text = "Economy:";
            // 
            // econTxtBox
            // 
            this.econTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.econTxtBox.Location = new System.Drawing.Point(229, 8);
            this.econTxtBox.Name = "econTxtBox";
            this.econTxtBox.ReadOnly = true;
            this.econTxtBox.Size = new System.Drawing.Size(35, 20);
            this.econTxtBox.TabIndex = 0;
            this.econTxtBox.Text = "0.00";
            this.econTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.econTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Environment:";
            // 
            // envTB
            // 
            this.envTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.envTB.LargeChange = 250;
            this.envTB.Location = new System.Drawing.Point(62, 44);
            this.envTB.Maximum = 1000;
            this.envTB.Minimum = -1000;
            this.envTB.Name = "envTB";
            this.envTB.Size = new System.Drawing.Size(161, 42);
            this.envTB.SmallChange = 100;
            this.envTB.TabIndex = 1;
            this.envTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.envTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // envTxtBox
            // 
            this.envTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.envTxtBox.Location = new System.Drawing.Point(229, 46);
            this.envTxtBox.Name = "envTxtBox";
            this.envTxtBox.ReadOnly = true;
            this.envTxtBox.Size = new System.Drawing.Size(35, 20);
            this.envTxtBox.TabIndex = 1;
            this.envTxtBox.Text = "0.00";
            this.envTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.envTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Wellbeing: ";
            // 
            // wellTB
            // 
            this.wellTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wellTB.LargeChange = 250;
            this.wellTB.Location = new System.Drawing.Point(62, 82);
            this.wellTB.Maximum = 1000;
            this.wellTB.Minimum = -1000;
            this.wellTB.Name = "wellTB";
            this.wellTB.Size = new System.Drawing.Size(161, 42);
            this.wellTB.SmallChange = 100;
            this.wellTB.TabIndex = 2;
            this.wellTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.wellTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // wellTxtBox
            // 
            this.wellTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wellTxtBox.Location = new System.Drawing.Point(229, 84);
            this.wellTxtBox.Name = "wellTxtBox";
            this.wellTxtBox.ReadOnly = true;
            this.wellTxtBox.Size = new System.Drawing.Size(35, 20);
            this.wellTxtBox.TabIndex = 2;
            this.wellTxtBox.Text = "0.00";
            this.wellTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.wellTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Money:";
            // 
            // monTB
            // 
            this.monTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.monTB.LargeChange = 250;
            this.monTB.Location = new System.Drawing.Point(62, 120);
            this.monTB.Maximum = 1000;
            this.monTB.Minimum = -1000;
            this.monTB.Name = "monTB";
            this.monTB.Size = new System.Drawing.Size(161, 42);
            this.monTB.SmallChange = 100;
            this.monTB.TabIndex = 3;
            this.monTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.monTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // monTxtBox
            // 
            this.monTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.monTxtBox.Location = new System.Drawing.Point(229, 122);
            this.monTxtBox.Name = "monTxtBox";
            this.monTxtBox.ReadOnly = true;
            this.monTxtBox.Size = new System.Drawing.Size(35, 20);
            this.monTxtBox.TabIndex = 3;
            this.monTxtBox.Text = "0.00";
            this.monTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.monTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Power: ";
            // 
            // powTB
            // 
            this.powTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.powTB.LargeChange = 250;
            this.powTB.Location = new System.Drawing.Point(62, 158);
            this.powTB.Maximum = 1000;
            this.powTB.Minimum = -1000;
            this.powTB.Name = "powTB";
            this.powTB.Size = new System.Drawing.Size(161, 42);
            this.powTB.SmallChange = 100;
            this.powTB.TabIndex = 4;
            this.powTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.powTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // powTxtBox
            // 
            this.powTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.powTxtBox.Location = new System.Drawing.Point(229, 160);
            this.powTxtBox.Name = "powTxtBox";
            this.powTxtBox.ReadOnly = true;
            this.powTxtBox.Size = new System.Drawing.Size(35, 20);
            this.powTxtBox.TabIndex = 4;
            this.powTxtBox.Text = "0.00";
            this.powTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.powTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Oil: ";
            // 
            // oilTB
            // 
            this.oilTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.oilTB.LargeChange = 250;
            this.oilTB.Location = new System.Drawing.Point(62, 196);
            this.oilTB.Maximum = 1000;
            this.oilTB.Minimum = -1000;
            this.oilTB.Name = "oilTB";
            this.oilTB.Size = new System.Drawing.Size(161, 42);
            this.oilTB.SmallChange = 100;
            this.oilTB.TabIndex = 5;
            this.oilTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.oilTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // oilTxtBox
            // 
            this.oilTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.oilTxtBox.Location = new System.Drawing.Point(229, 198);
            this.oilTxtBox.Name = "oilTxtBox";
            this.oilTxtBox.ReadOnly = true;
            this.oilTxtBox.Size = new System.Drawing.Size(35, 20);
            this.oilTxtBox.TabIndex = 5;
            this.oilTxtBox.Text = "0.00";
            this.oilTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.oilTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 239);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Homes: ";
            // 
            // homeTB
            // 
            this.homeTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.homeTB.LargeChange = 250;
            this.homeTB.Location = new System.Drawing.Point(62, 234);
            this.homeTB.Maximum = 1000;
            this.homeTB.Minimum = -1000;
            this.homeTB.Name = "homeTB";
            this.homeTB.Size = new System.Drawing.Size(161, 42);
            this.homeTB.SmallChange = 100;
            this.homeTB.TabIndex = 6;
            this.homeTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.homeTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // homeTxtBox
            // 
            this.homeTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.homeTxtBox.Location = new System.Drawing.Point(229, 236);
            this.homeTxtBox.Name = "homeTxtBox";
            this.homeTxtBox.ReadOnly = true;
            this.homeTxtBox.Size = new System.Drawing.Size(35, 20);
            this.homeTxtBox.TabIndex = 6;
            this.homeTxtBox.Text = "0.00";
            this.homeTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.homeTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 275);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Uniformity: ";
            // 
            // uniformTB
            // 
            this.uniformTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uniformTB.LargeChange = 250;
            this.uniformTB.Location = new System.Drawing.Point(62, 272);
            this.uniformTB.Maximum = 1000;
            this.uniformTB.Minimum = -1000;
            this.uniformTB.Name = "uniformTB";
            this.uniformTB.Size = new System.Drawing.Size(161, 42);
            this.uniformTB.SmallChange = 100;
            this.uniformTB.TabIndex = 6;
            this.uniformTB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.uniformTB.Scroll += new System.EventHandler(this.TrackBarScroll);
            // 
            // uniformTxtBox
            // 
            this.uniformTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uniformTxtBox.Location = new System.Drawing.Point(229, 272);
            this.uniformTxtBox.Name = "uniformTxtBox";
            this.uniformTxtBox.ReadOnly = true;
            this.uniformTxtBox.Size = new System.Drawing.Size(35, 20);
            this.uniformTxtBox.TabIndex = 6;
            this.uniformTxtBox.Text = "0.00";
            this.uniformTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.uniformTxtBox.DoubleClick += new System.EventHandler(this.TxtBoxDoubleClicked);
            // 
            // StrategyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uniformTxtBox);
            this.Controls.Add(this.homeTxtBox);
            this.Controls.Add(this.oilTxtBox);
            this.Controls.Add(this.powTxtBox);
            this.Controls.Add(this.monTxtBox);
            this.Controls.Add(this.wellTxtBox);
            this.Controls.Add(this.envTxtBox);
            this.Controls.Add(this.econTxtBox);
            this.Controls.Add(this.uniformTB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.homeTB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.oilTB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.powTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.monTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.wellTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.envTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.econTB);
            this.Controls.Add(this.label15);
            this.Name = "StrategyControl";
            this.Size = new System.Drawing.Size(267, 310);
            ((System.ComponentModel.ISupportInitialize)(this.econTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.envTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wellTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.powTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oilTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.homeTB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uniformTB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar econTB;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox econTxtBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar envTB;
        private System.Windows.Forms.TextBox envTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar wellTB;
        private System.Windows.Forms.TextBox wellTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar monTB;
        private System.Windows.Forms.TextBox monTxtBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar powTB;
        private System.Windows.Forms.TextBox powTxtBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar oilTB;
        private System.Windows.Forms.TextBox oilTxtBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar homeTB;
        private System.Windows.Forms.TextBox homeTxtBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar uniformTB;
        private System.Windows.Forms.TextBox uniformTxtBox;

    }
}
