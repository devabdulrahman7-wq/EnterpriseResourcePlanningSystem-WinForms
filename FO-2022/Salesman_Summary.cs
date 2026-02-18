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
            try
            {
                string partyID = salesmanidtxtbox.Text.Trim();
                string partyName = salesmannametxtbox.Text.Trim();

                if (string.IsNullOrEmpty(partyID) && string.IsNullOrEmpty(partyName))
                {
                    MessageBox.Show("Please enter Party ID or Party Name",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT SalesmanID,SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID OR SalesmanName=@SalesmanID";


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID",
                            string.IsNullOrEmpty(partyID) ? (object)DBNull.Value : partyID);

                        cmd.Parameters.AddWithValue("@SalesmanID",
                            string.IsNullOrEmpty(partyName) ? (object)DBNull.Value : partyName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                salesmanidtxtbox.Text = reader["SalesmanID"].ToString();
                                salesmannametxtbox.Text = reader["SalesmanID"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Party not found!",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                salesmanidtxtbox.Clear();
                                salesmannametxtbox.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching party: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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