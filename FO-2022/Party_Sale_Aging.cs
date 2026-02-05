using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Party_Sale_Aging : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Party_Sale_Aging()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Initialize combobox with conditions
            agingdayscombobox.Items.AddRange(new string[] {
                "≥ (Greater than or equal to)",
                "> (Greater than)",
                "≤ (Less than or equal to)",
                "< (Less than)",
                "= (Equal to)"
            });
            agingdayscombobox.SelectedIndex = 0;
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

                // Get aging days value
                if (!int.TryParse(txtboxwithcombobox.Text, out int agingDays))
                {
                    MessageBox.Show("Please enter a valid number for aging days.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get condition from combobox
                string condition = GetConditionFromComboBox(agingdayscombobox.SelectedItem.ToString());

                // Get report data
                DataTable reportData = GetPartySaleAgingData(agingDays, condition);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected criteria.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Party Sale Aging Report" },
                    //{ "AgingCriteria", $"{agingdayscombobox.SelectedItem} {agingDays} days" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Party_Sale_Aging", dataSources);
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

        private DataTable GetPartySaleAgingData(int agingDays, string condition)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        P.PartyID,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        SC.SectorName as Sector,
                        COUNT(DISTINCT S.SaleId) as TotalOverdueInvoices,
                        SUM(SB.TTLValue - S.ReceivedAmount) as TotalOverdueAmount,
                        MAX(DATEDIFF(day, S.DueDate, GETDATE())) as MaxOverdueDays,
                        AVG(DATEDIFF(day, S.DueDate, GETDATE())) as AvgOverdueDays,
                        
                        -- Aging buckets based on due date
                        SUM(CASE 
                            WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 1 AND 30 
                            THEN SB.TTLValue - S.ReceivedAmount 
                            ELSE 0 
                        END) as Age1to30,
                        
                        SUM(CASE 
                            WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 31 AND 60 
                            THEN SB.TTLValue - S.ReceivedAmount 
                            ELSE 0 
                        END) as Age31to60,
                        
                        SUM(CASE 
                            WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 61 AND 90 
                            THEN SB.TTLValue - S.ReceivedAmount 
                            ELSE 0 
                        END) as Age61to90,
                        
                        SUM(CASE 
                            WHEN DATEDIFF(day, S.DueDate, GETDATE()) > 90 
                            THEN SB.TTLValue - S.ReceivedAmount 
                            ELSE 0 
                        END) as AgeOver90
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
                    WHERE S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND S.DueDate < GETDATE() -- Only overdue invoices
                    AND SB.TTLValue > S.ReceivedAmount -- Only unpaid amounts
                    AND DATEDIFF(day, S.DueDate, GETDATE()) > 0"; // Overdue days

                // Add the user's condition
                query += $" AND DATEDIFF(day, S.DueDate, GETDATE()) {condition} @AgingDays";

                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, SC.SectorName
                    HAVING SUM(SB.TTLValue - S.ReceivedAmount) > 0
                    ORDER BY TotalOverdueAmount DESC, MaxOverdueDays DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AgingDays", agingDays);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private string GetConditionFromComboBox(string comboText)
        {
            // Extract the operator symbol from the combobox text
            if (comboText.Contains("≥")) return ">=";
            if (comboText.Contains(">")) return ">";
            if (comboText.Contains("≤")) return "<=";
            if (comboText.Contains("<")) return "<";
            if (comboText.Contains("=")) return "=";

            return ">="; // Default
        }

        private bool ValidateInput()
        {
            // Validate aging days input
            if (string.IsNullOrEmpty(txtboxwithcombobox.Text.Trim()))
            {
                MessageBox.Show("Please enter aging days.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtboxwithcombobox.Text, out int agingDays) || agingDays < 0)
            {
                MessageBox.Show("Please enter a valid positive number for aging days.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (agingdayscombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a condition from the dropdown.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Event handlers
        private void agingdayscombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // No special handling needed
        }

        private void txtboxwithcombobox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input as user types
            if (!string.IsNullOrEmpty(txtboxwithcombobox.Text))
            {
                if (!int.TryParse(txtboxwithcombobox.Text, out _))
                {
                    // Remove non-numeric characters
                    string cleanText = System.Text.RegularExpressions.Regex.Replace(txtboxwithcombobox.Text, "[^0-9]", "");
                    txtboxwithcombobox.Text = cleanText;
                    txtboxwithcombobox.SelectionStart = txtboxwithcombobox.Text.Length;
                }
            }
        }

        private void Party_Sale_Aging_Load(object sender, EventArgs e)
        {
            // Form loaded
        }

        // Empty click handler for the form background
        private void a_Click(object sender, EventArgs e) { }
    }
}