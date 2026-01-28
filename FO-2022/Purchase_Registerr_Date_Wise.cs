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
    public partial class Purchase_Registerr_Date_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedVendorId = "";

        public Purchase_Registerr_Date_Wise()
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

                DataTable dt = GetPurchaseRegisterData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase data found for selected criteria", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var parameters = new Dictionary<string, string>
                {
                //    { "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //    { "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //    { "VendorName", string.IsNullOrEmpty(vendornametxtbox.Text) ? "All Vendors" : vendornametxtbox.Text }
                };

                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_register_date_wise", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPurchaseRegisterData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    pur.PurchaseDate as PurchaseDate,
                    pur.BillNo as InvoiceNo,
                    p.PartyName as VendorName,
                    SUM(pb.TTLValue) as TotalAmount,
                    SUM(pb.TTLSTax) as SalesTax,
                    SUM(pb.TTLFTax) as FederalTax,
                    SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax) as GrossAmount,
                    ISNULL(pur.PaidAmount, 0) as PaidAmount,
                    (SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax) - ISNULL(pur.PaidAmount, 0)) as Balance
                FROM Purchase pur
                INNER JOIN PurchasesBody pb ON pur.PurchaseID = pb.PurchaseId
                INNER JOIN Parties p ON pur.VendorID = p.PartyID
                WHERE pur.PurchaseDate BETWEEN @FromDate AND @ToDate
                AND p.PartyType = 'V'";

                if (!string.IsNullOrEmpty(selectedVendorId))
                {
                    query += " AND pur.VendorID = @VendorID";
                }

                query += @" GROUP BY pur.PurchaseID, pur.PurchaseDate, pur.BillNo, p.PartyName, pur.PaidAmount
                          ORDER BY pur.PurchaseDate, pur.BillNo";

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Purchase_Registerr_Date_Wise_Load(object sender, EventArgs e)
        {
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            vendornametxtbox.Clear();
            selectedVendorId = "";
        }
    }
}