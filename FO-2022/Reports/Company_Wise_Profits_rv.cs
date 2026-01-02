using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class Customer_Wise_Profits_rv : Form
    {
        // ADD THESE PROPERTIES:
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Customer_Wise_Profits_rv()
        {
            InitializeComponent();
        }

        // Or use a method to set parameters:
        public void SetParameters(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        private void Customer_Wise_Profits_rv_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                // Get data
                DataTable dt = GetCustomerProfitData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for selected period.", "Information");
                    return;
                }

                // Find report file
                string reportPath = FindReportFile("Customer_Wise_Profits_report.rdlc");

                if (string.IsNullOrEmpty(reportPath))
                {
                    MessageBox.Show("Report file not found.", "Error");
                    return;
                }

                // Setup report
                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet1", dt));

                // Add parameters
                ReportParameter[] parameters =
                {
                    new ReportParameter("FromDate", FromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("ToDate", ToDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("ReportDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"))
                };
                reportViewer1.LocalReport.SetParameters(parameters);

                // Setup layout
                SetupReportLayout();

                // Refresh
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
        }

        private DataTable GetCustomerProfitData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT 
                        p.PartyID AS CustomerID,
                        p.PartyName AS CustomerName,
                        ISNULL(SUM(sb.TTLValue), 0) AS TotalValue,
                        ISNULL(SUM(sb.TTLSTax), 0) AS SalesTax,
                        ISNULL(SUM(sb.TTLFTax), 0) AS FurtherTax,
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) AS GrossSale,
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.7 AS PurchaseCost,
                        ISNULL(SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax), 0) * 0.3 AS NetProfit,
                        30.0 AS ProfitPercentage
                    FROM Parties p
                    INNER JOIN Sales s ON p.PartyID = s.CustomerId
                    INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                        AND s.IsPosted = 1
                        AND sb.IsDeleted = 0
                    GROUP BY p.PartyID, p.PartyName
                    HAVING ISNULL(SUM(sb.TTLValue), 0) > 0
                    ORDER BY p.PartyName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error");
                return null;
            }

            return dt;
        }

        private string FindReportFile(string fileName)
        {
            string[] possiblePaths =
            {
                Path.Combine(Application.StartupPath, fileName),
                Path.Combine(Application.StartupPath, "Reports", fileName),
                Path.Combine(Application.StartupPath, "..", "..", "Reports", fileName),
                Path.Combine(Application.StartupPath, "..", "..", fileName)
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        private void SetupReportLayout()
        {
            try
            {
                // Fix margins and layout
                System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
                pg.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
                pg.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1169);

                reportViewer1.SetPageSettings(pg);
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.PageWidth;
            }
            catch
            {
                // Silent fail
            }
        }
    }
}