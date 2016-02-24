using EnercitiesAI.Forms;

namespace StrategyEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.economistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.environmentalistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mayorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.strategyControl = new EnercitiesAI.Forms.StrategyControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabCtrlStratParams = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gameInfoControl = new EnercitiesAI.Forms.GameInfoControl();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.uniformTxtBox = new System.Windows.Forms.TextBox();
            this.homeTxtBox = new System.Windows.Forms.TextBox();
            this.oilTxtBox = new System.Windows.Forms.TextBox();
            this.powTxtBox = new System.Windows.Forms.TextBox();
            this.monTxtBox = new System.Windows.Forms.TextBox();
            this.wellTxtBox = new System.Windows.Forms.TextBox();
            this.envTxtBox = new System.Windows.Forms.TextBox();
            this.econTxtBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabCtrlStratParams.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.roleToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1082, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveasToolStripMenuItem,
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
            // 
            // saveasToolStripMenuItem
            // 
            this.saveasToolStripMenuItem.Name = "saveasToolStripMenuItem";
            this.saveasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.saveasToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveasToolStripMenuItem.Text = "Save &as...";
            this.saveasToolStripMenuItem.Click += new System.EventHandler(this.SaveasToolStripMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // roleToolStripMenuItem
            // 
            this.roleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.economistToolStripMenuItem,
            this.environmentalistToolStripMenuItem,
            this.mayorToolStripMenuItem});
            this.roleToolStripMenuItem.Name = "roleToolStripMenuItem";
            this.roleToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.roleToolStripMenuItem.Text = "&Role";
            // 
            // economistToolStripMenuItem
            // 
            this.economistToolStripMenuItem.Name = "economistToolStripMenuItem";
            this.economistToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.economistToolStripMenuItem.Text = "&Economist";
            this.economistToolStripMenuItem.Click += new System.EventHandler(this.EconomistToolStripMenuItemClick);
            // 
            // environmentalistToolStripMenuItem
            // 
            this.environmentalistToolStripMenuItem.Name = "environmentalistToolStripMenuItem";
            this.environmentalistToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.environmentalistToolStripMenuItem.Text = "En&vironmentalist";
            this.environmentalistToolStripMenuItem.Click += new System.EventHandler(this.EnvironmentalistToolStripMenuItemClick);
            // 
            // mayorToolStripMenuItem
            // 
            this.mayorToolStripMenuItem.Name = "mayorToolStripMenuItem";
            this.mayorToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.mayorToolStripMenuItem.Text = "&Mayor";
            this.mayorToolStripMenuItem.Click += new System.EventHandler(this.MayorToolStripMenuItemClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.strategyControl);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 328);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base strategy";
            // 
            // strategyControl
            // 
            this.strategyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.strategyControl.Location = new System.Drawing.Point(3, 16);
            this.strategyControl.Name = "strategyControl";
            this.strategyControl.Player = null;
            this.strategyControl.Size = new System.Drawing.Size(244, 309);
            this.strategyControl.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tabCtrlStratParams);
            this.groupBox2.Location = new System.Drawing.Point(268, 36);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 328);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Strategy adjustment parameters";
            // 
            // tabCtrlStratParams
            // 
            this.tabCtrlStratParams.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabCtrlStratParams.Controls.Add(this.tabPage1);
            this.tabCtrlStratParams.Controls.Add(this.tabPage2);
            this.tabCtrlStratParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlStratParams.Location = new System.Drawing.Point(3, 16);
            this.tabCtrlStratParams.Name = "tabCtrlStratParams";
            this.tabCtrlStratParams.SelectedIndex = 0;
            this.tabCtrlStratParams.Size = new System.Drawing.Size(394, 309);
            this.tabCtrlStratParams.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(386, 280);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(386, 280);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.gameInfoControl);
            this.groupBox3.Location = new System.Drawing.Point(685, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 328);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Simulated game values";
            // 
            // gameInfoControl
            // 
            this.gameInfoControl.AllowChanges = true;
            this.gameInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameInfoControl.GameInfo = ((EmoteEvents.EnercitiesGameInfo)(resources.GetObject("gameInfoControl.GameInfo")));
            this.gameInfoControl.Location = new System.Drawing.Point(3, 16);
            this.gameInfoControl.Name = "gameInfoControl";
            this.gameInfoControl.Size = new System.Drawing.Size(244, 309);
            this.gameInfoControl.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "strategy.json";
            this.openFileDialog.Filter = "Json files|*.json";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "json";
            this.saveFileDialog.FileName = "strategy.json";
            this.saveFileDialog.Filter = "Json files|*.json";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.uniformTxtBox);
            this.groupBox4.Controls.Add(this.homeTxtBox);
            this.groupBox4.Controls.Add(this.oilTxtBox);
            this.groupBox4.Controls.Add(this.powTxtBox);
            this.groupBox4.Controls.Add(this.monTxtBox);
            this.groupBox4.Controls.Add(this.wellTxtBox);
            this.groupBox4.Controls.Add(this.envTxtBox);
            this.groupBox4.Controls.Add(this.econTxtBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(941, 36);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(129, 328);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Simulated Strategy";
            // 
            // uniformTxtBox
            // 
            this.uniformTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uniformTxtBox.Location = new System.Drawing.Point(81, 286);
            this.uniformTxtBox.Name = "uniformTxtBox";
            this.uniformTxtBox.ReadOnly = true;
            this.uniformTxtBox.Size = new System.Drawing.Size(35, 20);
            this.uniformTxtBox.TabIndex = 27;
            this.uniformTxtBox.Text = "0.00";
            this.uniformTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // homeTxtBox
            // 
            this.homeTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homeTxtBox.Location = new System.Drawing.Point(81, 250);
            this.homeTxtBox.Name = "homeTxtBox";
            this.homeTxtBox.ReadOnly = true;
            this.homeTxtBox.Size = new System.Drawing.Size(35, 20);
            this.homeTxtBox.TabIndex = 28;
            this.homeTxtBox.Text = "0.00";
            this.homeTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // oilTxtBox
            // 
            this.oilTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.oilTxtBox.Location = new System.Drawing.Point(81, 212);
            this.oilTxtBox.Name = "oilTxtBox";
            this.oilTxtBox.ReadOnly = true;
            this.oilTxtBox.Size = new System.Drawing.Size(35, 20);
            this.oilTxtBox.TabIndex = 26;
            this.oilTxtBox.Text = "0.00";
            this.oilTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // powTxtBox
            // 
            this.powTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powTxtBox.Location = new System.Drawing.Point(81, 174);
            this.powTxtBox.Name = "powTxtBox";
            this.powTxtBox.ReadOnly = true;
            this.powTxtBox.Size = new System.Drawing.Size(35, 20);
            this.powTxtBox.TabIndex = 25;
            this.powTxtBox.Text = "0.00";
            this.powTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // monTxtBox
            // 
            this.monTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monTxtBox.Location = new System.Drawing.Point(81, 136);
            this.monTxtBox.Name = "monTxtBox";
            this.monTxtBox.ReadOnly = true;
            this.monTxtBox.Size = new System.Drawing.Size(35, 20);
            this.monTxtBox.TabIndex = 24;
            this.monTxtBox.Text = "0.00";
            this.monTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // wellTxtBox
            // 
            this.wellTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wellTxtBox.Location = new System.Drawing.Point(81, 98);
            this.wellTxtBox.Name = "wellTxtBox";
            this.wellTxtBox.ReadOnly = true;
            this.wellTxtBox.Size = new System.Drawing.Size(35, 20);
            this.wellTxtBox.TabIndex = 23;
            this.wellTxtBox.Text = "0.00";
            this.wellTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // envTxtBox
            // 
            this.envTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.envTxtBox.Location = new System.Drawing.Point(81, 60);
            this.envTxtBox.Name = "envTxtBox";
            this.envTxtBox.ReadOnly = true;
            this.envTxtBox.Size = new System.Drawing.Size(35, 20);
            this.envTxtBox.TabIndex = 22;
            this.envTxtBox.Text = "0.00";
            this.envTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // econTxtBox
            // 
            this.econTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.econTxtBox.Location = new System.Drawing.Point(81, 22);
            this.econTxtBox.Name = "econTxtBox";
            this.econTxtBox.ReadOnly = true;
            this.econTxtBox.Size = new System.Drawing.Size(35, 20);
            this.econTxtBox.TabIndex = 21;
            this.econTxtBox.Text = "0.00";
            this.econTxtBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 289);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Uniformity: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 253);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Homes: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 215);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "Oil: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Power: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Money:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Wellbeing: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Environment:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(6, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "Economy:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Location = new System.Drawing.Point(674, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(5, 328);
            this.panel1.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 376);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "EMOTE EnerCities Strategy Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabCtrlStratParams.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EnercitiesAI.Forms.StrategyControl strategyControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabCtrlStratParams;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private GameInfoControl gameInfoControl;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox uniformTxtBox;
        private System.Windows.Forms.TextBox homeTxtBox;
        private System.Windows.Forms.TextBox oilTxtBox;
        private System.Windows.Forms.TextBox powTxtBox;
        private System.Windows.Forms.TextBox monTxtBox;
        private System.Windows.Forms.TextBox wellTxtBox;
        private System.Windows.Forms.TextBox envTxtBox;
        private System.Windows.Forms.TextBox econTxtBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem roleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem economistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem environmentalistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mayorToolStripMenuItem;
    }
}

