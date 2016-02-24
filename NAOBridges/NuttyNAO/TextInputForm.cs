using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NuttyNAO
{
    public partial class TextInputForm : Form
    {
        public TextInputForm()
        {
            InitializeComponent();
        }

        public string Question(string question, string title, string defaultAnswer = "")
        {
            lblQuestion.Text = question;
            this.Text = title;
            txtAnswer.Text = defaultAnswer;
            txtAnswer.Focus();
            txtAnswer.SelectAll();
            if (this.ShowDialog() == DialogResult.OK)
            {
                return txtAnswer.Text;
            }
            else return "";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtAnswer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
