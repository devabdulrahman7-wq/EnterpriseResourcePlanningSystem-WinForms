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
    public partial class Product_Wise_Sales_1 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSalesmanId = "";
        private string selectedCompanyId = "";

        public Product_Wise_Sales_1()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-1);
            todatepcker.Value = DateTime.Today;
        }

        private void Product_Wise_Sales_1_Load(object sender, EventArgs e)
        {
            LoadCompaniesToListView();
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void LoadCompaniesToListView()
        {
            try
            {
                listView1.Items.Clear();
                listView1.CheckBoxes = true;
                listView1.View = View.Details;

                // Add columns
                listView1.Columns.Clear();
                listView1.Columns.Add("Company Name", 200);
                listView1.Columns.Add("Company ID", 100);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["CompanyName"].ToString());
                                item.SubItems.Add(reader["CompanyID"].ToString());
                                item.Checked = true; // Default checked
                                listView1.Items.Add(item);
                            }
                        }
                    }
                }

                // Adjust column width
                listView1.Columns[0].Width = -2; // Auto size
                listView1.Columns[1].Width = -2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading companies: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            string companyId = cmpnyidtxtbox.Text.Trim();
            string companyName = cmpnynametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(companyId) && string.IsNullOrEmpty(companyName))
            {
                MessageBox.Show("Please enter Company ID or Name",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(companyId))
                    {
                        query = @"SELECT CompanyID, CompanyName 
                          FROM Companies 
                          WHERE CompanyID = @Value";
                    }
                    else
                    {
                        query = @"SELECT CompanyID, CompanyName 
                          FROM Companies 
                          WHERE CompanyName = @Value";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(companyId) ? companyId : companyName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cmpnyidtxtbox.Text = reader["CompanyID"].ToString();
                                cmpnynametxtbox.Text = reader["CompanyName"].ToString();
                                selectedCompanyId = reader["CompanyID"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Company not found",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                cmpnyidtxtbox.Clear();
                                cmpnynametxtbox.Clear();
                                selectedCompanyId = "";
                                cmpnyidtxtbox.Focus();
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

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected companies from ListView
                List<string> selectedCompanies = GetSelectedCompanies();

                if (selectedCompanies.Count == 0)
                {
                    MessageBox.Show("Please select at least one company from the list",
                        "No Company Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetProductWiseSalesData(selectedCompanies);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "SalesmanName", string.IsNullOrEmpty(salesmannametxtbox.Text) ? "All Salesmen" : salesmannametxtbox.Text },
                    //{ "CompanyFilter", GetCompanyFilterText(selectedCompanies) }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Product_Wise_Sale_", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetSelectedCompanies()
        {
            List<string> selectedCompanies = new List<string>();

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    selectedCompanies.Add(item.SubItems[1].Text); // Company ID
                }
            }

            return selectedCompanies;
        }

        private string GetCompanyFilterText(List<string> selectedCompanies)
        {
            if (selectedCompanies.Count == listView1.Items.Count)
                return "All Companies";
            else if (selectedCompanies.Count <= 3)
                return string.Join(", ", selectedCompanies);
            else
                return $"{selectedCompanies.Count} Companies Selected";
        }

        private DataTable GetProductWiseSalesData(List<string> selectedCompanies)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    c.CompanyName,
                    sm.SalesmanName,
                    SUM(sb.QtyPack * sb.Multiplier + sb.QtyLose) as TotalQty,
                    SUM(sb.TTLValue) as TotalAmount,
                    AVG(sb.PricePack) as AverageRate,
                    COUNT(DISTINCT s.SaleId) as InvoiceCount
                FROM Sales s
                INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                INNER JOIN Products p ON sb.ProductId = p.ProductID
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate
                AND c.CompanyID IN (";

                // Add company placeholders
                for (int i = 0; i < selectedCompanies.Count; i++)
                {
                    query += $"@CompanyID{i}";
                    if (i < selectedCompanies.Count - 1) query += ",";
                }
                query += ")";

                // Add salesman filter
                if (!string.IsNullOrEmpty(selectedSalesmanId))
                {
                    query += " AND s.SalesmanId = @SalesmanID";
                }

                // Add company filter from textbox (if provided)
                if (!string.IsNullOrEmpty(selectedCompanyId))
                {
                    query += " AND c.CompanyID = @SpecificCompanyID";
                }

                query += @"
                GROUP BY p.ProductID, p.ProductName, c.CompanyName, sm.SalesmanName
                HAVING SUM(sb.QtyPack * sb.Multiplier + sb.QtyLose) > 0
                ORDER BY c.CompanyName, p.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

                    // Add company parameters
                    for (int i = 0; i < selectedCompanies.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@CompanyID{i}", selectedCompanies[i]);
                    }

                    // Add salesman parameter
                    if (!string.IsNullOrEmpty(selectedSalesmanId))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", selectedSalesmanId);
                    }

                    // Add specific company parameter
                    if (!string.IsNullOrEmpty(selectedCompanyId))
                    {
                        cmd.Parameters.AddWithValue("@SpecificCompanyID", selectedCompanyId);
                    }

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
            selectedSalesmanId = "";
        }

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            cmpnynametxtbox.Clear();
            selectedCompanyId = "";
        }
    }
}