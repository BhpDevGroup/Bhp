namespace Bhp.UI
{
    partial class ClaimForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClaimForm));
            this.lbl_available = new System.Windows.Forms.Label();
            this.lbl_unavailable = new System.Windows.Forms.Label();
            this.btn_claimAll = new System.Windows.Forms.Button();
            this.txt_available = new System.Windows.Forms.TextBox();
            this.txt_unavailable = new System.Windows.Forms.TextBox();
            this.lbl_claimTo = new System.Windows.Forms.Label();
            this.combo_claimTo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lbl_available
            // 
            resources.ApplyResources(this.lbl_available, "lbl_available");
            this.lbl_available.Name = "lbl_available";
            // 
            // lbl_unavailable
            // 
            resources.ApplyResources(this.lbl_unavailable, "lbl_unavailable");
            this.lbl_unavailable.Name = "lbl_unavailable";
            // 
            // btn_claimAll
            // 
            resources.ApplyResources(this.btn_claimAll, "btn_claimAll");
            this.btn_claimAll.Name = "btn_claimAll";
            this.btn_claimAll.UseVisualStyleBackColor = true;
            this.btn_claimAll.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_available
            // 
            this.txt_available.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.txt_available, "txt_available");
            this.txt_available.Name = "txt_available";
            this.txt_available.ReadOnly = true;
            // 
            // txt_unavailable
            // 
            this.txt_unavailable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.txt_unavailable, "txt_unavailable");
            this.txt_unavailable.Name = "txt_unavailable";
            this.txt_unavailable.ReadOnly = true;
            // 
            // lbl_claimTo
            // 
            resources.ApplyResources(this.lbl_claimTo, "lbl_claimTo");
            this.lbl_claimTo.Name = "lbl_claimTo";
            // 
            // combo_claimTo
            // 
            this.combo_claimTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_claimTo.FormattingEnabled = true;
            resources.ApplyResources(this.combo_claimTo, "combo_claimTo");
            this.combo_claimTo.Name = "combo_claimTo";
            this.combo_claimTo.TextChanged += new System.EventHandler(this.combo_address_TextChanged);
            // 
            // ClaimForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.combo_claimTo);
            this.Controls.Add(this.lbl_claimTo);
            this.Controls.Add(this.txt_unavailable);
            this.Controls.Add(this.txt_available);
            this.Controls.Add(this.btn_claimAll);
            this.Controls.Add(this.lbl_unavailable);
            this.Controls.Add(this.lbl_available);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ClaimForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClaimForm_FormClosing);
            this.Load += new System.EventHandler(this.ClaimForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_available;
        private System.Windows.Forms.Label lbl_unavailable;
        private System.Windows.Forms.Button btn_claimAll;
        private System.Windows.Forms.TextBox txt_available;
        private System.Windows.Forms.TextBox txt_unavailable;
        private System.Windows.Forms.Label lbl_claimTo;
        private System.Windows.Forms.ComboBox combo_claimTo;
    }
}