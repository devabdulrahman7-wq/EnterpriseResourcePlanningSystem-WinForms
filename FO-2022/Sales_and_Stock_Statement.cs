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
    public partial class Sales_and_Stock_Statement : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_and_Stock_Statement()
        {
            InitializeComponent();
        }

        private void Sales_and_Stock_Statement_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Now.AddDays(-30);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 📌 SEARCH COMPANY BY ID
        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmpnyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Company ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string companyName = GetCompanyName(cmpnyidtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(companyName))
            {
                cmpnynametxtbox.Text = companyName;
            }
            else
            {
                MessageBox.Show("Company not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmpnynametxtbox.Clear();
            }
        }

        private string GetCompanyName(string companyId)
        {
            string companyName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        companyName = result.ToString();
                }
            }

            return companyName;
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
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get company ID
            string companyId = string.IsNullOrWhiteSpace(cmpnyidtxtbox.Text) ? null : cmpnyidtxtbox.Text.Trim();
            string companyName = string.IsNullOrWhiteSpace(cmpnynametxtbox.Text) ? "All" : cmpnynametxtbox.Text;

            // Get data
            DataTable dt = GetSalesAndStockData(
                fromdatepcker.Value,
                todatepcker.Value,
                companyId);

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
                //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "CompanyName", companyName },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Sales_Stock", dataSources, null);
        }

        // 📌 GET SALES AND STOCK DATA
        private DataTable GetSalesAndStockData(DateTime fromDate, DateTime toDate, string companyId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    -- Get Product Master Data
                    SELECT 
                        PR.ProductID,
                        PR.ProductName,
                        C.CompanyName,
                        PG.GroupName,
                        PR.PurchasePrice,
                        PR.SalePrice,
                        -- Opening Stock (before fromDate)
                        ISNULL((
                            SELECT SUM(PB.QtyPack * PB.Multiplier + PB.QtyLose) 
                            FROM PurchasesBody PB
                            INNER JOIN Purchase P ON PB.PurchaseId = P.PurchaseID
                            WHERE PB.ProductId = PR.ProductID
                            AND P.PurchaseDate < @FromDate
                            AND PB.IsDeleted = 0
                        ), 0) 
                        -
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate < @FromDate
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS OpeningStock,
                        -- Purchases during period
                        ISNULL((
                            SELECT SUM(PB.QtyPack * PB.Multiplier + PB.QtyLose) 
                            FROM PurchasesBody PB
                            INNER JOIN Purchase P ON PB.PurchaseId = P.PurchaseID
                            WHERE PB.ProductId = PR.ProductID
                            AND P.PurchaseDate BETWEEN @FromDate AND @ToDate
                            AND PB.IsDeleted = 0
                        ), 0) AS PurchasesQty,
                        -- Sales during period
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            WHERE SB.ProductId = PR.ProductID
                            AND S.SaleDate BETWEEN @FromDate AND @ToDate
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS SalesQty,
                        -- Current Stock
                        ISNULL(CS.QtyLose, 0) AS CurrentStock
                    FROM Products PR
                    INNER JOIN Companies C ON PR.CompanyID = C.CompanyID
                    LEFT JOIN ProductGroups PG ON PR.CompanyID = PG.CompanyId AND PR.GroupId = PG.GroupId
                    LEFT JOIN CurrentStock CS ON PR.ProductID = CS.ProductiD
                    WHERE (@CompanyID IS NULL OR PR.CompanyID = @CompanyID)
                    ORDER BY C.CompanyName, PR.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    if (string.IsNullOrEmpty(companyId))
                        cmd.Parameters.AddWithValue("@CompanyID", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@CompanyID", companyId);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // 📌 TEXT CHANGED EVENTS
        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            cmpnynametxtbox.Clear();
        }

        // 📌 DATE CHANGED EVENTS
        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }
    }
}