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
    public partial class Stock_Going_to_Short : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Stock_Going_to_Short()
        {
            InitializeComponent();
        }

        private void Stock_Going_to_Short_Load(object sender, EventArgs e)
        {
            checkBoxincludecurrentstock.Checked = true;
            checkBox2showonlyorderlevelproducts.Checked = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            string companyId = cmpnyidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(companyId))
            {
                MessageBox.Show("Please enter Company ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmpnyidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CompanyID", companyId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        cmpnynametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Company not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cmpnynametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching company: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable stockData = GetStockGoingShortData();

                if (stockData.Rows.Count == 0)
                {
                    MessageBox.Show("No stock going short data found", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", stockData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Stock Going to Short Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("CompanyName", cmpnynametxtbox.Text);
                //parameters.Add("CompanyID", cmpnyidtxtbox.Text);
                //parameters.Add("IncludeCurrentStock", checkBoxincludecurrentstock.Checked ? "Yes" : "No");
                //parameters.Add("ShowOnlyOrderLevel", checkBox2showonlyorderlevelproducts.Checked ? "Yes" : "No");

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Stock_Going_to_Short", dataSources );
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

        private DataTable GetStockGoingShortData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT ");
                    query.AppendLine("    P.ProductID,");
                    query.AppendLine("    P.ProductName,");
                    query.AppendLine("    C.CompanyName,");
                    query.AppendLine("    P.MinStockLevel,");
                    query.AppendLine("    P.OrderStockLevel,");
                    query.AppendLine("    ISNULL(CS.QtyLose, 0) AS CurrentQty,");
                    query.AppendLine("    CASE ");
                    query.AppendLine("        WHEN ISNULL(CS.QtyLose, 0) <= P.OrderStockLevel THEN 'ORDER LEVEL'");
                    query.AppendLine("        WHEN ISNULL(CS.QtyLose, 0) <= P.MinStockLevel THEN 'MIN LEVEL'");
                    query.AppendLine("        ELSE 'SAFE'");
                    query.AppendLine("    END AS StockStatus,");
                    query.AppendLine("    P.OrderStockLevel - ISNULL(CS.QtyLose, 0) AS RequiredQty");
                    query.AppendLine("FROM Products P");
                    query.AppendLine("INNER JOIN Companies C ON P.CompanyID = C.CompanyID");
                    query.AppendLine("LEFT JOIN CurrentStock CS ON P.ProductID = CS.ProductiD");
                    query.AppendLine("WHERE 1=1");

                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                    {
                        query.AppendLine("AND C.CompanyID = @CompanyID");
                    }

                    if (checkBox2showonlyorderlevelproducts.Checked)
                    {
                        query.AppendLine("AND ISNULL(CS.QtyLose, 0) <= P.OrderStockLevel");
                    }

                    query.AppendLine("ORDER BY C.CompanyName, P.ProductName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);

                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                    {
                        cmd.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
    }
}