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
    public partial class Customer_Wise_Profits : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Customer_Wise_Profits()
        {
            InitializeComponent();
        }

        private void Customer_Wise_Profits_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
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

                DataTable profitData = GetCustomerWiseProfitData();

                if (profitData.Rows.Count == 0)
                {
                    MessageBox.Show("No profit data found for selected period", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", profitData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Customer Wise Profits Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("TotalCustomers", profitData.Rows.Count.ToString());

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Customer_Wise_Profits", dataSources);
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

        private DataTable GetCustomerWiseProfitData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        WITH SalesSummary AS (
                            SELECT 
                                P.PartyID,
                                P.PartyName,
                                SUM(SB.TTLValue) AS TotalSale,
                                SUM(SB.QtyLose * SB.PerUnitCostLose) AS TotalCost
                            FROM Sales S
                            INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                            GROUP BY P.PartyID, P.PartyName
                        ),
                        ReturnsSummary AS (
                            SELECT 
                                SR.CustomerId,
                                SUM(SRB.TTLValue) AS TotalReturn,
                                SUM(SRB.QtyLose * SRB.PerUnitCostLose) AS ReturnCost
                            FROM SaleReturns SR
                            INNER JOIN SaleReturnsBody SRB ON SR.ReturnId = SRB.ReturnId
                            WHERE SR.ReturnDate BETWEEN @FromDate AND @ToDate
                            GROUP BY SR.CustomerId
                        )
                        SELECT 
                            SS.PartyName,
                            ISNULL(SS.TotalSale, 0) AS TotalSale,
                            ISNULL(RS.TotalReturn, 0) AS TotalReturn,
                            ISNULL(SS.TotalSale, 0) - ISNULL(RS.TotalReturn, 0) AS NetSale,
                            (ISNULL(SS.TotalSale, 0) - ISNULL(SS.TotalCost, 0)) - 
                            (ISNULL(RS.TotalReturn, 0) - ISNULL(RS.ReturnCost, 0)) AS NetProfit,
                            CASE 
                                WHEN (ISNULL(SS.TotalSale, 0) - ISNULL(RS.TotalReturn, 0)) > 0
                                THEN (((ISNULL(SS.TotalSale, 0) - ISNULL(SS.TotalCost, 0)) - 
                                      (ISNULL(RS.TotalReturn, 0) - ISNULL(RS.ReturnCost, 0))) / 
                                      (ISNULL(SS.TotalSale, 0) - ISNULL(RS.TotalReturn, 0))) * 100
                                ELSE 0
                            END AS ProfitPercentage
                        FROM SalesSummary SS
                        LEFT JOIN ReturnsSummary RS ON SS.PartyID = RS.CustomerId
                        WHERE ISNULL(SS.TotalSale, 0) > 0
                        ORDER BY SS.PartyName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profit data: " + ex.Message);
            }

            return dt;
        }
    }
}