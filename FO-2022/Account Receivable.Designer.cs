namespace FO_2022
{
    partial class Account_Receivable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Account_Receivable));
            this.label2 = new System.Windows.Forms.Label();
            this.fromdatepcker = new System.Windows.Forms.Label();
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepicker = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.detailbtn = new System.Windows.Forms.RadioButton();
            this.shortbtn = new System.Windows.Forms.RadioButton();
            this.balancegreaterthantxtbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ctgrycombobox = new System.Windows.Forms.ComboBox();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 170);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 16);
            this.label2.TabIndex = 299;
            this.label2.Text = "To Date";
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.AutoSize = true;
            this.fromdatepcker.Location = new System.Drawing.Point(78, 139);
            this.fromdatepcker.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(67, 16);
            this.fromdatepcker.TabIndex = 298;
            this.fromdatepcker.Text = "From Date";
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(255, 165);
            this.todatepcker.Margin = new System.Windows.Forms.Padding(4);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(218, 23);
            this.todatepcker.TabIndex = 297;
            // 
            // fromdatepicker
            // 
            this.fromdatepicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepicker.Location = new System.Drawing.Point(255, 134);
            this.fromdatepicker.Margin = new System.Windows.Forms.Padding(4);
            this.fromdatepicker.Name = "fromdatepicker";
            this.fromdatepicker.Size = new System.Drawing.Size(218, 23);
            this.fromdatepicker.TabIndex = 296;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.detailbtn);
            this.groupBox1.Controls.Add(this.shortbtn);
            this.groupBox1.Location = new System.Drawing.Point(255, 292);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 53);
            this.groupBox1.TabIndex = 300;
            this.groupBox1.TabStop = false;
            // 
            // detailbtn
            // 
            this.detailbtn.AutoSize = true;
            this.detailbtn.Location = new System.Drawing.Point(136, 22);
            this.detailbtn.Name = "detailbtn";
            this.detailbtn.Size = new System.Drawing.Size(63, 20);
            this.detailbtn.TabIndex = 1;
            this.detailbtn.TabStop = true;
            this.detailbtn.Text = "Details";
            this.detailbtn.UseVisualStyleBackColor = true;
            // 
            // shortbtn
            // 
            this.shortbtn.AutoSize = true;
            this.shortbtn.Location = new System.Drawing.Point(6, 22);
            this.shortbtn.Name = "shortbtn";
            this.shortbtn.Size = new System.Drawing.Size(56, 20);
            this.shortbtn.TabIndex = 0;
            this.shortbtn.TabStop = true;
            this.shortbtn.Text = "Short";
            this.shortbtn.UseVisualStyleBackColor = true;
            // 
            // balancegreaterthantxtbox
            // 
            this.balancegreaterthantxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.balancegreaterthantxtbox.Location = new System.Drawing.Point(255, 197);
            this.balancegreaterthantxtbox.Margin = new System.Windows.Forms.Padding(5);
            this.balancegreaterthantxtbox.Multiline = true;
            this.balancegreaterthantxtbox.Name = "balancegreaterthantxtbox";
            this.balancegreaterthantxtbox.Size = new System.Drawing.Size(219, 25);
            this.balancegreaterthantxtbox.TabIndex = 306;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(78, 206);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 16);
            this.label3.TabIndex = 307;
            this.label3.Text = "Having balance greater than";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(78, 250);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 16);
            this.label4.TabIndex = 308;
            this.label4.Text = "Category";
            // 
            // ctgrycombobox
            // 
            this.ctgrycombobox.FormattingEnabled = true;
            this.ctgrycombobox.Location = new System.Drawing.Point(255, 241);
            this.ctgrycombobox.Name = "ctgrycombobox";
            this.ctgrycombobox.Size = new System.Drawing.Size(219, 24);
            this.ctgrycombobox.TabIndex = 309;
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(146, 394);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(5);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(90, 27);
            this.previewbtn.TabIndex = 312;
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
            this.closebtn.Location = new System.Drawing.Point(344, 394);
            this.closebtn.Margin = new System.Windows.Forms.Padding(5);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(90, 27);
            this.closebtn.TabIndex = 311;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.printbtn.FlatAppearance.BorderSize = 0;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(245, 394);
            this.printbtn.Margin = new System.Windows.Forms.Padding(5);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(90, 27);
            this.printbtn.TabIndex = 310;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(232, 41);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(242, 33);
            this.label5.TabIndex = 313;
            this.label5.Text = "Account Receivable";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(541, 0);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(39, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 314;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // Account_Receivable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(580, 466);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.ctgrycombobox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.balancegreaterthantxtbox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepicker);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Account_Receivable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Account_Receivable";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label fromdatepcker;
        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox balancegreaterthantxtbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ctgrycombobox;
        private System.Windows.Forms.RadioButton detailbtn;
        private System.Windows.Forms.RadioButton shortbtn;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}