using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Purchase_Summary_rv : Form
    {
        private DataTable _reportData;
        
        public Purchase_Summary_rv(DataTable reportData)
        {
            InitializeComponent();
            _reportData = reportData;
        }
        
        private void Purchase_Summary_rv_Load(object sender, EventArgs e)
        {
            try
            {
                // Check if we have data
                if (_reportData == null || _reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data to display in report!", "Empty Report",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                // Set processing mode
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                
                // Find RDLC file - check multiple locations
                string reportPath = FindReportFile();
                
                if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
                {
                    MessageBox.Show("Report file (Purchase_Summary.rdlc) not found!\n" +
                        "Please make sure the RDLC file is in the correct folder.", 
                        "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                
                // Set report path
                reportViewer1.LocalReport.ReportPath = reportPath;
                
                // Clear existing data sources
                reportViewer1.LocalReport.DataSources.Clear();
                
                // Try to add data source with common dataset names
                bool dataSourceAdded = AddDataSource();
                
                if (!dataSourceAdded)
                {
                    MessageBox.Show("Could not bind data to report. Check dataset name in RDLC file.", 
                        "Data Binding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                // Refresh and show report
                reportViewer1.RefreshReport();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report:\n" + ex.Message, 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private string FindReportFile()
        {
            string[] possiblePaths = 
            {
                // Check in current directory
                Path.Combine(Application.StartupPath, "Purchase_Summary.rdlc"),
                // Check in Reports folder
                Path.Combine(Application.StartupPath, "Reports", "Purchase_Summary.rdlc"),
                // Check in parent directories
                Path.Combine(Application.StartupPath, "..", "..", "Purchase_Summary.rdlc"),
                Path.Combine(Application.StartupPath, "..", "..", "Reports", "Purchase_Summary.rdlc"),
                Path.Combine(Application.StartupPath, "..", "..", "..", "Purchase_Summary.rdlc"),
            };
            
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return null;
        }

        private bool AddDataSource()
        {
            string[] datasetNames = 
            { 
                //"PurchaseSummaryDS",  // Most likely
                "DataSet1",           // Common default
                //"DS_Purchase",        // Alternative
                //"DataSet",            // Generic
                //"PurchaseDS"          // Another alternative
            };
            
            foreach (string datasetName in datasetNames)
            {
                try
                {
                    ReportDataSource rds = new ReportDataSource(datasetName, _reportData);
                    reportViewer1.LocalReport.DataSources.Add(rds);
                    return true;
                }
                catch
                {
                    // Try next name
                    reportViewer1.LocalReport.DataSources.Clear();
                }
            }
            
            return false;
        }
    }
}