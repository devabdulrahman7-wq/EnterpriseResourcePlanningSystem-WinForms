using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Season_Recovery : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Season_Recovery()
        {
            InitializeComponent();
        }

        private void Season_Recovery_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default radio buttons
                orderbyrecoveryradiobtn.Checked = true;
                ascendingradiobtn.Checked = true;

                // Load salesmen
                LoadSalesmen();

                // Load seasons (we'll use years from Recovery table)
                LoadSeasons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSalesmen()
        {
            try
            {
                slctsalesmancombobox.Items.Clear();
                slctsalesmancombobox.Items.Add("ALL SALESMEN"); // Default option

                string query = "SELECT SalesmanID, SalesmanName FROM Salesmen WHERE IsStockHolder = 0 ORDER BY SalesmanName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string salesmanInfo = $"{reader["SalesmanID"]} - {reader["SalesmanName"]}";
                            slctsalesmancombobox.Items.Add(salesmanInfo);
                        }
                    }
                }

                slctsalesmancombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salesmen: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSeasons()
        {
            try
            {
                slctseasoncombobox.Items.Clear();
                slctseasoncombobox.Items.Add("ALL SEASONS"); // Default option

                // Get distinct years from Recovery table
                string query = @"
                    SELECT DISTINCT YEAR(RecoveryDate) as SeasonYear 
                    FROM Recovery 
                    WHERE IsPosted = 1 
                    ORDER BY YEAR(RecoveryDate) DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int year = Convert.ToInt32(reader["SeasonYear"]);
                            slctseasoncombobox.Items.Add($"Season {year}");
                        }
                    }
                }

                // If no seasons found, add current year
                if (slctseasoncombobox.Items.Count == 1) // Only "ALL SEASONS"
                {
                    slctseasoncombobox.Items.Add($"Season {DateTime.Now.Year}");
                }

                slctseasoncombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading seasons: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void slctsalesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: You can add logic here if needed
        }

        private void slctseasoncombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: You can add logic here if needed
        }

        // Validate before generating report
        private bool ValidateInput()
        {
            // No specific validation needed - all fields are optional
            return true;
        }

        // Generate Report
        private void GenerateReport()
        {
            try
            {
                // Validate input
                if (!ValidateInput()) return;

                // Get data for report
                DataTable dt = GetSeasonRecoveryData(
                    slctsalesmancombobox.Text,
                    slctseasoncombobox.Text
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No recovery data found for the selected criteria.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Apply sorting
                dt = ApplySorting(dt);

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Season Recovery Report" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    //{ "SortOrder", GetSortOrderDescription() }
                };

                // Add filter parameters if applicable
                if (slctsalesmancombobox.SelectedIndex > 0)
                {
                    //parameters.Add("SalesmanFilter", slctsalesmancombobox.Text);
                }
                else
                {
                    //parameters.Add("SalesmanFilter", "ALL SALESMEN");
                }

                if (slctseasoncombobox.SelectedIndex > 0)
                {
                    //parameters.Add("SeasonFilter", slctseasoncombobox.Text);
                }
                else
                {
                    //parameters.Add("SeasonFilter", "ALL SEASONS");
                }

                // Show report viewer
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Season_Recovery", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetSeasonRecoveryData(string salesmanFilter, string seasonFilter)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("SalesmanID", typeof(string));
            dt.Columns.Add("SalesmanName", typeof(string));
            dt.Columns.Add("Season", typeof(string));
            dt.Columns.Add("TotalSales", typeof(decimal));
            dt.Columns.Add("TotalRecovery", typeof(decimal));
            dt.Columns.Add("RecoveryDays", typeof(int));
            dt.Columns.Add("AvgDailyRecovery", typeof(decimal));
            dt.Columns.Add("RecoveryPercentage", typeof(decimal));

            try
            {
                string query = @"
                    WITH SalesData AS (
                        -- Get total sales per salesman per year
                        SELECT 
                            s.SalesmanID,
                            YEAR(s.SaleDate) as SaleYear,
                            SUM(sb.TTLValue) as TotalSales
                        FROM Sales s
                        INNER JOIN SalesBody sb ON s.SaleId = sb.SaleId
                        WHERE s.IsPosted = 1 AND sb.IsDeleted = 0
                        GROUP BY s.SalesmanID, YEAR(s.SaleDate)
                    ),
                    RecoveryData AS (
                        -- Get total recovery and days with recovery per salesman per year
                        SELECT 
                            r.SalesmanID,
                            YEAR(r.RecoveryDate) as RecoveryYear,
                            SUM(rb.Amount) as TotalRecovery,
                            COUNT(DISTINCT r.RecoveryDate) as RecoveryDays
                        FROM Recovery r
                        INNER JOIN RecoveryBody rb ON r.RecoveryID = rb.RecoveryID
                        WHERE r.IsPosted = 1 AND rb.IsDeleted = 0 AND rb.IsPosted = 1
                        GROUP BY r.SalesmanID, YEAR(r.RecoveryDate)
                    )
                    SELECT 
                        sm.SalesmanID,
                        sm.SalesmanName,
                        CONCAT('Season ', COALESCE(rd.RecoveryYear, sd.SaleYear)) as Season,
                        ISNULL(sd.TotalSales, 0) as TotalSales,
                        ISNULL(rd.TotalRecovery, 0) as TotalRecovery,
                        ISNULL(rd.RecoveryDays, 0) as RecoveryDays,
                        CASE 
                            WHEN ISNULL(rd.RecoveryDays, 0) > 0 
                            THEN ISNULL(rd.TotalRecovery, 0) / rd.RecoveryDays 
                            ELSE 0 
                        END as AvgDailyRecovery,
                        CASE 
                            WHEN ISNULL(sd.TotalSales, 0) > 0 
                            THEN (ISNULL(rd.TotalRecovery, 0) * 100) / sd.TotalSales 
                            ELSE 0 
                        END as RecoveryPercentage
                    FROM Salesmen sm
                    LEFT JOIN SalesData sd ON sm.SalesmanID = sd.SalesmanID
                    LEFT JOIN RecoveryData rd ON sm.SalesmanID = rd.SalesmanID AND sd.SaleYear = rd.RecoveryYear
                    WHERE 1=1";

                // Add filters if provided
                if (salesmanFilter != "ALL SALESMEN")
                {
                    string salesmanId = salesmanFilter.Split('-')[0].Trim();
                    query += " AND sm.SalesmanID = @SalesmanID";
                }

                if (seasonFilter != "ALL SEASONS")
                {
                    int year = Convert.ToInt32(seasonFilter.Replace("Season ", "").Trim());
                    query += " AND (sd.SaleYear = @Year OR rd.RecoveryYear = @Year)";
                }

                query += " ORDER BY sm.SalesmanName, COALESCE(rd.RecoveryYear, sd.SaleYear) DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (salesmanFilter != "ALL SALESMEN")
                        {
                            string salesmanId = salesmanFilter.Split('-')[0].Trim();
                            cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                        }

                        if (seasonFilter != "ALL SEASONS")
                        {
                            int year = Convert.ToInt32(seasonFilter.Replace("Season ", "").Trim());
                            cmd.Parameters.AddWithValue("@Year", year);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["SalesmanID"] = reader["SalesmanID"];
                                row["SalesmanName"] = reader["SalesmanName"];
                                row["Season"] = reader["Season"];
                                row["TotalSales"] = reader["TotalSales"];
                                row["TotalRecovery"] = reader["TotalRecovery"];
                                row["RecoveryDays"] = reader["RecoveryDays"];
                                row["AvgDailyRecovery"] = reader["AvgDailyRecovery"];
                                row["RecoveryPercentage"] = reader["RecoveryPercentage"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching season recovery data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private DataTable ApplySorting(DataTable dt)
        {
            if (dt.Rows.Count == 0) return dt;

            // Determine sort column
            string sortColumn = "TotalRecovery"; // Default
            if (orderbysalesradiobtn.Checked) // Add this radio button if you have it
                sortColumn = "TotalSales";
            else if (orderbydaysradiobtn.Checked)
                sortColumn = "RecoveryDays";

            // Determine sort direction
            string sortDirection = "ASC";
            if (descendingradiobtn.Checked)
                sortDirection = "DESC";

            // Create DataView and apply sort
            DataView dv = dt.DefaultView;
            dv.Sort = $"{sortColumn} {sortDirection}";

            return dv.ToTable();
        }

        private string GetSortOrderDescription()
        {
            string orderBy = "Recovery";
            if (orderbysalesradiobtn.Checked) // Add this radio button if you have it
                orderBy = "Sale";
            else if (orderbydaysradiobtn.Checked)
                orderBy = "Days";

            string direction = ascendingradiobtn.Checked ? "Ascending" : "Descending";

            return $"Sorted by {orderBy} ({direction})";
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Radio button events
        private void orderbyrecoveryradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            // Sorting will be applied during report generation
        }

        private void orderbydaysradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            // Sorting will be applied during report generation
        }

        private void ascendingradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            // Sorting will be applied during report generation
        }

        private void descendingradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            // Sorting will be applied during report generation
        }
    }
}