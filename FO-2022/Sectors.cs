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
    public partial class Sectors : Form
    {
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        private SqlConnection connection;
        private DataTable sectorsDataTable;
        private bool isEditMode = false;
        private int currentSectorId = 0;
        private int currentTownId = 0;

        public Sectors()
        {
            InitializeComponent();
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

        private void LoadTownsComboBox()
        {
            try
            {
                townnamecombobox.Items.Clear();

                string query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        townnamecombobox.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }

                // Set display member and value member
                townnamecombobox.DisplayMember = "Name";
                townnamecombobox.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSectorsData()
        {
            try
            {
                string query = @"SELECT s.SectorID, s.SectorName, s.TownID, t.TownName 
                                FROM Sectors s 
                                LEFT JOIN Towns t ON s.TownID = t.TownID 
                                ORDER BY t.TownName, s.SectorName";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                sectorsDataTable = new DataTable();
                adapter.Fill(sectorsDataTable);

                dataGridView1.DataSource = sectorsDataTable;

                // Hide TownID column from grid
                dataGridView1.Columns["TownID"].Visible = false;

                // Set column headers
                dataGridView1.Columns["SectorID"].HeaderText = "Sector ID";
                dataGridView1.Columns["SectorName"].HeaderText = "Sector Name";
                dataGridView1.Columns["TownName"].HeaderText = "Town Name";

                // Set column widths
                dataGridView1.Columns["SectorID"].Width = 80;
                dataGridView1.Columns["SectorName"].Width = 150;
                dataGridView1.Columns["TownName"].Width = 120;

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sectors data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetForm()
        {
            sctrnametxtbox.Clear();
            townnamecombobox.SelectedIndex = -1;
            townnamecombobox.Text = "";
            isEditMode = false;
            currentSectorId = 0;
            currentTownId = 0;
            savebtn.Text = "Save";
            editbtn.Enabled = true;
            removebtn.Enabled = true;
            newbtn.Enabled = true;
            dataGridView1.Enabled = true;

            dataGridView1.ClearSelection();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(sctrnametxtbox.Text))
            {
                MessageBox.Show("Sector name cannot be empty!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sctrnametxtbox.Focus();
                return false;
            }

            if (sctrnametxtbox.Text.Length > 30)
            {
                MessageBox.Show("Sector name must be 30 characters or less!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sctrnametxtbox.Focus();
                return false;
            }

            if (townnamecombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a town!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                townnamecombobox.Focus();
                return false;
            }

            return true;
        }

        private bool SectorNameExists(string sectorName, int townId, int excludeSectorId = 0)
        {
            try
            {
                string query = excludeSectorId == 0
                    ? "SELECT COUNT(*) FROM Sectors WHERE SectorName = @SectorName AND TownID = @TownID"
                    : "SELECT COUNT(*) FROM Sectors WHERE SectorName = @SectorName AND TownID = @TownID AND SectorID != @SectorID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SectorName", sectorName);
                    cmd.Parameters.AddWithValue("@TownID", townId);
                    if (excludeSectorId > 0)
                    {
                        cmd.Parameters.AddWithValue("@SectorID", excludeSectorId);
                    }

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception)
            {
                // Silent fail
                return false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }

        private void Sectors_Load(object sender, EventArgs e)
        {
            InitializeDatabaseConnection();
            LoadTownsComboBox();
            LoadSectorsData();
            ResetForm();
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
                MessageBox.Show("Please select a sector to remove!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int sectorId = Convert.ToInt32(selectedRow.Cells["SectorID"].Value);
            string sectorName = selectedRow.Cells["SectorName"].Value.ToString();

            DialogResult result = MessageBox.Show($"Are you sure you want to delete '{sectorName}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Simple delete
                    string deleteQuery = "DELETE FROM Sectors WHERE SectorID = @SectorID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@SectorID", sectorId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Sector '{sectorName}' deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSectorsData();
                            ResetForm();
                        }
                        else
                        {
                            MessageBox.Show("Sector not found or already deleted.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Foreign key constraint error
                    {
                        MessageBox.Show($"Cannot delete '{sectorName}' because it is being used in other records.",
                            "Delete Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Error deleting sector: " + sqlEx.Message, "Error",
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
            if (!ValidateInput())
            {
                return;
            }

            string sectorName = sctrnametxtbox.Text.Trim();

            // Get selected town ID
            dynamic selectedTown = townnamecombobox.SelectedItem;
            int townId = selectedTown.Id;

            // Check if sector name already exists in the same town
            if (SectorNameExists(sectorName, townId, currentSectorId))
            {
                MessageBox.Show($"Sector name '{sectorName}' already exists in this town!", "Duplicate Entry",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (isEditMode && currentSectorId > 0)
                {
                    // Update existing sector
                    string updateQuery = "UPDATE Sectors SET SectorName = @SectorName, TownID = @TownID WHERE SectorID = @SectorID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@SectorName", sectorName);
                        cmd.Parameters.AddWithValue("@TownID", townId);
                        cmd.Parameters.AddWithValue("@SectorID", currentSectorId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sector updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSectorsData();
                            ResetForm();
                        }
                    }
                }
                else
                {
                    // Insert new sector
                    // Get next available SectorID
                    string maxIdQuery = "SELECT ISNULL(MAX(SectorID), 0) + 1 FROM Sectors";
                    using (SqlCommand maxCmd = new SqlCommand(maxIdQuery, connection))
                    {
                        int newSectorId = (int)maxCmd.ExecuteScalar();

                        string insertQuery = "INSERT INTO Sectors (SectorID, SectorName, TownID) VALUES (@SectorID, @SectorName, @TownID)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@SectorID", newSectorId);
                            cmd.Parameters.AddWithValue("@SectorName", sectorName);
                            cmd.Parameters.AddWithValue("@TownID", townId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("New sector added successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadSectorsData();
                                ResetForm();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving sector: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sector to edit!", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            currentSectorId = Convert.ToInt32(selectedRow.Cells["SectorID"].Value);
            string sectorName = selectedRow.Cells["SectorName"].Value.ToString();
            int townId = Convert.ToInt32(selectedRow.Cells["TownID"].Value);

            sctrnametxtbox.Text = sectorName;
            currentTownId = townId;

            // Select the town in combobox
            foreach (dynamic item in townnamecombobox.Items)
            {
                if (item.Id == townId)
                {
                    townnamecombobox.SelectedItem = item;
                    break;
                }
            }

            sctrnametxtbox.Focus();
            sctrnametxtbox.SelectAll();

            isEditMode = true;
            savebtn.Text = "Update";
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
            sctrnametxtbox.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentSectorId = Convert.ToInt32(row.Cells["SectorID"].Value);
                string sectorName = row.Cells["SectorName"].Value.ToString();
                int townId = Convert.ToInt32(row.Cells["TownID"].Value);

                sctrnametxtbox.Text = sectorName;
                currentTownId = townId;

                // Select the town in combobox
                foreach (dynamic item in townnamecombobox.Items)
                {
                    if (item.Id == townId)
                    {
                        townnamecombobox.SelectedItem = item;
                        break;
                    }
                }

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

        private void Sectors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void sctrnametxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                savebtn.PerformClick();
            }
        }
        private void sctrnametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }
        private void townnamecombobox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                savebtn.PerformClick();
            }
        }
    }
}