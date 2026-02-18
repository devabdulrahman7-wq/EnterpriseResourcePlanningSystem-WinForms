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
    public partial class Product_Wise_Profits : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Product_Wise_Profits()
        {
            InitializeComponent();
        }

        private void Product_Wise_Profits_Load(object sender, EventArgs e)
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
                cstmridtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    // ID priority
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
                                MessageBox.Show("Customer not found",
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
                MessageBox.Show("Error searching customer: " + ex.Message,
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

                DataTable productProfitData = GetProductWiseProfitData();

                if (productProfitData.Rows.Count == 0)
                {
                    MessageBox.Show("No product profit data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", productProfitData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Product Wise Profits Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("CustomerName", cstmrnametxtbox.Text);
                //parameters.Add("CustomerID", cstmridtxtbox.Text);
                //parameters.Add("TotalProducts", productProfitData.Rows.Count.ToString());

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Product_Wise_Profits", dataSources);
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

        private DataTable GetProductWiseProfitData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        -- Get product sales data
                        WITH ProductSales AS (
                            SELECT 
                                P.ProductID,
                                P.ProductName,
                                C.CompanyName,
                                PG.GroupName,
                                SUM(SB.QtyLose) AS SaleQty,
                                SUM(SB.TTLValue) AS SaleValue,
                                SUM(SB.QtyLose * SB.PerUnitCostLose) AS SaleCost
                            FROM Sales S
                            INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                            INNER JOIN Products P ON SB.ProductId = P.ProductID
                            INNER JOIN Companies C ON P.CompanyID = C.CompanyID
                            INNER JOIN ProductGroups PG ON P.CompanyID = PG.CompanyId AND P.GroupId = PG.GroupId
                            WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                            AND (@CustomerID IS NULL OR S.CustomerId = @CustomerID)
                            GROUP BY P.ProductID, P.ProductName, C.CompanyName, PG.GroupName
                        ),
                        -- Get product returns data
                        ProductReturns AS (
                            SELECT 
                                P.ProductID,
                                SUM(SRB.QtyLose) AS ReturnQty,
                                SUM(SRB.TTLValue) AS ReturnValue,
                                SUM(SRB.QtyLose * SRB.PerUnitCostLose) AS ReturnCost
                            FROM SaleReturns SR
                            INNER JOIN SaleReturnsBody SRB ON SR.ReturnId = SRB.ReturnId
                            INNER JOIN Products P ON SRB.ProductId = P.ProductID
                            WHERE SR.ReturnDate BETWEEN @FromDate AND @ToDate
                            AND (@CustomerID IS NULL OR SR.CustomerId = @CustomerID)
                            GROUP BY P.ProductID
                        )
                        -- Calculate product profits
                        SELECT 
                            PS.ProductID,
                            PS.ProductName,
                            PS.CompanyName,
                            PS.GroupName,
                            ISNULL(PS.SaleQty, 0) AS SaleQty,
                            ISNULL(PS.SaleValue, 0) AS SaleValue,
                            ISNULL(PR.ReturnQty, 0) AS ReturnQty,
                            ISNULL(PR.ReturnValue, 0) AS ReturnValue,
                            ISNULL(PS.SaleValue, 0) - ISNULL(PR.ReturnValue, 0) AS NetSale,
                            (ISNULL(PS.SaleValue, 0) - ISNULL(PS.SaleCost, 0)) - 
                            (ISNULL(PR.ReturnValue, 0) - ISNULL(PR.ReturnCost, 0)) AS Profit,
                            CASE 
                                WHEN (ISNULL(PS.SaleValue, 0) - ISNULL(PR.ReturnValue, 0)) > 0
                                THEN (((ISNULL(PS.SaleValue, 0) - ISNULL(PS.SaleCost, 0)) - 
                                      (ISNULL(PR.ReturnValue, 0) - ISNULL(PR.ReturnCost, 0))) / 
                                      (ISNULL(PS.SaleValue, 0) - ISNULL(PR.ReturnValue, 0))) * 100
                                ELSE 0
                            END AS ProfitPercentage
                        FROM ProductSales PS
                        LEFT JOIN ProductReturns PR ON PS.ProductID = PR.ProductID
                        ORDER BY PS.CompanyName, PS.GroupName, PS.ProductName";

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
                MessageBox.Show("Error loading product profit data: " + ex.Message);
            }

            return dt;
        }
    }
}