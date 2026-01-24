using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Accounts_Balancing : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private readonly Dictionary<string, string> accountTypes = new Dictionary<string, string>
        {
            {"All Accounts", ""},
            {"Asset", "Asset"},
            {"Liability", "Liability"},
            {"Capital", "Capital"},
            {"Revenue", "Revenue"},
            {"Expense", "Expense"},
            {"Parties", "Parties"},
            {"Salesman", "Salesman"}
        };

        public Accounts_Balancing()
        {
            InitializeComponent();
        }

        private void Accounts_Balancing_Load(object sender, EventArgs e)
        {
            LoadAccountTypes();
            accountcombobox.SelectedIndex = 0;
        }

        private void LoadAccountTypes()
        {
            accountcombobox.Items.Clear();
            accountcombobox.DisplayMember = "Key";
            accountcombobox.ValueMember = "Value";

            foreach (var type in accountTypes)
            {
                accountcombobox.Items.Add(type);
            }
        }

        private void ShowReport()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var selectedType = accountcombobox.SelectedItem as KeyValuePair<string, string>?;
                if (!selectedType.HasValue)
                {
                    MessageBox.Show("Please select an account type", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string accountTypeFilter = selectedType.Value.Value;
                bool excludeZeroBalance = excludeaccountshavingzerobalancecheckbox.Checked;

                DataTable reportData = GetAccountOpeningBalanceData(accountTypeFilter, excludeZeroBalance);

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found for the selected criteria", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                var parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", $"Accounts Opening Balance - {selectedType.Value.Key}" },
                    //{ "PrintDate", DateTime.Now.ToString("dd/MM/yyyy hh:mm tt") },
                    //{ "FilterText", GetFilterText(selectedType.Value.Key, excludeZeroBalance) }
                };

                var reportViewer = new Report_viewer();
                reportViewer.ShowReport("Account_Opening_Balance.rdlc", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private DataTable GetAccountOpeningBalanceData(string accountType, bool excludeZeroBalance)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Build query step by step
                    string query = "SELECT AccountNo, AccountName, OpeningDebit, OpeningCredit, AdjustedDebit, AdjustedCredit FROM ChartOfAccounts";

                    // Add WHERE if needed
                    List<string> conditions = new List<string>();

                    if (!string.IsNullOrEmpty(accountType))
                    {
                        if (accountType == "Parties")
                        {
                            conditions.Add("AccountType IN ('Customer', 'Vendor', 'Other Party')");
                        }
                        else if (accountType == "Salesman")
                        {
                            conditions.Add("AccountType = 'Salesman'");
                        }
                        else
                        {
                            conditions.Add("AccountType = @AccountType");
                        }
                    }

                    if (excludeZeroBalance)
                    {
                        conditions.Add("(OpeningDebit <> 0 OR OpeningCredit <> 0 OR AdjustedDebit <> 0 OR AdjustedCredit <> 0)");
                    }

                    // Add WHERE clause if conditions exist
                    if (conditions.Count > 0)
                    {
                        query += " WHERE " + string.Join(" AND ", conditions);
                    }

                    query += " ORDER BY AccountNo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(accountType) &&
                            accountType != "Parties" &&
                            accountType != "Salesman")
                        {
                            command.Parameters.AddWithValue("@AccountType", accountType);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        private string GetFilterText(string accountType, bool excludeZeroBalance)
        {
            string filter = $"Account Type: {accountType}";

            if (excludeZeroBalance)
            {
                filter += " | Excluding Zero Balance Accounts";
            }

            return filter;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional
        }

        private void excludeaccountshavingzerobalancecheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // Optional
        }
    }
}