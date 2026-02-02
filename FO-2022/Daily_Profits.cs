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
    public partial class Daily_Profits : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Daily_Profits()
        {
            InitializeComponent();
        }

        private void Daily_Profits_Load(object sender, EventArgs e)
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

                DataTable profitData = GetDailyProfitData();

                if (profitData.Rows.Count == 0)
                {
                    MessageBox.Show("No profit data found for selected period", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", profitData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Daily Profits Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("TotalSales", profitData.Rows.Count.ToString());

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Daily_Profits", dataSources);
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

        private DataTable GetDailyProfitData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        -- Get sales data
                        WITH SalesData AS (
                            SELECT 
                                S.SaleId,
                                CONVERT(DATE, S.SaleDate) AS SaleDate,
                                P.PartyName,
                                SUM(SB.TTLValue) AS SaleValue,
                                SUM(SB.QtyLose * SB.PerUnitCostLose) AS SaleCost
                            FROM Sales S
                            INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                            INNER JOIN Parties P ON S.CustomerId = P.PartyID
                            WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                            GROUP BY S.SaleId, S.SaleDate, P.PartyName
                        ),
                        -- Get returns data
                        ReturnsData AS (
                            SELECT 
                                SR.SaleId,
                                SUM(SRB.TTLValue) AS ReturnValue,
                                SUM(SRB.QtyLose * SRB.PerUnitCostLose) AS ReturnCost
                            FROM SaleReturns SR
                            INNER JOIN SaleReturnsBody SRB ON SR.ReturnId = SRB.ReturnId
                            WHERE SR.ReturnDate BETWEEN @FromDate AND @ToDate
                            GROUP BY SR.SaleId
                        )
                        -- Calculate daily profits
                        SELECT 
                            SD.SaleId,
                            SD.SaleDate,
                            SD.PartyName,
                            ISNULL(SD.SaleValue, 0) AS SaleValue,
                            ISNULL(RD.ReturnValue, 0) AS ReturnValue,
                            ISNULL(SD.SaleValue, 0) - ISNULL(RD.ReturnValue, 0) AS NetSale,
                            (ISNULL(SD.SaleValue, 0) - ISNULL(SD.SaleCost, 0)) - 
                            (ISNULL(RD.ReturnValue, 0) - ISNULL(RD.ReturnCost, 0)) AS Profit,
                            CASE 
                                WHEN (ISNULL(SD.SaleValue, 0) - ISNULL(RD.ReturnValue, 0)) > 0
                                THEN (((ISNULL(SD.SaleValue, 0) - ISNULL(SD.SaleCost, 0)) - 
                                      (ISNULL(RD.ReturnValue, 0) - ISNULL(RD.ReturnCost, 0))) / 
                                      (ISNULL(SD.SaleValue, 0) - ISNULL(RD.ReturnValue, 0))) * 100
                                ELSE 0
                            END AS ProfitPercentage
                        FROM SalesData SD
                        LEFT JOIN ReturnsData RD ON SD.SaleId = RD.SaleId
                        ORDER BY SD.SaleDate, SD.PartyName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading daily profit data: " + ex.Message);
            }

            return dt;
        }
    }
}