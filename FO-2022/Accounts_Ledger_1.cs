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
    public partial class Accounts_Ledger_1 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime? selectedFromDate = null;
        private DateTime? selectedToDate = null;
        private string selectedAccountNo = "";

        public Accounts_Ledger_1()
        {
            InitializeComponent();
        }
        private void Accounts_Ledger_1_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default radio button
                alldateradiobtn.Checked = true;

                // Clear fields
                accountnametxtbox.Clear();
                accountnotxtbox.Clear();

                // Disable other radio buttons initially
                datebwradiobtn.Enabled = true;
                dayradiobtn.Enabled = true;
                todayradiobtn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Search Account
        private void searchaccountno_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(accountnotxtbox.Text))
            {
                MessageBox.Show("Please enter Account No.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text.Trim());

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            accountnametxtbox.Text = result.ToString();
                            selectedAccountNo = accountnotxtbox.Text.Trim();
                        }
                        else
                        {
                            accountnametxtbox.Clear();
                            selectedAccountNo = "";
                            MessageBox.Show("Account not found.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching account: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear name when Account No changes
            if (string.IsNullOrWhiteSpace(accountnotxtbox.Text))
            {
                accountnametxtbox.Clear();
                selectedAccountNo = "";
            }
        }

        // Radio Button Events
        private void alldateradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (alldateradiobtn.Checked)
            {
                // No date filter - show all data
                selectedFromDate = null;
                selectedToDate = null;
            }
        }

        private void datebwradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (datebwradiobtn.Checked)
            {
                using (Form popup = new Form())
                {
                    popup.Text = "Select Date Range";
                    popup.TopMost = true;
                    popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                    popup.StartPosition = FormStartPosition.CenterParent;
                    popup.Size = new Size(320, 180);

                    DateTimePicker dtpFrom = new DateTimePicker { Location = new Point(100, 20), Width = 200 };
                    DateTimePicker dtpTo = new DateTimePicker { Location = new Point(100, 60), Width = 200 };
                    Button btnOK = new Button { Text = "OK", Location = new Point(100, 100), Width = 80 };
                    Button btnCancel = new Button { Text = "Cancel", Location = new Point(190, 100), Width = 80 };

                    dtpFrom.Value = DateTime.Today.AddMonths(-1);
                    dtpTo.Value = DateTime.Today;

                    btnOK.Click += (s, args) => {
                        if (dtpFrom.Value > dtpTo.Value)
                        {
                            MessageBox.Show("From date cannot be later than To date.");
                            return;
                        }
                        selectedFromDate = dtpFrom.Value;
                        selectedToDate = dtpTo.Value;
                        popup.DialogResult = DialogResult.OK;
                        popup.Close();
                    };

                    btnCancel.Click += (s, args) => {
                        alldateradiobtn.Checked = true;
                        popup.DialogResult = DialogResult.Cancel;
                        popup.Close();
                    };

                    popup.Controls.Add(new Label { Text = "From:", Location = new Point(20, 24) });
                    popup.Controls.Add(new Label { Text = "To:", Location = new Point(20, 64) });
                    popup.Controls.Add(dtpFrom);
                    popup.Controls.Add(dtpTo);
                    popup.Controls.Add(btnOK);
                    popup.Controls.Add(btnCancel);

                    if (popup.ShowDialog() != DialogResult.OK)
                    {
                        alldateradiobtn.Checked = true;
                    }
                }
            }
        }

        private void dayradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (dayradiobtn.Checked)
            {
                using (Form popup = new Form())
                {
                    popup.Text = "Select Date";
                    popup.TopMost = true;
                    popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                    popup.StartPosition = FormStartPosition.CenterParent;
                    popup.Size = new Size(320, 160);

                    DateTimePicker dtpDate = new DateTimePicker { Location = new Point(100, 30), Width = 200 };
                    Button btnOK = new Button { Text = "OK", Location = new Point(100, 80), Width = 80 };
                    Button btnCancel = new Button { Text = "Cancel", Location = new Point(190, 80), Width = 80 };

                    dtpDate.Value = DateTime.Today;

                    btnOK.Click += (s, args) => {
                        selectedFromDate = dtpDate.Value;
                        selectedToDate = dtpDate.Value;
                        popup.DialogResult = DialogResult.OK;
                        popup.Close();
                    };

                    btnCancel.Click += (s, args) => {
                        alldateradiobtn.Checked = true;
                        popup.DialogResult = DialogResult.Cancel;
                        popup.Close();
                    };

                    popup.Controls.Add(new Label { Text = "Date:", Location = new Point(20, 34) });
                    popup.Controls.Add(dtpDate);
                    popup.Controls.Add(btnOK);
                    popup.Controls.Add(btnCancel);

                    if (popup.ShowDialog() != DialogResult.OK)
                    {
                        alldateradiobtn.Checked = true;
                    }
                }
            }
        }

        private void todayradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (todayradiobtn.Checked)
            {
                // Set dates to today
                selectedFromDate = DateTime.Today;
                selectedToDate = DateTime.Today;
            }
        }

        // Validate before generating report
        private bool ValidateInput()
        {
            // Check if account is selected
            if (string.IsNullOrWhiteSpace(selectedAccountNo))
            {
                MessageBox.Show("Please search and select an Account first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                accountnotxtbox.Focus();
                return false;
            }

            // Check if account name is fetched
            if (string.IsNullOrWhiteSpace(accountnametxtbox.Text))
            {
                MessageBox.Show("Please validate Account No by clicking Search.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                searchaccountno.Focus();
                return false;
            }

            return true;
        }

        // Generate Report
        private void GenerateReport()
        {
            try
            {
                // Validate input
                if (!ValidateInput()) return;

                // Get data for report
                DataTable dt = GetLedgerData(
                    selectedAccountNo,
                    selectedFromDate,
                    selectedToDate
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No ledger entries found for the selected criteria.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "AccountNo", selectedAccountNo },
                    //{ "AccountName", accountnametxtbox.Text },
                    //{ "ReportTitle", "Account Ledger" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Add date range parameters if applicable
                if (selectedFromDate.HasValue && selectedToDate.HasValue)
                {
                    //parameters.Add("FromDate", selectedFromDate.Value.ToString("dd/MM/yyyy"));
                    //parameters.Add("ToDate", selectedToDate.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    //parameters.Add("FromDate", "All Dates");
                    //parameters.Add("ToDate", "All Dates");
                }

                // Show report viewer
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Accounts_Ledger_1", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetLedgerData(string accountNo, DateTime? fromDate, DateTime? toDate)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("EntryNo", typeof(int));
            dt.Columns.Add("VoucherDate", typeof(DateTime));
            dt.Columns.Add("VoucherType", typeof(string));
            dt.Columns.Add("VoucherNo", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("Balance", typeof(decimal));
            dt.Columns.Add("BalanceType", typeof(string));
            dt.Columns.Add("PartyName", typeof(string));
            dt.Columns.Add("Narration", typeof(string));
            dt.Columns.Add("ReferenceNo", typeof(string));

            try
            {
                // Get opening balance from AccountsBalances
                decimal openingBalance = 0;
                string openingBalanceType = "Dr"; // Default

                string openingQuery = @"
                    SELECT OpeningBal, OpeningBalType 
                    FROM AccountsBalances 
                    WHERE AccountNo = @AccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get opening balance
                    using (SqlCommand cmd = new SqlCommand(openingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                openingBalance = Convert.ToDecimal(reader["OpeningBal"]);
                                openingBalanceType = reader["OpeningBalType"].ToString();
                            }
                        }
                    }

                    // Add opening balance as first row
                    DataRow openingRow = dt.NewRow();
                    openingRow["EntryNo"] = 0;
                    openingRow["VoucherDate"] = fromDate.HasValue ? fromDate.Value : DateTime.MinValue;
                    openingRow["VoucherType"] = "OB";
                    openingRow["VoucherNo"] = 0;
                    openingRow["Description"] = "Opening Balance";
                    openingRow["Debit"] = openingBalanceType == "Dr" ? openingBalance : 0;
                    openingRow["Credit"] = openingBalanceType == "Cr" ? openingBalance : 0;
                    openingRow["Balance"] = openingBalance;
                    openingRow["BalanceType"] = openingBalanceType;
                    openingRow["PartyName"] = "";
                    openingRow["Narration"] = "";
                    openingRow["ReferenceNo"] = "";
                    dt.Rows.Add(openingRow);

                    // Get ledger transactions from AllVouchers
                    string transactionQuery = @"
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY VoucherDate, VoucherNo) as EntryNo,
                            VoucherDate,
                            VoucherType,
                            VoucherNo,
                            Narration,
                            Debit,
                            Credit,
                            AccountNo,
                            CASE 
                                WHEN VoucherType = 'JV' THEN 'Journal Voucher'
                                WHEN VoucherType = 'BV' THEN 'Bank Voucher'
                                WHEN VoucherType = 'CR' THEN 'Credit Voucher'
                                WHEN VoucherType = 'DR' THEN 'Debit Voucher'
                                WHEN VoucherType = 'CP' THEN 'Cash Payment'
                                WHEN VoucherType = 'CR' THEN 'Cash Receipt'
                                ELSE VoucherType
                            END as VoucherTypeDesc
                        FROM AllVouchers 
                        WHERE AccountNo = @AccountNo";

                    // Add date filter if applicable
                    if (fromDate.HasValue && toDate.HasValue)
                    {
                        transactionQuery += " AND VoucherDate BETWEEN @FromDate AND @ToDate";
                    }

                    transactionQuery += " ORDER BY VoucherDate, VoucherNo";

                    using (SqlCommand cmd = new SqlCommand(transactionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);

                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);
                            cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            decimal runningBalance = openingBalance;
                            string runningBalanceType = openingBalanceType;

                            while (reader.Read())
                            {
                                decimal debit = Convert.ToDecimal(reader["Debit"]);
                                decimal credit = Convert.ToDecimal(reader["Credit"]);

                                // Calculate running balance
                                if (runningBalanceType == "Dr")
                                {
                                    runningBalance = runningBalance + debit - credit;
                                    if (runningBalance < 0)
                                    {
                                        runningBalance = Math.Abs(runningBalance);
                                        runningBalanceType = "Cr";
                                    }
                                }
                                else // "Cr"
                                {
                                    runningBalance = runningBalance + credit - debit;
                                    if (runningBalance < 0)
                                    {
                                        runningBalance = Math.Abs(runningBalance);
                                        runningBalanceType = "Dr";
                                    }
                                }

                                DataRow row = dt.NewRow();
                                row["EntryNo"] = reader["EntryNo"];
                                row["VoucherDate"] = reader["VoucherDate"];
                                row["VoucherType"] = reader["VoucherTypeDesc"];
                                row["VoucherNo"] = reader["VoucherNo"];
                                row["Description"] = reader["VoucherTypeDesc"] + " #" + reader["VoucherNo"];
                                row["Debit"] = debit;
                                row["Credit"] = credit;
                                row["Balance"] = runningBalance;
                                row["BalanceType"] = runningBalanceType;
                                row["PartyName"] = GetPartyName(conn, reader["VoucherNo"].ToString(), reader["VoucherType"].ToString());
                                row["Narration"] = reader["Narration"];
                                row["ReferenceNo"] = reader["VoucherNo"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching ledger data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private string GetPartyName(SqlConnection conn, string voucherNo, string voucherType)
        {
            try
            {
                string query = "";

                switch (voucherType)
                {
                    case "JV":
                        query = @"SELECT TOP 1 p.PartyName 
                                 FROM JournalVouchersBody jvb
                                 LEFT JOIN Parties p ON jvb.AccountNo = p.PartyID
                                 WHERE jvb.VoucherNo = @VoucherNo 
                                   AND jvb.AccountNo != @MainAccountNo";
                        break;
                    case "BV":
                        query = @"SELECT TOP 1 p.PartyName 
                                 FROM BankVouchersBody bvb
                                 LEFT JOIN Parties p ON bvb.AccountNo = p.PartyID
                                 WHERE bvb.VoucherNo = @VoucherNo";
                        break;
                    case "CR":
                        query = @"SELECT TOP 1 p.PartyName 
                                 FROM CreditVouchers cv
                                 LEFT JOIN Parties p ON cv.VoucherNo = p.PartyID
                                 WHERE cv.VoucherNo = @VoucherNo";
                        break;
                    case "DR":
                        query = @"SELECT TOP 1 p.PartyName 
                                 FROM DebitVouchers dv
                                 LEFT JOIN Parties p ON dv.VoucherNo = p.PartyID
                                 WHERE dv.VoucherNo = @VoucherNo";
                        break;
                }

                if (!string.IsNullOrEmpty(query))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                        cmd.Parameters.AddWithValue("@MainAccountNo", selectedAccountNo);

                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "";
                    }
                }
            }
            catch
            {
                // Silent fail
            }

            return "";
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
    }
}