//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Expense_Report_Setting : Form
//    {
//        public Expense_Report_Setting()
//        {
//            InitializeComponent();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {

//        }
//    }
//}




//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Expense_Report_Setting : Form
//    {
//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//        private DataTable accountsDataTable;

//        public Expense_Report_Setting()
//        {
//            InitializeComponent();
//            LoadAccountTypesToComboBox();
//            // Load all accounts initially
//            LoadAccountsToDataGridView("All");
//        }

//        // Load distinct AccountTypes from ChartOfAccounts to ComboBox
//        private void LoadAccountTypesToComboBox()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    accountcombobox.Items.Clear();
//                    accountcombobox.Items.Add("All Types"); // First item for all types

//                    while (reader.Read())
//                    {
//                        accountcombobox.Items.Add(reader["AccountType"].ToString());
//                    }

//                    reader.Close();
//                    conn.Close();

//                    if (accountcombobox.Items.Count > 0)
//                        accountcombobox.SelectedIndex = 0;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading account types: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Load accounts to DataGridView based on selected AccountType
//        private void LoadAccountsToDataGridView(string accountType)
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = @"
//                        SELECT 
//                            AccountNo, 
//                            AccountName, 
//                            CASE WHEN ExpFlag = 1 THEN 'Yes' ELSE 'No' END as Setting,
//                            ExpFlag
//                        FROM ChartOfAccounts 
//                        WHERE 1=1";

//                    // Add filter if not "All Types"
//                    if (accountType != "All")
//                    {
//                        query += " AND AccountType = @AccountType";
//                    }

//                    query += " ORDER BY AccountNo";

//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    if (accountType != "All")
//                    {
//                        cmd.Parameters.AddWithValue("@AccountType", accountType);
//                    }

//                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                    accountsDataTable = new DataTable();
//                    adapter.Fill(accountsDataTable);

//                    // Configure DataGridView with only 4 columns including checkbox
//                    ConfigureDataGridView();

//                    conn.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading accounts: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Configure DataGridView with required columns
//        private void ConfigureDataGridView()
//        {
//            // Clear existing columns
//            dataGridView1.Columns.Clear();

//            // 1. Add Checkbox Column (for selection)
//            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
//            checkBoxColumn.HeaderText = "Select";
//            checkBoxColumn.Name = "Select";
//            checkBoxColumn.Width = 50;
//            dataGridView1.Columns.Add(checkBoxColumn);

//            // 2. Add AccountNo Column
//            DataGridViewTextBoxColumn accNoColumn = new DataGridViewTextBoxColumn();
//            accNoColumn.HeaderText = "Account No";
//            accNoColumn.Name = "AccountNo";
//            accNoColumn.DataPropertyName = "AccountNo";
//            accNoColumn.ReadOnly = true;
//            accNoColumn.Width = 100;
//            dataGridView1.Columns.Add(accNoColumn);

//            // 3. Add AccountName Column
//            DataGridViewTextBoxColumn accNameColumn = new DataGridViewTextBoxColumn();
//            accNameColumn.HeaderText = "Account Name";
//            accNameColumn.Name = "AccountName";
//            accNameColumn.DataPropertyName = "AccountName";
//            accNameColumn.ReadOnly = true;
//            accNameColumn.Width = 200;
//            dataGridView1.Columns.Add(accNameColumn);

//            // 4. Add Setting Column
//            DataGridViewTextBoxColumn settingColumn = new DataGridViewTextBoxColumn();
//            settingColumn.HeaderText = "Setting";
//            settingColumn.Name = "Setting";
//            settingColumn.DataPropertyName = "Setting";
//            settingColumn.ReadOnly = true;
//            settingColumn.Width = 80;
//            dataGridView1.Columns.Add(settingColumn);

//            // Set DataSource
//            dataGridView1.DataSource = accountsDataTable;

//            // Hide ExpFlag column from display (if it exists in DataTable)
//            if (dataGridView1.Columns.Contains("ExpFlag"))
//            {
//                dataGridView1.Columns["ExpFlag"].Visible = false;
//            }

//            // Pre-check checkboxes based on ExpFlag
//            PreCheckBoxesBasedOnExpFlag();
//        }

//        // Pre-check checkboxes where ExpFlag = 1
//        private void PreCheckBoxesBasedOnExpFlag()
//        {
//            if (accountsDataTable != null)
//            {
//                foreach (DataGridViewRow row in dataGridView1.Rows)
//                {
//                    if (row.Cells["AccountNo"].Value != null)
//                    {
//                        // Find corresponding row in DataTable
//                        string accountNo = row.Cells["AccountNo"].Value.ToString();
//                        DataRow[] foundRows = accountsDataTable.Select($"AccountNo = '{accountNo}'");

//                        if (foundRows.Length > 0)
//                        {
//                            // Check if ExpFlag = 1
//                            if (foundRows[0]["ExpFlag"] != DBNull.Value &&
//                                Convert.ToInt32(foundRows[0]["ExpFlag"]) == 1)
//                            {
//                                row.Cells["Select"].Value = true;
//                            }
//                            else
//                            {
//                                row.Cells["Select"].Value = false;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (accountcombobox.SelectedItem != null)
//            {
//                string selectedType = accountcombobox.SelectedItem.ToString();
//                if (selectedType == "All Types")
//                {
//                    LoadAccountsToDataGridView("All");
//                }
//                else
//                {
//                    LoadAccountsToDataGridView(selectedType);
//                }
//            }
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Update "Setting" column when checkbox is clicked
//            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // Checkbox column index
//            {
//                DataGridViewCheckBoxCell checkBox = dataGridView1.Rows[e.RowIndex].Cells["Select"] as DataGridViewCheckBoxCell;

//                if (checkBox != null)
//                {
//                    bool isChecked = Convert.ToBoolean(checkBox.Value);

//                    // Update "Setting" column
//                    dataGridView1.Rows[e.RowIndex].Cells["Setting"].Value = isChecked ? "Yes" : "No";

//                    // Update the underlying DataTable
//                    if (accountsDataTable != null && e.RowIndex < accountsDataTable.Rows.Count)
//                    {
//                        accountsDataTable.Rows[e.RowIndex]["Setting"] = isChecked ? "Yes" : "No";
//                    }
//                }
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Create a list to store accounts to update
//                List<Tuple<string, bool>> accountsToUpdate = new List<Tuple<string, bool>>();

//                // Collect data from DataGridView
//                foreach (DataGridViewRow row in dataGridView1.Rows)
//                {
//                    if (row.Cells["AccountNo"].Value != null &&
//                        row.Cells["Select"].Value != null)
//                    {
//                        string accountNo = row.Cells["AccountNo"].Value.ToString();
//                        bool isSelected = Convert.ToBoolean(row.Cells["Select"].Value);

//                        accountsToUpdate.Add(new Tuple<string, bool>(accountNo, isSelected));
//                    }
//                }

//                // Update database
//                UpdateExpFlagInDatabase(accountsToUpdate);

//                MessageBox.Show("Expense settings saved successfully!", "Success",
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving expense settings: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void UpdateExpFlagInDatabase(List<Tuple<string, bool>> accountsToUpdate)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                // Start transaction for data consistency
//                SqlTransaction transaction = conn.BeginTransaction();

//                try
//                {
//                    foreach (var account in accountsToUpdate)
//                    {
//                        string accountNo = account.Item1;
//                        bool isSelected = account.Item2;
//                        int expFlagValue = isSelected ? 1 : 0;

//                        string updateQuery = @"
//                            UPDATE ChartOfAccounts 
//                            SET ExpFlag = @ExpFlag 
//                            WHERE AccountNo = @AccountNo";

//                        SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction);
//                        cmd.Parameters.AddWithValue("@ExpFlag", expFlagValue);
//                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);

//                        cmd.ExecuteNonQuery();
//                    }

//                    // Commit transaction
//                    transaction.Commit();

//                    // Refresh data to show updated settings
//                    if (accountcombobox.SelectedItem != null)
//                    {
//                        string selectedType = accountcombobox.SelectedItem.ToString();
//                        LoadAccountsToDataGridView(selectedType == "All Types" ? "All" : selectedType);
//                    }
//                }
//                catch (Exception)
//                {
//                    transaction.Rollback();
//                    throw;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            // Clear all checkbox selections
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                row.Cells["Select"].Value = false;
//                row.Cells["Setting"].Value = "No";
//            }
//        }

//        // Optional: Select All button
//        private void selectAllbtn_Click(object sender, EventArgs e)
//        {
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                row.Cells["Select"].Value = true;
//                row.Cells["Setting"].Value = "Yes";
//            }
//        }

//        // Optional: Deselect All button
//        private void deselectAllbtn_Click(object sender, EventArgs e)
//        {
//            clearbtn_Click(sender, e);
//        }
//    }
//}




//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Expense_Report_Setting : Form
//    {
//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//        private DataTable accountsDataTable;

//        public Expense_Report_Setting()
//        {
//            InitializeComponent();
//            LoadAccountTypesToComboBox();
//            // Load all accounts initially
//            LoadAccountsToDataGridView("All");
//        }

//        // Load distinct AccountTypes from ChartOfAccounts to ComboBox
//        private void LoadAccountTypesToComboBox()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    SqlDataReader reader = cmd.ExecuteReader();

//                    accountcombobox.Items.Clear();
//                    accountcombobox.Items.Add("All Types"); // First item for all types

//                    while (reader.Read())
//                    {
//                        accountcombobox.Items.Add(reader["AccountType"].ToString());
//                    }

//                    reader.Close();
//                    conn.Close();

//                    if (accountcombobox.Items.Count > 0)
//                        accountcombobox.SelectedIndex = 0;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading account types: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Load accounts to DataGridView based on selected AccountType
//        private void LoadAccountsToDataGridView(string accountType)
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = @"
//                        SELECT 
//                            AccountNo, 
//                            AccountName, 
//                            ExpFlag
//                        FROM ChartOfAccounts 
//                        WHERE 1=1";

//                    // Add filter if not "All Types"
//                    if (accountType != "All")
//                    {
//                        query += " AND AccountType = @AccountType";
//                    }

//                    query += " ORDER BY AccountNo";

//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    if (accountType != "All")
//                    {
//                        cmd.Parameters.AddWithValue("@AccountType", accountType);
//                    }

//                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                    accountsDataTable = new DataTable();
//                    adapter.Fill(accountsDataTable);

//                    // Configure DataGridView with only 3 columns
//                    ConfigureDataGridView();

//                    conn.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading accounts: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Configure DataGridView with required columns (only 3 columns)
//        private void ConfigureDataGridView()
//        {
//            // Clear existing columns
//            dataGridView1.Columns.Clear();

//            // 1. Add AccountNo Column
//            DataGridViewTextBoxColumn accNoColumn = new DataGridViewTextBoxColumn();
//            accNoColumn.HeaderText = "Account No";
//            accNoColumn.Name = "AccountNo";
//            accNoColumn.DataPropertyName = "AccountNo";
//            accNoColumn.ReadOnly = true;
//            accNoColumn.Width = 100;
//            dataGridView1.Columns.Add(accNoColumn);

//            // 2. Add AccountName Column
//            DataGridViewTextBoxColumn accNameColumn = new DataGridViewTextBoxColumn();
//            accNameColumn.HeaderText = "Account Name";
//            accNameColumn.Name = "AccountName";
//            accNameColumn.DataPropertyName = "AccountName";
//            accNameColumn.ReadOnly = true;
//            accNameColumn.Width = 200;
//            dataGridView1.Columns.Add(accNameColumn);

//            // 3. Add Setting Column (Checkbox)
//            DataGridViewCheckBoxColumn settingColumn = new DataGridViewCheckBoxColumn();
//            settingColumn.HeaderText = "Setting";
//            settingColumn.Name = "Setting";
//            settingColumn.Width = 60;
//            dataGridView1.Columns.Add(settingColumn);

//            // Set DataSource
//            dataGridView1.DataSource = accountsDataTable;

//            // Pre-check checkboxes based on ExpFlag
//            PreCheckBoxesBasedOnExpFlag();
//        }

//        // Pre-check checkboxes where ExpFlag = 1
//        private void PreCheckBoxesBasedOnExpFlag()
//        {
//            if (accountsDataTable != null && dataGridView1.Rows.Count > 0)
//            {
//                for (int i = 0; i < dataGridView1.Rows.Count; i++)
//                {
//                    DataGridViewRow row = dataGridView1.Rows[i];

//                    // Make sure we're within bounds of DataTable
//                    if (i < accountsDataTable.Rows.Count)
//                    {
//                        DataRow dataRow = accountsDataTable.Rows[i];

//                        // Check if ExpFlag = 1
//                        if (dataRow["ExpFlag"] != DBNull.Value &&
//                            Convert.ToInt32(dataRow["ExpFlag"]) == 1)
//                        {
//                            row.Cells["Setting"].Value = true;
//                        }
//                        else
//                        {
//                            row.Cells["Setting"].Value = false;
//                        }
//                    }
//                }
//            }
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (accountcombobox.SelectedItem != null)
//            {
//                string selectedType = accountcombobox.SelectedItem.ToString();
//                if (selectedType == "All Types")
//                {
//                    LoadAccountsToDataGridView("All");
//                }
//                else
//                {
//                    LoadAccountsToDataGridView(selectedType);
//                }
//            }
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle checkbox click in Setting column (column index 2)
//            if (e.RowIndex >= 0 && e.ColumnIndex == 2) // Setting checkbox column
//            {
//                // The checkbox value is automatically toggled by DataGridView
//                // We can optionally add additional logic here if needed

//                // Force the cell to repaint
//                dataGridView1.InvalidateCell(e.ColumnIndex, e.RowIndex);
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Create a list to store accounts to update
//                List<Tuple<string, bool>> accountsToUpdate = new List<Tuple<string, bool>>();

//                // Collect data from DataGridView
//                foreach (DataGridViewRow row in dataGridView1.Rows)
//                {
//                    if (row.Cells["AccountNo"].Value != null &&
//                        row.Cells["Setting"].Value != null)
//                    {
//                        string accountNo = row.Cells["AccountNo"].Value.ToString();
//                        bool isChecked = Convert.ToBoolean(row.Cells["Setting"].Value);

//                        accountsToUpdate.Add(new Tuple<string, bool>(accountNo, isChecked));
//                    }
//                }

//                // Update database
//                UpdateExpFlagInDatabase(accountsToUpdate);

//                MessageBox.Show("Expense settings saved successfully!", "Success",
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving expense settings: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void UpdateExpFlagInDatabase(List<Tuple<string, bool>> accountsToUpdate)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                // Start transaction for data consistency
//                SqlTransaction transaction = conn.BeginTransaction();

//                try
//                {
//                    foreach (var account in accountsToUpdate)
//                    {
//                        string accountNo = account.Item1;
//                        bool isChecked = account.Item2;
//                        int expFlagValue = isChecked ? 1 : 0;

//                        string updateQuery = @"
//                            UPDATE ChartOfAccounts 
//                            SET ExpFlag = @ExpFlag 
//                            WHERE AccountNo = @AccountNo";

//                        SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction);
//                        cmd.Parameters.AddWithValue("@ExpFlag", expFlagValue);
//                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);

//                        cmd.ExecuteNonQuery();
//                    }

//                    // Commit transaction
//                    transaction.Commit();

//                    // Refresh data to show updated settings
//                    if (accountcombobox.SelectedItem != null)
//                    {
//                        string selectedType = accountcombobox.SelectedItem.ToString();
//                        LoadAccountsToDataGridView(selectedType == "All Types" ? "All" : selectedType);
//                    }
//                }
//                catch (Exception)
//                {
//                    transaction.Rollback();
//                    throw;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            // Clear all checkbox selections (set all checkboxes to false)
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                row.Cells["Setting"].Value = false;
//            }
//        }

//        // Optional: Select All button (check all checkboxes)
//        private void selectAllbtn_Click(object sender, EventArgs e)
//        {
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                row.Cells["Setting"].Value = true;
//            }
//        }
//    }
//}




using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace FO_2022
{
    public partial class Expense_Report_Setting : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable accountsDataTable;

        public Expense_Report_Setting()
        {
            InitializeComponent();
            LoadAccountTypesToComboBox();
            // Load all accounts initially
            LoadAccountsToDataGridView("All");
        }

        // Load distinct AccountTypes from ChartOfAccounts to ComboBox
        private void LoadAccountTypesToComboBox()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    accountcombobox.Items.Clear();
                    accountcombobox.Items.Add("All Types"); // First item for all types

                    while (reader.Read())
                    {
                        accountcombobox.Items.Add(reader["AccountType"].ToString());
                    }

                    reader.Close();
                    conn.Close();

                    if (accountcombobox.Items.Count > 0)
                        accountcombobox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account types: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load accounts to DataGridView based on selected AccountType
        private void LoadAccountsToDataGridView(string accountType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query without ExpFlag in SELECT
                    string query = @"
                        SELECT 
                            AccountNo, 
                            AccountName
                        FROM ChartOfAccounts 
                        WHERE 1=1";

                    // Add filter if not "All Types"
                    if (accountType != "All")
                    {
                        query += " AND AccountType = @AccountType";
                    }

                    query += " ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (accountType != "All")
                    {
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    accountsDataTable = new DataTable();
                    adapter.Fill(accountsDataTable);

                    // Configure DataGridView with only 3 columns
                    ConfigureDataGridView();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Configure DataGridView with required columns (only 3 columns)
        private void ConfigureDataGridView()
        {
            // Clear existing columns
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            // 1. Add AccountNo Column
            DataGridViewTextBoxColumn accNoColumn = new DataGridViewTextBoxColumn();
            accNoColumn.HeaderText = "Account No";
            accNoColumn.Name = "AccountNo";
            accNoColumn.DataPropertyName = "AccountNo";
            accNoColumn.ReadOnly = true;
            accNoColumn.Width = 100;
            dataGridView1.Columns.Add(accNoColumn);

            // 2. Add AccountName Column
            DataGridViewTextBoxColumn accNameColumn = new DataGridViewTextBoxColumn();
            accNameColumn.HeaderText = "Account Name";
            accNameColumn.Name = "AccountName";
            accNameColumn.DataPropertyName = "AccountName";
            accNameColumn.ReadOnly = true;
            accNameColumn.Width = 200;
            dataGridView1.Columns.Add(accNameColumn);

            // 3. Add Setting Column (Checkbox)
            DataGridViewCheckBoxColumn settingColumn = new DataGridViewCheckBoxColumn();
            settingColumn.HeaderText = "Setting";
            settingColumn.Name = "Setting";
            settingColumn.Width = 60;
            dataGridView1.Columns.Add(settingColumn);

            // Set DataSource
            dataGridView1.DataSource = accountsDataTable;

            // Now load ExpFlag separately to set checkbox values
            LoadExpFlagValuesToCheckboxes();
        }

        // Load ExpFlag values separately to set checkboxes
        private void LoadExpFlagValuesToCheckboxes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get all AccountNos and their ExpFlag values
                    string query = "SELECT AccountNo, ExpFlag FROM ChartOfAccounts";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Create a dictionary to store ExpFlag values
                    Dictionary<string, bool> expFlags = new Dictionary<string, bool>();

                    while (reader.Read())
                    {
                        string accountNo = reader["AccountNo"].ToString();
                        bool expFlagValue = Convert.ToInt32(reader["ExpFlag"]) == 1;
                        expFlags[accountNo] = expFlagValue;
                    }

                    reader.Close();
                    conn.Close();

                    // Set checkbox values in DataGridView
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["AccountNo"].Value != null)
                        {
                            string accountNo = row.Cells["AccountNo"].Value.ToString();
                            if (expFlags.ContainsKey(accountNo))
                            {
                                row.Cells["Setting"].Value = expFlags[accountNo];
                            }
                            else
                            {
                                row.Cells["Setting"].Value = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading expense flags: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedItem != null)
            {
                string selectedType = accountcombobox.SelectedItem.ToString();
                if (selectedType == "All Types")
                {
                    LoadAccountsToDataGridView("All");
                }
                else
                {
                    LoadAccountsToDataGridView(selectedType);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle checkbox click in Setting column (column index 2)
            if (e.RowIndex >= 0 && e.ColumnIndex == 2) // Setting checkbox column
            {
                // Force the cell to repaint
                dataGridView1.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a list to store accounts to update
                List<Tuple<string, bool>> accountsToUpdate = new List<Tuple<string, bool>>();

                // Collect data from DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["AccountNo"].Value != null &&
                        row.Cells["Setting"].Value != null)
                    {
                        string accountNo = row.Cells["AccountNo"].Value.ToString();
                        bool isChecked = Convert.ToBoolean(row.Cells["Setting"].Value);

                        accountsToUpdate.Add(new Tuple<string, bool>(accountNo, isChecked));
                    }
                }

                // Update database
                UpdateExpFlagInDatabase(accountsToUpdate);

                MessageBox.Show("Expense settings saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving expense settings: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateExpFlagInDatabase(List<Tuple<string, bool>> accountsToUpdate)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Start transaction for data consistency
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (var account in accountsToUpdate)
                    {
                        string accountNo = account.Item1;
                        bool isChecked = account.Item2;
                        int expFlagValue = isChecked ? 1 : 0;

                        string updateQuery = @"
                            UPDATE ChartOfAccounts 
                            SET ExpFlag = @ExpFlag 
                            WHERE AccountNo = @AccountNo";

                        SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@ExpFlag", expFlagValue);
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);

                        cmd.ExecuteNonQuery();
                    }

                    // Commit transaction
                    transaction.Commit();

                    // Refresh data to show updated settings
                    if (accountcombobox.SelectedItem != null)
                    {
                        string selectedType = accountcombobox.SelectedItem.ToString();
                        LoadAccountsToDataGridView(selectedType == "All Types" ? "All" : selectedType);
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            // Clear all checkbox selections (set all checkboxes to false)
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Setting"].Value = false;
            }
        }

        // Optional: Select All button (check all checkboxes)
        private void selectAllbtn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Setting"].Value = true;
            }
        }

        // Optional: Deselect All button
        private void deselectAllbtn_Click(object sender, EventArgs e)
        {
            clearbtn_Click(sender, e);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}