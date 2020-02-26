namespace Bhp.UI
{
    partial class CreateDestoryAddress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateDestoryAddress));
            this.lbl_account = new System.Windows.Forms.Label();
            this.combo_account = new System.Windows.Forms.ComboBox();
            this.btn_create = new System.Windows.Forms.Button();
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
            // btn_create
            // 
            resources.ApplyResources(this.btn_create, "btn_create");
            this.btn_create.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_create.Name = "btn_create";
            this.btn_create.UseVisualStyleBackColor = true;
            // 
            // btn_cancel
            // 
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // CreateDestoryAddress
            // 
            this.AcceptButton = this.btn_create;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_create);
            this.Controls.Add(this.combo_account);
            this.Controls.Add(this.lbl_account);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateDestoryAddress";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_account;
        private System.Windows.Forms.ComboBox combo_account;
        private System.Windows.Forms.Button btn_create;
        private System.Windows.Forms.Button btn_cancel;
    }
}