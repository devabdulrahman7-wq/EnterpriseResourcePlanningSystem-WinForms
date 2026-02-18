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
            LoadSectors();
        }

        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();

        private void townidsearch_Click(object sender, EventArgs e)
        {
            string townIdText = townidtxtbox.Text.Trim();
            string townNameText = townnametxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(townIdText) && string.IsNullOrWhiteSpace(townNameText))
            {
                MessageBox.Show("Please enter Town ID or Town Name.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                townidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    if (!string.IsNullOrWhiteSpace(townIdText))
                    {
                        if (!int.TryParse(townIdText, out int townId))
                        {
                            MessageBox.Show("Town ID must be numeric.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            townidtxtbox.Focus();
                            return;
                        }

                        cmd.CommandText = "SELECT TownID, TownName FROM Towns WHERE TownID = @Value";
                        cmd.Parameters.Add("@Value", SqlDbType.Int).Value = townId;
                    }
                    else
                    {
                        cmd.CommandText = "SELECT TownID, TownName FROM Towns WHERE TownName = @Value";
                        cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = townNameText;
                    }

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int foundTownId = Convert.ToInt32(reader["TownID"]);

                            townidtxtbox.Text = foundTownId.ToString();
                            townnametxtbox.Text = reader["TownName"].ToString();

                            // Always clear sectors before loading
                            LoadSectorsByTown(foundTownId);
                        }
                        else
                        {
                            townidtxtbox.Clear();
                            townnametxtbox.Clear();

                            MessageBox.Show("Town not found.",
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            townidtxtbox.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                List<int> selectedSectorIds = new List<int>();
                foreach (ListViewItem item in listViewforsector.Items)
                {
                    if (item.Checked)
                    {
                        if (int.TryParse(item.Text, out int sectorId))
                            selectedSectorIds.Add(sectorId);
                    }
                }

                DataTable reportData = GetCustomerReceivablesData(selectedSectorIds);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No receivables data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Customer Receivables (Invoice Wise)" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                {
                    //parameters.Add("TownInfo", $"{townidtxtbox.Text} - {townnametxtbox.Text}");
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Customer_Receivables__Invoice_Wise", dataSources, parameters);
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
                    AND SB.IsDeleted = 0";

                if (!string.IsNullOrEmpty(townnametxtbox.Text))
                {
                    query += " AND P.City = @TownName";
                }

                if (selectedSectorIds.Count > 0)
                {
                    string sectorIds = string.Join(",", selectedSectorIds);
                    query += $" AND SC.SectorID IN ({sectorIds})";
                }

                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, SC.SectorName, 
                             S.SaleId, S.SaleDate, S.DueDate, S.ReceivedAmount
                    HAVING (SUM(SB.TTLValue) - S.ReceivedAmount) > 0
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

            if (!string.IsNullOrEmpty(townidtxtbox.Text.Trim()) &&
                string.IsNullOrEmpty(townnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Town Name first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void townidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void listViewforsector_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}