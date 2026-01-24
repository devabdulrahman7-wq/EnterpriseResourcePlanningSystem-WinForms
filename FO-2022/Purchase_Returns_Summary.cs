using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Purchase_Returns_Summary : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Purchase_Returns_Summary()
        {
            InitializeComponent();
        }

        private void Purchase_Returns_Summary_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = new DateTime(2024, 1, 1);
            todatepcker.Value = DateTime.Today;

            // Clear textboxes
            ClearSearchFields();
        }

        private void ClearSearchFields()
        {
            vendoridtxtbox.Text = "";
            vendornametxtbox.Text = "";
        }

        // Vendor ID Search
        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            string vendorId = vendoridtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(vendorId))
            {
                MessageBox.Show("Please enter Vendor ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID AND PartyType = 'V'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", vendorId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            vendornametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Vendor not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            vendornametxtbox.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Preview Button
        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(false);
        }

        // Print Button
        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(true);
        }

        private void GenerateReport(bool directPrint)
        {
            try
            {
                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From Date cannot be greater than To Date", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get filter parameters
                string vendorId = string.IsNullOrWhiteSpace(vendoridtxtbox.Text) ? null : vendoridtxtbox.Text.Trim();

                // Fetch data
                DataTable dt = GetPurchaseReturnData(
                    fromdatepcker.Value,
                    todatepcker.Value,
                    vendorId
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources and parameters
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                var parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Purchase Returns Summary" },
                    { "FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy") },
                    { "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "VendorFilter", string.IsNullOrEmpty(vendornametxtbox.Text) ? "All Vendors" : $"Vendor: {vendornametxtbox.Text}" }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_Return_Summary", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable GetPurchaseReturnData(DateTime fromDate, DateTime toDate, string vendorId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ FIXED: Time issue resolve karne ke liye
                    // SQL Server me date comparison ke liye proper formatting
                    StringBuilder query = new StringBuilder();
                    query.AppendLine(@"
                SELECT 
                    PR.PurReturnId,
                    PR.ReturnDate,
                    CASE 
                        WHEN PR.PurchaseId IS NULL THEN 'N/A'
                        ELSE CAST(PR.PurchaseId AS VARCHAR(10))
                    END AS PurchaseId,
                    P.PartyName AS VendorName,
                    ISNULL(SUM(PRB.TTLValue), 0) AS GrossValue,
                    ISNULL(SUM(PRB.DiscValPack + PRB.DiscValLose), 0) AS Discount,
                    ISNULL(SUM(PRB.TTLSTax), 0) AS SalesTax,
                    ISNULL(SUM(PRB.TTLValue), 0) AS NetValue
                FROM PurReturns PR
                INNER JOIN PurReturnsBody PRB ON PR.PurReturnId = PRB.PurReturnId
                INNER JOIN Parties P ON PR.VendorId = P.PartyID
                WHERE CONVERT(DATE, PR.ReturnDate) BETWEEN @FromDate AND @ToDate
            ");

                    // Add vendor filter if provided
                    if (!string.IsNullOrEmpty(vendorId))
                    {
                        query.AppendLine(" AND PR.VendorId = @VendorId");
                    }

                    query.AppendLine(@"
                GROUP BY PR.PurReturnId, PR.ReturnDate, PR.PurchaseId, P.PartyName
                ORDER BY PR.ReturnDate
            ");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        // ✅ Time part hata ke pure date use kare
                        cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                        cmd.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1)); // End of day

                        if (!string.IsNullOrEmpty(vendorId))
                        {
                            cmd.Parameters.AddWithValue("@VendorId", vendorId);
                        }

                        Console.WriteLine("Executing query: " + query.ToString());
                        Console.WriteLine("FromDate: " + fromDate.Date);
                        Console.WriteLine("ToDate: " + toDate.Date);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        // Debug output
                        Console.WriteLine($"Rows returned: {dt.Rows.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching data: {ex.Message}\nStack Trace: {ex.StackTrace}", ex);
            }

            return dt;
        }
        //private DataTable GetPurchaseReturnData(DateTime fromDate, DateTime toDate, string vendorId)
        //{
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            StringBuilder query = new StringBuilder();
        //            query.AppendLine(@"
        //        SELECT 
        //            PR.PurReturnId,
        //            PR.ReturnDate,
        //            CASE 
        //                WHEN PR.PurchaseId IS NULL THEN 'N/A'
        //                ELSE CAST(PR.PurchaseId AS VARCHAR(10))
        //            END AS PurchaseId,
        //            P.PartyName AS VendorName,
        //            ISNULL(SUM(PRB.TTLValue), 0) AS GrossValue,
        //            ISNULL(SUM(PRB.DiscValPack + PRB.DiscValLose), 0) AS Discount,
        //            ISNULL(SUM(PRB.TTLSTax), 0) AS SalesTax,
        //            ISNULL(SUM(PRB.TTLValue), 0) AS NetValue
        //        FROM PurReturns PR
        //        INNER JOIN PurReturnsBody PRB ON PR.PurReturnId = PRB.PurReturnId
        //        INNER JOIN Parties P ON PR.VendorId = P.PartyID
        //        WHERE PR.ReturnDate BETWEEN @FromDate AND @ToDate
        //    ");

        //            // Add vendor filter if provided
        //            if (!string.IsNullOrEmpty(vendorId))
        //                query.AppendLine(" AND PR.VendorId = @VendorId");

        //            query.AppendLine(@"
        //        GROUP BY PR.PurReturnId, PR.ReturnDate, PR.PurchaseId, P.PartyName
        //        ORDER BY PR.ReturnDate
        //    ");

        //            using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
        //            {
        //                cmd.Parameters.AddWithValue("@FromDate", fromDate);
        //                cmd.Parameters.AddWithValue("@ToDate", toDate);

        //                if (!string.IsNullOrEmpty(vendorId))
        //                    cmd.Parameters.AddWithValue("@VendorId", vendorId);

        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    da.Fill(dt);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error fetching data: {ex.Message}", ex);
        //    }

        //    return dt;
        //}
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
                vendornametxtbox.Text = "";
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Validate date range
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Validate date range
        }
    }
}