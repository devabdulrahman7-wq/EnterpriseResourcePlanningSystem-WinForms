using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Product_Sub_Groups : Form
    {
        //private string connectionString = "Data Source=.;Initial Catalog=POS_1.2;Integrated Security=True; Connect Timeout=3";
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private int currentSubGroupId = 0;
        private bool isEditing = false;

        public Product_Sub_Groups()
        {
            InitializeComponent();
        }

        private void Product_Sub_Groups_Load(object sender, EventArgs e)
        {
            InitializeConnection();
            LoadCompanies();
            LoadDataGrid();
            ClearForm();
        }

        private void InitializeConnection()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message);
            }
        }

        private void LoadCompanies()
        {
            string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            cmpnynamecombobox1.Items.Clear();
            cmpnynamecombobox1.Items.Add(new ComboBoxItem("Select Company", "0"));

            while (reader.Read())
            {
                cmpnynamecombobox1.Items.Add(new ComboBoxItem(reader["CompanyName"].ToString(), reader["CompanyID"].ToString()));
            }
            reader.Close();
            cmpnynamecombobox1.SelectedIndex = 0;
        }

        private void LoadGroups(string companyId)
        {
            string query = "SELECT GroupId, GroupName FROM ProductGroups WHERE CompanyId = @CompanyId ORDER BY GroupName";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@CompanyId", companyId);
            SqlDataReader reader = cmd.ExecuteReader();

            grpnamecomboBox1.Items.Clear();
            grpnamecomboBox1.Items.Add(new ComboBoxItem("Select Group", "0"));

            while (reader.Read())
            {
                grpnamecomboBox1.Items.Add(new ComboBoxItem(reader["GroupName"].ToString(), reader["GroupId"].ToString()));
            }
            reader.Close();
            grpnamecomboBox1.SelectedIndex = 0;
        }

        private void LoadDataGrid()
        {
            string query = @"SELECT psg.CompanyId, c.CompanyName, psg.GroupID, pg.GroupName, 
                             psg.SubGroupId, psg.SubGroupName 
                             FROM ProductSubGroups psg
                             INNER JOIN Companies c ON psg.CompanyId = c.CompanyID
                             INNER JOIN ProductGroups pg ON psg.GroupID = pg.GroupId AND psg.CompanyId = pg.CompanyId
                             ORDER BY c.CompanyName, pg.GroupName, psg.SubGroupName";

            dataAdapter = new SqlDataAdapter(query, connection);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;
        }

        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmpnynamecombobox1.SelectedIndex > 0)
            {
                LoadGroups(((ComboBoxItem)cmpnynamecombobox1.SelectedItem).Value);
            }
            else
            {
                grpnamecomboBox1.Items.Clear();
                grpnamecomboBox1.Items.Add(new ComboBoxItem("Select Group", "0"));
                grpnamecomboBox1.SelectedIndex = 0;
            }

            EnableTextBox();
        }

        private void grpnamecomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableTextBox();
        }

        private void EnableTextBox()
        {
            //sbgrptxtbox1.Enabled = (cmpnynamecombobox1.SelectedIndex > 0 && grpnamecomboBox1.SelectedIndex > 0);
            sbgrptxtbox1.Enabled = true;  // Hamesha enabled rahe
        }


        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            isEditing = false;
            sbgrptxtbox1.Enabled = true;
            cmpnynamecombobox1.Focus();
        }

       
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (cmpnynamecombobox1.SelectedIndex <= 0 || grpnamecomboBox1.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(sbgrptxtbox1.Text))
            {
                MessageBox.Show("Please provide valid data.");
                return;
            }

            if (isEditing && currentSubGroupId > 0)
            {
                UpdateSubGroupSafe();
            }
            else
            {
                InsertSubGroupSafe();
            }

            LoadDataGrid();
            ClearForm();
        }

        private void InsertSubGroupSafe()
        {
            string companyId = ((ComboBoxItem)cmpnynamecombobox1.SelectedItem).Value;
            string groupId = ((ComboBoxItem)grpnamecomboBox1.SelectedItem).Value;
            string subGroupName = sbgrptxtbox1.Text.Trim();

            // Duplicate check
            string checkQuery = @"SELECT COUNT(*) FROM ProductSubGroups 
                          WHERE CompanyId=@CompanyId AND GroupId=@GroupId AND SubGroupName=@SubGroupName";
            using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@CompanyId", companyId);
                checkCmd.Parameters.AddWithValue("@GroupId", groupId);
                checkCmd.Parameters.AddWithValue("@SubGroupName", subGroupName);

                int exists = (int)checkCmd.ExecuteScalar();
                if (exists > 0)
                {
                    MessageBox.Show("This SubGroup already exists for selected Company and Group.");
                    return;
                }
            }

            // Proper SubGroupId generation inside transaction
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Generate new unique SubGroupId
                    string maxIdQuery = @"SELECT ISNULL(MAX(SubGroupId),0)+1 FROM ProductSubGroups 
                                  WHERE CompanyId=@CompanyId AND GroupId=@GroupId";
                    int newSubGroupId = 0;
                    using (SqlCommand maxIdCmd = new SqlCommand(maxIdQuery, connection, transaction))
                    {
                        maxIdCmd.Parameters.AddWithValue("@CompanyId", companyId);
                        maxIdCmd.Parameters.AddWithValue("@GroupId", groupId);
                        newSubGroupId = Convert.ToInt32(maxIdCmd.ExecuteScalar());
                    }

                    // Insert new record
                    string insertQuery = @"INSERT INTO ProductSubGroups 
                                   (CompanyId, GroupId, SubGroupId, SubGroupName) 
                                   VALUES (@CompanyId, @GroupId, @SubGroupId, @SubGroupName)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@CompanyId", companyId);
                        insertCmd.Parameters.AddWithValue("@GroupId", groupId);
                        insertCmd.Parameters.AddWithValue("@SubGroupId", newSubGroupId);
                        insertCmd.Parameters.AddWithValue("@SubGroupName", subGroupName);
                        insertCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("SubGroup added successfully!");
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error inserting SubGroup: " + ex.Message);
                }
            }
        }

        private void UpdateSubGroupSafe()
        {
            string companyId = ((ComboBoxItem)cmpnynamecombobox1.SelectedItem).Value;
            string groupId = ((ComboBoxItem)grpnamecomboBox1.SelectedItem).Value;
            string subGroupName = sbgrptxtbox1.Text.Trim();

            // Check duplicate name excluding current SubGroupId
            string checkQuery = @"SELECT COUNT(*) FROM ProductSubGroups 
                          WHERE CompanyId=@CompanyId AND GroupId=@GroupId AND SubGroupName=@SubGroupName
                          AND SubGroupId <> @SubGroupId";
            SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@CompanyId", companyId);
            checkCmd.Parameters.AddWithValue("@GroupId", groupId);
            checkCmd.Parameters.AddWithValue("@SubGroupName", subGroupName);
            checkCmd.Parameters.AddWithValue("@SubGroupId", currentSubGroupId);

            int exists = (int)checkCmd.ExecuteScalar();
            if (exists > 0)
            {
                MessageBox.Show("Another SubGroup with the same name exists for this Company and Group.");
                return;
            }

            string updateQuery = @"UPDATE ProductSubGroups
                           SET SubGroupName=@SubGroupName
                           WHERE CompanyId=@CompanyId AND GroupId=@GroupId AND SubGroupId=@SubGroupId";
            SqlCommand updateCmd = new SqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@CompanyId", companyId);
            updateCmd.Parameters.AddWithValue("@GroupId", groupId);
            updateCmd.Parameters.AddWithValue("@SubGroupId", currentSubGroupId);
            updateCmd.Parameters.AddWithValue("@SubGroupName", subGroupName);
            updateCmd.ExecuteNonQuery();

            MessageBox.Show("SubGroup updated successfully!");
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            cmpnynamecombobox1.SelectedIndex = 0;
            grpnamecomboBox1.Items.Clear();
            grpnamecomboBox1.Items.Add(new ComboBoxItem("Select Group", "0"));
            grpnamecomboBox1.SelectedIndex = 0;

            sbgrptxtbox1.Clear();
            //sbgrptxtbox1.Enabled = false;
            sbgrptxtbox1.Enabled = true;   // 👍 direct likhna allowed
            currentSubGroupId = 0;
            isEditing = false;
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() { return Text; }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                string companyId = row.Cells["CompanyId"].Value.ToString();
                string groupId = row.Cells["GroupID"].Value.ToString();

                // Company combobox select
                foreach (ComboBoxItem item in cmpnynamecombobox1.Items)
                {
                    if (item.Value == companyId)
                    {
                        cmpnynamecombobox1.SelectedItem = item;
                        break;
                    }
                }

                // Group load aur set
                LoadGroups(companyId);
                foreach (ComboBoxItem item in grpnamecomboBox1.Items)
                {
                    if (item.Value == groupId)
                    {
                        grpnamecomboBox1.SelectedItem = item;
                        break;
                    }
                }

                // Textbox fill
                sbgrptxtbox1.Text = row.Cells["SubGroupName"].Value.ToString();

                // Edit ke liye record ID set karo
                currentSubGroupId = Convert.ToInt32(row.Cells["SubGroupId"].Value);
                isEditing = true;

                sbgrptxtbox1.Enabled = true;
                savebtn.Enabled = true;
            }
        }

      

        private void removebtn_Click(object sender, EventArgs e)
        {
            // Ensure koi record select hua hai
            if (!isEditing || currentSubGroupId == 0)
            {
                MessageBox.Show("Please select a record from the grid to delete.");
                return;
            }

            // Confirmation dialog
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    string deleteQuery = "DELETE FROM ProductSubGroups WHERE SubGroupId = @Id";
                    SqlCommand cmd = new SqlCommand(deleteQuery, connection);
                    cmd.Parameters.AddWithValue("@Id", currentSubGroupId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record deleted successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Record not found or already deleted.");
                    }

                    // Refresh grid aur form clear karo
                    LoadDataGrid();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while deleting record: " + ex.Message);
                }
            }
            else
            {
                // User ne No ya Cancel click kiya
                MessageBox.Show("Delete operation cancelled.");
            }
        }

        private void prdctididsearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Base query
                string query = @"SELECT psg.CompanyId, c.CompanyName, psg.GroupID, pg.GroupName, 
                         psg.SubGroupId, psg.SubGroupName 
                         FROM ProductSubGroups psg
                         INNER JOIN Companies c ON psg.CompanyId = c.CompanyID
                         INNER JOIN ProductGroups pg ON psg.GroupID = pg.GroupId AND psg.CompanyId = pg.CompanyId
                         WHERE 1=1";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                // Filter by Company
                if (cmpnynamecombobox1.SelectedIndex > 0)
                {
                    string companyId = ((ComboBoxItem)cmpnynamecombobox1.SelectedItem).Value;
                    query += " AND psg.CompanyId = @CompanyId";
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                }

                // Filter by Group
                if (grpnamecomboBox1.SelectedIndex > 0)
                {
                    string groupId = ((ComboBoxItem)grpnamecomboBox1.SelectedItem).Value;
                    query += " AND psg.GroupID = @GroupId";
                    cmd.Parameters.AddWithValue("@GroupId", groupId);
                }

                // Filter by SubGroup Name (partial match)
                if (!string.IsNullOrWhiteSpace(sbgrptxtbox1.Text))
                {
                    query += " AND psg.SubGroupName LIKE @SubGroupName";
                    cmd.Parameters.AddWithValue("@SubGroupName", "%" + sbgrptxtbox1.Text.Trim() + "%");
                }

                // Order
                query += " ORDER BY c.CompanyName, pg.GroupName, psg.SubGroupName";

                // Execute and bind
                cmd.CommandText = query;
                DataTable searchTable = new DataTable();
                SqlDataAdapter searchAdapter = new SqlDataAdapter(cmd);
                searchAdapter.Fill(searchTable);

                dataGridView1.DataSource = searchTable;

                if (searchTable.Rows.Count == 0)
                {
                    MessageBox.Show("No records found matching your search criteria.");
                    ClearForm();
                    return;
                }

                // Agar sirf 1 record mila, automatically select aur edit mode me la do
                if (searchTable.Rows.Count == 1)
                {
                    DataGridViewRow row = dataGridView1.Rows[0];

                    string companyId = row.Cells["CompanyId"].Value.ToString();
                    string groupId = row.Cells["GroupID"].Value.ToString();

                    // Company combobox select
                    foreach (ComboBoxItem item in cmpnynamecombobox1.Items)
                    {
                        if (item.Value == companyId)
                        {
                            cmpnynamecombobox1.SelectedItem = item;
                            break;
                        }
                    }

                    // Group load aur set
                    LoadGroups(companyId);
                    foreach (ComboBoxItem item in grpnamecomboBox1.Items)
                    {
                        if (item.Value == groupId)
                        {
                            grpnamecomboBox1.SelectedItem = item;
                            break;
                        }
                    }

                    // Textbox fill
                    sbgrptxtbox1.Text = row.Cells["SubGroupName"].Value.ToString();

                    // Edit ke liye record ID set karo
                    currentSubGroupId = Convert.ToInt32(row.Cells["SubGroupId"].Value);
                    isEditing = true;

                    sbgrptxtbox1.Enabled = true;
                    savebtn.Enabled = true;
                }
                else
                {
                    // Multiple results → user manually select kare
                    currentSubGroupId = 0;
                    isEditing = false;
                    sbgrptxtbox1.Enabled = false;
                    savebtn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during search: " + ex.Message);
            }
        }

        private void editbtn_Click_1(object sender, EventArgs e)
        {
            // Check if koi record select hua hai
            if (!isEditing || currentSubGroupId == 0)
            {
                MessageBox.Show("Please select a record from the grid first!");
                return;
            }

            // Validate comboboxes aur textbox
            if (cmpnynamecombobox1.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a company.");
                cmpnynamecombobox1.Focus();
                return;
            }

            if (grpnamecomboBox1.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a group.");
                grpnamecomboBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(sbgrptxtbox1.Text))
            {
                MessageBox.Show("SubGroup Name cannot be empty.");
                sbgrptxtbox1.Focus();
                return;
            }

            // Confirm before updating
            DialogResult result = MessageBox.Show("Are you sure you want to update this record?", "Confirm Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UpdateSubGroupSafe();  // Database update
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
