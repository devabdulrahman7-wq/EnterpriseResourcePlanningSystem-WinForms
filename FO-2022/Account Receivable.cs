using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Account_Receivable : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Account_Receivable()
        {
            InitializeComponent();

            // Default dates (last month)
            fromdatepicker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;

            // Load categories
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                ctgrycombobox.Items.Clear();
                ctgrycombobox.Items.Add("All Categories");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT CategoryName FROM Category ORDER BY CategoryName";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ctgrycombobox.Items.Add(dr["CategoryName"].ToString());
                        }
                    }
                }
                ctgrycombobox.SelectedIndex = 0;
            }
            catch
            {
                ctgrycombobox.Items.Add("All Categories");
                ctgrycombobox.SelectedIndex = 0;
            }
        }

        // ⚡⚡⚡ MAIN BUTTON CLICK - SUPER SIMPLE ⚡⚡⚡
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // Step 1: Get data (FASTEST METHOD)
                DataTable reportData = GetSimpleData();

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Step 2: Show report
                ShowReport(reportData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ⭐⭐⭐ YE 3 METHODS TRY KARO - EK KAAM KAREGA ⭐⭐⭐

        // METHOD 1: AccountsBalances table se (FASTEST)
        private DataTable GetDataFromAccountsBalances()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // EXACT 4 COLUMNS jo tumhe chahiye
                string query = @"
                    SELECT 
                        AccountNo AS ACCOUNTNO,
                        AccountName AS ACCOUNTNAME,
                        ABS(Bal) AS CLOSINGBAL,
                        BalType AS CLOSINGBALTYPE
                    FROM AccountsBalances 
                    WHERE ABS(Bal) > 0
                    ORDER BY ABS(Bal) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 10;
                    con.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // METHOD 2: ChartOfAccounts se (SECOND OPTION)
        //private DataTable GetDataFromChartOfAccounts()
        //{
        //    DataTable dt = new DataTable();

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        string query = @"
        //            SELECT 
        //                AccountNo AS ACCOUNTNO,
        //                AccountName AS ACCOUNTNAME,
        //                ABS(AdjustedDebit - AdjustedCredit) AS CLOSINGBAL,
        //                CASE 
        //                    WHEN (AdjustedDebit - AdjustedCredit) >= 0 
        //                    THEN 'Dr' 
        //                    ELSE 'Cr' 
        //                END AS CLOSINGBALTYPE
        //            FROM ChartOfAccounts 
        //            WHERE IsDetailed = 1 
        //                AND AccountType LIKE '%Parties%'
        //                AND ABS(AdjustedDebit - AdjustedCredit) > 0
        //            ORDER BY ABS(AdjustedDebit - AdjustedCredit) DESC";

        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.CommandTimeout = 10;
        //            con.Open();

        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(dt);
        //            }
        //        }
        //    }

        //    return dt;
        //}
        private DataTable GetDataFromChartOfAccounts()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                AccountNo AS ACCOUNTNO,
                AccountName AS ACCOUNTNAME,
                (AdjustedDebit - AdjustedCredit) AS CLOSINGBAL,
                'Dr' AS CLOSINGBALTYPE
            FROM ChartOfAccounts 
            WHERE IsDetailed = 1
                AND (AdjustedDebit - AdjustedCredit) > 0   -- ⭐ RECEIVABLES ONLY
            ORDER BY (AdjustedDebit - AdjustedCredit) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandTimeout = 10;
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // METHOD 3: AllVouchers se calculate karo (THIRD OPTION)
        private DataTable GetDataFromAllVouchers()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //string query = @"
                //    SELECT 
                //        AccountNo AS ACCOUNTNO,
                //        MAX(AccountName) AS ACCOUNTNAME,
                //        ABS(SUM(Debit) - SUM(Credit)) AS CLOSINGBAL,
                //        CASE 
                //            WHEN SUM(Debit) - SUM(Credit) >= 0 
                //            THEN 'Dr' 
                //            ELSE 'Cr' 
                //        END AS CLOSINGBALTYPE
                //    FROM (
                //        SELECT 
                //            coa.AccountNo,
                //            coa.AccountName,
                //            av.Debit,
                //            av.Credit
                //        FROM ChartOfAccounts coa
                //        LEFT JOIN AllVouchers av ON coa.AccountNo = av.AccountNo
                //        WHERE coa.IsDetailed = 1 
                //            AND coa.AccountType LIKE '%Parties%'
                //            AND av.VoucherDate BETWEEN @FromDate AND @ToDate
                //    ) AS Data
                //    GROUP BY AccountNo
                //    HAVING ABS(SUM(Debit) - SUM(Credit)) > 0
                //    ORDER BY ABS(SUM(Debit) - SUM(Credit)) DESC";

                string query = @"
    SELECT 
    AccountNo AS ACCOUNTNO,
    MAX(AccountName) AS ACCOUNTNAME,
    SUM(Debit) - SUM(Credit) AS CLOSINGBAL,
    'Dr' AS CLOSINGBALTYPE
FROM (
    SELECT 
        coa.AccountNo,
        coa.AccountName,
        ISNULL(av.Debit, 0) AS Debit,
        ISNULL(av.Credit, 0) AS Credit
    FROM ChartOfAccounts coa
    LEFT JOIN AllVouchers av 
        ON coa.AccountNo = av.AccountNo
        AND av.VoucherDate BETWEEN @FromDate AND @ToDate
    WHERE coa.IsDetailed = 1 
      AND coa.AccountType LIKE '%Parties%'
) AS Data
GROUP BY AccountNo
HAVING (SUM(Debit) - SUM(Credit)) > 0
ORDER BY (SUM(Debit) - SUM(Credit)) DESC;
";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepicker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);
                    cmd.CommandTimeout = 10;
                    con.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // ⭐ MAIN METHOD JO SAB TRY KAREGA
        private DataTable GetSimpleData()
        {
            DataTable dt = new DataTable();

            // Try Method 1 first
            //try
            //{
            //    dt = GetDataFromAccountsBalances();
            //    if (dt.Rows.Count > 0) return dt;
            //}
            //catch { }

            //// Try Method 2
            //try
            //{
            //    dt = GetDataFromChartOfAccounts();
            //    if (dt.Rows.Count > 0) return dt;
            //}
            //catch { }

            // Try Method 3
            try
            {
                dt = GetDataFromAllVouchers();
                if (dt.Rows.Count > 0) return dt;
            }
            catch { }

            // Last resort: Dummy data
            return GetDataFromAllVouchers();
            //return GetDummyData();
        }

        private decimal GetMinBalance()
        {
            if (string.IsNullOrWhiteSpace(balancegreaterthantxtbox.Text))
                return 0;

            return decimal.TryParse(balancegreaterthantxtbox.Text, out decimal balance) ? Math.Abs(balance) : 0;
        }

        private void ShowReport(DataTable data)
        {
            try
            {
                // Apply balance filter
                decimal minBalance = GetMinBalance();
                if (minBalance > 0)
                {
                    DataTable filtered = data.Clone();
                    foreach (DataRow row in data.Rows)
                    {
                        decimal balance = Convert.ToDecimal(row["CLOSINGBAL"]);
                        if (balance >= minBalance)
                        {
                            filtered.ImportRow(row);
                        }
                    }
                    data = filtered;
                }

                if (data.Rows.Count == 0)
                {
                    MessageBox.Show($"No accounts with balance >= {minBalance:N2}", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report
                Dictionary<string, DataTable> sources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", data }
                };

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Accounts Receivable" },
                    //{ "FromDate", fromdatepicker.Value.ToString("dd/MM/yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "MinBalance", minBalance.ToString("N2") },
                    //{ "Category", ctgrycombobox.SelectedItem.ToString() },
                    //{ "TotalAccounts", data.Rows.Count.ToString() }
                };

                using (Report_viewer viewer = new Report_viewer())
                {
                    viewer.ShowReport("Account_Receivables", sources);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Report error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Dummy data for testing
        private DataTable GetDummyData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ACCOUNTNO", typeof(string));
            dt.Columns.Add("ACCOUNTNAME", typeof(string));
            dt.Columns.Add("CLOSINGBAL", typeof(decimal));
            dt.Columns.Add("CLOSINGBALTYPE", typeof(string));
            //Sample data
            dt.Rows.Add("622174", "Zulfiqar Traders Johi", 37380.00, "Dr");
            dt.Rows.Add("621396", "Zahid Somroo Kumb", 82148.00, "Dr");
            dt.Rows.Add("621335", "Zain Traders Gogra Morr", 44960.00, "Dr");
            dt.Rows.Add("622206", "Zain Traders Nowshera", 144365.00, "Dr");
            dt.Rows.Add("622298", "Zainab Traders Kotri", 241240.00, "Dr");
            dt.Rows.Add("622110", "Zaryab Trader Sanghi (Chutala)", 254943.00, "Dr");
            dt.Rows.Add("622150", "Zeeshan Traders Chichawatni", 198040.00, "Dr");
            dt.Rows.Add("622268", "Zia ul Hasan Wapda Town Dasti", 90000.00, "Dr");
            dt.Rows.Add("622418", "Zulfiqar Murree Tando Ghulam A", 87260.00, "Dr");
            dt.Rows.Add("622174", "Zulfiqar Traders Johi", 37380.00, "Dr");
            dt.Rows.Add("621954", "Zunaisha Traders Head Sidhni", 382505.00, "Dr");
            dt.Rows.Add("62174", "Zaidi Traders Bakhar", 8579041.00, "Dr");
            dt.Rows.Add("622357", "Zain Traders Jalalpur Jattan", 390520.00, "Dr");
            dt.Rows.Add("622153", "Zain Traders Shikarpur", 305810.00, "Dr");
            dt.Rows.Add("622269", "Zam Zam Traders Rohri", 117375.00, "Dr");
            dt.Rows.Add("621407", "Zeeshan Taza Garam Chakdara", 1859578.00, "Dr");
            dt.Rows.Add("622219", "Zia Traders Abbottabad", 628194.00, "Dr");
            dt.Rows.Add("622290", "Zubair Traders Khudian Khas", 240290.00, "Dr");
            dt.Rows.Add("621946", "Zulfiqar Traders Balakot", 794681.00, "Dr");
            dt.Rows.Add("622165", "Zulfiqar Traders Peer Mahal", 34740.00, "Dr");
            dt.Rows.Add("621896", "Zurriat Traders Dhrmal", 3.00, "Dr");
            return dt;
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void baltxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
        }

        private void Account_Receivable_Load(object sender, EventArgs e)
        {

        }
    }
}