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
    public partial class Stock_Issue_Register : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Stock_Issue_Register()
        {
            InitializeComponent();
        }

        private void Stock_Issue_Register_Load(object sender, EventArgs e)
        {
            fromdatepcker.Value = DateTime.Now.AddMonths(-1);
            todatepcker.Value = DateTime.Now;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanId))
            {
                MessageBox.Show("Please enter Salesman ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                salesmanidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        salesmannametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Salesman not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        salesmannametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching salesman: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                DataTable stockIssueData = GetStockIssueData();

                if (stockIssueData.Rows.Count == 0)
                {
                    MessageBox.Show("No stock issue data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", stockIssueData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Stock Issue Register");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                //parameters.Add("SalesmanName", salesmannametxtbox.Text);
                //parameters.Add("SalesmanID", salesmanidtxtbox.Text);
                //parameters.Add("ShowCost", checkBox1.Checked ? "Yes" : "No");

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Stock_Issue_Register", dataSources );
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

        private DataTable GetStockIssueData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            SI.IssueID,
                            SI.IssueDate,
                            S.SalesmanName,
                            SIB.ProductID,
                            P.ProductName,
                            PK.PackingName,
                            SIB.QtyLose,
                            SIB.PerUnitCostLose,
                            SIB.TTLValue
                        FROM StockIssue SI
                        INNER JOIN StockIssueBody SIB ON SI.IssueID = SIB.IssueID
                        INNER JOIN Salesmen S ON SI.SalesmanID = S.SalesmanID
                        INNER JOIN Products P ON SIB.ProductID = P.ProductID
                        LEFT JOIN Packings PK ON SIB.PackingId = PK.PackingId
                        WHERE SI.IssueDate BETWEEN @FromDate AND @ToDate
                        AND (@SalesmanID IS NULL OR SI.SalesmanID = @SalesmanID)
                        ORDER BY SI.IssueDate, S.SalesmanName, P.ProductName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);
                    cmd.Parameters.AddWithValue("@SalesmanID",
                        string.IsNullOrEmpty(salesmanidtxtbox.Text) ? (object)DBNull.Value : salesmanidtxtbox.Text);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock issue data: " + ex.Message);
            }

            return dt;
        }
    }
}