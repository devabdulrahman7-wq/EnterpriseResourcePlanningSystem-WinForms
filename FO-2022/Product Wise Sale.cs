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
    public partial class Product_Wise_Sale : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedCompanyId = "";
        private string selectedSalesmanId = "";

        public Product_Wise_Sale()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today.AddMonths(-1);
            todatepcker.Value = DateTime.Today;
        }

        private void Product_Wise_Sale_Load(object sender, EventArgs e)
        {
            LoadProductGroups();
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void LoadProductGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT DISTINCT 
                        pg.GroupId,
                        pg.GroupName,
                        pg.CompanyId
                    FROM ProductGroups pg
                    INNER JOIN Products p ON pg.CompanyId = p.CompanyID AND pg.GroupId = p.GroupId
                    ORDER BY pg.GroupName";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Add "All Groups" option
                        DataRow row = dt.NewRow();
                        row["GroupId"] = 0;
                        row["GroupName"] = "All Groups";
                        row["CompanyId"] = "";
                        dt.Rows.InsertAt(row, 0);

                        grpcombobox.DataSource = dt;
                        grpcombobox.DisplayMember = "GroupName";
                        grpcombobox.ValueMember = "GroupId";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product groups: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmpnyidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmpnyidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Company ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            cmpnynametxtbox.Text = result.ToString();
                            selectedCompanyId = cmpnyidtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Company ID not found", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            cmpnynametxtbox.Clear();
                            selectedCompanyId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salesmanidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Salesman ID", "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text.Trim());
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            salesmannametxtbox.Text = result.ToString();
                            selectedSalesmanId = salesmanidtxtbox.Text.Trim();
                        }
                        else
                        {
                            MessageBox.Show("Salesman ID not found", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            salesmannametxtbox.Clear();
                            selectedSalesmanId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // Validate sale ID range
                int fromSaleId = 0, toSaleId = 0;

                if (!string.IsNullOrWhiteSpace(fromsaletxtbox.Text))
                {
                    if (!int.TryParse(fromsaletxtbox.Text, out fromSaleId))
                    {
                        MessageBox.Show("From Sale ID must be a valid number",
                            "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(tosaletxtbox.Text))
                {
                    if (!int.TryParse(tosaletxtbox.Text, out toSaleId))
                    {
                        MessageBox.Show("To Sale ID must be a valid number",
                            "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (fromSaleId > 0 && toSaleId > 0 && fromSaleId > toSaleId)
                {
                    MessageBox.Show("From Sale ID cannot be greater than To Sale ID",
                        "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetProductWiseSalesData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No sales data found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                //    { "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //    { "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //    { "CompanyName", string.IsNullOrEmpty(cmpnynametxtbox.Text) ? "All Companies" : cmpnynametxtbox.Text },
                //    { "SalesmanName", string.IsNullOrEmpty(salesmannametxtbox.Text) ? "All Salesmen" : salesmannametxtbox.Text },
                //    { "GroupName", grpcombobox.Text },
                //    { "SaleIDRange", GetSaleIDRangeText() }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Product_Wise_Sale", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSaleIDRangeText()
        {
            string fromSale = fromsaletxtbox.Text.Trim();
            string toSale = tosaletxtbox.Text.Trim();

            if (string.IsNullOrEmpty(fromSale) && string.IsNullOrEmpty(toSale))
                return "All Sales";
            else if (!string.IsNullOrEmpty(fromSale) && string.IsNullOrEmpty(toSale))
                return $"From: {fromSale}";
            else if (string.IsNullOrEmpty(fromSale) && !string.IsNullOrEmpty(toSale))
                return $"Up to: {toSale}";
            else
                return $"{fromSale} to {toSale}";
        }

        private DataTable GetProductWiseSalesData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    c.CompanyName,
                    pg.GroupName,
                    sm.SalesmanName,
                    SUM(sb.QtyPack * sb.Multiplier + sb.QtyLose) as TotalQty,
                    SUM(sb.TTLValue) as TotalAmount,
                    AVG(sb.PricePack) as AverageRate
                FROM Sales s
                INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                INNER JOIN Products p ON sb.ProductId = p.ProductID
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                WHERE s.SaleDate BETWEEN @FromDate AND @ToDate";

                // Add company filter
                if (!string.IsNullOrEmpty(selectedCompanyId))
                {
                    query += " AND p.CompanyID = @CompanyID";
                }

                // Add salesman filter
                if (!string.IsNullOrEmpty(selectedSalesmanId))
                {
                    query += " AND s.SalesmanId = @SalesmanID";
                }

                // Add group filter if not "All Groups"
                if (grpcombobox.SelectedValue != null && grpcombobox.SelectedValue.ToString() != "0")
                {
                    query += " AND p.GroupId = @GroupID";
                }

                // Add sale ID range filters
                if (!string.IsNullOrWhiteSpace(fromsaletxtbox.Text) && int.TryParse(fromsaletxtbox.Text, out int fromSale))
                {
                    query += " AND s.SaleId >= @FromSaleID";
                }

                if (!string.IsNullOrWhiteSpace(tosaletxtbox.Text) && int.TryParse(tosaletxtbox.Text, out int toSale))
                {
                    query += " AND s.SaleId <= @ToSaleID";
                }

                query += @"
                GROUP BY p.ProductID, p.ProductName, c.CompanyName, pg.GroupName, sm.SalesmanName
                HAVING SUM(sb.QtyPack * sb.Multiplier + sb.QtyLose) > 0
                ORDER BY p.ProductName, c.CompanyName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));
                    cmd.CommandTimeout = 70;
                    // Add company parameter
                    if (!string.IsNullOrEmpty(selectedCompanyId))
                    {
                        cmd.Parameters.AddWithValue("@CompanyID", selectedCompanyId);
                    }

                    // Add salesman parameter
                    if (!string.IsNullOrEmpty(selectedSalesmanId))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", selectedSalesmanId);
                    }

                    // Add group parameter if not "All Groups"
                    if (grpcombobox.SelectedValue != null && grpcombobox.SelectedValue.ToString() != "0")
                    {
                        cmd.Parameters.AddWithValue("@GroupID", grpcombobox.SelectedValue);
                    }

                    // Add sale ID range parameters
                    if (!string.IsNullOrWhiteSpace(fromsaletxtbox.Text) && int.TryParse(fromsaletxtbox.Text, out fromSale))
                    {
                        cmd.Parameters.AddWithValue("@FromSaleID", fromSale);
                    }

                    if (!string.IsNullOrWhiteSpace(tosaletxtbox.Text) && int.TryParse(tosaletxtbox.Text, out toSale))
                    {
                        cmd.Parameters.AddWithValue("@ToSaleID", toSale);
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

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            cmpnynametxtbox.Clear();
            selectedCompanyId = "";
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
            selectedSalesmanId = "";
        }
    }
}