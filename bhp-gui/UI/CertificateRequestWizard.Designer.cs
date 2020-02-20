namespace Bhp.UI
{
    partial class CertificateRequestWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CertificateRequestWizard));
            this.grp_organization = new System.Windows.Forms.GroupBox();
            this.txt_serialnumber = new System.Windows.Forms.TextBox();
            this.lbl_serialnumber = new System.Windows.Forms.Label();
            this.txt_s = new System.Windows.Forms.TextBox();
            this.lbl_s = new System.Windows.Forms.Label();
            this.txt_c = new System.Windows.Forms.TextBox();
            this.lbl_c = new System.Windows.Forms.Label();
            this.txt_cn = new System.Windows.Forms.TextBox();
            this.lbl_cn = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.grp_account = new System.Windows.Forms.GroupBox();
            this.combo_pubKey = new System.Windows.Forms.ComboBox();
            this.lbl_pubKey = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.grp_organization.SuspendLayout();
            this.grp_account.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_organization
            // 
            resources.ApplyResources(this.grp_organization, "grp_organization");
            this.grp_organization.Controls.Add(this.txt_serialnumber);
            this.grp_organization.Controls.Add(this.lbl_serialnumber);
            this.grp_organization.Controls.Add(this.txt_s);
            this.grp_organization.Controls.Add(this.lbl_s);
            this.grp_organization.Controls.Add(this.txt_c);
            this.grp_organization.Controls.Add(this.lbl_c);
            this.grp_organization.Controls.Add(this.txt_cn);
            this.grp_organization.Controls.Add(this.lbl_cn);
            this.grp_organization.Name = "grp_organization";
            this.grp_organization.TabStop = false;
            // 
            // txt_serialnumber
            // 
            resources.ApplyResources(this.txt_serialnumber, "txt_serialnumber");
            this.txt_serialnumber.Name = "txt_serialnumber";
            this.txt_serialnumber.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_serialnumber
            // 
            resources.ApplyResources(this.lbl_serialnumber, "lbl_serialnumber");
            this.lbl_serialnumber.Name = "lbl_serialnumber";
            // 
            // txt_s
            // 
            resources.ApplyResources(this.txt_s, "txt_s");
            this.txt_s.Name = "txt_s";
            this.txt_s.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_s
            // 
            resources.ApplyResources(this.lbl_s, "lbl_s");
            this.lbl_s.Name = "lbl_s";
            // 
            // txt_c
            // 
            resources.ApplyResources(this.txt_c, "txt_c");
            this.txt_c.Name = "txt_c";
            this.txt_c.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_c
            // 
            resources.ApplyResources(this.lbl_c, "lbl_c");
            this.lbl_c.Name = "lbl_c";
            // 
            // txt_cn
            // 
            resources.ApplyResources(this.txt_cn, "txt_cn");
            this.txt_cn.Name = "txt_cn";
            this.txt_cn.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_cn
            // 
            resources.ApplyResources(this.lbl_cn, "lbl_cn");
            this.lbl_cn.Name = "lbl_cn";
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // grp_account
            // 
            resources.ApplyResources(this.grp_account, "grp_account");
            this.grp_account.Controls.Add(this.combo_pubKey);
            this.grp_account.Controls.Add(this.lbl_pubKey);
            this.grp_account.Name = "grp_account";
            this.grp_account.TabStop = false;
            // 
            // combo_pubKey
            // 
            resources.ApplyResources(this.combo_pubKey, "combo_pubKey");
            this.combo_pubKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_pubKey.FormattingEnabled = true;
            this.combo_pubKey.Name = "combo_pubKey";
            this.combo_pubKey.SelectedIndexChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_pubKey
            // 
            resources.ApplyResources(this.lbl_pubKey, "lbl_pubKey");
            this.lbl_pubKey.Name = "lbl_pubKey";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "req";
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // CertificateRequestWizard
            // 
            this.AcceptButton = this.btn_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.grp_account);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.grp_organization);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CertificateRequestWizard";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.CertificateRequestWizard_Load);
            this.grp_organization.ResumeLayout(false);
            this.grp_organization.PerformLayout();
            this.grp_account.ResumeLayout(false);
            this.grp_account.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_organization;
        private System.Windows.Forms.TextBox txt_cn;
        private System.Windows.Forms.Label lbl_cn;
        private System.Windows.Forms.TextBox txt_c;
        private System.Windows.Forms.Label lbl_c;
        private System.Windows.Forms.TextBox txt_s;
        private System.Windows.Forms.Label lbl_s;
        private System.Windows.Forms.TextBox txt_serialnumber;
        private System.Windows.Forms.Label lbl_serialnumber;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.GroupBox grp_account;
        private System.Windows.Forms.Label lbl_pubKey;
        private System.Windows.Forms.ComboBox combo_pubKey;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}