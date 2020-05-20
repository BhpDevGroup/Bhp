namespace Bhp.UI
{
    partial class SignZipDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignZipDialog));
            this.btn_ok = new System.Windows.Forms.Button();
            this.lbl_original = new System.Windows.Forms.Label();
            this.txt_original = new System.Windows.Forms.TextBox();
            this.btn_original = new System.Windows.Forms.Button();
            this.btn_key = new System.Windows.Forms.Button();
            this.txt_key = new System.Windows.Forms.TextBox();
            this.lbl_key = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // lbl_original
            // 
            resources.ApplyResources(this.lbl_original, "lbl_original");
            this.lbl_original.Name = "lbl_original";
            // 
            // txt_original
            // 
            resources.ApplyResources(this.txt_original, "txt_original");
            this.txt_original.Name = "txt_original";
            // 
            // btn_original
            // 
            resources.ApplyResources(this.btn_original, "btn_original");
            this.btn_original.Name = "btn_original";
            this.btn_original.UseVisualStyleBackColor = true;
            this.btn_original.Click += new System.EventHandler(this.btn_original_Click);
            // 
            // btn_key
            // 
            resources.ApplyResources(this.btn_key, "btn_key");
            this.btn_key.Name = "btn_key";
            this.btn_key.UseVisualStyleBackColor = true;
            this.btn_key.Click += new System.EventHandler(this.btn_key_Click);
            // 
            // txt_key
            // 
            resources.ApplyResources(this.txt_key, "txt_key");
            this.txt_key.Name = "txt_key";
            // 
            // lbl_key
            // 
            resources.ApplyResources(this.lbl_key, "lbl_key");
            this.lbl_key.Name = "lbl_key";
            // 
            // SignZipDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_key);
            this.Controls.Add(this.txt_key);
            this.Controls.Add(this.lbl_key);
            this.Controls.Add(this.btn_original);
            this.Controls.Add(this.txt_original);
            this.Controls.Add(this.lbl_original);
            this.Controls.Add(this.btn_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignZipDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Label lbl_original;
        private System.Windows.Forms.TextBox txt_original;
        private System.Windows.Forms.Button btn_original;
        private System.Windows.Forms.Button btn_key;
        private System.Windows.Forms.TextBox txt_key;
        private System.Windows.Forms.Label lbl_key;
    }
}