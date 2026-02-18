using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sales_Recovery_Register : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_Recovery_Register()
        {
            InitializeComponent();
        }

        private void Sales_Recovery_Register_Load(object sender, EventArgs e)
        {
            try
            {
                fromdatepcker.Value = DateTime.Today.AddMonths(-1);
                todatepcker.Value = DateTime.Today;

                cstmrnametxtbox.Clear();
                salesmannametxtbox.Clear();
                townnametxtbox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // CUSTOMER SEARCH
        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            string customerId = cstmridtxtbox.Text.Trim();
            if (string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show("Please enter Customer ID.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cstmridtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT PartyName FROM Parties WHERE PartyID = @CustomerID", conn))
                {
                    cmd.Parameters.Add("@CustomerID", SqlDbType.VarChar).Value = customerId;
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        cstmrnametxtbox.Text = result.ToString();
                    else
                    {
                        cstmrnametxtbox.Clear();
                        MessageBox.Show("Customer not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customer: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
                cstmrnametxtbox.Clear();
        }

        // SALESMAN SEARCH
        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();
            if (string.IsNullOrWhiteSpace(salesmanId))
            {
                MessageBox.Show("Please enter Salesman ID.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salesmanidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID", conn))
                {
                    cmd.Parameters.Add("@SalesmanID", SqlDbType.VarChar).Value = salesmanId;
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        salesmannametxtbox.Text = result.ToString();
                    else
                    {
                        salesmannametxtbox.Clear();
                        MessageBox.Show("Salesman not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching salesman: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(salesmanidtxtbox.Text))
                salesmannametxtbox.Clear();
        }

        // TOWN SEARCH (ID or Name)
        private void townidsearch_Click(object sender, EventArgs e)
        {
            string townIdText = townidtxtbox.Text.Trim();
            string townNameText = townnametxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(townIdText) && string.IsNullOrWhiteSpace(townNameText))
            {
                MessageBox.Show("Please enter Town ID or Town Name.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            MessageBox.Show("Town ID must be numeric.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            townidtxtbox.Text = reader["TownID"].ToString();
                            townnametxtbox.Text = reader["TownName"].ToString();
                        }
                        else
                        {
                            townidtxtbox.Clear();
                            townnametxtbox.Clear();
                            MessageBox.Show("Town not found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            townidtxtbox.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching town: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void townidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(townidtxtbox.Text))
                townnametxtbox.Clear();
        }

        // VALIDATE FILTERS
        private bool ValidateFilters()
        {
            if (!string.IsNullOrWhiteSpace(cstmridtxtbox.Text) && string.IsNullOrWhiteSpace(cstmrnametxtbox.Text))
            {
                MessageBox.Show("Please search and validate Customer ID first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cstmridtxtbox.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(salesmanidtxtbox.Text) && string.IsNullOrWhiteSpace(salesmannametxtbox.Text))
            {
                MessageBox.Show("Please search and validate Salesman ID first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salesmanidtxtbox.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(townidtxtbox.Text) && string.IsNullOrWhiteSpace(townnametxtbox.Text))
            {
                MessageBox.Show("Please search and validate Town ID first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                townidtxtbox.Focus();
                return false;
            }

            return true;
        }

        // GENERATE REPORT
        private void GenerateReport()
        {
            if (!ValidateFilters()) return;

            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = GetRecoveryData(
                fromdatepcker.Value,
                todatepcker.Value,
                cstmridtxtbox.Text.Trim(),
                salesmanidtxtbox.Text.Trim(),
                townidtxtbox.Text.Trim()
            );

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No recovery records found for the selected criteria.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Reports.Report_viewer reportViewer = new Reports.Report_viewer();
            reportViewer.ShowReport("Sales_Recovery_Register", new Dictionary<string, DataTable> { { "DataSet1", dt } }, new Dictionary<string, string>());
        }

        private DataTable GetRecoveryData(DateTime fromDate, DateTime toDate,
                                         string customerId, string salesmanId, string townId)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("RecoveryID", typeof(int));
            dt.Columns.Add("RecoveryDate", typeof(DateTime));
            dt.Columns.Add("CustomerID", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("SalesmanID", typeof(string));
            dt.Columns.Add("SalesmanName", typeof(string));
            dt.Columns.Add("TownID", typeof(int));
            dt.Columns.Add("TownName", typeof(string));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("Narration", typeof(string));
            dt.Columns.Add("PaymentMode", typeof(string));
            dt.Columns.Add("CompanyID", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("VendorID", typeof(string));
            dt.Columns.Add("VendorName", typeof(string));

            try
            {
                string query = @"
                    SELECT 
                        r.RecoveryID,
                        r.RecoveryDate,
                        rb.CustomerID,
                        p.PartyName AS CustomerName,
                        r.SalesmanID,
                        sm.SalesmanName,
                        t.TownID,
                        t.TownName,
                        rb.Amount,
                        rb.Discount,
                        rb.Narration,
                        rb.PaymentMode,
                        rb.CompanyID,
                        c.CompanyName,
                        rb.VendorID,
                        p2.PartyName AS VendorName
                    FROM Recovery r
                    INNER JOIN RecoveryBody rb ON r.RecoveryID = rb.RecoveryID
                    LEFT JOIN Parties p ON rb.CustomerID = p.PartyID
                    LEFT JOIN Salesmen sm ON r.SalesmanID = sm.SalesmanID
                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                    LEFT JOIN Towns t ON s.TownID = t.TownID
                    LEFT JOIN Companies c ON rb.CompanyID = c.CompanyID
                    LEFT JOIN Parties p2 ON rb.VendorID = p2.PartyID
                    WHERE r.RecoveryDate BETWEEN @FromDate AND @ToDate
                        AND rb.IsDeleted = 0
                        AND rb.IsPosted = 1";

                if (!string.IsNullOrWhiteSpace(customerId))
                    query += " AND rb.CustomerID = @CustomerID";
                if (!string.IsNullOrWhiteSpace(salesmanId))
                    query += " AND r.SalesmanID = @SalesmanID";
                if (!string.IsNullOrWhiteSpace(townId))
                    query += " AND t.TownID = @TownID";

                query += " ORDER BY r.RecoveryDate, r.RecoveryID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    if (!string.IsNullOrWhiteSpace(customerId))
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    if (!string.IsNullOrWhiteSpace(salesmanId))
                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                    if (!string.IsNullOrWhiteSpace(townId))
                        cmd.Parameters.AddWithValue("@TownID", townId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            row["RecoveryID"] = reader["RecoveryID"];
                            row["RecoveryDate"] = reader["RecoveryDate"];
                            row["CustomerID"] = reader["CustomerID"];
                            row["CustomerName"] = reader["CustomerName"];
                            row["SalesmanID"] = reader["SalesmanID"];
                            row["SalesmanName"] = reader["SalesmanName"];
                            row["TownID"] = reader["TownID"] != DBNull.Value ? reader["TownID"] : 0;
                            row["TownName"] = reader["TownName"];
                            row["Amount"] = reader["Amount"];
                            row["Discount"] = reader["Discount"];
                            row["Narration"] = reader["Narration"];
                            row["PaymentMode"] = reader["PaymentMode"];
                            row["CompanyID"] = reader["CompanyID"];
                            row["CompanyName"] = reader["CompanyName"];
                            row["VendorID"] = reader["VendorID"];
                            row["VendorName"] = reader["VendorName"];
                            dt.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching recovery data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e) => this.Close();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();
    }
}
