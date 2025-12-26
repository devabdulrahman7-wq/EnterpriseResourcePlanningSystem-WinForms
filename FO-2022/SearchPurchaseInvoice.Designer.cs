namespace FO_2022
{
    partial class SearchPurchaseInvoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchPurchaseInvoice));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fromdatepicker = new System.Windows.Forms.DateTimePicker();
            this.todatepicker = new System.Windows.Forms.DateTimePicker();
            this.searchpurchaseidtxtbox = new System.Windows.Forms.TextBox();
            this.radioButtonorderbydate = new System.Windows.Forms.RadioButton();
            this.radioButtonorderbyid = new System.Windows.Forms.RadioButton();
            this.refreshbtn = new System.Windows.Forms.Button();
            this.selectbtn = new System.Windows.Forms.Button();
            this.cancelbtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(65, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search Purchase Invoices";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(53, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "From Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(274, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "To Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(53, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Purchase ID:";
            // 
            // fromdatepicker
            // 
            this.fromdatepicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepicker.Location = new System.Drawing.Point(139, 84);
            this.fromdatepicker.Name = "fromdatepicker";
            this.fromdatepicker.Size = new System.Drawing.Size(109, 20);
            this.fromdatepicker.TabIndex = 4;
            this.fromdatepicker.ValueChanged += new System.EventHandler(this.fromdatepicker_ValueChanged);
            // 
            // todatepicker
            // 
            this.todatepicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepicker.Location = new System.Drawing.Point(332, 84);
            this.todatepicker.Name = "todatepicker";
            this.todatepicker.Size = new System.Drawing.Size(102, 20);
            this.todatepicker.TabIndex = 5;
            this.todatepicker.ValueChanged += new System.EventHandler(this.todatepicker_ValueChanged);
            // 
            // searchpurchaseidtxtbox
            // 
            this.searchpurchaseidtxtbox.Location = new System.Drawing.Point(139, 114);
            this.searchpurchaseidtxtbox.Name = "searchpurchaseidtxtbox";
            this.searchpurchaseidtxtbox.Size = new System.Drawing.Size(109, 20);
            this.searchpurchaseidtxtbox.TabIndex = 6;
            this.searchpurchaseidtxtbox.TextChanged += new System.EventHandler(this.searchpurchaseidtxtbox_TextChanged);
            // 
            // radioButtonorderbydate
            // 
            this.radioButtonorderbydate.AutoSize = true;
            this.radioButtonorderbydate.Location = new System.Drawing.Point(256, 117);
            this.radioButtonorderbydate.Name = "radioButtonorderbydate";
            this.radioButtonorderbydate.Size = new System.Drawing.Size(92, 17);
            this.radioButtonorderbydate.TabIndex = 7;
            this.radioButtonorderbydate.TabStop = true;
            this.radioButtonorderbydate.Text = "Order By Date";
            this.radioButtonorderbydate.UseVisualStyleBackColor = true;
            this.radioButtonorderbydate.CheckedChanged += new System.EventHandler(this.radioButtonorderbydate_CheckedChanged);
            // 
            // radioButtonorderbyid
            // 
            this.radioButtonorderbyid.AutoSize = true;
            this.radioButtonorderbyid.Location = new System.Drawing.Point(354, 117);
            this.radioButtonorderbyid.Name = "radioButtonorderbyid";
            this.radioButtonorderbyid.Size = new System.Drawing.Size(80, 17);
            this.radioButtonorderbyid.TabIndex = 8;
            this.radioButtonorderbyid.TabStop = true;
            this.radioButtonorderbyid.Text = "Order By ID";
            this.radioButtonorderbyid.UseVisualStyleBackColor = true;
            this.radioButtonorderbyid.CheckedChanged += new System.EventHandler(this.radioButtonorderbyid_CheckedChanged);
            // 
            // refreshbtn
            // 
            this.refreshbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.refreshbtn.FlatAppearance.BorderSize = 0;
            this.refreshbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshbtn.Location = new System.Drawing.Point(139, 479);
            this.refreshbtn.Name = "refreshbtn";
            this.refreshbtn.Size = new System.Drawing.Size(75, 26);
            this.refreshbtn.TabIndex = 9;
            this.refreshbtn.Text = "Refresh";
            this.refreshbtn.UseVisualStyleBackColor = false;
            this.refreshbtn.Click += new System.EventHandler(this.refreshbtn_Click);
            // 
            // selectbtn
            // 
            this.selectbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.selectbtn.FlatAppearance.BorderSize = 0;
            this.selectbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectbtn.Location = new System.Drawing.Point(224, 479);
            this.selectbtn.Name = "selectbtn";
            this.selectbtn.Size = new System.Drawing.Size(75, 26);
            this.selectbtn.TabIndex = 10;
            this.selectbtn.Text = "Select";
            this.selectbtn.UseVisualStyleBackColor = false;
            this.selectbtn.Click += new System.EventHandler(this.selectbtn_Click);
            // 
            // cancelbtn
            // 
            this.cancelbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.cancelbtn.FlatAppearance.BorderSize = 0;
            this.cancelbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelbtn.Location = new System.Drawing.Point(305, 479);
            this.cancelbtn.Name = "cancelbtn";
            this.cancelbtn.Size = new System.Drawing.Size(75, 26);
            this.cancelbtn.TabIndex = 11;
            this.cancelbtn.Text = "Cancel";
            this.cancelbtn.UseVisualStyleBackColor = false;
            this.cancelbtn.Click += new System.EventHandler(this.cancelbtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.Location = new System.Drawing.Point(56, 140);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(428, 311);
            this.dataGridView1.TabIndex = 12;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Purchase ID";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Date";
            this.Column2.Name = "Column2";
            this.Column2.Width = 70;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Vendor";
            this.Column3.Name = "Column3";
            this.Column3.Width = 130;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Amount";
            this.Column4.Name = "Column4";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(485, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(60, 41);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 267;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // SearchPurchaseInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(541, 542);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cancelbtn);
            this.Controls.Add(this.selectbtn);
            this.Controls.Add(this.refreshbtn);
            this.Controls.Add(this.radioButtonorderbyid);
            this.Controls.Add(this.radioButtonorderbydate);
            this.Controls.Add(this.searchpurchaseidtxtbox);
            this.Controls.Add(this.todatepicker);
            this.Controls.Add(this.fromdatepicker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchPurchaseInvoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SearchPurchaseInvoice";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker fromdatepicker;
        private System.Windows.Forms.DateTimePicker todatepicker;
        private System.Windows.Forms.TextBox searchpurchaseidtxtbox;
        private System.Windows.Forms.RadioButton radioButtonorderbydate;
        private System.Windows.Forms.RadioButton radioButtonorderbyid;
        private System.Windows.Forms.Button refreshbtn;
        private System.Windows.Forms.Button selectbtn;
        private System.Windows.Forms.Button cancelbtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}