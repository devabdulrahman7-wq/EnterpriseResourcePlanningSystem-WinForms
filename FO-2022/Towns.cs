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

//namespace FO_2022
//{
//    public partial class Towns : Form
//    {
//        // Database connection string - replace with your actual connection string
//        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

//        private SqlConnection connection;
//        private DataTable townsDataTable;
//        private bool isEditMode = false;
//        private int currentTownId = 0;

//        public Towns()
//        {
//            InitializeComponent();
//            InitializeDatabaseConnection();
//            LoadTownsData();
//            ResetForm();
//        }

//        private void InitializeDatabaseConnection()
//        {
//            try
//            {
//                connection = new SqlConnection(connectionString);
//                connection.Open();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Database connection error: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//                // Optionally close the form if connection fails
//                // this.Close();
//            }
//        }

//        private void LoadTownsData()
//        {
//            try
//            {
//                string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
//                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
//                townsDataTable = new DataTable();
//                adapter.Fill(townsDataTable);

//                dataGridView1.DataSource = townsDataTable;
//                dataGridView1.Columns["TownID"].HeaderText = "Town ID";
//                dataGridView1.Columns["TownName"].HeaderText = "Town Name";
//                dataGridView1.Columns["TownID"].Width = 130;
//                dataGridView1.Columns["TownName"].Width = 180;

//                // Clear selection
//                dataGridView1.ClearSelection();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading towns data: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void ResetForm()
//        {
//            townnametxtbox.Clear();
//            townnametxtbox.BackColor = Color.White;
//            townnametxtbox.Enabled = true;
//            isEditMode = false;
//            currentTownId = 0;
//            savebtn.Text = "Save";
//            savebtn.BackColor = Color.FromArgb(128, 128, 255);
//            editbtn.Enabled = true;
//            removebtn.Enabled = true;
//            newbtn.Enabled = true;
//            dataGridView1.Enabled = true;

//            // Clear grid selection
//            dataGridView1.ClearSelection();
//        }

//        private bool ValidateTownInput()
//        {
//            if (string.IsNullOrWhiteSpace(townnametxtbox.Text))
//            {
//                MessageBox.Show("Town name cannot be empty!", "Validation Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                townnametxtbox.Focus();
//                return false;
//            }

//            if (townnametxtbox.Text.Length > 30)
//            {
//                MessageBox.Show("Town name must be 30 characters or less!", "Validation Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                townnametxtbox.Focus();
//                return false;
//            }

//            return true;
//        }

//        private bool TownNameExists(string townName, int excludeTownId = 0)
//        {
//            try
//            {
//                string query = excludeTownId == 0
//                    ? "SELECT COUNT(*) FROM Towns WHERE TownName = @TownName"
//                    : "SELECT COUNT(*) FROM Towns WHERE TownName = @TownName AND TownID != @TownID";

//                using (SqlCommand cmd = new SqlCommand(query, connection))
//                {
//                    cmd.Parameters.AddWithValue("@TownName", townName);
//                    if (excludeTownId > 0)
//                    {
//                        cmd.Parameters.AddWithValue("@TownID", excludeTownId);
//                    }

//                    int count = (int)cmd.ExecuteScalar();
//                    return count > 0;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error checking town name: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return false;
//            }
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mainform1 = new Mainform1();
//            mainform1.Show();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mainform1 = new Mainform1();
//            mainform1.Show();
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ResetForm();
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count == 0)
//            {
//                MessageBox.Show("Please select a town to remove!", "No Selection",
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
//            int townId = Convert.ToInt32(selectedRow.Cells["TownID"].Value);
//            string townName = selectedRow.Cells["TownName"].Value.ToString();

//            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{townName}'?",
//                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//            if (result == DialogResult.Yes)
//            {
//                try
//                {
//                    // Check if town is being used in Sectors table
//                    string checkQuery = "SELECT COUNT(*) FROM Sectors WHERE TownID = @TownID";
//                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
//                    {
//                        checkCmd.Parameters.AddWithValue("@TownID", townId);
//                        int referenceCount = (int)checkCmd.ExecuteScalar();

//                        if (referenceCount > 0)
//                        {
//                            MessageBox.Show($"Cannot delete '{townName}' because it is referenced in {referenceCount} sector(s).\n" +
//                                          "Please delete the related sectors first.", "Delete Restricted",
//                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                            return;
//                        }
//                    }

//                    // Delete the town
//                    string deleteQuery = "DELETE FROM Towns WHERE TownID = @TownID";
//                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
//                    {
//                        cmd.Parameters.AddWithValue("@TownID", townId);
//                        int rowsAffected = cmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            MessageBox.Show($"Town '{townName}' deleted successfully!", "Success",
//                                MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            LoadTownsData();
//                            ResetForm();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Error deleting town: " + ex.Message, "Error",
//                        MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (!ValidateTownInput())
//            {
//                return;
//            }

//            string townName = townnametxtbox.Text.Trim();

//            // Check if town name already exists (for new records or when name is changed)
//            if (TownNameExists(townName, currentTownId))
//            {
//                MessageBox.Show($"Town name '{townName}' already exists!", "Duplicate Entry",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            try
//            {
//                if (isEditMode && currentTownId > 0)
//                {
//                    // Update existing town
//                    string updateQuery = "UPDATE Towns SET TownName = @TownName WHERE TownID = @TownID";
//                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
//                    {
//                        cmd.Parameters.AddWithValue("@TownName", townName);
//                        cmd.Parameters.AddWithValue("@TownID", currentTownId);
//                        int rowsAffected = cmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            MessageBox.Show("Town updated successfully!", "Success",
//                                MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            LoadTownsData();
//                            ResetForm();
//                        }
//                    }
//                }
//                else
//                {
//                    // Insert new town
//                    // Get next available TownID
//                    string maxIdQuery = "SELECT ISNULL(MAX(TownID), 0) + 1 FROM Towns";
//                    using (SqlCommand maxCmd = new SqlCommand(maxIdQuery, connection))
//                    {
//                        int newTownId = (int)maxCmd.ExecuteScalar();

//                        string insertQuery = "INSERT INTO Towns (TownID, TownName) VALUES (@TownID, @TownName)";
//                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
//                        {
//                            cmd.Parameters.AddWithValue("@TownID", newTownId);
//                            cmd.Parameters.AddWithValue("@TownName", townName);
//                            int rowsAffected = cmd.ExecuteNonQuery();

//                            if (rowsAffected > 0)
//                            {
//                                MessageBox.Show("New town added successfully!", "Success",
//                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//                                LoadTownsData();
//                                ResetForm();
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving town: " + ex.Message, "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void editbtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count == 0)
//            {
//                MessageBox.Show("Please select a town to edit!", "No Selection",
//                    MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
//            currentTownId = Convert.ToInt32(selectedRow.Cells["TownID"].Value);
//            string townName = selectedRow.Cells["TownName"].Value.ToString();

//            townnametxtbox.Text = townName;
//            townnametxtbox.Focus();
//            townnametxtbox.SelectAll();

//            isEditMode = true;
//            savebtn.Text = "Update";
//            savebtn.BackColor = Color.FromArgb(255, 128, 128); // Different color for update

//            // Highlight the textbox to indicate edit mode
//            townnametxtbox.BackColor = Color.LightYellow;
//        }

//        private void newbtn_Click(object sender, EventArgs e)
//        {
//            ResetForm();
//            townnametxtbox.Focus();
//        }

//        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
//            {
//                // Get selected town data
//                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
//                currentTownId = Convert.ToInt32(row.Cells["TownID"].Value);
//                string townName = row.Cells["TownName"].Value.ToString();

//                // Display in textbox
//                townnametxtbox.Text = townName;

//                // If we're not in edit mode, disable edit button until user clicks it
//                if (!isEditMode)
//                {
//                    editbtn.Enabled = true;
//                    removebtn.Enabled = true;
//                }
//            }
//        }

//        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count > 0 && !isEditMode)
//            {
//                // Enable edit and remove buttons when a row is selected
//                editbtn.Enabled = true;
//                removebtn.Enabled = true;
//            }
//            else if (dataGridView1.SelectedRows.Count == 0 && !isEditMode)
//            {
//                // Disable buttons when no row is selected
//                editbtn.Enabled = false;
//                removebtn.Enabled = false;
//            }
//        }

//        private void Towns_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            // Close database connection when form closes
//            if (connection != null && connection.State == ConnectionState.Open)
//            {
//                connection.Close();
//            }
//        }

//        private void townnametxtbox_KeyPress(object sender, KeyPressEventArgs e)
//        {
//            // Allow Enter key to trigger save
//            if (e.KeyChar == (char)Keys.Enter)
//            {
//                e.Handled = true;
//                savebtn.PerformClick();
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

namespace FO_2022
{
    public partial class Towns : Form
    {
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        private SqlConnection connection;
        private DataTable townsDataTable;
        private bool isEditMode = false;
        private int currentTownId = 0;

        public Towns()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadTownsData();
            ResetForm();
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTownsData()
        {
            try
            {
                string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                townsDataTable = new DataTable();
                adapter.Fill(townsDataTable);

                dataGridView1.DataSource = townsDataTable;
                dataGridView1.Columns["TownID"].HeaderText = "Town ID";
                dataGridView1.Columns["TownName"].HeaderText = "Town Name";
                dataGridView1.Columns["TownID"].Width = 130;
                dataGridView1.Columns["TownName"].Width = 180;

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            townnametxtbox.Clear();
            isEditMode = false;
            currentTownId = 0;
            savebtn.Text = "Save";
            editbtn.Enabled = true;
            removebtn.Enabled = true;
            newbtn.Enabled = true;
            dataGridView1.Enabled = true;

            dataGridView1.ClearSelection();
        }

        private bool ValidateTownInput()
        {
            if (string.IsNullOrWhiteSpace(townnametxtbox.Text))
            {
                MessageBox.Show("Town name cannot be empty!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                townnametxtbox.Focus();
                return false;
            }

            if (townnametxtbox.Text.Length > 30)
            {
                MessageBox.Show("Town name must be 30 characters or less!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                townnametxtbox.Focus();
                return false;
            }

            return true;
        }

        private bool TownNameExists(string townName, int excludeTownId = 0)
        {
            try
            {
                string query = excludeTownId == 0
                    ? "SELECT COUNT(*) FROM Towns WHERE TownName = @TownName"
                    : "SELECT COUNT(*) FROM Towns WHERE TownName = @TownName AND TownID != @TownID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TownName", townName);
                    if (excludeTownId > 0)
                    {
                        cmd.Parameters.AddWithValue("@TownID", excludeTownId);
                    }

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception)
            {
                // Silent fail - if there's an error, we'll assume name doesn't exist
                return false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a town to remove!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int townId = Convert.ToInt32(selectedRow.Cells["TownID"].Value);
            string townName = selectedRow.Cells["TownName"].Value.ToString();

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{townName}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Simple delete without checking Sectors table
                    string deleteQuery = "DELETE FROM Towns WHERE TownID = @TownID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@TownID", townId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Town '{townName}' deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTownsData();
                            ResetForm();
                        }
                        else
                        {
                            MessageBox.Show("Town not found or already deleted.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Foreign key constraint error
                    {
                        MessageBox.Show($"Cannot delete '{townName}' because it is being used in other records.",
                            "Delete Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Error deleting town: " + sqlEx.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateTownInput())
            {
                return;
            }

            string townName = townnametxtbox.Text.Trim();

            // Check if town name already exists
            if (TownNameExists(townName, currentTownId))
            {
                MessageBox.Show($"Town name '{townName}' already exists!", "Duplicate Entry",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (isEditMode && currentTownId > 0)
                {
                    // Update existing town
                    string updateQuery = "UPDATE Towns SET TownName = @TownName WHERE TownID = @TownID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@TownName", townName);
                        cmd.Parameters.AddWithValue("@TownID", currentTownId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Town updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTownsData();
                            ResetForm();
                        }
                    }
                }
                else
                {
                    // Insert new town
                    // Get next available TownID
                    string maxIdQuery = "SELECT ISNULL(MAX(TownID), 0) + 1 FROM Towns";
                    using (SqlCommand maxCmd = new SqlCommand(maxIdQuery, connection))
                    {
                        int newTownId = (int)maxCmd.ExecuteScalar();

                        string insertQuery = "INSERT INTO Towns (TownID, TownName) VALUES (@TownID, @TownName)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@TownID", newTownId);
                            cmd.Parameters.AddWithValue("@TownName", townName);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show(" Town added successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadTownsData();
                                ResetForm();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving town: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a town to edit!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            currentTownId = Convert.ToInt32(selectedRow.Cells["TownID"].Value);
            string townName = selectedRow.Cells["TownName"].Value.ToString();

            townnametxtbox.Text = townName;
            townnametxtbox.Focus();
            townnametxtbox.SelectAll();

            isEditMode = true;
            savebtn.Text = "Update";
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
            townnametxtbox.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentTownId = Convert.ToInt32(row.Cells["TownID"].Value);
                string townName = row.Cells["TownName"].Value.ToString();

                townnametxtbox.Text = townName;

                if (!isEditMode)
                {
                    editbtn.Enabled = true;
                    removebtn.Enabled = true;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && !isEditMode)
            {
                editbtn.Enabled = true;
                removebtn.Enabled = true;
            }
            else if (dataGridView1.SelectedRows.Count == 0 && !isEditMode)
            {
                editbtn.Enabled = false;
                removebtn.Enabled = false;
            }
        }

        private void Towns_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void townnametxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                savebtn.PerformClick();
            }
        }

        private void townnametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }
    }
}