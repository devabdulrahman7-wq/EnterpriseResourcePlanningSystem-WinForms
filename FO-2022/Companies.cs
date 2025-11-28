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

namespace FO_2022
{
    public partial class Companies : Form
    {
        private string connectionString; // Connection string ko store karein
        private DataTable dataTable;

        public Companies()
        {
            InitializeComponent();
            InitializeConnectionString();
            LoadCompanies();
        }

        private void InitializeConnectionString()
        {
            //string connectionString = "Data Source=.;Initial Catalog=UltraBiz20232;Integrated Security=True";
            connectionString = "Data Source=.;Initial Catalog=POS_1.2;Integrated Security=True; Connect Timeout=3";
        }

        private void LoadCompanies()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyID";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Pehle existing columns clear karein
                        dataGridView1.Columns.Clear();

                        // Auto generate columns disable karein
                        dataGridView1.AutoGenerateColumns = false;

                        // Manual columns add karein
                        if (dataGridView1.Columns.Count == 0)
                        {
                            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                            {
                                Name = "CompanyID",
                                DataPropertyName = "CompanyID",
                                HeaderText = "Company ID",
                                Width = 100
                            });

                            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                            {
                                Name = "CompanyName",
                                DataPropertyName = "CompanyName",
                                HeaderText = "Company Name",
                                Width = 200
                            });
                        }

                        // DataSource set karein
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void cmpnyidtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmpnynametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                cmpnyidtxtbox.Text = row.Cells["CompanyID"].Value.ToString();
                cmpnynametxtbox.Text = row.Cells["CompanyName"].Value.ToString();
            }
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            cmpnyidtxtbox.Clear();
            cmpnynametxtbox.Clear();
            cmpnyidtxtbox.Focus();
        }

        // USING BLOCK PATTERN - Save/Edit method
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmpnyidtxtbox.Text) || string.IsNullOrEmpty(cmpnynametxtbox.Text))
            {
                MessageBox.Show("Please enter Company ID and Name", "Warning");
                return;
            }

            try
            {
                // Using block ka istemal
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if company already exists
                    string checkQuery = "SELECT COUNT(*) FROM Companies WHERE CompanyID = @CompanyID";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query = "";
                        if (count > 0)
                        {
                            // Update existing company
                            query = "UPDATE Companies SET CompanyName = @CompanyName WHERE CompanyID = @CompanyID";
                        }
                        else
                        {
                            // Insert new company
                            query = "INSERT INTO Companies (CompanyID, CompanyName) VALUES (@CompanyID, @CompanyName)";
                        }

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());
                            command.Parameters.AddWithValue("@CompanyName", cmpnynametxtbox.Text.Trim());

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Company saved successfully!", "Success");
                                LoadCompanies();
                                newbtn_Click(sender, e); // Clear form
                            }
                        }
                    }
                } // Yahan connection automatically close ho jayega
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving company: " + ex.Message, "Error");
            }
            // finally block ki ab zaroorat nahi
        }

        private void Companies_Load(object sender, EventArgs e)
        {
            LoadCompanies(); // Same method call karein jo using block use karta hai
        }

        // USING BLOCK PATTERN - Delete method
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmpnyidtxtbox.Text))
            {
                MessageBox.Show("Please select a company to delete", "Warning");
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this company?", "Confirm Delete", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Using block ka istemal
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Companies WHERE CompanyID = @CompanyID";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Company deleted successfully!", "Success");
                                LoadCompanies();
                                newbtn_Click(sender, e); // Clear form
                            }
                        }
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting company: " + ex.Message, "Error");
                }
                
            }
        }

        // USING BLOCK PATTERN - Save method (same as editbtn)
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmpnyidtxtbox.Text) || string.IsNullOrEmpty(cmpnynametxtbox.Text))
            {
                MessageBox.Show("Please enter Company ID and Name", "Warning");
                return;
            }

            try
            {
                // Using block ka istemal
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if company already exists
                    string checkQuery = "SELECT COUNT(*) FROM Companies WHERE CompanyID = @CompanyID";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query = "";
                        if (count > 0)
                        {
                            // Update existing company
                            query = "UPDATE Companies SET CompanyName = @CompanyName WHERE CompanyID = @CompanyID";
                        }
                        else
                        {
                            // Insert new company
                            query = "INSERT INTO Companies (CompanyID, CompanyName) VALUES (@CompanyID, @CompanyName)";
                        }

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@CompanyID", cmpnyidtxtbox.Text.Trim());
                            command.Parameters.AddWithValue("@CompanyName", cmpnynametxtbox.Text.Trim());

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Company saved successfully!", "Success");
                                LoadCompanies();
                                newbtn_Click(sender, e); 
                            }
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving company: " + ex.Message, "Error");
            }
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
               
                if (e.RowIndex >= 0) // ColumnIndex check hata dein
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    cmpnyidtxtbox.Text = row.Cells["CompanyID"].Value?.ToString() ?? "";
                    cmpnynametxtbox.Text = row.Cells["CompanyName"].Value?.ToString() ?? "";

                    // Optional: Row ko visually highlight karein
                    row.Selected = true;
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            cmpnyidtxtbox.Clear();
            cmpnynametxtbox.Clear();
            cmpnyidtxtbox.Focus();
        }

        private void customeridsearch_Click(object sender, EventArgs e)
        {
            SearchCompanyByID();
        }
        private void SearchCompanyByID()
        {
            string companyID = cmpnyidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(companyID))
            {
                MessageBox.Show("Please enter Company ID", "Warning");
                cmpnyidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyName FROM Companies WHERE CompanyID = @CompanyID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyID", companyID);

                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            cmpnynametxtbox.Text = result.ToString();
                            MessageBox.Show("Company found!", "Success");
                        }
                        else
                        {
                            cmpnynametxtbox.Clear();
                            MessageBox.Show("Company not found!", "Info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching company: " + ex.Message, "Error");
            }
        }
    }
}