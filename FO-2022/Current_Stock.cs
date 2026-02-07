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
    public partial class Current_Stock : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Current_Stock()
        {
            InitializeComponent();
        }

        private void Current_Stock_Load(object sender, EventArgs e)
        {
            LoadCompanies();
            LoadProductGroups();
        }

        private void LoadCompanies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Clear existing items
                    cmpnyviewlist.Items.Clear();

                    // Add companies to ListView
                    foreach (DataRow row in dt.Rows)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = row["CompanyName"].ToString();
                        item.Tag = row["CompanyID"].ToString(); // Store ID in Tag
                        item.Checked = true; // Default checked
                        cmpnyviewlist.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message);
            }
        }

        private void LoadProductGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT DISTINCT GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Clear existing items
                    grpviewlist.Items.Clear();

                    // Add groups to ListView
                    foreach (DataRow row in dt.Rows)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = row["GroupName"].ToString();
                        item.Tag = row["GroupId"].ToString(); // Store ID in Tag
                        item.Checked = true; // Default checked
                        grpviewlist.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product groups: " + ex.Message);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void slctallcmpniesbtnfromlist_Click(object sender, EventArgs e)
        {
            // Select all companies
            foreach (ListViewItem item in cmpnyviewlist.Items)
            {
                item.Checked = true;
            }
        }

        private void slctallgrpbtnfromlist_Click(object sender, EventArgs e)
        {
            // Select all groups
            foreach (ListViewItem item in grpviewlist.Items)
            {
                item.Checked = true;
            }
        }

        private DataTable GetCurrentStockData()
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
                    query.AppendLine("    PG.GroupName,");
                    query.AppendLine("    CS.QtyLose,");
                    query.AppendLine("    CS.PerUnitCostLose,");
                    query.AppendLine("    (CS.QtyLose * CS.PerUnitCostLose) AS TotalValue");
                    query.AppendLine("FROM CurrentStock CS");
                    query.AppendLine("INNER JOIN Products P ON CS.ProductiD = P.ProductID");
                    query.AppendLine("INNER JOIN Companies C ON P.CompanyID = C.CompanyID");
                    query.AppendLine("INNER JOIN ProductGroups PG ON P.CompanyID = PG.CompanyId AND P.GroupId = PG.GroupId");
                    query.AppendLine("WHERE 1=1");

                    // Add Company filter
                    List<string> selectedCompanies = new List<string>();
                    foreach (ListViewItem item in cmpnyviewlist.CheckedItems)
                    {
                        selectedCompanies.Add(item.Tag.ToString());
                    }

                    if (selectedCompanies.Count > 0)
                    {
                        query.AppendLine("AND P.CompanyID IN ('" + string.Join("','", selectedCompanies) + "')");
                    }

                    // Add Group filter
                    List<string> selectedGroups = new List<string>();
                    foreach (ListViewItem item in grpviewlist.CheckedItems)
                    {
                        selectedGroups.Add(item.Tag.ToString());
                    }

                    if (selectedGroups.Count > 0)
                    {
                        query.AppendLine("AND P.GroupId IN (" + string.Join(",", selectedGroups) + ")");
                    }

                    query.AppendLine("ORDER BY C.CompanyName, PG.GroupName, P.ProductName");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock data: " + ex.Message);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable stockData = GetCurrentStockData();

                if (stockData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for selected criteria.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Determine view type
                string viewType = "Cost";
                if (viewonpurchaseradiobtn.Checked)
                    viewType = "Purchase";
                else if (viewonsaleradiobtn.Checked)
                    viewType = "Sale";
                else if (viewoncostradiobtn.Checked)
                    viewType = "Cost";

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", stockData);

                // Prepare parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //parameters.Add("ReportTitle", "Current Stock Report");
                //parameters.Add("ViewType", viewType);
                //parameters.Add("GeneratedDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));

                // Get selected companies for display
                List<string> selectedCompanyNames = new List<string>();
                foreach (ListViewItem item in cmpnyviewlist.CheckedItems)
                {
                    selectedCompanyNames.Add(item.Text);
                }
                //parameters.Add("SelectedCompanies",
                    //selectedCompanyNames.Count > 0 ? string.Join(", ", selectedCompanyNames) : "All Companies");

                // Get selected groups for display
                List<string> selectedGroupNames = new List<string>();
                foreach (ListViewItem item in grpviewlist.CheckedItems)
                {
                    selectedGroupNames.Add(item.Text);
                }
                //parameters.Add("SelectedGroups",selectedGroupNames.Count > 0 ? string.Join(", ", selectedGroupNames) : "All Groups");

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Current_Stock", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            // Same as preview for now - printing will be handled in report viewer
            previewbtn_Click(sender, e);
        }
    }
}