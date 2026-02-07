namespace FO_2022
{
    partial class Stock_Issue_to_Salesmen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stock_Issue_to_Salesmen));
            this.issuedatepcker = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.salesmennametxtbox = new System.Windows.Forms.TextBox();
            this.salesmenidtxtbox = new System.Windows.Forms.TextBox();
            this.issueidtxtbox = new System.Windows.Forms.TextBox();
            this.saledatelbl = new System.Windows.Forms.Label();
            this.saleidlbl = new System.Windows.Forms.Label();
            this.closebtn = new System.Windows.Forms.Button();
            this.removebtn = new System.Windows.Forms.Button();
            this.openbtn = new System.Windows.Forms.Button();
            this.clearbtn = new System.Windows.Forms.Button();
            this.savebtn = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.packnamecombobox = new System.Windows.Forms.ComboBox();
            this.qtyptxtbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.qtyltxtbox = new System.Windows.Forms.TextBox();
            this.packtxtbox = new System.Windows.Forms.TextBox();
            this.productnametxtbox = new System.Windows.Forms.TextBox();
            this.productidtxtbox = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // issuedatepcker
            // 
            this.issuedatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.issuedatepcker.Location = new System.Drawing.Point(194, 131);
            this.issuedatepcker.Name = "issuedatepcker";
            this.issuedatepcker.Size = new System.Drawing.Size(133, 23);
            this.issuedatepcker.TabIndex = 346;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(214, 165);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 345;
            this.label5.Text = "Salesman Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(80, 165);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 16);
            this.label6.TabIndex = 344;
            this.label6.Text = "Salesman ID";
            // 
            // salesmennametxtbox
            // 
            this.salesmennametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmennametxtbox.Location = new System.Drawing.Point(194, 184);
            this.salesmennametxtbox.Name = "salesmennametxtbox";
            this.salesmennametxtbox.Size = new System.Drawing.Size(322, 16);
            this.salesmennametxtbox.TabIndex = 343;
            // 
            // salesmenidtxtbox
            // 
            this.salesmenidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.salesmenidtxtbox.Location = new System.Drawing.Point(78, 184);
            this.salesmenidtxtbox.Name = "salesmenidtxtbox";
            this.salesmenidtxtbox.Size = new System.Drawing.Size(110, 16);
            this.salesmenidtxtbox.TabIndex = 342;
            this.salesmenidtxtbox.TextChanged += new System.EventHandler(this.salesmenidtxtbox_TextChanged);
            this.salesmenidtxtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.salesmenidtxtbox_KeyDown);
            // 
            // issueidtxtbox
            // 
            this.issueidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.issueidtxtbox.Location = new System.Drawing.Point(78, 131);
            this.issueidtxtbox.Name = "issueidtxtbox";
            this.issueidtxtbox.ReadOnly = true;
            this.issueidtxtbox.Size = new System.Drawing.Size(110, 16);
            this.issueidtxtbox.TabIndex = 341;
            this.issueidtxtbox.TextChanged += new System.EventHandler(this.issueidtxtbox_TextChanged);
            // 
            // saledatelbl
            // 
            this.saledatelbl.AutoSize = true;
            this.saledatelbl.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saledatelbl.Location = new System.Drawing.Point(191, 112);
            this.saledatelbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.saledatelbl.Name = "saledatelbl";
            this.saledatelbl.Size = new System.Drawing.Size(67, 16);
            this.saledatelbl.TabIndex = 340;
            this.saledatelbl.Text = "Issue Date";
            // 
            // saleidlbl
            // 
            this.saleidlbl.AutoSize = true;
            this.saleidlbl.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saleidlbl.Location = new System.Drawing.Point(75, 112);
            this.saleidlbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.saleidlbl.Name = "saleidlbl";
            this.saleidlbl.Size = new System.Drawing.Size(53, 16);
            this.saleidlbl.TabIndex = 339;
            this.saleidlbl.Text = "Issue ID";
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(493, 513);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(77, 28);
            this.closebtn.TabIndex = 389;
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
            this.removebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removebtn.Location = new System.Drawing.Point(410, 513);
            this.removebtn.Name = "removebtn";
            this.removebtn.Size = new System.Drawing.Size(77, 28);
            this.removebtn.TabIndex = 388;
            this.removebtn.Text = "Remove";
            this.removebtn.UseVisualStyleBackColor = false;
            this.removebtn.Click += new System.EventHandler(this.removebtn_Click);
            // 
            // openbtn
            // 
            this.openbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.openbtn.Enabled = false;
            this.openbtn.FlatAppearance.BorderSize = 0;
            this.openbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openbtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openbtn.Location = new System.Drawing.Point(322, 513);
            this.openbtn.Name = "openbtn";
            this.openbtn.Size = new System.Drawing.Size(77, 28);
            this.openbtn.TabIndex = 387;
            this.openbtn.Text = "Open";
            this.openbtn.UseVisualStyleBackColor = false;
            this.openbtn.Click += new System.EventHandler(this.openbtn_Click);
            // 
            // clearbtn
            // 
            this.clearbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.clearbtn.FlatAppearance.BorderSize = 0;
            this.clearbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearbtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearbtn.Location = new System.Drawing.Point(238, 513);
            this.clearbtn.Name = "clearbtn";
            this.clearbtn.Size = new System.Drawing.Size(77, 28);
            this.clearbtn.TabIndex = 386;
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
            this.savebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savebtn.Location = new System.Drawing.Point(157, 513);
            this.savebtn.Name = "savebtn";
            this.savebtn.Size = new System.Drawing.Size(77, 28);
            this.savebtn.TabIndex = 385;
            this.savebtn.Text = "Save";
            this.savebtn.UseVisualStyleBackColor = false;
            this.savebtn.Click += new System.EventHandler(this.savebtn_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(75, 264);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(570, 223);
            this.dataGridView1.TabIndex = 384;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(189, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 32);
            this.label1.TabIndex = 390;
            this.label1.Text = "Stock Issue to Salesmen";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(680, -1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(38, 35);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 391;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // packnamecombobox
            // 
            this.packnamecombobox.FormattingEnabled = true;
            this.packnamecombobox.Location = new System.Drawing.Point(260, 232);
            this.packnamecombobox.Name = "packnamecombobox";
            this.packnamecombobox.Size = new System.Drawing.Size(109, 24);
            this.packnamecombobox.TabIndex = 407;
            this.packnamecombobox.SelectedIndexChanged += new System.EventHandler(this.packnamecombobox_SelectedIndexChanged);
            // 
            // qtyptxtbox
            // 
            this.qtyptxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.qtyptxtbox.Location = new System.Drawing.Point(474, 240);
            this.qtyptxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.qtyptxtbox.Name = "qtyptxtbox";
            this.qtyptxtbox.Size = new System.Drawing.Size(82, 16);
            this.qtyptxtbox.TabIndex = 405;
            this.qtyptxtbox.TextChanged += new System.EventHandler(this.qtyptxtbox_TextChanged);
            this.qtyptxtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.qtyptxtbox_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(561, 218);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 16);
            this.label8.TabIndex = 403;
            this.label8.Text = "Qty(L)";
            // 
            // qtyltxtbox
            // 
            this.qtyltxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.qtyltxtbox.Location = new System.Drawing.Point(564, 238);
            this.qtyltxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.qtyltxtbox.Name = "qtyltxtbox";
            this.qtyltxtbox.Size = new System.Drawing.Size(81, 16);
            this.qtyltxtbox.TabIndex = 400;
            this.qtyltxtbox.TextChanged += new System.EventHandler(this.qtyltxtbox_TextChanged);
            this.qtyltxtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.qtyltxtbox_KeyDown);
            // 
            // packtxtbox
            // 
            this.packtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.packtxtbox.Location = new System.Drawing.Point(376, 240);
            this.packtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.packtxtbox.Name = "packtxtbox";
            this.packtxtbox.Size = new System.Drawing.Size(90, 16);
            this.packtxtbox.TabIndex = 399;
            this.packtxtbox.TextChanged += new System.EventHandler(this.packtxtbox_TextChanged);
            // 
            // productnametxtbox
            // 
            this.productnametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.productnametxtbox.Location = new System.Drawing.Point(157, 237);
            this.productnametxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.productnametxtbox.Name = "productnametxtbox";
            this.productnametxtbox.Size = new System.Drawing.Size(96, 16);
            this.productnametxtbox.TabIndex = 398;
            // 
            // productidtxtbox
            // 
            this.productidtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.productidtxtbox.Location = new System.Drawing.Point(78, 238);
            this.productidtxtbox.Margin = new System.Windows.Forms.Padding(4);
            this.productidtxtbox.Name = "productidtxtbox";
            this.productidtxtbox.Size = new System.Drawing.Size(71, 16);
            this.productidtxtbox.TabIndex = 397;
            this.productidtxtbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.productidtxtbox_KeyDown);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(471, 218);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(44, 16);
            this.label28.TabIndex = 396;
            this.label28.Text = "Qty(P)";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(374, 219);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(38, 16);
            this.label29.TabIndex = 395;
            this.label29.Text = "Pack";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(258, 212);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(78, 16);
            this.label30.TabIndex = 394;
            this.label30.Text = "Pack Name";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(154, 219);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(68, 16);
            this.label31.TabIndex = 393;
            this.label31.Text = "Pro.Name";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(77, 220);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(44, 16);
            this.label32.TabIndex = 392;
            this.label32.Text = "Pro.ID";
            // 
            // Stock_Issue_to_Salesmen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(718, 577);
            this.Controls.Add(this.packnamecombobox);
            this.Controls.Add(this.qtyptxtbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.qtyltxtbox);
            this.Controls.Add(this.packtxtbox);
            this.Controls.Add(this.productnametxtbox);
            this.Controls.Add(this.productidtxtbox);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.removebtn);
            this.Controls.Add(this.openbtn);
            this.Controls.Add(this.clearbtn);
            this.Controls.Add(this.savebtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.issuedatepcker);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.salesmennametxtbox);
            this.Controls.Add(this.salesmenidtxtbox);
            this.Controls.Add(this.issueidtxtbox);
            this.Controls.Add(this.saledatelbl);
            this.Controls.Add(this.saleidlbl);
            this.Enabled = false;
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Stock_Issue_to_Salesmen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock_Issue_to_Salesmen";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Stock_Issue_to_Salesmen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker issuedatepcker;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox salesmennametxtbox;
        private System.Windows.Forms.TextBox salesmenidtxtbox;
        private System.Windows.Forms.TextBox issueidtxtbox;
        private System.Windows.Forms.Label saledatelbl;
        private System.Windows.Forms.Label saleidlbl;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button removebtn;
        private System.Windows.Forms.Button openbtn;
        private System.Windows.Forms.Button clearbtn;
        private System.Windows.Forms.Button savebtn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox packnamecombobox;
        private System.Windows.Forms.TextBox qtyptxtbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox qtyltxtbox;
        private System.Windows.Forms.TextBox packtxtbox;
        private System.Windows.Forms.TextBox productnametxtbox;
        private System.Windows.Forms.TextBox productidtxtbox;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
    }
}