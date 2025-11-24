namespace FO_2022
{
    partial class Sales_Summary_Product_Wise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sales_Summary_Product_Wise));
            this.todate = new System.Windows.Forms.Label();
            this.fromdatepckr = new System.Windows.Forms.Label();
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepcker = new System.Windows.Forms.DateTimePicker();
            this.sectornametxtbox = new System.Windows.Forms.TextBox();
            this.sectoridtxtbox = new System.Windows.Forms.TextBox();
            this.sectoridsearch = new System.Windows.Forms.PictureBox();
            this.a = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.salesmannametxtbox = new System.Windows.Forms.TextBox();
            this.salesmanidtxtbox = new System.Windows.Forms.TextBox();
            this.salesmanidsearch = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.townnametxtbox = new System.Windows.Forms.TextBox();
            this.townidtxtbox = new System.Windows.Forms.TextBox();
            this.townidsearch = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cstmrnametxtbox = new System.Windows.Forms.TextBox();
            this.cstmridtxtbox = new System.Windows.Forms.TextBox();
            this.cstmridsearch = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.fromsaletxtbox = new System.Windows.Forms.TextBox();
            this.tosaletxtbox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.sectoridsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesmanidsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.townidsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cstmridsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // todate
            // 
            this.todate.AutoSize = true;
            this.todate.Location = new System.Drawing.Point(341, 104);
            this.todate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.todate.Name = "todate";
            this.todate.Size = new System.Drawing.Size(52, 16);
            this.todate.TabIndex = 330;
            this.todate.Text = "To Date";
            this.todate.Click += new System.EventHandler(this.todate_Click);
            // 
            // fromdatepckr
            // 
            this.fromdatepckr.AutoSize = true;
            this.fromdatepckr.Location = new System.Drawing.Point(203, 104);
            this.fromdatepckr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fromdatepckr.Name = "fromdatepckr";
            this.fromdatepckr.Size = new System.Drawing.Size(67, 16);
            this.fromdatepckr.TabIndex = 329;
            this.fromdatepckr.Text = "From Date";
            this.fromdatepckr.Click += new System.EventHandler(this.fromdatepckr_Click);
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(344, 124);
            this.todatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(114, 23);
            this.todatepcker.TabIndex = 328;
            this.todatepcker.ValueChanged += new System.EventHandler(this.todatepcker_ValueChanged);
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepcker.Location = new System.Drawing.Point(206, 124);
            this.fromdatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(119, 23);
            this.fromdatepcker.TabIndex = 327;
            this.fromdatepcker.ValueChanged += new System.EventHandler(this.fromdatepcker_ValueChanged);
            // 
            // sectornametxtbox
            // 
            this.sectornametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectornametxtbox.Location = new System.Drawing.Point(344, 349);
            this.sectornametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.sectornametxtbox.Multiline = true;
            this.sectornametxtbox.Name = "sectornametxtbox";
            this.sectornametxtbox.Size = new System.Drawing.Size(191, 20);
            this.sectornametxtbox.TabIndex = 326;
            this.sectornametxtbox.TextChanged += new System.EventHandler(this.accountnametxtbox_TextChanged);
            // 
            // sectoridtxtbox
            // 
            this.sectoridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sectoridtxtbox.Location = new System.Drawing.Point(206, 349);
            this.sectoridtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.sectoridtxtbox.Multiline = true;
            this.sectoridtxtbox.Name = "sectoridtxtbox";
            this.sectoridtxtbox.Size = new System.Drawing.Size(95, 20);
            this.sectoridtxtbox.TabIndex = 325;
            this.sectoridtxtbox.TextChanged += new System.EventHandler(this.accountnotxtbox_TextChanged);
            // 
            // sectoridsearch
            // 
            this.sectoridsearch.Image = ((System.Drawing.Image)(resources.GetObject("sectoridsearch.Image")));
            this.sectoridsearch.Location = new System.Drawing.Point(304, 349);
            this.sectoridsearch.Margin = new System.Windows.Forms.Padding(4);
            this.sectoridsearch.Name = "sectoridsearch";
            this.sectoridsearch.Size = new System.Drawing.Size(40, 20);
            this.sectoridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sectoridsearch.TabIndex = 324;
            this.sectoridsearch.TabStop = false;
            this.sectoridsearch.Click += new System.EventHandler(this.searchpartyid_Click);
            // 
            // a
            // 
            this.a.AutoSize = true;
            this.a.Location = new System.Drawing.Point(346, 329);
            this.a.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.a.Name = "a";
            this.a.Size = new System.Drawing.Size(81, 16);
            this.a.TabIndex = 323;
            this.a.Text = "Sector Name";
            this.a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.a.Click += new System.EventHandler(this.a_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(205, 329);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 322;
            this.label1.Text = "Sector ID";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(215, 462);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(77, 26);
            this.previewbtn.TabIndex = 333;
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
            this.printbtn.Location = new System.Drawing.Point(298, 462);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(77, 26);
            this.printbtn.TabIndex = 331;
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
            this.closebtn.Location = new System.Drawing.Point(381, 462);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(77, 26);
            this.closebtn.TabIndex = 332;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(163, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(372, 32);
            this.label2.TabIndex = 334;
            this.label2.Text = "Sales Summary Product Wise";
            // 
            // salesmannametxtbox
            // 
            this.salesmannametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmannametxtbox.Location = new System.Drawing.Point(344, 238);
            this.salesmannametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.salesmannametxtbox.Multiline = true;
            this.salesmannametxtbox.Name = "salesmannametxtbox";
            this.salesmannametxtbox.Size = new System.Drawing.Size(191, 20);
            this.salesmannametxtbox.TabIndex = 339;
            this.salesmannametxtbox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // salesmanidtxtbox
            // 
            this.salesmanidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmanidtxtbox.Location = new System.Drawing.Point(206, 238);
            this.salesmanidtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.salesmanidtxtbox.Multiline = true;
            this.salesmanidtxtbox.Name = "salesmanidtxtbox";
            this.salesmanidtxtbox.Size = new System.Drawing.Size(95, 20);
            this.salesmanidtxtbox.TabIndex = 338;
            this.salesmanidtxtbox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // salesmanidsearch
            // 
            this.salesmanidsearch.Image = ((System.Drawing.Image)(resources.GetObject("salesmanidsearch.Image")));
            this.salesmanidsearch.Location = new System.Drawing.Point(304, 238);
            this.salesmanidsearch.Margin = new System.Windows.Forms.Padding(4);
            this.salesmanidsearch.Name = "salesmanidsearch";
            this.salesmanidsearch.Size = new System.Drawing.Size(40, 20);
            this.salesmanidsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.salesmanidsearch.TabIndex = 337;
            this.salesmanidsearch.TabStop = false;
            this.salesmanidsearch.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(341, 218);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 336;
            this.label3.Text = "Salesman Name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(205, 218);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 16);
            this.label4.TabIndex = 335;
            this.label4.Text = "Salesman ID";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // townnametxtbox
            // 
            this.townnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.townnametxtbox.Location = new System.Drawing.Point(344, 294);
            this.townnametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.townnametxtbox.Multiline = true;
            this.townnametxtbox.Name = "townnametxtbox";
            this.townnametxtbox.Size = new System.Drawing.Size(191, 20);
            this.townnametxtbox.TabIndex = 344;
            this.townnametxtbox.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // townidtxtbox
            // 
            this.townidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.townidtxtbox.Location = new System.Drawing.Point(206, 294);
            this.townidtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.townidtxtbox.Multiline = true;
            this.townidtxtbox.Name = "townidtxtbox";
            this.townidtxtbox.Size = new System.Drawing.Size(95, 20);
            this.townidtxtbox.TabIndex = 343;
            this.townidtxtbox.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // townidsearch
            // 
            this.townidsearch.Image = ((System.Drawing.Image)(resources.GetObject("townidsearch.Image")));
            this.townidsearch.Location = new System.Drawing.Point(304, 294);
            this.townidsearch.Margin = new System.Windows.Forms.Padding(4);
            this.townidsearch.Name = "townidsearch";
            this.townidsearch.Size = new System.Drawing.Size(40, 20);
            this.townidsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.townidsearch.TabIndex = 342;
            this.townidsearch.TabStop = false;
            this.townidsearch.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(341, 274);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 16);
            this.label5.TabIndex = 341;
            this.label5.Text = "Town Name";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(205, 274);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 16);
            this.label6.TabIndex = 340;
            this.label6.Text = "Town ID";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // cstmrnametxtbox
            // 
            this.cstmrnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cstmrnametxtbox.Location = new System.Drawing.Point(344, 187);
            this.cstmrnametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.cstmrnametxtbox.Multiline = true;
            this.cstmrnametxtbox.Name = "cstmrnametxtbox";
            this.cstmrnametxtbox.Size = new System.Drawing.Size(191, 20);
            this.cstmrnametxtbox.TabIndex = 349;
            this.cstmrnametxtbox.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // cstmridtxtbox
            // 
            this.cstmridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cstmridtxtbox.Location = new System.Drawing.Point(206, 187);
            this.cstmridtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.cstmridtxtbox.Multiline = true;
            this.cstmridtxtbox.Name = "cstmridtxtbox";
            this.cstmridtxtbox.Size = new System.Drawing.Size(95, 20);
            this.cstmridtxtbox.TabIndex = 348;
            this.cstmridtxtbox.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // cstmridsearch
            // 
            this.cstmridsearch.Image = ((System.Drawing.Image)(resources.GetObject("cstmridsearch.Image")));
            this.cstmridsearch.Location = new System.Drawing.Point(304, 187);
            this.cstmridsearch.Margin = new System.Windows.Forms.Padding(4);
            this.cstmridsearch.Name = "cstmridsearch";
            this.cstmridsearch.Size = new System.Drawing.Size(40, 20);
            this.cstmridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cstmridsearch.TabIndex = 347;
            this.cstmridsearch.TabStop = false;
            this.cstmridsearch.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(341, 167);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 16);
            this.label7.TabIndex = 346;
            this.label7.Text = "Customer Name";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(205, 167);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 16);
            this.label8.TabIndex = 345;
            this.label8.Text = "Customer ID";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // fromsaletxtbox
            // 
            this.fromsaletxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fromsaletxtbox.Location = new System.Drawing.Point(206, 401);
            this.fromsaletxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.fromsaletxtbox.Multiline = true;
            this.fromsaletxtbox.Name = "fromsaletxtbox";
            this.fromsaletxtbox.Size = new System.Drawing.Size(106, 20);
            this.fromsaletxtbox.TabIndex = 350;
            this.fromsaletxtbox.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
            // 
            // tosaletxtbox
            // 
            this.tosaletxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tosaletxtbox.Location = new System.Drawing.Point(344, 401);
            this.tosaletxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.tosaletxtbox.Multiline = true;
            this.tosaletxtbox.Name = "tosaletxtbox";
            this.tosaletxtbox.Size = new System.Drawing.Size(111, 20);
            this.tosaletxtbox.TabIndex = 351;
            this.tosaletxtbox.TextChanged += new System.EventHandler(this.textBox8_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(205, 381);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 16);
            this.label9.TabIndex = 352;
            this.label9.Text = "From Sale ID";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(346, 381);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 16);
            this.label10.TabIndex = 353;
            this.label10.Text = "To Sale ID";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(621, 0);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(40, 30);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 354;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // Sales_Summary_Product_Wise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(663, 521);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tosaletxtbox);
            this.Controls.Add(this.fromsaletxtbox);
            this.Controls.Add(this.cstmrnametxtbox);
            this.Controls.Add(this.cstmridtxtbox);
            this.Controls.Add(this.cstmridsearch);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.townnametxtbox);
            this.Controls.Add(this.townidtxtbox);
            this.Controls.Add(this.townidsearch);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.salesmannametxtbox);
            this.Controls.Add(this.salesmanidtxtbox);
            this.Controls.Add(this.salesmanidsearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.todate);
            this.Controls.Add(this.fromdatepckr);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.sectornametxtbox);
            this.Controls.Add(this.sectoridtxtbox);
            this.Controls.Add(this.sectoridsearch);
            this.Controls.Add(this.a);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Sales_Summary_Product_Wise";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales_Summary_Product_Wise";
            this.Load += new System.EventHandler(this.Sales_Summary_Product_Wise_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sectoridsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salesmanidsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.townidsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cstmridsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label todate;
        private System.Windows.Forms.Label fromdatepckr;
        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepcker;
        private System.Windows.Forms.TextBox sectornametxtbox;
        private System.Windows.Forms.TextBox sectoridtxtbox;
        private System.Windows.Forms.PictureBox sectoridsearch;
        private System.Windows.Forms.Label a;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox salesmannametxtbox;
        private System.Windows.Forms.TextBox salesmanidtxtbox;
        private System.Windows.Forms.PictureBox salesmanidsearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox townnametxtbox;
        private System.Windows.Forms.TextBox townidtxtbox;
        private System.Windows.Forms.PictureBox townidsearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox cstmrnametxtbox;
        private System.Windows.Forms.TextBox cstmridtxtbox;
        private System.Windows.Forms.PictureBox cstmridsearch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox fromsaletxtbox;
        private System.Windows.Forms.TextBox tosaletxtbox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}