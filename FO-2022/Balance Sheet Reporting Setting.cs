using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace FO_2022
{
    public partial class Balance_Sheet_Reporting_Setting : Form
    {
        private string connectionString;
        private DataTable accountsData;
        private string currentAccountType;

        public Balance_Sheet_Reporting_Setting()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeDataGridView();
            LoadAccountTypes();
        }

        private void InitializeConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading connection string: " + ex.Message);
            }
        }

        private void InitializeDataGridView()
        {
            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Add columns
            dataGridView1.Columns.Add("AccountNo", "Account No");
            dataGridView1.Columns.Add("AccountName", "Account Name");

            // Add checkbox column for Settings
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.HeaderText = "Setting";
            checkColumn.Name = "Setting";
            checkColumn.Width = 70;
            dataGridView1.Columns.Add(checkColumn);

            // Set column properties
            dataGridView1.Columns["AccountNo"].Width = 100;
            dataGridView1.Columns["AccountName"].Width = 200;
            dataGridView1.Columns["AccountNo"].ReadOnly = true;
            dataGridView1.Columns["AccountName"].ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void LoadAccountTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            accountcombobox.Items.Clear();
                            while (reader.Read())
                            {
                                accountcombobox.Items.Add(reader["AccountType"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account types: " + ex.Message);
            }
        }

       
        private void LoadAccountsByType(string accountType)
        {
            try
            {
                currentAccountType = accountType;
                dataGridView1.Rows.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load accounts
                    string query = "SELECT AccountNo, AccountName FROM ChartOfAccounts WHERE AccountType = @AccountType ORDER BY AccountNo";

                    // Load saved settings from Registry table
                    Dictionary<string, bool> savedSettings = LoadBalanceSheetSettings(accountType, conn);

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string accountNo = reader["AccountNo"].ToString();
                                string accountName = reader["AccountName"].ToString();

                                // Check if this account is saved in registry
                                string settingKey = $"BALANCE_SHEET_{accountType}_{accountNo}";
                                bool isChecked = savedSettings.ContainsKey(settingKey) && savedSettings[settingKey];

                                dataGridView1.Rows.Add(accountNo, accountName, isChecked);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message);
            }
        }

        private Dictionary<string, bool> LoadBalanceSheetSettings(string accountType, SqlConnection conn)
        {
            Dictionary<string, bool> settings = new Dictionary<string, bool>();

            try
            {
                string query = @"SELECT RegistryKey, RegistryValue 
                        FROM Registry 
                        WHERE RegistryKey LIKE @Pattern";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pattern", $"BALANCE_SHEET_{accountType}_%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader["RegistryKey"].ToString();
                            string value = reader["RegistryValue"].ToString();

                            settings[key] = value == "1" || value.ToLower() == "true";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message);
            }

            return settings;
        }

        private DataTable LoadSavedSettings(string accountType, SqlConnection conn)
        {
            DataTable settings = new DataTable();
            try
            {
                string query = "SELECT AccountNo, IsSelected FROM BalanceSheetSettings WHERE AccountType = @AccountType";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountType", accountType);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(settings);
                    }
                }
            }
            catch
            {
                // Table might not exist yet, return empty table
            }
            return settings;
        }

        private bool IsAccountSelected(string accountNo, DataTable savedSettings)
        {
            if (savedSettings.Rows.Count == 0)
                return false;

            foreach (DataRow row in savedSettings.Rows)
            {
                if (row["AccountNo"].ToString() == accountNo)
                {
                    return Convert.ToBoolean(row["IsSelected"]);
                }
            }
            return false;
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedItem != null)
            {
                string selectedType = accountcombobox.SelectedItem.ToString();
                LoadAccountsByType(selectedType);
            }
        }

       
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentAccountType))
            {
                MessageBox.Show("Please select an Account Type first.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Begin transaction
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Delete existing settings for this account type
                        string deleteQuery = @"DELETE FROM Registry 
                                      WHERE RegistryKey LIKE @Pattern";

                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@Pattern", $"BALANCE_SHEET_{currentAccountType}_%");
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Insert new settings
                        string insertQuery = @"INSERT INTO Registry (RegistryKey, RegistryValue, Description) 
                                      VALUES (@RegistryKey, @RegistryValue, @Description)";

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["AccountNo"].Value != null)
                            {
                                string accountNo = row.Cells["AccountNo"].Value.ToString();
                                bool isSelected = false;

                                if (row.Cells["Setting"].Value != null)
                                {
                                    isSelected = Convert.ToBoolean(row.Cells["Setting"].Value);
                                }

                                string registryKey = $"BALANCE_SHEET_{currentAccountType}_{accountNo}";
                                string registryValue = isSelected ? "1" : "0";
                                string description = $"Balance Sheet Setting for {row.Cells["AccountName"].Value}";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@RegistryKey", registryKey);
                                    insertCmd.Parameters.AddWithValue("@RegistryValue", registryValue);
                                    insertCmd.Parameters.AddWithValue("@Description", description);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Settings saved successfully in Registry!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error saving settings: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentAccountType))
            {
                MessageBox.Show("Please select an Account Type first.");
                return;
            }

            if (MessageBox.Show("Clear all settings for this account type?", "Confirm Clear",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells["Setting"].Value = false;
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex == dataGridView1.Columns["Setting"].Index && e.RowIndex >= 0)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void Balance_Sheet_Reporting_Setting_Load(object sender, EventArgs e)
        {

        }
    }
}

