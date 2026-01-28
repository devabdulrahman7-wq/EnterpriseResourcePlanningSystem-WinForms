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
    public partial class Purchase_Return_Register : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedVendorId = "";

        public Purchase_Return_Register()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-1);
            todatepcker.Value = DateTime.Today;
        }

        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
                {
                    MessageBox.Show("Please enter Vendor ID", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @VendorID AND PartyType = 'V'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            vendornametxtbox.Text = result.ToString();
                            selectedVendorId = vendoridtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Vendor ID not found or not a vendor", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            vendornametxtbox.Clear();
                            selectedVendorId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dt = GetPurchaseReturnRegisterData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase return data found for selected criteria", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "VendorName", string.IsNullOrEmpty(vendornametxtbox.Text) ? "All Vendors" : vendornametxtbox.Text }
                };

                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_Return_Register", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPurchaseReturnRegisterData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    pr.ReturnDate,
                    pr.PurReturnId as ReturnNo,
                    pur.BillNo as OriginalInvoiceNo,
                    p.PartyName as VendorName,
                    prod.ProductName as ProductName,
                    (prb.QtyPack * prb.Multiplier + prb.QtyLose) as ReturnedQuantity,
                    prb.PricePack as ReturnRate,
                    prb.TTLValue as ReturnAmount,
                    prb.TTLSTax as SalesTaxReturned,
                    prb.TTLFTax as FederalTaxReturned,
                    (prb.TTLValue + prb.TTLSTax + prb.TTLFTax) as TotalReturnValue,
                    pr.Narration as Reason
                FROM PurReturns pr
                INNER JOIN PurReturnsBody prb ON pr.PurReturnId = prb.PurReturnId
                LEFT JOIN Purchase pur ON pr.PurchaseId = pur.PurchaseID
                INNER JOIN Parties p ON pr.VendorId = p.PartyID
                INNER JOIN Products prod ON prb.ProductId = prod.ProductID
                WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                AND p.PartyType = 'V'";

                if (!string.IsNullOrEmpty(selectedVendorId))
                {
                    query += " AND pr.VendorId = @VendorID";
                }

                query += " ORDER BY pr.ReturnDate, pr.PurReturnId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

                    if (!string.IsNullOrEmpty(selectedVendorId))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", selectedVendorId);
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
        private void Purchase_Return_Register_Load(object sender, EventArgs e)
        {
            fromdatepcker.MinDate = new DateTime(2021, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }
        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            vendornametxtbox.Clear();
            selectedVendorId = "";
        }
    }
}