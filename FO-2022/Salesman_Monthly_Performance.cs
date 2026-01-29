using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Salesman_Monthly_Performance : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Salesman_Monthly_Performance()
        {
            InitializeComponent();

            // Set default dates to current month
            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            fromdatepcker.Value = firstDayOfMonth;
            todatepcker.Value = lastDayOfMonth;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dt = GetSalesmanPerformanceData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No performance data found for selected period",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ReportDate", DateTime.Today.ToString("dd-MMM-yyyy HH:mm") }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Salesman_Monthly_Performance", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSalesmanPerformanceData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                WITH SalesData AS (
                    SELECT 
                        s.SalesmanId,
                        FORMAT(s.SaleDate, 'yyyy-MM') as MonthYear,
                        COUNT(DISTINCT s.SaleId) as TotalInvoices,
                        SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax) as TotalSalesAmount
                    FROM Sales s
                    INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                    WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND s.SalesmanId IS NOT NULL
                    GROUP BY s.SalesmanId, FORMAT(s.SaleDate, 'yyyy-MM')
                ),
                RecoveryData AS (
                    SELECT 
                        r.SalesmanID,
                        FORMAT(r.RecoveryDate, 'yyyy-MM') as MonthYear,
                        SUM(rb.Amount) as TotalRecovery
                    FROM Recovery r
                    INNER JOIN RecoveryBody rb ON r.RecoveryID = rb.RecoveryID
                    WHERE r.RecoveryDate BETWEEN @FromDate AND @ToDate
                    AND r.SalesmanID IS NOT NULL
                    GROUP BY r.SalesmanID, FORMAT(r.RecoveryDate, 'yyyy-MM')
                )
                SELECT 
                    sm.SalesmanID,
                    sm.SalesmanName,
                    COALESCE(sd.MonthYear, rd.MonthYear) as MonthYear,
                    ISNULL(sd.TotalInvoices, 0) as TotalInvoices,
                    ISNULL(sd.TotalSalesAmount, 0) as TotalSalesAmount,
                    ISNULL(rd.TotalRecovery, 0) as TotalRecovery,
                    CASE 
                        WHEN ISNULL(sd.TotalSalesAmount, 0) > 0 
                        THEN (ISNULL(rd.TotalRecovery, 0) * 100.0) / sd.TotalSalesAmount
                        ELSE 0 
                    END as RecoveryPercentage,
                    CASE 
                        WHEN ISNULL(sd.TotalInvoices, 0) > 0 
                        THEN ISNULL(sd.TotalSalesAmount, 0) / sd.TotalInvoices
                        ELSE 0 
                    END as AverageSale
                FROM Salesmen sm
                LEFT JOIN SalesData sd ON sm.SalesmanID = sd.SalesmanId
                LEFT JOIN RecoveryData rd ON sm.SalesmanID = rd.SalesmanID 
                    AND sd.MonthYear = rd.MonthYear
                WHERE (sd.TotalSalesAmount > 0 OR rd.TotalRecovery > 0)
                ORDER BY sm.SalesmanName, COALESCE(sd.MonthYear, rd.MonthYear)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));
                    cmd.CommandTimeout = 60;
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Salesman_Monthly_Performance_Load(object sender, EventArgs e)
        {
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }
    }
}