namespace Bhp.UI
{
    partial class AssetRenewDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetRenewDialog));
            this.lbl_asset = new System.Windows.Forms.Label();
            this.lbl_year = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.combo_asset = new System.Windows.Forms.ComboBox();
            this.txt_year = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbl_asset
            // 
            resources.ApplyResources(this.lbl_asset, "lbl_asset");
            this.lbl_asset.Name = "lbl_asset";
            // 
            // lbl_year
            // 
            resources.ApplyResources(this.lbl_year, "lbl_year");
            this.lbl_year.Name = "lbl_year";
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            // 
            // combo_asset
            // 
            this.combo_asset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_asset.FormattingEnabled = true;
            resources.ApplyResources(this.combo_asset, "combo_asset");
            this.combo_asset.Name = "combo_asset";
            this.combo_asset.SelectedIndexChanged += new System.EventHandler(this.CheckForm);
            // 
            // txt_year
            // 
            resources.ApplyResources(this.txt_year, "txt_year");
            this.txt_year.Name = "txt_year";
            this.txt_year.TextChanged += new System.EventHandler(this.CheckForm);
            // 
            // AssetRenewDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txt_year);
            this.Controls.Add(this.combo_asset);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.lbl_year);
            this.Controls.Add(this.lbl_asset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssetRenewDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.AssetRenewDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_asset;
        private System.Windows.Forms.Label lbl_year;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.ComboBox combo_asset;
        private System.Windows.Forms.TextBox txt_year;
    }
}