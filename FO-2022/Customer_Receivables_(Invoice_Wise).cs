using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Customer_Receivables__Invoice_Wise_ : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private List<DataRow> allSectors = new List<DataRow>();

        public Customer_Receivables__Invoice_Wise_()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default dates if you have date pickers (add them if needed)
            // fromdatepicker.Value = DateTime.Today.AddDays(-30);
            // todatepicker.Value = DateTime.Today;

            // Load sectors into listview
            LoadSectors();
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
                    LoadSectorsByTown(townIdNum); // Load sectors for this town
                    //MessageBox.Show("Town found.", "Success",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void LoadSectors()
        {
            try
            {
                listViewforsector.Items.Clear();
                allSectors.Clear();

                DataTable sectors = GetAllSectors();
                foreach (DataRow row in sectors.Rows)
                {
                    allSectors.Add(row);

                    ListViewItem item = new ListViewItem();
                    item.Text = row["SectorID"].ToString();
                    item.SubItems.Add(row["SectorName"].ToString());
                    item.Checked = false;

                    listViewforsector.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sectors: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSectorsByTown(int townId)
        {
            try
            {
                listViewforsector.Items.Clear();

                DataTable sectors = GetSectorsByTown(townId);
                foreach (DataRow row in sectors.Rows)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = row["SectorID"].ToString();
                    item.SubItems.Add(row["SectorName"].ToString());
                    item.Checked = false;

                    listViewforsector.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sectors: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetAllSectors()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM Sectors ORDER BY SectorName";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private DataTable GetSectorsByTown(int townId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM Sectors WHERE TownID = @TownID ORDER BY SectorName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TownID", townId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void slctallsectorbtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewforsector.Items)
            {
                item.Checked = true;
            }
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void GenerateReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (!ValidateInput()) return;

                // Get selected sectors
                List<int> selectedSectorIds = new List<int>();
                foreach (ListViewItem item in listViewforsector.Items)
                {
                    if (item.Checked)
                    {
                        if (int.TryParse(item.Text, out int sectorId))
                            selectedSectorIds.Add(sectorId);
                    }
                }

                // Get report data
                DataTable reportData = GetCustomerReceivablesData(selectedSectorIds);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No receivables data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Customer Receivables (Invoice Wise)" },
                    //{ "TownInfo", string.IsNullOrEmpty(townnametxtbox.Text) ?
                    //    "All Towns" : $"{townidtxtbox.Text} - {townnametxtbox.Text}" },
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
                    reportViewer.ShowReport("Customer_Receivables__Invoice_Wise", dataSources);
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

        private DataTable GetCustomerReceivablesData(List<int> selectedSectorIds)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Simple query for customer receivables
                string query = @"
                    SELECT 
                        P.PartyID as CustomerID,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        SC.SectorName as Sector,
                        S.SaleId as InvoiceNo,
                        S.SaleDate as InvoiceDate,
                        S.DueDate as DueDate,
                        SUM(SB.TTLValue) as InvoiceAmount,
                        S.ReceivedAmount as PaidAmount,
                        (SUM(SB.TTLValue) - S.ReceivedAmount) as Balance,
                        DATEDIFF(day, S.DueDate, GETDATE()) as OverdueDays,
                        CASE 
                            WHEN DATEDIFF(day, S.DueDate, GETDATE()) > 0 THEN 'Overdue'
                            ELSE 'Current'
                        END as Status
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
                    WHERE S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND (SUM(SB.TTLValue) - S.ReceivedAmount) > 0"; // Only show unpaid balances

                // Add filters if town is selected
                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                {
                    query = query.Replace("WHERE S.IsPosted = 1",
                        "WHERE P.City = @TownName AND S.IsPosted = 1");
                }

                // Add sector filter if sectors are selected
                if (selectedSectorIds.Count > 0)
                {
                    string sectorIds = string.Join(",", selectedSectorIds);
                    if (query.Contains("WHERE"))
                        query += $" AND SC.SectorID IN ({sectorIds})";
                    else
                        query += $" WHERE SC.SectorID IN ({sectorIds})";
                }

                // Complete the GROUP BY clause
                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, SC.SectorName, 
                             S.SaleId, S.SaleDate, S.DueDate, S.ReceivedAmount
                    ORDER BY P.PartyName, S.SaleDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                {
                    cmd.Parameters.AddWithValue("@TownName", townnametxtbox.Text);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            // Check if at least one sector is selected
            bool hasSelectedSector = false;
            foreach (ListViewItem item in listViewforsector.Items)
            {
                if (item.Checked)
                {
                    hasSelectedSector = true;
                    break;
                }
            }

            if (!hasSelectedSector)
            {
                MessageBox.Show("Please select at least one sector.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // If town ID is entered, name must be fetched
            if (!string.IsNullOrEmpty(townidtxtbox.Text.Trim()) &&
                string.IsNullOrEmpty(townnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Town Name first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Empty event handlers
        private void townidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void listViewforsector_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}