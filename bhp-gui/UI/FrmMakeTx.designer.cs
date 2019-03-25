namespace Bhp.UI
{
    partial class FrmMakeTx
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
            this.btn_start = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txt_money_min = new System.Windows.Forms.TextBox();
            this.lbl_random_money = new System.Windows.Forms.Label();
            this.txt_money_max = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_stop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_tx_max = new System.Windows.Forms.TextBox();
            this.txt_tx_min = new System.Windows.Forms.TextBox();
            this.lbl_random_tx_count = new System.Windows.Forms.Label();
            this.btn_create = new System.Windows.Forms.Button();
            this.txt_address_count = new System.Windows.Forms.TextBox();
            this.lbl_address = new System.Windows.Forms.Label();
            this.btn_open = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_start.Location = new System.Drawing.Point(340, 59);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 17;
            this.btn_start.Text = "开始";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(31, 148);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(449, 292);
            this.listBox1.TabIndex = 19;
            // 
            // txt_money_min
            // 
            this.txt_money_min.Location = new System.Drawing.Point(150, 60);
            this.txt_money_min.Name = "txt_money_min";
            this.txt_money_min.Size = new System.Drawing.Size(58, 21);
            this.txt_money_min.TabIndex = 22;
            this.txt_money_min.Text = "1";
            // 
            // lbl_random_money
            // 
            this.lbl_random_money.AutoSize = true;
            this.lbl_random_money.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_random_money.Location = new System.Drawing.Point(30, 65);
            this.lbl_random_money.Name = "lbl_random_money";
            this.lbl_random_money.Size = new System.Drawing.Size(89, 12);
            this.lbl_random_money.TabIndex = 20;
            this.lbl_random_money.Text = "随机金额范围：";
            // 
            // txt_money_max
            // 
            this.txt_money_max.Location = new System.Drawing.Point(245, 60);
            this.txt_money_max.Name = "txt_money_max";
            this.txt_money_max.Size = new System.Drawing.Size(58, 21);
            this.txt_money_max.TabIndex = 23;
            this.txt_money_max.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(220, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "-";
            // 
            // btn_stop
            // 
            this.btn_stop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_stop.Location = new System.Drawing.Point(340, 99);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(75, 23);
            this.btn_stop.TabIndex = 25;
            this.btn_stop.Text = "停止";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(220, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 29;
            this.label2.Text = "-";
            // 
            // txt_tx_max
            // 
            this.txt_tx_max.Location = new System.Drawing.Point(245, 100);
            this.txt_tx_max.Name = "txt_tx_max";
            this.txt_tx_max.Size = new System.Drawing.Size(58, 21);
            this.txt_tx_max.TabIndex = 28;
            this.txt_tx_max.Text = "5";
            // 
            // txt_tx_min
            // 
            this.txt_tx_min.Location = new System.Drawing.Point(150, 100);
            this.txt_tx_min.Name = "txt_tx_min";
            this.txt_tx_min.Size = new System.Drawing.Size(58, 21);
            this.txt_tx_min.TabIndex = 27;
            this.txt_tx_min.Text = "1";
            // 
            // lbl_random_tx_count
            // 
            this.lbl_random_tx_count.AutoSize = true;
            this.lbl_random_tx_count.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_random_tx_count.Location = new System.Drawing.Point(30, 105);
            this.lbl_random_tx_count.Name = "lbl_random_tx_count";
            this.lbl_random_tx_count.Size = new System.Drawing.Size(113, 12);
            this.lbl_random_tx_count.TabIndex = 26;
            this.lbl_random_tx_count.Text = "随机交易个数范围：";
            // 
            // btn_create
            // 
            this.btn_create.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_create.Location = new System.Drawing.Point(340, 19);
            this.btn_create.Name = "btn_create";
            this.btn_create.Size = new System.Drawing.Size(75, 23);
            this.btn_create.TabIndex = 30;
            this.btn_create.Text = "创建钱包";
            this.btn_create.UseVisualStyleBackColor = true;
            this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
            // 
            // txt_address_count
            // 
            this.txt_address_count.Location = new System.Drawing.Point(150, 20);
            this.txt_address_count.Name = "txt_address_count";
            this.txt_address_count.Size = new System.Drawing.Size(58, 21);
            this.txt_address_count.TabIndex = 32;
            this.txt_address_count.Text = "100";
            // 
            // lbl_address
            // 
            this.lbl_address.AutoSize = true;
            this.lbl_address.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_address.Location = new System.Drawing.Point(30, 25);
            this.lbl_address.Name = "lbl_address";
            this.lbl_address.Size = new System.Drawing.Size(65, 12);
            this.lbl_address.TabIndex = 31;
            this.lbl_address.Text = "地址个数：";
            // 
            // btn_open
            // 
            this.btn_open.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_open.Location = new System.Drawing.Point(423, 19);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 23);
            this.btn_open.TabIndex = 33;
            this.btn_open.Text = "打开钱包";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // FrmCreateWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 461);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.txt_address_count);
            this.Controls.Add(this.lbl_address);
            this.Controls.Add(this.btn_create);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_tx_max);
            this.Controls.Add(this.txt_tx_min);
            this.Controls.Add(this.lbl_random_tx_count);
            this.Controls.Add(this.btn_stop);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_money_max);
            this.Controls.Add(this.txt_money_min);
            this.Controls.Add(this.lbl_random_money);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btn_start);
            this.Name = "FrmCreateWallets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "转账";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCreateWallets_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txt_money_min;
        private System.Windows.Forms.Label lbl_random_money;
        private System.Windows.Forms.TextBox txt_money_max;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_tx_max;
        private System.Windows.Forms.TextBox txt_tx_min;
        private System.Windows.Forms.Label lbl_random_tx_count;
        private System.Windows.Forms.Button btn_create;
        private System.Windows.Forms.TextBox txt_address_count;
        private System.Windows.Forms.Label lbl_address;
        private System.Windows.Forms.Button btn_open;
    }
}