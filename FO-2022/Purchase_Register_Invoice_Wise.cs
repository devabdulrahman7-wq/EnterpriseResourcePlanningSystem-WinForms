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
    public partial class Purchase_Register_Invoice_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedVendorId = "";

        public Purchase_Register_Invoice_Wise()
        {
            InitializeComponent();
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

        private void frominvoicesearch_Click(object sender, EventArgs e)
        {
            // Validate from invoice is numeric
            if (!string.IsNullOrWhiteSpace(frominvoicetxtbox.Text) && !int.TryParse(frominvoicetxtbox.Text, out _))
            {
                MessageBox.Show("From Invoice must be a number", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                frominvoicetxtbox.Focus();
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate invoice numbers
                int fromInvoice = 0, toInvoice = 0;

                if (!string.IsNullOrWhiteSpace(frominvoicetxtbox.Text))
                {
                    if (!int.TryParse(frominvoicetxtbox.Text, out fromInvoice))
                    {
                        MessageBox.Show("From Invoice must be a valid number", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(toinvoicetxtbox.Text))
                {
                    if (!int.TryParse(toinvoicetxtbox.Text, out toInvoice))
                    {
                        MessageBox.Show("To Invoice must be a valid number", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Validate range
                if (fromInvoice > 0 && toInvoice > 0 && fromInvoice > toInvoice)
                {
                    MessageBox.Show("From Invoice cannot be greater than To Invoice", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetPurchaseRegisterInvoiceData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase data found for selected criteria", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "VendorName", string.IsNullOrEmpty(vendornametxtbox.Text) ? "All Vendors" : vendornametxtbox.Text },
                    //{ "FromInvoice", string.IsNullOrEmpty(frominvoicetxtbox.Text) ? "N/A" : frominvoicetxtbox.Text },
                    //{ "ToInvoice", string.IsNullOrEmpty(toinvoicetxtbox.Text) ? "N/A" : toinvoicetxtbox.Text }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_Register_Invoice_Wise", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPurchaseRegisterInvoiceData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    pur.BillNo as InvoiceNo,
                    pur.PurchaseDate as InvoiceDate,
                    p.PartyName as VendorName,
                    prod.ProductName as ProductName,
                    pack.PackingName as Packing,
                    (pb.QtyPack * pb.Multiplier + pb.QtyLose) as Quantity,
                    pb.PricePack as Rate,
                    (pb.DiscValPack + pb.DiscValLose) as Discount,
                    pb.TTLValue as Amount,
                    pb.TTLSTax as SalesTax,
                    pb.TTLFTax as FederalTax,
                    (pb.TTLValue + pb.TTLSTax + pb.TTLFTax) as Total
                FROM Purchase pur
                INNER JOIN PurchasesBody pb ON pur.PurchaseID = pb.PurchaseId
                INNER JOIN Parties p ON pur.VendorID = p.PartyID
                INNER JOIN Products prod ON pb.ProductId = prod.ProductID
                INNER JOIN Packings pack ON pb.PackingId = pack.PackingId
                WHERE p.PartyType = 'V'
                ";

                // Add vendor filter
                if (!string.IsNullOrEmpty(selectedVendorId))
                {
                    query += " AND pur.VendorID = @VendorID";
                }

                // Add invoice range filter
                if (!string.IsNullOrWhiteSpace(frominvoicetxtbox.Text) && int.TryParse(frominvoicetxtbox.Text, out int fromInvoice))
                {
                    query += " AND pur.BillNo >= @FromInvoice";
                }

                if (!string.IsNullOrWhiteSpace(toinvoicetxtbox.Text) && int.TryParse(toinvoicetxtbox.Text, out int toInvoice))
                {
                    query += " AND pur.BillNo <= @ToInvoice";
                }

                query += " ORDER BY pur.BillNo, prod.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add vendor parameter
                    if (!string.IsNullOrEmpty(selectedVendorId))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", selectedVendorId);
                    }

                    // Add invoice range parameters
                    if (!string.IsNullOrWhiteSpace(frominvoicetxtbox.Text) && int.TryParse(frominvoicetxtbox.Text, out fromInvoice))
                    {
                        cmd.Parameters.AddWithValue("@FromInvoice", fromInvoice);
                    }

                    if (!string.IsNullOrWhiteSpace(toinvoicetxtbox.Text) && int.TryParse(toinvoicetxtbox.Text, out toInvoice))
                    {
                        cmd.Parameters.AddWithValue("@ToInvoice", toInvoice);
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
        private void Purchase_Register_Invoice_Wise_Load(object sender, EventArgs e)
        {
            // Optional initialization
        }
        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            vendornametxtbox.Clear();
            selectedVendorId = "";
        }
        private void frominvoicetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Auto-validate numeric input
        }
        private void toinvoicetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Auto-validate numeric input
        }
    }
}