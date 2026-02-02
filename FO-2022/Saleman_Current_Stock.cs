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
    public partial class Saleman_Current_Stock : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Saleman_Current_Stock()
        {
            InitializeComponent();
        }

        private void Saleman_Current_Stock_Load(object sender, EventArgs e)
        {
            // Optional: Set default values or load initial data
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

        private void salesmansearch_Click(object sender, EventArgs e)
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
                DataTable salesmanStockData = GetSalesmanCurrentStockData();

                if (salesmanStockData.Rows.Count == 0)
                {
                    MessageBox.Show("No stock data found for selected criteria", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", salesmanStockData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Salesman Current Stock Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("CompanyName", cmpnynametxtbox.Text);
                //parameters.Add("CompanyID", cmpnyidtxtbox.Text);
                //parameters.Add("SalesmanName", salesmannametxtbox.Text);
                //parameters.Add("SalesmanID", salesmanidtxtbox.Text);
                //parameters.Add("ShowCost", checkBoxshowcost.Checked ? "Yes" : "No");

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Saleman_Current_Stock", dataSources);
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

        private DataTable GetSalesmanCurrentStockData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            S.SalesmanID,
                            S.SalesmanName,
                            SC.ProductId,
                            P.ProductName,
                            C.CompanyName,
                            SC.Qtylose,
                            SC.PerUnitCostLose,
                            (SC.Qtylose * SC.PerUnitCostLose) AS TotalValue
                        FROM SMCurrentStock SC
                        INNER JOIN Salesmen S ON SC.SalesmanId = S.SalesmanID
                        INNER JOIN Products P ON SC.ProductId = P.ProductID
                        INNER JOIN Companies C ON P.CompanyID = C.CompanyID
                        WHERE 1=1";

                    List<string> conditions = new List<string>();
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                    {
                        conditions.Add("C.CompanyID = @CompanyID");
                        parameters.Add(new SqlParameter("@CompanyID", cmpnyidtxtbox.Text));
                    }

                    if (!string.IsNullOrEmpty(salesmanidtxtbox.Text))
                    {
                        conditions.Add("S.SalesmanID = @SalesmanID");
                        parameters.Add(new SqlParameter("@SalesmanID", salesmanidtxtbox.Text));
                    }

                    if (conditions.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY S.SalesmanName, C.CompanyName, P.ProductName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salesman stock data: " + ex.Message);
            }

            return dt;
        }
    }
}