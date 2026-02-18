using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class General_Journal : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public General_Journal()
        {
            InitializeComponent();
        }

        private void General_Journal_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;
            fromvouchernotxtbox.Text = "0";
        }

        private void GenerateReport()
        {
            try
            {
                // Validate input
                if (!ValidateInput()) return;

                // Get data for report
                DataTable dt = GetGeneralJournalData(
                    fromdatepcker.Value,
                    todatepcker.Value,
                    GetIntValue(fromvouchernotxtbox.Text),
                    GetIntValue(tovouchernotxtbox.Text, int.MaxValue)
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No journal entries found for the selected criteria.", "Info",
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
                    //{ "ReportTitle", "General Journal Report" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Add voucher range parameters
                int fromVoucher = GetIntValue(fromvouchernotxtbox.Text);
                int toVoucher = GetIntValue(tovouchernotxtbox.Text, int.MaxValue);

                if (fromVoucher > 0)
                {
                    //parameters.Add("FromVoucherNo", fromVoucher.ToString());
                }

                if (toVoucher != int.MaxValue)
                {
                    //parameters.Add("ToVoucherNo", toVoucher.ToString());
                }

                // Show report viewer (universal one)
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("General_Journal", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetIntValue(string text, int defaultValue = 0)
        {
            return int.TryParse(text, out int result) ? result : defaultValue;
        }

        private bool ValidateInput()
        {
            // Check date range
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate voucher numbers
            int fromVoucher = GetIntValue(fromvouchernotxtbox.Text);
            int toVoucher = GetIntValue(tovouchernotxtbox.Text, int.MaxValue);

            if (fromVoucher > toVoucher && toVoucher != int.MaxValue)
            {
                MessageBox.Show("From Voucher cannot be greater than To Voucher.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private DataTable GetGeneralJournalData(DateTime fromDate, DateTime toDate, int fromVoucher, int toVoucher)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("VoucherNo", typeof(int));
            dt.Columns.Add("VoucherDate", typeof(DateTime));
            dt.Columns.Add("AccountNo", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("Narration", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("PartyName", typeof(string));
            dt.Columns.Add("Reference", typeof(string));

            try
            {
                string query = @"
                    SELECT 
                        jv.VoucherNo,
                        jv.VoucherDate,
                        jvb.AccountNo,
                        coa.AccountName,
                        jvb.Narration,
                        jvb.Debit,
                        jvb.Credit,
                        ISNULL(par.PartyName, '') as PartyName,
                        ISNULL(jvb.Refrence, '') as Reference
                    FROM JournalVouchers jv
                    INNER JOIN JournalVouchersBody jvb ON jv.VoucherNo = jvb.VoucherNo
                    INNER JOIN ChartOfAccounts coa ON jvb.AccountNo = coa.AccountNo
                    LEFT JOIN Parties par ON jvb.AccountNo = par.PartyID
                    WHERE jv.VoucherDate BETWEEN @FromDate AND @ToDate
                        AND jv.VoucherNo >= @FromVoucher";

                if (toVoucher != int.MaxValue)
                {
                    query += " AND jv.VoucherNo <= @ToVoucher";
                }

                query += " ORDER BY jv.VoucherDate, jv.VoucherNo, jvb.SerialNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);
                        cmd.Parameters.AddWithValue("@FromVoucher", fromVoucher);

                        if (toVoucher != int.MaxValue)
                        {
                            cmd.Parameters.AddWithValue("@ToVoucher", toVoucher);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["VoucherNo"] = reader["VoucherNo"];
                                row["VoucherDate"] = reader["VoucherDate"];
                                row["AccountNo"] = reader["AccountNo"];
                                row["AccountName"] = reader["AccountName"];
                                row["Narration"] = reader["Narration"];
                                row["Debit"] = reader["Debit"];
                                row["Credit"] = reader["Credit"];
                                row["PartyName"] = reader["PartyName"];
                                row["Reference"] = reader["Reference"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching journal data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();

        private void fromvouchernotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Allow only numbers
            if (!string.IsNullOrEmpty(fromvouchernotxtbox.Text) &&
                !int.TryParse(fromvouchernotxtbox.Text, out _))
            {
                MessageBox.Show("Please enter numbers only.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromvouchernotxtbox.Text = "";
            }
        }

        private void tovouchernotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Allow only numbers
            if (!string.IsNullOrEmpty(tovouchernotxtbox.Text) &&
                !int.TryParse(tovouchernotxtbox.Text, out _))
            {
                MessageBox.Show("Please enter numbers only.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tovouchernotxtbox.Text = "";
            }
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value;
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
    }
}