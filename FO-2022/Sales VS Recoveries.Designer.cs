namespace FO_2022
{
    partial class Sales_VS_Recoveries
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sales_VS_Recoveries));
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepcker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.datesearch = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nsmcombobox = new System.Windows.Forms.ComboBox();
            this.asmcombobox = new System.Windows.Forms.ComboBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.slctbtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.asmwithareabtn = new System.Windows.Forms.RadioButton();
            this.asmwithoutareabtn = new System.Windows.Forms.RadioButton();
            this.withbuiltybtn = new System.Windows.Forms.RadioButton();
            this.dailyrprtbtn = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.datesearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(413, 135);
            this.todatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(111, 27);
            this.todatepcker.TabIndex = 338;
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepcker.Location = new System.Drawing.Point(245, 135);
            this.fromdatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(121, 27);
            this.fromdatepcker.TabIndex = 337;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(412, 111);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 21);
            this.label2.TabIndex = 336;
            this.label2.Text = "To Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(245, 111);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 21);
            this.label1.TabIndex = 335;
            this.label1.Text = "From Date ";
            // 
            // datesearch
            // 
            this.datesearch.Image = ((System.Drawing.Image)(resources.GetObject("datesearch.Image")));
            this.datesearch.Location = new System.Drawing.Point(370, 131);
            this.datesearch.Margin = new System.Windows.Forms.Padding(4);
            this.datesearch.Name = "datesearch";
            this.datesearch.Size = new System.Drawing.Size(35, 28);
            this.datesearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.datesearch.TabIndex = 334;
            this.datesearch.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(718, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(41, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 340;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(274, 38);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(315, 42);
            this.label15.TabIndex = 339;
            this.label15.Text = "Sales vs Recoveries";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(245, 167);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 21);
            this.label3.TabIndex = 341;
            this.label3.Text = "NSM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(245, 217);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 21);
            this.label4.TabIndex = 342;
            this.label4.Text = "ASM";
            // 
            // nsmcombobox
            // 
            this.nsmcombobox.FormattingEnabled = true;
            this.nsmcombobox.Location = new System.Drawing.Point(245, 187);
            this.nsmcombobox.Margin = new System.Windows.Forms.Padding(4);
            this.nsmcombobox.Name = "nsmcombobox";
            this.nsmcombobox.Size = new System.Drawing.Size(279, 27);
            this.nsmcombobox.TabIndex = 343;
            // 
            // asmcombobox
            // 
            this.asmcombobox.FormattingEnabled = true;
            this.asmcombobox.Location = new System.Drawing.Point(245, 237);
            this.asmcombobox.Margin = new System.Windows.Forms.Padding(4);
            this.asmcombobox.Name = "asmcombobox";
            this.asmcombobox.Size = new System.Drawing.Size(279, 27);
            this.asmcombobox.TabIndex = 344;
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(243, 301);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(391, 118);
            this.listView1.TabIndex = 345;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeader2
            // 
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(245, 281);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 21);
            this.label5.TabIndex = 346;
            this.label5.Text = "Sales Officers";
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(297, 606);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(4);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(90, 33);
            this.previewbtn.TabIndex = 349;
            this.previewbtn.Text = "Preview";
            this.previewbtn.UseVisualStyleBackColor = false;
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(488, 606);
            this.closebtn.Margin = new System.Windows.Forms.Padding(4);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(90, 33);
            this.closebtn.TabIndex = 348;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.printbtn.FlatAppearance.BorderSize = 0;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(394, 606);
            this.printbtn.Margin = new System.Windows.Forms.Padding(4);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(90, 33);
            this.printbtn.TabIndex = 347;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            // 
            // slctbtn
            // 
            this.slctbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.slctbtn.FlatAppearance.BorderSize = 0;
            this.slctbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slctbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slctbtn.Location = new System.Drawing.Point(546, 273);
            this.slctbtn.Margin = new System.Windows.Forms.Padding(4);
            this.slctbtn.Name = "slctbtn";
            this.slctbtn.Size = new System.Drawing.Size(90, 24);
            this.slctbtn.TabIndex = 350;
            this.slctbtn.Text = "Select All";
            this.slctbtn.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.asmwithareabtn);
            this.groupBox1.Controls.Add(this.asmwithoutareabtn);
            this.groupBox1.Controls.Add(this.withbuiltybtn);
            this.groupBox1.Controls.Add(this.dailyrprtbtn);
            this.groupBox1.Location = new System.Drawing.Point(248, 427);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(392, 136);
            this.groupBox1.TabIndex = 351;
            this.groupBox1.TabStop = false;
            // 
            // asmwithareabtn
            // 
            this.asmwithareabtn.AutoSize = true;
            this.asmwithareabtn.Location = new System.Drawing.Point(22, 41);
            this.asmwithareabtn.Margin = new System.Windows.Forms.Padding(4);
            this.asmwithareabtn.Name = "asmwithareabtn";
            this.asmwithareabtn.Size = new System.Drawing.Size(323, 25);
            this.asmwithareabtn.TabIndex = 3;
            this.asmwithareabtn.TabStop = true;
            this.asmwithareabtn.Text = "Recovery Without Bilty(ASM with Area)";
            this.asmwithareabtn.UseVisualStyleBackColor = true;
            // 
            // asmwithoutareabtn
            // 
            this.asmwithoutareabtn.AutoSize = true;
            this.asmwithoutareabtn.Location = new System.Drawing.Point(22, 69);
            this.asmwithoutareabtn.Margin = new System.Windows.Forms.Padding(4);
            this.asmwithoutareabtn.Name = "asmwithoutareabtn";
            this.asmwithoutareabtn.Size = new System.Drawing.Size(347, 25);
            this.asmwithoutareabtn.TabIndex = 2;
            this.asmwithoutareabtn.TabStop = true;
            this.asmwithoutareabtn.Text = "Recovery Without Bilty(ASM without Area)";
            this.asmwithoutareabtn.UseVisualStyleBackColor = true;
            // 
            // withbuiltybtn
            // 
            this.withbuiltybtn.AutoSize = true;
            this.withbuiltybtn.Location = new System.Drawing.Point(22, 97);
            this.withbuiltybtn.Margin = new System.Windows.Forms.Padding(4);
            this.withbuiltybtn.Name = "withbuiltybtn";
            this.withbuiltybtn.Size = new System.Drawing.Size(175, 25);
            this.withbuiltybtn.TabIndex = 1;
            this.withbuiltybtn.TabStop = true;
            this.withbuiltybtn.Text = "Recovery With Bilty";
            this.withbuiltybtn.UseVisualStyleBackColor = true;
            // 
            // dailyrprtbtn
            // 
            this.dailyrprtbtn.AutoSize = true;
            this.dailyrprtbtn.Location = new System.Drawing.Point(22, 13);
            this.dailyrprtbtn.Margin = new System.Windows.Forms.Padding(4);
            this.dailyrprtbtn.Name = "dailyrprtbtn";
            this.dailyrprtbtn.Size = new System.Drawing.Size(305, 25);
            this.dailyrprtbtn.TabIndex = 0;
            this.dailyrprtbtn.TabStop = true;
            this.dailyrprtbtn.Text = "Recovery Without Bilty(Daily Report)";
            this.dailyrprtbtn.UseVisualStyleBackColor = true;
            this.dailyrprtbtn.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // Sales_VS_Recoveries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(762, 727);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.slctbtn);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.asmcombobox);
            this.Controls.Add(this.nsmcombobox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.datesearch);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Sales_VS_Recoveries";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales_VS_Recoveries";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.datesearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepcker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox datesearch;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox nsmcombobox;
        private System.Windows.Forms.ComboBox asmcombobox;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Button slctbtn;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton dailyrprtbtn;
        private System.Windows.Forms.RadioButton asmwithareabtn;
        private System.Windows.Forms.RadioButton asmwithoutareabtn;
        private System.Windows.Forms.RadioButton withbuiltybtn;
    }
}