using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Customer_Receivables_By_Aging : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Customer_Receivables_By_Aging()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();

        private void townidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string townId = townidtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(townId) || !int.TryParse(townId, out int townIdNum))
                {
                    MessageBox.Show("Please enter valid Town ID.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable townInfo = GetTownInfo(townIdNum);
                if (townInfo.Rows.Count > 0)
                {
                    townnametxtbox.Text = townInfo.Rows[0]["TownName"].ToString();
                    //MessageBox.Show("Town found.", "Success",
                        //MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    townnametxtbox.Clear();
                    MessageBox.Show("Town not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetTownInfo(int townId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TownID, TownName FROM Towns WHERE TownID = @TownID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TownID", townId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string sectorId = sectoridtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(sectorId) || !int.TryParse(sectorId, out int sectorIdNum))
                {
                    MessageBox.Show("Please enter valid Sector ID.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable sectorInfo = GetSectorInfo(sectorIdNum);
                if (sectorInfo.Rows.Count > 0)
                {
                    sectornametxtbox.Text = sectorInfo.Rows[0]["SectorName"].ToString();
                    //MessageBox.Show("Sector found.", "Success",
                        //MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sectornametxtbox.Clear();
                    MessageBox.Show("Sector not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSectorInfo(int sectorId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM Sectors WHERE SectorID = @SectorID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SectorID", sectorId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!ValidateInput()) return;

                // Get report data
                DataTable reportData = GetCustomerReceivablesAgingData();

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No receivables data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Customer Receivables By Aging" },
                    //{ "TownInfo", string.IsNullOrEmpty(townnametxtbox.Text) ?
                    //    "All Towns" : $"{townidtxtbox.Text} - {townnametxtbox.Text}" },
                    //{ "SectorInfo", string.IsNullOrEmpty(sectornametxtbox.Text) ?
                    //    "All Sectors" : $"{sectoridtxtbox.Text} - {sectornametxtbox.Text}" },
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
                    reportViewer.ShowReport("Customer_Receivables_By_Aging", dataSources, parameters);
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

        private DataTable GetCustomerReceivablesAgingData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                P.PartyID as CustomerID,
                P.PartyName as CustomerName,
                P.City as Town,
                SC.SectorName as Sector,
                SUM(SB.TTLValue) as TotalInvoiced,
                SUM(S.ReceivedAmount) as TotalReceived,
                SUM(SB.TTLValue) - SUM(S.ReceivedAmount) as TotalBalance,
                
                SUM(CASE 
                    WHEN DATEDIFF(day, S.DueDate, GETDATE()) <= 0 
                    THEN (SB.TTLValue - ISNULL(S.ReceivedAmount * (SB.TTLValue / NULLIF(InvTotals.TotalInvoiceValue, 0)), 0))
                    ELSE 0 
                END) as CurrentAmount,
                
                SUM(CASE 
                    WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 1 AND 30 
                    THEN (SB.TTLValue - ISNULL(S.ReceivedAmount * (SB.TTLValue / NULLIF(InvTotals.TotalInvoiceValue, 0)), 0))
                    ELSE 0 
                END) as Age1to30,
                
                SUM(CASE 
                    WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 31 AND 60 
                    THEN (SB.TTLValue - ISNULL(S.ReceivedAmount * (SB.TTLValue / NULLIF(InvTotals.TotalInvoiceValue, 0)), 0))
                    ELSE 0 
                END) as Age31to60,
                
                SUM(CASE 
                    WHEN DATEDIFF(day, S.DueDate, GETDATE()) BETWEEN 61 AND 90 
                    THEN (SB.TTLValue - ISNULL(S.ReceivedAmount * (SB.TTLValue / NULLIF(InvTotals.TotalInvoiceValue, 0)), 0))
                    ELSE 0 
                END) as Age61to90,
                
                SUM(CASE 
                    WHEN DATEDIFF(day, S.DueDate, GETDATE()) > 90 
                    THEN (SB.TTLValue - ISNULL(S.ReceivedAmount * (SB.TTLValue / NULLIF(InvTotals.TotalInvoiceValue, 0)), 0))
                    ELSE 0 
                END) as AgeOver90,
                
                COUNT(DISTINCT S.SaleId) as TotalInvoices,
                MAX(DATEDIFF(day, S.DueDate, GETDATE())) as MaxOverdueDays
            FROM Sales S
            INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
            INNER JOIN Parties P ON S.CustomerId = P.PartyID
            LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
            LEFT JOIN (
                SELECT SaleId, SUM(TTLValue) as TotalInvoiceValue 
                FROM SalesBody 
                WHERE IsDeleted = 0
                GROUP BY SaleId
            ) InvTotals ON S.SaleId = InvTotals.SaleId
            WHERE S.IsPosted = 1
            AND SB.IsDeleted = 0
            AND S.ReceivedAmount < ISNULL(InvTotals.TotalInvoiceValue, SB.TTLValue)";

                // Add filters
                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                    query += " AND P.City = @TownName";

                if (!string.IsNullOrEmpty(sectornametxtbox.Text))
                    query += " AND SC.SectorID = @SectorID";

                query += @"
            GROUP BY P.PartyID, P.PartyName, P.City, SC.SectorName
            HAVING SUM(SB.TTLValue) - SUM(S.ReceivedAmount) > 0
            ORDER BY P.PartyName";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                    cmd.Parameters.AddWithValue("@TownName", townnametxtbox.Text);

                if (!string.IsNullOrEmpty(sectornametxtbox.Text) &&
                    int.TryParse(sectoridtxtbox.Text, out int sectorId))
                    cmd.Parameters.AddWithValue("@SectorID", sectorId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            // If town ID is entered, name must be fetched
            if (!string.IsNullOrEmpty(townidtxtbox.Text.Trim()) &&
                string.IsNullOrEmpty(townnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Town Name first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // If sector ID is entered, name must be fetched
            if (!string.IsNullOrEmpty(sectoridtxtbox.Text.Trim()) &&
                string.IsNullOrEmpty(sectornametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Sector Name first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Empty event handlers
        private void townidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectoridtxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectornametxtbox_TextChanged(object sender, EventArgs e) { }
        private void Customer_Receivables_By_Aging_Load(object sender, EventArgs e) { }
    }
}