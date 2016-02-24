namespace NuttyNAO
{
    partial class TextInputForm
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
            this.txtAnswer = new System.Windows.Forms.TextBox();
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tlp.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAnswer
            // 
            this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlp.SetColumnSpan(this.txtAnswer, 2);
            this.txtAnswer.Location = new System.Drawing.Point(3, 16);
            this.txtAnswer.MinimumSize = new System.Drawing.Size(186, 20);
            this.txtAnswer.Name = "txtAnswer";
            this.txtAnswer.Size = new System.Drawing.Size(186, 20);
            this.txtAnswer.TabIndex = 1;
            this.txtAnswer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAnswer_KeyPress);
            // 
            // tlp
            // 
            this.tlp.AutoSize = true;
            this.tlp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlp.ColumnCount = 2;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tlp.Controls.Add(this.txtAnswer, 0, 1);
            this.tlp.Controls.Add(this.lblQuestion, 0, 0);
            this.tlp.Controls.Add(this.btnOk, 0, 2);
            this.tlp.Controls.Add(this.btnCancel, 1, 2);
            this.tlp.Location = new System.Drawing.Point(19, 19);
            this.tlp.Margin = new System.Windows.Forms.Padding(10);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 3;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp.Size = new System.Drawing.Size(192, 83);
            this.tlp.TabIndex = 2;
            // 
            // lblQuestion
            // 
            this.lblQuestion.AutoSize = true;
            this.tlp.SetColumnSpan(this.lblQuestion, 2);
            this.lblQuestion.Location = new System.Drawing.Point(3, 0);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(35, 13);
            this.lblQuestion.TabIndex = 1;
            this.lblQuestion.Text = "label1";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(3, 48);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 20);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(98, 48);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // TextInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(230, 140);
            this.Controls.Add(this.tlp);
            this.Name = "TextInputForm";
            this.Text = "TextInputForm";
            this.tlp.ResumeLayout(false);
            this.tlp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAnswer;
        private System.Windows.Forms.TableLayoutPanel tlp;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

    }
}