using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
            try
            {
                // Set default dates (last month to today)
                fromdatepcker.Value = DateTime.Today.AddMonths(-1);
                todatepcker.Value = DateTime.Today;

                // Clear vendor fields
                vendoridtxtbox.Clear();
                vendornametxtbox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Search Vendor
        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            string vendorId = vendoridtxtbox.Text.Trim();
            string vendorName = vendornametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(vendorId) && string.IsNullOrEmpty(vendorName))
            {
                MessageBox.Show("Please enter Vendor ID or Vendor Name.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                vendoridtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (!string.IsNullOrEmpty(vendorId))
                    {
                        query = @"SELECT PartyID, PartyName 
                          FROM Parties 
                          WHERE PartyID = @Value AND PartyType = 'V'";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", vendorId);
                    }
                    else
                    {
                        query = @"SELECT PartyID, PartyName 
                          FROM Parties 
                          WHERE PartyName = @Value AND PartyType = 'V'";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", vendorName);
                    }

                    using (cmd)
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendoridtxtbox.Text = reader["PartyID"].ToString();
                            vendornametxtbox.Text = reader["PartyName"].ToString();
                            //selectedVendorId = reader["PartyID"].ToString();
                        }
                        else
                        {
                            vendoridtxtbox.Clear();
                            vendornametxtbox.Clear();
                            MessageBox.Show("Vendor not found.",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear name when ID changes
            if (string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
            {
                vendornametxtbox.Clear();
            }
        }

        private void vendoridtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                vendoridsearch_Click(sender, e);
                e.Handled = true;
            }
        }

        // Validate before generating report
        private bool ValidateInput()
        {
            // Check date range
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check if vendor ID is entered but name is not fetched
            if (!string.IsNullOrWhiteSpace(vendoridtxtbox.Text) &&
                string.IsNullOrWhiteSpace(vendornametxtbox.Text))
            {
                MessageBox.Show("Please search and validate Vendor ID first.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Generate Report
        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate input
                if (!ValidateInput()) return;

                // Get data for report
                DataTable dt = GetPurchaseSummaryData(
                    fromdatepcker.Value,
                    todatepcker.Value,
                    vendoridtxtbox.Text.Trim()
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase records found for the selected criteria.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "ReportTitle", "Purchase Summary Report" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Add vendor filter if applicable
                if (!string.IsNullOrWhiteSpace(vendornametxtbox.Text))
                {
                    //parameters.Add("VendorFilter", $"{vendoridtxtbox.Text} - {vendornametxtbox.Text}");
                }

                // Show report viewer (universal one)
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_Summary", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetPurchaseSummaryData(DateTime fromDate, DateTime toDate, string vendorId)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("PurchaseID", typeof(int));
            dt.Columns.Add("PurchaseDate", typeof(DateTime));
            dt.Columns.Add("VendorID", typeof(string));
            dt.Columns.Add("VendorName", typeof(string));
            dt.Columns.Add("BillNo", typeof(string));
            dt.Columns.Add("BillDate", typeof(DateTime));
            dt.Columns.Add("GrossValue", typeof(decimal));
            dt.Columns.Add("TotalDiscount", typeof(decimal));
            dt.Columns.Add("TotalSalesTax", typeof(decimal));
            dt.Columns.Add("TotalFederalTax", typeof(decimal));
            dt.Columns.Add("NetValue", typeof(decimal));
            dt.Columns.Add("Narration", typeof(string));
            dt.Columns.Add("TransportName", typeof(string));
            dt.Columns.Add("BiltyNo", typeof(string));

            try
            {
                string query = @"
                    SELECT 
                        p.PurchaseID,
                        p.PurchaseDate,
                        p.VendorID,
                        ISNULL(par.PartyName, 'N/A') as VendorName,
                        ISNULL(p.BillNo, '') as BillNo,
                        p.BillDate,
                        SUM(pb.TTLValue) as GrossValue,
                        SUM(ISNULL(pb.DiscValPack, 0) + ISNULL(pb.DiscValLose, 0) + ISNULL(pb.OfferDiscount, 0)) as TotalDiscount,
                        SUM(pb.TTLSTax) as TotalSalesTax,
                        SUM(pb.TTLFTax) as TotalFederalTax,
                        SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax - 
                            (ISNULL(pb.DiscValPack, 0) + ISNULL(pb.DiscValLose, 0) + ISNULL(pb.OfferDiscount, 0))) as NetValue,
                        ISNULL(p.Narration, '') as Narration,
                        ISNULL(p.TransportName, '') as TransportName,
                        ISNULL(p.BiltyNo, '') as BiltyNo
                    FROM Purchase p
                    LEFT JOIN PurchasesBody pb ON p.PurchaseID = pb.PurchaseId
                    LEFT JOIN Parties par ON p.VendorID = par.PartyID
                    WHERE p.PurchaseDate BETWEEN @FromDate AND @ToDate
                        AND pb.IsDeleted = 0";

                // Add vendor filter if provided
                if (!string.IsNullOrWhiteSpace(vendorId))
                {
                    query += " AND p.VendorID = @VendorID";
                }

                query += @"
                    GROUP BY p.PurchaseID, p.PurchaseDate, p.VendorID, par.PartyName, 
                             p.BillNo, p.BillDate, p.Narration, p.TransportName, p.BiltyNo
                    ORDER BY p.PurchaseDate DESC, p.PurchaseID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        if (!string.IsNullOrWhiteSpace(vendorId))
                        {
                            cmd.Parameters.AddWithValue("@VendorID", vendorId);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["PurchaseID"] = reader["PurchaseID"];
                                row["PurchaseDate"] = reader["PurchaseDate"];
                                row["VendorID"] = reader["VendorID"];
                                row["VendorName"] = reader["VendorName"];
                                row["BillNo"] = reader["BillNo"];
                                row["BillDate"] = reader["BillDate"] != DBNull.Value ? reader["BillDate"] : DBNull.Value;
                                row["GrossValue"] = reader["GrossValue"];
                                row["TotalDiscount"] = reader["TotalDiscount"];
                                row["TotalSalesTax"] = reader["TotalSalesTax"];
                                row["TotalFederalTax"] = reader["TotalFederalTax"];
                                row["NetValue"] = reader["NetValue"];
                                row["Narration"] = reader["Narration"];
                                row["TransportName"] = reader["TransportName"];
                                row["BiltyNo"] = reader["BiltyNo"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching purchase data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value;
            }
        }

        private void vendornametxtbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}