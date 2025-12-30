using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Charts_of_Accounts_rv : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //private string connectionString=ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // Property to receive selected account type from main for
        public string SelectedAccountType { get; set; }

        public Charts_of_Accounts_rv()
        {
            InitializeComponent();
        }

        //private void Charts_of_Accounts_rv_Load(object sender, EventArgs e)
        //{
        //    LoadReport();
        //}

        //private void LoadReport()
        //{
        //    try
        //    {
        //        MessageBox.Show($"Selected Account Type: {SelectedAccountType}", "Debug");
        //        // Get data based on selected account type
        //        DataSet ds = GetChartOfAccountsData(SelectedAccountType);

        //        if (ds.Tables[0].Rows.Count == 0)
        //        {
        //            MessageBox.Show("No data found for selected account type.", "Information",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }

        //        // DEBUG: Show what data we got
        //        MessageBox.Show($"Got {ds.Tables[0].Rows.Count} rows of data", "Debug Info");

        //        // DEBUG: Show column names
        //        string columns = "";
        //        foreach (DataColumn col in ds.Tables[0].Columns)
        //        {
        //            columns += col.ColumnName + ", ";
        //        }
        //        MessageBox.Show($"Columns: {columns}", "Debug Info");

        //        // Set up report viewer
        //        Microsoft.Reporting.WinForms.ReportDataSource reportDataSource =
        //            new Microsoft.Reporting.WinForms.ReportDataSource();
        //        reportDataSource.Name = "DataSet1";  // Must match EXACTLY with RDLC Dataset name
        //        reportDataSource.Value = ds.Tables[0];

        //        // Fix 1: Correct file name (Charts not Chats)
        //        string reportPath = Path.Combine(Application.StartupPath, "Reports", "Chats_of_Accounts_report.rdlc");

        //        // Fix 2: Also check in parent folders
        //        if (!File.Exists(reportPath))
        //        {
        //            reportPath = Path.Combine(Application.StartupPath, "..", "..", "Reports", "Chats_of_Accounts_report.rdlc");
        //        }

        //        if (!File.Exists(reportPath))
        //        {
        //            reportPath = Path.Combine(Application.StartupPath, "..", "..", "..", "Reports", "Chats_of_Accounts_report.rdlc");
        //        }

        //        // DEBUG: Show file path
        //        MessageBox.Show($"Looking for report at: {reportPath}\nExists: {File.Exists(reportPath)}", "Debug Info");

        //        if (!File.Exists(reportPath))
        //        {
        //            MessageBox.Show($"Report file not found at: {reportPath}\nPlease check file exists.", "Error",
        //                MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        reportViewer1.LocalReport.ReportPath = reportPath;
        //        reportViewer1.LocalReport.DataSources.Clear();
        //        reportViewer1.LocalReport.DataSources.Add(reportDataSource);

        //        // Set report title based on selection
        //        string reportTitle = SelectedAccountType == "All Accounts"
        //            ? "All Accounts Report"
        //            : SelectedAccountType + " Accounts Report";

        //        // Optional: Only add parameter if it exists in RDLC
        //        try
        //        {
        //            Microsoft.Reporting.WinForms.ReportParameter titleParam =
        //                new Microsoft.Reporting.WinForms.ReportParameter("ReportTitle", reportTitle);
        //            reportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter[] { titleParam });
        //        }
        //        catch
        //        {
        //            // Parameter doesn't exist in RDLC, that's OK
        //        }

        //        // Refresh report
        //        reportViewer1.RefreshReport();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading report: {ex.Message}\n\nFull Error: {ex.ToString()}", "Error Details",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void LoadReport()
        {
            try
            {
                DataSet ds = GetChartOfAccountsData(SelectedAccountType);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("No data found.", "Information");
                    return;
                }

                // Simple path - adjust as needed
                string reportPath = @"Reports\Chats_of_Accounts_report.rdlc";

                if (!File.Exists(reportPath))
                    reportPath = @"..\..\Reports\Chats_of_Accounts_report.rdlc";

                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet1", ds.Tables[0]));
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
//       private void SetupReportLayout()
//{
//    try
//    {
//        // 1. Fix margins (A4 paper with 1 inch margins)
//        System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
//        pg.Margins = new System.Drawing.Printing.Margins(100, 100, 100, 100); // 1 inch = 100 units
//        pg.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1169);
        
//        // 2. Apply settings
//        reportViewer1.SetPageSettings(pg);
//        reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
//        reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.PageWidth;
//    }
//    catch (Exception ex)
//    {
//        // Silent fail - don't show error for margins
//    }
//}



        private DataSet GetChartOfAccountsData(string accountType)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query;

                    if (accountType == "All Accounts")
                    {
                        query = @"SELECT AccountNo, AccountName, AccountType, Narration, 
                                 OpeningDebit, OpeningCredit 
                                 FROM ChartOfAccounts 
                                 ORDER BY AccountNo";
                    }
                    else
                    {
                        query = @"SELECT AccountNo, AccountName, AccountType, Narration, 
                                 OpeningDebit, OpeningCredit 
                                 FROM ChartOfAccounts 
                                 WHERE AccountType = @AccountType 
                                 ORDER BY AccountNo";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (accountType != "All Accounts")
                    {
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds, "ChartOfAccounts");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data: " + ex.Message);
            }

            return ds;
        }
        private void Charts_of_Accounts_rv_Load(object sender, EventArgs e)
        {
            LoadReport();
        }
    }
}