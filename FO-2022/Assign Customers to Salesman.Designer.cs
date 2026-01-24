namespace FO_2022
{
    partial class Assign_Customers_to_Salesman
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Assign_Customers_to_Salesman));
            this.label10 = new System.Windows.Forms.Label();
            this.cstmrdatetxtbox = new System.Windows.Forms.Label();
            this.salesmanidtxtbox = new System.Windows.Forms.TextBox();
            this.salesmannametxtbox = new System.Windows.Forms.TextBox();
            this.salesmanidsearch = new System.Windows.Forms.PictureBox();
            this.actualsectorcombobox = new System.Windows.Forms.ComboBox();
            this.actualtowncombobox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.refreshbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.clearbtn = new System.Windows.Forms.Button();
            this.savebtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.multiplesalesmancheckbox = new System.Windows.Forms.CheckBox();
            this.UnAssignedCustomersdataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssignedCustomerdataGridView = new System.Windows.Forms.DataGridView();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.nextbtn = new System.Windows.Forms.Button();
            this.backbtn = new System.Windows.Forms.Button();
            this.dblnextbtn = new System.Windows.Forms.Button();
            this.bdlbackbtn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.salesmanidsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnAssignedCustomersdataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AssignedCustomerdataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(444, 109);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 21);
            this.label10.TabIndex = 409;
            this.label10.Text = "Salesman ID";
            // 
            // cstmrdatetxtbox
            // 
            this.cstmrdatetxtbox.AutoSize = true;
            this.cstmrdatetxtbox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cstmrdatetxtbox.Location = new System.Drawing.Point(605, 109);
            this.cstmrdatetxtbox.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.cstmrdatetxtbox.Name = "cstmrdatetxtbox";
            this.cstmrdatetxtbox.Size = new System.Drawing.Size(129, 21);
            this.cstmrdatetxtbox.TabIndex = 408;
            this.cstmrdatetxtbox.Text = "Salesman Name";
            // 
            // salesmanidtxtbox
            // 
            this.salesmanidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmanidtxtbox.Location = new System.Drawing.Point(438, 129);
            this.salesmanidtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.salesmanidtxtbox.Multiline = true;
            this.salesmanidtxtbox.Name = "salesmanidtxtbox";
            this.salesmanidtxtbox.Size = new System.Drawing.Size(130, 20);
            this.salesmanidtxtbox.TabIndex = 407;
            // 
            // salesmannametxtbox
            // 
            this.salesmannametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmannametxtbox.Location = new System.Drawing.Point(595, 129);
            this.salesmannametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.salesmannametxtbox.Multiline = true;
            this.salesmannametxtbox.Name = "salesmannametxtbox";
            this.salesmannametxtbox.Size = new System.Drawing.Size(194, 20);
            this.salesmannametxtbox.TabIndex = 406;
            // 
            // salesmanidsearch
            // 
            this.salesmanidsearch.Image = ((System.Drawing.Image)(resources.GetObject("salesmanidsearch.Image")));
            this.salesmanidsearch.Location = new System.Drawing.Point(568, 127);
            this.salesmanidsearch.Margin = new System.Windows.Forms.Padding(4);
            this.salesmanidsearch.Name = "salesmanidsearch";
            this.salesmanidsearch.Size = new System.Drawing.Size(26, 22);
            this.salesmanidsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.salesmanidsearch.TabIndex = 405;
            this.salesmanidsearch.TabStop = false;
            this.salesmanidsearch.Click += new System.EventHandler(this.salesmanidsearch_Click);
            // 
            // actualsectorcombobox
            // 
            this.actualsectorcombobox.FormattingEnabled = true;
            this.actualsectorcombobox.Location = new System.Drawing.Point(101, 99);
            this.actualsectorcombobox.Name = "actualsectorcombobox";
            this.actualsectorcombobox.Size = new System.Drawing.Size(231, 27);
            this.actualsectorcombobox.TabIndex = 410;
            this.actualsectorcombobox.SelectedIndexChanged += new System.EventHandler(this.actualsectorcombobox_SelectedIndexChanged);
            // 
            // actualtowncombobox
            // 
            this.actualtowncombobox.FormattingEnabled = true;
            this.actualtowncombobox.Location = new System.Drawing.Point(101, 129);
            this.actualtowncombobox.Name = "actualtowncombobox";
            this.actualtowncombobox.Size = new System.Drawing.Size(231, 27);
            this.actualtowncombobox.TabIndex = 411;
            this.actualtowncombobox.SelectedIndexChanged += new System.EventHandler(this.actualtowncombobox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 21);
            this.label1.TabIndex = 412;
            this.label1.Text = "Actual Sectors:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 21);
            this.label2.TabIndex = 413;
            this.label2.Text = "Actual Town:";
            // 
            // refreshbtn
            // 
            this.refreshbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.refreshbtn.FlatAppearance.BorderSize = 0;
            this.refreshbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refreshbtn.Location = new System.Drawing.Point(346, 122);
            this.refreshbtn.Margin = new System.Windows.Forms.Padding(2);
            this.refreshbtn.Name = "refreshbtn";
            this.refreshbtn.Size = new System.Drawing.Size(86, 28);
            this.refreshbtn.TabIndex = 575;
            this.refreshbtn.Text = "Refresh";
            this.refreshbtn.UseVisualStyleBackColor = false;
            this.refreshbtn.Click += new System.EventHandler(this.refreshbtn_Click);
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(474, 530);
            this.closebtn.Margin = new System.Windows.Forms.Padding(2);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(75, 28);
            this.closebtn.TabIndex = 574;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // clearbtn
            // 
            this.clearbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.clearbtn.FlatAppearance.BorderSize = 0;
            this.clearbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearbtn.Location = new System.Drawing.Point(395, 530);
            this.clearbtn.Margin = new System.Windows.Forms.Padding(2);
            this.clearbtn.Name = "clearbtn";
            this.clearbtn.Size = new System.Drawing.Size(75, 28);
            this.clearbtn.TabIndex = 573;
            this.clearbtn.Text = "Clear";
            this.clearbtn.UseVisualStyleBackColor = false;
            this.clearbtn.Click += new System.EventHandler(this.clearbtn_Click);
            // 
            // savebtn
            // 
            this.savebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.savebtn.FlatAppearance.BorderSize = 0;
            this.savebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.savebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savebtn.Location = new System.Drawing.Point(316, 530);
            this.savebtn.Margin = new System.Windows.Forms.Padding(2);
            this.savebtn.Name = "savebtn";
            this.savebtn.Size = new System.Drawing.Size(75, 28);
            this.savebtn.TabIndex = 572;
            this.savebtn.Text = "Save";
            this.savebtn.UseVisualStyleBackColor = false;
            this.savebtn.Click += new System.EventHandler(this.savebtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(204, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(468, 42);
            this.label3.TabIndex = 576;
            this.label3.Text = "Assign Customers to Salesman";
            // 
            // multiplesalesmancheckbox
            // 
            this.multiplesalesmancheckbox.AutoSize = true;
            this.multiplesalesmancheckbox.Location = new System.Drawing.Point(101, 159);
            this.multiplesalesmancheckbox.Name = "multiplesalesmancheckbox";
            this.multiplesalesmancheckbox.Size = new System.Drawing.Size(243, 25);
            this.multiplesalesmancheckbox.TabIndex = 577;
            this.multiplesalesmancheckbox.Text = "Multiple Salesman Allotment";
            this.multiplesalesmancheckbox.UseVisualStyleBackColor = true;
            this.multiplesalesmancheckbox.CheckedChanged += new System.EventHandler(this.multiplesalesmancheckbox_CheckedChanged);
            // 
            // UnAssignedCustomersdataGridView
            // 
            this.UnAssignedCustomersdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UnAssignedCustomersdataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.UnAssignedCustomersdataGridView.Location = new System.Drawing.Point(4, 216);
            this.UnAssignedCustomersdataGridView.Name = "UnAssignedCustomersdataGridView";
            this.UnAssignedCustomersdataGridView.RowHeadersWidth = 51;
            this.UnAssignedCustomersdataGridView.Size = new System.Drawing.Size(337, 294);
            this.UnAssignedCustomersdataGridView.TabIndex = 578;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Customer Name";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            this.Column1.Width = 155;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Customer Town";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            this.Column2.Width = 135;
            // 
            // AssignedCustomerdataGridView
            // 
            this.AssignedCustomerdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AssignedCustomerdataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column3,
            this.Column4});
            this.AssignedCustomerdataGridView.Location = new System.Drawing.Point(438, 216);
            this.AssignedCustomerdataGridView.Name = "AssignedCustomerdataGridView";
            this.AssignedCustomerdataGridView.RowHeadersWidth = 51;
            this.AssignedCustomerdataGridView.Size = new System.Drawing.Size(340, 294);
            this.AssignedCustomerdataGridView.TabIndex = 579;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Customer Name";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            this.Column3.Width = 155;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Customer Town";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            this.Column4.Width = 135;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(4, 192);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(337, 23);
            this.textBox1.TabIndex = 580;
            this.textBox1.Text = "                Available Un-Assigned Customers";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(438, 192);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(340, 23);
            this.textBox2.TabIndex = 581;
            this.textBox2.Text = "                Available Assigned Customers";
            // 
            // nextbtn
            // 
            this.nextbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.nextbtn.FlatAppearance.BorderSize = 0;
            this.nextbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextbtn.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextbtn.Location = new System.Drawing.Point(357, 279);
            this.nextbtn.Margin = new System.Windows.Forms.Padding(2);
            this.nextbtn.Name = "nextbtn";
            this.nextbtn.Size = new System.Drawing.Size(61, 36);
            this.nextbtn.TabIndex = 582;
            this.nextbtn.Text = "<";
            this.nextbtn.UseVisualStyleBackColor = false;
            this.nextbtn.Click += new System.EventHandler(this.nextbtn_Click);
            // 
            // backbtn
            // 
            this.backbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.backbtn.FlatAppearance.BorderSize = 0;
            this.backbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backbtn.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backbtn.Location = new System.Drawing.Point(357, 322);
            this.backbtn.Margin = new System.Windows.Forms.Padding(2);
            this.backbtn.Name = "backbtn";
            this.backbtn.Size = new System.Drawing.Size(61, 36);
            this.backbtn.TabIndex = 583;
            this.backbtn.Text = ">";
            this.backbtn.UseVisualStyleBackColor = false;
            this.backbtn.Click += new System.EventHandler(this.backbtn_Click);
            // 
            // dblnextbtn
            // 
            this.dblnextbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.dblnextbtn.FlatAppearance.BorderSize = 0;
            this.dblnextbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dblnextbtn.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dblnextbtn.Location = new System.Drawing.Point(357, 366);
            this.dblnextbtn.Margin = new System.Windows.Forms.Padding(2);
            this.dblnextbtn.Name = "dblnextbtn";
            this.dblnextbtn.Size = new System.Drawing.Size(61, 36);
            this.dblnextbtn.TabIndex = 584;
            this.dblnextbtn.Text = "<<";
            this.dblnextbtn.UseVisualStyleBackColor = false;
            this.dblnextbtn.Click += new System.EventHandler(this.dblnextbtn_Click);
            // 
            // bdlbackbtn
            // 
            this.bdlbackbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bdlbackbtn.FlatAppearance.BorderSize = 0;
            this.bdlbackbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bdlbackbtn.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bdlbackbtn.Location = new System.Drawing.Point(357, 410);
            this.bdlbackbtn.Margin = new System.Windows.Forms.Padding(2);
            this.bdlbackbtn.Name = "bdlbackbtn";
            this.bdlbackbtn.Size = new System.Drawing.Size(61, 36);
            this.bdlbackbtn.TabIndex = 585;
            this.bdlbackbtn.Text = ">>";
            this.bdlbackbtn.UseVisualStyleBackColor = false;
            this.bdlbackbtn.Click += new System.EventHandler(this.bdlbackbtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(761, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 30);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 586;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // Assign_Customers_to_Salesman
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(796, 592);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.bdlbackbtn);
            this.Controls.Add(this.dblnextbtn);
            this.Controls.Add(this.backbtn);
            this.Controls.Add(this.nextbtn);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.AssignedCustomerdataGridView);
            this.Controls.Add(this.UnAssignedCustomersdataGridView);
            this.Controls.Add(this.multiplesalesmancheckbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.refreshbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.clearbtn);
            this.Controls.Add(this.savebtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.actualtowncombobox);
            this.Controls.Add(this.actualsectorcombobox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cstmrdatetxtbox);
            this.Controls.Add(this.salesmanidtxtbox);
            this.Controls.Add(this.salesmannametxtbox);
            this.Controls.Add(this.salesmanidsearch);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Assign_Customers_to_Salesman";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assign_Customers_to_Salesman";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Assign_Customers_to_Salesman_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.salesmanidsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnAssignedCustomersdataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AssignedCustomerdataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label cstmrdatetxtbox;
        private System.Windows.Forms.TextBox salesmanidtxtbox;
        private System.Windows.Forms.TextBox salesmannametxtbox;
        private System.Windows.Forms.PictureBox salesmanidsearch;
        private System.Windows.Forms.ComboBox actualsectorcombobox;
        private System.Windows.Forms.ComboBox actualtowncombobox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button refreshbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button clearbtn;
        private System.Windows.Forms.Button savebtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox multiplesalesmancheckbox;
        private System.Windows.Forms.DataGridView UnAssignedCustomersdataGridView;
        private System.Windows.Forms.DataGridView AssignedCustomerdataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Button nextbtn;
        private System.Windows.Forms.Button backbtn;
        private System.Windows.Forms.Button dblnextbtn;
        private System.Windows.Forms.Button bdlbackbtn;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}