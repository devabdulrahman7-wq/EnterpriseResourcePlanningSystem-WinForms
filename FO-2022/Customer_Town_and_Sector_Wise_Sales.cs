using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Customer_Town_and_Sector_Wise_Sales : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Customer_Town_and_Sector_Wise_Sales()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;
        }

        private void pictureBox7_Click(object sender, EventArgs e) => this.Close();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be less than From date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value.AddDays(1);
            }
        }

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string customerId = cstmridtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(customerId))
                {
                    MessageBox.Show("Please enter Customer ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable customerInfo = GetCustomerInfo(customerId);
                if (customerInfo.Rows.Count > 0)
                {
                    //cstmrnametxtbox.Text = customerInfo.Rows[0]["PartyName"].ToString();
                    //MessageBox.Show("Customer found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cstmrnametxtbox.Clear();
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetCustomerInfo(string customerId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PartyID, PartyName FROM Parties WHERE PartyID = @PartyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PartyID", customerId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string companyId = cmpnyidtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(companyId))
                {
                    MessageBox.Show("Please enter Company ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable companyInfo = GetCompanyInfo(companyId);
                if (companyInfo.Rows.Count > 0)
                {
                    //cmpnynametxtbox.Text = companyInfo.Rows[0]["CompanyName"].ToString();
                    //MessageBox.Show("Company found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cmpnynametxtbox.Clear();
                    MessageBox.Show("Company not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Please enter valid Town ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable townInfo = GetTownInfo(townIdNum);
                if (townInfo.Rows.Count > 0)
                {
                    //townnametxtbox.Text = townInfo.Rows[0]["TownName"].ToString();
                    //MessageBox.Show("Town found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    townnametxtbox.Clear();
                    MessageBox.Show("Town not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Please enter valid Sector ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable sectorInfo = GetSectorInfo(sectorIdNum);
                if (sectorInfo.Rows.Count > 0)
                {
                    //sectornametxtbox.Text = sectorInfo.Rows[0]["SectorName"].ToString();
                    //MessageBox.Show("Sector found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sectornametxtbox.Clear();
                    MessageBox.Show("Sector not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                DateTime fromDate = fromdatepcker.Value.Date;
                DateTime toDate = todatepcker.Value.Date.AddDays(1).AddSeconds(-1);
                string customerId = cstmridtxtbox.Text.Trim();
                string companyId = cmpnyidtxtbox.Text.Trim();
                string sectorId = sectoridtxtbox.Text.Trim();
                string townId = townidtxtbox.Text.Trim();

                DataTable reportData = GetCustomerTownSectorSalesData(fromDate, toDate, customerId, companyId, townId, sectorId);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Customer/Town/Sector Wise Sales" },
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
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
                    reportViewer.ShowReport("Customer_Town_and_Sector_Wise_Sales", dataSources);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetCustomerTownSectorSalesData(DateTime fromDate, DateTime toDate, string customerId, string companyId, string townId, string sectorId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        P.PartyID as CustomerID,
                        P.PartyName as CustomerName,
                        P.City as Town,
                        SCT.SectorName as Sector,
                        C.CompanyID,
                        C.CompanyName,
                        COUNT(DISTINCT SL.SaleId) as TotalInvoices,
                        SUM(SB.TTLValue) as TotalAmount,
                        SUM(SB.TTLSTax) as TotalSalesTax,
                        SUM(SB.TTLFTax) as TotalFurtherTax
                    FROM Sales SL
                    INNER JOIN SalesBody SB ON SL.SaleId = SB.SaleId
                    INNER JOIN Products PD ON SB.ProductId = PD.ProductID
                    INNER JOIN Companies C ON PD.CompanyID = C.CompanyID
                    INNER JOIN Parties P ON SL.CustomerId = P.PartyID
                    LEFT JOIN Sectors SCT ON P.SectorID = SCT.SectorID
                    WHERE SL.SaleDate BETWEEN @FromDate AND @ToDate
                    AND SL.IsPosted = 1
                    AND SB.IsDeleted = 0";

                // Add filters
                if (!string.IsNullOrEmpty(customerId))
                    query += " AND P.PartyID = @CustomerID";

                if (!string.IsNullOrEmpty(companyId))
                    query += " AND C.CompanyID = @CompanyID";

                if (!string.IsNullOrEmpty(townId))
                    query += " AND P.City LIKE '%' + @TownName + '%'";

                if (!string.IsNullOrEmpty(sectorId) && int.TryParse(sectorId, out int sectorIdNum))
                    query += " AND SCT.SectorID = @SectorID";

                query += @"
                    GROUP BY P.PartyID, P.PartyName, P.City, SCT.SectorName, C.CompanyID, C.CompanyName
                    ORDER BY P.PartyName, P.City, SCT.SectorName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                if (!string.IsNullOrEmpty(customerId))
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                if (!string.IsNullOrEmpty(companyId))
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);

                if (!string.IsNullOrEmpty(townId))
                    cmd.Parameters.AddWithValue("@TownName", townnametxtbox.Text);

                if (!string.IsNullOrEmpty(sectorId) && int.TryParse(sectorId, out sectorIdNum))
                    cmd.Parameters.AddWithValue("@SectorID", sectorIdNum);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private bool ValidateInput()
        {
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate that if ID is entered, name must be fetched
            if (!string.IsNullOrEmpty(cstmridtxtbox.Text.Trim()) && string.IsNullOrEmpty(cstmrnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Customer Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text.Trim()) && string.IsNullOrEmpty(cmpnynametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Company Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrEmpty(townidtxtbox.Text.Trim()) && string.IsNullOrEmpty(townnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Town Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!string.IsNullOrEmpty(sectoridtxtbox.Text.Trim()) && string.IsNullOrEmpty(sectornametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Sector Name first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Empty event handlers
        private void cstmridtxtbox_TextChanged(object sender, EventArgs e) { }
        private void cstmrnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void cmpnynametxtbox_TextChanged(object sender, EventArgs e) { }
        private void townidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectoridtxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectornametxtbox_TextChanged(object sender, EventArgs e) { }
    }
}