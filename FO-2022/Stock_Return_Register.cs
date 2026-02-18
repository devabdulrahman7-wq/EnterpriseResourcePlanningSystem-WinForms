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
    public partial class Stock_Return_Register : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSalesmanId = "";

        public Stock_Return_Register()
        {
            InitializeComponent();
        }

        private void Stock_Return_Register_Load(object sender, EventArgs e)
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
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable stockReturnData = GetStockReturnData();

                if (stockReturnData.Rows.Count == 0)
                {
                    MessageBox.Show("No stock return data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", stockReturnData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ReportTitle", "Stock Return Register");
                parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                parameters.Add("FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy"));
                parameters.Add("ToDate", todatepcker.Value.ToString("dd-MMM-yyyy"));
                parameters.Add("SalesmanName", salesmannametxtbox.Text);
                parameters.Add("SalesmanID", salesmanidtxtbox.Text);
                parameters.Add("ShowCost", checkBoxshowcost.Checked ? "Yes" : "No");

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Stock_Return_Register", dataSources, parameters);
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

        // GetStockReturnData method me sirf 1 query use karein:
        private DataTable GetStockReturnData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    SR.ReturnId,
                    SR.Returndate,
                    S.SalesmanName,
                    SRB.ProductID,
                    P.ProductName,
                    PK.PackingName,
                    SRB.QtyLose,
                    SRB.PerUnitCostLose,
                    SRB.TTLValue
                FROM StockReturn SR
                INNER JOIN StockReturnBody SRB ON SR.ReturnId = SRB.ReturnID
                INNER JOIN Salesmen S ON SR.SalesmanID = S.SalesmanID
                INNER JOIN Products P ON SRB.ProductID = P.ProductID
                LEFT JOIN Packings PK ON SRB.PackingId = PK.PackingId
                WHERE SR.Returndate BETWEEN @FromDate AND @ToDate
                AND (@SalesmanID IS NULL OR SR.SalesmanID = @SalesmanID)
                ORDER BY SR.Returndate, S.SalesmanName, P.ProductName";

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
                MessageBox.Show("Error loading stock return data: " + ex.Message);
            }

            return dt;
        }
    }
}