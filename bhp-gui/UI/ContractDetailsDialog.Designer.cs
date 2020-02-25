namespace Bhp.UI
{
    partial class ContractDetailsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContractDetailsDialog));
            this.lbl_address = new System.Windows.Forms.Label();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.lbl_scriptHash = new System.Windows.Forms.Label();
            this.txt_scriptHash = new System.Windows.Forms.TextBox();
            this.lbl_redeemScript = new System.Windows.Forms.Label();
            this.txt_redeemScript = new System.Windows.Forms.TextBox();
            this.btn_close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_address
            // 
            resources.ApplyResources(this.lbl_address, "lbl_address");
            this.lbl_address.Name = "lbl_address";
            // 
            // txt_address
            // 
            resources.ApplyResources(this.txt_address, "txt_address");
            this.txt_address.Name = "txt_address";
            this.txt_address.ReadOnly = true;
            // 
            // lbl_scriptHash
            // 
            resources.ApplyResources(this.lbl_scriptHash, "lbl_scriptHash");
            this.lbl_scriptHash.Name = "lbl_scriptHash";
            // 
            // txt_scriptHash
            // 
            resources.ApplyResources(this.txt_scriptHash, "txt_scriptHash");
            this.txt_scriptHash.Name = "txt_scriptHash";
            this.txt_scriptHash.ReadOnly = true;
            // 
            // lbl_redeemScript
            // 
            resources.ApplyResources(this.lbl_redeemScript, "lbl_redeemScript");
            this.lbl_redeemScript.Name = "lbl_redeemScript";
            // 
            // txt_redeemScript
            // 
            resources.ApplyResources(this.txt_redeemScript, "txt_redeemScript");
            this.txt_redeemScript.Name = "txt_redeemScript";
            this.txt_redeemScript.ReadOnly = true;
            // 
            // btn_close
            // 
            resources.ApplyResources(this.btn_close, "btn_close");
            this.btn_close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_close.Name = "btn_close";
            this.btn_close.UseVisualStyleBackColor = true;
            // 
            // ContractDetailsDialog
            // 
            this.AcceptButton = this.btn_close;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_close;
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.txt_redeemScript);
            this.Controls.Add(this.lbl_redeemScript);
            this.Controls.Add(this.txt_scriptHash);
            this.Controls.Add(this.lbl_scriptHash);
            this.Controls.Add(this.txt_address);
            this.Controls.Add(this.lbl_address);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContractDetailsDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_address;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.Label lbl_scriptHash;
        private System.Windows.Forms.TextBox txt_scriptHash;
        private System.Windows.Forms.Label lbl_redeemScript;
        private System.Windows.Forms.TextBox txt_redeemScript;
        private System.Windows.Forms.Button btn_close;

    }
}