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
    public partial class Customer_Invoice_Wise_Sales : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Customer_Invoice_Wise_Sales()
        {
            InitializeComponent();
        }

        private void Customer_Invoice_Wise_Sales_Load(object sender, EventArgs e)
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
            SearchEntity("Parties",
                         "PartyID",
                         "PartyName",
                         cstmridtxtbox,
                         cstmrnametxtbox,
                         "AND PartyType = 'C'");
        }
        private void companyidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Companies",
                         "CompanyID",
                         "CompanyName",
                         cmpnyidtxtbox,
                         cmpnynametxtbox);
        }
        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Sectors",
                         "SectorID",
                         "SectorName",
                         sectoridtxtbox,
                         sectornametxtbox);
        }
        private void townidsearch_Click(object sender, EventArgs e)
        {
            SearchEntity("Towns",
                         "TownID",
                         "TownName",
                         townidtxtbox,
                         townnametxtbox);
        }
        private void SearchEntity(string tableName,
                                  string idColumn,
                                  string nameColumn,
                                  TextBox idTextBox,
                                  TextBox nameTextBox,
                                  string extraCondition = "")
        {
            string idValue = idTextBox.Text.Trim();
            string nameValue = nameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(idValue) && string.IsNullOrEmpty(nameValue))
            {
                MessageBox.Show("Please enter ID or Name",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idTextBox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(idValue))
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {idColumn} = @Value {extraCondition}";
                    else
                        query = $"SELECT {idColumn}, {nameColumn} FROM {tableName} WHERE {nameColumn} = @Value {extraCondition}";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(idValue) ? idValue : nameValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idTextBox.Text = reader[idColumn].ToString();
                                nameTextBox.Text = reader[nameColumn].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Record not found",
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                idTextBox.Clear();
                                nameTextBox.Clear();
                                idTextBox.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            string companyId = string.IsNullOrWhiteSpace(cmpnyidtxtbox.Text) ? null : cmpnyidtxtbox.Text.Trim();
            string townId = string.IsNullOrWhiteSpace(townidtxtbox.Text) ? null : townidtxtbox.Text.Trim();
            string sectorId = string.IsNullOrWhiteSpace(sectoridtxtbox.Text) ? null : sectoridtxtbox.Text.Trim();

            // Get data
            DataTable dt = GetCustomerInvoiceWiseSalesData(
                fromdatepcker.Value,
                todatepcker.Value,
                customerId,
                companyId,
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
                //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "Customer", string.IsNullOrEmpty(cstmrnametxtbox.Text) ? "All" : cstmrnametxtbox.Text },
                //{ "Company", string.IsNullOrEmpty(cmpnynametxtbox.Text) ? "All" : cmpnynametxtbox.Text },
                //{ "Town", string.IsNullOrEmpty(townnametxtbox.Text) ? "All" : townnametxtbox.Text },
                //{ "Sector", string.IsNullOrEmpty(sectornametxtbox.Text) ? "All" : sectornametxtbox.Text },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Customer_Invoice_Wise_Sales", dataSources, parameters,null);
            //viewer.ShowReport("Customer_Invoice_Wise_Sales", dataSources, parameters, null);

        }

        // 📌 GET CUSTOMER INVOICE WISE SALES DATA
        private DataTable GetCustomerInvoiceWiseSalesData(DateTime fromDate, DateTime toDate,
                                                        string customerId, string companyId,
                                                        string townId, string sectorId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        -- Invoice Details
                        S.SaleId,
                        S.SaleDate,
                        S.BillNo,
                        S.BiltyNo,
                        S.TransportName,
                        
                        -- Customer Details
                        P.PartyID AS CustomerID,
                        P.PartyName AS CustomerName,
                        P.City AS CustomerCity,
                        
                        -- Company Details
                        C.CompanyID,
                        C.CompanyName,
                        
                        -- Location Details
                        SEC.SectorID,
                        SEC.SectorName,
                        T.TownID,
                        T.TownName,
                        
                        -- Product Details
                        PR.ProductID,
                        PR.ProductName,
                        SB.PackingId,
                        PK.PackingName,
                        SB.Multiplier,
                        SB.QtyPack,
                        SB.QtyLose,
                        (SB.QtyPack * SB.Multiplier + SB.QtyLose) AS TotalQuantity,
                        SB.PricePack,
                        SB.DiscRatio,
                        SB.DiscValPack,
                        SB.STaxRatio,
                        SB.STaxValPack,
                        SB.TTLValue,
                        SB.TTLSTax,
                        SB.TTLFTax,
                        (SB.TTLValue + SB.TTLSTax + SB.TTLFTax) AS NetAmount
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                    INNER JOIN Companies C ON PR.CompanyID = C.CompanyID
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SEC ON P.SectorID = SEC.SectorID
                    LEFT JOIN Towns T ON SEC.TownID = T.TownID
                    LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND (@CustomerId IS NULL OR S.CustomerId = @CustomerId)
                    AND (@CompanyId IS NULL OR PR.CompanyID = @CompanyId)
                    AND (@TownId IS NULL OR SEC.TownID = @TownId)
                    AND (@SectorId IS NULL OR P.SectorID = @SectorId)
                    ORDER BY S.SaleDate, S.BillNo, PR.ProductID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    cmd.Parameters.AddWithValue("@CustomerId",
                        string.IsNullOrEmpty(customerId) ? (object)DBNull.Value : customerId);
                    cmd.Parameters.AddWithValue("@CompanyId",
                        string.IsNullOrEmpty(companyId) ? (object)DBNull.Value : companyId);
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

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            cmpnynametxtbox.Clear();
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