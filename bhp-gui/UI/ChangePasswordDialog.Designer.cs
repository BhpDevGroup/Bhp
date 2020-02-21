namespace Bhp.UI
{
    partial class ChangePasswordDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePasswordDialog));
            this.lbl_oldPW = new System.Windows.Forms.Label();
            this.txt_oldPW = new System.Windows.Forms.TextBox();
            this.lbl_newPW = new System.Windows.Forms.Label();
            this.txt_newPW = new System.Windows.Forms.TextBox();
            this.lbl_newPW2 = new System.Windows.Forms.Label();
            this.txt_newPW2 = new System.Windows.Forms.TextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_oldPW
            // 
            resources.ApplyResources(this.lbl_oldPW, "lbl_oldPW");
            this.lbl_oldPW.Name = "lbl_oldPW";
            // 
            // txt_oldPW
            // 
            resources.ApplyResources(this.txt_oldPW, "txt_oldPW");
            this.txt_oldPW.Name = "txt_oldPW";
            this.txt_oldPW.UseSystemPasswordChar = true;
            this.txt_oldPW.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_newPW
            // 
            resources.ApplyResources(this.lbl_newPW, "lbl_newPW");
            this.lbl_newPW.Name = "lbl_newPW";
            // 
            // txt_newPW
            // 
            resources.ApplyResources(this.txt_newPW, "txt_newPW");
            this.txt_newPW.Name = "txt_newPW";
            this.txt_newPW.UseSystemPasswordChar = true;
            this.txt_newPW.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // lbl_newPW2
            // 
            resources.ApplyResources(this.lbl_newPW2, "lbl_newPW2");
            this.lbl_newPW2.Name = "lbl_newPW2";
            // 
            // txt_newPW2
            // 
            resources.ApplyResources(this.txt_newPW2, "txt_newPW2");
            this.txt_newPW2.Name = "txt_newPW2";
            this.txt_newPW2.UseSystemPasswordChar = true;
            this.txt_newPW2.TextChanged += new System.EventHandler(this.textBox_TextChanged);
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
            // ChangePasswordDialog
            // 
            this.AcceptButton = this.btn_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.txt_newPW2);
            this.Controls.Add(this.lbl_newPW2);
            this.Controls.Add(this.txt_newPW);
            this.Controls.Add(this.lbl_newPW);
            this.Controls.Add(this.txt_oldPW);
            this.Controls.Add(this.lbl_oldPW);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_oldPW;
        private System.Windows.Forms.TextBox txt_oldPW;
        private System.Windows.Forms.Label lbl_newPW;
        private System.Windows.Forms.TextBox txt_newPW;
        private System.Windows.Forms.Label lbl_newPW2;
        private System.Windows.Forms.TextBox txt_newPW2;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
    }
}