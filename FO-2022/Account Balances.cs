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
    public partial class Account_Balances : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime? fromDate = null;
        private DateTime? toDate = null;

        public Account_Balances()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Set default radio buttons
            alldatebtnradio.Checked = true;
            showonlytransactionbtnradio.Checked = true;
            excludepartiesbtnradio.Checked = true;
            checkBoxExcludeaccountshavingzerobalance.Checked = false;

            // Set default date
            datepickerfortilldateradiobtn.Value = DateTime.Now;

            // Load party types
            LoadPartyTypes();
        }

        private void LoadPartyTypes()
        {
            partytypecombobox.Items.Clear();
            partytypecombobox.Items.Add("All Types");
            partytypecombobox.Items.Add("Customer");
            partytypecombobox.Items.Add("Vendor");
            partytypecombobox.Items.Add("Other");
            partytypecombobox.SelectedIndex = 0;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Account_Balances_Load(object sender, EventArgs e)
        {
            // Additional setup if needed
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void alldatebtnradio_CheckedChanged(object sender, EventArgs e)
        {
            if (alldatebtnradio.Checked)
            {
                fromDate = null;
                toDate = null;
                datepickerfortilldateradiobtn.Enabled = false;
            }
        }

        private void daterangebtnradio_CheckedChanged(object sender, EventArgs e)
        {
            if (daterangebtnradio.Checked)
            {
                datepickerfortilldateradiobtn.Enabled = false;
                // Show date range dialog
                ShowDateRangeDialog();
            }
        }

        private void tilldatebtnradio_CheckedChanged(object sender, EventArgs e)
        {
            if (tilldatebtnradio.Checked)
            {
                datepickerfortilldateradiobtn.Enabled = true;
                toDate = datepickerfortilldateradiobtn.Value;
                fromDate = null;
            }
        }

        private void datepickerfortilldateradiobtn_ValueChanged(object sender, EventArgs e)
        {
            if (tilldatebtnradio.Checked)
            {
                toDate = datepickerfortilldateradiobtn.Value;
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
                dateDialog.TopMost = true;   // 👈 yahan 

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
                        MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    fromDate = fromPicker.Value;
                    toDate = toPicker.Value;
                    dateDialog.DialogResult = DialogResult.OK;
                    dateDialog.Close();
                };

                cancelButton.Click += (s, ev) =>
                {
                    alldatebtnradio.Checked = true;
                    dateDialog.DialogResult = DialogResult.Cancel;
                    dateDialog.Close();
                };

                dateDialog.Controls.AddRange(new Control[] { fromLabel, fromPicker, toLabel, toPicker, okButton, cancelButton });

                if (dateDialog.ShowDialog() == DialogResult.OK)
                {
                    // Dates already set
                }
            }
        }

        private DataTable GetAccountBalancesData()
        {
            DataTable dt = new DataTable();

            try
            {
                StringBuilder queryBuilder = new StringBuilder();

                queryBuilder.Append(@"
                -- SIMPLE ACCOUNT BALANCES
                SELECT 
                    coa.AccountNo,
                    coa.AccountName,
                    coa.AccountType,
                    p.PartyType,
                    (coa.OpeningDebit - coa.OpeningCredit) AS OpeningBalance,
                    ISNULL(SUM(CASE WHEN av.Debit > 0 THEN av.Debit ELSE 0 END), 0) AS TotalDebit,
                    ISNULL(SUM(CASE WHEN av.Credit > 0 THEN av.Credit ELSE 0 END), 0) AS TotalCredit,
                    ((coa.OpeningDebit - coa.OpeningCredit) + 
                     ISNULL(SUM(av.Debit), 0) - 
                     ISNULL(SUM(av.Credit), 0)) AS CurrentBalance
                FROM [dbo].[ChartOfAccounts] coa
                LEFT JOIN [dbo].[Parties] p ON coa.AccountNo = p.PartyID
                LEFT JOIN [dbo].[AllVouchers] av ON coa.AccountNo = av.AccountNo ");

                // Add date filter
                if (daterangebtnradio.Checked && fromDate.HasValue && toDate.HasValue)
                {
                    queryBuilder.Append($" AND av.VoucherDate BETWEEN '{fromDate.Value.ToString("yyyy-MM-dd")}' AND '{toDate.Value.ToString("yyyy-MM-dd")}' ");
                }
                else if (tilldatebtnradio.Checked && toDate.HasValue)
                {
                    queryBuilder.Append($" AND av.VoucherDate <= '{toDate.Value.ToString("yyyy-MM-dd")}' ");
                }
                // else all dates - no filter

                queryBuilder.Append(@"
                WHERE coa.IsPosted = 1 ");

                // Filter by account type (Transaction/Group/Both)
                if (showonlytransactionbtnradio.Checked)
                {
                    queryBuilder.Append(" AND coa.IsDetailed = 1 ");
                }
                else if (showaccountgrpbtnradio.Checked)
                {
                    queryBuilder.Append(" AND coa.IsDetailed = 0 ");
                }
                // else both - no filter

                // Filter by party type (Exclude/Include/Only)
                if (excludepartiesbtnradio.Checked)
                {
                    queryBuilder.Append(" AND p.PartyID IS NULL ");
                }
                else if (onlypartiesbtnradio.Checked)
                {
                    queryBuilder.Append(" AND p.PartyID IS NOT NULL ");
                }
                // else include - no filter

                // Filter by party type from combobox
                if (partytypecombobox.SelectedIndex > 0)
                {
                    string partyType = partytypecombobox.SelectedItem.ToString();
                    queryBuilder.Append($" AND p.PartyType = '{partyType}' ");
                }

                // Group by
                queryBuilder.Append(@"
                GROUP BY 
                    coa.AccountNo, 
                    coa.AccountName, 
                    coa.AccountType, 
                    coa.OpeningDebit, 
                    coa.OpeningCredit,
                    p.PartyType ");

                // Exclude zero balance accounts if checkbox checked
                if (checkBoxExcludeaccountshavingzerobalance.Checked)
                {
                    queryBuilder.Append(@"
                HAVING 
                    ((coa.OpeningDebit - coa.OpeningCredit) + 
                     ISNULL(SUM(av.Debit), 0) - 
                     ISNULL(SUM(av.Credit), 0)) != 0 ");
                }

                queryBuilder.Append(@"
                ORDER BY coa.AccountType, coa.AccountNo");

                string query = queryBuilder.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching account balances: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport(bool isPrintMode = false)
        {
            try
            {
                // Validate date range
                if (daterangebtnradio.Checked && (!fromDate.HasValue || !toDate.HasValue))
                {
                    MessageBox.Show("Please select date range", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetAccountBalancesData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No accounts found with current filters", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate totals
                decimal totalOpening = 0;
                decimal totalDebit = 0;
                decimal totalCredit = 0;
                decimal totalCurrent = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalOpening += Convert.ToDecimal(row["OpeningBalance"]);
                    totalDebit += Convert.ToDecimal(row["TotalDebit"]);
                    totalCredit += Convert.ToDecimal(row["TotalCredit"]);
                    totalCurrent += Convert.ToDecimal(row["CurrentBalance"]);
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Account Balances Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                // Add period information
                if (alldatebtnradio.Checked)
                {
                    //parameters.Add("Period", "All Dates");
                }
                else if (tilldatebtnradio.Checked && toDate.HasValue)
                {
                    //parameters.Add("Period", $"Till Date: {toDate.Value.ToString("dd/MM/yyyy")}");
                }
                else if (daterangebtnradio.Checked && fromDate.HasValue && toDate.HasValue)
                {
                    //parameters.Add("Period", $"From {fromDate.Value.ToString("dd/MM/yyyy")} to {toDate.Value.ToString("dd/MM/yyyy")}");
                }

                // Add summary information
                //parameters.Add("TotalAccounts", dt.Rows.Count.ToString());
                //parameters.Add("TotalOpening", totalOpening.ToString("N2"));
                //parameters.Add("TotalDebit", totalDebit.ToString("N2"));
                //parameters.Add("TotalCredit", totalCredit.ToString("N2"));
                //parameters.Add("TotalCurrent", totalCurrent.ToString("N2"));

                // Create report viewer instance
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Account_Balances", dataSources);

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

        // Empty methods for designer events (not needed for functionality)
        private void showonlytransactionbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void showaccountgrpbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void showbothbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void excludepartiesbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void onlypartiesbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void includepartiesbtnradio_CheckedChanged(object sender, EventArgs e) { }
        private void partytypecombobox_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}