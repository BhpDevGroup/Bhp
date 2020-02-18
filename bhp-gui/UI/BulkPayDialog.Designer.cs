namespace Bhp.UI
{
    partial class BulkPayDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkPayDialog));
            this.txt_balance = new System.Windows.Forms.TextBox();
            this.lbl_balance = new System.Windows.Forms.Label();
            this.combo_asset = new System.Windows.Forms.ComboBox();
            this.lbl_asset = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_payto = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_balance
            // 
            resources.ApplyResources(this.txt_balance, "txt_balance");
            this.txt_balance.Name = "txt_balance";
            this.txt_balance.ReadOnly = true;
            // 
            // lbl_balance
            // 
            resources.ApplyResources(this.lbl_balance, "lbl_balance");
            this.lbl_balance.Name = "lbl_balance";
            // 
            // combo_asset
            // 
            resources.ApplyResources(this.combo_asset, "combo_asset");
            this.combo_asset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_asset.FormattingEnabled = true;
            this.combo_asset.Name = "combo_asset";
            this.combo_asset.SelectedIndexChanged += new System.EventHandler(this.combo_asset_SelectedIndexChanged);
            // 
            // lbl_asset
            // 
            resources.ApplyResources(this.lbl_asset, "lbl_asset");
            this.lbl_asset.Name = "lbl_asset";
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.txt_payto);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txt_payto
            // 
            this.txt_payto.AcceptsReturn = true;
            resources.ApplyResources(this.txt_payto, "txt_payto");
            this.txt_payto.Name = "txt_payto";
            this.txt_payto.TextChanged += new System.EventHandler(this.txt_payto_TextChanged);
            // 
            // BulkPayDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AcceptButton = this.btn_ok;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txt_balance);
            this.Controls.Add(this.lbl_balance);
            this.Controls.Add(this.combo_asset);
            this.Controls.Add(this.lbl_asset);
            this.Controls.Add(this.btn_ok);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BulkPayDialog";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_balance;
        private System.Windows.Forms.Label lbl_balance;
        private System.Windows.Forms.ComboBox combo_asset;
        private System.Windows.Forms.Label lbl_asset;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_payto;
    }
}