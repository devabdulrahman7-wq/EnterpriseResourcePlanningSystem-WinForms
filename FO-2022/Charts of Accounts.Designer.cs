namespace FO_2022
{
    partial class Charts_of_Accounts
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
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Current Assets");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Fixed Assets");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Other Assets");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Non-Current Assets");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Assets", new System.Windows.Forms.TreeNode[] {
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14});
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Liabilities");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Equity");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Revnue");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Expense");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Parties");
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.closebtn = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(492, 115);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(346, 396);
            this.dataGridView1.TabIndex = 629;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Account No";
            this.Column1.Name = "Column1";
            this.Column1.Width = 125;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Account Name";
            this.Column2.Name = "Column2";
            this.Column2.Width = 170;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(492, 91);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(346, 19);
            this.textBox1.TabIndex = 628;
            this.textBox1.Text = "                Available Accounts";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(311, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 32);
            this.label1.TabIndex = 630;
            this.label1.Text = "Chart of Accounts";
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.closebtn.FlatAppearance.BorderSize = 0;
            this.closebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closebtn.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closebtn.Location = new System.Drawing.Point(388, 516);
            this.closebtn.Margin = new System.Windows.Forms.Padding(2);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(71, 27);
            this.closebtn.TabIndex = 639;
            this.closebtn.Text = "Close";
            this.closebtn.UseVisualStyleBackColor = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.treeView1.Location = new System.Drawing.Point(86, 115);
            this.treeView1.Name = "treeView1";
            treeNode11.Name = "Node1";
            treeNode11.Text = "Current Assets";
            treeNode12.Name = "Node2";
            treeNode12.Text = "Fixed Assets";
            treeNode13.Name = "Node3";
            treeNode13.Text = "Other Assets";
            treeNode14.Name = "Node4";
            treeNode14.Text = "Non-Current Assets";
            treeNode15.Name = "Node0";
            treeNode15.Text = "Assets";
            treeNode16.Name = "Node5";
            treeNode16.Text = "Liabilities";
            treeNode17.Name = "Node6";
            treeNode17.Text = "Equity";
            treeNode18.Name = "Node7";
            treeNode18.Text = "Revnue";
            treeNode19.Name = "Node8";
            treeNode19.Text = "Expense";
            treeNode20.Name = "Node9";
            treeNode20.Text = "Parties";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20});
            this.treeView1.Size = new System.Drawing.Size(407, 396);
            this.treeView1.TabIndex = 640;
            // 
            // Charts_of_Accounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(846, 586);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Charts_of_Accounts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Charts_of_Accounts";
            this.Load += new System.EventHandler(this.Charts_of_Accounts_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button closebtn;
        private System.Windows.Forms.TreeView treeView1;
    }
}