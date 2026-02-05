using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sales_Month_Comparison_Distributors_Wise_Products : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_Month_Comparison_Distributors_Wise_Products()
        {
            InitializeComponent();
        }

        private void Sales_Month_Comparison_Distributors_Wise_Products_Load(object sender, EventArgs e)
        {
            // Set default dates - Last month and current month
            currentmonthdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lastmonthdatepcker.Value = currentmonthdatepcker.Value.AddMonths(-1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 PRINT BUTTON
        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GENERATE REPORT
        private void GenerateReport(bool autoPrint)
        {
            // Validate dates
            if (lastmonthdatepcker.Value > currentmonthdatepcker.Value)
            {
                MessageBox.Show("Last month date cannot be greater than Current month date",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate date ranges
            DateTime lastMonthStart = lastmonthdatepcker.Value;
            DateTime lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            DateTime currentMonthStart = currentmonthdatepcker.Value;
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            // Get data
            DataTable dt = GetSalesComparisonData(lastMonthStart, lastMonthEnd, currentMonthStart, currentMonthEnd);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the selected criteria", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prepare data sources
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
            {
                { "DataSet1", dt }
            };

            // Prepare parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                //{ "LastMonthPeriod", $"{lastMonthStart.ToString("MMM yyyy")}" },
                //{ "CurrentMonthPeriod", $"{currentMonthStart.ToString("MMM yyyy")}" },
                //{ "LastMonthStart", lastMonthStart.ToString("dd-MMM-yyyy") },
                //{ "LastMonthEnd", lastMonthEnd.ToString("dd-MMM-yyyy") },
                //{ "CurrentMonthStart", currentMonthStart.ToString("dd-MMM-yyyy") },
                //{ "CurrentMonthEnd", currentMonthEnd.ToString("dd-MMM-yyyy") },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            //viewer.ShowReport("Sales_Month_Comparison_Distributors_Wise_Products", dataSources, parameters, null);
            viewer.ShowReport("Sales_Month_Comparison_Distributors_Wise_Products", dataSources, null);

        }

        // 📌 GET SALES COMPARISON DATA
        private DataTable GetSalesComparisonData(DateTime lastMonthStart, DateTime lastMonthEnd,
                                                DateTime currentMonthStart, DateTime currentMonthEnd)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        -- Product Information
                        PR.ProductID,
                        PR.ProductName,
                        C.CompanyName,
                        PG.GroupName,
                        
                        -- Last Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) AS LastMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) AS LastMonthSalesQty,
                        
                        -- Current Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) AS CurrentMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) AS CurrentMonthSalesQty,
                        
                        -- Comparison Calculations
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) 
                        -
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                        ), 0) AS ValueDifference,
                        
                        CASE 
                            WHEN ISNULL((
                                SELECT SUM(SB.TTLValue) 
                                FROM SalesBody SB
                                INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                WHERE SB.ProductId = PR.ProductID
                                AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                AND SB.IsDeleted = 0
                                AND S.IsPosted = 1
                                AND P.IsDistributor = 1
                            ), 0) = 0 THEN 0
                            ELSE 
                                (
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                    ), 0) 
                                    -
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                    ), 0)
                                ) * 100.0 / NULLIF(
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                    ), 0), 0)
                        END AS PercentageChange
                        
                    FROM Products PR
                    INNER JOIN Companies C ON PR.CompanyID = C.CompanyID
                    LEFT JOIN ProductGroups PG ON PR.CompanyID = PG.CompanyId AND PR.GroupId = PG.GroupId
                    WHERE EXISTS (
                        SELECT 1 FROM SalesBody SB
                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                        WHERE SB.ProductId = PR.ProductID
                        AND S.SaleDate BETWEEN @LastMonthStart AND @CurrentMonthEnd
                        AND SB.IsDeleted = 0
                        AND S.IsPosted = 1
                        AND P.IsDistributor = 1
                    )
                    ORDER BY C.CompanyName, PR.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LastMonthStart", lastMonthStart);
                    cmd.Parameters.AddWithValue("@LastMonthEnd", lastMonthEnd);
                    cmd.Parameters.AddWithValue("@CurrentMonthStart", currentMonthStart);
                    cmd.Parameters.AddWithValue("@CurrentMonthEnd", currentMonthEnd);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // 📌 DATE CHANGED EVENTS
        private void lastmonthdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Ensure it's first day of month
            lastmonthdatepcker.Value = new DateTime(
                lastmonthdatepcker.Value.Year,
                lastmonthdatepcker.Value.Month,1);
        }

        private void currentmonthdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Ensure it's first day of month
            currentmonthdatepcker.Value = new DateTime(
                currentmonthdatepcker.Value.Year,
                currentmonthdatepcker.Value.Month,1);
        }
    }
}