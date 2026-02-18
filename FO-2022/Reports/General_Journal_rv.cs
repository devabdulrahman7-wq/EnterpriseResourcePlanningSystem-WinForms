using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class General_Journal_rv : Form
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int FromVoucherNo { get; set; }
        public int ToVoucherNo { get; set; }

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public General_Journal_rv()
        {
            InitializeComponent();
        }

        private void General_Journal_rv_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                DataTable data = GetData();
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("No data found.", "Info");
                    return;
                }
                string reportPath = GetReportPath("General_Journal.rdlc");
                if (reportPath == null) return;
                SetupReport(reportPath, data);
                SetupPageLayout();
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 50
                                jvb.AccountNo,
                                ISNULL(coa.AccountName, 'No Name') as AccountName,
                                ISNULL(jvb.Narration, '') as Narration,
                                ISNULL(jvb.Debit, 0) as Debit,
                                ISNULL(jvb.Credit, 0) as Credit
                               FROM JournalVouchersBody jvb
                               LEFT JOIN ChartOfAccounts coa ON jvb.AccountNo = coa.AccountNo
                               ORDER BY jvb.VoucherNo DESC";

                new SqlDataAdapter(query, conn).Fill(dt);
            }
            return dt;
        }
        private string GetReportPath(string fileName)
        {
            string startupPath = Application.StartupPath;

            // Check common locations
            string[] locations =
            {
                Path.Combine(startupPath, fileName),
                Path.Combine(startupPath, "Reports", fileName)
            };

            foreach (string path in locations)
                if (File.Exists(path))
                    return path;

            MessageBox.Show($"Report file '{fileName}' not found.", "Error");
            return null;
        }
        private void SetupReport(string reportPath, DataTable data)
        {
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(
                new ReportDataSource("DataSet1", data));
        }
        private void SetupPageLayout()
        {
            try
            {
                // Set to whole page view
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.FullPage;  // Changed to FullPage
                reportViewer1.ZoomPercent = 100;
                // Set A4 page with margins
                var pageSettings = new System.Drawing.Printing.PageSettings
                {
                    Margins = new System.Drawing.Printing.Margins(30, 30, 30, 30),
                    PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1169),
                    Landscape = false
                };
                reportViewer1.SetPageSettings(pageSettings);
            }
            catch
            {
                // Silent fail - layout not critical
            }
        }
    }
}
