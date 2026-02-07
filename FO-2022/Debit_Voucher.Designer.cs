namespace FO_2022
{
    partial class Debit_Voucher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debit_Voucher));
            this.vouchernotxtbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.todate = new System.Windows.Forms.Label();
            this.voucherdatepcker = new System.Windows.Forms.DateTimePicker();
            this.printbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.removebtn = new System.Windows.Forms.Button();
            this.openbtn = new System.Windows.Forms.Button();
            this.clearbtn = new System.Windows.Forms.Button();
            this.savebtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.billnotxtbox = new System.Windows.Forms.TextBox();
            this.narrationtxtbox = new System.Windows.Forms.TextBox();
            this.amounttxtbox = new System.Windows.Forms.TextBox();
            this.accountnametxtbox = new System.Windows.Forms.TextBox();
            this.accountidtxtbox = new System.Windows.Forms.TextBox();
            this.accountidsearch = new System.Windows.Forms.PictureBox();
            this.a = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.totaltxtbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountidsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // vouchernotxtbox
            // 
            this.vouchernotxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vouchernotxtbox.Location = new System.Drawing.Point(7, 114);
            this.vouchernotxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.vouchernotxtbox.Name = "vouchernotxtbox";
            this.vouchernotxtbox.Size = new System.Drawing.Size(104, 13);
            this.vouchernotxtbox.TabIndex = 558;
            this.vouchernotxtbox.TextChanged += new System.EventHandler(this.vouchernotxtbox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 93);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 14);
            this.label3.TabIndex = 557;
            this.label3.Text = "Voucher No";
            // 
            // todate
            // 
            this.todate.AutoSize = true;
            this.todate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.todate.Location = new System.Drawing.Point(158, 93);
            this.todate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.todate.Name = "todate";
            this.todate.Size = new System.Drawing.Size(83, 14);
            this.todate.TabIndex = 556;
            this.todate.Text = "Voucher Date";
            // 
            // voucherdatepcker
            // 
            this.voucherdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.voucherdatepcker.Location = new System.Drawing.Point(161, 114);
            this.voucherdatepcker.Margin = new System.Windows.Forms.Padding(5);
            this.voucherdatepcker.Name = "voucherdatepcker";
            this.voucherdatepcker.Size = new System.Drawing.Size(132, 20);
            this.voucherdatepcker.TabIndex = 555;
            this.voucherdatepcker.ValueChanged += new System.EventHandler(this.voucherdatepcker_ValueChanged);
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.printbtn.Enabled = false;
            this.printbtn.FlatAppearance.BorderSize = 0;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(521, 497);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(68, 25);
            this.printbtn.TabIndex = 553;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(596, 497);
            this.closebtn.Margin = new System.Windows.Forms.Padding(4);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(75, 25);
            this.closebtn.TabIndex = 552;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // removebtn
            // 
            this.removebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.removebtn.Enabled = false;
            this.removebtn.FlatAppearance.BorderSize = 0;
            this.removebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removebtn.Location = new System.Drawing.Point(439, 496);
            this.removebtn.Margin = new System.Windows.Forms.Padding(4);
            this.removebtn.Name = "removebtn";
            this.removebtn.Size = new System.Drawing.Size(75, 25);
            this.removebtn.TabIndex = 551;
            this.removebtn.Text = "Remove";
            this.removebtn.UseVisualStyleBackColor = false;
            this.removebtn.Click += new System.EventHandler(this.removebtn_Click);
            // 
            // openbtn
            // 
            this.openbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.openbtn.FlatAppearance.BorderSize = 0;
            this.openbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openbtn.Location = new System.Drawing.Point(358, 496);
            this.openbtn.Margin = new System.Windows.Forms.Padding(4);
            this.openbtn.Name = "openbtn";
            this.openbtn.Size = new System.Drawing.Size(75, 25);
            this.openbtn.TabIndex = 550;
            this.openbtn.Text = "Open";
            this.openbtn.UseVisualStyleBackColor = false;
            this.openbtn.Click += new System.EventHandler(this.openbtn_Click);
            // 
            // clearbtn
            // 
            this.clearbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.clearbtn.FlatAppearance.BorderSize = 0;
            this.clearbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearbtn.Location = new System.Drawing.Point(275, 496);
            this.clearbtn.Margin = new System.Windows.Forms.Padding(4);
            this.clearbtn.Name = "clearbtn";
            this.clearbtn.Size = new System.Drawing.Size(75, 25);
            this.clearbtn.TabIndex = 549;
            this.clearbtn.Text = "Clear";
            this.clearbtn.UseVisualStyleBackColor = false;
            this.clearbtn.Click += new System.EventHandler(this.clearbtn_Click);
            // 
            // savebtn
            // 
            this.savebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.savebtn.Enabled = false;
            this.savebtn.FlatAppearance.BorderSize = 0;
            this.savebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.savebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savebtn.Location = new System.Drawing.Point(192, 496);
            this.savebtn.Margin = new System.Windows.Forms.Padding(4);
            this.savebtn.Name = "savebtn";
            this.savebtn.Size = new System.Drawing.Size(75, 25);
            this.savebtn.TabIndex = 548;
            this.savebtn.Text = "Save";
            this.savebtn.UseVisualStyleBackColor = false;
            this.savebtn.Click += new System.EventHandler(this.savebtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(7, 196);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(851, 244);
            this.dataGridView1.TabIndex = 546;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(599, 142);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 14);
            this.label6.TabIndex = 545;
            this.label6.Text = "Narration";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(496, 142);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 14);
            this.label5.TabIndex = 544;
            this.label5.Text = "Bill No";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(382, 142);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 14);
            this.label4.TabIndex = 543;
            this.label4.Text = "Amount";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // billnotxtbox
            // 
            this.billnotxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.billnotxtbox.Location = new System.Drawing.Point(500, 163);
            this.billnotxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.billnotxtbox.Name = "billnotxtbox";
            this.billnotxtbox.Size = new System.Drawing.Size(100, 13);
            this.billnotxtbox.TabIndex = 542;
            this.billnotxtbox.TextChanged += new System.EventHandler(this.billnotxtbox_TextChanged);
            // 
            // narrationtxtbox
            // 
            this.narrationtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.narrationtxtbox.Location = new System.Drawing.Point(604, 163);
            this.narrationtxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.narrationtxtbox.Name = "narrationtxtbox";
            this.narrationtxtbox.Size = new System.Drawing.Size(254, 13);
            this.narrationtxtbox.TabIndex = 541;
            this.narrationtxtbox.TextChanged += new System.EventHandler(this.narrationtxtbox_TextChanged);
            // 
            // amounttxtbox
            // 
            this.amounttxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.amounttxtbox.Location = new System.Drawing.Point(384, 163);
            this.amounttxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.amounttxtbox.Name = "amounttxtbox";
            this.amounttxtbox.Size = new System.Drawing.Size(113, 13);
            this.amounttxtbox.TabIndex = 540;
            this.amounttxtbox.TextChanged += new System.EventHandler(this.amounttxtbox_TextChanged);
            this.amounttxtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.amounttxtbox_KeyDown);
            this.amounttxtbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.amounttxtbox_KeyPress);
            // 
            // accountnametxtbox
            // 
            this.accountnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.accountnametxtbox.Location = new System.Drawing.Point(159, 163);
            this.accountnametxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.accountnametxtbox.Name = "accountnametxtbox";
            this.accountnametxtbox.Size = new System.Drawing.Size(223, 13);
            this.accountnametxtbox.TabIndex = 539;
            this.accountnametxtbox.TextChanged += new System.EventHandler(this.accountnametxtbox_TextChanged);
            // 
            // accountidtxtbox
            // 
            this.accountidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.accountidtxtbox.Location = new System.Drawing.Point(7, 163);
            this.accountidtxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.accountidtxtbox.Name = "accountidtxtbox";
            this.accountidtxtbox.Size = new System.Drawing.Size(104, 13);
            this.accountidtxtbox.TabIndex = 538;
            this.accountidtxtbox.TextChanged += new System.EventHandler(this.accountidtxtbox_TextChanged);
            // 
            // accountidsearch
            // 
            this.accountidsearch.Image = ((System.Drawing.Image)(resources.GetObject("accountidsearch.Image")));
            this.accountidsearch.Location = new System.Drawing.Point(117, 157);
            this.accountidsearch.Margin = new System.Windows.Forms.Padding(5);
            this.accountidsearch.Name = "accountidsearch";
            this.accountidsearch.Size = new System.Drawing.Size(40, 20);
            this.accountidsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.accountidsearch.TabIndex = 537;
            this.accountidsearch.TabStop = false;
            this.accountidsearch.Click += new System.EventHandler(this.accountidsearch_Click);
            // 
            // a
            // 
            this.a.AutoSize = true;
            this.a.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.a.Location = new System.Drawing.Point(158, 142);
            this.a.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.a.Name = "a";
            this.a.Size = new System.Drawing.Size(88, 14);
            this.a.TabIndex = 536;
            this.a.Text = "Account Name";
            this.a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 142);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 14);
            this.label1.TabIndex = 535;
            this.label1.Text = "Account No";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(822, 20);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(47, 37);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 534;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(352, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 32);
            this.label2.TabIndex = 533;
            this.label2.Text = "Debit Voucher";
            // 
            // totaltxtbox
            // 
            this.totaltxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.totaltxtbox.Location = new System.Drawing.Point(752, 448);
            this.totaltxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.totaltxtbox.Name = "totaltxtbox";
            this.totaltxtbox.Size = new System.Drawing.Size(106, 13);
            this.totaltxtbox.TabIndex = 559;
            // 
            // Debit_Voucher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(872, 542);
            this.Controls.Add(this.totaltxtbox);
            this.Controls.Add(this.vouchernotxtbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.todate);
            this.Controls.Add(this.voucherdatepcker);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.removebtn);
            this.Controls.Add(this.openbtn);
            this.Controls.Add(this.clearbtn);
            this.Controls.Add(this.savebtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.billnotxtbox);
            this.Controls.Add(this.narrationtxtbox);
            this.Controls.Add(this.amounttxtbox);
            this.Controls.Add(this.accountnametxtbox);
            this.Controls.Add(this.accountidtxtbox);
            this.Controls.Add(this.accountidsearch);
            this.Controls.Add(this.a);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Debit_Voucher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debit_Voucher";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accountidsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox vouchernotxtbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label todate;
        private System.Windows.Forms.DateTimePicker voucherdatepcker;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button removebtn;
        private System.Windows.Forms.Button openbtn;
        private System.Windows.Forms.Button clearbtn;
        private System.Windows.Forms.Button savebtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox billnotxtbox;
        private System.Windows.Forms.TextBox narrationtxtbox;
        private System.Windows.Forms.TextBox amounttxtbox;
        private System.Windows.Forms.TextBox accountnametxtbox;
        private System.Windows.Forms.TextBox accountidtxtbox;
        private System.Windows.Forms.PictureBox accountidsearch;
        private System.Windows.Forms.Label a;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox totaltxtbox;
    }
}