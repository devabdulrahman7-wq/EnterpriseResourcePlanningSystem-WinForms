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
    public partial class Account_Payable : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Account_Payable()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-12); // Last month se
            todatepcker.Value = DateTime.Today;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date!", "Invalid Date",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dtPayables = GetPayableAccounts();

                if (dtPayables == null || dtPayables.Rows.Count == 0)
                {
                    MessageBox.Show("No payable accounts found for the selected period.",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                var parameters = new Dictionary<string, string>
                {
                    { "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    { "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dtPayables }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Account_Pay", dataSources, parameters);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error generating report:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPayableAccounts()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    COA.AccountName,
                    COA.AccountNo,
                    ISNULL(SUM(AV.Credit) - SUM(AV.Debit), 0) as Payable
                FROM ChartOfAccounts COA
                LEFT JOIN AllVouchers AV ON COA.AccountNo = AV.AccountNo
                WHERE AV.VoucherDate BETWEEN @FromDate AND @ToDate
                GROUP BY COA.AccountNo, COA.AccountName
                HAVING ISNULL(SUM(AV.Credit) - SUM(AV.Debit), 0) > 0
                ORDER BY Payable DESC, COA.AccountName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));
                    //cmd.CommandTimeout = 150;
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
            previewbtn_Click(sender, e); // Same as preview
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Account_Payable_Load(object sender, EventArgs e)
        {
            // Optional: Set minimum date
            fromdatepcker.MinDate = new DateTime(2021, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }
    }
}