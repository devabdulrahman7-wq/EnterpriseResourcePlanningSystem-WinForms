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
    public partial class Bank_Statement : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string currentAccountNo = "";
        private string currentAccountName = "";

        public Bank_Statement()
        {
            InitializeComponent();
            SetDefaultDates();
            checkBox1.Checked = true; // Default checked
        }

        private void SetDefaultDates()
        {
            // Set to current month
            fromdatepcker.Value = new DateTime(2022,01,01);
            todatepcker.Value = new DateTime(2023, 01, 01);
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }

        private void accountnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear account name when account number changes
            if (!string.IsNullOrEmpty(currentAccountNo) && accountnotxtbox.Text != currentAccountNo)
            {
                accountnametxtbox.Clear();
                currentAccountName = "";
                currentAccountNo = "";
            }
        }

        private void searchaccountno_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(accountnotxtbox.Text))
            {
                MessageBox.Show("Please enter Account Number", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                accountnotxtbox.Focus();
                return;
            }

            try
            {
                string query = @"
                SELECT 
                    AccountNo,
                    AccountName,
                    AccountType
                FROM [dbo].[ChartOfAccounts] 
                WHERE AccountNo = @AccountNo 
                    AND AccountType LIKE '%Bank%'"; // Only bank accounts

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text.Trim());

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        currentAccountNo = reader["AccountNo"].ToString();
                        currentAccountName = reader["AccountName"].ToString();
                        accountnametxtbox.Text = currentAccountName;

                        MessageBox.Show($"Bank account found: {currentAccountName}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Account not found or not a Bank Account", "Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        accountnametxtbox.Clear();
                        currentAccountNo = "";
                        currentAccountName = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching account: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field, so no action needed
        }

        private DataTable GetBankStatementData()
        {
            DataTable dt = new DataTable();

            try
            {
                if (string.IsNullOrEmpty(currentAccountNo))
                {
                    MessageBox.Show("Please search and select a bank account first", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return dt;
                }

                // Build dynamic SQL query based on checkbox selection
                StringBuilder queryBuilder = new StringBuilder();

                // OPENING BALANCE
                queryBuilder.Append(@"
                -- OPENING BALANCE (Before From Date)
                SELECT 
                    'OPENING BALANCE' AS TransactionType,
                    NULL AS VoucherNo,
                    @FromDate AS TransactionDate,
                    'Opening Balance' AS Description,
                    (SELECT ISNULL(SUM(Debit - Credit), 0) 
                     FROM [dbo].[AllVouchers] 
                     WHERE AccountNo = @AccountNo 
                       AND VoucherDate < @FromDate) AS Debit,
                    0 AS Credit,
                    (SELECT ISNULL(SUM(Debit - Credit), 0) 
                     FROM [dbo].[AllVouchers] 
                     WHERE AccountNo = @AccountNo 
                       AND VoucherDate < @FromDate) AS Balance,
                    'OB' AS VoucherTypeCode
                ");

                // BANK VOUCHERS (Always included)
                queryBuilder.Append(@"
                UNION ALL
                
                -- BANK VOUCHERS
                SELECT 
                    'BANK VOUCHER' AS TransactionType,
                    CAST(bv.VoucherNo AS VARCHAR(20)) AS VoucherNo,
                    bv.VoucherDate AS TransactionDate,
                    bvb.Narration AS Description,
                    bvb.Debit,
                    bvb.Credit,
                    0 AS Balance, -- Will calculate running balance in report
                    'BV' AS VoucherTypeCode
                FROM [dbo].[BankVouchers] bv
                INNER JOIN [dbo].[BankVouchersBody] bvb ON bv.VoucherNo = bvb.VoucherNo
                WHERE bvb.AccountNo = @AccountNo
                    AND bv.VoucherDate BETWEEN @FromDate AND @ToDate
                    AND bv.IsPosted = 1
                ");

                // Include other voucher types based on checkbox
                if (checkBox1.Checked)
                {
                    // JOURNAL VOUCHERS
                    queryBuilder.Append(@"
                    UNION ALL
                    
                    -- JOURNAL VOUCHERS
                    SELECT 
                        'JOURNAL VOUCHER' AS TransactionType,
                        CAST(jv.VoucherNo AS VARCHAR(20)) AS VoucherNo,
                        jv.VoucherDate AS TransactionDate,
                        jvb.Narration AS Description,
                        jvb.Debit,
                        jvb.Credit,
                        0 AS Balance,
                        'JV' AS VoucherTypeCode
                    FROM [dbo].[JournalVouchers] jv
                    INNER JOIN [dbo].[JournalVouchersBody] jvb ON jv.VoucherNo = jvb.VoucherNo
                    WHERE jvb.AccountNo = @AccountNo
                        AND jv.VoucherDate BETWEEN @FromDate AND @ToDate
                        AND jv.IsPosted = 1
                    ");

                    // DEBIT VOUCHERS
                    queryBuilder.Append(@"
                    UNION ALL
                    
                    -- DEBIT VOUCHERS
                    SELECT 
                        'DEBIT VOUCHER' AS TransactionType,
                        CAST(dv.VoucherNo AS VARCHAR(20)) AS VoucherNo,
                        dv.VoucherDate AS TransactionDate,
                        dvb.Narration AS Description,
                        dvb.Amount AS Debit,
                        0 AS Credit,
                        0 AS Balance,
                        'DV' AS VoucherTypeCode
                    FROM [dbo].[DebitVouchers] dv
                    INNER JOIN [dbo].[DebitVouchersBody] dvb ON dv.VoucherNo = dvb.VoucherNo
                    WHERE dvb.AccountNo = @AccountNo
                        AND dv.VoucherDate BETWEEN @FromDate AND @ToDate
                        AND dv.IsPosted = 1
                    ");

                    // CREDIT VOUCHERS
                    queryBuilder.Append(@"
                    UNION ALL
                    
                    -- CREDIT VOUCHERS
                    SELECT 
                        'CREDIT VOUCHER' AS TransactionType,
                        CAST(cv.VoucherNo AS VARCHAR(20)) AS VoucherNo,
                        cv.VoucherDate AS TransactionDate,
                        cvb.Narration AS Description,
                        0 AS Debit,
                        cvb.Amount AS Credit,
                        0 AS Balance,
                        'CV' AS VoucherTypeCode
                    FROM [dbo].[CreditVouchers] cv
                    INNER JOIN [dbo].[CreditVouchersBody] cvb ON cv.VoucherNo = cvb.VoucherNo
                    WHERE cvb.AccountNo = @AccountNo
                        AND cv.VoucherDate BETWEEN @FromDate AND @ToDate
                        AND cv.IsPosted = 1
                    ");

                    // BANK DEPOSITS
                    queryBuilder.Append(@"
                    UNION ALL
                    
                    -- BANK DEPOSITS
                    SELECT 
                        'BANK DEPOSIT' AS TransactionType,
                        CAST(VoucherNo AS VARCHAR(20)) AS VoucherNo,
                        VoucherDate AS TransactionDate,
                        Narration AS Description,
                        Amount AS Debit,
                        0 AS Credit,
                        0 AS Balance,
                        'BD' AS VoucherTypeCode
                    FROM [dbo].[BankDeposit]
                    WHERE BankAccountNo = @AccountNo
                        AND VoucherDate BETWEEN @FromDate AND @ToDate
                        AND IsPosted = 1
                    ");

                    // BANK CHEQUES
                    queryBuilder.Append(@"
                    UNION ALL
                    
                    -- BANK CHEQUES
                    SELECT 
                        'BANK CHEQUE' AS TransactionType,
                        CAST(VoucherNo AS VARCHAR(20)) AS VoucherNo,
                        VoucherDate AS TransactionDate,
                        Narration AS Description,
                        0 AS Debit,
                        Amount AS Credit,
                        0 AS Balance,
                        'BC' AS VoucherTypeCode
                    FROM [dbo].[BankCheques]
                    WHERE BankAccountNo = @AccountNo
                        AND VoucherDate BETWEEN @FromDate AND @ToDate
                        AND IsPosted = 1
                    ");
                }

                // ALL VOUCHERS (For transactions not captured above)
                queryBuilder.Append(@"
                UNION ALL
                
                -- ALL VOUCHERS (General transactions)
                SELECT 
                    CASE 
                        WHEN VoucherType = 'RV' THEN 'RECEIPT VOUCHER'
                        WHEN VoucherType = 'PV' THEN 'PAYMENT VOUCHER'
                        WHEN VoucherType = 'JV' THEN 'JOURNAL VOUCHER'
                        WHEN VoucherType = 'CV' THEN 'CASH VOUCHER'
                        WHEN VoucherType = 'DV' THEN 'DEBIT VOUCHER'
                        ELSE VoucherType
                    END AS TransactionType,
                    CAST(VoucherNo AS VARCHAR(20)) AS VoucherNo,
                    VoucherDate AS TransactionDate,
                    Narration AS Description,
                    Debit,
                    Credit,
                    0 AS Balance,
                    VoucherType AS VoucherTypeCode
                FROM [dbo].[AllVouchers]
                WHERE AccountNo = @AccountNo
                    AND VoucherDate BETWEEN @FromDate AND @ToDate
                    AND VoucherType NOT IN ('BV', 'JV', 'DV', 'CV') -- Exclude already included
                ");

                // CLOSING BALANCE
                queryBuilder.Append(@"
                UNION ALL
                
                -- CLOSING BALANCE
                SELECT 
                    'CLOSING BALANCE' AS TransactionType,
                    NULL AS VoucherNo,
                    @ToDate AS TransactionDate,
                    'Closing Balance' AS Description,
                    0 AS Debit,
                    0 AS Credit,
                    (SELECT ISNULL(SUM(Debit - Credit), 0) 
                     FROM [dbo].[AllVouchers] 
                     WHERE AccountNo = @AccountNo 
                       AND VoucherDate <= @ToDate) AS Balance,
                    'CB' AS VoucherTypeCode
                ");

                // Final ORDER BY
                queryBuilder.Append(@"
                ORDER BY 
                    CASE 
                        WHEN TransactionType = 'OPENING BALANCE' THEN 1
                        WHEN TransactionType = 'CLOSING BALANCE' THEN 999
                        ELSE 2
                    END,
                    TransactionDate,
                    VoucherTypeCode
                ");

                string query = queryBuilder.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", currentAccountNo);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1)); // Include entire day

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching bank statement: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(currentAccountNo))
                {
                    MessageBox.Show("Please search and select a bank account first", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchaccountno.Focus();
                    return;
                }

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetBankStatementData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions found for selected period", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Bank Statement");
                //parameters.Add("AccountNo", currentAccountNo);
                //parameters.Add("AccountName", currentAccountName);
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                //parameters.Add("IncludeOtherVouchers", checkBox1.Checked.ToString());

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Bank_Statement", dataSources);

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
    }
}