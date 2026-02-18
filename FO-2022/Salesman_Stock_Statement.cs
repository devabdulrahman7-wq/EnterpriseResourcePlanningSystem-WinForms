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
    public partial class Salesman_Stock_Statement : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSalesmanId = "";

        public Salesman_Stock_Statement()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-1);
            todatepcker.Value = DateTime.Today;
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();
            string salesmanName = salesmannametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanId) && string.IsNullOrEmpty(salesmanName))
            {
                MessageBox.Show("Please enter Salesman ID or Name",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(salesmanId))
                    {
                        query = @"SELECT SalesmanID, SalesmanName 
                          FROM Salesmen 
                          WHERE SalesmanID = @Value";
                    }
                    else
                    {
                        query = @"SELECT SalesmanID, SalesmanName 
                          FROM Salesmen 
                          WHERE SalesmanName = @Value";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(salesmanId) ? salesmanId : salesmanName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                salesmanidtxtbox.Text = reader["SalesmanID"].ToString();
                                salesmannametxtbox.Text = reader["SalesmanName"].ToString();
                                selectedSalesmanId = reader["SalesmanID"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Salesman not found",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                salesmanidtxtbox.Clear();
                                salesmannametxtbox.Clear();
                                selectedSalesmanId = "";
                                salesmanidtxtbox.Focus();
                            }
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
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (string.IsNullOrEmpty(selectedSalesmanId))
                {
                    MessageBox.Show("Please select a salesman first", "Salesman Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dt = GetSalesmanStockStatementData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No stock data found for selected salesman and period",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "SalesmanName", salesmannametxtbox.Text },
                    //{ "SalesmanID", selectedSalesmanId },
                    //{ "ReportDate", DateTime.Today.ToString("dd-MMM-yyyy") }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Salesman_Stock_Statement", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSalesmanStockStatementData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    c.CompanyName,
                    
                    -- Opening Stock (from SMOpeningStock)
                    ISNULL(os.QtyLose, 0) as OpeningStock,
                    ISNULL(os.PerUnitCostLose, 0) as OpeningCost,
                    
                    -- Stock Issued (from IssueStock)
                    ISNULL(iss.TotalIssued, 0) as IssuedStock,
                    
                    -- Stock Returned (from StockReturn)
                    ISNULL(ret.TotalReturned, 0) as ReturnedStock,
                    
                    -- Current Stock (from SMCurrentStock)
                    ISNULL(cs.QtyLose, 0) as CurrentStock,
                    ISNULL(cs.PerUnitCostLose, 0) as CurrentCost,
                    
                    -- Calculations
                    ISNULL(os.QtyLose, 0) + ISNULL(iss.TotalIssued, 0) - ISNULL(ret.TotalReturned, 0) as CalculatedStock,
                    (ISNULL(os.QtyLose, 0) * ISNULL(os.PerUnitCostLose, 0)) as OpeningValue,
                    (ISNULL(cs.QtyLose, 0) * ISNULL(cs.PerUnitCostLose, 0)) as CurrentValue
                    
                FROM Products p
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                
                LEFT JOIN (
                    SELECT ProductId, QtyLose, PerUnitCostLose 
                    FROM SMOpeningStock 
                    WHERE SalesmanId = @SalesmanID
                ) os ON p.ProductID = os.ProductId
                
                LEFT JOIN (
                    SELECT 
                        isb.ProductID,
                        SUM(isb.QtyPack * isb.Multiplier + isb.QtyLose) as TotalIssued
                    FROM IssueStockBody isb
                    INNER JOIN IssueStock iss ON isb.IssueID = iss.IssueID
                    WHERE iss.PartyID = @SalesmanID
                    AND iss.IssueDate BETWEEN @FromDate AND @ToDate
                    GROUP BY isb.ProductID
                ) iss ON p.ProductID = iss.ProductID
                
                LEFT JOIN (
                    SELECT 
                        srb.ProductID,
                        SUM(srb.QtyPack * srb.Multiplier + srb.QtyLose) as TotalReturned
                    FROM StockReturnBody srb
                    INNER JOIN StockReturn sr ON srb.ReturnID = sr.ReturnId
                    WHERE sr.SalesmanID = @SalesmanID
                    AND sr.ReturnDate BETWEEN @FromDate AND @ToDate
                    GROUP BY srb.ProductID
                ) ret ON p.ProductID = ret.ProductID
                
                LEFT JOIN (
                    SELECT ProductId, QtyLose, PerUnitCostLose 
                    FROM SMCurrentStock 
                    WHERE SalesmanId = @SalesmanID
                ) cs ON p.ProductID = cs.ProductId
                
                WHERE (ISNULL(os.QtyLose, 0) > 0 
                       OR ISNULL(iss.TotalIssued, 0) > 0 
                       OR ISNULL(ret.TotalReturned, 0) > 0 
                       OR ISNULL(cs.QtyLose, 0) > 0)
                ORDER BY c.CompanyName, p.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SalesmanID", selectedSalesmanId);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

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

        private void Salesman_Stock_Statement_Load(object sender, EventArgs e)
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