namespace FO_2022
{
    partial class Company_Wise_Sales
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Company_Wise_Sales));
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepcker = new System.Windows.Forms.DateTimePicker();
            this.customeridsearch = new System.Windows.Forms.PictureBox();
            this.sectoridsearch = new System.Windows.Forms.PictureBox();
            this.townidsearch = new System.Windows.Forms.PictureBox();
            this.townnametxtbox = new System.Windows.Forms.TextBox();
            this.townidtxtbox = new System.Windows.Forms.TextBox();
            this.cstmrnametxtbox = new System.Windows.Forms.TextBox();
            this.sectornametxtbox = new System.Windows.Forms.TextBox();
            this.cstmridtxtbox = new System.Windows.Forms.TextBox();
            this.sectoridtxtbox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.customeridsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sectoridsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.townidsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.SuspendLayout();
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(243, 157);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(96, 23);
            this.todatepcker.TabIndex = 371;
            this.todatepcker.ValueChanged += new System.EventHandler(this.todatepcker_ValueChanged);
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepcker.Location = new System.Drawing.Point(99, 157);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(104, 23);
            this.fromdatepcker.TabIndex = 370;
            this.fromdatepcker.ValueChanged += new System.EventHandler(this.fromdatepcker_ValueChanged);
            // 
            // customeridsearch
            // 
            this.customeridsearch.Image = ((System.Drawing.Image)(resources.GetObject("customeridsearch.Image")));
            this.customeridsearch.Location = new System.Drawing.Point(214, 221);
            this.customeridsearch.Name = "customeridsearch";
            this.customeridsearch.Size = new System.Drawing.Size(26, 18);
            this.customeridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.customeridsearch.TabIndex = 369;
            this.customeridsearch.TabStop = false;
            this.customeridsearch.Click += new System.EventHandler(this.customeridsearch_Click);
            // 
            // sectoridsearch
            // 
            this.sectoridsearch.Image = ((System.Drawing.Image)(resources.GetObject("sectoridsearch.Image")));
            this.sectoridsearch.Location = new System.Drawing.Point(214, 268);
            this.sectoridsearch.Name = "sectoridsearch";
            this.sectoridsearch.Size = new System.Drawing.Size(26, 18);
            this.sectoridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sectoridsearch.TabIndex = 368;
            this.sectoridsearch.TabStop = false;
            this.sectoridsearch.Click += new System.EventHandler(this.sectoridsearch_Click);
            // 
            // townidsearch
            // 
            this.townidsearch.Image = ((System.Drawing.Image)(resources.GetObject("townidsearch.Image")));
            this.townidsearch.Location = new System.Drawing.Point(214, 318);
            this.townidsearch.Name = "townidsearch";
            this.townidsearch.Size = new System.Drawing.Size(26, 18);
            this.townidsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.townidsearch.TabIndex = 367;
            this.townidsearch.TabStop = false;
            this.townidsearch.Click += new System.EventHandler(this.townidsearch_Click);
            // 
            // townnametxtbox
            // 
            this.townnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.townnametxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.townnametxtbox.Location = new System.Drawing.Point(243, 316);
            this.townnametxtbox.Multiline = true;
            this.townnametxtbox.Name = "townnametxtbox";
            this.townnametxtbox.Size = new System.Drawing.Size(230, 20);
            this.townnametxtbox.TabIndex = 365;
            //this.townnametxtbox.TextChanged += new System.EventHandler(this.townnametxtbox_TextChanged);
            // 
            // townidtxtbox
            // 
            this.townidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.townidtxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.townidtxtbox.Location = new System.Drawing.Point(97, 318);
            this.townidtxtbox.Multiline = true;
            this.townidtxtbox.Name = "townidtxtbox";
            this.townidtxtbox.Size = new System.Drawing.Size(117, 18);
            this.townidtxtbox.TabIndex = 364;
            this.townidtxtbox.TextChanged += new System.EventHandler(this.townidtxtbox_TextChanged);
            // 
            // cstmrnametxtbox
            // 
            this.cstmrnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cstmrnametxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cstmrnametxtbox.Location = new System.Drawing.Point(243, 221);
            this.cstmrnametxtbox.Multiline = true;
            this.cstmrnametxtbox.Name = "cstmrnametxtbox";
            this.cstmrnametxtbox.Size = new System.Drawing.Size(230, 20);
            this.cstmrnametxtbox.TabIndex = 363;
            //this.cstmrnametxtbox.TextChanged += new System.EventHandler(this.cstmrnametxtbox_TextChanged);
            // 
            // sectornametxtbox
            // 
            this.sectornametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectornametxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sectornametxtbox.Location = new System.Drawing.Point(243, 268);
            this.sectornametxtbox.Multiline = true;
            this.sectornametxtbox.Name = "sectornametxtbox";
            this.sectornametxtbox.Size = new System.Drawing.Size(230, 20);
            this.sectornametxtbox.TabIndex = 362;
            //this.sectornametxtbox.TextChanged += new System.EventHandler(this.sectornametxtbox_TextChanged);
            // 
            // cstmridtxtbox
            // 
            this.cstmridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cstmridtxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cstmridtxtbox.Location = new System.Drawing.Point(97, 221);
            this.cstmridtxtbox.Multiline = true;
            this.cstmridtxtbox.Name = "cstmridtxtbox";
            this.cstmridtxtbox.Size = new System.Drawing.Size(117, 18);
            this.cstmridtxtbox.TabIndex = 361;
            this.cstmridtxtbox.TextChanged += new System.EventHandler(this.cstmridtxtbox_TextChanged);
            // 
            // sectoridtxtbox
            // 
            this.sectoridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectoridtxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sectoridtxtbox.Location = new System.Drawing.Point(97, 270);
            this.sectoridtxtbox.Multiline = true;
            this.sectoridtxtbox.Name = "sectoridtxtbox";
            this.sectoridtxtbox.Size = new System.Drawing.Size(117, 18);
            this.sectoridtxtbox.TabIndex = 360;
            this.sectoridtxtbox.TextChanged += new System.EventHandler(this.sectoridtxtbox_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(240, 297);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 16);
            this.label10.TabIndex = 356;
            this.label10.Text = "Town Name";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(97, 299);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 16);
            this.label9.TabIndex = 355;
            this.label9.Text = "Town ID";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(240, 249);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 16);
            this.label8.TabIndex = 354;
            this.label8.Text = "Sector Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(97, 251);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 16);
            this.label7.TabIndex = 353;
            this.label7.Text = "Sector ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(240, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 16);
            this.label6.TabIndex = 352;
            this.label6.Text = "Customer Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(94, 204);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 16);
            this.label5.TabIndex = 351;
            this.label5.Text = "Customer ID";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(242, 138);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 16);
            this.label16.TabIndex = 348;
            this.label16.Text = "To Date";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(99, 139);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 16);
            this.label17.TabIndex = 347;
            this.label17.Text = "From Date ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(116, 59);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(260, 31);
            this.label15.TabIndex = 345;
            this.label15.Text = "Company Wise Sales";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(264, 157);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 16);
            this.label2.TabIndex = 344;
            this.label2.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(121, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 343;
            this.label1.Text = "From Date ";
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(137, 398);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(2);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(74, 24);
            this.previewbtn.TabIndex = 380;
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
            this.closebtn.Location = new System.Drawing.Point(291, 398);
            this.closebtn.Margin = new System.Windows.Forms.Padding(2);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(74, 24);
            this.closebtn.TabIndex = 379;
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
            this.printbtn.Location = new System.Drawing.Point(214, 398);
            this.printbtn.Margin = new System.Windows.Forms.Padding(2);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(74, 24);
            this.printbtn.TabIndex = 378;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(531, -1);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(39, 32);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 381;
            this.pictureBox7.TabStop = false;
            this.pictureBox7.Click += new System.EventHandler(this.pictureBox7_Click);
            // 
            // Company_Wise_Sales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(573, 480);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.customeridsearch);
            this.Controls.Add(this.sectoridsearch);
            this.Controls.Add(this.townidsearch);
            this.Controls.Add(this.townnametxtbox);
            this.Controls.Add(this.townidtxtbox);
            this.Controls.Add(this.cstmrnametxtbox);
            this.Controls.Add(this.sectornametxtbox);
            this.Controls.Add(this.cstmridtxtbox);
            this.Controls.Add(this.sectoridtxtbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Company_Wise_Sales";
            this.Text = "Company_Wise_Sales";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Company_Wise_Sales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.customeridsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sectoridsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.townidsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepcker;
        private System.Windows.Forms.PictureBox customeridsearch;
        private System.Windows.Forms.PictureBox sectoridsearch;
        private System.Windows.Forms.PictureBox townidsearch;
        private System.Windows.Forms.TextBox townnametxtbox;
        private System.Windows.Forms.TextBox townidtxtbox;
        private System.Windows.Forms.TextBox cstmrnametxtbox;
        private System.Windows.Forms.TextBox sectornametxtbox;
        private System.Windows.Forms.TextBox cstmridtxtbox;
        private System.Windows.Forms.TextBox sectoridtxtbox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.PictureBox pictureBox7;
    }
}