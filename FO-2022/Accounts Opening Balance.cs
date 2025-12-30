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
//    public partial class Accounts_Opening_Balance : Form
//    {
//        public Accounts_Opening_Balance()
//        {
//            InitializeComponent();
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
//    public partial class Accounts_Opening_Balance : Form
//    {
//        private string connectionString;
//        private SqlConnection connection;

//        public Accounts_Opening_Balance()
//        {
//            InitializeComponent();
//            InitializeDatabaseConnection();
//            LoadAccountTypes();
//        }

//        private void InitializeDatabaseConnection()
//        {
//            try
//            {
//                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//                connection = new SqlConnection(connectionString);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error initializing database connection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadAccountTypes()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    string query = "SELECT DISTINCT AccountType FROM ChartOfAccounts ORDER BY AccountType";
//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        using (SqlDataReader reader = cmd.ExecuteReader())
//                        {
//                            accountcombobox.Items.Clear();
//                            accountcombobox.Items.Add("-- Select Account Type --");

//                            while (reader.Read())
//                            {
//                                accountcombobox.Items.Add(reader["AccountType"].ToString());
//                            }

//                            if (accountcombobox.Items.Count > 0)
//                            {
//                                accountcombobox.SelectedIndex = 0;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading account types: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (accountcombobox.SelectedIndex > 0)
//            {
//                string selectedAccountType = accountcombobox.SelectedItem.ToString();
//                LoadAccountsByType(selectedAccountType);
//            }
//            else
//            {
//                dataGridView1.DataSource = null;
//                dataGridView1.Rows.Clear();
//            }
//        }

//        private void LoadAccountsByType(string accountType)
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    string query = "SELECT AccountNo, AccountName, OpeningDebit, OpeningCredit " +
//                                   "FROM ChartOfAccounts " +
//                                   "WHERE AccountType = @AccountType " +
//                                   "ORDER BY AccountNo";

//                    using (SqlCommand cmd = new SqlCommand(query, conn))
//                    {
//                        cmd.Parameters.AddWithValue("@AccountType", accountType);

//                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
//                        {
//                            DataTable dataTable = new DataTable();
//                            adapter.Fill(dataTable);
//                            dataGridView1.DataSource = dataTable;

//                            // Configure DataGridView columns
//                            ConfigureDataGridView();
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading accounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void ConfigureDataGridView()
//        {
//            if (dataGridView1.Columns.Count > 0)
//            {
//                // Hide any extra columns (just in case)
//                for (int i = dataGridView1.Columns.Count - 1; i >= 4; i--)
//                {
//                    dataGridView1.Columns.RemoveAt(i);
//                }

//                // Set column headers
//                if (dataGridView1.Columns.Contains("AccountNo"))
//                    dataGridView1.Columns["AccountNo"].HeaderText = "Account No";

//                if (dataGridView1.Columns.Contains("AccountName"))
//                    dataGridView1.Columns["AccountName"].HeaderText = "Account Name";

//                if (dataGridView1.Columns.Contains("OpeningDebit"))
//                    dataGridView1.Columns["OpeningDebit"].HeaderText = "Opening Debit";

//                if (dataGridView1.Columns.Contains("OpeningCredit"))
//                    dataGridView1.Columns["OpeningCredit"].HeaderText = "Opening Credit";

//                // Format currency columns
//                if (dataGridView1.Columns.Contains("OpeningDebit"))
//                {
//                    dataGridView1.Columns["OpeningDebit"].DefaultCellStyle.Format = "N2";
//                    dataGridView1.Columns["OpeningDebit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//                }

//                if (dataGridView1.Columns.Contains("OpeningCredit"))
//                {
//                    dataGridView1.Columns["OpeningCredit"].DefaultCellStyle.Format = "N2";
//                    dataGridView1.Columns["OpeningCredit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//                }

//                // Auto-size columns
//                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.Rows.Count == 0)
//            {
//                MessageBox.Show("No data to save!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    // Clear existing data for this account type (optional)
//                    string deleteQuery = "DELETE FROM AccountsBalances WHERE AccountNo IN " +
//                                        "(SELECT AccountNo FROM ChartOfAccounts WHERE AccountType = @AccountType)";

//                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
//                    {
//                        deleteCmd.Parameters.AddWithValue("@AccountType", accountcombobox.SelectedItem.ToString());
//                        deleteCmd.ExecuteNonQuery();
//                    }

//                    // Insert new data
//                    string insertQuery = "INSERT INTO AccountsBalances (AccountNo, OpeningDebit, OpeningCredit, OpeningBal, OpeningBalType) " +
//                                         "VALUES (@AccountNo, @OpeningDebit, @OpeningCredit, " +
//                                         "CASE WHEN @OpeningDebit > @OpeningCredit THEN @OpeningDebit - @OpeningCredit ELSE @OpeningCredit - @OpeningDebit END, " +
//                                         "CASE WHEN @OpeningDebit > @OpeningCredit THEN 'Dr' ELSE 'Cr' END)";

//                    int rowsAffected = 0;

//                    foreach (DataGridViewRow row in dataGridView1.Rows)
//                    {
//                        if (!row.IsNewRow)
//                        {
//                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
//                            {
//                                insertCmd.Parameters.AddWithValue("@AccountNo", row.Cells["AccountNo"].Value.ToString());
//                                insertCmd.Parameters.AddWithValue("@OpeningDebit", Convert.ToDecimal(row.Cells["OpeningDebit"].Value ?? 0));
//                                insertCmd.Parameters.AddWithValue("@OpeningCredit", Convert.ToDecimal(row.Cells["OpeningCredit"].Value ?? 0));

//                                rowsAffected += insertCmd.ExecuteNonQuery();
//                            }
//                        }
//                    }

//                    MessageBox.Show($"{rowsAffected} records saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            // Clear all fields
//            accountcombobox.SelectedIndex = 0;
//            dataGridView1.DataSource = null;
//            dataGridView1.Rows.Clear();
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle cell click events if needed
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
    public partial class Accounts_Opening_Balance : Form
    {
        private string connectionString;

        public Accounts_Opening_Balance()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadAccountTypes();
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing database connection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                            accountcombobox.Items.Add("-- Select Account Type --");

                            while (reader.Read())
                            {
                                accountcombobox.Items.Add(reader["AccountType"].ToString());
                            }

                            if (accountcombobox.Items.Count > 0)
                            {
                                accountcombobox.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account types: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accountcombobox.SelectedIndex > 0)
            {
                string selectedAccountType = accountcombobox.SelectedItem.ToString();
                LoadAccountsByType(selectedAccountType);
            }
            else
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
            }
        }

        private void LoadAccountsByType(string accountType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT AccountNo, AccountName, OpeningDebit, OpeningCredit " +
                                   "FROM ChartOfAccounts " +
                                   "WHERE AccountType = @AccountType " +
                                   "ORDER BY AccountNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountType", accountType);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Create a DataTable manually
                            DataTable dt = new DataTable();
                            dt.Columns.Add("AccountNo", typeof(string));
                            dt.Columns.Add("AccountName", typeof(string));
                            dt.Columns.Add("OpeningDebit", typeof(decimal));
                            dt.Columns.Add("OpeningCredit", typeof(decimal));

                            int rowCount = 0;
                            while (reader.Read())
                            {
                                dt.Rows.Add(
                                    reader["AccountNo"].ToString(),
                                    reader["AccountName"].ToString(),
                                    reader["OpeningDebit"],
                                    reader["OpeningCredit"]
                                );
                                rowCount++;
                            }

                            // Bind to DataGridView
                            dataGridView1.DataSource = dt;

                            // Show message about loaded rows
                            if (rowCount > 0)
                            {
                                MessageBox.Show($"Loaded {rowCount} accounts for '{accountType}'",
                                              "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Configure grid AFTER data is loaded
                                ConfigureDataGridView();
                            }
                            else
                            {
                                MessageBox.Show($"No accounts found for type: {accountType}",
                                              "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            try
            {
                // Clear any existing columns first
                dataGridView1.Columns.Clear();

                // Add columns with proper names
                DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                col1.Name = "AccountNo";
                col1.HeaderText = "Account No";
                col1.DataPropertyName = "AccountNo";
                dataGridView1.Columns.Add(col1);

                DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
                col2.Name = "AccountName";
                col2.HeaderText = "Account Name";
                col2.DataPropertyName = "AccountName";
                dataGridView1.Columns.Add(col2);

                DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
                col3.Name = "OpeningDebit";
                col3.HeaderText = "Opening Debit";
                col3.DataPropertyName = "OpeningDebit";
                col3.DefaultCellStyle.Format = "N2";
                col3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns.Add(col3);

                DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
                col4.Name = "OpeningCredit";
                col4.HeaderText = "Opening Credit";
                col4.DataPropertyName = "OpeningCredit";
                col4.DefaultCellStyle.Format = "N2";
                col4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns.Add(col4);

                // Set properties
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.ReadOnly = true; // Set to false if you want to edit
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error configuring grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No data to save!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Clear existing data for this account type (optional)
                    string deleteQuery = "DELETE FROM AccountsBalances WHERE AccountNo IN " +
                                        "(SELECT AccountNo FROM ChartOfAccounts WHERE AccountType = @AccountType)";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@AccountType", accountcombobox.SelectedItem.ToString());
                        int deletedRows = deleteCmd.ExecuteNonQuery();
                        MessageBox.Show($"Cleared {deletedRows} old records.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Insert new data
                    string insertQuery = "INSERT INTO AccountsBalances (AccountNo, OpeningDebit, OpeningCredit, OpeningBal, OpeningBalType) " +
                                         "VALUES (@AccountNo, @OpeningDebit, @OpeningCredit, " +
                                         "CASE WHEN @OpeningDebit > @OpeningCredit THEN @OpeningDebit - @OpeningCredit ELSE @OpeningCredit - @OpeningDebit END, " +
                                         "CASE WHEN @OpeningDebit > @OpeningCredit THEN 'Dr' ELSE 'Cr' END)";

                    int rowsAffected = 0;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                string accountNo = row.Cells["AccountNo"].Value?.ToString();
                                decimal openingDebit = Convert.ToDecimal(row.Cells["OpeningDebit"].Value ?? 0);
                                decimal openingCredit = Convert.ToDecimal(row.Cells["OpeningCredit"].Value ?? 0);

                                insertCmd.Parameters.AddWithValue("@AccountNo", accountNo);
                                insertCmd.Parameters.AddWithValue("@OpeningDebit", openingDebit);
                                insertCmd.Parameters.AddWithValue("@OpeningCredit", openingCredit);

                                rowsAffected += insertCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show($"{rowsAffected} records saved successfully to AccountsBalances table!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            // Clear all fields
            accountcombobox.SelectedIndex = 0;
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell click events if needed
        }

        // Add this method to test with sample data
        private void btnTestData_Click(object sender, EventArgs e)
        {
            // Test if combobox has items
            if (accountcombobox.Items.Count > 1)
            {
                // Select "Salesman" if it exists
                for (int i = 0; i < accountcombobox.Items.Count; i++)
                {
                    if (accountcombobox.Items[i].ToString() == "Salesman")
                    {
                        accountcombobox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}