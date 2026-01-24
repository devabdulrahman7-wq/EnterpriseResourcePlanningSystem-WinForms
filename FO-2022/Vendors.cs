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

namespace FO_2022
{
    public partial class Vendors : Form
    {
        // Database connection string
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        // Variables to track current state
        private bool isEditMode = false;
        private string currentVendorId = "";
        private DataTable vendorsData = new DataTable();

        public Vendors()
        {
            InitializeComponent();
            LoadVendorCategories();
            LoadVendorsData();
        }

        // Load all vendors data into grid
        private void LoadVendorsData(string searchText = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (string.IsNullOrEmpty(searchText))
                    {
                        query = @"
                            SELECT 
                                p.PartyID,
                                p.PartyName,
                                c.CategoryName,
                                p.Address,
                                p.City,
                                p.Phone1,
                                p.Mobile
                            FROM Parties p
                            LEFT JOIN Category c ON p.CategoryId = c.CategoryId
                            WHERE p.PartyType = 'V'
                            ORDER BY p.PartyID";
                        cmd = new SqlCommand(query, conn);
                    }
                    else
                    {
                        query = @"
                            SELECT 
                                p.PartyID,
                                p.PartyName,
                                c.CategoryName,
                                p.Address,
                                p.City,
                                p.Phone1,
                                p.Mobile
                            FROM Parties p
                            LEFT JOIN Category c ON p.CategoryId = c.CategoryId
                            WHERE p.PartyType = 'V' 
                            AND (p.PartyID LIKE @search OR p.PartyName LIKE @search)
                            ORDER BY p.PartyID";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    vendorsData.Clear();
                    adapter.Fill(vendorsData);

                    // Display in DataGridView
                    dataGridView1.DataSource = vendorsData;

                    // Configure DataGridView columns
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["PartyID"].HeaderText = "Vendors ID";
                        dataGridView1.Columns["PartyName"].HeaderText = "Name";
                        dataGridView1.Columns["CategoryName"].HeaderText = "Category";
                        dataGridView1.Columns["Address"].Visible = false;
                        dataGridView1.Columns["City"].Visible = false;
                        dataGridView1.Columns["Phone1"].Visible = false;
                        dataGridView1.Columns["Mobile"].Visible = false;

                        // Set column widths
                        dataGridView1.Columns["PartyID"].Width = 100;
                        dataGridView1.Columns["PartyName"].Width = 180;
                        dataGridView1.Columns["CategoryName"].Width = 120;
                    }

                    // Update count in textbox
                    textBox1.Text = $"                Available Vendors ({vendorsData.Rows.Count})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendors data: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load vendor categories into combobox
        private void LoadVendorCategories()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if Category table exists
                    string checkQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Category'";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (tableExists > 0)
                    {
                        string query = "SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryName";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        SqlDataReader reader = cmd.ExecuteReader();

                        vendorctgrycombobox.Items.Clear();
                        vendorctgrycombobox.Items.Add("Select Category");

                        while (reader.Read())
                        {
                            vendorctgrycombobox.Items.Add(reader["CategoryName"].ToString());
                        }
                        reader.Close();
                    }
                    else
                    {
                        // If Category table doesn't exist, add default options
                        vendorctgrycombobox.Items.Clear();
                        vendorctgrycombobox.Items.Add("Select Category");
                        vendorctgrycombobox.Items.Add("Supplier");
                        vendorctgrycombobox.Items.Add("Distributor");
                        vendorctgrycombobox.Items.Add("Manufacturer");
                    }

                    vendorctgrycombobox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Fallback options
                vendorctgrycombobox.Items.Clear();
                vendorctgrycombobox.Items.Add("Select Category");
                vendorctgrycombobox.Items.Add("Supplier");
                vendorctgrycombobox.SelectedIndex = 0;
            }
        }

        // Search vendors by name when Enter key is pressed
        private void nametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Stop beep sound
                LoadVendorsData(nametxtbox.Text.Trim());
            }
        }

        // Save/Update vendor
        private void savebtn_Click(object sender, EventArgs e)
        {
        }

        // Insert new vendor
        private void InsertVendor(SqlConnection conn, string vendorId, string categoryId)
        {
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Check if vendor ID already exists
                string checkQuery = "SELECT COUNT(*) FROM Parties WHERE PartyID = @partyId";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction);
                checkCmd.Parameters.AddWithValue("@partyId", vendorId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    transaction.Rollback();
                    MessageBox.Show("Vendor ID already exists!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Insert into Parties table
                string insertQuery = @"
                    INSERT INTO Parties 
                    (PartyID, PartyName, Address, City, Phone1, Phone2, Mobile, 
                     Fax, EMail, ContactPerson, PartyType, CategoryId, CreditLimit, 
                     CreditDays, IsRetailPrice, IsBlock, IsDistributor, IsMultiDistributor)
                    VALUES 
                    (@partyId, @partyName, @address, @city, @phone1, @phone2, @mobile, 
                     @fax, @email, @contactPerson, 'V', @categoryId, 0, 0, 0, 0, NULL, 0)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction);
                insertCmd.Parameters.AddWithValue("@partyId", vendorId);
                insertCmd.Parameters.AddWithValue("@partyName", nametxtbox1.Text.Trim());
                insertCmd.Parameters.AddWithValue("@address", addresstxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@city", citytxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@phone1", phonetxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@phone2", phone2txtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@mobile", mblnotxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@fax", faxtxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@email", emailtxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@contactPerson", conatactpersontxtbox.Text.Trim());
                insertCmd.Parameters.AddWithValue("@categoryId", categoryId);

                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Update existing vendor
        private void UpdateVendor(SqlConnection conn, string vendorId, string categoryId)
        {
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // Update Parties table
                string updateQuery = @"
                    UPDATE Parties SET 
                    PartyName = @partyName,
                    Address = @address,
                    City = @city,
                    Phone1 = @phone1,
                    Phone2 = @phone2,
                    Mobile = @mobile,
                    Fax = @fax,
                    EMail = @email,
                    ContactPerson = @contactPerson,
                    CategoryId = @categoryId
                    WHERE PartyID = @partyId";

                SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction);
                updateCmd.Parameters.AddWithValue("@partyId", vendorId);
                updateCmd.Parameters.AddWithValue("@partyName", nametxtbox1.Text.Trim());
                updateCmd.Parameters.AddWithValue("@address", addresstxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@city", citytxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@phone1", phonetxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@phone2", phone2txtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@mobile", mblnotxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@fax", faxtxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@email", emailtxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@contactPerson", conatactpersontxtbox.Text.Trim());
                updateCmd.Parameters.AddWithValue("@categoryId", categoryId);

                updateCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Delete vendor
        private void removebtn_Click(object sender, EventArgs e)
        {
            // If no vendor is selected in the form, try to get it from selected grid row
            if (string.IsNullOrEmpty(currentVendorId))
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    currentVendorId = dataGridView1.SelectedRows[0].Cells["PartyID"].Value.ToString();
                }
            }

            if (string.IsNullOrEmpty(currentVendorId))
            {
                MessageBox.Show("Please select a vendor to delete!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Are you sure you want to delete vendor {currentVendorId}?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            // Delete from Parties
                            string deleteQuery = "DELETE FROM Parties WHERE PartyID = @partyId";
                            SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction);
                            deleteCmd.Parameters.AddWithValue("@partyId", currentVendorId);
                            deleteCmd.ExecuteNonQuery();

                            transaction.Commit();

                            MessageBox.Show("Vendor deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadVendorsData(nametxtbox.Text.Trim());
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
                    MessageBox.Show("Error deleting vendor: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // Edit button - load selected vendor for editing
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a vendor to edit!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string vendorId = selectedRow.Cells["PartyID"].Value.ToString();
            LoadVendorDetails(vendorId);
        }

        // Load vendor details into form
        private void LoadVendorDetails(string vendorId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            p.PartyID,
                            p.PartyName,
                            p.Address,
                            p.City,
                            p.Phone1,
                            p.Phone2,
                            p.Mobile,
                            p.Fax,
                            p.EMail,
                            p.ContactPerson,
                            c.CategoryName
                        FROM Parties p
                        LEFT JOIN Category c ON p.CategoryId = c.CategoryId
                        WHERE p.PartyID = @vendorId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@vendorId", vendorId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        currentVendorId = reader["PartyID"].ToString();
                        
                        // Split PartyID into two parts if needed (e.g., "V001" -> "V" and "001")
                        if (currentVendorId.Length >= 1)
                        {
                            partyidtxtbox.Text = currentVendorId.Substring(0, 1);
                            if (currentVendorId.Length > 1)
                            {
                                partyid1txtbox.Text = currentVendorId.Substring(1);
                            }
                        }

                        nametxtbox1.Text = reader["PartyName"].ToString();
                        addresstxtbox.Text = reader["Address"].ToString();
                        citytxtbox.Text = reader["City"] != DBNull.Value ? reader["City"].ToString() : "";
                        phonetxtbox.Text = reader["Phone1"] != DBNull.Value ? reader["Phone1"].ToString() : "";
                        phone2txtbox.Text = reader["Phone2"] != DBNull.Value ? reader["Phone2"].ToString() : "";
                        mblnotxtbox.Text = reader["Mobile"] != DBNull.Value ? reader["Mobile"].ToString() : "";
                        faxtxtbox.Text = reader["Fax"] != DBNull.Value ? reader["Fax"].ToString() : "";
                        emailtxtbox.Text = reader["EMail"] != DBNull.Value ? reader["EMail"].ToString() : "";
                        conatactpersontxtbox.Text = reader["ContactPerson"] != DBNull.Value ? reader["ContactPerson"].ToString() : "";
                        
                        if (reader["CategoryName"] != DBNull.Value)
                        {
                            vendorctgrycombobox.Text = reader["CategoryName"].ToString();
                        }
                        else
                        {
                            vendorctgrycombobox.SelectedIndex = 0;
                        }

                        // Set edit mode
                        isEditMode = true;
                        savebtn.Text = "Update";
                        partyidtxtbox.Enabled = false;
                        partyid1txtbox.Enabled = false;
                        removebtn.Enabled = true;

                        MessageBox.Show("Vendor loaded for editing. Make changes and click Update.", "Edit Mode",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor details: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New button - clear form for new entry
        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // Clear button
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        // Clear all form fields
        private void ClearForm()
        {
            partyidtxtbox.Clear();
            partyid1txtbox.Clear();
            nametxtbox1.Clear();
            addresstxtbox.Clear();
            citytxtbox.Clear();
            phonetxtbox.Clear();
            phone2txtbox.Clear();
            mblnotxtbox.Clear();
            faxtxtbox.Clear();
            emailtxtbox.Clear();
            conatactpersontxtbox.Clear();
            vendorctgrycombobox.SelectedIndex = 0;

            isEditMode = false;
            currentVendorId = "";
            savebtn.Text = "Save";
            partyidtxtbox.Enabled = true;
            partyid1txtbox.Enabled = true;
            removebtn.Enabled = false;
        }

        // Close form
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        // Validate form input
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || string.IsNullOrWhiteSpace(partyid1txtbox.Text))
            {
                MessageBox.Show("Please enter Vendor ID.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                partyidtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(nametxtbox1.Text))
            {
                MessageBox.Show("Please enter Vendor Name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nametxtbox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(addresstxtbox.Text))
            {
                MessageBox.Show("Please enter Address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addresstxtbox.Focus();
                return false;
            }

            return true;
        }

        // DataGridView double click - load vendor for editing
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string vendorId = dataGridView1.Rows[e.RowIndex].Cells["PartyID"].Value.ToString();
                LoadVendorDetails(vendorId);
            }
        }

        // Form load event
        private void Vendors_Load(object sender, EventArgs e)
        {
            // Add double click event if not already added
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            
            // Set focus to search box
            nametxtbox.Focus();
        }

        // All KeyDown events for navigation
        private void partyidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                partyid1txtbox.Focus();
            }
        }

        private void partyid1txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                nametxtbox1.Focus();
            }
        }

        private void nametxtbox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                addresstxtbox.Focus();
            }
        }

        private void addresstxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                citytxtbox.Focus();
            }
        }

        private void citytxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                phonetxtbox.Focus();
            }
        }

        private void phonetxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                phone2txtbox.Focus();
            }
        }

        private void phone2txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                mblnotxtbox.Focus();
            }
        }

        private void mblnotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                faxtxtbox.Focus();
            }
        }

        private void faxtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                emailtxtbox.Focus();
            }
        }

        private void emailtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                conatactpersontxtbox.Focus();
            }
        }

        private void conatactpersontxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                vendorctgrycombobox.Focus();
            }
        }

        private void vendorctgrycombobox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                savebtn.Focus();
            }
        }

        private void savebtn_Click_1(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                string vendorId = partyidtxtbox.Text.Trim() + partyid1txtbox.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get Category ID
                    string categoryId = "0";
                    if (vendorctgrycombobox.SelectedIndex > 0)
                    {
                        string getCategoryQuery = "SELECT CategoryId FROM Category WHERE CategoryName = @categoryName";
                        SqlCommand catCmd = new SqlCommand(getCategoryQuery, conn);
                        catCmd.Parameters.AddWithValue("@categoryName", vendorctgrycombobox.Text);
                        object result = catCmd.ExecuteScalar();
                        if (result != null)
                        {
                            categoryId = result.ToString();
                        }
                    }

                    if (isEditMode)
                    {
                        // Update existing vendor
                        UpdateVendor(conn, vendorId, categoryId);
                    }
                    else
                    {
                        // Insert new vendor
                        InsertVendor(conn, vendorId, categoryId);
                    }

                    // Refresh data and clear form
                    LoadVendorsData(nametxtbox.Text.Trim());
                    ClearForm();

                    MessageBox.Show("Vendor saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627) // Unique constraint violation
                {
                    MessageBox.Show("Vendor ID already exists!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Database Error: " + sqlEx.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving vendor: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }
    }
}