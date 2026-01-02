//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Windows.Forms;
//using System.Configuration;
//using Microsoft.Reporting.WinForms;

//namespace FO_2022.Reports
//{
//    public partial class General_Journal_rv : Form
//    {
//        public DateTime FromDate { get; set; }
//        public DateTime ToDate { get; set; }
//        public int FromVoucherNo { get; set; }
//        public int ToVoucherNo { get; set; }

//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//        public General_Journal_rv()
//        {
//            InitializeComponent();
//        }

//        private void General_Journal_rv_Load(object sender, EventArgs e)
//        {
//            LoadReport();
//        }

//        private void LoadReport()
//        {
//            try
//            {
//                // TEST: Show what we're searching for
//                //MessageBox.Show($"Searching: {FromDate:dd-MMM-yyyy} to {ToDate:dd-MMM-yyyy}\n" +
//                              //$"Vouchers: {FromVoucherNo} to {ToVoucherNo}", "Debug");

//                DataTable dt = GetGeneralJournalData();

//                if (dt.Rows.Count == 0)
//                {
//                    MessageBox.Show("No data found.", "Info");
//                    return;
//                }

//                // TEST: Show sample data
//                string sample = $"Found {dt.Rows.Count} rows\nFirst row:\n";
//                if (dt.Rows.Count > 0)
//                {
//                    sample += $"Acc#: {dt.Rows[0]["AccountNo"]}\n";
//                    sample += $"Name: {dt.Rows[0]["AccountName"]}\n";
//                    sample += $"Narration: {dt.Rows[0]["Narration"]}\n";
//                    sample += $"Debit: {dt.Rows[0]["Debit"]}\n";
//                    sample += $"Credit: {dt.Rows[0]["Credit"]}";
//                }
//                //MessageBox.Show(sample, "Data Found");

//                string reportPath = FindReportFile("General_Journal.rdlc");

//                if (string.IsNullOrEmpty(reportPath))
//                {
//                    MessageBox.Show("Report file not found.", "Error");
//                    return;
//                }

//                reportViewer1.LocalReport.ReportPath = reportPath;
//                reportViewer1.LocalReport.DataSources.Clear();
//                reportViewer1.LocalReport.DataSources.Add(
//                    new ReportDataSource("DataSet1", dt));

//                reportViewer1.RefreshReport();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error: {ex.Message}", "Error");
//            }
//        }

//        private DataTable GetGeneralJournalData()
//        {
//            DataTable dt = new DataTable();

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    // SIMPLE TEST QUERY FIRST
//                    string query = @"
//                    SELECT TOP 50
//                        jvb.AccountNo,
//                        ISNULL(coa.AccountName, 'No Name') as AccountName,
//                        ISNULL(jvb.Narration, '') as Narration,
//                        ISNULL(jvb.Debit, 0) as Debit,
//                        ISNULL(jvb.Credit, 0) as Credit
//                    FROM JournalVouchersBody jvb
//                    LEFT JOIN ChartOfAccounts coa ON jvb.AccountNo = coa.AccountNo
//                    ORDER BY jvb.VoucherNo DESC";

//                    // If you want to add filters later:
//                    /* 
//                    string query = @"
//                    SELECT 
//                        jvb.AccountNo,
//                        ISNULL(coa.AccountName, 'No Name') as AccountName,
//                        ISNULL(jvb.Narration, '') as Narration,
//                        ISNULL(jvb.Debit, 0) as Debit,
//                        ISNULL(jvb.Credit, 0) as Credit
//                    FROM JournalVouchers jv
//                    INNER JOIN JournalVouchersBody jvb ON jv.VoucherNo = jvb.VoucherNo
//                    LEFT JOIN ChartOfAccounts coa ON jvb.AccountNo = coa.AccountNo
//                    WHERE (@FromDate IS NULL OR jv.VoucherDate >= @FromDate)
//                        AND (@ToDate IS NULL OR jv.VoucherDate <= @ToDate)
//                        AND (@FromVoucher = 0 OR jv.VoucherNo >= @FromVoucher)
//                        AND (@ToVoucher = 0 OR jv.VoucherNo <= @ToVoucher)
//                    ORDER BY jv.VoucherDate, jv.VoucherNo, jvb.SerialNo";
//                    */

//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    // Uncomment when using filtered query:
//                    // cmd.Parameters.AddWithValue("@FromDate", FromDate);
//                    // cmd.Parameters.AddWithValue("@ToDate", ToDate);
//                    // cmd.Parameters.AddWithValue("@FromVoucher", FromVoucherNo);
//                    // cmd.Parameters.AddWithValue("@ToVoucher", ToVoucherNo == int.MaxValue ? 0 : ToVoucherNo);

//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    da.Fill(dt);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("DB Error: " + ex.Message, "Error");
//            }

//            return dt;
//        }

//        private string FindReportFile(string fileName)
//        {
//            // Check in multiple locations
//            string startupPath = Application.StartupPath;

//            // Check in bin/Debug or bin/Release
//            string debugPath = Path.Combine(startupPath, fileName);
//            if (File.Exists(debugPath))
//            {
//                //MessageBox.Show($"Found at: {debugPath}", "File Found");
//                return debugPath;
//            }

//            // Check in Reports folder
//            string reportsPath = Path.Combine(startupPath, "Reports", fileName);
//            if (File.Exists(reportsPath))
//            {
//                //MessageBox.Show($"Found at: {reportsPath}", "File Found");
//                return reportsPath;
//            }

//            // Check parent folders
//            //string parentPath = Path.Combine(startupPath, "..", "..", "Reports", fileName);
//            //if (File.Exists(parentPath))
//            //{
//            //    //MessageBox.Show($"Found at: {parentPath}", "File Found");
//            //    return parentPath;
//            //}

//            MessageBox.Show($"NOT FOUND: {fileName}\n" +
//                          $"Searched:\n{debugPath}\n{reportsPath}",
//                          "File Error");
//            return null;
//        }
//    }
//}



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
//private DataTable GetData()
//{
//    DataTable dt = new DataTable();

//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        string query = @"SELECT 
//                        jvb.AccountNo,
//                        ISNULL(coa.AccountName, 'No Name') as AccountName,
//                        ISNULL(jvb.Narration, '') as Narration,
//                        ISNULL(jvb.Debit, 0) as Debit,
//                        ISNULL(jvb.Credit, 0) as Credit
//                       FROM JournalVouchers jv
//                       INNER JOIN JournalVouchersBody jvb ON jv.VoucherNo = jvb.VoucherNo
//                       LEFT JOIN ChartOfAccounts coa ON jvb.AccountNo = coa.AccountNo
//                       WHERE jv.VoucherDate BETWEEN @FromDate AND @ToDate
//                           AND (@FromVoucher = 0 OR jv.VoucherNo >= @FromVoucher)
//                           AND (@ToVoucher = 0 OR jv.VoucherNo <= @ToVoucher)
//                       ORDER BY jv.VoucherDate, jv.VoucherNo";

//        SqlCommand cmd = new SqlCommand(query, conn);
//        cmd.Parameters.AddWithValue("@FromDate", FromDate);
//        cmd.Parameters.AddWithValue("@ToDate", ToDate);
//        cmd.Parameters.AddWithValue("@FromVoucher", FromVoucherNo);
//        cmd.Parameters.AddWithValue("@ToVoucher", ToVoucherNo == int.MaxValue ? 0 : ToVoucherNo);

//        new SqlDataAdapter(cmd).Fill(dt);
//    }

//    return dt;
//}