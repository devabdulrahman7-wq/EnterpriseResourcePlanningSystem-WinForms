using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Due_Recovery_By_Credit_Days : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Due_Recovery_By_Credit_Days()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Load salesmen into combobox
            LoadSalesmen();

            // Set default radio button
            newpetroreportradiobtn.Checked = true;

            // Set recovery percentage combobox items
            recoverypercentagecombobox.Items.AddRange(new string[] {
                "50%", "60%", "70%", "75%", "80%", "85%", "90%", "95%", "100%"
            });
            recoverypercentagecombobox.SelectedIndex = 4; // Default to 80%
        }

        private void LoadSalesmen()
        {
            try
            {
                slctsalesmamcombobox.Items.Clear();
                slctsalesmamcombobox.Items.Add("All Salesmen");

                DataTable salesmen = GetSalesmen();
                foreach (DataRow row in salesmen.Rows)
                {
                    slctsalesmamcombobox.Items.Add($"{row["SalesmanID"]} - {row["SalesmanName"]}");
                }
                slctsalesmamcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading salesmen: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSalesmen()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!ValidateInput()) return;

                // Get selected salesman ID
                string selectedSalesman = slctsalesmamcombobox.SelectedItem.ToString();
                string salesmanId = null;
                if (slctsalesmamcombobox.SelectedIndex > 0)
                {
                    salesmanId = selectedSalesman.Split('-')[0].Trim();
                }

                // Get recovery percentage
                string recoveryPercentText = recoverypercentagecombobox.SelectedItem.ToString().Replace("%", "");
                if (!decimal.TryParse(recoveryPercentText, out decimal recoveryPercent))
                {
                    MessageBox.Show("Invalid recovery percentage.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get custom recovery amount if entered
                decimal? customRecoveryAmount = null;
                if (!string.IsNullOrEmpty(textBoxforrecoverycombobox.Text))
                {
                    if (decimal.TryParse(textBoxforrecoverycombobox.Text, out decimal customAmount))
                    {
                        customRecoveryAmount = customAmount;
                    }
                }

                // Get A/C prefix
                string acPrefix = acprefixtxtbox.Text.Trim();

                // Get report type
                string reportType = newpetroreportradiobtn.Checked ? "New" : "Old";

                // Get report data
                DataTable reportData = GetDueRecoveryData(salesmanId, recoveryPercent, customRecoveryAmount, acPrefix, reportType);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No due recovery data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Due Recovery By Credit Days" },
                    //{ "SalesmanInfo", slctsalesmamcombobox.SelectedIndex == 0 ? "All Salesmen" : selectedSalesman },
                    //{ "RecoveryPercentage", $"{recoveryPercent}%" },
                    //{ "ReportType", reportType + " Petro Report" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                // Add A/C prefix if provided
                if (!string.IsNullOrEmpty(acPrefix))
                {
                    parameters.Add("AccountPrefix", $"A/C Prefix: {acPrefix}");
                }

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Due_Recovery_By_Credit_Days", dataSources, parameters);
                }
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

        private DataTable GetDueRecoveryData(string salesmanId, decimal recoveryPercent, decimal? customRecoveryAmount, string acPrefix, string reportType)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        P.PartyID,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        P.CreditDays,
                        S.SalesmanName,
                        COUNT(DISTINCT SLS.SaleId) as OverdueInvoices,
                        SUM(SB.TTLValue) as TotalInvoiceAmount,
                        SUM(SLS.ReceivedAmount) as TotalReceived,
                        SUM(SB.TTLValue - SLS.ReceivedAmount) as TotalBalance,
                        MAX(DATEDIFF(day, SLS.DueDate, GETDATE())) as MaxOverdueDays,
                        AVG(DATEDIFF(day, SLS.DueDate, GETDATE())) as AvgOverdueDays,
                        
                        -- Recovery calculation based on percentage
                        SUM(SB.TTLValue - SLS.ReceivedAmount) * (@RecoveryPercent / 100.0) as RecoveryTarget,
                        
                        -- Recovery status
                        CASE 
                            WHEN SUM(SB.TTLValue - SLS.ReceivedAmount) * (@RecoveryPercent / 100.0) <= SUM(SLS.ReceivedAmount) 
                            THEN 'Achieved'
                            ELSE 'Pending'
                        END as RecoveryStatus,
                        
                        -- Amount to recover
                        CASE 
                            WHEN SUM(SB.TTLValue - SLS.ReceivedAmount) * (@RecoveryPercent / 100.0) > SUM(SLS.ReceivedAmount)
                            THEN SUM(SB.TTLValue - SLS.ReceivedAmount) * (@RecoveryPercent / 100.0) - SUM(SLS.ReceivedAmount)
                            ELSE 0
                        END as AmountToRecover,
                        
                        -- Credit utilization percentage
                        CASE 
                            WHEN P.CreditDays > 0 
                            THEN (MAX(DATEDIFF(day, SLS.DueDate, GETDATE())) * 100.0 / P.CreditDays)
                            ELSE 0
                        END as CreditUtilizationPercent
                    FROM Sales SLS
                    INNER JOIN SalesBody SB ON SLS.SaleId = SB.SaleId
                    INNER JOIN Parties P ON SLS.CustomerId = P.PartyID
                    LEFT JOIN Salesmen S ON SLS.SalesmanId = S.SalesmanID
                    WHERE SLS.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND SLS.DueDate < GETDATE()
                    AND SB.TTLValue > SLS.ReceivedAmount";

                // Add filters
                if (!string.IsNullOrEmpty(salesmanId))
                    query += " AND S.SalesmanID = @SalesmanID";

                if (!string.IsNullOrEmpty(acPrefix))
                    query += " AND P.PartyID LIKE @ACPrefix + '%'";

                // Different query for old vs new report (you can adjust this logic)
                if (reportType == "Old")
                {
                    // Old report might show additional columns or different calculations
                    query += " AND DATEDIFF(day, SLS.DueDate, GETDATE()) > 30"; // Example: Show only >30 days overdue
                }

                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, P.CreditDays, S.SalesmanName
                    HAVING SUM(SB.TTLValue - SLS.ReceivedAmount) > 0
                    ORDER BY AmountToRecover DESC, MaxOverdueDays DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RecoveryPercent", recoveryPercent);

                if (!string.IsNullOrEmpty(salesmanId))
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

                if (!string.IsNullOrEmpty(acPrefix))
                    cmd.Parameters.AddWithValue("@ACPrefix", acPrefix);

                // Add custom recovery amount if provided
                if (customRecoveryAmount.HasValue)
                {
                    cmd.Parameters.AddWithValue("@CustomRecoveryAmount", customRecoveryAmount.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            // Validate recovery percentage
            if (recoverypercentagecombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a recovery percentage.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate custom recovery amount if entered
            if (!string.IsNullOrEmpty(textBoxforrecoverycombobox.Text))
            {
                if (!decimal.TryParse(textBoxforrecoverycombobox.Text, out decimal customAmount) || customAmount < 0)
                {
                    MessageBox.Show("Please enter a valid positive number for custom recovery amount.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        // Event handlers
        private void recoverypercentagecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable custom recovery textbox based on selection
            if (recoverypercentagecombobox.SelectedItem != null)
            {
                string selected = recoverypercentagecombobox.SelectedItem.ToString();
                if (selected == "Custom")
                {
                    textBoxforrecoverycombobox.Enabled = true;
                    textBoxforrecoverycombobox.Focus();
                }
                else
                {
                    textBoxforrecoverycombobox.Enabled = false;
                    textBoxforrecoverycombobox.Clear();
                }
            }
        }

        private void textBoxforrecoverycombobox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input for custom recovery amount
            if (!string.IsNullOrEmpty(textBoxforrecoverycombobox.Text))
            {
                if (!decimal.TryParse(textBoxforrecoverycombobox.Text, out _))
                {
                    // Remove non-numeric characters
                    string cleanText = System.Text.RegularExpressions.Regex.Replace(textBoxforrecoverycombobox.Text, "[^0-9.]", "");
                    textBoxforrecoverycombobox.Text = cleanText;
                    textBoxforrecoverycombobox.SelectionStart = textBoxforrecoverycombobox.Text.Length;
                }
            }
        }

        private void acprefixtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add validation for A/C prefix format
            if (!string.IsNullOrEmpty(acprefixtxtbox.Text))
            {
                // Keep only uppercase letters and numbers
                string cleanText = System.Text.RegularExpressions.Regex.Replace(acprefixtxtbox.Text, "[^A-Z0-9]", "");
                acprefixtxtbox.Text = cleanText.ToUpper();
                acprefixtxtbox.SelectionStart = acprefixtxtbox.Text.Length;
            }
        }

        // Empty event handlers
        private void slctsalesmamcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void newpetroreportradiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void oldpetroreportradiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void Due_Recovery_By_Credit_Days_Load(object sender, EventArgs e) { }
    }
}