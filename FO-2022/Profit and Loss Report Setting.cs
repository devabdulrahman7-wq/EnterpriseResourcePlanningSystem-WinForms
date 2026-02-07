////using System;
////using System.Collections.Generic;
////using System.ComponentModel;
////using System.Data;
////using System.Drawing;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using System.Windows.Forms;

////namespace FO_2022
////{
////    public partial class Profit_and_Loss_Report_Setting : Form
////    {
////        public Profit_and_Loss_Report_Setting()
////        {
////            InitializeComponent();
////        }

////        private void pictureBox2_Click(object sender, EventArgs e)
////        {
////            this.Close();
////        }

////        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
////        {

////        }

////        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {

////        }

////        private void savebtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void clearbtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void closebtn_Click(object sender, EventArgs e)
////        {

////        }
////    }
////}






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
//using System.IO;
//using System.Xml;

//namespace FO_2022
//{
//    public partial class Profit_and_Loss_Report_Setting : Form
//    {
//        private string connectionString;
//        private DataTable accountsData;
//        private List<string> selectedAccounts = new List<string>();
//        private string settingsFilePath;
//        private int currentSelectedRowIndex = -1;

//        public Profit_and_Loss_Report_Setting()
//        {
//            InitializeComponent();
//            InitializeSettingsPath();
//            LoadConnectionString();
//            InitializeDataGridView();
//            LoadSavedSelections();
//        }

//        private void InitializeSettingsPath()
//        {
//            string appPath = Application.StartupPath;
//            string settingsFolder = Path.Combine(appPath, "Settings");

//            if (!Directory.Exists(settingsFolder))
//            {
//                Directory.CreateDirectory(settingsFolder);
//            }

//            settingsFilePath = Path.Combine(settingsFolder, "ProfitLossSettings.xml");
//        }

//        private void LoadConnectionString()
//        {
//            try
//            {
//                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading connection string: {ex.Message}\nUsing default connection.",
//                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                connectionString = "Data Source=.;Initial Catalog=db_ac0035_pos;Integrated Security=True";
//            }
//        }

//        private void InitializeDataGridView()
//        {
//            dataGridView1.Columns.Clear();

//            dataGridView1.AutoGenerateColumns = false;
//            dataGridView1.AllowUserToAddRows = false;
//            dataGridView1.AllowUserToDeleteRows = false;
//            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//            dataGridView1.MultiSelect = false;
//            dataGridView1.ReadOnly = true;
//            dataGridView1.RowHeadersVisible = false;

//            // AccountNo column
//            DataGridViewTextBoxColumn colAccountNo = new DataGridViewTextBoxColumn();
//            colAccountNo.Name = "AccountNo";
//            colAccountNo.HeaderText = "Account No";
//            colAccountNo.DataPropertyName = "AccountNo";
//            colAccountNo.Width = 100;
//            colAccountNo.ReadOnly = true;
//            dataGridView1.Columns.Add(colAccountNo);

//            // AccountName column
//            DataGridViewTextBoxColumn colAccountName = new DataGridViewTextBoxColumn();
//            colAccountName.Name = "AccountName";
//            colAccountName.HeaderText = "Account Name";
//            colAccountName.DataPropertyName = "AccountName";
//            colAccountName.Width = 250;
//            colAccountName.ReadOnly = true;
//            dataGridView1.Columns.Add(colAccountName);

//            // Settings column - Initially empty, will show "+" only for selected row
//            DataGridViewButtonColumn colSettings = new DataGridViewButtonColumn();
//            colSettings.Name = "Settings";
//            colSettings.HeaderText = "Settings";
//            colSettings.Text = ""; // Empty initially
//            colSettings.UseColumnTextForButtonValue = true;
//            colSettings.Width = 70;
//            colSettings.FlatStyle = FlatStyle.Flat;
//            colSettings.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
//            dataGridView1.Columns.Add(colSettings);

//            // Hidden column for tracking if account is saved/selected
//            DataGridViewCheckBoxColumn colIsSaved = new DataGridViewCheckBoxColumn();
//            colIsSaved.Name = "IsSaved";
//            colIsSaved.HeaderText = "Is Saved";
//            colIsSaved.DataPropertyName = "IsSaved";
//            colIsSaved.Visible = false;
//            dataGridView1.Columns.Add(colIsSaved);
//        }

//        private void Profit_and_Loss_Report_Setting_Load(object sender, EventArgs e)
//        {
//            LoadAccountTypes();
//        }

//        private void LoadAccountTypes()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts WHERE AccountType IS NOT NULL AND AccountType <> '' ORDER BY AccountType";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    conn.Open();

//                    SqlDataReader reader = cmd.ExecuteReader();
//                    accountcombobox.Items.Clear();

//                    while (reader.Read())
//                    {
//                        accountcombobox.Items.Add(reader["AccountType"].ToString());
//                    }

//                    reader.Close();

//                    if (accountcombobox.Items.Count > 0)
//                    {
//                        accountcombobox.SelectedIndex = 0;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading account types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (accountcombobox.SelectedItem != null)
//            {
//                LoadAccountsByType(accountcombobox.SelectedItem.ToString());
//            }
//        }


//        private void LoadAccountsByType(string accountType)
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT AccountNo, AccountName FROM ChartOfAccounts WHERE AccountType = @AccountType ORDER BY AccountNo";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@AccountType", accountType);

//                    conn.Open();
//                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                    accountsData = new DataTable();
//                    adapter.Fill(accountsData);

//                    // Add IsSaved column to track saved accounts
//                    if (!accountsData.Columns.Contains("IsSaved"))
//                    {
//                        accountsData.Columns.Add("IsSaved", typeof(bool));
//                    }

//                    // Mark saved accounts
//                    foreach (DataRow row in accountsData.Rows)
//                    {
//                        string accountNo = row["AccountNo"].ToString();
//                        row["IsSaved"] = selectedAccounts.Contains(accountNo);
//                    }

//                    dataGridView1.DataSource = accountsData;

//                    // Refresh the display
//                    dataGridView1.Refresh();
//                    dataGridView1.Invalidate();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void ClearAllPlusSigns()
//        {
//            // Set all Settings cells to empty initially
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                if (row.Cells["Settings"] != null)
//                {
//                    row.Cells["Settings"].Value = "";
//                }
//            }
//        }


//        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
//        {
//            // First, update display for all rows based on saved status
//            UpdateAllRowsDisplay();

//            // Then handle the selected row
//            if (dataGridView1.SelectedRows.Count > 0)
//            {
//                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
//                currentSelectedRowIndex = selectedRow.Index;

//                // Only show "+" if the row is not already saved
//                if (selectedRow.Cells["Settings"] != null && selectedRow.Cells["IsSaved"] != null)
//                {
//                    bool isSaved = Convert.ToBoolean(selectedRow.Cells["IsSaved"].Value);
//                    if (!isSaved)
//                    {
//                        selectedRow.Cells["Settings"].Value = "+";
//                    }
//                }
//            }
//            else
//            {
//                currentSelectedRowIndex = -1;
//            }
//        }
//        private void UpdateAllRowsDisplay()
//        {
//            // Update all rows to show saved status
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                if (row.Cells["Settings"] != null && row.Cells["IsSaved"] != null)
//                {
//                    bool isSaved = Convert.ToBoolean(row.Cells["IsSaved"].Value);
//                    if (isSaved)
//                    {
//                        row.Cells["Settings"].Value = "✓";
//                        row.Cells["Settings"].Style.BackColor = Color.LightGreen;
//                    }
//                    else
//                    {
//                        // Clear the cell if not selected
//                        if (row.Index != currentSelectedRowIndex)
//                        {
//                            row.Cells["Settings"].Value = "";
//                        }
//                    }
//                }
//            }
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

//            // Handle Settings button click
//            if (dataGridView1.Columns[e.ColumnIndex].Name == "Settings")
//            {
//                DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
//                string accountNo = clickedRow.Cells["AccountNo"].Value?.ToString() ?? "";
//                string accountName = clickedRow.Cells["AccountName"].Value?.ToString() ?? "";

//                // Check if already saved
//                bool isSaved = selectedAccounts.Contains(accountNo);

//                if (isSaved)
//                {
//                    // Ask for confirmation to remove
//                    DialogResult result = MessageBox.Show($"Remove '{accountName}' ({accountNo}) from saved accounts?",
//                        "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//                    if (result == DialogResult.Yes)
//                    {
//                        selectedAccounts.Remove(accountNo);
//                        clickedRow.Cells["IsSaved"].Value = false;
//                        MessageBox.Show($"Account {accountNo} removed from saved list.",
//                            "Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    }
//                }
//                else
//                {
//                    // Add to saved accounts
//                    selectedAccounts.Add(accountNo);
//                    clickedRow.Cells["IsSaved"].Value = true;

//                    MessageBox.Show($"Account {accountNo} added to saved list.",
//                        "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }

//                // Refresh the display to show the updated state
//                dataGridView1.Refresh();
//                dataGridView1.Invalidate();
//            }
//        }


//        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
//        {
//            if (e.RowIndex < 0) return;

//            // Format the Settings column
//            if (dataGridView1.Columns[e.ColumnIndex].Name == "Settings")
//            {
//                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

//                if (row.Cells["IsSaved"] != null && Convert.ToBoolean(row.Cells["IsSaved"].Value))
//                {
//                    // This is a saved account - show checkmark
//                    e.Value = "✓";
//                    e.CellStyle.BackColor = Color.LightGreen;
//                    e.CellStyle.ForeColor = Color.Black;
//                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
//                }
//                else if (e.RowIndex == currentSelectedRowIndex)
//                {
//                    // This is the selected row (but not saved) - show plus
//                    e.Value = "+";
//                    e.CellStyle.BackColor = Color.White;
//                    e.CellStyle.ForeColor = Color.Black;
//                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
//                }
//                else
//                {
//                    // Not saved and not selected - empty
//                    e.Value = "";
//                    e.CellStyle.BackColor = Color.White;
//                    e.CellStyle.ForeColor = Color.White; // Hide empty text
//                }

//                // Mark as formatted so we don't re-format unnecessarily
//                e.FormattingApplied = true;
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                SaveSelectionsToXml();

//                MessageBox.Show($"Saved {selectedAccounts.Count} account(s) for Profit & Loss Report\nSettings saved to: {settingsFilePath}",
//                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void SaveSelectionsToXml()
//        {
//            try
//            {
//                XmlDocument xmlDoc = new XmlDocument();

//                XmlElement root = xmlDoc.CreateElement("ProfitLossSettings");
//                xmlDoc.AppendChild(root);

//                XmlElement timestamp = xmlDoc.CreateElement("LastSaved");
//                timestamp.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//                root.AppendChild(timestamp);

//                XmlElement accountType = xmlDoc.CreateElement("SelectedAccountType");
//                accountType.InnerText = accountcombobox.SelectedItem?.ToString() ?? "";
//                root.AppendChild(accountType);

//                XmlElement accountsNode = xmlDoc.CreateElement("SelectedAccounts");
//                root.AppendChild(accountsNode);

//                foreach (string accountNo in selectedAccounts)
//                {
//                    XmlElement accountNode = xmlDoc.CreateElement("Account");

//                    XmlElement accNo = xmlDoc.CreateElement("AccountNo");
//                    accNo.InnerText = accountNo;
//                    accountNode.AppendChild(accNo);

//                    accountsNode.AppendChild(accountNode);
//                }

//                xmlDoc.Save(settingsFilePath);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error saving XML: {ex.Message}");
//            }
//        }

//        private void LoadSavedSelections()
//        {
//            try
//            {
//                if (File.Exists(settingsFilePath))
//                {
//                    XmlDocument xmlDoc = new XmlDocument();
//                    xmlDoc.Load(settingsFilePath);

//                    selectedAccounts.Clear();

//                    XmlNodeList accountNodes = xmlDoc.SelectNodes("//SelectedAccounts/Account");
//                    foreach (XmlNode accountNode in accountNodes)
//                    {
//                        string accountNo = accountNode.SelectSingleNode("AccountNo")?.InnerText;
//                        if (!string.IsNullOrEmpty(accountNo))
//                        {
//                            selectedAccounts.Add(accountNo);
//                        }
//                    }

//                    string savedAccountType = xmlDoc.SelectSingleNode("//SelectedAccountType")?.InnerText;
//                    if (!string.IsNullOrEmpty(savedAccountType) && accountcombobox.Items.Contains(savedAccountType))
//                    {
//                        accountcombobox.SelectedItem = savedAccountType;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading saved settings: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            DialogResult result = MessageBox.Show("Clear all saved accounts?", "Confirm Clear",
//                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//            if (result == DialogResult.Yes)
//            {
//                ClearAllSelections();
//            }
//        }

//        private void ClearAllSelections()
//        {
//            selectedAccounts.Clear();
//            currentSelectedRowIndex = -1;

//            // Clear all "+" signs and saved status
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                if (row.Cells["Settings"] != null)
//                {
//                    row.Cells["Settings"].Value = "";
//                    row.Cells["IsSaved"].Value = false;
//                }
//            }

//            // Delete settings file
//            if (File.Exists(settingsFilePath))
//            {
//                try
//                {
//                    File.Delete(settingsFilePath);
//                }
//                catch { }
//            }

//            MessageBox.Show("All saved accounts cleared.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        // Utility method to show saved accounts summary
//        private void ShowSavedAccountsSummary()
//        {
//            if (selectedAccounts.Count > 0)
//            {
//                string summary = $"Saved Accounts ({selectedAccounts.Count}):\n\n";
//                foreach (string accNo in selectedAccounts)
//                {
//                    string accName = GetAccountNameFromGrid(accNo);
//                    summary += $"{accNo} - {accName}\n";
//                }

//                MessageBox.Show(summary, "Saved Accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            else
//            {
//                MessageBox.Show("No accounts saved yet.", "Saved Accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private string GetAccountNameFromGrid(string accountNo)
//        {
//            foreach (DataGridViewRow row in dataGridView1.Rows)
//            {
//                if (row.Cells["AccountNo"].Value?.ToString() == accountNo)
//                {
//                    return row.Cells["AccountName"].Value?.ToString() ?? "";
//                }
//            }
//            return "";
//        }

//        public List<string> GetSelectedAccounts()
//        {
//            return new List<string>(selectedAccounts);
//        }

//        public bool IsAccountSelected(string accountNo)
//        {
//            return selectedAccounts.Contains(accountNo);
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
using System.IO;
using System.Xml;

namespace FO_2022
{
    public partial class Profit_and_Loss_Report_Setting : Form
    {
        private string connectionString;
        private DataTable accountsData;
        private List<string> selectedAccounts = new List<string>();
        private string currentAccountType;
        private int currentSelectedRowIndex = -1;

        public Profit_and_Loss_Report_Setting()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeDataGridView();
            LoadAccountTypes();
        }

        private void InitializeConnection()
        {
            //try/
            //{
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error loading connection string: {ex.Message}\nUsing default connection.",
            //        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    connectionString = "Data Source=.;Initial Catalog=db_ac0035_pos;Integrated Security=True";
            //}
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;

            // AccountNo column
            DataGridViewTextBoxColumn colAccountNo = new DataGridViewTextBoxColumn();
            colAccountNo.Name = "AccountNo";
            colAccountNo.HeaderText = "Account No";
            colAccountNo.DataPropertyName = "AccountNo";
            colAccountNo.Width = 100;
            colAccountNo.ReadOnly = true;
            dataGridView1.Columns.Add(colAccountNo);

            // AccountName column
            DataGridViewTextBoxColumn colAccountName = new DataGridViewTextBoxColumn();
            colAccountName.Name = "AccountName";
            colAccountName.HeaderText = "Account Name";
            colAccountName.DataPropertyName = "AccountName";
            colAccountName.Width = 250;
            colAccountName.ReadOnly = true;
            dataGridView1.Columns.Add(colAccountName);

            // Settings column - Initially empty, will show "+" only for selected row
            DataGridViewButtonColumn colSettings = new DataGridViewButtonColumn();
            colSettings.Name = "Settings";
            colSettings.HeaderText = "Settings";
            colSettings.Text = ""; // Empty initially
            colSettings.UseColumnTextForButtonValue = true;
            colSettings.Width = 70;
            colSettings.FlatStyle = FlatStyle.Flat;
            colSettings.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns.Add(colSettings);

            // Hidden column for tracking if account is saved/selected
            DataGridViewCheckBoxColumn colIsSaved = new DataGridViewCheckBoxColumn();
            colIsSaved.Name = "IsSaved";
            colIsSaved.HeaderText = "Is Saved";
            colIsSaved.DataPropertyName = "IsSaved";
            colIsSaved.Visible = false;
            dataGridView1.Columns.Add(colIsSaved);
        }

        private void Profit_and_Loss_Report_Setting_Load(object sender, EventArgs e)
        {
            LoadAccountTypes();
        }

        private void LoadAccountTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts WHERE AccountType IS NOT NULL AND AccountType <> '' ORDER BY AccountType";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    accountcombobox.Items.Clear();

                    while (reader.Read())
                    {
                        accountcombobox.Items.Add(reader["AccountType"].ToString());
                    }

                    reader.Close();

                    if (accountcombobox.Items.Count > 0)
                    {
                        accountcombobox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading account types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedItem != null)
            {
                currentAccountType = accountcombobox.SelectedItem.ToString();
                LoadAccountsByType(currentAccountType);
            }
        }

        private void LoadAccountsByType(string accountType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT AccountNo, AccountName FROM ChartOfAccounts WHERE AccountType = @AccountType ORDER BY AccountNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountType", accountType);

                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    accountsData = new DataTable();
                    adapter.Fill(accountsData);

                    // Add IsSaved column to track saved accounts
                    if (!accountsData.Columns.Contains("IsSaved"))
                    {
                        accountsData.Columns.Add("IsSaved", typeof(bool));
                    }

                    // Load saved accounts from Registry table
                    selectedAccounts = LoadProfitLossSettings(accountType, conn);

                    // Mark saved accounts
                    foreach (DataRow row in accountsData.Rows)
                    {
                        string accountNo = row["AccountNo"].ToString();
                        row["IsSaved"] = selectedAccounts.Contains(accountNo);
                    }

                    dataGridView1.DataSource = accountsData;

                    // Refresh display
                    UpdateAllRowsDisplay();
                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> LoadProfitLossSettings(string accountType, SqlConnection conn)
        {
            List<string> savedAccounts = new List<string>();

            try
            {
                string query = @"SELECT RegistryKey, RegistryValue 
                        FROM Registry 
                        WHERE RegistryKey LIKE @Pattern AND RegistryValue = '1'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Pattern", $"PROFIT_LOSS_{accountType}_%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string key = reader["RegistryKey"].ToString();
                            // Extract account number from key
                            // Format: PROFIT_LOSS_[AccountType]_[AccountNo]
                            string[] parts = key.Split('_');
                            if (parts.Length >= 4)
                            {
                                string accountNo = parts[3];
                                savedAccounts.Add(accountNo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return savedAccounts;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // First, update display for all rows based on saved status
            UpdateAllRowsDisplay();

            // Then handle the selected row
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                currentSelectedRowIndex = selectedRow.Index;

                // Only show "+" if the row is not already saved
                if (selectedRow.Cells["Settings"] != null && selectedRow.Cells["IsSaved"] != null)
                {
                    bool isSaved = Convert.ToBoolean(selectedRow.Cells["IsSaved"].Value);
                    if (!isSaved)
                    {
                        selectedRow.Cells["Settings"].Value = "+";
                    }
                }
            }
            else
            {
                currentSelectedRowIndex = -1;
            }
        }

        private void UpdateAllRowsDisplay()
        {
            // Update all rows to show saved status
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Settings"] != null && row.Cells["IsSaved"] != null)
                {
                    bool isSaved = Convert.ToBoolean(row.Cells["IsSaved"].Value);
                    if (isSaved)
                    {
                        row.Cells["Settings"].Value = "✓";
                        row.Cells["Settings"].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        // Clear the cell if not selected
                        if (row.Index != currentSelectedRowIndex)
                        {
                            row.Cells["Settings"].Value = "";
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Handle Settings button click
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Settings")
            {
                DataGridViewRow clickedRow = dataGridView1.Rows[e.RowIndex];
                string accountNo = clickedRow.Cells["AccountNo"].Value?.ToString() ?? "";
                string accountName = clickedRow.Cells["AccountName"].Value?.ToString() ?? "";

                // Check if already saved
                bool isSaved = selectedAccounts.Contains(accountNo);

                if (isSaved)
                {
                    // Ask for confirmation to remove
                    DialogResult result = MessageBox.Show($"Remove '{accountName}' ({accountNo}) from saved accounts?",
                        "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        selectedAccounts.Remove(accountNo);
                        clickedRow.Cells["IsSaved"].Value = false;
                        MessageBox.Show($"Account {accountNo} removed from saved list.",
                            "Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Add to saved accounts
                    selectedAccounts.Add(accountNo);
                    clickedRow.Cells["IsSaved"].Value = true;

                    MessageBox.Show($"Account {accountNo} added to saved list.",
                        "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Refresh the display to show the updated state
                dataGridView1.Refresh();
                dataGridView1.Invalidate();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Format the Settings column
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Settings")
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["IsSaved"] != null && Convert.ToBoolean(row.Cells["IsSaved"].Value))
                {
                    // This is a saved account - show checkmark
                    e.Value = "✓";
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else if (e.RowIndex == currentSelectedRowIndex)
                {
                    // This is the selected row (but not saved) - show plus
                    e.Value = "+";
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else
                {
                    // Not saved and not selected - empty
                    e.Value = "";
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.White; // Hide empty text
                }

                // Mark as formatted so we don't re-format unnecessarily
                e.FormattingApplied = true;
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentAccountType))
            {
                MessageBox.Show("Please select an Account Type first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            deleteCmd.Parameters.AddWithValue("@Pattern", $"PROFIT_LOSS_{currentAccountType}_%");
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
                                string accountName = row.Cells["AccountName"].Value?.ToString() ?? "";

                                bool isSelected = false;
                                if (row.Cells["IsSaved"].Value != null)
                                {
                                    isSelected = Convert.ToBoolean(row.Cells["IsSaved"].Value);
                                }

                                string registryKey = $"PROFIT_LOSS_{currentAccountType}_{accountNo}";
                                string registryValue = isSelected ? "1" : "0";
                                string description = $"Profit & Loss Setting for {accountName}";

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
                        MessageBox.Show($"Saved {selectedAccounts.Count} account(s) for Profit & Loss Report!\nSettings saved to Registry table.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to database: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentAccountType))
            {
                MessageBox.Show("Please select an Account Type first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Clear all saved accounts for this account type?", "Confirm Clear",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearAllSelections();
            }
        }

        private void ClearAllSelections()
        {
            selectedAccounts.Clear();
            currentSelectedRowIndex = -1;

            // Clear all saved status
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Settings"] != null)
                {
                    row.Cells["Settings"].Value = "";
                    row.Cells["IsSaved"].Value = false;
                }
            }

            // Also delete from database if save button was clicked before
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string deleteQuery = @"DELETE FROM Registry 
                                  WHERE RegistryKey LIKE @Pattern";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@Pattern", $"PROFIT_LOSS_{currentAccountType}_%");
                        deleteCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing from database: {ex.Message}", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dataGridView1.Refresh();
            MessageBox.Show("All saved accounts cleared.", "Cleared",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Utility method to show saved accounts summary
        private void ShowSavedAccountsSummary()
        {
            if (selectedAccounts.Count > 0)
            {
                string summary = $"Saved Accounts ({selectedAccounts.Count}):\n\n";
                foreach (string accNo in selectedAccounts)
                {
                    string accName = GetAccountNameFromGrid(accNo);
                    summary += $"{accNo} - {accName}\n";
                }

                MessageBox.Show(summary, "Saved Accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No accounts saved yet.", "Saved Accounts",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetAccountNameFromGrid(string accountNo)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["AccountNo"].Value?.ToString() == accountNo)
                {
                    return row.Cells["AccountName"].Value?.ToString() ?? "";
                }
            }
            return "";
        }

        public List<string> GetSelectedAccounts()
        {
            return new List<string>(selectedAccounts);
        }

        public bool IsAccountSelected(string accountNo)
        {
            return selectedAccounts.Contains(accountNo);
        }
    }
}