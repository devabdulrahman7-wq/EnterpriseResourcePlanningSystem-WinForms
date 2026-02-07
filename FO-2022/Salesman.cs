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
    public partial class Salesman : Form
    {
        // Database connection string
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Variables to track current state
        private bool isEditMode = false;
        private string currentSalesmanId = "";
        private DataTable salesmanData = new DataTable();

        public Salesman()
        {
            InitializeComponent();
            LoadSalesmanData();
        }

        // Load all salesmen data into grid
        private void LoadSalesmanData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to get salesman data from ChartOfAccounts and Salesmen tables
                    string query = @"
                        SELECT 
                            s.SalesmanID,
                            ca.AccountName as SalesmanName,
                            s.Address,
                            s.City,
                            s.Phone,
                            s.Mobile,
                            s.NIC,
                            s.CommissionRatio,
                            s.IsStockHolder
                        FROM Salesmen s
                        INNER JOIN ChartOfAccounts ca ON s.SalesmanID = ca.AccountNo
                        ORDER BY s.SalesmanID";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    salesmanData.Clear();
                    adapter.Fill(salesmanData);

                    // Display in DataGridView
                    dataGridView1.DataSource = salesmanData;

                    // Configure DataGridView columns
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["SalesmanID"].HeaderText = "Salesman ID";
                        dataGridView1.Columns["SalesmanName"].HeaderText = "Name";
                        dataGridView1.Columns["CommissionRatio"].HeaderText = "Commission %";
                        dataGridView1.Columns["IsStockHolder"].HeaderText = "Stock Holder";

                        // Hide unnecessary columns
                        dataGridView1.Columns["Address"].Visible = false;
                        dataGridView1.Columns["City"].Visible = false;
                        dataGridView1.Columns["Phone"].Visible = false;
                        dataGridView1.Columns["Mobile"].Visible = false;
                        dataGridView1.Columns["NIC"].Visible = false;

                        // Set column widths
                        dataGridView1.Columns["SalesmanID"].Width = 120;
                        dataGridView1.Columns["SalesmanName"].Width = 150;
                        dataGridView1.Columns["CommissionRatio"].Width = 100;
                        dataGridView1.Columns["IsStockHolder"].Width = 80;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salesman data: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Search salesmen by name
        private void nametxtbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nametxtbox.Text))
                {
                    dataGridView1.DataSource = salesmanData;
                }
                else
                {
                    DataView dv = salesmanData.DefaultView;
                    dv.RowFilter = $"SalesmanName LIKE '%{nametxtbox.Text}%'";
                    dataGridView1.DataSource = dv.ToTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message, "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Save button click
        private void savebtn_Click(object sender, EventArgs e)
        {
            // Validate input
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    if (isEditMode)
                    {
                        // Update existing salesman
                        UpdateSalesman(conn);
                    }
                    else
                    {
                        // Insert new salesman
                        InsertSalesman(conn);
                    }

                    // Refresh data
                    LoadSalesmanData();
                    ClearForm();

                    MessageBox.Show("Salesman saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving salesman: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Insert new salesman
        private void InsertSalesman(SqlConnection conn)
        {
            try
            {
                // 1. Get prefix and number
                string prefix = partyidtxtbox.Text.Trim(); // e.g., "03"
                string number = partyidtxtbox1.Text.Trim().PadLeft(2, '0'); // e.g., "01"

                // 2. Convert to INT: "03" + "01" = "0301" → 301
                string combined = prefix + number; // "0301"
                if (!int.TryParse(combined, out int salesmanId))
                {
                    MessageBox.Show("Invalid Salesman ID. Must be numeric.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Parent account as INT: "03-01-01" = 30101
                int parentAccountNo = 30101;

                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 4. Insert into ChartOfAccounts (INT)
                    string chartQuery = @"
                INSERT INTO ChartOfAccounts 
                (AccountNo, UserNo, AccountName, AccountType, AccountDepth, 
                 ParentAccountNo, OpeningDebit, OpeningCredit, AdjustedDebit, AdjustedCredit,
                 IsDetailed, IsLocked, IsPosted, IsEditable, BalFlag, PLFlag, ExpFlag)
                VALUES (@AccountNo, 1, @AccountName, 'Salesman', 3, 
                        @ParentAccountNo, 0, 0, 0, 0, 1, 0, 0, 1, NULL, 0, 0)";

                    SqlCommand chartCmd = new SqlCommand(chartQuery, conn, transaction);
                    chartCmd.Parameters.AddWithValue("@AccountNo", salesmanId); // INTEGER
                    chartCmd.Parameters.AddWithValue("@AccountName", nametxtbox1.Text.Trim());
                    chartCmd.Parameters.AddWithValue("@ParentAccountNo", parentAccountNo); // INTEGER
                    chartCmd.ExecuteNonQuery();

                    // 5. Insert into Salesmen (INT - SAME VALUE, NOT STRING!)
                    string salesmanQuery = @"
                INSERT INTO Salesmen 
                (SalesmanID, SalesmanName, Address, City, Phone, Mobile, NIC, 
                 CommissionRatio, OfficeID, IsStockHolder)
                VALUES (@SalesmanID, @SalesmanName, @Address, @City, @Phone, @Mobile, @NIC,
                        @CommissionRatio, 1, @IsStockHolder)";

                    SqlCommand salesmanCmd = new SqlCommand(salesmanQuery, conn, transaction);
                    salesmanCmd.Parameters.AddWithValue("@SalesmanID", salesmanId); // INTEGER, NOT STRING!
                    salesmanCmd.Parameters.AddWithValue("@SalesmanName", nametxtbox1.Text.Trim());
                    salesmanCmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
                    salesmanCmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
                    salesmanCmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
                    salesmanCmd.Parameters.AddWithValue("@Mobile", mblnotxtbox.Text.Trim());
                    salesmanCmd.Parameters.AddWithValue("@NIC", nicnotxtbox.Text.Trim());

                    // Handle commission ratio
                    decimal commission = 0;
                    if (decimal.TryParse(commisiontxtbox.Text.Trim(), out commission))
                        salesmanCmd.Parameters.AddWithValue("@CommissionRatio", commission);
                    else
                        salesmanCmd.Parameters.AddWithValue("@CommissionRatio", 0);

                    salesmanCmd.Parameters.AddWithValue("@IsStockHolder", isstockholderchkbox.Checked);
                    salesmanCmd.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show($"Salesman created successfully!\nID: {salesmanId} (Display: {combined})",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Show detailed error
                    MessageBox.Show($"INSERT ERROR:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Update existing salesman
        private void UpdateSalesman(SqlConnection conn)
        {
            // Begin transaction
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // Update ChartOfAccounts
                string chartQuery = @"
                    UPDATE ChartOfAccounts 
                    SET AccountName = @AccountName
                    WHERE AccountNo = @AccountNo";

                SqlCommand chartCmd = new SqlCommand(chartQuery, conn, transaction);
                chartCmd.Parameters.AddWithValue("@AccountNo", currentSalesmanId);
                chartCmd.Parameters.AddWithValue("@AccountName", nametxtbox1.Text.Trim());
                chartCmd.ExecuteNonQuery();

                // Update Salesmen
                string salesmanQuery = @"
                    UPDATE Salesmen 
                    SET SalesmanName = @SalesmanName,
                        Address = @Address,
                        City = @City,
                        Phone = @Phone,
                        Mobile = @Mobile,
                        NIC = @NIC,
                        CommissionRatio = @CommissionRatio,
                        IsStockHolder = @IsStockHolder
                    WHERE SalesmanID = @SalesmanID";

                SqlCommand salesmanCmd = new SqlCommand(salesmanQuery, conn, transaction);
                salesmanCmd.Parameters.AddWithValue("@SalesmanID", currentSalesmanId);
                salesmanCmd.Parameters.AddWithValue("@SalesmanName", nametxtbox1.Text.Trim());
                salesmanCmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
                salesmanCmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
                salesmanCmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
                salesmanCmd.Parameters.AddWithValue("@Mobile", mblnotxtbox.Text.Trim());
                salesmanCmd.Parameters.AddWithValue("@NIC", nicnotxtbox.Text.Trim());

                // Handle commission ratio
                decimal commission = 0;
                if (decimal.TryParse(commisiontxtbox.Text.Trim(), out commission))
                    salesmanCmd.Parameters.AddWithValue("@CommissionRatio", commission);
                else
                    salesmanCmd.Parameters.AddWithValue("@CommissionRatio", 0);

                salesmanCmd.Parameters.AddWithValue("@IsStockHolder", isstockholderchkbox.Checked);

                salesmanCmd.ExecuteNonQuery();

                // Commit transaction
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Edit button click
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a salesman to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                currentSalesmanId = selectedRow.Cells["SalesmanID"].Value.ToString();

                // Load data into form
                partyidtxtbox.Text = currentSalesmanId.Substring(0, Math.Min(currentSalesmanId.Length, 2));
                partyidtxtbox1.Text = currentSalesmanId.Length > 2 ? currentSalesmanId.Substring(2) : "";
                nametxtbox1.Text = selectedRow.Cells["SalesmanName"].Value.ToString();

                // Get complete salesman details
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Address, City, Phone, Mobile, NIC, 
                               CommissionRatio, IsStockHolder
                        FROM Salesmen 
                        WHERE SalesmanID = @SalesmanID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SalesmanID", currentSalesmanId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        addresstxtbox.Text = reader["Address"].ToString();
                        citytxtbox.Text = reader["City"].ToString();
                        phonenotxtbox.Text = reader["Phone"].ToString();
                        mblnotxtbox.Text = reader["Mobile"].ToString();
                        nicnotxtbox.Text = reader["NIC"].ToString();
                        commisiontxtbox.Text = reader["CommissionRatio"].ToString();
                        isstockholderchkbox.Checked = Convert.ToBoolean(reader["IsStockHolder"]);
                    }
                    reader.Close();
                }

                // Set edit mode
                isEditMode = true;
                savebtn.Text = "Update";
                partyidtxtbox.Enabled = false;
                partyidtxtbox1.Enabled = false;

                MessageBox.Show("Salesman loaded for editing. Make changes and click Update.", "Edit Mode",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salesman for edit: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Remove button click
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a salesman to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this salesman? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string salesmanId = selectedRow.Cells["SalesmanID"].Value.ToString();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Check if salesman has any transactions
                        string checkQuery = @"
                            SELECT COUNT(*) FROM Sales WHERE SalesmanId = @SalesmanID
                            UNION ALL
                            SELECT COUNT(*) FROM Recovery WHERE SalesmanID = @SalesmanID";

                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

                        int transactionCount = 0;
                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactionCount += Convert.ToInt32(reader[0]);
                            }
                        }

                        if (transactionCount > 0)
                        {
                            MessageBox.Show("Cannot delete salesman. There are transactions linked to this salesman.",
                                "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Begin transaction
                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            // Delete from Salesmen first
                            string deleteSalesmanQuery = "DELETE FROM Salesmen WHERE SalesmanID = @SalesmanID";
                            SqlCommand deleteSalesmanCmd = new SqlCommand(deleteSalesmanQuery, conn, transaction);
                            deleteSalesmanCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                            deleteSalesmanCmd.ExecuteNonQuery();

                            // Delete from ChartOfAccounts
                            string deleteChartQuery = "DELETE FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                            SqlCommand deleteChartCmd = new SqlCommand(deleteChartQuery, conn, transaction);
                            deleteChartCmd.Parameters.AddWithValue("@AccountNo", salesmanId);
                            deleteChartCmd.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("Salesman deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh data
                            LoadSalesmanData();
                            ClearForm();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting salesman: " + ex.Message, "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // New button click
        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            isEditMode = false;
            savebtn.Text = "Save";
            partyidtxtbox.Enabled = true;
            partyidtxtbox1.Enabled = true;

            // Auto-generate next ID
            AutoGenerateSalesmanId();
        }

        // Auto-generate salesman ID
        private void AutoGenerateSalesmanId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get max INT value (301, 302, etc.)
                    string query = "SELECT MAX(SalesmanID) FROM Salesmen WHERE SalesmanID BETWEEN 300 AND 399";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    int nextId = 301; // Start from 301
                    if (result != null && result != DBNull.Value)
                    {
                        int maxId = Convert.ToInt32(result);
                        if (maxId >= 300)
                        {
                            nextId = maxId + 1;
                        }
                    }

                    // Convert INT to display format: 301 → "03" + "01"
                    string idStr = nextId.ToString().PadLeft(4, '0');
                    partyidtxtbox.Text = idStr.Substring(0, 2); // "03"
                    partyidtxtbox1.Text = idStr.Substring(2);    // "01"
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating ID: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                partyidtxtbox.Text = "03";
                partyidtxtbox1.Text = "01";
            }
        }
        // Clear button click
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // Clear all form fields
        private void ClearForm()
        {
            partyidtxtbox.Clear();
            partyidtxtbox1.Clear();
            nametxtbox1.Clear();
            addresstxtbox.Clear();
            citytxtbox.Clear();
            phonenotxtbox.Clear();
            mblnotxtbox.Clear();
            nicnotxtbox.Clear();
            commisiontxtbox.Clear();
            isstockholderchkbox.Checked = false;

            isEditMode = false;
            currentSalesmanId = "";
            savebtn.Text = "Save";
            partyidtxtbox.Enabled = true;
            partyidtxtbox1.Enabled = true;
        }

        // Close button click
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
           
        }

        // Exit icon click
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }

        // Validate form input
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || string.IsNullOrWhiteSpace(partyidtxtbox1.Text))
            {
                MessageBox.Show("Please enter a valid Salesman ID.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(nametxtbox1.Text))
            {
                MessageBox.Show("Please enter Salesman Name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(addresstxtbox.Text))
            {
                MessageBox.Show("Please enter Address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(citytxtbox.Text))
            {
                MessageBox.Show("Please enter City.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate commission ratio
            if (!string.IsNullOrWhiteSpace(commisiontxtbox.Text))
            {
                decimal commission;
                if (!decimal.TryParse(commisiontxtbox.Text, out commission) || commission < 0 || commission > 100)
                {
                    MessageBox.Show("Commission must be a number between 0 and 100.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        // DataGridView cell click - load data for editing
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Optional: You can auto-load data when row is clicked
                // Uncomment if you want single-click to load for editing
                // editbtn_Click(sender, e);
            }
        }

        // Form load
        private void Salesman_Load(object sender, EventArgs e)
        {
            // Add cell click event if not already added in designer
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void nametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Important: Enter key ka beep sound rokne ke liye

                try
                {
                    string searchText = nametxtbox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(searchText))
                    {
                        // Agar search empty hai to original data show karo
                        dataGridView1.DataSource = salesmanData;
                    }
                    else
                    {
                        // Filter lagao
                        DataView dv = salesmanData.DefaultView;
                        dv.RowFilter = $"SalesmanName LIKE '%{searchText}%'";

                        // Filtered data grid mein dalo
                        dataGridView1.DataSource = dv.ToTable();

                        // Optional: First row select karo
                        if (dataGridView1.Rows.Count > 0)
                        {
                            dataGridView1.Rows[0].Selected = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Agar Enter nahi hai to normal key chalo
                // Isse aap real-time search bhi rakh sakte ho
                // Ya phir sirf Enter key se search kar sakte ho
            }
        }

        private void partyidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void nametxtbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void addresstxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void citytxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void mblnotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void phonenotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void nicnotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void commisiontxtbox_KeyDown(object sender, KeyEventArgs e)
        {
              if (e.KeyCode == Keys.Enter)
    {
        e.SuppressKeyPress = true;
    }
        }
    }
}