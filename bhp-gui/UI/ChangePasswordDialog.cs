using System;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class ChangePasswordDialog : Form
    {
        public string OldPassword
        {
            get
            {
                return txt_oldPW.Text;
            }
            set
            {
                txt_oldPW.Text = value;
            }
        }

        public string NewPassword
        {
            get
            {
                return txt_newPW.Text;
            }
            set
            {
                txt_newPW.Text = value;
                txt_newPW2.Text = value;
            }
        }

        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            btn_ok.Enabled = txt_oldPW.TextLength > 0 && txt_newPW.TextLength > 0 && txt_newPW2.Text == txt_newPW.Text;
        }
    }
}
