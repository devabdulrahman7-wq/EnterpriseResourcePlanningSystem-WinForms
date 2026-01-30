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
    public partial class Bank_Marup_Calculator : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string currentAccountNo = "";
        private string currentAccountName = "";

        public Bank_Marup_Calculator()
        {
            InitializeComponent();
            SetDefaultDates();
            SetDefaultValues();
        }

        private void SetDefaultDates()
        {
            // Set to current month
            fromdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            todatepcker.Value = DateTime.Now.Date;
        }

        private void SetDefaultValues()
        {
            // Set default values for calculations
            daytxtbox.Text = "30";
            ratetxtbox.Text = "10"; // 10% default
            ratiotxtbox.Text = "1"; // 1 for full year
            amounttxtbox.Text = "0";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // This is for reference if needed
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
            CalculateDays();
            CalculateMarkup();
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            CalculateDays();
            CalculateMarkup();
        }

        private void banknotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear account name when account number changes
            if (!string.IsNullOrEmpty(currentAccountNo) && banknotxtbox.Text != currentAccountNo)
            {
                banknametxtbox.Clear();
                currentAccountName = "";
                currentAccountNo = "";
            }
        }

        private void banknametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }

        private void ratiotxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateMarkup();
        }

        private void amounttxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateMarkup();
        }

        private void daytxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateMarkup();
        }

        private void ratetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateMarkup();
        }

        private void Bank_Marup_Calculator_Load(object sender, EventArgs e)
        {
            // Load any initial data if needed
        }

        private void CalculateDays()
        {
            // Calculate days between from and to date
            if (fromdatepcker.Value <= todatepcker.Value)
            {
                TimeSpan daysDifference = todatepcker.Value - fromdatepcker.Value;
                daytxtbox.Text = (daysDifference.Days + 1).ToString(); // +1 to include both dates
            }
        }

        private void CalculateMarkup()
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(amounttxtbox.Text) ||
                    string.IsNullOrWhiteSpace(ratetxtbox.Text) ||
                    string.IsNullOrWhiteSpace(daytxtbox.Text) ||
                    string.IsNullOrWhiteSpace(ratiotxtbox.Text))
                {
                    return;
                }

                decimal amount = Convert.ToDecimal(amounttxtbox.Text);
                decimal rate = Convert.ToDecimal(ratetxtbox.Text);
                int days = Convert.ToInt32(daytxtbox.Text);
                decimal ratio = Convert.ToDecimal(ratiotxtbox.Text);

                // Calculate markup using formula: (Amount * Rate * Days * Ratio) / (365 * 100)
                decimal markupAmount = (amount * rate * days * ratio) / (36500);

                // Display calculation details in a label or textbox (optional)
                // You can add a label to show the calculated markup
            }
            catch (FormatException)
            {
                // Invalid number format
            }
            catch (Exception)
            {
                // Other errors
            }
        }

        private void SearchBankAccount()
        {
            if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
            {
                MessageBox.Show("Please enter Bank Account Number", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                banknotxtbox.Focus();
                return;
            }

            try
            {
                string query = @"
                SELECT 
                    AccountNo,
                    AccountName,
                    AccountType,
                    OpeningDebit,
                    OpeningCredit,
                    (OpeningDebit - OpeningCredit) AS OpeningBalance
                FROM [dbo].[ChartOfAccounts] 
                WHERE AccountNo = @AccountNo 
                    AND (AccountType LIKE '%Bank%' OR AccountType LIKE '%Account%')";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        currentAccountNo = reader["AccountNo"].ToString();
                        currentAccountName = reader["AccountName"].ToString();
                        banknametxtbox.Text = currentAccountName;

                        // Get average balance for the period to pre-fill amount
                        GetAverageBalanceForPeriod();
                    }
                    else
                    {
                        MessageBox.Show("Bank Account not found", "Not Found",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        banknametxtbox.Clear();
                        currentAccountNo = "";
                        currentAccountName = "";
                        amounttxtbox.Text = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching bank account: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetAverageBalanceForPeriod()
        {
            try
            {
                string query = @"
                -- Calculate average balance for the selected period
                WITH DailyBalances AS (
                    SELECT 
                        VoucherDate,
                        SUM(Debit - Credit) OVER (ORDER BY VoucherDate) AS RunningBalance
                    FROM [dbo].[AllVouchers]
                    WHERE AccountNo = @AccountNo
                        AND VoucherDate BETWEEN @FromDate AND @ToDate
                    GROUP BY VoucherDate, Debit, Credit
                )
                SELECT 
                    ISNULL(AVG(RunningBalance), 0) AS AverageBalance,
                    ISNULL(SUM(CASE WHEN RunningBalance > 0 THEN RunningBalance ELSE 0 END), 0) AS TotalPositiveBalance,
                    ISNULL(SUM(CASE WHEN RunningBalance < 0 THEN RunningBalance ELSE 0 END), 0) AS TotalNegativeBalance
                FROM DailyBalances";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", currentAccountNo);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        decimal averageBalance = Convert.ToDecimal(reader["AverageBalance"]);
                        amounttxtbox.Text = Math.Abs(averageBalance).ToString("N2");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating average balance: {ex.Message}", "Calculation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataTable GetBankMarkupData()
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

                // Validate inputs
                if (!decimal.TryParse(amounttxtbox.Text, out decimal principalAmount) ||
                    !decimal.TryParse(ratetxtbox.Text, out decimal markupRate) ||
                    !int.TryParse(daytxtbox.Text, out int markupDays) ||
                    !decimal.TryParse(ratiotxtbox.Text, out decimal ratio))
                {
                    MessageBox.Show("Please enter valid numeric values for calculation", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return dt;
                }

                // Calculate different markup scenarios
                decimal markupAmount = (principalAmount * markupRate * markupDays * ratio) / (36500);
                decimal markupAmountMonthly = (principalAmount * markupRate * 30 * ratio) / (36500);
                decimal markupAmountQuarterly = (principalAmount * markupRate * 90 * ratio) / (36500);
                decimal markupAmountHalfYearly = (principalAmount * markupRate * 180 * ratio) / (36500);
                decimal markupAmountYearly = (principalAmount * markupRate * 365 * ratio) / (36500);

                // Calculate compound interest if needed (optional)
                decimal totalAmount = principalAmount + markupAmount;

                // Get account details
                string query = @"
                SELECT 
                    AccountNo,
                    AccountName,
                    AccountType,
                    OpeningDebit,
                    OpeningCredit,
                    (OpeningDebit - OpeningCredit) AS OpeningBalance
                FROM [dbo].[ChartOfAccounts] 
                WHERE AccountNo = @AccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", currentAccountNo);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    // Add calculated columns to DataTable
                    dt.Columns.Add("FromDate", typeof(DateTime));
                    dt.Columns.Add("ToDate", typeof(DateTime));
                    dt.Columns.Add("MarkupDays", typeof(int));
                    dt.Columns.Add("MarkupRate", typeof(decimal));
                    dt.Columns.Add("Ratio", typeof(decimal));
                    dt.Columns.Add("PrincipalAmount", typeof(decimal));
                    dt.Columns.Add("MarkupAmount", typeof(decimal));
                    dt.Columns.Add("TotalAmount", typeof(decimal));
                    dt.Columns.Add("MonthlyMarkup", typeof(decimal));
                    dt.Columns.Add("QuarterlyMarkup", typeof(decimal));
                    dt.Columns.Add("HalfYearlyMarkup", typeof(decimal));
                    dt.Columns.Add("YearlyMarkup", typeof(decimal));

                    // Add calculated values to each row
                    foreach (DataRow row in dt.Rows)
                    {
                        row["FromDate"] = fromdatepcker.Value;
                        row["ToDate"] = todatepcker.Value;
                        row["MarkupDays"] = markupDays;
                        row["MarkupRate"] = markupRate;
                        row["Ratio"] = ratio;
                        row["PrincipalAmount"] = principalAmount;
                        row["MarkupAmount"] = markupAmount;
                        row["TotalAmount"] = totalAmount;
                        row["MonthlyMarkup"] = markupAmountMonthly;
                        row["QuarterlyMarkup"] = markupAmountQuarterly;
                        row["HalfYearlyMarkup"] = markupAmountHalfYearly;
                        row["YearlyMarkup"] = markupAmountYearly;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching bank markup data: {ex.Message}", "Database Error",
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
                    return;
                }

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(amounttxtbox.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid positive amount", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    amounttxtbox.Focus();
                    return;
                }

                if (!decimal.TryParse(ratetxtbox.Text, out decimal rate) || rate <= 0)
                {
                    MessageBox.Show("Please enter a valid markup rate", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ratetxtbox.Focus();
                    return;
                }

                if (!int.TryParse(daytxtbox.Text, out int days) || days <= 0)
                {
                    MessageBox.Show("Please enter valid number of days", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    daytxtbox.Focus();
                    return;
                }

                if (!decimal.TryParse(ratiotxtbox.Text, out decimal ratio) || ratio <= 0)
                {
                    MessageBox.Show("Please enter a valid ratio", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ratiotxtbox.Focus();
                    return;
                }

                // Get data
                DataTable dt = GetBankMarkupData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data available for selected parameters", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Bank Markup Calculator");
                //parameters.Add("AccountNo", currentAccountNo);
                //parameters.Add("AccountName", currentAccountName);
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                //parameters.Add("CalculationFormula", "Markup = (Principal × Rate × Days × Ratio) ÷ (365 × 100)");

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Bank", dataSources);

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

        // Search button event (you need to add this in designer)
        private void searchbtn_Click(object sender, EventArgs e)
        {
            SearchBankAccount();
        }
    }
}