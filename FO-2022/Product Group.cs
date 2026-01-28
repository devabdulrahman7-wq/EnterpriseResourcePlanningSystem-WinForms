using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Product_Group : Form
    {
        //SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=POS_1.2;Integrated Security=True");
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        public Product_Group()
        {
            InitializeComponent();
            LoadCompanies();
            LoadGroups();

            // Ensure DataGridView row click is handled
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        // Load companies into ComboBox
        private void LoadCompanies()
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName", conn);
                SqlDataReader dr = cmd.ExecuteReader();

                cmpnynamecombobox1.Items.Clear();

                while (dr.Read())
                {
                    cmpnynamecombobox1.Items.Add(dr["CompanyName"].ToString());
                }

                dr.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message);
            }
        }

        // Load all product groups into DataGridView
        private void LoadGroups()
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT pg.CompanyId, c.CompanyName, pg.GroupId, pg.GroupName 
                    FROM ProductGroups pg 
                    INNER JOIN Companies c ON pg.CompanyId = c.CompanyID 
                    ORDER BY c.CompanyName, pg.GroupName", conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message);
            }
        }

        // Get company ID from ComboBox name
        private string GetCompanyId(string companyName)
        {
            SqlCommand cmd = new SqlCommand("SELECT CompanyID FROM Companies WHERE CompanyName = @name", conn);
            cmd.Parameters.AddWithValue("@name", companyName);
            return cmd.ExecuteScalar()?.ToString();
        }

        // Get next available Group ID for the company
        private int GetNewGroupId(string companyId)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT ISNULL(MAX(GroupId), 0) + 1 FROM ProductGroups WHERE CompanyId = @compid", conn);
            cmd.Parameters.AddWithValue("@compid", companyId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Clear form fields
        private void ClearForm()
        {
            prdctnametxtbox1.Clear();
            cmpnynamecombobox1.SelectedIndex = -1;
            prdctnametxtbox1.Focus();
        }

        // Save new group
        private void SaveGroup()
        {
            if (string.IsNullOrWhiteSpace(cmpnynamecombobox1.Text) || string.IsNullOrWhiteSpace(prdctnametxtbox1.Text))
            {
                MessageBox.Show("Please select company and enter group name");
                return;
            }

            try
            {
                conn.Open();

                string companyId = GetCompanyId(cmpnynamecombobox1.Text);
                int newGroupId = GetNewGroupId(companyId);

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO ProductGroups (CompanyId, GroupId, GroupName) VALUES (@compid, @grid, @grname)", conn);

                cmd.Parameters.AddWithValue("@compid", companyId);
                cmd.Parameters.AddWithValue("@grid", newGroupId);
                cmd.Parameters.AddWithValue("@grname", prdctnametxtbox1.Text);

                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Group saved successfully!");
                LoadGroups();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving group: " + ex.Message);
            }
        }

        // Edit selected group
        private void EditGroup()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a group to edit");
                return;
            }

            if (string.IsNullOrWhiteSpace(cmpnynamecombobox1.Text) || string.IsNullOrWhiteSpace(prdctnametxtbox1.Text))
            {
                MessageBox.Show("Please select company and enter group name");
                return;
            }

            try
            {
                conn.Open();

                string oldCompId = dataGridView1.CurrentRow.Cells["CompanyId"].Value.ToString();
                int oldGroupId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["GroupId"].Value);

                string newCompId = GetCompanyId(cmpnynamecombobox1.Text);
                string newGroupName = prdctnametxtbox1.Text;

                SqlCommand cmd = new SqlCommand(@"
                    UPDATE ProductGroups
                    SET CompanyId = @newCompId, GroupName = @newGroupName
                    WHERE CompanyId = @oldCompId AND GroupId = @oldGroupId", conn);

                cmd.Parameters.AddWithValue("@newCompId", newCompId);
                cmd.Parameters.AddWithValue("@newGroupName", newGroupName);
                cmd.Parameters.AddWithValue("@oldCompId", oldCompId);
                cmd.Parameters.AddWithValue("@oldGroupId", oldGroupId);

                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Group updated successfully!");
                LoadGroups();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating group: " + ex.Message);
            }
        }

        // Delete selected group
        //private void DeleteGroup()
        //{
        //    if (dataGridView1.CurrentRow == null)
        //    {
        //        MessageBox.Show("Please select a group to delete");
        //        return;
        //    }

        //    try
        //    {
        //        string compId = dataGridView1.CurrentRow.Cells["CompanyId"].Value.ToString();
        //        int groupId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["GroupId"].Value);

        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("DELETE FROM ProductGroups WHERE CompanyId = @compid AND GroupId = @grid", conn);
        //        cmd.Parameters.AddWithValue("@compid", compId);
        //        cmd.Parameters.AddWithValue("@grid", groupId);

        //        cmd.ExecuteNonQuery();
        //        conn.Close();

        //        MessageBox.Show("Group deleted successfully!");
        //        LoadGroups();
        //        ClearForm();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error deleting group: " + ex.Message);
        //    }
        //}
        // Delete selected group
        private void DeleteGroup()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a group to delete");
                return;
            }

            // Confirmation message
            DialogResult dr = MessageBox.Show("Are you sure you want to delete this group?",
                                  "Confirm Delete",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes)
            {
                return;
            }
            try
            {
                string compId = dataGridView1.CurrentRow.Cells["CompanyId"].Value.ToString();
                int groupId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["GroupId"].Value);

                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ProductGroups WHERE CompanyId = @compid AND GroupId = @grid", conn);
                cmd.Parameters.AddWithValue("@compid", compId);
                cmd.Parameters.AddWithValue("@grid", groupId);

                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Group deleted successfully!");
                LoadGroups();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting group: " + ex.Message);
            }
        }


        // When a row in the grid is clicked
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                prdctnametxtbox1.Text = row.Cells["GroupName"].Value.ToString();

                string companyName = row.Cells["CompanyName"].Value.ToString();
                int idx = cmpnynamecombobox1.FindStringExact(companyName);
                cmpnynamecombobox1.SelectedIndex = (idx >= 0) ? idx : -1;
            }
        }

        // Button click events
        private void newbtn_Click(object sender, EventArgs e) => ClearForm();
        private void savebtn_Click(object sender, EventArgs e) => SaveGroup();
        private void editbtn_Click(object sender, EventArgs e) => EditGroup();
        private void removebtn_Click(object sender, EventArgs e) => DeleteGroup();
        private void clearbtn_Click(object sender, EventArgs e) => ClearForm();
        private void closebtn_Click(object sender, EventArgs e) => this.Close();
        private void pictureBox2_Click(object sender, EventArgs e) => this.Close();

        // Optional empty events (keep names same)
        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void prdctnametxtbox1_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void cmpnyidsearch1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(prdctnametxtbox1.Text))
            {
                MessageBox.Show("Please enter group name to search");
                return;
            }

            try
            {
                conn.Open();

                // Fetch CompanyId, CompanyName, GroupId, GroupName
                SqlCommand cmd = new SqlCommand(@"
            SELECT pg.CompanyId, c.CompanyName, pg.GroupId, pg.GroupName
            FROM ProductGroups pg
            INNER JOIN Companies c ON pg.CompanyId = c.CompanyID
            WHERE pg.GroupName LIKE @name
            ORDER BY c.CompanyName, pg.GroupName", conn);

                cmd.Parameters.AddWithValue("@name", "%" + prdctnametxtbox1.Text + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                conn.Close();

                // Optional: clear selection so user can click row
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message);
            }
        }
    }
}
