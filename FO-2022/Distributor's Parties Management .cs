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
//    public partial class Distributor_s_Parties_Management : Form
//    {
//        public Distributor_s_Parties_Management()
//        {
//            InitializeComponent();
//        }

//        private void pictureBox1_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void distributorIdtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void distributoridsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void distributorNametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partynametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void sectornametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partiesdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void partynamegrptxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void SearchdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void switchbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void addmodbtn_Click(object sender, EventArgs e)
//        {

//        }
//    }
//}




//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Configuration;
//using System.Data.SqlClient;

//namespace FO_2022
//{
//    public partial class Distributor_s_Parties_Management : Form
//    {
//        private string connectionString;
//        private DataTable partiesDataTable;
//        private DataTable searchDataTable;
//        private int currentDistributorId = 0;

//        public Distributor_s_Parties_Management()
//        {
//            InitializeComponent();
//            InitializeConnection();
//            InitializeDataGridViews();
//            LoadAllDistributors();
//        }

//        private void InitializeConnection()
//        {
//            try
//            {
//                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading connection string: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                this.Close();
//            }
//        }

//        private void InitializeDataGridViews()
//        {
//            // Configure partiesdataGridView
//            partiesdataGridView.AutoGenerateColumns = false;
//            partiesdataGridView.Columns.Clear();

//            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PartyID",
//                HeaderText = "Party ID",
//                DataPropertyName = "PartyID",
//                Width = 80
//            });

//            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PartyName",
//                HeaderText = "Party Name",
//                DataPropertyName = "PartyName",
//                Width = 200
//            });

//            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SectorName",
//                HeaderText = "Sector",
//                DataPropertyName = "SectorName",
//                Width = 100
//            });

//            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "TownName",
//                HeaderText = "Town",
//                DataPropertyName = "TownName",
//                Width = 100
//            });

//            // Configure SearchdataGridView
//            SearchdataGridView.AutoGenerateColumns = false;
//            SearchdataGridView.Columns.Clear();

//            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "CustomerId",
//                HeaderText = "Customer ID",
//                DataPropertyName = "CustomerId",
//                Width = 80
//            });

//            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "CustomerName",
//                HeaderText = "Customer Name",
//                DataPropertyName = "CustomerName",
//                Width = 200
//            });

//            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "TownName",
//                HeaderText = "Town",
//                DataPropertyName = "TownName",
//                Width = 100
//            });

//            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SectorName",
//                HeaderText = "Sector",
//                DataPropertyName = "SectorName",
//                Width = 100
//            });
//        }

//        private void LoadAllDistributors()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"SELECT p.PartyID, p.PartyName 
//                                     FROM Parties p 
//                                     WHERE p.IsDistributor = 1 
//                                     ORDER BY p.PartyName";

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    conn.Open();

//                    SqlDataReader reader = cmd.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        // You can store this data or use it as needed
//                    }
//                    reader.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading distributors: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void pictureBox1_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void distributorIdtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void distributoridsearch_Click(object sender, EventArgs e)
//        {
//            SearchDistributor();
//        }

//        private void SearchDistributor()
//        {
//            if (string.IsNullOrWhiteSpace(distributorIdtxtbox.Text))
//            {
//                MessageBox.Show("Please enter a Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (!int.TryParse(distributorIdtxtbox.Text, out int distributorId))
//            {
//                MessageBox.Show("Please enter a valid numeric Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    // Query to get distributor details and their parties
//                    string query = @"
//                        -- Get Distributor Details
//                        SELECT p.PartyName AS DistributorName 
//                        FROM Parties p 
//                        WHERE p.PartyID = @DistributorId AND p.IsDistributor = 1;

//                        -- Get Parties under this distributor
//                        SELECT 
//                            p.PartyID,
//                            p.PartyName,
//                            s.SectorName,
//                            t.TownName,
//                            p.SectorID
//                        FROM Parties p
//                        LEFT JOIN Sectors s ON p.SectorID = s.SectorID
//                        LEFT JOIN Towns t ON s.TownID = t.TownID
//                        WHERE p.DistributorId = @DistributorId
//                        ORDER BY p.PartyName;";

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@DistributorId", distributorId);

//                    conn.Open();

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        // Read distributor name
//                        if (reader.Read())
//                        {
//                            distributorNametxtbox.Text = reader["DistributorName"].ToString();
//                            currentDistributorId = distributorId;
//                        }
//                        else
//                        {
//                            MessageBox.Show("Distributor not found or is not marked as a distributor", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            ClearDistributorFields();
//                            return;
//                        }

//                        // Move to next result set (parties)
//                        reader.NextResult();

//                        // Load parties into DataTable
//                        partiesDataTable = new DataTable();
//                        partiesDataTable.Load(reader);

//                        // Bind to DataGridView
//                        partiesdataGridView.DataSource = partiesDataTable;

//                        // Clear individual party fields
//                        ClearPartyFields();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error searching distributor: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void ClearDistributorFields()
//        {
//            distributorNametxtbox.Clear();
//            currentDistributorId = 0;
//            partiesdataGridView.DataSource = null;
//            ClearPartyFields();
//        }

//        private void ClearPartyFields()
//        {
//            partyidtxtbox.Clear();
//            partynametxtbox.Clear();
//            sectornametxtbox.Clear();
//             townnametxtbox.Clear(); // Removed as per your structure
//        }

//        private void distributorNametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partynametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void sectornametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partiesdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle cell content click if needed
//        }

//        private void partiesdataGridView_SelectionChanged(object sender, EventArgs e)
//        {
//            if (partiesdataGridView.SelectedRows.Count > 0)
//            {
//                DataGridViewRow selectedRow = partiesdataGridView.SelectedRows[0];
//                partyidtxtbox.Text = selectedRow.Cells["PartyID"].Value?.ToString() ?? "";
//                partynametxtbox.Text = selectedRow.Cells["PartyName"].Value?.ToString() ?? "";
//                sectornametxtbox.Text = selectedRow.Cells["SectorName"].Value?.ToString() ?? "";
//                 townnametxtbox.Text = selectedRow.Cells["TownName"].Value?.ToString() ?? ""; // Removed
//            }
//        }

//        private void partynamegrptxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void partynamegrptxtbox_KeyPress(object sender, KeyPressEventArgs e)
//        {
//            if (e.KeyChar == (char)Keys.Enter)
//            {
//                SearchPartiesByName();
//                e.Handled = true;
//            }
//        }

//        private void SearchPartiesByName()
//        {
//            if (string.IsNullOrWhiteSpace(partynamegrptxtbox.Text))
//            {
//                MessageBox.Show("Please enter a party name to search", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"
//                        SELECT 
//                            p.PartyID AS CustomerId,
//                            p.PartyName AS CustomerName,
//                            t.TownName,
//                            s.SectorName
//                        FROM Parties p
//                        LEFT JOIN Sectors s ON p.SectorID = s.SectorID
//                        LEFT JOIN Towns t ON s.TownID = t.TownID
//                        WHERE p.PartyName LIKE @PartyName + '%'
//                        ORDER BY p.PartyName";

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@PartyName", partynamegrptxtbox.Text.Trim());

//                    conn.Open();

//                    searchDataTable = new DataTable();
//                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
//                    {
//                        da.Fill(searchDataTable);
//                    }

//                    SearchdataGridView.DataSource = searchDataTable;

//                    if (searchDataTable.Rows.Count == 0)
//                    {
//                        MessageBox.Show("No parties found with the given name", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error searching parties: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void SearchdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle cell content click if needed
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            SaveParty();
//        }

//        private void SaveParty()
//        {
//            // Validate required fields
//            if (currentDistributorId == 0)
//            {
//                MessageBox.Show("Please select a distributor first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (string.IsNullOrWhiteSpace(partynametxtbox.Text))
//            {
//                MessageBox.Show("Please enter Party Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    // Check if we're updating or inserting
//                    if (!string.IsNullOrWhiteSpace(partyidtxtbox.Text) && int.TryParse(partyidtxtbox.Text, out int partyId))
//                    {
//                        // Update existing party
//                        string updateQuery = @"
//                            UPDATE Parties 
//                            SET PartyName = @PartyName,
//                                DistributorId = @DistributorId
//                            WHERE PartyID = @PartyID";

//                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
//                        updateCmd.Parameters.AddWithValue("@PartyName", partynametxtbox.Text.Trim());
//                        updateCmd.Parameters.AddWithValue("@DistributorId", currentDistributorId);
//                        updateCmd.Parameters.AddWithValue("@PartyID", partyId);

//                        int rowsAffected = updateCmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            MessageBox.Show("Party updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            // Refresh the grid
//                            SearchDistributor();
//                        }
//                    }
//                    else
//                    {
//                        // Insert new party - first get the next PartyID
//                        string maxIdQuery = "SELECT ISNULL(MAX(PartyID), 0) + 1 AS NextID FROM Parties";
//                        SqlCommand maxIdCmd = new SqlCommand(maxIdQuery, conn);
//                        int nextPartyId = Convert.ToInt32(maxIdCmd.ExecuteScalar());

//                        string insertQuery = @"
//                            INSERT INTO Parties 
//                            (PartyID, PartyName, DistributorId, IsDistributor)
//                            VALUES 
//                            (@PartyID, @PartyName, @DistributorId, 0)";

//                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
//                        insertCmd.Parameters.AddWithValue("@PartyID", nextPartyId);
//                        insertCmd.Parameters.AddWithValue("@PartyName", partynametxtbox.Text.Trim());
//                        insertCmd.Parameters.AddWithValue("@DistributorId", currentDistributorId);

//                        int rowsAffected = insertCmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            MessageBox.Show("Party added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            partyidtxtbox.Text = nextPartyId.ToString();
//                            // Refresh the grid
//                            SearchDistributor();
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error saving party: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        //private void switchbtn_Click(object sender, EventArgs e)
//        //{
//        //    SwitchPartyDistributor();
//        //}

//        //private void SwitchPartyDistributor()
//        //{
//        //    if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || !int.TryParse(partyidtxtbox.Text, out int partyId))
//        //    {
//        //        MessageBox.Show("Please select a party first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        return;
//        //    }

//        //    if (currentDistributorId == 0)
//        //    {
//        //        MessageBox.Show("Please select a distributor first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        return;
//        //    }

//        //    // Ask for new distributor ID
//        //    //string newDistributorId = Microsoft.VisualBasic.Interaction.InputBox(
//        //    //    "Enter new Distributor ID:",
//        //    //    "Switch Distributor",
//        //    //    "");

//        //    //if (string.IsNullOrWhiteSpace(newDistributorId) || !int.TryParse(newDistributorId, out int newId))
//        //    //{
//        //    //    MessageBox.Show("Please enter a valid Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //    //    return;
//        //    //}

//        //    try
//        //    {
//        //        using (SqlConnection conn = new SqlConnection(connectionString))
//        //        {
//        //            // Verify new distributor exists and is a distributor
//        //            string verifyQuery = "SELECT COUNT(*) FROM Parties WHERE PartyID = @DistributorId AND IsDistributor = 1";
//        //            SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn);
//        //            verifyCmd.Parameters.AddWithValue("@DistributorId", newId);

//        //            conn.Open();

//        //            int distributorCount = Convert.ToInt32(verifyCmd.ExecuteScalar());

//        //            if (distributorCount == 0)
//        //            {
//        //                MessageBox.Show("The specified ID does not belong to a valid distributor", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //                return;
//        //            }

//        //            // Update the party's distributor
//        //            string updateQuery = "UPDATE Parties SET DistributorId = @NewDistributorId WHERE PartyID = @PartyID";
//        //            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
//        //            updateCmd.Parameters.AddWithValue("@NewDistributorId", newId);
//        //            updateCmd.Parameters.AddWithValue("@PartyID", partyId);

//        //            int rowsAffected = updateCmd.ExecuteNonQuery();

//        //            if (rowsAffected > 0)
//        //            {
//        //                MessageBox.Show("Party distributor switched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        //                // Refresh current distributor's parties
//        //                SearchDistributor();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show($"Error switching distributor: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //    }
//        //}

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            RemovePartyFromDistributor();
//        }

//        private void RemovePartyFromDistributor()
//        {
//            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || !int.TryParse(partyidtxtbox.Text, out int partyId))
//            {
//                MessageBox.Show("Please select a party to remove", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            DialogResult result = MessageBox.Show(
//                "Are you sure you want to remove this party from the distributor?",
//                "Confirm Removal",
//                MessageBoxButtons.YesNo,
//                MessageBoxIcon.Question);

//            if (result == DialogResult.Yes)
//            {
//                try
//                {
//                    using (SqlConnection conn = new SqlConnection(connectionString))
//                    {
//                        string query = "UPDATE Parties SET DistributorId = NULL WHERE PartyID = @PartyID";
//                        SqlCommand cmd = new SqlCommand(query, conn);
//                        cmd.Parameters.AddWithValue("@PartyID", partyId);

//                        conn.Open();
//                        int rowsAffected = cmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            MessageBox.Show("Party removed from distributor successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                            // Refresh the grid
//                            SearchDistributor();
//                            ClearPartyFields();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error removing party: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void addmodbtn_Click(object sender, EventArgs e)
//        {
//            AddNewParty();
//        }

//        private void AddNewParty()
//        {
//            // Clear fields for new entry
//            ClearPartyFields();
//            partynametxtbox.Focus();

//            // Generate suggestion for party name if distributor is selected
//            if (!string.IsNullOrWhiteSpace(distributorNametxtbox.Text))
//            {
//                partynametxtbox.Text = $"New Party under {distributorNametxtbox.Text}";
//                partynametxtbox.SelectAll();
//            }
//        }

//        private void distributorIdtxtbox_KeyPress(object sender, KeyPressEventArgs e)
//        {
//            if (e.KeyChar == (char)Keys.Enter)
//            {
//                SearchDistributor();
//                e.Handled = true;
//            }
//        }
//    }
//}







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
    public partial class Distributor_s_Parties_Management : Form
    {
        private string connectionString;
        private DataTable partiesDataTable;
        private DataTable searchDataTable;
        private int currentDistributorId = 0;

        public Distributor_s_Parties_Management()
        {
            InitializeComponent();
            partynamegrptxtbox.KeyPress += partynamegrptxtbox_KeyPress;
            InitializeConnection();
            InitializeDataGridViews();
            LoadAllDistributors();
        }

        private void InitializeConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading connection string: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void InitializeDataGridViews()
        {
            // Configure partiesdataGridView
            partiesdataGridView.AutoGenerateColumns = false;
            partiesdataGridView.Columns.Clear();

            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PartyID",
                HeaderText = "Party ID",
                DataPropertyName = "PartyID",
                Width = 80
            });

            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PartyName",
                HeaderText = "Party Name",
                DataPropertyName = "PartyName",
                Width = 200
            });

            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SectorName",
                HeaderText = "Sector",
                DataPropertyName = "SectorName",
                Width = 100
            });

            partiesdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "TownName",
                HeaderText = "Town",
                DataPropertyName = "TownName",
                Width = 100
            });

            // Configure SearchdataGridView
            SearchdataGridView.AutoGenerateColumns = false;
            SearchdataGridView.Columns.Clear();

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                Width = 200
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "TownName",
                HeaderText = "Town",
                DataPropertyName = "TownName",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SectorName",
                HeaderText = "Sector",
                DataPropertyName = "SectorName",
                Width = 100
            });
        }

        private void LoadAllDistributors()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.PartyID, p.PartyName 
                                     FROM Parties p 
                                     WHERE p.IsDistributor = 1 
                                     ORDER BY p.PartyName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // You can store this data or use it as needed
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading distributors: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void distributorIdtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void distributoridsearch_Click(object sender, EventArgs e)
        {
            SearchDistributor();
        }

        private void SearchDistributor()
        {
            if (string.IsNullOrWhiteSpace(distributorIdtxtbox.Text))
            {
                MessageBox.Show("Please enter a Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(distributorIdtxtbox.Text, out int distributorId))
            {
                MessageBox.Show("Please enter a valid numeric Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query to get distributor details and their parties
                    string query = @"
                        -- Get Distributor Details
                        SELECT p.PartyName AS DistributorName 
                        FROM Parties p 
                        WHERE p.PartyID = @DistributorId AND p.IsDistributor = 1;
                        
                        -- Get Parties under this distributor
                        SELECT 
                            p.PartyID,
                            p.PartyName,
                            s.SectorName,
                            t.TownName,
                            p.SectorID
                        FROM Parties p
                        LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                        LEFT JOIN Towns t ON s.TownID = t.TownID
                        WHERE p.DistributorId = @DistributorId
                        ORDER BY p.PartyName;";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@DistributorId", distributorId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Read distributor name
                        if (reader.Read())
                        {
                            distributorNametxtbox.Text = reader["DistributorName"].ToString();
                            currentDistributorId = distributorId;
                        }
                        else
                        {
                            MessageBox.Show("Distributor not found or is not marked as a distributor", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearDistributorFields();
                            return;
                        }

                        // Move to next result set (parties)
                        reader.NextResult();

                        // Load parties into DataTable
                        partiesDataTable = new DataTable();
                        partiesDataTable.Load(reader);

                        // Bind to DataGridView
                        partiesdataGridView.DataSource = partiesDataTable;

                        // Clear individual party fields
                        ClearPartyFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching distributor: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDistributorFields()
        {
            distributorNametxtbox.Clear();
            currentDistributorId = 0;
            partiesdataGridView.DataSource = null;
            ClearPartyFields();
        }

        private void ClearPartyFields()
        {
            partyidtxtbox.Clear();
            partynametxtbox.Clear();
            sectornametxtbox.Clear();
            townnametxtbox.Clear();
        }

        private void distributorNametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void partynametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sectornametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void partiesdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell content click if needed
        }

        private void partiesdataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (partiesdataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = partiesdataGridView.SelectedRows[0];
                partyidtxtbox.Text = selectedRow.Cells["PartyID"].Value?.ToString() ?? "";
                partynametxtbox.Text = selectedRow.Cells["PartyName"].Value?.ToString() ?? "";
                sectornametxtbox.Text = selectedRow.Cells["SectorName"].Value?.ToString() ?? "";
                townnametxtbox.Text = selectedRow.Cells["TownName"].Value?.ToString() ?? "";
            }
        }

        private void partynamegrptxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void partynamegrptxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchParties();
                e.Handled = true;
            }
        }

        //private void SearchParties()
        //{
        //    if (string.IsNullOrWhiteSpace(partynamegrptxtbox.Text))
        //    {
        //        MessageBox.Show("Please enter a Party ID or Party Name to search", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            string query = "";
        //            SqlCommand cmd = new SqlCommand();

        //            // Check if input is numeric (PartyID search)
        //            if (int.TryParse(partynamegrptxtbox.Text.Trim(), out int partyId))
        //            {
        //                // Search by PartyID (exact match)
        //                query = @"
        //                    SELECT 
        //                        p.PartyID AS CustomerId,
        //                        p.PartyName AS CustomerName,
        //                        t.TownName,
        //                        s.SectorName
        //                    FROM Parties p
        //                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
        //                    LEFT JOIN Towns t ON s.TownID = t.TownID
        //                    WHERE p.PartyID = @PartyID
        //                    ORDER BY p.PartyName";

        //                cmd = new SqlCommand(query, conn);
        //                cmd.Parameters.AddWithValue("@PartyID", partyId);
        //            }
        //            else
        //            {
        //                // Search by PartyName (partial match)
        //                query = @"
        //                    SELECT 
        //                        p.PartyID AS CustomerId,
        //                        p.PartyName AS CustomerName,
        //                        t.TownName,
        //                        s.SectorName
        //                    FROM Parties p
        //                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
        //                    LEFT JOIN Towns t ON s.TownID = t.TownID
        //                    WHERE p.PartyName LIKE @PartyName + '%'
        //                    ORDER BY p.PartyName";

        //                cmd = new SqlCommand(query, conn);
        //                cmd.Parameters.AddWithValue("@PartyName", partynamegrptxtbox.Text.Trim());
        //            }

        //            conn.Open();

        //            searchDataTable = new DataTable();
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(searchDataTable);
        //            }

        //            SearchdataGridView.DataSource = searchDataTable;

        //            if (searchDataTable.Rows.Count == 0)
        //            {
        //                MessageBox.Show("No parties found with the given criteria", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error searching parties: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void SearchParties()
        //{
        //    if (string.IsNullOrWhiteSpace(partynamegrptxtbox.Text))
        //    {
        //        MessageBox.Show("Please enter a Party ID or Party Name to search", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            string query = "";
        //            SqlCommand cmd = new SqlCommand();

        //            // Check if input is numeric (PartyID search)
        //            if (int.TryParse(partynamegrptxtbox.Text.Trim(), out int partyId))
        //            {
        //                // Search sales data by PartyID - Convert PartyID to varchar to match CustomerId in Sales table
        //                query = @"
        //            SELECT 
        //                s.CustomerId,
        //                s.CustomerName,
        //                t.TownName,
        //                sec.SectorName,
        //                s.SaleId,
        //                s.SaleDate,
        //                s.BillNo,
        //                s.ReceivedAmount
        //            FROM Sales s
        //            LEFT JOIN Sectors sec ON s.SectorID = sec.SectorID
        //            LEFT JOIN Towns t ON sec.TownID = t.TownID
        //            LEFT JOIN Parties p ON s.CustomerId = CAST(p.PartyID AS VARCHAR(10))
        //            WHERE p.PartyID = @PartyID
        //            ORDER BY s.SaleDate DESC";

        //                cmd = new SqlCommand(query, conn);
        //                cmd.Parameters.AddWithValue("@PartyID", partyId);
        //            }
        //            else
        //            {
        //                // Search by PartyName (partial match) - Show parties information
        //                query = @"
        //            SELECT 
        //                p.PartyID AS CustomerId,
        //                p.PartyName AS CustomerName,
        //                t.TownName,
        //                s.SectorName
        //            FROM Parties p
        //            LEFT JOIN Sectors s ON p.SectorID = s.SectorID
        //            LEFT JOIN Towns t ON s.TownID = t.TownID
        //            WHERE p.PartyName LIKE @PartyName + '%'
        //            ORDER BY p.PartyName";

        //                cmd = new SqlCommand(query, conn);
        //                cmd.Parameters.AddWithValue("@PartyName", partynamegrptxtbox.Text.Trim());
        //            }

        //            conn.Open();

        //            searchDataTable = new DataTable();
        //            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //            {
        //                da.Fill(searchDataTable);
        //            }

        //            // Update DataGridView columns if searching by PartyID (showing sales data)
        //            if (int.TryParse(partynamegrptxtbox.Text.Trim(), out _))
        //            {
        //                UpdateSearchGridForSalesData();
        //            }
        //            else
        //            {
        //                UpdateSearchGridForPartyData();
        //            }

        //            SearchdataGridView.DataSource = searchDataTable;

        //            if (searchDataTable.Rows.Count == 0)
        //            {
        //                MessageBox.Show("No data found with the given criteria", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error searching data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void SearchParties()
        {
            if (string.IsNullOrWhiteSpace(partynamegrptxtbox.Text))
            {
                MessageBox.Show("Please enter a Party ID or Party Name to search", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "";
                    SqlCommand cmd = new SqlCommand();

                    // Check if input is numeric (PartyID search)
                    if (int.TryParse(partynamegrptxtbox.Text.Trim(), out int partyId))
                    {
                        // Search by PartyID - Get party information
                        query = @"
                    SELECT DISTINCT
                        p.PartyID AS CustomerId,
                        p.PartyName AS CustomerName,
                        t.TownName,
                        s.SectorName
                    FROM Parties p
                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                    LEFT JOIN Towns t ON s.TownID = t.TownID
                    WHERE p.PartyID = @PartyID";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PartyID", partyId);
                    }
                    else
                    {
                        // Search by PartyName (partial match) - Show parties information
                        query = @"
                    SELECT 
                        p.PartyID AS CustomerId,
                        p.PartyName AS CustomerName,
                        t.TownName,
                        s.SectorName
                    FROM Parties p
                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                    LEFT JOIN Towns t ON s.TownID = t.TownID
                    WHERE p.PartyName LIKE @PartyName + '%'
                    ORDER BY p.PartyName";

                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PartyName", partynamegrptxtbox.Text.Trim());
                    }

                    conn.Open();

                    searchDataTable = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(searchDataTable);
                    }

                    // Always use the same grid columns for both searches
                    UpdateSearchGridColumns();

                    SearchdataGridView.DataSource = searchDataTable;

                    if (searchDataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found with the given criteria", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSearchGridColumns()
        {
            SearchdataGridView.AutoGenerateColumns = false;
            SearchdataGridView.Columns.Clear();

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                Width = 200
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "TownName",
                HeaderText = "Town",
                DataPropertyName = "TownName",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SectorName",
                HeaderText = "Sector",
                DataPropertyName = "SectorName",
                Width = 100
            });
        }
        private void UpdateSearchGridForSalesData()
        {
            SearchdataGridView.AutoGenerateColumns = false;
            SearchdataGridView.Columns.Clear();

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                Width = 150
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "TownName",
                HeaderText = "Town",
                DataPropertyName = "TownName",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SectorName",
                HeaderText = "Sector",
                DataPropertyName = "SectorName",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SaleId",
                HeaderText = "Sale ID",
                DataPropertyName = "SaleId",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SaleDate",
                HeaderText = "Sale Date",
                DataPropertyName = "SaleDate",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "BillNo",
                HeaderText = "Bill No",
                DataPropertyName = "BillNo",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "ReceivedAmount",
                HeaderText = "Amount",
                DataPropertyName = "ReceivedAmount",
                Width = 80
            });
        }

        private void UpdateSearchGridForPartyData()
        {
            SearchdataGridView.AutoGenerateColumns = false;
            SearchdataGridView.Columns.Clear();

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                Width = 80
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                Width = 200
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "TownName",
                HeaderText = "Town",
                DataPropertyName = "TownName",
                Width = 100
            });

            SearchdataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SectorName",
                HeaderText = "Sector",
                DataPropertyName = "SectorName",
                Width = 100
            });
        }

        private void SearchdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell content click if needed
        }

        private void SearchdataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (SearchdataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = SearchdataGridView.SelectedRows[0];
                partynamegrptxtbox.Text = selectedRow.Cells["CustomerId"].Value?.ToString() ?? "";
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            SaveParty();
        }

        private void SaveParty()
        {
            // Validate required fields
            if (currentDistributorId == 0)
            {
                MessageBox.Show("Please select a distributor first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(partynametxtbox.Text))
            {
                MessageBox.Show("Please enter Party Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if we're updating or inserting
                    if (!string.IsNullOrWhiteSpace(partyidtxtbox.Text) && int.TryParse(partyidtxtbox.Text, out int partyId))
                    {
                        // Update existing party
                        string updateQuery = @"
                            UPDATE Parties 
                            SET PartyName = @PartyName,
                                DistributorId = @DistributorId
                            WHERE PartyID = @PartyID";

                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@PartyName", partynametxtbox.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@DistributorId", currentDistributorId);
                        updateCmd.Parameters.AddWithValue("@PartyID", partyId);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Party updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Refresh the grid
                            SearchDistributor();
                        }
                    }
                    else
                    {
                        // Insert new party - first get the next PartyID
                        string maxIdQuery = "SELECT ISNULL(MAX(PartyID), 0) + 1 AS NextID FROM Parties";
                        SqlCommand maxIdCmd = new SqlCommand(maxIdQuery, conn);
                        int nextPartyId = Convert.ToInt32(maxIdCmd.ExecuteScalar());

                        string insertQuery = @"
                            INSERT INTO Parties 
                            (PartyID, PartyName, DistributorId, IsDistributor)
                            VALUES 
                            (@PartyID, @PartyName, @DistributorId, 0)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@PartyID", nextPartyId);
                        insertCmd.Parameters.AddWithValue("@PartyName", partynametxtbox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@DistributorId", currentDistributorId);

                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Party added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            partyidtxtbox.Text = nextPartyId.ToString();
                            // Refresh the grid
                            SearchDistributor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving party: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            RemovePartyFromDistributor();
        }

        private void RemovePartyFromDistributor()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || !int.TryParse(partyidtxtbox.Text, out int partyId))
            {
                MessageBox.Show("Please select a party to remove", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to remove this party from the distributor?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Parties SET DistributorId = NULL WHERE PartyID = @PartyID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PartyID", partyId);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Party removed from distributor successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Refresh the grid
                            SearchDistributor();
                            ClearPartyFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing party: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addmodbtn_Click(object sender, EventArgs e)
        {
            AddNewParty();
        }

        private void AddNewParty()
        {
            // Clear fields for new entry
            ClearPartyFields();
            partynametxtbox.Focus();

            // Generate suggestion for party name if distributor is selected
            if (!string.IsNullOrWhiteSpace(distributorNametxtbox.Text))
            {
                partynametxtbox.Text = $"New Party under {distributorNametxtbox.Text}";
                partynametxtbox.SelectAll();
            }
        }

        private void distributorIdtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchDistributor();
                e.Handled = true;
            }
        }

        private void switchbtn_Click(object sender, EventArgs e)
        {
            SwitchPartyDistributor();
        }

        private void SwitchPartyDistributor()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text) || !int.TryParse(partyidtxtbox.Text, out int partyId))
            {
                MessageBox.Show("Please select a party first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentDistributorId == 0)
            {
                MessageBox.Show("Please select a distributor first", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ask for new distributor ID using a simple InputBox
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Switch Distributor",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter new Distributor ID:" };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 250 };
            Button confirmation = new Button() { Text = "OK", Left = 200, Width = 70, Top = 80, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                string newDistributorId = textBox.Text;

                if (string.IsNullOrWhiteSpace(newDistributorId) || !int.TryParse(newDistributorId, out int newId))
                {
                    MessageBox.Show("Please enter a valid Distributor ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        // Verify new distributor exists and is a distributor
                        string verifyQuery = "SELECT COUNT(*) FROM Parties WHERE PartyID = @DistributorId AND IsDistributor = 1";
                        SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn);
                        verifyCmd.Parameters.AddWithValue("@DistributorId", newId);

                        conn.Open();

                        int distributorCount = Convert.ToInt32(verifyCmd.ExecuteScalar());

                        if (distributorCount == 0)
                        {
                            MessageBox.Show("The specified ID does not belong to a valid distributor", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Update the party's distributor
                        string updateQuery = "UPDATE Parties SET DistributorId = @NewDistributorId WHERE PartyID = @PartyID";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@NewDistributorId", newId);
                        updateCmd.Parameters.AddWithValue("@PartyID", partyId);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Party distributor switched successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Refresh current distributor's parties
                            SearchDistributor();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error switching distributor: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}