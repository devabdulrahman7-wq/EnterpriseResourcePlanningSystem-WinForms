using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms;
using FO_2022.Reports;
using System.Configuration;

namespace FO_2022
{
    public partial class Company_Discount_Claim_Summary : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Company_Discount_Claim_Summary()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Today.AddDays(-30);
            todatepcker.Value = DateTime.Today;

            // Populate discount dropdown
            discountcombobox.Items.AddRange(new string[] {
                "All Discounts",
                "High Discount",
                "Low Discount",
                "Specific Discount"
            });
            discountcombobox.SelectedIndex = 0;

            // Load product groups
            LoadProductGroups();
        }

        private void LoadProductGroups(string companyId = null)
        {
            try
            {
                DataTable productGroups = GetProductGroups(companyId);
                prdctgrocombobox.Items.Clear();
                prdctgrocombobox.Items.Add("All Groups");

                foreach (DataRow row in productGroups.Rows)
                {
                    prdctgrocombobox.Items.Add($"{row["GroupId"]} - {row["GroupName"]}");
                }
                prdctgrocombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product groups: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetProductGroups(string companyId = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT GroupId, GroupName FROM ProductGroups WHERE 1=1";
                if (!string.IsNullOrEmpty(companyId))
                    query += " AND CompanyId = @CompanyId";
                query += " ORDER BY GroupName";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(companyId))
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value.AddDays(-1);
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be less than From date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value.AddDays(1);
            }
        }

        private void companyidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string companyId = cmpnyidtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(companyId))
                {
                    MessageBox.Show("Please enter Company ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable companyInfo = GetCompanyInfo(companyId);
                if (companyInfo.Rows.Count > 0)
                {
                    cmpnynametxtbox.Text = companyInfo.Rows[0]["CompanyName"].ToString();
                    LoadProductGroups(companyId);
                    MessageBox.Show("Company found successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cmpnynametxtbox.Clear();
                    MessageBox.Show("Company not found. Please check the Company ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching company: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetCompanyInfo(string companyId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE CompanyID = @CompanyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CompanyID", companyId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private void discountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            discountpercentpercenttxtbox.Enabled = discountcombobox.SelectedItem.ToString() == "Specific Discount";
            if (!discountpercentpercenttxtbox.Enabled) discountpercentpercenttxtbox.Clear();
        }

        private void discountpercentpercenttxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(discountpercentpercenttxtbox.Text))
            {
                if (!decimal.TryParse(discountpercentpercenttxtbox.Text, out decimal result))
                {
                    MessageBox.Show("Please enter a valid number for discount percent.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    discountpercentpercenttxtbox.Clear();
                }
                else if (result < 0 || result > 100)
                {
                    MessageBox.Show("Discount percent must be between 0 and 100.", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    discountpercentpercenttxtbox.Clear();
                }
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(false);
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(true);
        }

        private void GenerateReport(bool directPrint)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                if (!ValidateInput()) return;

                DateTime fromDate = fromdatepcker.Value.Date;
                DateTime toDate = todatepcker.Value.Date.AddDays(1).AddSeconds(-1);
                string companyId = cmpnyidtxtbox.Text.Trim();
                string discountType = discountcombobox.SelectedItem.ToString();
                decimal? discountPercent = null;

                int? groupId = null;
                if (prdctgrocombobox.SelectedIndex > 0)
                {
                    string selectedGroup = prdctgrocombobox.SelectedItem.ToString();
                    string groupIdStr = selectedGroup.Split('-')[0].Trim();
                    if (int.TryParse(groupIdStr, out int tempGroupId)) groupId = tempGroupId;
                }

                if (discountType == "Specific Discount" && !string.IsNullOrEmpty(discountpercentpercenttxtbox.Text))
                    discountPercent = decimal.Parse(discountpercentpercenttxtbox.Text);
                else if (discountType == "High Discount") discountPercent = 10;
                else if (discountType == "Low Discount") discountPercent = 5;

                DataTable reportData = GetDiscountClaimSummary(fromDate, toDate, companyId, groupId, discountType, discountPercent);
                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected criteria.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Company Discount Claim Summary" },
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "CompanyInfo", string.IsNullOrEmpty(companyId) ? "All Companies" : $"{companyId} - {cmpnynametxtbox.Text}" },
                    //{ "ProductGroup", prdctgrocombobox.SelectedIndex == 0 ? "All Groups" : prdctgrocombobox.SelectedItem.ToString() },
                    //{ "DiscountFilter", discountType },
                    //{ "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
                };

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport("Company_Discount_Claim_Summary", dataSources, parameters);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetDiscountClaimSummary(DateTime fromDate, DateTime toDate, string companyId, int? groupId, string discountType, decimal? discountPercent)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        C.CompanyID,
                        C.CompanyName,
                        PG.GroupId,
                        PG.GroupName,
                        P.ProductID,
                        P.ProductName,
                        COUNT(DISTINCT S.SaleId) as TotalInvoices,
                        SUM(SB.QtyLose) as TotalQuantity,
                        SUM(SB.TTLValue) as TotalValue,
                        AVG(SB.DiscRatio) as AvgDiscountPercent,
                        SUM(SB.DiscValLose) as TotalDiscountAmount,
                        COUNT(DISTINCT CASE WHEN SB.DiscRatio > 0 THEN S.SaleId ELSE NULL END) as DiscountedInvoices
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Products P ON SB.ProductId = P.ProductID
                    INNER JOIN Companies C ON P.CompanyID = C.CompanyID
                    LEFT JOIN ProductGroups PG ON P.CompanyID = PG.CompanyId AND P.GroupId = PG.GroupId
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND SB.IsDeleted = 0";

                if (!string.IsNullOrEmpty(companyId)) query += " AND C.CompanyID = @CompanyID";
                if (groupId.HasValue && groupId.Value > 0) query += " AND PG.GroupId = @GroupId";
                if (!string.IsNullOrEmpty(discountType) && discountType != "All Discounts")
                {
                    if (discountType == "High Discount" && discountPercent.HasValue) query += " AND SB.DiscRatio >= @DiscountPercent";
                    else if (discountType == "Low Discount" && discountPercent.HasValue) query += " AND SB.DiscRatio <= @DiscountPercent";
                    else if (discountType == "Specific Discount" && discountPercent.HasValue) query += " AND SB.DiscRatio = @DiscountPercent";
                }

                query += @"
                    GROUP BY C.CompanyID, C.CompanyName, PG.GroupId, PG.GroupName, P.ProductID, P.ProductName
                    ORDER BY C.CompanyName, PG.GroupName, P.ProductName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                if (!string.IsNullOrEmpty(companyId)) cmd.Parameters.AddWithValue("@CompanyID", companyId);
                if (groupId.HasValue && groupId.Value > 0) cmd.Parameters.AddWithValue("@GroupId", groupId.Value);
                if (!string.IsNullOrEmpty(discountType) && discountType != "All Discounts" && discountPercent.HasValue)
                    cmd.Parameters.AddWithValue("@DiscountPercent", discountPercent.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                dt.Columns.Add("DiscountPercentage", typeof(decimal));
                foreach (DataRow row in dt.Rows)
                {
                    int totalInvoices = Convert.ToInt32(row["TotalInvoices"]);
                    int discountedInvoices = Convert.ToInt32(row["DiscountedInvoices"]);
                    row["DiscountPercentage"] = totalInvoices > 0 ? Math.Round((discountedInvoices * 100.0m) / totalInvoices, 2) : 0;
                }
            }
            return dt;
        }

        private bool ValidateInput()
        {
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!string.IsNullOrEmpty(cmpnyidtxtbox.Text.Trim()) && string.IsNullOrEmpty(cmpnynametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please search for Company Name first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (discountcombobox.SelectedItem.ToString() == "Specific Discount" && string.IsNullOrEmpty(discountpercentpercenttxtbox.Text))
            {
                MessageBox.Show("Please enter Discount Percent for Specific Discount.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                discountpercentpercenttxtbox.Focus();
                return false;
            }
            return true;
        }

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void cmpnynametxtbox_TextChanged(object sender, EventArgs e) { }
        private void prdctgrocombobox_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}