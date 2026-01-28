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
    public partial class Product_Purchase_Return_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedProductId = "";

        public Product_Purchase_Return_Ledger()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-1);
            todatepcker.Value = DateTime.Today;
        }

        private void prdctidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prdctidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", prdctidtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            prdctnametxtbox.Text = result.ToString();
                            selectedProductId = prdctidtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Product ID not found", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            prdctnametxtbox.Clear();
                            selectedProductId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetProductPurchaseReturnLedger();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ProductID", string.IsNullOrEmpty(selectedProductId) ? "All Products" : selectedProductId },
                    //{ "ProductName", string.IsNullOrEmpty(prdctnametxtbox.Text) ? "All Products" : prdctnametxtbox.Text }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Product_Purchase_Return_Ledger", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetProductPurchaseReturnLedger()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    -- Product Info
                    p.ProductID as [Product Code],
                    p.ProductName as [Product Name],
                    
                    -- Purchase Info
                    pur.PurchaseDate as [Purchase Date],
                    pur.BillNo as [Invoice No],
                    pv.PartyName as [Vendor Name],
                    
                    -- Purchase Quantities
                    pb.QtyPack * pb.Multiplier + pb.QtyLose as [Purchased Qty],
                    
                    -- Return Quantities (from returns)
                    ISNULL(ret.ReturnedQty, 0) as [Returned Qty],
                    
                    -- Net Quantity
                    (pb.QtyPack * pb.Multiplier + pb.QtyLose) - ISNULL(ret.ReturnedQty, 0) as [Net Qty],
                    
                    -- Rates
                    pb.PricePack as [Purchase Rate],
                    ISNULL(ret.ReturnRate, 0) as [Return Rate],
                    
                    -- Values
                    pb.TTLValue as [Purchase Value],
                    ISNULL(ret.ReturnValue, 0) as [Return Value],
                    
                    -- Net Value
                    pb.TTLValue - ISNULL(ret.ReturnValue, 0) as [Net Value]
                    
                FROM PurchasesBody pb
                INNER JOIN Purchase pur ON pb.PurchaseId = pur.PurchaseID
                INNER JOIN Products p ON pb.ProductId = p.ProductID
                INNER JOIN Parties pv ON pur.VendorID = pv.PartyID
                
                LEFT JOIN (
                    SELECT 
                        prb.ProductId,
                        SUM(prb.QtyPack * prb.Multiplier + prb.QtyLose) as ReturnedQty,
                        AVG(prb.PricePack) as ReturnRate,
                        SUM(prb.TTLValue) as ReturnValue
                    FROM PurReturnsBody prb
                    INNER JOIN PurReturns pr ON prb.PurReturnId = pr.PurReturnId
                    WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                    GROUP BY prb.ProductId
                ) ret ON pb.ProductId = ret.ProductId
                
                WHERE pur.PurchaseDate BETWEEN @FromDate AND @ToDate
                ";

                // Add product filter if selected
                if (!string.IsNullOrEmpty(selectedProductId))
                {
                    query += " AND p.ProductID = @ProductID";
                }

                query += " ORDER BY p.ProductID, pur.PurchaseDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

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
            previewbtn_Click(sender, e); // Same functionality as preview
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Product_Purchase_Return_Ledger_Load(object sender, EventArgs e)
        {
            // Set date limits
            fromdatepcker.MinDate = new DateTime(2022, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void prdctidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear product name when ID changes
            prdctnametxtbox.Clear();
            selectedProductId = "";
        }
    }
}