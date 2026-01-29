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
    public partial class Purchase_Month_Comparison_Product_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Purchase_Month_Comparison_Product_Wise()
        {
            InitializeComponent();

            // Set default dates
            DateTime today = DateTime.Today;
            DateTime firstDayOfPrevMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            DateTime lastDayOfPrevMonth = firstDayOfPrevMonth.AddMonths(1).AddDays(-1);
            DateTime firstDayOfCurrMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfCurrMonth = firstDayOfCurrMonth.AddMonths(1).AddDays(-1);

            fromprevmonthdate.Value = firstDayOfPrevMonth;
            toprevdate.Value = lastDayOfPrevMonth;
            fromcrntmonthdate.Value = firstDayOfCurrMonth;
            tocrntmonthdate.Value = lastDayOfCurrMonth;

            // Load product groups
            LoadProductGroups();
        }

        private void LoadProductGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Add "All Groups" option
                        DataRow row = dt.NewRow();
                        row["GroupId"] = 0;
                        row["GroupName"] = "All Groups";
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

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate dates
                if (fromprevmonthdate.Value > toprevdate.Value)
                {
                    MessageBox.Show("Previous month: From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (fromcrntmonthdate.Value > tocrntmonthdate.Value)
                {
                    MessageBox.Show("Current month: From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetPurchaseComparisonData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No purchase data found for selected periods",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                //    { "PrevMonthFrom", fromprevmonthdate.Value.ToString("dd-MMM-yyyy") },
                //    { "PrevMonthTo", toprevdate.Value.ToString("dd-MMM-yyyy") },
                //    { "CurrMonthFrom", fromcrntmonthdate.Value.ToString("dd-MMM-yyyy") },
                //    { "CurrMonthTo", tocrntmonthdate.Value.ToString("dd-MMM-yyyy") },
                //    { "GroupName", grpcombobox.Text }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Purchase_Month_Comparison_Product_Wise", dataSources, parameters);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPurchaseComparisonData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        WITH ProductData AS (
            SELECT 
                p.ProductID,
                p.ProductName,
                pg.GroupName,
                ISNULL(prev.Qty, 0) as PrevMonthQty,
                ISNULL(prev.Amount, 0) as PrevMonthAmount,
                ISNULL(curr.Qty, 0) as CurrMonthQty,
                ISNULL(curr.Amount, 0) as CurrMonthAmount
            FROM Products p
            LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
            LEFT JOIN (
                SELECT 
                    pb.ProductId,
                    SUM(pb.QtyPack * pb.Multiplier + pb.QtyLose) as Qty,
                    SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax) as Amount
                FROM PurchasesBody pb
                INNER JOIN Purchase pur ON pb.PurchaseId = pur.PurchaseID
                WHERE pur.PurchaseDate BETWEEN @PrevFromDate AND @PrevToDate
                GROUP BY pb.ProductId
            ) prev ON p.ProductID = prev.ProductId
            LEFT JOIN (
                SELECT 
                    pb.ProductId,
                    SUM(pb.QtyPack * pb.Multiplier + pb.QtyLose) as Qty,
                    SUM(pb.TTLValue + pb.TTLSTax + pb.TTLFTax) as Amount
                FROM PurchasesBody pb
                INNER JOIN Purchase pur ON pb.PurchaseId = pur.PurchaseID
                WHERE pur.PurchaseDate BETWEEN @CurrFromDate AND @CurrToDate
                GROUP BY pb.ProductId
            ) curr ON p.ProductID = curr.ProductId
            WHERE 1=1
        )
        SELECT 
            ProductID,
            ProductName,
            GroupName,
            PrevMonthQty,
            PrevMonthAmount,
            CurrMonthQty,
            CurrMonthAmount,
            (CurrMonthQty - PrevMonthQty) as QtyDifference,
            (CurrMonthAmount - PrevMonthAmount) as AmountDifference
        FROM ProductData
        WHERE (PrevMonthQty > 0 OR CurrMonthQty > 0)  -- ✅ CORRECT: Use WHERE instead of HAVING
        ORDER BY ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PrevFromDate", fromprevmonthdate.Value.Date);
                    cmd.Parameters.AddWithValue("@PrevToDate", toprevdate.Value.Date.AddDays(1).AddSeconds(-1));
                    cmd.Parameters.AddWithValue("@CurrFromDate", fromcrntmonthdate.Value.Date);
                    cmd.Parameters.AddWithValue("@CurrToDate", tocrntmonthdate.Value.Date.AddDays(1).AddSeconds(-1));
                    cmd.CommandTimeout = 120;

                    if (grpcombobox.SelectedValue != null && grpcombobox.SelectedValue.ToString() != "0")
                    {
                        cmd.CommandText = cmd.CommandText.Replace("WHERE 1=1",
                            "WHERE p.GroupId = @GroupId");
                        cmd.Parameters.AddWithValue("@GroupId", grpcombobox.SelectedValue);
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

        private void Purchase_Month_Comparison_Product_Wise_Load(object sender, EventArgs e)
        {
            // Set date limits
            fromprevmonthdate.MinDate = new DateTime(2000, 1, 1);
            toprevdate.MaxDate = DateTime.Today;
            fromcrntmonthdate.MinDate = new DateTime(2000, 1, 1);
            tocrntmonthdate.MaxDate = DateTime.Today;
        }

        private void fromprevmonthdate_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Auto-adjust to date
        }

        private void toprevdate_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Auto-adjust from date
        }

        private void fromcrntmonthdate_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Auto-adjust to date
        }

        private void tocrntmonthdate_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Auto-adjust from date
        }

        private void grpcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Group selection changed
        }

        private void previewbtn_Click_1(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);

        }
    }
}