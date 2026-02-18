using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Chart_of_Accounts : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Chart_of_Accounts()
        {
            InitializeComponent();
            LoadAccountTypes();
        }

        private void LoadAccountTypes()
        {
            try
            {
                accountcombobox.Items.Clear();
                accountcombobox.Items.Add("ALL ACCOUNTS");

                string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accountcombobox.Items.Add(reader["AccountType"].ToString());
                        }
                    }
                }

                accountcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading account types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport()
        {
            try
            {
                string selectedAccountType = accountcombobox.Text;
                DataTable dt = GetChartOfAccountsData(selectedAccountType);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No accounts found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Chart of Accounts" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                if (accountcombobox.SelectedIndex > 0)
                {
                    //parameters.Add("AccountTypeFilter", selectedAccountType);
                }
                else
                {
                    //parameters.Add("AccountTypeFilter", "ALL ACCOUNTS");
                }

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Chats_of_Accounts_report", dataSources);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetChartOfAccountsData(string accountType)
        {
            DataTable dt = new DataTable();

            // Original columns only
            dt.Columns.Add("AccountNo", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("AccountType", typeof(string));
            dt.Columns.Add("Narration", typeof(string));
            dt.Columns.Add("OpeningDebit", typeof(decimal));
            dt.Columns.Add("OpeningCredit", typeof(decimal));

            try
            {
                string query = "SELECT AccountNo, AccountName, AccountType, Narration, OpeningDebit, OpeningCredit FROM ChartOfAccounts";

                if (accountType != "ALL ACCOUNTS")
                {
                    query += " WHERE AccountType = @AccountType";
                }

                query += " ORDER BY AccountNo";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (accountType != "ALL ACCOUNTS")
                        {
                            cmd.Parameters.AddWithValue("@AccountType", accountType);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["AccountNo"] = reader["AccountNo"];
                                row["AccountName"] = reader["AccountName"];
                                row["AccountType"] = reader["AccountType"];
                                row["Narration"] = reader["Narration"];
                                row["OpeningDebit"] = reader["OpeningDebit"];
                                row["OpeningCredit"] = reader["OpeningCredit"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void prntbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();
        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}