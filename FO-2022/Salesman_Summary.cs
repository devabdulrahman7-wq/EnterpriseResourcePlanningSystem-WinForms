//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Windows.Forms;
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Salesman_Summary : Form
//    {
//        private string connectionString;

//        public Salesman_Summary()
//        {
//            InitializeComponent();
//            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//        }

//        private void Salesman_Summary_Load(object sender, EventArgs e)
//        {
//            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
//            todatepcker.Value = DateTime.Now;
//        }

//        private void previewbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Validate dates
//                if (fromdatepcker.Value > todatepcker.Value)
//                {
//                    MessageBox.Show("From Date cannot be greater than To Date!", 
//                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    return;
//                }

//                // Get data from database
//                DataTable dt = GetReportData();

//                if (dt == null || dt.Rows.Count == 0)
//                {
//                    MessageBox.Show("No data found for selected criteria!", 
//                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    return;
//                }

//                // Create and show report window
//                Salesman_Summary_win_ reportWindow = new Salesman_Summary_win_();

//                // Call SetReportData method
//                reportWindow.SetReportData(
//                    dt, 
//                    fromdatepcker.Value, 
//                    todatepcker.Value,
//                    string.IsNullOrEmpty(salesmannametxtbox.Text) ? "All Salesmen" : salesmannametxtbox.Text
//                );

//                reportWindow.Show();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error: " + ex.Message, "Error", 
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // ... rest of your existing events (printbtn_Click, closebtn_Click, etc.)
//        private void printbtn_Click(object sender, EventArgs e)
//        {
//            MessageBox.Show("Please generate report first using Preview button", 
//                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void salesmanidsearch_Click(object sender, EventArgs e)
//        {
//            // Your existing search code
//            string salesmanId = salesmanidtxtbox.Text.Trim();

//            if (string.IsNullOrEmpty(salesmanId))
//            {
//                MessageBox.Show("Please enter Salesman ID", "Info", 
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

//                    conn.Open();
//                    object result = cmd.ExecuteScalar();

//                    if (result != null)
//                        salesmannametxtbox.Text = result.ToString();
//                    else
//                        MessageBox.Show("Salesman not found!", "Info", 
//                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Search error: " + ex.Message, "Error", 
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void pictureBox4_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private DataTable GetReportData()
//        {
//            DataTable dt = new DataTable("SalesmanSummary");

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"
//                        SELECT 
//                            p.PartyID AS CustomerID,
//                            p.PartyName AS CustomerName,
//                            ISNULL(SUM(sb.TTLValue), 0) AS SaleValue,
//                            ISNULL(SUM(CASE WHEN sr.ReturnDate BETWEEN @FromDate AND @ToDate THEN srb.TTLValue ELSE 0 END), 0) AS ReturnValue,
//                            ISNULL(SUM(s.ReceivedAmount), 0) AS ReceivedAmount,
//                            ISNULL(sm.SalesmanName, 'Not Assigned') AS SalesmanName
//                        FROM Parties p
//                        LEFT JOIN Sales s ON p.PartyID = s.CustomerId 
//                            AND s.SaleDate BETWEEN @FromDate AND @ToDate
//                            AND s.IsPosted = 1
//                        LEFT JOIN SalesBody sb ON s.SaleId = sb.SaleId 
//                            AND sb.IsDeleted = 0
//                        LEFT JOIN SaleReturns sr ON p.PartyID = sr.CustomerId 
//                            AND sr.ReturnDate BETWEEN @FromDate AND @ToDate
//                            AND sr.IsPosted = 1
//                        LEFT JOIN SaleReturnsBody srb ON sr.ReturnId = srb.ReturnId 
//                            AND srb.IsDeleted = 0
//                        LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
//                        WHERE p.PartyType = 'C'
//                            AND (@SalesmanID IS NULL OR sm.SalesmanID = @SalesmanID)
//                        GROUP BY p.PartyID, p.PartyName, sm.SalesmanName
//                        HAVING ISNULL(SUM(sb.TTLValue), 0) > 0
//                        ORDER BY p.PartyName";

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
//                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

//                    if (!string.IsNullOrEmpty(salesmanidtxtbox.Text.Trim()))
//                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text.Trim());
//                    else
//                        cmd.Parameters.AddWithValue("@SalesmanID", DBNull.Value);

//                    conn.Open();
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    // Create columns matching Dataset
//                    dt.Columns.Add("CustomerID", typeof(string));
//                    dt.Columns.Add("CustomerName", typeof(string));
//                    dt.Columns.Add("SaleValue", typeof(decimal));
//                    dt.Columns.Add("ReturnValue", typeof(decimal));
//                    dt.Columns.Add("NetValue", typeof(decimal));
//                    dt.Columns.Add("ReturnPercentage", typeof(decimal));
//                    dt.Columns.Add("ReceivedAmount", typeof(decimal));
//                    dt.Columns.Add("BalanceAmount", typeof(decimal));
//                    dt.Columns.Add("SalesmanName", typeof(string));

//                    while (reader.Read())
//                    {
//                        decimal saleValue = Convert.ToDecimal(reader["SaleValue"]);
//                        decimal returnValue = Convert.ToDecimal(reader["ReturnValue"]);
//                        decimal receivedAmount = Convert.ToDecimal(reader["ReceivedAmount"]);

//                        // Calculations
//                        decimal netValue = saleValue - returnValue;
//                        decimal returnPercentage = (saleValue > 0) ? (returnValue * 100 / saleValue) : 0;
//                        decimal balanceAmount = netValue - receivedAmount;

//                        DataRow row = dt.NewRow();
//                        row["CustomerID"] = reader["CustomerID"].ToString();
//                        row["CustomerName"] = reader["CustomerName"].ToString();
//                        row["SaleValue"] = saleValue;
//                        row["ReturnValue"] = returnValue;
//                        row["NetValue"] = netValue;
//                        row["ReturnPercentage"] = returnPercentage;
//                        row["ReceivedAmount"] = receivedAmount;
//                        row["BalanceAmount"] = balanceAmount;
//                        row["SalesmanName"] = reader["SalesmanName"].ToString();

//                        dt.Rows.Add(row);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Database error: " + ex.Message, "Error", 
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }

//            return dt;
//        }

//        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
//        {
//            // Optional: Add date validation or status update
//        }

//        private void todatepcker_ValueChanged(object sender, EventArgs e)
//        {
//            // Optional: Add date validation or status update
//        }

//        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Clear name when ID changes
//            if (string.IsNullOrEmpty(salesmanidtxtbox.Text))
//                salesmannametxtbox.Clear();
//        }
//    }
//}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace FO_2022
{
    public partial class Salesman_Summary : Form
    {
        private string connectionString;

        public Salesman_Summary()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void Salesman_Summary_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From Date cannot be greater than To Date!");
                    return;
                }

                DataTable dt = GetReportData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No data found!");
                    return;
                }

                Salesman_Summary_win_ reportWindow = new Salesman_Summary_win_();

                reportWindow.SetReportData(
                    dt,
                    fromdatepcker.Value,
                    todatepcker.Value,
                    string.IsNullOrEmpty(salesmannametxtbox.Text) ? "All Salesmen" : salesmannametxtbox.Text
                );

                reportWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private DataTable GetReportData()
        {
            DataTable dt = new DataTable("SalesmanSummary");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query to get salesman summary
                    string query = @"
                        SELECT 
                            p.PartyID AS CustomerID,
                            p.PartyName AS CustomerName,
                            ISNULL(SUM(sb.TTLValue), 0) AS SaleValue,
                            0 AS ReturnValue,
                            ISNULL(SUM(s.ReceivedAmount), 0) AS ReceivedAmount,
                            ISNULL(sm.SalesmanName, 'Not Assigned') AS SalesmanName
                        FROM Parties p
                        LEFT JOIN Sales s ON p.PartyID = s.CustomerId 
                            AND s.SaleDate BETWEEN @FromDate AND @ToDate
                            AND s.IsPosted = 1
                        LEFT JOIN SalesBody sb ON s.SaleId = sb.SaleId 
                            AND sb.IsDeleted = 0
                        LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                        WHERE p.PartyType = 'C'
                            AND (ISNULL(@SalesmanID, '') = '' OR sm.SalesmanID = @SalesmanID)
                        GROUP BY p.PartyID, p.PartyName, sm.SalesmanName
                        HAVING ISNULL(SUM(sb.TTLValue), 0) > 0
                        ORDER BY p.PartyName";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add parameters
                    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromdatepcker.Value;
                    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = todatepcker.Value;

                    // SalesmanID parameter
                    if (!string.IsNullOrEmpty(salesmanidtxtbox.Text.Trim()))
                    {
                        cmd.Parameters.Add("@SalesmanID", SqlDbType.VarChar, 10).Value = salesmanidtxtbox.Text.Trim();
                    }
                    else
                    {
                        cmd.Parameters.Add("@SalesmanID", SqlDbType.VarChar, 10).Value = DBNull.Value;
                    }

                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    // Add calculated columns
                    if (dt.Columns["NetValue"] == null)
                        dt.Columns.Add("NetValue", typeof(decimal));
                    if (dt.Columns["ReturnPercentage"] == null)
                        dt.Columns.Add("ReturnPercentage", typeof(decimal));
                    if (dt.Columns["BalanceAmount"] == null)
                        dt.Columns.Add("BalanceAmount", typeof(decimal));

                    // Calculate values for each row
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal saleValue = Convert.ToDecimal(row["SaleValue"]);
                        decimal returnValue = Convert.ToDecimal(row["ReturnValue"]);
                        decimal receivedAmount = Convert.ToDecimal(row["ReceivedAmount"]);

                        // Calculations
                        decimal netValue = saleValue - returnValue;
                        decimal returnPercentage = (saleValue > 0) ? (returnValue * 100 / saleValue) : 0;
                        decimal balanceAmount = netValue - receivedAmount;

                        row["NetValue"] = netValue;
                        row["ReturnPercentage"] = Math.Round(returnPercentage, 2);
                        row["BalanceAmount"] = balanceAmount;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }

            return dt;
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Generate report first using Preview button");
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanId))
            {
                MessageBox.Show("Please enter Salesman ID");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@SalesmanID", SqlDbType.VarChar, 10).Value = salesmanId;

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        salesmannametxtbox.Text = result.ToString();
                    else
                        MessageBox.Show("Salesman not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e) { }
        private void todatepcker_ValueChanged(object sender, EventArgs e) { }
        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e) { }
    }
}