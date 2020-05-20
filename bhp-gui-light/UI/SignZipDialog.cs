using Bhp.BhpExtensions.CertificateSign;
using System;
using System.IO;
using System.Windows.Forms;

namespace Bhp.UI
{
    internal partial class SignZipDialog : Form
    {
        public SignZipDialog()
        {
            InitializeComponent();
        }

        string originalName = string.Empty;
        private void btn_original_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "ZIP|*.zip";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txt_original.Text = dialog.FileName;
                    originalName = Path.GetFileNameWithoutExtension(dialog.FileName);
                }
            }
        }

        private void btn_key_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "TXT|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txt_key.Text = dialog.FileName;
                }
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txt_original.Text) || string.IsNullOrEmpty(this.txt_key.Text)) return;

            // copy zip file to a new folder
            string originalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "signzip");
            if (Directory.Exists(originalPath))
            {
                RSASign.DeleteDirectory(originalPath);
            }
            else
            {
                Directory.CreateDirectory(originalPath);
            }

            string copyName = Path.Combine(originalPath, originalName + ".zip");
            File.Copy(this.txt_original.Text, copyName);

            string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, originalName + "-s.zip");
            if (RSASign.SignZip(copyName, targetPath, this.txt_key.Text))
            {
                if (Directory.Exists(originalPath))
                {
                    RSASign.DeleteDirectory(originalPath);
                }
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
