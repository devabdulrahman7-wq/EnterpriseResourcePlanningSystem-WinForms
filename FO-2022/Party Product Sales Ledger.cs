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
    public partial class Party_Product_Sales_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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
            string productId = string.IsNullOrWhiteSpace(productidtxtbox.Text) ? null : productidtxtbox.Text;
            string partyId = string.IsNullOrWhiteSpace(partyidtxtbox.Text) ? null : partyidtxtbox.Text;
            string skipPartyId = string.IsNullOrWhiteSpace(searchforpartyidskip.Text) ? null : searchforpartyidskip.Text;

            // Get data
            DataTable dt = GetSalesLedgerData(fromdatepicker.Value, todatepicker.Value,
                productId, partyId, skipPartyId);

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
                //{ "FromDate", fromdatepicker.Value.ToString("dd-MMM-yyyy") },
                //{ "ToDate", todatepicker.Value.ToString("dd-MMM-yyyy") },
                //{ "ProductID", productId ?? "All" },
                //{ "PartyID", partyId ?? "All" },
                //{ "SkipPartyID", skipPartyId ?? "None" },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Party_Product_Sales_Ledger", dataSources);
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
            if (string.IsNullOrWhiteSpace(productidtxtbox.Text))
            {
                MessageBox.Show("Please enter Product ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productName = GetProductName(productidtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(productName))
            {
                productnametxtbox.Text = productName;
            }
            else
            {
                MessageBox.Show("Product not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                productnametxtbox.Clear();
            }
        }

        private string GetProductName(string productId)
        {
            string productName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        productName = result.ToString();
                }
            }

            return productName;
        }

        // 📌 SEARCH PARTY
        private void searchpartyid_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Party ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string partyName = GetPartyName(partyidtxtbox.Text.Trim());
            if (!string.IsNullOrEmpty(partyName))
            {
                partynametxtbox.Text = partyName;
            }
            else
            {
                MessageBox.Show("Party not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                partynametxtbox.Clear();
            }
        }

        private string GetPartyName(string partyId)
        {
            string partyName = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PartyID", partyId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        partyName = result.ToString();
                }
            }

            return partyName;
        }

        // 📌 SEARCH SKIP PARTY
        private void searchskippartyid_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchforpartyidskip.Text))
            {
                MessageBox.Show("Please enter Skip Party ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string skipPartyName = GetPartyName(searchforpartyidskip.Text.Trim());
            if (!string.IsNullOrEmpty(skipPartyName))
            {
                partynameskiptxtbox.Text = skipPartyName;
            }
            else
            {
                MessageBox.Show("Party not found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                partynameskiptxtbox.Clear();
            }
        }

        // 📌 TEXT CHANGED EVENTS
        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            productnametxtbox.Clear();
        }

        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            partynametxtbox.Clear();
        }

        private void searchforpartyidskip_TextChanged(object sender, EventArgs e)
        {
            partynameskiptxtbox.Clear();
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

        // Add these controls to your form designer (optional manual setup):
        // 1. searchforpartyidskip (TextBox)
        // 2. partynameskiptxtbox (TextBox) - ReadOnly = true
        // 3. searchskippartyid (Button) - Text = "Search"
        // 4. Labels for "Skip Party ID" and "Skip Party Name"
    }
}