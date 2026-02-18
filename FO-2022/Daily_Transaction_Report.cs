using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Daily_Transaction_Report : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Daily_Transaction_Report()
        {
            InitializeComponent();
        }

        private void Daily_Transaction_Report_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default dates (yesterday to today)
                fromdatepcker.Value = DateTime.Today.AddDays(-1);
                todatepcker.Value = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation: From date cannot be greater than To date
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation: To date cannot be earlier than From date
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value;
            }
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate date range
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be later than To date.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data for report
                DataTable dt = GetDailyTransactionData(
                    fromdatepcker.Value,
                    todatepcker.Value
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions found for the selected date range.", "Info",
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
                    //{ "ReportTitle", "Daily Transaction Report" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Show report viewer
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Daily_Transaction_Report", dataSources);
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

        private DataTable GetDailyTransactionData(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset (simple columns)
            dt.Columns.Add("TransactionDate", typeof(DateTime));
            dt.Columns.Add("TransactionType", typeof(string));
            dt.Columns.Add("TransactionNo", typeof(string));
            dt.Columns.Add("AccountNo", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("PartyName", typeof(string));

            try
            {
                string query = @"
                    SELECT 
                        VoucherDate as TransactionDate,
                        CASE VoucherType
                            WHEN 'JV' THEN 'Journal Voucher'
                            WHEN 'BV' THEN 'Bank Voucher'
                            WHEN 'CR' THEN 'Credit Voucher'
                            WHEN 'DR' THEN 'Debit Voucher'
                            WHEN 'CP' THEN 'Cash Payment'
                            WHEN 'CR' THEN 'Cash Receipt'
                            ELSE VoucherType
                        END as TransactionType,
                        CAST(VoucherNo as varchar) as TransactionNo,
                        AccountNo,
                        COA.AccountName,
                        Narration as Description,
                        Debit,
                        Credit,
                        P.PartyName
                    FROM AllVouchers AV
                    LEFT JOIN ChartOfAccounts COA ON AV.AccountNo = COA.AccountNo
                    LEFT JOIN Parties P ON AV.AccountNo = P.PartyID
                    WHERE VoucherDate BETWEEN @FromDate AND @ToDate
                    ORDER BY VoucherDate, VoucherNo, AccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["TransactionDate"] = reader["TransactionDate"];
                                row["TransactionType"] = reader["TransactionType"];
                                row["TransactionNo"] = reader["TransactionNo"];
                                row["AccountNo"] = reader["AccountNo"];
                                row["AccountName"] = reader["AccountName"];
                                row["Description"] = reader["Description"];
                                row["Debit"] = reader["Debit"];
                                row["Credit"] = reader["Credit"];
                                row["PartyName"] = reader["PartyName"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching transaction data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}