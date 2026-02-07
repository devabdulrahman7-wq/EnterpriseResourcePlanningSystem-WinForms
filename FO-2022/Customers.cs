using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace FO_2022
{
    public partial class Customers : Form
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;
        string query;

        public Customers()
        {
            InitializeComponent();
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            LoadGrid();
            LoadTownComboBox();
            LoadSectorComboBox();
            LoadCategoryComboBox();
            LoadDistributorComboBox();
            LoadTownComboBox1();
            LoadSectorComboBox1();
            ClearFields();
            nametxtbox.Focus();
        }

        // Load Party Grid
        void LoadGrid()
        {
            try
            {
                con.Open();
                query = "SELECT PartyID, PartyName FROM Parties ORDER BY PartyID";
                da = new SqlDataAdapter(query, con);
                dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading grid: " + ex.Message);
                con.Close();
            }
        }

        // Load Town ComboBox (Search)
        void LoadTownComboBox()
        {
            try
            {
                con.Open();
                query = "SELECT DISTINCT TownName FROM Towns ORDER BY TownName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                townnamecombobox.Items.Clear();
                while (dr.Read())
                {
                    townnamecombobox.Items.Add(dr["TownName"].ToString());
                }
                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns: " + ex.Message);
                con.Close();
            }
        }

        // Load Sector ComboBox (Search)
        void LoadSectorComboBox()
        {
            try
            {
                con.Open();
                query = "SELECT DISTINCT SectorName FROM Sectors ORDER BY SectorName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                sectornamecombobox.Items.Clear();
                while (dr.Read())
                {
                    sectornamecombobox.Items.Add(dr["SectorName"].ToString());
                }
                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sectors: " + ex.Message);
                con.Close();
            }
        }

        // Load Town ComboBox1 (Entry)
        void LoadTownComboBox1()
        {
            try
            {
                con.Open();
                query = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                townnametxtbox1.Items.Clear();
                while (dr.Read())
                {
                    townnametxtbox1.Items.Add(new { ID = dr["TownID"], Name = dr["TownName"].ToString() });
                    townnametxtbox1.DisplayMember = "Name";
                    townnametxtbox1.ValueMember = "ID";
                }
                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns: " + ex.Message);
                con.Close();
            }
        }

       
        void LoadSectorComboBox1()
        {
            try
            {
                con.Open();
                query = "SELECT SectorID, SectorName FROM Sectors ORDER BY SectorName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();

                sectoranamecombobox1.Items.Clear();

                // Create a list to store items
                var sectorList = new List<object>();

                while (dr.Read())
                {
                    sectorList.Add(new
                    {
                        ID = Convert.ToInt32(dr["SectorID"]),
                        Name = dr["SectorName"].ToString()
                    });
                }

                // Set DataSource instead of adding items individually
                sectoranamecombobox1.DataSource = sectorList;
                sectoranamecombobox1.DisplayMember = "Name";
                sectoranamecombobox1.ValueMember = "ID";

                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sectors: " + ex.Message);
                con.Close();
            }
        }
        // Load Category ComboBox
        void LoadCategoryComboBox()
        {
            try
            {
                con.Open();
                query = "SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                cstmrctgrycombobox.Items.Clear();
                while (dr.Read())
                {
                    cstmrctgrycombobox.Items.Add(new { ID = dr["CategoryId"], Name = dr["CategoryName"].ToString() });
                    cstmrctgrycombobox.DisplayMember = "Name";
                    cstmrctgrycombobox.ValueMember = "ID";
                }
                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
                con.Close();
            }
        }

        // Load Distributor ComboBox
        void LoadDistributorComboBox()
        {
            try
            {
                con.Open();
                query = "SELECT PartyID, PartyName FROM Parties WHERE IsDistributor = 1 ORDER BY PartyName";
                cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();
                distributorcombobox.Items.Clear();
                while (dr.Read())
                {
                    distributorcombobox.Items.Add(new { ID = dr["PartyID"], Name = dr["PartyName"].ToString() });
                    distributorcombobox.DisplayMember = "Name";
                    distributorcombobox.ValueMember = "ID";
                }
                dr.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading distributors: " + ex.Message);
                con.Close();
            }
        }

        // Clear all fields
        void ClearFields()
        {
            partyidtxtbox.Text = "";
            partyidtxtbox1.Text = "";
            nametxtbox1.Text = "";
            addresstxtbox.Text = "";
            townnametxtbox1.SelectedIndex = -1;
            sectoranamecombobox1.SelectedIndex = -1;
            citytxtbox.Text = "";
            phonetxtbox.Text = "";
            phone2txtbox.Text = "";
            mblnotxtbox.Text = "";
            faxtxtbox.Text = "";
            emailtxtbox.Text = "";
            conatacttxtbox.Text = "";
            creditlmttxtbox.Text = "0";
            creditdaytxtbox.Text = "0";
            cstmrctgrycombobox.SelectedIndex = -1;
            canalsobeavendorchkbox.Checked = false;
            isdistributorchkbox.Checked = false;
            distributortxtbox.Text = "";
            distributorcombobox.SelectedIndex = -1;
            nametxtbox.Text = "";
            townnamecombobox.SelectedIndex = -1;
            sectornamecombobox.SelectedIndex = -1;
            nametxtbox.Focus();
        }

        // Search by Name
        private void nametxtbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchName = nametxtbox.Text.Trim();
                string townFilter = townnamecombobox.Text;
                string sectorFilter = sectornamecombobox.Text;

                query = "SELECT PartyID, PartyName FROM Parties WHERE 1=1";

                if (!string.IsNullOrEmpty(searchName))
                    query += " AND PartyName LIKE '%" + searchName + "%'";
                if (!string.IsNullOrEmpty(townFilter))
                    query += " AND City LIKE '%" + townFilter + "%'";
                if (!string.IsNullOrEmpty(sectorFilter))
                    query += " AND SectorID IN (SELECT SectorID FROM Sectors WHERE SectorName LIKE '%" + sectorFilter + "%')";

                query += " ORDER BY PartyID";

                con.Open();
                da = new SqlDataAdapter(query, con);
                dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message);
                con.Close();
            }
        }

        // Town ComboBox Search Filter
        private void townnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            nametxtbox_TextChanged(sender, e);
        }

        // Sector ComboBox Search Filter
        private void sectornamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            nametxtbox_TextChanged(sender, e);
        }

        // Grid Row Click
        //private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
        //        string partyID = row.Cells["PartyID"].Value.ToString();

        //        try
        //        {
        //            con.Open();
        //            query = @"SELECT p.*, t.TownName, s.SectorName, c.CategoryName 
        //                      FROM Parties p 
        //                      LEFT JOIN Sectors s ON p.SectorID = s.SectorID 
        //                      LEFT JOIN Towns t ON s.TownID = t.TownID 
        //                      LEFT JOIN Category c ON p.CategoryId = c.CategoryId 
        //                      WHERE p.PartyID = @PartyID";
        //            cmd = new SqlCommand(query, con);
        //            cmd.Parameters.AddWithValue("@PartyID", partyID);
        //            SqlDataReader dr = cmd.ExecuteReader();

        //            if (dr.Read())
        //            {
        //                partyidtxtbox.Text = dr["PartyID"].ToString().Substring(0, Math.Min(2, dr["PartyID"].ToString().Length));
        //                partyidtxtbox1.Text = dr["PartyID"].ToString().Substring(Math.Min(2, dr["PartyID"].ToString().Length));
        //                nametxtbox1.Text = dr["PartyName"].ToString();
        //                addresstxtbox.Text = dr["Address"].ToString();
        //                citytxtbox.Text = dr["City"].ToString();
        //                phonetxtbox.Text = dr["Phone1"].ToString();
        //                phone2txtbox.Text = dr["Phone2"].ToString();
        //                mblnotxtbox.Text = dr["Mobile"].ToString();
        //                faxtxtbox.Text = dr["Fax"].ToString();
        //                emailtxtbox.Text = dr["EMail"].ToString();
        //                conatacttxtbox.Text = dr["ContactPerson"].ToString();
        //                creditlmttxtbox.Text = dr["CreditLimit"].ToString();
        //                creditdaytxtbox.Text = dr["CreditDays"].ToString();
        //                distributortxtbox.Text = dr["DistributorId"] != DBNull.Value ? dr["DistributorId"].ToString() : "";

        //                // Set Town
        //                foreach (var item in townnametxtbox1.Items)
        //                {
        //                    dynamic town = item;
        //                    if (town.Name == dr["TownName"].ToString())
        //                    {
        //                        townnametxtbox1.SelectedItem = item;
        //                        break;
        //                    }
        //                }

        //                // Set Sector
        //                foreach (var item in sectoranamecombobox1.Items)
        //                {
        //                    dynamic sector = item;
        //                    if (sector.Name == dr["SectorName"].ToString())
        //                    {
        //                        sectoranamecombobox1.SelectedItem = item;
        //                        break;
        //                    }
        //                }

        //                // Set Category
        //                foreach (var item in cstmrctgrycombobox.Items)
        //                {
        //                    dynamic cat = item;
        //                    if (cat.Name == dr["CategoryName"].ToString())
        //                    {
        //                        cstmrctgrycombobox.SelectedItem = item;
        //                        break;
        //                    }
        //                }

        //                // Set Distributor
        //                if (!string.IsNullOrEmpty(distributortxtbox.Text))
        //                {
        //                    foreach (var item in distributorcombobox.Items)
        //                    {
        //                        dynamic dist = item;
        //                        if (dist.ID.ToString() == distributortxtbox.Text)
        //                        {
        //                            distributorcombobox.SelectedItem = item;
        //                            break;
        //                        }
        //                    }
        //                }

        //                canalsobeavendorchkbox.Checked = dr["PartyType"].ToString().Contains("C");
        //                isdistributorchkbox.Checked = dr["IsDistributor"] != DBNull.Value ? Convert.ToBoolean(dr["IsDistributor"]) : false;
        //            }
        //            dr.Close();
        //            con.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error loading party details: " + ex.Message);
        //            con.Close();
        //        }
        //    }
        //}

       
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nametxtbox1.Text) || string.IsNullOrEmpty(addresstxtbox.Text))
            {
                MessageBox.Show("Please enter Name and Address.");
                return;
            }

            string partyType = "C"; // Customer
            if (canalsobeavendorchkbox.Checked)
                partyType = "V"; // Vendor

            try
            {
                con.Open();
                string partyID = partyidtxtbox.Text + partyidtxtbox1.Text;

                // Get selected values properly
                object sectorIDValue = null;
                object categoryIdValue = null;
                object distributorIdValue = null;

                // Get SectorID
                if (sectoranamecombobox1.SelectedItem != null)
                {
                    dynamic sector = sectoranamecombobox1.SelectedItem;
                    sectorIDValue = sector.ID;
                }

                // Get CategoryId
                if (cstmrctgrycombobox.SelectedItem != null)
                {
                    dynamic category = cstmrctgrycombobox.SelectedItem;
                    categoryIdValue = category.ID;
                }

                // Get DistributorId
                if (!string.IsNullOrEmpty(distributortxtbox.Text))
                {
                    distributorIdValue = distributortxtbox.Text;
                }

                // Check if new or update
                query = "SELECT COUNT(*) FROM Parties WHERE PartyID = @PartyID";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PartyID", partyID);
                int count = (int)cmd.ExecuteScalar();

                if (count == 0) // New
                {
                    query = @"INSERT INTO Parties (PartyID, PartyName, SectorID, Address, City, Phone1, Phone2, Mobile, Fax, EMail, 
                      ContactPerson, PartyType, CategoryId, CreditLimit, CreditDays, IsRetailPrice, IsBlock, IsDistributor, DistributorId, IsMultiDistributor) 
                      VALUES (@PartyID, @PartyName, @SectorID, @Address, @City, @Phone1, @Phone2, @Mobile, @Fax, @EMail, 
                      @ContactPerson, @PartyType, @CategoryId, @CreditLimit, @CreditDays, @IsRetailPrice, @IsBlock, @IsDistributor, @DistributorId, @IsMultiDistributor)";
                }
                else // Update
                {
                    query = @"UPDATE Parties SET PartyName = @PartyName, SectorID = @SectorID, Address = @Address, City = @City, 
                      Phone1 = @Phone1, Phone2 = @Phone2, Mobile = @Mobile, Fax = @Fax, EMail = @EMail, 
                      ContactPerson = @ContactPerson, PartyType = @PartyType, CategoryId = @CategoryId, 
                      CreditLimit = @CreditLimit, CreditDays = @CreditDays, IsRetailPrice = @IsRetailPrice, 
                      IsBlock = @IsBlock, IsDistributor = @IsDistributor, DistributorId = @DistributorId, 
                      IsMultiDistributor = @IsMultiDistributor WHERE PartyID = @PartyID";
                }

                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PartyID", partyID);
                cmd.Parameters.AddWithValue("@PartyName", nametxtbox1.Text);

                // Handle SectorID parameter properly
                if (sectorIDValue != null)
                    cmd.Parameters.AddWithValue("@SectorID", sectorIDValue);
                else
                    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);

                cmd.Parameters.AddWithValue("@Address", addresstxtbox.Text);
                cmd.Parameters.AddWithValue("@City", citytxtbox.Text);
                cmd.Parameters.AddWithValue("@Phone1", phonetxtbox.Text);
                cmd.Parameters.AddWithValue("@Phone2", phone2txtbox.Text);
                cmd.Parameters.AddWithValue("@Mobile", mblnotxtbox.Text);
                cmd.Parameters.AddWithValue("@Fax", faxtxtbox.Text);
                cmd.Parameters.AddWithValue("@EMail", emailtxtbox.Text);
                cmd.Parameters.AddWithValue("@ContactPerson", conatacttxtbox.Text);
                cmd.Parameters.AddWithValue("@PartyType", partyType);

                // Handle CategoryId parameter
                if (categoryIdValue != null)
                    cmd.Parameters.AddWithValue("@CategoryId", categoryIdValue);
                else
                    cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);

                cmd.Parameters.AddWithValue("@CreditLimit", decimal.Parse(creditlmttxtbox.Text));
                cmd.Parameters.AddWithValue("@CreditDays", int.Parse(creditdaytxtbox.Text));
                cmd.Parameters.AddWithValue("@IsRetailPrice", false);
                cmd.Parameters.AddWithValue("@IsBlock", false);
                cmd.Parameters.AddWithValue("@IsDistributor", isdistributorchkbox.Checked);

                // Handle DistributorId parameter
                if (distributorIdValue != null)
                    cmd.Parameters.AddWithValue("@DistributorId", distributorIdValue);
                else
                    cmd.Parameters.AddWithValue("@DistributorId", DBNull.Value);

                cmd.Parameters.AddWithValue("@IsMultiDistributor", false);

                int rowsAffected = cmd.ExecuteNonQuery();

                // Also insert into ChartOfAccounts if new
                if (count == 0)
                {
                    query = @"INSERT INTO ChartOfAccounts (AccountNo, UserNo, AccountName, AccountType, AccountDepth, 
                      OpeningDebit, OpeningCredit, AdjustedDebit, AdjustedCredit, IsDetailed, IsLocked, 
                      IsPosted, IsEditable, BalFlag, PLFlag, ExpFlag) 
                      VALUES (@AccountNo, 1, @AccountName, 'Party', 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@AccountNo", partyID);
                    cmd.Parameters.AddWithValue("@AccountName", nametxtbox1.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Party saved successfully!");
                con.Close();
                LoadGrid();
                ClearFields();
                LoadDistributorComboBox(); // Refresh distributor list
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving party: " + ex.Message);
                con.Close();
            }
        }

        // New Button
        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
            // Generate new Party ID
            try
            {
                con.Open();
                query = "SELECT MAX(PartyID) FROM Parties WHERE PartyID LIKE 'C%'";
                cmd = new SqlCommand(query, con);
                object result = cmd.ExecuteScalar();
                string maxID = result != DBNull.Value ? result.ToString() : "C00000";
                int num = int.Parse(maxID.Substring(1)) + 1;
                partyidtxtbox.Text = "C";
                partyidtxtbox1.Text = num.ToString("D5");
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating ID: " + ex.Message);
                con.Close();
            }
        }

        // Edit Button
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
            {
                MessageBox.Show("Please select a party to edit.");
                return;
            }
            // Fields are already loaded when clicked on grid
            // User can edit and then Save
        }

        // Remove Button
        // Remove Button - Fixed (Only for 'C' type)
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
            {
                MessageBox.Show("Please select a party to remove.");
                return;
            }

            string partyID = partyidtxtbox.Text + partyidtxtbox1.Text;

            try
            {
                // Check party type first
                con.Open();
                query = "SELECT PartyType FROM Parties WHERE PartyID = @PartyID";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PartyID", partyID);
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result == null)
                {
                    MessageBox.Show("Party not found.");
                    return;
                }

                string partyType = result.ToString();
                if (partyType != "C" && partyType != "O")
                {
                    MessageBox.Show("Only Customer ('C') type parties can be removed.");
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    $"Are you sure you want to DELETE this party?\n\n" +
                    $"Party ID: {partyID}\n" +
                    $"Party Name: {nametxtbox1.Text}\n\n" +
                    $"WARNING: This action cannot be undone!",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    con.Open();

                    // Delete from Parties table
                    query = "DELETE FROM Parties WHERE PartyID = @PartyID";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PartyID", partyID);
                    cmd.ExecuteNonQuery();

                    // Delete from ChartOfAccounts
                    query = "DELETE FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@AccountNo", partyID);
                    cmd.ExecuteNonQuery();

                    con.Close();

                    MessageBox.Show($"Party deleted successfully!\nParty ID: {partyID}",
                        "Deleted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting party: {ex.Message}");
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // Clear Button
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // Close Button
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        // Block Button
        //private void blockbtn_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
        //    {
        //        MessageBox.Show("Please select a party to block.");
        //        return;
        //    }

        //    try
        //    {
        //        con.Open();
        //        query = "UPDATE Parties SET IsBlock = 1 WHERE PartyID = @PartyID";
        //        cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@PartyID", partyidtxtbox.Text + partyidtxtbox1.Text);
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //        MessageBox.Show("Party blocked successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error blocking party: " + ex.Message);
        //        con.Close();
        //    }
        //}

        // Unblock Button
        //private void unblockbtn_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
        //    {
        //        MessageBox.Show("Please select a party to unblock.");
        //        return;
        //    }

        //    try
        //    {
        //        con.Open();
        //        query = "UPDATE Parties SET IsBlock = 0 WHERE PartyID = @PartyID";
        //        cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@PartyID", partyidtxtbox.Text + partyidtxtbox1.Text);
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //        MessageBox.Show("Party unblocked successfully!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error unblocking party: " + ex.Message);
        //        con.Close();
        //    }
        //}

        // Urdu Button (Optional - you can implement Urdu input logic)
        private void urdubtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Urdu input feature can be implemented here.");
        }

        // Distributor ComboBox Selection
        private void distributorcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (distributorcombobox.SelectedItem != null)
            {
                dynamic selected = distributorcombobox.SelectedItem;
                distributortxtbox.Text = selected.ID.ToString();
            }
        }

        // Sector ComboBox1 (Entry) - Optional: Load sectors based on town
        private void sectoranamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // You can add logic to filter sectors by town if needed
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mf = new Mainform1();
            //mf.Show();
        }

        // Block Button - Fixed
        private void blockbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
            {
                MessageBox.Show("Please select a party from grid first.");
                return;
            }

            string partyID = partyidtxtbox.Text + partyidtxtbox1.Text;
            string partyName = nametxtbox1.Text;

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to BLOCK this party?\n\n" +
                $"Party ID: {partyID}\n" +
                $"Party Name: {partyName}",
                "Confirm Block",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    query = "UPDATE Parties SET IsBlock = 1 WHERE PartyID = @PartyID";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PartyID", partyID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Party blocked successfully!\n\nParty ID: {partyID}",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Update UI
                        UpdateBlockStatusUI(true);

                        // Refresh grid
                        LoadGrid();
                    }
                    else
                    {
                        MessageBox.Show("Party not found or already blocked.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error blocking party: {ex.Message}");
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        // Unblock Button - Fixed
        private void unblockbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(partyidtxtbox.Text + partyidtxtbox1.Text))
            {
                MessageBox.Show("Please select a party from grid first.");
                return;
            }

            string partyID = partyidtxtbox.Text + partyidtxtbox1.Text;
            string partyName = nametxtbox1.Text;

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to UNBLOCK this party?\n\n" +
                $"Party ID: {partyID}\n" +
                $"Party Name: {partyName}",
                "Confirm Unblock",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    query = "UPDATE Parties SET IsBlock = 0 WHERE PartyID = @PartyID";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PartyID", partyID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Party unblocked successfully!\n\nParty ID: {partyID}",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Update UI
                        UpdateBlockStatusUI(false);

                        // Refresh grid
                        LoadGrid();
                    }
                    else
                    {
                        MessageBox.Show("Party not found or already active.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error unblocking party: {ex.Message}");
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }
        private void UpdateBlockStatusUI(bool isBlocked)
        {
            if (isBlocked)
            {
                label10.Text = "Party Status: BLOCKED";
                label10.ForeColor = Color.Red;
                blockbtn.Enabled = false;
                unblockbtn.Enabled = true;
            }
            else
            {
                label10.Text = "Party Status: ACTIVE";
                label10.ForeColor = Color.Green;
                blockbtn.Enabled = true;
                unblockbtn.Enabled = false;
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    string partyID = row.Cells["PartyID"].Value?.ToString();

                    if (string.IsNullOrEmpty(partyID))
                        return;

                    con.Open();
                    query = @"SELECT p.*, s.SectorID, s.SectorName, t.TownID, t.TownName, c.CategoryId, c.CategoryName 
                      FROM Parties p 
                      LEFT JOIN Sectors s ON p.SectorID = s.SectorID 
                      LEFT JOIN Towns t ON s.TownID = t.TownID 
                      LEFT JOIN Category c ON p.CategoryId = c.CategoryId 
                      WHERE p.PartyID = @PartyID";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PartyID", partyID);
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // Set Party ID parts
                        partyidtxtbox.Text = partyID.Substring(0, Math.Min(2, partyID.Length));
                        partyidtxtbox1.Text = partyID.Substring(Math.Min(2, partyID.Length));

                        // Set basic info
                        nametxtbox1.Text = dr["PartyName"].ToString();
                        addresstxtbox.Text = dr["Address"].ToString();
                        citytxtbox.Text = dr["City"].ToString();
                        phonetxtbox.Text = dr["Phone1"].ToString();
                        phone2txtbox.Text = dr["Phone2"].ToString();
                        mblnotxtbox.Text = dr["Mobile"].ToString();
                        faxtxtbox.Text = dr["Fax"].ToString();
                        emailtxtbox.Text = dr["EMail"].ToString();
                        conatacttxtbox.Text = dr["ContactPerson"].ToString();
                        creditlmttxtbox.Text = dr["CreditLimit"].ToString();
                        creditdaytxtbox.Text = dr["CreditDays"].ToString();
                        distributortxtbox.Text = dr["DistributorId"] != DBNull.Value ? dr["DistributorId"].ToString() : "";

                        // Check Party Type - Only 'C' type show karo
                        string partyType = dr["PartyType"].ToString();
                        if (partyType != "C" && partyType != "O")
                        {
                            MessageBox.Show("Only Customer ('C') type parties can be edited.");
                            dr.Close();
                            con.Close();
                            ClearFields();
                            return;
                        }

                        // Set Town ComboBox
                        if (dr["TownID"] != DBNull.Value)
                        {
                            int townID = Convert.ToInt32(dr["TownID"]);
                            foreach (var item in townnametxtbox1.Items)
                            {
                                dynamic town = item;
                                if (Convert.ToInt32(town.ID) == townID)
                                {
                                    townnametxtbox1.SelectedItem = item;
                                    break;
                                }
                            }
                        }

                        // Set Sector ComboBox
                        if (dr["SectorID"] != DBNull.Value)
                        {
                            int sectorID = Convert.ToInt32(dr["SectorID"]);
                            foreach (var item in sectoranamecombobox1.Items)
                            {
                                dynamic sector = item;
                                if (Convert.ToInt32(sector.ID) == sectorID)
                                {
                                    sectoranamecombobox1.SelectedItem = item;
                                    break;
                                }
                            }
                        }

                        // Set Category ComboBox
                        if (dr["CategoryId"] != DBNull.Value)
                        {
                            byte categoryID = Convert.ToByte(dr["CategoryId"]);
                            foreach (var item in cstmrctgrycombobox.Items)
                            {
                                dynamic category = item;
                                if (Convert.ToByte(category.ID) == categoryID)
                                {
                                    cstmrctgrycombobox.SelectedItem = item;
                                    break;
                                }
                            }
                        }

                        // Set Distributor ComboBox
                        if (!string.IsNullOrEmpty(distributortxtbox.Text))
                        {
                            foreach (var item in distributorcombobox.Items)
                            {
                                dynamic distributor = item;
                                if (distributor.ID.ToString() == distributortxtbox.Text)
                                {
                                    distributorcombobox.SelectedItem = item;
                                    break;
                                }
                            }
                        }

                        // Set Checkboxes
                        canalsobeavendorchkbox.Checked = partyType == "V";
                        isdistributorchkbox.Checked = dr["IsDistributor"] != DBNull.Value ? Convert.ToBoolean(dr["IsDistributor"]) : false;

                        // Set Block Status
                        bool isBlocked = dr["IsBlock"] != DBNull.Value ? Convert.ToBoolean(dr["IsBlock"]) : false;
                        UpdateBlockStatusUI(isBlocked);
                    }
                    dr.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading party details: " + ex.Message);
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

    }
}