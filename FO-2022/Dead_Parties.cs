using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Dead_Parties : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSectorId = "";

        public Dead_Parties()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-6); // Last 6 months
            todatepcker.Value = DateTime.Today;
        }

        private void Dead_Parties_Load(object sender, EventArgs e)
        {
            LoadTowns();
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void LoadTowns()
        {
            try
            {
                townlistView.Items.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["TownName"].ToString());
                                item.Tag = reader["TownID"].ToString();
                                townlistView.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading towns: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
                {
                    MessageBox.Show("Please enter Sector ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SectorName FROM Sectors WHERE SectorID = @SectorID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SectorID", sectoridtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            sectornametxtbox.Text = result.ToString();
                            selectedSectorId = sectoridtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Sector ID not found", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            sectornametxtbox.Clear();
                            selectedSectorId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectalltownfromlist_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in townlistView.Items)
            {
                item.Checked = true;
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected towns
                List<string> selectedTowns = new List<string>();
                foreach (ListViewItem item in townlistView.Items)
                {
                    if (item.Checked)
                    {
                        selectedTowns.Add(item.Tag.ToString());
                    }
                }

                // Get data
                DataTable dt = GetDeadPartiesData(selectedTowns);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No dead parties found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "SectorName", string.IsNullOrEmpty(sectornametxtbox.Text) ? "All Sectors" : sectornametxtbox.Text },
                    //{ "BalanceFilter", checkBoxHavingbalancegreaterthanzero.Checked ? "Balance > 0 Only" : "All Balances" }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Dead_Parties", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetDeadPartiesData(List<string> selectedTowns)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                WITH TransactionParties AS (
                    -- Get parties with transactions in the specified date range
                    SELECT DISTINCT VendorID as PartyID FROM Purchase 
                    WHERE PurchaseDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT VendorId as PartyID FROM PurReturns 
                    WHERE ReturnDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT CustomerId as PartyID FROM Sales 
                    WHERE SaleDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT CustomerId as PartyID FROM SaleReturns 
                    WHERE ReturnDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT AccountNo as PartyID FROM DebitVouchersBody dvb
                    INNER JOIN DebitVouchers dv ON dvb.VoucherNo = dv.VoucherNo
                    WHERE dv.VoucherDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT AccountNo as PartyID FROM CreditVouchersBody cvb
                    INNER JOIN CreditVouchers cv ON cvb.VoucherNo = cv.VoucherNo
                    WHERE cv.VoucherDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT AccountNo as PartyID FROM JournalVouchersBody jvb
                    INNER JOIN JournalVouchers jv ON jvb.VoucherNo = jv.VoucherNo
                    WHERE jv.VoucherDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT AccountNo as PartyID FROM BankCheques 
                    WHERE VoucherDate BETWEEN @FromDate AND @ToDate
                    UNION
                    SELECT DISTINCT AccountNo as PartyID FROM BankDeposit 
                    WHERE VoucherDate BETWEEN @FromDate AND @ToDate
                ),
                PartyBalance AS (
                    -- Calculate party balance (using AllVouchers)
                    SELECT 
                        AccountNo as PartyID,
                        SUM(ISNULL(Credit,0) - ISNULL(Debit,0)) as Balance
                    FROM AllVouchers
                    WHERE VoucherDate <= @ToDate
                    GROUP BY AccountNo
                )
                SELECT 
                    p.PartyID,
                    p.PartyName,
                    p.PartyType,
                    p.Address,
                    p.City,
                    p.Phone1,
                    s.SectorName,
                    ISNULL(pb.Balance, 0) as CurrentBalance
                FROM Parties p
                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                LEFT JOIN PartyBalance pb ON p.PartyID = pb.PartyID
                LEFT JOIN TransactionParties tp ON p.PartyID = tp.PartyID
                WHERE tp.PartyID IS NULL  -- Parties with NO transactions in the period
                AND p.PartyType IN ('C', 'V')  -- Customers and Vendors only
                ";

                // Add sector filter if selected
                if (!string.IsNullOrEmpty(selectedSectorId))
                {
                    query += " AND p.SectorID = @SectorID";
                }

                // Add towns filter if any town is selected
                if (selectedTowns.Count > 0)
                {
                    query += " AND s.TownID IN (";
                    for (int i = 0; i < selectedTowns.Count; i++)
                    {
                        query += $"@TownID{i}";
                        if (i < selectedTowns.Count - 1) query += ",";
                    }
                    query += ")";
                }

                // Add balance filter if checkbox checked
                if (checkBoxHavingbalancegreaterthanzero.Checked)
                {
                    query += " AND ISNULL(pb.Balance, 0) > 0";
                }

                query += " ORDER BY p.PartyName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));
                    cmd.CommandTimeout = 60;
                    if (!string.IsNullOrEmpty(selectedSectorId))
                    {
                        cmd.Parameters.AddWithValue("@SectorID", selectedSectorId);
                    }

                    // Add town parameters
                    if (selectedTowns.Count > 0)
                    {
                        for (int i = 0; i < selectedTowns.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@TownID{i}", selectedTowns[i]);
                        }
                    }

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sectoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            sectornametxtbox.Clear();
            selectedSectorId = "";
        }
    }
}