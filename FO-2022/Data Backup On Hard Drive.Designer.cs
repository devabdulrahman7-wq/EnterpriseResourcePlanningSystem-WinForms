namespace FO_2022
{
    partial class Data_Backup_On_Hard_Drive
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Data_Backup_On_Hard_Drive));
            this.closebtn = new System.Windows.Forms.Button();
            this.backupbtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.filesearch = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.backuppasswordtxtbox = new System.Windows.Forms.TextBox();
            this.harddrivebackupfiletxtbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.backuptimetxtbox = new System.Windows.Forms.TextBox();
            this.backupdatetxtbox = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.filesearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(323, 320);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(77, 28);
            this.closebtn.TabIndex = 403;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // backupbtn
            // 
            this.backupbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.backupbtn.FlatAppearance.BorderSize = 0;
            this.backupbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backupbtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backupbtn.Location = new System.Drawing.Point(240, 320);
            this.backupbtn.Name = "backupbtn";
            this.backupbtn.Size = new System.Drawing.Size(77, 28);
            this.backupbtn.TabIndex = 402;
            this.backupbtn.Text = "Backup";
            this.backupbtn.UseVisualStyleBackColor = false;
            this.backupbtn.Click += new System.EventHandler(this.backupbtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(137, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(367, 32);
            this.label1.TabIndex = 404;
            this.label1.Text = "Data Backup On Hard Drive";
            // 
            // filesearch
            // 
            this.filesearch.Image = ((System.Drawing.Image)(resources.GetObject("filesearch.Image")));
            this.filesearch.Location = new System.Drawing.Point(478, 184);
            this.filesearch.Name = "filesearch";
            this.filesearch.Size = new System.Drawing.Size(27, 20);
            this.filesearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.filesearch.TabIndex = 407;
            this.filesearch.TabStop = false;
            this.filesearch.Click += new System.EventHandler(this.filesearch_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(162, 220);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 16);
            this.label6.TabIndex = 406;
            this.label6.Text = "Backup Password";
            this.label6.Visible = false;
            // 
            // backuppasswordtxtbox
            // 
            this.backuppasswordtxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.backuppasswordtxtbox.Location = new System.Drawing.Point(165, 239);
            this.backuppasswordtxtbox.Name = "backuppasswordtxtbox";
            this.backuppasswordtxtbox.Size = new System.Drawing.Size(307, 16);
            this.backuppasswordtxtbox.TabIndex = 405;
            this.backuppasswordtxtbox.Visible = false;
            this.backuppasswordtxtbox.TextChanged += new System.EventHandler(this.backuppasswordtxtbox_TextChanged);
            // 
            // harddrivebackupfiletxtbox
            // 
            this.harddrivebackupfiletxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.harddrivebackupfiletxtbox.Location = new System.Drawing.Point(165, 186);
            this.harddrivebackupfiletxtbox.Name = "harddrivebackupfiletxtbox";
            this.harddrivebackupfiletxtbox.Size = new System.Drawing.Size(307, 16);
            this.harddrivebackupfiletxtbox.TabIndex = 408;
            this.harddrivebackupfiletxtbox.TextChanged += new System.EventHandler(this.harddrivebackupfiletxtbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(162, 167);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 409;
            this.label2.Text = "Backup File Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(302, 114);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 410;
            this.label3.Text = "Backup Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(162, 114);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 16);
            this.label4.TabIndex = 411;
            this.label4.Text = "Backup Date";
            // 
            // backuptimetxtbox
            // 
            this.backuptimetxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.backuptimetxtbox.Location = new System.Drawing.Point(305, 133);
            this.backuptimetxtbox.Name = "backuptimetxtbox";
            this.backuptimetxtbox.ReadOnly = true;
            this.backuptimetxtbox.Size = new System.Drawing.Size(120, 16);
            this.backuptimetxtbox.TabIndex = 412;
            this.backuptimetxtbox.TextChanged += new System.EventHandler(this.backuptimetxtbox_TextChanged);
            // 
            // backupdatetxtbox
            // 
            this.backupdatetxtbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.backupdatetxtbox.Location = new System.Drawing.Point(165, 133);
            this.backupdatetxtbox.Name = "backupdatetxtbox";
            this.backupdatetxtbox.ReadOnly = true;
            this.backupdatetxtbox.Size = new System.Drawing.Size(123, 16);
            this.backupdatetxtbox.TabIndex = 413;
            this.backupdatetxtbox.TextChanged += new System.EventHandler(this.backupdatetxtbox_TextChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(570, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(43, 29);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 414;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // Data_Backup_On_Hard_Drive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(615, 373);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.backupdatetxtbox);
            this.Controls.Add(this.backuptimetxtbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.harddrivebackupfiletxtbox);
            this.Controls.Add(this.filesearch);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.backuppasswordtxtbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.backupbtn);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Data_Backup_On_Hard_Drive";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data_Backup_On_Hard_Drive";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Data_Backup_On_Hard_Drive_Load);
            ((System.ComponentModel.ISupportInitialize)(this.filesearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.Button backupbtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox filesearch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox backuppasswordtxtbox;
        private System.Windows.Forms.TextBox harddrivebackupfiletxtbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox backuptimetxtbox;
        private System.Windows.Forms.TextBox backupdatetxtbox;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}