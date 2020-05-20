namespace Bhp.UI
{
    partial class TransferDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransferDialog));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_lock = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.combo_from = new System.Windows.Forms.ComboBox();
            this.lbl_from = new System.Windows.Forms.Label();
            this.combo_change = new System.Windows.Forms.ComboBox();
            this.lbl_change = new System.Windows.Forms.Label();
            this.txt_fee = new System.Windows.Forms.TextBox();
            this.lbl_fee = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.txOutListBox1 = new TxOutListBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.btn_lock);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // btn_lock
            // 
            resources.ApplyResources(this.btn_lock, "btn_lock");
            this.btn_lock.Name = "btn_lock";
            this.btn_lock.UseVisualStyleBackColor = true;
            this.btn_lock.Click += new System.EventHandler(this.btn_lock_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Image = global::Bhp.Properties.Resources.remark;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.combo_from);
            this.groupBox1.Controls.Add(this.lbl_from);
            this.groupBox1.Controls.Add(this.combo_change);
            this.groupBox1.Controls.Add(this.lbl_change);
            this.groupBox1.Controls.Add(this.txt_fee);
            this.groupBox1.Controls.Add(this.lbl_fee);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // combo_from
            // 
            resources.ApplyResources(this.combo_from, "combo_from");
            this.combo_from.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_from.FormattingEnabled = true;
            this.combo_from.Name = "combo_from";
            // 
            // lbl_from
            // 
            resources.ApplyResources(this.lbl_from, "lbl_from");
            this.lbl_from.Name = "lbl_from";
            // 
            // combo_change
            // 
            resources.ApplyResources(this.combo_change, "combo_change");
            this.combo_change.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_change.FormattingEnabled = true;
            this.combo_change.Name = "combo_change";
            // 
            // lbl_change
            // 
            resources.ApplyResources(this.lbl_change, "lbl_change");
            this.lbl_change.Name = "lbl_change";
            // 
            // txt_fee
            // 
            resources.ApplyResources(this.txt_fee, "txt_fee");
            this.txt_fee.Name = "txt_fee";
            // 
            // lbl_fee
            // 
            resources.ApplyResources(this.lbl_fee, "lbl_fee");
            this.lbl_fee.Name = "lbl_fee";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            //
            // txOutListBox1
            //
            resources.ApplyResources(this.txOutListBox1, "txOutListBox1");
            this.txOutListBox1.Asset = null;
            this.txOutListBox1.Name = "txOutListBox1";
            this.txOutListBox1.ReadOnly = false;
            this.txOutListBox1.ScriptHash = null;
            this.txOutListBox1.ItemsChanged += new System.EventHandler(this.txOutListBox1_ItemsChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Controls.Add(this.txOutListBox1);
            // 
            // TransferDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TransferDialog";
            this.ShowInTaskbar = false;
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_fee;
        private System.Windows.Forms.Label lbl_fee;
        private System.Windows.Forms.Label lbl_change;
        private System.Windows.Forms.ComboBox combo_change;
        private System.Windows.Forms.Button btn_lock;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox combo_from;
        private System.Windows.Forms.Label lbl_from;
        private System.Windows.Forms.Panel panel1;
        private Bhp.UI.TxOutListBox txOutListBox1;
    }
}