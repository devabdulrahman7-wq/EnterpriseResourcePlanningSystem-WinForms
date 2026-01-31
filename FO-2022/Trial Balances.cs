using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Trial_Balances : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime? fromDate = null;
        private DateTime? toDate = null;

        public Trial_Balances()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Set default radio buttons
            alldatebtn.Checked = true;
            showtransactionbtn.Checked = true;
            excludepartiesbtn.Checked = true;

            // Set default date for single date picker (need to add in designer)
            // singledatepicker.Value = DateTime.Now; // Uncomment if you have this control

            // Hide date range initially
            HideDateRangeControls();
        }

        private void HideDateRangeControls()
        {
            // If you have fromdatepicker and todatepicker controls, hide them
            // fromdatepicker.Visible = false;
            // todatepicker.Visible = false;
            // fromdatelabel.Visible = false;
            // todatelabel.Visible = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void alldatebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (alldatebtn.Checked)
            {
                fromDate = null;
                toDate = null;
                HideDateRangeControls();
                // singlesdatepicker.Visible = false; // Uncomment if you have this control
            }
        }

        private void daterangebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (daterangebtn.Checked)
            {
                ShowDateRangeDialog();
                // singlesdatepicker.Visible = false; // Uncomment if you have this control
            }
        }

        private void singledatebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (singledatebtn.Checked)
            {
                HideDateRangeControls();
                // singlesdatepicker.Visible = true; // Uncomment if you have this control
                toDate = DateTime.Now; // Or get from date picker
                fromDate = null;
            }
        }

        private void ShowDateRangeDialog()
        {
            using (var dateDialog = new Form())
            {
                dateDialog.Text = "Select Date Range";
                dateDialog.Size = new Size(350, 200);
                dateDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dateDialog.StartPosition = FormStartPosition.CenterParent;
                dateDialog.TopMost = true;
                var fromLabel = new Label { Text = "From Date:", Location = new Point(20, 30), Size = new Size(80, 25) };
                var fromPicker = new DateTimePicker { Location = new Point(100, 30), Size = new Size(150, 25) };

                var toLabel = new Label { Text = "To Date:", Location = new Point(20, 70), Size = new Size(80, 25) };
                var toPicker = new DateTimePicker { Location = new Point(100, 70), Size = new Size(150, 25) };

                var okButton = new Button { Text = "OK", Location = new Point(80, 120), Size = new Size(80, 30) };
                var cancelButton = new Button { Text = "Cancel", Location = new Point(170, 120), Size = new Size(80, 30) };

                okButton.Click += (s, ev) =>
                {
                    if (fromPicker.Value > toPicker.Value)
                    {
                        MessageBox.Show("From date cannot be greater than To date", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    fromDate = fromPicker.Value;
                    toDate = toPicker.Value;
                    dateDialog.DialogResult = DialogResult.OK;
                    dateDialog.Close();
                };

                cancelButton.Click += (s, ev) =>
                {
                    alldatebtn.Checked = true;
                    dateDialog.DialogResult = DialogResult.Cancel;
                    dateDialog.Close();
                };

                dateDialog.Controls.AddRange(new Control[] { fromLabel, fromPicker, toLabel, toPicker, okButton, cancelButton });

                if (dateDialog.ShowDialog() == DialogResult.OK)
                {
                    // Dates already set
                }
                else
                {
                    alldatebtn.Checked = true;
                }
            }
        }

        private DataTable GetTrialBalanceData()
        {
            DataTable dt = new DataTable();

            try
            {
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append(@"
                -- TRIAL BALANCE REPORT
                SELECT 
                    coa.AccountNo,
                    coa.AccountName,
                    coa.AccountType,
                    p.PartyType,
                    coa.OpeningDebit,
                    coa.OpeningCredit,
                    ISNULL(SUM(CASE WHEN av.VoucherDate < @FromDate THEN av.Debit ELSE 0 END), 0) AS OpeningPeriodDebit,
                    ISNULL(SUM(CASE WHEN av.VoucherDate < @FromDate THEN av.Credit ELSE 0 END), 0) AS OpeningPeriodCredit,
                    ISNULL(SUM(CASE WHEN av.VoucherDate BETWEEN @FromDate AND @ToDate THEN av.Debit ELSE 0 END), 0) AS TransactionDebit,
                    ISNULL(SUM(CASE WHEN av.VoucherDate BETWEEN @FromDate AND @ToDate THEN av.Credit ELSE 0 END), 0) AS TransactionCredit
                FROM [dbo].[ChartOfAccounts] coa
                LEFT JOIN [dbo].[Parties] p ON coa.AccountNo = p.PartyID
                LEFT JOIN [dbo].[AllVouchers] av ON coa.AccountNo = av.AccountNo
                WHERE coa.IsPosted = 1 ");

                // Set date parameters
                DateTime effectiveFromDate = fromDate ?? new DateTime(1900, 1, 1);
                DateTime effectiveToDate = toDate ?? DateTime.Now;

                // Filter by account type (Transaction/Group/Both)
                if (showtransactionbtn.Checked)
                {
                    queryBuilder.Append(" AND coa.IsDetailed = 1 ");
                }
                else if (showaccountgrpbtn.Checked)
                {
                    queryBuilder.Append(" AND coa.IsDetailed = 0 ");
                }
                // else showbothbtn - no filter

                // Filter by party type (Exclude/Include/Only)
                if (excludepartiesbtn.Checked)
                {
                    queryBuilder.Append(" AND p.PartyID IS NULL ");
                }
                else if (onlypartiesbtn.Checked)
                {
                    queryBuilder.Append(" AND p.PartyID IS NOT NULL ");
                }
                // else includepartiesbtn - no filter

                queryBuilder.Append(@"
                GROUP BY 
                    coa.AccountNo, 
                    coa.AccountName, 
                    coa.AccountType, 
                    coa.OpeningDebit, 
                    coa.OpeningCredit,
                    p.PartyType
                HAVING 
                    (coa.OpeningDebit + coa.OpeningCredit + 
                     ISNULL(SUM(CASE WHEN av.VoucherDate < @FromDate THEN av.Debit ELSE 0 END), 0) +
                     ISNULL(SUM(CASE WHEN av.VoucherDate < @FromDate THEN av.Credit ELSE 0 END), 0) +
                     ISNULL(SUM(CASE WHEN av.VoucherDate BETWEEN @FromDate AND @ToDate THEN av.Debit ELSE 0 END), 0) +
                     ISNULL(SUM(CASE WHEN av.VoucherDate BETWEEN @FromDate AND @ToDate THEN av.Credit ELSE 0 END), 0)) != 0
                ORDER BY coa.AccountType, coa.AccountNo");

                string query = queryBuilder.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", effectiveFromDate);
                    cmd.Parameters.AddWithValue("@ToDate", effectiveToDate);
                    cmd.CommandTimeout = 50;
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                // Add calculated columns
                dt.Columns.Add("TotalDebit", typeof(decimal));
                dt.Columns.Add("TotalCredit", typeof(decimal));
                dt.Columns.Add("BalanceDebit", typeof(decimal));
                dt.Columns.Add("BalanceCredit", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    decimal openingDebit = Convert.ToDecimal(row["OpeningDebit"]);
                    decimal openingCredit = Convert.ToDecimal(row["OpeningCredit"]);
                    decimal openingPeriodDebit = Convert.ToDecimal(row["OpeningPeriodDebit"]);
                    decimal openingPeriodCredit = Convert.ToDecimal(row["OpeningPeriodCredit"]);
                    decimal transactionDebit = Convert.ToDecimal(row["TransactionDebit"]);
                    decimal transactionCredit = Convert.ToDecimal(row["TransactionCredit"]);

                    // Total Debit = Opening + Opening Period + Transaction
                    decimal totalDebit = openingDebit + openingPeriodDebit + transactionDebit;

                    // Total Credit = Opening + Opening Period + Transaction
                    decimal totalCredit = openingCredit + openingPeriodCredit + transactionCredit;

                    // Balance calculation
                    decimal balance = totalDebit - totalCredit;

                    row["TotalDebit"] = totalDebit;
                    row["TotalCredit"] = totalCredit;
                    row["BalanceDebit"] = balance > 0 ? balance : 0;
                    row["BalanceCredit"] = balance < 0 ? Math.Abs(balance) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching trial balance: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // Validate date range
                if (daterangebtn.Checked && (!fromDate.HasValue || !toDate.HasValue))
                {
                    MessageBox.Show("Please select date range", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Set default dates if not set
                if (!fromDate.HasValue) fromDate = new DateTime(1900, 1, 1);
                if (!toDate.HasValue) toDate = DateTime.Now;

                // Get data
                DataTable dt = GetTrialBalanceData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No trial balance data found with current filters", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate totals
                decimal totalDebitBalance = 0;
                decimal totalCreditBalance = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalDebitBalance += Convert.ToDecimal(row["BalanceDebit"]);
                    totalCreditBalance += Convert.ToDecimal(row["BalanceCredit"]);
                }

                // Check if trial balance matches (Debit = Credit)
                bool isBalanced = Math.Abs(totalDebitBalance - totalCreditBalance) < 0.01m;

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Trial Balance Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                // Add period information
                if (alldatebtn.Checked)
                {
                    //parameters.Add("Period", "All Dates");
                }
                else if (singledatebtn.Checked)
                {
                    //parameters.Add("Period", $"As on: {toDate.Value.ToString("dd/MM/yyyy")}");
                }

                else if (daterangebtn.Checked && fromDate.HasValue && toDate.HasValue)
                {
                    //parameters.Add("Period", $"From {fromDate.Value.ToString("dd/MM/yyyy")} to {toDate.Value.ToString("dd/MM/yyyy")}");
                }

                // Add summary information
                //parameters.Add("TotalAccounts", dt.Rows.Count.ToString());
                //parameters.Add("TotalDebitBalance", totalDebitBalance.ToString("N2"));
                //parameters.Add("TotalCreditBalance", totalCreditBalance.ToString("N2"));
                //parameters.Add("IsBalanced", isBalanced.ToString());
                //parameters.Add("Difference", (totalDebitBalance - totalCreditBalance).ToString("N2"));

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Trial_Balance", dataSources);

                // Print mode specific logic (if needed)
                if (isPrintMode)
                {
                    // Add print-specific logic here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport(false); // Preview mode
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport(true); // Print mode
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Trial_Balances_Load(object sender, EventArgs e)
        {
            // Additional setup if needed
        }

        // Empty methods for designer events
        private void showtransactionbtn_CheckedChanged(object sender, EventArgs e) { }
        private void showaccountgrpbtn_CheckedChanged(object sender, EventArgs e) { }
        private void showbothbtn_CheckedChanged(object sender, EventArgs e) { }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { }
        private void excludepartiesbtn_CheckedChanged(object sender, EventArgs e) { }
        private void onlypartiesbtn_CheckedChanged(object sender, EventArgs e) { }
        private void includepartiesbtn_CheckedChanged(object sender, EventArgs e) { }
    }
}