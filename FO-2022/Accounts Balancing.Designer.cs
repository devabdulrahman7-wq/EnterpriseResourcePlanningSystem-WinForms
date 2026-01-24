namespace FO_2022
{
    partial class Accounts_Balancing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Accounts_Balancing));
            this.label1 = new System.Windows.Forms.Label();
            this.closebtn = new System.Windows.Forms.Button();
            this.prntbtn = new System.Windows.Forms.Button();
            this.previewbtn = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.accountcombobox = new System.Windows.Forms.ComboBox();
            this.excludeaccountshavingzerobalancecheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(92, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(444, 42);
            this.label1.TabIndex = 689;
            this.label1.Text = "Accounts Opening Balance";
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(315, 269);
            this.closebtn.Margin = new System.Windows.Forms.Padding(2);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(75, 28);
            this.closebtn.TabIndex = 688;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // prntbtn
            // 
            this.prntbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.prntbtn.FlatAppearance.BorderSize = 0;
            this.prntbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.prntbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prntbtn.Location = new System.Drawing.Point(236, 269);
            this.prntbtn.Margin = new System.Windows.Forms.Padding(2);
            this.prntbtn.Name = "prntbtn";
            this.prntbtn.Size = new System.Drawing.Size(75, 28);
            this.prntbtn.TabIndex = 687;
            this.prntbtn.Text = "Print";
            this.prntbtn.UseVisualStyleBackColor = false;
            this.prntbtn.Click += new System.EventHandler(this.prntbtn_Click);
            // 
            // previewbtn
            // 
            this.previewbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.previewbtn.FlatAppearance.BorderSize = 0;
            this.previewbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewbtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewbtn.Location = new System.Drawing.Point(157, 269);
            this.previewbtn.Margin = new System.Windows.Forms.Padding(2);
            this.previewbtn.Name = "previewbtn";
            this.previewbtn.Size = new System.Drawing.Size(75, 28);
            this.previewbtn.TabIndex = 686;
            this.previewbtn.Text = "Preview";
            this.previewbtn.UseVisualStyleBackColor = false;
            this.previewbtn.Click += new System.EventHandler(this.previewbtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(540, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 30);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 685;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 21);
            this.label3.TabIndex = 684;
            this.label3.Text = "Account Type";
            // 
            // accountcombobox
            // 
            this.accountcombobox.FormattingEnabled = true;
            this.accountcombobox.Location = new System.Drawing.Point(159, 140);
            this.accountcombobox.Name = "accountcombobox";
            this.accountcombobox.Size = new System.Drawing.Size(244, 27);
            this.accountcombobox.TabIndex = 683;
            this.accountcombobox.SelectedIndexChanged += new System.EventHandler(this.accountcombobox_SelectedIndexChanged);
            // 
            // excludeaccountshavingzerobalancecheckbox
            // 
            this.excludeaccountshavingzerobalancecheckbox.AutoSize = true;
            this.excludeaccountshavingzerobalancecheckbox.Location = new System.Drawing.Point(159, 186);
            this.excludeaccountshavingzerobalancecheckbox.Name = "excludeaccountshavingzerobalancecheckbox";
            this.excludeaccountshavingzerobalancecheckbox.Size = new System.Drawing.Size(319, 25);
            this.excludeaccountshavingzerobalancecheckbox.TabIndex = 690;
            this.excludeaccountshavingzerobalancecheckbox.Text = "Exclude Accounts Having Zero Balance";
            this.excludeaccountshavingzerobalancecheckbox.UseVisualStyleBackColor = true;
            this.excludeaccountshavingzerobalancecheckbox.CheckedChanged += new System.EventHandler(this.excludeaccountshavingzerobalancecheckbox_CheckedChanged);
            // 
            // Accounts_Balancing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(580, 355);
            this.Controls.Add(this.excludeaccountshavingzerobalancecheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.prntbtn);
            this.Controls.Add(this.previewbtn);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.accountcombobox);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Accounts_Balancing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Accounts_Balancing";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Accounts_Balancing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button prntbtn;
        private System.Windows.Forms.Button previewbtn;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox accountcombobox;
        private System.Windows.Forms.CheckBox excludeaccountshavingzerobalancecheckbox;
    }
}