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
    public partial class Define_Company_NSM : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable dataTable;
        private string currentNSMid = "";

        public Define_Company_NSM()
        {
            InitializeComponent();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false; // Optional: only allow one row to be selected

            InitializeDatabaseConnection();
            LoadData();
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
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                string query = "SELECT NSMid, NSMName FROM NSM ORDER BY NSMid";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
                dataGridView1.Columns["NSMid"].HeaderText = "NSM ID";
                dataGridView1.Columns["NSMName"].HeaderText = "NSM Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Mainform1 mf = new Mainform1();
            //mf.Show();
            //this.Hide();
            this.Close();
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
            nsmnametxtbox.Focus();
        }

        

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            string selectedNSMid = row.Cells["NSMid"].Value.ToString();

            if (string.IsNullOrEmpty(nsmnametxtbox.Text))
            {
                MessageBox.Show("Please enter NSM Name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE NSM SET NSMName=@NSMName WHERE NSMid=@NSMid";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NSMName", nsmnametxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@NSMid", selectedNSMid);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dataGridView1.SelectedRows[0];
            string selectedNSMid = row.Cells["NSMid"].Value.ToString();

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM NSM WHERE NSMid=@NSMid";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@NSMid", selectedNSMid);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }




        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nsmnametxtbox.Text))
            {
                MessageBox.Show("Please enter NSM Name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate a new NSMid (you can modify this logic as needed)
            string newNSMid = GenerateNewNSMid();

            try
            {
                string query = "INSERT INTO NSM (NSMid, NSMName) VALUES (@NSMid, @NSMName)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NSMid", newNSMid);
                command.Parameters.AddWithValue("@NSMName", nsmnametxtbox.Text.Trim());

                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateNewNSMid()
        {
            // This is a simple implementation. You can modify this based on your needs.
            try
            {
                string query = "SELECT MAX(NSMid) as MaxId FROM NSM";
                SqlCommand command = new SqlCommand(query, connection);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string maxId = result.ToString();
                    if (int.TryParse(maxId, out int numericId))
                    {
                        return (numericId + 1).ToString("D2"); // Format as 2-digit string
                    }
                }
                return "01"; // Default starting ID
            }
            catch
            {
                return "01";
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            //if (connection != null && connection.State == ConnectionState.Open)
            //{
            //    connection.Close();
            //}

            this.Close();

        }

        private void ClearFields()
        {
            nsmnametxtbox.Text = "";
            currentNSMid = "";
            nsmnametxtbox.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentNSMid = row.Cells["NSMid"].Value.ToString();
                nsmnametxtbox.Text = row.Cells["NSMName"].Value.ToString();
            }
        }

        private void nsmnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add any real-time validation here
        }

        private void Define_Company_NSM_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

       
    }
}