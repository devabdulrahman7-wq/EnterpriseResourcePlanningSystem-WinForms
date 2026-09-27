using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_Product_Sales_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedProductId = "";
        private string selectedPartyId = "";
        private string selectedSkipPartyId = "";

        public Party_Product_Sales_Ledger()
        {
            InitializeComponent();
        }

        private void Party_Product_Sales_Ledger_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepicker.Value = DateTime.Now.AddDays(-666);
            todatepicker.Value = DateTime.Now;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GENERATE REPORT METHOD
        private void GenerateReport(bool autoPrint)
        {
            // Validate dates
            if (fromdatepicker.Value > todatepicker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get parameters
            string productId = string.IsNullOrWhiteSpace(selectedProductId) ? null : selectedProductId;
            string partyId = string.IsNullOrWhiteSpace(selectedPartyId) ? null : selectedPartyId;
            string skipPartyId = string.IsNullOrWhiteSpace(selectedSkipPartyId) ? null : selectedSkipPartyId;

            // Get data
            DataTable dt = GetSalesLedgerData(fromdatepicker.Value, todatepicker.Value,
                productId, partyId, skipPartyId);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the selected criteria", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable reportTable = PartyProductSalesLedgerProfessionalBuilder.Build(dt);

            // Prepare data sources
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
            {
                { PartyProductSalesLedgerProfessionalBuilder.DataSetName, reportTable }
            };

            // Show report using the new professional RDLC only.
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("PartyProductSalesLedger_Professional", dataSources);
        }                       

        // 📌 GET SALES LEDGER DATA FROM DATABASE
        private DataTable GetSalesLedgerData(DateTime fromDate, DateTime toDate,
            string productId, string partyId, string skipPartyId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        S.SaleDate,
                        S.BillNo,
                        S.CustomerId,
                        P.PartyName AS CustomerName,
                        SB.ProductId,
                        PR.ProductName,
                        SB.PackingId,
                        PK.PackingName,
                        SB.Multiplier,
                        SB.QtyPack,
                        SB.QtyLose,
                        (SB.QtyPack * SB.Multiplier) + SB.QtyLose AS TotalQty,
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
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                    INNER JOIN Packings PK ON SB.PackingId = PK.PackingId
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND (@ProductId IS NULL OR SB.ProductId = @ProductId)
                    AND (@PartyId IS NULL OR S.CustomerId = @PartyId)
                    AND (@SkipPartyId IS NULL OR S.CustomerId != @SkipPartyId)
                    ORDER BY S.SaleDate, S.BillNo, SB.ProductId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    cmd.Parameters.AddWithValue("@ProductId",
                        string.IsNullOrEmpty(productId) ? (object)DBNull.Value : productId);
                    cmd.Parameters.AddWithValue("@PartyId",
                        string.IsNullOrEmpty(partyId) ? (object)DBNull.Value : partyId);
                    cmd.Parameters.AddWithValue("@SkipPartyId",
                        string.IsNullOrEmpty(skipPartyId) ? (object)DBNull.Value : skipPartyId);
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

        // 📌 SEARCH PRODUCT
        private void searchproductid_Click(object sender, EventArgs e)
        {
            string productId = productidtxtbox.Text.Trim();
            string productName = productnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(productId) && string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Please enter Product ID or Product Name",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;
                SqlCommand cmd;

                if (!string.IsNullOrEmpty(productId))
                {
                    query = "SELECT ProductID, ProductName FROM Products WHERE ProductID = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", productId);
                }
                else
                {
                    query = "SELECT ProductID, ProductName FROM Products WHERE ProductName = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", productName);
                }

                using (cmd)
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        productidtxtbox.Text = reader["ProductID"].ToString();
                        productnametxtbox.Text = reader["ProductName"].ToString();
                        selectedProductId = reader["ProductID"].ToString();
                    }
                    else
                    {
                        productidtxtbox.Clear();
                        productnametxtbox.Clear();
                        selectedProductId = "";

                        MessageBox.Show("Product not found",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        // 📌 SEARCH PARTY
        private void searchpartyid_Click(object sender, EventArgs e)
        {
            string partyId = partyidtxtbox.Text.Trim();
            string partyName = partynametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(partyId) && string.IsNullOrEmpty(partyName))
            {
                MessageBox.Show("Please enter Party ID or Party Name",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;
                SqlCommand cmd;

                if (!string.IsNullOrEmpty(partyId))
                {
                    query = "SELECT PartyID, PartyName FROM Parties WHERE PartyID = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", partyId);
                }
                else
                {
                    query = "SELECT PartyID, PartyName FROM Parties WHERE PartyName = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", partyName);
                }

                using (cmd)
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partyidtxtbox.Text = reader["PartyID"].ToString();
                        partynametxtbox.Text = reader["PartyName"].ToString();
                        selectedPartyId = reader["PartyID"].ToString();
                    }
                    else
                    {
                        partyidtxtbox.Clear();
                        partynametxtbox.Clear();
                        selectedPartyId = "";

                        MessageBox.Show("Party not found",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        // 📌 SEARCH SKIP PARTY
        private void searchskippartyid_Click(object sender, EventArgs e)
        {
            string partyId = partyidforskiptxtbox.Text.Trim();
            string partyName = partynameskiptxtbox.Text.Trim();

            if (string.IsNullOrEmpty(partyId) && string.IsNullOrEmpty(partyName))
            {
                MessageBox.Show("Please enter Skip Party ID or Name",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;
                SqlCommand cmd;

                if (!string.IsNullOrEmpty(partyId))
                {
                    query = "SELECT PartyID, PartyName FROM Parties WHERE PartyID = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", partyId);
                }
                else
                {
                    query = "SELECT PartyID, PartyName FROM Parties WHERE PartyName = @Value";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Value", partyName);
                }

                using (cmd)
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        partyidforskiptxtbox.Text = reader["PartyID"].ToString();
                        partynameskiptxtbox.Text = reader["PartyName"].ToString();
                        selectedSkipPartyId = reader["PartyID"].ToString();
                    }
                    else
                    {
                        partyidforskiptxtbox.Clear();
                        partynameskiptxtbox.Clear();
                        selectedSkipPartyId = "";

                        MessageBox.Show("Party not found",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        // 📌 TEXT CHANGED EVENTS
        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            productnametxtbox.Clear();
            selectedProductId = "";
        }

        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            partynametxtbox.Clear();
            selectedPartyId = "";
        }
        // 📌 DATE CHANGED EVENTS
        private void fromdatepicker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }

        private void todatepicker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }

        private void partyidforskiptxtbox_TextChanged(object sender, EventArgs e)
        {
            partynameskiptxtbox.Clear();
            selectedSkipPartyId = "";
        }

        // Add these controls to your form designer (optional manual setup):
        // 1. searchforpartyidskip (TextBox)
        // 2. partynameskiptxtbox (TextBox) - ReadOnly = true
        // 3. searchskippartyid (Button) - Text = "Search"
        // 4. Labels for "Skip Party ID" and "Skip Party Name"
    }
}
