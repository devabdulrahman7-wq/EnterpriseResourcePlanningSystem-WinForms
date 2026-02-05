using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Account_Balance__C_W : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private List<DataRow> allSectors = new List<DataRow>();

        public Account_Balance__C_W()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Load all sectors initially
            LoadSectors();
        }

        private void LoadSectors(string companyId = null)
        {
            try
            {
                listViewforsector.Items.Clear();
                allSectors.Clear();

                DataTable sectors = GetSectors(companyId);
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

        private DataTable GetSectors(string companyId = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT DISTINCT 
                        S.SectorID,
                        S.SectorName
                    FROM Sectors S
                    INNER JOIN Parties P ON S.SectorID = P.SectorID
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(companyId))
                {
                    query += @"
                        AND P.PartyID IN (
                            SELECT DISTINCT SL.CustomerId
                            FROM Sales SL
                            INNER JOIN SalesBody SB ON SL.SaleId = SB.SaleId
                            INNER JOIN Products PD ON SB.ProductId = PD.ProductID
                            WHERE PD.CompanyID = @CompanyID
                        )";
                }

                query += " ORDER BY S.SectorName";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(companyId))
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);

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
                    LoadSectors(companyId); // Load sectors for this company
                    //MessageBox.Show("Company found.", "Success",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void button1_Click(object sender, EventArgs e)
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

                // Get company ID if entered
                string companyId = cmpnyidtxtbox.Text.Trim();

                // Get report data
                DataTable reportData = GetAccountBalanceData(selectedSectorIds, companyId);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No account balance data found.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Account Balance (Customer Wise)" },
                    //{ "CompanyInfo", string.IsNullOrEmpty(companyId) ?
                    //    "All Companies" : $"{companyId} - {cmpnynametxtbox.Text}" },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") },
                    //{ "TotalAccounts", reportData.Rows.Count.ToString() },
                    //{ "TotalBalance", CalculateTotalBalance(reportData).ToString("N2") }
                };

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Account_Balance__C_W", dataSources, parameters);
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

        private decimal CalculateTotalBalance(DataTable data)
        {
            decimal total = 0;
            foreach (DataRow row in data.Rows)
            {
                total += Convert.ToDecimal(row["Balance"]);
            }
            return total;
        }

        private DataTable GetAccountBalanceData(List<int> selectedSectorIds, string companyId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    WITH AccountTransactions AS (
                        -- Get opening balances from ChartOfAccounts
                        SELECT 
                            AccountNo,
                            SUM(OpeningDebit - OpeningCredit + AdjustedDebit - AdjustedCredit) as OpeningBalance
                        FROM ChartOfAccounts
                        WHERE AccountNo IN (SELECT PartyID FROM Parties WHERE PartyType = 'C')
                        GROUP BY AccountNo
                    ),
                    SalesSummary AS (
                        -- Get total sales amounts
                        SELECT 
                            SL.CustomerId,
                            SUM(SB.TTLValue) as TotalSales,
                            SUM(SL.ReceivedAmount) as TotalReceived
                        FROM Sales SL
                        INNER JOIN SalesBody SB ON SL.SaleId = SB.SaleId
                        WHERE SL.IsPosted = 1
                        AND SB.IsDeleted = 0";

                // Add company filter if provided
                if (!string.IsNullOrEmpty(companyId))
                {
                    query += @"
                        AND SB.ProductId IN (
                            SELECT ProductID 
                            FROM Products 
                            WHERE CompanyID = @CompanyID
                        )";
                }

                query += @"
                        GROUP BY SL.CustomerId
                    ),
                    RecoverySummary AS (
                        -- Get total recoveries
                        SELECT 
                            RB.CustomerID,
                            SUM(RB.Amount) as TotalRecovered
                        FROM RecoveryBody RB
                        INNER JOIN Recovery R ON RB.RecoveryID = R.RecoveryID
                        WHERE R.IsPosted = 1
                        AND RB.IsDeleted = 0
                        GROUP BY RB.CustomerID
                    )
                    SELECT 
                        P.PartyID as AccountNo,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        SC.SectorName as Sector,
                        AT.OpeningBalance,
                        ISNULL(SS.TotalSales, 0) as TotalSales,
                        ISNULL(SS.TotalReceived, 0) as TotalReceived,
                        ISNULL(RS.TotalRecovered, 0) as TotalRecovered,
                        ISNULL(AT.OpeningBalance, 0) + 
                        ISNULL(SS.TotalSales, 0) - 
                        ISNULL(SS.TotalReceived, 0) - 
                        ISNULL(RS.TotalRecovered, 0) as Balance,
                        P.CreditDays,
                        P.CreditLimit,
                        P.IsBlock
                    FROM Parties P
                    LEFT JOIN AccountTransactions AT ON P.PartyID = AT.AccountNo
                    LEFT JOIN SalesSummary SS ON P.PartyID = SS.CustomerId
                    LEFT JOIN RecoverySummary RS ON P.PartyID = RS.CustomerID
                    LEFT JOIN Sectors SC ON P.SectorID = SC.SectorID
                    WHERE P.PartyType = 'C'";

                // Add sector filter
                if (selectedSectorIds.Count > 0)
                {
                    string sectorIds = string.Join(",", selectedSectorIds);
                    query += $" AND P.SectorID IN ({sectorIds})";
                }

                query += @"
                    ORDER BY P.PartyName";

                SqlCommand cmd = new SqlCommand(query, conn);

                if (!string.IsNullOrEmpty(companyId))
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);

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
        private void listViewforsector_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Account_Balance__C_W_Load(object sender, EventArgs e) { }
    }
}