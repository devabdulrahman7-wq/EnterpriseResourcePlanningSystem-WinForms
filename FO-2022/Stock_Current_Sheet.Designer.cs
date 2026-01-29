namespace FO_2022
{
    partial class Stock_Current_Sheet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stock_Current_Sheet));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Asad Umair Traders");
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmpnyviewlist = new System.Windows.Forms.ListView();
            this.slctallcompaniescheckbox = new System.Windows.Forms.CheckBox();
            this.showallproductofselectedcompanycheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(638, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 484;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(192, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(263, 32);
            this.label3.TabIndex = 483;
            this.label3.Text = "Stock Current Sheet";
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(208, 417);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(77, 25);
            this.previewbtn.TabIndex = 487;
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
            this.closebtn.Location = new System.Drawing.Point(363, 417);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(68, 25);
            this.closebtn.TabIndex = 486;
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
            this.printbtn.Location = new System.Drawing.Point(289, 417);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(68, 25);
            this.printbtn.TabIndex = 485;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(138, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 489;
            this.label2.Text = "Select Company";
            // 
            // cmpnyviewlist
            // 
            this.cmpnyviewlist.CheckBoxes = true;
            this.cmpnyviewlist.HideSelection = false;
            listViewItem1.StateImageIndex = 0;
            this.cmpnyviewlist.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.cmpnyviewlist.Location = new System.Drawing.Point(137, 134);
            this.cmpnyviewlist.Name = "cmpnyviewlist";
            this.cmpnyviewlist.Size = new System.Drawing.Size(390, 213);
            this.cmpnyviewlist.TabIndex = 488;
            this.cmpnyviewlist.UseCompatibleStateImageBehavior = false;
            this.cmpnyviewlist.View = System.Windows.Forms.View.SmallIcon;
            //this.cmpnyviewlist.SelectedIndexChanged += new System.EventHandler(this.cmpnyviewlist_SelectedIndexChanged);
            // 
            // slctallcompaniescheckbox
            // 
            this.slctallcompaniescheckbox.AutoSize = true;
            this.slctallcompaniescheckbox.Location = new System.Drawing.Point(381, 111);
            this.slctallcompaniescheckbox.Name = "slctallcompaniescheckbox";
            this.slctallcompaniescheckbox.Size = new System.Drawing.Size(146, 20);
            this.slctallcompaniescheckbox.TabIndex = 490;
            this.slctallcompaniescheckbox.Text = "Select All Companies";
            this.slctallcompaniescheckbox.UseVisualStyleBackColor = true;
            this.slctallcompaniescheckbox.CheckedChanged += new System.EventHandler(this.slctallcompaniescheckbox_CheckedChanged);
            // 
            // showallproductofselectedcompanycheckbox
            // 
            this.showallproductofselectedcompanycheckbox.AutoSize = true;
            this.showallproductofselectedcompanycheckbox.Location = new System.Drawing.Point(137, 353);
            this.showallproductofselectedcompanycheckbox.Name = "showallproductofselectedcompanycheckbox";
            this.showallproductofselectedcompanycheckbox.Size = new System.Drawing.Size(269, 20);
            this.showallproductofselectedcompanycheckbox.TabIndex = 491;
            this.showallproductofselectedcompanycheckbox.Text = "Show All Products of Selected Companies:\r\n";
            this.showallproductofselectedcompanycheckbox.UseVisualStyleBackColor = true;
            this.showallproductofselectedcompanycheckbox.CheckedChanged += new System.EventHandler(this.showallproductofselectedcompanycheckbox_CheckedChanged);
            // 
            // Stock_Current_Sheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(675, 481);
            this.Controls.Add(this.showallproductofselectedcompanycheckbox);
            this.Controls.Add(this.slctallcompaniescheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmpnyviewlist);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Stock_Current_Sheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stock_Current_Sheet";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Stock_Current_Sheet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView cmpnyviewlist;
        private System.Windows.Forms.CheckBox slctallcompaniescheckbox;
        private System.Windows.Forms.CheckBox showallproductofselectedcompanycheckbox;
    }
}