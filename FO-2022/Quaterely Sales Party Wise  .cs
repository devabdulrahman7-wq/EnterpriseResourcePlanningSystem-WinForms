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
    public partial class Quaterely_Sales_Party_Wise : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Quaterely_Sales_Party_Wise()
        {
            InitializeComponent();
        }

        private void Quaterely_Sales_Party_Wise_Load(object sender, EventArgs e)
        {
            // Set default year to current year
            numericUpDownforselectyear.Minimum = 2000;
            numericUpDownforselectyear.Maximum = DateTime.Now.Year + 10;

            // ✅ Then set value
            numericUpDownforselectyear.Value = DateTime.Now.Year;

            // Load salesman combobox
            LoadSalesmanComboBox();

            // Load quarter combobox
            LoadQuarterComboBox();
        }

        // 📌 LOAD SALESMAN COMBOBOX
        private void LoadSalesmanComboBox()
        {
            try
            {
                salesmancombobox.Items.Clear();
                salesmancombobox.Items.Add("All");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salesmancombobox.Items.Add(reader["SalesmanName"].ToString());
                            }
                        }
                    }
                }
                salesmancombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Salesmen: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 LOAD QUARTER COMBOBOX
        private void LoadQuarterComboBox()
        {
            try
            {
                selectyearquartercombobox.Items.Clear();
                selectyearquartercombobox.Items.Add("Q1 (Jan-Mar)");
                selectyearquartercombobox.Items.Add("Q2 (Apr-Jun)");
                selectyearquartercombobox.Items.Add("Q3 (Jul-Sep)");
                selectyearquartercombobox.Items.Add("Q4 (Oct-Dec)");
                selectyearquartercombobox.Items.Add("Full Year");

                selectyearquartercombobox.SelectedIndex = GetCurrentQuarterIndex();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Quarters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GET CURRENT QUARTER INDEX
        private int GetCurrentQuarterIndex()
        {
            int currentMonth = DateTime.Now.Month;

            if (currentMonth >= 1 && currentMonth <= 3)
                return 0; // Q1
            else if (currentMonth >= 4 && currentMonth <= 6)
                return 1; // Q2
            else if (currentMonth >= 7 && currentMonth <= 9)
                return 2; // Q3
            else
                return 3; // Q4
        }

        // 📌 GET QUARTER DATE RANGE
        private (DateTime StartDate, DateTime EndDate) GetQuarterDateRange(int year, int quarterIndex)
        {
            if (quarterIndex == 0) // Q1
                return (new DateTime(year, 1, 1), new DateTime(year, 3, 31));
            else if (quarterIndex == 1) // Q2
                return (new DateTime(year, 4, 1), new DateTime(year, 6, 30));
            else if (quarterIndex == 2) // Q3
                return (new DateTime(year, 7, 1), new DateTime(year, 9, 30));
            else if (quarterIndex == 3) // Q4
                return (new DateTime(year, 10, 1), new DateTime(year, 12, 31));
            else // Full Year
                return (new DateTime(year, 1, 1), new DateTime(year, 12, 31));
        }

        // 📌 GET SALESMAN ID BY NAME
        private string GetSalesmanIdByName(string salesmanName)
        {
            if (salesmanName == "All") return "All";

            string salesmanId = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SalesmanID FROM Salesmen WHERE SalesmanName = @SalesmanName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SalesmanName", salesmanName);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        salesmanId = result.ToString();
                }
            }

            return salesmanId;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
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
            // Validate inputs
            if (numericUpDownforselectyear.Value < 2000 || numericUpDownforselectyear.Value > 2100)
            {
                MessageBox.Show("Please select a valid year (2000-2100)", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectyearquartercombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select a quarter", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get inputs
            int year = (int)numericUpDownforselectyear.Value;
            string quarterText = selectyearquartercombobox.SelectedItem.ToString();
            string salesmanName = salesmancombobox.SelectedItem?.ToString() ?? "All";
            int quarterIndex = selectyearquartercombobox.SelectedIndex;

            // Get date range for selected quarter
            var dateRange = GetQuarterDateRange(year, quarterIndex);
            DateTime startDate = dateRange.StartDate;
            DateTime endDate = dateRange.EndDate;

            // Get data
            DataTable dt = GetQuarterlySalesData(startDate, endDate, salesmanName);

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
                //{ "Year", year.ToString() },
                //{ "Quarter", quarterText },
                //{ "Salesman", salesmanName },
                //{ "Period", $"{startDate.ToString("dd-MMM-yyyy")} to {endDate.ToString("dd-MMM-yyyy")}" },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            //viewer.ShowReport("Quaterely_Sales_Party_Wise", dataSources, parameters, null);
            viewer.ShowReport("Quaterely_Sales_Party_Wise", dataSources, parameters, null);

        }

        // 📌 GET QUARTERLY SALES DATA
        private DataTable GetQuarterlySalesData(DateTime startDate, DateTime endDate, string salesmanName)
        {
            DataTable dt = new DataTable();

            // Get salesman ID if not "All"
            string salesmanId = "All";
            if (salesmanName != "All")
            {
                salesmanId = GetSalesmanIdByName(salesmanName);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        P.PartyID,
                        P.PartyName,
                        P.City,
                        S.SectorID,
                        SEC.SectorName,
                        SM.SalesmanName,
                        COUNT(DISTINCT S.SaleId) AS TotalInvoices,
                        SUM(SB.QtyPack * SB.Multiplier + SB.QtyLose) AS TotalQuantity,
                        SUM(SB.TTLValue) AS TotalSalesValue,
                        SUM(SB.TTLSTax) AS TotalSalesTax,
                        SUM(SB.TTLFTax) AS TotalFTax,
                        SUM(SB.TTLValue + SB.TTLSTax + SB.TTLFTax) AS NetSalesAmount,
                        AVG(DATEDIFF(day, S.SaleDate, GETDATE())) AS AvgDaysSinceSale
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Sectors SEC ON P.SectorID = SEC.SectorID
                    LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID
                    WHERE S.SaleDate BETWEEN @StartDate AND @EndDate
                    AND S.IsPosted = 1
                    AND SB.IsDeleted = 0
                    AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                    GROUP BY P.PartyID, P.PartyName, P.City, S.SectorID, SEC.SectorName, SM.SalesmanName
                    ORDER BY NetSalesAmount DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@SalesmanId", salesmanId);

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // 📌 COMBOBOX CHANGED EVENTS
        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }

        private void selectyearquartercombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }

        private void numericUpDownforselectyear_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }

        // 📌 LABEL CLICKS (if needed for any action)
        private void label4_Click(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }
    }
}