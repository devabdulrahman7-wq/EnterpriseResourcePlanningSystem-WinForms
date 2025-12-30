namespace FO_2022
{
    partial class Purchase_Summary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Purchase_Summary));
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.vendornametxtbox = new System.Windows.Forms.TextBox();
            this.vendoridtxtbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.todate = new System.Windows.Forms.Label();
            this.fromdatepckr = new System.Windows.Forms.Label();
            this.todatepcker = new System.Windows.Forms.DateTimePicker();
            this.fromdatepcker = new System.Windows.Forms.DateTimePicker();
            this.vendoridsearch = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vendoridsearch)).BeginInit();
            this.SuspendLayout();
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(204, 287);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(5);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(109, 26);
            this.previewbtn.TabIndex = 446;
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
            this.closebtn.Location = new System.Drawing.Point(323, 287);
            this.closebtn.Margin = new System.Windows.Forms.Padding(5);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(61, 26);
            this.closebtn.TabIndex = 445;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // vendornametxtbox
            // 
            this.vendornametxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vendornametxtbox.Location = new System.Drawing.Point(322, 192);
            this.vendornametxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.vendornametxtbox.Multiline = true;
            this.vendornametxtbox.Name = "vendornametxtbox";
            this.vendornametxtbox.Size = new System.Drawing.Size(190, 27);
            this.vendornametxtbox.TabIndex = 443;
            this.vendornametxtbox.TextChanged += new System.EventHandler(this.vendornametxtbox_TextChanged);
            // 
            // vendoridtxtbox
            // 
            this.vendoridtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.vendoridtxtbox.Location = new System.Drawing.Point(134, 192);
            this.vendoridtxtbox.Margin = new System.Windows.Forms.Padding(6);
            this.vendoridtxtbox.Multiline = true;
            this.vendoridtxtbox.Name = "vendoridtxtbox";
            this.vendoridtxtbox.Size = new System.Drawing.Size(130, 27);
            this.vendoridtxtbox.TabIndex = 442;
            this.vendoridtxtbox.TextChanged += new System.EventHandler(this.vendoridtxtbox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(320, 170);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 16);
            this.label5.TabIndex = 440;
            this.label5.Text = "Vendor Name";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(131, 170);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 16);
            this.label6.TabIndex = 439;
            this.label6.Text = "Vendor ID";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(561, 1);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(37, 36);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 438;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(199, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(242, 31);
            this.label3.TabIndex = 437;
            this.label3.Text = "Purhcase Summary";
            // 
            // todate
            // 
            this.todate.AutoSize = true;
            this.todate.Location = new System.Drawing.Point(319, 108);
            this.todate.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.todate.Name = "todate";
            this.todate.Size = new System.Drawing.Size(52, 16);
            this.todate.TabIndex = 450;
            this.todate.Text = "To Date";
            // 
            // fromdatepckr
            // 
            this.fromdatepckr.AutoSize = true;
            this.fromdatepckr.Location = new System.Drawing.Point(131, 108);
            this.fromdatepckr.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.fromdatepckr.Name = "fromdatepckr";
            this.fromdatepckr.Size = new System.Drawing.Size(67, 16);
            this.fromdatepckr.TabIndex = 449;
            this.fromdatepckr.Text = "From Date";
            // 
            // todatepcker
            // 
            this.todatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.todatepcker.Location = new System.Drawing.Point(322, 130);
            this.todatepcker.Margin = new System.Windows.Forms.Padding(6);
            this.todatepcker.Name = "todatepcker";
            this.todatepcker.Size = new System.Drawing.Size(101, 23);
            this.todatepcker.TabIndex = 448;
            this.todatepcker.ValueChanged += new System.EventHandler(this.todatepcker_ValueChanged);
            // 
            // fromdatepcker
            // 
            this.fromdatepcker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromdatepcker.Location = new System.Drawing.Point(134, 130);
            this.fromdatepcker.Margin = new System.Windows.Forms.Padding(6);
            this.fromdatepcker.Name = "fromdatepcker";
            this.fromdatepcker.Size = new System.Drawing.Size(100, 23);
            this.fromdatepcker.TabIndex = 447;
            this.fromdatepcker.ValueChanged += new System.EventHandler(this.fromdatepcker_ValueChanged);
            // 
            // vendoridsearch
            // 
            this.vendoridsearch.Image = ((System.Drawing.Image)(resources.GetObject("vendoridsearch.Image")));
            this.vendoridsearch.Location = new System.Drawing.Point(277, 191);
            this.vendoridsearch.Margin = new System.Windows.Forms.Padding(6);
            this.vendoridsearch.Name = "vendoridsearch";
            this.vendoridsearch.Size = new System.Drawing.Size(33, 28);
            this.vendoridsearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.vendoridsearch.TabIndex = 461;
            this.vendoridsearch.TabStop = false;
            this.vendoridsearch.Click += new System.EventHandler(this.vendoridsearch_Click);
            // 
            // Purchase_Summary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(600, 353);
            this.Controls.Add(this.vendoridsearch);
            this.Controls.Add(this.todate);
            this.Controls.Add(this.fromdatepckr);
            this.Controls.Add(this.todatepcker);
            this.Controls.Add(this.fromdatepcker);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.vendornametxtbox);
            this.Controls.Add(this.vendoridtxtbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Purchase_Summary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase_Summary";
            this.Load += new System.EventHandler(this.Purchase_Summary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vendoridsearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.TextBox vendornametxtbox;
        private System.Windows.Forms.TextBox vendoridtxtbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label todate;
        private System.Windows.Forms.Label fromdatepckr;
        private System.Windows.Forms.DateTimePicker todatepcker;
        private System.Windows.Forms.DateTimePicker fromdatepcker;
        private System.Windows.Forms.PictureBox vendoridsearch;
    }
}