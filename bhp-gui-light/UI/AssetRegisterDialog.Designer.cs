namespace Bhp.UI
{
    partial class AssetRegisterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetRegisterDialog));
            this.lbl_asset = new System.Windows.Forms.Label();
            this.combo_asset = new System.Windows.Forms.ComboBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.lbl_capped = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txt_capped = new System.Windows.Forms.TextBox();
            this.lbl_owner = new System.Windows.Forms.Label();
            this.combo_owner = new System.Windows.Forms.ComboBox();
            this.lbl_admin = new System.Windows.Forms.Label();
            this.combo_admin = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbl_precision = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lbl_issuer = new System.Windows.Forms.Label();
            this.combo_issuer = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_asset
            // 
            resources.ApplyResources(this.lbl_asset, "lbl_asset");
            this.lbl_asset.Name = "lbl_asset";
            // 
            // combo_asset
            // 
            resources.ApplyResources(this.combo_asset, "combo_asset");
            this.combo_asset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_asset.FormattingEnabled = true;
            this.combo_asset.Name = "combo_asset";
            this.combo_asset.SelectedIndexChanged += new System.EventHandler(this.combo_asset_SelectedIndexChanged);
            // 
            // lbl_name
            // 
            resources.ApplyResources(this.lbl_name, "lbl_name");
            this.lbl_name.Name = "lbl_name";
            // 
            // txt_name
            // 
            resources.ApplyResources(this.txt_name, "txt_name");
            this.txt_name.Name = "txt_name";
            this.txt_name.TextChanged += new System.EventHandler(this.CheckForm);
            // 
            // lbl_capped
            // 
            resources.ApplyResources(this.lbl_capped, "lbl_capped");
            this.lbl_capped.Name = "lbl_capped";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txt_capped
            // 
            resources.ApplyResources(this.txt_capped, "txt_capped");
            this.txt_capped.Name = "txt_capped";
            this.txt_capped.TextChanged += new System.EventHandler(this.CheckForm);
            // 
            // lbl_owner
            // 
            resources.ApplyResources(this.lbl_owner, "lbl_owner");
            this.lbl_owner.Name = "lbl_owner";
            // 
            // combo_owner
            // 
            resources.ApplyResources(this.combo_owner, "combo_owner");
            this.combo_owner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_owner.FormattingEnabled = true;
            this.combo_owner.Name = "combo_owner";
            this.combo_owner.SelectedIndexChanged += new System.EventHandler(this.CheckForm);
            // 
            // lbl_admin
            // 
            resources.ApplyResources(this.lbl_admin, "lbl_admin");
            this.lbl_admin.Name = "lbl_admin";
            // 
            // combo_admin
            // 
            resources.ApplyResources(this.combo_admin, "combo_admin");
            this.combo_admin.FormattingEnabled = true;
            this.combo_admin.Name = "combo_admin";
            this.combo_admin.SelectedIndexChanged += new System.EventHandler(this.CheckForm);
            this.combo_admin.TextUpdate += new System.EventHandler(this.CheckForm);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lbl_precision
            // 
            resources.ApplyResources(this.lbl_precision, "lbl_precision");
            this.lbl_precision.Name = "lbl_precision";
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // lbl_issuer
            // 
            resources.ApplyResources(this.lbl_issuer, "lbl_issuer");
            this.lbl_issuer.Name = "lbl_issuer";
            // 
            // combo_issuer
            // 
            resources.ApplyResources(this.combo_issuer, "combo_issuer");
            this.combo_issuer.FormattingEnabled = true;
            this.combo_issuer.Name = "combo_issuer";
            this.combo_issuer.SelectedIndexChanged += new System.EventHandler(this.CheckForm);
            this.combo_issuer.TextUpdate += new System.EventHandler(this.CheckForm);
            // 
            // AssetRegisterDialog
            // 
            this.AcceptButton = this.button1;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.combo_issuer);
            this.Controls.Add(this.lbl_issuer);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.lbl_precision);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.combo_admin);
            this.Controls.Add(this.lbl_admin);
            this.Controls.Add(this.combo_owner);
            this.Controls.Add(this.lbl_owner);
            this.Controls.Add(this.txt_capped);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lbl_capped);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.combo_asset);
            this.Controls.Add(this.lbl_asset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssetRegisterDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.AssetRegisterDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_asset;
        private System.Windows.Forms.ComboBox combo_asset;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Label lbl_capped;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txt_capped;
        private System.Windows.Forms.Label lbl_owner;
        private System.Windows.Forms.ComboBox combo_owner;
        private System.Windows.Forms.Label lbl_admin;
        private System.Windows.Forms.ComboBox combo_admin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbl_precision;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label lbl_issuer;
        private System.Windows.Forms.ComboBox combo_issuer;
    }
}