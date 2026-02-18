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
    public partial class Invoice_Wise_Profit : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Invoice_Wise_Profit()
        {
            InitializeComponent();
        }

        private void Invoice_Wise_Profit_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cstmridsearch_Click(object sender, EventArgs e)
        {
            string customerId = cstmridtxtbox.Text.Trim();
            string customerName = cstmrnametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(customerName))
            {
                MessageBox.Show("Please enter Customer ID or Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(customerId))
                    {
                        query = @"SELECT PartyID, PartyName 
                          FROM Parties 
                          WHERE PartyID = @Value 
                          AND PartyType = 'C'";
                    }
                    else
                    {
                        query = @"SELECT PartyID, PartyName 
                          FROM Parties 
                          WHERE PartyName = @Value 
                          AND PartyType = 'C'";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(customerId) ? customerId : customerName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cstmridtxtbox.Text = reader["PartyID"].ToString();
                                cstmrnametxtbox.Text = reader["PartyName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Customer not found!",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                cstmridtxtbox.Clear();
                                cstmrnametxtbox.Clear();
                                cstmridtxtbox.Focus();
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
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable invoiceProfitData = GetInvoiceWiseProfitData();

                if (invoiceProfitData.Rows.Count == 0)
                {
                    MessageBox.Show("No invoice profit data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", invoiceProfitData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Invoice Wise Profit Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("CustomerID", cstmridtxtbox.Text);
                //parameters.Add("TotalInvoices", invoiceProfitData.Rows.Count.ToString());

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Invoice_Wise_Profit", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private DataTable GetInvoiceWiseProfitData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        -- Get invoice sales data
                        WITH InvoiceSales AS (
                            SELECT 
                                S.SaleId,
                                S.SaleDate,
                                P.PartyName,
                                SUM(SB.TTLValue) AS SaleValue,
                                SUM(SB.QtyLose * SB.PerUnitCostLose) AS SaleCost
                            FROM Sales S
                            INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                            AND (@CustomerID IS NULL OR S.CustomerId = @CustomerID)
                            GROUP BY S.SaleId, S.SaleDate, P.PartyName
                        ),
                        -- Get invoice returns data
                        InvoiceReturns AS (
                            SELECT 
                                SR.SaleId,
                                SUM(SRB.TTLValue) AS ReturnValue,
                                SUM(SRB.QtyLose * SRB.PerUnitCostLose) AS ReturnCost
                            FROM SaleReturns SR
                            INNER JOIN SaleReturnsBody SRB ON SR.ReturnId = SRB.ReturnId
                            WHERE SR.ReturnDate BETWEEN @FromDate AND @ToDate
                            GROUP BY SR.SaleId
                        )
                        -- Calculate invoice profits
                        SELECT 
                            IS.SaleId,
                            IS.SaleDate,
                            IS.PartyName,
                            ISNULL(IS.SaleValue, 0) AS SaleValue,
                            ISNULL(IR.ReturnValue, 0) AS ReturnValue,
                            ISNULL(IS.SaleValue, 0) - ISNULL(IR.ReturnValue, 0) AS NetSale,
                            (ISNULL(IS.SaleValue, 0) - ISNULL(IS.SaleCost, 0)) - 
                            (ISNULL(IR.ReturnValue, 0) - ISNULL(IR.ReturnCost, 0)) AS Profit,
                            CASE 
                                WHEN (ISNULL(IS.SaleValue, 0) - ISNULL(IR.ReturnValue, 0)) > 0
                                THEN (((ISNULL(IS.SaleValue, 0) - ISNULL(IS.SaleCost, 0)) - 
                                      (ISNULL(IR.ReturnValue, 0) - ISNULL(IR.ReturnCost, 0))) / 
                                      (ISNULL(IS.SaleValue, 0) - ISNULL(IR.ReturnValue, 0))) * 100
                                ELSE 0
                            END AS ProfitPercentage
                        FROM InvoiceSales IS
                        LEFT JOIN InvoiceReturns IR ON IS.SaleId = IR.SaleId
                        ORDER BY IS.SaleDate DESC, IS.SaleId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    if (!string.IsNullOrEmpty(cstmridtxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", cstmridtxtbox.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", DBNull.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoice profit data: " + ex.Message);
            }

            return dt;
        }
    }
}