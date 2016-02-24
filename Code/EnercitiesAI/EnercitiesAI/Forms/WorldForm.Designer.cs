namespace EnercitiesAI.Forms
{
    partial class WorldForm
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
            this.worldPanel = new EnercitiesAI.Forms.WorldPanel();
            this.SuspendLayout();
            // 
            // worldPanel
            // 
            this.worldPanel.BackColor = System.Drawing.Color.White;
            this.worldPanel.CellSize = 0;
            this.worldPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.worldPanel.ForceRepaint = true;
            this.worldPanel.Game = null;
            this.worldPanel.GridVisible = true;
            this.worldPanel.Location = new System.Drawing.Point(0, 0);
            this.worldPanel.Name = "worldPanel";
            this.worldPanel.Size = new System.Drawing.Size(292, 273);
            this.worldPanel.TabIndex = 0;
            // 
            // WorldForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.worldPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WorldForm";
            this.ShowInTaskbar = false;
            this.Text = "EMOTE EnerCities world grid visualizer";
            this.ResumeLayout(false);

        }

        #endregion

        private WorldPanel worldPanel;
    }
}