namespace FO_2022
{
    partial class Post_Short_Stock_Order_Invoices
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Post_Short_Stock_Order_Invoices));
            this.label1 = new System.Windows.Forms.Label();
            this.orderdatelbl = new System.Windows.Forms.Label();
            this.datepcker = new System.Windows.Forms.DateTimePicker();
            this.closebtn = new System.Windows.Forms.Button();
            this.postsalebtn = new System.Windows.Forms.Button();
            this.slctbtn = new System.Windows.Forms.Button();
            this.searchdate = new System.Windows.Forms.PictureBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.totalnetsalestxtbox = new System.Windows.Forms.TextBox();
            this.noofsalestxtbox = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.searchdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(242, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(405, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Post Short Stock Order Invoices";
            // 
            // orderdatelbl
            // 
            this.orderdatelbl.AutoSize = true;
            this.orderdatelbl.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orderdatelbl.Location = new System.Drawing.Point(48, 99);
            this.orderdatelbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.orderdatelbl.Name = "orderdatelbl";
            this.orderdatelbl.Size = new System.Drawing.Size(33, 16);
            this.orderdatelbl.TabIndex = 389;
            this.orderdatelbl.Text = "Date";
            // 
            // datepcker
            // 
            this.datepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datepcker.Location = new System.Drawing.Point(42, 118);
            this.datepcker.Name = "datepcker";
            this.datepcker.Size = new System.Drawing.Size(163, 23);
            this.datepcker.TabIndex = 388;
            this.datepcker.ValueChanged += new System.EventHandler(this.datepcker_ValueChanged);
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(372, 557);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(77, 31);
            this.closebtn.TabIndex = 398;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // postsalebtn
            // 
            this.postsalebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.postsalebtn.Enabled = false;
            this.postsalebtn.FlatAppearance.BorderSize = 0;
            this.postsalebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.postsalebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.postsalebtn.Location = new System.Drawing.Point(330, 594);
            this.postsalebtn.Name = "postsalebtn";
            this.postsalebtn.Size = new System.Drawing.Size(149, 31);
            this.postsalebtn.TabIndex = 397;
            this.postsalebtn.Text = "Post Selcted Sales\r\n\r\n";
            this.postsalebtn.UseVisualStyleBackColor = false;
            this.postsalebtn.Click += new System.EventHandler(this.postsalebtn_Click);
            // 
            // slctbtn
            // 
            this.slctbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.slctbtn.Enabled = false;
            this.slctbtn.FlatAppearance.BorderSize = 0;
            this.slctbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slctbtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slctbtn.Location = new System.Drawing.Point(678, 110);
            this.slctbtn.Name = "slctbtn";
            this.slctbtn.Size = new System.Drawing.Size(94, 31);
            this.slctbtn.TabIndex = 396;
            this.slctbtn.Text = "Select All";
            this.slctbtn.UseVisualStyleBackColor = false;
            this.slctbtn.Click += new System.EventHandler(this.slctbtn_Click);
            // 
            // searchdate
            // 
            this.searchdate.Image = ((System.Drawing.Image)(resources.GetObject("searchdate.Image")));
            this.searchdate.Location = new System.Drawing.Point(211, 118);
            this.searchdate.Name = "searchdate";
            this.searchdate.Size = new System.Drawing.Size(46, 23);
            this.searchdate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.searchdate.TabIndex = 399;
            this.searchdate.TabStop = false;
            this.searchdate.Click += new System.EventHandler(this.searchdate_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            this.dataGridView1.Location = new System.Drawing.Point(42, 147);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(730, 384);
            this.dataGridView1.TabIndex = 400;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Sale ID";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Sale Date";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 125;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Customer ID";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Width = 135;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Customer Name";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.Width = 175;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Net Sale";
            this.Column5.MinimumWidth = 6;
            this.Column5.Name = "Column5";
            this.Column5.Width = 125;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Post";
            this.Column6.MinimumWidth = 6;
            this.Column6.Name = "Column6";
            this.Column6.Width = 75;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(580, 534);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 401;
            this.label2.Text = "Total Net Sales:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(49, 534);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 402;
            this.label3.Text = "No of Sales:";
            // 
            // totalnetsalestxtbox
            // 
            this.totalnetsalestxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.totalnetsalestxtbox.Enabled = false;
            this.totalnetsalestxtbox.Location = new System.Drawing.Point(686, 532);
            this.totalnetsalestxtbox.Multiline = true;
            this.totalnetsalestxtbox.Name = "totalnetsalestxtbox";
            this.totalnetsalestxtbox.Size = new System.Drawing.Size(86, 18);
            this.totalnetsalestxtbox.TabIndex = 403;
            this.totalnetsalestxtbox.TextChanged += new System.EventHandler(this.totalnetsalestxtbox_TextChanged);
            // 
            // noofsalestxtbox
            // 
            this.noofsalestxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.noofsalestxtbox.Enabled = false;
            this.noofsalestxtbox.Location = new System.Drawing.Point(133, 532);
            this.noofsalestxtbox.Multiline = true;
            this.noofsalestxtbox.Name = "noofsalestxtbox";
            this.noofsalestxtbox.Size = new System.Drawing.Size(86, 18);
            this.noofsalestxtbox.TabIndex = 404;
            this.noofsalestxtbox.TextChanged += new System.EventHandler(this.noofsalestxtbox_TextChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(782, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 40);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 556;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(569, 79);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(204, 16);
            this.label4.TabIndex = 557;
            this.label4.Text = "Note: This action can not be undo.";
            // 
            // Post_Short_Stock_Order_Invoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(814, 653);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.noofsalestxtbox);
            this.Controls.Add(this.totalnetsalestxtbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.searchdate);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.postsalebtn);
            this.Controls.Add(this.slctbtn);
            this.Controls.Add(this.orderdatelbl);
            this.Controls.Add(this.datepcker);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Post_Short_Stock_Order_Invoices";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Post_Short_Stock_Order_Invoices";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.searchdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label orderdatelbl;
        private System.Windows.Forms.DateTimePicker datepcker;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button postsalebtn;
        private System.Windows.Forms.Button slctbtn;
        private System.Windows.Forms.PictureBox searchdate;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox totalnetsalestxtbox;
        private System.Windows.Forms.TextBox noofsalestxtbox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label4;
    }
}