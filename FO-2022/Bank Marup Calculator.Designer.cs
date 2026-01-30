namespace FO_2022
{
    partial class Bank_Marup_Calculator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bank_Marup_Calculator));
            this.todate = new System.Windows.Forms.Label();
            this.fromdatepckr = new System.Windows.Forms.Label();
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepcker = new System.Windows.Forms.DateTimePicker();
            this.banknametxtbox = new System.Windows.Forms.TextBox();
            this.banknotxtbox = new System.Windows.Forms.TextBox();
            this.searchbankno = new System.Windows.Forms.PictureBox();
            this.a = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ratiotxtbox = new System.Windows.Forms.TextBox();
            this.amounttxtbox = new System.Windows.Forms.TextBox();
            this.daytxtbox = new System.Windows.Forms.TextBox();
            this.ratetxtbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.searchbankno)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // todate
            // 
            this.todate.AutoSize = true;
            this.todate.Location = new System.Drawing.Point(158, 158);
            this.todate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.todate.Name = "todate";
            this.todate.Size = new System.Drawing.Size(52, 16);
            this.todate.TabIndex = 330;
            this.todate.Text = "To Date";
            // 
            // fromdatepckr
            // 
            this.fromdatepckr.AutoSize = true;
            this.fromdatepckr.Location = new System.Drawing.Point(158, 116);
            this.fromdatepckr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fromdatepckr.Name = "fromdatepckr";
            this.fromdatepckr.Size = new System.Drawing.Size(67, 16);
            this.fromdatepckr.TabIndex = 329;
            this.fromdatepckr.Text = "From Date";
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(243, 158);
            this.todatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(245, 23);
            this.todatepcker.TabIndex = 328;
            this.todatepcker.ValueChanged += new System.EventHandler(this.todatepcker_ValueChanged);
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepcker.Location = new System.Drawing.Point(243, 116);
            this.fromdatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(245, 23);
            this.fromdatepcker.TabIndex = 327;
            this.fromdatepcker.ValueChanged += new System.EventHandler(this.fromdatepcker_ValueChanged);
            // 
            // banknametxtbox
            // 
            this.banknametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.banknametxtbox.Location = new System.Drawing.Point(297, 223);
            this.banknametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.banknametxtbox.Multiline = true;
            this.banknametxtbox.Name = "banknametxtbox";
            this.banknametxtbox.Size = new System.Drawing.Size(191, 20);
            this.banknametxtbox.TabIndex = 326;
            this.banknametxtbox.TextChanged += new System.EventHandler(this.banknametxtbox_TextChanged);
            // 
            // banknotxtbox
            // 
            this.banknotxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.banknotxtbox.Location = new System.Drawing.Point(159, 223);
            this.banknotxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.banknotxtbox.Multiline = true;
            this.banknotxtbox.Name = "banknotxtbox";
            this.banknotxtbox.Size = new System.Drawing.Size(95, 20);
            this.banknotxtbox.TabIndex = 325;
            this.banknotxtbox.TextChanged += new System.EventHandler(this.banknotxtbox_TextChanged);
            // 
            // searchbankno
            // 
            this.searchbankno.Image = ((System.Drawing.Image)(resources.GetObject("searchbankno.Image")));
            this.searchbankno.Location = new System.Drawing.Point(257, 223);
            this.searchbankno.Margin = new System.Windows.Forms.Padding(4);
            this.searchbankno.Name = "searchbankno";
            this.searchbankno.Size = new System.Drawing.Size(40, 20);
            this.searchbankno.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchbankno.TabIndex = 324;
            this.searchbankno.TabStop = false;
            this.searchbankno.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // a
            // 
            this.a.AutoSize = true;
            this.a.Location = new System.Drawing.Point(294, 203);
            this.a.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.a.Name = "a";
            this.a.Size = new System.Drawing.Size(96, 16);
            this.a.TabIndex = 323;
            this.a.Text = "Bank A/C Name";
            this.a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(158, 203);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 16);
            this.label1.TabIndex = 322;
            this.label1.Text = "Bank A/C No";
            // 
            // ratiotxtbox
            // 
            this.ratiotxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ratiotxtbox.Location = new System.Drawing.Point(243, 359);
            this.ratiotxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.ratiotxtbox.Multiline = true;
            this.ratiotxtbox.Name = "ratiotxtbox";
            this.ratiotxtbox.Size = new System.Drawing.Size(245, 20);
            this.ratiotxtbox.TabIndex = 332;
            this.ratiotxtbox.TextChanged += new System.EventHandler(this.ratiotxtbox_TextChanged);
            // 
            // amounttxtbox
            // 
            this.amounttxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.amounttxtbox.Location = new System.Drawing.Point(243, 327);
            this.amounttxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.amounttxtbox.Multiline = true;
            this.amounttxtbox.Name = "amounttxtbox";
            this.amounttxtbox.Size = new System.Drawing.Size(245, 20);
            this.amounttxtbox.TabIndex = 333;
            this.amounttxtbox.TextChanged += new System.EventHandler(this.amounttxtbox_TextChanged);
            // 
            // daytxtbox
            // 
            this.daytxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.daytxtbox.Location = new System.Drawing.Point(243, 299);
            this.daytxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.daytxtbox.Multiline = true;
            this.daytxtbox.Name = "daytxtbox";
            this.daytxtbox.Size = new System.Drawing.Size(245, 20);
            this.daytxtbox.TabIndex = 334;
            this.daytxtbox.TextChanged += new System.EventHandler(this.daytxtbox_TextChanged);
            // 
            // ratetxtbox
            // 
            this.ratetxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ratetxtbox.Location = new System.Drawing.Point(243, 271);
            this.ratetxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.ratetxtbox.Multiline = true;
            this.ratetxtbox.Name = "ratetxtbox";
            this.ratetxtbox.Size = new System.Drawing.Size(245, 20);
            this.ratetxtbox.TabIndex = 335;
            this.ratetxtbox.TextChanged += new System.EventHandler(this.ratetxtbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 16);
            this.label2.TabIndex = 336;
            this.label2.Text = "Rate ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(158, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 16);
            this.label3.TabIndex = 337;
            this.label3.Text = "Days";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(158, 331);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 16);
            this.label4.TabIndex = 338;
            this.label4.Text = "Amount";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(158, 359);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 16);
            this.label5.TabIndex = 339;
            this.label5.Text = "Ratio";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(155, 39);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(324, 32);
            this.label6.TabIndex = 340;
            this.label6.Text = "Bank Markup Calculator";
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(224, 438);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(5);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(70, 23);
            this.previewbtn.TabIndex = 343;
            this.previewbtn.Text = "Preview";
            this.previewbtn.UseVisualStyleBackColor = false;
            this.previewbtn.Click += new System.EventHandler(this.previewbtn_Click);
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(359, 438);
            this.closebtn.Margin = new System.Windows.Forms.Padding(5);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(61, 23);
            this.closebtn.TabIndex = 342;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.printbtn.FlatAppearance.BorderSize = 0;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(297, 438);
            this.printbtn.Margin = new System.Windows.Forms.Padding(5);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(60, 23);
            this.printbtn.TabIndex = 341;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(633, 2);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(39, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 344;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // Bank_Marup_Calculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(673, 490);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ratetxtbox);
            this.Controls.Add(this.daytxtbox);
            this.Controls.Add(this.amounttxtbox);
            this.Controls.Add(this.ratiotxtbox);
            this.Controls.Add(this.todate);
            this.Controls.Add(this.fromdatepckr);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.banknametxtbox);
            this.Controls.Add(this.banknotxtbox);
            this.Controls.Add(this.searchbankno);
            this.Controls.Add(this.a);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Bank_Marup_Calculator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank_Marup_Calculator";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Bank_Marup_Calculator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.searchbankno)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label todate;
        private System.Windows.Forms.Label fromdatepckr;
        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepcker;
        private System.Windows.Forms.TextBox banknametxtbox;
        private System.Windows.Forms.TextBox banknotxtbox;
        private System.Windows.Forms.PictureBox searchbankno;
        private System.Windows.Forms.Label a;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ratiotxtbox;
        private System.Windows.Forms.TextBox amounttxtbox;
        private System.Windows.Forms.TextBox daytxtbox;
        private System.Windows.Forms.TextBox ratetxtbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}