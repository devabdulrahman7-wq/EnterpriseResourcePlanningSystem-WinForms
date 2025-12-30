using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Sector_rv : Form
    {
        private DataTable reportData;
        
        public Sector_rv()
        {
            InitializeComponent();
        }

        // Method to set report data
        public void SetReportData(DataTable data)
        {
            reportData = data;
        }

        private void Sector_rv_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                if (reportData == null || reportData.Rows.Count == 0)
                {
                    // Try to load data directly if not passed
                    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        reportData = LoadSectorDataDirectly(connectionString);
                    }
                }

                if (reportData != null && reportData.Rows.Count > 0)
                {
                    // Configure report data source
                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1"; // Must match the DataSet name in .rdlc
                    reportDataSource.Value = reportData;
                    
                    // Clear existing data sources
                    reportViewer1.LocalReport.DataSources.Clear();
                    
                    // Add new data source
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);

                    // Set report path (adjust path as needed)
                    //reportViewer1.LocalReport.ReportPath = @"Reports\Sector.rdlc";
                    //string reportPath = Path.Combine(Application.StartupPath, "Reports", "Sector.rdlc");
                    string reportPath = Path.Combine(Application.StartupPath, "..", "..", "Reports", "Sector.rdlc");
                    reportViewer1.LocalReport.ReportPath = Path.GetFullPath(reportPath);
                    reportViewer1.LocalReport.ReportPath = reportPath;

                    // Refresh the report
                    reportViewer1.RefreshReport();
                }
                else
                {
                    MessageBox.Show("No data available for the report.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // Method to print directly without showing form
        public void PrintReport()
        {
            try
            {
                if (reportData == null || reportData.Rows.Count == 0)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                    
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        reportData = LoadSectorDataDirectly(connectionString);
                    }
                }

                if (reportData != null && reportData.Rows.Count > 0)
                {
                    // Set up the report
                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DataSet1";
                    reportDataSource.Value = reportData;
                    
                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    //reportViewer1.LocalReport.ReportPath = @"Reports\Sector.rdlc";
                    string reportPath = Path.Combine(Application.StartupPath, "..", "..", "Reports", "Sector.rdlc");
                    reportViewer1.LocalReport.ReportPath = Path.GetFullPath(reportPath);
                    // Print the report
                    reportViewer1.PrintDialog();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Print error: {ex.Message}");
            }
        }

        private DataTable LoadSectorDataDirectly(string connectionString)
        {
            DataTable dt = new DataTable();
            
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM dbo.Sectors ORDER BY SectorID";
                
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    conn.Open();
                    
                    using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            
            return dt;
        }
    }
}