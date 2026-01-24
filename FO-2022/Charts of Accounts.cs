using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Charts_of_Accounts : Form
    {
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        public Charts_of_Accounts()
        {
            InitializeComponent();
        }

        private void Charts_of_Accounts_Load(object sender, EventArgs e)
        {
            LoadAllAccounts();
            LoadTreeViewFromDatabase();
        }

        // Load all accounts into DataGridView
        private void LoadAllAccounts()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName
                        FROM ChartOfAccounts 
                        ORDER BY AccountNo";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Bind to DataGridView
                    dataGridView1.DataSource = dt;

                    // Set column headers
                    if (dataGridView1.Columns.Count >= 2)
                    {
                        dataGridView1.Columns[0].HeaderText = "Account No";
                        dataGridView1.Columns[1].HeaderText = "Account Name";
                        dataGridView1.Columns[0].Width = 120;
                        dataGridView1.Columns[1].Width = 250;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load accounts into TreeView organized by account type
        private void LoadTreeViewFromDatabase()
        {
            try
            {
                treeView1.Nodes.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Define main account categories
                    TreeNode assetsNode = new TreeNode("Assets");
                    TreeNode liabilitiesNode = new TreeNode("Liabilities");
                    TreeNode equityNode = new TreeNode("Equity");
                    TreeNode revenueNode = new TreeNode("Revenue");
                    TreeNode expenseNode = new TreeNode("Expense");
                    TreeNode partiesNode = new TreeNode("Parties");

                    // Add subcategories under Assets
                    TreeNode currentAssetsNode = new TreeNode("Current Assets");
                    TreeNode fixedAssetsNode = new TreeNode("Fixed Assets");
                    TreeNode otherAssetsNode = new TreeNode("Other Assets");
                    TreeNode nonCurrentAssetsNode = new TreeNode("Non-Current Assets");

                    assetsNode.Nodes.Add(currentAssetsNode);
                    assetsNode.Nodes.Add(fixedAssetsNode);
                    assetsNode.Nodes.Add(otherAssetsNode);
                    assetsNode.Nodes.Add(nonCurrentAssetsNode);

                    // Load actual accounts from database and categorize them
                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName,
                            AccountType,
                            ParentAccountNo,
                            AccountDepth
                        FROM ChartOfAccounts 
                        WHERE IsDetailed = 1  -- Show only detailed accounts
                        ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string accountNo = reader["AccountNo"].ToString();
                        string accountName = reader["AccountName"].ToString();
                        string accountType = reader["AccountType"].ToString();
                        string parentAccount = reader["ParentAccountNo"]?.ToString();
                        int accountDepth = Convert.ToInt32(reader["AccountDepth"]);

                        TreeNode accountNode = new TreeNode($"{accountNo} - {accountName}");
                        accountNode.Tag = accountNo; // Store account number in Tag for reference

                        // Categorize based on account type or parent account
                        if (accountType.ToLower().Contains("asset") ||
                            (parentAccount != null && parentAccount.StartsWith("01")))
                        {
                            // Assets accounts typically start with 01
                            AddToAssetSubcategory(accountNode, accountNo, accountName);
                        }
                        else if (accountType.ToLower().Contains("liability") ||
                                 (parentAccount != null && parentAccount.StartsWith("02")))
                        {
                            // Liabilities accounts typically start with 02
                            liabilitiesNode.Nodes.Add(accountNode);
                        }
                        else if (accountType.ToLower().Contains("equity") ||
                                 (parentAccount != null && parentAccount.StartsWith("03")))
                        {
                            // Equity accounts typically start with 03
                            equityNode.Nodes.Add(accountNode);
                        }
                        else if (accountType.ToLower().Contains("revenue") ||
                                 accountType.ToLower().Contains("income") ||
                                 (parentAccount != null && parentAccount.StartsWith("04")))
                        {
                            // Revenue accounts typically start with 04
                            revenueNode.Nodes.Add(accountNode);
                        }
                        else if (accountType.ToLower().Contains("expense") ||
                                 (parentAccount != null && parentAccount.StartsWith("05")))
                        {
                            // Expense accounts typically start with 05
                            expenseNode.Nodes.Add(accountNode);
                        }
                        else if (accountType.ToLower().Contains("party") ||
                                 accountType.ToLower().Contains("customer") ||
                                 accountType.ToLower().Contains("vendor") ||
                                 accountType.ToLower().Contains("salesman"))
                        {
                            // Parties accounts
                            partiesNode.Nodes.Add(accountNode);
                        }
                        else
                        {
                            // Default to appropriate category based on account number prefix
                            AddBasedOnAccountPrefix(accountNode, accountNo);
                        }
                    }
                    reader.Close();

                    // Add all main nodes to TreeView
                    treeView1.Nodes.Add(assetsNode);
                    treeView1.Nodes.Add(liabilitiesNode);
                    treeView1.Nodes.Add(equityNode);
                    treeView1.Nodes.Add(revenueNode);
                    treeView1.Nodes.Add(expenseNode);
                    treeView1.Nodes.Add(partiesNode);

                    // Expand all nodes
                    treeView1.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tree view: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to add accounts to appropriate asset subcategory
        private void AddToAssetSubcategory(TreeNode accountNode, string accountNo, string accountName)
        {
            // Check account number or name to determine subcategory
            if (accountNo.StartsWith("01-01") || accountName.ToLower().Contains("cash") ||
                accountName.ToLower().Contains("bank") || accountName.ToLower().Contains("receivable"))
            {
                // Current Assets
                treeView1.Nodes[0].Nodes[0].Nodes.Add(accountNode);
            }
            else if (accountNo.StartsWith("01-02") || accountName.ToLower().Contains("property") ||
                     accountName.ToLower().Contains("plant") || accountName.ToLower().Contains("equipment"))
            {
                // Fixed Assets
                treeView1.Nodes[0].Nodes[1].Nodes.Add(accountNode);
            }
            else if (accountNo.StartsWith("01-03") || accountName.ToLower().Contains("investment") ||
                     accountName.ToLower().Contains("prepaid"))
            {
                // Other Assets
                treeView1.Nodes[0].Nodes[2].Nodes.Add(accountNode);
            }
            else if (accountNo.StartsWith("01-04") || accountName.ToLower().Contains("long term") ||
                     accountName.ToLower().Contains("non-current"))
            {
                // Non-Current Assets
                treeView1.Nodes[0].Nodes[3].Nodes.Add(accountNode);
            }
            else
            {
                // Default to Current Assets
                treeView1.Nodes[0].Nodes[0].Nodes.Add(accountNode);
            }
        }

        // Helper method to categorize based on account number prefix
        private void AddBasedOnAccountPrefix(TreeNode accountNode, string accountNo)
        {
            // Assets (01 series)
            if (accountNo.StartsWith("01"))
            {
                treeView1.Nodes[0].Nodes[0].Nodes.Add(accountNode); // Default to Current Assets
            }
            // Liabilities (02 series)
            else if (accountNo.StartsWith("02"))
            {
                treeView1.Nodes[1].Nodes.Add(accountNode);
            }
            // Equity (03 series)
            else if (accountNo.StartsWith("03"))
            {
                treeView1.Nodes[2].Nodes.Add(accountNode);
            }
            // Revenue (04 series)
            else if (accountNo.StartsWith("04"))
            {
                treeView1.Nodes[3].Nodes.Add(accountNode);
            }
            // Expenses (05 series)
            else if (accountNo.StartsWith("05"))
            {
                treeView1.Nodes[4].Nodes.Add(accountNode);
            }
            // Parties (typically 06 or specific ranges)
            else if (accountNo.StartsWith("06") || accountNo.StartsWith("07"))
            {
                treeView1.Nodes[5].Nodes.Add(accountNode);
            }
        }

        // When TreeView node is selected, filter DataGridView
        //private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        //{
           
        //}

        // Filter by main account type
        private void FilterByAccountType(string accountType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName
                        FROM ChartOfAccounts 
                        WHERE AccountType LIKE @SearchText + '%' 
                           OR AccountName LIKE '%' + @SearchText + '%'
                        ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SearchText", accountType);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Filter by subcategory
        private void FilterBySubcategory(string parentType, string subcategory)
        {
            // Map subcategories to account prefixes or types
            string searchPattern = "";

            if (parentType == "Assets")
            {
                if (subcategory == "Current Assets")
                    searchPattern = "01-01%";
                else if (subcategory == "Fixed Assets")
                    searchPattern = "01-02%";
                else if (subcategory == "Other Assets")
                    searchPattern = "01-03%";
                else if (subcategory == "Non-Current Assets")
                    searchPattern = "01-04%";
            }
            else if (parentType == "Liabilities")
            {
                searchPattern = "02%";
            }
            else if (parentType == "Equity")
            {
                searchPattern = "03%";
            }
            else if (parentType == "Revenue")
            {
                searchPattern = "04%";
            }
            else if (parentType == "Expense")
            {
                searchPattern = "05%";
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                FilterByAccountPattern(searchPattern);
            }
        }

        // Filter by account number pattern
        private void FilterByAccountPattern(string pattern)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName
                        FROM ChartOfAccounts 
                        WHERE AccountNo LIKE @Pattern
                        ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Pattern", pattern);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Filter by specific account number
        private void FilterByAccountNo(string accountNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName
                        FROM ChartOfAccounts 
                        WHERE AccountNo = @AccountNo
                        ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", accountNo);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Close button
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mf = new Mainform1();
            //mf.Show();
        }

        // Double click on DataGridView to show details
        //private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
          
        //}

        // Search functionality (if you add a search textbox)
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadAllAccounts();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            AccountNo,
                            AccountName
                        FROM ChartOfAccounts 
                        WHERE AccountNo LIKE '%' + @SearchText + '%' 
                           OR AccountName LIKE '%' + @SearchText + '%'
                        ORDER BY AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SearchText", searchText);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string accountNo = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string accountName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                MessageBox.Show($"Account No: {accountNo}\nAccount Name: {accountName}",
                    "Account Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void treeView1_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            try
            {
                string selectedNodeText = e.Node.Text;

                // If it's a main category node (Assets, Liabilities, etc.)
                if (e.Node.Level == 0)
                {
                    FilterByAccountType(selectedNodeText);
                }
                // If it's a subcategory node (Current Assets, Fixed Assets, etc.)
                else if (e.Node.Level == 1 && e.Node.Parent != null)
                {
                    FilterBySubcategory(e.Node.Parent.Text, selectedNodeText);
                }
                // If it's an actual account node
                else if (e.Node.Level >= 2 && e.Node.Tag != null)
                {
                    string accountNo = e.Node.Tag.ToString();
                    FilterByAccountNo(accountNo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering accounts: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}