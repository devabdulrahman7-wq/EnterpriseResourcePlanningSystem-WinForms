namespace FO_2022
{
    partial class Purchase_Register_Invoice_Wise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Purchase_Register_Invoice_Wise));
            this.vendornametxtbox = new System.Windows.Forms.TextBox();
            this.vendoridtxtbox = new System.Windows.Forms.TextBox();
            this.vendoridsearch = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.todate = new System.Windows.Forms.Label();
            this.fromdatepckr = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.frominvoicesearch = new System.Windows.Forms.PictureBox();
            this.toinvoicetxtbox = new System.Windows.Forms.TextBox();
            this.frominvoicetxtbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.vendoridsearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frominvoicesearch)).BeginInit();
            this.SuspendLayout();
            // 
            // vendornametxtbox
            // 
            this.vendornametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vendornametxtbox.Location = new System.Drawing.Point(321, 192);
            this.vendornametxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.vendornametxtbox.Multiline = true;
            this.vendornametxtbox.Name = "vendornametxtbox";
            this.vendornametxtbox.Size = new System.Drawing.Size(180, 27);
            this.vendornametxtbox.TabIndex = 415;
            // 
            // vendoridtxtbox
            // 
            this.vendoridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vendoridtxtbox.Location = new System.Drawing.Point(133, 192);
            this.vendoridtxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.vendoridtxtbox.Multiline = true;
            this.vendoridtxtbox.Name = "vendoridtxtbox";
            this.vendoridtxtbox.Size = new System.Drawing.Size(142, 27);
            this.vendoridtxtbox.TabIndex = 414;
            this.vendoridtxtbox.TextChanged += new System.EventHandler(this.vendoridtxtbox_TextChanged);
            // 
            // vendoridsearch
            // 
            this.vendoridsearch.Image = ((System.Drawing.Image)(resources.GetObject("vendoridsearch.Image")));
            this.vendoridsearch.Location = new System.Drawing.Point(275, 192);
            this.vendoridsearch.Margin = new System.Windows.Forms.Padding(6);
            this.vendoridsearch.Name = "vendoridsearch";
            this.vendoridsearch.Size = new System.Drawing.Size(43, 27);
            this.vendoridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.vendoridsearch.TabIndex = 413;
            this.vendoridsearch.TabStop = false;
            this.vendoridsearch.Click += new System.EventHandler(this.vendoridsearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(319, 170);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 16);
            this.label5.TabIndex = 412;
            this.label5.Text = "Vendor Name";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(130, 170);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 411;
            this.label6.Text = "Vendor ID";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(656, 1);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(37, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 410;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(186, 29);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 62);
            this.label3.TabIndex = 409;
            this.label3.Text = "Purhcase Register \r\n(Invoice Wise)";
            // 
            // todate
            // 
            this.todate.AutoSize = true;
            this.todate.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.todate.Location = new System.Drawing.Point(323, 115);
            this.todate.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.todate.Name = "todate";
            this.todate.Size = new System.Drawing.Size(85, 16);
            this.todate.TabIndex = 408;
            this.todate.Text = "To Invoice No";
            // 
            // fromdatepckr
            // 
            this.fromdatepckr.AutoSize = true;
            this.fromdatepckr.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromdatepckr.Location = new System.Drawing.Point(130, 115);
            this.fromdatepckr.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.fromdatepckr.Name = "fromdatepckr";
            this.fromdatepckr.Size = new System.Drawing.Size(100, 16);
            this.fromdatepckr.TabIndex = 407;
            this.fromdatepckr.Text = "From Invoice No";
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(199, 278);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(5);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(90, 25);
            this.previewbtn.TabIndex = 430;
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
            this.printbtn.Location = new System.Drawing.Point(293, 278);
            this.printbtn.Margin = new System.Windows.Forms.Padding(5);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(82, 25);
            this.printbtn.TabIndex = 428;
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
            this.closebtn.Location = new System.Drawing.Point(378, 278);
            this.closebtn.Margin = new System.Windows.Forms.Padding(5);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(80, 25);
            this.closebtn.TabIndex = 429;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // frominvoicesearch
            // 
            this.frominvoicesearch.Image = ((System.Drawing.Image)(resources.GetObject("frominvoicesearch.Image")));
            this.frominvoicesearch.Location = new System.Drawing.Point(275, 137);
            this.frominvoicesearch.Margin = new System.Windows.Forms.Padding(6);
            this.frominvoicesearch.Name = "frominvoicesearch";
            this.frominvoicesearch.Size = new System.Drawing.Size(43, 27);
            this.frominvoicesearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.frominvoicesearch.TabIndex = 431;
            this.frominvoicesearch.TabStop = false;
            this.frominvoicesearch.Click += new System.EventHandler(this.frominvoicesearch_Click);
            // 
            // toinvoicetxtbox
            // 
            this.toinvoicetxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.toinvoicetxtbox.Location = new System.Drawing.Point(322, 137);
            this.toinvoicetxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.toinvoicetxtbox.Multiline = true;
            this.toinvoicetxtbox.Name = "toinvoicetxtbox";
            this.toinvoicetxtbox.Size = new System.Drawing.Size(179, 27);
            this.toinvoicetxtbox.TabIndex = 433;
            this.toinvoicetxtbox.TextChanged += new System.EventHandler(this.toinvoicetxtbox_TextChanged);
            // 
            // frominvoicetxtbox
            // 
            this.frominvoicetxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.frominvoicetxtbox.Location = new System.Drawing.Point(133, 137);
            this.frominvoicetxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.frominvoicetxtbox.Multiline = true;
            this.frominvoicetxtbox.Name = "frominvoicetxtbox";
            this.frominvoicetxtbox.Size = new System.Drawing.Size(142, 27);
            this.frominvoicetxtbox.TabIndex = 434;
            this.frominvoicetxtbox.TextChanged += new System.EventHandler(this.frominvoicetxtbox_TextChanged);
            // 
            // Purchase_Register_Invoice_Wise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(695, 355);
            this.Controls.Add(this.frominvoicetxtbox);
            this.Controls.Add(this.toinvoicetxtbox);
            this.Controls.Add(this.frominvoicesearch);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.vendornametxtbox);
            this.Controls.Add(this.vendoridtxtbox);
            this.Controls.Add(this.vendoridsearch);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.todate);
            this.Controls.Add(this.fromdatepckr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Purchase_Register_Invoice_Wise";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase_Register_Invoice_Wise";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Purchase_Register_Invoice_Wise_Load);
            ((System.ComponentModel.ISupportInitialize)(this.vendoridsearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frominvoicesearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox vendornametxtbox;
        private System.Windows.Forms.TextBox vendoridtxtbox;
        private System.Windows.Forms.PictureBox vendoridsearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label todate;
        private System.Windows.Forms.Label fromdatepckr;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.PictureBox frominvoicesearch;
        private System.Windows.Forms.TextBox toinvoicetxtbox;
        private System.Windows.Forms.TextBox frominvoicetxtbox;
    }
}