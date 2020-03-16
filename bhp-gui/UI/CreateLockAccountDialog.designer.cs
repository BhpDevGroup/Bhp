namespace Bhp.UI
{
    partial class CreateLockAccountDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateLockAccountDialog));
            this.lbl_account = new System.Windows.Forms.Label();
            this.combo_account = new System.Windows.Forms.ComboBox();
            this.lbl_unlockTime = new System.Windows.Forms.Label();
            this.dtp_unlockTime = new System.Windows.Forms.DateTimePicker();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_account
            // 
            resources.ApplyResources(this.lbl_account, "lbl_account");
            this.lbl_account.Name = "lbl_account";
            // 
            // combo_account
            // 
            resources.ApplyResources(this.combo_account, "combo_account");
            this.combo_account.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_account.FormattingEnabled = true;
            this.combo_account.Name = "combo_account";
            this.combo_account.SelectedIndexChanged += new System.EventHandler(this.combo_account_SelectedIndexChanged);
            // 
            // lbl_unlockTime
            // 
            resources.ApplyResources(this.lbl_unlockTime, "lbl_unlockTime");
            this.lbl_unlockTime.Name = "lbl_unlockTime";
            // 
            // dtp_unlockTime
            // 
            resources.ApplyResources(this.dtp_unlockTime, "dtp_unlockTime");
            this.dtp_unlockTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_unlockTime.Name = "dtp_unlockTime";
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // CreateLockAccountDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AcceptButton = this.btn_ok;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.dtp_unlockTime);
            this.Controls.Add(this.lbl_unlockTime);
            this.Controls.Add(this.combo_account);
            this.Controls.Add(this.lbl_account);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateLockAccountDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_account;
        private System.Windows.Forms.ComboBox combo_account;
        private System.Windows.Forms.Label lbl_unlockTime;
        private System.Windows.Forms.DateTimePicker dtp_unlockTime;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
    }
}