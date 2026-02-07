namespace FO_2022
{
    partial class Season_Recovery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Season_Recovery));
            this.label1 = new System.Windows.Forms.Label();
            this.slctsalesmancombobox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.orderbydaysradiobtn = new System.Windows.Forms.RadioButton();
            this.orderbysalesradiobtn = new System.Windows.Forms.RadioButton();
            this.orderbyrecoveryradiobtn = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.descendingradiobtn = new System.Windows.Forms.RadioButton();
            this.ascendingradiobtn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.slctseasoncombobox = new System.Windows.Forms.ComboBox();
            this.previewbtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(171, 162);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 16);
            this.label1.TabIndex = 418;
            this.label1.Text = "Select Salesman";
            // 
            // slctsalesmancombobox
            // 
            this.slctsalesmancombobox.FormattingEnabled = true;
            this.slctsalesmancombobox.Location = new System.Drawing.Point(164, 181);
            this.slctsalesmancombobox.Name = "slctsalesmancombobox";
            this.slctsalesmancombobox.Size = new System.Drawing.Size(308, 24);
            this.slctsalesmancombobox.TabIndex = 417;
            this.slctsalesmancombobox.SelectedIndexChanged += new System.EventHandler(this.slctsalesmancombobox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.orderbydaysradiobtn);
            this.groupBox1.Controls.Add(this.orderbysalesradiobtn);
            this.groupBox1.Controls.Add(this.orderbyrecoveryradiobtn);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(164, 238);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 119);
            this.groupBox1.TabIndex = 419;
            this.groupBox1.TabStop = false;
            // 
            // orderbydaysradiobtn
            // 
            this.orderbydaysradiobtn.AutoSize = true;
            this.orderbydaysradiobtn.Location = new System.Drawing.Point(26, 67);
            this.orderbydaysradiobtn.Name = "orderbydaysradiobtn";
            this.orderbydaysradiobtn.Size = new System.Drawing.Size(106, 20);
            this.orderbydaysradiobtn.TabIndex = 3;
            this.orderbydaysradiobtn.TabStop = true;
            this.orderbydaysradiobtn.Text = "Order by Days";
            this.orderbydaysradiobtn.UseVisualStyleBackColor = true;
            this.orderbydaysradiobtn.CheckedChanged += new System.EventHandler(this.orderbydaysradiobtn_CheckedChanged);
            // 
            // orderbysalesradiobtn
            // 
            this.orderbysalesradiobtn.AutoSize = true;
            this.orderbysalesradiobtn.Location = new System.Drawing.Point(26, 38);
            this.orderbysalesradiobtn.Name = "orderbysalesradiobtn";
            this.orderbysalesradiobtn.Size = new System.Drawing.Size(110, 20);
            this.orderbysalesradiobtn.TabIndex = 2;
            this.orderbysalesradiobtn.TabStop = true;
            this.orderbysalesradiobtn.Text = "Order by Sales";
            this.orderbysalesradiobtn.UseVisualStyleBackColor = true;
            // 
            // orderbyrecoveryradiobtn
            // 
            this.orderbyrecoveryradiobtn.AutoSize = true;
            this.orderbyrecoveryradiobtn.Location = new System.Drawing.Point(26, 12);
            this.orderbyrecoveryradiobtn.Name = "orderbyrecoveryradiobtn";
            this.orderbyrecoveryradiobtn.Size = new System.Drawing.Size(131, 20);
            this.orderbyrecoveryradiobtn.TabIndex = 1;
            this.orderbyrecoveryradiobtn.TabStop = true;
            this.orderbyrecoveryradiobtn.Text = "Order by Recovery";
            this.orderbyrecoveryradiobtn.UseVisualStyleBackColor = true;
            this.orderbyrecoveryradiobtn.CheckedChanged += new System.EventHandler(this.orderbyrecoveryradiobtn_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.descendingradiobtn);
            this.groupBox2.Controls.Add(this.ascendingradiobtn);
            this.groupBox2.Location = new System.Drawing.Point(6, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(287, 37);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // descendingradiobtn
            // 
            this.descendingradiobtn.AutoSize = true;
            this.descendingradiobtn.Location = new System.Drawing.Point(162, 11);
            this.descendingradiobtn.Name = "descendingradiobtn";
            this.descendingradiobtn.Size = new System.Drawing.Size(90, 20);
            this.descendingradiobtn.TabIndex = 1;
            this.descendingradiobtn.TabStop = true;
            this.descendingradiobtn.Text = "Descending";
            this.descendingradiobtn.UseVisualStyleBackColor = true;
            this.descendingradiobtn.CheckedChanged += new System.EventHandler(this.descendingradiobtn_CheckedChanged);
            // 
            // ascendingradiobtn
            // 
            this.ascendingradiobtn.AutoSize = true;
            this.ascendingradiobtn.Location = new System.Drawing.Point(20, 10);
            this.ascendingradiobtn.Name = "ascendingradiobtn";
            this.ascendingradiobtn.Size = new System.Drawing.Size(83, 20);
            this.ascendingradiobtn.TabIndex = 0;
            this.ascendingradiobtn.TabStop = true;
            this.ascendingradiobtn.Text = "Ascending";
            this.ascendingradiobtn.UseVisualStyleBackColor = true;
            this.ascendingradiobtn.CheckedChanged += new System.EventHandler(this.ascendingradiobtn_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(171, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 421;
            this.label2.Text = "Select Season";
            // 
            // slctseasoncombobox
            // 
            this.slctseasoncombobox.FormattingEnabled = true;
            this.slctseasoncombobox.Location = new System.Drawing.Point(164, 124);
            this.slctseasoncombobox.Name = "slctseasoncombobox";
            this.slctseasoncombobox.Size = new System.Drawing.Size(308, 24);
            this.slctseasoncombobox.TabIndex = 420;
            this.slctseasoncombobox.SelectedIndexChanged += new System.EventHandler(this.slctseasoncombobox_SelectedIndexChanged);
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(197, 405);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(4);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(90, 25);
            this.previewbtn.TabIndex = 424;
            this.previewbtn.Text = "Preview";
            this.previewbtn.UseVisualStyleBackColor = false;
            this.previewbtn.Click += new System.EventHandler(this.previewbtn_Click);
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.printbtn.FlatAppearance.BorderSize = 0;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(294, 405);
            this.printbtn.Margin = new System.Windows.Forms.Padding(4);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(70, 25);
            this.printbtn.TabIndex = 422;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(367, 405);
            this.closebtn.Margin = new System.Windows.Forms.Padding(4);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(69, 25);
            this.closebtn.TabIndex = 423;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(570, -2);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(47, 37);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 426;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(158, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(298, 31);
            this.label3.TabIndex = 425;
            this.label3.Text = "Accounts Ledger Season";
            // 
            // Season_Recovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(617, 456);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.slctseasoncombobox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.slctsalesmancombobox);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Season_Recovery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Season_Recovery";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Season_Recovery_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox slctsalesmancombobox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox slctseasoncombobox;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.RadioButton orderbydaysradiobtn;
        private System.Windows.Forms.RadioButton orderbysalesradiobtn;
        private System.Windows.Forms.RadioButton orderbyrecoveryradiobtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton descendingradiobtn;
        private System.Windows.Forms.RadioButton ascendingradiobtn;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
    }
}