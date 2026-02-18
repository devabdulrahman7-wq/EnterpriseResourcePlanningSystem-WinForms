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
    public partial class Party_Product_Purchase_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedPartyId = "";
        private string selectedProductId = "";

        public Party_Product_Purchase_Ledger()
        {
            InitializeComponent();
            fromdatepicker.Value = DateTime.Today.AddMonths(-1);
            todatepicker.Value = DateTime.Today;
        }

        private void searchpartyid_Click(object sender, EventArgs e)
        {
            string partyId = partyidtxtbox.Text.Trim();
            string partyName = partynametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(partyId) && string.IsNullOrEmpty(partyName))
            {
                MessageBox.Show("Please enter Party ID or Party Name.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                partyidtxtbox.Focus();
                return;
            }

            try
            {
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

                            MessageBox.Show("Party not found.",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            partyidtxtbox.Focus();
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

        private void searchproductid_Click(object sender, EventArgs e)
        {
            string productId = productidtxtbox.Text.Trim();
            string productName = productnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(productId) && string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Please enter Product ID or Product Name.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                productidtxtbox.Focus();
                return;
            }

            try
            {
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

                            MessageBox.Show("Product not found.",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            productidtxtbox.Focus();
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

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (fromdatepicker.Value > todatepicker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dt = GetPartyProductPurchaseLedgerData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase ledger data found for selected criteria", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepicker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepicker.Value.ToString("dd-MMM-yyyy") },
                    //{ "PartyName", string.IsNullOrEmpty(partynametxtbox.Text) ? "All Parties" : partynametxtbox.Text },
                    //{ "ProductName", string.IsNullOrEmpty(productnametxtbox.Text) ? "All Products" : productnametxtbox.Text }
                };

                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Party_Product_Purchase_Ledger", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPartyProductPurchaseLedgerData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    pur.PurchaseDate,
                    pur.BillNo,
                    p.PartyName,
                    prod.ProductName,
                    pb.QtyPack,
                    pb.QtyLose,
                    pb.Multiplier,
                    pb.PricePack,
                    pb.DiscValPack,
                    pb.DiscValLose,
                    pb.TTLValue,
                    pb.TTLSTax,
                    pb.TTLFTax,
                    (pb.TTLValue + pb.TTLSTax + pb.TTLFTax) as TotalAmount
                FROM Purchase pur
                INNER JOIN PurchasesBody pb ON pur.PurchaseID = pb.PurchaseId
                INNER JOIN Parties p ON pur.VendorID = p.PartyID
                INNER JOIN Products prod ON pb.ProductId = prod.ProductID
                WHERE pur.PurchaseDate BETWEEN @FromDate AND @ToDate
                AND p.PartyType = 'V'";

                if (!string.IsNullOrEmpty(selectedPartyId))
                {
                    query += " AND pur.VendorID = @PartyID";
                }

                if (!string.IsNullOrEmpty(selectedProductId))
                {
                    query += " AND pb.ProductId = @ProductID";
                }

                query += " ORDER BY pur.PurchaseDate, pur.BillNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepicker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepicker.Value.Date.AddDays(1).AddSeconds(-1));

                    if (!string.IsNullOrEmpty(selectedPartyId))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", selectedPartyId);
                    }

                    if (!string.IsNullOrEmpty(selectedProductId))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", selectedProductId);
                    }

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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Party_Product_Purchase_Ledger_Load(object sender, EventArgs e)
        {
            fromdatepicker.MinDate = new DateTime(2000, 1, 1);
            todatepicker.MaxDate = DateTime.Today;
        }

        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            partynametxtbox.Clear();
            selectedPartyId = "";
        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            productnametxtbox.Clear();
            selectedProductId = "";
        }
    }
}