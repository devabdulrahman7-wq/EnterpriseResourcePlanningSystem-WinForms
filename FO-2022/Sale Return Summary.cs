using FO_2022.Reports;
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
    public partial class Sale_Return_Summary : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sale_Return_Summary()
        {
            InitializeComponent();
        }

        private void Sale_Return_Summary_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;

            // Clear textboxes
            ClearSearchFields();
        }

        private void ClearSearchFields()
        {
            cstmridtxtbox.Text = "";
            cstmrnametxtbox.Text = "";
            townidtxtbox.Text = "";
            townnametxtbox.Text = "";
            sectoridtxtbox.Text = "";
            sectornametxtbox.Text = "";
        }

        // Customer ID Search
        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            string customerId = cstmridtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show("Please enter Customer ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID AND PartyType = 'C'";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", customerId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            cstmrnametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Customer not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cstmrnametxtbox.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Town ID Search
        private void townidsearch_Click(object sender, EventArgs e)
        {
            string townText = townidtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(townText))
            {
                MessageBox.Show("Please enter Town ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TownName FROM Towns WHERE TownID = @TownID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Try parse, agar nahi ho saka to 0 bhejo (0 ID nahi hogi database me)
                        int townId;
                        if (!int.TryParse(townText, out townId))
                        {
                            townId = 0; // Invalid input = 0 (not found)
                        }

                        cmd.Parameters.AddWithValue("@TownID", townId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            townnametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Town not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            townnametxtbox.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sector ID Search
        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            string sectorText = sectoridtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(sectorText))
            {
                MessageBox.Show("Please enter Sector ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SectorName FROM Sectors WHERE SectorID = @SectorID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Try parse, agar nahi ho saka to 0 bhejo
                        int sectorId;
                        if (!int.TryParse(sectorText, out sectorId))
                        {
                            sectorId = 0; // Invalid input = 0 (not found)
                        }

                        cmd.Parameters.AddWithValue("@SectorID", sectorId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            sectornametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Sector not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            sectornametxtbox.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Preview Button
        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(false);
        }

        // Print Button
        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(true);
        }

        private void GenerateReport(bool directPrint)
        {
            try
            {
                // Validate dates
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From Date cannot be greater than To Date", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get filter parameters - SAFE WAY (no errors)
                string customerId = string.IsNullOrWhiteSpace(cstmridtxtbox.Text) ? null : cstmridtxtbox.Text.Trim();

                int? townId = null;
                if (!string.IsNullOrWhiteSpace(townidtxtbox.Text))
                {
                    int tempTownId;
                    if (int.TryParse(townidtxtbox.Text.Trim(), out tempTownId))
                        townId = tempTownId;
                    // else: ignore (townId remains null)
                }

                int? sectorId = null;
                if (!string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
                {
                    int tempSectorId;
                    if (int.TryParse(sectoridtxtbox.Text.Trim(), out tempSectorId))
                        sectorId = tempSectorId;
                    // else: ignore (sectorId remains null)
                }

                // Fetch data
                DataTable dt = GetSaleReturnData(
                    fromdatepcker.Value,
                    todatepcker.Value,
                    customerId,
                    townId,
                    sectorId
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare data sources and parameters
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                var parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Sale Return Summary" },
                    { "FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy") },
                    { "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "CustomerFilter", string.IsNullOrEmpty(cstmrnametxtbox.Text) ? "All Customers" : $"Customer: {cstmrnametxtbox.Text}" },
                    //{ "TownFilter", string.IsNullOrEmpty(townnametxtbox.Text) ? "All Towns" : $"Town: {townnametxtbox.Text}" },
                    //{ "SectorFilter", string.IsNullOrEmpty(sectornametxtbox.Text) ? "All Sectors" : $"Sector: {sectornametxtbox.Text}" }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Sale_Return_Summary", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSaleReturnData(DateTime fromDate, DateTime toDate,
                                          string customerId, int? townId, int? sectorId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Build query dynamically based on filters
                    StringBuilder query = new StringBuilder();
                    query.AppendLine(@"
                        SELECT 
                            SR.ReturnId,
                            SR.ReturnDate,
                            SR.SaleId,
                            P.PartyName AS CustomerName,
                            SUM(SRB.TTLValue) AS GrossValue,
                            SUM(SRB.DiscValPack + SRB.DiscValLose) AS Discount,
                            SUM(SRB.TTLSTax) AS Sales,
                            SUM(SRB.TTLValue - (SRB.DiscValPack + SRB.DiscValLose) + SRB.TTLSTax) AS NetValue
                        FROM SaleReturns SR
                        INNER JOIN SaleReturnsBody SRB ON SR.ReturnId = SRB.ReturnId
                        INNER JOIN Parties P ON SR.CustomerId = P.PartyID AND P.PartyType = 'C'
                        LEFT JOIN Sectors S ON P.SectorID = S.SectorID
                        LEFT JOIN Towns T ON S.TownID = T.TownID
                        WHERE SR.ReturnDate BETWEEN @FromDate AND @ToDate
                    ");

                    // Add filters if provided
                    if (!string.IsNullOrEmpty(customerId))
                        query.AppendLine(" AND SR.CustomerId = @CustomerId");

                    if (townId.HasValue)
                        query.AppendLine(" AND T.TownID = @TownId");

                    if (sectorId.HasValue)
                        query.AppendLine(" AND S.SectorID = @SectorId");

                    query.AppendLine(@"
                        GROUP BY SR.ReturnId, SR.ReturnDate, SR.SaleId, P.PartyName
                        ORDER BY SR.ReturnDate
                    ");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        if (!string.IsNullOrEmpty(customerId))
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);

                        if (townId.HasValue)
                            cmd.Parameters.AddWithValue("@TownId", townId.Value);

                        if (sectorId.HasValue)
                            cmd.Parameters.AddWithValue("@SectorId", sectorId.Value);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching data: {ex.Message}", ex);
            }

            return dt;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // TextChanged events
        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
                cstmrnametxtbox.Text = "";
        }

        private void townidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(townidtxtbox.Text))
                townnametxtbox.Text = "";
        }

        private void sectoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
                sectornametxtbox.Text = "";
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Validate date range
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Validate date range
        }

        private void cstmrnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void townnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sectornametxtbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}