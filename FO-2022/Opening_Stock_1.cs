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
    public partial class Opening_Stock_1 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Opening_Stock_1()
        {
            InitializeComponent();
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
                DataTable openingStockData = GetOpeningStockData();

                if (openingStockData.Rows.Count == 0)
                {
                    MessageBox.Show("No opening stock data found", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", openingStockData);

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Opening Stock Report");
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                //parameters.Add("CompanyName", cmpnynametxtbox.Text);
                //parameters.Add("CompanyID", cmpnyidtxtbox.Text);

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Opening_Stock", dataSources);
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

        private DataTable GetOpeningStockData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT ");
                    query.AppendLine("    OS.ProductId,");
                    query.AppendLine("    P.ProductName,");
                    query.AppendLine("    C.CompanyName,");
                    query.AppendLine("    PK.PackingName,");
                    query.AppendLine("    OS.Multiplier,");
                    query.AppendLine("    OS.QtyPack,");
                    query.AppendLine("    OS.QtyLose,");
                    query.AppendLine("    OS.BnsLose,");
                    query.AppendLine("    OS.PricePack,");
                    query.AppendLine("    OS.TTLValue,");
                    query.AppendLine("    OS.TTLSTax,");
                    query.AppendLine("    OS.TTLFTax");
                    query.AppendLine("FROM OpeningStock OS");
                    query.AppendLine("INNER JOIN Products P ON OS.ProductId = P.ProductID");
                    query.AppendLine("INNER JOIN Companies C ON P.CompanyID = C.CompanyID");
                    query.AppendLine("INNER JOIN Packings PK ON OS.PackingId = PK.PackingId");

                    if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text))
                    {
                        query.AppendLine("WHERE C.CompanyID = @CompanyID");
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
                MessageBox.Show("Error loading opening stock data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
    }
}