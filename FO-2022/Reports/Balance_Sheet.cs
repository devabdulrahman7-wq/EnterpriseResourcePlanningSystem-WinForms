using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class Balance_Sheet : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Balance_Sheet()
        {
            InitializeComponent();
            // Set default dates
            fromdatepicker.Value = DateTime.Now.AddDays(-30);
            todatepicker.Value = DateTime.Now;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable data = GetChartOfAccountsData();

                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("No data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepicker.Value.ToString("dd/MM/yyyy") },
                    //{ "ToDate", todatepicker.Value.ToString("dd/MM/yyyy") }
                };

                using (Report_viewer viewer = new Report_viewer())
                {
                    var dataSources = new Dictionary<string, DataTable>
                    {
                        { "DataSet1", data }
                    };
                    viewer.ShowReport("Balance_Sheet.rdlc", dataSources, parameters);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetChartOfAccountsData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        AccountNo,
                        AccountName,
                        AccountType,
                        OpeningDebit,
                        OpeningCredit,
                        AdjustedDebit,
                        AdjustedCredit,
                        (AdjustedDebit - AdjustedCredit) AS Balance,
                        CASE WHEN (AdjustedDebit - AdjustedCredit) >= 0 
                             THEN (AdjustedDebit - AdjustedCredit) 
                             ELSE 0 END AS DebitBalance,
                        CASE WHEN (AdjustedDebit - AdjustedCredit) < 0 
                             THEN ABS(AdjustedDebit - AdjustedCredit) 
                             ELSE 0 END AS CreditBalance
                    FROM ChartOfAccounts 
                    WHERE IsPosted = 1 
                    ORDER BY AccountType, AccountNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
    }
}