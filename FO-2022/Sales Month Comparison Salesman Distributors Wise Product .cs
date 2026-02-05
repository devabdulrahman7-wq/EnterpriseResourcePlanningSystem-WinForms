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
    public partial class Sales_Month_Comparison_Salesman_Distributors_Wise_Product : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_Month_Comparison_Salesman_Distributors_Wise_Product()
        {
            InitializeComponent();
        }

        private void Sales_Month_Comparison_Salesman_Distributors_Wise_Product_Load(object sender, EventArgs e)
        {
            // Set default dates - Last month and current month
            currentmonthdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lastmonthdatepcker.Value = currentmonthdatepcker.Value.AddMonths(-1);

            // Load salesman combobox
            LoadSalesmanComboBox();
        }

        // 📌 LOAD SALESMAN COMBOBOX
        private void LoadSalesmanComboBox()
        {
            try
            {
                salesmancombobox.Items.Clear();
                salesmancombobox.Items.Add("All");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesmancombobox.Items.Add(reader["SalesmanName"].ToString());
                            }
                        }
                    }
                }
                salesmancombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Salesmen: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GET SALESMAN ID BY NAME
        private string GetSalesmanIdByName(string salesmanName)
        {
            if (salesmanName == "All") return "All";

            string salesmanId = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SalesmanID FROM Salesmen WHERE SalesmanName = @SalesmanName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SalesmanName", salesmanName);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        salesmanId = result.ToString();
                }
            }

            return salesmanId;
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

            // Get selected salesman
            string selectedSalesman = salesmancombobox.SelectedItem?.ToString() ?? "All";
            string salesmanName = selectedSalesman;

            // Calculate date ranges
            DateTime lastMonthStart = lastmonthdatepcker.Value;
            DateTime lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            DateTime currentMonthStart = currentmonthdatepcker.Value;
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            // Get data
            DataTable dt = GetSalesComparisonData(lastMonthStart, lastMonthEnd,
                                                currentMonthStart, currentMonthEnd,
                                                selectedSalesman);

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
                //    { "LastMonthPeriod", $"{lastMonthStart.ToString("MMM yyyy")}" },
                //    { "CurrentMonthPeriod", $"{currentMonthStart.ToString("MMM yyyy")}" },
                //    { "LastMonthStart", lastMonthStart.ToString("dd-MMM-yyyy") },
                //    { "LastMonthEnd", lastMonthEnd.ToString("dd-MMM-yyyy") },
                //    { "CurrentMonthStart", currentMonthStart.ToString("dd-MMM-yyyy") },
                //    { "CurrentMonthEnd", currentMonthEnd.ToString("dd-MMM-yyyy") },
                //    { "Salesman", salesmanName },
                //    { "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Sales_Month_Comparison_Salesman_Distributors_Wise_Product", dataSources, parameters, null);
        }

        // 📌 GET SALES COMPARISON DATA
        private DataTable GetSalesComparisonData(DateTime lastMonthStart, DateTime lastMonthEnd,
                                                DateTime currentMonthStart, DateTime currentMonthEnd,
                                                string salesmanName)
        {
            DataTable dt = new DataTable();

            // Get salesman ID if not "All"
            string salesmanId = "All";
            if (salesmanName != "All")
            {
                salesmanId = GetSalesmanIdByName(salesmanName);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        -- Product Information
                        PR.ProductID,
                        PR.ProductName,
                        C.CompanyName,
                        PG.GroupName,
                        SM.SalesmanName,
                        
                        -- Last Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) AS LastMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) AS LastMonthSalesQty,
                        
                        -- Current Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) AS CurrentMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) AS CurrentMonthSalesQty,
                        
                        -- Comparison Calculations
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) 
                        -
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                            AND P.IsDistributor = 1
                            AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                        ), 0) AS ValueDifference,
                        
                        CASE 
                            WHEN ISNULL((
                                SELECT SUM(SB.TTLValue) 
                                FROM SalesBody SB
                                INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                                WHERE SB.ProductId = PR.ProductID
                                AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                AND SB.IsDeleted = 0
                                AND S.IsPosted = 1
                                AND P.IsDistributor = 1
                                AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                            ), 0) = 0 THEN 0
                            ELSE 
                                (
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                        AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                                    ), 0) 
                                    -
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                        AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                                    ), 0)
                                ) * 100.0 / NULLIF(
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                                        LEFT JOIN Salesmen SM2 ON S.SalesmanId = SM2.SalesmanID
                                        WHERE SB.ProductId = PR.ProductID
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                        AND P.IsDistributor = 1
                                        AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                                    ), 0), 0)
                        END AS PercentageChange
                        
                    FROM Products PR
                    INNER JOIN Companies C ON PR.CompanyID = C.CompanyID
                    LEFT JOIN ProductGroups PG ON PR.CompanyID = PG.CompanyId AND PR.GroupId = PG.GroupId
                    CROSS JOIN Salesmen SM
                    WHERE SM.SalesmanName = @SalesmanName
                    AND EXISTS (
                        SELECT 1 FROM SalesBody SB
                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                        INNER JOIN Parties P ON S.CustomerId = P.PartyID
                        WHERE SB.ProductId = PR.ProductID
                        AND S.SaleDate BETWEEN @LastMonthStart AND @CurrentMonthEnd
                        AND SB.IsDeleted = 0
                        AND S.IsPosted = 1
                        AND P.IsDistributor = 1
                        AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                    )
                    ORDER BY C.CompanyName, PR.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LastMonthStart", lastMonthStart);
                    cmd.Parameters.AddWithValue("@LastMonthEnd", lastMonthEnd);
                    cmd.Parameters.AddWithValue("@CurrentMonthStart", currentMonthStart);
                    cmd.Parameters.AddWithValue("@CurrentMonthEnd", currentMonthEnd);
                    cmd.Parameters.AddWithValue("@SalesmanName", salesmanName);
                    cmd.Parameters.AddWithValue("@SalesmanId", salesmanId);

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
            // Ensure it's first day of month
            lastmonthdatepcker.Value = new DateTime(
                lastmonthdatepcker.Value.Year,
                lastmonthdatepcker.Value.Month,
                1);
        }

        private void currentmonthdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Ensure it's first day of month
            currentmonthdatepcker.Value = new DateTime(
                currentmonthdatepcker.Value.Year,
                currentmonthdatepcker.Value.Month,
                1);
        }

        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }
    }
}