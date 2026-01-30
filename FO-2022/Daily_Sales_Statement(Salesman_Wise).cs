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
    public partial class Daily_Sales_Statement_Salesman_Wise_ : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSalesmanId = "";

        public Daily_Sales_Statement_Salesman_Wise_()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddYears(-3);
            todatepcker.Value = DateTime.Today;
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salesmanidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Salesman ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            salesmannametxtbox.Text = result.ToString();
                            selectedSalesmanId = salesmanidtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Salesman ID not found", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            salesmannametxtbox.Clear();
                            selectedSalesmanId = "";
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

                DataTable dt = GetDailySalesStatementData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var parameters = new Dictionary<string, string>
                {
                    //    { "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //    { "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //    { "SalesmanName", string.IsNullOrEmpty(salesmannametxtbox.Text) ? "All Salesmen" : salesmannametxtbox.Text },
                    //    { "ReportDate", DateTime.Today.ToString("dd-MMM-yyyy HH:mm") }
                };

                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Daily_Sales_Statement(Salesman_Wise)", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetDailySalesStatementData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    s.SaleDate,
                    s.BillNo,
                    p.PartyName as CustomerName,
                    sm.SalesmanName,
                    COUNT(DISTINCT sb.ProductId) as ProductCount,
                    SUM(sb.QtyPack * sb.Multiplier + sb.QtyLose) as TotalQty,
                    SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax) as TotalAmount,
                    s.ReceivedAmount,
                    s.CashReceived,
                    (SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax) - s.ReceivedAmount) as Balance,
                    CASE 
                        WHEN s.ReceivedAmount = 0 THEN 'Credit'
                        WHEN s.ReceivedAmount = SUM(sb.TTLValue + sb.TTLSTax + sb.TTLFTax) THEN 'Cash'
                        ELSE 'Partial'
                    END as PaymentStatus,
                    s.Narration
                FROM Sales s
                INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                INNER JOIN Parties p ON s.CustomerId = p.PartyID
                LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                AND s.IsPosted = 1";

                if (!string.IsNullOrEmpty(selectedSalesmanId))
                {
                    query += " AND s.SalesmanId = @SalesmanID";
                }

                query += @"
                GROUP BY s.SaleDate, s.BillNo, p.PartyName, sm.SalesmanName, 
                         s.ReceivedAmount, s.CashReceived, s.Narration
                ORDER BY s.SaleDate DESC, s.BillNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

                    if (!string.IsNullOrEmpty(selectedSalesmanId))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", selectedSalesmanId);
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
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Daily_Sales_Statement_Salesman_Wise__Load(object sender, EventArgs e)
        {
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
            selectedSalesmanId = "";
        }
    }
}