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
    public partial class Company_Wise_Sales : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Company_Wise_Sales()
        {
            InitializeComponent();
        }

        private void Company_Wise_Sales_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Now.AddDays(-30);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 📌 SEARCH CUSTOMER BY ID
        private void customeridsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
            {
                MessageBox.Show("Please enter Customer ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerName = GetCustomerName(cstmridtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(customerName))
            {
                cstmrnametxtbox.Text = customerName;
            }
            else
            {
                MessageBox.Show("Customer not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cstmrnametxtbox.Clear();
            }
        }

        private string GetCustomerName(string customerId)
        {
            string customerName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PartyID", customerId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        customerName = result.ToString();
                }
            }

            return customerName;
        }

        // 📌 SEARCH SECTOR BY ID
        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Sector ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sectorName = GetSectorName(sectoridtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(sectorName))
            {
                sectornametxtbox.Text = sectorName;
            }
            else
            {
                MessageBox.Show("Sector not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                sectornametxtbox.Clear();
            }
        }

        private string GetSectorName(string sectorId)
        {
            string sectorName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorName FROM Sectors WHERE SectorID = @SectorID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SectorID", sectorId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        sectorName = result.ToString();
                }
            }

            return sectorName;
        }

        // 📌 SEARCH TOWN BY ID
        private void townidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(townidtxtbox.Text))
            {
                MessageBox.Show("Please enter Town ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string townName = GetTownName(townidtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(townName))
            {
                townnametxtbox.Text = townName;
            }
            else
            {
                MessageBox.Show("Town not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                townnametxtbox.Clear();
            }
        }

        private string GetTownName(string townId)
        {
            string townName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TownName FROM Towns WHERE TownID = @TownID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TownID", townId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        townName = result.ToString();
                }
            }

            return townName;
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

            // Get filter values
            string customerId = string.IsNullOrWhiteSpace(cstmridtxtbox.Text) ? null : cstmridtxtbox.Text.Trim();
            string townId = string.IsNullOrWhiteSpace(townidtxtbox.Text) ? null : townidtxtbox.Text.Trim();
            string sectorId = string.IsNullOrWhiteSpace(sectoridtxtbox.Text) ? null : sectoridtxtbox.Text.Trim();

            // Get data
            DataTable dt = GetCompanyWiseSalesData(
                fromdatepcker.Value,
                todatepcker.Value,
                customerId,
                townId,
                sectorId);

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
            //    { "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
            //    { "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
            //    { "Customer", string.IsNullOrEmpty(cstmrnametxtbox.Text) ? "All" : cstmrnametxtbox.Text },
            //    { "Town", string.IsNullOrEmpty(townnametxtbox.Text) ? "All" : townnametxtbox.Text },
                //{ "Sector", string.IsNullOrEmpty(sectornametxtbox.Text) ? "All" : sectornametxtbox.Text },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Company_Wise_Sales", dataSources, parameters, null);
        }

        // 📌 GET COMPANY WISE SALES DATA
        private DataTable GetCompanyWiseSalesData(DateTime fromDate, DateTime toDate,
                                                string customerId, string townId, string sectorId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        C.CompanyID,
                        C.CompanyName,
                        COUNT(DISTINCT S.SaleId) AS TotalInvoices,
                        COUNT(DISTINCT S.CustomerId) AS TotalCustomers,
                        SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) AS TotalQuantity,
                        SUM(SB.TTLValue) AS TotalSalesValue,
                        SUM(SB.TTLSTax) AS TotalSalesTax,
                        SUM(SB.TTLFTax) AS TotalFTax,
                        SUM(SB.TTLValue + SB.TTLSTax + SB.TTLFTax) AS NetSalesAmount
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                    INNER JOIN Companies C ON PR.CompanyID = C.CompanyID
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SEC ON P.SectorID = SEC.SectorID
                    LEFT JOIN Towns T ON SEC.TownID = T.TownID
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND (@CustomerId IS NULL OR S.CustomerId = @CustomerId)
                    AND (@TownId IS NULL OR SEC.TownID = @TownId)
                    AND (@SectorId IS NULL OR P.SectorID = @SectorId)
                    GROUP BY C.CompanyID, C.CompanyName
                    ORDER BY NetSalesAmount DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    cmd.Parameters.AddWithValue("@CustomerId",
                        string.IsNullOrEmpty(customerId) ? (object)DBNull.Value : customerId);
                    cmd.Parameters.AddWithValue("@TownId",
                        string.IsNullOrEmpty(townId) ? (object)DBNull.Value : townId);
                    cmd.Parameters.AddWithValue("@SectorId",
                        string.IsNullOrEmpty(sectorId) ? (object)DBNull.Value : sectorId);

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
        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
        {
            cstmrnametxtbox.Clear();
        }

        private void sectoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            sectornametxtbox.Clear();
        }

        private void townidtxtbox_TextChanged(object sender, EventArgs e)
        {
            townnametxtbox.Clear();
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