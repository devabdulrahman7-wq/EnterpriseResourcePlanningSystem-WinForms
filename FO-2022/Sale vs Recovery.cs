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
    public partial class Sale_vs_Recovery : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Sale_vs_Recovery()
        {
            InitializeComponent();
        }

        private void Sale_vs_Recovery_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = DateTime.Now.AddDays(-30);
            todatepcker.Value = DateTime.Now;

            // Load salesman combobox
            LoadSalesmanComboBox();
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
                                salesmancombobox.Items.Add(new
                                {
                                    Text = reader["SalesmanName"].ToString(),
                                    Value = reader["SalesmanID"].ToString()
                                });
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
            if (fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be greater than To date", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected salesman
            string selectedSalesmanId = "All";
            string salesmanName = "All";

            if (salesmancombobox.SelectedItem != null && salesmancombobox.SelectedIndex > 0)
            {
                dynamic selectedItem = salesmancombobox.SelectedItem;
                selectedSalesmanId = selectedItem.Value;
                salesmanName = selectedItem.Text;
            }

            // Get data
            DataTable dt = GetSaleVsRecoveryData(
                fromdatepcker.Value,
                todatepcker.Value,
                selectedSalesmanId);

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
                //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                //{ "Salesman", salesmanName },
                //{ "ReportDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") }
            };

            // Show report
            Report_viewer viewer = new Report_viewer();
            viewer.ShowReport("Sale_vs_Recovery", dataSources);
        }

        // 📌 GET SALES VS RECOVERY DATA
        private DataTable GetSaleVsRecoveryData(DateTime fromDate, DateTime toDate, string salesmanId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        S.SaleDate,
                        S.BillNo,
                        S.CustomerId,
                        P.PartyName AS CustomerName,
                        SM.SalesmanName,
                        SUM(SB.TTLValue) AS TotalSalesValue,
                        SUM(SB.TTLSTax) AS TotalSalesTax,
                        SUM(SB.TTLFTax) AS TotalFTax,
                        (SUM(SB.TTLValue) + SUM(SB.TTLSTax) + SUM(SB.TTLFTax)) AS NetSalesAmount,
                        R.RecoveryDate,
                        SUM(ISNULL(RB.Amount, 0)) AS TotalRecoveryAmount,
                        (SUM(SB.TTLValue) + SUM(SB.TTLSTax) + SUM(SB.TTLFTax)) - SUM(ISNULL(RB.Amount, 0)) AS Balance
                    FROM Sales S
                    INNER JOIN SalesBody SB ON S.SaleId = SB.SaleId
                    INNER JOIN Parties P ON S.CustomerId = P.PartyID
                    LEFT JOIN Salesmen SM ON S.SalesmanId = SM.SalesmanID
                    LEFT JOIN RecoveryBody RB ON S.CustomerId = RB.CustomerID 
                        AND RB.IsDeleted = 0
                    LEFT JOIN Recovery R ON RB.RecoveryID = R.RecoveryID 
                        AND R.RecoveryDate BETWEEN @FromDate AND @ToDate
                    WHERE S.SaleDate BETWEEN @FromDate AND @ToDate
                    AND S.IsPosted = 1
                    AND (@SalesmanId = 'All' OR S.SalesmanId = @SalesmanId)
                    GROUP BY 
                        S.SaleDate, S.BillNo, S.CustomerId, P.PartyName,
                        SM.SalesmanName, R.RecoveryDate
                    ORDER BY S.SaleDate, S.BillNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
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

        // 📌 DATE CHANGED EVENTS
        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional validation
        }

        // 📌 SALESMAN SELECTION CHANGED
        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add logic if needed
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}