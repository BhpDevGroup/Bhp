using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace Bhp.UI
{
    internal partial class InputBox : Form
    {
        private InputBox(string text, string caption, string content)
        {
            InitializeComponent();
            this.Text = caption;
            groupBox1.Text = text;
            textBox1.Text = content;
        }

        public static string Show(string text, string caption, string content = "")
        {
            using (InputBox dialog = new InputBox(text, caption, content))
            {
                if (dialog.ShowDialog() != DialogResult.OK) return null;

                if (!string.IsNullOrEmpty(dialog.textBox7.Text)) 
                {
                    return "cipertext" + dialog.textBox7.Text;
                }
                return dialog.textBox1.Text;
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            string stringToEncrypt = textBox1.Text;
            string key = txt_key.Text;

            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                MessageBox.Show("请输入备注！");
                return;
            }
            if (string.IsNullOrEmpty(key)) 
            {
                MessageBox.Show("密钥必须为八位字符！");
                return;
            }
            if (key.Length != 8)
            {
                MessageBox.Show("密钥必须为八位字符！");
                return;
            }

            this.textBox7.Text = Encrypt(stringToEncrypt, key);
        }


        public static string Encrypt(string stringToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(stringToEncrypt);
            des.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
            des.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }


        public static string Decrypt(string stringToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[stringToDecrypt.Length / 2];
            for (int x = 0; x < stringToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(stringToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.UTF8.GetBytes(sKey);
            des.IV = ASCIIEncoding.UTF8.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string dekeyString =  Decrypt(textBox7.Text, txt_key.Text);
            MessageBox.Show(dekeyString);
        }
    }
}
