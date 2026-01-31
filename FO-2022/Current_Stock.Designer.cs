namespace FO_2022
{
    partial class Current_Stock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Current_Stock));
            this.previewbtn = new System.Windows.Forms.Button();
            this.closebtn = new System.Windows.Forms.Button();
            this.printbtn = new System.Windows.Forms.Button();
            this.cmpnyviewlist = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.viewonsaleradiobtn = new System.Windows.Forms.RadioButton();
            this.viewonpurchaseradiobtn = new System.Windows.Forms.RadioButton();
            this.viewoncostradiobtn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpviewlist = new System.Windows.Forms.ListView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.slctallgrpbtnfromlist = new System.Windows.Forms.Button();
            this.slctallcmpniesbtnfromlist = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(304, 400);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(77, 25);
            this.previewbtn.TabIndex = 474;
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
            this.closebtn.Location = new System.Drawing.Point(459, 400);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(75, 25);
            this.closebtn.TabIndex = 473;
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
            this.printbtn.Location = new System.Drawing.Point(385, 400);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(68, 25);
            this.printbtn.TabIndex = 472;
            this.printbtn.Text = "Print";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // cmpnyviewlist
            // 
            this.cmpnyviewlist.CheckBoxes = true;
            this.cmpnyviewlist.HideSelection = false;
            this.cmpnyviewlist.Location = new System.Drawing.Point(238, 121);
            this.cmpnyviewlist.Name = "cmpnyviewlist";
            this.cmpnyviewlist.Size = new System.Drawing.Size(180, 213);
            this.cmpnyviewlist.TabIndex = 476;
            this.cmpnyviewlist.UseCompatibleStateImageBehavior = false;
            this.cmpnyviewlist.View = System.Windows.Forms.View.SmallIcon;
            //this.cmpnyviewlist.SelectedIndexChanged += new System.EventHandler(this.cmpnyviewlist_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.viewonsaleradiobtn);
            this.groupBox1.Controls.Add(this.viewonpurchaseradiobtn);
            this.groupBox1.Controls.Add(this.viewoncostradiobtn);
            this.groupBox1.Location = new System.Drawing.Point(69, 190);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 102);
            this.groupBox1.TabIndex = 477;
            this.groupBox1.TabStop = false;
            // 
            // viewonsaleradiobtn
            // 
            this.viewonsaleradiobtn.AutoSize = true;
            this.viewonsaleradiobtn.Location = new System.Drawing.Point(6, 70);
            this.viewonsaleradiobtn.Name = "viewonsaleradiobtn";
            this.viewonsaleradiobtn.Size = new System.Drawing.Size(100, 20);
            this.viewonsaleradiobtn.TabIndex = 2;
            this.viewonsaleradiobtn.TabStop = true;
            this.viewonsaleradiobtn.Text = "View on Sale";
            this.viewonsaleradiobtn.UseVisualStyleBackColor = true;
            //this.viewonsaleradiobtn.CheckedChanged += new System.EventHandler(this.viewonsaleradiobtn_CheckedChanged);
            // 
            // viewonpurchaseradiobtn
            // 
            this.viewonpurchaseradiobtn.AutoSize = true;
            this.viewonpurchaseradiobtn.Location = new System.Drawing.Point(6, 44);
            this.viewonpurchaseradiobtn.Name = "viewonpurchaseradiobtn";
            this.viewonpurchaseradiobtn.Size = new System.Drawing.Size(127, 20);
            this.viewonpurchaseradiobtn.TabIndex = 1;
            this.viewonpurchaseradiobtn.TabStop = true;
            this.viewonpurchaseradiobtn.Text = "View on Purchase";
            this.viewonpurchaseradiobtn.UseVisualStyleBackColor = true;
            //this.viewonpurchaseradiobtn.CheckedChanged += new System.EventHandler(this.viewonpurchaseradiobtn_CheckedChanged);
            // 
            // viewoncostradiobtn
            // 
            this.viewoncostradiobtn.AutoSize = true;
            this.viewoncostradiobtn.Location = new System.Drawing.Point(6, 18);
            this.viewoncostradiobtn.Name = "viewoncostradiobtn";
            this.viewoncostradiobtn.Size = new System.Drawing.Size(100, 20);
            this.viewoncostradiobtn.TabIndex = 0;
            this.viewoncostradiobtn.TabStop = true;
            this.viewoncostradiobtn.Text = "View on Cost";
            this.viewoncostradiobtn.UseVisualStyleBackColor = true;
            //this.viewoncostradiobtn.CheckedChanged += new System.EventHandler(this.viewoncostradiobtn_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(239, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 16);
            this.label2.TabIndex = 478;
            this.label2.Text = "Select Company";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(433, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 479;
            this.label1.Text = "Select Group";
            // 
            // grpviewlist
            // 
            this.grpviewlist.CheckBoxes = true;
            this.grpviewlist.HideSelection = false;
            this.grpviewlist.Location = new System.Drawing.Point(436, 121);
            this.grpviewlist.Name = "grpviewlist";
            this.grpviewlist.Size = new System.Drawing.Size(190, 213);
            this.grpviewlist.TabIndex = 480;
            this.grpviewlist.UseCompatibleStateImageBehavior = false;
            this.grpviewlist.View = System.Windows.Forms.View.SmallIcon;
            //this.grpviewlist.SelectedIndexChanged += new System.EventHandler(this.grpviewlist_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(689, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 482;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(277, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(188, 32);
            this.label3.TabIndex = 481;
            this.label3.Text = "Current Stock";
            // 
            // slctallgrpbtnfromlist
            // 
            this.slctallgrpbtnfromlist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.slctallgrpbtnfromlist.FlatAppearance.BorderSize = 0;
            this.slctallgrpbtnfromlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slctallgrpbtnfromlist.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slctallgrpbtnfromlist.Location = new System.Drawing.Point(551, 93);
            this.slctallgrpbtnfromlist.Name = "slctallgrpbtnfromlist";
            this.slctallgrpbtnfromlist.Size = new System.Drawing.Size(75, 25);
            this.slctallgrpbtnfromlist.TabIndex = 483;
            this.slctallgrpbtnfromlist.Text = "Select All";
            this.slctallgrpbtnfromlist.UseVisualStyleBackColor = false;
            this.slctallgrpbtnfromlist.Click += new System.EventHandler(this.slctallgrpbtnfromlist_Click);
            // 
            // slctallcmpniesbtnfromlist
            // 
            this.slctallcmpniesbtnfromlist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.slctallcmpniesbtnfromlist.FlatAppearance.BorderSize = 0;
            this.slctallcmpniesbtnfromlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.slctallcmpniesbtnfromlist.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slctallcmpniesbtnfromlist.Location = new System.Drawing.Point(343, 93);
            this.slctallcmpniesbtnfromlist.Name = "slctallcmpniesbtnfromlist";
            this.slctallcmpniesbtnfromlist.Size = new System.Drawing.Size(75, 25);
            this.slctallcmpniesbtnfromlist.TabIndex = 484;
            this.slctallcmpniesbtnfromlist.Text = "Select All";
            this.slctallcmpniesbtnfromlist.UseVisualStyleBackColor = false;
            this.slctallcmpniesbtnfromlist.Click += new System.EventHandler(this.slctallcmpniesbtnfromlist_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(433, 337);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 16);
            this.label4.TabIndex = 485;
            this.label4.Text = "No of Groups:6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(239, 337);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 486;
            this.label5.Text = "No of Companies: 1";
            // 
            // Current_Stock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(726, 455);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.slctallcmpniesbtnfromlist);
            this.Controls.Add(this.slctallgrpbtnfromlist);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.grpviewlist);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmpnyviewlist);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.printbtn);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Current_Stock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Current_Stock";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Current_Stock_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.ListView cmpnyviewlist;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView grpviewlist;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton viewonsaleradiobtn;
        private System.Windows.Forms.RadioButton viewonpurchaseradiobtn;
        private System.Windows.Forms.RadioButton viewoncostradiobtn;
        private System.Windows.Forms.Button slctallgrpbtnfromlist;
        private System.Windows.Forms.Button slctallcmpniesbtnfromlist;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}