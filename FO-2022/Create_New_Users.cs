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
    public partial class Create_New_Users : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Create_New_Users()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Please select a valid user.");
                return;
            }

            if (dataGridView1.CurrentRow.IsNewRow)
                return;

            int userNo = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UserNo"].Value);

            string query = "DELETE FROM dbo.Users WHERE UserNo=@UserNo";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserNo", userNo);
                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User removed successfully!");
                    LoadUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }



        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            int userNo = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UserNo"].Value);
            string username = usernametxtbox.Text.Trim();
            string password = passwordtxtbox.Text.Trim();
            int userType = standarduserbtn.Checked ? 0 : 1;

            string query = "UPDATE dbo.Users SET UserName=@UserName, Password=@Password, " +
                           "IsAdministrator=@IsAdmin WHERE UserNo=@UserNo";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@IsAdmin", userType);
                    cmd.Parameters.AddWithValue("@UserNo", userNo);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User edited successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            usernametxtbox.Clear();
            passwordtxtbox.Clear();
            retypepasswordtxtbox.Clear();
            standarduserbtn.Checked = true;
        }

        private void LoadUserList()
        {
            dataGridView1.AutoGenerateColumns = true;  // FIX

            string query = "SELECT UserNo, UserName, IsAdministrator FROM dbo.Users ORDER BY UserNo";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void savebtn_Click(object sender, EventArgs e)
        {
            string username = usernametxtbox.Text.Trim();
            string password = passwordtxtbox.Text.Trim();
            string retypePassword = retypepasswordtxtbox.Text.Trim();
            int userType = standarduserbtn.Checked ? 0 : 1;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(retypePassword))
            {
                MessageBox.Show("All fields are required!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != retypePassword)
            {
                MessageBox.Show("Passwords do not match!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO dbo.Users (UserName, Password, IsAdministrator) " +
                           "VALUES (@UserName, @Password, @IsAdmin)";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@IsAdmin", userType);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User created successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearFields();
                    LoadUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Create_New_Users_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            LoadUserList();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // make sure not header
            {
                var row = dataGridView1.Rows[e.RowIndex];
                usernametxtbox.Text = row.Cells["UserName"].Value.ToString();
                //passwordtxtbox.Text = row.Cells["Password"].Value.ToString();
                //retypepasswordtxtbox.Text = row.Cells["Password"].Value.ToString();
                int isAdmin = Convert.ToInt32(row.Cells["IsAdministrator"].Value);
                standarduserbtn.Checked = (isAdmin == 0);
                administratorbtn.Checked = (isAdmin == 1);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

