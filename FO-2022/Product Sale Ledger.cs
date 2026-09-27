using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Product_Sale_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Product_Sale_Ledger()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            fromdatepicker.Value = DateTime.Today.AddDays(-30);
            todatepicker.Value = DateTime.Today;
        }

        private void pictureBox1_Click(object sender, EventArgs e) => this.Close();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();

        private void fromdatepicker_ValueChanged(object sender, EventArgs e)
        {
            if (fromdatepicker.Value > todatepicker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepicker.Value = todatepicker.Value.AddDays(-1);
            }
        }

        private void todatepicker_ValueChanged(object sender, EventArgs e)
        {
            if (todatepicker.Value < fromdatepicker.Value)
            {
                MessageBox.Show("To date cannot be less than From date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepicker.Value = fromdatepicker.Value.AddDays(1);
            }
        }

        private void searchproductid_Click(object sender, EventArgs e)
        {
            string productID = productidtxtbox.Text.Trim();
            string productName = productnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(productID) && string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Please enter Product ID or Name.",
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

                    if (!string.IsNullOrEmpty(productID))
                        query = "SELECT ProductID, ProductName FROM Products WHERE ProductID = @Value";
                    else
                        query = "SELECT ProductID, ProductName FROM Products WHERE ProductName = @Value";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(productID) ? productID : productName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                productidtxtbox.Text = reader["ProductID"].ToString();
                                productnametxtbox.Text = reader["ProductName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Product not found.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                productidtxtbox.Clear();
                                productnametxtbox.Clear();
                                productidtxtbox.Focus();
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


        private DataTable GetProductInfo(string productId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductID, ProductName FROM Products WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!ValidateInput()) return;

                DateTime fromDate = fromdatepicker.Value.Date;
                DateTime toDate = todatepicker.Value.Date.AddDays(1).AddSeconds(-1);
                string productId = productidtxtbox.Text.Trim();
                string productName = productnametxtbox.Text;

                DataTable reportData = GetProductSaleLedgerData(fromDate, toDate, productId);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Product Sale Ledger" },
                    //{ "FromDate", fromdatepicker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepicker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ProductInfo", $"{productId} - {productName}" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Product_Sale_Ledger", dataSources);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetProductSaleLedgerData(DateTime fromDate, DateTime toDate, string productId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Simple query with fewer columns
                string query = @"
                    SELECT 
                        S.SaleDate as [Date],
                        S.SaleId as [InvoiceNo],
                        PT.PartyName as [Customer],
                        PK.PackingName as [Packing],
                        (SB.QtyPack * SB.Multiplier) + SB.QtyLose as [Quantity],
                        SB.PricePack as [Rate],
                        SB.DiscRatio as [Discount%],
                        SB.TTLValue as [Amount],
                        SB.TTLSTax as [SalesTax],
                        SM.SalesmanName as [Salesman],
                        S.BiltyNo as [BiltyNo]
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Products P ON SB.ProductId = P.ProductID
                    INNER JOIN Parties PT ON S.CustomerId = PT.PartyID
                    LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId
                    LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND SB.ProductId = @ProductId
                    AND S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    ORDER BY S.SaleDate, S.SaleId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            if (fromdatepicker.Value > todatepicker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(productidtxtbox.Text.Trim()))
            {
                MessageBox.Show("Please enter Product ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(productnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Product Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }

        private void Product_Sale_Ledger_Load(object sender, EventArgs e)
        {

        }
    }
}