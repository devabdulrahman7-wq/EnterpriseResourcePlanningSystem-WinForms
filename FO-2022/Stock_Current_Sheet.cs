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
    public partial class Stock_Current_Sheet : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Stock_Current_Sheet()
        {
            InitializeComponent();
        }

        private void Stock_Current_Sheet_Load(object sender, EventArgs e)
        {
            LoadCompanies();
            slctallcompaniescheckbox.Checked = true; // Default select all
        }

        private void LoadCompanies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            cmpnyviewlist.Items.Clear();
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["CompanyName"].ToString());
                                item.Tag = reader["CompanyID"].ToString();
                                cmpnyviewlist.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading companies: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void slctallcompaniescheckbox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in cmpnyviewlist.Items)
            {
                item.Checked = slctallcompaniescheckbox.Checked;
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Get selected companies
                List<string> selectedCompanies = new List<string>();

                foreach (ListViewItem item in cmpnyviewlist.Items)
                {
                    if (item.Checked)
                    {
                        selectedCompanies.Add(item.Tag.ToString());
                    }
                }

                if (selectedCompanies.Count == 0 && !slctallcompaniescheckbox.Checked)
                {
                    MessageBox.Show("Please select at least one company", "No Company Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetCurrentStockData(selectedCompanies);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No stock data found for selected companies", "No Data",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                string companiesText = slctallcompaniescheckbox.Checked ?
                    "All Companies" : $"{selectedCompanies.Count} Company(s) Selected";

                var parameters = new Dictionary<string, string>
                {
                    //{ "ReportDate", DateTime.Today.ToString("dd-MMM-yyyy") },
                    //{ "ShowAllProducts", showallproductofselectedcompanycheckbox.Checked.ToString() },
                    //{ "Companies", companiesText }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Stock_Current_Sheet", dataSources);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetCurrentStockData(List<string> selectedCompanies)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    c.CompanyID,
                    c.CompanyName,
                    p.ProductID,
                    p.ProductName,
                    p.PurchasePrice,
                    p.SalePrice,
                    ISNULL(cs.QtyLose, 0) as CurrentQty,
                    ISNULL(cs.PerUnitCostLose, 0) as PerUnitCost,
                    ISNULL(cs.QtyLose * cs.PerUnitCostLose, 0) as TotalValue
                FROM Products p
                INNER JOIN Companies c ON p.CompanyID = c.CompanyID
                LEFT JOIN CurrentStock cs ON p.ProductID = cs.ProductiD
                WHERE 1=1";

                // Add company filter if not all selected
                if (!slctallcompaniescheckbox.Checked && selectedCompanies.Count > 0)
                {
                    query += " AND c.CompanyID IN (";
                    for (int i = 0; i < selectedCompanies.Count; i++)
                    {
                        query += $"@CompanyID{i}";
                        if (i < selectedCompanies.Count - 1) query += ",";
                    }
                    query += ")";
                }

                // Add product filter based on checkbox
                if (!showallproductofselectedcompanycheckbox.Checked)
                {
                    query += " AND ISNULL(cs.QtyLose, 0) > 0";
                }

                query += " ORDER BY c.CompanyName, p.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add company parameters if not all selected
                    if (!slctallcompaniescheckbox.Checked && selectedCompanies.Count > 0)
                    {
                        for (int i = 0; i < selectedCompanies.Count; i++)
                        {
                            cmd.Parameters.AddWithValue($"@CompanyID{i}", selectedCompanies[i]);
                        }
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmpnyviewlist_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Update "Select All" checkbox state
            bool allChecked = true;
            foreach (ListViewItem item in cmpnyviewlist.Items)
            {
                if (!item.Checked)
                {
                    allChecked = false;
                    break;
                }
            }
            slctallcompaniescheckbox.CheckedChanged -= slctallcompaniescheckbox_CheckedChanged;
            slctallcompaniescheckbox.Checked = allChecked;
            slctallcompaniescheckbox.CheckedChanged += slctallcompaniescheckbox_CheckedChanged;
        }

        private void showallproductofselectedcompanycheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // This checkbox value is used in the query
        }
    }
}