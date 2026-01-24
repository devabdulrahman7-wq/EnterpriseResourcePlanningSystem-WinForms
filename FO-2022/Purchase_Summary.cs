using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace FO_2022
{
    public partial class Purchase_Summary : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Purchase_Summary()
        {
            InitializeComponent();
        }

        private void Purchase_Summary_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;

            // Clear vendor fields
            vendoridtxtbox.Clear();
            vendornametxtbox.Clear();

            // Disable vendor name field (read-only)
            vendornametxtbox.ReadOnly = true;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate date range
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // SQL Query to get purchase summary with vendor name from Parties table
                    string query = @"
                    SELECT 
                        p.PurchaseID,
                        p.PurchaseDate,
                        ISNULL(pt.PartyName, 'Unknown Vendor') as VendorName,
                        ISNULL(SUM(pb.TTLValue), 0) as GrossValue,
                        ISNULL(SUM(
                            ISNULL(pb.DiscValPack, 0) * ISNULL(pb.QtyPack, 0) + 
                            ISNULL(pb.DiscValLose, 0) + 
                            ISNULL(pb.OfferDiscount, 0)
                        ), 0) as Discount,
                        ISNULL(SUM(pb.TTLSTax), 0) as SalesTax,
                        ISNULL(SUM(pb.TTLFTax), 0) as FederalTax,
                        ISNULL(SUM(
                            ISNULL(pb.TTLValue, 0) - 
                            (ISNULL(pb.DiscValPack, 0) * ISNULL(pb.QtyPack, 0) + 
                             ISNULL(pb.DiscValLose, 0) + 
                             ISNULL(pb.OfferDiscount, 0)) + 
                            ISNULL(pb.TTLSTax, 0) + 
                            ISNULL(pb.TTLFTax, 0)
                        ), 0) as NetValue
                    FROM Purchase p
                    LEFT JOIN PurchasesBody pb ON p.PurchaseID = pb.PurchaseId
                    LEFT JOIN Parties pt ON p.VendorID = pt.PartyID 
                        AND pt.PartyType = 'V'  -- Only vendors
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                        AND (@VendorID IS NULL OR p.VendorID = @VendorID)
                    GROUP BY p.PurchaseID, p.PurchaseDate, pt.PartyName
                    ORDER BY p.PurchaseDate DESC, p.PurchaseID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

                    // Add vendor filter if provided
                    if (!string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text.Trim());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@VendorID", DBNull.Value);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    conn.Close();

                    // Check if data exists
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No purchase records found for the selected criteria!", "No Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Debug: Show data info
                    string debugInfo = $"Found {dt.Rows.Count} purchase records\n";
                    debugInfo += $"First record:\n";
                    debugInfo += $"PurchaseID: {dt.Rows[0]["PurchaseID"]}\n";
                    debugInfo += $"Date: {dt.Rows[0]["PurchaseDate"]}\n";
                    debugInfo += $"Vendor: {dt.Rows[0]["VendorName"]}\n";
                    debugInfo += $"Gross: {dt.Rows[0]["GrossValue"]:N2}\n";
                    debugInfo += $"Discount: {dt.Rows[0]["Discount"]:N2}\n";
                    debugInfo += $"Tax: {dt.Rows[0]["SalesTax"]:N2}\n";
                    debugInfo += $"Net: {dt.Rows[0]["NetValue"]:N2}";

                    MessageBox.Show(debugInfo, "Data Preview");

                    // Open report
                    Reports.Purchase_Summary_rv reportForm = new Reports.Purchase_Summary_rv(dt);
                    reportForm.Text = "Purchase Summary Report";
                    reportForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SEARCH VENDOR BY ID (FROM PARTIES TABLE WHERE PartyType = 'V')
        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string vendorID = vendoridtxtbox.Text.Trim();

                if (string.IsNullOrWhiteSpace(vendorID))
                {
                    MessageBox.Show("Please enter Vendor ID first!", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query Parties table for vendor (PartyType = 'V')
                    string query = @"
                    SELECT PartyName 
                    FROM Parties 
                    WHERE PartyID = @VendorID 
                      AND PartyType = 'V'";  // V = Vendor

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VendorID", vendorID);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != null)
                    {
                        vendornametxtbox.Text = result.ToString();
                        MessageBox.Show($"Vendor Found!\nID: {vendorID}\nName: {result}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Try if it's a valid account in ChartOfAccounts but not in Parties
                        string query2 = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @VendorID";
                        SqlCommand cmd2 = new SqlCommand(query2, conn);
                        cmd2.Parameters.AddWithValue("@VendorID", vendorID);

                        conn.Open();
                        object result2 = cmd2.ExecuteScalar();
                        conn.Close();

                        if (result2 != null)
                        {
                            vendornametxtbox.Text = result2.ToString() + " (Chart Account)";
                            MessageBox.Show($"Account found in ChartOfAccounts but not in Vendors (Parties table)\n" +
                                $"Name: {result2}", "Note",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            vendornametxtbox.Clear();
                            MessageBox.Show("Vendor ID not found!\n" +
                                "Check:\n1. Vendor exists in Parties table\n2. PartyType = 'V'\n3. Correct Vendor ID",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching vendor: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear vendor name when ID changes
            if (string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
            {
                vendornametxtbox.Clear();
            }
            else
            {
                //vendornametxtbox.Text = "Enter ID and click Search →";
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            // Same as preview
            previewbtn_Click(sender, e);
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (fromdatepcker.Value > todatepcker.Value)
            {
                fromdatepcker.Value = todatepcker.Value;
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validate date range
            if (todatepcker.Value < fromdatepcker.Value)
            {
                todatepcker.Value = fromdatepcker.Value;
            }
        }

        // Auto-search on Enter key press in VendorID textbox
        private void vendoridtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                vendoridsearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}