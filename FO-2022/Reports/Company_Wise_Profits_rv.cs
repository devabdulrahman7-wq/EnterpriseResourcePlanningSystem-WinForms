//using Microsoft.Reporting.WinForms;
//using System;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Windows.Forms;

//namespace FO_2022.Reports
//{
//    public partial class Customer_Wise_Profits_rv : Form
//    {
//        private DateTime _fromDate;
//        private DateTime _toDate;

//        public Customer_Wise_Profits_rv()
//        {
//            InitializeComponent();
//        }

//        // Set date parameters
//        public void SetParameters(DateTime fromDate, DateTime toDate)
//        {
//            _fromDate = fromDate;
//            _toDate = toDate;
//        }



//        private void LoadReportData()
//        {
//            try
//            {
//                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

//                if (string.IsNullOrEmpty(connectionString))
//                {
//                    MessageBox.Show("Connection string not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    return;
//                }

//                using (SqlConnection connection = new SqlConnection(connectionString))
//                {
//                    connection.Open();

//                    // **Customer-wise profit query**
//                    string query = @"
//                    SELECT 
//                        p.PartyID AS [CustomerID],
//                        p.PartyName AS [CustomerName],
//                        ISNULL(SUM(sb.TTLValue), 0) AS [TotalValue],
//                        ISNULL(SUM(sb.TTLSTax), 0) AS [SalesTax],
//                        ISNULL(SUM(sb.TTLFTax), 0) AS [FurtherTax],
//                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) AS [GrossSale],
//                        -- Calculate purchase cost (simplified: 70% of gross sale)
//                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.7 AS [PurchaseCost],
//                        -- Calculate net profit (simplified: 30% of gross sale)
//                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.3 AS [NetProfit],
//                        -- Profit percentage
//                        30.0 AS [ProfitPercentage]
//                    FROM Parties p
//                    INNER JOIN Sales s ON p.PartyID = s.CustomerId
//                    INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
//                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
//                        AND s.IsPosted = 1
//                        AND sb.IsDeleted = 0
//                    GROUP BY p.PartyID, p.PartyName
//                    HAVING ISNULL(SUM(sb.TTLValue), 0) > 0
//                    ORDER BY p.PartyName";

//                    using (SqlCommand command = new SqlCommand(query, connection))
//                    {
//                        // Validate dates for SQL Server
//                        DateTime safeFromDate = GetSafeSqlDate(_fromDate);
//                        DateTime safeToDate = GetSafeSqlDate(_toDate);

//                        command.Parameters.AddWithValue("@FromDate", safeFromDate);
//                        command.Parameters.AddWithValue("@ToDate", safeToDate);

//                        SqlDataAdapter adapter = new SqlDataAdapter(command);
//                        DataTable dataTable = new DataTable();
//                        adapter.Fill(dataTable);

//                        if (dataTable.Rows.Count == 0)
//                        {
//                            MessageBox.Show("No data found", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            return;
//                        }

//                        // Bind to report
//                        BindToReport(dataTable);
//                    }
//                }
//            }
//            catch (SqlException sqlEx)
//            {
//                MessageBox.Show("SQL Error: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Fix: DateTime cannot be const, use static readonly instead
//        private static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);
//        private static readonly DateTime MaxSqlDate = new DateTime(9999, 12, 31);

//        private DateTime GetSafeSqlDate(DateTime date)
//        {
//            if (date < MinSqlDate) return MinSqlDate;
//            if (date > MaxSqlDate) return MaxSqlDate;
//            return date;
//        }

//        private void BindToReport(DataTable dataTable)
//        {
//            try
//            {
//                // Clear and add data source
//                reportViewer1.LocalReport.DataSources.Clear();
//                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataTable));

//                // Set parameters
//                ReportParameter[] parameters = {
//                    new ReportParameter("FromDate", _fromDate.ToString("dd-MMM-yyyy")),
//                    new ReportParameter("ToDate", _toDate.ToString("dd-MMM-yyyy")),
//                    new ReportParameter("PrintedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"))
//                };

//                reportViewer1.LocalReport.SetParameters(parameters);
//                reportViewer1.RefreshReport();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Report Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Public method to print the report directly
//        public void PrintReport()
//        {
//            try
//            {
//                LoadReportData();
//                if (reportViewer1.LocalReport.DataSources.Count > 0)
//                {
//                    reportViewer1.PrintDialog();  // Opens print dialog for user to choose printer and settings
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Print Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void Customer_Wise_Profits_rv_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            try
//            {
//                reportViewer1.LocalReport.DataSources.Clear();
//                reportViewer1.LocalReport.ReleaseSandboxAppDomain();
//            }
//            catch { }
//        }

//        private void Customer_Wise_Profits_rv_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                // **Set RDLC file path**
//                // Ensure the RDLC file is embedded as a resource:
//                // 1. Right-click the RDLC file → Properties → Build Action = Embedded Resource
//                // 2. Namespace: FO_2022.Reports (adjust according to your project structure)
//                //this.reportViewer1.LocalReport.ReportPath = "Reports\\Company_Wise_Profits_report.rdlc";
//                string reportPath = Path.Combine(Application.StartupPath, "..", "..", "Reports", "Sector.rdlc");

//                //this.reportViewer1.LocalReport.ReportPath = "Reports\\Company_Wise_Profits_rv.rdlc";


//                // Load data
//                LoadReportData();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }
//    }
//}
using Microsoft.Reporting.WinForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Customer_Wise_Profits_rv : Form
    {
        private DateTime _fromDate;
        private DateTime _toDate;

        public Customer_Wise_Profits_rv()
        {
            InitializeComponent();
        }

        // Set date parameters
        public void SetParameters(DateTime fromDate, DateTime toDate)
        {
            _fromDate = fromDate;
            _toDate = toDate;
        }

        private void LoadReportData()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Connection string not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // **Customer-wise profit query**
                    string query = @"
                    SELECT 
                        p.PartyID AS [CustomerID],
                        p.PartyName AS [CustomerName],
                        ISNULL(SUM(sb.TTLValue), 0) AS [TotalValue],
                        ISNULL(SUM(sb.TTLSTax), 0) AS [SalesTax],
                        ISNULL(SUM(sb.TTLFTax), 0) AS [FurtherTax],
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) AS [GrossSale],
                        -- Calculate purchase cost (simplified: 70% of gross sale)
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.7 AS [PurchaseCost],
                        -- Calculate net profit (simplified: 30% of gross sale)
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.3 AS [NetProfit],
                        -- Profit percentage
                        30.0 AS [ProfitPercentage]
                    FROM Parties p
                    INNER JOIN Sales s ON p.PartyID = s.CustomerId
                    INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                        AND s.IsPosted = 1
                        AND sb.IsDeleted = 0
                    GROUP BY p.PartyID, p.PartyName
                    HAVING ISNULL(SUM(sb.TTLValue), 0) > 0
                    ORDER BY p.PartyName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Validate dates for SQL Server
                        DateTime safeFromDate = GetSafeSqlDate(_fromDate);
                        DateTime safeToDate = GetSafeSqlDate(_toDate);

                        command.Parameters.AddWithValue("@FromDate", safeFromDate);
                        command.Parameters.AddWithValue("@ToDate", safeToDate);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            MessageBox.Show("No data found", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Bind to report
                        BindToReport(dataTable);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);
        private static readonly DateTime MaxSqlDate = new DateTime(9999, 12, 31);

        private DateTime GetSafeSqlDate(DateTime date)
        {
            if (date < MinSqlDate) return MinSqlDate;
            if (date > MaxSqlDate) return MaxSqlDate;
            return date;
        }

        private void BindToReport(DataTable dataTable)
        {
            try
            {
                // Clear and add data source
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataTable));

                // Set parameters
                ReportParameter[] parameters = {
                    new ReportParameter("FromDate", _fromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("ToDate", _toDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("PrintedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"))
                };

                reportViewer1.LocalReport.SetParameters(parameters);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Report Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Public method to print the report directly
        public void PrintReport()
        {
            try
            {
                LoadReportData();
                if (reportViewer1.LocalReport.DataSources.Count > 0)
                {
                    reportViewer1.PrintDialog();  // Opens print dialog for user to choose printer and settings
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Customer_Wise_Profits_rv_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            }
            catch { }
        }

        private void Customer_Wise_Profits_rv_Load(object sender, EventArgs e)
        {
            try
            {
                // **FIXED: Set RDLC file path properly**
                string reportPath = FindReportFile("Customer_Wise_Profits_report.rdlc");

                if (!string.IsNullOrEmpty(reportPath) && File.Exists(reportPath))
                {
                    reportViewer1.LocalReport.ReportPath = reportPath;
                }
                else
                {
                    MessageBox.Show("Report file not found. Please make sure the RDLC file exists.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set processing mode
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                // Load data
                LoadReportData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FindReportFile(string reportFileName)
        {
            // Try multiple locations where the RDLC file might be

            // 1. Check in Application Startup Path
            string startupPath = Application.StartupPath;
            string[] searchPaths = {
                Path.Combine(startupPath, "Reports", reportFileName),
                Path.Combine(startupPath, "..", "Reports", reportFileName),
                Path.Combine(startupPath, "..", "..", "Reports", reportFileName),
                Path.Combine(startupPath, "..", "..", "..", "Reports", reportFileName),
                Path.Combine(startupPath, reportFileName)
            };

            foreach (string path in searchPaths)
            {
                if (File.Exists(path))
                    return path;
            }

            // 2. Check in Resources (if embedded)
            try
            {
                // If RDLC is embedded as resource, you can load it this way
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"FO_2022.Reports.{reportFileName}";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        // Save to temp file
                        string tempPath = Path.GetTempFileName() + ".rdlc";
                        using (FileStream fileStream = File.Create(tempPath))
                        {
                            stream.CopyTo(fileStream);
                        }
                        return tempPath;
                    }
                }
            }
            catch
            {
                // Ignore if not found in resources
            }

            return null;
        }
    }
}