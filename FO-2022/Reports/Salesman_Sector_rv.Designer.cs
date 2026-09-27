namespace FO_2022.Reports
{
    partial class Salesman_Sector_rv
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
            this._Salesman_Summary_dataset_ = new FO_2022._Salesman_Summary_dataset_();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this._Salesman_Summary_dataset_)).BeginInit();
            this.SuspendLayout();
            // 
            // _Salesman_Summary_dataset_
            // 
            this._Salesman_Summary_dataset_.DataSetName = "Salesman_Summary[dataset]";
            this._Salesman_Summary_dataset_.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(800, 450);
            this.reportViewer1.TabIndex = 0;
            //this.reportViewer1.Load += new System.EventHandler(this.reportViewer1_Load);
            // 
            // Salesman_Sector_rv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.reportViewer1);
            this.Name = "Salesman_Sector_rv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Salesman_Sector_rv";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Salesman_Sector_rv_Load);
            ((System.ComponentModel.ISupportInitialize)(this._Salesman_Summary_dataset_)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private _Salesman_Summary_dataset_ _Salesman_Summary_dataset_;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}