using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Customer_Receivable_C_W : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private List<DataRow> allSectors = new List<DataRow>();

        public Customer_Receivable_C_W()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Load all sectors initially
            LoadSectors();
        }

        private void LoadSectors(string townId = null)
        {
            try
            {
                listViewforsector.Items.Clear();
                allSectors.Clear();

                DataTable sectors = GetSectors(townId);
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

        private DataTable GetSectors(string townId = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM Sectors WHERE 1=1";

                if (!string.IsNullOrEmpty(townId))
                {
                    query += " AND TownID = @TownID";
                }

                query += " ORDER BY SectorName";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(townId) && int.TryParse(townId, out int townIdNum))
                    cmd.Parameters.AddWithValue("@TownID", townIdNum);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox4_Click(object sender, EventArgs e) => this.Close();

        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string companyId = cmpnyidtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(companyId))
                {
                    MessageBox.Show("Please enter Company ID.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable companyInfo = GetCompanyInfo(companyId);
                if (companyInfo.Rows.Count > 0)
                {
                    cmpnynametxtbox.Text = companyInfo.Rows[0]["CompanyName"].ToString();
                    MessageBox.Show("Company found.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cmpnynametxtbox.Clear();
                    MessageBox.Show("Company not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetCompanyInfo(string companyId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CompanyID", companyId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

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
                    LoadSectors(townId); // Load sectors for this town
                    MessageBox.Show("Town found.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void Selectallsectorbtn_Click(object sender, EventArgs e)
        {
            // Select All button
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
                    if (item.Checked && int.TryParse(item.Text, out int sectorId))
                    {
                        selectedSectorIds.Add(sectorId);
                    }
                }

                // Get company and town IDs
                string companyId = cmpnyidtxtbox.Text.Trim();
                string townId = townidtxtbox.Text.Trim();

                // Get report data
                DataTable reportData = GetCustomerReceivableData(selectedSectorIds, companyId, townId);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No receivable data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Customer Receivable (Customer Wise)" },
                    //{ "CompanyInfo", string.IsNullOrEmpty(companyId) ?
                    //    "All Companies" : $"{companyId} - {cmpnynametxtbox.Text}" },
                    //{ "TownInfo", string.IsNullOrEmpty(townId) ?
                    //    "All Towns" : $"{townId} - {townnametxtbox.Text}" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") },
                    //{ "TotalCustomers", reportData.Rows.Count.ToString() },
                    //{ "TotalReceivable", CalculateTotalReceivable(reportData).ToString("N2") }
                };

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Customer_Receivable_C_W", dataSources, parameters);
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

        private decimal CalculateTotalReceivable(DataTable data)
        {
            decimal total = 0;
            foreach (DataRow row in data.Rows)
            {
                total += Convert.ToDecimal(row["TotalReceivable"]);
            }
            return total;
        }

        private DataTable GetCustomerReceivableData(List<int> selectedSectorIds, string companyId, string townId)
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
                        C.CompanyName,
                        COUNT(DISTINCT S.SaleId) as TotalInvoices,
                        SUM(SB.TTLValue) as TotalSales,
                        SUM(S.ReceivedAmount) as TotalReceived,
                        SUM(SB.TTLValue - S.ReceivedAmount) as TotalReceivable,
                        MAX(DATEDIFF(day, S.DueDate, GETDATE())) as MaxOverdueDays,
                        AVG(DATEDIFF(day, S.DueDate, GETDATE())) as AvgOverdueDays,
                        P.CreditDays,
                        P.CreditLimit
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Products PD ON SB.ProductId = PD.ProductID
                    INNER JOIN Companies C ON PD.CompanyID = C.CompanyID
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
                    WHERE S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND SB.TTLValue > S.ReceivedAmount";

                // Add filters
                if (!string.IsNullOrEmpty(companyId))
                    query += " AND C.CompanyID = @CompanyID";

                if (!string.IsNullOrEmpty(townId) && int.TryParse(townId, out int townIdNum))
                    query += " AND P.City = (SELECT TownName FROM Towns WHERE TownID = @TownID)";

                if (selectedSectorIds.Count > 0)
                {
                    string sectorIds = string.Join(",", selectedSectorIds);
                    query += $" AND SC.SectorID IN ({sectorIds})";
                }

                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, SC.SectorName, C.CompanyName, P.CreditDays, P.CreditLimit
                    HAVING SUM(SB.TTLValue - S.ReceivedAmount) > 0
                    ORDER BY TotalReceivable DESC, P.PartyName";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(companyId))
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);

                if (!string.IsNullOrEmpty(townId) && int.TryParse(townId, out townIdNum))
                    cmd.Parameters.AddWithValue("@TownID", townIdNum);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            // If company ID is entered, name must be fetched
            if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text.Trim()) &&
                string.IsNullOrEmpty(cmpnynametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Company Name first.", "Error",
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

            return true;
        }

        // Empty event handlers
        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void cmpnynametxtbox_TextChanged(object sender, EventArgs e) { }
        private void townidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void listViewforsector_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Customer_Receivable_C_W_Load(object sender, EventArgs e) { }
    }
}