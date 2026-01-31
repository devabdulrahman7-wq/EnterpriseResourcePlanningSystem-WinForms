namespace FO_2022
{
    partial class Account_Balances
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Account_Balances));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.includepartiesbtnradio = new System.Windows.Forms.RadioButton();
            this.excludepartiesbtnradio = new System.Windows.Forms.RadioButton();
            this.onlypartiesbtnradio = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.showbothbtnradio = new System.Windows.Forms.RadioButton();
            this.showaccountgrpbtnradio = new System.Windows.Forms.RadioButton();
            this.showonlytransactionbtnradio = new System.Windows.Forms.RadioButton();
            this.tilldatebtnradio = new System.Windows.Forms.RadioButton();
            this.daterangebtnradio = new System.Windows.Forms.RadioButton();
            this.alldatebtnradio = new System.Windows.Forms.RadioButton();
            this.datepickerfortilldateradiobtn = new System.Windows.Forms.DateTimePicker();
            this.checkBoxExcludeaccountshavingzerobalance = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.partytypecombobox = new System.Windows.Forms.ComboBox();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.includepartiesbtnradio);
            this.groupBox2.Controls.Add(this.excludepartiesbtnradio);
            this.groupBox2.Controls.Add(this.onlypartiesbtnradio);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(223, 340);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 89);
            this.groupBox2.TabIndex = 209;
            this.groupBox2.TabStop = false;
            // 
            // includepartiesbtnradio
            // 
            this.includepartiesbtnradio.AutoSize = true;
            this.includepartiesbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.includepartiesbtnradio.Location = new System.Drawing.Point(13, 62);
            this.includepartiesbtnradio.Name = "includepartiesbtnradio";
            this.includepartiesbtnradio.Size = new System.Drawing.Size(109, 20);
            this.includepartiesbtnradio.TabIndex = 4;
            this.includepartiesbtnradio.TabStop = true;
            this.includepartiesbtnradio.Text = "Include Parties";
            this.includepartiesbtnradio.UseVisualStyleBackColor = true;
            this.includepartiesbtnradio.CheckedChanged += new System.EventHandler(this.includepartiesbtnradio_CheckedChanged);
            // 
            // excludepartiesbtnradio
            // 
            this.excludepartiesbtnradio.AutoSize = true;
            this.excludepartiesbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excludepartiesbtnradio.Location = new System.Drawing.Point(13, 13);
            this.excludepartiesbtnradio.Name = "excludepartiesbtnradio";
            this.excludepartiesbtnradio.Size = new System.Drawing.Size(111, 20);
            this.excludepartiesbtnradio.TabIndex = 3;
            this.excludepartiesbtnradio.TabStop = true;
            this.excludepartiesbtnradio.Text = "Exclude Parties";
            this.excludepartiesbtnradio.UseVisualStyleBackColor = true;
            this.excludepartiesbtnradio.CheckedChanged += new System.EventHandler(this.excludepartiesbtnradio_CheckedChanged);
            // 
            // onlypartiesbtnradio
            // 
            this.onlypartiesbtnradio.AutoSize = true;
            this.onlypartiesbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.onlypartiesbtnradio.Location = new System.Drawing.Point(13, 36);
            this.onlypartiesbtnradio.Name = "onlypartiesbtnradio";
            this.onlypartiesbtnradio.Size = new System.Drawing.Size(93, 20);
            this.onlypartiesbtnradio.TabIndex = 2;
            this.onlypartiesbtnradio.TabStop = true;
            this.onlypartiesbtnradio.Text = "Only Parties";
            this.onlypartiesbtnradio.UseVisualStyleBackColor = true;
            this.onlypartiesbtnradio.CheckedChanged += new System.EventHandler(this.onlypartiesbtnradio_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.showbothbtnradio);
            this.groupBox1.Controls.Add(this.showaccountgrpbtnradio);
            this.groupBox1.Controls.Add(this.showonlytransactionbtnradio);
            this.groupBox1.Location = new System.Drawing.Point(223, 197);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 92);
            this.groupBox1.TabIndex = 208;
            this.groupBox1.TabStop = false;
            // 
            // showbothbtnradio
            // 
            this.showbothbtnradio.AutoSize = true;
            this.showbothbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showbothbtnradio.Location = new System.Drawing.Point(13, 65);
            this.showbothbtnradio.Name = "showbothbtnradio";
            this.showbothbtnradio.Size = new System.Drawing.Size(141, 20);
            this.showbothbtnradio.TabIndex = 2;
            this.showbothbtnradio.TabStop = true;
            this.showbothbtnradio.Text = "Show Both Accounts";
            this.showbothbtnradio.UseVisualStyleBackColor = true;
            this.showbothbtnradio.CheckedChanged += new System.EventHandler(this.showbothbtnradio_CheckedChanged);
            // 
            // showaccountgrpbtnradio
            // 
            this.showaccountgrpbtnradio.AutoSize = true;
            this.showaccountgrpbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showaccountgrpbtnradio.Location = new System.Drawing.Point(13, 39);
            this.showaccountgrpbtnradio.Name = "showaccountgrpbtnradio";
            this.showaccountgrpbtnradio.Size = new System.Drawing.Size(177, 20);
            this.showaccountgrpbtnradio.TabIndex = 1;
            this.showaccountgrpbtnradio.TabStop = true;
            this.showaccountgrpbtnradio.Text = "Show only Accounts Group";
            this.showaccountgrpbtnradio.UseVisualStyleBackColor = true;
            this.showaccountgrpbtnradio.CheckedChanged += new System.EventHandler(this.showaccountgrpbtnradio_CheckedChanged);
            // 
            // showonlytransactionbtnradio
            // 
            this.showonlytransactionbtnradio.AutoSize = true;
            this.showonlytransactionbtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showonlytransactionbtnradio.Location = new System.Drawing.Point(13, 13);
            this.showonlytransactionbtnradio.Name = "showonlytransactionbtnradio";
            this.showonlytransactionbtnradio.Size = new System.Drawing.Size(216, 20);
            this.showonlytransactionbtnradio.TabIndex = 0;
            this.showonlytransactionbtnradio.TabStop = true;
            this.showonlytransactionbtnradio.Text = "Show only Transactions Accounts";
            this.showonlytransactionbtnradio.UseVisualStyleBackColor = true;
            this.showonlytransactionbtnradio.CheckedChanged += new System.EventHandler(this.showonlytransactionbtnradio_CheckedChanged);
            // 
            // tilldatebtnradio
            // 
            this.tilldatebtnradio.AutoSize = true;
            this.tilldatebtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tilldatebtnradio.Location = new System.Drawing.Point(233, 151);
            this.tilldatebtnradio.Name = "tilldatebtnradio";
            this.tilldatebtnradio.Size = new System.Drawing.Size(72, 20);
            this.tilldatebtnradio.TabIndex = 206;
            this.tilldatebtnradio.TabStop = true;
            this.tilldatebtnradio.Text = "Till Date";
            this.tilldatebtnradio.UseVisualStyleBackColor = true;
            this.tilldatebtnradio.CheckedChanged += new System.EventHandler(this.tilldatebtnradio_CheckedChanged);
            // 
            // daterangebtnradio
            // 
            this.daterangebtnradio.AutoSize = true;
            this.daterangebtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.daterangebtnradio.Location = new System.Drawing.Point(233, 125);
            this.daterangebtnradio.Name = "daterangebtnradio";
            this.daterangebtnradio.Size = new System.Drawing.Size(91, 20);
            this.daterangebtnradio.TabIndex = 205;
            this.daterangebtnradio.TabStop = true;
            this.daterangebtnradio.Text = "Date Range";
            this.daterangebtnradio.UseVisualStyleBackColor = true;
            this.daterangebtnradio.CheckedChanged += new System.EventHandler(this.daterangebtnradio_CheckedChanged);
            // 
            // alldatebtnradio
            // 
            this.alldatebtnradio.AutoSize = true;
            this.alldatebtnradio.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alldatebtnradio.Location = new System.Drawing.Point(233, 102);
            this.alldatebtnradio.Name = "alldatebtnradio";
            this.alldatebtnradio.Size = new System.Drawing.Size(69, 20);
            this.alldatebtnradio.TabIndex = 204;
            this.alldatebtnradio.TabStop = true;
            this.alldatebtnradio.Text = "All Date";
            this.alldatebtnradio.UseVisualStyleBackColor = true;
            this.alldatebtnradio.CheckedChanged += new System.EventHandler(this.alldatebtnradio_CheckedChanged);
            // 
            // datepickerfortilldateradiobtn
            // 
            this.datepickerfortilldateradiobtn.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datepickerfortilldateradiobtn.Location = new System.Drawing.Point(351, 148);
            this.datepickerfortilldateradiobtn.Name = "datepickerfortilldateradiobtn";
            this.datepickerfortilldateradiobtn.Size = new System.Drawing.Size(99, 23);
            this.datepickerfortilldateradiobtn.TabIndex = 210;
            this.datepickerfortilldateradiobtn.ValueChanged += new System.EventHandler(this.datepickerfortilldateradiobtn_ValueChanged);
            // 
            // checkBoxExcludeaccountshavingzerobalance
            // 
            this.checkBoxExcludeaccountshavingzerobalance.AutoSize = true;
            this.checkBoxExcludeaccountshavingzerobalance.Location = new System.Drawing.Point(223, 314);
            this.checkBoxExcludeaccountshavingzerobalance.Name = "checkBoxExcludeaccountshavingzerobalance";
            this.checkBoxExcludeaccountshavingzerobalance.Size = new System.Drawing.Size(241, 20);
            this.checkBoxExcludeaccountshavingzerobalance.TabIndex = 211;
            this.checkBoxExcludeaccountshavingzerobalance.Text = "Exclude accounts having zero balance";
            this.checkBoxExcludeaccountshavingzerobalance.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(255, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 33);
            this.label1.TabIndex = 212;
            this.label1.Text = "Account Balances ";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(640, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 30);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 271;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 467);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 16);
            this.label2.TabIndex = 272;
            this.label2.Text = "Party Type";
            // 
            // partytypecombobox
            // 
            this.partytypecombobox.FormattingEnabled = true;
            this.partytypecombobox.Location = new System.Drawing.Point(223, 490);
            this.partytypecombobox.Name = "partytypecombobox";
            this.partytypecombobox.Size = new System.Drawing.Size(291, 24);
            this.partytypecombobox.TabIndex = 273;
            this.partytypecombobox.SelectedIndexChanged += new System.EventHandler(this.partytypecombobox_SelectedIndexChanged);
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(253, 557);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(77, 24);
            this.previewbtn.TabIndex = 276;
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
            this.closebtn.Location = new System.Drawing.Point(419, 557);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(77, 24);
            this.closebtn.TabIndex = 275;
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
            this.printbtn.Location = new System.Drawing.Point(336, 557);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(77, 24);
            this.printbtn.TabIndex = 274;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // Account_Balances
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(679, 618);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.partytypecombobox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxExcludeaccountshavingzerobalance);
            this.Controls.Add(this.datepickerfortilldateradiobtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tilldatebtnradio);
            this.Controls.Add(this.daterangebtnradio);
            this.Controls.Add(this.alldatebtnradio);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Account_Balances";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Account_Balances";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Account_Balances_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton excludepartiesbtnradio;
        private System.Windows.Forms.RadioButton onlypartiesbtnradio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton showaccountgrpbtnradio;
        private System.Windows.Forms.RadioButton showonlytransactionbtnradio;
        private System.Windows.Forms.RadioButton tilldatebtnradio;
        private System.Windows.Forms.RadioButton daterangebtnradio;
        private System.Windows.Forms.RadioButton alldatebtnradio;
        private System.Windows.Forms.RadioButton includepartiesbtnradio;
        private System.Windows.Forms.RadioButton showbothbtnradio;
        private System.Windows.Forms.DateTimePicker datepickerfortilldateradiobtn;
        private System.Windows.Forms.CheckBox checkBoxExcludeaccountshavingzerobalance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox partytypecombobox;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
    }
}