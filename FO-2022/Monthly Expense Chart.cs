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
    public partial class Monthly_Expense_Chart : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Monthly_Expense_Chart()
        {
            InitializeComponent();
            LoadQuarterOptions();
            SetDefaultDates();
        }

        private void LoadQuarterOptions()
        {
            selectyearquartercombobox.Items.Clear();
            selectyearquartercombobox.Items.Add("Select Quarter");
            selectyearquartercombobox.Items.Add("Quarter 1 (Jan - Mar)");
            selectyearquartercombobox.Items.Add("Quarter 2 (Apr - Jun)");
            selectyearquartercombobox.Items.Add("Quarter 3 (Jul - Sep)");
            selectyearquartercombobox.Items.Add("Quarter 4 (Oct - Dec)");
            selectyearquartercombobox.Items.Add("Half Year (Jan - Jun)");
            selectyearquartercombobox.Items.Add("Half Year (Jul - Dec)");
            selectyearquartercombobox.Items.Add("Full Year (Jan - Dec)");
            selectyearquartercombobox.Items.Add("Custom Range");

            selectyearquartercombobox.SelectedIndex = 0;
        }

        private void SetDefaultDates()
        {
            // Set to year 2023
            fromdatepcker.Value = new DateTime(2023, 1, 1); // Jan 1, 2023
            todatepcker.Value = new DateTime(2023, 12, 31); // Dec 31, 2023
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Monthly_Expense_Chart_Load(object sender, EventArgs e)
        {
            // Form load logic
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // If user manually changes date, set to Custom Range
            selectyearquartercombobox.SelectedIndex = selectyearquartercombobox.Items.Count - 1; // Custom Range
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // If user manually changes date, set to Custom Range
            selectyearquartercombobox.SelectedIndex = selectyearquartercombobox.Items.Count - 1; // Custom Range
        }

        private void selectyearquartercombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectyearquartercombobox.SelectedIndex <= 0) return;

            int currentYear = DateTime.Now.Year;
            DateTime fromDate, toDate;

            switch (selectyearquartercombobox.SelectedItem.ToString())
            {
                case "Quarter 1 (Jan - Mar)":
                    fromDate = new DateTime(currentYear, 1, 1);
                    toDate = new DateTime(currentYear, 3, 31);
                    break;

                case "Quarter 2 (Apr - Jun)":
                    fromDate = new DateTime(currentYear, 4, 1);
                    toDate = new DateTime(currentYear, 6, 30);
                    break;

                case "Quarter 3 (Jul - Sep)":
                    fromDate = new DateTime(currentYear, 7, 1);
                    toDate = new DateTime(currentYear, 9, 30);
                    break;

                case "Quarter 4 (Oct - Dec)":
                    fromDate = new DateTime(currentYear, 10, 1);
                    toDate = new DateTime(currentYear, 12, 31);
                    break;

                case "Half Year (Jan - Jun)":
                    fromDate = new DateTime(currentYear, 1, 1);
                    toDate = new DateTime(currentYear, 6, 30);
                    break;

                case "Half Year (Jul - Dec)":
                    fromDate = new DateTime(currentYear, 7, 1);
                    toDate = new DateTime(currentYear, 12, 31);
                    break;

                case "Full Year (Jan - Dec)":
                    fromDate = new DateTime(currentYear, 1, 1);
                    toDate = new DateTime(currentYear, 12, 31);
                    break;

                default: // Custom Range
                    return;
            }

            fromdatepcker.Value = fromDate;
            todatepcker.Value = toDate;
        }

        private DataTable GetMonthlyExpenseData()
        {
            DataTable dt = new DataTable();

            try
            {
                string query = @"
                SELECT 
                    YEAR(av.VoucherDate) AS Year,
                    MONTH(av.VoucherDate) AS Month,
                    DATENAME(MONTH, av.VoucherDate) AS MonthName,
                    SUM(av.Debit) AS TotalExpense,
                    COUNT(*) AS TransactionCount
                FROM [dbo].[AllVouchers] av
                INNER JOIN [dbo].[ChartOfAccounts] coa ON av.AccountNo = coa.AccountNo
                WHERE coa.ExpFlag = 1  -- Expense Accounts
                    AND av.VoucherDate BETWEEN @FromDate AND @ToDate
                    AND av.Debit > 0  -- Only debit transactions
                GROUP BY YEAR(av.VoucherDate), MONTH(av.VoucherDate), DATENAME(MONTH, av.VoucherDate)
                ORDER BY YEAR(av.VoucherDate), MONTH(av.VoucherDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            try
            {
                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetMonthlyExpenseData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No expense data found for selected period", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Monthly Expense Chart");
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("Period", selectyearquartercombobox.Text);

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Monthly_Expense", dataSources);

                // If print mode, you can add additional logic here if needed
                if (isPrintMode)
                {
                    // You can add any print-specific logic here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport(false); // Show in preview mode
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport(true); // Show in print mode
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}