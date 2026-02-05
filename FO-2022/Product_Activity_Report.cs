using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FO_2022
{
    public partial class Product_Activity_Report : Form
    {
        // Database connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Product_Activity_Report()
        {
            InitializeComponent();
            // Add columns to listview
            SetupListViewColumns();
        }

        private void SetupListViewColumns()
        {
            productlistview.View = View.Details;
            productlistview.CheckBoxes = true;
            productlistview.FullRowSelect = true;

            productlistview.Columns.Clear();
            productlistview.Columns.Add("Product ID", 80);
            productlistview.Columns.Add("Product Name", 200);
            productlistview.Columns.Add("Company", 100);
            productlistview.Columns.Add("Group", 100);
        }

        private void Product_Activity_Report_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default dates
                fromdatepcker.Value = DateTime.Today.AddMonths(-1);
                todatepcker.Value = DateTime.Today;

                // Load groups
                LoadGroups();

                // Load all products
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGroups()
        {
            try
            {
                groupcombobox.Items.Clear();
                groupcombobox.Items.Add("ALL GROUPS"); // Default option

                string query = "SELECT DISTINCT GroupName FROM ProductGroups ORDER BY GroupName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            groupcombobox.Items.Add(reader["GroupName"].ToString());
                        }
                    }
                }

                groupcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                productlistview.Items.Clear();
                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ProductName,
                        c.CompanyName,
                        pg.GroupName
                    FROM Products p
                    LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                    LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                    WHERE 1=1";

                // Add group filter if not "ALL GROUPS"
                if (groupcombobox.SelectedIndex > 0)
                {
                    query += " AND pg.GroupName = @GroupName";
                }

                query += " ORDER BY p.ProductName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (groupcombobox.SelectedIndex > 0)
                        {
                            cmd.Parameters.AddWithValue("@GroupName", groupcombobox.Text);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem();
                                item.Text = reader["ProductID"].ToString();
                                item.SubItems.Add(reader["ProductName"].ToString());
                                item.SubItems.Add(reader["CompanyName"].ToString());
                                item.SubItems.Add(reader["GroupName"].ToString());
                                productlistview.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void groupcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reload products when group changes
            LoadProducts();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport(false); // false = preview mode
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport(true); // true = print mode
        }

        private void ShowReport(bool autoPrint)
        {
            try
            {
                // Get selected products
                List<string> selectedProductIds = new List<string>();
                foreach (ListViewItem item in productlistview.Items)
                {
                    if (item.Checked)
                    {
                        selectedProductIds.Add(item.Text); // ProductID
                    }
                }

                if (selectedProductIds.Count == 0)
                {
                    MessageBox.Show("Please select at least one product.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data for report
                DataTable dt = GetProductActivityData(
                    fromdatepcker.Value,
                    todatepcker.Value,
                    selectedProductIds
                );

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt); // Use DataSet1 as in your requirement

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                //parameters.Add("ReportTitle", "Product Activity Report");

                // Show report viewer
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();

                // You can modify Report_viewer to accept autoPrint parameter
                // For now, both preview and print will show viewer
                reportViewer.ShowReport("Product_Activity_Report", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetProductActivityData(DateTime fromDate, DateTime toDate, List<string> productIds)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("ProductID", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("GroupName", typeof(string));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("TransactionType", typeof(string));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("ReferenceNo", typeof(string));
            dt.Columns.Add("PartyName", typeof(string));

            if (productIds.Count == 0) return dt;

            // Convert product list to comma-separated string for IN clause
            string productIdList = string.Join(",", productIds.Select(id => $"'{id}'"));

            string query = @"
                -- SALES TRANSACTIONS
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    c.CompanyName,
                    pg.GroupName,
                    s.SaleDate AS Date,
                    'SALE' AS TransactionType,
                    (sb.QtyPack * pp.Multiplier + sb.QtyLose) AS Quantity,
                    sb.TTLValue AS Amount,
                    CAST(s.SaleId AS VARCHAR) AS ReferenceNo,
                    par.PartyName
                FROM SalesBody sb
                INNER JOIN Sales s ON sb.SaleId = s.SaleId
                INNER JOIN Products p ON sb.ProductId = p.ProductID
                INNER JOIN ProductPacking pp ON sb.ProductId = pp.ProductId AND sb.PackingId = pp.PackingId
                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                LEFT JOIN Parties par ON s.CustomerId = par.PartyID
                WHERE p.ProductID IN (" + productIdList + @")
                    AND s.SaleDate BETWEEN @FromDate AND @ToDate
                    AND sb.IsDeleted = 0
                
                UNION ALL
                
                -- PURCHASE TRANSACTIONS
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    c.CompanyName,
                    pg.GroupName,
                    pur.PurchaseDate AS Date,
                    'PURCHASE' AS TransactionType,
                    (pb.QtyPack * pp.Multiplier + pb.QtyLose) AS Quantity,
                    pb.TTLValue AS Amount,
                    CAST(pur.PurchaseID AS VARCHAR) AS ReferenceNo,
                    par.PartyName
                FROM PurchasesBody pb
                INNER JOIN Purchase pur ON pb.PurchaseId = pur.PurchaseID
                INNER JOIN Products p ON pb.ProductId = p.ProductID
                INNER JOIN ProductPacking pp ON pb.ProductId = pp.ProductId AND pb.PackingId = pp.PackingId
                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                LEFT JOIN Parties par ON pur.VendorID = par.PartyID
                WHERE p.ProductID IN (" + productIdList + @")
                    AND pur.PurchaseDate BETWEEN @FromDate AND @ToDate
                    AND pb.IsDeleted = 0
                
                ORDER BY Date, ProductID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["ProductID"] = reader["ProductID"];
                                row["ProductName"] = reader["ProductName"];
                                row["CompanyName"] = reader["CompanyName"];
                                row["GroupName"] = reader["GroupName"];
                                row["Date"] = reader["Date"];
                                row["TransactionType"] = reader["TransactionType"];
                                row["Quantity"] = reader["Quantity"];
                                row["Amount"] = reader["Amount"];
                                row["ReferenceNo"] = reader["ReferenceNo"];
                                row["PartyName"] = reader["PartyName"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value;
            }
        }

        private void selectsllproductfromlist_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in productlistview.Items)
            {
                item.Checked = true;
            }
        }
    }
}