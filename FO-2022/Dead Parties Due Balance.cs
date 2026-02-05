using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Dead_Parties_Due_Balance : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Dead_Parties_Due_Balance()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default values
            saledaysbeforetxtbox.Text = "30";
            duebalgreaterthantxtbox.Text = "10000";
        }

        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!ValidateInput()) return;

                // Get parameters
                int saleDaysBefore = int.Parse(saledaysbeforetxtbox.Text);
                decimal dueBalanceGreaterThan = decimal.Parse(duebalgreaterthantxtbox.Text);

                // Get report data
                DataTable reportData = GetDeadPartiesDueBalanceData(saleDaysBefore, dueBalanceGreaterThan);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No dead parties found with due balance.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Dead Parties Due Balance Report" },
                    //{ "SaleDaysBefore", saleDaysBefore.ToString() },
                    //{ "DueBalanceGreaterThan", dueBalanceGreaterThan.ToString("N2") },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") },
                    //{ "TotalDeadParties", reportData.Rows.Count.ToString() },
                    //{ "TotalDueBalance", CalculateTotalDueBalance(reportData).ToString("N2") }
                };

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Dead_Parties_Due_Balance", dataSources, parameters);
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

        private decimal CalculateTotalDueBalance(DataTable data)
        {
            decimal total = 0;
            foreach (DataRow row in data.Rows)
            {
                total += Convert.ToDecimal(row["TotalDueBalance"]);
            }
            return total;
        }

        private DataTable GetDeadPartiesDueBalanceData(int saleDaysBefore, decimal dueBalanceGreaterThan)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    WITH PartyLastSale AS (
                        SELECT 
                            S.CustomerId,
                            MAX(S.SaleDate) as LastSaleDate,
                            COUNT(S.SaleId) as TotalSales
                        FROM Sales S
                        WHERE S.IsPosted = 1
                        GROUP BY S.CustomerId
                    ),
                    PartyDueBalance AS (
                        SELECT 
                            S.CustomerId,
                            SUM(SB.TTLValue - S.ReceivedAmount) as TotalDueBalance
                        FROM Sales S
                        INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                        WHERE S.IsPosted = 1
                        AND SB.IsDeleted = 0
                        AND SB.TTLValue > S.ReceivedAmount
                        GROUP BY S.CustomerId
                        HAVING SUM(SB.TTLValue - S.ReceivedAmount) > 0
                    )
                    SELECT 
                        P.PartyID,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        P.SectorID,
                        SC.SectorName as Sector,
                        P.Phone1,
                        P.Mobile,
                        P.Address,
                        PLS.LastSaleDate,
                        DATEDIFF(day, PLS.LastSaleDate, GETDATE()) as DaysSinceLastSale,
                        PLS.TotalSales,
                        PDB.TotalDueBalance,
                        CASE 
                            WHEN PLS.LastSaleDate IS NULL THEN 'No Sales History'
                            WHEN DATEDIFF(day, PLS.LastSaleDate, GETDATE()) > @SaleDaysBefore THEN 'Dead Party'
                            ELSE 'Active'
                        END as PartyStatus
                    FROM Parties P
                    LEFT JOIN PartyLastSale PLS ON P.PartyID = PLS.CustomerId
                    LEFT JOIN PartyDueBalance PDB ON P.PartyID = PDB.CustomerId
                    LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
                    WHERE 
                        -- Conditions for dead parties
                        (
                            PLS.LastSaleDate IS NULL 
                            OR DATEDIFF(day, PLS.LastSaleDate, GETDATE()) > @SaleDaysBefore
                        )
                        -- Has due balance greater than specified
                        AND (PDB.TotalDueBalance > @DueBalanceGreaterThan OR PDB.TotalDueBalance IS NOT NULL)
                    ORDER BY 
                        CASE 
                            WHEN PLS.LastSaleDate IS NULL THEN 1
                            ELSE DATEDIFF(day, PLS.LastSaleDate, GETDATE())
                        END DESC,
                        PDB.TotalDueBalance DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SaleDaysBefore", saleDaysBefore);
                cmd.Parameters.AddWithValue("@DueBalanceGreaterThan", dueBalanceGreaterThan);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            // Validate sale days before
            if (string.IsNullOrEmpty(saledaysbeforetxtbox.Text))
            {
                MessageBox.Show("Please enter days since last sale.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                saledaysbeforetxtbox.Focus();
                return false;
            }

            if (!int.TryParse(saledaysbeforetxtbox.Text, out int saleDays) || saleDays < 0)
            {
                MessageBox.Show("Please enter a valid positive number for days.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                saledaysbeforetxtbox.Focus();
                return false;
            }

            // Validate due balance
            if (string.IsNullOrEmpty(duebalgreaterthantxtbox.Text))
            {
                MessageBox.Show("Please enter minimum due balance.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                duebalgreaterthantxtbox.Focus();
                return false;
            }

            if (!decimal.TryParse(duebalgreaterthantxtbox.Text, out decimal dueBalance) || dueBalance < 0)
            {
                MessageBox.Show("Please enter a valid positive number for due balance.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                duebalgreaterthantxtbox.Focus();
                return false;
            }

            return true;
        }

        private void saledaysbeforetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input
            if (!string.IsNullOrEmpty(saledaysbeforetxtbox.Text))
            {
                if (!int.TryParse(saledaysbeforetxtbox.Text, out _))
                {
                    // Remove non-numeric characters
                    string cleanText = System.Text.RegularExpressions.Regex.Replace(saledaysbeforetxtbox.Text, "[^0-9]", "");
                    saledaysbeforetxtbox.Text = cleanText;
                    saledaysbeforetxtbox.SelectionStart = saledaysbeforetxtbox.Text.Length;
                }
            }
        }

        private void duebalgreaterthantxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input
            if (!string.IsNullOrEmpty(duebalgreaterthantxtbox.Text))
            {
                if (!decimal.TryParse(duebalgreaterthantxtbox.Text, out _))
                {
                    // Remove non-numeric characters except decimal point
                    string cleanText = System.Text.RegularExpressions.Regex.Replace(duebalgreaterthantxtbox.Text, "[^0-9.]", "");
                    duebalgreaterthantxtbox.Text = cleanText;
                    duebalgreaterthantxtbox.SelectionStart = duebalgreaterthantxtbox.Text.Length;
                }
            }
        }

        private void Dead_Parties_Due_Balance_Load(object sender, EventArgs e)
        {
            // Form loaded
        }
    }
}