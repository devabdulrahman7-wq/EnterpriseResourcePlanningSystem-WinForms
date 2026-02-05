using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
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
    public partial class Sales_Month_Comparison_Product_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sales_Month_Comparison_Product_Wise()
        {
            InitializeComponent();
        }

        private void Sales_Month_Comparison_Product_Wise_Load(object sender, EventArgs e)
        {
            // Set default dates
            currentmonthdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lastmonthdatepcker.Value = currentmonthdatepcker.Value.AddMonths(-1);

            // Load product groups in listview
            LoadProductGroups();
        }

        // 📌 LOAD PRODUCT GROUPS IN LISTVIEW
        private void LoadProductGroups()
        {
            try
            {
                listView1.Items.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT PG.GroupId, PG.GroupName, C.CompanyName 
                        FROM ProductGroups PG
                        INNER JOIN Companies C ON PG.CompanyId = C.CompanyID
                        ORDER BY C.CompanyName, PG.GroupName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem();
                                item.Text = reader["GroupName"].ToString();
                                item.SubItems.Add(reader["GroupId"].ToString());
                                item.SubItems.Add(reader["CompanyName"].ToString());
                                item.Checked = true;
                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Product Groups: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 SELECT ALL BUTTON
        private void slctbtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Checked = true;
            }
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 PRINT BUTTON
        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GENERATE REPORT
        private void GenerateReport(bool autoPrint)
        {
            // Validate dates
            if (lastmonthdatepcker.Value > currentmonthdatepcker.Value)
            {
                MessageBox.Show("Last month date cannot be greater than Current month date",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected groups
            List<int> selectedGroupIds = new List<int>();
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked && item.SubItems.Count > 1)
                {
                    selectedGroupIds.Add(Convert.ToInt32(item.SubItems[1].Text));
                }
            }

            if (selectedGroupIds.Count == 0)
            {
                MessageBox.Show("Please select at least one Product Group", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate date ranges
            DateTime lastMonthStart = lastmonthdatepcker.Value;
            DateTime lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);

            DateTime currentMonthStart = currentmonthdatepcker.Value;
            DateTime currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            // Get data
            DataTable dt = GetSalesComparisonData(lastMonthStart, lastMonthEnd,
                                                currentMonthStart, currentMonthEnd,
                                                selectedGroupIds);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the selected criteria", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prepare data sources
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
            {
                { "DataSet1", dt }
            };

            // Prepare parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                //{ "LastMonthPeriod", $"{lastMonthStart.ToString("MMM yyyy")}" },
                //{ "CurrentMonthPeriod", $"{currentMonthStart.ToString("MMM yyyy")}" },
                //{ "LastMonthStart", lastMonthStart.ToString("dd-MMM-yyyy") },
                //{ "LastMonthEnd", lastMonthEnd.ToString("dd-MMM-yyyy") },
                //{ "CurrentMonthStart", currentMonthStart.ToString("dd-MMM-yyyy") },
                //{ "CurrentMonthEnd", currentMonthEnd.ToString("dd-MMM-yyyy") },
                //{ "SelectedGroups", selectedGroupIds.Count.ToString() },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Sales_Month_Comparison_Product_Wise", dataSources,  null);
        }

        // 📌 GET SALES COMPARISON DATA
        private DataTable GetSalesComparisonData(DateTime lastMonthStart, DateTime lastMonthEnd,
                                                DateTime currentMonthStart, DateTime currentMonthEnd,
                                                List<int> groupIds)
        {
            DataTable dt = new DataTable();

            // Create comma-separated string of group IDs
            string groupIdsString = string.Join(",", groupIds);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        -- Product Group Information
                        PG.GroupId,
                        PG.GroupName,
                        C.CompanyName,
                        
                        -- Last Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS LastMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS LastMonthSalesQty,
                        
                        -- Current Month Sales
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS CurrentMonthSalesValue,
                        
                        ISNULL((
                            SELECT SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS CurrentMonthSalesQty,
                        
                        -- Comparison Calculations
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) 
                        -
                        ISNULL((
                            SELECT SUM(SB.TTLValue) 
                            FROM SalesBody SB
                            INNER JOIN Sales S ON SB.SaleId = S.SaleId
                            INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                            WHERE PR.GroupId = PG.GroupId
                            AND PR.CompanyID = PG.CompanyId
                            AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                            AND SB.IsDeleted = 0
                            AND S.IsPosted = 1
                        ), 0) AS ValueDifference,
                        
                        CASE 
                            WHEN ISNULL((
                                SELECT SUM(SB.TTLValue) 
                                FROM SalesBody SB
                                INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                                WHERE PR.GroupId = PG.GroupId
                                AND PR.CompanyID = PG.CompanyId
                                AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                AND SB.IsDeleted = 0
                                AND S.IsPosted = 1
                            ), 0) = 0 THEN 0
                            ELSE 
                                (
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                                        WHERE PR.GroupId = PG.GroupId
                                        AND PR.CompanyID = PG.CompanyId
                                        AND S.SaleDate BETWEEN @CurrentMonthStart AND @CurrentMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                    ), 0) 
                                    -
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                                        WHERE PR.GroupId = PG.GroupId
                                        AND PR.CompanyID = PG.CompanyId
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                    ), 0)
                                ) * 100.0 / NULLIF(
                                    ISNULL((
                                        SELECT SUM(SB.TTLValue) 
                                        FROM SalesBody SB
                                        INNER JOIN Sales S ON SB.SaleId = S.SaleId
                                        INNER JOIN Products PR ON SB.ProductId = PR.ProductID
                                        WHERE PR.GroupId = PG.GroupId
                                        AND PR.CompanyID = PG.CompanyId
                                        AND S.SaleDate BETWEEN @LastMonthStart AND @LastMonthEnd
                                        AND SB.IsDeleted = 0
                                        AND S.IsPosted = 1
                                    ), 0), 0)
                        END AS PercentageChange
                        
                    FROM ProductGroups PG
                    INNER JOIN Companies C ON PG.CompanyId = C.CompanyID
                    WHERE ',' + @GroupIds + ',' LIKE '%,' + CAST(PG.GroupId AS NVARCHAR(10)) + ',%'
                    ORDER BY C.CompanyName, PG.GroupName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@LastMonthStart", lastMonthStart);
                    cmd.Parameters.AddWithValue("@LastMonthEnd", lastMonthEnd);
                    cmd.Parameters.AddWithValue("@CurrentMonthStart", currentMonthStart);
                    cmd.Parameters.AddWithValue("@CurrentMonthEnd", currentMonthEnd);
                    cmd.Parameters.AddWithValue("@GroupIds", groupIdsString);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // 📌 DATE CHANGED EVENTS
        private void lastmonthdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Ensure it's first day of month
            lastmonthdatepcker.Value = new DateTime(
                lastmonthdatepcker.Value.Year,
                lastmonthdatepcker.Value.Month,
                1);
        }

        private void currentmonthdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Ensure it's first day of month
            currentmonthdatepcker.Value = new DateTime(
                currentmonthdatepcker.Value.Year,
                currentmonthdatepcker.Value.Month,
                1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }
    }
}